using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReportColumnsListModel : BaseListModel
    {
        public ReportColumnsListModel()
        {
            ColumnList = new List<ReportColumnModel>();
            CustomReportColumnList = new List<ReportColumnModel>();
        }
        public List<ReportColumnModel> ColumnList { get; set; }
        public List<ReportColumnModel> CustomReportColumnList { get; set; }
    }
}
