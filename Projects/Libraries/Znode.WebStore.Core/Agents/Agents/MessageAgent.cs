using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore.Agents
{
    public class MessageAgent : BaseAgent, IMessageAgent
    {
        #region Private Variables
        //Field(s) not marked as readonly is intentional. Since on Locale change, the client instance has to be recycled.
        private IWebStoreMessageClient _messageClient;
        #endregion

        #region Constructors
        public MessageAgent(IWebStoreMessageClient messageClient)
        {
            _messageClient = GetClient<IWebStoreMessageClient>(messageClient);
        }
        #endregion
        //Get Message by Message key, area for current portal.
        public virtual string GetMessage(string key, string area)
        {
            try
            {
                //Get message list by area name.
                List<ManageMessageModel> model = GetMessages(area);
                if (HelperUtility.IsNotNull(model))
                    return (model.FirstOrDefault(x => x.MessageKey.Equals(key))?.Message)?.Replace("<p>", string.Empty).Replace("</p>", string.Empty);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
            return string.Empty;
        }

        //Get Portal messages from API.
        private List<ManageMessageModel> GetMessages(string area)
        {
            string portalId = Convert.ToString(PortalAgent.CurrentPortal?.PortalId);



            string cacheKey = string.Concat("MessageKey_", portalId, PortalAgent.CurrentPortal.PublishState,PortalAgent.LocaleId);
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeCMSPortalMessageEnum.PortalId.ToString(), FilterOperators.Equals, portalId));

                _messageClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
                _messageClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
                _messageClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
                ManageMessageListModel model = _messageClient.GetMessages(null, filters, PortalAgent.LocaleId);

                //Add Message collection in cache.
                if (model != null && model.ManageMessages != null && model.ManageMessages.Any())
                {
                    List<ManageMessageModel> _messageList = new List<ManageMessageModel>();
                    _messageList.AddRange(model.ManageMessages);
                    Helper.AddIntoCache(_messageList, cacheKey, "ManageMessageCacheDuration");
                }
                else
                {
                    ZnodeLogging.LogMessage($"MessageAgent.GetMessages No messages area {area}, portalId {portalId}", "Message", TraceLevel.Info);
                }
            }

            return Helper.GetFromCache<List<ManageMessageModel>>(cacheKey);
        }

        //Get container based on the container Key
        public virtual ContentContainerDataViewModel GetContainer(string containerKey)
        {
            int portalId = PortalAgent.CurrentPortal.PortalId;
            int localeId = Helper.GetLocaleId().GetValueOrDefault();
            int profileId = Helper.GetProfileId().GetValueOrDefault();
            string cacheKey = string.Concat("ContainerKey_", containerKey, Convert.ToString(portalId), Convert.ToString(profileId), Convert.ToString(localeId));
            //Add content container in cache.
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                ContentContainerDataViewModel model = _messageClient.GetContentContainer(containerKey, localeId, portalId, profileId).ToViewModel<ContentContainerDataViewModel>();
                if (HelperUtility.IsNotNull(model) && HelperUtility.IsNotNull(model.Groups) && HelperUtility.IsNotNull(model.Attributes))
                {
                    Helper.AddIntoCache(model, cacheKey, "CurrentPortalCacheDuration");
                }
                else
                {
                    ZnodeLogging.LogMessage($"MessageAgent.GetContainer No container {containerKey}, portalId {portalId}, localeId {localeId}, profileId {profileId}", "Message", TraceLevel.Info);
                }
                return model;
            }
            else
            {
                return Helper.GetFromCache<ContentContainerDataViewModel>(cacheKey);
            }
        }
    }
}