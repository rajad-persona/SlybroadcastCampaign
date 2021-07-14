using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Attribute
    {
        [JsonIgnore()]
        public long ID { get; set; }

        //[JsonIgnore()]
        //public string CustomerCategory { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("customer_category")]
        public string customer_category { get; set; }

        [JsonProperty("customer_source")]
        public string customer_source { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_subscribe")]
        public string EmailSubscribe { get; set; }

        [JsonProperty("push_subscribe")]
        public string PushSubscribe { get; set; }
    }

    public class BrazeUser
    {
        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }
    }


}
