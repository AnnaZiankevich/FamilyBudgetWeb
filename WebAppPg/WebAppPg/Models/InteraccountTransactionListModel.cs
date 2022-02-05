using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class InteraccountTransactionItemList
    {
        public int id { get; set; }
        public DateTime transaction_date { get; set; }
        public int source_account_id { get; set; }
        public string source_account_name { get; set; }
        public int target_account_id { get; set; }
        public string target_account_name { get; set; }
        public Decimal source_amount { get; set; }
        public Decimal target_amount { get; set; }
        public string source_currency_code { get; set; }
        public string target_currency_code { get; set; }
        public int row_version { get; set; }
    }

    public class InteraccountTransactionListModel
    {
        public List<InteraccountTransactionItemList> interaccountTransactionList { get; set; }
    }
}
