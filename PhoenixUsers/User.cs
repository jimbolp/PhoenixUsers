using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixUsers
{
    public class User
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
    }
}
