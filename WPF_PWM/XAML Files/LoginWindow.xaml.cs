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
    public partial class LoginWindow : Window, ILoginWindow
    {
        private string username;
        private string password;
        public LoginWindow()
        {
            InitializeComponent();
            Client.GetInstance();
            Client.GetInstance().SetLoginWindow(this);
        }

        public void Login(bool status)
        {
            if (status)
            {
                Client.GetInstance().SetDataWindow(username, password);
            }
        }

        public void Message(string message)
        {
            MessageBox.Show(message, "PWManager");
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            this.username = usernameTB.Text.ToString();
            this.password = passwordTB.Password.ToString();
            
            Client.GetInstance().TryLogin(username, password);
        }
    }
}
