using Znode.Libraries.Abstract.Models;

namespace Znode.Engine.klaviyo.Models
{
    public class EmailProviderModel : BaseModel
    {
        public int EmailProviderId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
    }
}
