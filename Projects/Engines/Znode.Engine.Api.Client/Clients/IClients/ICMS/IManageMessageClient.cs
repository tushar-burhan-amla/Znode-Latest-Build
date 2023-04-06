using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IManageMessageClient : IBaseClient
    {
        /// <summary>
        /// Get Manage Message list.
        /// </summary>
        /// <param name="expands">Expands for ManageMessage List.</param>
        /// <param name="filters">Filters for ManageMessage List.</param>
        /// <param name="sorts">Sorts of ManageMessage List.</param>
        /// <param name="page">Page size.</param>
        /// <returns>List of messages</returns>
        ManageMessageListModel GetManageMessages(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new manage message.
        /// </summary>
        /// <param name="messageModel">model with message details.</param>
        /// <returns>ManageMessageModel</returns>
        ManageMessageModel CreateManageMessage(ManageMessageModel messageModel);

        /// <summary>
        /// Get message details.
        /// </summary>
        /// <param name="manageMessageMapperModel">manageMessageMapperModel.</param>
        /// <returns>ManageMessageModel</returns>
        ManageMessageModel GetManageMessage(ManageMessageMapperModel manageMessageMapperModel);

        /// <summary>
        /// Update the existing manage message.
        /// </summary>
        /// <param name="messageModel">ManageMessageModel</param>
        /// <returns>boolean value true/false</returns>
        ManageMessageModel UpdateManageMessage(ManageMessageModel messageModel);

        /// <summary>
        /// Delete ManageMessage.
        /// </summary>
        /// <param name="cmsManageMessageId">ParameterModel contains cmsManageMessageIds to delete managemessages</param>
        /// <returns>boolean value true/false</returns>
        bool DeleteManageMessage(ParameterModel cmsManageMessageId);

        /// <summary>
        /// Publish ManageMessage
        /// </summary>
        /// <param name="contentPageParameterModel"></param>
        /// <returns>boolean value true/false</returns>
        PublishedModel PublishManageMessage(ContentPageParameterModel contentPageParameterModel);        
    }
}
