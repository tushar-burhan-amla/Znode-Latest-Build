using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IManageMessageService
    {
        #region Manage ManageMessages
        /// <summary>
        /// Get ManageMessage list.
        /// </summary>
        /// <param name="expands">Expands for ManageMessage List.</param>
        /// <param name="filters">Filters for ManageMessage List.</param>
        /// <param name="sorts">Sorts of ManageMessage List.</param>
        /// <param name="page">Page size.</param>
        /// <returns>List of messages/returns>
        ManageMessageListModel GetManageMessages(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new ManageMessage.
        /// </summary>
        /// <param name="ManageMessageModel">ManageMessageModel</param>
        /// <returns>ManageMessageModel</returns>
        ManageMessageModel CreateManageMessage(ManageMessageModel manageMessageModel);

        /// <summary>
        /// Get Manage Message details.
        /// </summary>
        /// <param name="manageMessageMapperModel">manageMessageMapperModel</param>
        /// <returns>ManageMessageModel</returns>
        ManageMessageModel GetManageMessage(ManageMessageMapperModel manageMessageMapperModel);

        /// <summary>
        /// Update the ManageMessage.
        /// </summary>
        /// <param name="ManageMessageModel">ManageMessageModel</param>
        /// <returns>boolean value true/false</returns>
        bool UpdateManageMessage(ManageMessageModel manageMessageModel);

        /// <summary>
        /// Delete ManageMessage.
        /// </summary>
        /// <param name="cmsManageMessageId">ParameterModel contains cmsManageMessageIds to delete ManageMessages</param>
        /// <returns>boolean value true/false</returns>
        bool DeleteManageMessage(ParameterModel cmsManageMessageId);

        /// <summary>
        /// Publish content block message
        /// </summary>
        /// <param name="cmsMessageKeyId"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns>boolean value true/false</returns>
        PublishedModel PublishManageMessage(string cmsMessageKeyId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);
                
        #endregion

        #region Web Store
        /// <summary>
        /// Get Message by Message Key, Area and Portal Id.
        /// </summary>
        /// <param name="expands">NameValueCollection expands.</param>
        /// <param name="filters">Filters containing Message Key, Area and Portal Id.</param>
        /// <returns>Returns ManageMessageModel containing required message.</returns>
        ManageMessageModel GetMessage(NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Get Messages by Area and Portal Id.
        /// </summary>
        /// <param name="expands">NameValueCollection expands.</param>
        /// <param name="filters">Filters containing Area and Portal Id.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns ManageMessageListModel containing required messages.</returns>
        ManageMessageListModel GetMessages(NameValueCollection expands, FilterCollection filters, int localeId);
        #endregion
    }
}
