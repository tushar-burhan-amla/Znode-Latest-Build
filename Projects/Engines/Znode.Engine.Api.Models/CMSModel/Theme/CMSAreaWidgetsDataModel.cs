using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSAreaWidgetsDataModel : BaseModel
    {
        public List<CMSAreaModel> CMSAreaList { get; set; }
        public List<CMSWidgetsModel> AvailableWidgets { get; set; }
        public List<CMSWidgetsModel> SelectedWidgets { get; set; }
        public string[] WidgetIds { get; set; }
    }
}
