## ProjectAPI

> Bu proje, bilimsel çalışmalarınızın kaydı için geliştirilmiş, **.NET 8.0** altyapısını kullanan güçlü ve ölçeklenebilir bir **RESTful API** servisidir.

[](https://www.google.com/search?q=https://github.com/hdprogramming/ProjectAPI/blob/main/LICENSE)
[](https://www.google.com/search?q=https://github.com/hdprogramming/ProjectAPI/stargazers)
[](https://www.google.com/search?q=https://github.com/hdprogramming/ProjectAPI/commits/main)

-----

### 🌟 Özellikler

  * **RESTful Mimari:** CRUD operasyonları için standart HTTP metodlarının kullanımı.
  * **Hızlı ve Güvenilir:** Performans için optimize edilmiş **.NET 8.0** altyapısı.
  * **OpenAPI Desteği:** Swagger UI aracılığıyla uç nokta testleri ve otomatik dökümantasyon.
  * **Katmanlı Mimari:** Sürdürülebilirlik ve test edilebilirlik için ayrılmış katmanlar (Örn: Repository/Service Pattern).
  * **Veritabanı Entegrasyonu:** [Entity Framework Core].

-----

### 🛠️ Kullanılan Teknolojiler

Bu API projesinde aşağıdaki temel teknolojiler ve kütüphaneler kullanılmıştır:

  * **Backend Framework:** [.NET 8.0 (ASP.NET Core API)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
  * **Programlama Dili:** C\#
  * **Veritabanı:** [MSSQL Server]
  * **ORM:** [Entity Framework Core]
  * **Dökümantasyon:** Swashbuckle (Swagger UI)

-----

### 📦 Kurulum ve Çalıştırma

Projenin yerel makinenizde geliştirme ortamında çalıştırılması için aşağıdaki adımları izleyin.

#### 📝 Ön Koşullar

Sisteminizde aşağıdaki yazılımların kurulu olması gerekmektedir:

  * [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) veya daha yenisi.
  * Tercih edilen bir IDE (Örn: Visual Studio, Visual Studio Code, JetBrains Rider).
  * Database için https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads

#### ⚙️ Adımlar

1.  **Projeyi Klonlayın:**
    ```bash
    git clone https://github.com/hdprogramming/ProjectAPI.git
    cd ProjectAPI/ProjectAPI # Ana .csproj dosyasının olduğu dizine girin
    ```
2.  **Ortam Ayarlarını Yapılandırın:**
      * `appsettings.Development.json` dosyasını açın.
      * **Veritabanı Bağlantı Dizesini (`ConnectionStrings`)** kendi yerel veritabanı ayarlarınıza göre güncelleyin.
      * Program.cs içindeki satırı bulup <code>policy.WithOrigins("Buraya FrontEnd sunucunuzun adresi gelecek mesela http://localhost:5173 gibi")</code> güncelleyin
 3.  **Veritabanını Hazırlayın:**
      * Gerekli migrasyonları uygulayın:
        ```bash
        dotnet ef database update
        ```
4.  **Projeyi Çalıştırın:**
      * Projenin ana dizinindeyken (`.csproj` dosyasının olduğu yerde):
        ```bash
        dotnet run
        yada dotnet watch
        ```
      * **Alternatif:** IDE (Visual Studio/VS Code) üzerinden `Run` veya `Debug` tuşuna basarak da çalıştırabilirsiniz.

#### 🌐 Erişim

Uygulama varsayılan olarak şu adreslerde yayında olacaktır:

  * **HTTP:** `http://localhost:5000` (veya `7000` küsurlu bir port)
  * **HTTPS:** `https://localhost:5001` (veya `7000` küsurlu bir port)

Swagger dökümantasyonuna genellikle `/swagger` yolundan erişebilirsiniz: `https://localhost:5001/swagger`

-----

### 🛣️ API Uç Noktaları ve Dökümantasyon

Tüm mevcut uç noktaların detaylı bilgileri ve deneme ekranı, uygulama çalışır durumdayken **Swagger UI** üzerinden otomatik olarak sağlanmaktadır.

Detaylar için:Dökümantasyon.html'ye bakınız

-----




