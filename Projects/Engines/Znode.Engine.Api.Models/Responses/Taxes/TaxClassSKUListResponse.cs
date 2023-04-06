using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TaxClassSKUListResponse : BaseListResponse
    {
        public List<TaxClassSKUModel> TaxClassSKUList { get; set; }
    }
}
