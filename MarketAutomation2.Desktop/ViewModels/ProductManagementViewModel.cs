using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class ProductManagementViewModel : ObservableObject
    {
        private readonly ProductApiService _productApiService;

        // =====================================================
        // ÜRÜNLER
        // =====================================================

        public ObservableCollection<Product> Products { get; }
            = new ObservableCollection<Product>();

        // =====================================================
        // ARAMA
        // =====================================================

        [ObservableProperty]
        private string searchText = string.Empty;

        // =====================================================
        // KOMUTLAR
        // =====================================================

        public IAsyncRelayCommand LoadProductsCommand { get; }

        public IAsyncRelayCommand SearchCommand { get; }

        public IRelayCommand NewProductCommand { get; }

        public IRelayCommand<Product> EditCommand { get; }

        public IAsyncRelayCommand<Product> DeleteCommand { get; }

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public ProductManagementViewModel()
        {
            _productApiService = new ProductApiService();

            LoadProductsCommand =
                new AsyncRelayCommand(LoadProductsAsync);

            SearchCommand =
                new AsyncRelayCommand(SearchProductsAsync);

            NewProductCommand =
                new RelayCommand(NewProduct);

            EditCommand =
                new RelayCommand<Product>(EditProduct);

            DeleteCommand =
                new AsyncRelayCommand<Product>(DeleteProductAsync);
        }

        // =====================================================
        // TÜM ÜRÜNLERİ GETİR
        // =====================================================

        public async Task LoadProductsAsync()
        {
            try
            {
                var products =
                    await _productApiService.GetAllProductsAsync();

                Products.Clear();

                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ürünler yüklenirken hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =====================================================
        // ÜRÜN ARAMA
        // =====================================================

        private async Task SearchProductsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadProductsAsync();
                return;
            }

            var allProducts =
                await _productApiService.GetAllProductsAsync();

            var searchResult = allProducts
                .Where(x =>
                    x.Name.Contains(
                        SearchText,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    x.Barcode.Contains(
                        SearchText,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            Products.Clear();

            foreach (var product in searchResult)
            {
                Products.Add(product);
            }
        }

        // =====================================================
        // YENİ ÜRÜN
        // =====================================================

        private void NewProduct()
        {
            var viewModel = new ProductEditViewModel();

            var window = new ProductEditWindow(viewModel);

            var result = window.ShowDialog();

            if (result == true)
            {
                _ = LoadProductsAsync();
            }
        }

        // =====================================================
        // ÜRÜN DÜZENLE
        // =====================================================

        private void EditProduct(Product? product)
        {
            if (product == null)
                return;

            var viewModel =
                new ProductEditViewModel(product);

            var window =
                new ProductEditWindow(viewModel);

            var result = window.ShowDialog();

            if (result == true)
            {
                _ = LoadProductsAsync();
            }
        }

        // =====================================================
        // ÜRÜN SİL
        // =====================================================

        private async Task DeleteProductAsync(Product? product)
        {
            if (product == null)
                return;

            var result = MessageBox.Show(
                $"'{product.Name}' ürününü silmek istediğinize emin misiniz?",
                "Ürün Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var success =
                    await _productApiService
                        .DeleteProductAsync(product.Id);

                if (!success)
                {
                    MessageBox.Show(
                        "Ürün silinemedi.",
                        "Hata",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                Products.Remove(product);

                MessageBox.Show(
                    "Ürün başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ürün silinirken hata oluştu.\n\n{ex.Message}",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}