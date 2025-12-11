using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MyLMS.Core;
using MyLMS.Data;
using MyLMS.MVVM.Models;

namespace MyLMS.MVVM.ViewModels
{
    internal class UsersViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;

        public UsersViewModel()
        {
            _context = new LibraryContext();

            Users = new ObservableCollection<User>();

            LoadAllCommand = new RelayCommand(LoadAllUsers);
            SearchCommand = new RelayCommand(SearchUsers);
            NewCommand = new RelayCommand(NewUser, CanAdd);
            SaveCommand = new RelayCommand(SaveUser, CanSave);
            DeleteCommand = new RelayCommand(DeleteUser, CanDelete);

            Message = string.Empty;

            LoadAllUsers();
        }

        // --------- Collezione utenti ---------

        public ObservableCollection<User> Users { get; }

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

                if (value != null)
                {
                    FullName = value.FullName;
                    Email = value.Email;
                }
                else
                {
                    ClearEditingFields();
                }

                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                (NewCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                (NewCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
            ClearEditingFields();
            Message = string.Empty;
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
            //_isNewUser = false;
            //ClearEditingFields();
            Message = string.Empty;
        }
        private bool CanAdd()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Email);
        }

        private void NewUser()
        {
            if (!Email.Contains("@"))
            {
                MessageColor = "Red";
                Message = "Email non valida (deve contenere '@')";
                return;
            }
            else
            {
                var user = new User
                {
                    FullName = FullName,
                    Email = Email
                };

                _context.Users.Add(user);
                Users.Add(user);
                SelectedUser = user;

                _context.SaveChanges();

                ClearEditingFields();
                MessageColor = "Green";
                Message = "Nuovo utente aggiunto con successo!";
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Email)
                && (SelectedUser != null);
        }

        private void SaveUser()
        {
            if (SelectedUser == null)
                return;

            // Validazione semplice email
            if (!Email.Contains("@"))
            {
                MessageColor = "Red";
                Message = "Email non valida (deve contenere '@')";
                return;
            }

            try {
                SelectedUser.FullName = FullName;
                SelectedUser.Email = Email;

                _context.SaveChanges();

                MessageColor = "Green";
                Message = "Utente aggiornato correttamente";

                LoadAllUsers();
            }
            catch (DbUpdateException ex)
            {
                MessageColor = "Red";
                Message = "Errore nel salvataggio: " + ex.Message;
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

            // Controllo prestiti attivi prima di eliminare
            bool hasActiveLoans = _context.Loans
                .Any(l => l.UserId == SelectedUser.Id && l.ReturnDate == null);

            if (hasActiveLoans)
            {
                MessageColor = "Red";
                Message = "Impossibile eliminare l'utente: ha prestiti attivi";
                return;
            }

            _context.Users.Remove(SelectedUser);
            _context.SaveChanges();

            Users.Remove(SelectedUser);
            SelectedUser = null;
            ClearEditingFields();

            MessageColor = "Green";
            Message = "Utente eliminato";
        }

        private void ClearEditingFields()
        {
            SearchText = string.Empty;

            FullName = string.Empty;
            Email = string.Empty;
        }
    }
}