using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportViewListResponse : BaseListResponse
    {
        public ReportViewListResponse()
        {
            ReportView = new List<ReportViewModel>();
        }
        public List<ReportViewModel> ReportView { get; set; }
    }
}
