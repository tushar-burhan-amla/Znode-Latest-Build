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
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class RMAConfigurationAgent : BaseAgent, IRMAConfigurationAgent
    {
        #region Private Variables
        private readonly IRMAConfigurationClient _rmaConfigurationClient;
        #endregion

        #region Constructor

        public RMAConfigurationAgent(IRMAConfigurationClient rmaConfigurationClient)
        {
            _rmaConfigurationClient = GetClient<IRMAConfigurationClient>(rmaConfigurationClient);
        }

        #endregion

        #region RMA Configuration
        //Create or Update RMA Configuration.
        public virtual RMAConfigurationViewModel CreateRMAConfiguration(RMAConfigurationViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                model.ActionMode = model.RmaConfigurationId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                return _rmaConfigurationClient.CreateRMAConfiguration(model?.ToModel<RMAConfigurationModel>())?.ToViewModel<RMAConfigurationViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (RMAConfigurationViewModel)GetViewModelWithErrorMessage(new RMAConfigurationViewModel(), model.RmaConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);

            }
        }

        //Get RMA Configuration.
        public virtual RMAConfigurationViewModel GetRMAConfiguration()
           => _rmaConfigurationClient.GetRMAConfiguration().ToViewModel<RMAConfigurationViewModel>();
        #endregion

        #region Reason For Return/Request Status
        //Get the list of Reason For Return or Request Status on the basis of isRequestStatus flag.
        public virtual RequestStatusListViewModel GetReasonForReturnOrRequestStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isRequestStatus)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            SortCollection sortlist = new SortCollection();
            sortlist.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            ZnodeLogging.LogMessage("Input parameters expands,filters and sorts:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sorts });
            RequestStatusListModel list = isRequestStatus ? _rmaConfigurationClient.GetRequestStatusList(expands, filters, sorts, pageIndex, pageSize) :
                _rmaConfigurationClient.GetReasonForReturnList(expands, filters, sorts, pageIndex, pageSize);

            RequestStatusListViewModel listViewModel = new RequestStatusListViewModel { RequestStatusList = list?.RequestStatusList?.ToViewModel<RequestStatusViewModel>().ToList() };
            listViewModel.RequestStatusList?.ForEach(item => { item.IsActiveList = GetBooleanList(); });

            SetListPagingData(listViewModel, list);

            //Set the Tool Menus for  Reason For Return or Request Status List Grid View.
            SetToolMenus(listViewModel, isRequestStatus);
            return list?.RequestStatusList?.Count > 0 ? listViewModel : new RequestStatusListViewModel() { RequestStatusList = new List<RequestStatusViewModel>() };
        }

        //Create Request Status.
        public virtual RequestStatusViewModel CreateReasonForReturn(RequestStatusViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                RequestStatusModel model = _rmaConfigurationClient.CreateReasonForReturn(viewModel.ToModel<RequestStatusModel>());
                return IsNotNull(model) ? model.ToViewModel<RequestStatusViewModel>() : new RequestStatusViewModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (RequestStatusViewModel)GetViewModelWithErrorMessage(new RequestStatusViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get Reason For Return on the basis of reasonForReturnId or get Request Status on the basis of requestStatusId .
        public virtual RequestStatusViewModel GetReasonForReturnOrRequestStatus(int rmaReasonForReturnId, int rmaRequestStatusId, bool isRequestStatus)
            => isRequestStatus ? _rmaConfigurationClient.GetRequestStatus(rmaRequestStatusId).ToViewModel<RequestStatusViewModel>()
                : _rmaConfigurationClient.GetReasonForReturn(rmaReasonForReturnId).ToViewModel<RequestStatusViewModel>();


        //Update Reason For Return data or RequestStatus on the basis of isRequestStatus flag.
        public virtual RequestStatusViewModel UpdateReasonForReturnOrRequestStatus(RequestStatusViewModel viewModel, bool isRequestStatus)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                RequestStatusModel model = new RequestStatusModel();
                if (isRequestStatus)
                    model = _rmaConfigurationClient.UpdateRequestStatus(viewModel.ToModel<RequestStatusModel>());
                else
                {
                    viewModel.Reason = viewModel.Name;
                    ZnodeLogging.LogMessage("Reason:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, viewModel.Name);
                    model = _rmaConfigurationClient.UpdateReasonForReturn(viewModel.ToModel<RequestStatusModel>());
                }
                return IsNotNull(model) ? model.ToViewModel<RequestStatusViewModel>() : (RequestStatusViewModel)GetViewModelWithErrorMessage(new RequestStatusViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (RequestStatusViewModel)GetViewModelWithErrorMessage(new RequestStatusViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete Reason For Return or RequestStatus on the basis of isRequestStatus flag.
        public virtual bool DeleteReasonForReturnOrRequestStatus(string id, bool isRequestStatus, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToCreate;
            try
            {
                return isRequestStatus ? _rmaConfigurationClient.DeleteRequestStatus(new ParameterModel { Ids = id })
                    : _rmaConfigurationClient.DeleteReasonForReturn(new ParameterModel { Ids = id });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotDeleteActiveRecord:
                        errorMessage = Admin_Resources.RequestStatusDeleteError;
                        return false;
                    case ErrorCodes.DefaultDataDeletionError:
                        errorMessage = Admin_Resources.ReasonForReturnDeleteError;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }
        #endregion

        #region Private Methods.
        //Set the Tool Menus for Reason For Return or Request Status List Grid View.
        private void SetToolMenus(RequestStatusListViewModel model, bool isRequestStatus)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();

                if (isRequestStatus)
                    model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('RequestStatusDeletePopup')", ControllerName = "RMAConfiguration", ActionName = "DeleteRequestStatus" });
                else
                    model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ReasonForReturnDeletePopup')", ControllerName = "RMAConfiguration", ActionName = "DeleteReasonForReturn" });

            }
        }

        #endregion
    }
}