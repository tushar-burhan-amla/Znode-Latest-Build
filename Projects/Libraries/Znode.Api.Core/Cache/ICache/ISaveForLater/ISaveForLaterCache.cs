
namespace Znode.Api.Core.Cache.ICache.ISaveForLater
{
    public interface ISaveForLaterCache
    {
        /// <summary>
        /// Get the saved cart for later
        /// </summary>
        /// <param name="userId">LoggedIn user Id</param>
        /// <param name="templateType">Template type</param>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns></returns>
        string GetCartForLater(int userId, string templateType, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Saved later cart template
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <param name="routeUri">routUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string GetCartTemplate(int omsTemplateId, string routeUri, string routeTemplate);
    }
}
