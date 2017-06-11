using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryDAL;

namespace LibraryWPF.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private static volatile LoginPageViewModel LoginVm;
        public static object LockingObject { get; set; }
        public static LoginPageViewModel GetLoginPageViewModel()
        {
            LockingObject = new object();
            if (LoginVm == null)
            {
                lock (LockingObject)
                {
                    if (LoginVm == null)
                    {
                        LoginVm = new LoginPageViewModel();
                        //this LoadHomePage() call out has to be done when homeVM is created, NOT when homeVM is called out.
                        //It can't be done in ctor because, homeVM won't be assinged a value and algorithm becomes indefitely recursive.
                        //Call it out here when homeVM has been assign non-null value, so this part of code won't be reached again.
                        LoginVm.LoadLoginPage();
                    }
                }
            }
            return LoginVm;
        }
        private LoginPageViewModel()
        {
            LoginCommand = new DelegateCommand(o => Login(o as PasswordBox));
            InitializeCommands();
            //Username = "Preload Username";
        }

        private string _validationErrorMessage;
        public string ValidationErrorMessage
        {
            get { return _validationErrorMessage; }
            set { _validationErrorMessage = value; OnPropertyChanged("ValidationErrorMessage"); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }
        public DelegateCommand LoginCommand { get; private set; }

        /// <summary>
        /// Calls repository to validate username and passwork(hashed).
        /// </summary>
        /// <param name="pbox">Passwordbox where password is held. This is hashed before forwarded to repository</param>
        public void Login(PasswordBox pbox)
        {
            Repository repo = new Repository();
            if (repo.ValidatePassword(Username, getHashString(pbox.Password)))
            {
                IsLoggedIn = true;
                LoggedInUsername = Username;
                LoadHomePage();
            }
            else
            {
                ValidationErrorMessage = "Invalid credentials!";
            }
        }
        /// <summary>
        /// Hashes given string to sha256 hash.
        /// </summary>
        /// <param name="text">String to be hashed</param>
        /// <returns>Sha 256 hashed stirng</returns>
        public string getHashString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }
    }
}
