using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class AccessPermissionCache : BaseCache, IAccessPermissionCache
    {
        private readonly IAccessPermissionService _service;

        public AccessPermissionCache(IAccessPermissionService accountService)
        {
            _service = accountService;
        }

        //Get user account list
        public virtual string AccountPermissionList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AccessPermissionListModel Accounts = _service.AccountPermissionList(Expands, Filters, Sorts, Page);
                if (Accounts?.AccountPermissions?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AccessPermissionResponse response = new AccessPermissionResponse { AccessPermissionList = Accounts };
                    response.MapPagingDataFromModel(Accounts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get user account list
        public virtual string AccessPermissionList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AccessPermissionListModel Accounts = _service.AccessPermissionList(Expands, Filters, Sorts, Page);
                if (Accounts?.AccountPermissions?.Count > 0)
                {
                    AccessPermissionResponse response = new AccessPermissionResponse { AccessPermissionList = Accounts };
                    response.MapPagingDataFromModel(Accounts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get user account list
        public virtual string GetAccountPermission(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AccessPermissionModel Accounts = _service.GetAccountPermission(Expands, Filters);
                if (!Equals(Accounts, null))
                {
                    //Get response and insert it into cache.
                    AccessPermissionResponse response = new AccessPermissionResponse { AccessPermission = Accounts };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}