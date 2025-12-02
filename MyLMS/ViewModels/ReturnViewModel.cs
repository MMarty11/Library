using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
using MyLMS.Data;
using MyLMS.Utils;

namespace MyLMS.ViewModels
{
    internal class ReturnViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public ReturnViewModel()
        {
            _context = new LibraryContext();

            ActiveLoans = new ObservableCollection<Loan>();

            SearchCommand = new RelayCommand(SearchLoans);
            LoadAllCommand = new RelayCommand(LoadAllActiveLoans);
            ReturnCommand = new RelayCommand(ReturnSelectedLoan, CanReturn);

            LoadAllActiveLoans();
        }

        // --------- Proprietà bindate ---------

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

        public ObservableCollection<Loan> ActiveLoans { get; }

        private Loan? _selectedLoan;
        public Loan? SelectedLoan
        {
            get => _selectedLoan;
            set
            {
                _selectedLoan = value;
                OnPropertyChanged();
                (ReturnCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // --------- Comandi ---------

        public ICommand SearchCommand { get; }
        public ICommand LoadAllCommand { get; }
        public ICommand ReturnCommand { get; }

        // --------- Metodi di caricamento ---------

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

        // --------- Restituzione ---------

        private bool CanReturn() => SelectedLoan != null;

        private void ReturnSelectedLoan()
        {
            if (SelectedLoan == null)
                return;

            // Ricarico il prestito dal contesto per sicurezza
            var loan = _context.Loans
                               .Include(l => l.Book)
                               .FirstOrDefault(l => l.Id == SelectedLoan.Id);

            if (loan == null || loan.ReturnDate != null)
                return;

            loan.ReturnDate = System.DateTime.Now;

            if (loan.Book != null)
            {
                loan.Book.IsAvailable = true;
            }

            _context.SaveChanges();

            // Ricarico lista prestiti attivi
            LoadAllActiveLoans();
        }
    }
}
