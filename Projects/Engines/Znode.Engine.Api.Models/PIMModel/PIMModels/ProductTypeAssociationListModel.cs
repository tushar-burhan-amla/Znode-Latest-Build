using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductTypeAssociationListModel : BaseListModel
    {
        public ProductTypeAssociationListModel()
        {
            AssociatedProducts = new List<ProductTypeAssociationModel>();
        }
        public List<ProductTypeAssociationModel> AssociatedProducts { get; set; }
    }
}
