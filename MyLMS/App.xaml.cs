using System.Configuration;
using System.Data;
using System.Windows;
using MyLMS.Data;

namespace MyLMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new LibraryContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }

}
