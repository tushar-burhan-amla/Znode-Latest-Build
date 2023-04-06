using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportDetailListResponse : BaseListResponse
    {
        public List<ReportDetailModel> ReportDetailList { get; set; }
    }
}
