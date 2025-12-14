using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // IPasswordHasher için
using ProjectAPI.DTOs;
using ProjectAPI.Models;
using ProjectAPI.Utils;
using System.Net;
using System.Security.Claims; // Bu using satırı önemli!
using System;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.RateLimiting;

// using ProjectApi.Services; // IJwtService'in olduğu namespace

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private Guid? GetCurrentUserId()
        {
            // 'ClaimTypes.NameIdentifier' genellikle User ID'yi (Guid) tutar.
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userIdGuid))
            {
                return userIdGuid;
            }
            return null;
        }
        // Constructor'ı kendi servislerinle güncelle (IJwtService)
        public UploadsController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        // POST: api/Upload/Image
        [HttpPost("Image")]
        [Authorize]
      
        public async Task<IActionResult> UploadImage([FromForm] UploadImageDTO uploadImage)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                // Token geçerli ama içinde ID yoksa (anormal bir durum)
                return Unauthorized("Kullanıcı kimliği token içinde bulunamadı.");
            }

            if (!Guid.TryParse(userIdString, out Guid userGuid))
            {
                // Token'daki ID, Guid formatında değilse (hata)
                return BadRequest("Geçersiz kullanıcı kimliği formatı.");
            }
            try
            {
                 // URL oluşturma kısmı
                var scheme = HttpContext.Request.Scheme;
                var host = HttpContext.Request.Host.Value;
                var pathBase = HttpContext.Request.PathBase.Value;
                var baseUrl = $"{scheme}://{host}{pathBase}";
                if (uploadImage.image?.Length > 3 * 1024 * 1024)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 3 MB");
                }
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                if (uploadImage.image == null)
                    return BadRequest("Image eklenmemiş");
                string createdImageName = await _fileService.SaveFileAsync(uploadImage.image, allowedFileExtentions);
                var fileUrl = $"/Uploads/{createdImageName}";
                var newFileRecord = new UploadFile
                {
                    name = uploadImage.name ?? createdImageName,
                    filename = createdImageName, // Veya orijinal dosya adı, size kalmış
                                                 // Burası en önemli kısım!
                    UserId = userGuid
                };
                //Eğer projede kullanıldıysa o projede kullanılan dosyaların takibi için
                // Eğer bir proje ID'si geldiyse, N-N ilişkiyi kur
                if (uploadImage.ProjectID != null)
                {
                    // 1. Proje ID'sini güvenle parse et
                    if (!int.TryParse(uploadImage.ProjectID.ToString(), out int projectId))
                    {
                        return BadRequest("ProjectID sayı olmalı");
                    }

                    // 2. Projeyi GÜVENLE bul (FirstAsync YERİNE)
                    var project = await _context.Projects
                                                .FirstOrDefaultAsync(p => p.Id == projectId);

                    // 3. Projenin var olup olmadığını KONTROL ET
                    if (project == null)
                    {
                        return BadRequest($"ID'si {projectId} olan proje bulunamadı.");
                    }
                    else
                    {
                        project.Files.Add(newFileRecord);
                    }
                }
                _context.UploadFiles.Add(newFileRecord);
                //
                await _context.SaveChangesAsync();
                return Ok(new { url = $"{baseUrl}/Uploads/{newFileRecord.filename}" });
            }
            catch (System.Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpGet("MyFiles")]
        [Authorize]
        public async Task<IActionResult> MyFiles()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ID kontrol blokları (Aynen koruyoruz)
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            if (!Guid.TryParse(userIdString, out Guid userGuid))
            {
                return BadRequest("Geçersiz kullanıcı kimliği formatı.");
            }

            try
            {
                // URL oluşturma kısmı
                var scheme = HttpContext.Request.Scheme;
                var host = HttpContext.Request.Host.Value;
                var pathBase = HttpContext.Request.PathBase.Value;
                var baseUrl = $"{scheme}://{host}{pathBase}";

                // --- OPTİMİZE EDİLMİŞ KISIM ---

                // 1. Direkt Dosya tablosuna sorgu atıyoruz (_context.UploadFiles)
                // 2. Where ile sadece bu kullanıcıya ait VE silinmemiş (!IsDeleted) olanları seçiyoruz.
                // (Eğer UploadFile için Global Filter eklediysen !IsDeleted kısmı otomatik çalışır ama elle yazmak daha garanti ve okunaklıdır)

                var files = await _context.UploadFiles
                    .Where(u => u.UserId == userGuid && !u.IsDeleted)
                    .OrderByDescending(u => u.Id) // Yeniden eskiye sırala
                    .Select(u => new UploadedFilesDTO
                    {
                        id = u.Id,
                        name = u.name,
                        url = $"{baseUrl}/Uploads/{u.filename}"
                    })
                    .ToListAsync(); // Veritabanından sadece ihtiyacımız olan veriyi çeker

                return Ok(files);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpGet("MyDeletedFiles")]
        [Authorize]
        public async Task<IActionResult> MyDeletedFiles()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userGuid))
            {
                return Unauthorized("Geçersiz kullanıcı kimliği.");
            }

            try
            {
                // URL oluşturma kısmı aynen kalıyor
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                // --- DEĞİŞİKLİK BURADA BAŞLIYOR ---

                // 1. User tablosunu değil, direkt UploadFile tablosunu sorguluyoruz.
                // 2. IgnoreQueryFilters() diyoruz ki silinenleri görebilelim.
                // 3. Where ile hem KULLANICIYI hem de SİLİNME durumunu SQL'de filtreliyoruz.
                var deletedFiles = await _context.UploadFiles // DbContext'inde bu tablonun adı neyse (UploadFiles veya Files)
                    .IgnoreQueryFilters()
                    .Where(f => f.UserId == userGuid && f.IsDeleted == true)
                    .OrderByDescending(f => f.Id) // Sıralamayı da veritabanına yaptırıyoruz
                    .Select(u => new UploadedFilesDTO // Select ile direkt DTO'ya çeviriyoruz (Foreach'e gerek kalmıyor)
                    {
                        id = u.Id,
                        name = u.name, // Modelindeki property ismi küçük harfliyse "name", büyükse "Name"
                        url = $"{baseUrl}/Uploads/{u.filename}"
                    })
                    .ToListAsync();

                return Ok(deletedFiles);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpPut("Update/{id}")]
        [Authorize]      
        public async Task<IActionResult> ModifyFile(int id, UploadModDTO uploadModDTO)
        {
            var foundedfile = await _context.UploadFiles.FirstAsync(up => up.Id == id);
            if (foundedfile == null)
                return NotFound();
            if (uploadModDTO.name != null)
            {
                foundedfile.name = uploadModDTO.name;
                _context.UploadFiles.Update(foundedfile);
                await _context.SaveChangesAsync();
                return Ok(new { name = uploadModDTO.name });
            }
            return Ok("Değişiklik yok");
        }
        [HttpDelete("Delete/{id}")]
        [Authorize]
      
        public async Task<IActionResult> DeleteFile(int id)
        {
            var CurrentUserID = GetCurrentUserId();
            var founded = await _context.UploadFiles.FirstOrDefaultAsync(f => f.Id == id && f.UserId == CurrentUserID);

            if (founded == null)
                return NotFound();
            founded.IsDeleted = true;
            await _context.SaveChangesAsync();


            return Ok("Dosya silindi");
        }
        [HttpPost("Recover/{id}")]
        [Authorize]
      
        public async Task<IActionResult> RecoverFile(int id)
        {
            var CurrentUserID = GetCurrentUserId();
            var founded = await _context.UploadFiles.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == id && f.UserId == CurrentUserID);

            if (founded == null)
                return NotFound();
            founded.IsDeleted = false;
            await _context.SaveChangesAsync();
            return Ok("Dosya kurtarıldı");
        }
        [HttpDelete("Permanent/{id}")]
        [Authorize]
      
        public async Task<IActionResult> HardDeleteFile(int id)
        {
            var CurrentUserID = GetCurrentUserId();
            var founded = await _context.UploadFiles.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == id && f.UserId == CurrentUserID);

            if (founded == null)
                return NotFound();
            if (founded.filename != null)
                _fileService.DeleteFile(founded.filename);
            _context.UploadFiles.Remove(founded);
            await _context.SaveChangesAsync();


            return Ok("Dosya kalıcı olarak silindi");
        }
        [HttpDelete("DeleteFiles")]
        [Authorize]
      
        public async Task<IActionResult> DeleteFiles(int[] ids)
        {
            var CurrentUserID = GetCurrentUserId();
            var founded = await _context.UploadFiles.Where(p => ids.Contains(p.Id) && p.UserId == CurrentUserID).ToListAsync();
            if (founded == null)
                return NotFound();
            foreach (var f in founded)
                f.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok("Dosyalar silindi");
        }
        //İleride admin için bir takım yetkiler gerekebilir
        [HttpDelete("DeleteFilesByAdmin")]
        [Authorize(Roles = "Admin")]
      
        public async Task<IActionResult> DeleteFilesByAdmin(int[] ids)
        {
            var founded = await _context.UploadFiles.Where(p => ids.Contains(p.Id)).ToListAsync();
            if (founded == null)
                return NotFound();
            foreach (var f in founded)
                if (f.filename != null)
                    _fileService.DeleteFile(f.filename);
            _context.UploadFiles.RemoveRange(founded);
            await _context.SaveChangesAsync();
            return Ok("Admin tarafından Dosyalar silindi");
        }
    }
}
