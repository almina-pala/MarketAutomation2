using MarketAutomation2.Desktop.ViewModels;
using System.Windows;

namespace MarketAutomation2.Desktop.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel();
        }
    }
}