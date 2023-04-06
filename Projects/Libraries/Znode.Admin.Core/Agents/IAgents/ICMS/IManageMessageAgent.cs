using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IManageMessageAgent
    {
        #region Manage Message
        /// <summary>
        /// Get Message list. 
        /// </summary>
        /// <param name="expands">Expands for Message.</param>
        /// <param name="filters">Filters for Message.</param>
        /// <param name="sorts">Sorts for Message.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Message.</returns>
        ManageMessageListViewModel GetManageMessages(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get ManageMessage details.
        /// </summary>
        /// <param name="cmsMessageKeyId">cmsMessageKeyId.</param>
        /// <param name="portalId">portalId.</param>
        /// <param name="localeId">localeId.</param>
        /// <returns>Returns manage message details.</returns>
        ManageMessageViewModel GetManageMessage(int cmsMessageKeyId, int portalId, int localeId);

        /// <summary>
        /// Create ManageMessage.
        /// </summary>
        /// <param name="manageMessageViewModel">ManageMessageViewModel.</param>
        /// <returns>Returns ManageMessageViewModel.</returns>
        ManageMessageViewModel CreateManageMessage(ManageMessageViewModel manageMessageViewModel);

        /// <summary>
        /// Update ManageMessage.
        /// </summary>
        /// <param name="manageMessageViewModel">ManageMessageViewModel.</param>
        /// <returns>Returns updated ManageMessageViewModel.</returns>
        ManageMessageViewModel UpdateManageMessage(ManageMessageViewModel manageMessageViewModel);

        /// <summary>
        /// Delete ManageMessage.
        /// </summary>
        /// <param name="cmsManageMessageId">cmsManageMessageIds to be deleted.</param>
        /// <returns>Returns true if ManageMessage deleted successfully else return false.</returns>
        bool DeleteManageMessage(string cmsManageMessageId);

        /// <summary>
        /// Get manage message view model,
        /// </summary>
        /// <param name="viewModel">view model of manage message</param>
        void GetManageMessageViewModel(ManageMessageViewModel viewModel);

        /// <summary>
        /// Publish Message
        /// </summary>
        /// <param name="cmsMessageId">cmsMessageIds to publish the message</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false on publish true else false</returns>
        bool PublishManageMessage(string cmsMessageId, int portalId, out string errorMessage);

        /// <summary>
        /// Publish Message
        /// </summary>
        /// <param name="cmsMessageId"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        bool PublishManageMessage(string cmsMessageId, int portalId, int localeId, out string errorMessage, string targetPublishState = null, bool takeFromDraftFirst = false);
        
        #endregion

    }
}
