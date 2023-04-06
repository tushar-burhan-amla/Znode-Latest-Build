using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class SalesRepAgent:BaseAgent,ISalesRepAgent
    {
        #region Private Variable
        private readonly IUserClient _userClient;
        #endregion

        #region Public Constructor
        public SalesRepAgent(IUserClient userClient)
        {
            _userClient = GetClient<IUserClient>(userClient);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the Sales Rep Users List
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordPerPage"></param>
        /// <returns></returns>
        public virtual UsersListViewModel GetSalesRepList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;

            SetFilterProperties(filters);
            UserListModel userListModel = _userClient.GetUserAccountList(loggedUserAccountId, filters, sortCollection, pageIndex, recordPerPage);
            UsersListViewModel listViewModel = new UsersListViewModel { List = userListModel?.Users?.ToViewModel<UsersViewModel>().ToList() };

            SetListPagingData(listViewModel, userListModel);

            //Set the Tool Menus for User Account List Grid View.
            SetUserAccountToolMenuList(listViewModel);            
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.RoleName, StringComparison.InvariantCultureIgnoreCase));
            return listViewModel?.List?.Count > 0 ? listViewModel : new UsersListViewModel() { List = new List<UsersViewModel>() };
        }
        #endregion

        #region Private Methods
        //Set the Tool Menus for Sales Rep User List Grid View.
        private void SetUserAccountToolMenuList(UsersListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AccountDeletePopup')", ControllerName = "User", ActionName = "DeleteUser" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Unlock, JSFunctionName = "EditableText.prototype.DialogDelete('accountEnable')", ControllerName = "SalesRep", ActionName = "EnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Lock, JSFunctionName = "EditableText.prototype.DialogDelete('accountdisable')", ControllerName = "SalesRep", ActionName = "EnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ResetPasswordButtonText, JSFunctionName = "EditableText.prototype.DialogDelete('accountresetpassword')", ControllerName = "SalesRep", ActionName = "BulkResetPassword" });
            }
        }
        
        //Set Filter properties 
        private void SetFilterProperties(FilterCollection filters)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.IsStoreAdmin, StringComparison.InvariantCultureIgnoreCase));
            filters.Add(AdminConstants.IsStoreAdmin.ToString(), FilterOperators.Equals, AdminConstants.True);
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.RoleName, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.IsSalesRepList, StringComparison.InvariantCultureIgnoreCase));
            filters.Add(FilterKeys.RoleName, FilterOperators.Contains, AdminConstants.SalesRepRole);
            filters.Add(AdminConstants.IsSalesRepList, FilterOperators.Equals, AdminConstants.True);


        }

        #endregion
    }
}
