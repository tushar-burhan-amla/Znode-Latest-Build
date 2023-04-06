using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class StoreLocatorListResponse:BaseListResponse
    {
        public List<StoreLocatorDataModel> StoreLocatorList { get; set; }
    }
}
