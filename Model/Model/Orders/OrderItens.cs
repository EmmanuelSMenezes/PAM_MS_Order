using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class OrderItens
    {

        public Guid Product_id { get; set; }
        public string Product_name { get; set; }
        public int Quantity { get; set; }
        public decimal Product_value { get; set; }
    }
}
