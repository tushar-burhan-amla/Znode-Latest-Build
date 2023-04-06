using Newtonsoft.Json;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Exceptions;
using Znode.Engine.klaviyo.Client.Endpoints;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.klaviyo.Models.Responses;
using Znode.Engine.Klaviyo.IClient;
using Znode.Libraries.Abstract.Client;

namespace Znode.Engine.klaviyo.Client
{
    public class KlaviyoClient : BaseClient, IKlaviyoClient
    {

        // Get klaviyo details
        public virtual KlaviyoModel GetKlaviyo(int portalId)
        {
            string endpoint = KlaviyoEndpoint.Get(portalId);
            ApiStatus status = new ApiStatus();
            KlaviyoResponse response = GetResourceFromEndpoint<KlaviyoResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Klaviyo;
        }

        // update klaviyo details
        public virtual KlaviyoModel UpdateKlaviyo(KlaviyoModel klaviyoModel)
        {
            string endpoint = KlaviyoEndpoint.Update();
            ApiStatus status = new ApiStatus();
            KlaviyoResponse response = PostResourceToEndpoint<KlaviyoResponse>(endpoint, JsonConvert.SerializeObject(klaviyoModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Klaviyo;
        }

        // To get track api data
        public virtual bool TrackKlaviyo(KlaviyoProductDetailModel klaviyoProductDetailModel)
        {
            string endpoint = KlaviyoEndpoint.Track();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(klaviyoProductDetailModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // To get Identify api data
        public virtual bool IdentifyKlaviyo(IdentifyModel userModel)
        {
            string endpoint = KlaviyoEndpoint.Identify();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // To get Email Provider list data
        public virtual List<EmailProviderModel> GetEmailProviderList()
        {
            string endpoint = KlaviyoEndpoint.GetEmailProviderList();
            ApiStatus status = new ApiStatus();
            KlaviyoResponse response = GetResourceFromEndpoint<KlaviyoResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.EmailProviderModelList;
        }
    }
}
