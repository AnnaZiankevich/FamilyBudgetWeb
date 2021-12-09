using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class Account
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string account_type_code { get; set; }
        public List<AccountType> accountTypesList { get; set; }
        public int account_owner_id { get; set; }
        //public List<AccountOwner> accountOwnersList { get; set; }
        public Boolean is_active { get; set; }
        public string currency_code { get; set; }
        public List<CurrencyCode> currencyCodesList { get; set; }
        public int row_version { get; set; }
    }

    public class AccountType
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class CurrencyCode
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
