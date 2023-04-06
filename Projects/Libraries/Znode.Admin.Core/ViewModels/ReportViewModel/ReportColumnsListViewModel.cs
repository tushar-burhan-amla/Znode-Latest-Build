
using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportColumnsListViewModel : BaseViewModel
    {
        public ReportColumnsListViewModel()
        {
            ColumnList = new List<ReportColumnViewModel>();
            CustomReportColumnList = new List<ReportColumnViewModel>();
        }
        public List<ReportColumnViewModel> ColumnList { get; set; }
        public List<ReportColumnViewModel> CustomReportColumnList { get; set; }
    }
}