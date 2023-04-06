using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReportParameterListModel : BaseListModel
    {
        public ReportParameterListModel()
        {
            ParameterList = new List<ReportParameterModel>();
            CustomReportParameterList = new List<ReportParameterModel>();
        }
        public List<ReportParameterModel> ParameterList { get; set; }
        public List<ReportParameterModel> CustomReportParameterList { get; set; }
    }
}
