using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class CashRegisterViewModel : ViewModelBase
    {
        // =====================================================
        // API SERVİSLERİ
        // =====================================================

        private readonly ProductApiService _productApiService;
        private readonly SaleApiService _saleApiService;


        // =====================================================
        // SEPET
        // =====================================================

        public ObservableCollection<CartItem> Cart { get; }
            = new ObservableCollection<CartItem>();


        // =====================================================
        // KOMUTLAR
        // =====================================================

        public IAsyncRelayCommand CompleteSaleCommand { get; }

        public IRelayCommand<CartItem> RemoveItemCommand { get; }

        public IRelayCommand SelectCashCommand { get; }

        public IRelayCommand SelectCardCommand;


        // =====================================================
        // BİNDING ALANLARI
        // =====================================================

        [ObservableProperty]
        private string barcode = string.Empty;

        [ObservableProperty]
        private decimal total;

        [ObservableProperty]
        private string paymentType = "Cash";


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public CashRegisterViewModel()
        {
            _productApiService = new ProductApiService();
            _saleApiService = new SaleApiService();

            CompleteSaleCommand =
                new AsyncRelayCommand(CompleteSale);

            RemoveItemCommand =
                new RelayCommand<CartItem>(RemoveItem);

            SelectCashCommand =
                new RelayCommand(SelectCash);

            SelectCardCommand =
                new RelayCommand(SelectCard);
        }


        // =====================================================
        // NAKİT SEÇ
        // =====================================================

        private void SelectCash()
        {
            PaymentType = "Cash";
        }


        // =====================================================
        // KART SEÇ
        // =====================================================

        private void SelectCard()
        {
            PaymentType = "Card";
        }


        // =====================================================
        // BARKOD ARA
        // =====================================================

        public async Task SearchBarcode()
        {
            if (string.IsNullOrWhiteSpace(Barcode))
                return;

            try
            {
                var product =
                    await _productApiService
                        .GetByBarcodeAsync(Barcode);

                if (product == null)
                {
                    MessageBox.Show(
                        "Bu barkoda ait ürün bulunamadı.",
                        "Ürün Bulunamadı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    Barcode = string.Empty;

                    return;
                }


                // -------------------------------------------------
                // Ürün zaten sepette mi?
                // -------------------------------------------------

                var existing =
                    Cart.FirstOrDefault(
                        x => x.ProductId == product.Id);


                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    Cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Barcode = product.Barcode,
                        Name = product.Name,
                        UnitPrice = product.SalePrice,
                        Quantity = 1
                    });
                }


                // -------------------------------------------------
                // TOPLAM HESAPLA
                // -------------------------------------------------

                Total =
                    Cart.Sum(x => x.TotalPrice);


                // Barkod alanını temizle
                Barcode = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ürün aranırken hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // SATIŞI TAMAMLA
        // =====================================================

        private async Task CompleteSale()
        {
            // -------------------------------------------------
            // Sepet boş mu?
            // -------------------------------------------------

            if (Cart.Count == 0)
            {
                MessageBox.Show(
                    "Sepet boş. Önce ürün ekleyin.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // -------------------------------------------------
            // Satış isteği oluştur
            // -------------------------------------------------

            var request = new CreateSaleRequest
            {
                PaymentType = PaymentType
            };


            // -------------------------------------------------
            // Sepetteki ürünleri ekle
            // -------------------------------------------------

            foreach (var item in Cart)
            {
                request.Items.Add(
                    new CreateSaleItemRequest
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
            }


            try
            {
                // -------------------------------------------------
                // API'YE SATIŞ GÖNDER
                // -------------------------------------------------

                var result =
                    await _saleApiService
                        .CreateSale(request);


                // -------------------------------------------------
                // BAŞARILI
                // -------------------------------------------------

                if (result)
                {
                    MessageBox.Show(
                        $"Satış başarıyla tamamlandı.\n\n" +
                        $"Toplam: {Total:N2} ₺\n" +
                        $"Ödeme: {PaymentType}",
                        "Satış Tamamlandı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);


                    // Sepeti temizle
                    Cart.Clear();

                    // Toplamı sıfırla
                    Total = 0;

                    // Barkodu temizle
                    Barcode = string.Empty;

                    // Ödeme tipini varsayılan yap
                    PaymentType = "Cash";
                }
                else
                {
                    MessageBox.Show(
                        "Satış API tarafından kabul edilmedi.",
                        "Satış Hatası",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Satış sırasında hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // SEPETTEN ÜRÜN SİL
        // =====================================================

        private void RemoveItem(CartItem? item)
        {
            if (item == null)
                return;

            Cart.Remove(item);

            Total =
                Cart.Sum(x => x.TotalPrice);
        }
    }
}