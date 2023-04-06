using DevExpress.XtraReports.Parameters;
using System;

namespace Znode.Libraries.DevExpress.Report
{
    public interface IReportFilterHelper
    {
        ReportSettingModel GetReportSetting(Func<ReportSettingModel> reportSetting);
        void AddParameters(ReportModel report, ReportSettingModel reportSettingModel);
        void SetParameterDefaultValues(ParameterCollection parameters);
    }
}
