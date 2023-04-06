using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingRuleTypeListResponse : BaseListResponse
    {
        public List<PIMAttributeDefaultValueModel> ShippingRuleTypeList { get; set; }
    }
}
