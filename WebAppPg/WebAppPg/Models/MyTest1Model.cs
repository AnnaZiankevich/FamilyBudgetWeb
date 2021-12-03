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
        public Boolean is_active { get; set; }
        public int app_user_id { get; set; }
    }
}
