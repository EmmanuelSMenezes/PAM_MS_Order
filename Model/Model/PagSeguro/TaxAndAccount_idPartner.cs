using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class TaxAndAccount_idPartner
    {
        public decimal Tax_admin { get; set; }
        public decimal Tax_partner { get; set; }
        public string Account_id { get; set; }
    }
}
