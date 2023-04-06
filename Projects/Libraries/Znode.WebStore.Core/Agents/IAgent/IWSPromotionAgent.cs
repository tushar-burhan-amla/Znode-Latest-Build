using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.WebStore.Core.Agents
{
   public interface IWSPromotionAgent 
    {
        /// <summary>
        /// Get promotion list for webstore.
        /// </summary>
        /// <returns>Returns promotion list view model.</returns>
        PromotionListViewModel GetPromotionListByPortalId(int portalId);
    }
}
