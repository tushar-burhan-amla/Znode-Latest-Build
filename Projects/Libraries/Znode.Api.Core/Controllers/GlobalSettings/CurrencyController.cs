using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Controllers
{
    public class CurrencyController : BaseController
    {
        #region Private Variables
        private readonly ICurrencyService _service;
        private readonly ICurrencyCache _cache;
        #endregion

        #region Default Constructor
        public CurrencyController(ICurrencyService service)
        {
            _service = service;
            _cache = new CurrencyCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of all currencies.
        /// </summary>
        /// <returns>List of all currencies.</returns>
        [ResponseType(typeof(CurrencyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCurrencies(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CurrencyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new CurrencyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a currency.
        /// </summary>
        /// <returns>Currency.</returns>
        [ResponseType(typeof(CurrencyResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCurrency(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CurrencyResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                var data = new CurrencyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update currency details.
        /// </summary>
        /// <param name="defaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>HttpResponse for Default Global Config List Model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]DefaultGlobalConfigListModel defaultGlobalConfigListModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isSuccess = _service.UpdateCurrency(defaultGlobalConfigListModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                var data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of all Culture.
        /// </summary>
        /// <returns>List of all Culture.</returns>
        [ResponseType(typeof(CultureListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCultureList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCulture(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CultureListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new CultureListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a Culture Code.
        /// </summary>
        /// <returns>Currency.</returns>
        [ResponseType(typeof(CultureResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCultureCode()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCultureCode(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CultureResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new CultureResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }

        [ResponseType(typeof(CurrencyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCurrencyCultureList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCurrencyCultureList(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CurrencyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new CurrencyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }
    }
}
