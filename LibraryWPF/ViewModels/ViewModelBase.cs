using System.ComponentModel;
using System.Windows.Input;

namespace LibraryWPF.ViewModels
{
    /// <summary>
    /// Base ViewModel for sharing usersession, currentViewModel and access to all switch views.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged interface implementations - eventhandler and method to catch property changes
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            //if propertychanged == null means it doesn't have any listneners
            //following code can be written like this:
            //if (PropertyChanged != null)
            //{
            //    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //}
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Fields
        public static bool IsLoggedIn { get; protected set; } = false;
        public static string LoggedInUsername { get; protected set; }
        private static ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }
        public ICommand LoadHomePageCommand { get; set; } 
        public ICommand LoadLoginPageCommand { get; set; }
        public ICommand LoadSignUpPageCommand { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes heritable commands
        /// </summary>
        protected void InitializeCommands()
        {
            LoadHomePageCommand = new DelegateCommand(o => LoadHomePage());
            LoadLoginPageCommand = new DelegateCommand(o => LoadLoginPage());
            LoadSignUpPageCommand = new DelegateCommand(o => LoadSignUpPage());
        }
        /// <summary>
        /// Updates CurrentViewModel to HomePageViewModel. If view's DataTemplate and DataContext is binded, then view is changed and control is given over to HomeViewModel. 
        /// </summary>
        protected void LoadHomePage()
        {
            if (IsLoggedIn)
                CurrentViewModel = HomePageViewModel.GetHomePageViewModel();
            else
            {
                LoadLoginPage();
            }
                
        }
        /// <summary>
        /// Updates CurrentViewModel to LoginPageViewModel. If view's DataTemplate and DataContext is binded, then view is changed and control is given over to LoginPageViewModel. 
        /// </summary>
        protected void LoadLoginPage()
        {            
            CurrentViewModel = LoginPageViewModel.GetLoginPageViewModel();
        }
        /// <summary>
        /// Updates CurrentViewModel to SignUpPageViewModel. If view's DataTemplate and DataContext is binded, then view is changed and control is given over to SignUpPageViewModel. 
        /// </summary>
        protected void LoadSignUpPage()
        {
            CurrentViewModel = SignUpPageViewModel.GetSignUpPageViewModel();
        }
        #endregion
    }
}
