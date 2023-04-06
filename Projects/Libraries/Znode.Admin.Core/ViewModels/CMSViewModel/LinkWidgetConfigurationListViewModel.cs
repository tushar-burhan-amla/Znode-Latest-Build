using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class LinkWidgetConfigurationListViewModel : BaseViewModel
    {
        public List<LinkWidgetConfigurationViewModel> LinkWidgetConfigurationList { get; set; }

        public GridModel GridModel { get; set; }

        public int CMSWidgetsId { get; set; }
        public int CMSMappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
    }
}