namespace Znode.Engine.Api.Client.Endpoints
{
    public class ApplicationSettingsEndpoint : BaseEndpoint
    {
        public static string GetFilterConfigurationXML(string itemName, int? userId = null) => $"{ApiRoot}/applicationsettings/{itemName}/{userId}";

        //XML Configuration Endpoints
        public static string Create() => $"{ApiRoot}/applicationsettings";
        public static string CreateNewView() => $"{ApiRoot}/applicationsettings/createnewview";

        public static string DeleteView() => $"{ApiRoot}/applicationsettings/deleteview";

        public static string GetView(int itemViewId) => $"{ApiRoot}/applicationsettings/getviewbyid/{itemViewId}";

        public static string UpdateViewSelectedStatus(int applicationSettingId) => $"{ApiRoot}/applicationsettings/updateviewselectedstatus/{applicationSettingId}";

        public static string ColumnList(string entityType, string entityName) => $"{ApiRoot}/applicationsettings/getcolumnlist/{entityType}/{entityName}";
        public static string List() => $"{ApiRoot}/applicationsettings";
    }
}
