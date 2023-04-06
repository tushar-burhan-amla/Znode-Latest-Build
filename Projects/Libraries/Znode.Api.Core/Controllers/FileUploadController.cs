using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Enum;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.MediaStorage;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace Znode.Engine.Api.Controllers
{

    public class FileUploadController : BaseController
    {
        #region Private Variables
        private readonly IMediaManagerServices _service;
        private readonly IMediaManagerCache _cache;
        private readonly IMediaConfigurationCache _configurationCache;
        private MediaConfigurationResponse _mediaConfiguration;
        private GlobalMediaDisplaySettingModel _displaySetting;
        #endregion

        #region Default Constructor
        public FileUploadController(IMediaManagerServices service, IMediaConfigurationService configurationService, IPortalService portalService)
        {
            _service = service;
            _cache = new MediaManagerCache(_service);
            _configurationCache = new MediaConfigurationCache(configurationService);
            _mediaConfiguration = new MediaConfigurationResponse();
        }
        #endregion

        /// <summary>
        /// Upload files as async
        /// </summary>
        /// <returns>File uploaded status</returns>
        [ResponseType(typeof(FileUploadResponse))]
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<List<FileUploadResponse>> PostAsync()
        {
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    //gets the default server configuration and global media display setting
                    _mediaConfiguration = JsonConvert.DeserializeObject<MediaConfigurationResponse>(_configurationCache.GetDefaultMediaConfiguration(RouteUri, RouteTemplate));

                    GlobalMediaDisplaySettingModel displaySetting = _mediaConfiguration?.MediaConfiguration?.GlobalMediaDisplaySetting;
                    _displaySetting = HelperUtility.IsNull(displaySetting) ? GlobalMediaDisplaySettingModel.GetGlobalMediaDisplaySetting() : displaySetting;

                    string uploadPath = HttpContext.Current.Server.MapPath("~/Data/Media/TempImage");

                    CheckDirectoryExistOrCreate(uploadPath);

                    var _queryString = Request.GetQueryNameValuePairs();
                    MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);

                    try
                    {
                        await Request.Content.ReadAsMultipartAsync(streamProvider);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    }

                    List<FileUploadResponse> messages = new List<FileUploadResponse>();
                    foreach (var file in streamProvider.FileData)
                    {
                        string fileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                        FileInfo fi = new FileInfo(file.LocalFileName);

                        if (Convert.ToBoolean(GetKeyPairValue(_queryString, "isMediaReplace")))
                        {
                            fileName = Convert.ToString(GetKeyPairValue(_queryString, "filename"));
                            GetStatus(messages, fi, Convert.ToInt32(GetKeyPairValue(_queryString, "folderid")), file.Headers.ContentType.MediaType, true, fileName, Convert.ToBoolean(GetKeyPairValue(_queryString, "isMediaReplace")), Convert.ToInt32(GetKeyPairValue(_queryString, "mediaId")));
                        }
                        else
                            GetStatus(messages, fi, Convert.ToInt32(GetKeyPairValue(_queryString, "folderid")), file.Headers.ContentType.MediaType, Convert.ToBoolean(GetKeyPairValue(_queryString, "isreplace")), fileName);
                    }
                    return messages;
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

        public bool IsValidImage(string filename)
        {
            try
            {
                using (var bmp = new Bitmap(filename))

                    return true;
            }
            catch (Exception)
            {
                ZnodeLogging.LogMessage("Failed to upload as the image " + filename + " is corrupted.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return false;
            }
        }

        [ResponseType(typeof(FileUploadResponse))]
        [HttpPost]
        public virtual async Task<List<FileUploadResponse>> Remove()
        {
            List<FileUploadResponse> response = new List<FileUploadResponse>();
            try
            {
                dynamic parameters = await Request.Content.ReadAsAsync<JObject>();
                var mediaids = parameters.MediaIds.Value;
                string data = _service.DeleteMedia(new DeleteMediaModel { MediaIds = Convert.ToString(mediaids) });
                DeleteFromServer(data);
                response.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Removed), FileName = "", MediaId = Convert.ToString(mediaids) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                response.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Error) });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                response.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Error) });
            }
            return response;
        }

        /// <summary>
        /// Upload files as async
        /// </summary>
        /// <returns>File uploaded status</returns>
        [ResponseType(typeof(FileUploadResponse))]
        [HttpPost]
        public virtual async Task<FileUploadResponse> UploadPODocument()
        {
            try
            {
                FileUploadResponse messages = null;
                if (Request.Content.IsMimeMultipartContent())
                {
                    //Get the query string parameters.
                    var _queryString = Request.GetQueryNameValuePairs();

                    //Get file path to upload.
                    string filePath = GetKeyPairValue(_queryString, "filePath");
                    string uploadPath = HttpContext.Current.Server.MapPath(filePath);
                    CheckDirectoryExistOrCreate(uploadPath);
                    //Upload file.
                    MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);

                    try
                    {
                        await Request.Content.ReadAsMultipartAsync(streamProvider);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    }

                    var fileStream = streamProvider.FileData?.FirstOrDefault();

                    return messages = new FileUploadResponse { FileName = fileStream?.LocalFileName.Substring(fileStream.LocalFileName.LastIndexOf('\\') + 1) };

                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

        //Remove Uploaded Po document.
        [ResponseType(typeof(FileUploadResponse))]
        [HttpPost]
        public virtual FileUploadResponse RemovePoDocument()
        {
            try
            {
                FileUploadResponse messages = null;
                if (Request.Content.IsMimeMultipartContent())
                {
                    //Get the query string parameters.
                    var _queryString = Request.GetQueryNameValuePairs();
                    //Get file path to upload.
                    string filePath = GetKeyPairValue(_queryString, "filePath");
                    string fileName = GetKeyPairValue(_queryString, "file");
                    string deletePath = Path.Combine(HttpContext.Current.Server.MapPath(filePath), fileName);
                    bool IsDocumentRemove = false;
                    if (File.Exists(deletePath))
                    {
                        try
                        {
                            File.Delete(deletePath);
                            IsDocumentRemove = true;
                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                            IsDocumentRemove = false;
                        }
                    }

                    return messages = new FileUploadResponse { IsDocumentRemove = IsDocumentRemove };
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        /// Upload files as async
        /// </summary>
        /// <returns>File uploaded status</returns>
        [ResponseType(typeof(FileUploadResponse))]
        [HttpPost]
        public virtual async Task<FileUploadResponse> UploadFormDocument()
        {
            try
            {
                FileUploadResponse messages = null;
                if (Request.Content.IsMimeMultipartContent())
                {
                    //Get the query string parameters.
                    var _queryString = Request.GetQueryNameValuePairs();

                    //Get file path to upload.
                    string filePath = GetKeyPairValue(_queryString, "filePath");
                    string uploadPath = HttpContext.Current.Server.MapPath(filePath);
                    CheckDirectoryExistOrCreate(uploadPath);
                    //Upload file.
                    FormBuilderStreamProvider streamProvider = new FormBuilderStreamProvider(uploadPath);

                    try
                    {
                        await Request.Content.ReadAsMultipartAsync(streamProvider);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    }

                    var fileName = string.Empty;
                    foreach (var fileStream in streamProvider.FileData)
                    {
                        if (string.IsNullOrEmpty(fileName))
                            fileName = fileStream.LocalFileName.Substring(fileStream.LocalFileName.LastIndexOf('\\') + 1);
                        else
                            fileName = fileName + "," + fileStream.LocalFileName.Substring(fileStream.LocalFileName.LastIndexOf('\\') + 1);
                    }

                    return messages = new FileUploadResponse { FileName = fileName, ImagePath = uploadPath };
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }


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

                //delete images from catalog folder which is generated by media setting.
                DeleteImageFromMediaCatalogFolder(mediaPaths);

                return !Equals(deletedObject, null);

            }
            return false;
        }

        //delete image from catalog folder which is generated by media setting
        private bool DeleteImageFromMediaCatalogFolder(string mediaPath, bool isMediaReplace = false, bool isImageTypeMedia = false)
        {
            string className = string.Empty;
            int mediaConfigurationId = 0;
            if (HelperUtility.IsNull(_displaySetting))
            {
                GlobalMediaDisplaySettingModel displaySetting = _mediaConfiguration?.MediaConfiguration?.GlobalMediaDisplaySetting;
                _displaySetting = HelperUtility.IsNull(displaySetting) ? GlobalMediaDisplaySettingModel.GetGlobalMediaDisplaySetting() : displaySetting;

            }
            ServerConnector _connectorobj = GetServerConnection(out className, out mediaConfigurationId);

            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxSmallThumbnailWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxLargeWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxSmallWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxThumbnailWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxMediumWidth));
            _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPath, string.Format("Catalog/{0}", _displaySetting.MaxCrossSellWidth));

            return true;
        }

        //Get status of uploaded files
        private void GetStatus(List<FileUploadResponse> messages, FileInfo fi, int folderId, string fileType, bool isOverWrite, string fileName, bool isMediaReplace = false, int reqMediaId = 0)
        {
            try
            {
                var _allowExtensions = GetExtensions()?.FamilyExtensionListModel?.FamilyExtensions;
                int familyId = 0;
                int mediaId;

                if (AllowExtension(_allowExtensions, fi.Extension, out familyId))
                {
                    if (AllowFileSize(_allowExtensions, fi.Length.ToString(), fi.Extension))
                    {
                        if (isMediaReplace && reqMediaId > 0)
                            mediaId = reqMediaId;
                        else
                            mediaId = CheckExist(fileName, folderId);

                        if (mediaId == 0)
                        {
                            mediaId = UploadFiles(0, fi, folderId, false, fi.FullName, familyId, fileType, fileName);
                            if (mediaId > 0)
                                messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Done), FileName = fileName, MediaId = Convert.ToString(mediaId) });
                            else
                                messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Error), FileName = fileName });
                        }
                        else if (isOverWrite)
                        {
                            mediaId = UploadFiles(mediaId, fi, folderId, true, fi.FullName, familyId, fileType, fileName, isMediaReplace);
                            if (mediaId > 0)
                                messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Done), FileName = fileName, MediaId = Convert.ToString(mediaId) });
                            else
                                messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.Error), FileName = fileName });
                        }
                        else
                            messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.FileAlreadyExist), FileName = fileName, MediaId = Convert.ToString(mediaId) });
                    }
                    else
                        messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.MaxFileSize), FileName = fileName });
                }
                else
                {
                    messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.ExtensionNotAllow), FileName = fileName });
                    fi.Delete();
                }
            }
            catch(ZnodeException ex)
            {

                if (fi.Extension.ToLower() == ".webp" && ex.ErrorCode==100)
                {
                    messages.Add(new FileUploadResponse { StatusCode = Convert.ToInt32(UploadStatusCode.UnSupportedFile), FileName = fileName });
                }
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
            }
        }

        //Get Key pair values from query string.
        private string GetKeyPairValue(IEnumerable<KeyValuePair<string, string>> collection, string keyName) => (from item in collection where item.Key == keyName select item.Value).Distinct().AsEnumerable().FirstOrDefault();

        //Get All allowed system extensions.
        private MediaManagerResponses GetExtensions() => JsonConvert.DeserializeObject<MediaManagerResponses>(_cache.GetAllowedExtensions(RouteUri, RouteTemplate));

        //Check File extension is allowed.
        private bool AllowExtension(List<FamilyExtensionModel> extensionList, string extension, out int familyId)
        {
            var _extension = extensionList.FirstOrDefault(x => x.ValidationName.ToLower() == extension.ToLower());
            familyId = !Equals(_extension, null) ? _extension.MediaAttributeFamilyId : 0;
            return !Equals(_extension, null);
        }

        //Check uploaded file size is allowed from system .
        private bool AllowFileSize(List<FamilyExtensionModel> extensionList, string fileSize, string extension)
        {
            double size = (Convert.ToDouble(fileSize) / 1024f) / 1024f;
            var _fileSize = extensionList?.FirstOrDefault(x => x.ValidationName.ToLower() == extension.ToLower());
            return string.IsNullOrEmpty(_fileSize?.MaxFileSize) ? true : (Convert.ToDecimal(size) <= Convert.ToDecimal(_fileSize?.MaxFileSize)) ? true : false;
        }

        //Check uploaded file is already exist in folder/system.
        private int CheckExist(string fileName, int folderId)
        {
            try
            {
                return _service.FileAlreadyExist(fileName, folderId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return 0;
            }

        }

        // Upload on server and save in to DB
        private MediaManagerModel ToMediaManagerModel(string fileType, FileInfo file, string actualFilePath, int folderId, int? attributeFamilyId, string fileName)
        {
           
                MediaManagerModel mediaManagerModel = new MediaManagerModel();
                mediaManagerModel.Path = file.Name;
                mediaManagerModel.Size = file.Length.ToString();
                mediaManagerModel.FileName = fileName;
                mediaManagerModel.Length = Convert.ToString(file.Length);
                mediaManagerModel.MediaType = file.Extension;
                string allowedExtention = ZnodeApiSettings.AllowedExtention;
                mediaManagerModel.IsSVGImage = (allowedExtention?.Contains(mediaManagerModel.MediaType.ToLower())).GetValueOrDefault();
            
                if (mediaManagerModel.IsSVGImage)
                {
                    XDocument document = XDocument.Load(actualFilePath);
                    XElement svg_Element = document.Root;

                    //Read height width attribute from svg xml.
                    string height = Convert.ToString(svg_Element.Attribute("height")?.Value);
                    string width = Convert.ToString(svg_Element.Attribute("width")?.Value);

                    if (!string.IsNullOrEmpty(height))
                        height = Regex.Replace(height, "[^0-9.]", "");

                    if (!string.IsNullOrEmpty(width))
                        width = Regex.Replace(width, "[^0-9.]", "");

                    //Read viewBox attribute from svg xml.
                    string viewBox = svg_Element.Attribute("viewBox")?.Value;

                    string[] vieBoxParameter = null;

                    if (!string.IsNullOrEmpty(viewBox))
                        vieBoxParameter = viewBox.Split(' ');

                    //if xml height attribute value null then read from viewbox and if viewbox height not exist the it will be null.
                    mediaManagerModel.Height = string.IsNullOrEmpty(height) ? HelperUtility.IsNotNull(vieBoxParameter.ElementAtOrDefault(3)) ? vieBoxParameter[3] : null : height;

                    mediaManagerModel.Width = string.IsNullOrEmpty(width) ? HelperUtility.IsNotNull(vieBoxParameter.ElementAtOrDefault(2)) ? vieBoxParameter[2] : null : width;
                }

                if (string.Equals(fileType.Split('/')[0], "image") && !mediaManagerModel.IsSVGImage)
                {
                    if (mediaManagerModel.MediaType.ToLower() != ".webp")
                    {
                        using (Bitmap img = new Bitmap(actualFilePath))
                        {
                            mediaManagerModel.Height = Convert.ToString(img.Height);
                            mediaManagerModel.Width = Convert.ToString(img.Width);
                        }
                        mediaManagerModel.IsImage = true;
                    }
                    else
                    {
                        try
                        {

                            byte[] photoBytes = File.ReadAllBytes(actualFilePath);
                            using (MemoryStream inStream = new MemoryStream(photoBytes))
                            {
                                using (MemoryStream outStream = new MemoryStream())
                                {
                                    using (var imageFactory = new ImageFactory())
                                    {
                                        imageFactory
                                            .Load(inStream);
                                        mediaManagerModel.Height = imageFactory.Image.Height.ToString();
                                        mediaManagerModel.Width  = imageFactory.Image.Width.ToString();
                                    }

                                }
                            }
                            mediaManagerModel.IsImage = true;
                        }
                        catch(Exception ex)
                        {
                            throw new ZnodeException(100, UploadStatusCode.UnSupportedFile.ToString());
                        }
                    }
                }

                //assing value to model which is to be save in database
                mediaManagerModel.AttributeFamilyId = attributeFamilyId;
                mediaManagerModel.MediaPathId = folderId;
                return mediaManagerModel;
        }

        private int UploadFiles(int mediaId, FileInfo files, int folderId, bool isOverrideFile, string actualFilePath, int? attributeFamilyId, string fileType, string fileName, bool isMediaReplace = false)
        {
            try
            {
                MediaManagerModel mediaDetails = ToMediaManagerModel(fileType, files, actualFilePath, folderId, attributeFamilyId, fileName);

                if (HelperUtility.IsNotNull(mediaDetails) && !string.IsNullOrEmpty(mediaDetails.Size))
                {
                    string className = string.Empty;
                    int mediaConfigurationId = 0;
                    ServerConnector _connectorobj = GetServerConnection(out className, out mediaConfigurationId);

                    mediaDetails.MediaConfigurationId = mediaConfigurationId;

                    if (isOverrideFile)
                    {
                        return UpdateExistingMedia(mediaId, mediaDetails, className, _connectorobj, isMediaReplace);
                    }

                    //upload media to server
                    UploadFilesMedia(className, _connectorobj, mediaDetails);
                    MediaManagerModel mediamanagermodel = _service.SaveMedia(mediaDetails);

                    try
                    {
                        if (mediaDetails.IsImage)
                        {
                            ImageMediaHelper imageHelper = new ImageMediaHelper(_mediaConfiguration.MediaConfiguration);
                            imageHelper.GenerateImageOnEdit(mediaDetails.Path);
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    }

                    return mediamanagermodel.MediaId;
                }
            }
            catch(ZnodeException ex)
            {
                if(ex.ErrorCode==100)
                {
                    throw new ZnodeException(100, UploadStatusCode.UnSupportedFile.ToString());
                }
                
            }
            return 0;
        }

        //Gets the server connection
        private ServerConnector GetServerConnection(out string className, out int mediaConfigurationId)
        {
            ServerConnector _connectorobj = null;
            //gets the default server configuration

            if (HelperUtility.IsNotNull(_mediaConfiguration.MediaConfiguration))
            {
                //Sets the server connection
                _connectorobj = new ServerConnector(new FileUploadPolicyModel(_mediaConfiguration.MediaConfiguration.AccessKey, _mediaConfiguration.MediaConfiguration.SecretKey, _mediaConfiguration.MediaConfiguration.BucketName, _mediaConfiguration.MediaConfiguration.ThumbnailFolderName, _mediaConfiguration.MediaConfiguration.URL, _mediaConfiguration.MediaConfiguration.NetworkUrl));
                className = _mediaConfiguration.MediaConfiguration.MediaServer.ClassName;
                mediaConfigurationId = _mediaConfiguration.MediaConfiguration.MediaConfigurationId;
            }
            else
            {
                _mediaConfiguration = JsonConvert.DeserializeObject<MediaConfigurationResponse>(_configurationCache.GetDefaultMediaConfiguration(RouteUri, RouteTemplate));
                //Local
                if (_mediaConfiguration.MediaConfiguration.Server == ZnodeConstant.NetworkDrive)
                {
                    _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, APIConstant.DefaultMediaFolder, APIConstant.ThumbnailFolderName, _mediaConfiguration.MediaConfiguration.URL, _mediaConfiguration.MediaConfiguration.NetworkUrl));
                    className = _mediaConfiguration.MediaConfiguration.MediaServer.ClassName;
                    mediaConfigurationId = _mediaConfiguration.MediaConfiguration.MediaConfigurationId;
                }
                else
                {
                    _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, APIConstant.DefaultMediaFolder, APIConstant.ThumbnailFolderName, _mediaConfiguration.MediaConfiguration.URL, _mediaConfiguration.MediaConfiguration.NetworkUrl));
                    className = APIConstant.DefaultMediaClassName;
                    mediaConfigurationId = _mediaConfiguration.MediaConfiguration.MediaConfigurationId;

                }
            }
            return _connectorobj;
        }

        //Create Thumbnail image and save 
        private void UploadFilesMedia(string serverAgentClassName, ServerConnector connectorobj, MediaManagerModel mediaManagerModel)
        {
            string mediaPath = Path.Combine(HttpContext.Current.Server.MapPath($"~/{APIConstant.DefaultMediaFolder}/{APIConstant.TempImage}"), mediaManagerModel.Path);

            if (mediaManagerModel.IsImage || mediaManagerModel.IsSVGImage)
            {
                if (Equals(mediaManagerModel.MediaType, ZnodeConstant.FaviconExtension) || mediaManagerModel.IsSVGImage)
                {
                    using (MemoryStream fileStream = new MemoryStream(File.ReadAllBytes(mediaPath)))
                    {
                        //send media to upload
                        connectorobj.CallConnector(serverAgentClassName, MediaStorageAction.Upload, fileStream, mediaManagerModel.Path, connectorobj.UploadPolicyModel.ThumbnailFolderName);
                    }
                }
                else
                {
                    try
                    {
                        if (mediaManagerModel.MediaType.ToLower() != ".webp")
                        {
                            using (Image image = Image.FromFile(mediaPath))
                            {
                                string pathOfMedia = $"/{APIConstant.DefaultMediaFolder}/{ connectorobj.UploadPolicyModel.ThumbnailFolderName}";

                                if (_mediaConfiguration.MediaConfiguration.Server == ZnodeConstant.NetworkDrive)
                                {
                                    if (!Directory.Exists(_mediaConfiguration.MediaConfiguration.NetworkUrl + pathOfMedia))
                                        Directory.CreateDirectory(_mediaConfiguration.MediaConfiguration.NetworkUrl + pathOfMedia);
                                }
                                else
                                {
                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(pathOfMedia)))
                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(pathOfMedia));
                                }
                                int newWidth = 0, newHeight = 0;

                                GetImageParameter(image, out newWidth, out newHeight);//Sets the width and height

                                //Generate thumbnail from image
                                Image _image = image.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

                                //set image format
                                ImageFormat imageFormat = GetImageFormat(mediaManagerModel.MediaType);

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    //save image as stream
                                    _image.Save(stream, imageFormat);

                                    //send image to upload
                                    connectorobj.CallConnector(serverAgentClassName, MediaStorageAction.Upload, stream, mediaManagerModel.Path, connectorobj.UploadPolicyModel.ThumbnailFolderName);
                                }
                            }
                        }
                        else
                        {
                            int newHeight = 0;
                            int newWidth = 0;

                            byte[] photoBytes = File.ReadAllBytes(mediaPath);
                            string pathOfMedia = $"/{APIConstant.DefaultMediaFolder}/{ connectorobj.UploadPolicyModel.ThumbnailFolderName}";

                            if (_mediaConfiguration.MediaConfiguration.Server == ZnodeConstant.NetworkDrive)
                            {
                                if (!Directory.Exists(_mediaConfiguration.MediaConfiguration.NetworkUrl + pathOfMedia))
                                    Directory.CreateDirectory(_mediaConfiguration.MediaConfiguration.NetworkUrl + pathOfMedia);
                            }
                            else
                            {
                                if (!Directory.Exists(HttpContext.Current.Server.MapPath(pathOfMedia)))
                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(pathOfMedia));
                            }
                            
                            // Format is automatically detected though can be changed.
                             using (MemoryStream inStream = new MemoryStream(photoBytes))
                             {
                                  using (MemoryStream outStream = new MemoryStream())
                                  {
                                    
                                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                                    {
                                        ISupportedImageFormat format = new JpegFormat { Quality = 100 };
                                        imageFactory.Load(inStream);
                                        int Height = imageFactory.Image.Height;
                                        int Width = imageFactory.Image.Width;
                                        GetWebpImageParameter(Width, Height, out newWidth, out newHeight);//Sets the width and height
                                        Size size = new Size(newWidth, newHeight);
                                        ResizeLayer resizeLayer = new ResizeLayer(size, ResizeMode.Max);
                                        
                                        imageFactory
                                                        .Resize(resizeLayer)
                                                        .Format(format)
                                                        .Save(outStream);
                                        connectorobj.CallConnector(serverAgentClassName, MediaStorageAction.Upload, outStream, mediaManagerModel.Path, connectorobj.UploadPolicyModel.ThumbnailFolderName);
                                    }
                                  }

                              }
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    }
                }

                //generate media stream
                using (MemoryStream fileStream = new MemoryStream(File.ReadAllBytes(mediaPath)))
                {
                    //send media to upload
                    connectorobj.CallConnector(serverAgentClassName, MediaStorageAction.Upload, fileStream, mediaManagerModel.Path, string.Empty);
                }
                //delete temporary image saved in local machine 
                File.Delete(mediaPath);
            }
        }

        //Sets the width and height of image according to aspect ratio
        private void GetImageParameter(Image sourceImage, out int newWidth, out int newHeight)
		{
			decimal scaleFactor;
			int maxWidth = 150, maxHeight = 150;//Maximum file height and width for thumbnail
			decimal originalProportion = (decimal)sourceImage.Width / sourceImage.Height;
			decimal resizeProportion = (decimal)maxWidth / maxHeight;

			if (originalProportion > resizeProportion)
				scaleFactor = Convert.ToDecimal(maxWidth) / Convert.ToDecimal(sourceImage.Width);
			else
				scaleFactor = Convert.ToDecimal(maxHeight) / Convert.ToDecimal(sourceImage.Height);

			newWidth = (int)Math.Round((sourceImage.Width * scaleFactor));
			newHeight = (int)Math.Round((sourceImage.Height * scaleFactor));
		}

        //Sets the width and height for Webp image according to aspect ratio
        private void GetWebpImageParameter(int width, int height, out int newWidth, out int newHeight)
        {
            decimal scaleFactor;
            int maxWidth = 150, maxHeight = 150;//Maximum file height and width for thumbnail
            decimal originalProportion = (decimal)width / height;
            decimal resizeProportion = (decimal)maxWidth / maxHeight;

            if (originalProportion > resizeProportion)
                scaleFactor = Convert.ToDecimal(maxWidth) / Convert.ToDecimal(width);
            else
                scaleFactor = Convert.ToDecimal(maxHeight) / Convert.ToDecimal(height);

            newWidth = (int)Math.Round((width * scaleFactor));
            newHeight = (int)Math.Round((height * scaleFactor));
        }

        //get ImageFormat from string extentions
        private ImageFormat GetImageFormat(string extension)
		{
			switch (extension.ToLower())
			{
				case @".bmp":
					return ImageFormat.Bmp;

				case @".gif":
					return ImageFormat.Gif;

				case @".ico":
					return ImageFormat.Icon;

				case @".jpg":
				case @".jpeg":
					return ImageFormat.Jpeg;

				case @".png":
					return ImageFormat.Png;

				case @".tif":
				case @".tiff":
					return ImageFormat.Tiff;

				case @".wmf":
					return ImageFormat.Wmf;

				default:
					return ImageFormat.Png;
			}
		}

		//Check directory exist or not and create directory.
		private void CheckDirectoryExistOrCreate(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

        //Update existing media.
        protected virtual int UpdateExistingMedia(int mediaId, MediaManagerModel mediaDetails, string className, ServerConnector _connectorobj, bool isMediaReplace = false)
		{
			mediaDetails.MediaId = mediaId;
			MediaManagerModel mediaManagerModel = _service.GetMediaByID(mediaId, null);
			if (HelperUtility.IsNotNull(mediaManagerModel))
			{
				string mediaPath = Path.Combine(HttpContext.Current.Server.MapPath($"~/{APIConstant.DefaultMediaFolder}/{APIConstant.TempImage}/"));

                string destPath = mediaPath + mediaManagerModel.Path;

                //Clean if destination file already exists, before replacing it, otherwise it will throw exception
                if (File.Exists(destPath))
                {
                    File.Delete(destPath);
                }

                //Rename new image file with old image name in the temp folder.
                File.Move(mediaPath + mediaDetails.Path, mediaPath + mediaManagerModel.Path);

				mediaDetails.Path = mediaManagerModel.Path;
				MediaManagerModel mediaModel = _service.Update(mediaDetails);
			}
			UploadFilesMedia(className, _connectorobj, mediaDetails);

            //delete images from catalog folder which is generated by media setting.
            DeleteImageFromMediaCatalogFolder(mediaManagerModel.Path, isMediaReplace, mediaDetails.IsImage);

            try
            {
                if (mediaDetails.IsImage)
                {
                    ImageMediaHelper imageHelper = new ImageMediaHelper(_mediaConfiguration.MediaConfiguration);
                    imageHelper.GenerateImageOnEdit(mediaDetails.Path);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
            }
            return mediaDetails.MediaId;
		}
     }
 
	public class MyStreamProvider : MultipartFormDataStreamProvider
	{
		public MyStreamProvider(string uploadPath)
			: base(uploadPath)
		{

		}

		public override string GetLocalFileName(HttpContentHeaders headers)
		{
			string fileName = headers.ContentDisposition.FileName;

			if (string.IsNullOrWhiteSpace(fileName))
			{
				fileName = Guid.NewGuid().ToString() + ".data";
			}

			return Guid.NewGuid().ToString() + fileName.Replace("\"", string.Empty);
		}
	}
	public class FormBuilderStreamProvider : MultipartFormDataStreamProvider
	{
		public FormBuilderStreamProvider(string uploadPath)
			: base(uploadPath)
		{

		}
		public override string GetLocalFileName(HttpContentHeaders headers)
		{
			string fileName = headers.ContentDisposition.FileName;

			if (string.IsNullOrWhiteSpace(fileName))
			{
				fileName = Guid.NewGuid().ToString() + ".data";
			}

			return Guid.NewGuid().ToString() + "--" + fileName.Replace("\"", string.Empty);
		}
	}
}