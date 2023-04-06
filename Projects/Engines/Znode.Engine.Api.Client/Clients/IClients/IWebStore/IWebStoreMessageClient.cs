using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreMessageClient : IBaseClient
    {
        /// <summary>
        /// Get message by Message Key, Area and Portal Id passed in filters.
        /// </summary>
        /// <param name="expands">Expand Collection if any.</param>
        /// <param name="filters">Filter containing Message Key, Area and Portal Id.</param>
        /// <returns>Returns ManageMessageModel returning required message.</returns>
        ManageMessageModel GetMessage(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Get message by Area and Portal Id passed in filters.
        /// </summary>
        /// <param name="expands">Expand Collection if any.</param>
        /// <param name="filters">Filter containing Area and Portal Id.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns ManageMessageListModel returning required messages.</returns>
        ManageMessageListModel GetMessages(ExpandCollection expands, FilterCollection filters, int localeId);

        /// <summary>
        /// Get content container based on the container Key passed. 
        /// </summary>
        /// <param name="containerKey">Container Key</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="profileId">Profile Id</param>
        /// <returns></returns>
        ContentContainerDataModel GetContentContainer(string containerKey, int localeId, int portalId = 0, int profileId = 0);
    }
}
