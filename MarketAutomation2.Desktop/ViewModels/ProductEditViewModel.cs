using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class ProductEditViewModel : ObservableObject
    {
        private readonly CategoryApiService _categoryApiService;
        private readonly ProductApiService _productApiService;
        private readonly int? _productId;

        public ObservableCollection<Category> Categories { get; } = new();

        public ObservableCollection<string> UnitOptions { get; } = new()
        {
            "Adet", "Kg", "Gram", "Litre"
        };

        [ObservableProperty]
        private string windowTitle = "Yeni Ürün";

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

        [ObservableProperty]
        private bool isLoading;

        public event EventHandler<bool>? CloseWindow;

        public IAsyncRelayCommand SaveCommand { get; }

        public IRelayCommand CancelCommand { get; }

        public ProductEditViewModel()
        {
            _categoryApiService = new CategoryApiService();
            _productApiService = new ProductApiService();

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(Cancel);

            WindowTitle = "Yeni Ürün";
            _ = InitializeAsync();
        }

        public ProductEditViewModel(int productId)
        {
            _categoryApiService = new CategoryApiService();
            _productApiService = new ProductApiService();
            _productId = productId;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(Cancel);

            WindowTitle = "Ürün Düzenle";
            _ = InitializeAsync(productId);
        }

        private async Task InitializeAsync(int? productId = null)
        {
            IsLoading = true;

            try
            {
                await LoadCategoriesAsync();

                if (productId.HasValue)
                    await LoadProductAsync(productId.Value);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryApiService.GetAllCategoriesAsync();

                Categories.Clear();

                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (HttpRequestException)
            {
                ShowError("Kategoriler yüklenemedi. Sunucuya bağlanılamadı.");
            }
            catch (Exception)
            {
                ShowError("Kategoriler yüklenirken bir hata oluştu.");
            }
        }

        private async Task LoadProductAsync(int productId)
        {
            try
            {
                var product = await _productApiService.GetByIdAsync(productId);

                if (product == null)
                {
                    ShowError("Ürün bulunamadı.");
                    CloseWindow?.Invoke(this, false);
                    return;
                }

                Barcode = product.Barcode;
                Name = product.Name;
                Brand = product.Brand;
                PurchasePrice = product.PurchasePrice;
                SalePrice = product.SalePrice;
                Stock = product.Stock;
                CriticalStock = product.CriticalStock;
                Unit = string.IsNullOrWhiteSpace(product.Unit) ? "Adet" : product.Unit;
                IsActive = product.IsActive;

                var category = Categories.FirstOrDefault(
                    c => c.Name.Equals(product.CategoryName, StringComparison.OrdinalIgnoreCase));

                if (category != null)
                    CategoryId = category.Id;
            }
            catch (HttpRequestException)
            {
                ShowError("Ürün bilgileri yüklenemedi. Sunucuya bağlanılamadı.");
            }
            catch (Exception)
            {
                ShowError("Ürün bilgileri yüklenirken bir hata oluştu.");
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Barcode))
            {
                ShowWarning("Barkod alanı boş bırakılamaz.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowWarning("Ürün adı boş bırakılamaz.");
                return;
            }

            if (SalePrice < 0)
            {
                ShowWarning("Satış fiyatı negatif olamaz.");
                return;
            }

            if (PurchasePrice < 0)
            {
                ShowWarning("Alış fiyatı negatif olamaz.");
                return;
            }

            if (Stock < 0)
            {
                ShowWarning("Stok miktarı negatif olamaz.");
                return;
            }

            if (CriticalStock < 0)
            {
                ShowWarning("Kritik stok miktarı negatif olamaz.");
                return;
            }

            if (CategoryId <= 0)
            {
                ShowWarning("Lütfen bir kategori seçin.");
                return;
            }

            try
            {
                if (_productId == null)
                {
                    var request = new CreateProductRequest
                    {
                        Barcode = Barcode.Trim(),
                        Name = Name.Trim(),
                        Brand = Brand.Trim(),
                        PurchasePrice = PurchasePrice,
                        SalePrice = SalePrice,
                        Stock = Stock,
                        CriticalStock = CriticalStock,
                        Unit = Unit,
                        CategoryId = CategoryId
                    };

                    var success = await _productApiService.CreateProductAsync(request);

                    if (!success)
                    {
                        ShowError("Ürün eklenemedi.");
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

                var updateRequest = new UpdateProductRequest
                {
                    Barcode = Barcode.Trim(),
                    Name = Name.Trim(),
                    Brand = Brand.Trim(),
                    PurchasePrice = PurchasePrice,
                    SalePrice = SalePrice,
                    Stock = Stock,
                    CriticalStock = CriticalStock,
                    Unit = Unit,
                    CategoryId = CategoryId,
                    IsActive = IsActive
                };

                var updateSuccess = await _productApiService.UpdateProductAsync(
                    _productId.Value,
                    updateRequest);

                if (!updateSuccess)
                {
                    ShowError("Ürün güncellenemedi.");
                    return;
                }

                MessageBox.Show(
                    "Ürün başarıyla güncellendi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CloseWindow?.Invoke(this, true);
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı.");
            }
            catch (Exception)
            {
                ShowError("İşlem sırasında bir hata oluştu.");
            }
        }

        private void Cancel()
        {
            CloseWindow?.Invoke(this, false);
        }

        private static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
