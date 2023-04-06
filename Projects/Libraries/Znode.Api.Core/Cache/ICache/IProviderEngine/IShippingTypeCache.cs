namespace Znode.Engine.Api.Cache
{
    public interface IShippingTypeCache
    {
        /// <summary>
        /// Get the list of all shipping type types.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>list of all shipping type type in string format.</returns>
        string GetShippingTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a shipping typeType.
        /// </summary>
        /// <param name="shippingTypeId">shippingTypeId to get shippingType.</param>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Returns ShippingType Model in string format.</returns>
        string GetShippingType(int shippingTypeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get all shipping type Types which are not present in database.
        /// </summary>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Return List shipping type Type.</returns>
        string GetAllShippingTypesNotInDatabase(string routeUri, string routeTemplate);
    }
}
