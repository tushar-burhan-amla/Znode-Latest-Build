using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PublishProductListResponse : BaseListResponse
    {
        public List<PublishProductModel> PublishProducts { get; set; }
        public List<WebStoreConfigurableProductModel> ConfigurableProducts { get; set; }
    }
}
