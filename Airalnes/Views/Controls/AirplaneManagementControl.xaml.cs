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
using Airalnes.Views;
using Airalnes.Views.Controls;
using Airalnes.Models;
using Airalnes.Services;

namespace Airalnes.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для AirplaneManagementControl.xaml
    /// </summary>
    public partial class AirplaneManagementControl : UserControl
    {
        private readonly AirplaneService airplaneService = new AirplaneService();
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
            AirplaneComboBox.ItemsSource = airplaneService.GetAllAirplanes();
        }

        private void LoadAirports()
        {
            var airports = airplaneService.GetAllAirports();
            FromTextBox.ItemsSource = airports;
            ToTextBox.ItemsSource = airports;
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                string userRights = mainWindow.CurrentUserRights;

                if (userRights == "Worker")
                {
                    mainWindow.MainContent.Content = new HistoryFlightsUserControl();
                }
                else if (userRights == "User")
                {
                    mainWindow.MainContent.Content = new AirplaneUsersControl();
                }
            }
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var fromAirport = FromTextBox.SelectedItem as Airport;
            var toAirport = ToTextBox.SelectedItem as Airport;
            var selectedAirplane = AirplaneComboBox.SelectedItem as Airplane;
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

            var formData = new FlightFormData
            {
                From = fromAirport.IATACode,
                To = toAirport.IATACode,
                DepartureDate = DepartureTextBox.Text,
                ArrivalDate = ArrivalTextBox.Text,
                FlightClass = ClassComboBox.Text,
                AirplaneId = selectedAirplane.Id,
                Capacity = selectedAirplane.Capacity,
                FlightNumber = FlightNumberTextBox.Text,
                DepartureTime = DepartureTimePicker.Text,
                ArrivalTime = ArrivalTimePicker.Text
            };

            var service = new FlightService();
            try
            {
                service.CreateFlight(formData);
            }
            catch
            {
                GlobalError.Visibility = Visibility.Visible;
            }
        }
    }
    }


