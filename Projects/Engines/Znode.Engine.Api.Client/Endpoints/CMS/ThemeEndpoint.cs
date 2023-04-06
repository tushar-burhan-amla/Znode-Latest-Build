namespace Znode.Engine.Api.Client.Endpoints
{
    public class ThemeEndpoint : BaseEndpoint
    {
        //Get Theme List Endpoint
        public static string List() => $"{ApiRoot}/theme/list";

        //Create Theme Endpoint
        public static string Create() => $"{ApiRoot}/theme";

        //Get theme on the basis of cmsThemeId Endpoint.
        public static string GetTheme(int cmsThemeId) => $"{ApiRoot}/theme/gettheme/{cmsThemeId}";

        //Update theme Endpoint.
        public static string UpdateTheme() => $"{ApiRoot}/theme/updatetheme";

        //Delete Theme Endpoint
        public static string Delete() => $"{ApiRoot}/theme/delete";

        #region Associate Store
        //Endpoint for associate store.
        public static string AssociateStore() => $"{ApiRoot}/theme/associatestore";

        //Remove associated stores endpoint.
        public static string RemoveAssociatedStores() => $"{ApiRoot}/theme/removeassociatedstores";
        #endregion

        #region CMS Widgets

        //Get Area list Endpoint
        public static string AreaList() => $"{ApiRoot}/theme/getareas";
        #endregion
    }
}
