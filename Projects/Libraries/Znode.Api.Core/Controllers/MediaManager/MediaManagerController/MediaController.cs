using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Znode.Libraries.MediaStorage;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Api.Controllers
{
    public class MediaController : BaseController
    {
        #region Private Variables
        private readonly IMediaManagerServices _service;
        private readonly IMediaManagerCache _cache;
        private readonly IMediaConfigurationCache _configurationCache;
        private readonly GlobalMediaDisplaySettingModel _displaySetting;
        #endregion

        #region Default Constructor
        public MediaController(IMediaManagerServices service, IMediaConfigurationService configurationService, IPortalService portalService)
        {
            _service = service;
            _cache = new MediaManagerCache(_service);
            _configurationCache = new MediaConfigurationCache(configurationService);
            GlobalMediaDisplaySettingModel displaySetting = configurationService.GetDefaultMediaConfiguration()?.GlobalMediaDisplaySetting;
            _displaySetting = HelperUtility.IsNull(displaySetting) ? GlobalMediaDisplaySettingModel.GetGlobalMediaDisplaySetting() : displaySetting;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get media list.
        /// </summary>
        /// <returns>Returns list of medias.</returns>  
        [ResponseType(typeof(MediaManagerListResponses))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get media from cache if not in cache call to service and get data.
                string data = _cache.GetMedias(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerListResponses>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerListResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Save media to database.
        /// </summary>
        /// <param name="mediaManagerModel">Media model to be save to database.</param>
        /// <returns>Returns created medias.</returns>
        [ResponseType(typeof(MediaManagerResponses))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody]MediaManagerModel mediaManagerModel)
        {
            HttpResponseMessage response;
            try
            {
                //Save data to database for media's meta data.
                MediaManagerModel data = _service.SaveMedia(mediaManagerModel);
                response = IsNotNull(data) ? CreateCreatedResponse(new MediaManagerResponses { Media = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update media configuration.
        /// </summary>
        /// <param name="mediaManagerModel">Media configuration model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(MediaManagerResponses))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] MediaManagerModel mediaManagerModel)
        {
            HttpResponseMessage response;
            try
            {
                MediaManagerModel mediaModel = _service.Update(mediaManagerModel);
                if (IsNotNull(mediaModel))
                {
                    response = CreateOKResponse(new MediaManagerResponses { Media = mediaModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(mediaModel.MediaId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used get folder tree structure.
        /// </summary>
        /// <returns>Returns media tree.</returns>
        [ResponseType(typeof(MediaManagerTreeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTree()
        {
            HttpResponseMessage response;
            try
            {
                //Get Tree folder structure from database.
                string data = _cache.GetTree(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerTreeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerTreeResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used to add folder to database.
        /// </summary>
        /// <param name="model">Model to rename folder.</param>
        /// <returns>Returns added folder.</returns>
        [ResponseType(typeof(MediaManagerFolderResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AddFolder(MediaManagerFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.AddMedia(model);
                response = data ? CreateOKResponse(new MediaManagerFolderResponse { MediaManagerFolder = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaManagerFolderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerFolderResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used to rename folder to database.
        /// </summary>
        /// <param name="model">>Model to rename folder</param>
        /// <returns>Returns renamed folder.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage RenameFolder(MediaManagerFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.RenameMedia(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get media with specific ids
        /// </summary>
        /// <param name="mediaId">Media Id.</param>
        /// <returns>Returns media by media id.</returns>
        [ResponseType(typeof(MediaManagerResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetMediaByID(int mediaId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaID(mediaId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerResponses>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets the attributes for the specified media.
        /// </summary>
        /// <param name="mediaId">ID of the media.</param>
        /// <returns>List of attributes for the media.</returns>
        [ResponseType(typeof(MediaAttributeValuesListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeValuesById(int mediaId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaAttributeValues(RouteUri, RouteTemplate, mediaId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaAttributeValuesListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaAttributeValuesListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaAttributeValuesListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used to move folder to another folder.
        /// </summary>
        /// <param name="model">MediaManagerMoveFolderModel</param>
        /// <returns>Returns true/false on move folder.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage MoveFolder(MediaManagerMoveFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.MoveFolder(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotPermitted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method used to copy media to another folder,
        /// </summary>
        /// <param name="model">AddMediatoFolder model.</param>
        /// <returns>Returns true/false on move media.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage MoveMedia(AddMediaToFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.MoveMediaToFolder(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used to delete media .
        /// </summary>
        /// <param name="deleteMediaModel">Model containing media ids to be deleted.</param>
        /// <returns>Returns deleted medias.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete(DeleteMediaModel deleteMediaModel)
        {
            HttpResponseMessage response;
            try
            {
                //save data to database for media's meta data.
                string data = _service.DeleteMedia(deleteMediaModel);
                DeleteFromServer(data);
                response = IsNotNull(data) ? CreateOKResponse(new StringResponse { Response = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Update Media Attribute Values.
        /// </summary>
        /// <param name="model">MediaAttributeValuesListModel for update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeValue(MediaAttributeValuesListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update media attribute values.
                bool isUpdated = _service.UpdateMediaAttributeValue(model);
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = isUpdated, ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method used to get attribute family by file extension.
        /// </summary>
        /// <param name="extension">File extension.</param>
        /// <returns>Returns Attribute Family Id by Name.</returns>
        [ResponseType(typeof(MediaAttributeFamilyResponse))]
        public virtual HttpResponseMessage GetAttributeFamilyIdByName(string extension)
        {
            HttpResponseMessage response;
            try
            {
                //get extension with specified extsion from database
                var data = _cache.GetAttributeFamilyIdByName(RouteUri, RouteTemplate, extension);
                response = IsNotNull(data) ? CreateOKResponse<MediaAttributeFamilyResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Creates Media attribute model.
        /// </summary>
        /// <param name="model">Media model to be save to database.</param>
        /// <returns>Returns created attribute values.</returns>
        [ResponseType(typeof(MediaAttributeValueResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateAttributeValue(MediaAttributeValuesModel model)
        {
            HttpResponseMessage response;
            try
            {
                //save data to database for media's meta data.
                MediaAttributeValuesModel data = _service.CreateMediaAttribute(model);
                response = IsNotNull(data) ? CreateCreatedResponse(new MediaAttributeValueResponse { MediaAttributeValue = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaAttributeValueResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaAttributeValueResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Share Media/Folder with different users.
        /// </summary>
        /// <param name="listModel">Share media folder list model.</param>
        /// <returns>Returns share folder.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ShareFolder([FromBody] List<ShareMediaFolderModel> listModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isShared = _service.ShareFolder(listModel);
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = isShared, ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// This method is used to delete media.
        /// </summary>
        /// <returns>Returns allowed extensions.</returns>
        [ResponseType(typeof(MediaManagerResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetAllowedExtensions()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAllowedExtensions(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerResponses>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Generate specific image on edit.
        /// </summary>
        /// <param name="generateImageModel">MediaPath of media.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GenerateImageOnEdit([FromBody] GenerateImageModel generateImageModel)
        {
            HttpResponseMessage response;
            try
            {
                bool result = _service.GenerateImageOnEdit(generateImageModel.Path);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = result });
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
        /// Get media list.
        /// </summary>
        /// <returns>Returns list of medias.</returns>  
        [ResponseType(typeof(MediaManagerListResponses))]
        [HttpGet]
        public virtual HttpResponseMessage MediaList()
        {
            HttpResponseMessage response;
            try
            {
                //Get media from cache if not in cache call to service and get data.
                string data = _cache.GetMedias(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerListResponses>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerListResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get media with specific ids
        /// </summary>
        /// <param name="mediaId">Media Id.</param>
        /// <returns>Returns media by media id.</returns>
        [ResponseType(typeof(MediaDetailResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetMediaDetailsById(int mediaId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaDetailsById(mediaId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaManagerResponses>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaManagerResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get media with specific ids
        /// </summary>
        /// <param name="mediaGuid">Media Id</param>
        /// <returns>Returns media by media id.</returns>
        [ResponseType(typeof(MediaDetailResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetMediaDetailsByGuid(string mediaGuid)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaDetailsByGuid(mediaGuid, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MediaDetailResponses>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MediaDetailResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new MediaDetailResponses { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        private bool DeleteFromServer(string mediaPaths)
        {
            if (!string.IsNullOrEmpty(mediaPaths))
            {
                string className = string.Empty;
                int mediaConfigurationId = 0;
                ServerConnector _connectorobj = GetServerConnection(out className, out mediaConfigurationId);

                //Delete the original file
                object deletedObject = _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPaths, string.Empty);

                //delete the thumbnail file
                _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPaths, _connectorobj.UploadPolicyModel.ThumbnailFolderName);

                //delete image and category file from storesetting folder
                DeleteImageFromStoreFolder(mediaPaths);

                return !Equals(deletedObject, null);

            }
            return false;
        }

        //delete image and category file from storesetting folder
        private bool DeleteImageFromStoreFolder(string mediaPath)
        {
            string className = string.Empty;
            int mediaConfigurationId = 0;
            ServerConnector _connectorobj = GetServerConnection(out className, out mediaConfigurationId);

            GlobalMediaDisplaySettingModel displaySettingList = _displaySetting;

            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxSmallThumbnailWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxLargeWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxSmallWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxThumbnailWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxMediumWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxCrossSellWidth));

            return false;
        }

        //Gets the server connection
        private ServerConnector GetServerConnection(out string className, out int mediaConfigurationId)
        {
            ServerConnector _connectorobj = null;
            //gets the default server configuration
            var defaultConfiguration = JsonConvert.DeserializeObject<MediaConfigurationResponse>(_configurationCache.GetDefaultMediaConfiguration(RouteUri, RouteTemplate));

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
                _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, APIConstant.DefaultMediaFolder, APIConstant.ThumbnailFolderName,string.Empty, string.Empty));
                className = APIConstant.DefaultMediaClassName;
                mediaConfigurationId = defaultConfiguration.MediaConfiguration.MediaConfigurationId;
            }
            return _connectorobj;
        }
    }
}
