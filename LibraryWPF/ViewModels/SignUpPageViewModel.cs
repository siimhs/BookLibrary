using LibraryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryWPF.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
       
        private static volatile SignUpPageViewModel SignUpVm;
        public static object LockingObject { get; set; }
        public static SignUpPageViewModel GetSignUpPageViewModel()
        {
            LockingObject = new object();
            if (SignUpVm == null)
            {
                lock (LockingObject)
                {
                    if (SignUpVm == null)
                    {
                        SignUpVm = new SignUpPageViewModel();
                        //this LoadHomePage() call out has to be done when homeVM is created, NOT when homeVM is called out.
                        //It can't be done in ctor because, homeVM won't be assinged a value and algorithm becomes indefitely recursive.
                        //Call it out here when homeVM has been assign non-null value, so this part of code won't be reached again.
                        //base.LoadLoginPage();
                    }
                }
            }
            return SignUpVm;
        }
        private SignUpPageViewModel()
        {
            NewFormValidationError = string.Empty;
            SubmitNewFormCommand = new DelegateCommand(o => SubmitNewForm(o as PasswordBox));
        }
        
        private string _newFormValidationError;
        public string NewFormValidationError
        {
            get { return _newFormValidationError; }
            set { _newFormValidationError = value; OnPropertyChanged("NewFormValidationError"); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged("FirstName"); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged("LastName"); }
        }
        public ICommand SubmitNewFormCommand { get; set; }

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
        /// <summary>
        /// Creates new user to be added to database by repository. Validates empty fields and uniqueness of username.
        /// </summary>
        /// <param name="passwordBox">Passwordbox where password is inserted</param>
        public void SubmitNewForm(PasswordBox passwordBox)
        {
            //LoadLoginPage();
            if (!string.IsNullOrEmpty(passwordBox.Password) &&
               !string.IsNullOrEmpty(LastName) &&
               !string.IsNullOrEmpty(FirstName) &&
               !string.IsNullOrEmpty(Username))
            {
                var repo = new Repository();
                if (repo.ValidateUsername(Username))
                {
                    repo.AddUser(new User()
                    {
                        Username = Username,
                        Password = getHashString(passwordBox.Password),
                    });
                    LoadLoginPage();
                }
                else
                {
                    NewFormValidationError = "Username is taken!";
                }
            }
            else
            {
                NewFormValidationError = "Please fill in all fields!";
            }
        }
    }
}
