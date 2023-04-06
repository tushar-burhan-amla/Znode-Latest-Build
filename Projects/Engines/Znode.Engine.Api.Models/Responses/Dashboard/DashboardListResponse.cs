using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DashboardListResponse : BaseListResponse
    {
        public List<DashboardTopItemsModel> TopItems { get; set; }
    }
}
