using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService()
        {
            var apiClientService = new ApiHttpClientService();
            _httpClient = apiClientService.Client;
        }

        // Kategorileri getir
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/Category");

            response.EnsureSuccessStatusCode();

            var categories =
                await response.Content.ReadFromJsonAsync<List<Category>>();

            return categories ?? new List<Category>();
        }

        // Yeni kategori ekle
        public async Task<Category?> CreateCategoryAsync(string name)
        {
            var data = new
            {
                Name = name
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Category",
                data);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Category>();
        }

        // Kategori güncelle
        public async Task<bool> UpdateCategoryAsync(int id, string name)
        {
            var data = new
            {
                Name = name
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"api/Category/{id}",
                data);

            return response.IsSuccessStatusCode;
        }

        // Kategori sil
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/Category/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}