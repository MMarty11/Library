using System;
using System.Collections.Generic;
using System.Text;
using MyLMS.Core;

namespace MyLMS.MVVM.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }

        private int _currentView;

        public int CurrentView
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
            
        }
    }
}

