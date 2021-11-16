using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAppPg.Models
{
    public class MyTest1
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [DataType(DataType.Date)]
        public DateTime first_date { get; set; }
        public bool first_date_is_null { get; set; }
    }
}
