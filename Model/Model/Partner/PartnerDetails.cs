using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PartnerDetails
    {
        public Guid User_id { get; set; }
        public Guid Partner_id { get; set; }
        public int Identifier { get; set; }
        public string Legal_name { get; set; }
        public string Fantasy_name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone_number { get; set; }
        public string Branch_id { get; set; }
        public string Branch_name { get; set; }
        public string Phone { get; set; }
    }
}
