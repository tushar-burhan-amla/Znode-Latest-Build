namespace Znode.Engine.Api.Cache
{
    public interface IAddressCache
    {
        /// <summary>
        /// Get address list
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAddressList(string routeUri, string routeTemplate);
    }
}
