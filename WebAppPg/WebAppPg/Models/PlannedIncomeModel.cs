using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PlannedIncome
    {
        public int id { get; set; }
        public string name { get; set; }
        public int account_id { get; set; }
        public List<Account> accountList { get; set; }
        public int income_source_id { get; set; }
        public List<IncomeSourse> incomeSoursesList { get; set; }
        public int income_type_id { get; set; }
        public List<IncomeType> incomeTypesList { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
        public List<CurrencyCode> currencyCodesList { get; set; }
        public DateTime planned_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int row_version { get; set; }
    }
}
