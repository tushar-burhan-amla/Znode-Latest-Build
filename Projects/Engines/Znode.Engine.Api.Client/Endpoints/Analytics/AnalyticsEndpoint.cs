namespace Znode.Engine.Api.Client.Endpoints
{
    public class AnalyticsEndpoint: BaseEndpoint
    {
        public static string GetAnalyticsDashboardData() => $"{ApiRoot}/analytics/getanalyticsdashboarddata";

        public static string GetAnalyticsJSONKey() => $"{ApiRoot}/analytics/getanalyticsjsonkey";

        public static string UpdateAnalyticsDetails() => $"{ApiRoot}/analytics/updateanalyticsdetails";
    }
}
