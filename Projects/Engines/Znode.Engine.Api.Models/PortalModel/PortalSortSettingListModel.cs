using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalSortSettingListModel : BaseListModel
    {
        public List<PortalSortSettingModel> SortSettings { get; set; }
    }
}
