using MvcReportViewer;
using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportViewModel: BaseViewModel
    {
        public ControlSettings ControlSetting { get; set; }
        public Dictionary<string, object> ParameterList { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public int ReportTypeId { get; set; }
        public string ReportType { get; set; }
        public int CustomReportTemplateId { get; set; }
        public bool isDynamicReport { get; set; }
    }
}