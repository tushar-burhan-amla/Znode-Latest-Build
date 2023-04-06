using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSWidgetConfigurationListModel:BaseListModel
    {
        public List<CMSWidgetConfigurationModel> WidgetConfigurationList { get; set; }
    }
}
