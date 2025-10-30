using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models; // Urun sınıfımızın namespace'ini ekliyoruz
using Microsoft.AspNetCore.Identity;
public class AppDbContext : DbContext
{
    // Constructor (Yapıcı Metot): Bağlantı ayarlarını alır
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSet: Veritabanındaki 'Urunler' tablosuna karşılık gelir.
   
     public DbSet<User> Users { get; set; }
     public DbSet<Project> Projects { get; set; }
     public DbSet<UploadFile> UploadFiles{ get; set; }
     public DbSet<RefreshToken> RefreshTokens { get; set; }
     public DbSet<ProjectStatusMsg> projectStatusMsgs { get; set; }
     public DbSet<Category> categories { get; set; }
      public DbSet<ProjectCategory> projectCategories { get; set; }
    // İlişkileri ve kısıtlamaları yapılandırmak istersen bu metodu kullanırsın.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
       .HasIndex(u => u.EMail)
       .IsUnique(); // E-posta alanının benzersiz olmasını zorunlu kılar.
        modelBuilder.Entity<Project>()
            .HasOne(p => p.user)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId);
        modelBuilder.Entity<UploadFile>()
            .HasOne(uf => uf.user) // UploadFiles'ın bir 'user'ı var
            .WithMany(u => u.uploadFiles) // User'ın ise birçok 'uploadFiles'ı var
            .HasForeignKey(uf => uf.UserId) // 'UploadFiles' üzerindeki yabancı anahtar 'UserId'dir
            .OnDelete(DeleteBehavior.Restrict); // ÖNEMLİ: Bir User silinmeye çalışılırsa 
                                                    // ama hala yüklediği dosyaları varsa,
                                                    // veritabanı bu silme işlemini engeller.
                                                    // (Eğer 'Cascade' yapsaydınız, user silinince tüm dosyaları da silinirdi)
        modelBuilder.Entity<Project>()
                .HasMany(p => p.Files) // Project'in birçok 'Files'ı var
                .WithMany(uf => uf.Projects) // UploadFiles'ın da birçok 'Projects'i var
                .UsingEntity(j => j.ToTable("ProjectUploadFiles"));
        modelBuilder.Entity<RefreshToken>()
            // Bir RefreshToken'ın bir tane User'ı vardır (HasOne(rt => rt.user))
            .HasOne(rt => rt.user)
            // Bir User'ın sıfır veya daha fazla RefreshToken'ı olabilir (WithMany())
            // Eğer User modelinde RefreshToken koleksiyonu olsaydı buraya onu yazardık (örnek: WithMany(u => u.RefreshTokens))
            // Şu an User modelinizde RefreshToken koleksiyonu yok, bu yüzden WithMany() yeterli.
            .WithMany()
            // Foreign Key olarak RefreshToken sınıfındaki UserID alanını kullan
            .HasForeignKey(rt => rt.UserID)
            // Foreign Key'in zorunlu olduğunu belirtir
            .IsRequired();
        modelBuilder.Entity<RefreshToken>()
            .HasKey(rt => rt.id);
        // İlişkiler de burada tanımlanır (Kategori-Urun gibi)
        string ADMIN_USER_ID = "B22698B8-42A2-4115-9631-1C2D1E2AC5F7";

        // 2. DI olmadan hasher'ı doğrudan oluşturuyoruz.
        // Bu, sadece seeding işlemi için yapılır ve güvenlidir.
        var hasher = new PasswordHasher<User>();

        // 3. Admin kullanıcısını oluşturuyoruz.
        var adminUser = new User
        {
            Id = Guid.Parse(ADMIN_USER_ID),
            UserName = "admin",
            EMail = "admin@projectapi.com",
            isRole = "Admin"
            // Diğer zorunlu alanlar varsa burada doldurmalısın.
        };

        // 4. Parolayı hash'liyoruz.
        adminUser.PasswordHashed = "AQAAAAIAAYagAAAAEDiRNCYoDBz1VQzr+rtj7cicug4dhAXqqYyxUBgdawRxt8dSOMpIGW+KTVm2m3YsUQ==";

        // 5. HasData metodu ile veritabanı oluşturulurken bu veriyi eklemesini söylüyoruz.
        modelBuilder.Entity<User>().HasData(adminUser);
        
        // Proje ve Statü arasındaki ilişkiyi de burada tanımlayabilirsiniz (Fluent API)
        modelBuilder.Entity<Project>()
    .HasOne(p => p.Status)
    .WithMany()
    .HasForeignKey(p => p.StatusId);                   // Statüye ait birden çok proje olabilir
            // İlişkiyi StatusId üzerinden kur.
        
        modelBuilder.Entity<ProjectCategory>()
        .HasKey(pc => new { pc.ProjectId, pc.CategoryId });

        // İlişkileri kurma
        modelBuilder.Entity<ProjectCategory>()
            .HasOne(pc => pc.Project)
            .WithMany(p => p.ProjectCategories)
            .HasForeignKey(pc => pc.ProjectId);
        
        modelBuilder.Entity<ProjectCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.ProjectCategories)
            .HasForeignKey(pc => pc.CategoryId);
        modelBuilder.Entity<Category>().HasData(
      new Category { Id = 1, Name = "Kimya" },
      new Category { Id = 2, Name = "Fizik" },
      new Category { Id = 3, Name = "Elektronik" },
      new Category { Id = 4, Name = "Yazılım" },
      new Category { Id = 5, Name = "Katıhal Fiziği" },
      new Category { Id = 6, Name = "Elektrokimya" },
      new Category { Id = 7, Name = "Kuantum Fiziği" });
       // İhtiyaca göre ekleyebilirsiniz
        modelBuilder.Entity<ProjectStatusMsg>().HasData(
            new ProjectStatusMsg { Id = 1, Name = "Başlangıç Aşamasında" },
            new ProjectStatusMsg { Id = 2, Name = "Devam Ediyor" },
            new ProjectStatusMsg { Id = 3, Name = "Test Aşamasında" },
            new ProjectStatusMsg { Id = 4, Name = "Tamamlandı" } // İhtiyaca göre ekleyebilirsiniz
        );
       
    }
    
}
