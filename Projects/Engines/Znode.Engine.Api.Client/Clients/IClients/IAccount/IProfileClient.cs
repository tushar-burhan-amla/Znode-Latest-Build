using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IProfileClient : IBaseClient
    {
        #region Profile
        /// <summary>
        /// Get profile by profile Id.
        /// </summary>
        /// <param name="profileId">Id of profile.</param>
        /// <returns>Returns ProfileModel. </returns>
        ProfileModel GetProfile(int profileId);

        /// <summary>
        /// //Get profile list 
        /// </summary>
        /// <param name="filters">collection of filter.</param>
        /// <param name="sorts">sort collection.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>Return paged profile list. </returns>
        ProfileListModel GetProfileList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Save profile.
        /// </summary>
        /// <param name="model">ProfileModel</param>
        /// <returns>Return Saved profile model.</returns>
        ProfileModel SaveProfile(ProfileModel model);

        /// <summary>
        /// Update profile.
        /// </summary>
        /// <param name="model">ProfileModel</param>
        /// <returns>Return status.</returns>
        ProfileModel UpdateProfile(ProfileModel model);

        /// <summary>
        /// Delete profile by profile Id.
        /// </summary>
        /// <param name="profileId">Id of profile.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteProfile(ParameterModel profileId);
        #endregion

        #region Profile Catalog
        /// <summary>
        /// //Get profile catalog list 
        /// </summary>
        /// <param name="filters">collection of filter.</param>
        /// <param name="sorts">sort collection.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>Return profile catalog list. </returns>
        ProfileCatalogListModel GetProfileCatalogList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete associated catalogs to profile by profileCatalogId.
        /// </summary>
        /// <param name="profileId">profileId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAssociatedProfileCatalog(int profileId);

        /// <summary>
        /// Associate catalog to profile.
        /// </summary>
        /// <param name="profileCatalogModel">ProfileCatalogModel</param>
        /// <returns>Returns true if catalog associated successfully else return false.</returns>
        bool AssociateCatalogToProfile(ProfileCatalogModel profileCatalogModel);
        #endregion
    }
}
