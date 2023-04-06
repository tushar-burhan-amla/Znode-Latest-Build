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
    public class CMSWidgetsClient : BaseClient ,ICMSWidgetsClient
    {
        #region Public Methods
        //Get CMS Widget list.
        public virtual CMSWidgetsListModel List(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetsEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CMSWidgetsListResponse response = GetResourceFromEndpoint<CMSWidgetsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //check the status of response of type CMS Widgets type.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSWidgetsListModel list = new CMSWidgetsListModel { CMSWidgetsList = response?.CMSWidgetsList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get CMS Widget by widgetCodes.
        public virtual CMSWidgetsListModel GetWidgetByCodes(ParameterModel widgetCodes)
        {
            string endpoint = CMSWidgetsEndpoint.GetWidgetByCodes();

            ApiStatus status = new ApiStatus();
            CMSWidgetsListResponse response = PostResourceToEndpoint<CMSWidgetsListResponse>(endpoint, JsonConvert.SerializeObject(widgetCodes), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //check the status of response of type CMS Widgets.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSWidgetsListModel list = new CMSWidgetsListModel { CMSWidgetsList = response?.CMSWidgetsList };
            return list;
        }
        #endregion
    }
}
