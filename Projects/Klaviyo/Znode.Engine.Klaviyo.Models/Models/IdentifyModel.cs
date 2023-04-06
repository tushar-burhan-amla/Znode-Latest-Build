using System.Collections.Generic;

namespace Znode.Engine.klaviyo.Models
{
    public class IdentifyModel : KlaviyoBaseModel
    {
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Image { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public List<string> Consent { get; set; }
    }
}
