using Newtonsoft.Json;

namespace Znode.Libraries.Cloudflare.API
{
    public class PurgeAllModel
    {
        [JsonProperty("purge_everything")]
        public bool PurgeEverything { get; set; } = true;
    }
}
    