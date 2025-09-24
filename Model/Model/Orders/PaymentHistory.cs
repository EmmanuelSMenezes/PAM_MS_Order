using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PaymentHistory
    {
        public string Id { get; set; }
        public DateTime? Created_at_payment { get; set; }
        public int Status_payment { get; set; }
        public string Status_payment_name { get; set; }
    }
}
