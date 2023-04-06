using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DashboardTopItemsListModel : BaseListModel
    {
        public List<DashboardTopItemsModel> TopItemsList { get; set; }
    }
}
