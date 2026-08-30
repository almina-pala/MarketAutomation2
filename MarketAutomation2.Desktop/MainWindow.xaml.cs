using MarketAutomation2.Desktop.Views;
using MarketAutomation2.Desktop.Services;
using System.Windows;

namespace MarketAutomation2.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settingsService = new AppSettingsService();

            var settings = settingsService.Load();

            if (!string.IsNullOrWhiteSpace(settings.MarketName))
            {
                txtMarketName.Text =
                    settings.MarketName.ToUpper();
            }
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

        private void CategoryManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CategoryManagementWindow
            {
                Owner = this
            };

            window.ShowDialog();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReportWindow();

            window.Owner = this;

            window.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            window.Owner = this;

            window.ShowDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
