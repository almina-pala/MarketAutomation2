using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using MarketAutomation2.Desktop.Views;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class ProductManagementViewModel : ObservableObject
    {
        private readonly ProductApiService _productApiService;

        public ObservableCollection<Product> Products { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public IAsyncRelayCommand LoadProductsCommand { get; }

        public IAsyncRelayCommand SearchCommand { get; }

        public IRelayCommand NewProductCommand { get; }

        public IRelayCommand<Product> EditCommand { get; }

        public IAsyncRelayCommand<Product> DeleteCommand { get; }

        public ProductManagementViewModel()
        {
            _productApiService = new ProductApiService();

            LoadProductsCommand = new AsyncRelayCommand(LoadProductsAsync);
            SearchCommand = new AsyncRelayCommand(SearchProductsAsync);
            NewProductCommand = new RelayCommand(NewProduct);
            EditCommand = new RelayCommand<Product>(EditProduct);
            DeleteCommand = new AsyncRelayCommand<Product>(DeleteProductAsync);
        }

        public async Task LoadProductsAsync()
        {
            IsLoading = true;

            try
            {
                var products = await _productApiService.GetAllProductsAsync();

                Products.Clear();

                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.");
            }
            catch (Exception)
            {
                ShowError("Ürünler yüklenirken bir hata oluştu.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchProductsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadProductsAsync();
                return;
            }

            IsLoading = true;

            try
            {
                var allProducts = await _productApiService.GetAllProductsAsync();

                var searchResult = allProducts
                    .Where(x =>
                        x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Barcode.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Products.Clear();

                foreach (var product in searchResult)
                {
                    Products.Add(product);
                }
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı.");
            }
            catch (Exception)
            {
                ShowError("Arama sırasında bir hata oluştu.");
            }
            finally
            {
                IsLoading = false;
            }
        }

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

        private void EditProduct(Product? product)
        {
            if (product == null)
                return;

            var viewModel = new ProductEditViewModel(product.Id);
            var window = new ProductEditWindow(viewModel);
            var result = window.ShowDialog();

            if (result == true)
            {
                _ = LoadProductsAsync();
            }
        }

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
                var success = await _productApiService.DeleteProductAsync(product.Id);

                if (!success)
                {
                    ShowError("Ürün silinemedi.");
                    return;
                }

                Products.Remove(product);

                MessageBox.Show(
                    "Ürün başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı.");
            }
            catch (Exception)
            {
                ShowError("Ürün silinirken bir hata oluştu.");
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
