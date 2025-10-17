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
     public DbSet<Project> Projects{ get; set; }
     public DbSet<RefreshToken> RefreshTokens { get; set; }

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
                isRole="Admin"
                // Diğer zorunlu alanlar varsa burada doldurmalısın.
            };

        // 4. Parolayı hash'liyoruz.
        adminUser.PasswordHashed = "AQAAAAIAAYagAAAAEDiRNCYoDBz1VQzr+rtj7cicug4dhAXqqYyxUBgdawRxt8dSOMpIGW+KTVm2m3YsUQ==";
            
            // 5. HasData metodu ile veritabanı oluşturulurken bu veriyi eklemesini söylüyoruz.
            modelBuilder.Entity<User>().HasData(adminUser);
    }
}
