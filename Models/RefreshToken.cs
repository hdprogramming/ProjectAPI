using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace ProjectAPI.Models;
public class RefreshToken
{
    public Guid id  { get; set; }
    public Guid UserID { get; set; }
    
    [Required]
    public string Token { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public User user { get; set; } = null!;
    public bool IsRevoked { get; set; } = false;
}