using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetCategoryListModel : BaseListModel
    {
        public List<WebStoreWidgetCategoryModel> Categories { get; set; }
    }
}
