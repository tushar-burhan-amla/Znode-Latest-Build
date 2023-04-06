using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Get the details required for generic navigation control.
        /// </summary>
        /// <param name="model">Parameters required to get details of generic navigation control</param>
        /// <returns>GenericNavigation Model</returns>
        NavigationModel GetNavigationDetails(NavigationParamModel model);
    }
}
