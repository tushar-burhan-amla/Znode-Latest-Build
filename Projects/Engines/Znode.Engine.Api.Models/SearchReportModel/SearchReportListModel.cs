using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchReportListModel : BaseListModel
    {
        public List<SearchReportModel> SearchReportList { get; set; }
    }
}
