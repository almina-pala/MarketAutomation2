using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Services;
using System.Net;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly AppSettingsService _settingsService;

        [ObservableProperty]
        private string marketName = "";

        [ObservableProperty]
        private string phone = "";

        [ObservableProperty]
        private string address = "";

        [ObservableProperty]
        private string cashierName = "";

        [ObservableProperty]
        private string apiUrl = "";

        [ObservableProperty]
        private string statusMessage = "";

        public SettingsViewModel()
        {
            _settingsService = new AppSettingsService();

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.Load();

            MarketName = settings.MarketName;
            Phone = settings.Phone;
            Address = settings.Address;
            CashierName = settings.CashierName;
            ApiUrl = settings.ApiUrl;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MarketName))
                {
                    MessageBox.Show(
                        "Market adı boş bırakılamaz.",
                        "Uyarı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (string.IsNullOrWhiteSpace(ApiUrl))
                {
                    MessageBox.Show(
                        "API adresi boş bırakılamaz.",
                        "Uyarı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var settings = new AppSettings
                {
                    MarketName = MarketName,
                    Phone = Phone,
                    Address = Address,
                    CashierName = CashierName,
                    ApiUrl = ApiUrl
                };

                _settingsService.Save(settings);

                StatusMessage = "✓ Ayarlar kaydedildi.";

                MessageBox.Show(
                    "Ayarlar başarıyla kaydedildi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ayarlar kaydedilirken hata oluştu:\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ResetSettings()
        {
            var result = MessageBox.Show(
                "Ayarları varsayılan değerlere döndürmek istediğinize emin misiniz?",
                "Ayarları Sıfırla",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _settingsService.Reset();

            LoadSettings();

            StatusMessage = "✓ Varsayılan ayarlar yüklendi.";
        }
    }
}