using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ListPayment
    {
        public string Payment_id { get; set; }
        public string Payment_options_id { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
        public int Installments { get; set; }
        public decimal Amount_paid { get; set; }
        public Guid Payment_situation_id { get; set; }
        public string Description { get; set; }
        public int Identifier { get; set; }
        public Guid Payment_local_id { get; set; }
        public string Payment_local_name { get; set; }
    }
}
