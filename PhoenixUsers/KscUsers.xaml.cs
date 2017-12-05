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
    public partial class KscUsers : Window
    {
        public List<KSC> kscUsers;
        public KscUsers(List<KSC> kscUsers)
        {
            InitializeComponent();
            this.kscUsers = kscUsers;
        }

        private void KscUsersWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            KscDataGrid newGrid = new KscDataGrid(kscUsers);
            KscUsersWindow.WholeGrid.Children.Add(newGrid);
        }
    }
}
