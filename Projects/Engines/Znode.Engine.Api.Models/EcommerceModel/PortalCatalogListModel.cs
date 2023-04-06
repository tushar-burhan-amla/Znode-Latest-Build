using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalCatalogListModel : BaseListModel
    {
        public PortalCatalogListModel()
        {
            PortalCatalogs = new List<PortalCatalogModel>();
        }
        public List<PortalCatalogModel> PortalCatalogs { get; set; }
    }
}
