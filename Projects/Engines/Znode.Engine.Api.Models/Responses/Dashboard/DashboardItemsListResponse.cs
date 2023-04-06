using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DashboardItemsListResponse : BaseListResponse
    {
        public List<DashboardItemsModel> TopItems { get; set; }
    }
}
