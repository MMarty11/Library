using System;
using System.Collections.Generic;
using System.Text;
using MyLMS.MVVM.Models;
using Microsoft.EntityFrameworkCore;
using MyLMS.Data;

namespace MyLMS.Services
{
    internal class LoanService
    {
        private readonly LibraryContext _context;

        public LoanService(LibraryContext context)
        {
            _context = context; 
        }

        public async Task<Loan> LoanBookAsync(int bookId, int userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
                throw new InvalidOperationException("Libro non disponibile.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Utente non esiste");

            var loan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.Now
            };

            book.IsAvailable = false;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task ReturnBookAsync(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null || loan.IsReturned)
                throw new InvalidOperationException("Prestito non valido");

            loan.ReturnDate = DateTime.Now;
            loan.Book.IsAvailable = true;

            await _context.SaveChangesAsync();
        }
    }
}
