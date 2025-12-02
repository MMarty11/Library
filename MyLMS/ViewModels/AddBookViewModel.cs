using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Biblioteca.Models;
using MyLMS.Data;
using MyLMS.Utils;

namespace MyLMS.ViewModels
{
    internal class AddBookViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public AddBookViewModel()
        {
            _context = new LibraryContext();

            SaveCommand = new RelayCommand(SaveBook, CanSave);
            ClearCommand = new RelayCommand(ClearForm);
        }


        // --------- Proprietà bindate alla View ---------

        private string _title = "";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _author = "";
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _isbn = string.Empty;
        public string Isbn
        {
            get => _isbn;
            set { _isbn = value; OnPropertyChanged(); }
        }

        private string _year = string.Empty; // lo trattiamo come string e lo convertiamo in int nel salvataggio
        public string Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }


        // --------- Comandi ---------

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }


        // --------- Logica ---------

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Author);
        }

        private void SaveBook()
        {
            int parsedYear = 0;
            if (!string.IsNullOrWhiteSpace(Year))
            {
                int.TryParse(Year, out parsedYear);
            }

            var book = new Book
            {
                Title = Title,
                Author = Author,
                Isbn = Isbn,
                Year = parsedYear,
                IsAvailable = true
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            // dopo il salvataggio svuoto il form
            ClearForm();
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Year = string.Empty;
        }
    }
}
