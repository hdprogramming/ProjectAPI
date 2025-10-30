using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey ve SigningCredentials için
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler için
using System.Security.Claims; // Claim, ClaimTypes için
using System.Text;
using ProjectAPI.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore; // User modelini tanımak için

namespace ProjectAPI.Utils
{
    public interface IJwtService
    {
        // Kullanici yerine User kullanıldı
        Task<string> GenerateToken(Guid UserID);
        Task<string> GenerateToken(User user);
        Task<string> GenerateAndRefreshToken(Guid userid);
        Task<Guid?> ValidateRefreshToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;
        private readonly string _securityKey;


        public JwtService(IConfiguration config, AppDbContext appdbcontext)
        {
            _config = config;
            _db = appdbcontext;
            // appsettings.json dosyasından Gizli Anahtarı okur
            _securityKey = _config["Jwt:SecurityKey"]
                         ?? throw new InvalidOperationException("Jwt:SecurityKey ayarı bulunamadı.");
        }
        private string CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // JWT içine ekleyeceğimiz iddialar (claims)
            var claims = new[]
            {
            // Kullanıcının benzersiz ID'si
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // Kullanıcının adı
            new Claim(ClaimTypes.Name, user.UserName!),
            // Zorunlu: E-posta
            new Claim(ClaimTypes.Email, user.EMail),
            new Claim(ClaimTypes.Role,user.isRole)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token 1 saat geçerli
                signingCredentials: credentials);

            // Token'ı string olarak döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateToken(Guid UserID)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == UserID);
            if (user == null)
                throw new UnauthorizedAccessException();

            return CreateToken(user);

        }
        public async Task<string> GenerateToken(User user)
        {
            return CreateToken(user);
        }
        public async Task<string> GenerateAndRefreshToken(Guid userid)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
            string RefreshToken = String.Concat(randomBytes.Select(b => b.ToString("x2")));
            var RefReshTokenObj = new RefreshToken
            {
                id = Guid.NewGuid(),
                UserID = userid,
                Token = RefreshToken,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            };
            var excitingToken = _db.RefreshTokens.FirstOrDefault(rt => rt.UserID == userid);
            if (excitingToken != null)
            {
                excitingToken.IsRevoked = true;
                _db.RefreshTokens.Update(excitingToken);                
            }
            _db.RefreshTokens.Add(RefReshTokenObj);
            await _db.SaveChangesAsync();
            return RefreshToken;
        }
        public async Task<Guid?> ValidateRefreshToken(string token)
        {
            var existingToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

            // 1. Durum: Token veritabanında yoksa veya iptal edilmişse (IsRevoked)
            if (existingToken == null)
            {
                return null;
            }
            if (existingToken.ExpiresOnUtc < DateTime.UtcNow || existingToken.IsRevoked)
            {
                return null;
            }
            return existingToken.UserID;
        }
    }
}