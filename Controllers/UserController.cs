using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTOs;
using ProjectAPI.Models;
using ProjectAPI.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // BU ÇOK ÖNEMLİ: Bu kontrolcüdeki tüm endpoint'ler artık kimlik doğrulaması gerektirir.
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _hasherUtil;
        
                 public UsersController(AppDbContext context,IPasswordHasher<User> hasherUtil)
        {
            _context = context;
            _hasherUtil = hasherUtil;            
            
        }

        // GET: api/users
        /// <summary>
        /// Tüm kullanıcıları listeler. Sadece "Admin" rolüne sahip kullanıcılar erişebilir.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<IEnumerable<SecuredUserDto>>> GetUsers([FromQuery] bool deleted = false)
        {
            if(deleted==true)
            {
             var users = await _context.Users.Where(u => u.EMail != "admin@projectapi.com")
                .IgnoreQueryFilters().OrderDescending().Where(u => u.IsDeleted == true)
                .Select(u => new SecuredUserDto{  id=u.Id,UserName = u.UserName, Email = u.EMail,ProfileImageUrl=u.ProfileImageUrl, Bio=u.Bio,isDeleted=u.IsDeleted })
                .ToListAsync();
            return Ok(users);   
            }
            else{
            var users = await _context.Users.Where(u => u.EMail != "admin@projectapi.com")
                .OrderDescending()
                .Select(u => new SecuredUserDto{  id=u.Id,UserName = u.UserName, Email = u.EMail,ProfileImageUrl=u.ProfileImageUrl, Bio=u.Bio})
                .ToListAsync();
            return Ok(users);
            }
        }

        // GET: api/users/5
        /// <summary>
        /// Belirli bir kullanıcıyı ID'ye göre getirir.
        /// Kullanıcı ya kendi bilgilerine ya da "Admin" rolündeyse başka bir kullanıcının bilgilerine erişebilir.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTODetails>> GetUser(Guid id)
        {
             if (!IsUserAuthorized(id))
            {
                return Forbid(); // 403 Forbidden - Yetkiniz yok.
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id== id);

            if (user == null)
            {
                return NotFound(); // 404 Not Found - Kullanıcı bulunamadı.
            }

            var userDto = new UserDTODetails {
                UserID = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.EMail,
                ProfileImageUrl = user.ProfileImageUrl,
                Bio=user.Bio
            };
            return Ok(userDto);
        }
        // GET: api/users/5
        /// <summary>
        /// Belirli bir kullanıcıyı ID'ye göre getirir.
        /// Kullanıcı ya kendi bilgilerine ya da "Admin" rolündeyse başka bir kullanıcının bilgilerine erişebilir.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<string>> CreateUser(CreateUserDto createUserDto)
        {
            UsernameChecker UC = new UsernameChecker();
            var user = new User
            {
                UserName = UC.UsernameCheckerGenerator(createUserDto.UserName,createUserDto.Email),
                EMail = createUserDto.Email,
            };
            string hashed = _hasherUtil.HashPassword(user,createUserDto.Password);
            user.PasswordHashed = hashed;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı Oluşturuldu");
        }
        // PATCH: api/users/5
        /// <summary>
        /// Bir kullanıcının bilgilerini günceller. Kullanıcı sadece kendi bilgilerini güncelleyebilir.
        /// </summary>
        [HttpPatch("TargetUserID")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid TargetUserID, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!IsUserAuthorized(TargetUserID))
            {
                return Forbid(); // 403 Forbidden - Yetkiniz yok.
            }
            var userToUpdate = await _context.Users.FindAsync(TargetUserID);

            if (userToUpdate == null)
            {
                return NotFound();
            }
            userToUpdate.MergeNonNullProperties(updateUserDto);
                       
            // DTO'dan gelen verileri denetle ona göre entity'yi güncelle
             if (updateUserDto.Password != null)
            {
                string PasswordHashed = _hasherUtil.HashPassword(userToUpdate, updateUserDto.Password);
                userToUpdate.PasswordHashed = PasswordHashed;
            }
            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - Başarılı, içerik dönmeye gerek yok.
        }

        // DELETE: api/users/5
        /// <summary>
        /// Bir kullanıcıyı siler. Kullanıcı ya kendini silebilir ya da Admin ise başka birini silebilir.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (!IsUserAuthorized(id))
            {
                return Forbid(); // 403 Forbidden - Yetkiniz yok.
            }
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete == null)
            {
                return NotFound();
            }

            // Soft Delete işlemi
    userToDelete.IsDeleted = true;
    userToDelete.DeletedAt = DateTime.UtcNow; // Tarihçeyi tutmak için

    // Sadece durumu güncelliyoruz, kaydı silmiyoruz
    _context.Users.Update(userToDelete); 
    // Bağlı projeleri de soft delete yapıyoruz (Önceki konuşmamızdaki gibi)
        foreach (var project in userToDelete.Projects)
        {
            project.IsDeleted = true;
        }
    await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("Recover/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> RecoverUser(Guid id)
        {
            if (!IsUserAuthorized(id))
            {
                return Forbid(); // 403 Forbidden - Yetkiniz yok.
            }
            var userToDelete = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
            if (userToDelete == null)
            {
                return NotFound();
            }

            // Soft Delete işlemi
    userToDelete.IsDeleted = false;
    userToDelete.DeletedAt = DateTime.UtcNow; // Tarihçeyi tutmak için

    // Sadece durumu güncelliyoruz, kaydı silmiyoruz
    _context.Users.Update(userToDelete); 
    // Bağlı projeleri de soft delete yapıyoruz (Önceki konuşmamızdaki gibi)
        foreach (var project in userToDelete.Projects)
        {
            project.IsDeleted = true;
        }
    await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("Permanent/{id}")]
    public async Task<IActionResult> HardDeleteUser(Guid id)
    {
        // DİKKAT: Burada 'IgnoreQueryFilters()' kullanmak ZORUNDAYIZ.
        // Çünkü kullanıcı daha önce Soft Delete yapılmış olabilir. 
        // Filtreyi kaldırmazsak EF Core onu bulamaz ve "yok" sanır.
        var user = await _context.Users
                                 .IgnoreQueryFilters() 
                                 .Include(u => u.Projects)
                                 .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound("Silinecek kullanıcı bulunamadı.");

        // Hard Delete İşlemi
        // Remove komutu SQL'de 'DELETE FROM Users WHERE...' çalıştırır.
        _context.Users.Remove(user); 
        
        // EF Core, Cascade Delete ayarı açıksa projeleri de otomatik silebilir
        // Ama garanti olsun istersen projeleri de RemoveRange ile silebilirsin.
        
        await _context.SaveChangesAsync();

        return Ok("Kullanıcı veritabanından tamamen silindi (Hard Deleted).");
    }

        // --- Yardımcı Metotlar ---

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString!);
        }
        
        private bool IsUserAuthorized(Guid targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            
            // Eğer işlem yapılan ID, giriş yapan kullanıcıya aitse VEYA kullanıcı Admin rolündeyse yetkilidir.
            return targetUserId == currentUserId || currentUserRole == "Admin";
        }
        
    }
    
}
