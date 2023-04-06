using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingServiceCodeListResponse : BaseListResponse
    {
        public List<ShippingServiceCodeModel> ShippingServiceCodes { get; set; }
    }
}
