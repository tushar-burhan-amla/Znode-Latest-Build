
namespace Znode.Engine.Api.Cache
{
    public interface IContentContainerCache
    {

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>List</returns>
        string List(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>ContentContainerResponseModel</returns>
        string GetContentContainer(string containerKey, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Associated Variant
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate"><routeTemplate/param>
        /// <returns>List of Associated Variant</returns>
        string GetAssociatedVariants(string containerKey, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the List of Associated Variants
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>List</returns>
        string GetAssociatedVariantList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Content Container Attribute Values based on Container variants.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <param name="containerKey">containerKey.</param>
        /// <param name="localeId">localeId.</param>
        /// <param name="portalId">portalId</param>
        /// <param name="profileId">profileId</param>
        /// <returns>returns the correct container variant data according to the specified parameters and variant precedence.</returns>
        string GetContentContainerData(string routeUri, string routeTemplate, string containerKey, int localeId, int portalId = 0, int profileId = 0);

        /// <summary>
        /// Get Content Container variant locale data
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>ContentContainerResponseModel</returns>
        string GetVariantLocaleData(int variantId, string routeUri, string routeTemplate);
    }
}
