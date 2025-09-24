using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Shipping
    {
        public Guid Delivery_option_id { get; set; }
        public decimal? Value { get; set; }
         public bool Shipping_free { get; set; }
        public string Name { get; set; }
    }
}
