using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public partial class ManageMessageService
    {
        //To Do
        //Get message by message key, area and portal id.
        public virtual ManageMessageModel GetMessage(NameValueCollection expands, FilterCollection filters) => new ManageMessageModel();

        // Get messages list by locale id and portal id.
        public virtual ManageMessageListModel GetMessages(NameValueCollection expands, FilterCollection filters, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            //Get portalId
            int portalId;          
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            IPublishedPortalDataService publishedDataService = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>();


            List<ZnodePublishMessageEntity> messages = publishedDataService.GetMessageList(portalId, localeId);

            List<ZnodePublishGlobalMessageEntity> globalMessages = publishedDataService.GetGlobalMessageList(localeId);

            var resultmessages = globalMessages?.Where(gm => gm.LocaleId == localeId && !messages.Any(mes => mes.MessageKey == gm.MessageKey && mes.LocaleId == gm.LocaleId));

            foreach (var resultmessage in resultmessages)
            {
                messages.Add(new ZnodePublishMessageEntity() {
                    LocaleId = resultmessage.LocaleId,
                    MessageKey = resultmessage.MessageKey,
                    Message = resultmessage.Message,
                    PortalId = null,                    
                });
            }
            ZnodeLogging.LogMessage("MessageEntity and GlobalMessageEntity list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, messages?.Count);

            return new ManageMessageListModel() { ManageMessages = messages?.ToModel<ManageMessageModel>()?.ToList() };
        }
    }

}
