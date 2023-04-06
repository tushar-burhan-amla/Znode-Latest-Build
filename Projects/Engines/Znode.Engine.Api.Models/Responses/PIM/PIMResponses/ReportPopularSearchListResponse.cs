using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportPopularSearchListResponse : BaseListResponse
    {
        public List<ReportPopularSearchModel> ReportPopularSearchList { get; set; }
    }
}
