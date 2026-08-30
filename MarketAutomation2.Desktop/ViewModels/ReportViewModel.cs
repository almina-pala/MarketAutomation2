using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using System.Collections.ObjectModel;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        private readonly ReportApiService _reportApiService;

        [ObservableProperty]
        private int totalProducts;

        [ObservableProperty]
        private int criticalStock;

        [ObservableProperty]
        private int todaySalesCount;

        [ObservableProperty]
        private decimal todayTotalAmount;

        [ObservableProperty]
        private decimal cashTotal;

        [ObservableProperty]
        private decimal cardTotal;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private ObservableCollection<BestSellingProduct>
            bestSellingProducts = new();

        [ObservableProperty]
        private ObservableCollection<RecentSale>
            recentSales = new();

        public ReportViewModel()
        {
            _reportApiService = new ReportApiService();

            _ = LoadReportAsync();
        }

        [RelayCommand]
        private async Task LoadReportAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var report =
                    await _reportApiService.GetDashboardAsync();

                if (report == null)
                {
                    ErrorMessage =
                        "Rapor bilgileri alınamadı. API bağlantısını kontrol edin.";

                    return;
                }

                TotalProducts =
                    report.TotalProducts;

                CriticalStock =
                    report.CriticalStock;

                TodaySalesCount =
                    report.TodaySalesCount;

                TodayTotalAmount =
                    report.TodayTotalAmount;

                CashTotal =
                    report.CashTotal;

                CardTotal =
                    report.CardTotal;

                BestSellingProducts =
                    new ObservableCollection<BestSellingProduct>(
                        report.BestSellingProducts);

                RecentSales =
                    new ObservableCollection<RecentSale>(
                        report.RecentSales);
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    $"Rapor yüklenirken hata oluştu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}