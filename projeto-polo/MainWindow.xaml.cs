using projeto_polo.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace projeto_polo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void FromDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker fromDateDatePicker)
            {
                ToDate.DisplayDateStart = fromDateDatePicker.SelectedDate;
                ToDate.DisplayDateEnd = DateTime.Today;
            }
        }

        private void ToDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker toDateDatePicker)
            {
                FromDate.DisplayDateEnd = toDateDatePicker.SelectedDate;
                FromDate.DisplayDateStart = DateTime.Today.AddMonths(-1);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}