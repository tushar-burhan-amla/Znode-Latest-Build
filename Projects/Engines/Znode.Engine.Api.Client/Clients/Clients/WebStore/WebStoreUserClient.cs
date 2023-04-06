using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebStoreUserClient : BaseClient, IWebStoreUserClient
    {
        //Create Account address.
        public virtual AddressModel CreateAccountAddress(AddressModel addressModel)
        {
            string endpoint = WebStoreUserEndpoint.CreateUserAddress();

            ApiStatus status = new ApiStatus();
            WebStoreAccountResponse response = PostResourceToEndpoint<WebStoreAccountResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountAddress;
        }

        //Update Account address
        public virtual AddressModel UpdateAccountAddress(AddressModel addressModel)
        {
            string endpoint = WebStoreUserEndpoint.UpdateUserAddress();

            ApiStatus status = new ApiStatus();
            WebStoreAccountResponse response = PutResourceToEndpoint<WebStoreAccountResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountAddress;
        }

        //Get address list for user.
        public virtual List<AddressModel> GetUserAddressList(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = WebStoreUserEndpoint.GetUserAddressList();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreAccountResponse response = GetResourceFromEndpoint<WebStoreAccountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.UserAddressList;
        }

        //Get Address information on the basis of Address Id.
        public virtual AddressModel GetAddress(int? addressId)
        {
            string endpoint = WebStoreUserEndpoint.GetAddress(addressId);

            ApiStatus status = new ApiStatus();
            WebStoreAccountResponse response = GetResourceFromEndpoint<WebStoreAccountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AccountAddress;
        }

        //Delete Address on the basis of Address Id and UserId.
        public virtual bool DeleteAddress(int? addressId, int? userId)
        {
            //Get Endpoint.
            string endpoint = WebStoreUserEndpoint.DeleteAddress(addressId, userId);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<BaseResponse>(endpoint, status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);
            return deleted;
        }
    }
}
