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
    public class PublishHistoryService : BaseService, IPublishHistoryService
    {

        #region Public Method
        // List of log message
        public PublishHistoryListModel GetPublishHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            // bind to page list and get the list of log message 
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList :", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            List<ZnodePublishPreviewLogEntity> historyLogs = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetPreviewLogEntity(pageListModel);
                
            ZnodeLogging.LogMessage("historyLogs count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { historyLogs?.Count });

            //map logmessage entity to logmessage model
            PublishHistoryListModel logMessage = new PublishHistoryListModel() { PublishHistoryList = historyLogs?.ToModel<PublishHistoryModel>()?.ToList() };

            logMessage.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return logMessage;
        }

        public virtual void DeleteProductLogs(int versionId) => PreviewHelper.DeletePublishPreviewLog(versionId);

        #endregion
    }
}
