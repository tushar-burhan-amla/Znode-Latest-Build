using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportCategoryListResponse : BaseListResponse
    {
        public List<ReportCategoryModel> ReportCategoryList { get; set; }
    }
}
