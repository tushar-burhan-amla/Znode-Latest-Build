using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSWidgetProductListModel : BaseListModel
    {
        public bool EnableCMSPreview { get; set; }
        public int LocaleId { get; set; }
        public List<CMSWidgetProductModel> CMSWidgetProducts { get; set; }
        public List<CMSWidgetProductCategoryModel> CMSWidgetProductCategories { get; set; }
    }
}
