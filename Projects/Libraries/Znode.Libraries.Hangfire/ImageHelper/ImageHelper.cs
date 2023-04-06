using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.MediaStorage;

namespace Znode.Libraries.Hangfire
{
    public class ImageHelper : BaseScheduler, ISchedulerProviders
    {
        #region Private Variables
        private MediaConfigurationModel Configuration;
        private string ApiURL = string.Empty;        
        private int TotalFilesPublished = 0;
        private long QualityFactor = 90;

        private string AuthorizationHeaderValue = string.Empty;
        private string TokenValue = string.Empty;

        protected override string SchedulerName { get => "ImageScheduler"; }
        #endregion

        #region Public Properties
        public MediaConfigurationModel DefaultMediaConfiguration
        {
            get
            {
                return Configuration;
            }
        }
        #endregion

        #region Public Methods
        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ExeParameters))
                {
                    var args = model.ExeParameters.Split(',');
                    string type = Convert.ToString(args[0]);
                    int chunkSize = int.Parse(args[1].ToString());
                    int userId = int.Parse(args[2].ToString());
                    int localeId = int.Parse(args[3].ToString());
                    ApiURL = Convert.ToString(args[4]);
                    int qualityFactor = Convert.ToInt32(args[5]);
                    AuthorizationHeaderValue = Convert.ToString(args[6]);
                    if (args.Length > 7)
                    {
                        TokenValue = Convert.ToString(args[7]);
                    }

                    if (args.Length > 8 && !string.IsNullOrEmpty(args[8]))
                    {
                        base.RequestTimeout = int.Parse(args[8]);
                    }

                    if (qualityFactor > 0)
                    {
                        QualityFactor = qualityFactor;
                    }

                    if (type.Equals(ZnodeConstant.ImageHelper, StringComparison.InvariantCultureIgnoreCase))
                    {
                        GenerateAllImages(chunkSize);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

        #endregion

        #region Protected Methods

        //Generate All Images
        protected virtual void GenerateAllImages(int pageLength)
        {
            int pageIndex = 1;
            decimal totalPages;
            int pageSize = pageLength;
            int totalCount = GetMediaCount();

            if (totalCount < pageLength)
                totalPages = 1;
            else
                totalPages = Math.Ceiling((decimal)totalCount / pageLength);

            ZnodeLogging.LogMessage($"{SchedulerName}: Total Pages: {totalPages}", LoggingComponent, TraceLevel.Info);

            //Call the API to get default Media Configuration and global display setting.
            Configuration = GetMediaConfiguration();

            string orgPath = Configuration.Server == ZnodeConstant.NetworkDrive ? Path.Combine(Configuration?.NetworkUrl + Configuration.BucketName) : GetMediaServerUrl(Configuration);

            string destinationPath = $"{orgPath}/Catalog";

            ImageHelperModel imageHelperModel = GetImageHelperModel(destinationPath, orgPath, Configuration?.GlobalMediaDisplaySetting);
          
            while (pageIndex <= totalPages)
            {
                //Get all images chunk wise
                MediaManagerListModel mediaList = GetMediaList(pageIndex, pageSize);

                ZnodeLogging.LogMessage($"Total Images Receive :{mediaList?.MediaList.Count}", LoggingComponent, TraceLevel.Info);

                if (mediaList?.MediaList?.Count > 0)
                {
                    //Get all images to resize
                    List<string> imageList = GetImageList(mediaList, Configuration?.GlobalMediaDisplaySetting);

                    if (imageList?.Count > 0)
                    {
                        //Log the start of process
                        ZnodeLogging.LogMessage($"Image generation started for {imageList.Count} images.", LoggingComponent, TraceLevel.Info);

                        //Generate images
                        GenerateResizedImages(imageList, imageHelperModel);

                        int failedFiles = imageList.Count - TotalFilesPublished;

                        ZnodeLogging.LogMessage($"Image creation completed. {TotalFilesPublished} images generated successfully. {failedFiles} images failed to generate.", LoggingComponent, TraceLevel.Info);
                    }
                    else
                        ZnodeLogging.LogMessage($"{SchedulerName}: No images available. Please check media list.", LoggingComponent, TraceLevel.Info);
                }

                pageIndex++;
            }
        }

        //Get the media list data
        protected virtual MediaManagerListModel GetMediaList(int pageNo, int pageSize)
        {
            MediaManagerListModel model = new MediaManagerListModel();
            string jsonString = string.Empty;

            string requestPath = $"{ApiURL}/mediaconfiguration/getmedialistforgenerateimages";
            requestPath += BuildEndpointQueryString(null, null, null, pageNo, pageSize);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Timeout = base.RequestTimeout;
                request.Headers.Add($"{ AuthorizationHeader }: Basic { AuthorizationHeaderValue }");
                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                    request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    MediaManagerListResponses responseModel = JsonConvert.DeserializeObject<MediaManagerListResponses>(jsonString);
                    if (responseModel!=null && responseModel.MediaList?.MediaList.Count > 0)
                    {
                        model = responseModel.MediaList;
                    }
                    reader.Close();
                    datastream.Close();
                    ZnodeLogging.LogMessage("GetMediaList - API Call Success.", LoggingComponent, TraceLevel.Info);
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenValue = GetToken(ApiURL, AuthorizationHeaderValue);
                    GetMediaList(pageNo, pageSize);
                }
                else
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }

            return model;
        }

        //Create helper for the image generation operation
        protected virtual ImageHelperModel GetImageHelperModel(string destinationPath, string orgPath, GlobalMediaDisplaySettingModel displaySettings)
        {
            return new ImageHelperModel
            {
                SourcePath = orgPath,
                DestinationPath = destinationPath,
                LargeImgWidth = displaySettings.MaxLargeWidth,
                MediumImgWidth = displaySettings.MaxMediumWidth,
                SmallImgWidth = displaySettings.MaxSmallWidth,
                CrossImgWidth = displaySettings.MaxCrossSellWidth,
                ThumbImgWidth = displaySettings.MaxThumbnailWidth,
                SmallThumbImgWidth = displaySettings.MaxSmallThumbnailWidth,
                BucketName = Configuration.BucketName
            };
        }

        //Get the default image and returns the list of media.
        protected virtual List<string> GetImageList(MediaManagerListModel mediaList,GlobalMediaDisplaySettingModel displaySettings)
        {
            List<string> imageList = new List<string>();
            //Add default image
            if (!string.IsNullOrEmpty(displaySettings?.DefaultImageName))
                imageList.Add(displaySettings.DefaultImageName);

            if (mediaList?.MediaList.Count > 0)
            {
                imageList.AddRange(mediaList.MediaList.Select(x => x.Path)?.ToList());
            }

            return imageList;
        }

        //Delete all the media files from server
        protected virtual void DeleteMediaImages()
        {
            try
            {
                if (!DefaultMediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                {
                    string folderName = $"Catalog/";
                    ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(DefaultMediaConfiguration.AccessKey, DefaultMediaConfiguration.SecretKey, DefaultMediaConfiguration.BucketName, DefaultMediaConfiguration.ThumbnailFolderName, DefaultMediaConfiguration.URL, DefaultMediaConfiguration.NetworkUrl));
                    mediaServerConnector.CallConnector(DefaultMediaConfiguration.MediaServer.ClassName, "DeleteFolder", null, "", folderName);
                    mediaServerConnector = null;
                }
                else
                {
                    DirectoryInfo dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
                    string path = dir.Parent.FullName;
                    path = $"{path}\\Media\\Catalog";
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

        //Get media configuration data
        protected virtual MediaConfigurationModel GetMediaConfiguration()
        {
            FilterCollection filters = new FilterCollection
            {
                { WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(true) }
            };

            ExpandCollection expands = new ExpandCollection
            {
                "mediaserver"
            };

            MediaConfigurationResponse result = new MediaConfigurationResponse();
            MediaConfigurationModel model = new MediaConfigurationModel();
            string jsonString = string.Empty;

            string requestPath = $"{ApiURL}/mediaconfiguration";
            requestPath += BuildEndpointQueryString(expands, filters, null, null, null);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Timeout = base.RequestTimeout;
                request.Headers.Add($"{ AuthorizationHeader }: Basic { AuthorizationHeaderValue }");
                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                {
                    request.Headers.Add($"{ TokenHeader }: { TokenValue }");
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();

                    result = JsonConvert.DeserializeObject<MediaConfigurationResponse>(jsonString);
                    model = result.MediaConfiguration;
                    reader.Close();
                    datastream.Close();
                    ZnodeLogging.LogMessage("GetMediaConfiguration - API Call Success.", LoggingComponent, TraceLevel.Info);
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenValue = GetToken(ApiURL, AuthorizationHeaderValue);
                    GetMediaConfiguration();
                }
                else
                {
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }

            return model;
        }

        //generate resize images
        protected virtual void GenerateResizedImages(List<string> imageList, ImageHelperModel imageHelperModel)
        {
            TotalFilesPublished = 0;
            foreach (string imgs in imageList)
            {
                try
                {
                    foreach (string img in imgs.Split(','))
                    {
                        string imageNamewithPath = GetImageName(imageHelperModel.SourcePath, img);
                        ZnodeLogging.LogMessage($"GenerateResizedImages - Started for - {img} - ImagePath - {imageNamewithPath}", LoggingComponent, TraceLevel.Info);

                        ResizeAndSaveImage(imageNamewithPath, imageHelperModel, img);

                        ZnodeLogging.LogMessage($"GenerateResizedImages - Completed for - {img} - ImagePath - {imageNamewithPath}", LoggingComponent, TraceLevel.Info);

                        TotalFilesPublished++;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
                    continue;
                }
            }
        }

        //Resize the image and save it for all Sizes
        protected virtual void ResizeAndSaveImage(string imageNamewithPath, ImageHelperModel imageHelperModel, string imageName)
        {
            SaveLargeImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.LargeImgWidth, imageHelperModel.BucketName);
            SaveMediumImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.MediumImgWidth, imageHelperModel.BucketName);
            SaveSmallImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallImgWidth, imageHelperModel.BucketName);
            SaveCrossSellImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.CrossImgWidth, imageHelperModel.BucketName);
            SaveThumbnailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.ThumbImgWidth, imageHelperModel.BucketName);
            SaveSmallThumbnailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallThumbImgWidth, imageHelperModel.BucketName);
        }

        //Save the small thumbnail images
        protected virtual void SaveSmallThumbnailImage(string imageNamewithPath, string imageName, string destinationPath, int smallThumbImgWidth, string bucketName)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, smallThumbImgWidth, smallThumbImgWidth);
            SaveResizedImage(imageToSave, smallThumbImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the thumbnail images
        protected virtual void SaveThumbnailImage(string imageNamewithPath, string imageName, string destinationPath, int thumbImgWidth, string bucketName)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, thumbImgWidth, thumbImgWidth);
            SaveResizedImage(imageToSave, thumbImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the cross-sell images
        protected virtual void SaveCrossSellImage(string imageNamewithPath, string imageName, string destinationPath, int crossImgWidth, string bucketName)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, crossImgWidth, crossImgWidth);
            SaveResizedImage(imageToSave, crossImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the small images
        protected virtual void SaveSmallImage(string imageNamewithPath, string imageName, string destinationPath, int smallImgWidth, string bucketName)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, smallImgWidth, smallImgWidth);
            SaveResizedImage(imageToSave, smallImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the medium images
        protected virtual void SaveMediumImage(string imageNamewithPath, string imageName, string destinationPath, int mediumImgWidth, string bucketName)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, mediumImgWidth, mediumImgWidth);
            SaveResizedImage(imageToSave, mediumImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the large images
        protected virtual void SaveLargeImage(string imageNamewithPath, string imageName, string destinationPath, int width, string bucketName)
        {
            //Generate image using image details
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, width, width);
            SaveResizedImage(imageToSave, width, destinationPath, imageName, bucketName);
        }

        //Get the image to use
        protected virtual Image GetImageToUse(string imageNamewithPath)
        {
            Image img = null;
            byte[] stream = GetImageStream(imageNamewithPath);
            using (MemoryStream mStream = new MemoryStream(stream))
            {
                img = Image.FromStream(mStream);
            }
            return img;
        }

        //Get image in stream
        protected virtual byte[] GetImageStream(string url)
        {
            byte[] imageData = null;

            using (WebClient wc = new WebClient())
            {
                imageData = wc.DownloadData(url);
            }

            return imageData;
        }

        //Save the resized image
        protected virtual void SaveResizedImage(Image imageToSave, int folderName, string destinationPath, string imageName, string bucketName)
        {
            try
            {
                if (!Equals(imageToSave, null))
                {
                    string localHostPath = $"~/{bucketName}/Catalog/";
                    string destPath = $"{destinationPath}/{folderName}/{imageName}";

                    ImageFormat imageFormat = GetImageFormat(Path.GetExtension(imageName));

                    ImageCodecInfo jpgEncoder = GetEncoder(imageFormat);
                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, QualityFactor);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (MemoryStream stream = new MemoryStream())
                    {

                        imageToSave.Save(stream, jpgEncoder, myEncoderParameters);

                        if (DefaultMediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                        {
                            SaveLocalImage(folderName.ToString(), imageName, stream);
                        }
                        else if (DefaultMediaConfiguration.MediaServer.ClassName.Equals("NetworkDriveAgent"))
                        {
                            SaveLocalImageForNetworkDrive(folderName.ToString(), imageName, stream);
                        }
                        else
                        {
                            string destFolderName = $"Catalog/{folderName}";
                            ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(DefaultMediaConfiguration.AccessKey, DefaultMediaConfiguration.SecretKey, DefaultMediaConfiguration.BucketName, DefaultMediaConfiguration.ThumbnailFolderName, DefaultMediaConfiguration.URL, DefaultMediaConfiguration.NetworkUrl));
                            mediaServerConnector.CallConnector(DefaultMediaConfiguration.MediaServer.ClassName, MediaStorageAction.Upload, stream, imageName, destFolderName);
                            mediaServerConnector = null;
                        }

                        imageToSave = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error occurred in SaveResizedImage().", LoggingComponent, TraceLevel.Error);
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

        protected virtual ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        //Save image for local server
        protected virtual void SaveLocalImage(string folderName, string imageName, MemoryStream stream)
        {
            // The changes are for ZPD-18503, Removed the chunk of code because it was referring the root folder location instead of the API Location,
            // Referred the Code of SaveLocalImage() from ImageMediaHelper.cs 
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\Media";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\Catalog";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\" + folderName.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\" + imageName;

            if (!File.Exists(path))
            {
                //write to file
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }
            }
        }

        protected virtual void SaveLocalImageForNetworkDrive(string folderName, string imageName, MemoryStream stream)
        {
            string path = Path.Combine(Configuration.NetworkUrl + $"/{Configuration.BucketName}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\Catalog";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\" + folderName.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\" + imageName;

            if (!File.Exists(path))
            {
                //write to file
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }
            }
        }

        //Get image format
        protected virtual ImageFormat GetImageFormat(string extension)
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

        //Create the resized image
        protected virtual Image ResizeImage(Image sourceImage, int maxHeight, int maxWidth)
        {
            try
            {
                int originalWidth = sourceImage.Width;
                int originalHeight = sourceImage.Height;

                float percentWidth = (float)maxWidth / originalWidth;
                float percentHeight = (float)maxHeight / originalHeight;

                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;

                int newWidth = (int)(originalWidth * percent);
                int newHeight = (int)(originalHeight * percent);

                Bitmap thumbnailBitmap = new Bitmap(sourceImage, newWidth, newHeight);

                Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);

                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.AntiAlias;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                thumbnailGraph.CompositingMode = CompositingMode.SourceCopy;
                thumbnailGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                thumbnailGraph.Clear(Color.White);

                Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbnailGraph.DrawImage(sourceImage, imageRectangle);

                Image imageToSave = thumbnailBitmap;

                return imageToSave;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error occurred in ResizeImage().", LoggingComponent, TraceLevel.Error);
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
                return null;
            }
        }

        //Get complete image name
        protected virtual string GetImageName(string sourcePath, string imageName)
            => sourcePath.EndsWith("/") ? $"{sourcePath}{imageName}" : $"{sourcePath}/{imageName}";

        //Get Media Server Url
        protected virtual string GetMediaServerUrl(MediaConfigurationModel configuration)
        {
            if (configuration!=null)
            {
                return string.IsNullOrWhiteSpace(configuration.CDNUrl) ? configuration.URL
                           : configuration.CDNUrl.EndsWith("/") ? configuration.CDNUrl : $"{configuration.CDNUrl}/";
            }
            return string.Empty;
        }
    
        //Get total media count.
        protected virtual int GetMediaCount()
        {
            StringResponse model = new StringResponse();

            string jsonString = string.Empty;

            string requestPath = $"{ApiURL}/mediaconfiguration/getmediacount";
            requestPath += BuildEndpointQueryString(null, null, null, null, null);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Timeout = base.RequestTimeout;
                request.Headers.Add($"{ AuthorizationHeader }: Basic { AuthorizationHeaderValue }");
                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                    request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<StringResponse>(jsonString);
                    reader.Close();
                    datastream.Close();
                    ZnodeLogging.LogMessage("getmediacount - API Call Success.", LoggingComponent, TraceLevel.Info);
                }
            }
            catch (WebException webex)
            {
                if (CheckTokenIsInvalid(webex))
                {
                    TokenValue = GetToken(ApiURL, AuthorizationHeaderValue);
                    GetMediaCount();
                }
                else
                    ZnodeLogging.LogMessage(webex, LoggingComponent, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"getmediacount - Failed: {ex}", LoggingComponent, TraceLevel.Error);
            }

            ZnodeLogging.LogMessage($"TotalImagesCount : {Convert.ToInt32(model.Response)}", LoggingComponent, TraceLevel.Info);

            return Convert.ToInt32(model.Response);
        }
        #endregion
    }
}
