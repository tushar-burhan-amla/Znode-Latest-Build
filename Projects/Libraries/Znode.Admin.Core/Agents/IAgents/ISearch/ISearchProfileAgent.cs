using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Admin.Core.Areas.Search.ViewModels;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using System;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface ISearchProfileAgent
    {
        /// <summary>
        /// Create Search Profile
        /// </summary>
        /// <param name="model">Search Profile ViewModel</param>
        /// <param name="fieldNames">field names list.</param>
        /// <param name="fieldValues">filed values list</param>
        /// <returns>returns created SearchProfile Model</returns>
        SearchProfileViewModel CreateSearchProfile(SearchProfileViewModel model, List<string> fieldNames, List<int> fieldValues);

        /// <summary>
        /// Gets List of all details required for creating search profile
        /// </summary>
        /// <returns>returns search profile details list</returns>
        SearchProfileViewModel GetSearchProfileDetails();

        /// <summary>
        /// Gets created Search Profile by provided search profile id 
        /// </summary>
        /// <param name="profileId">Id of search profile to be get</param>
        /// <returns>search profile details</returns>
        SearchProfileViewModel GetSearchProfile(int profileId);

        /// <summary>
        /// Get list of search profiles
        /// </summary>
        /// <param name="expands">expands for search profiles</param>
        /// <param name="filters">filters for search profiles</param>
        /// <param name="sorts">sorts for search profiles</param>
        /// <param name="page">page number for search profiles</param>
        /// <param name="recordPerPage">no. of records for search profiles</param>
        /// <returns>search profiles list</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId and catalogName as parameter ")]
        SearchProfileListViewModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int catalogId, string catalogName);

        /// <summary>
        /// Get list of search profiles
        /// </summary>
        /// <param name="expands">expands for search profiles</param>
        /// <param name="filters">filters for search profiles</param>
        /// <param name="sorts">sorts for search profiles</param>
        /// <param name="pageIndex">page number for search profiles</param>
        /// <param name="pageSize">no. of records for search profiles</param>
        /// <returns>search profiles list</returns>
        SearchProfileListViewModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Deletes Search Profile based on provided searchProfileId
        /// </summary>
        /// <param name="searchProfileId">Id of search profile to be deleted</param>
        /// <param name="isDeletePublishSearchProfile">To determine whether search profile published data needs to be deleted or not.</param>
        /// <param name="errorMessage">error message for errors</param>
        /// <returns>returns status</returns>
        bool DeleteSearchProfile(string searchProfileId, bool isDeletePublishSearchProfile, ref string errorMessage);

        /// <summary>
        /// Get search profile products
        /// </summary>
        /// <param name="model">search profile model</param>
        /// <returns>list of product</returns>
        List<SearchProfileProductViewModel> GetSearchProfileProduct(SearchProfileViewModel model, out string errorMessage);

        /// <summary>
        /// updates the search profile
        /// </summary>
        /// <param name="searchProfileViewModel">searchProfileViewModel</param>
        /// <param name="fieldNames">field names list.</param>
        /// <param name="fieldValues">filed values list</param>
        /// <returns>returns the updated searchProfile view model</returns>
        SearchProfileViewModel Update(SearchProfileViewModel searchProfileViewModel, List<string> fieldNames, List<int> fieldValues);

        /// <summary>
        ///Get Features list by query id.
        /// </summary>
        /// <param name="queryId">query id.</param>
        /// <returns>list of features.</returns>
        List<SearchFeatureViewModel> GetFeaturesByQueryId(int queryId);

        /// <summary>
        /// Get attributes based on provided catalog Id
        /// </summary>
        /// <param name="publishCatalogId"></param>
        /// <returns>catalog specific searchable attributes list</returns>
        SearchAttributesListViewModel GetCatalogBasedAttributes(string associatedAttributes, int publishCatalogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get tabular structure for profile.
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>SearchProfileViewModel</returns>
        SearchProfileViewModel GetTabularStructure(int profileId);

        /// <summary>
        /// Set default search profile.
        /// </summary>
        /// <param name="searchProfileId"></param>
        /// <returns>return updated model</returns>
        bool SetDefaultSearchProfile(int portalId, int searchProfileId, int publishCatalogId);
   

        #region Search Triggers 
        /// <summary>
        /// Gets list of search profile triggers.
        /// </summary>
        /// <returns></returns>
        SearchTriggersListViewModel GetSearchProfileTriggerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int searchProfileId);

        /// <summary>
        /// Create search profile triggers.
        /// </summary>
        /// <returns></returns>
        bool CreateSearchProfileTriggers(SearchTriggersViewModel searchTriggersModel, ref string errorMessage);

        /// <summary>
        /// Get search profile trigger on the basis of searchProfileTriggerId.
        /// </summary>
        /// <param name="searchProfileTriggerId"></param>
        /// <returns></returns>
        SearchTriggersViewModel GetSearchProfileTrigger(int searchProfileTriggerId);

        /// <summary>
        /// Updates search profile triggers.
        /// </summary>
        /// <param name="searchTriggersModel"></param>
        /// <returns></returns>
        bool UpdateSearchProfileTriggers(int searchProfileTriggerId, string data, ref string message);

        /// <summary>
        /// Deletes search profile triggers.
        /// </summary>
        /// <param name="searchProfileTriggerId"></param>
        /// <returns></returns>
        bool DeleteSearchProfileTriggers(string searchProfileTriggerId);

        /// <summary>
        /// Get user profile list
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> GetProfileList(int profileId);
        #endregion

        #region Search Facets
        /// <summary>
        /// Get list of search attributes.
        /// </summary>
        /// <param name="expands">expands for search attributes</param>
        /// <param name="filters">filters for search attributes</param>
        /// <param name="sortCollection">sorts for search attributes</param>
        /// <param name="page">page number for search attributes</param>
        /// <param name="recordPerPage">no. of records for search attributes</param>
        /// <param name="isAssociated">isAssociated is flag to identify if search attribute list is associated to search profile or not.</param>
        /// <returns>search profiles list</returns>
        SearchAttributesListViewModel GetAssociatedUnAssociatedCatalogAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? pageSize, int searchProfileId, bool isAssociated);

        /// <summary>
        /// Associate UnAssociated search attributes to search profile.
        /// </summary>
        /// <param name="searchProfileId">searchProfileId to which attributes to be associated.</param>
        /// <param name="attributeCode">attributeCode to be associated.</param>
        /// <returns>Returns true if associated successfully else return false.</returns>
        bool AssociateAttributesToProfile(int searchProfileId, string attributeCode);

        /// <summary>
        /// UnAssociate search attributes from search profile.
        /// </summary>
        /// <param name="searchProfilesAttributeMappingId">searchProfilesAttributeMappingIds to be deleted.</param>
        /// <param name="errorMessage">set error message.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociateAttributesFromProfile(string searchProfilesAttributeMappingIds, out string errorMessage);
        #endregion

        SearchProfilePortalListViewModel GetSearchProfilePortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage, int searchProfileId);

        bool AssociatePortalToSearchProfile(int searchProfileId, string portalIds, bool isAssociate, out string message);

        /// <summary>
        /// Gets unassociated portal List
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="page">page</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <param name="searchProfileId">searchProfileId</param>
        /// <returns>unassociated portal List</returns>
        StoreListViewModel GetUnAssociatedPortalList(FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage, int searchProfileId);

        /// <summary>
        /// Get Field value list by catalog id
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id</param>
        /// <param name="searchProfileId">search profile id.</param>
        /// <returns>field view model.</returns>
        SearchProfileViewModel GetfieldValuesList(int publishCatalogId, int searchProfileId);

        /// <summary>
        /// To add the attribute code filter which will be used to fetch the default searchable attributes for a search profile.
        /// </summary>
        /// <param name="filters">Filters</param>
        void SetFiltersForDefaultSearchableAttributes(FilterCollection filters);

        /// <summary>
        /// Publish Search Profile based On Search profile Id
        /// </summary>
        /// <param name="searchProfileId">Search Profile Id will be Used to publish Search Profile</param>
        /// <returns>Return True if publish is Successful otherwise false</returns>
        bool PublishSearchProfile(int searchProfileId);

        /// <summary>
        /// To get the catalog list that is not associated with any of the search profiles.
        /// </summary>
        /// <returns>List<AutoComplete></returns>
        List<AutoComplete> GetCatalogListForSearchProfile();
    }
}
