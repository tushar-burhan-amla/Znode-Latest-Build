using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSTextWidgetConfigurationListModel : BaseListModel
    {
        public List<CMSTextWidgetConfigurationModel> TextWidgetConfigurationList { get; set; }
    }
}
