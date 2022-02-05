using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class BalancesListItem
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
    }

    public class BalancesListModel
    {
        public List<BalancesListItem> balancesList { get; set; }
        public int account_id { get; set; }
        public List<Account> accountList { get; set; }
        public int period_id { get; set; }
        public List<Period> periodList { get; set; }
        public string currency_code { get; set; }
        public List<CurrencyCode> currencyCodelist { get; set; }
    }
}
