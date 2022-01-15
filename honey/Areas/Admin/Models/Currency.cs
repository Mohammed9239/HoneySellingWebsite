using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace honey.Areas.Admin.Models
{
    public class Currency
    {
        public Currency()
        {
            Products = new HashSet<Product>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
