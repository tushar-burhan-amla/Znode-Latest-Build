using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishCategoryListModel : BaseListModel
    {
        public List<PublishCategoryModel> PublishCategories { get; set; }
    }
}
