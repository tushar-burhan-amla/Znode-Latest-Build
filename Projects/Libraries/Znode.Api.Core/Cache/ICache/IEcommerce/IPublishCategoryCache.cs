namespace Znode.Engine.Api.Cache
{
    public interface IPublishCategoryCache
    {
        /// <summary>
        /// Get Publish Categories 
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCategoryList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Categories excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">route uriassigned ids.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetUnAssignedPublishCategoryList(string assignedIds, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Category 
        /// </summary>
        /// <param name="publishCategoryId">publish Category Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCategory(int publishCategoryId,string routeUri, string routeTemplate);

    }
}
