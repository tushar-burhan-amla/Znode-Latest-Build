using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Api.Core.Cache.Cache.SaveForLater;
using Znode.Api.Core.Cache.ICache.ISaveForLater;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers.SaveForLater
{
    public class SaveForLaterController : BaseController
    {

        #region Private variables readonly
        private readonly IAccountQuoteService _AccountQuoteService;
        private readonly ISaveForLaterCache _saveForLaterCache;
        #endregion

        #region Public constructor
        public SaveForLaterController(IAccountQuoteService accountQuoteService)
        {
            _AccountQuoteService = accountQuoteService;
            _saveForLaterCache = new SaveForLaterCache(_AccountQuoteService);
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Create cart for later.
        /// </summary>
        /// <param name="shoppingCartModel">Shopping cart model to create save for later item.</param>
        /// <returns>Returns Account template model.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateCartForLater([FromBody] AccountTemplateModel shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                AccountTemplateModel accountTemplateModel = _AccountQuoteService.CreateTemplate(shoppingCartModel);

                if (HelperUtility.IsNotNull(accountTemplateModel))
                {
                    response = CreateCreatedResponse(new AccountQuoteResponse { AccountTemplate = accountTemplateModel });
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get saved cart for later
        /// </summary>
        /// <param name="userId">LoggedIn user ID</param>
        /// <param name="templateType">Template type</param>
        /// <returns></returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCartForLater(int userId, string templateType)
        {
            HttpResponseMessage response;
            try
            {
                //Gets an account template.
                string data = _saveForLaterCache.GetCartForLater(userId, templateType, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountQuoteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete cart item.
        /// </summary>
        /// <param name="accountTemplateModel">Account Template Model</param>
        /// <returns>if deleted successfully returns true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteCartItem([FromBody] AccountTemplateModel accountTemplateModel)
        {
            HttpResponseMessage response;

            try
            {
                response = _AccountQuoteService.DeleteCartItem(accountTemplateModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete all cart items.
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <returns>if deleted successfully returns true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteAllCartItems(int omsTemplateId, bool isFromSavedCart = false)
        {
            HttpResponseMessage response;

            try
            {
                response = _AccountQuoteService.DeleteAllCartitemsForLater(omsTemplateId, isFromSavedCart) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets an Cart template.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <returns>return account template model.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCartTemplate(int omsTemplateId)
        {
            HttpResponseMessage response;
            try
            {
                //Gets an account template.
                string data = _saveForLaterCache.GetCartTemplate(omsTemplateId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountQuoteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }



        #endregion
    }
}
