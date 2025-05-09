using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using static MaterialDesignThemes.Wpf.Theme;
using Airalnes.Views;
using Airalnes.Models;
using Airalnes.Services;

namespace Airalnes.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для RegistrationControl.xaml
    /// </summary>
    public partial class RegistrationControl : UserControl
    {
        private readonly UserService _userService = new UserService();
        public RegistrationControl()
        {
            InitializeComponent();
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new LoginControl();
            }
        }
        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckIfExists(EmailTextBox.Text.Trim(), "email", EmailError, EmailTextBox);
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckIfExists(UsernameTextBox.Text.Trim(), "name", UsernameError, UsernameTextBox);
        }
        private void CheckIfExists(string value, string column, TextBlock errorBlock, Control inputControl)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errorBlock.Visibility = Visibility.Collapsed;
                inputControl.ClearValue(BorderBrushProperty);
                return;
            }

            bool exists = _userService.IsUserExists(column, value);
            if (exists)
            {
                errorBlock.Visibility = Visibility.Visible;
                inputControl.BorderBrush = Brushes.Red;
            }
            else
            {
                errorBlock.Visibility = Visibility.Collapsed;
                inputControl.BorderBrush = Brushes.White;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = RoleComboBox.Text;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            EmailTextBox.BorderBrush = string.IsNullOrEmpty(email) ? Brushes.Red : Brushes.White;
            UsernameTextBox.BorderBrush = string.IsNullOrEmpty(username) ? Brushes.Red : Brushes.White;
            PasswordBox.BorderBrush = string.IsNullOrEmpty(password) ? Brushes.Red : Brushes.White;
            RoleComboBox.BorderBrush = string.IsNullOrEmpty(role) ? Brushes.Red : Brushes.White;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                GlobalError.Visibility = Visibility.Visible;
                return;
            }

            GlobalError.Visibility = Visibility.Collapsed;

            if (!Regex.IsMatch(email, emailPattern))
            {
                EmailError.Text = "Please enter a valid email address.";
                EmailError.Visibility = Visibility.Visible;
                return;
            }

            EmailError.Visibility = Visibility.Collapsed;

            bool nameExists = _userService.IsUserExists("name", username);
            bool emailExists = _userService.IsUserExists("email", email);

            if (nameExists || emailExists)
            {
                UsernameError.Visibility = nameExists ? Visibility.Visible : Visibility.Collapsed;
                EmailError.Visibility = emailExists ? Visibility.Visible : Visibility.Collapsed;
                UsernameTextBox.BorderBrush = nameExists ? Brushes.Red : Brushes.White;
                EmailTextBox.BorderBrush = emailExists ? Brushes.Red : Brushes.White;
                return;
            }

            User newUser = new User
            {
                Username = username,
                Email = email,
                Password = password,
                Role = role
            };

            bool isRegistered = _userService.RegisterUser(newUser);

            if (isRegistered)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainContent.Content = new LoginControl();
                }
            }
            else
            {
                RegistrationError.Visibility = Visibility.Visible;
            }
        }

    }
}
