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
        // 1. Status Bilgisini Yükle
         
        .Include(p => p.ProjectCategories)
        .Include(p=>p.Status).OrderDescending()
            .Select(p => new ProjectDto
    {
        // Temel Alanlar
        id = p.Id,
        title = p.Title,
        icon = p.Icon,
        description = p.Description,
        content = p.Content,
        isAlive = p.isAlive,
        date = p.StartingDate,
        lastdate=p.LastModificationDate,
        // İlişkili Tekil Alan (Status)
        // Varsayım: En son Status (Durum) mesajının içeriğini Status olarak alıyoruz.
        // Mesajları tarihe göre sıralayıp en yenisinin içeriğini almalıyız.
        status = p.Status.Name             // Mesaj içeriğini seç
            ,                   // İlk (en yeni) mesaj içeriğini al (yoksa null)
         statusID=p.Status.Id,
        // İlişkili Liste Alanı (CategoryIds)
        
        categoryIds = p.ProjectCategories
            .Select(pc => pc.Category.Id).ToList()
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
    .Include(p => p.Status)
    .Include(p => p.ProjectCategories)
    .Where(p => p.Id == id)
    .Select(p => new ProjectDto
    {
        // Temel Alanlar
        id = p.Id,
        title = p.Title,
        icon = p.Icon,
        description = p.Description,
        content = p.Content,
        isAlive = p.isAlive,
        date = p.StartingDate,
        lastdate=p.LastModificationDate,
        status = p.Status.Name, 
         statusID=p.Status.Id,
        // İlişkili Liste Alanı (CategoryIds)
        categoryIds = p.ProjectCategories
            .Select(pc => pc.Category.Id)
            .ToList()
    })
    .FirstOrDefaultAsync();
            
            if (p == null)
            {
                return NotFound();
            }
                       // Doğrudan ProjectDto objesini dönüyoruz
            return Ok(p);
        }

        // POST: api/Projects
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectCreateDto>> CreateProject(ProjectCreateDto projectDto)
        {
            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }
            var existingCategories = await _context.categories
        .Where(c => projectDto.categoryIds.Contains(c.Id))
        .ToListAsync();
            if (existingCategories.Count != projectDto.categoryIds.Distinct().Count())
            {
                return BadRequest("Seçilen bazı kategori ID'leri geçersizdir.");
            }
            // DTO'dan Entity'ye dönüştür
            var project = new Project
            {
                Title = projectDto.title!, // Null kontrolünü DTO'da Required ile yapın
                Icon = projectDto.icon,
                Description = projectDto.description,
                Content = projectDto.content,
                isAlive = projectDto.isAlive,
                UserId = currentUserId.Value, // Güvenlik: Token'dan gelen ID kullanılır
                StartingDate = DateOnly.FromDateTime(DateTime.UtcNow),
                StatusId=projectDto.statusID!.Value,
                ProjectCategories = new List<ProjectCategory>()
            };
            foreach (var category in existingCategories)
            {
                project.ProjectCategories.Add(new ProjectCategory
                {
                    // CategoryId'yi elle atamak yerine, Category nesnesinin kendisini atayın.
                    // EF Core, kaydetme anında CategoryId'yi bu nesneden alacaktır.
                    Category = category,

                    // ProjectId'yi atamanıza GEREK YOK.
                    // EF Core, newProject kaydedildikten sonra (ID'si atanınca) bu değeri otomatik doldurur.
                });
            }
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

            var existingProject = await _context.Projects
    .Include(p => p.ProjectCategories) // <-- Çekilmesi gereken navigasyon property'si
    .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProject == null)
            {
                return NotFound();
            }

            // --- YETKİ KONTROLÜ (Mükemmel) ---
            if (!IsAuthorizedToManageProject(existingProject.UserId))
            {
                return Forbid();
            }
            if (projectUpdate.categoryIds != null)
            {
                var existingCategories = await _context.categories
            .Where(c => projectUpdate.categoryIds.Contains(c.Id))
            .ToListAsync();
                if (existingCategories.Count != projectUpdate.categoryIds.Distinct().Count())
                {
                    return BadRequest("Seçilen bazı kategori ID'leri geçersizdir.");
                }
                // 2. DÜZELTME: Eski ilişkileri veritabanından silmek için EF Core'a bildir
        // Bu, veritabanında DELETE sorgusunu tetikleyecektir.
                // KRİTİK DÜZELTME: RemoveRange çağrılmadan önce null kontrolü yapılmalı
    if (existingProject.ProjectCategories != null)
    {
                    // Eski ilişkileri veritabanından silmek için EF Core'a bildir
                    // Bu, veritabanında DELETE sorgusunu tetikleyecektir.
                    _context.projectCategories.RemoveRange(existingProject.ProjectCategories);
       
    }
    
    // Yeni liste oluştur (Veritabanındaki eski kayıtlar silinmek üzere işaretlendi)
    existingProject.ProjectCategories = new List<ProjectCategory>();
    
    // Yeni ilişkileri ekle
    foreach (var categoryId in projectUpdate.categoryIds.Distinct())
    {
        existingProject.ProjectCategories.Add(new ProjectCategory
        {
            ProjectId = existingProject.Id, 
            CategoryId = categoryId       
        });
    }
            }
            // Güncelleme alanlarını eşle

            existingProject.Icon = projectUpdate.icon ?? existingProject.Icon;
            existingProject.Title = projectUpdate.title ?? existingProject.Title;
            existingProject.Description = projectUpdate.description ?? existingProject.Description;
            existingProject.Content = projectUpdate.content ?? existingProject.Content;
            existingProject.isAlive = projectUpdate.isAlive ?? existingProject.isAlive;
            existingProject.StatusId = projectUpdate.statusID ?? existingProject.StatusId;
            existingProject.LastModificationDate = DateTime.UtcNow;
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