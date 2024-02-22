using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;
using projeto_polo.Model;

namespace projeto_polo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DatabaseFacade facade = new DatabaseFacade(new DatabaseContext()); 
            facade.EnsureCreated();
        }
    }

}
