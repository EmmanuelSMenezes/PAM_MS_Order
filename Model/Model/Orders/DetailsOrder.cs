using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class DetailsOrder
    {
        public Guid Order_id { get; set; }
        public long Order_number { get; set; }
        public decimal Amount { get; set; }
        public Guid order_status_id { get; set; }
        public string Status_name { get; set; }
        public Consumer Consumer { get; set; }
        public Partner Partner { get; set; }
        public ShippingOptions Shipping_options { get; set; }
        public List<ListOrderItens> Order_itens { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
