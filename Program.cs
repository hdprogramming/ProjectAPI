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
var builder = WebApplication.CreateBuilder(args);
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
             policy.WithOrigins("http://localhost:5173") // 👈 BURAYI DÜZELT: React uygulamanın TAM adresi
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

// Authorization servisini ekle
builder.Services.AddAuthorization();
var app = builder.Build();

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
app.UseCors("MyAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


