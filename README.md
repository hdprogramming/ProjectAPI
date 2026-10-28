# 🚀 ProjectAPI

Bu proje, temel kurumsal işlemler için geliştirilmiş, **.NET 8.0** altyapısını kullanan güçlü ve ölçeklenebilir bir **RESTful API** servisidir.

[![GitHub lisansı](https://img.shields.io/github/license/hdprogramming/ProjectAPI)](https://github.com/hdprogramming/ProjectAPI/blob/main/LICENSE)
[![GitHub yıldız sayısı](https://img.shields.io/github/stars/hdprogramming/ProjectAPI)](https://github.com/hdprogramming/ProjectAPI/stargazers)
[![GitHub son commit](https://img.shields.io/github/last-commit/hdprogramming/ProjectAPI)](https://github.com/hdprogramming/ProjectAPI/commits/main)

---

### 🌟 Özellikler

* **RESTful Mimari:** CRUD operasyonları için standart HTTP metodlarının kullanımı.
* **Hızlı ve Güvenilir:** Performans için optimize edilmiş .NET 8.0 altyapısı.
* **OpenAPI Desteği:** Swagger UI aracılığıyla uç nokta testleri ve otomatik dökümantasyon.
* **Katmanlı Mimari:** Sürdürülebilirlik ve test edilebilirlik için ayrılmış katmanlar (**Repository/Service Pattern**).
* **Veritabanı Entegrasyonu:** Entity Framework Core desteği.

---

### 🛠️ Kullanılan Teknolojiler

| Araç | Açıklama |
| :--- | :--- |
| **Framework** | [.NET 8.0 (ASP.NET Core API)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) |
| **Dil** | C# |
| **ORM** | Entity Framework Core |
| **Dökümantasyon** | Swashbuckle (Swagger UI) |

---

### 📦 Kurulum ve Çalıştırma

Projenin yerel makinenizde geliştirme ortamında çalıştırılması için aşağıdaki adımları izleyin.

#### 📝 Ön Koşullar
* **.NET 8.0 SDK** veya daha yenisi.
* Tercih edilen bir IDE (Visual Studio, VS Code vb.).
* SQL Server veya uygun bir veritabanı sunucusu.

#### ⚙️ Adımlar

1.  **Projeyi Klonlayın:**
    ```bash
    git clone [https://github.com/hdprogramming/ProjectAPI.git](https://github.com/hdprogramming/ProjectAPI.git)
    cd ProjectAPI/ProjectAPI
    ```

2.  **Ortam Ayarlarını Yapılandırın:**
    `appsettings.Development.json` dosyasındaki `ConnectionStrings` bölümünü kendi yerel veritabanı ayarlarınıza göre güncelleyin.

3.  **Veritabanını Hazırlayın:**
    ```bash
    dotnet ef database update
    ```

4.  **Projeyi Çalıştırın:**
    ```bash
    dotnet run
    ```

---

### 🌐 Erişim

Uygulama çalıştıktan sonra aşağıdaki adreslerden erişilebilir:

| Servis | Adres |
| :--- | :--- |
| **HTTP** | `http://localhost:5000` |
| **HTTPS** | `https://localhost:5001` |
| **Swagger UI** | `https://localhost:5001/swagger` |

---

### 🛣️ API Uç Noktaları

| Metot | Uç Nokta | Açıklama |
| :---: | :--- | :--- |
| `GET` | `/api/v1/products` | Ürün listesini getirir. |
| `POST` | `/api/v1/auth/register` | Yeni kullanıcı kaydı oluşturur. |
| `GET` | `/api/v1/users/{id}` | Belirli bir kullanıcıyı getirir. |

---

### 🤝 Katkıda Bulunma

1.  Projeyi Fork'layın.
2.  Yeni bir Branch oluşturun (`git checkout -b feature/yeniOzellik`).
3.  Değişikliklerinizi Commit edin (`git commit -m 'Yeni özellik eklendi'`).
4.  Branch'inizi Push edin (`git push origin feature/yeniOzellik`).
5.  Bir Pull Request oluşturun.

---

### 📧 İletişim

**hdprogramming** - [GitHub Profilim](https://github.com/hdprogramming)

---
