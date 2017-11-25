using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixUsers
{
    class User
    {
        [DisplayName("№")]
        public int ID { get; set; }
        [DisplayName("Потребител")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("Активна Директория")]
        public string ActiveDirectory { get; set; }
        [DisplayName("Длъжност")]
        public string Position { get; set; }
        [DisplayName("Склад")]
        public string Depo { get; set; }
        [DisplayName("Фармос Акаунт")]
        public string PharmosUserName { get; set; }
        [DisplayName("Акаунт в UADM")]
        public string UADMUserName { get; set; }
        [DisplayName("GoodsIn")]
        public string GoodsIn { get; set; }
        [DisplayName("Purchase")]
        public string PurchaseAccount { get; set; }
        [DisplayName("Tender")]
        public string TenderAccount { get; set; }
        [DisplayName("Фибра")]
        public string PhibraAccount { get; set; }
        [DisplayName("KSC акаунт")]
        public string KSCAccount { get; set; }
        [DisplayName("Статус")]
        public string State { get; set; }
        [DisplayName("Доп. Информация")]
        public string Description { get; set; }
    }
}
