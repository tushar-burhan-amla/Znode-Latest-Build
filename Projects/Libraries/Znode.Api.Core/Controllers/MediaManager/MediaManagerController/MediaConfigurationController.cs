using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.MediaStorage;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class MediaConfigurationController : BaseController
    {
        #region Private Variables
        private readonly IMediaConfigurationService _service;
        private readonly IMediaConfigurationCache _cache;
        #endregion

        #region Default Constructor
        public MediaConfigurationController(IMediaConfigurationService service)
        {
            _service = service;
            _cache = new MediaConfigurationCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets the list of media server.
        /// </summary>
        /// <returns>Returns list of media server.</returns>
        [HttpGet]
        [ResponseType(typeof(MediaConfigurationResponse))]
        public virtual HttpResponseMessage GetMediaServerList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaServers(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get media configuration.
        /// </summary>
        /// <returns>Returns media configuration.</returns>
        [HttpGet]
        [ResponseType(typeof(MediaConfigurationResponse))]
        public virtual HttpResponseMessage Get()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaConfiguration(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets default media configuration.
        /// </summary>
        /// <returns>Returns default media configuration.</returns>
        [HttpGet]
        [ResponseType(typeof(MediaConfigurationResponse))]
        public virtual HttpResponseMessage GetDefaultMediaConfiguration()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDefaultMediaConfiguration(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create Media configuration.
        /// </summary>
        /// <param name="model">Media configuration model to create.</param>
        /// <returns>Returns created model.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(MediaConfigurationResponse))]
        public virtual HttpResponseMessage Create([FromBody] MediaConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                MediaConfigurationModel mediaConfiguration = _service.Create(model);
                if (mediaConfiguration?.MediaConfigurationId > 0)
                {
                    response = CreateCreatedResponse(new MediaConfigurationResponse { MediaConfiguration = mediaConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(mediaConfiguration.MediaConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.ErrorMessage, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update media configuration.
        /// </summary>
        /// <param name="model">Media configuration model to update.</param>
        /// <returns>Returns updated model.</returns>
        [HttpPut, ValidateModel]
        [ResponseType(typeof(MediaConfigurationResponse))]
        public virtual HttpResponseMessage Update([FromBody] MediaConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                MediaConfigurationModel mediaConfiguration = _service.Update(model);
                if (HelperUtility.IsNotNull(mediaConfiguration))
                {
                    response = CreateOKResponse(new MediaConfigurationResponse { MediaConfiguration = mediaConfiguration, ErrorCode = 0 });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(mediaConfiguration.MediaConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Sync media files 
        /// </summary>
        /// <returns>Sync media.</returns>
        [HttpGet]
        public virtual HttpResponseMessage SyncMedia(string folderName)
        {
            HttpResponseMessage response;
            try
            {
                string className = string.Empty;
                int mediaConfigurationId = 0;
                ServerConnector _connectorobj = GetServerConnection(out className, out mediaConfigurationId);
                //send image to upload
                var obj = _connectorobj.CallConnector(className, MediaStorageAction.Copy, "", folderName);

                _service.InsertSyncMedia(obj as Dictionary<string, long>, mediaConfigurationId);
                response = CreateOKResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;

        }

        /// <summary>
        /// Generate all images as per display setting.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GenerateImages()
        {
            HttpResponseMessage response;
            try
            {
                bool result = _service.GenerateImages();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get media count.
        /// </summary>
        /// <returns>Return count of media.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetMediaCount()
        {
            HttpResponseMessage response;
            try
            {                
                string data = _cache.GetMediaCount(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<StringResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get media list.
        /// </summary>
        /// <returns>List of media</returns>
        [ResponseType(typeof(MediaManagerListResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetMediaListForGenerateImages()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetMediaListData(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }        

        //Gets the server connection
        private ServerConnector GetServerConnection(out string className, out int mediaConfigurationId)
        {
            ServerConnector _connectorobj = null;
            //gets the default server configuration
            var defaultConfiguration = JsonConvert.DeserializeObject<MediaConfigurationResponse>(_cache.GetDefaultMediaConfiguration(RouteUri, RouteTemplate));

            if (HelperUtility.IsNotNull(defaultConfiguration.MediaConfiguration))
            {
                //Sets the server connection
                _connectorobj = new ServerConnector(new FileUploadPolicyModel(defaultConfiguration.MediaConfiguration.AccessKey, defaultConfiguration.MediaConfiguration.SecretKey, defaultConfiguration.MediaConfiguration.BucketName, defaultConfiguration.MediaConfiguration.ThumbnailFolderName, defaultConfiguration.MediaConfiguration.URL, defaultConfiguration.MediaConfiguration.NetworkUrl));
                className = defaultConfiguration.MediaConfiguration.MediaServer.ClassName;
                mediaConfigurationId = defaultConfiguration.MediaConfiguration.MediaConfigurationId;
            }
            else
            {
                //Local
                _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, APIConstant.DefaultMediaFolder, APIConstant.ThumbnailFolderName, string.Empty, string.Empty));
                className = APIConstant.DefaultMediaClassName;
                mediaConfigurationId = defaultConfiguration.MediaConfiguration.MediaConfigurationId;
            }
            return _connectorobj;
        }
    }
}