using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MarketAutomation2.Desktop.Services
{
    public class ApiHttpClientService
    {
        private readonly HttpClient _httpClient;

        public ApiHttpClientService()
        {
            var settingsService = new AppSettingsService();
            var settings = settingsService.Load();

            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback =
                (message, certificate, chain, errors) =>
                {
                    // API yalnızca localhost üzerinde çalıştığı için
                    // localhost HTTPS sertifikasını kabul et.
                    return message.RequestUri?.Host == "localhost";
                };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(settings.ApiUrl)
            };
        }

        public HttpClient Client => _httpClient;
    }
}