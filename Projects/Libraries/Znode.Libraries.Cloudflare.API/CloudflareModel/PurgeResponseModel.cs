using Newtonsoft.Json;

namespace Znode.Libraries.Cloudflare.API
{
    public class PurgeResponseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public PurgeResponseErrorModel[] Errors { get; set; }
    }
}
