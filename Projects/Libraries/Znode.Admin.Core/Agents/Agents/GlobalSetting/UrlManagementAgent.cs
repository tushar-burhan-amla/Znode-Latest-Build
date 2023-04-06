using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class UrlManagementAgent : BaseAgent, IUrlManagementAgent
    {
        #region Private Variables
        private readonly IDomainClient _domainClient;
        private readonly IPortalClient _portalClient;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for UrlManagement agent.
        /// </summary>
        public UrlManagementAgent(IDomainClient domainClient, IPortalClient portalClient)
        {
            _domainClient = GetClient<IDomainClient>(domainClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
        }
        #endregion

        //This method will return admin/api domain lists
        public DomainListViewModel GetDomainList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters :", string.Empty, TraceLevel.Verbose, new { filters= filters, sortCollection = sortCollection });

			//checks if the filter collection null
			filters = IsNotNull(filters) ? filters : new FilterCollection();
            FilterTuple webstoreFilter = new FilterTuple(FilterKeys.ApplicationType, FilterOperators.NotContains, FilterKeys.WebStore);
            //First Removing the filter if exists then adding it back
            filters.Remove(webstoreFilter);
            filters.Add(webstoreFilter);

            DomainListModel domainList = _domainClient.GetDomains(filters, sortCollection, pageIndex, recordPerPage);
            if (domainList?.Domains?.Count > 0)
            {
                foreach (DomainModel item in domainList.Domains)
                {
                    item.Status = item.IsActive;
                    item.IsActive = !item.IsActive;
                }
            }

            DomainListViewModel listViewModel = new DomainListViewModel { Domains = domainList?.Domains?.ToViewModel<DomainViewModel>().ToList() };
            SetListPagingData(listViewModel, domainList);

            //Set tool menu for URL list grid view.
            SetUrlListToolMenu(listViewModel);
			ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
			return domainList?.Domains?.Count > 0 ? listViewModel : new DomainListViewModel() { Domains = new List<DomainViewModel>() };
        }

        //Method to get domain by domain id.
        public virtual DomainViewModel GetDomain(int domainId)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
			DomainModel domainModel = _domainClient.GetDomain(domainId);
			ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
			return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : new DomainViewModel();
        }

        //Returns Admin/API Types
        public virtual List<SelectListItem> GetAdminAPIApplicationTypes()
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);

			List<SelectListItem> applicationType = new List<SelectListItem>();
            applicationType.Add(new SelectListItem() { Text = ApplicationTypesEnum.Admin.ToString(), Value = ApplicationTypesEnum.Admin.ToString() });
            applicationType.Add(new SelectListItem() { Text = ApplicationTypesEnum.API.ToString(), Value = ApplicationTypesEnum.API.ToString() });
			ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);

			return applicationType;
        }

        //Method to create url.
        public virtual DomainViewModel CreateDomainUrl(DomainViewModel domainViewModel)
        {
            try
            {
				ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);

				DomainModel domainModel = domainViewModel.ToModel<DomainModel>();
                domainModel.ApiKey = Guid.NewGuid().ToString();
                domainModel = _domainClient.CreateDomain(domainModel);
				ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
				return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : new DomainViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorDomainNameAlreadyExists);
                    default:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Enable disable domain.
        public virtual bool EnableDisableDomain(string domainId, bool isActive, out string errorMessage)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
			errorMessage = Admin_Resources.ErrorMessageEnableDisable;
            try
            {
                return _domainClient.EnableDisableDomain(new DomainModel { DomainIds = domainId, IsActive = isActive });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                return false;
            }
        }

        //Check whether the domain name is already exists or not.
        public virtual bool CheckDomainNameExist(string domainName, int domainId)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameters :", string.Empty, TraceLevel.Verbose, new { domainName = domainName, domainId = domainId });

			// Remove any trailing "/"
			if (domainName.EndsWith("/"))
                domainName = domainName.Remove(domainName.Length - 1);
            if (domainName.Contains("www."))
                domainName = domainName.Replace("www.", string.Empty);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeDomainEnum.DomainName.ToString(), FilterOperators.Contains, domainName));

            //Get the Domain List based on the domain name filter.
            DomainListModel domainList = _domainClient.GetDomains(filters, null, null, null);
            DomainListViewModel listViewModel = new DomainListViewModel { Domains = domainList?.Domains?.ToViewModel<DomainViewModel>().ToList() };
			if (IsNotNull(listViewModel) && IsNotNull(listViewModel.Domains))
			{
				if (domainId > 0)
				{
					//Set the status in case the Domain is open in edit mode.
					DomainViewModel domain = listViewModel.Domains.Find(x => x.DomainId == domainId);
					if (IsNotNull(domain))
						return !Equals(domain.DomainName.ToLower(), domainName.ToLower());
				}
				return listViewModel.Domains.FindIndex(x => x.DomainName == domainName) != -1;
			}
			return false;
        }

        //Method to update domain.
        public virtual DomainViewModel UpdateDomainUrl(DomainViewModel domainViewModel)
        {
            try
            {
				ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
				domainViewModel.IsActive = domainViewModel.Status;
                DomainModel domainModel = _domainClient.UpdateDomain(domainViewModel.ToModel<DomainModel>());
				ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
				return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : (DomainViewModel)GetViewModelWithErrorMessage(new DomainViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorDomainNameAlreadyExists);
                    default:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        
        //Delete admin/api domain url.
        public virtual bool DeleteDomainUrl(string domainId, out string message)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
			message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _domainClient.DeleteDomain(new ParameterModel() { Ids = domainId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        #region Private Methods

        //Set tool menu for URL list grid view.
        private void SetUrlListToolMenu(DomainListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('URLDeletePopup')", ControllerName = "UrlManagement", ActionName = "DeleteUrl" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('domainEnable')", ControllerName = "UrlManagement", ActionName = "EnableDisableDomain" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "EditableText.prototype.DialogDelete('domainDisable')", ControllerName = "UrlManagement", ActionName = "EnableDisableDomain" });
            }
        }

        #endregion Private Methods
    }
}
