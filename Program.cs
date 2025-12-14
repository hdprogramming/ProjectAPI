using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Security;
using Microsoft.AspNetCore.Authorization; // [Authorize] için
using System.Security.Claims;
using System.Text; // Encoding için
using Microsoft.AspNetCore.Authentication.JwtBearer; // Authentication Middleware
using Microsoft.IdentityModel.Tokens;
using ProjectAPI.Models;
using ProjectAPI.Utils;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
//Admin Email:admin@projectapi.com
//Admin Şifre:Admin123+
var builder = WebApplication.CreateBuilder(args);
// Rate Limiting Servislerini Ekleme
builder.Services.AddRateLimiter(options =>
{
    // *** 1. HATA DURUMUNU BELİRLEME ***
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // *** 2. LİMİTLEME POLİTİKASINI TANIMLAMA VE UYGULAMA ***
    // Burada "PerSecondLimit" adında, IP adresine göre çalışan bir politika tanımlıyoruz.
    options.AddPolicy("PerSecondLimit", httpContext =>
    {
        // İstek Anahtarı: Gelen isteğin hangi kritere göre ayrılacağını belirler.
        // Burada IP adresini kullanıyoruz. (Anonim kullanıcılar için "anonymous" kullanılır)
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        // Fixed Window Limiter'ı, tanımladığımız anahtar üzerinden oluşturur.
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,                 // İzin verilen istek sayısı: 10
                Window = TimeSpan.FromSeconds(1), // Pencere süresi: 1 saniye
                QueueLimit = 0                    // Kuyruk boyutu: 0 (Limit aşılırsa hemen 429 döndürülür)
            }
        );
    });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Bağlantı Dizisi 'DefaultConnection' bulunamadı.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
    
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddSingleton<IPasswordHasher<User>, HasherUtil>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Project API",
        Description = "Kullanıcı ve Proje yönetimi için ASP.NET Core Web API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
         policy =>
         {
             policy.WithOrigins("http://localhost:5173","https://localhost:5173","http://localhost:3000","https://localhost:3000") // 👈 BURAYI DÜZELT: React uygulamanın TAM adresi
                   .AllowAnyHeader()                     // İzin verilen HTTP başlıkları
                   .AllowAnyMethod()                     // İzin verilen HTTP metotları (GET, POST, vb.)
                   .AllowCredentials();                  // 👈 KRİTİK AYAR: Çerezlerin (Cookies) gönderilmesine izin ver!
         });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecurityKey"]!));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = securityKey
    };
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = 
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
// Authorization servisini ekle
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Uploads"
});
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("MyAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("PerSecondLimit");
app.Run();


