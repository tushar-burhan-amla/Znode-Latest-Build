using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class UserCache : BaseCache, IUserCache
    {
        private readonly IUserService _service;

        public UserCache()
        {
        }
        public UserCache(IUserService userService)
        {
            _service = userService;
        }

        //Get user by user id.
        public string GetUser(int userId, string routeUri, string routeTemplate, int portalId = 0)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserModel user = _service.GetUserById(userId, Expands, portalId);
                if (HelperUtility.IsNotNull(user))
                {
                    UserResponse response = new UserResponse { User = user, ErrorCode = 0 };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get user by username.
        public string GetUserByUsername(string username, int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserModel account = _service.GetUserByUsername(username, portalId);
                if (HelperUtility.IsNotNull(account))
                {
                    UserResponse response = new UserResponse { User = account, ErrorCode = 0 };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get user account list
        public string GetUserList(int loggedUserAccountId, string routeUri, string routeTemplate, string columnList = "")
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            UserListModel list = null;

            if (string.IsNullOrEmpty(data))           
                list = _service.GetUserList(loggedUserAccountId, Expands, Filters, Sorts, Page, columnList);

            return SetUserListInCache(list, data, routeUri, routeTemplate);           
        }

        public string GetUserListForAdmin(int loggedUserAccountId, string routeUri, string routeTemplate, string userName = null, string roleName = null, string columnList = "")
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            UserListModel list = null;

            if (string.IsNullOrEmpty(data))           
                list = _service.GetUserListForAdmin(loggedUserAccountId, userName, roleName, Expands, Filters, Sorts, Page, columnList); ;
            
            return SetUserListInCache(list, data, routeUri, routeTemplate);
        }

        public string SetUserListInCache(UserListModel list, string data, string routeUri, string routeTemplate)
        {
            if (list?.Users?.Count > 0)
            {
                //Get response and insert it into cache.
                UserListResponse response = new UserListResponse { Users = list.Users };
                response.MapPagingDataFromModel(list);

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get UnAssociated Customer(s).
        public string GetUnAssociatedCustomerList(int portalId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //User list
                UserListModel list = _service.GetUnAssociatedCustomerList(portalId, Expands, Filters, Sorts, Page);
                if (list?.Users?.Count > 0)
                {
                    //Get response and insert it into cache.
                    UserListResponse response = new UserListResponse { Users = list.Users };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Sales Rep list
        public string GetSalesRepForAssociation(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //User list
                UserListModel list = _service.GetSalesRepListForAssociation(Expands, Filters, Sorts, Page);
                if (list?.Users?.Count > 0)
                {
                    //Get response and insert it into cache.
                    UserListResponse response = new UserListResponse { Users = list.Users };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        //Get user account list
        public string GetPortalIds(string aspNetUserId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //User list
                UserPortalModel userPortal = _service.GetPortalIds(aspNetUserId);
                if (HelperUtility.IsNotNull(userPortal))
                {
                    //Get response and insert it into cache.
                    UserResponse response = new UserResponse { UserPortal = userPortal };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Sales Rep list
        public string GetSalesRepForAccount(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //User list
                SalesRepUserListModel list = _service.GetSalesRepListForAccount(Expands, Filters, Sorts, Page);
                if (list?.SalesRepUsers?.Count > 0)
                {
                    //Get response and insert it into cache.
                    SalesRepUserListResponse response = new SalesRepUserListResponse { SalesRepUsers = list.SalesRepUsers };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Logged in the User.
        public UserModel Login(int portalId, UserModel model, out int? errorCode)
            => _service.Login(portalId, model, out errorCode, Expands);

        //Get user account details
        public string GetCustomerAccountdetails(int userId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //User details
                UserModel userModel = _service.GetCustomerAccountdetails(userId);
                if (HelperUtility.IsNotNull(userModel))
                {
                    //Get response and insert it into cache.
                    UserResponse response = new UserResponse { User = userModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Remove all registration use attempt details.
        public bool RemoveUserRegistrationAttemptDetail()
            => _service.ClearAllUserRegisterAttempts();

        //Get user detail by user id.
        public string GetUserDetailById(int userId, string routeUri, string routeTemplate, int portalId)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserModel user = _service.GetUserDetailById(userId, portalId);
                if (HelperUtility.IsNotNull(user))
                {
                    UserResponse response = new UserResponse { User = user, ErrorCode = 0 };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }
    }
}