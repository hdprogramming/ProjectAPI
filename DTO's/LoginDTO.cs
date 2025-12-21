namespace ProjectAPI.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }       
    }
    public class RefreshTokenDto
    {
        public string? Token{ get; set; }
    }
}
