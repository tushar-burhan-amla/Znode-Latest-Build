using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface ISearchProfileCache
    {
        /// <summary>
        /// Gets search profile details
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>returns search profile details</returns>
        string GetSearchProfileDetails(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets search profile list
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>returns search profile list</returns>
        string GetSearchProfilesList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get SearchProfile on the basis of searchProfileId.
        /// </summary>
        /// <param name="searchProfileId">searchProfileId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Returns SearchProfile.</returns>
        string GetSearchProfile(int searchProfileId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get SearchProfile features the basis of query id.
        /// </summary>
        /// <param name="queryId">query id</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Returns SearchProfile.</returns>
        string GetFeaturesByQueryId(int queryId,string routeUri, string routeTemplate);

        #region Search Triggers
        /// <summary>
        /// Gets search profile trigger list.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>returns search profile trigger list</returns>
        string GetSearchTriggerList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Search Profile Trigger on the basis of searchProfileTriggerId.
        /// </summary>
        /// <param name="searchProfileTriggerId">searchProfileTriggerId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Returns Search Profile Trigger.</returns>
        string GetSearchTrigger(int searchProfileTriggerId, string routeUri, string routeTemplate);
        #endregion

        #region Search Facets
        /// <summary>
        /// Gets search attribute list.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>returns search profile list</returns>
        string GetAssociatedUnAssociatedCatalogAttributes(string routeUri, string routeTemplate);
        #endregion

        /// <summary>
        /// Gets catalog based attributes
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>catalog based attributes</returns>
        string GetCatalogBasedAttributes(ParameterModel associatedAttributes, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Associated portal list
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Associated portal list</returns>
        string SearchProfilePortalList(string routeUri, string routeTemplate);

        /// <summary>
        /// list of Unassociated portals
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>list of Unassociated portals</returns>
        string GetUnAssociatedPortalList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Field Value list by catalt id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id.</param>
        /// <param name="searchProfileId">search profile id.</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>get list of fields for field value.</returns>
        string GetFieldValuesList(int publishCatalogId, int searchProfileId, string routeUri, string routeTemplate);
    }
}
