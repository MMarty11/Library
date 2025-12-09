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
using MyLMS.MVVM.ViewModels;

namespace MyLMS.MVVM.Views
{
    /// <summary>
    /// Logica di interazione per LoanView.xaml
    /// </summary>
    public partial class LoansView : UserControl
    {
        public LoansView()
        {
            InitializeComponent();
            DataContext = new LoansViewModel();
        }
    }
}
