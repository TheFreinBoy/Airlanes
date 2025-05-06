using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Airalnes
{
    /// <summary>
    /// Логика взаимодействия для AirplaneUsersControl.xaml
    /// </summary>
    public partial class AirplaneUsersControl : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        public AirplaneUsersControl()
        {
            InitializeComponent();
            LoadAirports();
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserPopup.IsOpen = !UserPopup.IsOpen;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка");

        }
        private CustomPopupPlacement[] CustomPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(targetSize.Width + 10, -10), PopupPrimaryAxis.Horizontal)
            };
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new LoginControl();
            }

        }
        private void Airplane_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DashboardControl();
            }
        }
        private void LoadAirports()
        {
            var airports = dbHelper.GetAirports();
            FromTextBox.ItemsSource = airports;
            ToTextBox.ItemsSource = airports;
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            FromTextBox.BorderBrush = string.IsNullOrEmpty(FromTextBox.Text) ? Brushes.Red : Brushes.Black;
            ToTextBox.BorderBrush = string.IsNullOrEmpty(ToTextBox.Text) ? Brushes.Red : Brushes.Black;
            PassengersComboBox.BorderBrush = string.IsNullOrEmpty(PassengersComboBox.Text) ? Brushes.Red : Brushes.Black;
            DepartureTextBox.BorderBrush = DepartureTextBox.SelectedDate == null ? Brushes.Red : Brushes.Black;
            ArrivalTextBox.BorderBrush = ArrivalTextBox.SelectedDate == null ? Brushes.Red : Brushes.Black;
            ClassComboBox.BorderBrush = string.IsNullOrEmpty(ClassComboBox.Text) ? Brushes.Red : Brushes.Black;
            if (string.IsNullOrEmpty(FromTextBox.Text) || string.IsNullOrEmpty(ToTextBox.Text) || DepartureTextBox.SelectedDate == null || 
                ArrivalTextBox.SelectedDate == null || string.IsNullOrEmpty(PassengersComboBox.Text) || string.IsNullOrEmpty(ClassComboBox.Text))
            {
                FieldsError.Visibility = Visibility.Visible;
                return;
            }           
            
            FieldsError.Visibility = Visibility.Collapsed;

                string from = FromTextBox.Text;
                string to = ToTextBox.Text;
                string departure = DepartureTextBox.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
                string arrival = ArrivalTextBox.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
                string flightClass = (ClassComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

                int passengers = 1;

            if (PassengersComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int parsed))
            {
                passengers = parsed;
            }

            var results = dbHelper.SearchFlights(from, to, departure, arrival, flightClass, passengers);
                FlightsDataGrid.ItemsSource = results;
            
        }
        private void BookFlight_Click(object sender, RoutedEventArgs e)
        {                    
            var selectedFlight = FlightsDataGrid.SelectedItem as Flight; 
            if (selectedFlight == null)
            {
                BookError.Visibility = Visibility.Visible;
                return;
            }
            BookError.Visibility = Visibility.Collapsed;

            MessageBox.Show("Бронь");
        }

    }
}
