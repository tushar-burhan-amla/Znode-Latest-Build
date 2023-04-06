using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreCategoryListResponse : BaseListResponse
    {
        public List<WebStoreCategoryModel> Categories { get; set; }
        public List<WebStoreCategoryModel> SubCategories { get; set; }
        public List<WebStoreProductModel> ProductList { get; set; }
    }
}
