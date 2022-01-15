using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace honey.Areas.Admin.Models
{
    public class Phone
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}
