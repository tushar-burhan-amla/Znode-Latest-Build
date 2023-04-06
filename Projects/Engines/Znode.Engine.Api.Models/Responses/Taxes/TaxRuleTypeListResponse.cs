using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TaxRuleTypeListResponse : BaseListResponse
    {
        public List<TaxRuleTypeModel> TaxRuleTypes { get; set; }
    }
}
