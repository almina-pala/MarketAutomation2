using MarketAutomation2.Desktop.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7116/")
            };
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/Category");

            response.EnsureSuccessStatusCode();

            var categories =
                await response.Content.ReadFromJsonAsync<List<Category>>();

            return categories ?? new List<Category>();
        }
    }
}