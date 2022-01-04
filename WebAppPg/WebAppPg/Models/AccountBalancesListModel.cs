using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class AccountBalanceListItem
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public string account_name { get; set; }
        public int period_id { get; set; }
        public string period_name { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
    }

    public class AccountBalancesList
    {
        public List<AccountBalanceListItem> accountBalancesList { get; set; }
        public int account_id { get; set; }
    }
}
