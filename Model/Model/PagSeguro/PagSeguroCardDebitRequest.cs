using Domain.Model.PagSeguro;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PagSeguroCardDebitRequest
    {
        public string reference_id { get; set; }
        public Customer customer { get; set; }
        public List<ChargesDebit> charges { get; set; }
        public List<Item> items { get; set; }
        public ShippingRequest shipping { get; set; }
    }
    public class PaymentMethodDebitCardRequest
    {
        public string type { get; set; }
        public int installments { get; set; }
        public bool capture { get; set; }
        public CardRequest card { get; set; }
        public string soft_descriptor { get; set; }
        public AuthenticationMethod authentication_method { get; set; }
    }
}

