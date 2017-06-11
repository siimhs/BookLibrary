using System.Windows;
using LibraryWPF.ViewModels;

namespace LibraryWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Create window
            MainWindow mw = new MainWindow();
            //Create viewmodel
            ViewModelBase vm = LoginPageViewModel.GetLoginPageViewModel();            
            //ViewModelBase vm = MainWindowViewModel.GetMainViewModel();
            //Implement created viewmodel to window's datacontext
            mw.DataContext = vm;
            //Show window
            mw.Show();
        }
    }
}
