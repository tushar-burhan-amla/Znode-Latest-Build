using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISearchProfileClient : IBaseClient
    {
        /// <summary>
        /// Create Search Profile
        /// </summary>
        /// <param name="attributeModel">model search profile details</param>
        /// <returns>created search profile model</returns>
        SearchProfileModel Create(SearchProfileModel searchProfileModel);
       
        /// <summary>
        /// Gets Search profile details
        /// </summary>
        /// <returns>Search profile details</returns>
        SearchProfileModel GetSearchProfileDetails(FilterCollection filters);

        /// <summary>
        /// Gets search profile list
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>returns search profile list</returns>
        SearchProfileListModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Deletes search profile
        /// </summary>
        /// <param name="searchProfileId">searchProfileId</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSearchProfile(ParameterModel searchProfileId);

        KeywordSearchModel GetSearchProfileProducts(SearchProfileModel model, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection);

        /// <summary>
        /// Gets search profile based on id
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>search Profile Model</returns>
        SearchProfileModel GetSearchProfile(int profileId);

        /// <summary>
        /// Updates search profile
        /// </summary>
        /// <param name="searchProfileModel">searchProfileModel</param>
        /// <returns>returns updated search profile model</returns>
        SearchProfileModel UpdateSearchProfile(SearchProfileModel searchProfileModel);

        /// <summary>
        /// Get features list by query id.
        /// </summary>
        /// <param name="queryId">query id</param>
        /// <returns>list of features</returns>
        List<SearchFeatureModel> GetFeaturesByQueryId(int queryId);

        /// <summary>
        /// Set default search profile.
        /// </summary>
        /// <param name="portalSearchProfileModel"></param>
        /// <returns>return updated model</returns>
        bool SetDefaultSearchProfile(PortalSearchProfileModel portalSearchProfileModel);

        #region Search Triggers 
        /// <summary>
        /// Gets list of search profile triggers.
        /// </summary>
        /// <returns></returns>
        SearchTriggersListModel GetSearchProfileTriggerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create search profile triggers.
        /// </summary>
        /// <returns></returns>
        bool CreateSearchProfileTriggers(SearchTriggersModel searchTriggersModel);

        /// <summary>
        /// Get search profile trigger on the basis of searchProfileTriggerId.
        /// </summary>
        /// <param name="searchProfileTriggerId"></param>
        /// <returns></returns>
        SearchTriggersModel GetSearchProfileTriggers(int searchProfileTriggerId);

        /// <summary>
        /// Updates search profile triggers.
        /// </summary>
        /// <param name="searchTriggersModel"></param>
        /// <returns></returns>
        bool UpdateSearchProfileTriggers(SearchTriggersModel searchTriggersModel);

        /// <summary>
        /// Deletes search profile triggers.
        /// </summary>
        /// <param name="searchProfileTriggerId"></param>
        /// <returns></returns>
        bool DeleteSearchProfileTriggers(ParameterModel searchProfileTriggerId);
        #endregion

        #region Search Facets
        /// <summary>
        /// Gets search attribute list
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>returns search attribute list</returns>
        SearchAttributesListModel GetAssociatedUnAssociatedCatalogAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate UnAssociated search attributes to search profile.
        /// </summary>
        /// <param name="searchAttributesModel">searchAttributesModel.</param>
        /// <returns>Returns true if associated successfully else return false.</returns>
        bool AssociateAttributesToProfile(SearchAttributesModel searchAttributesModel);

        /// <summary>
        /// UnAssociate search attributes from search profile.
        /// </summary>
        /// <param name="profileIds">model containing profileIds to be deleted.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociateAttributesFromProfile(ParameterModel profileIds);
        #endregion

        /// <summary>
        /// returns searchable attributes based on catalogId
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        SearchAttributesListModel GetCatalogBasedAttributes(ParameterModel associatedAttributes, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        SearchProfilePortalListModel GetSearchProfilePortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);
        bool AssociatePortalToSearchProfile(SearchProfileParameterModel parameterModelUserProfile);

        /// <summary>
        /// Get UnAssociated Portal List
        /// </summary>
        /// <param name="p"></param>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="page"></param>
        /// <param name="recordPerPage"></param>
        /// <returns>UnAssociated Portal List</returns>
        PortalListModel GetUnAssociatedPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        ///Get field Value List by Catalog id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id</param>
        /// <param name="searchProfileId">search profile id.</param>
        /// <returns>Search profile model.</returns>
        SearchProfileModel GetfieldValuesList(int publishCatalogId, int searchProfileId);

        /// <summary>
        /// Publish Search Profile
        /// </summary>
        /// <param name="searchProfileId">Search Profile Id is used to select the profile which need to be publish</param>
        /// <returns>return True if publish is successfull otherwise False </returns>
        bool PublishSearchProfile(int searchProfileId);

        /// <summary>
        /// To get the catalog list that is not associated with any of the search profiles.
        /// </summary>
        /// <returns>Catalog list</returns>
        TypeaheadResponselistModel GetCatalogListForSearchProfile();

    }
}
