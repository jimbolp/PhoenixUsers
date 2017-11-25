using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserDBEntities db;
        ObservableCollection<Branch> branches;
        ObservableCollection<Position> positions;
        ObservableCollection<UserMasterData> users;
        public MainWindow()
        {
            try
            {
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                db = new UserDBEntities();
                InitializeComponent();
                InitializeBranchListBox();
                InitializePositionsListBox();
                InitializeDataGrid();
            }
            catch (EntityException)
            {
                MessageBox.Show("There was a problem connecting to the database!");
                Application.Current.Shutdown(-1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Current.Shutdown(-1);
            }
        }

        private void InitializeDataGrid()
        {
            string sql = "select * from UserMasterData";
            users = new ObservableCollection<UserMasterData>(db.Database.SqlQuery<UserMasterData>(sql).ToList());
            
            UsersTable.ItemsSource = users;
        }

        private void InitializePositionsListBox()
        {

        }

        private void InitializeBranchListBox()
        {
            string sql = "select * from Branch";
            branches = new ObservableCollection<Branch>();
            var br = from branch in db.Database.SqlQuery<Branch>(sql)
                     select branch;
            branches.Clear();
            foreach (Branch b in br)
            {
                branches.Add(b);
            }
            //ListBranches.DataContext = branches;
            ListBranches.ItemsSource = from branch in branches
                                       select branch.BranchName;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox == null)
                return;
            if (string.IsNullOrEmpty(SearchBox.Text.Trim()) || string.IsNullOrWhiteSpace(SearchBox.Text.Trim()))
            {
                SearchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB6B3B3"));
                SearchBox.Text = "Search";
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(SearchBox.Text.Trim() == "Search")
            {
                SearchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                SearchBox.Text = "";
            }
        }
    }
}
