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
    public class ERPConfiguratorClient : BaseClient, IERPConfiguratorClient
    {
        // Get the list of ERPConfigurator
        public virtual ERPConfiguratorListModel GetERPConfiguratorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of ERP Configurator.
            string endpoint = ERPConfiguratorEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);
            ApiStatus status = new ApiStatus();
            ERPConfiguratorListResponse response = GetResourceFromEndpoint<ERPConfiguratorListResponse>(endpoint, status);
            //check the status of response of type ERP Configurator.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            ERPConfiguratorListModel list = new ERPConfiguratorListModel { ERPConfiguratorList = response?.ERPConfigurator };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Get all ERP Configurator Classes which are not present in database.
        public virtual ERPConfiguratorListModel GetAllERPConfiguratorClassesNotInDatabase()
        {
            string endpoint = ERPConfiguratorEndpoint.GetAllERPConfiguratorClassesNotInDatabase();
            ApiStatus status = new ApiStatus();
            ERPConfiguratorListResponse response = GetResourceFromEndpoint<ERPConfiguratorListResponse>(endpoint, status);
            //check the status of response of type ERP Configurator Classes.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            ERPConfiguratorListModel list = new ERPConfiguratorListModel { ERPConfiguratorList = response?.ERPConfiguratorClasses };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Create ERPConfigurator.
        public virtual ERPConfiguratorModel Create(ERPConfiguratorModel eRPConfiguratorModel)
        {
            string endpoint = ERPConfiguratorEndpoint.Create();
            ApiStatus status = new ApiStatus();
            ERPConfiguratorResponse response = PostResourceToEndpoint<ERPConfiguratorResponse>(endpoint, JsonConvert.SerializeObject(eRPConfiguratorModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ERPConfigurator;
        }

        // Get eRPConfigurator on the basis of eRPConfigurator id.
        public virtual ERPConfiguratorModel GetERPConfigurator(int eRPConfiguratorId)
        {
            string endpoint = ERPConfiguratorEndpoint.GetERPConfigurator(eRPConfiguratorId);
            ApiStatus status = new ApiStatus();
            ERPConfiguratorResponse response = GetResourceFromEndpoint<ERPConfiguratorResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.ERPConfigurator;
        }

        // Update eRPConfigurator data.
        public virtual ERPConfiguratorModel Update(ERPConfiguratorModel eRPConfiguratorModel)
        {
            string endpoint = ERPConfiguratorEndpoint.Update();
            ApiStatus status = new ApiStatus();
            ERPConfiguratorResponse response = PutResourceToEndpoint<ERPConfiguratorResponse>(endpoint, JsonConvert.SerializeObject(eRPConfiguratorModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ERPConfigurator;
        }

        // Delete ERPConfigurator.
        public virtual bool Delete(ParameterModel eRPConfiguratorId)
        {
            string endpoint = ERPConfiguratorEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(eRPConfiguratorId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Enable disable ERPConfigurator on the basis of eRPConfiguratorId.
        public virtual bool EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive)
        {
            string endpoint = ERPConfiguratorEndpoint.EnableDisableERPConfigurator(eRPConfiguratorId,isActive);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(eRPConfiguratorId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Get the class name of active ERP.
        public virtual string GetActiveERPClassName()
        {
            string endpoint = ERPConfiguratorEndpoint.GetActiveERPClassName();
            ApiStatus status = new ApiStatus();
            ERPConfiguratorResponse response = GetResourceFromEndpoint<ERPConfiguratorResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.ActiveERPClassName;
        }

        // Get the class name of ERP defined by user. 
        public virtual string GetERPClassName()
        {
            string endpoint = ERPConfiguratorEndpoint.GetERPClassName();
            ApiStatus status = new ApiStatus();
            ERPConfiguratorResponse response = GetResourceFromEndpoint<ERPConfiguratorResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.ERPClassName;
        }
    }
}
