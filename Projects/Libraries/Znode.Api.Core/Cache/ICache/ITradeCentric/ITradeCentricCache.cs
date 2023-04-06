namespace Znode.Engine.Api.Cache
{
    public interface ITradeCentricCache
    {
        /// <summary>
        /// Get TradeCentric User details.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>TradeCentric user details</returns>
        string GetTradeCentricUser(int userId, string routeUri, string routeTemplate);
    }
}
