using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalPageSettingListModel : BaseListModel
    {
        public List<PortalPageSettingModel> PageSettings { get; set; }
    }
}
