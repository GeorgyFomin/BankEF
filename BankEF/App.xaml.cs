using Application.ViewModels;
using System.Windows;

namespace BankEF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel mainViewModel = new();
            MainWindow mainWindow = new() { DataContext = mainViewModel };
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
