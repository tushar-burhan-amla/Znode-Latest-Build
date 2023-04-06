using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class TradeCentricClient : BaseClient, ITradeCentricClient
    {
        //Get tradecentric user details.
        public virtual TradeCentricUserModel GetTradeCentricUser(int userId)
        {
            //Create Endpoint to get a tradecentric user details.
            string endpoint = TradeCentricEndpoint.GetTradeCentricUser(userId);

            ApiStatus status = new ApiStatus();
            TradeCentricUserResponse response = GetResourceFromEndpoint<TradeCentricUserResponse>(endpoint, status);

            //Check the status of response of tradecentric user details.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TradeCentricUser;
        }

        //Save tradecentric user details.
        public virtual bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel)
        {
            //Create Endpoint to save tradecentric user.
            string endpoint = TradeCentricEndpoint.SaveTradeCentricUser();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(tradeCentricUserModel), status);

            //Check the status of response of save tradecentric user.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Transfer cart.
        public virtual string TransferCart(TradeCentricUserModel tradeCentricUserModel)
        {
            //Create Endpoint to transfer cart.
            string endpoint = TradeCentricEndpoint.TransferCart();

            ApiStatus status = new ApiStatus();
            TradeCentricCartTransferResponse response = PostResourceToEndpoint<TradeCentricCartTransferResponse>(endpoint, JsonConvert.SerializeObject(tradeCentricUserModel), status);

            //Check the status of response of transfer cart.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.RedirectUrl;
        }
    }
}
