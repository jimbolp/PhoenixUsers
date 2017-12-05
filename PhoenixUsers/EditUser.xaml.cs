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

namespace PhoenixUsers
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class EditUser : Window
    {
        public List<KSC> kscUsers;
        public EditUser(List<KSC> kscUsers)
        {
            InitializeComponent();
            this.kscUsers = kscUsers;
        }

        private void EditUserWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            KscDataGrid newGrid = new KscDataGrid(kscUsers);
            EditUserWindow.WholeGrid.Children.Add(newGrid);
        }
    }
}
