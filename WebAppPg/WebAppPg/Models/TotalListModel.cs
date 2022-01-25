using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class TotalIncomeModelListItem
    {
        public int income_id { get; set; }
        public string income_name { get; set; }
        public DateTime income_date { get; set; }
        public int income_source_id { get; set; }
        public string income_source_name { get; set; }
        public decimal income_amount { get; set; }
        public string income_currency_code { get; set; }
        public Boolean is_income_planned { get; set; }
    }

    public class TotalPaymentsModelListItem
    {
        public int payment_id { get; set; }
        public string payment_name { get; set; }
        public DateTime payment_date { get; set; }
        public int payment_receiver_id { get; set; }
        public string payment_receiver_name { get; set; }
        public decimal payment_amount { get; set; }
        public string payment_currency_code { get; set; }
        public Boolean is_payment_planned { get; set; }
    }

    public class TotalModelList
    {
        public List<TotalIncomeModelListItem> totalIncomeModelList { get; set; }
        public List<TotalPaymentsModelListItem> totalPaymentsModelList { get; set; }
        public int account_id { get; set; }
        public List<Account> accountList { get; set; }
        public int period_id { get; set; }
        public List<Period> periodList { get; set; }
        public string currency_code { get; set; }
        public List<CurrencyCode> currencyCodelist { get; set; }
    }

    public class Period
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
