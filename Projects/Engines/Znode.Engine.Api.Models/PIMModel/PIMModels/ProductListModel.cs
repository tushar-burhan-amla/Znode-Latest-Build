using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductListModel : BaseListModel
    {
        public List<ProductModel> Products { get; set; }
        public List<PIMAttributeModel> Attributes { get; set; }
        public List<PIMAttributeValueModel> AttributesValues { get; set; }

        public ProductListModel()
        {
            Products = new List<ProductModel>();
            Attributes = new List<PIMAttributeModel>();
            AttributesValues = new List<PIMAttributeValueModel>();
        }
    }
}
