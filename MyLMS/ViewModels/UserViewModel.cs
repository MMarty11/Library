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
    internal class UserViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;
        private bool _isNewUser = false;

        public UserViewModel()
        {
            _context = new LibraryContext();

            Users = new ObservableCollection<User>();

            LoadAllCommand = new RelayCommand(LoadAllUsers);
            SearchCommand = new RelayCommand(SearchUsers);
            NewCommand = new RelayCommand(NewUser);
            SaveCommand = new RelayCommand(SaveUser, CanSave);
            DeleteCommand = new RelayCommand(DeleteUser, CanDelete);

            LoadAllUsers();
        }

        // --------- Collezione utenti ---------

        public ObservableCollection<User> Users { get; }

        // --------- Ricerca ---------

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

        // --------- Utente selezionato dalla lista ---------

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();

                if (_selectedUser != null)
                {
                    _isNewUser = false;
                    FullName = _selectedUser.FullName;
                    Email = _selectedUser.Email;
                }
                else
                {
                    ClearEditingFields();
                }

                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // --------- Campi di editing (form in basso) ---------

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // --------- Comandi ---------

        public ICommand LoadAllCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        // --------- Metodi ---------

        private void LoadAllUsers()
        {
            Users.Clear();

            var query = _context.Users
                                .OrderBy(u => u.FullName);

            foreach (var u in query)
                Users.Add(u);

            // reset form
            SelectedUser = null;
            _isNewUser = false;
            ClearEditingFields();
        }

        private void SearchUsers()
        {
            Users.Clear();

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var text = SearchText.ToLower();

                query = query.Where(u =>
                    (u.FullName ?? string.Empty).ToLower().Contains(text) ||
                    (u.Email ?? string.Empty).ToLower().Contains(text)
                );
            }

            foreach (var u in query.OrderBy(u => u.FullName))
                Users.Add(u);

            SelectedUser = null;
            _isNewUser = false;
            ClearEditingFields();
        }

        private void NewUser()
        {
            // Modalità "nuovo utente"
            SelectedUser = null;
            _isNewUser = true;
            ClearEditingFields();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Email);
        }

        private void SaveUser()
        {
            if (_isNewUser || SelectedUser == null)
            {
                // Creazione nuovo utente
                var user = new User
                {
                    FullName = FullName,
                    Email = Email
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                Users.Add(user);
                SelectedUser = user;
                _isNewUser = false;
            }
            else
            {
                // Modifica utente esistente
                SelectedUser.FullName = FullName;
                SelectedUser.Email = Email;

                _context.SaveChanges();
            }
        }

        private bool CanDelete()
        {
            return SelectedUser != null;
        }

        private void DeleteUser()
        {
            if (SelectedUser == null)
                return;

            // Attenzione: se l'utente ha prestiti collegati,
            // potresti dover gestire il vincolo di FK (Loan.UserId).
            _context.Users.Remove(SelectedUser);
            _context.SaveChanges();

            Users.Remove(SelectedUser);
            SelectedUser = null;
            _isNewUser = false;
            ClearEditingFields();
        }

        private void ClearEditingFields()
        {
            FullName = string.Empty;
            Email = string.Empty;
        }
    }
}