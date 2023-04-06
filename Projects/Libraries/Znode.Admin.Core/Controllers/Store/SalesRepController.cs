using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class SalesRepController:BaseController
    {
        #region Private ReadOnly members
        private readonly ISalesRepAgent _salesRepAgent;
        private readonly IUserAgent _userAgent;
        private readonly IRoleAgent _roleAgent;
        private readonly string createEdit = "../User/CreateEditUser";
       
        #endregion

        #region Public Constructor
        public SalesRepController(ISalesRepAgent salesRepAgent, IUserAgent userAgent,IRoleAgent roleRepAgent)
        {
            _salesRepAgent = salesRepAgent;
            _userAgent = userAgent;
            _roleAgent = roleRepAgent;
        }
        #endregion

        #region "Public Methods"

        /// <summary>
        /// Get the list of all Sales Reps.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "SalesRep", Area = "", ParentKey = "Admin")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeSalesReps.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSalesReps.ToString(), model);

            //Get the list of all stores.
            UsersListViewModel salesRepList = _salesRepAgent.GetSalesRepList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            salesRepList.GridModel = FilterHelpers.GetDynamicGridModel(model, salesRepList.List, GridListType.ZnodeSalesReps.ToString(), string.Empty, null, true, true, salesRepList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            salesRepList.GridModel.TotalRecordCount = salesRepList.TotalResults;

            return ActionView(salesRepList);
        }

        /// <summary>
        /// Create Sales Rep User
        /// </summary>
        /// <returns>view For Create user</returns>
        [HttpGet]
        public virtual ActionResult CreateUser()  
        => View(createEdit, new UsersViewModel() { RoleList = _roleAgent.GetAdminRoles(), Portals = _userAgent.GetPortals() });
        
        /// <summary>
        /// Create Sales Rep User
        /// </summary>
        /// <param name="usersViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult CreateUser(UsersViewModel usersViewModel)
        {
            //If the IsSelectAllPortal is true then remove the validation on PortalIds.
            if (usersViewModel.IsSelectAllPortal)
                ModelState.Remove("PortalIds");
            if (ModelState.IsValid)
            {
                usersViewModel.RoleName = AdminConstants.SalesRepRole;
                UsersViewModel model = _userAgent.CreateUser(usersViewModel);
                if (!model.HasError)
                {
                    if (model.IsEmailSentFailed)
                        SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.EmailTemplateNotExists));
                    else
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.AccountCreationSuccessMessage));
                    return RedirectToAction<SalesRepController>(x => x.EditUser(model.UserId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            usersViewModel.RoleList = _roleAgent.GetAdminRoles();
            usersViewModel.Portals = _userAgent.GetPortals();
            return View(createEdit, usersViewModel);
        }

        /// <summary>
        /// Edit Sales Rep User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult EditUser(int userId)
        {
            ActionResult action = GotoBackURL();
            
            if (action != null)
                return action;

            if (userId > 0)
            {
                UsersViewModel users = _userAgent.GetUserAccountData(userId);

                if (HelperUtility.IsNotNull(users))
                {
                    users.RoleList = _roleAgent.GetAdminRoles();
                    users.Portals = _userAgent.GetPortals();
                }
                return ActionView(createEdit, users);
            }
            return RedirectToAction<SalesRepController>(x => x.List(null));
        }

        /// <summary>
        /// This method update the sales rep users account details.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Returns updated account data.</returns>
        [HttpPost]
        public virtual ActionResult EditUser(UsersViewModel model)
        {
            //If the IsSelectAllPortal is true then remove the validation on PortalIds.
            if (model.IsSelectAllPortal)
                ModelState.Remove("PortalIds");

            if (ModelState.IsValid)
            {
                model.RoleName = AdminConstants.SalesRepRole;
                if (_userAgent.UpdateUserAccountData(model))
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return RedirectToAction<SalesRepController>(x => x.EditUser(model.UserId));
            }
            model.RoleList = _roleAgent.GetAdminRoles();
            model.Portals = _userAgent.GetPortals();
            return ActionView(createEdit, model);
        }

        /// <summary>
        /// This method will enable or disable the sales rep user account.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="isLock">To check if user account is locked or not.</param>
        /// <returns>Returns if the user account is enabled or disabled.</returns>
        public virtual ActionResult EnableDisableAccount(string userId, bool isLock)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                string message = string.Empty;
                bool status = _userAgent.EnableDisableUser(userId, !isLock, HttpContext.User.Identity.Name, out message);
                if (status && isLock)
                    SetNotificationMessage(string.IsNullOrEmpty(message)
                        ? GetSuccessNotificationMessage(Admin_Resources.UnlockMessage)
                        : GetSuccessNotificationMessage(message));
                else if (status && !isLock)
                    SetNotificationMessage(string.IsNullOrEmpty(message)
                       ? GetSuccessNotificationMessage(Admin_Resources.LockMessage)
                       : GetSuccessNotificationMessage(message));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<SalesRepController>(x => x.List(null));
        }

        /// <summary>
        /// This method will reset the password for the Sales Rep user in bulk.
        /// </summary>
        /// <param name="id">User Ids whose password has to be reset.</param>
        /// <returns>Returns account with reset password.</returns>
        public virtual ActionResult BulkResetPassword(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                string message = string.Empty;
                if (!_userAgent.BulkResetPassword(userId, out message))
                {
                    SetNotificationMessage(GetErrorNotificationMessage(message));
                    return RedirectToAction<SalesRepController>(x => x.List(null));
                }
                SetNotificationMessage(GetSuccessNotificationMessage((Admin_Resources.SuccessResetPassword)));
            }
            return RedirectToAction<SalesRepController>(x => x.List(null));
        }
        
        #endregion

    }
}
