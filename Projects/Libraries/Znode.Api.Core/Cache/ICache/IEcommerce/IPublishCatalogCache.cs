namespace Znode.Engine.Api.Cache
{
    public interface IPublishCatalogCache
    {
        /// <summary>
        /// Get Publish Catalogs 
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCatalogList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get publish catelog excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">assigned Ids.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetUnAssignedPublishCatelogList(string assignedIds, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Catalog 
        /// </summary>
        /// <param name="publishCatalogId">publish Catalog Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCatalog(int publishCatalogId, int? localeId, string routeUri,  string routeTemplate);
    }
}
