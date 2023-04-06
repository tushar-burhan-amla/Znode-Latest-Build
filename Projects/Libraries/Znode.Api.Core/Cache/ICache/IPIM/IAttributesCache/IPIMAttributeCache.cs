namespace Znode.Engine.Api.Cache
{
    public interface IPIMAttributeCache
    {
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
        /// <returns>Gets the attribute.</returns>
        string GetAttribute(int id, string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute type list from database.
        /// </summary>
        /// <param name="isCategory">get types according to category flag.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns attribute type string</returns>
        string GetAttributeTypes(bool isCategory,string routeUri, string routeTemplate);

        /// <summary>
        /// Get Input validation
        /// </summary>
        /// <param name="typeId">Attribute Type Id</param>
        /// <param name="attributeId">Attribute Id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetInputValidations(int typeId,int attributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Front End Properties
        /// </summary>
        /// <param name="pimAttributeId"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string FrontEndProperties(int pimAttributeId,string routeUri, string routeTemplate);

        /// <summary>
        /// GEt attribute Locale Value
        /// </summary>
        /// <param name="pimAttributeId">attribute id</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetAttributeLocale(int pimAttributeId, string routeUri, string routeTemplate);
        
        /// <summary>
        ///get attribute default values 
        /// </summary>
        /// <param name="pimAttributeId">attribute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetDefaultValues(int pimAttributeId, string routeUri, string routeTemplate);

    }
}
