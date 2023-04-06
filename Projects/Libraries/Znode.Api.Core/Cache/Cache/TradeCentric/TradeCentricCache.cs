using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class TradeCentricCache : BaseCache, ITradeCentricCache
    {
        #region Private Variable
        private ITradeCentricService _tradeCentricService;
        #endregion

        #region Public Constructor
        public TradeCentricCache(ITradeCentricService tradeCentricService)
        {
            _tradeCentricService = tradeCentricService;
        }
        #endregion

        #region Public Methods
       
        // Get TradeCentric user details.
        public virtual string GetTradeCentricUser(int userId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TradeCentricUserModel user = _tradeCentricService.GetTradeCentricUser(userId);
 
                if (!Equals(user, null))
                {
                    //Create response
                    TradeCentricUserResponse response = new TradeCentricUserResponse { TradeCentricUser = user };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
