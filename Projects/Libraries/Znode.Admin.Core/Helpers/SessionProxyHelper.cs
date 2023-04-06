using System;
using System.Collections.Generic;
using System.Web;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Libraries.ECommerce.Utilities;
using System.Linq;
using Znode.Admin.Core.Helpers;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Helpers
{
    public class SessionProxyHelper
    {
        //Summary
        //Redirect User to Index page in case the un authorized access.
        //Return the List of action Permission list in List<string> format.
        public static List<RolePermissionViewModel> GetUserPermission()
        {
            List<RolePermissionViewModel> lstUserPermission  = null;
            try
            {
                lstUserPermission = SessionHelper.GetDataFromSession<List<RolePermissionViewModel>>(AdminConstants.UserPermissionListSessionKey);

                if (Equals(lstUserPermission, null))
                {
                    //Get the User Roles Permission List
                    IEnumerable<RolePermissionViewModel> permissionList = GetService<IDependencyHelper>().GetPermissions();
                    if (!Equals(permissionList, null) && permissionList.Count() > 0)
                    {
                        lstUserPermission = permissionList.ToList();
                    }
                    SessionHelper.SaveDataInSession<List<RolePermissionViewModel>>(AdminConstants.UserPermissionListSessionKey, lstUserPermission);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return lstUserPermission;
        }

        //Summary
        // To Check whether Current login user is Admin or Not.
        public static bool IsAdminUser()
        {
            bool? isAdminUser = SessionHelper.GetDataFromSession<bool?>(AdminConstants.IsAdminUserSessionKey);
            if (Equals(isAdminUser, null))
            {
                isAdminUser = GetService<IDependencyHelper>().IsUserInRole();
                SessionHelper.SaveDataInSession<bool?>(AdminConstants.IsAdminUserSessionKey, isAdminUser);
            }
            
            return isAdminUser == true;
        }


        //Summary
        //To Get the Menu list.
        //Return the List of Menu in MenuListModel format
        public MenuListViewModel GetMenuListByUserName()
        {
            MenuListViewModel menuList = null;
            try
            {
                menuList = SessionHelper.GetDataFromSession<MenuListViewModel>(AdminConstants.UserMenuListSessionKey);

                if (Equals(menuList, null))
                {
                    //Gets the Menu List
                    IMenuAgent _menuAgent = new MenuAgent(new MenuClient(), new RoleClient(), new AccessPermissionClient());

                    FilterCollection filters = new FilterCollection();
                    filters.Add(new FilterTuple(ZnodeMenuEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
                    if (!IsAdminUser() && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        filters.Add(new FilterTuple(FilterKeys.Username.ToString(), FilterOperators.Equals, HttpContext.Current.User.Identity.Name));
                    }
                    menuList = _menuAgent.GetMenuList(filters, null, null, null);
                    SessionHelper.SaveDataInSession<MenuListViewModel>(AdminConstants.UserMenuListSessionKey, menuList);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return menuList;
        }

        //Get the Login User Details based on the user name. To bind user id in Api request.
        public static UserViewModel GetUserDetails()
        {
            UserViewModel model = null;
            try
            {
                model = SessionHelper.GetDataFromSession<UserViewModel>(AdminConstants.UserAccountSessionKey);

                if (Equals(model, null))
                {
                    if (HttpContext.Current.User != null)
                    {
                        //Get the User Details. 
                        //Don't Use the Agent here, it will cause the infinite looping. As this method gets called from the BaseAgent.
                        UserClient client = new UserClient();
                        var userModel = client.GetAccountByUser(HttpContext.Current.User.Identity.Name);
                        if (!Equals(userModel, null))
                        {
                            model = userModel.ToViewModel<UserViewModel>();
                            SessionHelper.SaveDataInSession<UserViewModel>(AdminConstants.UserAccountSessionKey, model);
                        }
                        client = null;

                    }

                }
            }
            catch { }
            return model;
        }

        //Get the Collapse Menu Status from Session.
        public bool GetCollapseMenuStatus()
        {
            bool? menuStatus = SessionHelper.GetDataFromSession<bool?>(AdminConstants.CollapseMenuStatus);

            //if menuStatus true or null, it should be treated as true, only explicit false should return as false
            return menuStatus != false;
        }

        //Summary
        // To Check whether Current login user is Sales Rep or Not.
        public static bool IsSalesRepUser()
        {
            bool isSalesRepUser = false;
            isSalesRepUser = GetService<IDependencyHelper>().IsUserInRole(true);
            return isSalesRepUser;
        }
    }
}
