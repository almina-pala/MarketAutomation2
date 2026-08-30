using MarketAutomation2.Desktop.ViewModels;
using System.Windows;

namespace MarketAutomation2.Desktop.Views
{
    public partial class CategoryManagementWindow : Window
    {
        private readonly CategoryManagementViewModel _viewModel;

        public CategoryManagementWindow()
        {
            InitializeComponent();

            _viewModel = new CategoryManagementViewModel();

            DataContext = _viewModel;

            Loaded += CategoryManagementWindow_Loaded;
        }

        private async void CategoryManagementWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.LoadCategoriesAsync();
        }
    }
}