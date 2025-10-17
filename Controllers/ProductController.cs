using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models;
using System.Security.Claims;
using ProjectAPI.DTOs;
namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // --- Yardımcı Metotlar ---

        /// <summary>
        /// JWT claim'lerinden giriş yapan kullanıcının GUID ID'sini alır.
        /// </summary>
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

        /// <summary>
        /// Giriş yapan kullanıcının verilen projeyi yönetme yetkisi olup olmadığını kontrol eder.
        /// Ya projenin sahibi olmalı ya da "Admin" rolüne sahip olmalıdır.
        /// </summary>
        private bool IsAuthorizedToManageProject(Guid projectOwnerId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Eğer giriş yapan kullanıcı projenin sahibiyse VEYA Admin rolündeyse yetkilidir.
            return currentUserId.HasValue && (currentUserId.Value == projectOwnerId || currentUserRole == "Admin");
        }
        
        // --------------------------------------------------------------------------------
        
        // GET: api/Projects
        /// <summary>
        /// Tüm projeleri listeler. (Genellikle Auth gereksiz olabilir, ihtiyaca göre eklenir)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .Select(p => new ProjectDto
                {
                    id = p.Id,
                    Name = p.Name,
                    IconName = p.IconName,
                    Description = p.Description,
                    Content = p.Content,
                    isAlive = p.isAlive,
                    Status = p.Status,
                    StartingDate = p.StartingDate,
                })
                .ToListAsync();

            // Doğrudan List<ProjectDto> dönüyoruz
            return Ok(projects);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var p = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null)
            {
                return NotFound();
            }
            
            // DTO'ya dönüştür
            ProjectDto projectDto = new ProjectDto
            {
                id = p.Id,
                Name = p.Name,
                IconName = p.IconName,
                Description = p.Description,
                Content = p.Content,
                isAlive = p.isAlive,
                Status = p.Status,
                StartingDate = p.StartingDate,
            };
            
            // Doğrudan ProjectDto objesini dönüyoruz
            return Ok(projectDto);
        }

        // POST: api/Projects
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectDto>> CreateProject(ProjectDto projectDto)
        {
            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }
            
            // DTO'dan Entity'ye dönüştür
            var project = new Project
            {
                Name = projectDto.Name!, // Null kontrolünü DTO'da Required ile yapın
                IconName = projectDto.IconName,
                Description = projectDto.Description,
                Content = projectDto.Content,
                Status = projectDto.Status,
                isAlive = projectDto.isAlive,
                UserId = currentUserId.Value, // Güvenlik: Token'dan gelen ID kullanılır
                StartingDate = DateTime.UtcNow
            };
            
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            
            // Oluşturulan objenin ID'sini set et (DTO'yu response etmeden önce)
            projectDto.id = project.Id; 

            // Oluşturulan kaynağa yönlendiren 201 Created döner.
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, ProjectUpdateDto projectUpdate)
        {
            if (id != projectUpdate.id)
            {
                return BadRequest("ID uyuşmazlığı. Route'taki ID ile gövdedeki ID aynı olmalıdır.");
            }

            var existingProject = await _context.Projects.FindAsync(id);

            if (existingProject == null)
            {
                return NotFound();
            }

            // --- YETKİ KONTROLÜ (Mükemmel) ---
            if (!IsAuthorizedToManageProject(existingProject.UserId))
            {
                return Forbid(); 
            }

            // Güncelleme alanlarını eşle
            existingProject.IconName =projectUpdate.IconName??existingProject.IconName;
            existingProject.Name = projectUpdate.Name??existingProject.Name;
            existingProject.Description = projectUpdate.Description??existingProject.Description;
            existingProject.Content = projectUpdate.Content??existingProject.Content;
            existingProject.isAlive = projectUpdate.isAlive??existingProject.isAlive;
            existingProject.Status = projectUpdate.Status??existingProject.Status;
            
            // existingProject.UserId ve StartingDate'e dokunmuyoruz.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!_context.Projects.Any(e => e.Id == id))
            {
                return NotFound();
            }

            return NoContent(); 
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // --- YETKİ KONTROLÜ (Mükemmel) ---
            if (!IsAuthorizedToManageProject(project.UserId))
            {
                return Forbid(); 
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}