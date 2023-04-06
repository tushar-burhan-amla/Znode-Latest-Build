using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportOrderItemsDetailsResponse : BaseListResponse
    {
        public List<ReportOrderItemsDetailsModel> OrderDetailsList { get; set; }
    }
}
