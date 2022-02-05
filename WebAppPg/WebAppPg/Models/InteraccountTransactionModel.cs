using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class InteraccountTransaction
    {
        public int id { get; set; }
        public DateTime transaction_date { get; set; }
        public int source_account_id { get; set; }
        public List<Account> sourceAccountList { get; set; }
        public int target_account_id { get; set; }
        public List<Account> targetAccountList { get; set; }
        public Decimal source_amount { get; set; }
        public Decimal target_amount { get; set; }
        public string source_currency_code { get; set; }
        public List<CurrencyCode> sourceCurrCodesList { get; set; }
        public string target_currency_code { get; set; }
        public List<CurrencyCode> targetCurrCodesList { get; set; }
        public int row_version { get; set; }
    }
}
