using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PriceListResponse : BaseListResponse
    {
        public List<PriceModel> PriceList { get; set; }
        public bool HasParentAccounts { get; set; }
        public string CustomerName { get; set; }
    }
}
