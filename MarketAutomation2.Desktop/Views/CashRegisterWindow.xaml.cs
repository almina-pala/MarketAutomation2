using MarketAutomation2.Desktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MarketAutomation2.Desktop.Views
{
    public partial class CashRegisterWindow : Window
    {
        private readonly CashRegisterViewModel _viewModel;

        public CashRegisterWindow()
        {
            InitializeComponent();

            _viewModel = new CashRegisterViewModel();

            DataContext = _viewModel;

            Loaded += (s, e) => txtBarcode.Focus();
        }

        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await _viewModel.SearchBarcode();

                txtBarcode.Focus();
            }
        }

        private void dgProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}