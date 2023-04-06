using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.MediaStorage;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using System.Web;
using ImageProcessor.Imaging;

namespace Znode.Engine.Services
{
    public class ImageMediaHelper : IImageMediaHelper
    {
        private MediaConfigurationModel _mediaConfiguration;

        public ImageMediaHelper(MediaConfigurationModel mediaConfiguration)
        {
            _mediaConfiguration = mediaConfiguration;
        }

        //Generates the image
        public virtual void GenerateImageOnEdit(string mediaPath)
        {
            string orgPath = _mediaConfiguration.Server == ZnodeConstant.NetworkDrive ? Path.Combine(_mediaConfiguration.NetworkUrl + _mediaConfiguration.BucketName) : GetMediaServerUrl(_mediaConfiguration);

            string destinationPath = $"{orgPath}/Catalog";

            GlobalMediaDisplaySettingModel displaySetting = HelperUtility.IsNull(_mediaConfiguration?.GlobalMediaDisplaySetting) ? GlobalMediaDisplaySettingModel.GetGlobalMediaDisplaySetting() : _mediaConfiguration?.GlobalMediaDisplaySetting;

            ImageModel imageHelperModel = GetImageHelperModel(destinationPath, orgPath, displaySetting);

            GenerateResizedImages(new List<string> { mediaPath }, imageHelperModel);
        }

