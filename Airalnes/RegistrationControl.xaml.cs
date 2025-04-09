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
    }
}
