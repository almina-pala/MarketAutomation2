using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.DTOs.Categories
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}