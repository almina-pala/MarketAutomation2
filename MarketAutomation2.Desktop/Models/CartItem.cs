using CommunityToolkit.Mvvm.ComponentModel;
using MarketAutomation2.Desktop.Helpers;

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
        private string unit = "Adet";

        [ObservableProperty]
        private decimal unitPrice;

        [ObservableProperty]
        private decimal availableStock;

        [ObservableProperty]
        private decimal quantity = 1;

        public decimal TotalPrice => UnitPrice * Quantity;

        public string QuantityDisplay => QuantityHelper.FormatQuantity(Quantity, Unit);

        public string UnitPriceDisplay => QuantityHelper.FormatUnitPrice(UnitPrice, Unit);

        partial void OnUnitPriceChanged(decimal value)
        {
            NotifyPriceChanged();
        }

        partial void OnQuantityChanged(decimal value)
        {
            NotifyPriceChanged();
        }

        partial void OnUnitChanged(string value)
        {
            OnPropertyChanged(nameof(QuantityDisplay));
            OnPropertyChanged(nameof(UnitPriceDisplay));
        }

        private void NotifyPriceChanged()
        {
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(QuantityDisplay));
            OnPropertyChanged(nameof(UnitPriceDisplay));
        }
    }
}
