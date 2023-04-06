using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class AccessPermissionAgent : BaseAgent, IAccessPermissionAgent
    {
        #region Private Variables
        private readonly IAccessPermissionClient _accessPermissionClient;
        #endregion

        #region Constructor
        public AccessPermissionAgent(IAccessPermissionClient accessPermissionClient)
        {
            _accessPermissionClient = GetClient<IAccessPermissionClient>(accessPermissionClient);
        }
        #endregion

        #region Public Methods
        public virtual AccountPermissionListViewModel AccountPermissionList(int accountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            HelperMethods.SetAccountIdFilters(filters, accountId);
            ZnodeLogging.LogMessage("Input parameters filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { filters, sortCollection });
            AccessPermissionListModel accountPermissions = _accessPermissionClient.AccountPermissionList(null, filters, sortCollection, pageIndex, recordPerPage);
            if (accountPermissions?.AccountPermissions?.Count > 0)
            {
                AccountPermissionListViewModel accountPermissionListViewModel = new AccountPermissionListViewModel { AccountPermissionList = accountPermissions.AccountPermissions.ToViewModel<AccountPermissionViewModel>().ToList(), AccountId = accountId };
                SetListPagingData(accountPermissionListViewModel, accountPermissions);
                return accountPermissionListViewModel;
            }
            return new AccountPermissionListViewModel { AccountPermissionList = new List<AccountPermissionViewModel>(), AccountId = accountId };
        }

        public virtual List<SelectListItem> AccessPermissionList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set Filters.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccessPermissionEnum.TypeOfPermission.ToString(), FilterOperators.Contains, "B2B"));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { filters});
            AccessPermissionListModel accountPermissions = _accessPermissionClient.AccessPermissionList(null, filters, null, null, null);

            List<SelectListItem> accessPermissions = new List<SelectListItem>();

            if (accountPermissions?.AccountPermissions?.Count > 0)
                foreach (AccessPermissionModel access in accountPermissions.AccountPermissions)
                    accessPermissions.Add(new SelectListItem() { Text = access.PermissionsName, Value = access.AccessPermissionId.ToString() });


            ZnodeLogging.LogMessage("accessPermissions list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accessPermissions?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return accessPermissions;
        }

        public virtual AccountPermissionViewModel GetAccountPermission(int accountPermissionId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountPermissionId.ToString(), FilterOperators.Equals, accountPermissionId.ToString()));
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.ZnodeAccountPermissionAccesses);
            ZnodeLogging.LogMessage("filters and expands:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { filters, expands });
            AccessPermissionModel accessPermission = _accessPermissionClient.GetAccountPermission(expands, filters);
            if (HelperUtility.IsNotNull(accessPermission))
            {
                AccountPermissionViewModel accountPermissionViewModel = accessPermission.ToViewModel<AccountPermissionViewModel>();
                if (accessPermission.AccountPermissionAccessList?.Count > 0)
                    accountPermissionViewModel.AccessPermissionId = accessPermission.AccountPermissionAccessList.FirstOrDefault().AccessPermissionId;
                return accountPermissionViewModel;
            }
            return new AccountPermissionViewModel { HasError = true };
        }

        public virtual bool CreateAccountPermission(AccountPermissionViewModel model)
            => _accessPermissionClient.CreateAccountPermission(model.ToModel<AccessPermissionModel>())?.AccessPermissionId > 0;

        public virtual AccountPermissionViewModel UpdateAccountPermission(string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            AccountPermissionViewModel accountPermissionViewModel = JsonConvert.DeserializeObject<AccountPermissionViewModel[]>(data)[0];
            try
            {
                return _accessPermissionClient.UpdateAccountPermission(accountPermissionViewModel.ToModel<AccessPermissionModel>())?.ToViewModel<AccountPermissionViewModel>();
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (AccountPermissionViewModel)GetViewModelWithErrorMessage(accountPermissionViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        public virtual bool DeleteAccountPermission(string accountPermissionIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accessPermissionClient.DeleteAccountPermission(new ParameterModel() { Ids = accountPermissionIds });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Check whether Permission already exists.
        public virtual bool CheckAccountPermissionExist(string accountPermissionName, int accountId, int accountPermissionId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            accountPermissionName = accountPermissionName.Trim();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountPermissionName.ToString(), FilterOperators.Is, accountPermissionName));
            filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { filters });
            //Get the Banner List based on the Banner name filter.
            AccessPermissionListModel accountPermissions = _accessPermissionClient.AccountPermissionList(null, filters, null, null, null);
            if (HelperUtility.IsNotNull(accountPermissions) && HelperUtility.IsNotNull(accountPermissions.AccountPermissions))
            {
                if (accountPermissionId > 0)
                {
                    //Set the status in case the account permission is open in edit mode.
                    AccessPermissionModel permission = accountPermissions.AccountPermissions.Find(x => x.AccountPermissionId == accountPermissionId);
                    if (HelperUtility.IsNotNull(permission))
                        return !Equals(permission.AccountPermissionName, accountPermissionName);
                }
                return accountPermissions.AccountPermissions.Any(x => string.Equals(x.AccountPermissionName, accountPermissionName, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
        #endregion
    }
}