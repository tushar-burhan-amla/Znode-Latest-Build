using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class StateAgent : BaseAgent, IStateAgent
    {
        #region Private Variables
        private readonly IStateClient _stateClient;
        #endregion

        #region Constructor
        public StateAgent(IStateClient stateClient)
        {
            _stateClient = GetClient<IStateClient>(stateClient);
        }
        #endregion

        #region public Methods

        //Method to get State list
        public virtual StateListViewModel GetStateList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sorts: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }

            StateListModel stateListModel = _stateClient.GetStateList(filters, sorts, pageIndex, pageSize);
            StateListViewModel listViewModel = new StateListViewModel { States = stateListModel?.States?.ToViewModel<StateViewModel>().ToList() };

            SetListPagingData(listViewModel, stateListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return stateListModel?.States?.Count > 0 ? listViewModel : new StateListViewModel() { States = new List<StateViewModel>() };
        }

        //Method to get active State list
        public virtual List<SelectListItem> GetActiveStateList(FilterCollection filters, string stateCode = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (IsNull(filters))
                filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            StateListViewModel stateList = GetStateList(filters);

            List<SelectListItem> selectedStateList = new List<SelectListItem>();
            if (stateList?.States?.Count > 0)
            {
                if (!string.IsNullOrEmpty(stateCode))
                    stateList.States.ForEach(item => { selectedStateList.Add(new SelectListItem() { Text = item.StateName, Value = item.StateCode, Selected = item.StateCode == stateCode ? true : false }); });
                stateList.States.ForEach(item => { selectedStateList.Add(new SelectListItem() { Text = item.StateName, Value = item.StateCode }); });
            }
            ZnodeLogging.LogMessage("selectedStateList count: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, selectedStateList?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return selectedStateList;
        }

        #endregion
    }
}