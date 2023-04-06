using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebSiteClient : BaseClient, IWebSiteClient
    {
        #region WebSite Logo
        //Get the Theme Associated Portals.
        public virtual PortalListModel GetPortalList(FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage)
        {
            string endpoint = WebSiteEndpoint.GetPortalList();
            endpoint += BuildEndpointQueryString(null, filters, sortCollection, pageIndex, recordPerPage);

            ApiStatus status = new ApiStatus();
            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the Web Site Logo Details based on Portal Id.
        public virtual WebSiteLogoModel GetWebSiteLogo(int portalId)
        {
            string endpoint = WebSiteEndpoint.GetLogo(portalId);

            ApiStatus status = new ApiStatus();
            WebSiteLogoResponse response = GetResourceFromEndpoint<WebSiteLogoResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.WebSiteLogo;
        }

        //Update Web Site Logo Details for the Portal.
        public virtual WebSiteLogoModel SaveWebSiteLogo(WebSiteLogoModel companyModel)
        {
            string endpoint = WebSiteEndpoint.SaveWebSiteLogo();

            ApiStatus status = new ApiStatus();
            WebSiteLogoResponse response = PutResourceToEndpoint<WebSiteLogoResponse>(endpoint, JsonConvert.SerializeObject(companyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.WebSiteLogo;
        }
        #endregion

        #region Portal Product Page
        //Method to get the list of Portal Product Page associated to store.
        public virtual PortalProductPageModel GetPortalProductPageList(int portalId)
        {
            string endpoint = WebSiteEndpoint.GetPortalProductPage(portalId);

            ApiStatus status = new ApiStatus();
            PortalProductPageResponse response = GetResourceFromEndpoint<PortalProductPageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalProductPage;
        }

        //Assign new pdp template to product type.
        public virtual bool UpdatePortalProductPage(PortalProductPageModel portalProductPageModel)
        {
            //Get endpoint having api url.
            string endpoint = WebSiteEndpoint.UpdatePortalProductPage();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalProductPageModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
        #endregion

        #region Publish and Preview

        //Publish CMS configuration
        public virtual bool Publish(int portalId, string targetPublishState = null, string publishContent = null)
        {
            //Get Endpoint.
            string endpoint = WebSiteEndpoint.Publish(portalId, targetPublishState, publishContent, true);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        #endregion

        //Get the widget id by its code.
        public virtual int GetWidgetIdByCode(string widgetCode)
        {
            string endpoint = WebSiteEndpoint.GetWidgetIdByCode(widgetCode);

            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = GetResourceFromEndpoint<WebStoreWidgetResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.CMSWidgetsId;
        }

        public virtual string GetAssociatedCatalogId(int portalId)
        {
            string endpoint = WebSiteEndpoint.GetAssociatedCatalogId(portalId);
            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.Response;
        }
    }
}
