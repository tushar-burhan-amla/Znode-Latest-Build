using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.MediaStorage;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
namespace Znode.Engine.Services
{
    public class ImageHelper : IImageHelper
    {
        public const string DefaultMediaClassName = "LocalAgent";
        public const string TempImage = "TempImage";
        private readonly MediaConfigurationModel _configuration;
        private readonly GlobalMediaDisplaySettingModel _displaySetting;      
        private readonly bool _generateRuntimeImages;

        public ImageHelper()
        {
            _configuration = new MediaConfigurationService().GetDefaultMediaConfiguration();
            GlobalMediaDisplaySettingModel displaySetting = _configuration?.GlobalMediaDisplaySetting;
            _displaySetting = HelperUtility.IsNull(displaySetting) ? GetDefaultDisplaySetting() : displaySetting;
         _generateRuntimeImages = Convert.ToBoolean(ZnodeApiSettings.GenerateRuntimeImages);
        }

        public MediaConfigurationModel DefaultMediaConfiguration
        {
            get
            {
                return _configuration;
            }
        }

        public GlobalMediaDisplaySettingModel GlobalMediaDisplaySetting
        {
            get
            {
                return _displaySetting;
            }
        }       

        public bool GenerateRuntimeImages
        {
            get
            {
                return _generateRuntimeImages;
            }
        }
        public ServerConnector _connectorobj
        {
            get
            {
                return new ServerConnector(new FileUploadPolicyModel(DefaultMediaConfiguration.AccessKey, DefaultMediaConfiguration.SecretKey, DefaultMediaConfiguration.BucketName, DefaultMediaConfiguration.ThumbnailFolderName, DefaultMediaConfiguration.URL, DefaultMediaConfiguration.NetworkUrl));
            }
        }

        //Get Original image path.
        public string GetOriginalImagepath(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxLargeWidth, imageFileName, true);

        //Get Image large path.
        public string GetImageHttpPathLarge(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxLargeWidth, imageFileName);

        //Get image medium path.
        public string GetImageHttpPathMedium(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxMediumWidth, imageFileName);

        //Get image small path.
        public string GetImageHttpPathSmall(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxSmallWidth, imageFileName);

        //Get image thumbnail path.
        public string GetImageHttpPathThumbnail(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxThumbnailWidth, imageFileName);

        //Get image small thumbnail path.
        public string GetImageHttpPathSmallThumbnail(string imageFileName) =>
        GetRelativeImageUrl(GlobalMediaDisplaySetting?.MaxSmallThumbnailWidth, imageFileName);

        //Get Image URL.
        public string GetRelativeImageUrl(int? imageSize, string imageFileName, bool isOriginalImagePath = false)
        {
            string returnFileName = string.Empty;
            
            if (string.IsNullOrEmpty(imageFileName) && !string.IsNullOrEmpty(GlobalMediaDisplaySetting?.DefaultImageName))
                imageFileName = GlobalMediaDisplaySetting?.DefaultImageName;

            returnFileName = GetImageHttpPath(imageSize.GetValueOrDefault(), imageFileName, isOriginalImagePath);

            return returnFileName;
        }

        //Get Full path for image.
        public string GetImageHttpPath(int imageSize, string imageFileName, bool isoriginalImagePath)
        {
            if (DefaultMediaConfiguration.Server == ZnodeConstant.NetworkDrive)
            {
                return GetImageNetworkPath(imageSize, imageFileName, isoriginalImagePath);
            }
            string returnFilePath = string.Empty;
            string className = DefaultMediaConfiguration.MediaServer.ClassName;
            string ServerPath = GetMediaServerUrl(DefaultMediaConfiguration);

            if (isoriginalImagePath || IsSVGImage(imageFileName))
            {
                returnFilePath = $"{ServerPath}{imageFileName}";
                return returnFilePath;
            }

            return $"{ServerPath}{"Catalog/"}{imageSize.ToString()}{"/"}{imageFileName}";
        }


        //Get Full path for image.
        public string GetImageNetworkPath(int imageSize, string imageFileName, bool isoriginalImagePath)
        {
            string returnFilePath = string.Empty;
            string className = DefaultMediaConfiguration.MediaServer.ClassName;
            string ServerPath = GetMediaServerUrl(DefaultMediaConfiguration);

            string ServerPathNetworkDrive = DefaultMediaConfiguration.NetworkUrl;

            string _newImageOnNetwork = ServerPathNetworkDrive + $"{DefaultMediaConfiguration.BucketName}/";

            if (isoriginalImagePath || IsSVGImage(imageFileName))
            {
                returnFilePath = $"{ServerPath}{imageFileName}";
                return returnFilePath;
            }

            return $"{ServerPath}{"Catalog/"}{imageSize.ToString()}{"/"}{imageFileName}";
        }


