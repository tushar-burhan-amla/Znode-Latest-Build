using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportServiceRequestListResponse : BaseListResponse
    {
        public List<ReportServiceRequestModel> ServiceRequestList { get; set; }
    }
}
