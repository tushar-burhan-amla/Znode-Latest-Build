using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IShippingTypeClient : IBaseClient
    {
        /// <summary>
        /// Get the list of shipping type type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns shipping typeType list model.</returns>
        ShippingTypeListModel GetShippingTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a shipping type type.
        /// </summary>
        /// <param name="shippingTypeId">ID of ShippingType to get details of perticular ShippingType.</param>
        /// <returns>Returns shipping type type model.</returns>
        ShippingTypeModel GetShippingType(int shippingTypeId);

        /// <summary>
        /// Create new shipping type type.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model.</param>
        /// <returns>Returns newly created ShippingType Model.</returns>
        ShippingTypeModel CreateShippingType(ShippingTypeModel shippingTypeModel);

        /// <summary>
        /// Update a shipping typeType.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model.</param>
        /// <returns>Returns shipping type Type Model as response.</returns>
        ShippingTypeModel UpdateShippingType(ShippingTypeModel shippingTypeModel);

        /// <summary>
        /// Delete a shipping typeType.
        /// </summary>
        /// <param name="entityIds">ID of shipping type type to delete.</param>
        /// <returns>Returns true if ShippingType deleted else returns false.</returns>
        bool DeleteShippingType(ParameterModel entityIds);

        /// <summary>
        /// Get all shipping type Types which are not present in database.
        /// </summary>
        /// <returns>Returns shipping typeType list ViewModel which are not in database.</returns>
        ShippingTypeListModel GetAllShippingTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable shipping types
        /// </summary>
        /// <param name="entityIds">Ids of shipping type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable shipping type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisableShippingTypes(ParameterModel entityIds, bool isEnable);
    }
}
