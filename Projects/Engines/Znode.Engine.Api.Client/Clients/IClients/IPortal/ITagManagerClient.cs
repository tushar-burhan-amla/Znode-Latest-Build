using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ITagManagerClient:IBaseClient
    {
        /// <summary>
        /// Save tag manager data.
        /// </summary>
        /// <param name="tagManagerModel">Model with tag manager data.</param>
        /// <returns>Model with tag manager information.</returns>
        bool SaveTagManager(TagManagerModel tagManagerModel);

        /// <summary>
        /// Get tag manager data for store.
        /// </summary>
        /// <param name="portalId">Portal id to get tag manager data.</param>
        /// <param name="expands">Expand for tag manager data.</param>
        /// <returns>Model with tag manager information.</returns>
        TagManagerModel GetTagManager(int portalId, ExpandCollection expands);
    }
}
