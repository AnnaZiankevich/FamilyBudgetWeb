using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class Balances
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
        public List<CurrencyCode> cЫurrencyCodesList { get; set; }
    }
}
