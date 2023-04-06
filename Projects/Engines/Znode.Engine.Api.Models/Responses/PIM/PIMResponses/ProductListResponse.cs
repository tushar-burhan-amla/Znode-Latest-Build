using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductListResponse : BaseListResponse
    {
        public List<ProductModel> Products { get; set; }
        public List<ProductDetailsModel> ProductDetails { get; set; }
        public PIMAttributeValueListModel AttributeValues { get; set; }
        public PIMProductAttributeValuesListModel ProductAttributeValues { get; set; }
        public List<dynamic> ProductDetailsDynamic { get; set; }
        public List<dynamic> NewAttributeList { get; set; }
        public List<LocaleModel> Locale { get; set; }
        public Dictionary<string, object> AttrubuteColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
    }
}
