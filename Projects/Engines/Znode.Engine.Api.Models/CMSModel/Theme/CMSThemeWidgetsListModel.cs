using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSThemeWidgetsListModel : BaseListModel
    {
        public List<CMSThemeWidgetsModel> CMSThemeWidgets { get; set; }
        public int CMSAreaId { get; set; }
    }
}
