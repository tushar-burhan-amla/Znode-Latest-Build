using MvcSiteMapProvider;
using System;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class UserController : BaseController
    {
        #region Private ReadOnly members
        private readonly IUserAgent _userAgent;
        private readonly IRoleAgent _roleAgent;
        private readonly AuthenticationHelper _authenticationHelper;
        private LoginViewModel model = null;

        private readonly string createEditUser = "CreateEditUser";
        private readonly string customerCreateEditView = "CustomerCreateEdit";
        private readonly string usersList = "UsersList";
        private readonly string manageUser = "_ManageUser";
        #endregion

        #region Public Constructor
        public UserController(IUserAgent userAgent, IRoleAgent roleAgent, AuthenticationHelper authenticationHelper)
        {
            _userAgent = userAgent;
            _authenticationHelper = authenticationHelper;
            _roleAgent = roleAgent;
        }
        #endregion

        #region Public Methods

        //Get the index view for Admin Landing Page
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Admin", Area = "", ParentKey = "Home")]
        public virtual ActionResult Index() => View();

        #region Login
        //Login index page
        //This method first validates the Znode License. If valid then redirects to login page else redirects to License Activation page
        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                _userAgent.Logout();

            //Get user name from cookies
            GetLoginRememberMeCookie();
            return View(model );
        }

        // Posts the LoginViewModel to authenticate the user.
        // Logs in if the user is authenticated or it shows error messages accordingly.
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                LoginViewModel loginviewModel = _userAgent.Login(model);
                if (HelperUtility.IsNotNull(loginviewModel))
                {
                    if (!loginviewModel.HasError)
                    {
                        _authenticationHelper.SetAuthCookie(model.Username, model.RememberMe);

                        if (model.RememberMe)
                            SaveLoginRememberMeCookie(model.Username);

                        return RedirectToLocal(returnUrl);
                    }
                    if (loginviewModel.IsResetAdminPassword)
                    {
                        TempData[AdminConstants.UserName] = model.Username;
                        return RedirectToAction<UserController>(x => x.ResetAdminPassword(string.Empty));
                    }
                    SetNotificationMessage(GetErrorNotificationMessage(loginviewModel.ErrorMessage));
                }
            }
            return View(model);
        }
        #endregion

        #region Log Out
        //Logs off the user from the site.
        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
                _userAgent.Logout();

            return RedirectToAction<UserController>(x => x.Login(string.Empty));
        }
        #endregion

        #region Change Password
        //Change Password Page
        [Authorize]
        [HttpGet]
        [MvcSiteMapNode(Title = "Change Password", Key = "ChangePassword", Area = "", ParentKey = "Home")]
        public virtual ActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = HelperUtility.EncodeBase64(User.Identity.Name);
                model = _userAgent.ChangePassword(model);
                SetNotificationMessage(model.HasError
                    ? GetErrorNotificationMessage(model.ErrorMessage)
                    : GetSuccessNotificationMessage(model.SuccessMessage));

                if (!model.HasError)
                    return RedirectToAction<DashboardController>(x => x.Dashboard());
            }
            return View(model);
        }
        #endregion

        #region Forgot Password
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult ForgotPassword(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = _userAgent.ForgotPassword(model);
                SetNotificationMessage(model.HasError
                   ? GetErrorNotificationMessage(model.ErrorMessage)
                   : GenerateNotificationMessages(model.SuccessMessage, NotificationType.success));
            }
            return View(model);
        }
        #endregion

        #region Reset Password
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ResetPassword(string passwordToken, string userName)
        {
            ChangePasswordViewModel resetPassword = new ChangePasswordViewModel();
            resetPassword.UserName = userName;
            resetPassword.PasswordToken = passwordToken;
            //Set ResetPasword flag, use to hide Old Password field in View.
            resetPassword.IsResetPassword = true;

            ResetPasswordStatusTypes enumStatus = _userAgent.VerifyResetPasswordLinkStatus(resetPassword);
            switch (enumStatus)
            {
                case ResetPasswordStatusTypes.Continue:
                    return View(resetPassword);
                case ResetPasswordStatusTypes.LinkExpired:
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ResetPasswordLinkExpired));
                    break;
                case ResetPasswordStatusTypes.TokenMismatch:
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ResetPasswordLinkExpired));
                    break;
                case ResetPasswordStatusTypes.NoRecord:
                default:
                    return RedirectToAction<UserController>(x => x.ForgotPassword(null));
            }
            return RedirectToAction<UserController>(x => x.ForgotPassword(null));
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult ResetPassword(ChangePasswordViewModel model)
        {
            model.IsResetPassword = true;
            ModelState.Remove("OldPassword");
            if (ModelState.IsValid)
            {
                ChangePasswordViewModel changepasswordmodel = _userAgent.ChangePassword(model);

                if (!changepasswordmodel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.ResetPasswordSuccess));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(changepasswordmodel.ErrorMessage));
            }
            return RedirectToAction<UserController>(x => x.Login(string.Empty));
        }
        #endregion

        #region Reset Admin Password       
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ResetAdminPassword(string userName = "")
        {
            if (!string.IsNullOrEmpty(userName))
                return View(new ChangePasswordViewModel() { UserName = Convert.ToString(TempData[AdminConstants.UserName]) });

            if (!_userAgent.CheckUserKey() || HelperUtility.IsNull(TempData[AdminConstants.UserName]))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View(new ChangePasswordViewModel() { UserName = Convert.ToString(TempData[AdminConstants.UserName]) });
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult ResetAdminPassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = HelperUtility.EncodeBase64(model.UserName);
                ChangePasswordViewModel resetPasswordModel = _userAgent.ChangePassword(model);
                if (!resetPasswordModel.HasError)
                {
                    //If Current login user is after reset is different from older user logout older user.
                    if (!string.IsNullOrEmpty(User.Identity.Name) && User.Identity.Name != HelperUtility.DecodeBase64(model.UserName))
                        _userAgent.Logout();

                    LoginViewModel loginModel = new LoginViewModel() { Username = HelperUtility.DecodeBase64(model.UserName), Password = model.NewPassword, };

                    LoginViewModel accountViewModel = _userAgent.Login(loginModel);

                    if (!accountViewModel.HasError)
                    {
                        SessionHelper.SaveDataInSession<string>(AdminConstants.SuccessMessage, resetPasswordModel.SuccessMessage);
                        _authenticationHelper.SetAuthCookie(loginModel.Username, true);
                        SetNotificationMessage(GetSuccessNotificationMessage(resetPasswordModel.SuccessMessage));
                        return RedirectToAction<DashboardController>(x => x.Dashboard());
                    }
                    else
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(accountViewModel.ErrorMessage));
                        return View(resetPasswordModel);
                    }
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(resetPasswordModel.ErrorMessage));
                    return View(resetPasswordModel);
                }
            }
            return View(model);
        }

        [AllowAnonymous]

        public virtual ActionResult IsShowChangePasswordPopup() => Json(new
        {
            status = _userAgent.IsShowChangePasswordPopup()
        }, JsonRequestBehavior.AllowGet);

        [AllowAnonymous]

        public virtual ActionResult SaveInCookie() => Json(new
        {
            status = _userAgent.SaveInCookie()
        }, JsonRequestBehavior.AllowGet);


        #endregion

        #region Helper

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(HttpUtility.UrlDecode(returnUrl));

            return RedirectToAction<DashboardController>(x => x.Dashboard());
        }

        private void SaveLoginRememberMeCookie(string userId)
        {
            //Check if the browser support cookies 
            if ((HttpContext.Request.Browser.Cookies))
            {
                CookieHelper.SetCookie(AdminConstants.LoginCookieNameValue, userId, (Convert.ToDouble(ZnodeAdminSettings.CookieExpiresValue) * ZnodeConstant.MinutesInADay), true);

            }
        }

        private void GetLoginRememberMeCookie()
        {
            if ((HttpContext.Request.Browser.Cookies))
            {
                if (CookieHelper.IsCookieExists(AdminConstants.LoginCookieNameValue))
                {
                    if (CookieHelper.IsCookieExists(AdminConstants.LoginCookieNameValue))
                    {
                        string loginName = HttpUtility.HtmlEncode(CookieHelper.GetCookieValue<string>(AdminConstants.LoginCookieNameValue));
                        model = new LoginViewModel();
                        model.Username = loginName;
                        model.RememberMe = true;
                    }
                }
            }
        }

        public virtual ActionResult SetCollapseMenuStatus(bool status)
        {
            _userAgent.SetCollapseMenuStatus(status);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customers
        /// <summary>
        /// This method will fetch the list of all the customer account details.
        /// </summary>
        /// <param name="model">Filtercollection model</param>
        /// <returns>Returns the list of admin account details.</returns>
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Customer", Area = "", ParentKey = "Admin")]
        public virtual ActionResult CustomersList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCustomerAccount.ToString(), model);
            //Get the list of attributes            
            CustomerListViewModel customerViewModel = _userAgent.GetCustomerAccountList(HttpContext.User.Identity.Name, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, GridListType.ZnodeCustomerAccount.ToString(), string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            gridModel.TotalRecordCount = customerViewModel.TotalResults;

            //Returns the attribute list view
            return ActionView(new CustomerListViewModel() { List = customerViewModel.List, GridModel = gridModel });
        }

        /// <summary>
        /// Create Customer User.
        /// </summary>
        /// <returns>Create view for Customer User.</returns>
        public virtual ActionResult CustomerCreate()
            => ActionView(customerCreateEditView, new CustomerViewModel { Portals = _userAgent.GetPortals(), Accounts = _userAgent.GetAccounts() });

        /// <summary>
        /// This method will add Customer User.
        /// </summary>
        /// <returns>Returns created customer account.</returns>
        [HttpPost]
        public virtual ActionResult CustomerCreate(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomerViewModel model = _userAgent.CreateCustomerAccount(customerViewModel);

                if (!model?.HasError ?? false)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<UserController>(x => x.CustomersList(null));
                }
                SetNotificationMessage(GetErrorNotificationMessage(string.IsNullOrEmpty(model.ErrorMessage) ? Admin_Resources.ErrorFailedToCreate : model?.ErrorMessage));
            }
            _userAgent.SetCustomerViewModel(customerViewModel);
            return ActionView(customerCreateEditView, customerViewModel);
        }

        /// <summary>
        /// Edit Customer User.
        /// </summary>
        /// <param name="id">User Id whose account has to be updated.</param>
        /// <returns>Returns updated customer account.</returns>
        [HttpGet]
        public virtual ActionResult CustomerEdit(int userId)
        {
            if (userId > 0)
            {
                CustomerViewModel customerAccountDetails = _userAgent.GetCustomerAccountById(userId);
                _userAgent.SetCustomerViewModel(customerAccountDetails);
                return ActionView(customerCreateEditView, customerAccountDetails);
            }
            return RedirectToAction<UserController>(x => x.CustomersList(null));
        }

        /// <summary>
        /// This method update the users account details.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Returns updated account data.</returns>
        [HttpPost]
        public virtual ActionResult CustomerEdit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string errorMessage = string.Empty;

                if (_userAgent.UpdateCustomerAccount(model, out errorMessage))
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
                    _userAgent.SetCustomerViewModel(model);
                    return ActionView(customerCreateEditView, model);
                }
            }
            return RedirectToAction<UserController>(x => x.CustomersList(null));
        }

        /// <summary>
        /// This method will enable or disable the customer account.
        /// </summary>
        /// <param name="id">User Ids whose accounts have to be enabled or disabled.</param>
        /// <param name="isLock">To check if customer account is locked or not.</param>
        /// <returns>Returns if the customer account is enabled or disabled.</returns>
        public virtual ActionResult CustomerEnableDisableAccount(string userId, bool isLock)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                bool status = _userAgent.EnableDisableUser(userId, !isLock, out message);
                if (status && isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UnlockMessage));
                else if (status && !isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.LockMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<CustomerController>(x => x.CustomersList(null, 0, null));
        }

        /// <summary>
        /// This method will redirect to the Manage page for Customer User.
        /// </summary>
        /// <param name="id">User Id of customer.</param>
        /// <returns>Returns Manage View of Customer User.</returns>
        public virtual ActionResult CustomerManage(int? userId)
        {
            if (userId > 0)
            {
                CustomerViewModel customerAccountDetails = _userAgent.GetCustomerAccountById(userId.GetValueOrDefault());
                return Request.IsAjaxRequest() ? PartialView("_CustomerManage", customerAccountDetails) : ActionView(customerAccountDetails);
            }
            return RedirectToAction<CustomerController>(x => x.CustomersList(null, 0, null));
        }

        /// <summary>
        /// Delete customer account.
        /// </summary>
        /// <param name="accountId">User Ids of customer.</param>
        /// <returns>Returns true if account deleted successfully, else return false.</returns>
        public virtual JsonResult CustomerDelete(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                string message = string.Empty;
                string currentUserName = HttpContext.User.Identity.Name;
                bool status = _userAgent.DeleteCustomerAccount(userId, currentUserName, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets b2b account role list.
        /// </summary>
        /// <returns>Json value containing b2b account list.</returns>
        [HttpGet]
        public virtual JsonResult GetRoleList()
            => Json(_userAgent.GetAccountRoleList(), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// Gets b2b account role list.
        /// </summary>
        /// <returns>Json value containing b2b account list.</returns>
        [HttpGet]
        public virtual JsonResult GetAccountDepartments(int? accountId)
            => Json(_userAgent.GetAccountDepartments(accountId), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// Gets b2b permission list.
        /// </summary>
        /// <returns>Json value containing b2b permission list.</returns>
        [AllowAnonymous]
        [HttpGet]
        public virtual JsonResult GetPermissionList(int? accountId, int accountPermissionId = 0)
            => Json(_userAgent.GetPermissionList(accountId.GetValueOrDefault(), accountPermissionId), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// Gets user approver list.
        /// </summary>
        /// <returns>Json value containing user approver list.</returns>
        [HttpGet]
        public virtual JsonResult GetApproverList(int accountId, int? userId)
            => Json(_userAgent.GetApproverList(accountId, userId, HttpContext.User.Identity.Name), JsonRequestBehavior.AllowGet);
        #endregion

        #region Users
        /// <summary>
        /// Get User List With Admin Role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "StoreAdmin", Area = "", ParentKey = "Admin")]
        public virtual ActionResult UsersList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeUser.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUser.ToString(), model);
            //Get the list of attributes            
            UsersListViewModel usersViewModel = _userAgent.GetUserAccountList(HttpContext.User.Identity.Name, AdminConstants.Admin, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, usersViewModel.List, GridListType.ZnodeUser.ToString(), string.Empty, null, true, true, usersViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            gridModel.TotalRecordCount = usersViewModel.TotalResults;
            //Returns the attribute list view
            return ActionView(usersList, new UsersListViewModel() { List = usersViewModel.List, GridModel = gridModel });
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <returns>view For Create user</returns>
        [HttpGet]
        public virtual ActionResult CreateUser()
            => View(createEditUser, new UsersViewModel() { RoleList = _roleAgent.GetAdminRoles(), Portals = _userAgent.GetPortals() });

        /// <summary>
        /// Create User
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
                UsersViewModel model = _userAgent.CreateUser(usersViewModel);
                if (!model.HasError)
                {
                    if(model.IsEmailSentFailed)
                        SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.EmailTemplateNotExists));
                    else
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.AccountCreationSuccessMessage));
                    return RedirectToAction<UserController>(x => x.EditUser(model.UserId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            usersViewModel.RoleList = _roleAgent.GetAdminRoles();
            usersViewModel.Portals = _userAgent.GetPortals();
            return View(createEditUser, usersViewModel);
        }

        /// <summary>
        /// Manage Existing User User
        /// </summary>
        /// <param name="accountId">user account id</param>
        /// <returns></returns>
        public virtual ActionResult ManageUser(int userId)
        {
            if (userId > 0)
            {
                UsersViewModel users = _userAgent.GetUserAccountData(userId);
                if (HelperUtility.IsNotNull(users))
                    users.RoleList = _roleAgent.GetAdminRoles();

                return Request.IsAjaxRequest() ? PartialView(manageUser, users) : ActionView(users);
            }
            return RedirectToAction<UserController>(x => x.UsersList(null));
        }

        /// <summary>
        /// This method will redirect to the edit store admin details page.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Returns edit page.</returns>
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
                return ActionView(createEditUser, users);
            }
            return RedirectToAction<UserController>(x => x.UsersList(null));
        }

        /// <summary>
        /// This method update the users account details.
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
                if (_userAgent.UpdateUserAccountData(model))
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return RedirectToAction<UserController>(x => x.EditUser(model.UserId));
            }
            model.RoleList = _roleAgent.GetAdminRoles();
            model.Portals = _userAgent.GetPortals();
            return ActionView(createEditUser, model);
        }

        /// <summary>
        /// Delete user account.
        /// </summary>
        /// <param name="accountId">User Ids to be deleted.</param>
        /// <returns>Returns true if account deleted successfully, else return false.</returns>
        public virtual JsonResult DeleteUser(string userId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            if (!string.IsNullOrEmpty(userId))
            {
                string currentUserName = HttpContext.User.Identity.Name;
                bool status = _userAgent.DeleteUser(userId, currentUserName, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method will enable or disable the user admin account.
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
            return RedirectToAction<UserController>(x => x.UsersList(null));
        }

        /// <summary>
        /// This method will reset the password for the user admins in bulk.
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
                    return RedirectToAction<UserController>(x => x.UsersList(null));
                }
                SetNotificationMessage(GetSuccessNotificationMessage((Admin_Resources.SuccessResetPassword)));
            }
            return RedirectToAction<UserController>(x => x.UsersList(null));
        }

        /// <summary>
        /// This method will reset the password for the single user admin.
        /// </summary>
        /// <param name="id">User Id whose password has to be reset.</param>
        /// <returns>Returns account with reset password.</returns>
        [HttpGet]
        public virtual ActionResult SingleResetPassword(int userId)
        {
            if (userId > 0)
            {
                string errorMessage = string.Empty;
                bool status = _userAgent.ResetPassword(userId, out errorMessage);
                return Json(new { status = status, message = status ? Admin_Resources.SuccessResetPassword : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorAccessDenied }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method will lock unlock single account for manage user
        /// </summary>
        /// <param name="userId">user Id which has to be Locked/Unlocked</param>
        /// <param name="isLock">To check if the account is locked.</param>
        /// <param name="isAdminUser">To check if the account is Admin User</param>
        /// <returns></returns>
        public virtual ActionResult EnableDisableSingleAccount(string userId, bool isLock, bool isAdminUser)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                string message = string.Empty;
                bool status = isAdminUser ? _userAgent.EnableDisableUser(userId, !isLock, HttpContext.User.Identity.Name, out message):_userAgent.EnableDisableUser(userId, !isLock, out message);
                if (!string.IsNullOrEmpty(message))
                    return Json(new { status = false, isLock, message }, JsonRequestBehavior.AllowGet);

                if (status && isLock)
                    message = Admin_Resources.UnlockSingleAccount;
                if (status && !isLock)
                    message = Admin_Resources.LockSingleAccount;

                return Json(new { status = true, message }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { status = false, message = Admin_Resources.ErrorLockUnlock }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult UserPermissions(string aspNetUserId, int userId, bool isCustomer = false)
            => ActionView("_assignedPortals", _userAgent.GetPortalIds(aspNetUserId, userId));

        [HttpPost]
        public virtual ActionResult UserPermissions(UserPortalViewModel model)
        {
            SetNotificationMessage(_userAgent.SavePortalsIds(model) ? GetSuccessNotificationMessage(Admin_Resources.AssignSuccessful) : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToAssign));
            return model.IsCustomer ? RedirectToAction<UserController>(x => x.CustomerManage(model.UserId)) : RedirectToAction<UserController>(x => x.ManageUser(model.UserId));
        }

        //Check user name already exists or not.
        public virtual JsonResult IsUserNameExists(string userName, int portalId)
          => Json(!_userAgent.CheckUserNameExist(userName, portalId), JsonRequestBehavior.AllowGet);

        //Check is user name is existing shopper.
        public virtual JsonResult IsUserNameAnExistingShopper(string userName)
        {
            string errorMessage = string.Empty;
            return  Json(new { status = _userAgent.CheckIsUserNameAnExistingShopper(userName, out errorMessage), message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //This method will convert shopper to admin.
        [HttpPost]
        public virtual ActionResult ConvertShopperToAdmin(UsersViewModel usersViewModel)
        {
            //If the IsSelectAllPortal is true then remove the validation on PortalIds.
            if (usersViewModel.IsSelectAllPortal)
                ModelState.Remove("PortalIds");
            if (ModelState.IsValid)
            {
                UsersViewModel model = _userAgent.ConvertShopperToAdmin(usersViewModel);
                if (!model.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.AccountConversionSuccessMessage));
                    return RedirectToAction<UserController>(x => x.EditUser(model.UserId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            usersViewModel.RoleList = _roleAgent.GetAdminRoles();
            usersViewModel.Portals = _userAgent.GetPortals();
            return View(createEditUser, usersViewModel);
        }

        #endregion

        #region Update Billing Number
        [HttpGet]
        public ActionResult UpdateBillingNumber(int userId)
         => ActionView(_userAgent.GetEntityAttributeDetails(userId));

        [HttpPost]
        public virtual ActionResult UpdateBillingNumber([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            string errorMessage = string.Empty;

            EntityAttributeViewModel entityAttributeViewModel = _userAgent.UpdateBillingNumber(model, out errorMessage);
            SetNotificationMessage(entityAttributeViewModel.IsSuccess ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(errorMessage));

            return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
        }
        #endregion

        #endregion
    }
}