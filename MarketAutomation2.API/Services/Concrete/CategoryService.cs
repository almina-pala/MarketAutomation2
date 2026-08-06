using MarketAutomation2.API.DTOs.Categories;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using MarketAutomation2.API.Services.Abstract;

namespace MarketAutomation2.API.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            return categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var existing = await _unitOfWork.Categories.GetByNameAsync(dto.Name);

            if (existing != null)
                throw new Exception("Kategori zaten mevcut.");

            var category = new Category
            {
                Name = dto.Name
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                return false;

            category.Name = dto.Name;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                return false;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}