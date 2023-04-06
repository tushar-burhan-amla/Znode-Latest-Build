using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Libraries.Cloudflare.API
{
    public class HostListModel
    {
        [JsonProperty("hosts")]
        public List<string> Hosts { get; set; }
    }
}
