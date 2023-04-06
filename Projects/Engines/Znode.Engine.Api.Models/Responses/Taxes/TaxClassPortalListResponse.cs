using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TaxClassPortalListResponse : BaseListResponse
    {
        public List<TaxClassPortalModel> TaxClassPortalList { get; set; }
    }
}
