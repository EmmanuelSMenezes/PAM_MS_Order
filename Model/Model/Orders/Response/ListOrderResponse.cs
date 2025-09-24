using System.Collections.Generic;

namespace Domain.Model.Response
{
    public class ListOrderResponse
    {
        public List<ListOrder> Orders { get; set; }
        public Pagination Pagination { get; set; }
    }
}
