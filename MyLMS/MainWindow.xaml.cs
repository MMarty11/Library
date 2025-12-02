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
using MyLMS.Views;

namespace MyLMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Inizializzo una sola istanza per view (così il loro stato rimane finché l'app è aperta)
        private readonly AddBookView _addBookView = new AddBookView();
        private readonly SearchBooksView _searchBookView = new SearchBooksView();
        private readonly UserView _userView = new UserView();
        private readonly LoanView _loansView = new LoanView();
        private readonly ReturnView _returnView = new ReturnView();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Mostra una certa view al posto del menu
        private void ShowView(FrameworkElement view)
        {
            MainMenuPanel.Visibility = Visibility.Collapsed;
            ContentArea.Visibility = Visibility.Visible;
            ContentArea.Content = view;
        }

        // Metodo pubblico per tornare al menu principale
        public void ShowMainMenu()
        {
            ContentArea.Visibility = Visibility.Collapsed;
            ContentArea.Content = null;
            MainMenuPanel.Visibility = Visibility.Visible;
        }

        // Gestori dei pulsanti del menu

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView(_addBookView);
        }

        private void SearchBookButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView(_searchBookView);
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView(_userView);
        }

        private void LoansButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView(_loansView);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView(_returnView);
        }
    }
}