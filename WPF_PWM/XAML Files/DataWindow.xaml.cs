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
using General;
using WPF_PWM.Classes;

namespace WPF_PWM.XAML_Files
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DataWindow : Window, IDataWindow
    {
        private string username;
        private string password;
        private ObservableCollection<LoginCredentials> items = new ObservableCollection<LoginCredentials>();

        public DataWindow(string username, string password)
        {
            InitializeComponent();

            //ask data in after login
            items.Add(new LoginCredentials("Facebookisawaytoolongappname", "John", "Dough"));
            items.Add(new LoginCredentials("Instagram", "zzzz", "Dough"));
            items.Add(new LoginCredentials("Google", "Jane", "zzzz"));
            DataList.ItemsSource = items;

            this.username = username;
            this.password = password;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Client.GetInstance().RequestRefresh(username, password);
            DataList.ItemsSource = items;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            Popup popup = new Popup(false, items, new LoginCredentials("", "", ""), this.username, this.password);
            popup.Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            dynamic dyn = DataList.SelectedItem;
                   
            if (!(DataList.SelectedItem == null))
            {
                Popup popup = new Popup(true, items, dyn.Place, dyn.Username, dyn.Password);
                popup.Show();
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if(!(DataList.SelectedItem == null))
            {
                items.Remove((dynamic)DataList.SelectedItem);
            }
            
        }

        public void GiveData(List<LoginCredentials> logins)
        {
            items = new ObservableCollection<LoginCredentials>(logins as List<LoginCredentials>);
            DataList.ItemsSource = items;
        }

        public void Stop()
        {
            this.Close();
        }

        public void Message(string message)
        {
            MessageBox.Show(message, "PWManager");
        }

        public void DeleteData(LoginCredentials login)
        {
            throw new NotImplementedException();
        }
    }
}
