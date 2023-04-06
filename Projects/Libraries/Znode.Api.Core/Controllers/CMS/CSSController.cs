using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Exceptions;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class CSSController : BaseController
    {
        #region Private Variables
        private readonly ICSSCache _cache;
        private readonly ICSSService _service;
        #endregion

        #region Constructor
        public CSSController(ICSSService service)
        {
            _service = service;
            _cache = new CSSCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Create new CSS.
        /// </summary>
        /// <param name="cssModel">CSSModel</param>
        /// <returns>Returns css response.</returns>
        [ResponseType(typeof(CSSResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]CSSModel cssModel)
        {
            HttpResponseMessage response;
            try
            {
                cssModel = _service.CreateCSS(cssModel);
                if (cssModel?.CMSThemeCSSId > 0)
                {
                    response = CreateCreatedResponse(new CSSResponse { CSS = cssModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(cssModel.CMSThemeCSSId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CSSResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CSSResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete CSS.
        /// </summary>
        /// <param name="cssIds">int css id</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel cssIds)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _service.DeleteCSS(cssIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse theme = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(theme);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the list of css associated to Theme
        /// </summary>
        /// <param name="cmsThemeId">Id of Theme.</param>
        /// <returns>Returns css list.</returns>
        [ResponseType(typeof(CSSListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCssListByThemeId(int cmsThemeId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCssListByThemeId(cmsThemeId,RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CSSListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CSSListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CSSListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
