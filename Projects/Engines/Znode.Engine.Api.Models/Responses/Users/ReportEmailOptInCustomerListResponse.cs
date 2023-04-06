using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportEmailOptInCustomerListResponse : BaseListResponse
    {
        public List<ReportEmailOptInCustomerModel> EmailOptInCustomerList { get; set; }
    }
}
