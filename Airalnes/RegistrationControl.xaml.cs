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

namespace Airalnes
{
    /// <summary>
    /// Логика взаимодействия для RegistrationControl.xaml
    /// </summary>
    public partial class RegistrationControl : UserControl
    {
        private DatabaseHelper _dbHelper = new DatabaseHelper();

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

            using (SQLiteConnection conn = _dbHelper.GetConnection())
            {                
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM users WHERE {column} = @Value";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value", value);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
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
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = RoleComboBox.Text;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            EmailTextBox.BorderBrush = string.IsNullOrEmpty(EmailTextBox.Text) ? Brushes.Red : Brushes.White;
            UsernameTextBox.BorderBrush = string.IsNullOrEmpty(UsernameTextBox.Text) ? Brushes.Red : Brushes.White;
            PasswordBox.BorderBrush = string.IsNullOrEmpty(PasswordBox.Password) ? Brushes.Red : Brushes.White;
            RoleComboBox.BorderBrush = string.IsNullOrEmpty(RoleComboBox.Text) ? Brushes.Red : Brushes.White;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||  string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
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

            using (SQLiteConnection conn = _dbHelper.GetConnection())
            {
                try
                {
                    conn.Open();

                    string checkNameQuery = "SELECT COUNT(*) FROM users WHERE name = @Username";
                    using (SQLiteCommand checkNameCmd = new SQLiteCommand(checkNameQuery, conn))
                    {
                        checkNameCmd.Parameters.AddWithValue("@Username", username);
                        int nameCount = Convert.ToInt32(checkNameCmd.ExecuteScalar());

                        string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                        using (SQLiteCommand checkEmailCmd = new SQLiteCommand(checkEmailQuery, conn))
                        {
                            checkEmailCmd.Parameters.AddWithValue("@Email", email);
                            int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                            if (nameCount > 0 || emailCount > 0)
                            {
                                UsernameError.Visibility = nameCount > 0 ? Visibility.Visible : Visibility.Collapsed;
                                EmailError.Visibility = emailCount > 0 ? Visibility.Visible : Visibility.Collapsed;
                                UsernameTextBox.BorderBrush = nameCount > 0 ? Brushes.Red : Brushes.White;
                                EmailTextBox.BorderBrush = emailCount > 0 ? Brushes.Red : Brushes.White;
                                return;
                            }
                        }
                    }

                    string insertQuery = "INSERT INTO users (name, email, pass, rights) VALUES (@Username, @Email, @Password, @Role)";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", role);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            var mainWindow = Application.Current.MainWindow as MainWindow;
                            mainWindow.MainContent.Content = new LoginControl();
                        }
                        else
                        {
                            RegistrationError.Visibility = Visibility.Visible;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database Error: " + ex.Message);
                }
            }
        }



    }
}
