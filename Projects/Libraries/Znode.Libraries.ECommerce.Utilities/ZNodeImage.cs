using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    [Serializable]
    public class ZnodeImage
    {
        #region Private Variables
        private string noImageFileName = string.Empty;
        private bool isAlreadyChecked = false;
        private bool useSiteConfig = false;
        #endregion

        #region Public Constructor
        // Initializes a new instance of the ZNodeImage class.
        public ZnodeImage()
        {
            // Get No image from store settings
            if (!string.IsNullOrEmpty(ZnodeConfigManager.SiteConfig.ImageNotAvailablePath))
            {
                this.noImageFileName = Path.GetFileName(ZnodeConfigManager.SiteConfig.ImageNotAvailablePath).Trim();
            }
            // Get No Image file name from web.config
            if ((System.Configuration.ConfigurationManager.AppSettings["ImageNotAvailableFileName"] != null) && string.IsNullOrEmpty(this.noImageFileName))
            {
                this.noImageFileName = System.Configuration.ConfigurationManager.AppSettings["ImageNotAvailableFileName"].ToString();
            }
            // Hard  code the No image file name.
            if (string.IsNullOrEmpty(this.noImageFileName))
            {
                this.noImageFileName = "noimage.gif";
            }
        }
        #endregion

        #region TO DO Work remaining for Portal Display Settings
        //Get the large size image relative path
        public string GetImageHttpPathLarge(string imageFileName)
            => string.Empty;
       
        //Get the small medium size image relative path.
        public string GetImageHttpPathMedium(string imageFileName)
            => string.Empty;
        
        //Get the small image relative path.
        public string GetImageHttpPathSmall(string imageFileName)
            => string.Empty;
       
        //Get the thumbnail image relative path.
        public string GetImageHttpPathThumbnail(string imageFileName)
            => string.Empty;
        
        //Get the cross sell image relative path.
        public string GetImageHttpPathCrossSell(string imageFileName)
            => string.Empty;
        
        //Get the small thumbnail image relative path.
        public string GetImageHttpPathSmallThumbnail(string imageFileName)
            => string.Empty;
        #endregion

        //Get the image by specified size
        public string GetImageBySize(int imageSize, string imageFileName)
        {
            return this.GetRelativeImageUrl(imageSize, imageFileName, false);
        }

        public string GetStoreLogoBySize(Size imageSize, string imageFileName)
        {
            string orginalImageDirectory = $"{ZnodeConfigManager.EnvironmentConfig.DataPath}images/catalog/original/turnkey/{ZnodeConfigManager.SiteConfig.PortalId}/";

            string returnUrl = string.Empty;
            string imageFileFullName = string.Empty;
            imageFileName = Equals(imageFileName, null) ? string.Empty : Path.Combine(orginalImageDirectory, imageFileName);
            if (!string.IsNullOrEmpty(imageFileName))
            {
                // If file exists and resized image folder and no new image uploaded in original folder then use already resized image.
                if (!ZnodeStorageManager.Exists(imageFileFullName))
                {
                    string originalImageDirectory = $"{ZnodeConfigManager.EnvironmentConfig.DataPath}images/catalog/{imageSize.Width}/";
                    string originalImageFileFullName = Path.Combine(originalImageDirectory, imageFileName);
                    returnUrl = ZnodeStorageManager.Exists(originalImageFileFullName) ? originalImageFileFullName : this.ResizeImage(originalImageFileFullName, imageSize.Height, imageSize.Width, originalImageDirectory);
                }
                else
                {
                    string resizedImageDirectory = $"{ZnodeConfigManager.EnvironmentConfig.DataPath}images/catalog/{imageSize.Width}/turnkey/{ZnodeConfigManager.SiteConfig.PortalId}/";
                    string resizedImageFullName = Path.Combine(resizedImageDirectory, imageFileName);
                    returnUrl = ZnodeStorageManager.Exists(resizedImageFullName) ? resizedImageFullName : this.ResizeImage(imageFileFullName, imageSize.Height, imageSize.Width, resizedImageDirectory);
                }
            }
            else
            {
                returnUrl = GetImageHttpPathSmall($"Turnkey/{ZnodeConfigManager.SiteConfig.PortalId}/{Path.GetFileName(imageFileName)}");
            }
            return returnUrl;
        }
        #region Helper Methods

        // Get the relative image path to a resized image. If the resized image does not exist then it will be created. If the image does not exist.
        // then the "No Image" image will be returned.
        public string GetImageHttpPath(int imageSize, string imageFileName, bool cropImage, out bool imageFileExists)
        {
            string imageFileFullName = string.Empty;
            string imageFilePath = string.Empty;
            string fileName = string.Empty;

            // Build up a path for our resized image.
            imageFilePath = Path.Combine(ZnodeConfigManager.EnvironmentConfig.ImagePath, imageSize.ToString()) + "/";
            imageFileFullName = Path.Combine(imageFilePath, imageFileName);

            // If file exists and resized image folder and no new image uploaded in original folder then use already resized image.
            if (ZnodeStorageManager.Exists(imageFileFullName))
            {
                imageFileExists = true;
                return imageFileFullName;
            }
            else
            {
                string originalFileName = cropImage ? imageFileName.ToLower().Replace("-swatch.", ".") : imageFileName;

                string originalImagePath = string.Empty;
                originalImagePath = this.useSiteConfig ? Path.Combine(Path.GetDirectoryName(ZnodeConfigManager.SiteConfig.ImageNotAvailablePath), imageFileName) : Path.Combine(ZnodeConfigManager.EnvironmentConfig.OriginalImagePath, originalFileName);
                imageFileExists = false;

                // Check is file exist in Original folder.
                if (ZnodeStorageManager.Exists(originalImagePath))
                {
                    // Resize the image for the current request image size.
                    fileName = this.ResizeImage(originalImagePath, imageSize, imageFilePath);

                    // Crop the image only for swatch.
                    if (cropImage)
                    {
                        //TO DO Work remaining for Portal Display Settings
                        int maxSmallThumbnailWidthHeight = 0;
                        string croppedImageFileName = Path.GetFileName(this.CropImage(originalImagePath, maxSmallThumbnailWidthHeight, maxSmallThumbnailWidthHeight, fileName.ToLower().Replace("-swatch.", ".")));
                        fileName = Path.Combine(imageFilePath, croppedImageFileName);
                    }
                }
                else
                {
                    // If source file doesn't exist in Original folder then return the Image Not Available file.
                    string noImageRelativePath = string.Empty;

                    // Get the no image file from store settings.
                    if (ZnodeConfigManager.SiteConfig.ImageNotAvailablePath.Trim().Length > 0)
                    {
                        noImageRelativePath = ZnodeConfigManager.SiteConfig.ImageNotAvailablePath;
                        if (ZnodeStorageManager.Exists(noImageRelativePath))
                        {
                            // Set to use the SiteConfig for source file lookup.
                            this.useSiteConfig = true;

                            // Call the current method recursively with the site "Not Available" image name.
                            fileName = this.GetImageHttpPath(imageSize, Path.GetFileName(noImageRelativePath), cropImage, out imageFileExists);

                            if (cropImage)
                            {
                                fileName = Path.Combine(imageFilePath, Path.GetFileNameWithoutExtension(fileName) + "-swatch" + Path.GetExtension(fileName));
                            }
                        }
                        else
                        {
                            fileName = imageFileFullName;
                        }
                    }
                    else
                    {
                        noImageRelativePath = Path.Combine(ZnodeConfigManager.EnvironmentConfig.OriginalImagePath, this.noImageFileName);
                        if (ZnodeStorageManager.Exists(noImageRelativePath))
                        {
                            // Call the current method recursively with the "Original" folder this.noImageFileName.
                            fileName = this.GetImageHttpPath(imageSize, this.noImageFileName, cropImage, out imageFileExists);

                            if (cropImage)
                            {
                                fileName = Path.Combine(imageFilePath, Path.GetFileNameWithoutExtension(fileName) + "-swatch" + Path.GetExtension(fileName));
                            }
                        }
                        else
                        {
                            // Image not available in Data/Default/Images and Data/Default/Images/Catalog/Original folders.
                            fileName = imageFileFullName;
                        }
                    }
                }

                // Reset to use EnvironmentConfig for Original file folder.
                this.useSiteConfig = false;

                return fileName;
            }
        }

        // Resizing the image size and storing it in the respective folder and Returns the resized image relative path..
        public string ResizeImage(string relativeImageFilePath, int maxHeight, int maxWidth, string saveToFullPath)
        {
            string fullName = string.Empty;
            string fileName = Path.GetFileName(relativeImageFilePath);
            string[] str = relativeImageFilePath.Split(new char[] { '/' });
            if (this.useSiteConfig)
            {
                fullName = Path.Combine(Path.GetDirectoryName(ZnodeConfigManager.SiteConfig.ImageNotAvailablePath), fileName);
            }
            else
            {
                fullName = Equals(str.Length, 9) ? ZnodeConfigManager.EnvironmentConfig.OriginalImagePath + str[6] + "/" + str[7] + "/" + fileName :
                        ZnodeConfigManager.EnvironmentConfig.OriginalImagePath + fileName;
            }

            byte[] fileData = ZnodeStorageManager.ReadBinaryStorage(fullName);
            MemoryStream ms = new MemoryStream(fileData);
            Image sourceImage = Image.FromStream(ms);
            decimal scaleFactor;
            decimal originalProportion = (decimal)sourceImage.Width / sourceImage.Height;
            decimal resizeProportion = (decimal)maxWidth / maxHeight;

            scaleFactor = originalProportion > resizeProportion ? Convert.ToDecimal(maxWidth) / Convert.ToDecimal(sourceImage.Width) :
                Convert.ToDecimal(maxHeight) / Convert.ToDecimal(sourceImage.Height);

            int newWidth = (int)Math.Round((sourceImage.Width * scaleFactor));
            int newHeight = (int)Math.Round((sourceImage.Height * scaleFactor));
            Bitmap thumbnailBitmap = new Bitmap(newWidth, newHeight);

            Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            thumbnailGraph.Clear(Color.White);

            Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(sourceImage, imageRectangle);

            MemoryStream stream = new MemoryStream();
            Image imageToSave = thumbnailBitmap;
            imageToSave.Save(stream, ImageFormat.Jpeg);
            imageToSave.Dispose();
            stream.Seek(0, SeekOrigin.Begin);
            fileData = new byte[stream.Length];
            stream.Read(fileData, 0, fileData.Length);

            ZnodeStorageManager.WriteBinaryStorage(fileData, Path.Combine(saveToFullPath, fileName));

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            sourceImage.Dispose();

            ms.Close();

            return Path.Combine(saveToFullPath, fileName);
        }

        // Resize the image without losing quality and Returns the image physical path with file name.
        public string ResizeImage(string originalImagePath, int maxSize, string saveToFullPath)
        {
            string fullName = string.Empty;
            string fileName = Path.GetFileName(originalImagePath);
            string[] str = originalImagePath.Split(new char[] { '/' });
            if (this.useSiteConfig)
            {
                fullName = Path.Combine(Path.GetDirectoryName(ZnodeConfigManager.SiteConfig.ImageNotAvailablePath), fileName);
            }
            else
            {
                if (Equals(str.Length, 9))
                {
                    fullName = ZnodeConfigManager.EnvironmentConfig.OriginalImagePath + str[6] + "/" + str[7] + "/" + fileName;
                    saveToFullPath = saveToFullPath + str[6] + "/" + str[7] + "/";
                }
                else
                {
                    fullName = ZnodeConfigManager.EnvironmentConfig.OriginalImagePath + fileName;
                }
            }

            byte[] bytes = ZnodeStorageManager.ReadBinaryStorage(fullName);

            if (Equals(bytes, null) || Equals(bytes.Length, 0))
            {
                return string.Empty;
            }

            MemoryStream ms = new MemoryStream(bytes);
            Image sourceImage = Image.FromStream(ms);
            decimal scaleFactor;
            scaleFactor = sourceImage.Width >= sourceImage.Height ? Convert.ToDecimal(maxSize) / Convert.ToDecimal(sourceImage.Width) :
                Convert.ToDecimal(maxSize) / Convert.ToDecimal(sourceImage.Height);

            int newWidth = (int)Math.Round((sourceImage.Width * scaleFactor));
            int newHeight = (int)Math.Round((sourceImage.Height * scaleFactor));
            Bitmap thumbnailBitmap = new Bitmap(newWidth, newHeight);

            Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            thumbnailGraph.Clear(Color.White);

            Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(sourceImage, imageRectangle);

            MemoryStream stream = new MemoryStream();
            Image imageToSave = thumbnailBitmap;
            imageToSave.Save(stream, ImageFormat.Jpeg);
            imageToSave.Dispose();
            stream.Seek(0, SeekOrigin.Begin);
            bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            ZnodeStorageManager.WriteBinaryStorage(bytes, Path.Combine(saveToFullPath, fileName));

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            sourceImage.Dispose();

            ms.Close();

            return Path.Combine(saveToFullPath, fileName);
        }

        // Method for cropping an image and Returns the image relative path..
        public string CropImage(string originalImagePath, int width, int height, string saveToFullPath)
        {
            string fileName = originalImagePath;

            try
            {
                // Original image
                byte[] fileData = ZnodeStorageManager.ReadBinaryStorage(fileName);
                MemoryStream stream = new MemoryStream(fileData);
                Image imgPhoto = Image.FromStream(stream);

                int targetW = width;
                int targetH = height;
                int targetX = 0;
                int targetY = 0;

                int pointX = imgPhoto.Width / 2;
                int pointY = imgPhoto.Height / 2;

                targetX = pointX - (targetW / 2);
                targetY = pointY - (targetH / 2) - 2;

                Bitmap bmpPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
                bmpPhoto.SetResolution(80, 60);

                Graphics gfxPhoto = Graphics.FromImage(bmpPhoto);
                gfxPhoto.CompositingQuality = CompositingQuality.HighQuality;
                gfxPhoto.SmoothingMode = SmoothingMode.HighQuality;
                gfxPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfxPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfxPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);

                fileName = Path.Combine(Path.GetDirectoryName(saveToFullPath), Path.GetFileNameWithoutExtension(saveToFullPath) + "-Swatch" + Path.GetExtension(fileName));

                stream = new MemoryStream();
                Image imageToSave = bmpPhoto;
                imageToSave.Save(stream, ImageFormat.Jpeg);
                imageToSave.Dispose();
                stream.Seek(0, SeekOrigin.Begin);
                fileData = new byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);

                ZnodeStorageManager.WriteBinaryStorage(fileData, fileName);

                // Dispose of all the objects to prevent memory leaks
                imgPhoto.Dispose();
                bmpPhoto.Dispose();
                gfxPhoto.Dispose();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
            }
            return fileName;
        }

        // Resize the mobile splash images.Splash image width hard-coded as 320 and 640.
        public void ResizeMobileSplashImage(string relativeImageFilePath)
        {
            string splashImageWidthSmallDir = Path.Combine(ZnodeConfigManager.EnvironmentConfig.DataPath, "images/splash/320");
            string splashImageWidthLargeDir = Path.Combine(ZnodeConfigManager.EnvironmentConfig.DataPath, "images/splash/640");

            this.ResizeImage(relativeImageFilePath, 320, splashImageWidthSmallDir + "/");
            this.ResizeImage(relativeImageFilePath, 640, splashImageWidthLargeDir + "/");
        }

        // Get a resized image relative path and Returns the resized relative image path.
        public string GetRelativeImageUrl(int imageSize, string imageFileName, bool cropImage)
        {
            string returnFileName = string.Empty;
            bool isImageFileExists = true;

            if (string.IsNullOrEmpty(imageFileName))
            {
                imageFileName = this.noImageFileName;
            }

            returnFileName = this.GetImageHttpPath(imageSize, imageFileName, cropImage, out isImageFileExists);

            if (!isImageFileExists && !this.isAlreadyChecked)
            {
                isImageFileExists = this.isAlreadyChecked = true;

                // Call the current function recursively to load image on first time.
                returnFileName = this.GetRelativeImageUrl(imageSize, imageFileName, cropImage);
            }

            // Reset the recursion check to default value.
            this.isAlreadyChecked = false;

            return ZnodeStorageManager.HttpPath(returnFileName);
        }
        #endregion
    }
}
