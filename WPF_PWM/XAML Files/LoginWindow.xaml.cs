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
using WPF_PWM.Classes;

namespace WPF_PWM.XAML_Files
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class LoginWindow : Window, ILoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
         
        }

        public void Login(bool status)
        {
            if (status)
            {
                DataWindow datawindow = new DataWindow();
                datawindow.Show();
                this.Close();
            }
        }

        public void Message(string message)
        {
            MessageBox.Show(message, "PWManager");
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            string username = usernameTB.Text.ToString();
            string password = passwordTB.Password.ToString();
            DataWindow datawindow = new DataWindow();
            if (username == password) datawindow.Show();
            this.Close();
            
        }
    }
}
