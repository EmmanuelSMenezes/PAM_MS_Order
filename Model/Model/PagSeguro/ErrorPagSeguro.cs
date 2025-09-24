using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.PagSeguro
{
    public class ErrorPagSeguro
    {
        [JsonProperty("error_messages")]
        public ErrorMessage[] ErrorMessages { get; set; }
    }

    public class ErrorMessage
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parameter_name")]
        public string ParameterName { get; set; }
    }
}
