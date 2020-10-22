using General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private ObservableCollection<LoginCredentials> updateList;
        private bool editPopup;

        private string sPlace;
        private string sUsername;
        private string sPassword;

        private string mUsername;
        private string mPassword;

        public Popup(bool editPopup, ObservableCollection<LoginCredentials> items, LoginCredentials login, string username, string password)
        {
            InitializeComponent();
            this.editPopup = editPopup;
            this.updateList = items;

            modPlace.Text = login.Password;
            modUsername.Text = login.Username;
            modPassword.Text = login.Password;

            this.sPlace = login.Place;
            this.sUsername = login.Username;
            this.sPassword = login.Password;

            this.mUsername = username;
            this.mPassword = password;

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
                if (modPlace.Text != "" && modUsername.Text != "" && modPassword.Text != "")
                {
                    for (int i = 0; i < updateList.Count - 1; i++)
                    {
                        if (updateList[i].Place == this.sPlace && updateList[i].Username == this.sUsername && updateList[i].Password == this.sPassword)
                        {
                            Client.GetInstance().DeleteRequest(this.mUsername, this.mPassword, new LoginCredentials(this.sPlace, this.sUsername, this.sPassword));
                            updateList[i].Place = modPlace.Text;
                            updateList[i].Username = modUsername.Text;
                            updateList[i].Password = modPassword.Text;
                            Client.GetInstance().AddRequest(this.mUsername, this.mPassword, new LoginCredentials(modPlace.Text, modUsername.Text, modPassword.Text));
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Fill in all empty fields.", "Error");
                }
            }
            else
            {
                if(modPlace.Text != "" && modUsername.Text != "" && modPassword.Text != "")
                {
                    Client.GetInstance().AddRequest(this.mUsername, this.mPassword, new LoginCredentials(modPlace.Text, modUsername.Text, modPassword.Text));
                    updateList.Add(new LoginCredentials(modPlace.Text, modUsername.Text, modPassword.Text));
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Fill in all empty fields.", "Error");
                }
            }
            
        }
    }
}
