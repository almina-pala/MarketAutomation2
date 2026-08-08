using CommunityToolkit.Mvvm.ComponentModel;

namespace MarketAutomation2.Desktop.Models
{
    public partial class CartItem : ObservableObject
    {
        [ObservableProperty]
        private int productId;

        [ObservableProperty]
        private string barcode = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private decimal unitPrice;

        partial void OnUnitPriceChanged(decimal value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        [ObservableProperty]
        private decimal quantity = 1;

        partial void OnQuantityChanged(decimal value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}