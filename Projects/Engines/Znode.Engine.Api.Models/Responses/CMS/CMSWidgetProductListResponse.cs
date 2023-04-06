using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSWidgetProductListResponse : BaseListResponse
    {
        public List<CMSWidgetProductModel> CMSWidgetProducts { get; set; }
        public List<CMSWidgetProductCategoryModel> CMSWidgetProductCategories { get; set; }
    }
}
