using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class PagSeguroPixResponse
    {
        public string id { get; set; }
        public string reference_id { get; set; }
        public DateTime created_at { get; set; }
        public Customer customer { get; set; }
        public List<QrCodeResponse> qr_codes { get; set; }
        public List<string> notification_urls { get; set; }
        public List<Link> links { get; set; }

    }

    public class QrCodeResponse
    {
        public string id { get; set; }
        public DateTime expiration_date { get; set; }
        public AmountPix amount { get; set; }
        public string text { get; set; }
        public List<string> arrangements { get; set; }
        public List<Link> links { get; set; }
    }
}
