namespace Znode.Engine.Api.Cache
{
    public interface IFormBuilderCache
    {
        /// <summary>
        /// Get form builder list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns form builder list.</returns>
        string GetFormBuilderList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get form details on the basis of form id.
        /// </summary>
        /// <param name="id">Form id.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Get the form data.</returns>
        string GetForm(int id, string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned attributes list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string UnAssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned groups.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Return unassigned groups.</returns>
        string GetUnAssignedGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get FormBuilder Attribute Group.
        /// </summary>
        /// <param name="formBuilderId">int formbuilderId</param>
        /// <param name="localeId">int localeId</param>
        /// <param name="mappingId">int CMSMappingId</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>returns string</returns>
        string GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId, string routeUri, string routeTemplate);
    }
}
