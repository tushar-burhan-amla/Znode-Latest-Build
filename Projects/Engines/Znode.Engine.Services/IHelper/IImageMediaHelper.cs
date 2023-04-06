using System.Collections.Generic;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Services
{
    public interface IImageMediaHelper
    {
        /// <summary>
        /// This method is used to Generate the image
        /// </summary>
        /// <param name="mediaPath">Image path</param>
        void GenerateImageOnEdit(string mediaPath);

        /// <summary>
        /// This method is used to resize the image
        /// </summary>
        /// <param name="mediaList"></param>
        /// <param name="imageHelperModel"></param>
        //generate resize images
        void GenerateResizedImages(List<string> mediaList, ImageModel imageHelperModel);
    }
}
