using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class AccountOwnerListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public Boolean is_active { get; set; }
        public int app_user_id { get; set; }
        public string app_user_name { get; set; }
        public int row_version { get; set; }
    }

    public class AccountOwnerList
    {
        public List<AccountOwnerListItem> accountOwnerList { get; set; }
    }
}
