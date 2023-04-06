using Newtonsoft.Json;
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
    public class SEOSettingClient : BaseClient, ISEOSettingClient
    {

        public virtual PortalSEOSettingModel GetPortalSEOSetting(int portalId)
        {
            string endpoint = SEOSettingEndpoints.GetPortalSEOSettings(portalId);

            ApiStatus status = new ApiStatus();
            PortalSEOSettingResponse response = GetResourceFromEndpoint<PortalSEOSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalSEOSetting;
        }

        public virtual PortalSEOSettingModel CreatePortalSEOSetting(PortalSEOSettingModel model)
        {
            string endpoint = SEOSettingEndpoints.CreatePortalSEOSettings();

            ApiStatus status = new ApiStatus();
            PortalSEOSettingResponse response = PostResourceToEndpoint<PortalSEOSettingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PortalSEOSetting;
        }

        public virtual PortalSEOSettingModel UpdatePortalSEOSetting(PortalSEOSettingModel model)
        {
            string endpoint = SEOSettingEndpoints.UpdatePortalSEOSettings();

            ApiStatus status = new ApiStatus();
            PortalSEOSettingResponse response = PutResourceToEndpoint<PortalSEOSettingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PortalSEOSetting;
        }

        public virtual SEODetailsModel CreateSEODetails(SEODetailsModel model)
        {
            string endpoint = SEOSettingEndpoints.CreateSEODetails();

            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = PostResourceToEndpoint<SEODetailsResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SEODetail;
        }

        public virtual SEODetailsModel UpdateSEODetails(SEODetailsModel model)
        {
            string endpoint = SEOSettingEndpoints.UpdateSEODetails();

            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = PutResourceToEndpoint<SEODetailsResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SEODetail;
        }
        
        public virtual SEODetailsModel GetSEODetailId(int? itemId, int seoTypeId, int localeId, int portalId)
        {
            string endpoint = SEOSettingEndpoints.GetSEODetails(itemId, seoTypeId, localeId, portalId);

            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = GetResourceFromEndpoint<SEODetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SEODetail;
        }

        public virtual SEODetailsModel GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            string endpoint = SEOSettingEndpoints.GetSEODetailsBySEOCode(seoCode, seoTypeId, localeId, portalId);

            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = GetResourceFromEndpoint<SEODetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SEODetail;
        }

        public virtual SEODetailsModel GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId)
        {
            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = GetResourceFromEndpoint<SEODetailsResponse>(SEOSettingEndpoints.GetDefaultSEODetails(seoCode, seoTypeId, localeId, portalId,itemId), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SEODetail;
        }

        public virtual SEODetailsModel GetPublishSEODetail(int itemId, string seoType, int localeId, int portalId, string seoCode)
        {
            string endpoint = SEOSettingEndpoints.GetPublishSEODetail(itemId, seoType, localeId, portalId,seoCode);

            ApiStatus status = new ApiStatus();
            SEODetailsResponse response = GetResourceFromEndpoint<SEODetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SEODetail;
        }
        //Gets the list of Seo details.
        public virtual SEODetailsListModel GetSeoDetails(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetSeoDetails(expands, filters, sorts, null, null);

        //Gets the list of seo details with paging.
        public virtual SEODetailsListModel GetSeoDetails(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = SEOSettingEndpoints.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            SEODetailsListResponse response = GetResourceFromEndpoint<SEODetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SEODetailsListModel list = new SEODetailsListModel { SEODetailsList = response?.SEODetails };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Publish Seo details.
        public PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId)
        {
            string endpoint = SEOSettingEndpoints.Publish(seoCode, portalId, localeId, seoTypeId);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, seoCode, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }
       
        // Gets the list of Publish Product List.
        public virtual PublishProductListModel GetProductsForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SEOSettingEndpoints.GetProductsForSEO();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = GetResourceFromEndpoint<PublishProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel list = new PublishProductListModel { PublishProducts = response?.PublishProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            string endpoint = SEOSettingEndpoints.PublishWithPreview(seoCode, portalId, localeId, seoTypeId, targetPublishState, takeFromDraftFirst);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, seoCode, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        public virtual bool DeleteSeo(int seoTypeId, int portalId, string seoCode = "")
        {
            string endpoint = SEOSettingEndpoints.SeoDelete(seoTypeId, portalId, seoCode);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
