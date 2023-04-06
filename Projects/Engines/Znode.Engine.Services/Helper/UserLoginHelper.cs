using System.Collections.Generic;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class UserLoginHelper : IUserLoginHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<AspNetZnodeUser> _userRepository;
        private readonly IZnodeRepository<AspNetUser> _aspNetUserRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _znodeUserPortalRepository;
        private readonly IZnodeRepository<ZnodeUser> _ZnodeUserRepository;

        #endregion

        #region Constructor
        public UserLoginHelper()
        {
            _userRepository = new ZnodeRepository<AspNetZnodeUser>();
            _aspNetUserRepository = new ZnodeRepository<AspNetUser>();
            _znodeUserPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _ZnodeUserRepository = new ZnodeRepository<ZnodeUser>();
        }
        #endregion

        #region Protected Method
        //Generates the Unique Id.
        protected virtual string GetUniqueId()
             => System.Guid.NewGuid().ToString();

        //Check the User Existence, in case of Global User creation is false.To allow only valid user to create the account.
        protected virtual bool CheckUserExistByPortal(string username, int? portalId)
        {
            //Get all the users based on the username.
            var availableUsers = _userRepository.Table.Where(x => x.UserName == username)?.ToList();
            if (availableUsers?.Count > 0)
            {
                //Check whether existing user has all portal access. If Yes, then not allow to create another one.
                if (availableUsers.Any(x => x.PortalId == null))
                    return true;

                //Check whether user has particular portal access or all portal access. 
                //If Particular Portal, then check whether same entry already exists or not.
                //If all portal, then not allow to create user, as there is already particular portal entry is exists.
                if (portalId > 0)
                    return availableUsers.Any(x => x.PortalId == portalId);
                else
                    return true;
            }
            return false;
        }
        #endregion

        #region Public Methods
        //Check whether the User Already exists or not.
        public virtual bool IsUserExists(string username, int portalId)
        {
            bool isUserExists = false;
            if (!string.IsNullOrEmpty(username))
            {
                isUserExists = (DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                     ? _userRepository.Table.Where(x => x.UserName == username).Any()
                     : CheckUserExistByPortal(username, portalId);
            }
            return isUserExists;
        }

        //Insert New User Details.
        public virtual AspNetZnodeUser CreateUser(UserModel model)
        {
            //Check whether the user already exists or not.
            if (!IsUserExists(model.User.Username, model.PortalId.GetValueOrDefault()))
            {
                //Insert the User details.
                return _userRepository.Insert(new AspNetZnodeUser() { AspNetZnodeUserId = GetUniqueId(), UserName = model.User.Username, PortalId = (model.PortalId > 0) ? model.PortalId : (int?)null });
            }
            return null;
        }

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUser(UserModel model, int? portalId = 0)
            => GetUserInfo(model.User.Username, portalId);

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUserById(string userId)
            => new ZnodeRepository<AspNetZnodeUser>().Table.FirstOrDefault(x => x.AspNetZnodeUserId == userId);

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUserInfo(string username, int? portalId)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var availableUsers = (from userPortal in _znodeUserPortalRepository.Table
                                      join user in _ZnodeUserRepository.Table on userPortal.UserId equals user.UserId
                                      join aspNetUser in _aspNetUserRepository.Table on user.AspNetUserId equals aspNetUser.Id
                                      join aspnetZnodeUser in _userRepository.Table on aspNetUser.UserName equals aspnetZnodeUser.AspNetZnodeUserId
                                      where aspnetZnodeUser.UserName == username
                                      select new
                                      {
                                          AspNetUserId = user.AspNetUserId,
                                          PortalId = aspnetZnodeUser.PortalId,
                                          AspNetZnodeUserId = aspnetZnodeUser.AspNetZnodeUserId
                                      }).ToList();

                if (availableUsers?.Count > 0)
                {
                    //Select the only user which is having portal access.
                    var selectedUser = availableUsers.FirstOrDefault(x => x.PortalId == portalId || x.PortalId == null || portalId == null);
                    if (HelperUtility.IsNotNull(selectedUser))
                        return _userRepository.Table.FirstOrDefault(x => x.AspNetZnodeUserId == selectedUser.AspNetZnodeUserId);
                }
            }
            return null;
        }

        //Get user list for same username associated with different portals.
        public virtual List<AspNetZnodeUser> GetUserListByUsername(string username)
        {
            List<AspNetZnodeUser> availableUsers = null;
            if (!string.IsNullOrEmpty(username))
            {
                availableUsers = (from userPortal in _znodeUserPortalRepository.Table
                                  join user in _ZnodeUserRepository.Table on userPortal.UserId equals user.UserId
                                  join aspNetUser in _aspNetUserRepository.Table on user.AspNetUserId equals aspNetUser.Id
                                  join aspnetZnodeUser in _userRepository.Table on aspNetUser.UserName equals aspnetZnodeUser.AspNetZnodeUserId
                                  where aspnetZnodeUser.UserName == username
                                  select  aspnetZnodeUser)?.ToList();
            }
            return availableUsers;
        }
        
        //Returns true if the user belongs to account else false.
        public virtual bool IsAccountCustomer(UserModel model)
         => !string.IsNullOrEmpty(model.RoleName) && (model.RoleName.Equals(ZnodeRoleEnum.User.ToString()) || model.RoleName.Equals(ZnodeRoleEnum.Manager.ToString()) || model.RoleName.Equals(ZnodeRoleEnum.Administrator.ToString()));

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUserInfoByAspNetUserId(UserModel model, int? portalId = 0)
            => GetUserInfoByAspNetUserId(model.User.Username, portalId);

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUserInfoByAspNetUserId(string AspNetUserId, int? portalId)
        {
            if (!string.IsNullOrEmpty(AspNetUserId))
            {
                var availableUsers = (from userPortal in _znodeUserPortalRepository.Table
                                      join user in _ZnodeUserRepository.Table on userPortal.UserId equals user.UserId
                                      join aspNetUser in _aspNetUserRepository.Table on user.AspNetUserId equals AspNetUserId
                                      join aspnetZnodeUser in _userRepository.Table on aspNetUser.UserName equals aspnetZnodeUser.AspNetZnodeUserId
                                      where aspNetUser.Id == AspNetUserId
                                      select new
                                      {
                                          user.AspNetUserId,
                                          aspnetZnodeUser.PortalId,
                                          aspnetZnodeUser.AspNetZnodeUserId,
                                          aspnetZnodeUser.UserName
                                      }).FirstOrDefault();


                if (HelperUtility.IsNotNull(availableUsers))
                    return _userRepository.Table.FirstOrDefault(x => x.AspNetZnodeUserId == availableUsers.AspNetZnodeUserId);
            }
            return null;
        }

        //Get if user is exists by username and aspNetUserId.
        public virtual AspNetZnodeUser GetIsExistsByAspNetUserId(UserModel userModel)
        {
            if (!string.IsNullOrEmpty(userModel.AspNetUserId))
            {
                var availableUsers = (from userPortal in _znodeUserPortalRepository.Table
                                      join user in _ZnodeUserRepository.Table on userPortal.UserId equals user.UserId
                                      join aspNetUser in _aspNetUserRepository.Table on user.AspNetUserId equals userModel.AspNetUserId
                                      join aspnetZnodeUser in _userRepository.Table on aspNetUser.UserName equals aspnetZnodeUser.AspNetZnodeUserId
                                      where aspNetUser.Id == userModel.AspNetUserId && aspnetZnodeUser.UserName == userModel.UserName
                                      select new
                                      {
                                          user.AspNetUserId,
                                          aspnetZnodeUser.PortalId,
                                          aspnetZnodeUser.AspNetZnodeUserId,
                                          aspnetZnodeUser.UserName
                                      }).ToList();

                if (availableUsers?.Count > 0)
                {
                    //Select the only user which is having portal access.
                    var selectedUser = availableUsers.FirstOrDefault(x => x.PortalId == userModel.PortalId || x.PortalId == null || userModel.PortalId == null);
                    if (HelperUtility.IsNotNull(selectedUser))
                        return _userRepository.Table.FirstOrDefault(x => x.AspNetZnodeUserId == selectedUser.AspNetZnodeUserId);
                }
            }
            return null;
        }

        //Get the Znode User Details.
        public virtual AspNetZnodeUser GetUserDetails(UserModel userModel, int? portalId)
        {
            if (string.IsNullOrEmpty(userModel.User.Username))
                return null;

            DataSet dataSet = GetDataSetByUserName(userModel.User.Username, portalId);

            if (HelperUtility.IsNull(dataSet))
                return null;

            SetDataTableNames(dataSet);
            ConvertDataTableToList dataTable = new ConvertDataTableToList();
            AspNetZnodeUser aspNetZnodeUser = dataTable.ConvertDataTable<AspNetZnodeUser>(dataSet.Tables[ZnodeConstant.AspNetZnodeUser])?.FirstOrDefault();
            AspNetRole aspNetRole = dataTable.ConvertDataTable<AspNetRole>(dataSet.Tables[ZnodeConstant.RoleName])?.FirstOrDefault();
            ZnodeAccessPermission znodeAccessPermission = dataTable.ConvertDataTable<ZnodeAccessPermission>(dataSet.Tables[ZnodeConstant.PermissionCode])?.FirstOrDefault();
            ZnodePortalCatalog znodePortalCatalog = dataTable.ConvertDataTable<ZnodePortalCatalog>(dataSet.Tables[ZnodeConstant.PublishCatalogId])?.FirstOrDefault();
            userModel.PermissionCode = znodeAccessPermission?.PermissionCode;
            userModel.RoleName = aspNetRole?.Name;
            userModel.PublishCatalogId = znodePortalCatalog?.PublishCatalogId;
            return aspNetZnodeUser;
        }

        //returns the dataset consisting of AspNetZnodeUser,RoleName,PermissionCode.
        protected virtual DataSet GetDataSetByUserName(string userName, int? portalId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            objStoredProc.GetParameter("@UserName", userName, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);
            return objStoredProc.GetSPResultInDataSet("Znode_GetUserDetailsByUserName");
        }

        //Set DataSet table names.
        protected virtual void SetDataTableNames(DataSet dataset)
        {
            dataset.Tables[0].TableName = ZnodeConstant.AspNetZnodeUser;
            dataset.Tables[1].TableName = ZnodeConstant.RoleName;
            dataset.Tables[2].TableName = ZnodeConstant.PermissionCode;
            dataset.Tables[3].TableName = ZnodeConstant.PublishCatalogId;
        }
        #endregion
    }
}
