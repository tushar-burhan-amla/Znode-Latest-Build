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
    public class WebStoreCategoryClient : BaseClient, IWebStoreCategoryClient
    {
        #region Public Methods

        // Gets the list of Categories,SubCategories and associated Products.
        public virtual WebStoreCategoryListModel GetWebStoreCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = WebStoreCategoryEndpoint.GetPublishedCategoryProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            WebStoreCategoryListResponse response = GetResourceFromEndpoint<WebStoreCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreCategoryListModel list = new WebStoreCategoryListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.Categories = response.Categories;              
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion
    }
}
