
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
    // Bu sınıf, Ürünleri ekleyen kişiyi temsil eder.
    public class Project
    {
        // Primary Key
        public int Id { get; set; }
        public Guid UserId{ get; set; }
        [Required]
        public string IconName { get; set; } = "None";
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool isAlive { get; set; } = true;
        public string Status { get; set; } = string.Empty;
        public DateTime StartingDate { get; set; }
        public User user { get; set; } = null!;
    }
}