using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryDAL
{
    /// <summary>
    /// Intermediate class between ViewModels and Database for the separation of concerns.
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Makes a query to database to retrive all books in database.
        /// </summary>
        /// <returns>ICollection of Books</returns>
        public ICollection<Book> getBooks()
        {            
            var query = new LibraryDBEntities().Book.Select(p => p as Book);
            return query.ToList();
        }
        /// <summary>
        /// Makes a query to database. Retrives FirstOrDefault() instance of book based on id. 
        /// </summary>
        /// <param name="id">Id used to search a book</param>
        /// <returns>Book if found, otherwise null</returns>
        public Book getBookById(int id)
        {
            var query = new LibraryDBEntities().Book.Where(p => p.BookId.Equals(id)).FirstOrDefault();
            return query;
        }
        /// <summary>
        /// Makes a query to database to retrive user who has loaned out that book. If book is not loaned out then null is returned.
        /// </summary>
        /// <param name="book">Book is used to search a book by id</param>
        /// <returns>User or null</returns>
        public User getBookHolder(Book book)
        {
            //var db = new LibraryDBEntities();
            //var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).FirstOrDefault();
            //return query.User;
            return getBookById(book.BookId).User;
        }
        /// <summary>
        /// Makes a query to database to retrive user by id. If user is not found then null is returned.
        /// </summary>
        /// <param name="id">Id is used to search user</param>
        /// <returns>User or null</returns>
        public User getUserById(int id)
        {
            var db = new LibraryDBEntities();
            var query = db.User.Where(p => p.UserId.Equals(id)).FirstOrDefault();
            return query;
        }
        /// <summary>
        /// Makes a query to database to retrive author by id. If author is not found then null is returned.
        /// </summary>
        /// <param name="id">Id is used to search author</param>
        /// <returns>Author or null</returns>
        public Author getAuthorById(int id)
        {
            var db = new LibraryDBEntities();
            var query = db.Author.Where(p => p.AuthorId.Equals(id)).FirstOrDefault();
            return query;
        }
        /// <summary>
        /// Makes a query to database to retrive book author. If book hasn't an author, then null is returned. 
        /// </summary>
        /// <param name="book">Book is used to search a book by id</param>
        /// <returns>Author or null</returns>
        public Author getBookAuthor(Book book)
        {
            //var db = new LibraryDBEntities();
            //var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).First();
            //return query.Author;
            return getBookById(book.BookId).Author;
        }
        /// <summary>
        /// Makes a query to database to retrive all books that user has loaned. If user hasn't loaned any books then null is returned.
        /// </summary>
        /// <param name="user">User is used to search user from database based on id</param>
        /// <returns>ICollection of Book or null</returns>
        public ICollection<Book> getBooksOnLoan(User user)
        {
            //var db = new LibraryDBEntities();
            //var query = db.User.Where(p => p.UserId.Equals(user.UserId)).FirstOrDefault();
            return getUserById(user.UserId).Book;
        }
        /// <summary>
        /// Makes a query to database to retrive all books that author has written. If author hasn't written any books then null is returned.
        /// </summary>
        /// <param name="author">Author is used to search author by id from database</param>
        /// <returns>ICollection of Books or null</returns>
        public ICollection<Book> getWrittenBooks(Author author)
        {
            //var db = new LibraryDBEntities();
            //var query = db.Author.Where(p => p.AuthorId.Equals(author.AuthorId)).First();
            //return query.Book;
            return getAuthorById(author.AuthorId).Book;
        }
        /// <summary>
        /// Assignes user to given book.
        /// </summary>
        /// <param name="user">user who loans a book</param>
        /// <param name="book">book to be loaned</param>
        public void setBookHolder(User user, Book book)
        {
            var db = new LibraryDBEntities();
            var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).First();
            query.User = user;
            db.SaveChanges();
        }
        /// <summary>
        /// Adds author to database.
        /// </summary>
        /// <param name="author">Author to be added to database</param>
        public void setAuthor(Author author)
        {
            var db = new LibraryDBEntities();
            using (db)
            {
                var query = db.Author.Where(p => p.AuthorId.Equals(author.AuthorId)).FirstOrDefault();

                query.AuthorId = author.AuthorId;
                query.AuthorFirstName = author.AuthorFirstName;
                query.AuthorLastName = author.AuthorLastName;

                db.SaveChanges();
            }
        }
        /// <summary>
        /// Deletes given book from database.
        /// </summary>
        /// <param name="book">Book to be deleted</param>
        public void DeleteBook(Book book)
        {
            var db = new LibraryDBEntities();
            var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).FirstOrDefault();
            db.Book.Remove(query);
            db.SaveChanges();
        }
        /// <summary>
        /// Updates given book's User(holder) to null and DueDate to null. Meaning that User has returned that book.
        /// </summary>
        /// <param name="book">Book whose user is to be assigned to null</param>
        public void ReturnBook(Book book)
        {
            var db = new LibraryDBEntities();
            var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).First();
            query.BookHolderId = null;
            query.DueDate = null;
            db.SaveChanges();
        }
        /// <summary>
        /// Makes a query to database to check if given book is over due. 
        /// </summary>
        /// <param name="book">Book to be checked</param>
        /// <returns>True if book is over due</returns>
        public bool IsDue(Book book)
        {
            if (book != null)
            {
                var db = new LibraryDBEntities();
                var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).First();
                if (query.DueDate < DateTime.Now)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Makes a query to database to update given book's User and DueDate. Meaning that book is loaned out to given user by username for given timespan.
        /// </summary>
        /// <param name="book">Book to be updated</param>
        /// <param name="username">User's username who will loan given book</param>
        /// <param name="loanTime">Due date is calculated by DateTime.Now + loanTime</param>
        public void LoanBook(Book book, string username, TimeSpan loanTime)
        {
            var db = new LibraryDBEntities();
            var query = db.Book.Where(p => p.BookId.Equals(book.BookId)).First();
            query.User = db.User.Where(p => p.Username.Equals(username)).First();
            query.DueDate = DateTime.Now + loanTime;
            db.SaveChanges();
        }
        /// <summary>
        /// Makes a query to database to update book's title. All books that have same title and author are updated.
        /// </summary>
        /// <param name="title">New title</param>
        /// <param name="book">Book to be updated</param>
        public void setBookTitle(string title, Book book)
        {
            var db = new LibraryDBEntities();
            var query = db.Book.Where(p => p.BookTitle.Equals(book.BookTitle) && p.BookAuthorId.Equals(book.BookAuthorId)).Select(p => (Book)p);
            foreach (var _book in query)
            {
                _book.BookTitle = title;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Validates username and password for authentication.
        /// </summary>
        /// <param name="username">Username is used to search user from database</param>
        /// <param name="hashedPassword">HashedPassword is used to compare password from database</param>
        /// <returns>True if passwords match</returns>
        public bool ValidatePassword(string username, string hashedPassword)
        {
            var query = new LibraryDBEntities().User.Where(p => p.Username.Equals(username)).FirstOrDefault();
            return query == null ? false : query.Password.Equals(hashedPassword) ? true : false;
        }
        /// <summary>
        /// Checks if username is unique.
        /// </summary>
        /// <param name="username">username to be checked</param>
        /// <returns>True if username is unique</returns>
        public bool ValidateUsername(string username)
        {
            var query = new LibraryDBEntities().User.Where(p => p.Username.Equals(username)).FirstOrDefault();
            return query == null ? true : false;
        }
        /// <summary>
        /// Adds given user to database. Exception is thrown if user.username is taken.
        /// </summary>
        /// <param name="user">New User to be added to database</param>
        public void AddUser(User user)
        {
            if (ValidateUsername(user.Username))
            {
                User _user = new User()
                {
                    Username = user.Username,
                    Password = user.Password
                };
                if (user.Book != null)
                {
                    foreach (Book book in user.Book)
                    {
                        _user.Book.Add(book);
                    }
                }

                var db = new LibraryDBEntities();
                db.User.Add(_user);
                db.SaveChanges();
            }
            else
            {
                throw new Exception("Username taken! Can't allow duplicate usernames in database.");
            }
        }
        /// <summary>
        /// Adds given book to database. If book.Author's first name and last name are not found in database then new author is created.
        /// </summary>
        /// <param name="book">Book to be added to database</param>
        public void AddBook(Book book)
        {
            var db = new LibraryDBEntities();
            Book newBook = new Book()
            {
                BookTitle = book.BookTitle,                
            };
            var author = db.Author.Where(p => p.AuthorFirstName.Equals(book.Author.AuthorFirstName) && p.AuthorLastName.Equals(book.Author.AuthorLastName)).FirstOrDefault();            

            if (author == null)
            {
                newBook.Author = book.Author;
                db.Book.Add(newBook);
            }
            else
            {
                newBook.Author = author;
                db.Book.Add(newBook);                    
            }
            db.SaveChanges();
        }
    }
}
