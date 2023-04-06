namespace Znode.Engine.Api.Client.Endpoints
{
    public class MediaConfigurationEndpoint : BaseEndpoint
    {
        //Endpoint to get media server list.
        public static string GetMediaServerList() => $"{ApiRoot}/getmediaserver/list";

        //Endpoint to update existing media configuration setting.
        public static string Update() => $"{ApiRoot}/mediaconfiguration/update";

        //Endpoint to get media configuration setting.
        public static string GetMediaConfiguration() => $"{ApiRoot}/mediaconfiguration";

        //Endpoint to create new media configuration setting.
        public static string Create() => $"{ApiRoot}/mediaconfiguration/create";

        //Endpoint to get default media configuration setting.
        public static string GetDefaultMediaConfiguration() => $"{ApiRoot}/defaultmediaconfiguration";

        //Endpoint to sync new media.
        public static string SyncMedia(string folderName) => $"{ApiRoot}/mediaconfiguration/syncmedia/{folderName}";

        #region Generate Images.
        //Endpoint to generate all images.
        public static string GenerateImages() => $"{ApiRoot}/mediaconfiguration/generateimages";
        #endregion

        #region Display Setting.
        //Endpoint to get display setting data.
        public static string GetDisplaySetting() => $"{ApiRoot}/mediaconfiguration/globalmediadisplaysetting";
        #endregion
    }
}
