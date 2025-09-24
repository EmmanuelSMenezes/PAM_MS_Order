using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Model
{
    public class Filter
    {
        public string? Filters { get; set; }
        public int? Page { get; set; }
        public int? ItensPerPage { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
    }

    public class FilterPartner
    {
        public string? Order_number { get; set; }
        public string? Status { get; set; }
        public string? Consumer { get; set; }
        public string? Filial { get; set; }
        public int? Page { get; set; }
        public int? ItensPerPage { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
    }

    public class FilterAdmin
    {
        public string? Order_number { get; set; }
        public string? Status { get; set; }
        public string? Consumer { get; set; }
        public string? Partner { get; set; }
        public int? Page { get; set; }
        public int? ItensPerPage { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
    }
}
