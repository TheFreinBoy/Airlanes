using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Airalnes.Views;
using Airalnes.Views.Controls;
using Airalnes.Services;

namespace Airalnes.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private readonly UserService _userService = new UserService();

        public LoginControl()
        {
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new RegistrationControl();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            UsernameTextBox.BorderBrush = string.IsNullOrEmpty(username) ? Brushes.Red : Brushes.White;
            PasswordBox.BorderBrush = string.IsNullOrEmpty(password) ? Brushes.Red : Brushes.White;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                BlankError.Visibility = Visibility.Visible;
                InvalidError.Visibility = Visibility.Collapsed;
                return;
            }

            BlankError.Visibility = Visibility.Collapsed;          
            string role = _userService.AuthenticateUser(username, password);

            if (role != null)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.CurrentUserRights = role;
                mainWindow.MainContent.Content = new DashboardControl();
                InvalidError.Visibility = Visibility.Collapsed;
            }
            else
            {
                BlankError.Visibility = Visibility.Collapsed;
                InvalidError.Visibility = Visibility.Visible;
            }
        }
    }
}