        private string UploadFile(string originalImagePath, string newImageSaveDirectory, int imageSize, string imageFileName, ServerConnector _connectorobj, string className)
        {
            string imagePath = $"{newImageSaveDirectory}{"/"}{GlobalMediaDisplaySetting.DefaultImageName}";
            if (string.IsNullOrEmpty(imageFileName))
                return imagePath;
            try
            {
                string tempPath = HttpContext.Current.Server.MapPath("~/Data/Media/TempImage");
                //Check directory exist or not and create directory.
                CheckDirectoryExistOrCreate(tempPath);
                string tempImagePath = Path.Combine(tempPath, imageFileName);
                try
                {
                    if (!File.Exists(tempImagePath))
                        GetImageAtLocal(originalImagePath, imageFileName);
                }
                catch (Exception ex)
                {
                    return $"{newImageSaveDirectory}{"/"}{imageFileName}";
                }

                using (Image image = Image.FromFile(tempImagePath))
                {
                    string pathOfMedia = newImageSaveDirectory;
                    if (className == DefaultMediaClassName || className == "NetworkDriveAgent")
                    {
                        string localDirectory = DefaultMediaConfiguration.Server == ZnodeConstant.NetworkDrive ? Path.Combine((DefaultMediaConfiguration.NetworkUrl + "/Data/Media"), "Catalog", imageSize.ToString()) : Path.Combine(HttpContext.Current.Server.MapPath("~/Data/Media"), "Catalog", imageSize.ToString());

                        if (!Directory.Exists(localDirectory))
                            Directory.CreateDirectory(localDirectory);
                    }

                    Image saveImage = SetImageParameter(image, imageSize, imageSize);

                    ImageFormat imageFormat = GetImageFormat(Path.GetExtension(imageFileName));
                    string folder = $"Catalog/{imageSize.ToString()}";

                    ImageCodecInfo jpgEncoder = GetEncoder(imageFormat);
                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 91L);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        //save image as stream
                        saveImage.Save(stream, jpgEncoder, myEncoderParameters);

                        //send image to upload
                        _connectorobj.CallConnector(className, MediaStorageAction.Upload, stream, imageFileName, folder);
                    }

                    imagePath = $"{newImageSaveDirectory}{"/"}{imageFileName}";
                }
                File.Delete(tempImagePath);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error,ex);
            }

            return imagePath;
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
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

        public void GetImageAtLocal(string originalImagePath, string fileName)
        {
            var _connectorobj = new ServerConnector(new FileUploadPolicyModel("", "", "Data/Media", "TempImage", string.Empty, string.Empty));
            byte[] data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(originalImagePath);
                MemoryStream stream = new MemoryStream(data);
                _connectorobj.CallConnector("LocalAgent", MediaStorageAction.Upload, stream, fileName, TempImage);
            }
        }



        private Image SetImageParameter(Image sourceImage, int maxHeight, int maxWidth)
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
            thumbnailGraph.Clear(Color.White);

            Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(sourceImage, imageRectangle);

            Image imageToSave = thumbnailBitmap;

            return imageToSave;
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

        //Check If File already exist
        private bool AlreadyExist(string url)
        {
            bool result = false;
            HttpWebResponse response = null;
          
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                response = (HttpWebResponse)request.GetResponse();
                result = true;
            }
            catch (Exception ex)
            {
                /* A WebException will be thrown if the status of the response is not `200 OK` */
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        //Check directory exist or not and create directory.
        private void CheckDirectoryExistOrCreate(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        //Get default display setting width.
        private GlobalMediaDisplaySettingModel GetDefaultDisplaySetting()
        {
            return new GlobalMediaDisplaySettingModel
            {
                MaxSmallThumbnailWidth = ZnodeConstant.MaxSmallThumbnailWidth,
                MaxLargeWidth = ZnodeConstant.MaxLargeWidth,
                MaxSmallWidth = ZnodeConstant.MaxSmallWidth,
                MaxThumbnailWidth = ZnodeConstant.MaxThumbnailWidth,
                MaxMediumWidth = ZnodeConstant.MaxMediumWidth,
                MaxCrossSellWidth = ZnodeConstant.MaxCrossSellWidth,
            };
        }

        //to check image is of svg type or not
        private bool IsSVGImage(string imageOriginalPath)
        {
            if (!string.IsNullOrEmpty(imageOriginalPath) && imageOriginalPath.Contains("."))
            {
                string[] imagePath = imageOriginalPath.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (imagePath.Length > 1)
                {
                    return imagePath[1].Equals("svg", StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return false;
        }

        //Get the Media Server Url
        private string GetMediaServerUrl(MediaConfigurationModel configuration)
        {
            if (HelperUtility.IsNotNull(configuration))
            {
                return string.IsNullOrWhiteSpace(configuration.CDNUrl) ? configuration.URL
                           : configuration.CDNUrl.EndsWith("/") ? configuration.CDNUrl : $"{configuration.CDNUrl}/";
            }
            return string.Empty;
        }
    }
}
