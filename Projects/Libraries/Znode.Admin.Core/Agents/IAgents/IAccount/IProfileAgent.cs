using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IProfileAgent
    {
        #region Profile
        /// <summary>
        ///Get profile list.
        /// </summary>
        /// <param name="filters">filter list across profile.</param>
        /// <param name="sortCollection">sort collection for profile.</param>
        /// <param name="pageIndex">pageIndex for profiles record. </param>
        /// <param name="recordPerPage">paging profiles record per page.</param>
        /// <returns>profile list</returns>
        ProfileListViewModel GetProfileList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create new profile.
        /// </summary>
        /// <param name="profileViewModel">ProfileViewModel</param>
        /// <returns>Returns profileViewModel.</returns>
        ProfileViewModel CreateProfile(ProfileViewModel profileViewModel);

        /// <summary>
        /// Get profile by profile Id.
        /// </summary>
        /// <param name="profileId">profile Id</param>
        /// <returns>Returns profileViewModel.</returns>
        ProfileViewModel GetProfileById(int profileId);

        /// <summary>
        /// Update profile.
        /// </summary>
        /// <param name="profileId">profileId.</param>
        /// <returns>Returns updated ProfileViewModel.</returns>
        ProfileViewModel UpdateProfile(int profileId, string data);

        /// <summary>
        /// Delete profile.
        /// </summary>
        /// <param name="profileId">profileId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteProfile(string profileId, out string errorMessage);

        /// <summary>
        /// Get profile as list of SelectListItem.
        /// </summary>
        /// <returns>List of SelectListItem</returns>
        List<SelectListItem> GetProfileList();
        #endregion

        #region Profile Catalog
        /// <summary>
        ///Get profile catalog list.
        /// </summary>
        /// <param name="filters">filter list across profile.</param>
        /// <param name="profileId">profileId.</param>
        /// <param name="sortCollection">sort collection for profile catalog.</param>
        /// <param name="pageIndex">pageIndex for profiles record. </param>
        /// <param name="recordPerPage">paging profile catalog record per page.</param>
        /// <returns>profile catalog list</returns>
        ProfileCatalogListViewModel GetProfileCatalogList(int profileId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        ///Get profile unassociated catalog list.
        /// </summary>
        /// <param name="filters">filter list across profile.</param>
        /// <param name="profileId">profileId.</param>
        /// <param name="sortCollection">sort collection for profile catalog.</param>
        /// <param name="pageIndex">pageIndex for profiles record. </param>
        /// <param name="recordPerPage">paging profile catalog record per page.</param>
        /// <returns>ProfileCatalog list</returns>
        ProfileCatalogListViewModel GetProfileUnAssociatedCatalogList(int profileId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Delete profile Catalog.
        /// </summary>
        /// <param name="profileId">profileId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAssociatedProfileCatalog(int profileId, out string errorMessage);

        /// <summary>
        /// Associate catalogs to profile.
        /// </summary>
        /// <param name="profileId">Profile Id to which catalogs to be associated.</param>
        /// <param name="pimCatalogId">pimCatalogId to be associated.</param>
        /// <returns>Returns true if catalog associated successfully else return false.</returns>
        bool AssociateCatalogToProfile(int profileId, int pimCatalogId);
        #endregion

        #region Profile Shipping
        /// <summary>
        /// Get associated shipping list for profile.
        /// </summary>
        /// <param name="filters">Filters for profile shipping.</param>
        /// <param name="sorts">Sorts for for profile shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Profile Shipping.</returns>
        ShippingListViewModel GetAssociatedShippingList(int profileId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of unassociated shipping for profile.
        /// </summary>
        /// <param name="filters">Filters for profile shipping.</param>
        /// <param name="sorts">Sorts for for profile shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated shipping list.</returns>
        ShippingListViewModel GetUnAssociatedShippingList(int profileId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate shipping to profile.
        /// </summary>
        /// <param name="profileId">profileId.</param>
        /// <param name="shippingIds">shippingIds to be associated.</param>
        /// <returns>Returns true if shipping associated successfully else return false.</returns>
        bool AssociateShipping(int profileId, string shippingIds);

        /// <summary>
        /// Remove associated shipping from profile.
        /// </summary>
        /// <param name="shippingId">ShippingIds to be removed.</param>
        /// <param name="profileId">profileId</param>
        /// <returns>Returns true if shipping unassociated successfully else return false.</returns>
        bool UnAssociateAssociatedShipping(string shippingId, int profileId);


        /// <summary>
        /// Update profile shipping
        /// </summary>
        /// <param name="shippingId">ShippingId to update</param>
        /// <param name="profileId">ProfileId to update</param>
        /// <param name="data">data</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateProfileShipping(int shippingId, int profileId, string data);
        #endregion

        #region Payment Setting
        /// <summary>
        /// Get payment settings list.
        /// </summary>
        /// <param name="profileId">Profile id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="isUnassociated">Is Unassociated</param>
        /// <returns>Returns payment settings list.</returns>
        PaymentSettingListViewModel GetPaymentSettingsList(int profileId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isUnassociated);

        /// <summary>
        /// Associate payment settings to profile.
        /// </summary>
        /// <param name="profileId">Profile id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePaymentSettings(int profileId, string paymentSettingIds);

        /// <summary>
        /// Remove associated payment settings to profile.
        /// </summary>
        /// <param name="profileId">Profile Id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPaymentSettings(int profileId, string paymentSettingIds);

        /// <summary>
        /// Update profile payment setting
        /// </summary>
        /// <param name="paymentSettingId">paymentSetting Id</param>
        /// <param name="profileId">profile Id</param>
        /// <param name="data">data</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateProfilePaymentSetting(int paymentSettingId, int profileId, string data);
        #endregion
    }
}
