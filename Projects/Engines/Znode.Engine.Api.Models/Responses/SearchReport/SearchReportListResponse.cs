using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchReportListResponse : BaseListResponse
    {
        public List<SearchReportModel> SearchReportList { get; set; }
    }
}
