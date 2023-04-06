namespace Znode.Engine.Api.Cache
{
    public interface IAccessPermissionCache
    {
        /// <summary>
        /// Get account permission list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string AccountPermissionList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get account permission.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAccountPermission(string routeUri, string routeTemplate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string AccessPermissionList(string routeUri, string routeTemplate);
    }
}
