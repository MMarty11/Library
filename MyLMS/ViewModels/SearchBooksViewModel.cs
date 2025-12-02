using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Biblioteca.Models;
using MyLMS.Data;
using MyLMS.Utils;

namespace MyLMS.ViewModels
{
    internal class SearchBooksViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public SearchBooksViewModel()
        {
            _context = new LibraryContext();

            Books = new ObservableCollection<Book>();

            SearchCommand = new RelayCommand(SearchBooks);
            LoadAllCommand = new RelayCommand(LoadAllBooks);
            SaveCommand = new RelayCommand(SaveChanges, CanSave);
            DeleteCommand = new RelayCommand(DeleteBook, CanDelete);

            // Carica tutti i libri all'avvio
            LoadAllBooks();
        }

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

        public ObservableCollection<Book> Books { get; }

        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();

                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // --------- Comandi ---------

        public ICommand SearchCommand { get; }
        public ICommand LoadAllCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        // --------- Metodi ---------

        private void LoadAllBooks()
        {
            Books.Clear();

            var query = _context.Books
                                .OrderBy(b => b.Title);

            foreach (var book in query)
                Books.Add(book);
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
        }

        private bool CanSave() => SelectedBook != null;

        private void SaveChanges()
        {
            // I Book in Books sono tracciati da EF, qui basta salvare
            _context.SaveChanges();

            // Aggiorno la lista secondo il filtro corrente
            SearchBooks();
        }

        private bool CanDelete() => SelectedBook != null;

        private void DeleteBook()
        {
            if (SelectedBook == null)
                return;

            _context.Books.Remove(SelectedBook);
            _context.SaveChanges();

            Books.Remove(SelectedBook);
            SelectedBook = null;
        }
    }
}