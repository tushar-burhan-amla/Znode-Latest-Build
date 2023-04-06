using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Api.Cache;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreAccountController : BaseController
    {
        #region Private Variables
        private readonly IAccountService _service;
        private readonly IWebStoreAccountCache _cache;
        #endregion

        #region Constructor
        public WebStoreAccountController(IAccountService service)
        {
            _service = service;
            _cache = new WebStoreAccountCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get list of user address.
        /// </summary>
        /// <returns>Returns list of user address.</returns>   
        [ResponseType(typeof(WebStoreAccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUserAddressList()
        {
            HttpResponseMessage response;
            try
            {
                //Get data from cache
                string data = _cache.GetUserAddressList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create account address.
        /// </summary>
        /// <param name="model">AddressModel.</param>
        /// <returns>Returns created address model.</returns>
        [ResponseType(typeof(WebStoreAccountResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateAccountAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;

            try
            {
                AddressModel address = _service.CreateWebStoreAccountAddress(model);
                response = HelperUtility.IsNotNull(address) ? CreateCreatedResponse(new WebStoreAccountResponse { AccountAddress = address }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get address information by address id.
        /// </summary>
        /// <param name="addressId">Id of address to get address information.</param>
        /// <returns>Returns address info by address id.</returns>
        [ResponseType(typeof(WebStoreAccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAddress(int addressId)
        {
            HttpResponseMessage response;
            try
            {
                AddressModel address = _service.GetAddress(addressId);
                response = HelperUtility.IsNotNull(address) ? CreateOKResponse(new WebStoreAccountResponse { AccountAddress = address }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update account address.
        /// </summary>
        /// <param name="model">AddressModel.</param>
        /// <returns>Returns updated address model.</returns>
        [ResponseType(typeof(WebStoreAccountResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateAccountAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UpdateWebStoreAccountAddress(model) ? CreateCreatedResponse(new WebStoreAccountResponse { AccountAddress = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AccountAddressId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete address on the basis of addressId and userId.
        /// </summary>       
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(BaseResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage DeleteAddress(int? addressId, int? userId)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.DeleteAddress(addressId, userId) ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