        //generate resize images
        public virtual void GenerateResizedImages(List<string> mediaList, ImageModel imageHelperModel)
        {
            int TotalFilesPublished = 0;
            foreach (string images in mediaList)
            {
                try
                {
                    foreach (string image in images.Split(','))
                    {
                        string imageNamewithPath = GetImageName(imageHelperModel.SourcePath, image);
                        ZnodeLogging.LogMessage($"GenerateResizedImage - Started for - {image} - ImagePath - {imageNamewithPath}");

                        ResizeAndSaveImage(imageNamewithPath, imageHelperModel, image);

                        ZnodeLogging.LogMessage($"GenerateResizedImage - Completed for - {image} - ImagePath - {imageNamewithPath}");

                        TotalFilesPublished++;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage($"GenerateResizedImage - Error - {ex.Message}, stack: {ex.StackTrace}");
                    continue;
                }
            }
        }

        //Resize the image and save it for all Sizes
        protected virtual void ResizeAndSaveImage(string imageNamewithPath, ImageModel imageHelperModel, string imageName)
        {
            FileInfo fi = new FileInfo(imageName);

            if(fi.Extension.ToLower() == ".webp")
            {
                SaveLargeImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.LargeImgWidth, imageHelperModel.BucketName);
                SaveMediumImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.MediumImgWidth, imageHelperModel.BucketName);
                SaveSmallImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallImgWidth, imageHelperModel.BucketName);
                SaveCrossSellImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.CrossImgWidth, imageHelperModel.BucketName);
                SaveThumbnailImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.ThumbImgWidth, imageHelperModel.BucketName);
                SaveSmallThumbanailImageWebp(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallThumbImgWidth, imageHelperModel.BucketName);
            }
            else
            {
                SaveLargeImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.LargeImgWidth, imageHelperModel.BucketName);
                SaveMediumImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.MediumImgWidth, imageHelperModel.BucketName);
                SaveSmallImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallImgWidth, imageHelperModel.BucketName);
                SaveCrossSellImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.CrossImgWidth, imageHelperModel.BucketName);
                SaveThumbnailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.ThumbImgWidth, imageHelperModel.BucketName);
                SaveSmallThumbanailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallThumbImgWidth, imageHelperModel.BucketName);
            }
        }

        //Save the small thumbnail images
        protected virtual void SaveSmallThumbanailImage(string imageNamewithPath, string imageName, string destinationPath, int smallThumbImgWidth, string bucketName)
        {
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, smallThumbImgWidth, smallThumbImgWidth);
            SaveResizedImage(imageToSave, smallThumbImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the thumbnail images
        protected virtual void SaveThumbnailImage(string imageNamewithPath, string imageName, string destinationPath, int thumbImgWidth, string bucketName)
        {
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, thumbImgWidth, thumbImgWidth);
            SaveResizedImage(imageToSave, thumbImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the cross-sell images
        protected virtual void SaveCrossSellImage(string imageNamewithPath, string imageName, string destinationPath, int crossImgWidth, string bucketName)
        {
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, crossImgWidth, crossImgWidth);
            SaveResizedImage(imageToSave, crossImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the small images
        protected virtual void SaveSmallImage(string imageNamewithPath, string imageName, string destinationPath, int smallImgWidth, string bucketName)
        {
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, smallImgWidth, smallImgWidth);
            SaveResizedImage(imageToSave, smallImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the medium images
        protected virtual void SaveMediumImage(string imageNamewithPath, string imageName, string destinationPath, int mediumImgWidth, string bucketName)
        {
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, mediumImgWidth, mediumImgWidth);
            SaveResizedImage(imageToSave, mediumImgWidth, destinationPath, imageName, bucketName);
        }

        //Save the large images
        protected virtual void SaveLargeImage(string imageNamewithPath, string imageName, string destinationPath, int width, string bucketName)
        {
            //Generate image using image details
            Image image = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(image, width, width);
            SaveResizedImage(imageToSave, width, destinationPath, imageName, bucketName);
        }

        //Get the image to use
        protected virtual Image GetImageToUse(string imageNamewithPath)
        {
            Image image = null;
            byte[] stream = GetImageStream(imageNamewithPath);
            using (MemoryStream mStream = new MemoryStream(stream))
                image = Image.FromStream(mStream);
            return image;
        }

        //Get image in stream
        protected virtual byte[] GetImageStream(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return imageData;
        }

        //Save the resized image
        protected virtual void SaveResizedImage(Image imageToSave, int folderName, string destinationPath, string imageName, string bucketName)
        {
            try
            {
                if (!Equals(imageToSave, null))
                {
                    string localHostPath = $"~/{bucketName}/Catalog";
                    string destPath = $"{destinationPath}/{folderName}/{imageName}";

                    ImageFormat imageFormat = GetImageFormat(Path.GetExtension(imageName));

                    ImageCodecInfo jpgEncoder = GetEncoder(imageFormat);
                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    long QualityFactor = 90;
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, QualityFactor);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        imageToSave.Save(stream, jpgEncoder, myEncoderParameters);

                        if (_mediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                            SaveLocalImage(folderName.ToString(), imageName, stream);
                        else if (_mediaConfiguration.MediaServer.ClassName.Equals("NetworkDriveAgent"))
                            SaveLocalImageForNetworkDrive(folderName.ToString(), imageName, stream);
                        else
                        {
                            string destFolderName = $"Catalog/{folderName}";
                            ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(_mediaConfiguration.AccessKey, _mediaConfiguration.SecretKey, _mediaConfiguration.BucketName, _mediaConfiguration.ThumbnailFolderName, _mediaConfiguration.URL, _mediaConfiguration.NetworkUrl));
                            mediaServerConnector.CallConnector(_mediaConfiguration.MediaServer.ClassName, MediaStorageAction.Upload, stream, imageName, destFolderName);
                            mediaServerConnector = null;
                        }

                        imageToSave = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error occurred in SaveResizedImage function. Error - " + ex.Message.ToString());
                ZnodeLogging.LogMessage("Inner Base Exception - " + ex.GetBaseException().ToString());
                ZnodeLogging.LogMessage("Stack Trace - " + ex.StackTrace);
            }
        }

        //get ImageFormat from string extentions
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
            DirectoryInfo dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            dir = dir.Parent;

            string path = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\Media";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\Catalog";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\" + folderName.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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
            string path = Path.Combine(_mediaConfiguration.NetworkUrl + $"/{_mediaConfiguration.BucketName}");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\Catalog";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\" + folderName.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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

        //Create the resize image
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
                ZnodeLogging.LogMessage("Error occurred in ResizeImage function. Error - " + ex.Message.ToString());
                ZnodeLogging.LogMessage("Stack Trace - " + ex.StackTrace);
                return null;
            }
        }

        //Create helper for the image generation operation
        public virtual ImageModel GetImageHelperModel(string destinationPath, string orgPath, GlobalMediaDisplaySettingModel displaySettings)
        {
            return new ImageModel
            {
                SourcePath = orgPath,
                DestinationPath = destinationPath,
                LargeImgWidth = displaySettings.MaxLargeWidth,
                MediumImgWidth = displaySettings.MaxMediumWidth,
                SmallImgWidth = displaySettings.MaxSmallWidth,
                CrossImgWidth = displaySettings.MaxCrossSellWidth,
                ThumbImgWidth = displaySettings.MaxThumbnailWidth,
                SmallThumbImgWidth = displaySettings.MaxSmallThumbnailWidth,
                BucketName = _mediaConfiguration.BucketName
            };
        }

        //Get Media Server Url
        protected virtual string GetMediaServerUrl(MediaConfigurationModel configuration)
        {
            if (HelperUtility.IsNotNull(configuration))
            {
                return string.IsNullOrWhiteSpace(configuration.CDNUrl) ? configuration.URL
                           : configuration.CDNUrl.EndsWith("/") ? configuration.CDNUrl : $"{configuration.CDNUrl}/";
            }
            return string.Empty;
        }

        //Get complete image name
        protected virtual string GetImageName(string sourcePath, string imageName)
            => sourcePath.EndsWith("/") ? $"{sourcePath}{imageName}" : $"{sourcePath}/{imageName}";

        //#region Webp Images 

        //Save the small thumbnail webp images
        protected virtual void SaveSmallThumbanailImageWebp(string imageNamewithPath, string imageName, string destinationPath, int smallThumbImgWidth, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, smallThumbImgWidth, bucketName);
        }
        //Save the small webp images
        protected virtual void SaveSmallImageWebp(string imageNamewithPath, string imageName, string destinationPath, int smallImgWidth, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, smallImgWidth, bucketName);
        }

        //Save the cross-sell webp images
        protected virtual void SaveCrossSellImageWebp(string imageNamewithPath, string imageName, string destinationPath, int crossImgWidth, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, crossImgWidth, bucketName);
        }

        //Save the medium webp images
        protected virtual void SaveMediumImageWebp(string imageNamewithPath, string imageName, string destinationPath, int mediumImgWidth, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, mediumImgWidth, bucketName);
        }

        //Save the thumbnail webp images
        protected virtual void SaveThumbnailImageWebp(string imageNamewithPath, string imageName, string destinationPath, int thumbImgWidth, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, thumbImgWidth, bucketName);
        }

        //Save the large webp images
        protected virtual void SaveLargeImageWebp(string imageNamewithPath, string imageName, string destinationPath, int width, string bucketName)
        {
            SaveWebpImage(imageNamewithPath, imageName, destinationPath, width, bucketName);
        }

        //Save WebP Images
        public virtual void SaveWebpImage(string imageNamewithPath, string imageName, string destinationPath, int width, string bucketName)
        {
            if (!Equals(imageNamewithPath, null))
            {
               byte[] photoBytes = GetImageStream(imageNamewithPath);

                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        using (var imageFactory = new ImageFactory())
                        {
                            ISupportedImageFormat format = new JpegFormat { Quality = 100 };

                            imageFactory
                                .Load(inStream);
                            int Height = imageFactory.Image.Height;
                            int Width = imageFactory.Image.Width;
                            string ImgDimension = ImageHeightWidth(width, width);
                            string[] tokens = ImgDimension.Split(',');
                            int newWidth = Convert.ToInt32(tokens[0]);
                            int newHeight = Convert.ToInt32(tokens[1]);

                            Size size = new Size(newWidth, newHeight);
                            ResizeLayer resizeLayer = new ResizeLayer(size, ResizeMode.Max);

                            imageFactory
                                .Resize(resizeLayer)
                                .AutoRotate()
                                .Save(outStream);

                            if (_mediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                                SaveLocalImage(width.ToString(), imageName, outStream);
                            else if (_mediaConfiguration.MediaServer.ClassName.Equals("NetworkDriveAgent"))
                                SaveLocalImageForNetworkDrive(width.ToString(), imageName, inStream);
                            else
                            {
                                string destFolderName = $"Catalog/{width}";
                                ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(_mediaConfiguration.AccessKey, _mediaConfiguration.SecretKey, _mediaConfiguration.BucketName, _mediaConfiguration.ThumbnailFolderName, _mediaConfiguration.URL, _mediaConfiguration.NetworkUrl));
                                mediaServerConnector.CallConnector(_mediaConfiguration.MediaServer.ClassName, MediaStorageAction.Upload, inStream, imageName, destFolderName);
                                mediaServerConnector = null;
                            }
                        }
                    }
                }
            }
        }

        //Get Webp Image width & Height
        public string ImageHeightWidth(int width, int height)
        {
            int originalWidth = width;
            int originalHeight = height;

            float percentWidth = (float)width / originalWidth;
            float percentHeight = (float)height / originalHeight;

            float percent = percentHeight < percentWidth ? percentHeight : percentWidth;

            int newWidth = (int)(originalWidth * percent);
            int newHeight = (int)(originalHeight * percent);

            return newWidth.ToString() + ',' + newHeight.ToString();
        }
       //#endregion
    }
}
