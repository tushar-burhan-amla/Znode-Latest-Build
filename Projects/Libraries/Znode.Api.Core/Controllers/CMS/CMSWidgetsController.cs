using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class CMSWidgetsController : BaseController
    {
        #region Private Variables
        private readonly ICMSWidgetsCache _cache;
        private readonly ICMSWidgetsService _service;
        #endregion

        #region Constructor
        public CMSWidgetsController(ICMSWidgetsService service)
        {
            _service = service;
            _cache = new CMSWidgetsCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get CMS Widgets list.
        /// </summary>
        /// <returns>Returns cms widgets list.</returns>
        [ResponseType(typeof(CMSWidgetsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get Data from Cache.
                string data = _cache.List(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSWidgetsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get CMS Widgets Details by Widget Codes.
        /// </summary>
        /// <returns>Returns CMS widgets details list.</returns>
        [ResponseType(typeof(CMSWidgetsListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetWidgetByCodes([FromBody] ParameterModel widgetCodes)
        {
            HttpResponseMessage response;
            try
            {
                //Get the Widget Details by the Widget Code.
                var widgetModelList = _service.GetWidgetByCodes(widgetCodes)?.CMSWidgetsList;

                response = (!Equals(widgetModelList, null) && widgetModelList.Count > 0) ? CreateOKResponse(new CMSWidgetsListResponse { CMSWidgetsList = widgetModelList }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetsListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
