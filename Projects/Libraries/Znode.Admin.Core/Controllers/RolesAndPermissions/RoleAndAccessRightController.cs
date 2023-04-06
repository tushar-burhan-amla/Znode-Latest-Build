using MvcSiteMapProvider;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Controllers
{
    //Role and Access Right Controller
    public class RoleAndAccessRightController : BaseController
    {
        #region Private Variables 
        private readonly  IMenuAgent _menuAgent;
        private readonly  IRoleAgent _roleAgent;
        private readonly string _CreateEditMenuView = "~/Views/RolesAndPermissions/CreateEditMenu.cshtml";
        private readonly string _CreateEditRoleView = "~/Views/RolesAndPermissions/CreateEditRole.cshtml";
        private readonly string _ListPermissionsView = "~/Views/RolesAndPermissions/_Permission.cshtml";
        private readonly string _MenuListView = "~/Views/RolesAndPermissions/MenuList.cshtml";
        private readonly string _RoleListView = "~/Views/RolesAndPermissions/RoleList.cshtml";
        private readonly string _ManageMenuView = "~/Views/RolesAndPermissions/ManageMenu.cshtml";
        private readonly string IsParentMenuProperty = "IsParentMenu";
        #endregion

        #region Public constructor
        public RoleAndAccessRightController(IMenuAgent menuAgent, IRoleAgent roleAgent)
        {                                              
            _menuAgent = menuAgent;
            _roleAgent = roleAgent;
        }
        #endregion

        #region Menu
        //Method to get menu list. 
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Menu", Area = "", ParentKey = "Admin")]
        public virtual ActionResult MenuList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeMenu.ToString(), model);
            //Get the list of menu            

            MenuListViewModel menuListViewModel = _menuAgent.GetMenuList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            menuListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, menuListViewModel.Menus, GridListType.ZnodeMenu.ToString(), string.Empty, null, true, true, menuListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            menuListViewModel.GridModel.TotalRecordCount = menuListViewModel.TotalResults;

            //Returns the menu list view
            return ActionView(_MenuListView, menuListViewModel);
        }

        //Method to create menu.
        [HttpGet]
        public virtual ActionResult CreateMenu()
            => View(_CreateEditMenuView, new MenuViewModel() { ParentMenuList = _menuAgent.GetMenuList().ParentMenus, IsActive = true });

        //Method to create menu.
        [HttpPost]
        public virtual ActionResult CreateMenu(MenuViewModel model)
        {
            string message = string.Empty;
            int menuId = 0;
            ModelState.Remove(IsParentMenuProperty);
            if (ModelState.IsValid)
            {
                bool status = _menuAgent.SaveMenu(model, out message, out menuId);
                SetNotificationMessage(status
                    ? GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage)
                    : GetErrorNotificationMessage(message));
                if (status)
                     return RedirectToAction<RoleAndAccessRightController>(x => x.EditMenu((int)menuId));
            }
            return RedirectToAction<RoleAndAccessRightController>(x => x.MenuList(null));
        }

        //Edit existing menu.
        [HttpGet]
        public virtual ActionResult EditMenu(int menuId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            MenuViewModel model = new MenuViewModel();
            if (!Equals(menuId, null) && menuId > 0)
            {
                model = _menuAgent.GetMenu(menuId);
                model.IsParentMenu = model.ParentMenuId == null ? true : false;
            }
            model.ParentMenuList = _menuAgent.GetMenuList().ParentMenus;
            return View(_CreateEditMenuView, model);
        }

        //Edit existing menu.
        [HttpPost]
        public virtual ActionResult EditMenu(MenuViewModel model)
        {
            string message = string.Empty;
            SetNotificationMessage(_menuAgent.UpdateMenu(model, out message)
                ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                : GetErrorNotificationMessage(message));
            return RedirectToAction<RoleAndAccessRightController>(x => x.EditMenu(model.MenuId));
        }

        //Method to delete menu.
        public virtual ActionResult DeleteMenu(string menuId)
        {
            if (!string.IsNullOrEmpty(menuId))
            {
                string message = string.Empty;
                bool status = _menuAgent.DeleteMenu(menuId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.DeleteErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get the details required for assigning permissions to menu.
        [HttpGet]
        public virtual ActionResult ManageMenu(int menuId, string menuName)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

          return ActionView(_ManageMenuView, _menuAgent.GetMenuActionsPermissionDetails(menuId, menuName));
        }
        //Update the permission values against action.
        [HttpPost]
        public virtual ActionResult ManageMenu(MenuActionsPermissionViewModel menuActionsPermissionViewModel)
        {
            bool status = _menuAgent.UpdateMenuActionPermissions(menuActionsPermissionViewModel);
            if (status)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<RoleAndAccessRightController>(x => x.ManageMenu(menuActionsPermissionViewModel.MenuId, menuActionsPermissionViewModel.MenuName));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return RedirectToAction<RoleAndAccessRightController>(x => x.ManageMenu(menuActionsPermissionViewModel.MenuId, menuActionsPermissionViewModel.MenuName));
            }

        }

        //Method to get sub menus on the basis of parent menu id apart from already selected menus.
        public virtual JsonResult GetChildMenus(string id, string alreadySelectedMenus)
            => Json(RenderRazorViewToString(_ListPermissionsView, MenuViewModelMap.ToListViewModel(_menuAgent.GetSubMenuWithPermission(id, string.IsNullOrEmpty(alreadySelectedMenus) ? "0" : alreadySelectedMenus))), JsonRequestBehavior.AllowGet);
        #endregion

        #region Role
        //Method to get role list
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Role", Area = "", ParentKey = "Admin")]
        public virtual ActionResult RoleList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.RoleList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.RoleList.ToString(), model);
            //get the list of roles            
            RoleListViewModel roleViewModel = _roleAgent.GetRoleList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //get the grid model
            roleViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, roleViewModel.Roles, GridListType.RoleList.ToString(), string.Empty, null, true, true, roleViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //set the total record count
            roleViewModel.GridModel.TotalRecordCount = roleViewModel.TotalResults;

            //returns the view
            return ActionView(_RoleListView, roleViewModel);
        }

        //Method to create role.
        [HttpGet]
        public virtual ActionResult Role()
        {
            MenuListViewModel permissionList = _menuAgent.GetMenuWithPermission();
            return View(_CreateEditRoleView, new RoleViewModel() { IsActive = true, Menus = permissionList.Menus, Permissions = permissionList.Permissions, RoleMenuAccessMapper = new List<RoleMenuAccessMapperViewModel>() });
        }
        //Method to create role.
        [HttpPost]
        public virtual ActionResult Role(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = _roleAgent.SaveRole(model);
                if (!model?.HasError ?? false)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<RoleAndAccessRightController>(x => x.EditRole(model.Id));
                }
            }
            MenuListViewModel permissionList = _menuAgent.GetMenuWithPermission();
            SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            model.Menus = permissionList.Menus;
            model.Permissions = permissionList.Permissions;
            model.RoleMenuAccessMapper = new List<RoleMenuAccessMapperViewModel>();
            return ActionView(_CreateEditRoleView, model);
        }

        //Edit existing role.
        [HttpGet]
        public virtual ActionResult EditRole(string Id)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            RoleViewModel model = new RoleViewModel();
            if (!string.IsNullOrEmpty(Id))
            {
                model = _roleAgent.GetRole(Id);
                MenuListViewModel permissionList = _menuAgent.GetMenuWithPermission();
                model.Menus = permissionList.Menus;
                model.Permissions = permissionList.Permissions;
            }
            return View(_CreateEditRoleView, model);
        }

        //Edit existing role.
        [HttpPost]
        public virtual ActionResult EditRole(RoleViewModel model)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (_roleAgent.UpdateRole(model, out message))
                {
                    SessionHelper.RemoveDataFromSession(AdminConstants.UserMenuListSessionKey);
                    message = GetSuccessNotificationMessage(Admin_Resources.UpdateMessage);
                }
                else
                {
                    message = GetErrorNotificationMessage(string.IsNullOrEmpty(message) ? Admin_Resources.UpdateErrorMessage : message);
                }
                SetNotificationMessage(message);
                return RedirectToAction<RoleAndAccessRightController>(x => x.EditRole(model.Id));
            }
            var roleModel = _roleAgent.GetRole(model.Id);
            MenuListViewModel permissionList = _menuAgent.GetMenuWithPermission();
            model.Menus = permissionList.Menus;
            model.Permissions = permissionList.Permissions;
            model.RoleMenuAccessMapper = roleModel.RoleMenuAccessMapper;
            return View(_CreateEditRoleView, model);
        }

        //Method to delete role.
        public virtual ActionResult DeleteRole(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string message = string.Empty;
                bool status = _roleAgent.DeleteRole(id, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.DeleteErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult IsRoleNameExist(string Name, string id)
            => Json(!_roleAgent.CheckRoleNameExist(Name, id), JsonRequestBehavior.AllowGet);

        #endregion
    }
}