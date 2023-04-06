using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.Search
{
    public interface ICMSContentPageSearchService
    {
        /// <summary>
        /// Get Portal Latest Version Id.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="revisionType">revision type of portal.</param>
        /// <param name="localeId">local id of portal.</param>
        /// <returns>Latest version ID of webstore.</returns>
        int GetLatestVersionId(int portalId, string revisionType, int localeId = 0);

        /// <summary>
        /// Get all CMS pages for creating index.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="versionId">portal version Id</param>
        /// <param name="start">Start index of the pages list.</param>
        /// <param name="pageLength">Page length of the pages list.</param>
        /// <param name="indexstartTime">current create index start time</param>
        /// <param name="localeId">local id of page</param>
        /// <param name="totalCount">Total count of the pages.</param>
        /// <returns>All the pages according to portal Id.</returns>
        List<SearchCMSPages> GetAllCmsPages(int portalId, int versionId, int start, int pageLength, long indexStartTime, int localeId, out decimal totalPages);

        /// <summary>
        ///  Get the latest version ids of portal from local ids.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="revisionType"></param>
        /// <param name="localIds"></param>
        /// <returns>List of WebStoreEntity which has Version Ids</returns>     
        List<ZnodePublishWebstoreEntity> GetVersionIds(int portalId, string revisionType, List<LocaleModel> localIds);

        /// <summary>
        /// Get the list of all Blog and News of all version and locales of particular store.
        /// </summary>
        /// <param name="searchCmsPagesParameterModel">Request model searchCmsPagesParameterModel</param>
        /// <returns>List of Blog and News of type SearchCMSPages.</returns> 
        List<SearchCMSPages> GetAllBlogsNews(SearchCMSPagesParameterModel searchCmsPagesParameterModel);

        /// <summary>
        /// Get the list of all CMS pages of all version and locales of particular store.
        /// </summary>
        /// <param name="searchCmsPagesParameterModel">Request model searchCmsPagesParameterModel</param>
        /// <param name="webStoreEntitylist">WebStore entity list</param>
        /// <returns>List of all CMS pages Of all version and locales of type SearchCMSPages.</returns> 
        List<SearchCMSPages> GetAllCMSPagesOfAllVersionAndLocales(SearchCMSPagesParameterModel searchCmsPagesParameterModel, List<ZnodePublishWebstoreEntity> webStoreEntitylist);
    }
}
