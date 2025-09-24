using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PagSeguroCardResponse
    {
        public string Id { get; set; }
        public string Reference_id { get; set; }
        public string Status { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Paid_at { get; set; }
        public string Description { get; set; }
        public Amount Amount { get; set; }
        public PaymentResponse Payment_response { get; set; }
        public PaymentMethod Payment_method { get; set; }
        public List<string> Notification_urls { get; set; }
        public List<Link> Links { get; set; }
        public Splits Splits { get; set; }
    }

    public class Amount
    {
        public int Value { get; set; }
        public string Currency { get; set; }
        public Summary Summary { get; set; }
    }

    public class CardResponse
    {
        public string Number { get; set; }
        public string Exp_month { get; set; }
        public string Exp_year { get; set; }
        public string Security_code { get; set; }
        public Holder Holder { get; set; }
        public string Brand { get; set; }
        public string First_digits { get; set; }
        public string Last_digits { get; set; }
    }

    public class Holder
    {
        public string Name { get; set; }
        public string Tax_id { get; set; }
    }

    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Media { get; set; }
        public string Type { get; set; }
    }


    public class PaymentMethod
    {
        public string Type { get; set; }
        public int Installments { get; set; }
        public bool Capture { get; set; }
        public CardResponse Card { get; set; }
        public string Soft_descriptor { get; set; }
    }

    public class PaymentResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Reference { get; set; }
    }

    public class Summary
    {
        public int Total { get; set; }
        public int Paid { get; set; }
        public int Refunded { get; set; }
    }

    public class Splits
    {
        public string Method { get; set; }
        public List<Receiver> Receivers { get; set; }
    }
    public class Receiver
    {
        public Account Account { get; set; }
        public Amount amount { get; set; }
    }
    public class Account
    {
        public string Id { get; set; }
    }

}
