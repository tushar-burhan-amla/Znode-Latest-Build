using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class PublishHistoryAgent : BaseAgent, IPublishHistoryAgent
    {
        #region Private Variables
        private readonly IPublishHistoryClient _publishHistoryClient;
        #endregion

        #region Constructor
        public PublishHistoryAgent(IPublishHistoryClient publishHistoryClient)
        {
            _publishHistoryClient = publishHistoryClient;
        }
        #endregion

        #region Public Method

        public PublishHistoryListViewModel GetPublishHistoryList(string publishState, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;

            publishState = filters.FirstOrDefault(x => x.FilterName == ZnodeConstant.SourcePublishState)?.FilterValue;
            if (!string.IsNullOrEmpty(publishState))
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.SourcePublishState, StringComparison.OrdinalIgnoreCase));
                filters.Add(ZnodeConstant.SourcePublishState, FilterOperators.Is, publishState);
            }

            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.LogCreatedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { expands= expands, filters = filters, sorts = sorts });
            PublishHistoryListModel list = _publishHistoryClient.List(expands, filters, sorts, pageIndex, pageSize);
            PublishHistoryListViewModel listViewModel = new PublishHistoryListViewModel { PublishHistoryList = list?.PublishHistoryList?.ToViewModel<PublishHistoryViewModel>().ToList() };

            SetListPagingData(listViewModel, list);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return list?.PublishHistoryList?.Count > 0 ? listViewModel : new PublishHistoryListViewModel() { PublishHistoryList = new List<PublishHistoryViewModel>() };
        }

        public bool DeleteProductLog(int versionid) => _publishHistoryClient.DeleteProductLogs(versionid);

        #endregion
    }
}
