using System;
using System.Collections.Generic;

namespace Domain.Model
{
  public class OrderDetails
    {
        public Guid Order_id { get; set; }
        public Guid? Chat_id { get; set; }
        public long Order_number { get; set; }
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
        public Guid Order_status_id { get; set; }
        public string Status_name { get; set; }
        public string? Observation { get; set; }
        public decimal Service_fee { get; set; }
        public decimal Card_fee { get; set; }
        public ConsumerDetails Consumer { get; set; }
        public PartnerDetails Partner { get; set; }
        public ShippingCompany Shipping { get; set; }
        public ShippingOptions Shipping_options { get; set; }
        public List<ListOrderItens> Order_itens { get; set; }
        public List<ListPayment> Payments { get; set; }
        public List<PaymentHistory> Payment_history { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
