using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class SavedCartController : BaseController
    {
        #region Private variables readonly
        private readonly IAccountQuoteService _AccountQuoteService;
        #endregion

        #region Public constructor
        public SavedCartController(IAccountQuoteService accountQuoteService)
        {
            _AccountQuoteService = accountQuoteService;
        }
        #endregion

        /// <summary>
        /// Create Saved cart.
        /// </summary>
        /// <param name="shoppingCartTemplateModel">Shopping cart Template model to create saved Cart item.</param>
        /// <returns>Returns Account template model.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSavedCart([FromBody] AccountTemplateModel shoppingCartTemplateModel)
        {
            HttpResponseMessage response;
            try
            {
                AccountTemplateModel accountTemplateModel = _AccountQuoteService.CreateTemplate(shoppingCartTemplateModel);//todo

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

        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage EditSaveCart([FromBody] AccountTemplateModel shoppingCartTemplateModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _AccountQuoteService.EditSaveCart(shoppingCartTemplateModel) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _AccountQuoteService.AddProductToCartForSaveCart(omsTemplateId, userId, portalId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage EditSaveCartName(string templateName, int templateId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _AccountQuoteService.EditSaveCartName(templateName, templateId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}
