namespace Znode.Engine.Api.Client.Endpoints
{
    public class ManageMessageEndpoint : BaseEndpoint
    {
        #region Manage Message
        //Create Message Endpoint.
        public static string CreateManageMessage() => $"{ApiRoot}/managemessage/createmanagemessage";

        //Get Message List Endpoint.
        public static string GetManageMessages() => $"{ApiRoot}/managemessage/managemessagelist";

        //Get Message details Endpoint.
        public static string GetManageMessage() => $"{ApiRoot}/managemessage/getmanagemessage";

        //Update Message Endpoint.
        public static string UpdateManageMessage() => $"{ApiRoot}/managemessage/updatemanagemessage";

        //Delete Message Endpoint.
        public static string DeleteManageMessage() => $"{ApiRoot}/managemessage/deletemanagemessage";

        //Publish Message endpoint.
        public static string PublishManageMessage() => $"{ApiRoot}/managemessage/publishmanagemessage";

        //Publish Message endpoint.
        public static string PublishManageMessageWithPreview() => $"{ApiRoot}/managemessage/publishmanagemessagewithpreview";        
        #endregion
    }
}
