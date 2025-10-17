using Microsoft.AspNetCore.Identity;
using ProjectAPI.Models;

// IPasswordHasher<Kullanici> arayüzünü implemente ederek,
// Identity framework'ünün standart parola hashleme mantığını kullanıyoruz.
// Bu, güvenli,tuzlamalı (salted) ve endüstri standardı bir yaklaşımdır.
namespace ProjectAPI.Utils
{
  public class HasherUtil : IPasswordHasher<User>
{
    // Parolayı hash'leyen metot.
    // Düz metin parolayı alıp, güvenli bir hash stringi döndürür.
    public string HashPassword(User user, string password)
    {
        // PasswordHasher<TUser> sınıfının statik bir örneğini kullanıyoruz.
        // Bu, salt üretme, hashleme ve iterasyon sayısını hash stringine gömme işini halleder.
        return new PasswordHasher<User>().HashPassword(user, password);
    }

    // Düz metin parolayı (providedPassword) veritabanındaki hash ile karşılaştıran metot.
    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        // Hashlenmiş parolayı doğrular. Sonuç olarak başarı (Success), başarısızlık (Failed)
        // veya yeniden hashleme gerekliliği (RehashNeeded) döndürür.
        return new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}
}

