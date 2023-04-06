namespace Znode.Engine.Services
{
    public interface IImageHelper
    {
        //Get Original image path.
        string GetOriginalImagepath(string imageFileName);

        //Get Image large path.
        string GetImageHttpPathLarge(string imageFileName);

        //Get image medium path.
        string GetImageHttpPathMedium(string imageFileName);

        //Get image small path.
        string GetImageHttpPathSmall(string imageFileName);

        //Get image thumbnail path.
        string GetImageHttpPathThumbnail(string imageFileName);

        //Get image small thumbnail path.
        string GetImageHttpPathSmallThumbnail(string imageFileName);

        //Get Image URL.
        string GetRelativeImageUrl(int? imageSize, string imageFileName, bool isOriginalImagePath);

        //Gey Full path for image.
        string GetImageHttpPath(int imageSize, string imageFileName, bool isoriginalImagePath);

        //Gey Full path for image.
        string GetImageNetworkPath(int imageSize, string imageFileName, bool isoriginalImagePath);

    }
}
