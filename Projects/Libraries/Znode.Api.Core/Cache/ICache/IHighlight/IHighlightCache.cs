namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface HighlightCache.
    /// </summary>
    public interface IHighlightCache
    {
        /// <summary>
        /// Get highlight details.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return highlight details.</returns>
        string GetHighlights(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the highlight.
        /// </summary>
        ///<param name="highLightId">Highlight ID of the highlight.</param>
        /// <param name="productId">ID of the product.</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return page content.</returns>
        string GetHighlight(int highlightId, int productId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the highlight by Highlight code.
        /// </summary>
        ///<param name="highLightCode">Highlight Code of the highlight.</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return page content.</returns>
        string GetHighlightByCode(string highLightCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get highlight code list.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <param name="attributeCode">Attribute code </param>
        /// <returns></returns>
        string GetHighlightCodeList(string attributeCode, string routeUri, string routeTemplate);

        #region Highlight Rule Type

        /// <summary>
        /// Get list of highlight type.
        /// </summary>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns list of highlight type.</returns>
        string GetHighlightTypeList(string routeUri, string routeTemplate);
        #endregion       
    }
}