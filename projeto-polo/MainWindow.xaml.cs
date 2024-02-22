using projeto_polo.ViewModel;
using System.Windows;

namespace projeto_polo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;

            FromDate.DisplayDateStart = DateTime.Today.AddMonths(-1);
            FromDate.DisplayDateEnd = DateTime.Today;

            ToDate.DisplayDateStart = DateTime.Today.AddMonths(-1);
            ToDate.DisplayDateEnd = DateTime.Today;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}