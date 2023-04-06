using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreCategoryListModel : BaseListModel
    {
        public List<WebStoreCategoryModel> Categories { get; set; }

        public WebStoreCategoryListModel()
        {
            Categories = new List<WebStoreCategoryModel>();
        }
    }
}
