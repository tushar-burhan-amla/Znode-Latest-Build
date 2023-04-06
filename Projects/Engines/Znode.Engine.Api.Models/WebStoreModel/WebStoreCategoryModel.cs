using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreCategoryModel : PublishCategoryModel
    {
        public WebStoreSEOModel SEODetails { get; set; }
        public List<WebStoreCategoryModel> SubCategories { get; set; }

        public WebStoreCategoryModel()
        {
            SubCategories = new List<WebStoreCategoryModel>();
            SEODetails = new WebStoreSEOModel();
        }
    }
}
