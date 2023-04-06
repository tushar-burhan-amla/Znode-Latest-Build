namespace Znode.Engine.Api.Cache
{
    public interface IRecommendationCache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Return product recommendation setting</returns>
        string GetRecommendationSetting(int portalId, string touchPointName, string routeUri, string routeTemplate);
    }
}
