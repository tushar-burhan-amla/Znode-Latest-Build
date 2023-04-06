using System.Collections.Specialized;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface ITagManagerService
    {

        /// <summary>
        /// Get tag manager data for store.
        /// </summary>
        /// <param name="portalId">Id to get tag manager data for store.</param>
        /// <param name="expands">Expands for tag manager data.</param>
        /// <returns>Returns model with tag manager information.</returns>
        TagManagerModel GetTagManager(int portalId, NameValueCollection expands);

        /// <summary>
        /// Save tag manager data.
        /// </summary>
        /// <param name="tagManagerModel">Model with tag manager data.</param>
        /// <returns>Returns true if tag manager data updated successfully else returns false.</returns>
        bool SaveTagManager(TagManagerModel tagManagerModel);
    }
}
