using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreAccountCache : BaseCache, IWebStoreAccountCache
    {
        #region Private Variables
        private readonly IAccountService _service;
        #endregion

        #region Constructor
        public WebStoreAccountCache(IAccountService storeAccountService)
        {
            _service = storeAccountService;
        }
        #endregion

        #region Public Methods
        //Get list of User Address.
        public virtual string GetUserAddressList(string routeUri, string routeTemplate)
        {
            //Remove data from cache.
            HttpRuntime.Cache.Remove(routeUri);
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (HelperUtility.IsNull(data))
            {
                //Get User Address list.
                AddressListModel list = _service.GetUserAddressList(Expands, Filters, Sorts, Page);
                if (list?.AddressList?.Count > 0)
                {
                    //Create response.
                    WebStoreAccountResponse response = new WebStoreAccountResponse { UserAddressList = list.AddressList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        
        #endregion
    }
}