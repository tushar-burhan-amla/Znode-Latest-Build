using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class OrderDetailsListResponse : BaseListResponse
    {
        public List<ReportOrderDetailsModel> OrderDetailsList { get; set; }
    }
}
