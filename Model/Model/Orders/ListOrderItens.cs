using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ListOrderItens
    {
        public Guid Order_item_id { get; set; }
        public string Product_name { get; set; }
        public int Quantity { get; set; }
        public decimal Product_value { get; set; }
        public Guid Product_id { get; set; }
        public Guid? Image_default { get; set; }
        public string Url { get; set; }
    }
}
