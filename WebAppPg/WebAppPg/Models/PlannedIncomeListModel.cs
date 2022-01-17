using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PlannedIncomeListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int account_id { get; set; }
        public string account_name { get; set; }
        public int income_source_id { get; set; }
        public string income_source_name { get; set; }
        public int income_type_id { get; set; }
        public string income_type_name { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
        public DateTime? planned_date { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int row_version { get; set; }
    }

    public class PlannedIncomeList
    {
        public List<PlannedIncomeListItem> plannedIncomeList { get; set; }
    }
}

