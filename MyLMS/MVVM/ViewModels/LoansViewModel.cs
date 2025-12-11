using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using MyLMS.MVVM.Models;
using Microsoft.EntityFrameworkCore;
using MyLMS.Data;
using MyLMS.Core;

namespace MyLMS.MVVM.ViewModels
{
    internal class LoansViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public LoansViewModel()
        {
            _context = new LibraryContext();

            Users = new ObservableCollection<User>();
            AvailableBooks = new ObservableCollection<Book>();
            ActiveLoans = new ObservableCollection<Loan>();

            LoanBookCommand = new RelayCommand(LoanBook, CanLoanBook);
            ReturnBookCommand = new RelayCommand(ReturnBook, CanReturnBook);
            RefreshCommand = new RelayCommand(LoadData);

            Message = string.Empty;

            LoadData();
        }

        // --------- Collezioni ---------

        public ObservableCollection<User> Users { get; }
        public ObservableCollection<Book> AvailableBooks { get; }
        public ObservableCollection<Loan> ActiveLoans { get; }

        // --------- Messaggi utente ---------

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        private string _messageColor = string.Empty;
        public string MessageColor
        {
            get => _messageColor;
            set { _messageColor = value; OnPropertyChanged(); }
        }

        // --------- Selezioni correnti ---------

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private Loan? _selectedLoan;
        public Loan? SelectedLoan
        {
            get => _selectedLoan;
            set
            {
                _selectedLoan = value;
                OnPropertyChanged();
                (ReturnBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                (LoanBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private Book? _selectedAvailableBook;
        public Book? SelectedAvailableBook
        {
            get => _selectedAvailableBook;
            set
            {
                _selectedAvailableBook = value;
                OnPropertyChanged();
                (LoanBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private Loan? _selectedActiveLoan;
        public Loan? SelectedActiveLoan
        {
            get => _selectedActiveLoan;
            set
            {
                _selectedActiveLoan = value;
                OnPropertyChanged();
                (ReturnBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // --------- Comandi ---------


        public ICommand SearchCommand { get; }
        public ICommand LoadAllCommand { get; }

        public ICommand LoanBookCommand { get; }
        public ICommand ReturnBookCommand { get; }
        public ICommand RefreshCommand { get; }

        // --------- Logica ---------

        private void LoadAllActiveLoans()
        {
            ActiveLoans.Clear();

            var query = _context.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Where(l => l.ReturnDate == null)
                                .OrderBy(l => l.LoanDate);

            foreach (var loan in query)
                ActiveLoans.Add(loan);
        }

        private void SearchLoans()
        {
            ActiveLoans.Clear();

            var query = _context.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Where(l => l.ReturnDate == null)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var text = SearchText.ToLower();

                query = query.Where(l =>
                    ((l.Book.Title ?? string.Empty).ToLower().Contains(text)) ||
                    ((l.Book.Author ?? string.Empty).ToLower().Contains(text)) ||
                    ((l.User.FullName ?? string.Empty).ToLower().Contains(text)) ||
                    ((l.Book.Isbn ?? string.Empty).ToLower().Contains(text))
                );
            }

            foreach (var loan in query.OrderBy(l => l.LoanDate))
                ActiveLoans.Add(loan);
        }


        private void LoadData()
        {
            // Utenti
            Users.Clear();
            foreach (var u in _context.Users.OrderBy(u => u.FullName))
                Users.Add(u);

            // Libri disponibili
            AvailableBooks.Clear();
            var books = _context.Books
                                .Where(b => b.IsAvailable)
                                .OrderBy(b => b.Title);
            foreach (var b in books)
                AvailableBooks.Add(b);

            // Prestiti attivi (non restituiti)
            ActiveLoans.Clear();
            var loans = _context.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Where(l => l.ReturnDate == null)
                                .OrderBy(l => l.LoanDate);
            foreach (var l in loans)
                ActiveLoans.Add(l);
        }

        private bool CanLoanBook()
        {
            return SelectedUser != null && SelectedAvailableBook != null;
        }

        private void LoanBook()
        {
            if (SelectedUser == null || SelectedAvailableBook == null)
                return;

            var loan = new Loan
            {
                BookId = SelectedAvailableBook.Id,
                UserId = SelectedUser.Id,
                LoanDate = System.DateTime.Now
            };

            SelectedAvailableBook.IsAvailable = false;

            _context.Loans.Add(loan);
            _context.SaveChanges();

            // Ricarico dati per aggiornare liste
            LoadData();
        }

        private bool CanReturnBook()
        {
            return SelectedActiveLoan != null;
        }

        private void ReturnBook()
        {
            if (SelectedActiveLoan == null)
                return;

            var loan = _context.Loans
                               .Include(l => l.Book)
                               .FirstOrDefault(l => l.Id == SelectedActiveLoan.Id);

            if (loan == null || loan.ReturnDate != null)
                return;

            loan.ReturnDate = System.DateTime.Now;
            if (loan.Book != null)
            {
                loan.Book.IsAvailable = true;
            }

            _context.SaveChanges();

            // Ricarico dati per aggiornare liste
            LoadData();
        }
    }
}
