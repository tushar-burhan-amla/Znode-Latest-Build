using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TierPriceListResponse : BaseListResponse
    {  
        public List<PriceTierModel> TierPriceList { get; set; }
    }
}
