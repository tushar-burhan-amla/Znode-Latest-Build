using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class CMSPageSearchClient : BaseClient, ICMSPageSearchClient
    {

        #region CMS search Index

        //Insert create index data for CMS Page elastic search.
        public virtual CMSPortalContentPageIndexModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            string endpoint = CMSPageSearchEndpoint.InsertCreateCmsPageIndexData();

            ApiStatus status = new ApiStatus();
            CMSPortalContentPageIndexResponse response = PostResourceToEndpoint<CMSPortalContentPageIndexResponse>(endpoint, JsonConvert.SerializeObject(cmsPortalContentPageIndex), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.Created, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CMSPortalContentPageIndex;
        }

        //Get list of search index monitor.
        public virtual CMSSearchIndexMonitorListModel GetCmsPageSearchIndexMonitorList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = CMSPageSearchEndpoint.GetCmsPageSearchIndexMonitorList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            CMSSearchIndexMonitorListResponse response = GetResourceFromEndpoint<CMSSearchIndexMonitorListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            if (HelperUtility.IsNotNull(response))
            {
                CMSSearchIndexMonitorListModel list = new CMSSearchIndexMonitorListModel
                {
                    CMSSearchIndexMonitorList = response.CMSSearchIndexMonitorList,
                    PortalId = response.PortalId,
                    CMSSearchIndexId = response.CMSSearchIndexId
                };
                list.MapPagingDataFromResponse(response);
                return list;
            }
            return new CMSSearchIndexMonitorListModel();
        }

        //Get CMS page index data.
        public virtual CMSPortalContentPageIndexModel GetCmsPageIndexData(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = CMSPageSearchEndpoint.GetCmsPageIndexData();

            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();

            CMSPortalContentPageIndexResponse response = GetResourceFromEndpoint<CMSPortalContentPageIndexResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSPortalContentPageIndexModel cmsPortalIndexData = response?.CMSPortalContentPageIndex;

            return cmsPortalIndexData;
        }

        #endregion

        #region CMS page search request
        //Get Cms pages base on keyword search by user
        public virtual CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestModel keywordSearchModel, FilterCollection filters)
        {
            string endpoint = CMSPageSearchEndpoint.FullTextContentPageSearch();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            CMSKeywordSearchResponse response = PostResourceToEndpoint<CMSKeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(keywordSearchModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return HelperUtility.IsNotNull(response?.CMSSearch) ? response.CMSSearch : new CMSKeywordSearchModel { CMSPages = new List<SearchCMSPageModel>() };
        }

        #endregion
    }
}
