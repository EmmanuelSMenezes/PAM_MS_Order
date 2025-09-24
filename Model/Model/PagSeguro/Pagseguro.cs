using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class Pagseguro
    {
        public string SucessPayment { get; set; }
        public ErrorPagSeguro ErrorPayment { get; set; }
    }
}
