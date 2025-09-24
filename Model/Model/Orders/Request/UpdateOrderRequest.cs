using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Domain.Model
{
    public class UpdateOrderRequest
    {
        public Guid Order_id { get; set; }
        public decimal Amount { get; set; }
        public decimal? Change { get; set; }
        public string Observation { get; set; }
        public Guid Address_id { get; set; }
        public Guid Branch_id { get; set; }
        public Guid Consumer_id { get; set; }
        public Guid Shipping_company_id { get; set; }
        public Shipping Shipping_options { get; set; }
        public List<OrderItens> Order_itens { get; set; }
        public List<Payment> Payments { get; set; }
        public ConsumerDetails Address { get; set; }    
        public AuthenticationMethod AuthenticationMethod { get; set; }
        public string Encrypted { get; set; }
        [JsonIgnore]
        public Guid Order_status_id {get; set;}
        public Guid Updated_by {get; set; }
    }
}
