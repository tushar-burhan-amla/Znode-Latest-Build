using System;
using System.Collections.Generic;
using System.Diagnostics;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class CustomerReviewAgent : BaseAgent, ICustomerReviewAgent
    {
        #region Private Variables
        private readonly ICustomerReviewClient _customerReviewClient;
        #endregion

        #region Constructor
        public CustomerReviewAgent(ICustomerReviewClient customerReviewClient)
        {
            _customerReviewClient = GetClient<ICustomerReviewClient>(customerReviewClient);
        }
        #endregion

        #region public Methods

        //Get Customer Review list.
        public virtual CustomerReviewListViewModel GetCustomerReviewList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);            
            sorts = HelperMethods.SortDesc(ZnodeCMSCustomerReviewEnum.CMSCustomerReviewId.ToString(), sorts);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            CustomerReviewListViewModel listViewModel = CustomerReviewViewModelMap.ToListViewModel(_customerReviewClient.GetCustomerReviewList(DefaultSettingHelper.DefaultLocale, null, filters, sorts, pageIndex, pageSize));

            //Set tool menu for customer review list on grid view.
            SetCustomerReviewListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listViewModel?.CustomerReviewList?.Count > 0 ? listViewModel : new CustomerReviewListViewModel() { CustomerReviewList = new List<CustomerReviewViewModel>() };
        }

        //Get customer review details on the basis of customer review id.
        public virtual CustomerReviewViewModel GetCustomerReview(int customerReviewId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            CustomerReviewViewModel customerReviewModel = _customerReviewClient.GetCustomerReview(customerReviewId, DefaultSettingHelper.DefaultLocale)?.ToViewModel<CustomerReviewViewModel>();
            if (!Equals(customerReviewModel, null))
            {
                //Binds the review ratings value and review status value.
                customerReviewModel.GetReviewRatings = CustomerReviewViewModelMap.GetReviewRatings(customerReviewModel.Rating.Value);
                customerReviewModel.GetReviewStatus = CustomerReviewViewModelMap.GetReviewStatus(customerReviewModel.Status);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return !Equals(customerReviewModel, null) ? customerReviewModel : new CustomerReviewViewModel();
        }

        //Update the customer review.
        public virtual CustomerReviewViewModel UpdateCustomerReview(CustomerReviewViewModel customerReviewViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Update customer review and map the model to view model.
                customerReviewViewModel = _customerReviewClient.UpdateCustomerReview(customerReviewViewModel?.ToModel<CustomerReviewModel>())?.ToViewModel<CustomerReviewViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return customerReviewViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return (CustomerReviewViewModel)GetViewModelWithErrorMessage(new CustomerReviewViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CustomerReviewViewModel)GetViewModelWithErrorMessage(new CustomerReviewViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete customer review.
        public virtual bool DeleteCustomerReview(string customerReviewId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                //Delete customer review.
                return _customerReviewClient.DeleteCustomerReview(new ParameterModel { Ids = customerReviewId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;

            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Change status of customer reviews.
        public virtual bool BulkStatusChange(string cmsCustomerReviewId, string statusId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                //Change status of customer reviews.
                return _customerReviewClient.BulkStatusChange(new ParameterModel { Ids = cmsCustomerReviewId }, statusId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                message = Admin_Resources.ErrorMessageFailedStatus;
                return false;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorMessageFailedStatus;
                return false;
            }
        }
        #endregion

        #region Private method.
        //Set tool option menus for customer review grid.
        private void SetCustomerReviewListToolMenu(CustomerReviewListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('CustomerReviewDeletePopup')", ControllerName = "Review", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextStatusNew, JSFunctionName = "EditableText.prototype.DialogDelete('statusNew')", ControllerName = "Review", ActionName = "BulkStatusChange", ControlId = AdminConstants.New });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextStatusActive, JSFunctionName = "EditableText.prototype.DialogDelete('statusActive')", ControllerName = "Review", ActionName = "BulkStatusChange", ControlId = AdminConstants.Active });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextStatusInactive, JSFunctionName = "EditableText.prototype.DialogDelete('statusInactive')", ControllerName = "Review", ActionName = "BulkStatusChange", ControlId = AdminConstants.Inactive });
            }
        }
        #endregion
    }
}
