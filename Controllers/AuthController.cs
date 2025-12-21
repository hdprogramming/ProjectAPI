using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // IPasswordHasher için
using ProjectAPI.DTOs;
using ProjectAPI.Models;
using ProjectAPI.Utils;
using System.Net;


// using ProjectApi.Services; // IJwtService'in olduğu namespace

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService; // Bu servisi enjekte edeceğiz

        // Constructor'ı kendi servislerinle güncelle (IJwtService)
        public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher, IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }        
        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // Herkesin erişebilmesi için
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            // E-posta adresi zaten kullanımda mı diye kontrol et
            if (await _context.Users.AnyAsync(u => u.EMail == createUserDto.Email))
            {
                return BadRequest("Bu e-posta adresi zaten kullanılıyor.");
            }
            UsernameChecker UC = new UsernameChecker();
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = UC.UsernameCheckerGenerator(createUserDto.UserName,createUserDto.Email),
                EMail = createUserDto.Email,
            };

            // Parolayı hash'le
            user.PasswordHashed = _passwordHasher.HashPassword(user, createUserDto.Password);
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Kullanıcıyı DTO'ya map'leyerek geri dönebiliriz.
            var userDto = new UserDto {  UserName = user.UserName, Email = user.EMail };

            // 201 Created durum kodu ile oluşturulan kaynağın adresini ve kendisini dönmek best practice'dir.
            return CreatedAtAction(nameof(UsersController.GetUser), "Users", new { id = user.Id }, userDto);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EMail == loginDto.Email);

            if (user == null)
            {
                // Güvenlik için "Kullanıcı bulunamadı" demek yerine genel bir hata mesajı verilir.
                return Unauthorized("Geçersiz e-posta veya parola.");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, loginDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Geçersiz e-posta veya parola.");
            }

            var token = await _jwtService.GenerateToken(user);
            var refreshToken = await _jwtService.GenerateAndRefreshToken(user.Id); // Bu Refresh Token

            // 2. Refresh Token'ı GÜVENLİ Cookie olarak ayarla
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // KRİTİK: JavaScript erişimini engeller (XSS koruması)
                Secure = true,   // ÖNERİLİR: Yalnızca HTTPS üzerinden gönderilir (MITM koruması)
                SameSite = SameSiteMode.Strict, // ÖNERİLİR: CSRF riskini azaltır
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Cookie geçerlilik süresi
            };
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
               var request = HttpContext.Request;
        // Şunu oluşturur: https://localhost:7123 veya https://api.site.com
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
       
            return Ok(new { UserID=user.Id,Token = token,Username=user.UserName,ProfileImageUrl=baseUrl+user.ProfileImageUrl,Role=user.isRole });
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var cookies = HttpContext.Request.Cookies;            
            string RefreshToken="";
            if (cookies.TryGetValue("refreshToken", out string refreshTokenValue))
            {
                RefreshToken = refreshTokenValue;
            }
            else
                return BadRequest("Refresh Token göndermelisiniz");

            Guid? UserID= await _jwtService.ValidateRefreshToken(RefreshToken);
            if (UserID == null)
                return BadRequest("Tokenin Süresi Dolmuş");
            
            var NewRefreshToken = await _jwtService.GenerateAndRefreshToken(UserID.Value);
            string NewToken = await _jwtService.GenerateToken(UserID.Value);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // KRİTİK: JavaScript erişimini engeller (XSS koruması)
                Secure = true,   // ÖNERİLİR: Yalnızca HTTPS üzerinden gönderilir (MITM koruması)
                SameSite = SameSiteMode.Strict, // ÖNERİLİR: CSRF riskini azaltır
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Cookie geçerlilik süresi
            };
            HttpContext.Response.Cookies.Append("refreshToken", NewRefreshToken, cookieOptions);
            
            return Ok(new { Token=NewToken});
        } 

    }
}
