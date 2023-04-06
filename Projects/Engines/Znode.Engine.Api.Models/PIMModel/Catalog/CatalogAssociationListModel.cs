
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CatalogAssociationListModel : BaseListModel
    {
        public List<CatalogAssociationModel> CatalogCategories { get; set; }

        public CatalogAssociationListModel()
        {
            CatalogCategories = new List<CatalogAssociationModel>();
        }
    }
}
