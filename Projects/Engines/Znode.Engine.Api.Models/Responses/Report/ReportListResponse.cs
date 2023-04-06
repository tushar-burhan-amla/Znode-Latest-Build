using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportListResponse : BaseListResponse
    {
        public List<ReportModel> ReportList { get; set; }
    }
}
