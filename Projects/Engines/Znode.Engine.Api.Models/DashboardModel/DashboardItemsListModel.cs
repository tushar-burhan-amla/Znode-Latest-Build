using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DashboardItemsListModel : BaseListModel
    {
        public List<DashboardItemsModel> TopItemsList { get; set; }
    }
}
