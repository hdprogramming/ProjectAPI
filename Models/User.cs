
namespace ProjectAPI.Models
{
    // Bu sınıf, Ürünleri ekleyen kişiyi temsil eder.
    public class User
    {
        // Primary Key
        public Guid Id { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string? EMail{ get; set; }

        // Kullanıcı Adı (Örn: "admin", "tester")
        public string UserName { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string isRole { get; set; } = "User";
        public ICollection<Project> Projects=new List<Project>();
        // Navigasyon Özelliği (Navigation Property)
        // Bu kullanıcı tarafından eklenen tüm projeleri tutar. 
        // EF Core, bu koleksiyonu otomatik olarak doldurabilir.
        
    }
}