using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Attributes;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class ApplicationSettingsController : BaseController
    {
        #region private variables
        private readonly IApplicationSettingsCache _cache;
        private readonly IApplicationSettingsService _service;
        #endregion

        #region Constructor
        public ApplicationSettingsController(IApplicationSettingsService service)
        {
            _service = service;
            _cache = new ApplicationSettingsCache(_service);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get dynamic grid configuration XML.
        /// </summary>
        /// <param name="itemName">Item name of the XML.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>returns HttpResponseMessage</returns>
        [ResponseType(typeof(ApplicationSettingsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFilterConfigurationXML(string itemName,int? userId=null)
        {
            //Create Response object
            HttpResponseMessage response;
            try
            {
                //Get Data from cache
                string data = _cache.GetFilterConfigurationXML(itemName, RouteUri, RouteTemplate, userId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ApplicationSettingsResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create XML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApplicationSettingListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateNewView([FromBody] ViewModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Save view
                var status = _service.CreateNewView(model);
                if (!Equals(status, null))
                {
                    response = CreateCreatedResponse(new ApplicationSettingListResponse { View = status });
                    response.Headers.Add("Location", GetUriLocation(status.ToString()));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Get dynamic grid configuration XML.
        /// </summary>
        /// <param name="applicationSettingId"></param>
        /// <returns>returns HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateViewSelectedStatus(int applicationSettingId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UpdateViewSelectedStatus(applicationSettingId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete View.
        /// </summary>
        /// <param name="listViewId">listViewId to delete.</param>
        /// <returns>returns HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteView(ParameterModel listViewId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteView(listViewId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Get View by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApplicationSettingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetViewById(int id)
        {
            HttpResponseMessage response;
            try
            {
                var status = _cache.GetView(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(status) ? CreateOKResponse<ApplicationSettingListResponse>(status) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #region XML Editor    
        /// <summary>
        /// Gets a list of Xml configuration
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ApplicationSettingListResponse))]
        [PageIndex, PageSize]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get Data from cache
                string data = _cache.GetApplicationSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ApplicationSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of column names
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <returns></returns>
        [ResponseType(typeof(ApplicationSettingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetColumnList(string entityType, string entityName = "")
        {
            HttpResponseMessage response;

            try
            {
                //Get Data from cache
                string data = _cache.GetColumnList(RouteUri, RouteTemplate, entityType, string.IsNullOrEmpty(Convert.ToString(entityName)) ? string.Empty : entityName);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ApplicationSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create XML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApplicationSettingListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] ApplicationSettingDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Save XmlConfiguration
                var status = _service.SaveXmlConfiguration(model);
                if (IsNotNull(status))
                {
                    response = CreateCreatedResponse(new ApplicationSettingListResponse { CreateStatus = status });
                    response.Headers.Add("Location", GetUriLocation(status.ToString()));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApplicationSettingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion

    }
}
