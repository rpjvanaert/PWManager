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

namespace WPF_PWM.XAML_Files
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        //private List<LoginCredentials> items;
        private ObservableCollection<LoginCredentials> items = new ObservableCollection<LoginCredentials>();
        public DataWindow()
        {
            InitializeComponent();
            items.Add(new LoginCredentials("Facebookisawaytoolongappname", "John", "Dough"));
            items.Add(new LoginCredentials("Instagram", "zzzz", "Dough"));
            items.Add(new LoginCredentials("Google", "Jane", "zzzz"));
            DataList.ItemsSource = items;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            DataList.ItemsSource = items;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            Popup popup = new Popup(false, items, "", "", "");
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
    }
}
