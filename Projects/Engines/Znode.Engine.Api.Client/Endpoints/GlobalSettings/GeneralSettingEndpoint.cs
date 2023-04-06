namespace Znode.Engine.Api.Client.Endpoints
{
    public class GeneralSettingEndpoint : BaseEndpoint
    {
        //Endpoint to get lists of General Settings.
        public static string List() => $"{ApiRoot}/generalsetting/list";

        //Endpoint update the existing General Setting.
        public static string Update() => $"{ApiRoot}/generalsetting/update";

        //Endpoint to get cache management data.
        public static string CacheData() => $"{ApiRoot}/generalsetting/getcachedata";

        //Endpoint to create/update cache data.
        public static string CreateUpdateCache() => $"{ApiRoot}/generalsetting/createupdatecache";

        //Endpoint to refresh cache data.
        public static string RefreshCacheData() => $"{ApiRoot}/generalsetting/refreshcache";

        //Endpoint to get lists of all publish states.
        public static string PublishStateMappingList() => $"{ApiRoot}/publishstatemapping/list";

        //Endpoint to enable/disable publish state to application type mapping.
        public static string EnableDisablePublishStateMapping(bool isEnabled) => $"{ApiRoot}/publishstatemapping/enabledisable/{isEnabled}";

        //Get global configuration settings for application.
        public static string GetConfigurationSettings() => $"{ApiRoot}/generalsetting/getconfigurationsettings";

        //Update global configuration settings for application.
        public static string UpdateConfigurationSettings() => $"{ApiRoot}/generalsetting/updateconfigurationsettings";

        //Get the Power Bi details
        public static string GetPowerBISettings() => $"{ApiRoot}/generalsetting/getpowerbisettings";

        //Updated the Power Bi details
        public static string UpdatePowerBISettings() => $"{ApiRoot}/generalsetting/updatepowerbisettings";

        // Get stock notice settings for application.
        public static string GetStockNoticeSettings() => $"{ApiRoot}/generalsetting/getstocknoticesettings";

        // Update stock notice settings for application.
        public static string UpdateStockNoticeSettings() => $"{ApiRoot}/generalsetting/updatestocknoticesettings";
    }
}
