using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalProfileShippingListResponse : BaseListResponse
    {
        public List<PortalProfileShippingModel> Shippings { get; set; }
    }
}
