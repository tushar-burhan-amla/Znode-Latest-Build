using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ERPConnectorClient : BaseClient, IERPConnectorClient
    {
        //Get ERPConnectorControl List.
        public virtual ERPConnectorControlListModel GetERPConnectorControls(ERPConfiguratorModel erpConfiguratorModel)
        {
            //Get Endpoint
            string endpoint = ERPConnectorEndpoint.GetERPConnectorControls();

            //Get response
            ApiStatus status = new ApiStatus();
            ERPConnectorControlListResponse response = PutResourceToEndpoint<ERPConnectorControlListResponse>(endpoint, JsonConvert.SerializeObject(erpConfiguratorModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ERPConnectorControlListModel list = new ERPConnectorControlListModel { ERPConnectorControlList = response?.ERPConnectorControls };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Method to Save ERP Control Data in json file.
        public virtual ERPConnectorControlListModel CreateERPControlData(ERPConnectorControlListModel erpConnectorControlListModel)
        {
            //Get Endpoint
            string endpoint = ERPConnectorEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ERPConnectorControlListResponse response = PostResourceToEndpoint<ERPConnectorControlListResponse>(endpoint, JsonConvert.SerializeObject(erpConnectorControlListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            ERPConnectorControlListModel list = new ERPConnectorControlListModel { ERPConnectorControlList = response?.ERPConnectorControls };
            list.MapPagingDataFromResponse(response);
            return list;
        }
    }
}
