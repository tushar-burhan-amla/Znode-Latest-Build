namespace Znode.Engine.Api.Client.Endpoints
{
    public class SearchReportEndpoint : BaseEndpoint
    {
        //Get no result found report.
        public static string GetNoResultsFoundReport() => $"{ApiRoot}/searchreport/getnoresultsfoundreport";

        //Get top keyword search report.
        public static string GetTopKeywordsReport() => $"{ApiRoot}/searchreport/gettopkeywordsreport";

        //Save search report data
        public static string SaveSearchReport() => $"{ApiRoot}/searchreport/savesearchreport";

    }
}
