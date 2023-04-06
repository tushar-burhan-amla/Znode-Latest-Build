using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingPortalListResponse: BaseListResponse
    {
        public List<ShippingPortalModel> ShippingPortalList { get; set; }
    }
}
