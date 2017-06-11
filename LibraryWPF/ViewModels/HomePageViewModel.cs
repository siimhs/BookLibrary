using LibraryDAL;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibraryWPF.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private static volatile HomePageViewModel HomeVm;
        public static object LockingObject { get; set; }
        /// <summary>
        /// Returns HomePageViewModel if viewmodel has been created previuously. Otherwise creates it and then returns it. Thread safe.
        /// </summary>
        /// <returns>HomeViewModel object</returns>
        public static HomePageViewModel GetHomePageViewModel()
        {
            LockingObject = new object();
            if (HomeVm == null)
            {
                lock (LockingObject)
                {
                    if (HomeVm == null)
                    {
                        HomeVm = new HomePageViewModel();
                        //this LoadHomePage() call out has to be done when homeVM is created, NOT when homeVM is called out.
                        //It can't be done in ctor because, homeVM won't be assinged a value and algorithm becomes indefitely recursive.
                        //Call it out here when homeVM has been assigned non-null value, so this part of code won't be reached again.
                        ////HomeVm.LoadHomePage();
                    }
                }
            }
            return HomeVm;
        }
                                
        private HomePageViewModel()
        {
            AddBookCommand = new DelegateCommand(o => AddBook(), p => _AddBookAuthorFirstName != null && _AddBookAuthorLastName != null && _AddBookTitle != null);
            ModifyBookCommand = new DelegateCommand(o => ModifyBook((Book)o));
            LoanBookCommand = new DelegateCommand(o => LoanBook((Book)o));
            ReturnBookCommand = new DelegateCommand(o => ReturnBook((Book)o));
            DeleteBookCommand = new DelegateCommand(o => DeleteBook((Book)o));
            SearchCommand = new DelegateCommand(o => Search());
            LogoutCommand = new DelegateCommand(o => Logout());
            FilterOverDueBooksCommand = new DelegateCommand(o => FilterOverDueBooks(), o => FilterLoanedBooksCommandIsExecuted);
            FilterLoanedBooksCommand = new DelegateCommand(o => FilterLoanedBooks());

            LoanTime = new TimeSpan(0, 0, 10); //Set book loan time. hours, minutes, seconds.

            SearchFirstName = string.Empty;
            SearchLastName = string.Empty;
            SearchTitle = string.Empty;

            GetBooks();
        }

        #region Fields and Properties
        public ICommand FilterLoanedBooksCommand { get; set; }
        public ICommand FilterOverDueBooksCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand DeleteBookCommand { get; set; }
        public ICommand AddBookCommand { get; set; }
        public ICommand LoanBookCommand { get; set; }
        public ICommand ReturnBookCommand { get; set; }
        public ICommand ModifyBookCommand { get; set; }
        public TimeSpan LoanTime { get; set; }
        public bool FilterLoanedBooksCommandIsExecuted { get; set; } = false;

        private string _searchLastName;
        public string SearchLastName
        {
            get { return _searchLastName; }
            set { _searchLastName = value; OnPropertyChanged("SearchLastName"); }
        }

        private string _searchFirstName;
        public string SearchFirstName
        {
            get { return _searchFirstName; }
            set { _searchFirstName = value; OnPropertyChanged("SearchFirstName"); }
        }

        private string _searchTitle;
        public string SearchTitle
        {
            get { return _searchTitle; }
            set { _searchTitle = value; OnPropertyChanged("SearchTitle"); }
        }

        private string _AddBookTitle;
        public string AddBookTitle
        {
            get { return _AddBookTitle; }
            set { _AddBookTitle = value; OnPropertyChanged("AddBookTitle"); }
        }

        private string _AddBookAuthorFirstName;
        public string AddBookAuthorFirstName
        {
            get { return _AddBookAuthorFirstName; }
            set { _AddBookAuthorFirstName = value; OnPropertyChanged("AddBookAuthorFirstName"); }
        }
        private string _AddBookAuthorLastName;
        public string AddBookAuthorLastName
        {
            get { return _AddBookAuthorLastName; }
            set { _AddBookAuthorLastName = value; OnPropertyChanged("AddBookAuthorLastName"); }
        }

        private ObservableCollection<Book> _book = new ObservableCollection<Book>();
        public ObservableCollection<Book> BookCollection
        {
            get { return _book; }
            set { _book = value; OnPropertyChanged("Book"); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Removes books from BookCollection that are not loaned out, therefore filtering loaned books.
        /// </summary>
        private void FilterLoanedBooks()
        {
            if (FilterLoanedBooksCommandIsExecuted == false)
            {
                FilterLoanedBooksCommandIsExecuted = true;
                foreach (Book book in BookCollection.ToList())
                {
                    if (book.User == null)
                        BookCollection.Remove(book);
                }
            }
            else
            {
                FilterLoanedBooksCommandIsExecuted = false;
                GetBooks();
            }
        }
        /// <summary>
        /// Removes books from BookCollection that are not over due, therefore filtering over due books.
        /// </summary>
        private void FilterOverDueBooks()
        {
            var repo = new Repository();
            foreach (Book book in BookCollection.ToList())
            {
                if (!repo.IsDue(book))
                    BookCollection.Remove(book);
            }
        }
        /// <summary>
        /// Clears usersession and loads login page.
        /// </summary>
        private void Logout()
        {
            //LoggedInUsername = string.Empty;
            //IsLoggedIn = false;
            LoadLoginPage();
        }
        /// <summary>
        /// Makes a query to BookColletction based on filled search fields and copies query to BookCollection. If all search fields are empty, then all books in current BookCollection are return by query.  
        /// </summary>
        private void Search()
        {
            GetBooks();

            ObservableCollection<Book> bufferList = new ObservableCollection<Book>();

            var SearchQuery = BookCollection.ToList().Where(p =>
            !SearchTitle.Equals(string.Empty) ? p.BookTitle.ToLower().Contains(SearchTitle.Trim()) : true &&
            !SearchFirstName.Equals(string.Empty) ? p.Author.AuthorFirstName.ToLower().Contains(SearchFirstName.Trim().ToLower()) : true &&
            !SearchLastName.Equals(string.Empty) ? p.Author.AuthorLastName.ToLower().Contains(SearchLastName.Trim().ToLower()) : true).Select(p => (Book)p);


            foreach (Book b in SearchQuery)
            {
                bufferList.Add(b);
            }

            if (bufferList != null)
            {
                BookCollection.Clear();
                foreach (Book b in bufferList)
                {
                    BookCollection.Add(b);
                }
            }
        }
        /// <summary>
        /// Calls repository to retrive books. Populates BookCollection with retrived books. 
        /// </summary>
        private void GetBooks()
        {
            BookCollection.Clear();
            foreach (Book book in new Repository().getBooks())
            {
                BookCollection.Add(book);
            }
        }
        /// <summary>
        /// Creates a book, calls repository to add and refreshes BookCollection. Expects that AddBookTitle, AddAuthorFirstName and AddAuthorLastName are not null.
        /// </summary>
        private void AddBook()
        {
            var repo = new Repository();
            Author a = new Author()
            {
                AuthorFirstName = AddBookAuthorFirstName,
                AuthorLastName = AddBookAuthorLastName
            };
            Book b = new Book()
            {
                BookTitle = AddBookTitle,
                Author = a
            };

            repo.AddBook(b);
            GetBooks();
            //OnPropertyChanged("Book");
        }
        /// <summary>
        /// Calls repository to delete a book.
        /// </summary>
        /// <param name="book"></param>
        private void DeleteBook(Book book)
        {
            var repo = new Repository();
            if (book != null)
            {
                BookCollection.Remove(book);
                repo.DeleteBook(book);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a book!");
            }
        }
        /// <summary>
        /// Calls repository to remove user from book. Accepts books that are over due, but let's user to know that they are late.
        /// </summary>
        /// <param name="book"></param>
        private void ReturnBook(Book book)
        {
            var repo = new Repository();
            if (book != null)
            {
                if (book.User.Username.Equals(LoggedInUsername))
                {
                    if (repo.IsDue(book))
                    {
                        System.Windows.Forms.MessageBox.Show("Sir, You are late!");
                    }
                    repo.ReturnBook(book);
                    GetBooks();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("You can't return a book that you did't loan!");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a book!");
            }
        }    
        /// <summary>
        /// Calls repository to add user to book. Allows to add user to book only when book is not loaned out.
        /// </summary>
        /// <param name="book"></param>
        private void LoanBook(Book book)
        {
            var repo = new Repository();
            if (book != null)
            {
                if (book.BookHolderId == null)
                {
                    repo.LoanBook(book, LoggedInUsername, LoanTime);
                    GetBooks();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show($"Book is loaned out to {book.User.Username}!");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a book!");
            }
        }
        /// <summary>
        /// Calls repository to update book data and refreshes BookCollection after successful update.
        /// </summary>
        /// <param name="book"></param>
        private void ModifyBook(Book book)
        {

            var repo = new Repository();
            if (book != null)
            {
                repo.setAuthor(book.Author);
                repo.setBookTitle(book.BookTitle, repo.getBookById(book.BookId));
                GetBooks();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a book!");
            }
        }
        #endregion
    }
}
