using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IProfileService
    {
        #region Profile
        /// <summary>
        /// Create profile.
        /// </summary>
        /// <param name="profileModel">Profile Model</param>
        /// <returns>returns ProfileModel</returns>
        ProfileModel CreateProfile(ProfileModel profileModel);

        /// <summary>
        /// Update profile.
        /// </summary>
        /// <param name="profileModel">Profile Model</param>
        /// <returns>return status</returns>
        bool UpdateProfile(ProfileModel profileModel);

        /// <summary>
        /// Get profile by profileId
        /// </summary>
        /// <param name="profileId">Id of profile</param>
        /// <returns>returns ProfileModel </returns>
        ProfileModel GetProfile(int profileId);

        /// <summary>
        /// Get paged profile list.
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filters list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>return ProfileListModel </returns>
        ProfileListModel GetProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete profile by profileId
        /// </summary>
        /// <param name="profileId">Id of profile</param>
        /// <returns>return status</returns>
        bool DeleteProfile(ParameterModel profileId);
        #endregion

        #region Profile Catalog
        /// <summary>
        /// Get paged profile catalog list.
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filters list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>return ProfileCatalogListModel </returns>
        ProfileCatalogListModel GetProfileCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete associated catalog to profile by profileId.
        /// </summary>
        /// <param name="profileId">profileId</param>
        /// <returns>return true/false</returns>
        bool DeleteAssociatedProfileCatalog(int profileId);

        /// <summary>
        /// Associate Catalog To Profile. 
        /// </summary>
        /// <param name="profileCatalogModel">Model to associate catalog to profile.</param>
        /// <returns>Returns true/false/></returns>
        bool AssociateCatalogToProfile(ProfileCatalogModel profileCatalogModel);

        #endregion
    }
}
