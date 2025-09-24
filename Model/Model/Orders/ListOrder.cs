using Domain.Model.PagSeguro;
using System;
using System.Collections.Generic;

namespace Domain.Model
{
  public class ListOrder
    {
        public Guid Order_id { get; set; }
        public Guid? Chat_id { get; set; }
        public long Order_number { get; set; }
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
        public Guid Order_status_id { get; set; }
        public string Status_name { get; set; }
        public Consumer Consumer { get; set; }
        public Partner Partner { get; set; }
        public ShippingOptions Shipping_options { get; set; }
        public List<ListOrderItens> Order_itens { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public decimal Service_fee { get; set; }
        public decimal Card_fee { get; set; }
        public Guid? Payment_options_id { get; set; }
        public string Description { get; set; }
        public Guid? Payment_local_id { get; set; }
        public string Payment_local_name { get; set; }
        public Pagseguro Pagseguro { get; set; }
    }
}
