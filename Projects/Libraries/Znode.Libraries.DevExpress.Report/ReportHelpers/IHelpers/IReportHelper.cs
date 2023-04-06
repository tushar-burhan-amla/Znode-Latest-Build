using System;
using System.Web;

namespace Znode.Libraries.DevExpress.Report
{
    public interface IReportHelper
    {
        ReportModel GenerateReport(string reportCode, string reportName, HttpContext currentContext, Func<ReportSettingModel> reportSetting, Func<dynamic, dynamic> dataSourcefun, Func<string, Engine.Api.Models.ReportViewModel> reportViewModel);

        string SaveReportIntoDatabase();
    }
}
