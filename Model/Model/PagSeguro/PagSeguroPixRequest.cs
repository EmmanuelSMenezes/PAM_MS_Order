using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class PagSeguroPixRequest
    {
        public Customer customer { get; set; }
        public string reference_id { get; set; }
        public List<QrCode> qr_codes { get; set; }
        public List<string> notification_urls { get; set; }

    }
    public class Customer
    {
        public string name { get; set; }
        public string email { get; set; }
        public string tax_id { get; set; }
    }
    public class QrCode
    {
        public AmountPix amount { get; set; }
        public string expiration_date { get; set; }
    }
    public class AmountPix
    {
        public string value { get; set; }
    }
}
