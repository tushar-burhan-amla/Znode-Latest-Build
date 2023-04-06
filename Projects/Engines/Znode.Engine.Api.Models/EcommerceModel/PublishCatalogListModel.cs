using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishCatalogListModel : BaseListModel
    {
        public PublishCatalogListModel()
        {
            PublishCatalogs = new List<PublishCatalogModel>();
        }
        public List<PublishCatalogModel> PublishCatalogs { get; set; }
    }
}
