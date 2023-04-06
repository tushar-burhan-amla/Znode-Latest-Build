namespace Znode.Engine.Api.Client.Endpoints
{
    public class MediaManagerEndpoints : BaseEndpoint
    {
        #region Public Method

        //Endpoint for saving mediafile metadata.
        public static string SaveMediaFile() => $"{ApiRoot}/media/create";

        //Endpoint for updating mediafile metadata.
        public static string Update() => $"{ApiRoot}/media/update";

        //Endpoint for saving mediafile metadata.
        public static string AddFolder() => $"{ApiRoot}/media/addfolder";

        //Endpoint for saving mediafile metadata.
        public static string RenameFolder() => $"{ApiRoot}/media/renamefolder";

        //Endpoint for getting medias 
        public static string GetMedias() => $"{ApiRoot}/media/list";

        //Endpoint for getting folder structure for tree.
        public static string GetTree() => $"{ApiRoot}/media/gettree";

        //Endpoint for getting media by media id.
        public static string GetMedia(int mediaId) => $"{ApiRoot}/media/getmedia/{mediaId}";

        //Endpoint for moving folder to another folder.
        public static string MoveFolder() => $"{ApiRoot}/media/movefolder";

        //Endpoint for copying media from one folder to another folder.
        public static string MoveMediaToFolder() => $"{ApiRoot}/media/move";

        //Endpoint for getting media attribute values by media id.
        public static string GetMediaAttributeValues(int mediaId) => $"{ApiRoot}/media/getattributevalues/{mediaId}";

        //Endpoint for delete media.
        public static string DeleteMedia() => $"{ApiRoot}/media/delete";

        //Endpoint for updating media attribute values.
        public static string UpdateMediaAttributeValue() => $"{ApiRoot}/media/attributevalue/update";

        //Endpoint for getting attribute family id by name.
        public static string GetAttributeFamilyIdByName(string extension) => $"{ApiRoot}/media/getattributefamilyidbyname/{extension}";

        //Endpoint for creating media attribute values.
        public static string CreateMediaAttributeValue() => $"{ApiRoot}/media/attributevalues/create";

        //Endpoint for share media folder.
        public static string ShareMediaFolder() => $"{ApiRoot}/media/share";
        
        //Endpoint for getting allowed extensions.
        public static string GetAllowedExtensions() => $"{ApiRoot}/media/getallowedextensions";

        //Generate specific image on edit.
        public static string GenerateImageOnEdit() => $"{ApiRoot}/media/generateimageonedit";

        //Endpoint for getting media by media id.
        public static string GetMediaDetailsById(int mediaId) => $"{ApiRoot}/media/getmediadetailsbyid/{mediaId}";

        //Endpoint for getting media by media Guid.
        public static string GetMediaDetailsByGuid(string mediaGuid) => $"{ApiRoot}/media/getmediadetailsbyguid/{mediaGuid}";
        #endregion
    }
}
