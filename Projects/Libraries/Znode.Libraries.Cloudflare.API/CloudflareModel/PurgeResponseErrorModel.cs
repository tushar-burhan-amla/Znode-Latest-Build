using Newtonsoft.Json;

namespace Znode.Libraries.Cloudflare.API
{
    public class PurgeResponseErrorModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
