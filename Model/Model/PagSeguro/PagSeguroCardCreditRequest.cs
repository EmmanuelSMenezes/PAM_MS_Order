using Domain.Model.PagSeguro;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PagSeguroCardCreditRequest
    {
        public string reference_id { get; set; }
        public Customer customer { get; set; }
        public List<Charges> charges { get; set; }
        public List<Item> items { get; set; }
        public ShippingRequest shipping { get; set; }
    }

    public class Charges
    {
        public string reference_id { get; set; }
        public string description { get; set; }
        public AmountRequest amount { get; set; }
        public PaymentMethodRequest payment_method { get; set; }
        public SplitsRequest splits { get; set; }
    }

    public class ChargesDebit
    {
        public string reference_id { get; set; }
        public string description { get; set; }
        public AmountRequest amount { get; set; }
        public PaymentMethodDebitCardRequest payment_method { get; set; }
    }

    public class AmountRequest
    {
        public int value { get; set; }
        public string currency { get; set; }
    }

    public class CardRequest
    {
        public string encrypted { get; set; }
       // public string security_code { get; set; }
        public bool store { get; set; }
        public HolderRequest holder { get; set; }
    }

    public class HolderRequest
    {
        public string name { get; set; }
       // public string tax_id { get; set; }
    }
    public class PaymentMethodRequest
    {
        public string type { get; set; }
        public int installments { get; set; }
        public bool capture { get; set; }
        public CardRequest card { get; set; }
        public string soft_descriptor { get; set; }
    }

    public class AuthenticationMethod
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class SplitsRequest
    {
        public string method { get; set; }
        public List<ReceiverRequest> receivers { get; set; }
    }
    public class ReceiverRequest
    {
        public AccountRequest account { get; set; }
        public AmountSplit amount { get; set; }
    }
    public class AccountRequest
    {
        public string id { get; set; }
    }

    public class AmountSplit
    {
        public int value { get; set; }
    }

    public class Item
    {
        public string reference_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public int unit_amount { get; set; }
    }
    public class ShippingRequest
    {
        public AddressRequestPagseguro address { get; set; }
    }
    public class AddressRequestPagseguro
    {
        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string locality { get; set; }
        public string city { get; set; }
        public string region_code { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
    }
}
