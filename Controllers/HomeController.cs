using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTOs;
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
       

    }
}