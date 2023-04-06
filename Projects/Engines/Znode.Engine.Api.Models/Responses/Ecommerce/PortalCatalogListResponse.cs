using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalCatalogListResponse : BaseListResponse
    {
        public List<PortalCatalogModel> PortalCatalogs { get; set; }
    }
}
