using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Newtonsoft.Json;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Maps
{
    public class RoleViewModelMap
    {
        //Convert RoleViewModel model to RoleModel. 
        public static RoleModel ToModel(RoleViewModel roleViewModel)
        {
            if (HelperUtility.IsNotNull(roleViewModel))
            {
                return new RoleModel()
                {
                    RoleId = roleViewModel.Id,
                    IsActive = roleViewModel.IsActive,
                    Name = roleViewModel.Name,
                    IsSystemDefined = roleViewModel.IsSystemDefined,
                    TypeOfRole = roleViewModel.TypeOfRole,
                    RoleRights = ToRolePermissionModel(JsonConvert.DeserializeObject<List<ManageDataModel>>(roleViewModel.PermissionAccessString)),
                    IsAssociated = roleViewModel.IsAssociated,
                };
            }
            else
                return null;
        }

        //Converts Role Model to Role View Model.
        public static RoleViewModel ToViewModel(RoleModel roleModel)
        {
            if (HelperUtility.IsNull(roleModel))
                return null;

            return new RoleViewModel()
            {
                Id = roleModel.RoleId,
                Name = roleModel.Name,
                IsActive = roleModel.IsActive.Value,
                IsSystemDefined = roleModel.IsSystemDefined,
                TypeOfRole = roleModel.TypeOfRole,
                IsAssociated = roleModel.IsAssociated,
            };
        }

        //Converts RoleListModel to RoleListViewModel
        public static RoleListViewModel ToListViewModel(RoleListModel models)
        {
            RoleListViewModel listViewModel = new RoleListViewModel();

            if (!Equals(models, null) && !Equals(models.Roles, null))
            {
                foreach (RoleModel model in models.Roles)
                {
                    //maps the role model to role view model and adds to roles
                    listViewModel.Roles.Add(ToViewModel(model));
                }
                //set for pagination
                listViewModel.Page = Convert.ToInt32(models.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(models.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(models.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(models.TotalResults);
            }
            return listViewModel;
        }

        //Converts List<DataObjectViewModel> data, int roleId to RoleMenuModel
        public static RoleMenuModel ToRoleMenuPermissionModel(List<DataObjectViewModel> data, string roleId)
        {
            var rolePermissions = new List<DataObjectModel>();

            foreach (var item in data)
            {
                rolePermissions.Add(new DataObjectModel() { MenuId = item.MenuId, Rights = item.Rights });
            }

            return new RoleMenuModel()
            {
                RoleId = roleId,
                RolePermissions = rolePermissions
            };
        }

        //Converts List<DataObjectViewModel> data, int roleId to RoleMenuModel
        public static RoleMenuModel ToRolePermissionModel(List<DataObjectViewModel> data, int? roleMenuId)
        {
            var rolePermissions = new List<DataObjectModel>();

            foreach (var item in data)
            {
                rolePermissions.Add(new DataObjectModel() { MenuId = item.MenuId, Rights = item.Rights });
            }

            return new RoleMenuModel()
            {
                RoleMenuId = roleMenuId,
                RolePermissions = rolePermissions
            };
        }

        //Converts RoleMenuListModel models to RoleMenuListViewModel
        public static RoleMenuListViewModel ToRoleMenuListViewModel(RoleMenuListModel models)
        {
            RoleMenuListViewModel listViewModel = new RoleMenuListViewModel();

            if (HelperUtility.IsNotNull(models))
            {
                if (HelperUtility.IsNotNull(models.Menus))
                {
                    foreach (MenuModel model in models.Menus)
                    {
                        listViewModel.Menus.Add(model);
                    }
                }

                if (HelperUtility.IsNotNull(models.RoleMenuAccessMapper))
                {
                    foreach (RoleMenuAccessMapperModel model in models.RoleMenuAccessMapper)
                    {
                        listViewModel.RoleMenuAccessMapper.Add(ToAccessMapperViewModel(model));
                    }
                }
            }
            return listViewModel;
        }

        //Converts RoleMenuAccessMapperModel to RoleMenuAccessMapperViewModel
        public static RoleMenuAccessMapperViewModel ToAccessMapperViewModel(RoleMenuAccessMapperModel roleMenuAccessMapperModel)
        {
            if (Equals(roleMenuAccessMapperModel, null))
                return null;

            return new RoleMenuAccessMapperViewModel()
            {
                RoleMenuAccessMapperId = roleMenuAccessMapperModel.RoleMenuAccessMapperId,
                AccessPermissionsId = roleMenuAccessMapperModel.AccessPermissionsId,
                RoleMenuId = roleMenuAccessMapperModel.RoleMenuId,
                MenuId = roleMenuAccessMapperModel.MenuId

            };
        }

        //Converts List<ManageDataModel> data to List<DataObjectModel>
        public static List<DataObjectModel> ToRolePermissionModel(List<ManageDataModel> data)
        {
            var rolePermissions = new List<DataObjectModel>();

            foreach (var item in data)
            {
                rolePermissions.Add(new DataObjectModel() { MenuId = item.MenuId, Rights = item.Rights });
            }

            return rolePermissions;
        }

        /// <summary>
        /// Convert IEnumerable<RoleModel> to List<SelectListItem>
        /// </summary>
        /// <param name="model">To convert to list item type.</param>
        /// <returns>Returns List<SelectListItem></returns>
        public static List<SelectListItem> ToListItems(IEnumerable<RoleModel> roleModel)
        {
            List<SelectListItem> roles = new List<SelectListItem>();

            if (HelperUtility.IsNotNull(roleModel))
            {
                roles = (from item in roleModel
                         select new SelectListItem
                         {
                             Text = item.Name,
                             Value = item.Name.ToString(),
                         }).ToList();
            }
            return roles;
        }
    }
}