using System;
using System.Data.Entity;
using System.Net;

namespace Library.Domain
{
    public class LibraryService
    {
        private readonly ILibraryDbContext _db;

        public LibraryService(ILibraryDbContext db)
        {
            _db = db;
        }

        public void LendBook(int bookId, int userId)
        {
            try
            {
                _db.BeginTransaction();

                var book = _db.Books.Find(bookId);
                var user = _db.Users.Find(userId);

                if (book == null) throw new Exception("Book does not exist.");
                if (user == null) throw new Exception("User does not exist.");

                if (user.HasPendingFines)
                {
                    throw new Exception($"User {user.Name} has pending fines.");
                }

                if (book.OnLoan) throw new Exception("Book is already on loan.");

                book.OnLoan = true;

                _db.SaveChanges();
                _db.Commit();
            }
            catch (Exception)
            {
                _db.Rollback();
                throw;
            }
        }

        public void ReturnBook(int bookId, int userId, DateTime actualReturnDate)
        {
            var book = _db.Books.Find(bookId);
            var user = _db.Users.Find(userId);

            if (book == null || user == null)
                throw new Exception("Book or User not found.");

            var daysOverdue = (actualReturnDate - book.DueDate).Days;

            if (daysOverdue > 0)
            {
                decimal calculatedFine = daysOverdue * 10;
                user.TotalFines += calculatedFine;

                if (user.TotalFines > 50)
                {
                    user.HasPendingFines = true;
                }
            }

            book.OnLoan = false;
            _db.SaveChanges();
        }
    }
}