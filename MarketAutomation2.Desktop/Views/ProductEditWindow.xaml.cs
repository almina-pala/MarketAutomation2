using MarketAutomation2.Desktop.ViewModels;
using System.Windows;

namespace MarketAutomation2.Desktop.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow(ProductEditViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            viewModel.CloseWindow += ViewModel_CloseWindow;
        }

        private void ViewModel_CloseWindow(object? sender, bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}