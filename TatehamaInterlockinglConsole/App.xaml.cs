using System.Windows;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Helpers;
using TatehamaInterlockinglConsole.ViewModels;
using TatehamaInterlockinglConsole.Views;

namespace TatehamaInterlockinglConsole
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainViewModel(new TimeService(), new UIElementLoader(), DataManager.Instance);
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }
    }
}
