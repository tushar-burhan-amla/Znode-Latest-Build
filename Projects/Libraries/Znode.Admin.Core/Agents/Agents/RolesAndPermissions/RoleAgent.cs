using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
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
    public class RoleAgent : BaseAgent, IRoleAgent
    {
        #region Private Variables
        private readonly IRoleClient _roleClient;
        #endregion

        #region Constructor
        public RoleAgent(IRoleClient roleClient)
        {
            _roleClient = GetClient<IRoleClient>(roleClient);
        }
        #endregion

        #region public Methods
        //Create role.
        public virtual RoleViewModel SaveRole(RoleViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return RoleViewModelMap.ToViewModel(_roleClient.CreateRole(RoleViewModelMap.ToModel(model)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (RoleViewModel)GetViewModelWithErrorMessage(new RoleViewModel(), Admin_Resources.RoleNameAlreadyExist);
                    default:
                        return (RoleViewModel)GetViewModelWithErrorMessage(new RoleViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return (RoleViewModel)GetViewModelWithErrorMessage(new RoleViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get role list
        public virtual RoleListViewModel GetRoleList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sortCollection.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { filters, sortCollection });

            filters.Add(FilterKeys.GetStoreAdminRoles, FilterOperators.NotContains, "User");

            RoleListViewModel roleList = RoleViewModelMap.ToListViewModel(_roleClient.GetRoleList(filters, sortCollection, pageIndex, recordPerPage));
            ZnodeLogging.LogMessage("Roles count.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { roleList?.Roles?.Count });

            if (roleList?.Roles?.Count > 0)
            {
                SetToolMenus(roleList);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return roleList;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new RoleListViewModel();
        }

        //Delete role by role id.
        public virtual bool DeleteRole(string roleIds, out string message)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                message = Admin_Resources.ErrorDeleteSystemDefineRole;
                return _roleClient.DeleteRole(new ParameterModel { Ids = roleIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorDeleteSystemDefineRole;
                        return false;
                    default:
                        message = Admin_Resources.DeleteErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                message = Admin_Resources.DeleteErrorMessage;
                return false;
            }
        }

        //Get role by role id.
        public virtual RoleViewModel GetRole(string roleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            RoleViewModel model = _roleClient.GetRole(roleId).ToViewModel<RoleViewModel>();

            RoleMenuListViewModel list = GetRolesMenusPermissionsWithRoleMenus(roleId);

            if (!Equals(list.Menus, null))
            {
                model.RoleMenuAccessMapper = list.RoleMenuAccessMapper;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }

        public virtual bool UpdateRole(RoleViewModel model, out string message)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                message = string.Empty;

                //Updates role on the basis of role id.              
                RoleModel role = _roleClient.UpdateRole(model.Id, RoleViewModelMap.ToModel(model));
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return (!string.IsNullOrEmpty(role?.RoleId));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        public virtual RoleMenuListViewModel GetRolesMenusPermissionsWithRoleMenus(string roleId, FilterCollection filters = null)
        {
            ZnodeLogging.LogMessage("Input parameters roleId.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { roleId });

            //Adding role id to filters.
            if (Equals(filters, null))
                filters = new FilterCollection();

            filters.Add(new FilterTuple(FilterKeys.RoleId, FilterOperators.Like, roleId.ToString()));
            return RoleViewModelMap.ToRoleMenuListViewModel(_roleClient.GetRolesMenusPermissionsWithRoleMenus(filters));
        }

        public virtual List<SelectListItem> RoleList(FilterCollection filters = null, SortCollection sortCollection = null)
            => RoleViewModelMap.ToListItems(_roleClient.GetRoleList(filters, sortCollection, 1, int.MaxValue)?.Roles);

        public virtual IEnumerable<RolePermissionViewModel> GetRolePermission(string userName)
            => _roleClient.GetRolePermissionByUserName(userName)?.ToViewModel<RolePermissionViewModel>();

        public virtual List<SelectListItem> GetAdminRoles()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Gets the filtered list of roles which are not contains b2b roles and customer role.
            List<RoleModel> rolesList = _roleClient.GetRoleList(null, null, null, null)?.Roles?
                .Where(x => !Equals(x.TypeOfRole?.ToLower(), ZnodeRoleEnum.B2B.ToString().ToLower()) && !Equals(x.Name?.ToLower(), ZnodeRoleEnum.Customer.ToString().ToLower()) && x.IsActive == true).ToList();
            if (rolesList?.Count > 0)
                return rolesList.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.RoleId }).ToList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new List<SelectListItem>();
        }

        //Check whether the User has the mentioned role or not
        public virtual bool IsUserInRole(string userName, string roleName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Get User details from Session
            UserViewModel model = GetFromSession<UserViewModel>(AdminConstants.UserAccountSessionKey.ToString());
            if (!Equals(model, null) && !string.IsNullOrEmpty(model.RoleId))
            {
                //Get all admin roles list.
                List<SelectListItem> roleList = GetAdminRoles();
                ZnodeLogging.LogMessage("roleList count returned from GetAdminRoles method.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { roleList?.Count });

                SelectListItem role = roleList?.Find(x => x.Value == model.RoleId);
                if (!Equals(role, null))
                {
                    return Equals(role.Text, roleName);
                }
            }
            return false;
        }

        //Check whether the Role Name already exists.
        public virtual bool CheckRoleNameExist(string roleName, string roleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters roleName, roleId.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { roleName, roleId });

            if (!string.IsNullOrEmpty(roleName))
                roleName = roleName.Trim();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(AspNetRoleEnum.Name.ToString(), FilterOperators.Is, roleName));
            ZnodeLogging.LogMessage("filters :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { filters });

            //Get the Role List based on the role name filter.
            RoleListModel roleList = _roleClient.GetRoleList(filters, null);
            if (HelperUtility.IsNotNull(roleList) && HelperUtility.IsNotNull(roleList.Roles))
            {
                if (!string.IsNullOrEmpty(roleId))
                {
                    //Set the status in case the Role is open in edit mode.
                    RoleModel role = roleList.Roles.Find(x => x.RoleId == roleId);                    
                    if (HelperUtility.IsNotNull(role) && (!string.IsNullOrEmpty(role.Name)) && (!string.IsNullOrEmpty(roleName)))
                        return !role.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase);
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return roleList.Roles.FindIndex(x => x.Name == roleName) != -1;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }

        //Set the Tool Menus for Role List Grid View.
        private void SetToolMenus(RoleListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('RoleDeletePopup')", ControllerName = "RoleAndAccessRight", ActionName = "DeleteRole" });
            }
        }

        #endregion
    }
}