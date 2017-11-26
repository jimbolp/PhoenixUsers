using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
        ObservableCollection<User> users;
        CollectionViewSource _itemsSourceList;
        ICollectionView itemsView;
        public MainWindow()
        {
            try
            {
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                db = new UserDBEntities();
                InitializeComponent();
                InitializeBranchListBox();
                InitializePositionsListBox();
                Thread t = new Thread(InitializeDataGrid);
                t.Start();
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
            string where = "";
            //string[] BranchName = null;
            //if(FilterBranches(out BranchName))
            //{
            //    where = "";
            //    if (BranchName != null && BranchName.Length != 0)
            //    {
            //        where = " where ";
            //        for (int i = 0; i < BranchName.Length; ++i)
            //        {
            //            if (i != BranchName.Length - 1)
            //            {
            //                where += " b.BranchName = \'" + BranchName[i] + "\' OR ";
            //            }
            //            else
            //            {
            //                where += " b.BranchName = \'" + BranchName[i] + "\'";
            //            }
            //        }
            //    }
            //}
            string sqlQuery = @"select
                            umd.ID
                            ,umd.UserName
                            ,umd.Email 
                            ,ad.ADName as ActiveDirectory
                            ,p.Position
                            ,b.BranchName as Depo
                            ,umd.PharmosUserName
                            ,umd.UADMUserName 
                            ,CAST(Case when k.UserName is NULL
                            then 'Не'
                            else 'Да'
                            end as nvarchar) as KSC
                            ,umd.GI as GoodsIn  
                            ,umd.Purchase as PurchaseAccount
                            ,umd.Tender as TenderAccount 
                            ,umd.Phibra as PhibraAccount
                            ,umd.Active as State 
                            ,umd.Description 
                            from UserMasterData umd
                            left join ADUsers ad on ad.UserID = umd.ID
                            left join Positions p on p.ID = umd.PositionID
                            left join Branch b on b.ID = umd.BranchID
                            left join KSC k on k.UserID = umd.ID";

            string groupby = @" group by umd.ID
                            ,umd.UserName
                            ,umd.Email 
                            ,ad.ADName
                            ,p.Position
                            ,b.BranchName
                            ,umd.PharmosUserName
                            ,umd.UADMUserName 
                            ,umd.GI
                            ,umd.Purchase
                            ,umd.Tender
                            ,umd.Phibra
                            ,umd.Active
                            ,umd.Description
                            ,k.UserName";
            string orderby = " order by umd.UserName";
            string WholeQuery = sqlQuery + where + groupby + orderby;
            
            users = new ObservableCollection<User>(db.Database.SqlQuery<User>(WholeQuery));
            _itemsSourceList = new CollectionViewSource() { Source = users };
            itemsView = _itemsSourceList.View;
            _itemsSourceList.Filter += new FilterEventHandler(FilterUsers);
            UsersTable.Dispatcher.Invoke(() => UsersTable.ItemsSource = itemsView);
        }
        

        private void FilterUsers(object sender, FilterEventArgs e)
        {
            var user = e.Item as User;
            if (user != null)
            {
                if (ListBranches.Dispatcher.Invoke(() => ListBranches.SelectedItems.Cast<string>().Any(b => b == user.Depo)) &&
                    SearchBox.Dispatcher.Invoke(() => SearchBox.Text != "Search..."? user.UserName.Contains(SearchBox.Text) : true))
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }
        //private bool FilterBranches(out string[] branchName)
        //{
        //    try
        //    {
        //        if (ListBranches.Dispatcher.Thread != Thread.CurrentThread)
        //        {
        //            branchName = ListBranches.Dispatcher.Invoke(() => ListBranches.SelectedItems.Cast<string>().ToArray());
        //            return true;
        //        }
        //        else
        //        {
        //            branchName = ListBranches.SelectedItems.Cast<string>().ToArray();
        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        branchName = null;
        //        return false;
        //    }
            
        //}

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
            ListBranches.SelectAll();
        }
        

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox == null)
                return;
            if (string.IsNullOrEmpty(SearchBox.Text.Trim()) || string.IsNullOrWhiteSpace(SearchBox.Text.Trim()))
            {
                SearchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB6B3B3"));
                SearchBox.Text = "Search...";
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(SearchBox.Text.Trim() == "Search...")
            {
                SearchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                SearchBox.Text = "";
            }
        }

        private void ListBranches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(btnFilter != null && !btnFilter.IsEnabled)
            {
                btnFilter.IsEnabled = true;
            }            
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            //Thread t = new Thread(InitializeDataGrid);
            //t.Start();
            itemsView.Refresh();
            btnFilter.IsEnabled = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus();
            itemsView.Refresh();
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnSearch_Click(sender, new RoutedEventArgs());
            }
        }
    }
}
