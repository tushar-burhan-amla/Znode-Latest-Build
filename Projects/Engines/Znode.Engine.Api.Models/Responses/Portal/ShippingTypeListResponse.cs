using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingTypeListResponse : BaseListResponse
    {
        public List<ShippingTypeModel> ShippingTypeList { get; set; }
    }
}
