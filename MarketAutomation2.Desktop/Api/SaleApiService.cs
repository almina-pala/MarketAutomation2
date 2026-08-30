using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class SaleApiService
    {
        private readonly HttpClient _httpClient;

        public SaleApiService()
        {
            var apiClientService = new ApiHttpClientService();
            _httpClient = apiClientService.Client;
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