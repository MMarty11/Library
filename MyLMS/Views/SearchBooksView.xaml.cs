using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyLMS.ViewModels;

namespace MyLMS.Views
{
    /// <summary>
    /// Logica di interazione per SearchBooksView.xaml
    /// </summary>
    public partial class SearchBooksView : UserControl
    {
        public SearchBooksView()
        {
            InitializeComponent();
            DataContext = new SearchBooksViewModel();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Chiama il metodo pubblico della MainWindow
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.ShowMainMenu();
            }
        }
    }
}
