using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Libraries.Cloudflare.API
{
    public class ZoneList
    {
        [JsonProperty("zone")]
        public List<string> zone { get; set; }
    }
}

