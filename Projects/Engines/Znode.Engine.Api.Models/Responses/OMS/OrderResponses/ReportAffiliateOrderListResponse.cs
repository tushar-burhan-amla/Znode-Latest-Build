using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportAffiliateOrderListResponse : BaseListResponse
    {
        public List<ReportAffiliateOrderModel> ReportAffiliateOrderList { get; set; }
    }
}
