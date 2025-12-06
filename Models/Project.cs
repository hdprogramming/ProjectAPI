
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
    // Bu sınıf, Ürünleri ekleyen kişiyi temsil eder.
    public class Project
    {
        // Primary Key
        public int Id { get; set; }
        public Guid UserId{ get; set; }

        public string Icon { get; set; } = "None";
        [Required]
        public string? Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool isAlive { get; set; } = true;
        public int StatusId { get; set; }  
        public DateOnly StartingDate { get; set; }
        public DateTime LastModificationDate { get; set; } = DateTime.UtcNow;
        public User user { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public ProjectStatusMsg? Status { get; set; }
        public ICollection<ProjectCategory>? ProjectCategories { get; set; }
     public ICollection<UploadFile> Files { get; set; } = new List<UploadFile>();
    }
}