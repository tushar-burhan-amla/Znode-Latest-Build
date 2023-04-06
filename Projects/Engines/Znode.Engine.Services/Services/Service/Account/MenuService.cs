using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IZnodeRepository<ZnodeMenu> _menuRepository;
        private readonly IZnodeRepository<ZnodeAccessPermission> _permissionRepository;
        private readonly IZnodeRepository<ZnodeRoleMenu> _roleMenuRepository;

        #region Constructor
        public MenuService()
        {
            _menuRepository = new ZnodeRepository<ZnodeMenu>();
            _permissionRepository = new ZnodeRepository<ZnodeAccessPermission>();
            _roleMenuRepository = new ZnodeRepository<ZnodeRoleMenu>();
        }
        #endregion

        //Get menu list with paging. 
        public virtual MenuListModel GetMenuList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (filters.Exists(x => x.FilterName.Equals(FilterKeys.Username)))
                //Get the Menu List filters based on the Login user Role.
                GetMenuListFiltersbasedOnRole(filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get menuList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //maps the entity list to model
            MenuListModel menuList = MenuMap.ToListModel(_menuRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount));

            //Bind Parent Menu Details.
            BindParentMenus(menuList);

            //Set for pagination
            menuList.BindPageListModel(pageListModel);
            //Set ERP menu name according to active ERP class name
            string erpClassName = GetService<IERPConfiguratorService>().GetERPClassName();
            menuList?.Menus?.Select(c => { if (Equals(c.MenuName, ZnodeConstant.ERPConfigurator)) c.MenuName = IsNotNull(erpClassName) ? erpClassName + " Configuration" : c.MenuName; return c; })?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //returns result
            return menuList;
        }

        //Create New Menu
        public virtual MenuModel CreateMenu(MenuModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.MenuModelNotNull);

            //Create the Menu.
            ZnodeMenu menu = _menuRepository.Insert(model.ToEntity<ZnodeMenu>());
            ZnodeLogging.LogMessage("MenuId value: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, menu?.MenuId);
            ZnodeLogging.LogMessage(menu?.MenuId > 0 ? string.Format(Admin_Resources.SuccessMenuInsert, model.MenuName) : string.Format(Admin_Resources.ErrorMenuInsert, model.MenuName), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            model.MenuId = menu?.MenuId;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }
        
        //Get All Permissions
        public virtual PermissionListModel GetPermissions() => PermissionsMap.ToListModel(_permissionRepository.GetEntityList(string.Empty));

        //Get parent menu by menu id.
        public virtual MenuListModel GetMenusByParentMenuId(string parentMenuId, string preSelectedMenuIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { parentMenuId = parentMenuId, preSelectedMenuIds = preSelectedMenuIds });
            //Creates filter list for where clause.
            FilterTuple filters = new FilterTuple(ZnodeMenuEnum.ParentMenuId.ToString(), ProcedureFilterOperators.In, parentMenuId);
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(filters);

            string where = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("Where clause generated list of menus: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, where);

            IList<ZnodeMenu> menus;

            if (!Equals(parentMenuId, string.Empty))

                //If parent menu id string contains id, it returns list based on where clause generated.(List of menus apart from already selected menus)
                menus = _menuRepository.GetEntityList(where).Where(x => !Array.ConvertAll(preSelectedMenuIds.Split(','), int.Parse).Contains(x.MenuId) && x.IsActive == true).ToList();
            else

                //If parent menu id string is empty, it returns entire list.(List of menus apart from already selected menus)
                menus = _menuRepository.GetEntityList(string.Empty).Where(x => !Array.ConvertAll(preSelectedMenuIds.Split(','), int.Parse).Contains(x.MenuId) && x.IsActive == true).ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return MenuMap.ToListModel(menus);
        }

        //Delete multiple menus.
        public virtual bool DeleteMenu(ParameterModel menuIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter to delete menu: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, menuIds?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("MenuIds", menuIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMenu @MenuIds, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, deleteResult?.Count());

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessMenuDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorMenuDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Get Menu by MenuId
        public virtual MenuModel GetMenu(int menuId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { menuId  = menuId });
            //Get Menu Details by MenuId.
            ZnodeMenu entity = _menuRepository.Table.FirstOrDefault(x => x.MenuId == menuId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(entity) ? entity.ToModel<MenuModel>() : null;
        }

        //Update menu.
        public virtual bool UpdateMenu(MenuModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.MenuModelNotNull);
           
            if (model.MenuId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.MenuIdNotLessThanOne);
            ZnodeLogging.LogMessage("MenuId property of input parameter MenuModel :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { MenuId = model?.MenuId } );

            //if the controller name change then remove its mapping from menu actions permission table.
            if (IsControllerNameChange(model))
                DeleteFromMenuActionsPermission(model.MenuId.GetValueOrDefault());

            bool status = (_menuRepository.Update(model.ToEntity<ZnodeMenu>()));
            if (status) 
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessMenuUpdate, model.MenuName), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            else
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorMenuUpdate, model.MenuName), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return status;
        }

        //Get Unselected menu.
        public virtual MenuListModel GetUnSelectedMenus(ParameterModel menuIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { menuIds = menuIds?.Ids });

            //List of all menus from which parent menus list will be generated.
            var allMenus = _menuRepository.Table.AsEnumerable().ToList();

            //Gets list of menus apart from the menus that have already been selected.
            var menus = allMenus.Where(x => !Array.ConvertAll(menuIds.Ids.Split(','), int.Parse).Contains(x.MenuId) && x.IsActive == true).ToList();

            //returns list of all parent menus.
            var parentMenuIdList = (from parent in allMenus
                                    where parent.ParentMenuId == null && parent.IsActive == true
                                    select new
                                    {
                                        ParentMenuId = parent.MenuId,
                                        MenuName = parent.MenuName
                                    }).ToList();
            ZnodeLogging.LogMessage("parentMenuIdList count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, parentMenuIdList?.Count());

            MenuListModel result = MenuMap.ToListModel(menus);

            //Adds list of parent menus to model.
            foreach (var parent in parentMenuIdList)
            {
                result.ParentMenus.Add(new MenuModel() { MenuName = parent.MenuName, MenuId = parent.ParentMenuId });
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return result;
        }

        //Get the Menu Action Names based on the Controller Name assigned to the MenuId
        public virtual MenuActionsPermissionModel GetMenuActionsPermissionList(int menuId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { menuId = menuId });

            MenuActionsPermissionModel menuActionsPermissionModel = new MenuActionsPermissionModel();
            IZnodeRepository<ZnodeAction> _actionRepository = new ZnodeRepository<ZnodeAction>();
            IZnodeRepository<ZnodeMenuActionsPermission> _menuActionsRepository = new ZnodeRepository<ZnodeMenuActionsPermission>();

            string controllerName = _menuRepository.Table.Where(x => x.MenuId == menuId)?.Select(x => x.ControllerName)?.FirstOrDefault();
            controllerName = controllerName.Split('/').Count() > 1 ? controllerName.Split('/')[1] : controllerName;
            ZnodeLogging.LogMessage("controllerName value :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, controllerName);

            //Get the list of all available actions against menu Id.
            List<ZnodeAction> menuActions = _actionRepository.Table.Where(x => x.ControllerName == controllerName)?.ToList();
            ZnodeLogging.LogMessage("menuActions list count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, menuActions?.Count());

            //Assign entity list of actions to MenuActionPermissionListModel.
            if (HelperUtility.IsNotNull(menuActions))
            {
                menuActionsPermissionModel.ActionList = new List<ActionModel>();
                foreach (ZnodeAction item in menuActions)
                    menuActionsPermissionModel.ActionList.Add(item?.ToModel<ActionModel>());
            }

            //Get the list of all selected permissions ofs actions against menu Id.
            menuActionsPermissionModel.ActionPermissionList = _menuActionsRepository.Table?.Where(x => x.MenuId == menuId)?.ToModel<ActionPermissionMapperModel>()?.ToList();
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return menuActionsPermissionModel;
        }

        //Update the permission values against action.
        public virtual bool UpdateMenuActionPermissions(MenuActionsPermissionModel menuActionsPermissionModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(menuActionsPermissionModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.MenuModelNotNull);

            //Set filters.
            List<int> actionIds = (menuActionsPermissionModel.ActionPermissionList?.Select(x => x.ActionId))?.ToList();
            ZnodeLogging.LogMessage("MenuId value of menuActionsPermissionModel and actionIds list count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { MenuId = menuActionsPermissionModel?.MenuId, actionIdsListCount = actionIds?.Count() });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMenuActionsPermissionEnum.ActionId.ToString(), FilterOperators.In, string.Join(",", actionIds)));
            filters.Add(new FilterTuple(ZnodeMenuActionsPermissionEnum.MenuId.ToString(), FilterOperators.Equals, menuActionsPermissionModel.MenuId.ToString()));

            //Generate where clause.
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause generated to get menuActionsPermissions list: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, whereClause);

            IZnodeRepository<ZnodeMenuActionsPermission> _menuActionsRepository = new ZnodeRepository<ZnodeMenuActionsPermission>();
            IList<ZnodeMenuActionsPermission> menuActionsPermissions = _menuActionsRepository.GetEntityList(whereClause);

            //Delete action's permission if any.
            if (menuActionsPermissions?.Count > 0)
                _menuActionsRepository.Delete(whereClause);

            //insert permissions for actions against menu.
            IList<ZnodeMenuActionsPermission> themeAssets = _menuActionsRepository.Insert(menuActionsPermissionModel.ActionPermissionList.ToEntity<ZnodeMenuActionsPermission>().ToList())?.ToList();
            ZnodeLogging.LogMessage("menuActionsPermissions and themeAssets list count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { menuActionsPermissionsListCount = menuActionsPermissions?.Count(), themeAssetsListCount = themeAssets?.Count() });

            return themeAssets?.FirstOrDefault().MenuActionsPermissionId > 0;
        }

        #region Private Methods
        //Deletes record form Menu Action Permission table.
        private void DeleteFromMenuActionsPermission(int menuId)
        {
            IZnodeRepository<ZnodeMenuActionsPermission> _menuActionsRepository = new ZnodeRepository<ZnodeMenuActionsPermission>();

            //Create filter for menu id.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMenuActionsPermissionEnum.MenuId.ToString(), ProcedureFilterOperators.Equals, menuId.ToString()));
            EntityWhereClauseModel whereMenuId = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, whereMenuId.WhereClause);
            ZnodeLogging.LogMessage(_menuActionsRepository.Delete(whereMenuId.WhereClause, whereMenuId.FilterValues) ? Admin_Resources.SuccessMenuActionPermissionDelete : Admin_Resources.ErrorMenuActionPermissionDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        // Checks weather the contoller name is same as of in the model.
        private bool IsControllerNameChange(MenuModel model)
           => !Equals(_menuRepository.Table.FirstOrDefault(x => x.MenuId == model.MenuId)?.ControllerName.ToLower(), model.ControllerName.ToLower());

        //Update the filter for role menus based on the username filter.
        private void GetMenuListFiltersbasedOnRole(FilterCollection filters)
        {
            //Get filter value of username.
            string userName = filters.FirstOrDefault(x => x.FilterName == FilterKeys.Username.ToString().ToLower())?.FilterValue;
            ZnodeLogging.LogMessage("userName value: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userName);

            IUserService service = GetService<IUserService>();

            //To do this call is being replaced by get by id.
            //Get the User Account Details by its UserName.
            UserModel model = service.GetUserByUsername(userName, ZnodeConfigManager.SiteConfig.PortalId);
            ZnodeLogging.LogMessage("RoleId value in UserModel: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model?.User?.RoleId);

            List<int?> lstMenuIds = new List<int?>();
            if (!Equals(model, null) && !Equals(model.User, null) && !string.IsNullOrEmpty(model.User.RoleId))
            {
                //Get list of menu ids.
                lstMenuIds = _roleMenuRepository.Table.Where(x => x.RoleId == model.User.RoleId).Select(m => m.MenuId).ToList();
            }

            int? menuId = _menuRepository.Table.Where(x => x.ActionName == "Dashboard").Select(y => y.MenuId)?.FirstOrDefault();
            ZnodeLogging.LogMessage("menuId value: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, menuId);

            lstMenuIds.Add(menuId);
            ZnodeLogging.LogMessage("lstMenuIds list count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstMenuIds?.Count());

            filters.Clear();
            filters.Add(new FilterTuple(ZnodeMenuEnum.MenuId.ToString(), FilterOperators.In, string.Join(",", lstMenuIds)));
            filters.Add(new FilterTuple(ZnodeMenuEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
        }


        //Bind the Parent Menu details.
        private static void BindParentMenus(MenuListModel menuList)
        {
            //returns list of all parent menus.
            if (menuList?.Menus?.Count > 0)
            {
                var parentMenuIdList = (from parent in menuList.Menus
                                        where parent.ParentMenuId == null && parent.IsActive == true
                                        select new
                                        {
                                            ParentMenuId = parent.MenuId,
                                            MenuName = parent.MenuName
                                        }).ToList();
                ZnodeLogging.LogMessage("parentMenuIdList count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, parentMenuIdList?.Count());

                //Adds list of parent menus to model.
                if (parentMenuIdList?.Count() > 0)
                    parentMenuIdList.ForEach(parent => { menuList.ParentMenus.Add(new MenuModel() { MenuName = parent.MenuName, MenuId = parent.ParentMenuId }); });

            }
        }
        #endregion
    }
}
