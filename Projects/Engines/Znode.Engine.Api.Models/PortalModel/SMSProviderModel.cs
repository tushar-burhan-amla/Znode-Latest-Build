using System;

namespace Znode.Engine.Api.Models
{
    public class SMSProviderModel : BaseModel
    {
        public int SMSProviderId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
    }
}
