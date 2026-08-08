using MarketAutomation2.Desktop.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class SaleApiService
    {
        private readonly HttpClient _httpClient;

        public SaleApiService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress =
                new Uri("https://localhost:7116/");
        }

        public async Task<bool> CreateSale(CreateSaleRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Sale",
                request);

            return response.IsSuccessStatusCode;
        }
    }
}