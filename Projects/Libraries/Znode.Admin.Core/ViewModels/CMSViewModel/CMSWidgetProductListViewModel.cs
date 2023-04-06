using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSWidgetProductListViewModel : BaseViewModel
    {
        public List<CMSWidgetProductViewModel> CMSWidgetProducts { get; set; }
        public List<CMSWidgetProductCategoryModel> CMSWidgetProductCategories { get; set; }
        public GridModel GridModel { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOfMapping { get; set; }
        public int CMSWidgetProductId { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }
    }
}