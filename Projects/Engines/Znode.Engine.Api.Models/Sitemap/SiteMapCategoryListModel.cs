using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SiteMapCategoryListModel : BaseListModel
    {
        public List<SiteMapCategoryModel> CategoryList { get; set; }
    }
}
