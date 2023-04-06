using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class ERPConnectorAgent : BaseAgent, IERPConnectorAgent
    {
        #region Private Variables
        private readonly IERPConnectorClient _erpConnectorClient;
        private readonly IERPConfiguratorAgent _erpConfiguratorAgent;
        #endregion

        #region Constructor
        public ERPConnectorAgent(IERPConnectorClient erpConnectorClient)
        {
            _erpConnectorClient = GetClient<IERPConnectorClient>(erpConnectorClient);
            _erpConfiguratorAgent = new ERPConfiguratorAgent(GetClient<ERPConfiguratorClient>());
        }
        #endregion

        #region Public Methods

        //Get ERP Connector controls.
        public virtual ERPConnectorListViewModel GetERPConnectorControls()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue);
            ERPConfiguratorListViewModel erpConfiguratorListViewModel = _erpConfiguratorAgent.GetERPConfiguratorList(null, filters, null, null, null);
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { filters = filters });
            ERPConnectorListViewModel erpConnectorListViewModel = new ERPConnectorListViewModel() { Properties = new List<Property>() };
            if (erpConfiguratorListViewModel?.ERPConfiguratorList?.Count > 0)
            {
                ERPConnectorControlListModel erpConnectorControlListModel = _erpConnectorClient.GetERPConnectorControls(erpConfiguratorListViewModel.ERPConfiguratorList.FirstOrDefault().ToModel<ERPConfiguratorModel>());
                erpConnectorListViewModel.Properties = erpConnectorControlListModel?.ERPConnectorControlList?.ToViewModel<Property>().ToList();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpConnectorListViewModel;
        }

        // Method to Save ERP Control Data in json file.
        public virtual ERPConnectorListViewModel CreateERPControlData(BindDataModel bindDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            try
            {
                ERPConnectorListViewModel viewModel = GetERPConnectorViewModel(bindDataModel);
                if (viewModel?.ERPConnectorControlList?.Count > 0)
                {
                    List<ERPConnectorControlModel> list = _erpConnectorClient.CreateERPControlData(new ERPConnectorControlListModel() { ERPConnectorControlList = viewModel.ERPConnectorControlList.ToModel<ERPConnectorControlModel>().ToList() })?.ERPConnectorControlList;
                    ZnodeLogging.LogMessage("ERPConnectorControlModel list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, list?.Count());
                    viewModel.ERPConnectorControlList = list.ToViewModel<ERPConnectorViewModel>().ToList();
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return viewModel;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                return new ERPConnectorListViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        #endregion
        #region Private Methods
        //Get attribute Data
        private ERPConnectorListViewModel GetERPConnectorViewModel(BindDataModel bindDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPConnectorListViewModel erpConnectorListViewModel = new ERPConnectorListViewModel();
            foreach (var item in bindDataModel.ControlsData)
            {
                ERPConnectorViewModel erpConnectorViewModel = new ERPConnectorViewModel();
                erpConnectorViewModel.Name = item.Key;
                erpConnectorViewModel.Value = item.Value.ToString();
                erpConnectorListViewModel.ERPConnectorControlList.Add(erpConnectorViewModel);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpConnectorListViewModel;
        }

        #endregion
    }

}
