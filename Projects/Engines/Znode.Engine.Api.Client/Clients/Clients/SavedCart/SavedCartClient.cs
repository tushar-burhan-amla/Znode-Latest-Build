using Newtonsoft.Json;
using System.Net;
using Znode.Engine.Api.Client.Endpoints.SavedCart;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;


namespace Znode.Engine.Api.Client
{
    public class SavedCartClient: BaseClient, ISavedCartClient
    {
        public virtual AccountTemplateModel CreateSavedCart(AccountTemplateModel accountTemplateModel)
        {
            string endpoint = SavedCartEndpoint.CreateSavedCart();

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = PostResourceToEndpoint<AccountQuoteResponse>(endpoint, JsonConvert.SerializeObject(accountTemplateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountTemplate;
        }

        public virtual bool EditSaveCart(AccountTemplateModel templateModel) 
        {
            string endpoint = SavedCartEndpoint.EditSaveCart();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(templateModel), status);
            
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            
            return response.IsSuccess;
        }
        public virtual bool AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId)
        {
            string endpoint = SavedCartEndpoint.AddProductToCartForSaveCart(omsTemplateId, userId, portalId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        public virtual bool EditSaveCartName(string templateName, int templateId)
        {
            string endpoint = SavedCartEndpoint.EditSaveCartName(templateName, templateId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }
    }
}
