using MarketAutomation2.Desktop.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MarketAutomation2.Desktop.Views
{
    /// <summary>
    /// CashRegisterWindow - Professional POS Cash Register UI
    /// </summary>
    public partial class CashRegisterWindow : Window
    {
        private readonly CashRegisterViewModel _viewModel;

        public CashRegisterWindow()
        {
            InitializeComponent();

            _viewModel = new CashRegisterViewModel();
            DataContext = _viewModel;

            // Wire up barcode focus requests from ViewModel
            _viewModel.RequestBarcodeFocus += (_, _) => FocusBarcode();

            // Auto-focus barcode input on window load
            Loaded += (_, _) => FocusBarcode();
        }

        /// <summary>
        /// Set focus to barcode input and select all text
        /// </summary>
        private void FocusBarcode()
        {
            txtBarcode.Focus();
            txtBarcode.SelectAll();
        }

        /// <summary>
        /// Handle Enter key press on barcode input
        /// </summary>
        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await _viewModel.SearchBarcode();
                FocusBarcode();
            }
        }

        /// <summary>
        /// Handle Enter key press in Quantity TextBox to validate and update
        /// </summary>
        private void QuantityTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                // Force the binding to update the source (ViewModel property)
                if (sender is TextBox tb)
                {
                    // Convert comma to dot for decimal parsing (Turkish locale support)
                    // User types 5,4 but we need to convert to 5.4 for decimal parsing
                    tb.Text = tb.Text.Replace(",", ".");

                    var bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
                    bindingExpression?.UpdateSource();
                }

                FocusBarcode(); // Return focus to barcode input
            }
        }

        /// <summary>
        /// Handle text input in Quantity TextBox - allow only numbers and decimal separators (comma or dot)
        /// </summary>
        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow 0-9, comma (,) and dot (.) as decimal separators
            // User can type either 5,4 or 5.4
            var allowedChars = "0123456789,.";

            // Allow only numeric input and decimal separators
            foreach (var c in e.Text)
            {
                if (!allowedChars.Contains(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
