using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PricePortalListResponse : BaseListResponse
    {
        public List<PricePortalModel> PricePortalList { get; set; }
    }
}
