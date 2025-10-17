using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTOs;
using ProjectAPI.Models;
using Microsoft.AspNetCore.Identity;
namespace ProjectAPI.Controllers
{
    [ApiController]
    
    public class HomeController : ControllerBase
    {
        public record SecretKeyDTO(string key);
        /// <summary>
        /// API'nin ana adresine (/) yapılan GET isteklerini karşılar.
        /// </summary>
        /// <returns>API'nin çalıştığını belirten bir hoş geldin mesajı.</returns>
        [Route("/")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Project API'ye hoşgeldin bro! Her şey yolunda görünüyor.");
        }
        [Route("/hasher")]
        [HttpPost]
        public IActionResult Hash(SecretKeyDTO secretKeyDTO)
        {
            if (secretKeyDTO.key != "1234")
                return BadRequest();
            string ADMIN_USER_ID = "B22698B8-42A2-4115-9631-1C2D1E2AC5F7";
            
            // 2. DI olmadan hasher'ı doğrudan oluşturuyoruz.
            // Bu, sadece seeding işlemi için yapılır ve güvenlidir.
            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = Guid.Parse(ADMIN_USER_ID),
                UserName = "admin",
                EMail = "admin@projectapi.com",
                isRole = "Admin"
                // Diğer zorunlu alanlar varsa burada doldurmalısın.
            };
            
            return Ok(new{ Hash=hasher.HashPassword(adminUser, "Admin123+")});
        }

    }
}