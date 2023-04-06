namespace Znode.Engine.Api.Client.Endpoints
{
    public class LocaleEndpoint : BaseEndpoint
    {
        #region Locale
        //Get Locale list endpoint.
        public static string GetLocaleList() => $"{ApiRoot}/locale/list";

        //Get a Locale Endpoint
        public static string GetLocale() => $"{ApiRoot}/locale";

        //Update Locale endpoint
        public static string UpdateLocale() => $"{ApiRoot}/locale/update";

        #endregion
    }
}
