using LibraryDAL;
using System.Linq;

namespace ConsoleTestApp
{
    /// <summary>
    /// Testing platform for various situations
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var db = new LibraryDBEntities();
            
            var q = db.Book.Where(p => p.BookId.Equals(1)).FirstOrDefault();
            
            System.Console.WriteLine(q.BookAuthorId);
            System.Console.WriteLine(q.BookId);
            System.Console.WriteLine(q.BookTitle);
            System.Console.WriteLine(q.Author.AuthorFirstName);
        }
    }
}
