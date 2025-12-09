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
    internal class BooksViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public BooksViewModel()
        {
            _context = new LibraryContext();

            Books = new ObservableCollection<Book>();

            SearchCommand = new RelayCommand(SearchBooks);
            LoadAllCommand = new RelayCommand(LoadAllBooks);
            NewBookCommand = new RelayCommand(NewBook, CanAdd);
            SaveCommand = new RelayCommand(SaveChanges, CanSave);
            DeleteCommand = new RelayCommand(DeleteBook, CanDelete);

            Message = string.Empty;

            // Carica tutti i libri all'avvio
            LoadAllBooks();
        }


        // --------- Collezioni ---------

        public ObservableCollection<Book> Books { get; }

        // --------- Proprietà bindate alla View ---------

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

        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();

                if (value != null)
                {
                    Title = value.Title;
                    Author = value.Author;
                    Isbn = value.Isbn;
                    Year = value.Year.ToString();
                }

                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _title = "";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                if(SelectedBook != null) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NewBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                if(SelectedBook != null) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NewBookCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _isbn = string.Empty;
        public string Isbn
        {
            get => _isbn;
            set { _isbn = value; OnPropertyChanged(); }
        }

        private string _year = string.Empty;
        public string Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }


        // --------- Comandi ---------

        public ICommand SearchCommand { get; }
        public ICommand LoadAllCommand { get; }
        public ICommand NewBookCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }


        // --------- Metodi ---------

        private void LoadAllBooks()
        {
            Title = string.Empty;
            Author = string.Empty;
            Year = string.Empty;
            Isbn = string.Empty;

            Books.Clear();

            var query = _context.Books
                                .OrderBy(b => b.Title);

            foreach (var book in query)
                Books.Add(book);

            SelectedBook = null;
            Message = string.Empty;
        }

        private void SearchBooks()
        {
            Books.Clear();

            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var text = SearchText.ToLower();

                query = query.Where(b =>
                    ((b.Title ?? string.Empty).ToLower().Contains(text)) ||
                    ((b.Author ?? string.Empty).ToLower().Contains(text)) ||
                    ((b.Isbn ?? string.Empty).ToLower().Contains(text))
                );
            }

            foreach (var book in query.OrderBy(b => b.Title))
                Books.Add(book);

            SelectedBook = null;
            Message = string.Empty;
        }

        private bool CanAdd()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Author);
        }

        private void NewBook()
        {
            ClearEditingFields();
            Message = "Nuovo libro aggiunto con successo!";

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

            // Lo aggiungo solo alla lista in memoria;
            // lo stato "Added" per EF si applica al SaveChanges (facendo Attach/Add).
            _context.Books.Add(book); // EF lo traccia come Added
            Books.Add(book);
            SelectedBook = book;
        }

        private bool CanSave() => SelectedBook != null;
        /*private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Author);
        }*/

        private void SaveChanges()
        {
            if (SelectedBook == null)
                return;

            // Validazione minima
            if (string.IsNullOrWhiteSpace(SelectedBook.Title) ||
                string.IsNullOrWhiteSpace(SelectedBook.Author))
            {
                Message = "Titolo e Autore sono obbligatori.";
                return;
            }

            bool isNew = SelectedBook.Id == 0; // se 0, verrà creato al SaveChanges

            try
            {
                _context.SaveChanges();

                Message = isNew
                    ? "Libro aggiunto correttamente."
                    : "Libro aggiornato correttamente.";

                // ricarico i risultati secondo il filtro corrente
                SearchBooks();
            }
            catch (DbUpdateException ex)
            {
                Message = "Errore nel salvataggio: " + ex.Message;
            }
        }

        private bool CanDelete() => SelectedBook != null;

        private void DeleteBook()
        {
            if (SelectedBook == null)
                return;

            // opzionale ma consigliato: impedire eliminazione se il libro ha prestiti attivi
            bool hasActiveLoans = _context.Loans
                .Any(l => l.BookId == SelectedBook.Id && l.ReturnDate == null);

            if (hasActiveLoans)
            {
                Message = "Impossibile eliminare il libro: ha prestiti attivi.";
                return;
            }

            _context.Books.Remove(SelectedBook);
            _context.SaveChanges();

            Books.Remove(SelectedBook);
            SelectedBook = null;
            Message = "Libro eliminato.";
        }

        private void ClearEditingFields()
        {
            Title = string.Empty;
            Author = string.Empty;
            Isbn = string.Empty;
            Year = string.Empty;
        }
    }
}