using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// Method to get analytics dashboard data
        /// </summary>
        /// <returns>AnalyticsModel containing analytics dashboard data</returns>
        AnalyticsModel GetAnalyticsDashboardData();

        /// <summary>
        /// Method to get analytics JSON key
        /// </summary>
        /// <returns>string containing JSON key</returns>
        string GetAnalyticsJSONKey();

        /// <summary>
        /// Method to update the analytics details containing JSON key
        /// </summary>
        /// <param name="analyticsDetailsModel">AnalyticsModel to be updated</param>
        /// <returns>return true if updated or created successfully else return false.</returns>
        bool UpdateAnalyticsDetails(AnalyticsModel analyticsDetailsModel);
    }
}
