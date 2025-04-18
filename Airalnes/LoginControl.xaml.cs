using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Airalnes
{
    /// <summary>
    /// Логика взаимодействия для LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private DatabaseHelper _dbHelper = new DatabaseHelper();

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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                BlankError.Visibility = Visibility.Visible;
                UsernameTextBox.BorderBrush = Brushes.Red;
                PasswordBox.BorderBrush = Brushes.Red;
                return;
            }

            BlankError.Visibility = Visibility.Collapsed;

            using (SQLiteConnection conn = _dbHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE name = @Username AND pass = @Password";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["rights"].ToString();

                                BlankError.Visibility = Visibility.Collapsed;

                                var mainWindow = Application.Current.MainWindow as MainWindow;
                                mainWindow.MainContent.Content = new DashboardControl();
                            }
                            else
                            {
                                InvalidError.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }
    }
}
