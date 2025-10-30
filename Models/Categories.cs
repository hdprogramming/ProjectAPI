namespace ProjectAPI.Models;
  public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectCategory>? ProjectCategories { get; set; } // İlişki koleksiyonu
    }
public class ProjectCategory // İlişki tablosu
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } // Navigasyon

    public int CategoryId { get; set; }
    public Category Category { get; set; } // Navigasyon
}
  