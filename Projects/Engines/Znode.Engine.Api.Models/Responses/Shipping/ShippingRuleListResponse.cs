using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingRuleListResponse : BaseListResponse
    {
        public List<ShippingRuleModel> ShippingRuleList { get; set; }
    }
}
