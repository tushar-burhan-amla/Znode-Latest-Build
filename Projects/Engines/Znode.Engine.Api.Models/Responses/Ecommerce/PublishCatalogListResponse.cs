using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PublishCatalogListResponse : BaseListResponse
    {
        public List<PublishCatalogModel> PublishCatalogs { get; set; }
    }
}
