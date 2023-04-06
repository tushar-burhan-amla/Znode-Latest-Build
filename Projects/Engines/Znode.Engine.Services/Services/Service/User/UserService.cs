using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Observer;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using System.Threading.Tasks;
using Znode.Engine.Api.Models.Enum;

namespace Znode.Engine.Services
{
    public class UserService : BaseService, IUserService
    {
        #region Private Variables
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationRoleManager _roleManager;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<AspNetZnodeUser> _aspNetZnodeUserRepository;
        private readonly IZnodeRepository<AspNetUser> _aspNetUserRepository;
        private readonly IZnodeRepository<ZnodeUserProfile> _accountProfileRepository;
        private readonly IZnodeRepository<ZnodeAccountUserPermission> _accountUserPermissionRepository;
        private readonly IZnodeRepository<ZnodeDepartmentUser> _departmentUserRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        private readonly IZnodeRepository<ZnodeAccessPermission> _accessPermissionRepository;
        private readonly IZnodeRepository<ZnodeAccountUserOrderApproval> _accountUserOrderApprovalRepository;
        private readonly IZnodeRepository<ZnodeAccountPermissionAccess> _accountPermissionAccessRepository;
        private readonly IZnodeRepository<ZnodePortalProfile> _portalProfileRepository;
        private readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        private readonly IZnodeRepository<ZnodeUserProfile> _userProfileRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeUserAddress> _userAddressRepository;
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodeAccountProfile> _accountAssociatedProfileRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodePasswordLog> _passwordLogRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderRepository;
        private readonly IZnodeRepository<ZnodeAccountPermission> _accountPermissionRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IUserLoginHelper _loginHelper;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;
        #endregion

        #region Public Constructors
        public UserService()
        {
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _accountProfileRepository = new ZnodeRepository<ZnodeUserProfile>();
            _accountUserPermissionRepository = new ZnodeRepository<ZnodeAccountUserPermission>();
            _departmentUserRepository = new ZnodeRepository<ZnodeDepartmentUser>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _aspNetZnodeUserRepository = new ZnodeRepository<AspNetZnodeUser>();
            _aspNetUserRepository = new ZnodeRepository<AspNetUser>();
            _accessPermissionRepository = new ZnodeRepository<ZnodeAccessPermission>();
            _accountUserOrderApprovalRepository = new ZnodeRepository<ZnodeAccountUserOrderApproval>();
            _accountPermissionAccessRepository = new ZnodeRepository<ZnodeAccountPermissionAccess>();
            _portalProfileRepository = new ZnodeRepository<ZnodePortalProfile>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _userProfileRepository = new ZnodeRepository<ZnodeUserProfile>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _userAddressRepository = new ZnodeRepository<ZnodeUserAddress>();
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _accountAssociatedProfileRepository = new ZnodeRepository<ZnodeAccountProfile>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _passwordLogRepository = new ZnodeRepository<ZnodePasswordLog>();
            _orderRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _accountPermissionRepository = new ZnodeRepository<ZnodeAccountPermission>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _loginHelper = GetService<IUserLoginHelper>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
        }

        public UserService(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region Public Properties
        //Get value of ApplicationSignInManager which is used for the application.
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //Get value of ApplicationUserManager which is used for the application.
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Get value of ApplicationRoleManager which is used for the application.
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region Public Methods
        public virtual UserModel Login(int portalId, UserModel model, out int? errorCode, NameValueCollection expand = null)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            errorCode = null;
            //Validate the Login user, and return the user details.
            ApplicationUser user = GetAspNetUserDetails(ref model, portalId);
            UserVerificationTypeEnum userVerificationTypeEnum;

            DataSet dataset = GetUserDetailsByAspNetUserId(user.Id, portalId);

            if (IsNull(dataset))
                return new UserModel();

            SetDataTableNames(dataset);
            ConvertDataTableToList dataTable = new ConvertDataTableToList();
            ZnodeUser znodeUser = dataTable.ConvertDataTable<ZnodeUser>(dataset.Tables[ZnodeConstant.ZnodeUser])?.FirstOrDefault();


            if (IsNotNull(znodeUser) && znodeUser.IsVerified)
            {
                CheckUserPortalAccess(model.PortalId.GetValueOrDefault(), dataset);

                //This method is used to login the User having valid username and password.
                SignInStatus result = SignInManager.PasswordSignIn(user.UserName, model.User.Password, model.User.RememberMe, true);

                if (Equals(result, SignInStatus.Success))
                {
                    //Check Password is expired or not.
                    ZnodeUserAccountBase.CheckLastPasswordChangeDate(user.Id, out errorCode);

                    model = MapUserModel(znodeUser, string.Empty, model, user, dataTable, dataset);

                    model = BindUserDetails(model, expand);

                    model = GetUsersAdditionalAttributes(model);

                    //Set Media Path from Media Id.
                    SetMediaPath(model);

                    return model;
                }
            }
            userVerificationTypeEnum = IsNotNull(znodeUser?.UserVerificationType) ? (UserVerificationTypeEnum)Enum.Parse(typeof(UserVerificationTypeEnum), znodeUser.UserVerificationType) : UserVerificationTypeEnum.None;
            //Throw the Exceptions for Invalid Login.
            InvalidLoginError(user, model.IsVerified, userVerificationTypeEnum);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }

        //Get paged user account list
        public virtual UserListModel GetUserList(int loggedUserAccountId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string columnList = "")
        {
            return GetUserList(loggedUserAccountId, null, null, expands, filters, sorts, page, columnList, ZnodeConstant.Znode_AdminUsersByUserId);
        }

        //Get paged user account list for admin.
        public virtual UserListModel GetUserListForAdmin(int loggedUserAccountId, string userName, string roleName, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string columnList = "")
        {
            return GetUserList(loggedUserAccountId, userName, roleName, expands, filters, sorts, page, columnList, "Znode_AdminUsers");
        }

        //Get paged user account list
        protected virtual UserListModel GetUserList(int loggedUserAccountId, string userName, string roleName, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string columnList = "", string spName = "")
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { loggedUserAccountId = loggedUserAccountId });
            int userId = 0;
            bool? isSalesRepList = false;
            bool isApproval, isWebstore, isCustomerEditMode, IsGuestUser;
            string currentUserName, currentRoleName;
            int portalId;
            string portalAccess = "";

