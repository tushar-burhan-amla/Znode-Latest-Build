using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    [Obsolete("This class is marked as Obsolete, instead use the UserLoginHelper.")]
    public static class LoginHelper
    {
        private static readonly IZnodeRepository<AspNetZnodeUser> _userRepository = new ZnodeRepository<AspNetZnodeUser>();
        private static readonly IZnodeRepository<AspNetUser> _aspNetUserRepository = new ZnodeRepository<AspNetUser>();
        private static readonly IZnodeRepository<ZnodeUserPortal> _znodeUserPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
        private static IZnodeRepository<ZnodePortalAccount> _znodeAccountPortalRepository = new ZnodeRepository<ZnodePortalAccount>();
        private static readonly IZnodeRepository<ZnodeUser> _ZnodeUserRepository = new ZnodeRepository<ZnodeUser>();

        //Check whether the User Already exists or not.
        public static bool IsUserExists(string username, int portalId)
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
        public static AspNetZnodeUser CreateUser(UserModel model)
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
        public static AspNetZnodeUser GetUser(UserModel model, int? portalId = 0)
            => GetUserInfo(model.User.Username, portalId);

        //Get the Znode User Details.
        public static AspNetZnodeUser GetUserById(string userId)
            => new ZnodeRepository<AspNetZnodeUser>().Table.FirstOrDefault(x => x.AspNetZnodeUserId == userId);

        //Generates the Unique Id.
        private static string GetUniqueId()
             => System.Guid.NewGuid().ToString();

        //Check the User Existence, in case of Global User creation is false.To allow only valid user to create the account.
        private static bool CheckUserExistByPortal(string username, int? portalId)
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

        //Get the Znode User Details.
        public static AspNetZnodeUser GetUserInfo(string username, int? portalId)
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
                    var selectedUser = availableUsers.FirstOrDefault(x => x.PortalId == portalId || x.PortalId == null|| portalId == null);
                    if (HelperUtility.IsNotNull(selectedUser))
                        return _userRepository.Table.FirstOrDefault(x => x.AspNetZnodeUserId == selectedUser.AspNetZnodeUserId);
                }
            }
            return null;
        }

        //Returns true if the user belongs to account else false.
        public static bool IsAccountCustomer(UserModel model)
         => !string.IsNullOrEmpty(model.RoleName) && (model.RoleName.Equals(ZnodeRoleEnum.User.ToString()) || model.RoleName.Equals(ZnodeRoleEnum.Manager.ToString()) || model.RoleName.Equals(ZnodeRoleEnum.Administrator.ToString()));
    }
}
