using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SiteMapCategoryListResponse : BaseListResponse
    {
        public List<SiteMapCategoryModel> CategoryList { get; set; }
        public List<WebStoreCategoryModel> SubCategoryList { get; set; }
        public List<WebStoreProductModel> ProductList { get; set; }
    }
}
