using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ShippingRuleTypeListModel : BaseListModel
    {
        public List<PIMAttributeDefaultValueModel> ShippingRuleTypeList { get; set; }
    }
}
