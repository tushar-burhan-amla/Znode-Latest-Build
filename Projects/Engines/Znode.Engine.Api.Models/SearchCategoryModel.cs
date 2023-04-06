using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchCategoryModel : BaseModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public long Count { get; set; }
        public string SEOUrl { get; set; }
        public List<SearchCategoryModel> ParentCategories { get; set; }
    }
}
