using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICustomerService
    {
        #region Profile Association
        /// <summary>
        /// Get list of unassociate profiles.
        /// </summary>
        /// <param name="expands">expand for profile.</param>
        /// <param name="filters">filter for profile.</param>
        /// <param name="sorts">sorts for profile.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>ProfileListModel</returns>
        ProfileListModel GetUnAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated profile list to customer.
        /// </summary>
        /// <param name="expands">expand for profile</param>
        /// <param name="filters">filter for profile</param>
        /// <param name="sorts">sorts for profile</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>ProfileListModel</returns>
        ProfileListModel GetAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Unassociate associated profiles.
        /// </summary>
        /// <param name="profileIds">Ids to delete associated profiles.</param>
        /// <returns>true if removed else false.</returns>
        bool UnAssociateProfiles(ParameterModel profileIds, int userId);

        /// <summary>
        /// Associate profiles to customer.
        /// </summary>
        /// <param name="model">model contains userId and profileIds to associate profiles.</param>
        /// <returns>true or false response</returns>
        bool AssociateProfiles(ParameterModelUserProfile model);

        /// <summary>
        /// Set default profile for customer. 
        /// </summary>
        /// <param name="model">ParameterModelUserProfile</param>
        /// <returns>true/false response.</returns>
        bool SetDefaultProfile(ParameterModelUserProfile model);


        /// <summary>
        /// Get list of associated profiles based on portal.
        /// </summary>
        /// <param name="expands">expand for profile.</param>
        /// <param name="filters">filter for profile.</param>
        /// <param name="sorts">sorts for profile.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>ProfileListModel</returns>
        ProfileListModel GetCustomerPortalProfilelist(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        #endregion

        #region Affiliate
        /// <summary>
        /// Get list of referral commission type.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns list of referral commission type.</returns>
        ReferralCommissionTypeListModel GetReferralCommissionTypeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get customer affiliate data. 
        /// </summary>
        /// <param name="userId">User id of the customer.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns customer affiliate data.</returns>
        ReferralCommissionModel GetCustomerAffiliate(int userId, NameValueCollection expands);

        /// <summary>
        /// Update customer affiliate data. 
        /// </summary>
        /// <param name="referralCommissionModel">Model to update in database.</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateCustomerAffiliate(ReferralCommissionModel referralCommissionModel);

        /// <summary>
        /// Get list of referral commission.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns list of referral commission.</returns>
        ReferralCommissionListModel GetReferralCommissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page collection.</param>
        /// <returns>Returns address list.</returns>
        AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete customer address.
        /// </summary>
        /// <param name="userAddressId">User Address Id to delete</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteCustomerAddress(ParameterModel userAddressId);

        /// <summary>
        /// Update customer address.
        /// </summary>
        /// <param name="addressModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        bool UpdateCustomerAddress(AddressModel addressModel);

        /// <summary>
        /// Create customer address.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateCustomerAddress(AddressModel addressModel);

        /// <summary>
        /// Get customer address. 
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns customer address model.</returns>
        AddressModel GetCustomerAddress(FilterCollection filters, NameValueCollection expands);


        /// <summary>
        /// Get list of search locations.
        /// </summary>
        /// <param name="portalId">PortalId.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <returns>Returns the list of matched search locations.</returns>
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
        /// Associate price list to customer.
        /// </summary>
        /// <param name="priceUserModel">priceUserModel contains data to associate.</param>
        /// <returns>returns true if associated else false.</returns>
        bool AssociatePriceList(PriceUserModel priceUserModel);

        /// <summary>
        /// UnAssociate associated price list from customer.
        /// </summary>
        /// <param name="priceUserModel">priceAccountModel contains price list ids to unassociate.</param>
        /// <returns>returns true if unassociated else false.</returns>
        bool UnAssociatePriceList(PriceUserModel priceUserModel);

        /// <summary>
        /// Get Associated Price List with Precedence data for customer.
        /// </summary>
        /// <param name="priceUserModel">PriceUserModel</param>
        /// <returns>PriceUserModel</returns>
        PriceUserModel GetAssociatedPriceListPrecedence(PriceUserModel priceUserModel);

        /// <summary>
        /// Update the precedence value for associated price list for customer.
        /// </summary>
        /// <param name="priceUserModel">PriceUserModel</param>
        /// <returns>PriceUserModel</returns>
        bool UpdateAssociatedPriceListPrecedence(PriceUserModel priceUserModel);
        #endregion        
    }
}
