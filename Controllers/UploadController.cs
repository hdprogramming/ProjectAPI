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
        public UploadsController(AppDbContext context,  IFileService fileService)
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
            name = createdImageName, // Veya orijinal dosya adı, size kalmış
            // Burası en önemli kısım!
            UserId = userGuid 
        };
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
    }
}
