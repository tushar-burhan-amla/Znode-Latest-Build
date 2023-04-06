using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportParameterListViewModel : BaseViewModel
    {
        public ReportParameterListViewModel()
        {
            ParamList = new List<ReportParameterViewModel>();
            CustomReportParamList = new List<ReportParameterViewModel>();
        }

        public List<ReportParameterViewModel> ParamList { get; set; }
        public List<ReportParameterViewModel> CustomReportParamList { get; set; }
    }
}