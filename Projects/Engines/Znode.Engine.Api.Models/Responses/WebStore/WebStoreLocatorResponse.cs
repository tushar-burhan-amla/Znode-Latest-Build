using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreLocatorResponse : BaseResponse
    {
      public List<StoreLocatorDataModel> StoreLocatorList { get; set; }
    }
}
