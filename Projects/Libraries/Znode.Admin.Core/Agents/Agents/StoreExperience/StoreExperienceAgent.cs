using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Znode.Admin.Core.Helpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Agents
{
    public class StoreExperienceAgent : BaseAgent, IStoreExperienceAgent
    {
        #region Private Variables
        private readonly IPortalClient _portalClient;
        #endregion

        public StoreExperienceAgent(IPortalClient portalClient)
        {
            _portalClient = GetClient<IPortalClient>(portalClient);
        }

        //Get the list of all stores.
        public virtual StoreListViewModel GetStoreExperienceList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?))
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            sorts = HelperMethods.SortAsc(ZnodePortalEnum.StoreName.ToString(), sorts);
            PortalListModel storeList = _portalClient.GetPortalList(new ExpandCollection { ZnodePortalEnum.ZnodeDomains.ToString().ToLower() }, filters, sorts, pageIndex, pageSize);
            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList() };
            storeListViewModel?.StoreList?.ForEach(item => { item.UrlEncodedStoreName = HttpUtility.UrlEncode(item.StoreName); });

            //Set the Tool Menus for Store List Grid View.
            //SetStoreListToolMenus(storeListViewModel);

            SetListPagingData(storeListViewModel, storeList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return storeListViewModel;
        }
    }
}
