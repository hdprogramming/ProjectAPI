<header>
        <h1>🚀 ProjectAPI</h1>
        <p>Bu proje, temel kurumsal işlemler için geliştirilmiş, <strong>.NET 7.0</strong> altyapısını kullanan güçlü ve ölçeklenebilir bir <strong>RESTful API</strong> servisidir.</p>
        
        <div class="badges">
            <img src="https://img.shields.io/github/license/hdprogramming/ProjectAPI" alt="License" class="badge">
            <img src="https://img.shields.io/github/stars/hdprogramming/ProjectAPI" alt="Stars" class="badge">
            <img src="https://img.shields.io/github/last-commit/hdprogramming/ProjectAPI" alt="Last Commit" class="badge">
        </div>
    </header>

    <hr>

    <section>
        <h2>🌟 Özellikler</h2>
        <ul>
            <li><strong>RESTful Mimari:</strong> CRUD operasyonları için standart HTTP metodlarının kullanımı.</li>
            <li><strong>Hızlı ve Güvenilir:</strong> Performans için optimize edilmiş .NET 7.0 altyapısı.</li>
            <li><strong>OpenAPI Desteği:</strong> Swagger UI aracılığıyla uç nokta testleri ve otomatik dökümantasyon.</li>
            <li><strong>Katmanlı Mimari:</strong> Sürdürülebilirlik ve test edilebilirlik için ayrılmış katmanlar (Repository/Service Pattern).</li>
            <li><strong>Veritabanı Entegrasyonu:</strong> Entity Framework Core / SQL Server Desteği.</li>
        </ul>
    </section>

    <section>
        <h2>🛠️ Kullanılan Teknolojiler</h2>
        <ul>
            <li><strong>Backend Framework:</strong> <a href="https://dotnet.microsoft.com/en-us/download/dotnet/7.0">.NET 7.0 (ASP.NET Core API)</a></li>
            <li><strong>Programlama Dili:</strong> C#</li>
            <li><strong>ORM:</strong> Entity Framework Core</li>
            <li><strong>Dökümantasyon:</strong> Swashbuckle (Swagger UI)</li>
        </ul>
    </section>

    <section>
        <h2>📦 Kurulum ve Çalıştırma</h2>
        <p>Projenin yerel makinenizde geliştirme ortamında çalıştırılması için aşağıdaki adımları izleyin.</p>

        <h3>📝 Ön Koşullar</h3>
        <ul>
            <li>.NET 7.0 SDK veya daha yenisi.</li>
            <li>Tercih edilen bir IDE (Visual Studio, VS Code vb.).</li>
            <li>SQL Server veya uygun bir veritabanı sunucusu.</li>
        </ul>

        <h3>⚙️ Adımlar</h3>
        <ol>
            <li>
                <strong>Projeyi Klonlayın:</strong>
                <pre><code>git clone https://github.com/hdprogramming/ProjectAPI.git
cd ProjectAPI/ProjectAPI</code></pre>
            </li>
            <li>
                <strong>Ortam Ayarlarını Yapılandırın:</strong>
                <p><code>appsettings.Development.json</code> dosyasındaki <code>ConnectionStrings</code> bölümünü kendi yerel veritabanı ayarlarınıza göre güncelleyin.</p>
            </li>
            <li>
                <strong>Veritabanını Hazırlayın:</strong>
                <pre><code>dotnet ef database update</code></pre>
            </li>
            <li>
                <strong>Projeyi Çalıştırın:</strong>
                <pre><code>dotnet run</code></pre>
            </li>
        </ol>
    </section>

    <section>
        <h2>🌐 Erişim</h2>
        <p>Uygulama çalıştıktan sonra aşağıdaki adreslerden erişilebilir:</p>
        <table>
            <thead>
                <tr>
                    <th>Protokol</th>
                    <th>Adres</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>HTTP</td>
                    <td><code>http://localhost:5000</code></td>
                </tr>
                <tr>
                    <td>HTTPS</td>
                    <td><code>https://localhost:5001</code></td>
                </tr>
                <tr>
                    <td><strong>Swagger UI</strong></td>
                    <td><code>https://localhost:5001/swagger</code></td>
                </tr>
            </tbody>
        </table>
    </section>

    <section>
        <h2>🛣️ API Uç Noktaları</h2>
        <p>Detaylı bilgi Swagger üzerinden sunulmaktadır ancak temel uç noktalar şunlardır:</p>
        <table>
            <thead>
                <tr>
                    <th>Metot</th>
                    <th>Uç Nokta</th>
                    <th>Açıklama</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>GET</td>
                    <td><code>/api/v1/products</code></td>
                    <td>Ürün listesini getirir.</td>
                </tr>
                <tr>
                    <td>POST</td>
                    <td><code>/api/v1/auth/register</code></td>
                    <td>Yeni kullanıcı kaydı oluşturur.</td>
                </tr>
            </tbody>
        </table>
    </section>
