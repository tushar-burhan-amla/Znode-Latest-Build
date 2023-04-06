using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints.SaveForLater;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class SaveForLaterClient : BaseClient, ISaveForLaterClient
    {

        #region Public Methods

        //Create Save for Later template.
        public virtual AccountTemplateModel CreateSaveForLater(AccountTemplateModel accountTemplateModel)
        {
            string endpoint = SaveForLaterEndpoint.CreateSaveForLater();

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = PostResourceToEndpoint<AccountQuoteResponse>(endpoint, JsonConvert.SerializeObject(accountTemplateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountTemplate;
        }


        //Get save for later Template
        public AccountTemplateModel GetSaveForLaterTemplate(int userId, string templateType, ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = SaveForLaterEndpoint.GetSaveForLaterTemplate(userId, templateType);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = GetResourceFromEndpoint<AccountQuoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AccountTemplate;
        }

        //Delete cart item list.
        public virtual bool DeleteCartItem(AccountTemplateModel accountTemplateModel)
        {
            string endpoint = SaveForLaterEndpoint.DeleteCartItem();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(accountTemplateModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Delete all cart line items.
        public virtual bool DeleteAllCartItems(int omsTemplateId, bool isFromSavedCart = false)
        {
            string endpoint = SaveForLaterEndpoint.DeleteAllCartItems(omsTemplateId, isFromSavedCart);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(omsTemplateId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Cart template details by omsTemplateId.
        public virtual AccountTemplateModel GetAccountTemplate(int omsTemplateId, ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = SaveForLaterEndpoint.GetAccountTemplate(omsTemplateId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = GetResourceFromEndpoint<AccountQuoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AccountTemplate;
        }

        #endregion
    }
}


