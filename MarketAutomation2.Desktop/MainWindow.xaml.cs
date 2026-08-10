using MarketAutomation2.Desktop.Views;
using System.Windows;

namespace MarketAutomation2.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CashRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CashRegisterWindow();
            window.Show();
        }

        private void ProductManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProductManagementWindow();
            window.ShowDialog();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Raporlar modülü yakında eklenecek.",
                "Raporlar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Ayarlar modülü yakında eklenecek.",
                "Ayarlar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
