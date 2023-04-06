using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IAnalyticsClient : IBaseClient
    {
        /// <summary>
        /// Method to get analytics dashboard data
        /// </summary>
        /// <returns>AnalyticsModel containing access token and expiry time</returns>
        AnalyticsModel GetAnalyticsDashboardData();

        /// <summary>
        /// Method to get analytics JSON key
        /// </summary>
        /// <returns>string containing analytics JSON key</returns>
        string GetAnalyticsJSONKey();

        /// <summary>
        /// Updated the analytics details
        /// </summary>
        /// <param name="analyticsDetailsModel">AnalyticsModel</param>
        /// <returns>true/false</returns>
        bool UpdateAnalyticsDetails(AnalyticsModel analyticsDetailsModel);
    }
}
