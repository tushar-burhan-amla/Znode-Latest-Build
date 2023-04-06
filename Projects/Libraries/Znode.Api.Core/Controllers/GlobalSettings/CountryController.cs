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
    public class CountryController : BaseController
    {
        #region Private Variables
        private readonly ICountryService _service;
        private readonly ICountryCache _cache;
        #endregion

        #region Default Constructor
        public CountryController(ICountryService service)
        {
            _service = service;
            _cache = new CountryCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of all countries.
        /// </summary>
        /// <returns>List of all countries.</returns>
        [ResponseType(typeof(CountryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCountries(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CountryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new CountryListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }
        [Obsolete("Method Not Used")]
        /// <summary>
        /// Get a country.
        /// </summary>
        /// <returns>Returns country.</returns>
        [ResponseType(typeof(CountryResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCountry(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CountryResponse>(data) : CreateNoContentResponse();
            }

            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                var data = new CountryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates country details.
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
                bool isSuccess = _service.UpdateCountry(defaultGlobalConfigListModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }

            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                var data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }
    }
}
