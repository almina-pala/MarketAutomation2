using MarketAutomation2.Desktop.ViewModels;
using System.Windows;

namespace MarketAutomation2.Desktop.Views
{
    public partial class ProductManagementWindow : Window
    {
        private readonly ProductManagementViewModel _viewModel;

        public ProductManagementWindow()
        {
            InitializeComponent();

            _viewModel = new ProductManagementViewModel();

            DataContext = _viewModel;

            Loaded += async (_, _) =>
            {
                await _viewModel.LoadProductsAsync();
            };
        }
    }
}