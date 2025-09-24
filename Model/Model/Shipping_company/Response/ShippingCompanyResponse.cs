using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Response
{
    public class ShippingCompanyResponse
    {
        public ShippingCompany shippingCompany { get; set; }
        public bool created { get; set; }
    }
}
