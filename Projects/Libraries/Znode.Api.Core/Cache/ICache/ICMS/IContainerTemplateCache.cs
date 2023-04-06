

namespace Znode.Engine.Api.Cache
{
    public interface IContainerTemplateCache
    {
        /// <summary>
        /// Get the List of Container Templates
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string List(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Container Template
        /// </summary>
        /// <param name="templateCode">templateCode</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>ContainerTemplateModel model</returns>
        string GetContainerTemplate(string templateCode, string routeUri, string routeTemplate);


    }
}
