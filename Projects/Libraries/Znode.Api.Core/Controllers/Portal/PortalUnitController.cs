using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class PortalUnitController : BaseController
    {
        #region Private Variables
        private readonly IPortalUnitCache _cache;
        private readonly IPortalUnitService _service;
        #endregion

        #region Constructor
        public PortalUnitController(IPortalUnitService service)
        {
            _service = service;
            _cache = new PortalUnitCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get PortalUnit details for portal by portal Id.
        /// </summary>
        /// <param name="portalId">Int PortalId to get PortalUnit details</param>
        /// <returns>PortalUnit details as response</returns>
        [ResponseType(typeof(PortalUnitResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalUnit(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalUnit(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalUnitResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                PortalUnitResponse portalUnitResponse = new PortalUnitResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalUnitResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Update PortalUnit details.
        /// </summary>
        /// <param name = "portalUnitModel" > portalUnitModel to Create/update PortalUnit</param>
        /// <returns>PortalUnit Response</returns>
        [ResponseType(typeof(PortalUnitResponse))]
        [HttpPut]
        [ValidateModel]
        public virtual HttpResponseMessage CreateUpdatePortalUnit([FromBody] PortalUnitModel portalUnitModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.CreateUpdatePortalUnit(portalUnitModel);
                response = isUpdated ? CreateOKResponse(new PortalUnitResponse { PortalUnit = portalUnitModel }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                PortalUnitResponse portalUnitResponse = new PortalUnitResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalUnitResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
    #endregion
}

