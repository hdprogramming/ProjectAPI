namespace ProjectAPI.Models;

public class UploadFile
{
    public int Id { get; set; }
    public string? name { get; set; }
    public string? filename { get; set; }
    public bool IsDeleted { get; set; } = false;
    // 1. Foreign Key (Yabancı Anahtar)
    // User modelinizin Primary Key'i (Id) Guid tipinde olduğu için,
    // buradaki Foreign Key de Guid tipinde olmalıdır.
    public Guid UserId { get; set; }
    public User user { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}