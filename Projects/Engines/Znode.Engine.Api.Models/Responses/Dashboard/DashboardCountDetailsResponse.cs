using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DashboardCountDetailsResponse : BaseListResponse
    {
        public List<DashboardTopItemsModel> TopItems { get; set; }
    }
}
