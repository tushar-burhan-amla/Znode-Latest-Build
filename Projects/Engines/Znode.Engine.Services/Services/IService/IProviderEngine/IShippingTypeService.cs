using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IShippingTypeService
    {
        /// <summary>
        /// Get the list of all Shipping Types.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection to generate orderby clause.</param>
        /// <param name="page">Collection  of paging parameters.</param>
        /// <returns>List of ShippingType Model.</returns>
        ShippingTypeListModel GetShippingTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get a Shipping Type.
        /// </summary>
        /// <param name="shippingTypeId">ID of ShippingType to get details of perticular ShippingType.</param>
        /// <returns>Returns ShippingType Model.</returns>
        ShippingTypeModel GetShippingType(int shippingTypeId);

        /// <summary>
        /// Creates a new Shipping Type.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model to create new shipping type.</param>
        /// <returns>Returns newly created ShippingType Model.</returns>
        ShippingTypeModel CreateShippingType(ShippingTypeModel shippingTypeModel);

        /// <summary>
        /// Update a Shipping Type.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model to update shipping type.</param>
        /// <returns>Returns true if ShippingType updated else returns false.</returns>
        bool UpdateShippingType(ShippingTypeModel shippingTypeModel);

        /// <summary>
        /// Delete a Shipping Type.
        /// </summary>
        /// <param name="shippingTypeIds">ID of shipping type to delete.</param>
        /// <returns>Returns true if ShippingType deleted else returns false.</returns>
        bool DeleteShippingType(ParameterModel shippingTypeIds);

        /// <summary>
        /// Get all Shipping Types which are not present in database.
        /// </summary>
        /// <returns>Return List Shipping Type.</returns>
        ShippingTypeListModel GetAllShippingTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable shipping types
        /// </summary>
        /// <param name="shippingTypeIds">Ids of shipping type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable shipping type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool EnableDisableShippingType(ParameterModel shippingTypeIds, bool isEnable);
    }
}
