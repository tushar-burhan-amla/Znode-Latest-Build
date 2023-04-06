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
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class RoleService : BaseService, IRoleService
    {
        #region Private Variables
        private readonly IZnodeRepository<AspNetRole> _roleRepository;
        private readonly IZnodeRepository<ZnodeAccessPermission> _permissionRepository;
        private readonly IZnodeRepository<ZnodeRoleMenu> _roleMenuRepository;
        private readonly IZnodeRepository<ZnodeRoleMenuAccessMapper> _roleMenuAccessMapperRepository;
        private readonly IZnodeRepository<ZnodeMenu> _menuRepository;
        private readonly IZnodeRepository<ZnodeMenuActionsPermission> _menuActionPermissionRepository;
        private readonly IZnodeRepository<ZnodeAction> _actionRepository;
        #endregion

        #region Constructor
        //Constructor
        public RoleService()
        {
            _roleRepository = new ZnodeRepository<AspNetRole>();
            _permissionRepository = new ZnodeRepository<ZnodeAccessPermission>();
            _roleMenuRepository = new ZnodeRepository<ZnodeRoleMenu>();
            _roleMenuAccessMapperRepository = new ZnodeRepository<ZnodeRoleMenuAccessMapper>();
            _menuRepository = new ZnodeRepository<ZnodeMenu>();
            _menuActionPermissionRepository = new ZnodeRepository<ZnodeMenuActionsPermission>();
            _actionRepository = new ZnodeRepository<ZnodeAction>();
        }
        #endregion

        #region Public  Methods
        //Create role
        public virtual RoleModel CreateRole(RoleModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            //Maps role id.
            model.RoleId = Guid.NewGuid().ToString();

            //Check if role name already exists.
            if (_roleRepository.Table.Count(x => x.Name.Trim() == model.Name.Trim()) > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.RoleNameAlreadyExists);

            AspNetRole role = _roleRepository.Insert(model.ToEntity<AspNetRole>());

            if (HelperUtility.IsNotNull(role))
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessRoleInsert, model.Name), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                EditManagedRolePermissions(model.RoleRights, role.Id);
                ZnodeLogging.LogMessage("RoleId and Name properties of RoleModel to be returned: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { RoleId = role?.Id, Name = role?.Name });
                return role.ToModel<RoleModel>();
            }
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorRoleInsert, model.Name), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
      
        }

        //Get all permissions list
        public virtual PermissionListModel GetPermissions(FilterCollection filters)
        {
            EntityWhereClauseModel model = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return PermissionsMap.ToListModel(_permissionRepository.GetEntityList(model.WhereClause, model.FilterValues));
        }

        //Get roles
        public virtual RoleListModel GetRoles(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Set filters for store admin roles.
            SetFilterForStoreAdmin(filters);

            //Set Default Sort
            SetDefaultSorting(sorts);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get roleList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //maps the entity list to model
            RoleListModel roleList = RoleMap.ToListModel(_roleRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount));
            ZnodeLogging.LogMessage("Count of roleList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roleList?.Roles?.Count);

            //Set for pagination
            roleList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return roleList;
        }

        public virtual bool DeleteRole(ParameterModel roleIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter roleIds to delete role: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roleIds?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("RoleIds", roleIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteRole @RoleIds, @Status OUT", 1, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessRoleDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorRoleDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Get role by role id.
        public virtual RoleModel GetRole(string roleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter roleId: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roleId);

            if (!string.IsNullOrEmpty(roleId))
            {
                IZnodeViewRepository<RoleModel> objStoredProc = new ZnodeViewRepository<RoleModel>();
                objStoredProc.SetParameter("RoleId", roleId, ParameterDirection.Input, DbType.String);
                RoleModel roleDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetRole @RoleId").FirstOrDefault();
                ZnodeLogging.LogMessage("RoleId and Name properties of RoleModel to be returned: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { RoleId = roleDetails?.RoleId, Name = roleDetails?.Name });
                return !string.IsNullOrEmpty(roleDetails?.RoleId) ? roleDetails : new RoleModel();
            }
            return new RoleModel();
        }

        //Update role.
        public virtual bool UpdateRole(RoleModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (string.IsNullOrEmpty(model.RoleId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.RoleIdCanNotNullOrEmpty);
            ZnodeLogging.LogMessage("RoleId and Name properties of RoleModel: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { RoleId = model?.RoleId, Name = model?.Name });

            model.IsAssociated = GetRole(model.RoleId).IsAssociated;
            if (Equals(model.IsAssociated, true) && (Equals(model.IsActive, false)))
                throw new ZnodeException(ErrorCodes.AssociationUpdateError, Admin_Resources.RoleIdNotInActive);
            else
                //Updates role model returning void.
                _roleRepository.Update(model.ToEntity<AspNetRole>());

            //Update the Role Access Permissions.
            EditManagedRolePermissions(model.RoleRights, model.RoleId);
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessRoleUpdate, model.Name), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return true;
        }
        //Get role menu permissions list.
        public virtual RoleMenuListModel GetRolesMenusPermissionsWithRoleMenus(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause generated to get roleMenuList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);

            //Get Role Menu List, along with Access Mapping Permissions. 
            RoleMenuListModel roleMenuList = RoleMap.ToRoleMenuListModel(_roleMenuRepository.GetEntityList(whereClause.WhereClause, new List<string> { ZnodeRoleMenuEnum.ZnodeRoleMenuAccessMappers.ToString() }, whereClause.FilterValues));

            //Get All available Menus.
            List<MenuModel> listMenu = MenuMap.ToListModel(_menuRepository.GetEntityList(string.Empty)).Menus;
            ZnodeLogging.LogMessage("available Menus count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listMenu?.Count);

            //Bind the Parent Menu Names.
            listMenu.ForEach(item =>
            {
                item.ParentMenuName = listMenu.Find(x => x.MenuId == item.ParentMenuId)?.MenuName ?? string.Empty;
            });

            roleMenuList.Menus = listMenu;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return roleMenuList;
        }

        //Update the Role Menu access permissions.
        public virtual bool EditManagedRolePermissions(List<DataObjectModel> data, string roleId)
        {
            List<int> selectedMenuIds = data.Where(x => x.Rights.Count() > 0).Select(x => x.MenuId).ToList();

            //Get the List of assigned Role Menus based on the Role Id.
            List<ZnodeRoleMenu> roleMenus = _roleMenuRepository.Table.Where(x => x.RoleId == roleId).ToList();
            List<ZnodeRoleMenu> assignedRoleMenus = roleMenus.ToList();
            ZnodeLogging.LogMessage("assigned role menus count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, assignedRoleMenus?.Count);

            List<int> unSelectedMenuIds = new List<int>();
            if (!Equals(roleMenus, null) && roleMenus.Count > 0)
            {
                //Delete the Role menu access permissions.
                DeleteAssignedRoleMenuAccessPermissions(roleMenus);

                //Delete already exist entries from upcoming new entries to avoid duplicate insertion.
                DeleteMappedEntries(selectedMenuIds, roleMenus, assignedRoleMenus, unSelectedMenuIds);
            }
            //Insert the role menus for the selected role.
            IEnumerable<ZnodeRoleMenu> _entity = InsertRoleMenus(roleId, selectedMenuIds);

            //Delete the existing permission entries, which is not selected.
            if (unSelectedMenuIds.Count > 0)
                DeleteRoleMenuByMenuId(roleId, unSelectedMenuIds);

            //Insert the newly created entities in existing collection.
            if (!Equals(_entity, null) && _entity.Count() > 0)
                assignedRoleMenus.AddRange(_entity);

            //Insert the role menu access permission.
            InsertRoleAccessPermissions(data, assignedRoleMenus);
            return true;
        }

        //Get the User Permission List
        public virtual IEnumerable<RolePermissionModel> GetPermissionListByUserName(string userName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorUsernameEmpty);
                ZnodeLogging.LogMessage("Input parameter userName: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userName);

                //Get User details by username.
                UserModel model = GetUserDetails(userName);
                ZnodeLogging.LogMessage("User RoleId of UserModel: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model?.User?.RoleId);

                //Get User Role Action Permission based on the Role. In Case of Admin role, all the permissions gets fetched.
                IZnodeViewRepository<RolePermissionModel> objStoredProc = new ZnodeViewRepository<RolePermissionModel>();
                objStoredProc.SetParameter("RoleId", model.User.RoleId, ParameterDirection.Input, DbType.String);
                return objStoredProc.ExecuteStoredProcedureList("Znode_GetRoleActionPermission @RoleId");
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                return Enumerable.Empty<RolePermissionModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return Enumerable.Empty<RolePermissionModel>();
            }
        }
        #endregion

        #region Private Method     
        //Get the User Details Based on the User Name
        private UserModel GetUserDetails(string userName)
        {
            IUserService userService = GetService<IUserService>();
            //To do this call is being replaced by get by id.
            UserModel model = userService.GetUserByUsername(userName, ZnodeConfigManager.SiteConfig.PortalId);
            return model;
        }

        //Delete the already assigned role menu access permissions, to add new access permission entries.
        private void DeleteAssignedRoleMenuAccessPermissions(List<ZnodeRoleMenu> roleMenus)
        {
            //Set the Filters for role menu ids.
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodeRoleMenuEnum.RoleMenuId.ToString(), ProcedureFilterOperators.In, string.Join(",", roleMenus.Select(x => x.RoleMenuId))));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel generated to delete already assigned role menu access permissions: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            bool status = _roleMenuAccessMapperRepository.Delete(whereClauseModel.WhereClause);
            //Delete the access permissions.
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessRoleMenuAccessMapperDelete : Admin_Resources.ErrorRoleMenuAccessMapperDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            
        }

        //Insert the Role Access permission based on the selected Role Menu configuration.
        private void InsertRoleAccessPermissions(List<DataObjectModel> data, List<ZnodeRoleMenu> roleMenus)
        {
            List<ZnodeRoleMenuAccessMapper> lstAccessMapper = new List<ZnodeRoleMenuAccessMapper>();

            //Convert the Permission & Menu id combination in list.
            foreach (ZnodeRoleMenu item in roleMenus)
            {
                var permissions = data.Where(x => x.MenuId == item.MenuId).Select(y => y.Rights).FirstOrDefault();
                foreach (int accessId in permissions)
                {
                    lstAccessMapper.Add(new ZnodeRoleMenuAccessMapper() { AccessPermissionId = accessId, RoleMenuId = item.RoleMenuId });
                }
            }
            //Insert access permissions.
            _roleMenuAccessMapperRepository.Insert(lstAccessMapper);
        }

        //Method used to Insert the Menus for the selected Role.
        private IEnumerable<ZnodeRoleMenu> InsertRoleMenus(string roleId, List<int> selectedMenuIds)
        {
            List<ZnodeRoleMenu> lstRoleMenu = new List<ZnodeRoleMenu>();

            //Bind the Menu Ids & Role Ids in the list.
            foreach (int item in selectedMenuIds)
            {
                lstRoleMenu.Add(new ZnodeRoleMenu() { MenuId = item, RoleId = roleId });
            }
            //Insert role menus.
            return _roleMenuRepository.Insert(lstRoleMenu);
        }



        //Delete the assigned role menu based on role id & Menu Id.
        private void DeleteRoleMenuByMenuId(string roleId, List<int> unSelectedMenuIds)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(roleId))
            {
                //Set the Filters for role menu ids.
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(new FilterTuple(ZnodeRoleMenuEnum.RoleId.ToString(), ProcedureFilterOperators.Like, roleId));
                filtersList.Add(new FilterTuple(ZnodeRoleMenuEnum.MenuId.ToString(), ProcedureFilterOperators.In, string.Join(",", unSelectedMenuIds.Select(x => x))));

                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel generated to delete the assigned role menu: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

                status = _roleMenuRepository.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
                //Delete the Role Menus
                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessRoleMenuDelete : Admin_Resources.ErrorRoleMenuDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            
        }

 

        //Delete the already existing mapping entries, to avoid duplicate data manipulation.
        private void DeleteMappedEntries(List<int> selectedMenuIds, List<ZnodeRoleMenu> roleMenus, List<ZnodeRoleMenu> assignedRoleMenus, List<int> unSelectedMenuIds)
        {
            roleMenus.ForEach(item =>
            {
                int index = selectedMenuIds.FindIndex(x => x == item.MenuId);
                if (index != -1)
                    //Remove Selected Menu Ids, which are already exists in the Database.
                    selectedMenuIds.RemoveAt(index);
                else
                {
                    //Add Un Selected Menu Ids, so that it can be delete from the Database.
                    unSelectedMenuIds.Add(Convert.ToInt32(item.MenuId));
                    index = assignedRoleMenus.FindIndex(x => x.MenuId == item.MenuId);
                    if (index != -1)
                        //Delete the Unselected menu entries from the Collection.
                        assignedRoleMenus.RemoveAt(index);
                }
            });
        }

        //Set filters for store admin roles.
        private void SetFilterForStoreAdmin(FilterCollection filters)
        {
            //Check for accountId filter present in filters.
            if (filters.Exists(x => x.FilterName.Equals(FilterKeys.GetStoreAdminRoles, StringComparison.InvariantCultureIgnoreCase)))
            {
                //Remove GetStoreAdminRoles from filter.
                filters.RemoveAll(x => x.Item1.Equals(FilterKeys.GetStoreAdminRoles, StringComparison.InvariantCultureIgnoreCase));

                //Get the AspNetRole entity which having role user.
                AspNetRole apNetRole = _roleRepository.Table.FirstOrDefault(x => x.Name == ZnodeRoleEnum.User.ToString());
                //Remove the TypeOfRole filter.
                if (filters.Exists(x => x.FilterName.Equals(AspNetRoleEnum.TypeOfRole.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    filters.RemoveAll(x => x.Item1.Equals(AspNetRoleEnum.TypeOfRole.ToString(), StringComparison.InvariantCultureIgnoreCase));
                //Remove the Id filter.
                if (filters.Exists(x => x.FilterName.Equals(AspNetRoleEnum.Id.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    filters.RemoveAll(x => x.Item1.Equals(AspNetRoleEnum.Id.ToString(), StringComparison.InvariantCultureIgnoreCase));
                //Add the filters.
                filters.Add(AspNetRoleEnum.TypeOfRole.ToString(), FilterOperators.Equals, "Null");
                filters.Add(AspNetRoleEnum.Id.ToString(), FilterOperators.NotContains, apNetRole?.Id);
            }
        }

        //Set Default Sorting for Role List
        private void SetDefaultSorting(NameValueCollection sorts)
        {
            sorts = HelperUtility.IsNull(sorts) ? new NameValueCollection() : sorts;
            if (!CheckSortHasKeys(sorts))
                //Set Default sorting by CreatedDate 
                sorts.Add(AspNetRoleEnum.CreatedDate.ToString().ToLower(), "desc");

        }

        //Check for existing keys in Sort Collection
        private bool CheckSortHasKeys(NameValueCollection sorts)
        {
            bool hasKeys = false;
            if (sorts?.Count > 0 && sorts.HasKeys())
            {
                foreach (string key in sorts.AllKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        hasKeys = true;
                    }
                }
            }
            return hasKeys;
        }
        #endregion
    }
}

