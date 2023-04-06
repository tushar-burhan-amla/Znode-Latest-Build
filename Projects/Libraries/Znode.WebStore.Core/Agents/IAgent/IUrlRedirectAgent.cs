using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.Agents
{
    public interface IUrlRedirectAgent
    {
        /// <summary>
        /// Get active 301 Url Redirects.
        /// </summary>
        /// <returns>Returns active 301 Url Redirects.</returns>
        UrlRedirectListModel GetActive301Redirects();
    }
}