            if (IsNotNull(filters?.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase)
                && x.FilterValue.Equals(FilterKeys.ActiveTrueValue, StringComparison.InvariantCultureIgnoreCase))))
            {
                portalAccess = GetAvailablePortals(loggedUserAccountId);
                ZnodeLogging.LogMessage("Logged in User has portalIds access : ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalAccess = portalAccess });
            }

            filters = GetValuesFromFilter(loggedUserAccountId, filters, out isApproval, out isWebstore, out currentUserName, out currentRoleName, out portalId, out isCustomerEditMode, out IsGuestUser, userName, roleName);

            if (portalId < 1)
                portalId = (_userPortalRepository.Table?.Where(x => x.UserId == loggedUserAccountId)?.Select(x => x.PortalId)?.FirstOrDefault()) ?? 0;

            if (filters.Any(x => x.FilterName == FilterKeys.UserId))
            {
                int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);
                IsGuestUser = (IsNotNull(userId) && userId > 0) ? IsNull(_userRepository.Table.FirstOrDefault(x => x.UserId == userId).AspNetUserId) : false;
            }

            if (filters.Any(x => string.Equals(x.FilterName , ZnodeConstant.IsSalesRepList,StringComparison.InvariantCultureIgnoreCase)))
            { 
                isSalesRepList = Convert.ToBoolean(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeConstant.IsSalesRepList, StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
            }

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.IsSalesRepList, StringComparison.InvariantCultureIgnoreCase));

            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Generate Datatable of selected Column
            DataTable columnNameTable = ConvertKeywordListToDataTable(columnList, "ColumnNameList");

            //Get Filter in XML format
            string filterXML = GenerateFilterXML(filters);
            string portalParameter = string.IsNullOrEmpty(portalAccess) ? Convert.ToString(portalId) : portalAccess;

            //get value for pagination
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<UserModel> objStoredProc = new ZnodeViewRepository<UserModel>();

            //SP parameters
            objStoredProc.SetParameter("@RoleName", currentRoleName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserName", currentUserName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@WhereClause", filterXML, ParameterDirection.Input, DbType.Xml);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsCallOnSite", isWebstore, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@PortalId", portalParameter, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@IsGuestUser", IsGuestUser, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetTableValueParameter("@ColumnName", columnNameTable, ParameterDirection.Input, SqlDbType.Structured, "dbo.SelectColumnList");
            objStoredProc.SetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);
            List<UserModel> accountList;
            if (string.Equals(spName,ZnodeConstant.Znode_AdminUsersByUserId))
            {
                objStoredProc.SetParameter("@IsSalesRepList", isSalesRepList, ParameterDirection.Input, DbType.Boolean);
                accountList = objStoredProc.ExecuteStoredProcedureList(" " + spName + "  @RoleName,@UserName,@WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@IsCallOnSite,@PortalId,@IsGuestUser,@ColumnName,@SalesRepUserId,@IsSalesRepList", 6, out pageListModel.TotalRowCount)?.ToList();
            }
            else
            {
                accountList = objStoredProc.ExecuteStoredProcedureList(" " + spName + "  @RoleName,@UserName,@WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@IsCallOnSite,@PortalId,@IsGuestUser,@ColumnName,@SalesRepUserId", 6, out pageListModel.TotalRowCount)?.ToList();
            }
            UserListModel list = new UserListModel();

            if (accountList?.Count > 0)
                list.Users = accountList;

            //Remove all the users from list who is having role 'AccountUser'.
            RemoveAcccountUsers(isApproval, list);

            //Sets the user's portal Id.
            SetAssignedPortal(list.Users.FirstOrDefault(), isCustomerEditMode);

            list.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return list;
        }

        //Convert filter into XML format
        public virtual string GenerateFilterXML(FilterCollection filters)
        {

            List<Tuple<string, string>> filterTuples = DynamicClauseHelper.GenerateDynamicFilterTupleForSP(filters.ToFilterDataCollection());

            if (filterTuples?.Count > 0)
                return FilterTupleToXMLString(filterTuples);

            return string.Empty;
        }

        public virtual UserListModel GetSalesRepListForAssociation(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            int userId = 0;

            if (filters.Any(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase)))
            {
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase));
            }

            //get value for pagination
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<UserModel> objStoredProc = new ZnodeViewRepository<UserModel>();

            //SP parameters
            objStoredProc.SetParameter("@RoleName", string.Empty, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);

            List<UserModel> accountList = objStoredProc.ExecuteStoredProcedureList("Znode_AdminUsersSalesRep @RoleName,@WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@UserId", 5, out pageListModel.TotalRowCount)?.ToList();
            UserListModel list = new UserListModel();

            if (accountList?.Count > 0)
                list.Users = accountList;

            list.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return list;

        }

        //Get the Account By Account Id
        public virtual UserModel GetUserById(int userId, NameValueCollection expands, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId });

            if (userId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.AccountIdNotLessThanOne);

            //This method is used to get the account details.
            ZnodeUser user = portalId > 0 ? _userRepository.Table.FirstOrDefault(x => x.UserId == userId && x.ZnodeUserPortals.Select(y => y.PortalId == portalId).FirstOrDefault()) :
                                            _userRepository.Table.Where(x => x.UserId == userId).Include(x => x.ZnodeUserPortals)?.FirstOrDefault();

            //If user is global level, get its details.
            if (portalId > 0 && IsNull(user))
            {
                user = _userRepository.Table.Where(x => x.UserId == userId).Include(x => x.ZnodeUserPortals)?.FirstOrDefault();

                if (IsNotNull(user) && user.ZnodeUserPortals?.FirstOrDefault()?.PortalId.GetValueOrDefault() > 0 && user.ZnodeUserPortals?.FirstOrDefault()?.PortalId != portalId)
                    return null;
            }

            //Check if user is guest user for gift card creation/ update.
            if (IsNotNull(expands) && expands[ZnodeConstant.IsGuestUserForGiftCard]?.Length > 0 && IsNull(user?.AspNetUserId))
                return null;

            if (IsNotNull(user))
            {
                ApplicationUser userDetails = UserManager.FindById(user.AspNetUserId);

                //Map ZnodeUser Entity to User Model.
                UserModel accountDetails = UserMap.ToModel(user, string.Empty);

                //To verify whether the user is a tradecentric user.
                if (IsNotNull(accountDetails))
                {
                    accountDetails.IsTradeCentricUser = new ZnodeRepository<ZnodeTradeCentricUser>().Table.Any(x => x.UserId.Equals(userId));
                }

                //In the case of anonymous user, skip below checks.
                if (IsNotNull(userDetails))
                {
                    //get the lock status of the user
                    var lockedStatus = _aspNetUserRepository.Table.FirstOrDefault(x => x.Id == userDetails.Id).LockoutEndDateUtc;
                    accountDetails.IsLock = IsNotNull(lockedStatus);

                    //Sets the portal ids.
                    SetPortal(accountDetails);

                    //Sets the customer details.
                    SetCustomerDetails(userDetails, accountDetails, user);
                }
                else
                {
                    accountDetails.IsGuestUser = true;
                }

                SetMediaPath(accountDetails);
                //For global level user
                if ((accountDetails?.PortalId < 0) || IsNull(accountDetails?.PortalId))
                    accountDetails.PortalId = portalId;

                return BindUserDetails(accountDetails, expands);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        // Returns SMSOptIn flag by user id.
        public virtual bool IsSMSOptIn(int userId)
        {
            return Convert.ToBoolean(_userRepository.Table.Where(x => x.UserId == userId)?.Select(x => x.SMSOptIn)?.FirstOrDefault());           
        }

        //Converts comma separated column name to data table
        protected virtual DataTable ConvertKeywordListToDataTable(string columnList, string columnName)
        {
            DataTable columnNameTable= new DataTable(columnName);

            columnNameTable.Columns.Add("ColumnName");

            if (!string.IsNullOrEmpty(columnList))
            {
                List<string> visibleColumnList = columnList.Split(',').Select(x => Convert.ToString(x)).ToList();

                foreach (string column in visibleColumnList)
                    columnNameTable.Rows.Add(column);
            }

            return columnNameTable;
        }

        //Get the User Account Details by its UserName
        public virtual UserModel GetUserByUsername(string username, int portalId, bool isSocialLogin = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { username = username, portalId = portalId });

            if (string.IsNullOrEmpty(username))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.UserNameNotEmpty);

            //Get the AspNet Znode user.
            AspNetZnodeUser aspnetZnodeUser = _loginHelper.GetUserInfo(username, portalId);

            if (IsNull(aspnetZnodeUser))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.UserNotPresent, username));

            ApplicationUser user = UserManager.FindByName(aspnetZnodeUser.AspNetZnodeUserId);

            if (IsNull(user))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.UserNotFound, username));

            //This method is used to get the account details.
            ZnodeUser znodeUser = _userRepository.GetEntity(FilterClauseForUserId(user).WhereClause, FilterClauseForUserId(user).FilterValues);

            //Map ZnodeUser Entity to User Model.
            UserModel model = UserMap.ToModel(znodeUser, user.Email);
            if (IsNotNull(model.User) && IsNotNull(user.Roles) && user.Roles.Count > 0)
                model.User.RoleId = user.Roles?.Select(s => s.RoleId)?.FirstOrDefault();

            //check whether the current login user has Customer user and B2B role or not.
            model.IsAdminUser = IsAdminUser(user);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(FilterKeys.Profiles, FilterKeys.Profiles);

            BindUserDetails(model, expands);

            //Bind user details required after login.
            model = BindDetails(model, user, expands);

            model.UserName = aspnetZnodeUser.UserName;

            SetMediaPath(model);

            //Throw the Exceptions for Invalid social Login.
            if (isSocialLogin && UserManager.IsLockedOut(user.Id))
            {
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, "Login failed", null);
                throw new ZnodeUnauthorizedException(ErrorCodes.AccountLocked, string.Empty, HttpStatusCode.Unauthorized);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }

        public virtual UserModel ChangePassword(int portalId, UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (IsNull(model.User))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.LoginUserModelNotNull);

            //Get the AspNet Znode user.
            model.User.Username = DecodeBase64(model.User.Username);
            AspNetZnodeUser znodeUser = _loginHelper.GetUser(model, (model.PortalId > 0) ? model.PortalId.Value : portalId);

            if (IsNull(znodeUser))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.UserNameUnavailable, model.UserName));

            //This method is used to find the User having valid username.
            ApplicationUser user = UserManager.FindByNameAsync(znodeUser.AspNetZnodeUserId).Result;

            if (IsNull(user))
                throw new ZnodeException(ErrorCodes.InvalidData, "User " + model.User.Username + " not found.");
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return PasswordVerification((model.PortalId > 0) ? model.PortalId.Value : portalId, model, user);
        }

        public virtual bool IsDefaultAdminPasswordReset()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeEncryption encryption = new ZnodeEncryption();
            string encryptedPassword = encryption.EncryptData(ZnodeConstant.DefaultPassword);
            string AspNetUserId = (from zu in _aspNetZnodeUserRepository.Table
                                   join user in _aspNetUserRepository.Table on zu.AspNetZnodeUserId equals user.UserName
                                   where zu.UserName == ZnodeConstant.DefaultAdminUser
                                   select user.Id).FirstOrDefault();
            ZnodeLogging.LogMessage("AspNetUserId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, AspNetUserId);

            //Check if admin is logging in for the first time, if yes then skip condition check for reset password popup.
            int? errorCode;
            ZnodeUserAccountBase.CheckLastPasswordChangeDate(AspNetUserId, out errorCode);
            if (errorCode == 2) return true;

            //Check if admin is still using the default password.
            string currentPassword = _passwordLogRepository.Table.Where(x => x.UserId == AspNetUserId).ToList()?.LastOrDefault()?.Password;
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return !string.Equals(currentPassword, encryptedPassword, StringComparison.InvariantCultureIgnoreCase);
        }

        //Bulk password reset functionality.
        public virtual bool BulkResetPassword(ParameterModel accountIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            bool isPasswordReset = true;
            StringBuilder failedUserNames = new StringBuilder();
            List<ZnodeUser> accountList = GetUsers(accountIds);
            ZnodeLogging.LogMessage("accountList list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, accountList?.Count());
            if (accountList?.Count > 0)
            {
                foreach (var account in accountList)
                {
                    try
                    {
                        if (IsNull(account))
                            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.UserModelNotNull);

                        ApplicationUser userDetails = UserManager.FindById(account.AspNetUserId);
                        if (IsNull(userDetails))
                            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ApplicationUserNotNull);
                        //This method is used to find the User having valid user name.
                        ApplicationUser user = UserManager.FindByNameAsync(userDetails.UserName).Result;

                        if (IsNull(user))
                            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.UserNotFound);

                        //This method is used to verify the user with password stored in database.
                        if (UserManager.IsLockedOut(user.Id))
                            throw new ZnodeException(ErrorCodes.AccountLocked, Admin_Resources.ErrorResetAccountPassword);

                        //This method is used to generate password reset token.
                        string passwordResetToken = UserManager.GeneratePasswordResetToken(user.Id);
                        //Reset Password Link in Email - Start

                        //Get the User name for Znode user.
                        string userName = account.FirstName;
                        if(string.IsNullOrEmpty(userName))
                        {
                            userName = _loginHelper.GetUserById(userDetails.UserName)?.UserName;
                        }
                        int portalId = _userPortalRepository.Table.Where(x => x.UserId == account.UserId)?.Select(x => x.PortalId).FirstOrDefault() ?? PortalId;
                        //Send the Reset Password Email
                        SendResetPasswordMail(userDetails, passwordResetToken, userName, portalId);
                    }
                    catch (ZnodeException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        isPasswordReset = false;
                        failedUserNames.Append($"{account.MiddleName} ");
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                    }
                }
                if (!string.IsNullOrEmpty(Convert.ToString(failedUserNames)))
                    throw new ZnodeException(ErrorCodes.ErrorResetPassword, Convert.ToString(failedUserNames));
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return isPasswordReset;
        }

        //Single reset password functionality.
        public virtual UserModel ForgotPassword(int portalId, UserModel model, bool isUserCreateFromAdmin = false, bool isAdminUser = false)
        {
            ZnodeLogging.LogMessage("Execution Started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (IsNull(model.User))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.LoginUserModelNotNull);

            portalId = (model.PortalId > 0) ? model.PortalId.Value : PortalId;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Get the AspNet Znode user.
            AspNetZnodeUser znodeUser = _loginHelper.GetUser(model, portalId);

            if (IsNull(znodeUser))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.UserNameUnavailable, model.User));

            //This method is used to find the User having valid username.
            ApplicationUser user = UserManager.FindByNameAsync(znodeUser.AspNetZnodeUserId).Result;

            if (IsNull(user))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.UserNotFound, model.User));


            ZnodeUser userStatusCode = _userRepository.Table?.FirstOrDefault(x => x.AspNetUserId == user.Id);

            if (!userStatusCode.IsVerified && IsNotNull(userStatusCode?.UserVerificationType)
                && IsAdminApprovalType((UserVerificationTypeEnum)Enum.Parse(typeof(UserVerificationTypeEnum), userStatusCode.UserVerificationType)))
                throw new ZnodeException(ErrorCodes.AdminApprovalLoginFail, WebStore_Resources.LoginFailAdminApproval);

            //This method is used to verify the userwith password stored in database.
            if (UserManager.IsLockedOut(user.Id))
                throw new ZnodeException(ErrorCodes.AccountLocked, WebStore_Resources.AccountDisableErrorMessage);

            if (portalId < 1)
            {
                // Get the portal id.
                SetAssignedPortal(model, true);
                if (model.PortalId < 1)
                    model.PortalId = PortalId;
            }
            else
                model.PortalId = portalId;

            //Reset Password Link in Email - Start
            model.User.PasswordToken = SendResetPassword(model, user, isUserCreateFromAdmin, isAdminUser);
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //Verify reset password link status.
        public virtual int? VerifyResetPasswordLinkStatus(int portalId, UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);
            //Get the AspNet Znode user.
            AspNetZnodeUser aspnetZnodeUser = _loginHelper.GetUserInfo(DecodeBase64(model.User.Username), (model.PortalId > 0) ? model.PortalId.Value : portalId);

            if (IsNull(aspnetZnodeUser))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ProvideValidUserName);

            int? statusCode = null;

            //Get the application user.
            ApplicationUser user = UserManager.FindByName(aspnetZnodeUser.AspNetZnodeUserId);
            if (IsNotNull(user))
                //This method is used to verify the reset password token is valid or not.
                statusCode = UserManager.VerifyUserToken(user.Id, "ResetPassword", model.User.PasswordToken) ? ErrorCodes.ResetPasswordContinue : ErrorCodes.ResetPasswordLinkExpired;
            ZnodeLogging.LogMessage("Output Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { statusCode = statusCode });

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return statusCode;
        }

        //Method to lock or unlock a user account.
        //(accountIds of users and lockUser specifies if user account has to be locked or not).
        public virtual bool EnableDisableUser(ParameterModel userIds, bool lockUser)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            bool isAllowedToLock = false;
            List<ZnodeUser> list = GetUsers(userIds);
            if (list?.Count > 0)
            {
                ZnodeLogging.LogMessage("ZnodeUser list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, list?.Count());
                int failedOperations = 0;
                foreach (ZnodeUser accountDetails in list)
                {
                    bool isError = EnableDisableUser(accountDetails, lockUser, out isAllowedToLock);

                    if (isError)
                        failedOperations++;
                }

                if (failedOperations > 0)
                {
                    throw new ZnodeException(ErrorCodes.LockOutEnabled, Admin_Resources.LockedOutDisableForAccount);
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return isAllowedToLock;
        }

        //Create New Admin User.
        public virtual UserModel CreateAdminUser(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("AccountIds:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.AccountId);
            //when an user is created from admin it should always be verified. 
            model.IsVerified = true;
            //Save the Login Details for the user.
            ApplicationUser user = CreateOwinUser(model);
            bool isUserCreateFromAdmin = false;
            if (IsNotNull(user))
            {
                //Checks if role name is null, then create normal user, else create admin user.
                if (!string.IsNullOrEmpty(model.RoleName))
                    UserManager.AddToRole(user.Id, model.RoleName);

                //Save account details.
                ZnodeUser account = _userRepository.Insert(model.ToEntity<ZnodeUser>());

                if (IsNotNull(account))
                {
                    ZnodeLogging.LogMessage(account?.AccountId > 0 ? Admin_Resources.SuccessUserDataSave : Admin_Resources.ErrorUserDataSave, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                    //Insert Portal Mapping for the Admin user.
                    InsertUpdateUserPortals(String.Join(",", model.PortalIds), account.UserId);

                    //Insert profile in AccountProfile table.
                    _accountProfileRepository.Insert(new ZnodeUserProfile { UserId = account.UserId, ProfileId = null, IsDefault = true });
                }

                // Get the portal id.
                SetAssignedPortal(model, false);
                if (!model.IsWebStoreUser)
                {
                    isUserCreateFromAdmin = true;
                    ForgotPassword((model.PortalId > 0) ? model.PortalId.Value : PortalId, model, isUserCreateFromAdmin, true/*flag for admin user*/);
                }

                //map account model to account entity.
                if (!SendEmailToStoreAdministrator(model.FirstName, model.User.Password, account.Email, (model.PortalId > 0) ? model.PortalId.Value : PortalId, model.LocaleId, false, model.IsTradeCentricUser, model.IsWebStoreUser, isUserCreateFromAdmin))
                {
                    UserModel accountModel = UserMap.ToModel(account, user.Email, model.UserName);
                    accountModel.IsEmailSentFailed = true;
                    return accountModel;
                }
                return UserMap.ToModel(account, user.Email, model.UserName);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new UserModel();
        }

        //Update user account data.
        public virtual bool UpdateUserData(UserModel userModel, bool webStoreUser)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("AccountIds:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userModel.AccountId);

            if (IsNull(userModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (userModel.UserId > 0)
            {
                //Update roles in AspNetUserRoles table.
                List<string> roles = UserManager.GetRoles(userModel.AspNetUserId).ToList();
                ZnodeLogging.LogMessage("roles list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roles?.Count());
                if (!webStoreUser)
                {
                    if (IsNotNull(roles))
                    {
                        if (!Equals(roles.FirstOrDefault(), userModel.RoleName))
                        {
                            UserManager.RemoveFromRoles(userModel.AspNetUserId, roles.ToArray());
                            UserManager.AddToRole(userModel.AspNetUserId, userModel.RoleName);
                            ZnodeLogging.LogMessage(Admin_Resources.SuccessUpdateRole, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                        }
                    }
                    else
                    {
                        UserManager.AddToRole(userModel.AspNetUserId, userModel.RoleName);
                        ZnodeLogging.LogMessage(Admin_Resources.SuccessUpdateRole, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    }
                }

                bool updatedAccount = UpdateUser(userModel, webStoreUser);

                ZnodeLogging.LogMessage(updatedAccount ? Admin_Resources.SuccessUpdateData : Admin_Resources.ErrorUpdateData, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Get user details by UserId.
                ApplicationUser user = UserManager.FindById(userModel.AspNetUserId);
                if (!webStoreUser)
                    InsertUpdateUserPortals(String.Join(",", userModel.PortalIds), userModel.UserId);

                if(string.Equals(userModel.RoleName, ZnodeConstant.SalesRepRole, StringComparison.InvariantCultureIgnoreCase))                
                {
                    UnassociateSalesRepWithAccount(userModel);
                }                

                //Update email in AspNetUsers table.
                if (!Equals(user?.Email, userModel.Email))
                    user.Email = userModel.Email;

                if (updatedAccount)
                {
                    IdentityResult applicationUserManager = UserManager.Update(user);
                    ZnodeLogging.LogMessage(applicationUserManager.Succeeded ? Admin_Resources.SuccessUpdateAccount : Admin_Resources.ErrorUpdateAccount, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }

        //Unassociate SalesRep with Account
        protected virtual void UnassociateSalesRepWithAccount(UserModel userModel)
        {   
            if(userModel?.PortalIds?.Length > 0)
            {
                List<int> portalIds = userModel.PortalIds.Select(x => int.Parse(x)).ToList();
                IZnodeRepository<ZnodeAccount> accountRepository = new ZnodeRepository<ZnodeAccount>();
                IZnodeRepository<ZnodePortalAccount> portalAccountRepository = new ZnodeRepository<ZnodePortalAccount>();
                List<int> accountIds = (from a in accountRepository.Table
                                        join pa in portalAccountRepository.Table on a.AccountId equals pa.AccountId
                                        where a.SalesRepId == userModel.UserId && !portalIds.Contains(pa.PortalId ?? 0)
                                        select a.AccountId)?.ToList();

                if (accountIds?.Count > 0)
                {
                    List<ZnodeAccount> znodeAccounts = accountRepository.Table.Where(m => accountIds.Contains(m.AccountId))?.ToList();
                    if (znodeAccounts?.Count > 0)
                    {
                        znodeAccounts.ForEach(m => { m.SalesRepId = null; });
                        accountRepository.BatchUpdate(znodeAccounts);
                    }
                }
            }            
        }

        public virtual bool DeleteUser(ParameterModel accountIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("AccountIds:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, accountIds.Ids);

            if (Equals(accountIds, null) || string.IsNullOrEmpty(accountIds.Ids))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserIdNotLessThanOne);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("UserId", accountIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteUserDetails @UserId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessUserDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteAccount, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteSomeRecord);
            }
        }

        //Create New Customer User.
        public virtual UserModel CreateCustomer(int portalId, UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId, AccountId = model.AccountId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            //Set the email as an username for external login.
            if (model.IsSocialLogin)
                SetUserNameForSocialLoginUser(model);

            //Create guest user account.
            if (model.IsGuestUser)
                return CreateGuestUserAccount(model);

            model.PortalId = DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation ? (int?)null : model.PortalId;

            //Save the user login Details.
            ApplicationUser user = CreateOwinUser(model);
            if (IsNotNull(user))
            {
                //Save the user Account, Profile Details 
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                UserModel userdataModel = InsertUserData(model, user, model.User.Password);

                //Avoid log password for user created from admin site,For forcing user to reset the password for the first time login.
                SetRequestHeaderForPasswordLog(null, userdataModel.UserId);
                ZnodeUserAccountBase.LogPassword(Convert.ToString(userdataModel.AspNetUserId), model.User.Password);
                //Save recently password change date.
                ZnodeUserAccountBase.SetPasswordChangedDate(userdataModel.AspNetUserId);
                return userdataModel;

            }
            else
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.LoginInalreadyExist);
        }

        public virtual bool UpdateCustomer(UserModel userModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = userModel?.UserId });

            if (IsNull(userModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (userModel.UserId > 0)
            {
                //Check whether the Registered default profile present for the portal.
                CheckRegisterProfile(userModel);

                //Todo: Need to replace Hardcoded code.
                if (Equals(userModel.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString()) || (Equals(userModel.PermissionCode, ZnodePermissionCodeEnum.ARA.ToString())) || (Equals(userModel.RoleName, ZnodeRoleEnum.Administrator.ToString())) || (Equals(userModel.RoleName, ZnodeRoleEnum.Manager.ToString())))
                    userModel.BudgetAmount = 0;

                int? userAccountId = null;

                // Update account details in account table.
                if (!string.IsNullOrEmpty(userModel.AspNetUserId))
                {
                    var userDetails = (from userTable in _userRepository.Table
                                       where userTable.AspNetUserId == userModel.AspNetUserId
                                       select new { userTable.IsVerified, userTable.AccountId }).FirstOrDefault();

                    userModel.IsVerified = (userDetails?.IsVerified).GetValueOrDefault();
                    userAccountId = userDetails?.AccountId;

                    bool updateStatus = _userRepository.Update(userModel.ToEntity<ZnodeUser>());

                    ZnodeLogging.LogMessage(updateStatus ? Admin_Resources.SuccessUpdateData : Admin_Resources.ErrorUpdateData, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                }
                else
                {
                    //Method to update guest user account.
                    UpdateGuestUserAccount(userModel);
                    if (userModel.IsEmailSentFailed) { return false; } else { return true; }
                }

                //Get user details by UserId.
                ApplicationUser user = UserManager.FindById(userModel.AspNetUserId);

                //Updates into AspnetUserRole
                UpdateUserRole(userModel, user);

                //Update email in AspNetUsers table.
                if (!Equals(user?.Email, userModel.Email))
                    user.Email = userModel.Email;

                if (userModel.AccountId > 0)
                {
                    InsertIntoUserDepartment(userModel, true);

                    //Update profile on converting normal user to B2B user.
                    if (IsNull(userAccountId) || userAccountId <= 0)
                        UpdateProfileForAccountAssociatedUser(userModel);

                    //Save the Account user permissions.
                    SaveAccountUserPermission(userModel);

                    //Get the portal id of account.
                    userModel.PortalId = GetAccountPortalId(userModel.AccountId.GetValueOrDefault());
                }

                InsertIntoUserPortal(userModel.PortalId, userModel.UserId, userModel.SalesRepId);
                IdentityResult applicationUserManager = UserManager.Update(user);

                ZnodeLogging.LogMessage(applicationUserManager.Succeeded ? Admin_Resources.SuccessUpdateAccount : Admin_Resources.ErrorUpdateAccount, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return applicationUserManager.Succeeded;
            }
            return false;
        }

        //Gets the assigned portals to user.
        public virtual UserPortalModel GetPortalIds(string aspNetUserId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { aspNetUserId = aspNetUserId });

            if (string.IsNullOrEmpty(aspNetUserId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.AspNetUserIdNotNull);

            IList<ZnodeUserPortal> userPortals = (from userPortal in _userPortalRepository.Table
                                                  join user in _userRepository.Table on userPortal.UserId equals user.UserId
                                                  where user.AspNetUserId == aspNetUserId
                                                  select userPortal).ToList();
            ZnodeLogging.LogMessage("userPortals list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userPortals?.Count());

            if (userPortals?.Count > 0)
            {
                IPortalService _portalService = GetService<IPortalService>();
                return new UserPortalModel { AspNetUserId = aspNetUserId, PortalIds = IsNull(userPortals.Select(x => x.PortalId).FirstOrDefault()) ? _portalService.GetPortalList(null, null, null, null)?.PortalList?.Select(x => x.PortalId.ToString())?.ToArray() : userPortals.Select(x => x.PortalId.ToString())?.ToArray() };
            }
            return null;
        }

        //Save portal ids against the user.
        public virtual bool SavePortalsIds(UserPortalModel userPortalModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(userPortalModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            return InsertUpdateUserPortals(String.Join(",", userPortalModel.PortalIds), userPortalModel.UserId);
        }

        //Get the Customer Profiles based on the Portal & User Id.
        public virtual List<ProfileModel> GetCustomerProfile(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId });

            if (userId <= 0)
                return new List<ProfileModel> { GetDefaultAnonymousProfile(portalId) };

            //Get Customer profile,associated to the user.
            return GetCustomerProfileByUserId(userId);
        }

        //Sign up for news letter.
        public virtual bool SignUpForNewsLetter(NewsLetterSignUpModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(model?.Email))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.EmailNotNull);

            //Check if user already subscribed for news letter.
            if (!IsUserAlreadyExist(model.Email.Trim()))
            {
                ZnodeUser insertUser = InsertIntoZnodeUser(model);
                if (insertUser?.UserId > 0)
                {
                    //Insert portal in UserPortal table.
                    _userPortalRepository.Insert(new ZnodeUserPortal { UserId = insertUser.UserId, PortalId = PortalId });

                    ZnodeAddress insertAddress = InsertIntoAddress();
                    if (insertAddress?.AddressId > 0)
                        InsertIntoZnodeUserAddress(insertUser, insertAddress);
                    InsertIntoUserProfile(insertUser);
                    ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            else
                throw new ZnodeException(ErrorCodes.UserNameUnavailable, Admin_Resources.EmailAddressAlreadyExists);
            return false;
        }

        //Get unassociated Customer(s).
        public virtual UserListModel GetUnAssociatedCustomerList(int portalId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //get value for pagination
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<UserModel> objStoredProc = new ZnodeViewRepository<UserModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            List<UserModel> accountList = objStoredProc.ExecuteStoredProcedureList("Znode_AdminUnassociatedUsers  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId", 4, out pageListModel.TotalRowCount)?.ToList();

            UserListModel list = new UserListModel();

            if (accountList?.Count > 0)
                list.Users = accountList;

            list.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return list;
        }

        //Update user account mapping
        public virtual bool UpdateUserAccountMapping(UserAccountModel userModel)
        {
            bool isUserAccountAssociatedSuccessfully = false;
            if(IsNull(userModel))
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.UserModelNotNull);
            }
            else if (!string.IsNullOrEmpty(userModel.UserIds) && userModel.AccountId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.In, userModel.UserIds));

                //Insert the access permission for user.
                bool isInsertUserPermission = InsertAccessPermissionForUsers(userModel, filters);

                List<ZnodeUser> userList = _userRepository.Table.Where(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()))?.ToList();
                if(userList?.Count > 0)
                {
                    userList.ForEach(user =>
                    {
                        user.AccountId = userModel.AccountId;
                    });
                    isUserAccountAssociatedSuccessfully = _userRepository.BatchUpdate(userList) && isInsertUserPermission;
                }
                if (isUserAccountAssociatedSuccessfully)
                {
                    bool isUserProfileAssociatedSuccessfully = UpdateUserProfileBasedOnAccountProfile(userModel.AccountId, userModel.UserIds);
                    ZnodeLogging.LogMessage((isUserProfileAssociatedSuccessfully) ? Api_Resources.SuccessAssociateAccountProfileToUserProfile : Api_Resources.FailedAssociateAccountProfileToUserProfile, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                }
            }
            return isUserAccountAssociatedSuccessfully;
        }

        // Updating User Profile same as Account Profile.
        protected virtual bool UpdateUserProfileBasedOnAccountProfile(int accountId, string userIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("AccountId", accountId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("UserIds", userIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> updateUserProfileResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateUserProfileBasedOnAccountProfile @AccountId,@UserIds,@Status OUT", 2, out status);
            return updateUserProfileResult.FirstOrDefault().Status.Value;
        }
        
        //Insert the access permission for user.
        public virtual bool InsertAccessPermissionForUsers(UserAccountModel model, FilterCollection filters)
        {
            int[] exceptUserIds = _accountUserPermissionRepository.Table.Where(DynamicClauseHelper.GenerateDynamicWhereClause(filters?.ToFilterDataCollection())).Select(m => m.UserId ?? 0 )?.ToArray();
            IEnumerable<int> selectedUserIds = model.UserIds?.Split(',')?.Select(int.Parse);
            List<int> insertUserIds = selectedUserIds?.Except(exceptUserIds)?.ToList();
            int accountPermissionAccessId = GetDNRAAccountPermissionAccessId().GetValueOrDefault();
            List<ZnodeAccountUserPermission> insertAccountUser = new List<ZnodeAccountUserPermission>();
            insertUserIds?.ForEach(usrId =>
            {
                insertAccountUser.Add(new ZnodeAccountUserPermission { UserId = usrId, AccountPermissionAccessId = accountPermissionAccessId });
            });

            if(insertAccountUser?.Count > 0)
            {
                return _accountUserPermissionRepository.Insert(insertAccountUser)?.Count() > 0;
            }
            else
            {
                return true;
            }
        }

        //Get sales rep list for the portal.
        public virtual SalesRepUserListModel GetSalesRepListForAccount(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            int portalId = 0;

            if (filters.Any(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase)))
            {
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
            }

            //get value for pagination
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<SalesRepUserModel> objStoredProc = new ZnodeViewRepository<SalesRepUserModel>();

            //SP parameters
            objStoredProc.SetParameter("@RoleName", string.Empty, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            List<SalesRepUserModel> accountList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSalesRepUsersByPortal @RoleName,@WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId", 5, out pageListModel.TotalRowCount)?.ToList();
            SalesRepUserListModel list = new SalesRepUserListModel();

            if (accountList?.Count > 0)
                list.SalesRepUsers = accountList;

            list.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return list;

        }

        //Clear all user register attempt details.
        public virtual bool ClearAllUserRegisterAttempts()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            int Status = 0;
            //SP parameters
            objStoredProc.SetParameter("@Status", 1, ParameterDirection.Output, DbType.Int32);

            objStoredProc.ExecuteStoredProcedureList("Znode_RemoveUserRegistrationAttemptDetails @Status OUT", 0, out Status);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return (Status == 1);
        }

        #region Social Login
        //Login the user from social login.
        public virtual UserModel SocialLogin(SocialLoginModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(model) || IsNull(model.LoginInfo))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            //Throw Validation for Invalid Login if user is not verified.
            if (!IsSocialUserVerified(model.LoginInfo.Email, model.UserVerificationTypeCode))
            {
                ZnodeLogging.LogMessage(Admin_Resources.LogInFailed, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AdminApprovalLoginFail, Admin_Resources.LogInFailed, HttpStatusCode.Unauthorized);
            }

            SignInStatus result = SignInManager.ExternalSignIn(model.LoginInfo, model.IsPersistent);

            //Register user in application.
            if (Equals(SignInStatus.Success, result))
            {
                //Get the AspNet Znode user.
                AspNetZnodeUser aspnetZnodeUser = _loginHelper.GetUserInfo(model.LoginInfo.Email, model.PortalId);

                return IsNull(aspnetZnodeUser) ? GetUserByUsername(model.UserName, model.PortalId, true) : GetUserByUsername(model.LoginInfo.Email, model.PortalId, true);
            }
            else if (Equals(SignInStatus.Failure, result))
                throw new ZnodeException(ErrorCodes.AtLeastSelectOne, SignInStatus.Failure.ToString());
            else if (Equals(SignInStatus.LockedOut, result))
                throw new ZnodeException(ErrorCodes.AccountLocked, SignInStatus.Failure.ToString());
            else
                throw new ZnodeException(ErrorCodes.LoginFailed, string.Format(Admin_Resources.LogInFail, model.UserName));
        }

        //Get the login providers.
        public virtual List<SocialDomainModel> GetLoginProviders()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            IZnodeRepository<ZnodePortalLoginProvider> _portalLoginProvider = new ZnodeRepository<ZnodePortalLoginProvider>();

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodePortalLoginProviderEnum.ZnodeLoginProvider.ToString(), ZnodePortalLoginProviderEnum.ZnodeLoginProvider.ToString());
            expands.Add(ZnodePortalLoginProviderEnum.ZnodeDomain.ToString(), ZnodePortalLoginProviderEnum.ZnodeDomain.ToString());

            //Get all the login providers.
            List<ZnodePortalLoginProvider> allConfiguration = _portalLoginProvider.GetEntityList(string.Empty, GetSocialLoginExpands(expands)).ToList();
            ZnodeLogging.LogMessage("allConfiguration list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, allConfiguration?.Count());

            if (allConfiguration?.Count > 0)
            {
                //Get the login providers domain wise.
                List<ZnodePortalLoginProvider> domainWiseProvider = allConfiguration.GroupBy(x => x.DomainId).Select(x => x.First()).ToList();
                if (domainWiseProvider?.Count > 0)
                    //Map the login provider data domain wise.
                    return MapLoginProvider(allConfiguration, domainWiseProvider);
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return null;
        }
        #endregion
        
        #endregion

        #region Protected Methods

        //Update profile on converting normal user to B2B user.
        protected virtual void UpdateProfileForAccountAssociatedUser(UserModel userModel)
        {
            //Delete all existing profiles of the user.
            DeleteExistingProfiles(userModel);

            //Get profiles of associated account.
            List<ZnodeAccountProfile> accountProfiles = _accountAssociatedProfileRepository.Table.Where(x => x.AccountId == userModel.AccountId).ToList();
            ZnodeLogging.LogMessage("accountProfiles list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, accountProfiles?.Count());
            if (accountProfiles?.Count > 0)
            {
                //Insert all the profiles of account associated to user.
                List<ZnodeUserProfile> entriesToInsert = new List<ZnodeUserProfile>();
                foreach (var item in accountProfiles)
                    entriesToInsert.Add(new ZnodeUserProfile() { UserId = userModel.UserId, ProfileId = Convert.ToInt32(item.ProfileId), IsDefault = item.IsDefault });

                ZnodeLogging.LogMessage("entriesToInsert list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, entriesToInsert?.Count());

                //Insert user profiles.
                ZnodeLogging.LogMessage(IsNotNull(_userProfileRepository.Insert(entriesToInsert)) ? Admin_Resources.SuccessUserProfileInsert : Admin_Resources.ErrorUserProfileInsert, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
        }

        //Delete all existing profiles of the user.
        protected virtual void DeleteExistingProfiles(UserModel userModel)
        {
            //Create tuple to generate where clause.  
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userModel?.UserId.ToString()));

            //Delete existing profiles of the user.
            ZnodeLogging.LogMessage(_userProfileRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause) ? Admin_Resources.SuccessUserProfileDelete : Admin_Resources.ErrorUserProfileDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        protected virtual bool SendEmailToStoreAdministrator(string firstName, string newPassword, string email, int portalId, int localeId, bool isCustomerUser = false, bool isWebstoreUser = false, bool isUserCreateFromAdmin = false, bool isTradeCentricUser = false, string userId = null, string userName = null)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId, email = email });

            string emailTemplateCode = isWebstoreUser ? ZnodeConstant.NewCustomerAccountFromWebstore : ZnodeConstant.NewCustomerAccountFromAdmin;
            //Method to get Email Template Details by Code.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(emailTemplateCode, portalId, localeId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = emailTemplateMapperModel?.Subject;
                string senderEmail, messageText;
                //Generate Email Message Content Based on the Email Template.
                GenerateActivationEmail(firstName, newPassword, emailTemplateMapperModel.Descriptions, portalId, isCustomerUser, out senderEmail, ref subject, out messageText, isWebstoreUser, userId, userName);
                try
                {
                    //We can set email to bcc user on the basis of emailTemplateMapperModel.IsEnableBcc flag
                    if (!isUserCreateFromAdmin && !isTradeCentricUser)
                        ZnodeEmail.SendEmail(portalId, email, senderEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), subject, messageText, true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, firstName, string.Empty, null, ex.Message, null);
                    return false;
                }
                return true;
            }
            return false;
        }

        //Create the New User. Mapped Entries in Znode User & Owin User.
        protected virtual ApplicationUser CreateOwinUser(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model) && IsNotNull(model.User))
            {
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { User = model?.User, AccountId = model?.AccountId });

                //If the user belongs to account get the portal id of the account.
                if (model.AccountId > 0)
                    model.PortalId = GetAccountPortalId(model.AccountId.GetValueOrDefault());

                //Check whether the Registered default profile present for the portal.
                CheckRegisterProfile(model);

                model.Email = string.IsNullOrEmpty(model.User.Email) ? model.Email : model.User.Email;
                if (model.User.Username?.Any() ?? false)
                {
                    //Check if the user name already exists or not.
                    if (_loginHelper.IsUserExists(model.User.Username, model.PortalId.GetValueOrDefault()))
                    {
                        try
                        {
                            //sending the change password email if user is already exist.
                            if (model.IsWebStoreUser)
                            {

                                model.PortalId = (model.PortalId > 0) ? model.PortalId.Value : PortalId;
                                int? registeredUserAttemptId = GetUserRegisteredAttemptId(model.User.Username, model.PortalId.GetValueOrDefault());
                                if (registeredUserAttemptId == null || registeredUserAttemptId == 0)
                                {

                                    ZnodePortal portal = _portalRepository.GetById(model.PortalId.GetValueOrDefault());
                                    model.IsInvalidCredential = true;
                                    model.StoreName = portal?.StoreName;
                                    model.StoreCode = portal?.StoreCode;
                                    ForgotPassword(portalId: Convert.ToInt32(model.PortalId), model: model, isUserCreateFromAdmin: false, isAdminUser: false);
                                    CreateUserRegisteredAttempt(model);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                        }
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.LoginNamesAlreadyExist);
                    }

                    //Create the User in Znode Mapper to allow portal level user creation.
                    AspNetZnodeUser znodeUser = _loginHelper.CreateUser(model);
                    if (IsNotNull(znodeUser) && !string.IsNullOrEmpty(znodeUser.AspNetZnodeUserId))
                    {
                        string znodeUserName = znodeUser.AspNetZnodeUserId;
                        ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { znodeUserName = znodeUserName });

                        ApplicationUser user = new ApplicationUser { UserName = znodeUserName, Email = model.User.Email };

                        //Get the Password for the user.
                        string password = GetPassword(model);
                        IdentityResult result = UserManager.Create(user, password);
                        if (result.Succeeded)
                        {
                            user = UserManager.FindByName(znodeUserName);

                            //Save recently password change date.
                            ZnodeUserAccountBase.SetPasswordChangedDate(user.Id);
                            model.User.UserId = user.Id;
                            model.AspNetUserId = user.Id;
                            model.User.Password = password;
                            if (IsNotNull(model.ExternalLoginInfo?.Login))
                            {
                                result = UserManager.AddLogin(user.Id, model.ExternalLoginInfo.Login);
                                if (result.Succeeded)
                                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                            }
                            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                            return user;
                        }
                    }
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.LoginNamesAlreadyExist);
                }
            }
            return null;
        }

        //Check for available Registered profile for the current portal.
        protected virtual void CheckRegisterProfile(UserModel model)
        {
            ZnodeLogging.LogMessage("Profile Id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.ProfileId);
            if (model.ProfileId <= 0)
            {
                int portalId = model.PortalId > 0 ? model.PortalId.Value : PortalId;

                //Check the Registered Profile for the Portal.
                int profileId = GetCurrentProfileId(portalId);
                if (profileId <= 0)
                    throw new ZnodeException(ErrorCodes.ProfileNotPresent, Admin_Resources.ErrorUserAccountDeleteConfigureDefaultProfile);
                model.ProfileId = profileId;
            }
        }

        //Generate the Unique Password for the new user.
        protected virtual string GenerateNewPassword()
          => GetUniqueKey(8);

        protected virtual void InvalidLoginError(ApplicationUser user , bool isVerified , UserVerificationTypeEnum UserVerificationTypeCode)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, user?.Id);
            if (!Equals(user, null))
            {
                if (!isVerified && UserVerificationTypeCode != UserVerificationTypeEnum.None)
                {
                    if (UserVerificationTypeEnum.AdminApprovalCode == UserVerificationTypeCode)
                    {
                        ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.LogInFailed, null);
                        throw new ZnodeUnauthorizedException(ErrorCodes.AdminApproval, string.Empty, HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.LogInFailed, null);
                        throw new ZnodeUnauthorizedException(ErrorCodes.LoginFailed, string.Empty, HttpStatusCode.Unauthorized);
                    }

                }
                else if (UserManager.IsLockedOut(user.Id))
                {
                    ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, "Login failed", null);
                    throw new ZnodeUnauthorizedException(ErrorCodes.AccountLocked, WebStore_Resources.AccountLockedErrorMessage, HttpStatusCode.Unauthorized);
                }
                else
                {
                    //Gets current failed password attempt count for setting the message.
                    int inValidAttemtCount = user.AccessFailedCount;

                    //Gets Maximum failed password attempt count from web.config
                    int maxInvalidPasswordAttemptCount = UserManager.MaxFailedAccessAttemptsBeforeLockout;
                    ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { inValidAttemtCount = inValidAttemtCount, maxInvalidPasswordAttemptCount = maxInvalidPasswordAttemptCount });

                    if (inValidAttemtCount > 0 && maxInvalidPasswordAttemptCount > 0)
                    {
                        if (maxInvalidPasswordAttemptCount <= inValidAttemtCount)
                        {
                            ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorLogin, null);
                            throw new ZnodeUnauthorizedException(ErrorCodes.AccountLocked, string.Empty, HttpStatusCode.Unauthorized);
                        }

                        //Set warning error at (MaxFailureAttemptCount - 2) & (MaxFailureAttemptCount - 1) attempt.
                        if (Equals((maxInvalidPasswordAttemptCount - inValidAttemtCount), 2))
                        {
                            ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorLogin, null);
                            throw new ZnodeUnauthorizedException(ErrorCodes.TwoAttemptsToAccountLocked, string.Empty, HttpStatusCode.Unauthorized);
                        }
                        else if (Equals((maxInvalidPasswordAttemptCount - inValidAttemtCount), 1))
                        {
                            ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorLogin, null);
                            throw new ZnodeUnauthorizedException(ErrorCodes.OneAttemptToAccountLocked, string.Empty, HttpStatusCode.Unauthorized);
                        }
                        else
                        {
                            ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorUsernameAndPassword, null);
                            throw new ZnodeUnauthorizedException(ErrorCodes.LoginFailed, string.Empty, HttpStatusCode.Unauthorized);
                        }
                    }
                    else
                    {
                        ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorUsernameAndPassword, null);
                        throw new ZnodeUnauthorizedException(ErrorCodes.LoginFailed, string.Empty, HttpStatusCode.Unauthorized);
                    }
                }
            }
            else
            {
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.ErrorUsernameAndPassword, null);
                throw new ZnodeUnauthorizedException(ErrorCodes.LoginFailed, string.Empty, HttpStatusCode.Unauthorized);
            }
        }

        //This method will send account activation email to relevant user.        
        protected virtual void SendAccountActivationEmail(string firstName, string email, int portalId, bool isLock)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { firstName = firstName, email = email, portalId = portalId });

            EmailTemplateMapperModel emailTemplateMapperModel = !isLock ? GetEmailTemplateByCode(ZnodeConstant.CustomerAccountActivation, portalId)
                                                                        : GetEmailTemplateByCode(ZnodeConstant.AccountDeActivation, portalId);
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            if (IsNotNull(emailTemplateMapperModel))
            {
                firstName = string.IsNullOrEmpty(firstName) ? email : firstName;
                
                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText("#BillingFirstName#", firstName, messageText);
                messageText = ReplaceTokenWithMessageText("#StoreLogo#", portalModel?.StoreLogo, messageText);

                //These method is used to set null to unwanted keys.
                messageText = SetNullToUnWantedKeys(messageText);
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { messageText = messageText });

                string bcc = string.Empty;
                try
                {
                    if (IsNotNull(portalModel) && portalId > 0)
                        ZnodeEmail.SendEmail(portalId, email, portalModel?.AdministratorEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, bcc), $"{portalModel?.StoreName} - {emailTemplateMapperModel?.Subject}", messageText, true);
                    else
                        ZnodeEmail.SendEmail(email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, bcc), $"{ZnodeConfigManager.SiteConfig.StoreName} - {emailTemplateMapperModel?.Subject}", messageText, true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, firstName, string.Empty, null, ex.Message, null);
                }
            }
        }


        public virtual ZnodePortal GetPortalDetails(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            IZnodeRepository<ZnodePortal> _znodePortalRepository = new ZnodeRepository<ZnodePortal>();
            return _znodePortalRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
        }

        protected virtual string GetPassword(UserModel model)
            => IsNull(model.User) ? GenerateNewPassword() : (string.IsNullOrEmpty(model.User.Password)) ? GenerateNewPassword() : model.User.Password;

        //Set passwordResetUrl and send mail.
        protected virtual void SendResetPasswordMail(ApplicationUser userDetails, string passwordResetToken, string userName, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userName = userName, portalId = portalId });

            //check whether the current login user has B2B role or not.
            bool isAdminUser = IsAdminUser(userDetails);

            string baseUrl = !isAdminUser ? GetDomains(portalId) : ZnodeApiSettings.AdminWebsiteUrl;

            //Get Portal Details.
            PortalModel portalModel = GetCustomPortalDetails(portalId);

            //Set the Http protocol based on the portal setting.
            baseUrl = !isAdminUser ? (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}" : baseUrl;

            string passwordResetUrl = $"{baseUrl}/User/ResetPassword?passwordToken={WebUtility.UrlEncode(passwordResetToken)}&userName={EncodeBase64(userName)}";
            string passwordResetLink = $"<a href=\"{passwordResetUrl}\"> here</a>";

            // Genrate Reset Password Email Content.
            string subject = string.Empty;
            //Set to True to send this email in HTML format.
            bool isHtml = false;
            bool isEnableBcc = false;
            try
            {
                string messageText = GenerateResetPasswordEmail(userName, passwordResetUrl, passwordResetLink, portalId, out subject, out isHtml, out isEnableBcc);
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { messageText = messageText });

                if (!string.IsNullOrEmpty(messageText))
                    //This method is used to send an email.          
                    if (IsNotNull(portalModel) && portalId > 0)
                        ZnodeEmail.SendEmail(portalId, userDetails.Email, portalModel?.AdministratorEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, isHtml);
                    else
                        ZnodeEmail.SendEmail(userDetails.Email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, isHtml);
                else
                    throw new ZnodeException(ErrorCodes.EmailTemplateDoesNotExists, Admin_Resources.ErrorResetPasswordLinkReset);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.ErrorSendResetPasswordLink, Admin_Resources.ErrorSendResetPasswordLink);
            }
        }

        //Get domain name on the basis of portal id.
        protected virtual string GetDomains(int portalId)
           => _domainRepository.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ZnodeConstant.WebStore && x.IsActive && x.IsDefault)?.Select(x => x.DomainName)?.FirstOrDefault().ToString();

        //Password verification.
        protected virtual UserModel PasswordVerification(int portalId, UserModel model, ApplicationUser user)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            bool isResetPassword = !string.IsNullOrEmpty(model.User.PasswordToken);

            //This method is used to verify user.
            if (!isResetPassword && SignInManager.HasBeenVerified())
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCurrentPassword);

            //This method is used to verify the reset password token is valid or not.
            if (isResetPassword && !UserManager.VerifyUserToken(user.Id, "ResetPassword", model.User.PasswordToken))
                throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.ResetPasswordLinkExpired);

            bool verified = (isResetPassword) ? true : ZnodeUserAccountBase.VerifyNewPassword(user.Id, model.User.NewPassword);

            if (verified)
            {
                // Update/Reset the password for this user
                var result = (isResetPassword)
                    //This method is used to reset the password having valid user Id, password token and new password.
                    ? UserManager.ResetPassword(user.Id, model.User.PasswordToken, model.User.NewPassword)

                    //This method is used to change the password having valid user Id, old password and new password.
                    : UserManager.ChangePassword(user.Id, model.User.Password, model.User.NewPassword);
                if (result.Succeeded)
                {
                    ZnodeUser znodeUser =_userRepository.Table.FirstOrDefault(x => x.AspNetUserId == user.Id);
                    //Set user is valid if reset password is success.
                    if (!znodeUser.IsVerified)
                    {
                        znodeUser.IsVerified = true;
                        znodeUser.UserVerificationType = null;
                        _userRepository.Update(znodeUser);
                    }
                    // Log password
                    SetRequestHeaderForPasswordLog(user.Id, 0);
                    ZnodeUserAccountBase.LogPassword(user.Id, model.User.NewPassword);
                    model.User.Password = model.User.NewPassword;

                    //Save recently password change date.
                    ZnodeUserAccountBase.SetPasswordChangedDate(user.Id);
                    int? errorCode = null;
                    return Login(portalId, model, out errorCode);
                }
                throw new ZnodeException(ErrorCodes.InvalidData, result.Errors.FirstOrDefault());
            }
            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.EnterNewPassword);
        }

        //Check whether the current login user has Customer user and B2B role or not.
        protected virtual bool IsAdminUser(ApplicationUser user)
        {
            List<string> lstRole = GetRoles();

            //Get roles by user id.
            IList<string> roles = UserManager.GetRoles(user.Id.ToString());
            ZnodeLogging.LogMessage("roles list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roles?.Count());
            return IsNotNull(roles) ? !roles.Any(x => lstRole.Contains(x)) : false;
        }

        //Get roles.
        protected virtual List<string> GetRoles()
        {
            List<string> lstRole = new List<string>();
            lstRole.Add(ZnodeRoleEnum.User.ToString());
            lstRole.Add(ZnodeRoleEnum.Customer.ToString());
            lstRole.Add(ZnodeRoleEnum.Administrator.ToString());
            lstRole.Add(ZnodeRoleEnum.Manager.ToString());
            ZnodeLogging.LogMessage("lstRole list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstRole?.Count());

            return lstRole;
        }

        //Get Account details of various account ids.
        protected virtual List<ZnodeUser> GetUsers(ParameterModel accountIds)
        {
            FilterCollection filterList = new FilterCollection();
            filterList.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.In, accountIds.Ids));

            return _userRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();
        }        

        //Enable Disable user account.
        public virtual bool EnableDisableUser(ZnodeUser accountDetails, bool lockUser, out bool isAllowedToLock)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            isAllowedToLock = true;
            bool isVerifiedUpdated = false;

            //Get current user details.
            ApplicationUser user = UserManager.FindById(accountDetails.AspNetUserId.ToString());
            if (IsNotNull(user))
            {
                int? portalId = _userPortalRepository.Table.FirstOrDefault(x => x.UserId == accountDetails.UserId)?.PortalId;
                // Gets the portal ids.
                int portalIdValue = ((portalId < 1) || IsNull(portalId)) ? PortalId : portalId.Value;
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });
              
                IdentityResult result = null;
                //Code for unlocking user account.
                if (UserManager.IsLockedOut(user.Id) && !lockUser)
                {
                    user.LockoutEndDateUtc = null;
                    result = UserManager.Update(user);
                    if (!accountDetails.IsVerified)
                    {
                        if (IsAdminApprovalType(accountDetails.UserVerificationType))
                        {
                            isVerifiedUpdated = VerifyAccountAndSendMail(accountDetails, portalIdValue);
                            var onAccountVerificationSuccessfulInit = new ZnodeEventNotifier<UserModel>(accountDetails.ToModel<UserModel>(), EventConstant.OnAccountVerificationSuccessful);
                        }
                    }
                }            
                else if (lockUser)
                    //Code for locking user account.
                    result = UserManager.SetLockoutEndDate(user.Id, DateTimeOffset.MaxValue);

                if (IsNotNull(portalId) && !isVerifiedUpdated)
                {
                    SendAccountActivationEmail(accountDetails.FirstName, user.Email, portalId.Value, lockUser);
                    // lockUser flag - Check the user is activated or deactivated and allow to trigger events accordingly.
                    if (!lockUser)
                    {
                        var onCustomerAccountActivationInit = new ZnodeEventNotifier<UserModel>(accountDetails.ToModel<UserModel>(), EventConstant.OnCustomerAccountActivation);
                    }
                }
                if (result.Succeeded)
                    isAllowedToLock = true;
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return false;
        }
       
        //Get Filter clause for Users Id.
        protected virtual EntityWhereClauseModel FilterClauseForUserId(ApplicationUser user)
        {
            FilterCollection filters = new FilterCollection();
            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodeUserEnum.AspNetUserId.ToString(), ProcedureFilterOperators.Is, user.Id.ToString()));

            //Generating where clause to get account details.             
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        protected virtual void GenerateActivationEmail(string firstName, string newPassword, string templateContent, int portalId, bool isCustomerUser, out string senderEmail, ref string subject, out string messageText, bool isWebstoreUser = false, string userId = null, string userName = "")
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            string portalName = ZnodeConfigManager.SiteConfig.StoreName;
            senderEmail = ZnodeConfigManager.SiteConfig.AdminEmail;
            ZnodeLogging.LogMessage("Portal Name:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, portalName);

            string vendorLoginUrl = $"{HttpContext.Current.Request.Url.Scheme}://{ZnodeConfigManager.DomainConfig?.DomainName}";
            if (vendorLoginUrl == $"{HttpContext.Current.Request.Url.Scheme}://")
                vendorLoginUrl = ZnodeApiSettings.AdminWebsiteUrl;

            //Get Portal details as well as the Domain Url based on the WEbStore Application Type.
            PortalModel model = GetCustomPortalDetails(portalId);

            //Get Portal Details in case of Customer User
            if (isCustomerUser && IsNotNull(model))
            {
                portalName = model.StoreName;
                senderEmail = model.AdministratorEmail;
                vendorLoginUrl = $"{HttpContext.Current.Request.Url.Scheme}://{model.DomainUrl}";
            }

            vendorLoginUrl = GetVendorLoginUrlForAdminUser(isWebstoreUser, userId, vendorLoginUrl, userName);

            subject = $"{portalName} - {subject}";

            //Set Parameters for the Email Templates to be get replaced.
            Dictionary<string, string> setDictionary = new Dictionary<string, string>
            {
                {"#FirstName#", firstName},
                {"#UserName#", firstName},
                {"#Url#", vendorLoginUrl},
                {"#Password#", newPassword},
                {"#StoreLogo#", model.StoreLogo}
            };

            //Replace the Email Template Keys, based on the passed email template parameters.
            messageText = _emailTemplateSharedService.ReplaceTemplateTokens(templateContent, setDictionary);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

        }

        //These method is used to set null to unwanted keys.
        protected virtual string SetNullToUnWantedKeys(string messageText)
        {
            //It is used to find keys in between special characters.
            List<string> list = Regex.Matches(messageText, @"#\w*#")
                                    .Cast<Match>()
                                     .Select(m => m.Value)
                                        .ToList();

            ZnodeLogging.LogMessage("list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, list?.Count());
            foreach (string key in list)
                messageText = ReplaceTokenWithMessageText(key, null, messageText);

            ZnodeLogging.LogMessage("Output Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { messageText = messageText });

            return messageText;
        }

        //Insert data for guest user.
        protected virtual void InsertDataForGuestUser(UserModel userModel, ApplicationUser user, ZnodeUser account)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Create B2B Customer.
            CreateB2BCustomer(userModel, account, user);

            //Insert for the user portal.
            InsertIntoUserPortal(userModel.PortalId, userModel.UserId);

            // Gets the portal ids.
            SetAssignedPortal(userModel, true);

            //Set Display name 
            SetAddressName(userModel.UserId);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

        }

        //Get the department user entity.
        protected virtual ZnodeDepartmentUser GetDepartmentUserEntity(UserModel userModel)
            => new ZnodeDepartmentUser { DepartmentId = userModel.DepartmentId, DepartmentUserId = Convert.ToInt32(userModel.DepartmentUserId), UserId = userModel.UserId };

        //Save the account permissions for user.
        protected virtual void SaveAccountUserPermission(UserModel userModel)
        {
            ZnodeLogging.LogMessage("AccountPermissionAccessId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userModel?.AccountPermissionAccessId);
            //Update Account Permission Access user if already exists else create.
            if (userModel.AccountPermissionAccessId > 0)
            {
                ZnodeAccountUserPermission accountUserPermission = _accountUserPermissionRepository.Table.FirstOrDefault(x => x.UserId == userModel.UserId);
                if (IsNotNull(accountUserPermission))
                    // update account permission and approval
                    UpdateAccountPermission(userModel, accountUserPermission);
                else
                    _accountUserPermissionRepository.Insert(new ZnodeAccountUserPermission { AccountPermissionAccessId = userModel.AccountPermissionAccessId.GetValueOrDefault(), UserId = userModel.UserId });

            }
        }

        //Method to update account permission and approval
        protected virtual void UpdateAccountPermission(UserModel userModel, ZnodeAccountUserPermission accountUserPermission)
        {
            accountUserPermission.AccountPermissionAccessId = userModel.AccountPermissionAccessId.GetValueOrDefault();
            //Update Permission.
            _accountUserPermissionRepository.Update(accountUserPermission);
        }


        protected virtual void InsertIntoUserPortal(int? portalId, int userId, int salesRepId = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId, userId = userId });

            //if the global level user creation is set to true then set portal id to null.
            if (DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                portalId = null;

            if (salesRepId > 0)
                SalesRepAssociation(salesRepId, userId, portalId);

            if (_userPortalRepository.Table.Any(x => x.UserId == userId && x.PortalId == portalId))
                return;

            InsertUpdateUserPortals(Convert.ToString(portalId), userId);

        }

        //Insert User Portals In ZnodeUserPortal
        protected virtual bool InsertUpdateUserPortals(string portalIds, int userId)
        {

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PortalId", portalIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdate_UserPortal @UserId,@PortalId,@Status OUT", 2, out status);
            return deleteResult.FirstOrDefault().Status.Value;

        }

        //Associate Sales Rep 
        protected virtual void SalesRepAssociation(int salesRepId, int userId, int? portalId)
        {

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@CustomerUserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@CustomerPortalId", portalId > 0 ? portalId : 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);

            IList<View_ReturnBoolean> SalesRepAssociation = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdate_SalesRepCustomerUserPortal @SalesRepUserId,@CustomerUserId,@CustomerPortalId,@UserId,@Status OUT", 4, out status);
        }

        //Check if IsApprovalList exists in filters and remove it.
        protected virtual void IsRoleNameExists(FilterCollection filters, ref bool isApproval, ref string currentUserName, ref string currentRoleName)
        {
            if (filters.Exists(x => x.FilterName == FilterKeys.IsApprovalList))
            {
                isApproval = true;
                filters.RemoveAll(x => x.FilterName == FilterKeys.IsApprovalList);
                currentRoleName = string.Empty;
                currentUserName = string.Empty;
            }
        }

        //Check if IsApprovalList exists in filters and remove it.
        protected virtual bool IsCustomerEditMode(FilterCollection filters)
        {
            if (filters.Exists(x => x.FilterName == FilterKeys.CustomerEditMode))
            {
                filters.RemoveAll(x => x.FilterName == FilterKeys.CustomerEditMode);
                return true;
            }
            return false;
        }

        //Create B2B customers.
        protected virtual void CreateB2BCustomer(UserModel model, ZnodeUser account, ApplicationUser user)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Checks if role name is null, then create normal user with role Customer, else create admin user.
            if (_loginHelper.IsAccountCustomer(model))
            {
                InsertIntoUserDepartment(model);

                //Insert all the profiles of the account to which user is associated.
                InsertUserProfile(model, account);

                //Inserts into AspnetUserRole
                UserManager.AddToRole(user.Id, model.RoleName);
            }
            else if (account?.AccountId == null)
            {
                UserManager.AddToRole(user.Id, ZnodeRoleEnum.Customer.ToString());
            }
            else
                UserManager.AddToRole(user.Id, ZnodeRoleEnum.User.ToString());

            if (string.IsNullOrEmpty(model.PermissionCode))
            {
                model.PermissionCode = ZnodePermissionCodeEnum.DNRA.ToString();
                model.AccountPermissionAccessId = GetDNRAAccountPermissionAccessId();
            }

            //Insert the access permission for user.
            InsertAccessPermissionForUser(model, account);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Get DNRA account permission access id.
        protected virtual int? GetDNRAAccountPermissionAccessId()
            => Convert.ToInt32(_accountPermissionAccessRepository.Table.Join(_accountPermissionRepository.Table.Where(o => !o.AccountId.HasValue && o.AccountPermissionName == ZnodeConstant.DNRAAccountPermissionAccessName),
                                                                                 o => o.AccountPermissionId,
                                                                                 ob => ob.AccountPermissionId,
                                                                                 (o, ob) => o)?.FirstOrDefault()?.AccountPermissionAccessId);

        //Insert the access permission for user
        protected virtual void InsertAccessPermissionForUser(UserModel model, ZnodeUser account)
        {
            if (model.AccountPermissionAccessId > 0)
                _accountUserPermissionRepository.Insert(new ZnodeAccountUserPermission { UserId = account.UserId, AccountPermissionAccessId = model.AccountPermissionAccessId.GetValueOrDefault() });

            if (!IsDoesNotRequirePermission(model.PermissionCode))
                _accountUserOrderApprovalRepository.Insert(new ZnodeAccountUserOrderApproval() { UserId = account.UserId });
        }

        //Updates into AspnetUserRole
        protected virtual void UpdateUserRole(UserModel userModel, ApplicationUser user)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string oldRoleName = UserManager.GetRoles(user.Id).FirstOrDefault();
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { oldRoleName = oldRoleName });

            if (!string.Equals(userModel.RoleName, ZnodeConstant.PleaseSelectLabel, StringComparison.InvariantCultureIgnoreCase) && IsNotNull(userModel.RoleName) && !string.Equals(userModel.RoleName, oldRoleName, StringComparison.InvariantCultureIgnoreCase) && IsNotNull(oldRoleName))
            {
                UserManager.RemoveFromRole(user.Id, oldRoleName);
                UserManager.AddToRole(user.Id, userModel.RoleName);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Checks whether permission for user is does not require permission or not.
        protected virtual bool IsDoesNotRequirePermission(string permissionCode)
            => !string.IsNullOrEmpty(permissionCode) && Equals(permissionCode, ZnodePermissionCodeEnum.DNRA.ToString());

        //Method to insert User data.
        protected virtual UserModel InsertUserData(UserModel model, ApplicationUser user, string password)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            bool isUserCreateFromAdmin = false;
            model.UserVerificationType = model.UserVerificationTypeCode.ToString();
            model.UserName = model?.User?.Username;
            //Verification set to true for admin user registration and no verification type registration.
            if ((!model.IsWebStoreUser) || model.IsWebStoreUser && model.UserVerificationTypeCode == UserVerificationTypeEnum.NoVerificationCode)
            {
                model.IsVerified = true;
                //Update UserVerificationType if user is verified so that we don't have unneeded data. 
                model.UserVerificationType = null;
            }
            //Save account details.
            ZnodeUser account = _userRepository.Insert(model.ToEntity<ZnodeUser>());
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { UserId = account?.UserId });

            if (account?.UserId > 0)
            {
                model.UserId = account.UserId;
                ZnodeLogging.LogMessage(account?.UserId > 0 ? Admin_Resources.SuccessUserDataSave : Admin_Resources.ErrorUserDataSave, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                if (!_loginHelper.IsAccountCustomer(model))
                    _accountProfileRepository.Insert(new ZnodeUserProfile { UserId = account.UserId, ProfileId = model.ProfileId, IsDefault = true });

                //Create B2B Customer.
                CreateB2BCustomer(model, account, user);

                //Insert for the user portal.
                InsertIntoUserPortal(model.PortalId, account.UserId);

                // Gets the portal ids.
                SetAssignedPortal(model, true);
                //Create link for new register user which is register from admin side.
                //For that we send new admin user 
                if (!model.IsWebStoreUser)
                {
                    isUserCreateFromAdmin = true;
                    ForgotPassword((model.PortalId > 0) ? model.PortalId.Value : PortalId, model, isUserCreateFromAdmin);
                }
                //If firstName is empty,then bind username (at the time of new user creation from webstore).
                model.FirstName = model?.FirstName ?? model?.User?.Username;

                if (!model.IsSocialLogin && model.IsWebStoreUser && IsEmailVerificationType(model.UserVerificationTypeCode))
                {
                    ForgotPassword((model.PortalId > 0) ? model.PortalId.Value : PortalId, model);
                }
                else if (model.IsWebStoreUser && IsAdminApprovalType(model.UserVerificationTypeCode))
                {
                    EnableDisableUser(new ParameterModel { Ids = model.UserId.ToString() }, true);
                    //Account verification mail send to customer.
                    GenerateAccountActivationStatusEmail(model, ZnodeConstant.AccountVerificationRequestInProgress);
                    var onAccountVerificationRequestInProgresslInit = new ZnodeEventNotifier<UserModel>(model, EventConstant.OnAccountVerificationRequestInProgress);
                    UserModel adminModel = SetAdminMailParameters(new UserModel { UserName = model?.Email, PortalId = model.PortalId });
                    //Account verification mail send to Admin.
                    GenerateAccountActivationStatusEmail(adminModel, ZnodeConstant.AccountVerificationRequest);
                }
                //map account model to account entity.
                else if (!SendEmailToStoreAdministrator(model.FirstName, password, account.Email, (model.PortalId > 0) ? model.PortalId.Value : PortalId, model.LocaleId, true, model.IsTradeCentricUser, model.IsWebStoreUser, isUserCreateFromAdmin))
                {
                    UserModel accountModel = UserMap.ToModel(account, user.Email, !string.IsNullOrEmpty(model.User?.Username) ? model.User.Username : model.UserName);
                    accountModel.IsEmailSentFailed = true;
                    return accountModel;
                }
                else
                {

                    string onAccountCreated = model.IsWebStoreUser ? EventConstant.OnNewCustomerAccountFromWebstore : EventConstant.OnNewCustomerAccountFromAdmin;
                    //Call to SMS Event
                    if (!isUserCreateFromAdmin)
                    {
                        var onCustomerAccountCreateInit = new ZnodeEventNotifier<UserModel>(model, onAccountCreated);
                    }
                }
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return UserMap.ToModel(account, user.Email, !string.IsNullOrEmpty(model.User.Username) ? model.User?.Username : model.UserName);
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUserDataSave, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.ExceptionalError, Admin_Resources.ErrorUserCreate);
            }
        }

        //Insert all the profiles of the account to which user is associated.
        protected virtual void InsertUserProfile(UserModel model, ZnodeUser account)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (account.AccountId > 0)
            {
                List<ZnodeAccountProfile> associatedProfile = _accountAssociatedProfileRepository.Table.Where(x => x.AccountId == account.AccountId).ToList();

                if (associatedProfile?.Count > 0)
                {
                    //Get all the profiles of user associated account.
                    List<int?> accountProfileIds = associatedProfile.Select(x => x.ProfileId).ToList();
                    ZnodeLogging.LogMessage("accountProfileIds list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, accountProfileIds?.Count());
                    if (accountProfileIds?.Count > 0)
                    {
                        int defaultProfile = associatedProfile.Where(x => x.IsDefault)?.FirstOrDefault()?.ProfileId ?? 0;
                        ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { defaultProfile = defaultProfile });

                        //Create entities to insert
                        List<ZnodeUserProfile> entriesToInsert = new List<ZnodeUserProfile>();
                        foreach (int? item in accountProfileIds)
                            entriesToInsert.Add(new ZnodeUserProfile() { UserId = account.UserId, ProfileId = Convert.ToInt32(item), IsDefault = false });

                        //If account profile contains a profile already present in user then simply set it as IsDefault else add it to the entities to be inserted.
                        if (defaultProfile > 0 && accountProfileIds.Contains(defaultProfile))
                            entriesToInsert.Where(x => x.ProfileId == defaultProfile).ToList().ForEach(x => x.IsDefault = true);
                        else
                            entriesToInsert.Add(new ZnodeUserProfile() { UserId = account.UserId, ProfileId = defaultProfile > 0 ? defaultProfile : model.ProfileId, IsDefault = true });
                        ZnodeLogging.LogMessage("entriesToInsert list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, entriesToInsert?.Count());

                        ZnodeLogging.LogMessage(IsNotNull(_accountProfileRepository.Insert(entriesToInsert)) ? Admin_Resources.SuccessUserProfileInsert : Admin_Resources.ErrorUserProfileInsert, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    }
                }
            }
            else
            {
                _accountProfileRepository.Insert(new ZnodeUserProfile { UserId = account.UserId, ProfileId = model.ProfileId, IsDefault = true });
            }
        }

        //Get expands and add them to navigation properties
        protected virtual List<string> GetExpands(NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<string> navigationProperties = new List<string>();
            if (expands?.HasKeys() ?? false)
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (key.Equals(ZnodeAccountUserOrderApprovalEnum.ZnodeUser.ToString(), StringComparison.InvariantCultureIgnoreCase)) SetExpands(ZnodeAccountUserOrderApprovalEnum.ZnodeUser.ToString(), navigationProperties);
                    if (Equals(key, FilterKeys.WishLists)) SetExpands(ZnodeUserEnum.ZnodeUserWishLists.ToString(), navigationProperties);
                    if (Equals(key, FilterKeys.Profiles)) SetExpands(ZnodeUserEnum.ZnodeUserProfiles.ToString(), navigationProperties);
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return navigationProperties;
        }

        //Get expands and add them to navigation properties.
        protected virtual List<string> GetSocialLoginExpands(NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<string> navigationProperties = new List<string>();
            if (expands?.HasKeys() ?? false)
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (key.Equals(ZnodePortalLoginProviderEnum.ZnodeLoginProvider.ToString(), StringComparison.InvariantCultureIgnoreCase)) SetExpands(ZnodePortalLoginProviderEnum.ZnodeLoginProvider.ToString(), navigationProperties);
                    if (key.Equals(ZnodePortalLoginProviderEnum.ZnodeDomain.ToString(), StringComparison.InvariantCultureIgnoreCase)) SetExpands(ZnodePortalLoginProviderEnum.ZnodeDomain.ToString(), navigationProperties);
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return navigationProperties;
        }

        //Get the Login User Details
        protected virtual ApplicationUser GetAspNetUserDetails(ref UserModel model, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (IsNull(model.User))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.LoginUserModelNotNull);

            //Get the User Details from Znode Mapper
            AspNetZnodeUser znodeUser = _loginHelper.GetUserDetails(model, (model?.PortalId > 0) ? Convert.ToInt32(model.PortalId) : portalId);
            if (IsNull(znodeUser))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.ErrorUserNotExist, model.User.Username));

            //Bind the Aspnet Znode User Id.
            model.AspNetZnodeUserId = znodeUser.AspNetZnodeUserId;

            //Get the Login User Details from Owin Mapper.
            var user = UserManager.FindByName(znodeUser.AspNetZnodeUserId);
            if (IsNull(user))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.ErrorUserExist, model.User.Username));
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return user;
        }

        //Sends the reset password.
        protected virtual string SendResetPassword(UserModel model, ApplicationUser user, bool isUserCreateFromAdmin = false, bool isAdminUser = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //This method is used to update the security stamp to make invalid previous token which is required in case of reset password. 
            UserManager.UpdateSecurityStamp(user.Id);
            //This method is used to generate password reset token.
            string passwordResetToken = UserManager.GeneratePasswordResetToken(user.Id);
            string baseUrl = string.IsNullOrEmpty(model.BaseUrl) ? ZnodeApiSettings.AdminWebsiteUrl : model.BaseUrl;
            if (isUserCreateFromAdmin && !isAdminUser)
            {
                //Get Portal details as well as the Domain Url based on the WEbStore Application Type.
                PortalModel portalModel = GetCustomPortalDetails(model.PortalId.Value);
                baseUrl = (IsNotNull(portalModel)) ? $"{HttpContext.Current.Request.Url.Scheme}://{portalModel.DomainUrl}" : ZnodeApiSettings.AdminWebsiteUrl;
            }
            //Generate the Password Reset Url
            string passwordResetUrl = $"{baseUrl}/User/ResetPassword?passwordToken={WebUtility.UrlEncode(passwordResetToken)}&userName={EncodeBase64(model.User.Username)}";
            string subject = string.Empty;
            bool isHtml = false;
            bool isEnableBcc = false;
            //Generate Reset Password Email Content.

            try
            {
                string messageText = GenerateResetPasswordEmail(string.IsNullOrEmpty(model.FirstName) ? model.User.Username : model.FirstName, passwordResetUrl, $"<a href=\"{passwordResetUrl}\"> Click Here</a>", model.PortalId.Value, out subject, out isHtml, out isEnableBcc, model.LocaleId, isUserCreateFromAdmin, model.IsInvalidCredential, model.StoreName);
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { messageText = messageText });

                if (!string.IsNullOrEmpty(messageText))
                {
                    ZnodePortalSmtpSetting smtpConfigurations = ZnodeEmail.GetSMTPSetting(model.PortalId.Value);
                    string userName = GetDecryptSMTPUserName(smtpConfigurations.UserName);
                    smtpConfigurations.UserName = userName;
                    HttpContext httpContext = HttpContext.Current;
                    //This method is used to send an email.
                    if (model.PortalId.Value > 0)
                    {                        
                        Task.Run(() =>
                        {
                            HttpContext.Current = httpContext;
                            ZnodeEmail.SendEmail(model.PortalId.Value, model.User.Email, userName, ZnodeEmail.GetBccEmail(isEnableBcc, model.PortalId.Value, string.Empty), subject, messageText, isHtml, smtpConfigurations, userName);
                        });
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            HttpContext.Current = httpContext;
                            ZnodeEmail.SendEmail(model.User.Email, userName, ZnodeEmail.GetBccEmail(isEnableBcc, model.PortalId.Value, string.Empty), subject, messageText, isHtml, smtpConfigurations);
                        });
                    }
                }
                else
                    throw new ZnodeException(ErrorCodes.EmailTemplateDoesNotExists, Admin_Resources.ErrorResetPasswordLinkReset);
                if (model.IsInvalidCredential)
                {
                 var onResetPasswordInitiator = new ZnodeEventNotifier<UserModel>(model, EventConstant.OnRegistrationAttemptUsingExistingUsername);
                }
                else
                {
                    string onNewCustomerCreate = isUserCreateFromAdmin ? EventConstant.OnNewCustomerAccountFromAdmin : EventConstant.OnResetPassword ;
                    var onResetPasswordInit = new ZnodeEventNotifier<UserModel>(model, onNewCustomerCreate);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                model.IsEmailSentFailed = true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                model.IsEmailSentFailed = true;
                throw new ZnodeException(ErrorCodes.ErrorSendResetPasswordLink, Admin_Resources.ErrorSendResetPasswordLink);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return passwordResetToken;
        }

        //Save the portal for the account.
        protected virtual void InsertIntoUserDepartment(UserModel model, bool isUpdate = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = model.AccountId });

            if (isUpdate)
            {
                //If record exists then return.
                if (_departmentUserRepository.Table.Any(x => x.UserId == model.UserId && x.DepartmentId == model.DepartmentId))
                    return;

                //Delete the existing entry.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeDepartmentUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, model.UserId.ToString()));

                ZnodeLogging.LogMessage(_departmentUserRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()))
                    ? string.Format(Admin_Resources.SuccessDeleteUserDepartment, model.DepartmentName) : string.Format(Admin_Resources.ErrorDeleteUserDepartment, model.DepartmentName), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }

            if (model.DepartmentId > 0)
                ZnodeLogging.LogMessage(_departmentUserRepository.Insert(GetDepartmentUserEntity(model))?.DepartmentUserId > 0
                    ? string.Format(Admin_Resources.SuccessInsertUserDepartment, model.DepartmentName) : string.Format(Admin_Resources.ErrorInsertUserDepartment, model.DepartmentName), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Get the Default Anonymous Profile Details based on the Portal
        protected virtual ProfileModel GetDefaultAnonymousProfile(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Get the Default Anonymous Profile Id associated to the Portal.
            ZnodePortalProfile portalProfile = _portalProfileRepository.Table.Where(x => x.PortalId == portalId && x.IsDefaultAnonymousProfile)?.FirstOrDefault();

            //Get the Profile based on the Profile Id.
            if (IsNotNull(portalProfile))
            {
                ProfileModel profile = _profileRepository.Table.Where(x => x.ProfileId == portalProfile.ProfileId)?.FirstOrDefault()?.ToModel<ProfileModel>();
                if (IsNotNull(profile))
                    profile.IsDefault = portalProfile.IsDefaultAnonymousProfile;
                return profile;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        //Get the Default Register Profile Details based on the Portal
        protected virtual ProfileModel GetDefaultRegisteredProfile(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Get the Default Anonymous Profile Id associated to the Portal.
            var portalProfile = _portalProfileRepository.Table.Where(x => x.PortalId == portalId && x.IsDefaultRegistedProfile)?.FirstOrDefault();

            //Get the Profile based on the Profile Id.
            if (IsNotNull(portalProfile))
                return _profileRepository.Table.Where(x => x.ProfileId == portalProfile.ProfileId)?.FirstOrDefault()?.ToModel<ProfileModel>();

            return null;
        }

        //Get the Profiles Associated to the Customer based on the User Id.
        protected virtual List<ProfileModel> GetCustomerProfileByUserId(int userId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId });

            //Get the Profiles based on userId.
            return (from userProfile in _userProfileRepository.Table
                    join profile in _profileRepository.Table on userProfile.ProfileId equals profile.ProfileId
                    where userProfile.UserId == userId
                    select new ProfileModel()
                    {
                        ProfileId = profile.ProfileId,
                        ProfileName = profile.ProfileName,
                        Weighting = profile.Weighting,
                        TaxExempt = profile.TaxExempt,
                        DefaultExternalAccountNo = profile.DefaultExternalAccountNo,
                        ShowOnPartnerSignup = profile.ShowOnPartnerSignup,
                        IsDefault = userProfile.IsDefault
                    })?.ToList();
        }

        //Get the account's portal Id.
        protected virtual int? GetAccountPortalId(int accountId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { accountId = accountId });

            IZnodeRepository<ZnodePortalAccount> _portalAccountRepository = new ZnodeRepository<ZnodePortalAccount>();
            return _portalAccountRepository.Table.FirstOrDefault(x => x.AccountId == accountId)?.PortalId;
        }

        //Bind the User Details based on the expand collection
        protected virtual UserModel BindUserDetails(UserModel model, NameValueCollection expands)
        {
            if (IsNotNull(expands) && expands.HasKeys())
                ExpandProfiles(expands, model);
            GetUserAccountCatalogDetails(model);
            return model;
        }
        //Set User Profile Expand.
        protected virtual void ExpandProfiles(NameValueCollection expands, UserModel user)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(expands.Get(FilterKeys.Profiles)))
            {
                //Get the List of Profile associated with the user.
                user.Profiles = (from userProfile in _userProfileRepository.Table
                                 join profile in _profileRepository.Table on userProfile.ProfileId equals profile.ProfileId
                               
                                 where userProfile.UserId == user.UserId
                                 select new ProfileModel
                                 {
                                     IsDefault = userProfile.IsDefault,
                                     ProfileId = profile.ProfileId,
                                     ProfileName = profile.ProfileName,
                                     Weighting = profile.Weighting,
                                     ShowOnPartnerSignup = profile.ShowOnPartnerSignup,
                                     TaxExempt = profile.TaxExempt,
                                     DefaultExternalAccountNo = profile.DefaultExternalAccountNo,
                                     PimCatalogId = profile.PimCatalogId,
                                     PublishCatalogId = profile.PimCatalogId
                                 }).ToList();


            }
        }

        /// <summary>
        /// This method will return the Profile Id on the basis of portal id.
        /// </summary>
        /// <param name="portalId">integer Portal Id</param>
        /// <returns>Returns the profile id</returns>
        protected virtual int GetCurrentProfileId(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId });
            int profileID = 0;
            //Get the Register Profile Details based on the portalId.
            ProfileModel profile = GetDefaultRegisteredProfile(portalId);

            if (HelperUtility.IsNotNull(profile))
                profileID = profile.ProfileId;
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return profileID;
        }

        //Generate Reset Password Email Content based on the Email Template
        protected virtual string GenerateResetPasswordEmail(string firstName, string resetUrl, string resetLink, int portalId, out string subject, out bool isHtml, out bool isEnableBcc, int localeId = 0, bool isUserCreateFromAdmin = false, bool isInvalidCredential = false, string storeName = "")
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { firstName = firstName, portalId = portalId });

            // Send email to the user
            string messageText = string.Empty;

            //Method to get Email Template Details.
            EmailTemplateMapperModel emailTemplateMapperModel;
            if (isInvalidCredential)
            {
                subject = "Registration Attempt";
                emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.RegistrationAttemptUsingExistingUsername, portalId, localeId);
            }
            else
            {
                subject = "Reset Password Notification";
                emailTemplateMapperModel = GetEmailTemplateByCode((isUserCreateFromAdmin ? ZnodeConstant.NewCustomerAccountFromAdmin : ZnodeConstant.ResetPassword), portalId, localeId);
            }
                
            isHtml = false;
            isEnableBcc = false;
            
            
            if (IsNotNull(emailTemplateMapperModel))
            {
                PortalModel portalModel = GetCustomPortalDetails(portalId);
                string storeLogo = portalModel?.StoreLogo;
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { storeLogo = storeLogo });

                //Set Parameters for the Email Templates to be get replaced.
                SetParameterOfEmailTemplate(firstName, resetUrl, resetLink, storeLogo, storeName, out Dictionary<string, string> setDictionary);

                //Replace the Email Template Keys, based on the passed email template parameters.
                messageText = _emailTemplateSharedService.ReplaceTemplateTokens(emailTemplateMapperModel.Descriptions, setDictionary);
                subject = GenerateResetPasswordEmailSubject(portalModel.StoreName, emailTemplateMapperModel.Subject);
                isHtml = true;
                isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return messageText;
        }

        //Set Parameters for the Email Templates to be get replaced.
        protected virtual void SetParameterOfEmailTemplate(string firstName, string resetUrl, string resetLink, string storeLogo, string storeName, out Dictionary<string, string> setDictionary)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { firstName = firstName });
            setDictionary = new Dictionary<string, string>
            {
                {"#FirstName#", firstName},
                {"#UserName#", firstName},
                {"#Link#", resetLink},
                {"#Url#", resetUrl},
                {"#StoreLogo#", storeLogo},
                {"#StoreName#", storeName}
            };
        }

        // Set parameters for the subject of Email Templates and to replace the Token for storeName.
        protected virtual string GenerateResetPasswordEmailSubject(string storeName, string subject)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { storeName = storeName, subject = subject });
            EmailTemplateHelper.SetParameter("#StoreName#", storeName);
            return (EmailTemplateHelper.ReplaceTemplateTokens(subject));
        }

        //Get the assigned portal id for user.
        protected virtual void SetAssignedPortal(UserModel userModel, bool isCustomerEditMode)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Check for userId greater than 0 and sent form customer edit mode.
            if (userModel?.UserId > 0 && isCustomerEditMode)
                SetPortal(userModel);

            if (userModel?.UserId < 1 && !isCustomerEditMode && userModel.PortalIds.Any())
                userModel.PortalId = Convert.ToInt32(userModel.PortalIds.FirstOrDefault());
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        protected virtual void SetAddressName(int userId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId });

            if (userId > 0)
            {
                AddressModel addressModel = GetAddressByUserId(userId);

                //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
                AddressHelper.SetAddressFlagsToFalse(addressModel);

                _addressRepository.Update(addressModel.ToEntity<ZnodeAddress>());
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

        }

        //Sets the portal ids.
        protected virtual void SetPortal(UserModel userModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //If the user belongs to the account then fetch the account's portal Id otherwise user portal id.
            if (userModel.AccountId > 0)
                userModel.PortalId = GetAccountPortalId(Convert.ToInt32(userModel.AccountId));
            else
            {
                List<ZnodeUserPortal> userPortalList = _userPortalRepository.Table.Where(x => x.UserId == userModel.UserId)?.ToList();

                if ((userPortalList?.Any(x => x.PortalId > 0)).GetValueOrDefault())
                    userModel.PortalIds = userPortalList.Select(x => x.PortalId.ToString()).ToArray();
                userModel.PortalId = string.IsNullOrEmpty(userModel.PortalIds?.FirstOrDefault()) ? (int?)null : Convert.ToInt32(userModel.PortalIds?.FirstOrDefault());
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Sets the customer details.
        protected virtual void SetCustomerDetails(ApplicationUser userDetails, UserModel accountDetails, ZnodeUser account)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Get the role Id.
            if (userDetails.Roles?.Count > 0)
                accountDetails.RoleId = userDetails.Roles.Select(x => x.RoleId).FirstOrDefault();
            //Get the role name.
            IRoleService role = GetService<IRoleService>();
            accountDetails.RoleName = role.GetRole(accountDetails?.RoleId)?.Name;

            //Set the user details
            accountDetails.User.UserId = userDetails.Id;
            accountDetails.User.Username = _loginHelper.GetUserById(userDetails.UserName)?.UserName;
            accountDetails.UserName = accountDetails.User.Username;
            accountDetails.User.Email = userDetails.Email;

            //Check for is b2b customer.

            if (_loginHelper.IsAccountCustomer(accountDetails))
            {
                //Get the user permission.
                UserModel accessPermissionEntity = GetUserAccessPermission(account.UserId);

                if (IsNotNull(accessPermissionEntity))
                {
                    accountDetails.PermissionCode = accessPermissionEntity.PermissionCode;
                    accountDetails.PermissionsName = accessPermissionEntity.PermissionsName;
                    accountDetails.AccountPermissionAccessId = accessPermissionEntity.AccountPermissionAccessId;
                    accountDetails.AccountUserPermissionId = accessPermissionEntity.AccountUserPermissionId;
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Remove all the users from list who is having role 'AccountUser'.
        protected virtual void RemoveAcccountUsers(bool isApproval, UserListModel list)
        {
            if (isApproval && list.Users.Count > 0)
                list.Users.RemoveAll(x => x.RoleName == ZnodeConstant.AccountUser);
        }

        //Get User data and bind to update model.
        protected virtual UserModel GetAndBindUserData(UserModel userModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userModel.UserId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.IsWebstore, ProcedureFilterOperators.Equals, "true"));

            UserModel userdata = GetUserListForWebstore(userModel.UserId, filters)?.FirstOrDefault();

            if (IsNotNull(userdata))
            {
                userdata.UserName = userModel.UserName;
                userdata.FirstName = userModel.FirstName;
                userdata.LastName = userModel.LastName;
                userdata.PhoneNumber = userModel.PhoneNumber;
                userdata.Email = userModel.Email;
                userdata.SMSOptIn = userModel.SMSOptIn;
                userdata.EmailOptIn = userModel.EmailOptIn;
                userdata.CustomerPaymentGUID = userModel?.CustomerPaymentGUID;
                userdata.ExternalId = IsNotNull(userModel.ExternalId) ? userModel.ExternalId : userdata.ExternalId;
                userdata.Custom1 = userModel.Custom1;
                userdata.Custom2 = userModel.Custom2;
                userdata.Custom3 = userModel.Custom3;
                userdata.Custom4 = userModel.Custom4;
                userdata.Custom5 = userModel.Custom5;
                userdata.IsVerified = userModel.IsVerified;
                userdata.ModifiedDate = GetDateTime();
                userdata.AccountId = userModel.AccountId == null ? userdata.AccountId : userModel.AccountId; 
            }
            return userdata;
        }

        //Check if user already subscribed for news letter.
        protected virtual bool IsUserAlreadyExist(string emailId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { emailId = emailId });

            List<UserModel> userData = (from user in _userRepository.Table
                                        join userPortal in _userPortalRepository.Table on user.UserId equals userPortal.UserId
                                        where user.Email == emailId && userPortal.PortalId == PortalId && user.EmailOptIn
                                        select new UserModel()
                                        {
                                            PortalId = userPortal.PortalId,
                                            EmailOptIn = user.EmailOptIn
                                        }).ToList();
            if (userData?.Count > 0)
                return (userData?.FirstOrDefault().PortalId == PortalId && userData?.FirstOrDefault().EmailOptIn == true);

            return false;
        }

        //Get Address by userId.
        protected virtual AddressModel GetAddressByUserId(int userId)
         => (from user in _userRepository.Table
             join userAddress in _userAddressRepository.Table on user.UserId equals userAddress.UserId
             join address in _addressRepository.Table on userAddress.AddressId equals address.AddressId
             where user.UserId == userId
             select new AddressModel
             {
                 Address1 = address.Address1,
                 Address2 = address.Address2,
                 Address3 = address.Address3,
                 CountryName = address.CountryName,
                 CityName = address.CityName,
                 AddressId = address.AddressId,
                 AlternateMobileNumber = address.AlternateMobileNumber,
                 DisplayName = address.DisplayName ?? ZnodeConstant.DefaultAddress,
                 ExternalId = address.ExternalId,
                 FaxNumber = address.FaxNumber,
                 FirstName = address.FirstName,
                 IsActive = address.IsActive,
                 LastName = address.LastName,
                 Mobilenumber = address.Mobilenumber,
                 PhoneNumber = address.PhoneNumber,
                 PostalCode = address.PostalCode,
                 StateName = address.StateName,
                 UserId = userId,
                 IsDefaultBilling = true,
                 IsDefaultShipping = true
             }).FirstOrDefault();


        //Add entry in ZnodeUserProfile table.
        protected virtual void InsertIntoUserProfile(ZnodeUser insertUser)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeUserProfile insertUserProfile = _userProfileRepository.Insert(new ZnodeUserProfile() { UserId = insertUser.UserId, ProfileId = GetDefaultAnonymousProfile(PortalId).ProfileId });
            ZnodeLogging.LogMessage(insertUserProfile?.UserProfileID > 0 ? string.Format(Admin_Resources.SuccessSaveProfileEntryForEmailSubscription, insertUserProfile?.UserProfileID) : Admin_Resources.ErrorSaveProfileEntryForEmailSubscription, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Add entry in ZnodeUserAddress table.
        protected virtual void InsertIntoZnodeUserAddress(ZnodeUser insertUser, ZnodeAddress insertAddress)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeUserAddress insertUserAddress = _userAddressRepository.Insert(new ZnodeUserAddress() { AddressId = insertAddress.AddressId, UserId = insertUser.UserId });
            ZnodeLogging.LogMessage(insertUserAddress?.UserAddressId > 0 ? string.Format(Admin_Resources.SuccessSaveAddressEntryForEmailSubscription, insertUserAddress?.UserAddressId) : Admin_Resources.ErrorSaveAddressEntryForEmailSubscription, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Add entry in ZnodeAddress table.
        protected virtual ZnodeAddress InsertIntoAddress()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeAddress insertAddress = _addressRepository.Insert(new ZnodeAddress() { StateName = string.Empty, PostalCode = string.Empty, CityName = string.Empty });
            ZnodeLogging.LogMessage(insertAddress?.AddressId > 0 ? string.Format(Admin_Resources.SuccessSaveAddressDataForEmailSubscription, insertAddress?.AddressId) : Admin_Resources.ErrorAddressUserDataForEmailSubscription, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return insertAddress;
        }

        //Add entry in ZnodeUser table.
        protected virtual ZnodeUser InsertIntoZnodeUser(NewsLetterSignUpModel model)
        {
            ZnodeUser insertUser = _userRepository.Insert(new ZnodeUser() { Email = model.Email, EmailOptIn = true });
            ZnodeLogging.LogMessage(insertUser?.UserId > 0 ? string.Format(Admin_Resources.SuccessSaveUserDataForEmailSubscription, insertUser.UserId) : Admin_Resources.ErrorSaveUserDataForEmailSubscription, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return insertUser;
        }

        //Bind user datails required after login.
        protected virtual UserModel BindDetails(UserModel model, ApplicationUser user, NameValueCollection expand)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Gets the User Details by User Id.
            model = GetUserByUserId(user.Id, expand);
            //Get role id.
            model.User.RoleId = user.Roles.Select(s => s.RoleId).FirstOrDefault();
            //check whether the current login user has Customer user and B2B role or not.
            model.IsAdminUser = IsAdminUser(user);
            //Get the role name.

            if (IsNull(model.RoleName))
            {
                IRoleService role = GetService<IRoleService>();
                model.RoleName = role.GetRole(model.User.RoleId)?.Name;
            }
            //Get user address list.
            model.Addresses = GetAddressList(model);
            //Get user access permission.
            if (IsNull(model.PermissionCode))
            {
                model.PermissionCode = GetUserAccessPermission(model.UserId)?.PermissionCode;
            }
            model.ProfileId = Convert.ToInt32(model.Profiles?.Select(x => x.ProfileId).FirstOrDefault());

            GetUserAccountCatalogDetails(model);

            ZnodeLogging.LogMessage("ExternalId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.ExternalId);
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return GetUsersAdditionalAttributes(model);
        }

        //Get the details of global attribute associated with store and user.
        protected virtual UserModel GetUsersAdditionalAttributes(UserModel model)
        {
            model.PortalId = PortalId;
            GetGlobalDetails(model);
            return model;

        }

        //gets the budget amount for the user
        protected virtual void GetGlobalDetails(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = model.AccountId });

            List<GlobalAttributeValuesModel> allUserAttributes = GetGlobalLevelAttributeList(model.UserId, ZnodeConstant.User);
            List<GlobalAttributeValuesModel> allStoreAttributes = GetGlobalLevelAttributeList(model.PortalId.GetValueOrDefault(), ZnodeConstant.Store);
            ZnodeLogging.LogMessage("List Count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { allUserAttributes = allUserAttributes?.Count(), allStoreAttributesCount = allStoreAttributes?.Count() });

            model.BillingAccountNumber = allUserAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.BillingAccountNumber)?.AttributeValue;
            model.PerOrderLimit = Convert.ToDecimal(string.IsNullOrEmpty(allUserAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PerOrderLimit)?.AttributeValue) ? "0" : allUserAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PerOrderLimit)?.AttributeValue);
            model.AnnualOrderLimit = Convert.ToDecimal(string.IsNullOrEmpty(allUserAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PerOrderAnnualLimit)?.AttributeValue) ? "0" : allUserAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PerOrderAnnualLimit)?.AttributeValue);
            int annualOrderLimitStartMonth = model.PortalId.GetValueOrDefault() > 0 ? Convert.ToInt32(allStoreAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PerOrderAnnualLimitStartMonth && x.AttributeValue != null)?.AttributeValue) : 1;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { annualOrderLimitStartMonth = annualOrderLimitStartMonth });

            model.AnnualBalanceOrderAmount = CalculateUserBalanceAmount(annualOrderLimitStartMonth, model.UserId, model.AnnualOrderLimit);
            model.UserGlobalAttributes = allUserAttributes;
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Calculate the annual balance amount for the user
        protected virtual decimal CalculateUserBalanceAmount(int annualOrderLimitStartMonth, int userId, decimal annualOrderLimit)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { annualOrderLimitStartMonth = annualOrderLimitStartMonth, userId = userId, annualOrderLimit = annualOrderLimit });

            if (annualOrderLimitStartMonth > 0)
            {
                var startDate = new DateTime(DateTime.Now.Year, annualOrderLimitStartMonth, 1);
                var endDate = startDate.AddMonths(12).AddSeconds(-1);

                if (startDate > DateTime.Now)
                    return 0;

                decimal? orderTotalforYear = _orderRepository.Table.Where(x => x.UserId == userId && x.OrderDate >= startDate && x.OrderDate <= endDate)?.Sum(y => y.Total);
                decimal limit = annualOrderLimit - orderTotalforYear.GetValueOrDefault();
                ZnodeLogging.LogMessage("Order limit:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, limit);
                ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return limit <= 0 ? 0 : limit;

            }
            else
                return 0;
        }

        //Get user account catalog details
        protected virtual void GetUserAccountCatalogDetails(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = model?.AccountId });

            if (IsNull(model.PublishCatalogId) || model.PublishCatalogId <= 0)
                GetPublishCatalogId(model);

            IZnodeRepository<ZnodeAccount> _accountRepository = new ZnodeRepository<ZnodeAccount>();

            if (model.AccountId > 0)
            {
                ZnodeAccount accountDetails = _accountRepository.GetById(model.AccountId.GetValueOrDefault());
                model.Accountname = accountDetails.Name;

                //Set Bussiness Identification Number.
                SetBusinessIdentificationNumber(model);

                if (accountDetails?.PublishCatalogId > 0)
                    model.PublishCatalogId = accountDetails?.PublishCatalogId;
                else if (accountDetails?.ParentAccountId > 0)
                {
                    ZnodeAccount parentAccountDetails = _accountRepository.GetById(accountDetails.ParentAccountId.GetValueOrDefault());
                    if (parentAccountDetails?.PublishCatalogId > 0)
                        model.PublishCatalogId = parentAccountDetails.PublishCatalogId;
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        protected virtual void SetBusinessIdentificationNumber(UserModel model)
        {
            List<GlobalAttributeValuesModel> globalLevelAttributeAccount = GetGlobalLevelAttributeList(model.AccountId.GetValueOrDefault(), ZnodeConstant.Account);
            model.BusinessIdentificationNumber = globalLevelAttributeAccount?.FirstOrDefault(x => string.Equals(x.AttributeCode, ZnodeConstant.BusinessIdentificationNumber, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue;
        }

        protected virtual void GetPublishCatalogId(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = model?.AccountId });

            int? portalCatalogId = _portalCatalogRepository.Table.Where(x => x.PortalId == model.PortalId)?.FirstOrDefault()?.PublishCatalogId;

            ZnodeLogging.LogMessage("PublishCatalogId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, portalCatalogId);

            if (portalCatalogId > 0)
                model.PublishCatalogId = portalCatalogId;
        }

        //Get address list.
        protected virtual List<AddressModel> GetAddressList(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = model?.AccountId });

            FilterCollection filters = new FilterCollection();

            List<string> navigationProperties = new List<string>();
            navigationProperties.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString());

            //If login user is B2B get account address list else get customer address list.
            if (_loginHelper.IsAccountCustomer(model) && model.AccountId > 0)
            {
                filters.Add(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, model.AccountId.ToString());

                IZnodeRepository<ZnodeAccountAddress> _accountAddressRepository = new ZnodeRepository<ZnodeAccountAddress>();
                return AccountMap.ToListModel(_accountAddressRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause, navigationProperties))?.AddressList;
            }
            else
            {
                filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, model.UserId.ToString());
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return UserMap.ToAddressListModel(_userAddressRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause, navigationProperties), null)?.AddressList;
            }

        }

        //Create guest user account.
        protected virtual UserModel CreateGuestUserAccount(UserModel userModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = userModel?.AccountId });

            //Save account details.
            ZnodeUser account = _userRepository.Insert(userModel?.ToEntity<ZnodeUser>());
            if (account?.UserId > 0 && IsNotNull(userModel))
            {
                userModel.UserId = account.UserId;
                ZnodeLogging.LogMessage(account.UserId > 0 ? Admin_Resources.SuccessUserDataSave : Admin_Resources.ErrorUserDataSave, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Insert portal in UserPortal table.
                _userPortalRepository.Insert(new ZnodeUserPortal { UserId = account.UserId, PortalId = userModel.PortalId });

                //Insert profile in AccountProfile table.
                _accountProfileRepository.Insert(new ZnodeUserProfile { UserId = account.UserId, ProfileId = userModel.ProfileId, IsDefault = true });
                ZnodeLogging.LogMessage("User Id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userModel?.UserId);
                return account.ToModel<UserModel>();

            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new UserModel();
        }

        //Map the login provider data domain wise.
        protected virtual List<SocialDomainModel> MapLoginProvider(List<ZnodePortalLoginProvider> allConfiguration, List<ZnodePortalLoginProvider> domainWise)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SocialDomainModel> socialDomainList = new List<SocialDomainModel>();
            //Maps the fields into model.
            foreach (ZnodePortalLoginProvider item in domainWise)
            {
                SocialDomainModel socialDomainModel = new SocialDomainModel();
                socialDomainModel.DomainId = item.DomainId;
                socialDomainModel.PortalId = item.PortalId;
                socialDomainModel.DomainName = item.ZnodeDomain?.DomainName;
                socialDomainModel.SocialTypeList = allConfiguration.Where(x => x.DomainId == item.DomainId).Select(type => new SocialTypeModel() { Key = string.IsNullOrEmpty(type.ProviderKey) ? ZnodeLogging.Components.Admin.ToString() : type.ProviderKey.Trim(), SecretKey = string.IsNullOrEmpty(type.SecretKey) ? string.Empty : type.SecretKey.Trim(), ProviderName = type.ZnodeLoginProvider?.Name }).ToList();
                socialDomainList.Add(socialDomainModel);
            }
            ZnodeLogging.LogMessage("socialDomainList list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, socialDomainList?.Count());
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return socialDomainList;
        }

        //Get portal id from filters.
        protected virtual int GetPortalId(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int portalId = 0;
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase)))
            {
                Int32.TryParse(filters.FirstOrDefault(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase))?.FilterValue, out portalId);
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("Portal Id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, portalId);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return portalId;
        }

        //Add guest user filter.
        protected virtual FilterCollection AddGuestUserFilter(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string filterValue = filters.Find(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
            filterValue = Convert.ToBoolean(filterValue) ? FilterKeys.ActiveTrue : FilterKeys.ActiveFalse;
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase));
            filters.Add(FilterKeys.IsGuestUser, FilterOperators.Equals, filterValue);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return filters;
        }

        //Set the email as an username for external login.
        protected virtual void SetUserNameForSocialLoginUser(UserModel model)
        {
            //If the user not exits by email then store email as a username else not.
            if (!string.IsNullOrEmpty(model.User.Email) && !_loginHelper.IsUserExists(model.Email, model.PortalId.GetValueOrDefault()))
                model.User.Username = model.Email;
        }

        //Update user data in user table.
        public virtual bool UpdateUser(UserModel userModel, bool webStoreUser)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = userModel.AccountId });

            bool updatedAccount = false;
            if (webStoreUser)
            {
                //Get User data and bind to update model.
                UserModel userdata = GetAndBindUserData(userModel);


                var _entity = userdata.ToEntity<ZnodeUser>();

                if (IsNotNull(userdata))
                    updatedAccount = _userRepository.Update(_entity);

                if (updatedAccount)
                    new ERPInitializer<ZnodeUser>(_entity, "EmailUpdate");
            }
            else
                updatedAccount = _userRepository.Update(userModel.ToEntity<ZnodeUser>());
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return updatedAccount;
        }

        //Update user data in user table.
        public virtual bool UpdateCustomer(UserModel userModel, bool webStoreUser)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = userModel.AccountId });

            bool updatedAccount = false;

            var _entity = userModel.ToEntity<ZnodeUser>();

            if (IsNotNull(userModel))
                updatedAccount = _userRepository.Update(_entity);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return updatedAccount;
        }

        //Gets the User Details by User Id.
        protected virtual UserModel GetUserByUserId(string userId, NameValueCollection expand)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId });

            if (!string.IsNullOrEmpty(userId))
            {
                //Create tuple to generate where clause.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserEnum.AspNetUserId.ToString(), ProcedureFilterOperators.Is, userId));

                //Generating where clause to get account details.
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                List<string> navigationProperties = GetExpands(expand);

                //This method is used to get the user details.
                ZnodeUser account = _userRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties, whereClauseModel.FilterValues);
                
                if (PortalId > 0 && HelperUtility.IsNotNull(account.ZnodeUserWishLists))
                    account.ZnodeUserWishLists = account.ZnodeUserWishLists.Where(x => x.PortalId == PortalId).ToList();
                
                if (IsNotNull(account))
                    //Bind the User details based on the expand collection
                    return BindUserDetails(UserMap.ToModel(account, string.Empty), expand);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new UserModel();
        }

        public virtual UserModel GetUserAccessPermission(int userId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId });

            IZnodeRepository<ZnodeAccountUserPermission> _userPermission = new ZnodeRepository<ZnodeAccountUserPermission>();

            //Get the user permission.
            return (from userPermission in _userPermission.Table
                    join accountPermission in _accountPermissionAccessRepository.Table on userPermission.AccountPermissionAccessId equals accountPermission.AccountPermissionAccessId
                    join accessPermission in _accessPermissionRepository.Table on accountPermission.AccessPermissionId equals accessPermission.AccessPermissionId
                    where userPermission.UserId == userId
                    select new UserModel()
                    {
                        PermissionCode = accessPermission.PermissionCode,
                        AccountPermissionAccessId = accountPermission.AccountPermissionAccessId,
                        PermissionsName = accessPermission.PermissionsName,
                        AccountUserPermissionId = userPermission.AccountUserPermissionId
                    })?.FirstOrDefault();
        }

        //Convert shopper to admin functionality.
        public virtual UserModel ConvertShopperToAdmin(UserModel model)
        {
            ZnodeLogging.LogMessage("Execution Started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = model.PortalId });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            if (IsNull(model.User))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.LoginUserModelNotNull);

            //Get user details.
            var aspNetZnodeUserId = _loginHelper.GetUser(model, null)?.AspNetZnodeUserId;
            ApplicationUser user = UserManager.FindByName(aspNetZnodeUserId);
            model.UserId = IsNotNull(_userRepository.Table.FirstOrDefault(x => x.Email.Equals(model.UserName))) ? _userRepository.Table.FirstOrDefault(x => x.Email.Equals(model.UserName)).UserId : 0;
            model.Email= string.IsNullOrEmpty(model.Email) ? model.UserName : model.Email;
            //Updates role into AspnetUserRole
            UpdateUserRole(model, user);

            //To convert shopper to admin, update accountId to null for the user.
            var userAccount = _userRepository.Table.FirstOrDefault(x => x.UserId == model.UserId);
            if (IsNotNull(userAccount))
            {
                userAccount.FirstName = string.IsNullOrEmpty(model.FirstName) ? userAccount.FirstName : model.FirstName;
                userAccount.LastName = string.IsNullOrEmpty(model.LastName) ? userAccount.LastName : model.LastName;
                userAccount.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? userAccount.PhoneNumber : model.PhoneNumber;
                userAccount.AccountId = null;
                _userRepository.Update(userAccount);
            }

            //associate the user to given portal ids. 
            var userPortals = _userPortalRepository.Table.Where(x => x.UserId == model.UserId)?.ToList();
            if (userPortals.Count > 0)
            {
                _userPortalRepository.Delete(userPortals);
            }

            if (model.PortalIds?.Length > 0)
            {
                foreach (var portalId in model.PortalIds)
                {
                    var userPortal = new ZnodeUserPortal() { UserId = model.UserId };
                    if (portalId.Equals("0"))
                        userPortal.PortalId = null;//If portalId not specified.
                    else
                        userPortal.PortalId = Convert.ToInt32(portalId);
                    _userPortalRepository.Insert(userPortal);
                }
            }
            //update portal id as null in AspNetZnodeUser
            var aspNetZnodeUser = _aspNetZnodeUserRepository.Table.FirstOrDefault(x => x.AspNetZnodeUserId == aspNetZnodeUserId);
            if (IsNotNull(aspNetZnodeUser))
            {
                aspNetZnodeUser.PortalId = null;
                _aspNetZnodeUserRepository.Update(aspNetZnodeUser);
            }

            //Remove all the associated user profiles.
            var userProfile = _userProfileRepository.Table.Where(x => x.UserId == model.UserId)?.ToList();
            if (IsNotNull(userProfile))
            {
                _userProfileRepository.Delete(userProfile);
            }
            if (IsNotNull(model))
            {
                SendShopperToAdminConversionMail(model.Email, model.LocaleId, userAccount.FirstName, userAccount.LastName);
                var onConvertCustomerToAdministratorInit = new ZnodeEventNotifier<UserModel>(model, EventConstant.OnConvertCustomerToAdministrator);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }

        //Check and validate the username is an existing Shopper or not.
        public virtual bool CheckIsUserNameAnExistingShopper(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                List<AspNetZnodeUser> aspNetZnodeUsers = _loginHelper.GetUserListByUsername(username);
                if (aspNetZnodeUsers?.Count > 0)
                {
                    AspNetZnodeUser aspNetZnodeUser = aspNetZnodeUsers.FirstOrDefault();

                    string aspNetZnodeUserId = aspNetZnodeUser?.AspNetZnodeUserId;

                    ApplicationUser user = UserManager.FindByName(aspNetZnodeUserId);

                    if (IsAdminUser(user))
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorUserNameAlreadyExists);

                    if (aspNetZnodeUsers?.Count > 1)
                        throw new ZnodeException(ErrorCodes.CustomerAccountError, Admin_Resources.ErrorMultipleShopperAccounts);
                    return true;
                }
                return false;
            }
            return false;
        }

        //Get the User Detail By User Id
        public virtual UserModel GetUserDetailById(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId });

            if (userId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.AccountIdNotLessThanOne);

            //This method is used to get the user details.
            ZnodeUser user = portalId > 0 ? _userRepository.Table.FirstOrDefault(x => x.UserId == userId && x.ZnodeUserPortals.Select(y => y.PortalId == portalId).FirstOrDefault()):
                                            _userRepository.Table.Where(x => x.UserId == userId).Include(x => x.ZnodeUserPortals)?.FirstOrDefault();

            //If user is global level, get its details.
            if (portalId > 0 && IsNull(user))
            {
                user = _userRepository.Table?.Where(x => x.UserId == userId)?.Include(x => x.ZnodeUserPortals)?.FirstOrDefault();

                if (IsNotNull(user) && user.ZnodeUserPortals?.FirstOrDefault()?.PortalId.GetValueOrDefault() > 0 && user.ZnodeUserPortals?.FirstOrDefault()?.PortalId != portalId)
                    return null;
            }

            if (IsNotNull(user))
            {
                ApplicationUser userDetails = UserManager.FindById(user.AspNetUserId);

                UserModel accountDetails = new UserModel();

                //get the lock status of the user
                var lockedStatus = userDetails?.LockoutEndDateUtc;
                accountDetails.IsLock = IsNotNull(lockedStatus);

                //In the case of anonymous user
                if (IsNull(userDetails))
                {
                    accountDetails.IsGuestUser = true;
                }

                return accountDetails;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }


        protected virtual void SendShopperToAdminConversionMail(string email, int localeId, string firstName, string lastName)
        {
            string messageText = string.Empty;
            try
            {
                string displayName = string.IsNullOrEmpty(firstName) ? email : firstName + " " + lastName;
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { displayName = displayName });

                //Method to get Email Template Details.
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ConvertCustomerToAdministrator, PortalId, localeId);

                if (IsNotNull(emailTemplateMapperModel))
                {
                    string logInUrl = $"<a href=\"{ZnodeApiSettings.AdminWebsiteUrl}\">Click Here</a>";
                    //Set parameters for the Email Template.
                    SetParameterOfShopperToAdminEmailTemplate(displayName, logInUrl);

                    //Replace the Email Template Keys, based on the passed email template parameters.
                    messageText = EmailTemplateHelper.ReplaceTemplateTokens(emailTemplateMapperModel.Descriptions);

                    ZnodeEmail.SendEmail(email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, 0, string.Empty), emailTemplateMapperModel.Subject, messageText, true);

                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
        }

        //Set Parameters for the Email Templates to be get replaced.
        protected virtual void SetParameterOfShopperToAdminEmailTemplate(string displayName, string logInUrl)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { displayName = displayName });

            EmailTemplateHelper.SetParameter("#DisplayName#", displayName);
            EmailTemplateHelper.SetParameter("#LogInUrl#", logInUrl);
            EmailTemplateHelper.SetParameter("#ContactNo#", Admin_Resources.AdminContactNo);
        }

        //Method to update guest user account.
        protected virtual void UpdateGuestUserAccount(UserModel userModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountId = userModel.AccountId });

            //Save the user login Details.
            ApplicationUser user = CreateOwinUser(userModel);

            //Save account details.
            _userRepository.Update(userModel.ToEntity<ZnodeUser>());

            ZnodeUser account = _userRepository.GetById(userModel.UserId);
            ZnodeLogging.LogMessage("UserId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose,userModel?.UserId );
            if (userModel?.UserId > 0)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessUpdateUserData, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Insert data for guest user.
                InsertDataForGuestUser(userModel, user, account);

                //Get the AspNet Znode user.
                AspNetZnodeUser znodeUser = _loginHelper.GetUser(userModel, (userModel.PortalId > 0) ? userModel.PortalId.Value : PortalId);
                ApplicationUser applicationUser = null;
                if (IsNotNull(znodeUser))
                {
                    //This method is used to find the User having valid username.
                    applicationUser = UserManager.FindByNameAsync(znodeUser.AspNetZnodeUserId).Result;
                }
                //map account model to account entity.
                if (!SendEmailToStoreAdministrator(userModel.FirstName, userModel.User.Password, userModel.Email, (userModel.PortalId > 0) ? userModel.PortalId.Value : PortalId, userModel.LocaleId, true, false, false, false, applicationUser.Id, userModel.UserName))
                {
                    UserModel accountModel = UserMap.ToModel(account, user.Email);
                    accountModel.IsEmailSentFailed = true;
                }
                UserMap.ToModel(account, user.Email);
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdateAccountData, string.Empty, TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.ExceptionalError, Admin_Resources.ErrorUpdateUser);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Get the values from filter.
        protected virtual FilterCollection GetValuesFromFilter(int loggedUserAccountId, FilterCollection filters, out bool isApproval, out bool isWebstore, out string currentUserName, out string currentRoleName, out int portalId, out bool isCustomerEditMode, out bool IsGuestUser, string userName = null, string roleName = null)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { loggedUserAccountId = loggedUserAccountId });

            isApproval = false;
            IsGuestUser = false;
            isWebstore = filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsWebstore, StringComparison.CurrentCultureIgnoreCase));
            bool IsStoreAdmin = filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsStoreAdmin, StringComparison.CurrentCultureIgnoreCase));

            if (filters.Any(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase)))
            {
                bool.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out IsGuestUser);
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase));
            }

            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsWebstore, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsStoreAdmin, StringComparison.InvariantCultureIgnoreCase));
            if (!IsNotNull(userName) && !IsNotNull(roleName))
            {
                UserModel userModel = GetUserById(loggedUserAccountId, null);
                currentUserName = userModel?.User?.Username;
                currentRoleName = IsStoreAdmin ? userModel?.RoleName : string.Empty;
            }
            else
            {
                currentUserName = userName;
                currentRoleName = IsStoreAdmin ? roleName : string.Empty;
            }
            //Check if filter contains guest user.
            if (filters.Any(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase)))
                filters = AddGuestUserFilter(filters);

            //Get portal id from filters.
            portalId = GetPortalId(filters);

            //Check if rolename exists in filters and remove it.
            IsRoleNameExists(filters, ref isApproval, ref currentUserName, ref currentRoleName);

            //Check if filters contains customereditmode and remove it.
            isCustomerEditMode = IsCustomerEditMode(filters);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return filters;
        }
        /// <summary>
        /// Set user id in Header specifally in case of new user created from webstore or password reset through link send on mail
        /// </summary>
        /// <param name="AspNetUserId"> Parameter used to get userId from AspNetUserId</param>
        protected virtual void SetRequestHeaderForPasswordLog(string AspNetUserId = null, int userId = 0)
        {
            if (HelperMethods.GetLoginUserId() <= 0)
            {
                if (!string.IsNullOrEmpty(AspNetUserId))
                {
                    userId = _userRepository.Table.Where(x => x.AspNetUserId == AspNetUserId).Select(x => x.UserId).FirstOrDefault();
                }
                if (userId != null && userId > 0)
                {
                    HttpContext.Current.Request.Headers.Add("Znode-UserId", Convert.ToString(userId));
                }
            }
        }

        //Generate Account Verification Email Content based on the Email Template
        protected virtual string GenerateAccountActivationStatusEmail(UserModel model, string contentBlockKey, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            string messageText = string.Empty;
            if (IsNull(model.Email))
            {
                return messageText;
            }
            portalId = (portalId == 0 && IsNotNull(model.PortalId.Value)) ? model.PortalId.Value : portalId;
            string firstName = string.IsNullOrEmpty(model.FirstName) ? model.Email : model.FirstName;
            string userName = string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName;

            //Method to get Email Template Details.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(contentBlockKey, portalId);
            if (IsNotNull(emailTemplateMapperModel))
            {
                PortalModel portalModel = GetCustomPortalDetails(portalId);

                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { storeLogo = portalModel?.StoreLogo });

                //Set Parameters for the Email Templates to be get replaced.
                Dictionary<string, string> setDictionary = SetParameterOfEmailTemplateForVerification(firstName, userName, portalModel?.StoreLogo, portalModel?.StoreName);

                //Replace the Email Template Keys, based on the passed email template parameters.
                messageText = _emailTemplateSharedService.ReplaceTemplateTokens(emailTemplateMapperModel.Descriptions, setDictionary);
                string subject = emailTemplateMapperModel.Subject;
                bool isHtml = true;
                bool isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                ZnodeEmail.SendEmail(portalId, model.Email, ZnodeEmailBase.SMTPUserName, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, isHtml);

            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return messageText;
        }
        //Set Parameters for the Email Templates to be get replaced.
        protected virtual Dictionary<string, string> SetParameterOfEmailTemplateForVerification(string firstName, string userName, string storeLogo, string storeName)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { firstName = firstName });
            return new Dictionary<string, string>
            {
                {"#FirstName#", firstName},
                {"#UserName#", userName},
                {"#StoreLogo#", storeLogo},
                { "#StoreName#", storeName }
            };
        }

        protected virtual bool VerifyAccountAndSendMail(ZnodeUser accountDetails, int portalId)
        {
            //Execute only when request is for unlocking and non verified.
            accountDetails.IsVerified = true;
            accountDetails.UserVerificationType = null;
            bool updateStatus =_userRepository.Update(accountDetails);
            var onAccountVerificationSuccessfulInit = new ZnodeEventNotifier<UserModel>(accountDetails.ToModel<UserModel>(), EventConstant.OnAccountVerificationSuccessful);
            GenerateAccountActivationStatusEmail(accountDetails.ToModel<UserModel>(), ZnodeConstant.AccountVerificationSuccessful, portalId);
            return updateStatus;
        }

        //Gets user Model based on email id.
        protected virtual bool IsSocialUserVerified(string email, UserVerificationTypeEnum userVerificationTypeEnum)
        {
            ZnodeUser userModel = _userRepository.Table.FirstOrDefault(x => x.Email == email);
            return IsSocialUserVerified(userModel, userVerificationTypeEnum);
        }

        //return bool on user verification status.
        protected virtual bool IsSocialUserVerified(ZnodeUser userModel, UserVerificationTypeEnum userVerificationTypeEnum)
        {
            if (IsNotNull(userModel) && !userModel.IsVerified && userVerificationTypeEnum == UserVerificationTypeEnum.AdminApprovalCode)
            {
                return false;
            }
            return true;
        }

        //Method to verify email verification type.
        protected virtual bool IsEmailVerificationType(UserVerificationTypeEnum userVerificationType)
        {
            if (userVerificationType == UserVerificationTypeEnum.EmailVerificationCode)
            {
                return true;
            }
            return false;
        }

        //Method to verify admin approval type.
        protected virtual bool IsAdminApprovalType(string userVerificationType)=> IsAdminApprovalType((UserVerificationTypeEnum)Enum.Parse(typeof(UserVerificationTypeEnum), userVerificationType));

        //Method to verify admin approval type.
        protected virtual bool IsAdminApprovalType(UserVerificationTypeEnum userVerificationType)
        {
            if (userVerificationType == UserVerificationTypeEnum.AdminApprovalCode)
            {
                return true;
            }
            return false;
        }

        //Method to set parameters for admin mail.
        protected virtual UserModel SetAdminMailParameters(UserModel userModel)
        {
            userModel.PortalId = IsNotNull(userModel.PortalId) ? userModel.PortalId : PortalId;
            userModel.FirstName = "admin";
            userModel.Email = _portalRepository.GetById(userModel.PortalId.Value)?.AdminEmail;
            return userModel;
        }

        //Set Media Path from Media Id
        protected virtual void SetMediaPath(UserModel accountDetails)
        {
            //Set media path for user image.
            if (accountDetails?.MediaId > 0)
            {
                MediaManagerModel mediaDetails = GetMediaDetails(accountDetails.MediaId.GetValueOrDefault());
                if (IsNotNull(mediaDetails))
                    accountDetails.MediaPath = string.IsNullOrEmpty(mediaDetails.MediaServerThumbnailPath) ? string.Empty : mediaDetails.MediaServerThumbnailPath;
            }
        }

        //This method is used to get media details from media Id.
        protected virtual MediaManagerModel GetMediaDetails(int mediaId)
            => (mediaId > 0) ? ZnodeDependencyResolver.GetService<IMediaManagerServices>().GetMediaByID(mediaId, null) : new MediaManagerModel();

        //Get Vendor Login Url if it is an admin user.
        protected virtual string GetVendorLoginUrlForAdminUser(bool isWebstoreUser, string userId, string vendorLoginUrl, string userName)
        {
            if (!isWebstoreUser && IsNotNull(userId))
            {
                UserManager.UpdateSecurityStamp(userId);
                //This method is used to generate password reset token.
                string passwordResetToken = UserManager.GeneratePasswordResetToken(userId);

                //Generate the Password Reset Url
                vendorLoginUrl = $"{vendorLoginUrl}/User/ResetPassword?passwordToken={WebUtility.UrlEncode(passwordResetToken)}&userName={WebUtility.UrlEncode(userName)}";
            }
            return vendorLoginUrl;
        }

        //To get & set user details for login. 
        protected virtual UserModel GetUserDetailsForLogin(UserModel model, string aspNetUserId)
        {
            UserModel userModel = _userRepository.Table?
            .Select(x => new UserModel
            {
                UserId = x.UserId,
                AspNetUserId = x.AspNetUserId,
                IsVerified = x.IsVerified,
                UserVerificationType = x.UserVerificationType,
                PhoneNumber = x.PhoneNumber,
                ExternalId = x.ExternalId,
                BudgetAmount = x.BudgetAmount
            })
            ?.FirstOrDefault(x => x.AspNetUserId == aspNetUserId);

            if (IsNotNull(model) && IsNotNull(userModel))
            {
                model.PhoneNumber = userModel.PhoneNumber;
                model.ExternalId = userModel.ExternalId;
                model.BudgetAmount = userModel.BudgetAmount;
            }

            return userModel;
        }

        //Get user account details
        public virtual UserModel GetCustomerAccountdetails(int userId)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { UserId = userId });
                //This method check the access of manage screen for sales rep user
                ValidateAccessForSalesRepUser(ZnodeConstant.AccountUser, userId);
                UserModel userModel = null;
                IZnodeViewRepository<UserModel> objStoredProc = new ZnodeViewRepository<UserModel>();

                //Stored Procedure parameters
                objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);

                userModel = objStoredProc.ExecuteStoredProcedureList("Znode_AdminUserDetailsByUserId  @UserId")?.FirstOrDefault();

                //To verify whether the user is a tradecentric user.
                if (IsNotNull(userModel))
                {
                    userModel.IsTradeCentricUser = new ZnodeRepository<ZnodeTradeCentricUser>().Table.Any(x => x.UserId.Equals(userModel.UserId));
                }

                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return userModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.API.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get paged user account list
        public virtual List<UserModel> GetUserListForWebstore(int loggedUserAccountId, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.API.ToString(), TraceLevel.Verbose, new { LoggedUserAccountId = loggedUserAccountId });

            List<UserModel> userAccountList = null;

            IZnodeViewRepository<UserModel> objStoredProc = new ZnodeViewRepository<UserModel>();

            objStoredProc.SetParameter("@LoggedInUserId", loggedUserAccountId, ParameterDirection.Input, DbType.Int32);

            userAccountList = objStoredProc.ExecuteStoredProcedureList("Znode_AdminUsersEditProfile @LoggedInUserId")?.ToList();

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);

            return userAccountList;
        }

        // Pay Invoice
        public virtual OrderModel PayInvoice(PayInvoiceModel payInvoiceModel)
        {
            IOrderService _orderService = GetService<IOrderService>();

            OrderModel orderDetail = _orderService.GetOrderById(payInvoiceModel.OmsOrderId, null, new NameValueCollection());

            orderDetail.PaymentSettingId = payInvoiceModel?.PaymentDetails?.PaymentSettingId;
            payInvoiceModel.UserId = payInvoiceModel.UserId > 0 ? payInvoiceModel.UserId : orderDetail.UserId;

            ShoppingCartModel shoppingCartModel = GetShoppingCartModel(orderDetail);
            shoppingCartModel.UserDetails = GetUserById(payInvoiceModel.UserId, new NameValueCollection());
            shoppingCartModel.BillingAddress = shoppingCartModel.BillingAddress ?? orderDetail.BillingAddress;
            IPaymentHelper paymentHelper = GetService<IPaymentHelper>();
            //Process For Payment
            GatewayResponseModel gatewayResponseModel = paymentHelper.ProcessPayment(payInvoiceModel, shoppingCartModel);

            decimal updatedRemainingAmount =  UpdateRemainingAmount(payInvoiceModel, orderDetail);
            _orderService.GetPaymentHistory(orderDetail);

            if (gatewayResponseModel.IsSuccess)
            {
                orderDetail.RemainingOrderAmount = updatedRemainingAmount;
                //Get generated unique order number on basis of current date.
                return orderDetail;
            }
            else
            {
                ZnodeLogging.LogMessage("Error while processing payment", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new OrderModel();
            }
       
        }

        // Pay Invoice
        public virtual decimal UpdateRemainingAmount(PayInvoiceModel payInvoiceModel, OrderModel orderDetail)
        {
            ZnodeOmsOrderDetail znodeOmsOrderDetail = _orderDetailRepository.Table.FirstOrDefault(x => x.OmsOrderId == payInvoiceModel.OmsOrderId && x.IsActive);

            znodeOmsOrderDetail.RemainingOrderAmount = orderDetail.RemainingOrderAmount.GetValueOrDefault() - payInvoiceModel.PaymentDetails.PaymentAmount;
            bool isSuccess = _orderDetailRepository.Update(znodeOmsOrderDetail);

            //Add order payment details
            IOrderService _orderService = GetService<IOrderService>();
            OrderPaymentDataModel orderPaymentModel = MapOrderPaymentDetails(payInvoiceModel);
            orderPaymentModel.RemainingOrderAmount = znodeOmsOrderDetail.RemainingOrderAmount.GetValueOrDefault();

            _orderService.SaveOrderPaymentDetail(orderPaymentModel);//OrderPaymentDataModel

            return isSuccess ? znodeOmsOrderDetail.RemainingOrderAmount.GetValueOrDefault() : 0;

        }

        //Get shopping cart model
        public virtual ShoppingCartModel GetShoppingCartModel(OrderModel orderDetail)
        {
            IQuoteService _quoteService = GetService<IQuoteService>();
            IShoppingCartService _checkoutService = GetService<IShoppingCartService>();

            CartParameterModel cartParameterModel = _quoteService.ToCartParameterModel(orderDetail.UserId, orderDetail.PortalId, 0, orderDetail.ShippingId, 0, orderDetail.OmsOrderId);

            ShoppingCartModel shoppingCartModel = _checkoutService.GetShoppingCartDetails(cartParameterModel);

            return shoppingCartModel;
        }

        // To update username in AspNetZnodeUser, ZnodeUser tables.
        public virtual BooleanModel UpdateUsernameForRegisteredUser(UserDetailsModel userDetailsModel)
        {
            ZnodeLogging.LogMessage("Update username execution started:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { userName = userDetailsModel?.UserName, userId = userDetailsModel?.UserId, portalId = userDetailsModel?.PortalId });

            BooleanModel response = new BooleanModel();
            ZnodeUser znodeUserDetails = _userRepository.Table.FirstOrDefault(x => x.UserId == userDetailsModel.UserId);

            ValidateRequestForUsernameUpdate(userDetailsModel, znodeUserDetails);

            // To get the AspNetZnodeUser table details by UserId.
            AspNetZnodeUser aspNetZnodeUserDetails =
                    (from znodeUser in _userRepository.Table
                     join aspNetUser in _aspNetUserRepository.Table on znodeUser.AspNetUserId equals aspNetUser.Id
                     join aspNetZnodeUser in _aspNetZnodeUserRepository.Table on aspNetUser.UserName equals aspNetZnodeUser.AspNetZnodeUserId
                     where znodeUser.UserId == znodeUserDetails.UserId
                     select aspNetZnodeUser)?.FirstOrDefault();
            
            // To update the username in the AspnetZnodeUser table.
            aspNetZnodeUserDetails.UserName = userDetailsModel?.UserName;
            response.IsSuccess = _aspNetZnodeUserRepository.Update(aspNetZnodeUserDetails);                

            if (response.IsSuccess)
            {
                // To update the username in ZnodeUser table.
                znodeUserDetails.UserName = userDetailsModel?.UserName;
                response.IsSuccess = _userRepository.Update(znodeUserDetails);
            }           

            ZnodeLogging.LogMessage("Update username execution ended:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return response;
        }

        // To validate the user details before proceeding for the username update.
        protected virtual void ValidateRequestForUsernameUpdate(UserDetailsModel userDetailsModel, ZnodeUser znodeUserDetails)
        {
            // To check userId is valid or not.
            if (userDetailsModel?.UserId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.InvalidUserId);

            // To get user details by UserId
            if (IsNull(znodeUserDetails))
                throw new ZnodeException(ErrorCodes.NotFound, Api_Resources.DetailsNotFound);

            // To check the user is guest or not based on IsVerified flag available in the Znodeuser table.            
            if (!znodeUserDetails.IsVerified)
                throw new ZnodeException(ErrorCodes.NotPermitted, Api_Resources.ErrorIsGuestUser);

            // To check whether username already exists or not.
            bool isUsernameExists = _aspNetZnodeUserRepository.Table.Any(x => x.UserName.Equals(userDetailsModel.UserName, StringComparison.InvariantCultureIgnoreCase) && (x.PortalId == userDetailsModel.PortalId));
            
            if (isUsernameExists)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.UsernameAlreadyExists);
        }

        //Map user model with znodeUser entity
        protected virtual UserModel MapUserModel(ZnodeUser entity, string email, UserModel userModel, ApplicationUser user, ConvertDataTableToList dataTable, DataSet dataset)
        {
            if (IsNull(entity))
                return new UserModel();

            UserModel model = entity.ToModel<UserModel>();

            model = UserMap.ToModel(entity, email);
            model.RoleName = userModel.RoleName;
            model.PermissionCode = userModel.PermissionCode;
            model.ProfileId = Convert.ToInt32(userModel.Profiles?.Select(x => x.ProfileId).FirstOrDefault());
            model.IsAdminUser = IsAdminUser(user);
            model.User.RoleId = user.Roles.Select(s => s.RoleId).FirstOrDefault();
            model.RoleId = model.User.RoleId;
            model.Addresses = dataTable.ConvertDataTable<AddressModel>(dataset.Tables[ZnodeConstant.AddressList]);
            List<ZnodeUserWishList> znodeUserWishLists = dataTable.ConvertDataTable<ZnodeUserWishList>(dataset.Tables[ZnodeConstant.ZnodeUserWishLists]);
            model.Profiles = dataTable.ConvertDataTable<ProfileModel>(dataset.Tables[ZnodeConstant.ZnodeUserProfiles]);
            model.WishList = znodeUserWishLists?.ToModel<WishListModel>().ToList();

            return model;

        }

        //Returns the dataset consisting of AspNetUserId,PortalId.
        protected virtual DataSet GetUserDetailsByAspNetUserId(string aspNetUserId, int? portalId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            objStoredProc.GetParameter("@AspNetUserId", aspNetUserId, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);

            return objStoredProc.GetSPResultInDataSet("Znode_GetUserDetailsByAspNetUserId");
        }

        //Set DataSet table names.
        protected virtual void SetDataTableNames(DataSet dataset)
        {
            dataset.Tables[0].TableName = ZnodeConstant.ZnodeUser;
            dataset.Tables[1].TableName = ZnodeConstant.AddressList;
            dataset.Tables[2].TableName = ZnodeConstant.ZnodeUserWishLists;
            dataset.Tables[3].TableName = ZnodeConstant.ZnodeUserProfiles;
            dataset.Tables[4].TableName = ZnodeConstant.UserPortalStatus;
        }

        //Check User Portal Access
        protected virtual void CheckUserPortalAccess(int portalId, DataSet dataset)
        {
            if (portalId > 0)
            {
                bool hasPortalAccess = dataset.Tables[ZnodeConstant.UserPortalStatus].Rows[0].Field<bool>(ZnodeConstant.UserPortalStatus);

                if (!hasPortalAccess)
                    throw new ZnodeUnauthorizedException(ErrorCodes.NotPermitted, Admin_Resources.UserDontHaveTheRequestedPortalAccess, HttpStatusCode.Unauthorized);

            }
        }

        protected virtual  string GetDecryptSMTPUserName(string UserName)
        {
            string SMTPUserName = string.Empty;

            ZnodeEncryption encrypt = new ZnodeEncryption();
            SMTPUserName = encrypt.DecryptData(UserName);

            return SMTPUserName;
        }

        //Get the user registration attempt Id.
        protected virtual int? GetUserRegisteredAttemptId(string userName, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { userName = userName, portalId = portalId });

            IZnodeRepository<ZnodeUserRegistrationAttempt> _userRegistrationAttemptRepository = new ZnodeRepository<ZnodeUserRegistrationAttempt>();
            return (from user in _userRegistrationAttemptRepository.Table where user.UserName == userName && user.PortalId == portalId select (user.Id)).FirstOrDefault();
        }

        //Crete user registration attempt record.
        protected virtual void CreateUserRegisteredAttempt(UserModel userModel)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { userName = userModel.User.Username, portalId = userModel.PortalId.GetValueOrDefault() });
            ZnodeUserRegistrationAttempt znodeUserRegistrationAttempt = new ZnodeUserRegistrationAttempt { UserName = userModel.User.Username, PortalId = userModel.PortalId.GetValueOrDefault(), CreatedDate = DateTime.UtcNow };
            IZnodeRepository<ZnodeUserRegistrationAttempt> _userRegistrationAttemptRepository = new ZnodeRepository<ZnodeUserRegistrationAttempt>();
            _userRegistrationAttemptRepository.Insert(znodeUserRegistrationAttempt);
        }

        // Map order payment details.
        protected virtual OrderPaymentDataModel MapOrderPaymentDetails(PayInvoiceModel payInvoiceModel)
        {
            OrderPaymentDataModel orderPaymentModel = new OrderPaymentDataModel();
            if (HelperUtility.IsNotNull(orderPaymentModel))
            {
                orderPaymentModel.OmsOrderId = payInvoiceModel.OmsOrderId;
                orderPaymentModel.TransactionReference = payInvoiceModel.PaymentDetails.TransactionId;
                orderPaymentModel.Total = payInvoiceModel.PaymentDetails.PaymentAmount;
                orderPaymentModel.TransactionStatus = ZnodePaymentStatus.PENDING.ToString();
                orderPaymentModel.PaymentSettingId = payInvoiceModel.PaymentDetails.PaymentSettingId;
                orderPaymentModel.TransactionDate = Convert.ToDateTime(payInvoiceModel.PaymentDetails.TransactionDate);
                orderPaymentModel.CreatedBy = payInvoiceModel.UserId;
            }
            return orderPaymentModel;
        }
        public virtual PortalModel GetPortalDomain(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            PortalModel model = new PortalModel();
            if (portalId > 0)
            {
                IZnodeRepository<ZnodeDomain> znodePortalDomain = new ZnodeRepository<ZnodeDomain>();

                model = (from domain in znodePortalDomain.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ApplicationTypesEnum.WebStore.ToString() && x.IsActive && x.IsDefault)
                         where domain.PortalId == portalId
                         select new PortalModel
                         {
                             DomainUrl = domain.DomainName,
                         }).FirstOrDefault();
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return model;
        }

        #endregion
    }
}