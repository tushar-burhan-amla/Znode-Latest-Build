namespace Znode.Engine.Api.Cache
{
    public interface IProductReviewStateCache 
    {
        /// <summary>
        /// Get ProductReviewState List
        /// </summary>
        /// <param name="routeUri">URI to Route</param>
        /// <param name="routeTemplate">Template to Route</param>
        /// <returns>return in string format</returns>
        string GetProductReviewStates(string routeUri, string routeTemplate);
    }
}
