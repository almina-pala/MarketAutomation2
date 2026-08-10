using CommunityToolkit.Mvvm.ComponentModel;

namespace MarketAutomation2.Desktop.Models
{
    public partial class PaymentOption : ObservableObject
    {
        [ObservableProperty]
        private string paymentType = string.Empty;

        [ObservableProperty]
        private string displayName = string.Empty;

        [ObservableProperty]
        private bool isSelected;
    }
}
