using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportOrderStatusListResponse: BaseListResponse
    {
        public List<ReportOrderStatusModel> OrderStatusList { get; set; }
    }
}
