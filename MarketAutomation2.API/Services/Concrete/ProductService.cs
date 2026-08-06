using MarketAutomation2.API.DTOs.Products;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using MarketAutomation2.API.Services.Abstract;

namespace MarketAutomation2.API.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();

            return products.Select(x => new ProductDto
            {
                Id = x.Id,
                Barcode = x.Barcode,
                Name = x.Name,
                Brand = x.Brand,
                PurchasePrice = x.PurchasePrice,
                SalePrice = x.SalePrice,
                Stock = x.Stock,
                CriticalStock = x.CriticalStock,
                Unit = x.Unit,
                IsActive = x.IsActive,
                CategoryName = x.Category.Name
            }).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Barcode = product.Barcode,
                Name = product.Name,
                Brand = product.Brand,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                Stock = product.Stock,
                CriticalStock = product.CriticalStock,
                Unit = product.Unit,
                IsActive = product.IsActive,
                CategoryName = product.Category.Name
            };
        }

        public async Task<ProductDto?> GetByBarcodeAsync(string barcode)
        {
            var product = await _unitOfWork.Products.GetByBarcodeAsync(barcode);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Barcode = product.Barcode,
                Name = product.Name,
                Brand = product.Brand,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                Stock = product.Stock,
                CriticalStock = product.CriticalStock,
                Unit = product.Unit,
                IsActive = product.IsActive,
                CategoryName = product.Category.Name
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _unitOfWork.Products.BarcodeExistsAsync(dto.Barcode))
                throw new Exception("Bu barkoda sahip bir ürün zaten mevcut.");

            var product = new Product
            {
                Barcode = dto.Barcode,
                Name = dto.Name,
                Brand = dto.Brand,
                PurchasePrice = dto.PurchasePrice,
                SalePrice = dto.SalePrice,
                Stock = dto.Stock,
                CriticalStock = dto.CriticalStock,
                Unit = dto.Unit,
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var createdProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id);

            return new ProductDto
            {
                Id = createdProduct!.Id,
                Barcode = createdProduct.Barcode,
                Name = createdProduct.Name,
                Brand = createdProduct.Brand,
                PurchasePrice = createdProduct.PurchasePrice,
                SalePrice = createdProduct.SalePrice,
                Stock = createdProduct.Stock,
                CriticalStock = createdProduct.CriticalStock,
                Unit = createdProduct.Unit,
                IsActive = createdProduct.IsActive,
                CategoryName = createdProduct.Category.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return false;

            product.Barcode = dto.Barcode;
            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.PurchasePrice = dto.PurchasePrice;
            product.SalePrice = dto.SalePrice;
            product.Stock = dto.Stock;
            product.CriticalStock = dto.CriticalStock;
            product.Unit = dto.Unit;
            product.CategoryId = dto.CategoryId;
            product.IsActive = dto.IsActive;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return false;

            // Soft Delete
            product.IsActive = false;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}