using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Consumer
    {
        public Guid User_id { get; set; }
        public Guid Consumer_id { get; set; }
        public string Legal_name { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
    }
}
