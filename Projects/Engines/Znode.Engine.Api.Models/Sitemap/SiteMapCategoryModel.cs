using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SiteMapCategoryModel : BaseModel
    {
        public string CategoryName { get; set; }
        public string SEOUrl { get; set; }
        public int ZnodeCategoryId { get; set; }
        public string ZnodeParentCategoryIds { get; set; }
        public List<SiteMapCategoryModel> SubCategoryItems { get; set; }
    }
}
