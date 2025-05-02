using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace Airalnes
{
    /// <summary>
    /// Логика взаимодействия для AirplaneManagementControl.xaml
    /// </summary>
    public partial class AirplaneManagementControl : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        public AirplaneManagementControl()
        {
            InitializeComponent();
            LoadAirplanes();
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
        
        private void LoadAirplanes()
        {
            AirplaneComboBox.ItemsSource = dbHelper.GetAirplanes();
        }
        private void LoadAirports()
        {
            var airports = dbHelper.GetAirports();
            FromTextBox.ItemsSource = airports;
            ToTextBox.ItemsSource = airports;
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {           
            FromTextBox.BorderBrush = string.IsNullOrEmpty(FromTextBox.Text) ? Brushes.Red : Brushes.Black;
            ToTextBox.BorderBrush = string.IsNullOrEmpty(ToTextBox.Text) ? Brushes.Red : Brushes.Black;           
            DepartureTextBox.BorderBrush = string.IsNullOrEmpty(DepartureTextBox.Text) ? Brushes.Red : Brushes.Black;
            ArrivalTextBox.BorderBrush = string.IsNullOrEmpty(ArrivalTextBox.Text) ? Brushes.Red : Brushes.Black;
            ClassComboBox.BorderBrush = string.IsNullOrEmpty(ClassComboBox.Text) ? Brushes.Red : Brushes.Black;
            AirplaneComboBox.BorderBrush = string.IsNullOrEmpty(AirplaneComboBox.Text) ? Brushes.Red : Brushes.Black;
            FlightNumberTextBox.BorderBrush = string.IsNullOrEmpty(FlightNumberTextBox.Text) ? Brushes.Red : Brushes.Black;
            DepartureTimePicker.BorderBrush = string.IsNullOrEmpty(DepartureTimePicker.Text) ? Brushes.Red : Brushes.Black;
            ArrivalTimePicker.BorderBrush = string.IsNullOrEmpty(ArrivalTimePicker.Text) ? Brushes.Red : Brushes.Black;
            if (string.IsNullOrEmpty(FromTextBox.Text) || string.IsNullOrEmpty(ToTextBox.Text) || string.IsNullOrEmpty(DepartureTextBox.Text) || string.IsNullOrEmpty(ArrivalTextBox.Text) 
                || string.IsNullOrEmpty(ClassComboBox.Text) || string.IsNullOrEmpty(AirplaneComboBox.Text) || string.IsNullOrEmpty(FlightNumberTextBox.Text) || string.IsNullOrEmpty(DepartureTimePicker.Text) 
                || string.IsNullOrEmpty(ArrivalTimePicker.Text))
            {
                GlobalError.Visibility = Visibility.Visible;
                return;
            }
            GlobalError.Visibility = Visibility.Collapsed;

            if (FromTextBox.SelectedItem is Airport fromAirport &&
                ToTextBox.SelectedItem is Airport toAirport &&
                AirplaneComboBox.SelectedItem is Airplane selectedAirplane)
            {
                try
                {
                    dbHelper.CreateFlight(
                        from: fromAirport.IATACode,
                        to: toAirport.IATACode,
                        departure: DepartureTextBox.Text,
                        returnDate: ArrivalTextBox.Text,
                        flightClass: ClassComboBox.Text,
                        airplaneId: selectedAirplane.Id,
                        flightNumber: FlightNumberTextBox.Text,
                        capacity: selectedAirplane.Capacity,
                        timeDP: DepartureTimePicker.Text,
                        timeAR: ArrivalTimePicker.Text
                    );

                    Console.WriteLine("Рейс успішно створено!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
            else
            {
               
            }
        }
    }
    }


