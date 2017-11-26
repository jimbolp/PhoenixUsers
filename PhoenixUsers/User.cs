using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixUsers
{
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _userName;
        private string _email;
        private string _activeDirectory;
        private string _position;

        [DisplayName("№")]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value )
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        [DisplayName("Потребител")]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }
        [DisplayName("Активна Директория")]
        public string ActiveDirectory
        {
            get
            {
                return _activeDirectory;
            }
            set
            {
                if (_activeDirectory != value)
                {
                    _activeDirectory = value;
                    NotifyPropertyChanged("ActiveDirectory");
                }
            }
        }
        [DisplayName("Длъжност")]
        public string Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    NotifyPropertyChanged("Position");
                }
            }
        }
        [DisplayName("Склад")]
        public string Depo { get; set; }
        [DisplayName("Фармос Акаунт")]
        public string PharmosUserName { get; set; }
        [DisplayName("Акаунт в UADM")]
        public string UADMUserName { get; set; }
        [DisplayName("GoodsIn")]
        public bool GoodsIn { get; set; }
        [DisplayName("Purchase")]
        public bool PurchaseAccount { get; set; }
        [DisplayName("Tender")]
        public bool TenderAccount { get; set; }
        [DisplayName("Фибра")]
        public bool PhibraAccount { get; set; }
        [DisplayName("KSC акаунт")]
        public string KSCAccount { get; set; }
        [DisplayName("Статус")]
        public bool State { get; set; }
        [DisplayName("Доп. Информация")]
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
