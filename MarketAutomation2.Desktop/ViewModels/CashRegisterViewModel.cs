using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Helpers;
using MarketAutomation2.Desktop.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class CashRegisterViewModel : ViewModelBase
    {
        private readonly ProductApiService _productApiService;
        private readonly SaleApiService _saleApiService;
        private readonly DispatcherTimer _clockTimer;

        public ObservableCollection<CartItem> Cart { get; } = new();

        public ObservableCollection<PaymentOption> PaymentOptions { get; } = new();

        public IAsyncRelayCommand CompleteSaleCommand { get; }

        public IRelayCommand<CartItem> RemoveItemCommand { get; }

        public IRelayCommand<CartItem> IncreaseQuantityCommand { get; }

        public IRelayCommand<CartItem> DecreaseQuantityCommand { get; }

        public IRelayCommand<string> SelectPaymentCommand { get; }

        [ObservableProperty]
        private string barcode = string.Empty;

        [ObservableProperty]
        private decimal total;

        [ObservableProperty]
        private string paymentType = "Cash";

        [ObservableProperty]
        private decimal amountReceived;

        [ObservableProperty]
        private string storeName = "Market Automation";

        [ObservableProperty]
        private string registerName = "Kasa 1";

        [ObservableProperty]
        private string cashierName = "Kasiyer";

        [ObservableProperty]
        private string currentDateTime = string.Empty;

        public decimal Change => AmountReceived > Total ? AmountReceived - Total : 0;

        public bool IsCashPayment => PaymentType == "Cash";

        public string PaymentTypeDisplay => PaymentType switch
        {
            "Cash" => "Nakit",
            "Card" => "Kart",
            _ => "Nakit"
        };

        public event EventHandler? RequestBarcodeFocus;

        public CashRegisterViewModel()
        {
            _productApiService = new ProductApiService();
            _saleApiService = new SaleApiService();

            CompleteSaleCommand = new AsyncRelayCommand(CompleteSale);
            RemoveItemCommand = new RelayCommand<CartItem>(RemoveItem);
            IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand<CartItem>(DecreaseQuantity);
            SelectPaymentCommand = new RelayCommand<string>(SelectPayment);

            PaymentOptions.Add(new PaymentOption { PaymentType = "Cash", DisplayName = "NAKİT", IsSelected = true });
            PaymentOptions.Add(new PaymentOption { PaymentType = "Card", DisplayName = "KART", IsSelected = false });

            // Subscribe to cart collection changes to monitor item additions
            Cart.CollectionChanged += (_, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
                {
                    foreach (CartItem item in e.NewItems)
                    {
                        SubscribeToCartItemChanges(item);
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
                {
                    foreach (CartItem item in e.OldItems)
                    {
                        UnsubscribeFromCartItemChanges(item);
                    }
                }
            };

            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += (_, _) => UpdateDateTime();
            _clockTimer.Start();
            UpdateDateTime();
        }

        partial void OnPaymentTypeChanged(string value)
        {
            foreach (var option in PaymentOptions)
            {
                option.IsSelected = option.PaymentType == value;
            }

            OnPropertyChanged(nameof(PaymentTypeDisplay));
            OnPropertyChanged(nameof(IsCashPayment));

            if (value == "Card")
                AmountReceived = 0;

            OnPropertyChanged(nameof(Change));
        }

        partial void OnAmountReceivedChanged(decimal value)
        {
            OnPropertyChanged(nameof(Change));
        }

        partial void OnTotalChanged(decimal value)
        {
            OnPropertyChanged(nameof(Change));
        }

        private void UpdateDateTime()
        {
            CurrentDateTime = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss");
        }

        private void SelectPayment(string? paymentType)
        {
            if (string.IsNullOrWhiteSpace(paymentType))
                return;

            PaymentType = paymentType;
        }

        public async Task SearchBarcode()
        {
            if (string.IsNullOrWhiteSpace(Barcode))
                return;

            var scannedBarcode = Barcode.Trim();

            try
            {
                var product = await _productApiService.GetByBarcodeAsync(scannedBarcode);

                if (product == null)
                {
                    ShowWarning("Bu barkoda ait ürün bulunamadı.", "Ürün Bulunamadı");
                    Barcode = string.Empty;
                    RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (!product.IsActive)
                {
                    ShowWarning("Bu ürün pasif durumda ve satışa kapalı.", "Ürün Pasif");
                    Barcode = string.Empty;
                    RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (product.Stock <= 0)
                {
                    ShowWarning("Yeterli stok bulunmuyor.", "Stok Hatası");
                    Barcode = string.Empty;
                    RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var increment = QuantityHelper.GetScanIncrement(product.Unit);
                var existing = Cart.FirstOrDefault(x => x.ProductId == product.Id);

                if (existing != null)
                {
                    var newQuantity = existing.Quantity + increment;

                    if (newQuantity > product.Stock)
                    {
                        ShowWarning(
                            $"'{product.Name}' için yeterli stok bulunmuyor.\nMevcut stok: {product.Stock:N3} {product.Unit}",
                            "Stok Hatası");
                        Barcode = string.Empty;
                        RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    existing.Quantity = newQuantity;
                    existing.AvailableStock = product.Stock;
                }
                else
                {
                    if (increment > product.Stock)
                    {
                        ShowWarning(
                            $"'{product.Name}' için yeterli stok bulunmuyor.\nMevcut stok: {product.Stock:N3} {product.Unit}",
                            "Stok Hatası");
                        Barcode = string.Empty;
                        RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    Cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Barcode = product.Barcode,
                        Name = product.Name,
                        Unit = string.IsNullOrWhiteSpace(product.Unit) ? "Adet" : product.Unit,
                        UnitPrice = product.SalePrice,
                        AvailableStock = product.Stock,
                        Quantity = increment
                    });
                }

                RecalculateTotal();
                Barcode = string.Empty;
                RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.", "Bağlantı Hatası");
            }
            catch (Exception)
            {
                ShowError("Ürün aranırken bir hata oluştu.", "Hata");
            }
        }

        private void IncreaseQuantity(CartItem? item)
        {
            if (item == null)
                return;

            var step = QuantityHelper.GetStep(item.Unit);
            var newQuantity = item.Quantity + step;

            if (newQuantity > item.AvailableStock)
            {
                ShowWarning("Yeterli stok bulunmuyor.", "Stok Hatası");
                return;
            }

            item.Quantity = newQuantity;
            RecalculateTotal();
        }

        private void DecreaseQuantity(CartItem? item)
        {
            if (item == null)
                return;

            var step = QuantityHelper.GetStep(item.Unit);
            var newQuantity = item.Quantity - step;

            if (newQuantity <= 0)
            {
                Cart.Remove(item);
            }
            else
            {
                item.Quantity = newQuantity;
            }

            RecalculateTotal();
        }

        private async Task CompleteSale()
        {
            if (Cart.Count == 0)
            {
                ShowWarning("Sepet boş. Önce ürün ekleyin.", "Uyarı");
                return;
            }

            if (IsCashPayment && AmountReceived < Total)
            {
                ShowWarning("Alınan tutar, ödenecek tutardan az olamaz.", "Ödeme Hatası");
                return;
            }

            if (!IsCashPayment)
            {
                var confirm = MessageBox.Show(
                    $"Toplam: {Total:N2} ₺\n\nKart ile satışı tamamlamak istiyor musunuz?",
                    "Kart Ödemesi Onayı",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes)
                    return;
            }

            var request = new CreateSaleRequest
            {
                PaymentType = PaymentType
            };

            foreach (var item in Cart)
            {
                request.Items.Add(new CreateSaleItemRequest
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            try
            {
                var result = await _saleApiService.CreateSale(request);

                if (result)
                {
                    var changeText = IsCashPayment && Change > 0
                        ? $"\nPara üstü: {Change:N2} ₺"
                        : string.Empty;

                    MessageBox.Show(
                        $"Satış başarıyla tamamlandı.\n\nToplam: {Total:N2} ₺\nÖdeme: {PaymentTypeDisplay}{changeText}",
                        "Satış Tamamlandı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    ResetAfterSale();
                }
                else
                {
                    ShowError("Satış tamamlanamadı.", "Satış Hatası");
                }
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.", "Bağlantı Hatası");
            }
            catch (Exception)
            {
                ShowError("Satış tamamlanamadı. Stok veya bağlantı sorunu olabilir.", "Satış Hatası");
            }
        }

        private void ResetAfterSale()
        {
            Cart.Clear();
            Total = 0;
            Barcode = string.Empty;
            AmountReceived = 0;
            PaymentType = "Cash";
            RequestBarcodeFocus?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveItem(CartItem? item)
        {
            if (item == null)
                return;

            UnsubscribeFromCartItemChanges(item);
            Cart.Remove(item);
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            Total = Cart.Sum(x => x.TotalPrice);
        }

        /// <summary>
        /// Subscribe to CartItem property changes to validate manual quantity edits
        /// </summary>
        private void SubscribeToCartItemChanges(CartItem item)
        {
            item.PropertyChanged += CartItem_PropertyChanged;
        }

        /// <summary>
        /// Unsubscribe from CartItem property changes
        /// </summary>
        private void UnsubscribeFromCartItemChanges(CartItem item)
        {
            item.PropertyChanged -= CartItem_PropertyChanged;
        }

        /// <summary>
        /// Handle CartItem property changes (specifically Quantity edits from TextBox)
        /// </summary>
        private void CartItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CartItem.Quantity))
                return;

            if (sender is not CartItem item)
                return;

            // Validate the new quantity
            if (item.Quantity <= 0)
            {
                // Remove item if quantity is 0 or negative
                Cart.Remove(item);
                ShowWarning("Miktar 0'dan büyük olmalıdır. Ürün sepetten çıkarıldı.", "Geçersiz Miktar");
                RecalculateTotal();
                return;
            }

            // Check stock availability
            if (item.Quantity > item.AvailableStock)
            {
                // Revert to the previous valid quantity
                var step = QuantityHelper.GetStep(item.Unit);
                var maxAllowedQuantity = Math.Floor((item.AvailableStock / step)) * step;

                item.Quantity = Math.Max(step, maxAllowedQuantity);

                ShowWarning(
                    $"'{item.Name}' için yeterli stok bulunmuyor.\nMevcut stok: {item.AvailableStock:N3} {item.Unit}\nMiktar {item.Quantity:N3} {item.Unit} olarak ayarlandı.",
                    "Yeterli Stok Yok");

                return;
            }

            // Quantity is valid, recalculate totals
            RecalculateTotal();
        }

        private static void ShowWarning(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
