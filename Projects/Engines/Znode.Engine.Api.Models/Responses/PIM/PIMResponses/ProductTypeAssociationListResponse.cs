using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductTypeAssociationListResponse : BaseListResponse
    {
        public List<ProductTypeAssociationModel> AssociatedProductList { get; set; }
    }
}
