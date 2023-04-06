using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICustomerClient : IBaseClient 
    {
        #region Profiles Association.
        /// <summary>
        /// Get list of unassociated profiles.
        /// </summary>
        /// <param name="expands">expand for profiles.</param>
        /// <param name="filters">filter for profiles.</param>
        /// <param name="sorts">sorts for profiles</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>UnAssociated profiles list.</returns>
        ProfileListModel GetUnAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get associated profile list based on customer.
        /// </summary>
        /// <param name="expands">expand for profiles.</param>
        /// <param name="filters">filter for profiles.</param>
        /// <param name="sorts">sorts for profiles.</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>Associated profile list based on customer.</returns>
        ProfileListModel GetAssociatedProfilelist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Remove associated profiles to customer by profile id.
        /// </summary>
        /// <param name="profileIds">profileIds to unassociate customer.</param>
        /// <returns>true/or false response.</returns>
        bool UnAssociateProfiles(ParameterModel profileIds, int userId);

        /// <summary>
        /// Associate profiles to customer.
        /// </summary>
        /// <param name="model">model contains profileIds to associate customer.</param>
        /// <returns>true/false response</returns>
        bool AssociateProfiles(ParameterModelUserProfile model);

        /// <summary>
        /// Set default profile for customer. 
        /// </summary>
        /// <param name="model">ParameterModelUserProfile</param>
        /// <returns>true/false response.</returns>
        bool SetDefaultProfile(ParameterModelUserProfile model);
        #endregion

        #region Affiliate
        /// <summary>
        /// Get list of referral commission type.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns list of referral commission type.</returns>
        ReferralCommissionTypeListModel GetReferralCommissionTypeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get customer affiliate data.
        /// </summary>
        /// <param name="userId">User id of the customer.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns customer affiliate data.</returns>
        ReferralCommissionModel GetCustomerAffiliate(int userId, ExpandCollection expands);

        /// <summary>
        /// Update customer affiliate data.
        /// </summary>
        /// <param name="model">Model to update in database.</param>
        /// <returns>Returns customer affiliate data.</returns>
        ReferralCommissionModel UpdateCustomerAffiliate(ReferralCommissionModel model);

        /// <summary>
        /// Get referral commission list for userId.
        /// </summary>
        /// <param name="expands">expand for profiles.</param>
        /// <param name="filters">filter for profiles.</param>
        /// <param name="sorts">sorts for profiles.</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>Referral commission list based on customer.</returns>
        ReferralCommissionListModel GetReferralCommissionlist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get customer portal profile list by user Id and portal Id.
        /// </summary>
        /// <param name="expands">expand for profiles.</param>
        /// <param name="filters">filter for profiles.</param>
        /// <param name="sorts">sorts for profiles.</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>Customer associated profile list based on portal.</returns>
        ProfileListModel GetCustomerPortalProfilelist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>Returns address list.</returns>
        AddressListModel GetAddressList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get customer address. 
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <returns>Returns address.</returns>
        AddressModel GetCustomerAddress(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Create customer address.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateCustomerAddress(AddressModel addressModel);

        /// <summary>
        /// Update customer address.
        /// </summary>
        /// <param name="addressModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        AddressModel UpdateCustomerAddress(AddressModel addressModel);

        /// <summary>
        /// Delete customer address.
        /// </summary>
        /// <param name="userAddressId">Customer Address Id to delete</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteCustomerAddress(ParameterModel userAddressId);


        /// <summary>
        /// Get search locations on the basis of search term.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="searchTerm">Search Term.</param>
        /// <returns>Returns list of matched search locations.</returns>
        AddressListModel GetSearchLocation(int portalId, string searchTerm);

        /// <summary>
        /// Update search address.
        /// </summary>
        /// <param name="addressModel">Model to update.</param>
        /// <returns>Returns updated model.</returns>
        AddressModel UpdateSearchAddress(AddressModel addressModel);

        #endregion

        #region Associate Price
        /// <summary>
        /// Associate Price to Customer.
        /// </summary>
        /// <param name="priceCustomerModel">PriceUserModel</param>
        /// <returns>Returns true if price associated successfully else return false.</returns>
        bool AssociatePriceList(PriceUserModel priceUserModel);

        /// <summary>
        /// UnAssociate associated price list from Customer.
        /// </summary>
        /// <param name="priceUserModel">Model containing price list ids and User Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociatePriceList(PriceUserModel priceUserModel);

        /// <summary>
        /// Get associated price list precedence value for Customer.
        /// </summary>
        /// <param name="priceUserModel">priceUserModel contains price list id and user id to get precedence.</param>
        /// <returns>Returns PriceUserModel.</returns>
        PriceUserModel GetAssociatedPriceListPrecedence(PriceUserModel priceUserModel);

        /// <summary>
        /// Update associated price list precedence value Customer.
        /// </summary>
        /// <param name="priceUserModel">PriceUserModel.</param>
        /// <returns>Returns updated PriceUserModel.</returns>
        PriceUserModel UpdateAssociatedPriceListPrecedence(PriceUserModel priceUserModel);
        #endregion
    }
}
