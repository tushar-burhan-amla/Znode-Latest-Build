using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportStoresDetailsListResponse : BaseListResponse
    {
        public List<ReportStoresDetailsModel> StoresDetails { get; set; }
    }
}
