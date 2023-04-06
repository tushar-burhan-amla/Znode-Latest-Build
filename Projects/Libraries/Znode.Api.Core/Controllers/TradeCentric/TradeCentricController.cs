using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Engine.Exceptions;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class TradeCentricController : BaseController
    {
        #region Private Variables 
        private readonly ITradeCentricCache _tradeCentricCache;
        private readonly ITradeCentricService _tradeCentricService;
        #endregion

        #region Constructor
        public TradeCentricController(ITradeCentricService tradeCentricService)
        {
            _tradeCentricService = tradeCentricService;
            _tradeCentricCache = new TradeCentricCache(tradeCentricService);
        }
        #endregion

        #region Public

        /// <summary>
        /// Get the TradeCentric User.
        /// </summary>
        /// <param name="userId">User id of the TradeCentric User.</param>
        /// <returns>Return TradeCentric User.</returns>
        [ResponseType(typeof(TradeCentricUserResponse))]
        [HttpGet]
        public HttpResponseMessage GetTradeCentricUser(int userId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _tradeCentricCache.GetTradeCentricUser(userId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TradeCentricUserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TradeCentricUserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TradeCentricUserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Save TradeCentric User details.
        /// </summary>
        /// <param name="tradeCentricUserModel">TradeCentricUserModel</param>
        /// <returns>bool</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel)
        {
            HttpResponseMessage response;
            try
            {

                bool result = _tradeCentricService.SaveTradeCentricUser(tradeCentricUserModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = result });
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Transfer cart.
        /// </summary>
        /// <param name="tradeCentricUserModel">tradeCentricUserModel</param>
        /// <returns>TradeCentricCartTransferResponse model</returns>
        [ResponseType(typeof(TradeCentricCartTransferResponse))]
        [HttpPost]
        public HttpResponseMessage TransferCart(TradeCentricUserModel tradeCentricUserModel)
        {
            HttpResponseMessage response;
            try
            {
                var data = _tradeCentricService.TransferTradeCentricCart(tradeCentricUserModel);
                response = !Equals(data, null) ? CreateOKResponse<TradeCentricCartTransferResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
    }
}
