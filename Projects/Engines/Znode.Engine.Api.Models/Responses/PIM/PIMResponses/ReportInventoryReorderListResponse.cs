using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportInventoryReorderListResponse : BaseListResponse
    {
        public List<ReportInventoryReorderModel> ReportInventoryReorderList { get; set; }
    }
}
