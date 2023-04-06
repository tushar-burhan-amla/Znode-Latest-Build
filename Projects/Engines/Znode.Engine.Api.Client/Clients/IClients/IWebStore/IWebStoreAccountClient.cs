using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreUserClient : IBaseClient
    {
        /// <summary>
        /// Create account address.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Update account address.
        /// </summary>
        /// <param name="addressModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        AddressModel UpdateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Gets the list of address for user.
        /// </summary>       
        /// <param name="expands">expand to have data from related table</param>
        /// <param name="filters">filters for address list</param>
        /// <returns>Address list model.</returns>
        List<AddressModel> GetUserAddressList(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Get Address information on the basis of Address Id.
        /// </summary>
        /// <param name="addressId">addressId.</param>
        /// <returns>Returns model.</returns>
        AddressModel GetAddress(int? addressId);

        /// <summary>
        /// Delete Address on the basis of Address Id and User Id.
        /// </summary>
        /// <param name="addressId">addressId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAddress(int? addressId, int? userId);
    }
}
