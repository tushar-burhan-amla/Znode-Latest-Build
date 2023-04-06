using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ShippingRuleListModel: BaseListModel
    {
        public List<ShippingRuleModel> ShippingRuleList { get; set; }
    }
}
