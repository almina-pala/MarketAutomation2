using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketAutomation2.Desktop.Api;
using MarketAutomation2.Desktop.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;

namespace MarketAutomation2.Desktop.ViewModels
{
    public partial class CategoryManagementViewModel : ObservableObject
    {
        private readonly CategoryApiService _categoryApiService;

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        private string categoryName = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public IAsyncRelayCommand LoadCategoriesCommand { get; }

        public IAsyncRelayCommand AddCategoryCommand { get; }

        public IRelayCommand<Category> EditCommand { get; }

        public IAsyncRelayCommand<Category> DeleteCommand { get; }

        public CategoryManagementViewModel()
        {
            _categoryApiService = new CategoryApiService();

            LoadCategoriesCommand =
                new AsyncRelayCommand(LoadCategoriesAsync);

            AddCategoryCommand =
                new AsyncRelayCommand(AddCategoryAsync);

            EditCommand =
                new RelayCommand<Category>(EditCategory);

            DeleteCommand =
                new AsyncRelayCommand<Category>(DeleteCategoryAsync);
        }

        public async Task LoadCategoriesAsync()
        {
            IsLoading = true;

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
            catch (HttpRequestException)
            {
                ShowError(
                    "Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.");
            }
            catch (Exception)
            {
                ShowError("Kategoriler yüklenirken bir hata oluştu.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                ShowError("Kategori adı boş bırakılamaz.");
                return;
            }

            try
            {
                var category =
                    await _categoryApiService.CreateCategoryAsync(
                        CategoryName.Trim());

                if (category != null)
                {
                    Categories.Add(category);
                }

                CategoryName = string.Empty;

                MessageBox.Show(
                    "Kategori başarıyla eklendi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanılamadı.");
            }
            catch (Exception ex)
            {
                ShowError($"Kategori eklenirken hata oluştu:\n{ex.Message}");
            }
        }

        private void EditCategory(Category? category)
        {
            if (category == null)
                return;

            string newName = Microsoft.VisualBasic.Interaction.InputBox(
                "Yeni kategori adını girin:",
                "Kategori Düzenle",
                category.Name);

            if (string.IsNullOrWhiteSpace(newName))
                return;

            _ = UpdateCategoryAsync(category, newName.Trim());
        }

        private async Task UpdateCategoryAsync(
            Category category,
            string newName)
        {
            try
            {
                var success =
                    await _categoryApiService.UpdateCategoryAsync(
                        category.Id,
                        newName);

                if (!success)
                {
                    ShowError("Kategori güncellenemedi.");
                    return;
                }

                category.Name = newName;

                var index = Categories.IndexOf(category);

                if (index >= 0)
                {
                    Categories[index] = category;
                }

                MessageBox.Show(
                    "Kategori başarıyla güncellendi.",
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
                ShowError("Kategori güncellenirken hata oluştu.");
            }
        }

        private async Task DeleteCategoryAsync(Category? category)
        {
            if (category == null)
                return;

            var result = MessageBox.Show(
                $"'{category.Name}' kategorisini silmek istediğinize emin misiniz?",
                "Kategori Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var success =
                    await _categoryApiService.DeleteCategoryAsync(
                        category.Id);

                if (!success)
                {
                    ShowError("Kategori silinemedi.");
                    return;
                }

                Categories.Remove(category);

                MessageBox.Show(
                    "Kategori başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (HttpRequestException)
            {
                ShowError("Sunucuya bağlanamadı.");
            }
            catch (Exception ex)
            {
                ShowError(
                    $"Kategori silinirken hata oluştu:\n{ex.Message}");
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Hata",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}