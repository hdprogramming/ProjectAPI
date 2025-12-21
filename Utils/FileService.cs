using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ProjectAPI.Utils;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, bool profile = false);
    bool DeleteFile(string fileNameWithExtension, bool profile = false);
}

public class FileService(IWebHostEnvironment environment) : IFileService
{
    // Dosya yollarını sabit olarak tutmak yazım hatalarını önler
    private const string FolderUploads = "Uploads";
    private const string FolderProfileImages = "ProfileImages";

    public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, bool profile = false)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }

        // 1. Ana klasör yolunu al (Helper metoddan)
        var path = GetBasePath(profile);

        // 2. Klasör yoksa oluştur
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // 3. Uzantı kontrolü (Büyük/Küçük harf duyarsız)
        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        
        // Allowed extensions dizisindeki her şeyi de küçük harfe çevirerek karşılaştırıyoruz
        if (!allowedFileExtensions.Any(x => x.Equals(ext, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        // 4. Benzersiz dosya adı oluştur
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);

        // 5. Dosyayı kaydet
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return fileName;
    }

    public bool DeleteFile(string fileNameWithExtension, bool profile = false)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }

        // Klasör yolunu aynı helper metoddan alıyoruz
        var path = Path.Combine(GetBasePath(profile), fileNameWithExtension);

        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception)
        {
            // Loglama mekanizması eklenebilir
            return false;
        }
    }

    // Kod tekrarını önlemek için path bulma mantığını ayırdık
    private string GetBasePath(bool isProfile)
    {
        // ContentRootPath yerine WebRootPath (wwwroot) kullanılması, 
        // statik dosyaların sunulması için daha uygundur.
        // Eğer wwwroot yoksa null gelebilir, o yüzden ?? ile ContentRootPath'e fallback yapabiliriz.
        var webRoot = environment.WebRootPath ?? environment.ContentRootPath;
        
        var folderName = isProfile ? FolderProfileImages : FolderUploads;
        
        return Path.Combine(webRoot, folderName);
    }
}