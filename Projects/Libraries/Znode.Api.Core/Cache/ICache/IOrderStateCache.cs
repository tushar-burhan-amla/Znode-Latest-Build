namespace Znode.Engine.Api.Cache
{
    public interface IOrderStateCache
    {
        /// <summary>
        /// Get OrderState List
        /// </summary>
        /// <param name="routeUri">URI to Route</param>
        /// <param name="routeTemplate">Template to Route</param>
        /// <returns>return in string format</returns>
        string GetOrderStates(string routeUri, string routeTemplate);
    }
}
