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
using MySql.Data.MySqlClient;

namespace Airalnes
{
    /// <summary>
    /// Логика взаимодействия для RegistrationControl.xaml
    /// </summary>
    public partial class RegistrationControl : UserControl
    {
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

            string connectionString = "server=localhost;user=root;password=12345;database=airlines;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM users WHERE {column} = @Value";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    MessageBox.Show("DataBase Error: " + ex.Message);
                }
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = RoleComboBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {             
                GlobalError.Visibility = Visibility.Visible;
                return;
            }
            GlobalError.Visibility = Visibility.Collapsed; 

            string connectionString = "server=localhost;user=root;password=12345;database=airlines;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string checkNameQuery = "SELECT COUNT(*) FROM users WHERE name = @Username";
                    MySqlCommand checkNameCmd = new MySqlCommand(checkNameQuery, conn);
                    checkNameCmd.Parameters.AddWithValue("@Username", username);
                    int nameCount = Convert.ToInt32(checkNameCmd.ExecuteScalar());

                    string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                    MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, conn);
                    checkEmailCmd.Parameters.AddWithValue("@Email", email);
                    int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                    if (nameCount > 0 || emailCount > 0)
                    {
                        if (nameCount > 0)
                        {
                            UsernameError.Visibility = Visibility.Visible;
                            UsernameTextBox.BorderBrush = Brushes.Red;
                        }
                        else
                        {
                            UsernameError.Visibility = Visibility.Collapsed;
                            UsernameTextBox.BorderBrush = Brushes.White;
                        }

                        if (emailCount > 0)
                        {
                            EmailError.Visibility = Visibility.Visible;
                            EmailTextBox.BorderBrush = Brushes.Red;
                        }
                        else
                        {
                            EmailError.Visibility = Visibility.Collapsed;
                            EmailTextBox.BorderBrush = Brushes.White;
                        }

                        return; 
                    }

                    string insertQuery = "INSERT INTO users (name, email, pass, rights) VALUES (@Username, @Email, @Password, @Role)";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
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
                catch (Exception ex)
                {
                    MessageBox.Show("DataBase Error: " + ex.Message);
                }
            }
        }


    }
}
