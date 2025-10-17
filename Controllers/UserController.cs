using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApi.DTOs;
using ProjectAPI.Models;
using ProjectAPI.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace ProjectApi.Controllers
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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            
            var users = await _context.Users
                .Select(u => new UserDto {  UserName = u.UserName, Email = u.EMail })
                .ToListAsync();
            return Ok(users);
        }

        // GET: api/users/5
        /// <summary>
        /// Belirli bir kullanıcıyı ID'ye göre getirir.
        /// Kullanıcı ya kendi bilgilerine ya da "Admin" rolündeyse başka bir kullanıcının bilgilerine erişebilir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTODetails>> GetUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);

            if (user == null)
            {
                return NotFound(); // 404 Not Found - Kullanıcı bulunamadı.
            }

            var userDto = new UserDTODetails { UserID = user.Id.ToString(), Name = user.UserName, Email = user.EMail };
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

            var user = new User
            {
                UserName = createUserDto.UserName!,
                EMail = createUserDto.Email,
            };
            string hashed = _hasherUtil.HashPassword(user,createUserDto.Password);
            user.PasswordHashed = hashed;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı Oluşturuldu");
        }
        // PUT: api/users/5
        /// <summary>
        /// Bir kullanıcının bilgilerini günceller. Kullanıcı sadece kendi bilgilerini güncelleyebilir.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!IsUserAuthorized(id))
            {
                return Forbid(); // 403 Forbidden - Yetkiniz yok.
            }
            var userToUpdate = await _context.Users.FindAsync(id);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // DTO'dan gelen verilerle entity'yi güncelle
            if(updateUserDto.UserName!=null)
            userToUpdate.UserName = updateUserDto.UserName;
            userToUpdate.ProfileImageUrl = updateUserDto.ProfileImageUrl;
            string PasswordHashed = _hasherUtil.HashPassword(userToUpdate, updateUserDto.Password);
            userToUpdate.PasswordHashed = PasswordHashed;
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

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- Yardımcı Metotlar ---

        private Guid GetCurrentUserId()
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
