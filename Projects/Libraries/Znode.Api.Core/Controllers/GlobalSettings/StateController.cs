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
    public class StateController : BaseController
    {
        #region Private Variables
        private readonly IStateCache _cache;
        #endregion

        #region Default Constructor
        public StateController(IStateService service)
        {
            _cache = new StateCache(service);
        }
        #endregion

        /// <summary>
        /// Gets list of all states.
        /// </summary>
        /// <returns>List of all states.</returns>
        [ResponseType(typeof(StateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetStateList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<StateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new StateListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }
    }
}
