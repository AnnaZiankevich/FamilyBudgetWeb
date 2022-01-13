using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PaymentListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int account_id { get; set; }
        public string account_name { get; set; }
        public int planned_payment_id { get; set; }
        public string planned_payment_name { get; set; }
        public int payment_receiver_id { get; set; }
        public string payment_receiver_name { get; set; }
        public int payment_type_id { get; set; }
        public string payment_type_name { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
        public DateTime payment_date { get; set; }
        public int row_version { get; set; }
    }

    public class PaymentList
    {
        public List<PaymentListItem> paymentList { get; set; }
    }
}
