using Newtonsoft.Json;
using System;

namespace Domain.Model.Request
{
    public class ShippingCompanyRequest
    {
        [JsonIgnore]
        public Guid Shipping_company_id { get; set; }
        public string Company_name { get; set; }
        public string Document { get; set; }
        public Address Address { get; set; }
    }
}
