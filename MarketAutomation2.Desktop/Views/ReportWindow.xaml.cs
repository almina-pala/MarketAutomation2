using MarketAutomation2.Desktop.ViewModels;
using System.Windows;

namespace MarketAutomation2.Desktop.Views
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();

            DataContext = new ReportViewModel();
        }
    }
}