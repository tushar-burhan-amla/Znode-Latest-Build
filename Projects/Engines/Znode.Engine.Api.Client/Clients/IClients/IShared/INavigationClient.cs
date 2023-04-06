using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface INavigationClient : IBaseClient
    {
        /// <summary>
        /// Get the details required for generic navigation control.
        /// </summary>
        /// <param name="model">Parameters required to get details of generic navigation control</param>
        /// <returns>Navigation Model</returns>
        NavigationModel GetNavigationDetails(NavigationParamModel model);
    }
}
