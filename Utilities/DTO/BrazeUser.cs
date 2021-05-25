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

        [JsonProperty("external_id")]
        public string ExternalId { get { return "wk" + ID.ToString(); } set { } }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class BrazeUser
    {
        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }
    }

}
