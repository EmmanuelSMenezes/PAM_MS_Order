using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class StatusResponse
    {
       public List<Status> Status { get; set; }

    }

    public class Status {
        public Guid Order_status_id { get; set; }
        public string Name { get; set; }

    }
}
