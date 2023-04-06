using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TaxRuleListModel : BaseListModel
    {
        public List<TaxRuleModel> TaxRuleList { get; set; }
    }
}
