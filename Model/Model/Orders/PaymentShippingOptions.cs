using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PaymentShippingOptions
    {
        public List<PaymentOptions> Payment_options { get; set; }
        public List<ShippingOptions> Shipping_options { get; set; }
    }

    public class PaymentOptions
    {
        public Guid Payment_options_id { get; set; }
        public string Description { get; set; }
        public Guid Payment_local_id { get; set; }
        public string Payment_local_name { get; set; }

    }

    public class ShippingOptions
    {
        public Guid Delivery_option_id { get; set; }
        public decimal Value { get; set; }
        public string Name { get; set; }

    }
}
