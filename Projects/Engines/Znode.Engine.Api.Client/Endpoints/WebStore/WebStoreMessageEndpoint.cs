namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreMessageEndpoint : BaseEndpoint
    {
        //Get Message by Message Key, Area and Portal Id.
        public static string GetMessage() => $"{ApiRoot}/webstoremessage/get";

        //Get Messages by Locale Id and Portal Id.
        public static string GetMessages(int localeId) => $"{ApiRoot}/webstoremessage/list/{localeId}";

        //Get container variant data on the basis of passed input parameters and variant precedence.
        public static string GetContainer() => $"{ApiRoot}/contentcontainer/getcontentcontainerdata";
    }
}
