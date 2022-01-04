using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class AccountListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string account_type_code { get; set; }
        public string account_type_name { get; set; }
        public int account_owner_id { get; set; }
        public string account_owner_name { get; set; }
        public Boolean is_active { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public int row_version { get; set; }

    }
    public class AccountList
    {
        public List<AccountListItem> accountList { get; set; }
        public int accountOwnerId { get; set; }
    }
}
