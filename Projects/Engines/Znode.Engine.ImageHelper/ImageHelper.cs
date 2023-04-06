using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.MediaStorage;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.ImageHelper
{
    public class ImageHelper
    {
        #region Private Variables
        private static MediaConfigurationModel Configuration;
        private static string ApiURL = string.Empty;
        private static string UriItemSeparator = ZnodeApiSettings.ZnodeApiUriItemSeparator;
        private static string UriKeyValueSeparator = ZnodeApiSettings.ZnodeApiUriKeyValueSeparator;
        private static string LogFileName = $"ImageUtilityLogFile_{DateTime.Now.ToString("MMddyyyy")}.txt";
        private static int TotalFilesPublished = 0;
        private static string LogFilePath = string.Empty;
        #endregion

        #region Public Properties
        public static MediaConfigurationModel DefaultMediaConfiguration
        {
            get
            {
                return Configuration;
            }
        }
        #endregion

        #region Main
        //args[0] - Type. - ImageUtility
        //args[1] - PortalId
        //args[2] - Chunck Size
        //args[3] - User Id
        //args[4] - Catalogs Id
        //args[5] - Locale Id
        //args[6] - version Id
        //args[5] - api URL 

        static void Main(string[] args)
        {
            try
            {
                string type = Convert.ToString(args[0]);
                int portalId = int.Parse(args[1].ToString());
                int chunkSize = int.Parse(args[2].ToString());
                int userId = int.Parse(args[3].ToString());
                int catalogId = int.Parse(args[4].ToString());
                int localeId = int.Parse(args[5].ToString());
                ApiURL = Convert.ToString(args[6]);
                LogFilePath = Convert.ToString(args[7]);

                if (type.ToLower().Equals("imageutility"))
                {
                    //Get all images chunkwise
                    PublishProductListModel publishList = GetPublishProductData(catalogId, localeId);

                    //Get all display settings
                    PortalDisplaySettingModel displaySettings = GetDisplaySettings(portalId);

                    //Get category images
                    PublishCategoryListModel categoryList = GetPublishCategoryData(catalogId, localeId);

                    //Get all images to resize
                    List<string> publishProdImgList = GetImageList(publishList, displaySettings, categoryList);

                    if (publishProdImgList?.Count > 0)
                    {
                        //Log the start of process
                        LogMessage($"Image publishing started for {publishProdImgList.Count} images.");

                        //Call the API to get default Media Configuration
                        Configuration = GetMediaConfiguration();
                        string orgPath = Configuration?.URL;
                        string destinationPath = $"{orgPath}Catalog/{portalId}/";
                        ImageHelperModel imghelperModel = GetImageHelperModel(destinationPath, orgPath, displaySettings, portalId);

                        LogMessage("All data gathered from Server. Started delete operation on server. URL - " + orgPath);
                        //Delete all images and folders on the server.
                        DeleteMediaImages(portalId.ToString());
                        LogMessage("Delete operation completed on server. URL - " + orgPath);

                        LogMessage("Image creation started.");
                        //Generate images
                        GenerateResizedImages(publishProdImgList, imghelperModel);
                        int failedFiles = publishProdImgList.Count - TotalFilesPublished;
                        LogMessage($"Image creation completed.  {TotalFilesPublished} images published successfully. {failedFiles} images failed to published");
                    }
                    else
                        LogMessage("No images available. Please check publish product list.");
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error occurred while image publishing. Error - " + ex.Message.ToString());
                LogMessage("Stack Trace - " + ex.StackTrace);
            }
        }
        #endregion

        #region Private Methods

        //Get the product main image, gallery images and portal default image and returns the list of it
        private static List<string> GetImageList(PublishProductListModel publishList, PortalDisplaySettingModel displaySettings, PublishCategoryListModel categoryList)
        {
            List<string> imageList = new List<string>();
            //Add default portal image
            if (!string.IsNullOrEmpty(displaySettings.PortalDefaultImageName))
                imageList.Add(displaySettings.PortalDefaultImageName);

            //get products main images
            if (!Equals(publishList, null) && publishList?.PublishProducts?.Count > 0)
                imageList.AddRange(publishList.PublishProducts.SelectMany(x => x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.Select(z => z.AttributeValues)).ToList());

            //get category image list
            if (!Equals(categoryList, null) && categoryList?.PublishCategories?.Count > 0)
                imageList.AddRange(categoryList.PublishCategories.SelectMany(x => x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.CategoryImage)?.Select(z => z.AttributeValues)).ToList());

            //Get the products gallery images and add it in image list
            List<string[]> galleryImagesList = publishList.PublishProducts.SelectMany(x => x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.GalleryImages)?.Select(z => z.AttributeValues.Split(','))).ToList();
            imageList.AddRange(galleryImagesList.SelectMany(x => x));

            return imageList;
        }

        //get the category images
        private static PublishCategoryListModel GetPublishCategoryData(int catalogId, int localeId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(catalogId));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, Convert.ToString(localeId));

            PublishCategoryListModel model = new PublishCategoryListModel();
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = $"{ApiURL}/publishcategory/list";
            requestPath += BuildEndpointQueryString(null, filters, null, null, null);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<PublishCategoryListModel>(jsonString);
                    reader.Close();
                    datastream.Close();
                    LogMessage("GetPublishCategoryData - API Call Success.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"GetPublishCategoryData - Failed: {ex.Message}");
            }

            return model;
        }

        //Create helper for the image generation operation
        private static ImageHelperModel GetImageHelperModel(string destinationPath, string orgPath, PortalDisplaySettingModel displaySettings, int portalId)
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
                BucketName = Configuration.BucketName,
                PortalId = portalId
            };
        }

        //Logs all the messages in the file daywise
        private static void LogMessage(string message)
        {
            if (!Directory.Exists(LogFilePath))
                Directory.CreateDirectory(LogFilePath);

            DirectorySecurity sec = Directory.GetAccessControl(LogFilePath);
            // Using this instead of the "Everyone" string means we work on non-English systems.
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(LogFilePath, sec);

            string fileNameWithPath = $"{LogFilePath}\\{LogFileName}";
            if (!File.Exists(fileNameWithPath))
                File.Create(fileNameWithPath).Dispose();

            using (StreamWriter outfile = File.AppendText(fileNameWithPath))
                outfile.WriteLine($"{DateTime.Now} -- {message}\n");
        }

        //Delete all the media files from server
        private static void DeleteMediaImages(string portalId)
        {
            try
            {
                if (!DefaultMediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                {
                    string folderName = $"Catalog/{portalId}/";
                    ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(DefaultMediaConfiguration.AccessKey, DefaultMediaConfiguration.SecretKey, DefaultMediaConfiguration.BucketName, DefaultMediaConfiguration.ThumbnailFolderName));
                    mediaServerConnector.CallConnector(DefaultMediaConfiguration.MediaServer.ClassName, "DeleteFolder", null, portalId, folderName);
                    mediaServerConnector = null;
                }
                else
                {
                    DirectoryInfo dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
                    string path = dir.Parent.FullName;
                    path = $"{path}\\Media\\Catalog\\{portalId}";
                    if (Directory.Exists(path))
                        Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                LogMessage("DeleteMediaImages - Error - " + ex.Message.ToString());
                LogMessage("Stack Trace - " + ex.StackTrace);
            }
        }

        //Get the published products data
        private static PublishProductListModel GetPublishProductData(int catalogId, int localeId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(catalogId));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, Convert.ToString(localeId));

            PublishProductListModel model = new PublishProductListModel();
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = $"{ApiURL}/publishproduct/list";
            requestPath += BuildEndpointQueryString(null, filters, null, null, null);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<PublishProductListModel>(jsonString);
                    reader.Close();
                    datastream.Close();
                    LogMessage("GetPublishProductData - API Call Success.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"GetPublishProductData - Failed: {ex.Message}");
            }

            return model;
        }

        //Get media configuration data
        private static MediaConfigurationModel GetMediaConfiguration()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(true));

            ExpandCollection expands = new ExpandCollection();
            expands.Add("mediaserver");

            MediaConfigurationResponse result = new MediaConfigurationResponse();
            MediaConfigurationModel model = new MediaConfigurationModel();
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = $"{ApiURL}/mediaconfiguration";
            requestPath += BuildEndpointQueryString(expands, filters, null, null, null);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<MediaConfigurationResponse>(jsonString);
                    model = result.MediaConfiguration;
                    reader.Close();
                    datastream.Close();
                    LogMessage("GetMediaConfiguration - API Call Success.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"GetMediaConfiguration - Failed: {ex.Message}");
            }

            return model;
        }

        //Get Display settings
        private static PortalDisplaySettingModel GetDisplaySettings(int portalId)
        {
            PortalDisplaySettingResponse result = new PortalDisplaySettingResponse();
            PortalDisplaySettingModel model = new PortalDisplaySettingModel();
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = $"{ApiURL}/portal/getdisplaysetting/{portalId}";

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<PortalDisplaySettingResponse>(jsonString);
                    model = result.DisplaySetting;
                    reader.Close();
                    datastream.Close();
                    LogMessage("GetDisplaySettings - API Call Success.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"GetDisplaySettings - API Call Failed: {ex.Message}");
            }

            return model;
        }

        //Build the query string for request
        private static string BuildEndpointQueryString(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            // IMPORTANT: Expand always starts with ? while all the others start with &, which
            // means the expands must be added first when building the querystring parameters.

            var queryString = BuildExpandQueryString(expands);
            queryString += BuildFilterQueryString(filters);
            queryString += BuildSortQueryString(sorts);
            queryString += BuildPageQueryString(pageIndex, pageSize);

            return queryString;
        }

        //Genreate query string for Expand
        private static string BuildExpandQueryString(ExpandCollection expands)
        {
            var queryString = "?expand=";

            if (expands != null)
            {
                foreach (var e in expands)
                    queryString += e + UriItemSeparator;

                if (!string.IsNullOrEmpty(UriItemSeparator))
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        //Genreate query string for Filter
        private static string BuildFilterQueryString(FilterCollection filters)
        {
            var queryString = "&filter=";

            if (filters != null)
            {
                foreach (var f in filters)
                    queryString += $"{f.FilterName}{UriKeyValueSeparator}{f.FilterOperator }{UriKeyValueSeparator }{HttpUtility.UrlEncode(f.FilterValue)}{UriItemSeparator }";

                if (!string.IsNullOrEmpty(UriItemSeparator))
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        //Genreate query string for Sort
        private static string BuildSortQueryString(SortCollection sorts)
        {
            var queryString = "&sort=";

            if (sorts != null)
            {
                foreach (var s in sorts)
                    queryString += $"{ s.Key}{UriKeyValueSeparator}{s.Value}{UriItemSeparator}";

                if (!string.IsNullOrEmpty(UriItemSeparator))
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        //Genreate query string for Pagination
        private static string BuildPageQueryString(int? pageIndex, int? pageSize)
        {
            var queryString = "&page=";

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                queryString += $"index{UriKeyValueSeparator}{pageIndex.Value}";
                queryString += UriItemSeparator;
                queryString += $"size{ UriKeyValueSeparator} { pageSize.Value}";
            }

            return queryString;
        }

        //generate resize images
        private static void GenerateResizedImages(List<string> publishProdImgList, ImageHelperModel imageHelperModel)
        {
            TotalFilesPublished = 0;
            foreach (string img in publishProdImgList)
            {
                try
                {
                    string imageNamewithPath = GetImageName(imageHelperModel.SourcePath, img);
                    LogMessage($"GenerateResizedImages - Started for - {img} - ImagePath - {imageNamewithPath}");
                    ResizeAndSaveImage(imageNamewithPath, imageHelperModel, img);
                    LogMessage($"GenerateResizedImages - Completed for - {img} - ImagePath - {imageNamewithPath}");
                    TotalFilesPublished++;
                }
                catch (Exception ex)
                {
                    LogMessage($"GenerateResizedImages - Error - {ex.Message}");
                    continue;
                }
            }
        }

        //Resize the image and save it for all Sizes
        private static void ResizeAndSaveImage(string imageNamewithPath, ImageHelperModel imageHelperModel, string imageName)
        {
            SaveLargeImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.LargeImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
            SaveMediumImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.MediumImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
            SaveSmallImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
            SaveCrossSellImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.CrossImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
            SaveThumbnailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.ThumbImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
            SaveSmallThumbanailImage(imageNamewithPath, imageName, imageHelperModel.DestinationPath, imageHelperModel.SmallThumbImgWidth, imageHelperModel.BucketName, imageHelperModel.PortalId);
        }

        //Save the small thumbnail images
        private static void SaveSmallThumbanailImage(string imageNamewithPath, string imageName, string destinationPath, int smallThumbImgWidth, string bucketName, int portalId)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, smallThumbImgWidth, smallThumbImgWidth);
            SaveResizedImage(imageToSave, smallThumbImgWidth, destinationPath, imageName, bucketName, portalId);
        }

        //Save the thumbnail images
        private static void SaveThumbnailImage(string imageNamewithPath, string imageName, string destinationPath, int thumbImgWidth, string bucketName, int portalId)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, thumbImgWidth, thumbImgWidth);
            SaveResizedImage(imageToSave, thumbImgWidth, destinationPath, imageName, bucketName, portalId);
        }

        //Save the cross-sell images
        private static void SaveCrossSellImage(string imageNamewithPath, string imageName, string destinationPath, int crossImgWidth, string bucketName, int portalId)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, crossImgWidth, crossImgWidth);
            SaveResizedImage(imageToSave, crossImgWidth, destinationPath, imageName, bucketName, portalId);
        }

        //Save the small images
        private static void SaveSmallImage(string imageNamewithPath, string imageName, string destinationPath, int smallImgWidth, string bucketName, int portalId)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, smallImgWidth, smallImgWidth);
            SaveResizedImage(imageToSave, smallImgWidth, destinationPath, imageName, bucketName, portalId);
        }

        //Save the medium images
        private static void SaveMediumImage(string imageNamewithPath, string imageName, string destinationPath, int mediumImgWidth, string bucketName, int portalId)
        {
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, mediumImgWidth, mediumImgWidth);
            SaveResizedImage(imageToSave, mediumImgWidth, destinationPath, imageName, bucketName, portalId);
        }

        //Save the large images
        private static void SaveLargeImage(string imageNamewithPath, string imageName, string destinationPath, int width, string bucketName, int portalId)
        {
            //Generate image using image details
            Image img = GetImageToUse(imageNamewithPath);
            Image imageToSave = ResizeImage(img, width, width);
            SaveResizedImage(imageToSave, width, destinationPath, imageName, bucketName, portalId);
        }

        //Get the image to use
        private static Image GetImageToUse(string imageNamewithPath)
        {
            Image img = null;
            byte[] stream = GetImageStream(imageNamewithPath);
            using (MemoryStream mStream = new MemoryStream(stream))
                img = Image.FromStream(mStream);
            return img;
        }

        //Get image in stream
        private static byte[] GetImageStream(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return imageData;
        }

        //Save the resized image
        private static void SaveResizedImage(Image imageToSave, int folderName, string destinationPath, string imageName, string bucketName, int portalId)
        {
            try
            {
                if (!Equals(imageToSave, null))
                {
                    string localHostPath = $"~/{bucketName}/Catalog/{portalId}";
                    string destPath = $"{destinationPath}/{folderName}/{imageName}";
                    using (MemoryStream stream = new MemoryStream())
                    {
                        imageToSave.Save(stream, GetImageFormat(Path.GetExtension(imageName)));
                        if (DefaultMediaConfiguration.MediaServer.ClassName.Equals("LocalAgent"))
                            SaveLocalImage(portalId.ToString(), folderName.ToString(), imageName, stream);
                        else
                        {
                            string destFolderName = $"Catalog/{portalId}/{folderName}";
                            ServerConnector mediaServerConnector = new ServerConnector(new FileUploadPolicyModel(DefaultMediaConfiguration.AccessKey, DefaultMediaConfiguration.SecretKey, DefaultMediaConfiguration.BucketName, DefaultMediaConfiguration.ThumbnailFolderName));
                            mediaServerConnector.CallConnector(DefaultMediaConfiguration.MediaServer.ClassName, MediaStorageAction.Upload, stream, imageName, destFolderName);
                            mediaServerConnector = null;
                        }

                        imageToSave = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error occurred in SaveResizedImage function. Error - " + ex.Message.ToString());
                LogMessage("Stack Trace - " + ex.StackTrace);
            }
        }

        //Save image for local server
        private static void SaveLocalImage(string portalId, string folderName, string imageName, MemoryStream stream)
        {
            DirectoryInfo dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            dir = dir.Parent;

            string path = $"{dir.FullName}\\Media";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\Catalog";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\" + portalId.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\" + folderName.ToString();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\" + imageName;

            //write to file
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        //Get image format
        private static ImageFormat GetImageFormat(string extension)
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

        //Create the rezised image
        private static Image ResizeImage(Image sourceImage, int maxHeight, int maxWidth)
        {
            try
            {
                decimal originalProportion = sourceImage.Width / sourceImage.Height;
                decimal resizeProportion = maxWidth / maxHeight;

                decimal scalefactorWidth = Convert.ToDecimal(maxWidth) / Convert.ToDecimal(sourceImage.Width);
                decimal scalefactorHeight = Convert.ToDecimal(maxHeight) / Convert.ToDecimal(sourceImage.Height);

                int newWidth = (int)Math.Round((sourceImage.Width * scalefactorWidth));
                int newHeight = (int)Math.Round((sourceImage.Height * scalefactorHeight));
                Bitmap thumbnailBitmap = new Bitmap(newWidth, newHeight);

                Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                thumbnailGraph.Clear(Color.White);

                Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbnailGraph.DrawImage(sourceImage, imageRectangle);

                Image imageToSave = thumbnailBitmap;

                return imageToSave;
            }
            catch (Exception ex)
            {
                LogMessage("Error occurred in ResizeImage function. Error - " + ex.Message.ToString());
                LogMessage("Stack Trace - " + ex.StackTrace);
                return null;
            }
        }

        //Get complete image name
        private static string GetImageName(string sourcePath, string imageName)
            => $"{sourcePath}{imageName}";
        #endregion

    }
}
