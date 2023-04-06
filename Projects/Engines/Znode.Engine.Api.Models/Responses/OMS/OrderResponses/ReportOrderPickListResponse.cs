using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportOrderPickListResponse : BaseListResponse
    {
        public List<ReportOrderPickModel> ReportOrderPickList { get; set; }
    }
}
