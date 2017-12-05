using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhoenixUsers
{
    /// <summary>
    /// Interaction logic for KscDataGrid.xaml
    /// </summary>
    public partial class KscDataGrid : UserControl
    {
        public KscDataGrid()
        {
            InitializeComponent();
        }
        public KscDataGrid(List<KSC> users)
        {
            InitializeComponent();
            if (users == null || users.Count == 0)
                return;
            InitializeDataGrid(users);
        }

        private void InitializeDataGrid(List<KSC> users)
        {
            ObservableCollection<KSC> userCollection = new ObservableCollection<KSC>(users);
            CollectionViewSource viewSource = new CollectionViewSource() { Source = userCollection };
            ICollectionView view = viewSource.View;
            KscGrid.ItemsSource = view;
        }
    }
}
