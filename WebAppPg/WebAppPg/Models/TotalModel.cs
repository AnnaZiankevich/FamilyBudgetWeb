using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class TotalIncome
    {
        public int income_id { get; set; }
        public string income_name { get; set; }
        public DateTime income_date { get; set; }
        public int income_source_id { get; set; }
        public List<IncomeSource> incomeSoursesList { get; set; }      
        public decimal income_amount { get; set; }
        public string income_currency_code { get; set; }
        public List<CurrencyCode> incCurrencyCodesList { get; set; }
        public Boolean is_income_planned { get; set; }
    }

    public class TotalPayments
    {
        public int payment_id { get; set; }
        public string payment_name { get; set; }
        public DateTime payment_date { get; set; }
        public int payment_receiver_id { get; set; }
        public List<PaymentReceiver> paymentReceiverList { get; set; }
        public decimal payment_amount { get; set; }
        public string payment_currency_code { get; set; }
        public List<CurrencyCode> paymCurrencyCodesList { get; set; }
        public Boolean is_payment_planned { get; set; }
    }
}
