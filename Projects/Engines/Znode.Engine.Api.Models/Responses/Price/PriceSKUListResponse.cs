using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PriceSKUListResponse : BaseListResponse
    {
        public List<PriceSKUModel> PriceSKUList { get; set; }
    }
}
