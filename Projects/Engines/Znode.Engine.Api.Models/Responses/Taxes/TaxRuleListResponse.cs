using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TaxRuleListResponse : BaseListResponse
    {
        public List<TaxRuleModel> TaxRuleList { get; set; }
    }
}
