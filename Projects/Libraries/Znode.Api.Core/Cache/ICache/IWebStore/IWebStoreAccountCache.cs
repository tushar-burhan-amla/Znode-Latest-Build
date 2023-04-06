namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreAccountCache
    {
        /// <summary>
        /// Get a list of user address.
        /// </summary>   
        /// /// <param name="routeUri">URI to route.</param>       
        /// /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>list of user address</returns>
        string GetUserAddressList(string routeUri, string routeTemplate);       
    }
}