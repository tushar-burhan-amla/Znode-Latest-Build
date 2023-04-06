using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
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

namespace Znode.Engine.Admin.Agents
{
    public class ERPConfiguratorAgent : BaseAgent, IERPConfiguratorAgent
    {
        #region Private Variables
        private readonly IERPConfiguratorClient _eRPConfiguratorClient;
        #endregion

        #region Constructor
        public ERPConfiguratorAgent(IERPConfiguratorClient eRPConfiguratorClient)
        {
            _eRPConfiguratorClient = GetClient<IERPConfiguratorClient>(eRPConfiguratorClient);
        }
        #endregion

        #region public Methods
        // Get the list of ERP Configurator.
        public virtual ERPConfiguratorListViewModel GetERPConfiguratorList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filter and sorts: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            ERPConfiguratorListModel eRPConfiguratorList = _eRPConfiguratorClient.GetERPConfiguratorList(expands, filters, sorts, pageIndex, pageSize);
            ERPConfiguratorListViewModel listViewModel = new ERPConfiguratorListViewModel { ERPConfiguratorList = eRPConfiguratorList?.ERPConfiguratorList?.ToViewModel<ERPConfiguratorViewModel>().ToList() };
            SetListPagingData(listViewModel, eRPConfiguratorList);

            //Set tool menu for ERP classes list grid view.
            SetERPConfiguratorListToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return eRPConfiguratorList?.ERPConfiguratorList?.Count > 0 ? listViewModel : new ERPConfiguratorListViewModel() { ERPConfiguratorList = new List<ERPConfiguratorViewModel>() };
        }

        // Get all ERP Configurator Class which are not present in database.
        public virtual List<SelectListItem> GetAllERPConfiguratorClassesNotInDatabase()
           => ERPConfiguratorViewModelMap.ToERPConfiguratorClassesListItems(_eRPConfiguratorClient.GetAllERPConfiguratorClassesNotInDatabase().ERPConfiguratorList);

        //Get eRPConfigurator by eRPConfigurator id.
        public virtual ERPConfiguratorViewModel GetERPConfigurator(int eRPConfiguratorId)
            => _eRPConfiguratorClient.GetERPConfigurator(eRPConfiguratorId).ToViewModel<ERPConfiguratorViewModel>();

        //Create new ERP Configurator Class.
        public virtual ERPConfiguratorViewModel Create(ERPConfiguratorViewModel eRPConfiguratorViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                ERPConfiguratorModel eRPConfiguratorModel = _eRPConfiguratorClient.Create(eRPConfiguratorViewModel.ToModel<ERPConfiguratorModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return IsNotNull(eRPConfiguratorModel) ? eRPConfiguratorModel.ToViewModel<ERPConfiguratorViewModel>() : new ERPConfiguratorViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ERPConfiguratorViewModel)GetViewModelWithErrorMessage(eRPConfiguratorViewModel, Admin_Resources.AlreadyExistCode);
                    default:
                        return (ERPConfiguratorViewModel)GetViewModelWithErrorMessage(eRPConfiguratorViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                return (ERPConfiguratorViewModel)GetViewModelWithErrorMessage(eRPConfiguratorViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update eRPConfigurator.
        public virtual ERPConfiguratorViewModel Update(ERPConfiguratorViewModel eRPConfiguratorViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                ERPConfiguratorModel eRPConfiguratorModel = _eRPConfiguratorClient.Update(eRPConfiguratorViewModel.ToModel<ERPConfiguratorModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return IsNotNull(eRPConfiguratorModel) ? eRPConfiguratorModel.ToViewModel<ERPConfiguratorViewModel>() : (ERPConfiguratorViewModel)GetViewModelWithErrorMessage(new ERPConfiguratorViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                return (ERPConfiguratorViewModel)GetViewModelWithErrorMessage(new ERPConfiguratorViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete eRPConfigurator.
        public virtual bool Delete(string eRPConfiguratorId, out string errorMessage)
        {
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return _eRPConfiguratorClient.Delete(new ParameterModel { Ids = eRPConfiguratorId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotDeleteActiveRecord:
                        errorMessage = ERP_Resources.ErrorDeleteActiveClass;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Enable disable ERPConfigurator.
        public virtual bool EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive)
        => _eRPConfiguratorClient.EnableDisableERPConfigurator(eRPConfiguratorId, isActive);
        #endregion

        #region Private Methods
        //Method for Set ERPConfigurator List ToolMenus
        private void SetERPConfiguratorListToolMenus(ERPConfiguratorListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ERPConfiguratorDeletePopup')", ControllerName = "ERPConfigurator", ActionName = "Delete" });
            }
        }

        //Method for class name of active ERP. 
        public virtual string GetActiveERPClassName()
           => _eRPConfiguratorClient.GetActiveERPClassName();

        //Method for class name of ERP defined by user. 
        public virtual string GetERPClassName()
           => _eRPConfiguratorClient.GetERPClassName();
        #endregion
    }
}