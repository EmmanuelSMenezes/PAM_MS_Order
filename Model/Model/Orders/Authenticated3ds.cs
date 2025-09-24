using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Authenticated3ds
    {
        public string session { get; set; }
        public string expires_at { get; set; }
    }
}
