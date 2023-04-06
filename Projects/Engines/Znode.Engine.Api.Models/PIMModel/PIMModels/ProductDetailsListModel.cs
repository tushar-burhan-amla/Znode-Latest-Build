using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductDetailsListModel : BaseListModel
    {
        public List<ProductDetailsModel> ProductDetailList { get; set; }

        public List<dynamic> ProductDetailListDynamic { get; set; }
        public List<dynamic> NewAttributeList { get; set; }
        public List<LocaleModel> Locale { get; set; }
        public Dictionary<string, object> AttributeColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
        public int? DisplayOrder { get; set; }
        public ProductDetailsListModel()
        {
            ProductDetailList = new List<ProductDetailsModel>();
        }
    }
}
