using System;
using System.Collections.Generic;
using System.Text;
using MyLMS.Core;

namespace MyLMS.MVVM.ViewModels
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand BooksViewCommand { get; set; }
        public RelayCommand UsersViewCommand { get; set; }
        public RelayCommand LoansViewCommand { get; set; }


        public HomeViewModel HomeVM { get; set; }
        public BooksViewModel BooksVM { get; set; }
        public UsersViewModel UsersVM { get; set; }
        public LoansViewModel LoansVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }   
    
        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            BooksVM = new BooksViewModel();
            UsersVM = new UsersViewModel();
            LoansVM = new LoansViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => {
                CurrentView = HomeVM;
            });

            BooksViewCommand = new RelayCommand(o => {
                CurrentView = BooksVM;
            });

            UsersViewCommand = new RelayCommand(o => {
                CurrentView = UsersVM;
            });

            LoansViewCommand = new RelayCommand(o => {
                CurrentView = LoansVM;
            });
        }
    }
}

