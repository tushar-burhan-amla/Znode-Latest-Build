using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CatalogListModel : BaseListModel
    {
        public List<CatalogModel> Catalogs { get; set; }

        public CatalogListModel()
        {
            Catalogs = new List<CatalogModel>();
        }
    }
}
