using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using System.Collections;
using System.Windows;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class ProductEditViewModel : ObservableObject
    {
        private readonly CategoryApiService _categoryApiService;

        public ObservableCollection<Category> Categories { get; }
            = new ObservableCollection<Category>();

        private readonly ProductApiService _productApiService;

        // Düzenlenen ürünün ID'si
        private readonly int? _productId;

        // Pencerenin başlığı
        [ObservableProperty]
        private string windowTitle = "Yeni Ürün";

        // Form alanları
        [ObservableProperty]
        private string barcode = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string brand = string.Empty;

        [ObservableProperty]
        private decimal purchasePrice;

        [ObservableProperty]
        private decimal salePrice;

        [ObservableProperty]
        private decimal stock;

        [ObservableProperty]
        private decimal criticalStock;

        [ObservableProperty]
        private string unit = "Adet";

        [ObservableProperty]
        private int categoryId;

        [ObservableProperty]
        private bool isActive = true;

        // Pencereyi kapatmak için
        public event EventHandler<bool>? CloseWindow;

        // Komutlar
        public IAsyncRelayCommand SaveCommand { get; }

        public IRelayCommand CancelCommand { get; }

        // =====================================================
        // YENİ ÜRÜN
        // =====================================================

        public ProductEditViewModel()
        {
            _categoryApiService = new CategoryApiService();
            _productApiService = new ProductApiService();

            _ = LoadCategoriesAsync();

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(Cancel);

            WindowTitle = "Yeni Ürün";
        }

        // =====================================================
        // ÜRÜN DÜZENLE
        // =====================================================

        public ProductEditViewModel(Product product)
        {
            _productApiService = new ProductApiService();
            _categoryApiService = new CategoryApiService();

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(Cancel);

            _productId = product.Id;

            WindowTitle = "Ürün Düzenle";

            Barcode = product.Barcode;
            Name = product.Name;
            SalePrice = product.SalePrice;
            Stock = product.Stock;

            _ = LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories =
                    await _categoryApiService.GetAllCategoriesAsync();

                Categories.Clear();

                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Kategoriler yüklenirken hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



        // =====================================================
        // KAYDET
        // =====================================================

        private async Task SaveAsync()
        {
            // ---------------------------------------------
            // VALIDATION
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(Barcode))
            {
                MessageBox.Show(
                    "Barkod alanı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show(
                    "Ürün adı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (SalePrice < 0)
            {
                MessageBox.Show(
                    "Satış fiyatı negatif olamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (PurchasePrice < 0)
            {
                MessageBox.Show(
                    "Alış fiyatı negatif olamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (Stock < 0)
            {
                MessageBox.Show(
                    "Stok miktarı negatif olamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (CriticalStock < 0)
            {
                MessageBox.Show(
                    "Kritik stok miktarı negatif olamaz.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (CategoryId <= 0)
            {
                MessageBox.Show(
                    "Lütfen bir kategori seçin.",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                // =================================================
                // YENİ ÜRÜN
                // =================================================

                if (_productId == null)
                {
                    var request = new CreateProductRequest
                    {
                        Barcode = Barcode,
                        Name = Name,
                        Brand = Brand,
                        PurchasePrice = PurchasePrice,
                        SalePrice = SalePrice,
                        Stock = Stock,
                        CriticalStock = CriticalStock,
                        Unit = Unit,
                        CategoryId = CategoryId
                    };

                    var success =
                        await _productApiService
                            .CreateProductAsync(request);

                    if (!success)
                    {
                        MessageBox.Show(
                            "Ürün eklenemedi.",
                            "Hata",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        return;
                    }

                    MessageBox.Show(
                        "Ürün başarıyla eklendi.",
                        "Başarılı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    CloseWindow?.Invoke(this, true);

                    return;
                }

                // =================================================
                // ÜRÜN GÜNCELLE
                // =================================================

                var updateRequest = new UpdateProductRequest
                {
                    Barcode = Barcode,
                    Name = Name,
                    Brand = Brand,
                    PurchasePrice = PurchasePrice,
                    SalePrice = SalePrice,
                    Stock = Stock,
                    CriticalStock = CriticalStock,
                    Unit = Unit,
                    CategoryId = CategoryId,
                    IsActive = IsActive
                };

                var updateSuccess =
                    await _productApiService
                        .UpdateProductAsync(
                            _productId.Value,
                            updateRequest);

                if (!updateSuccess)
                {
                    MessageBox.Show(
                        "Ürün güncellenemedi.",
                        "Hata",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                MessageBox.Show(
                    "Ürün başarıyla güncellendi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CloseWindow?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"İşlem sırasında hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =====================================================
        // İPTAL
        // =====================================================

        private void Cancel()
        {
            CloseWindow?.Invoke(this, false);
        }
    }
}