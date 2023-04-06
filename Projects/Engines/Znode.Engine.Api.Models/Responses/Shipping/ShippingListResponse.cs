using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingListResponse : BaseListResponse
    {
        public List<ShippingModel> ShippingList { get; set; }
        public string ProfileName { get; set; }
        public string PortalName { get; set; }
    }
}
