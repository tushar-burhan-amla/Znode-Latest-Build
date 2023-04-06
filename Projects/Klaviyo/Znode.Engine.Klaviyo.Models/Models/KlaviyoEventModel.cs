using Newtonsoft.Json;

namespace Znode.Libraries.Klaviyo.Model
{
    public class KlaviyoEventModel
    {
        public string token { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        public CustomerProperty customer_properties { get; set; }
        public dynamic properties { get; set; }
    }
    public class CustomerProperty
    {
        public string email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
