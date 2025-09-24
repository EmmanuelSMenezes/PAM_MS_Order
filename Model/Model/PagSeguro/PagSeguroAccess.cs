using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class PagSeguroAccess
    {
        public string Token { get; set; }
        public string Url { get; set; }
        public string Method_Split { get; set; }
        public string Account_Id { get;set; }
    }
}
