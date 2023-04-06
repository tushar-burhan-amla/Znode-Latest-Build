using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportTopSpendingCustomersListResponse : BaseListResponse
    {
        public List<ReportTopSpendingCustomersModel> SpendingCustomersList { get; set; }
    }
}
