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
    public class ContainerTemplateClient : BaseClient, IContainerTemplateClient
    {
        //Get the List of Container Template
        public virtual ContainerTemplateListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ContainerTemplateEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ContainerTemplateListResponse response = GetResourceFromEndpoint<ContainerTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ContainerTemplateListModel list = new ContainerTemplateListModel { ContainerTemplates = response?.ContainerTemplates };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Crete Container Template
        public virtual ContainerTemplateModel Create(ContainerTemplateCreateModel model)
        {
            string endpoint = ContainerTemplateEndpoint.Create();

            ApiStatus status = new ApiStatus();
            ContainerTemplateListResponse response = PostResourceToEndpoint<ContainerTemplateListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContainerTemplate;
        }

        //Get Container Template
        public virtual ContainerTemplateModel GetContainerTemplate(string templateCode)
        {
            string endpoint = ContainerTemplateEndpoint.Get(templateCode);

            ApiStatus status = new ApiStatus();
            ContainerTemplateListResponse response = GetResourceFromEndpoint<ContainerTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ContainerTemplate;
        }

        //Update Container Template
        public virtual ContainerTemplateModel Update(ContainerTemplateUpdateModel model)
        {
            string endpoint = ContainerTemplateEndpoint.Update();

            ApiStatus status = new ApiStatus();
            ContainerTemplateListResponse response = PutResourceToEndpoint<ContainerTemplateListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContainerTemplate;
        }

        //Delete Container Template
        public virtual bool Delete(ParameterModel containerTemplateIds)
        {
            string endpoint = ContainerTemplateEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(containerTemplateIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Validate if the Container Template Exist
        public virtual bool IsContainerTemplateExist(string templateCode)
        {
            string endpoint = ContainerTemplateEndpoint.IsContainerTemplateExist(templateCode);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
