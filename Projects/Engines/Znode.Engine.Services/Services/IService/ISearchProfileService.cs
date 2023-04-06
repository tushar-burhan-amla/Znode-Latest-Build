using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISearchProfileService
    {
        /// <summary>
        /// Gets List of Search Profiles
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>returns List of Search Profiles</returns>
        SearchProfileListModel GetSearchProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets search profile based on provided portalId
        /// </summary>
        /// <param name="searchProfileId">Id of search profile</param>
        /// <returns>search profile model</returns>
        SearchProfileModel GetSearchProfile(int searchProfileId);

        /// <summary>
        /// Create Search Profile
        /// </summary>
        /// <param name="model">Search Profile Model</param>
        /// <returns>returns created search profile model</returns>
        SearchProfileModel Create(SearchProfileModel model);

        /// <summary>
        /// Gets search profile details
        /// </summary>
        /// <returns>returns search profile details</returns>
        SearchProfileModel GetSearchProfileDetails(FilterCollection filters);

        /// <summary>
        /// deletes search profile
        /// </summary>
        /// <param name="searchProfileId">searchProfile Id</param>
        /// <returns>true if search profile deleted successfully otherwise false.</returns>
        bool DeleteSearchProfile(ParameterModel searchProfileId);

        /// <summary>
        /// Updates search profile
        /// </summary>
        /// <param name="searchProfileModel">searchProfileModel</param>
        /// <returns>true if search profile updated successfully otherwise false.</returns>
        bool UpdateSearchProfile(SearchProfileModel searchProfileModel);

        /// <summary>
        /// Gets search features based on query id
        /// </summary>
        /// <param name="queryId">query type id</param>
        /// <returns>list of profiles</returns>
        List<SearchFeatureModel> GetFeaturesByQueryId(int queryId, int searchProfileId = 0);

        /// <summary>
        /// Set default search profile.
        /// </summary>
        /// <param name="portalSearchProfileModel"></param>
        /// <returns>return updated model</returns>
        bool SetDefaultSearchProfile(PortalSearchProfileModel portalSearchProfileModel);

        #region Search Triggers
        /// <summary>
        /// Gets List of Search Profiles Triggers
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>returns List of Search Profiles</returns>
        SearchTriggersListModel GetSearchTriggersList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets search profile triggers based on provided searchProfileTriggerId
        /// </summary>
        /// <param name="searchProfileTriggerId">Id of search profile triggers</param>
        /// <returns>search profile model</returns>
        SearchTriggersModel GetSearchTrigger(int searchProfileTriggerId);

        /// <summary>
        /// Create Search Profile triggers
        /// </summary>
        /// <param name="model">Search Profile Model</param>
        /// <returns>returns created search profile model</returns>
        bool CreateSearchTriggers(SearchTriggersModel searchTriggersModel);

        /// <summary>
        /// Deletes search profile triggers.
        /// </summary>
        /// <param name="searchProfileTriggerId">searchProfileTriggerId</param>
        /// <returns>true if search profile deleted successfully otherwise false.</returns>
        bool DeleteSearchTriggers(ParameterModel searchProfileTriggerId);

        /// <summary>
        /// Updates search profile triggers.
        /// </summary>
        /// <param name="searchProfileModel">searchProfileModel</param>
        /// <returns>true if search profile updated successfully otherwise false.</returns>
        bool UpdateSearchTriggers(SearchTriggersModel searchTriggersModel);
        #endregion

        #region Search Facets
        /// <summary>
        /// Gets List of Search Attributes
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>returns List of Search Profiles</returns>
        SearchAttributesListModel GetAssociatedUnAssociatedCatalogAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate UnAssociated search attributes to search profile.
        /// </summary>
        /// <param name="searchAttributesModel">searchAttributesModel.</param>
        /// <returns>Returns true if associated successfully else return false.</returns>
        bool AssociateAttributesToProfile(SearchAttributesModel searchAttributesModel);

        /// <summary>
        /// UnAssociate each attribute from search profile.
        /// </summary>
        /// <param name="searchProfilesAttributeMappingId">model containing searchProfilesAttributeMappingIds to be deleted.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociateAttributesFromProfile(ParameterModel searchProfilesAttributeMappingIds);
        #endregion

        /// <summary>
        /// Gets search attributes based on catalog id
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <returns>search attributes based on catalog id</returns>
        SearchAttributesListModel GetCatalogBasedSearchableAttributes(ParameterModel associatedAttributes, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        SearchProfilePortalListModel SearchProfilePortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associates portal to search profile
        /// </summary>
        /// <param name="model">Search Profile Parameter Model</param>
        /// <returns>True / False</returns>
        bool AssociatePortalToSearchProfile(SearchProfileParameterModel model);

        /// <summary>
        /// Gets Unassociated Portal List
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>Unassociated Portal List</returns>
        PortalListModel GetUnAssociatedPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        ///Get field Value List by Catalog id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id</param>
        /// <param name="searchProfileId">search profile id.</param>
        /// <returns>Search profile model.</returns>
        SearchProfileModel GetFieldValuesList(int publishCatalogId, int searchProfileId);

        /// <summary>
        /// Publish Search Profile Based on the Search Profile Id 
        /// </summary>
        /// <param name="searchProfileId"></param>
        /// <returns>Return True if publish is success Otherwise false</returns>
        bool PublishSearchProfile(int searchProfileId);

        /// <summary>
        /// To get the catalog list that is not associated with any of the search profiles.
        /// </summary>
        /// <returns>Catalog list</returns>
        TypeaheadResponselistModel GetCatalogListForSearchProfile();
    }
}
