using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportMostFrequentCustomerListResponse : BaseListResponse
    {
        public List<ReportMostFrequentCustomerModel> FrequentCustomerList { get; set; }
    }
}
