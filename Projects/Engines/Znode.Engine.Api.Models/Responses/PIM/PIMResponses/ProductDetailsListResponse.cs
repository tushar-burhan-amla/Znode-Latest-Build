using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductDetailsListResponse : BaseListResponse
    {
        public List<ProductDetailsModel> ProductDetailList { get; set; }

        public List<dynamic> ProductDetailsDynamic { get; set; }
        public List<dynamic> NewAttributeList { get; set; }
        public List<LocaleModel> Locale { get; set; }
        public Dictionary<string, object> AttributeColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
    }
}