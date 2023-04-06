using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class DynamicContentController : BaseController
    {
        #region Private Variables

        private readonly IDynamicContentCache _dynamicContentCache;

        #endregion

        public DynamicContentController(IDynamicContentService dynamicContentService)
        {
            _dynamicContentCache = new DynamicContentCache(dynamicContentService);
        }

        [ResponseType(typeof(EditorFormatsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetEditorFormats(int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _dynamicContentCache.GetEditorFormats(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EditorFormatsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new EditorFormatsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EditorFormatsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}
