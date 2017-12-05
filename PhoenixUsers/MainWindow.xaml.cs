using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
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
        //ObservableCollection<Position> positions;
        ObservableCollection<User> users;
        CollectionViewSource _itemsSourceList;
        ICollectionView itemsView;
        User selectedUser = null;
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
                            end as varchar) as KSCAccount
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
            UsersTable.Dispatcher.Invoke(() => UsersTable.AutoGeneratingColumn += (s, e) =>
            {
                if(e.Column.Header.ToString() == "ID")
                {
                    e.Column.Visibility = Visibility.Hidden;
                }
            });
            UsersTable.Dispatcher.Invoke(() => UsersTable.ItemsSource = itemsView);
        }
        
        private void FilterUsers(object sender, FilterEventArgs e)
        {
            var user = e.Item as User;
            if (user != null)
            {
                if (ListBranches.Dispatcher.Invoke(() => ListBranches.SelectedItems.Cast<string>().Any(b => b == user.Depo)) &&
                    SearchBox.Dispatcher.Invoke(() => SearchBox.Text != "Search..." ? user.UserName.ToLower().Contains(SearchBox.Text.ToLower()) : true))
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }

        private void InitializePositionsListBox()
        {

        }

        private void InitializeBranchListBox()
        {
            string sql = "select * from Branch";
            branches = new ObservableCollection<Branch>();
            IEnumerable<Branch> br = from branch in db.Database.SqlQuery<Branch>(sql)
                     select branch;
            branches.Clear();
            foreach (Branch b in br)
            {
                branches.Add(b);
            }
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

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            itemsView.Refresh();
            btnFilter.IsEnabled = false;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus();
            itemsView.Refresh();
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, new RoutedEventArgs());
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow selectedRow = sender as DataGridRow;
            selectedUser = selectedRow.DataContext as User;

            string sql = $"select * from KSC where UserID = {selectedUser.ID}";
            List<KSC> kscUsers = db.Database.SqlQuery<KSC>(sql).ToList();
            if (kscUsers == null || kscUsers.Count == 0)
            {
                selectedUser = null;
                return;
            }

            KscUsers newWindow = new KscUsers(kscUsers);
            newWindow.Closed += NewWindow_Closed;
            newWindow.Show();
            
        }

        private void NewWindow_Closed(object sender, EventArgs e)
        {
            MainWindow window = Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow as MainWindow);
            
        }

        private void FormEditUser_Closed(object sender, EventArgs e)
        {
            InitializeDataGrid();
        }

        private void UsersTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if(e.Row.DataContext is User user)
            {
                if(e.EditAction == DataGridEditAction.Commit)
                {
                    if((string)e.Column.Header == "Email")
                    {
                        if(ValidateEmail((e.EditingElement as TextBox).Text))
                        {
                            e.EditingElement.Effect = null;
                        }
                        else
                        {
                            //e.EditingElement.Effect = new DropShadowEffect { BlurRadius = 120, Color = Color.FromArgb(100, 255, 0 , 0), Direction = 0, ShadowDepth = 0 };
                            ShowWarningLabel("Невалиден имейл");
                            e.Cancel = true;
                        }
                        /*
                        try
                        {
                            var mail = new MailAddress((e.EditingElement as TextBox).Text).Address;
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Въведеният email адрес е невалиден!", "Невалиден формат!");
                            return;
                        }//*/
                    }
                    var text = e.EditingElement as TextBox;
                    string t = text.Text;
                }
            }
        }
        private bool ValidateEmail(string address)
        {
            string pattern = @"(?:[A-Za-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+\/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[A-Za-z0-9-]*[A-Za-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
            Regex emailValidation = new Regex(pattern);

            //Match match = emailValidation.Match(address);
            MatchCollection matches = emailValidation.Matches(address);
            if (matches.Count != 1)
                return false;
            if (matches[0].Success)
                return true;
            return false;
        }
        
        private void ShowWarningLabel(string text)
        {
            WarningLabel.Content = text;
            WarningLabel.BringIntoView();
            WarningLabel.Visibility = Visibility.Visible;
            //WarningLabel.Width = 150;
            //WarningLabel.Height = 40;
            WarningLabel.FontSize = 16.0;
            WarningLabel.Background = new SolidColorBrush(Color.FromArgb(75, 170, 0, 0));
            
            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(2),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, WarningLabel);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { WarningLabel.Visibility = Visibility.Collapsed; };
            storyboard.Begin();
        }//*/

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (UsersTable.SelectedItems == null || UsersTable.SelectedItems.Count == 0)
                return;
            UsersTable.IsReadOnly = false;
        }

        private void UsersTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersTable.SelectedItems.Count != 1)
                btnEditUser.IsEnabled = false;
            else
                btnEditUser.IsEnabled = true;
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedValue == null)
                return;
            User selectedUser = dataGrid.SelectedValue as User;
            if (selectedUser.State)
            {

            }
        }

        private void UsersTable_LostFocus(object sender, RoutedEventArgs e)
        {
            UsersTable.SelectedIndex = -1;
        }
    }
}
