using General;
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

namespace WPF_PWM.XAML_Files
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private List<LoginCredentials> updateList;
        private bool editPopup;

        public Popup(bool editPopup, List<LoginCredentials> items, string place, string username, string password)
        {
            InitializeComponent();
            this.editPopup = editPopup;
            this.updateList = items;
            modPlace.Text = place;
            modUsername.Text = username;
            modPassword.Text = password;
            if (this.editPopup)
            {
                Title = "Edit Login Credential";
            }
            else
            {
                Title = "Add Login Credential";
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (this.editPopup)
            {

            }
            else
            {
                updateList.Add(new LoginCredentials(modPlace.Text, modUsername.Text, modPassword.Text));
            }
        }
    }
}
