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

// using ProjectApi.Services; // IJwtService'in olduğu namespace

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
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
                return Ok(new { url = fileUrl });
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
                var userWithfiles = await _context.Users.Include(user => user.uploadFiles).FirstOrDefaultAsync(u => u.Id == userGuid);
                if (userWithfiles == null)
                {
                    return NoContent();
                }
                else
                {
                    List<UploadedFilesDTO> founded = new List<UploadedFilesDTO>();
                    var scheme = HttpContext.Request.Scheme;   // "http" veya "https://
                    var host = HttpContext.Request.Host.Value; // "localhost:5001" veya "api.siteniz.com"

                    // Eğer uygulamanız bir alt dizin üzerinden yayın yapıyorsa (örn: /api)
                    var pathBase = HttpContext.Request.PathBase.Value; // "/api" veya ""

                    // Tam adres (PathBase'i de ekleyerek)
                    var baseUrl = $"{scheme}://{host}{pathBase}";
                    foreach (UploadFile u in userWithfiles.uploadFiles)
                    {
                        founded.Add(new UploadedFilesDTO
                        {
                            id=u.Id,
                            name = u.name,
                            url = $"{baseUrl}/Uploads/{u.filename}"
                        });
                    }
                    return Ok(founded);
                }
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }

        }
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var founded = await _context.UploadFiles.FirstOrDefaultAsync(f => f.Id == id);
            if (founded == null)
                return NoContent();
            
            if (founded.filename != null)
                _fileService.DeleteFile(founded.filename);
            _context.UploadFiles.Remove(founded);
            await _context.SaveChangesAsync();
            return Ok("Dosya silindi");
        }
    }
}
