using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTOs;
using ProjectAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/projectutils/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StatusController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// API'nin ana adresine (/) yapılan GET isteklerini karşılar.
        /// </summary>
        /// <returns>API'nin çalıştığını belirten bir hoş geldin mesajı.</returns>
        [HttpGet]
        [Route("/api/ProjectStatusMessages")]
        public IActionResult GetStatusMessages()
        {
            var StatusMessages = _context.projectStatusMsgs.ToList();
            return Ok(StatusMessages);
        }
        public record CategoriesDTO(int id, string name);
        [HttpGet]
        [Route("/api/ProjectCategories")]
        public async Task<ActionResult<IEnumerable<CategoriesDTO>>> GetCategories()
        {
            var categories = await _context.categories
            .Select(p => new CategoriesDTO(p.Id, p.Name))
            .ToListAsync();
            return Ok(categories);
        }


    }
}