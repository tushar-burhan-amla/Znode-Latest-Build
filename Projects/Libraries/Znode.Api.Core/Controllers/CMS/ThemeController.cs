using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class ThemeController : BaseController
    {
        #region Private Variables
        private readonly IThemeCache _cache;
        private readonly IThemeService _service;
        #endregion

        #region Constructor
        public ThemeController(IThemeService service)
        {
            _service = service;
            _cache = new ThemeCache(_service);
        }
        #endregion

        #region Public Methods
        #region Theme Configuration
        /// <summary>
        /// Get the list of theme.
        /// </summary>
        /// <returns>Returns list of theme.</returns>
        [ResponseType(typeof(ThemeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetThemes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ThemeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create theme.
        /// </summary>
        /// <param name="model">Theme model to create.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(ThemeResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] ThemeModel model)
        {
            HttpResponseMessage response;
            try
            {
                ThemeModel theme = _service.CreateTheme(model);

                if (HelperUtility.IsNotNull(theme))
                {
                    response = CreateCreatedResponse(new ThemeResponse { Theme = theme });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(theme.CMSThemeId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get theme by cmsThemeId.
        /// </summary>
        /// <param name="cmsThemeId">CMS Theme Id to get theme details.</param>
        /// <returns>Returns theme details.</returns>
        [ResponseType(typeof(ThemeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTheme(int cmsThemeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetTheme(cmsThemeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ThemeResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            return response;
        }

        /// <summary>
        /// Update theme.
        /// </summary>
        /// <param name="themeModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(ThemeResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateTheme([FromBody] ThemeModel themeModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update theme.
                response = _service.UpdateTheme(themeModel) ? CreateCreatedResponse(new ThemeResponse { Theme = themeModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(themeModel.CMSThemeId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ThemeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete theme which is not associated to portal
        /// </summary>
        /// <param name="themeId">Theme Id to delete theme.</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete(ParameterModel themeId)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _service.DeleteTheme(themeId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Associate Store

        /// <summary>
        /// Associate Store to CMS Theme.
        /// </summary>
        /// <param name="listModel">PricePortalModel.</param>
        /// <returns>Returns associated store to CMS Theme  if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateStore([FromBody] PricePortalListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateStore(listModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Remove associated stores.
        /// </summary>
        /// <param name="cmsPortalThemeId">cmsPortalThemeId contains data to remove.</param>
        /// <returns>Returns true if removed successfully else returns  false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage RemoveAssociatedStores([FromBody] ParameterModel cmsPortalThemeId)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.RemoveAssociatedStores(cmsPortalThemeId) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region CMS Widgets

        /// <summary>
        /// Get cms areas for cms theme.
        /// </summary>
        /// <returns> Return  CMS area list.</returns>
        [ResponseType(typeof(CMSAreaListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAreas()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAreas(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSAreaListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSAreaListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
        #endregion
    }
}
