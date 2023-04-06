using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class CityController : BaseController
    {
        #region Private Variables
        private readonly ICityCache _cache;
        #endregion

        #region Default Constructor
        public CityController(ICityService service)
        {          
            _cache = new CityCache(service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of all cities.
        /// </summary>
        /// <returns>List of all cities.</returns>
        [ResponseType(typeof(CityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCityList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<CityListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CityListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}
