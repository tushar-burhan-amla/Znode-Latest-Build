using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetProductListModel : BaseListModel
    {
        public List<WebStoreWidgetProductModel> Products { get; set; }
        public string DisplayName { get; set; }
    }
}
