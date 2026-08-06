using MarketAutomation2.API.DTOs.Categories;
using MarketAutomation2.API.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MarketAutomation2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Tüm kategorileri getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // Id'ye göre kategori getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound("Kategori bulunamadı.");

            return Ok(category);
        }

        // Yeni kategori ekle
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Kategori güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Kategori bulunamadı.");

            return NoContent();
        }

        // Kategori sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (!result)
                return NotFound("Kategori bulunamadı.");

            return NoContent();
        }
    }
}