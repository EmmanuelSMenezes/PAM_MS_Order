using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class PagSeguroReversal
    {
        public AmountReversal amount { get; set; }
    }
    public class AmountReversal
    {
        public string value { get; set; }
    }
}
