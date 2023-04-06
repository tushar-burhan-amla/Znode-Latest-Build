using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TaxRuleTypeListModel : BaseListModel
    {
        public TaxRuleTypeListModel()
        {
            TaxRuleTypes = new List<TaxRuleTypeModel>();
        }

        public List<TaxRuleTypeModel> TaxRuleTypes { get; set; }
    }
}
