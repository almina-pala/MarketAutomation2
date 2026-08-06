using MarketAutomation2.API.Data;
using Microsoft.EntityFrameworkCore;

using MarketAutomation2.API.Repositories.Abstract;
using MarketAutomation2.API.Repositories.Concrete;
using MarketAutomation2.API.Services.Abstract;
using MarketAutomation2.API.Services.Concrete;

namespace MarketAutomation2.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // SQL Server bağlantısı
            builder.Services.AddDbContext<MarketDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<ISaleRepository, SaleRepository>();

            builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
            builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<ISaleService, SaleService>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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