namespace Znode.Engine.Api.Cache
{
    public interface IGlobalAttributeCache
    {
        /// <summary>
        /// Get Input validation.
        /// </summary>
        /// <param name="typeId">Attribute Type Id.</param>
        /// <param name="attributeId">Global attribute Id.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetInputValidations(int typeId, int attributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the list of attributes.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns the list of attribute list.</returns>
        string GetAttributeList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets Attribute for the specified ID.
        /// </summary>
        /// <param name="id">ID of the Attribute.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Get the attribute data.</returns>
        string GetAttribute(int id, string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute locale Value.
        /// </summary>
        /// <param name="globalAttributeId">attribute id</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetAttributeLocale(int globalAttributeId, string routeUri, string routeTemplate);

        /// <summary>
        ///Get attribute default values.
        /// </summary>
        /// <param name="globalAttributeId">attribute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetDefaultValues(int globalAttributeId, string routeUri, string routeTemplate);
    }
}
