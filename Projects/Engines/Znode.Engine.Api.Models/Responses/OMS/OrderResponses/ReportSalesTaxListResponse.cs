using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportSalesTaxListResponse : BaseListResponse
    {
        public List<ReportSalesTaxModel> ReportSalesTaxList { get; set; }
    }
}
