using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace MarketAutomation2.Desktop.Api
{
    public class ReportApiService
    {
        private readonly HttpClient _httpClient;

        public ReportApiService()
        {
            var apiClientService = new ApiHttpClientService();
            _httpClient = apiClientService.Client;
        }

        public async Task<ReportDashboard?> GetDashboardAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    "api/Reports/dashboard");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(
                        $"Rapor API hatası: {(int)response.StatusCode} - {error}");
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ReportDashboard>();

                if (result == null)
                    throw new Exception("API boş rapor döndürdü.");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Rapor verileri alınamadı: {ex.Message}", ex);
            }
        }
    }
}