using System.ComponentModel.DataAnnotations;

namespace ProjectApi.DTOs
{
    /// <summary>
    /// API'den dışarıya kullanıcı bilgisi dönerken kullanılacak model.
    /// Hassas veriler (örn: PasswordHash) içermez.
    /// </summary>
    public class UserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
    public class CreateUserDto
    {
        public string? UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
    public class UserDTODetails
    {
        public string? UserID{ get; set; }
         public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}