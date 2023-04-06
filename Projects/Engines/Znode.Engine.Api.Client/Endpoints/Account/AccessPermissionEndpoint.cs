namespace Znode.Engine.Api.Client.Endpoints
{
    public class AccessPermissionEndpoint : BaseEndpoint
    {
        public static string AccountPermissionList() => $"{ApiRoot}/accesspermission/accountpermissionlist";

        public static string DeleteAccountPermission() => $"{ApiRoot}/accesspermission/deleteaccountpermission";

        public static string CreateAccountPermission() => $"{ApiRoot}/accesspermission/createaccountpermission";

        public static string UpdateAccountPermission() => $"{ApiRoot}/accesspermission/updateaccountpermission";

        public static string GetAccountPermission() => $"{ApiRoot}/accesspermission/getaccountpermission";

        public static string AccessPermissionList()=> $"{ApiRoot}/accesspermission/accesspermissionlist";
    }
}
