using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace honey.Areas.Admin.Models
{
    public class Product
    {
        public Product()
        {
            Phones = new HashSet<Phone>();
            Images = new HashSet<Image>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Seller { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int WhatsappNum { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyID { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<Phone> Phones { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
