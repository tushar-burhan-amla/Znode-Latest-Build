using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportVendorsListResponse : BaseListResponse
    {
        public List<ReportVendorsModel> ReportVendorsList { get; set; }
    }
}
