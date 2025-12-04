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
using MyLMS.MVVM.Views;

namespace MyLMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Inizializzo una sola istanza per view (così il loro stato rimane finché l'app è aperta)
        private readonly BooksView _searchBookView = new BooksView();
        private readonly UsersView _userView = new UsersView();
        private readonly LoansView _loansView = new LoansView();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Mostra una certa view al posto del menu
        /*private void ShowView(FrameworkElement view)
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
        }*/
    }
}