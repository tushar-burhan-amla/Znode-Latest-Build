using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReportCategoryListModel : BaseListModel
    {
        public List<ReportCategoryModel> ReportCategoryList { get; set; }
    }
}
