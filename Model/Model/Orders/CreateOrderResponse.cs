using Domain.Model.PagSeguro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CreateOrderResponse
    {
        public Order Order { get; set; }
        public Pagseguro Pagseguro { get; set; }
    }
}
