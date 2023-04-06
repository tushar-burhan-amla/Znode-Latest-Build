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
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class MenuAgent : BaseAgent, IMenuAgent
    {
        #region Private Variables
        private readonly IMenuClient _menuClient;
        private readonly IRoleClient _roleClient;
        private readonly IAccessPermissionClient _accessPermissionClient;
        private readonly string RoleAndAccessRightControllerName = "RoleAndAccessRight";
        private readonly string GetChildMenusActionName = "GetChildMenus";
        private readonly string DropDownSuccessCallBack = "DynamicGrid.GetChildMenus";
        private readonly string ParentMenuHeader = "--Parent Menu--";
        #endregion

        #region Constructor
        public MenuAgent(IMenuClient menuClient, IRoleClient roleClient, IAccessPermissionClient accessPermissionClient)
        {
            _menuClient = GetClient<IMenuClient>(menuClient);
            _roleClient = GetClient<IRoleClient>(roleClient);
            _accessPermissionClient = GetClient<IAccessPermissionClient>(accessPermissionClient);
        }
        #endregion

        #region Public Methods
        public virtual bool SaveMenu(MenuViewModel model, out string message, out int menuId)
        {
            menuId = 0;
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                message = string.Empty;
                menuId = 0;
                if (HelperUtility.IsNotNull(model))
                {
                    //Create menu.
                    MenuModel menuModel = _menuClient.CreateMenu(model?.ToModel<MenuModel>());
                    menuId = (int)menuModel?.MenuId;
                    ZnodeLogging.LogMessage("Input parameters menuId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { menuId });
                    return menuModel?.MenuId > 0 ? true : false;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                message = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                message = Admin_Resources.SaveErrorMessage;
                return false;
            }
        }

        //Get menu list.
        public virtual MenuListViewModel GetMenuList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filters, sortCollection });

            MenuListModel menuListModel = _menuClient.GetMenuList(filters, sortCollection, pageIndex, recordPerPage);
            ZnodeLogging.LogMessage("Menus count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { menuListModel?.Menus?.Count });

            if (menuListModel?.Menus?.Count > 0)
            {
                MenuListViewModel menuList = MenuViewModelMap.ToListViewModel(menuListModel);
                SetToolMenus(menuList);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return menuList;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new MenuListViewModel();
        }

        public virtual MenuListViewModel GetMenuWithPermission()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMenuEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
            MenuListModel menus = _menuClient.GetMenuList(filters, null, null, null);
            ZnodeLogging.LogMessage("filters :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { filters });

            menus?.Menus?.ForEach(item =>
            {
                item.ParentMenuName = menus?.Menus?.Find(x => x.MenuId == item.ParentMenuId)?.MenuName ?? string.Empty;
            });

            //Returns list of permissions with menus.
            PermissionListModel permission = _roleClient.GetPermissionList(null, null);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return new MenuListViewModel()
            {
                Menus = menus?.Menus?.ToViewModel<MenuViewModel>().OrderBy(x => x.ParentMenuName).ToList(),
                Permissions = permission?.Permissions.ToViewModel<PermissionsViewModel>().ToList(),
                ParentMenus = menus?.ParentMenus?.ToViewModel<MenuViewModel>().ToList()
            };
        }

        //Delete menu by menu id.
        public virtual bool DeleteMenu(string menuIds, out string message)
        {
            message = Admin_Resources.ErrorMessageMenuDeleteAssociation;

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return _menuClient.DeleteMenu(new ParameterModel { Ids = menuIds.ToString() });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                message = ErrorCodes.AssociationDeleteError.Equals(ex.ErrorCode) ? Admin_Resources.ErrorMessageMenuDeleteAssociation : Admin_Resources.DeleteErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Gets menu by menu id.
        public virtual MenuViewModel GetMenu(int menuId)
          => _menuClient.GetMenu(menuId)?.ToViewModel<MenuViewModel>();

        public virtual bool UpdateMenu(MenuViewModel model, out string message)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                message = string.Empty;
                if (HelperUtility.IsNotNull(model))
                    //Updates menu on the basis of menu id.
                    return _menuClient.UpdateMenu(model.MenuId, model.ToModel<MenuModel>())?.MenuId > 0;
                return false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                message = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        public virtual MenuListModel GetSubMenuWithPermission(string id, string preSelectedMenuIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Gets menu list by parent menu id apart from already selected menus.
            MenuListModel menuList = _menuClient.GetMenuListByParentMenuId(new ParameterModel() { Ids = id }, preSelectedMenuIds);
            MenuListViewModel model = MenuViewModelMap.ToListViewModel(menuList);

            //Gets permission list.
            PermissionListModel permission = _roleClient.GetPermissionList(null, null);
            menuList.Permissions = permission;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return menuList;
        }

        public virtual MenuListViewModel GetUnSelectedMenusWithPermission(string menuIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            MenuListModel menus = _menuClient.GetUnSelectedMenus(new ParameterModel { Ids = menuIds });

            //Returns list of permissions with menus.
            PermissionListModel permission = _roleClient.GetPermissionList(null, null);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return new MenuListViewModel()
            {
                Menus = MenuViewModelMap.ToListViewModel(menus).Menus,
                Permissions = permission?.Permissions.ToViewModel<PermissionsViewModel>().ToList(),
                ParentMenus = MenuViewModelMap.ToListViewModel(menus).ParentMenus
            };
        }

        public virtual DropDownOptions CreateMultiSelectDDl(ICollection<MenuViewModel> parentMenu)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            List<BaseDropDownList> list = new List<BaseDropDownList>();
            if (!Equals(parentMenu, null))
            {
                foreach (var item in parentMenu)
                {
                    list.Add(new BaseDropDownList() { id = item.MenuId.ToString(), name = item.MenuName });
                }
            }
            DropDownOptions dropdownOptions = new DropDownOptions { IsAllowCheckAll = false, IsDraggable = false, SelectOptionName = ParentMenuHeader, Action = GetChildMenusActionName, assignable = true, Controller = RoleAndAccessRightControllerName, IsAjax = true, IsMultiple = true, SuccessCallBack = DropDownSuccessCallBack };
            dropdownOptions.DropDownList = list;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return dropdownOptions;
        }

        //Get list of all available and selected Permissions for actions of perticular controller.
        public virtual MenuActionsPermissionViewModel GetMenuActionsPermissionDetails(int menuId, string menuName)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                MenuActionsPermissionModel menuActionsPermissionModel = _menuClient.GetMenuActionsPermissionList(menuId, null);

                if (HelperUtility.IsNotNull(menuActionsPermissionModel))
                {
                    MenuActionsPermissionViewModel menuActionsPermissionViewModel = new MenuActionsPermissionViewModel();
                    menuActionsPermissionViewModel.ActionList = GetActionList(menuActionsPermissionModel.ActionList);

                    menuActionsPermissionViewModel.ActionPermissionList = menuActionsPermissionModel.ActionPermissionList?.ToViewModel<ActionPermissionMapperViewModel>().ToList();

                    ZnodeLogging.LogMessage("ActionList and ActionPermissionList count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { menuActionsPermissionViewModel.ActionList.Count, menuActionsPermissionViewModel.ActionPermissionList.Count });

                    //Get the list of all selected as well as available access permissions.
                    AccessPermissionListModel accountPermissions = new AccessPermissionListModel { AccountPermissions = _accessPermissionClient.AccessPermissionList(null, null, null, null, null)?.AccountPermissions?.Where(x => x.TypeOfPermission != "B2B").ToList() };
                    if (menuActionsPermissionModel.ActionPermissionList?.Count > 0)
                    {
                        for (int index = 0; index < menuActionsPermissionViewModel.ActionList.Count; index++)
                            if (menuActionsPermissionViewModel.ActionPermissionList.Exists(x => x.ActionId.ToString().Equals(menuActionsPermissionViewModel.ActionList[index].Value)))
                                menuActionsPermissionViewModel.ActionPermissionList[index].AccessPermissionList  = GetPermissionList(accountPermissions, menuActionsPermissionViewModel.ActionPermissionList[index].AccessPermissionId);
                            else
                            {
                                menuActionsPermissionViewModel.ActionPermissionList.Add(new ActionPermissionMapperViewModel { AccessPermissionList  = GetPermissionList(accountPermissions, 1) });
                                menuActionsPermissionViewModel.ActionPermissionList[index].AccessPermissionList  = GetPermissionList(accountPermissions, 1);
                            }
                    }
                    else
                    {
                        menuActionsPermissionViewModel.ActionPermissionList = new List<ActionPermissionMapperViewModel>();
                        for (int index = 0; index < menuActionsPermissionViewModel.ActionList.Count; index++)
                            menuActionsPermissionViewModel.ActionPermissionList.Add(new ActionPermissionMapperViewModel { AccessPermissionList  = GetPermissionList(accountPermissions, 1) });

                    }

                    menuActionsPermissionViewModel.MenuId = menuId;
                    menuActionsPermissionViewModel.MenuName = menuName;
                    return menuActionsPermissionViewModel;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return new MenuActionsPermissionViewModel() { ActionPermissionList = new List<ActionPermissionMapperViewModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return new MenuActionsPermissionViewModel();
            }

        }

        public virtual bool UpdateMenuActionPermissions(MenuActionsPermissionViewModel menuActionsPermissionViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                if (HelperUtility.IsNotNull(menuActionsPermissionViewModel) && !string.IsNullOrEmpty(menuActionsPermissionViewModel.ActionPermissions))
                {
                    //Get list of selected permission against every action of menu.
                    List<int> actions = menuActionsPermissionViewModel.ActionPermissions.Split(',')?.Select(x => Convert.ToInt32(x.Split('_')[0])).ToList();
                    List<int> permissions = menuActionsPermissionViewModel.ActionPermissions.Split(',')?.Select(x => Convert.ToInt32(x.Split('_')[1])).ToList();
                    menuActionsPermissionViewModel.ActionPermissionList = new List<ActionPermissionMapperViewModel>();

                    ZnodeLogging.LogMessage("ActionList count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { actions?.Count});

                    for (int length = 0; length < actions.Count; length++)
                        menuActionsPermissionViewModel.ActionPermissionList.Add(new ActionPermissionMapperViewModel
                        {
                            MenuId = menuActionsPermissionViewModel.MenuId,
                            ActionId = actions[length],
                            AccessPermissionId = permissions[length]
                        });

                    //Update permissions for every action of menu.
                    _menuClient.UpdateActionPermissions(menuActionsPermissionViewModel.ToModel<MenuActionsPermissionModel>());
                    return true;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return false;
            }
        }


        #endregion

        #region Private Methods
        //Get all the access permissions.
        private List<SelectListItem> GetPermissionList(AccessPermissionListModel accountPermissions, int accessPermissionId)
        {
            //Get the list of all access permissions in key value pair (SelectListItems).
            List<SelectListItem> accessPermissions = new List<SelectListItem>();

            ZnodeLogging.LogMessage("AccountPermissions count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { accountPermissions?.AccountPermissions?.Count });

            if (accountPermissions?.AccountPermissions?.Count > 0)
                foreach (AccessPermissionModel access in accountPermissions.AccountPermissions)
                    accessPermissions.Add(new SelectListItem() { Text = access.PermissionsName, Value = access.AccessPermissionId.ToString(), Selected = access.AccessPermissionId == accessPermissionId });

            return accessPermissions;
        }

        //Get all the access permissions.
        private List<SelectListItem> GetActionList(List<Api.Models.ActionModel> ActionList)
        {
            //Get the list of all access permissions in key value pair (SelectListItems).
            List<SelectListItem> accessPermissions = new List<SelectListItem>();
            if (ActionList?.Count > 0)
                foreach (Api.Models.ActionModel action in ActionList)
                    accessPermissions.Add(new SelectListItem() { Text = action.ActionName, Value = action.ActionId.ToString() });

            return accessPermissions;
        }

        //Set the Tool Menus for menu List Grid View.
        private void SetToolMenus(MenuListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MenuDeletePopup')", ControllerName = "RoleAndAccessRight", ActionName = "DeleteMenu" });
            }
        }

        #endregion
    }

}