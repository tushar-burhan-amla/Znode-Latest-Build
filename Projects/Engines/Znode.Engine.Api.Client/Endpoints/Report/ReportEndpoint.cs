namespace Znode.Engine.Api.Client.Endpoints
{
    public class ReportEndpoint : BaseEndpoint
    {
        public static string List() => $"{ApiRoot}/report/list";

        public static string GetExportData(string dynamicReportType) => $"{ApiRoot}/report/getexportdata/{dynamicReportType}";

        public static string GenerateDynamicReport() => $"{ApiRoot}/report/generatedynamicreport";

        public static string DeleteDynamicReport() => $"{ApiRoot}/report/deletedynamicreport";
        public static string GetCustomReport(int customReportId) => $"{ApiRoot}/report/getcustomreport/{customReportId}";
    }
}
