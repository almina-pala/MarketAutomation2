using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService()
        {
            var apiClientService = new ApiHttpClientService();
            _httpClient = apiClientService.Client;
        }

        // Tüm ürünleri getir
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/Product");

            if (!response.IsSuccessStatusCode)
                return new List<Product>();

            var products = await response.Content
                .ReadFromJsonAsync<List<Product>>();

            return products ?? new List<Product>();
        }

        // ID'ye göre ürün getir
        public async Task<Product?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(
                $"api/Product/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<Product>();
        }

        // Barkoda göre ürün getir
        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            
            var response = await _httpClient.GetAsync(
                $"api/Product/barcode/{Uri.EscapeDataString(barcode)}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<Product>();
        }

        // Yeni ürün ekle
        public async Task<bool> CreateProductAsync(
            CreateProductRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Product",
                request);

            return response.IsSuccessStatusCode;
        }

        // Ürün güncelle
        public async Task<bool> UpdateProductAsync(
            int id,
            UpdateProductRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/Product/{id}",
                request);

            return response.IsSuccessStatusCode;
        }

        // Ürün sil
        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/Product/{id}");

            return response.IsSuccessStatusCode;
        }


    }
}