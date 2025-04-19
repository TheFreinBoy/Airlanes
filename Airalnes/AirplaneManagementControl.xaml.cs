using System;
using System.Collections.Generic;
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
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (AirplaneComboBox.SelectedItem is Airplane selectedAirplane)
            {
                int airplaneId = selectedAirplane.Id;
                int airplaneCapacity = selectedAirplane.Capacity;
                using (var connection = new SQLiteConnection("Data Source=mydatabase2.db"))
                {
                    connection.Open();
                    string query = "SELECT capacity FROM Airplanes WHERE id = @id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", airplaneId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                airplaneCapacity = reader.GetInt32(0);
                            }
                            else
                            {
                                Console.WriteLine("Не вдалося знайти місткість літака.");
                                return;
                            }
                        }
                    }
                }

                using (var connection = new SQLiteConnection("Data Source=mydatabase2.db"))
                {
                    connection.Open();

                    string query = @"
                    INSERT INTO Flights (
                        from_location, to_location, departure, return_date, class, airplane_id, flight_number, capacity, time_DP, time_AR
                    )
                    VALUES (
                        @from, @to, @departure,  @return, @class, @airplane_id, @flight_number, @capacity, @timeDP, @timeAR
                    );";

                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@from", FromTextBox.Text);
                        cmd.Parameters.AddWithValue("@to", ToTextBox.Text);
                        cmd.Parameters.AddWithValue("@departure", DepartureTextBox.Text);
                        cmd.Parameters.AddWithValue("@return", ArrivalTextBox.Text);
                        cmd.Parameters.AddWithValue("@class", ClassComboBox.Text);
                        cmd.Parameters.AddWithValue("@airplane_id", airplaneId);
                        cmd.Parameters.AddWithValue("@flight_number", FlightNumberTextBox.Text);
                        cmd.Parameters.AddWithValue("@capacity", airplaneCapacity);
                        cmd.Parameters.AddWithValue("@timeDP", DepartureTimePicker.Text);
                        cmd.Parameters.AddWithValue("@timeAR", ArrivalTimePicker.Text);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Рейс успішно створено!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка при створенні рейсу: {ex.Message}");
                        }
                    }
                }
            }           
        }
    }
}
