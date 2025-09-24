using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model.PagSeguro;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class Order
    {
        public Guid Order_id { get; set; }
        public List<OrderItens> Order_itens { get; set; }
        public long Order_number { get; set; }
        public decimal Freight{ get; set; }
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
        public decimal Service_fee { get; set; }
        public decimal Card_fee { get; set; }
        public Guid Address_id { get; set; }
        public Guid Branch_id { get; set; }
        [JsonIgnore]
        public Guid Partner_id { get; set; }
        public Guid? Shipping_company_id { get; set; }
        public Guid Order_status_id { get; set; }
        public string? Observation { get; set; }
        public List<Payment> Payments { get; set; }
        public Guid? Chat_id { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
        public Pagseguro Pagseguro { get; set; }
    }

}
