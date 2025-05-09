using System;
using System.Collections.Generic;
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
using Airalnes.Views;
using Airalnes.Views.Controls;
using Airalnes.Helpers;

namespace Airalnes.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для HistoryFlightsUserControl.xaml
    /// </summary>
    public partial class HistoryFlightsUserControl : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public HistoryFlightsUserControl()
        {
            InitializeComponent();
            LoadAllFlights();
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
                string userRights = mainWindow.CurrentUserRights;

                if (userRights == "Worker")
                {
                    mainWindow.MainContent.Content = new AirplaneManagementControl();
                }
                else if (userRights == "User")
                {
                    mainWindow.MainContent.Content = new AirplaneUsersControl();
                }
            }
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new DashboardControl();
            }
        }

        private void LoadAllFlights()
        {
            var allFlights = dbHelper.SearchFlights("", "", "", "", "", 0);
            FlightsDataGrid.ItemsSource = allFlights;
        }
    }
}
