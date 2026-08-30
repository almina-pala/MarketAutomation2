using MarketAutomation2.API.Data;
using Microsoft.EntityFrameworkCore;

using MarketAutomation2.API.Repositories.Abstract;
using MarketAutomation2.API.Repositories.Concrete;
using MarketAutomation2.API.Services.Abstract;
using MarketAutomation2.API.Services.Concrete;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MarketAutomation2.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("https://localhost:7116");

            // ==========================================
            // HTTPS SERTİFİKASI
            // ==========================================

            var certificateFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MarketAutomation2",
                "Certificates");

            Directory.CreateDirectory(certificateFolder);

            var certificatePath = Path.Combine(
                certificateFolder,
                "marketautomation.pfx");

            if (!File.Exists(certificatePath))
            {
                using var rsa = RSA.Create(2048);

                var request = new CertificateRequest(
                    "CN=localhost",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName("localhost");
                sanBuilder.AddIpAddress(System.Net.IPAddress.Loopback);

                request.CertificateExtensions.Add(
                    sanBuilder.Build());

                request.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(
                        certificateAuthority: false,
                        hasPathLengthConstraint: false,
                        pathLengthConstraint: 0,
                        critical: true));

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature |
                        X509KeyUsageFlags.KeyEncipherment,
                        critical: true));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection
                        {
                new Oid("1.3.6.1.5.5.7.3.1")
                        },
                        critical: true));

                using var certificate = request.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddMinutes(-5),
                    DateTimeOffset.UtcNow.AddYears(5));

                File.WriteAllBytes(
                    certificatePath,
                    certificate.Export(X509ContentType.Pfx));
            }

            var httpsCertificate = new X509Certificate2(certificatePath);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(https =>
                {
                    https.ServerCertificate = httpsCertificate;
                });
            });

            // SQLite veritabanı

            var databaseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MarketAutomation2"
            );

            Directory.CreateDirectory(databaseFolder);

            var databasePath = Path.Combine(
                databaseFolder,
                "MarketAutomation.db"
            );

            builder.Services.AddDbContext<MarketDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<ISaleRepository, SaleRepository>();

            builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
            builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<ISaleService, SaleService>();

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            // ==========================================
            // DATABASE MIGRATION
            // ==========================================

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<MarketDbContext>();

                    Console.WriteLine("Veritabanı kontrol ediliyor...");

                    dbContext.Database.EnsureCreated();

                    Console.WriteLine("Veritabanı hazır.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Veritabanı hazırlanırken hata oluştu:");
                    Console.WriteLine(ex.Message);
                }
            }


            // ==========================================
            // HTTP PIPELINE
            // ==========================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}