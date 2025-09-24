using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Payment
    {
        public Guid? Payment_id { get; set; }
        public Guid Payment_options_id { get; set; }
        public Guid Card_id { get; set; }
        public string Security_code { get; set; }
        public decimal Amount_paid { get; set; }
        public int Installments { get; set; }
    }

    public class Card
    {
        public string Number { get; set; }
        public string Validity { get; set; }
        public string Security_code { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Encrypted { get; set; }
    }

    public class Payment_Options
    {
        public enum Payment_PagSeguro
        {

            CREDIT_CARD,
            DEBIT_CARD,
            PIX
        }

        public enum Payment_PagSeguro_Status
        {
            PENDING = -1,
            CANCELED = 0,
            PAID = 1,
            AUTHORIZED = 2,
            IN_ANALYSIS = 3,
            DECLINED = 4,
            WAITING_PIX = 5
        }

        private static readonly Dictionary<Guid, Payment_PagSeguro> PaymentMappings = new Dictionary<Guid, Payment_PagSeguro>
        {
             { Guid.Parse("68e05062-eb22-42b1-bdba-b0de058de52e"), Payment_PagSeguro.CREDIT_CARD },
             { Guid.Parse("c336dc68-88ba-49c9-a9ca-dcc89952acb6"), Payment_PagSeguro.DEBIT_CARD },
             { Guid.Parse("ec50fa62-d353-4cd9-8fad-b55ed491c2a5"), Payment_PagSeguro.PIX }
         };

        public static Payment_PagSeguro GetValuePaymentPagSeguro(Guid payment_options_id)
        {
            if (PaymentMappings.TryGetValue(payment_options_id, out Payment_PagSeguro payment_PagSeguro))
            {
                return payment_PagSeguro;
            }

            throw new Exception();
        }
    }


}
