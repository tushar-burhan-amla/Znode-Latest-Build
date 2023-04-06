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
    public class HighlightClient : BaseClient, IHighlightClient
    {
        //Gets the list of Highlights.
        public virtual HighlightListModel GetHighlight(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetHighlight(expands, filters, sorts, null, null);

        //Gets the list of Highlights with paging.
        public virtual HighlightListModel GetHighlight(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = HighlightEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            HighlightListResponse response = GetResourceFromEndpoint<HighlightListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            HighlightListModel list = new HighlightListModel { HighlightList = response?.Highlights };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Creates highlight.
        public virtual HighlightModel CreateHighlight(HighlightModel highlightModel)
        {
            string endpoint = HighlightEndpoint.Create();

            ApiStatus status = new ApiStatus();
            HighlightResponse response = PostResourceToEndpoint<HighlightResponse>(endpoint, JsonConvert.SerializeObject(highlightModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Highlight;
        }

        // Gets Highlight according to highlightId.
        public virtual HighlightModel GetHighlight(int highlightId, FilterCollection filters, int productId = 0)
        {
            string endpoint = HighlightEndpoint.Get(highlightId, productId);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            HighlightResponse response = GetResourceFromEndpoint<HighlightResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Highlight;
        }

        // Gets Highlight according to highlightCode.
        public virtual HighlightModel GetHighlightByCode(string highlightCode, FilterCollection filters)
        {
            string endpoint = HighlightEndpoint.GetByCode(highlightCode);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            HighlightResponse response = GetResourceFromEndpoint<HighlightResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Highlight;
        }

        // Update highlight.
        public virtual HighlightModel UpdateHighlight(HighlightModel highlightModel)
        {
            string endpoint = HighlightEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            HighlightResponse response = PutResourceToEndpoint<HighlightResponse>(endpoint, JsonConvert.SerializeObject(highlightModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Highlight;
        }

        // Delete highlight.
        public virtual bool DeleteHighlight(ParameterModel highlightId)
        {
            string endpoint = HighlightEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(highlightId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #region Highlight Type

        //Get list of highlight types.
        public virtual HighlightTypeListModel GetHighlightTypeList()
        {
            //Get Endpoint.
            string endpoint = HighlightEndpoint.GetHighlightTypeList();

            //Get response.
            ApiStatus status = new ApiStatus();
            HighlightTypeListResponse response = GetResourceFromEndpoint<HighlightTypeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Highlight type list.
            HighlightTypeListModel highlightTypeList = new HighlightTypeListModel { HighlightTypes = response?.HighlightTypeList, TemplateTokens = response?.TemplateTokens };

            return highlightTypeList;
        }
        #endregion

        #region Highlight Product

        //Get highlight code list.
        public virtual HighlightListModel GetHighlightCodeList(string attributeCode)
        {
            //Get Endpoint.
            string endpoint = HighlightEndpoint.HighlightCodeList(attributeCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            HighlightListResponse response = GetResourceFromEndpoint<HighlightListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //highlight code list.
            HighlightListModel list = new HighlightListModel { HighlightCodes = response?.HighlightCodes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Associate/Unassociate product from highlight. 
        public virtual bool AssociateAndUnAssociateProduct(HighlightProductModel highlightProductModel)
        {
            string endpoint = HighlightEndpoint.AssociateAndUnAssociateProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(highlightProductModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }
        #endregion
    }
}
