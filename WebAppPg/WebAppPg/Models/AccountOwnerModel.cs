using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppPg.Models
{
    public class AccountOwner
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }
        public Boolean is_active { get; set; }
        public int row_version { get; set; }
        public int app_user_id { get; set; }
        public string app_user_name { get; set; }
        public List<AppUserList> app_user_list { get; set; }
    }

    public class AppUserList
    {
        public int? id { get; set; }
        public string user_name { get; set; }
    }
}
