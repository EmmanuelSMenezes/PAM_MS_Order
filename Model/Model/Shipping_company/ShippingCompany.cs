using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ShippingCompany
    {
        public Guid Shipping_company_id { get; set; }
        public string Company_name { get; set; }
        public string Document { get; set; }
        public Guid Address_id { get; set; }
        public Address Address { get; set; }
    }
}
