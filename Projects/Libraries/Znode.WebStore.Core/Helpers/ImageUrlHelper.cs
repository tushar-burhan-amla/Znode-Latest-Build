using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore;
namespace Znode.WebStore.Core.Helpers
{
    public static class ImageUrlHelper
    {
        /// <summary>
        /// Use for getting full image name with large path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return large image full path with the requested image name
        /// </returns>
        public static string ImageLargePath(string imageName)
        {
            if(!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageLargeUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with medium path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return medium image full path with the requested image name 
        /// </returns>
        public static string ImageMediumPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageMediumUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with small path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>Return small image full path with the requested image name 
        /// </returns>
        public static string ImageSmallPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageSmallUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with Cross-Sell path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return cross-sell image full path with the requested image name 
        /// </returns>
        public static string ImageCrossSellPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageCrossSellUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with small thumbnail path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return small thumbnail image full path with the requested image name 
        /// </returns>
        public static string ImageSmallThumbnailPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageSmallThumbnailUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with thumbnail path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return thumbnail image full path with the requested image name 
        /// </returns>
        public static string ImageThumbnailPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.ImageSmallThumbnailUrl}{imageName}";
            }
            return null;
        }

        /// <summary>
        /// Use for getting full image name with server path
        /// </summary>
        /// <param name="imageName">Image file name</param>
        /// <returns>
        /// Return image server full path with the requested image name 
        /// </returns>
        public static string ImageServerPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.MediaServerUrl}{imageName}";
            }
            return null;
        }

        public static string GetImageWithThumbnailPath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return $"{PortalAgent.CurrentPortal.MediaServerUrl}{WebStoreConstants.Thumbnail}{imageName}";
            }
            return null;
        }
    }
}
