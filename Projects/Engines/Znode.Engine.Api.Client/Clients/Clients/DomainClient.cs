using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class DomainClient : BaseClient, IDomainClient
    {
        //Method to get domain list.
        public virtual DomainListModel GetDomains(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DomainEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DomainListResponse response = GetResourceFromEndpoint<DomainListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DomainListModel list = new DomainListModel { Domains = response?.Domains };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Method to get domain by domain id.
        public virtual DomainModel GetDomain(int domainId)
        {
            string endpoint = DomainEndpoint.Get(domainId);
            ApiStatus status = new ApiStatus();
            DomainResponse response = GetResourceFromEndpoint<DomainResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.Domain;
        }

        //Method to create domain.
        public virtual DomainModel CreateDomain(DomainModel domainModel)
        {
            string endpoint = DomainEndpoint.Create();
            ApiStatus status = new ApiStatus();
            DomainResponse response = PostResourceToEndpoint<DomainResponse>(endpoint, JsonConvert.SerializeObject(domainModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Domain;
        }

        //Method to update domain.
        public virtual DomainModel UpdateDomain(DomainModel domainModel)
        {
            string endpoint = DomainEndpoint.Update();
            ApiStatus status = new ApiStatus();
            DomainResponse response = PutResourceToEndpoint<DomainResponse>(endpoint, JsonConvert.SerializeObject(domainModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Domain;
        }

        //Method to delete domain.
        public virtual bool DeleteDomain(ParameterModel domainIds)
        {
            string endpoint = DomainEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(domainIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Method to enable/disable domain.
        public virtual bool EnableDisableDomain(DomainModel domainModel)
        {
            //Get Endpoint.
            string endpoint = DomainEndpoint.EnableDisableDomain();

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(domainModel), status);

            //Check status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
