namespace Znode.Engine.Api.Cache
{
    public interface IDynamicContentCache
    {
        /// <summary>
        /// To get WYSISYG editor formats.
        /// </summary>
        /// <param name="portalId">To specify portal Id</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>List of formats for WYSIWYG editor</returns>
        string GetEditorFormats(int portalId, string routeUri, string routeTemplate);
    }
}
