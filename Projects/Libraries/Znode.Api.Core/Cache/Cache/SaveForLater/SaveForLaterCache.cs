using Znode.Api.Core.Cache.ICache.ISaveForLater;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Api.Core.Cache.Cache.SaveForLater
{
    public class SaveForLaterCache : BaseCache, ISaveForLaterCache
    {
        #region Private Readonly Variables
        private readonly IAccountQuoteService _service;
        #endregion

        #region Public Constructor
        public SaveForLaterCache(IAccountQuoteService service)
        {
            _service = service;
        }
        #endregion

        #region Public methods

        //Get saved cart for later
        public string GetCartForLater(int userId, string templateType, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountTemplateModel accountTemplateModel = _service.GetCartForLater(userId, templateType, Expands, Filters);

                if (HelperUtility.IsNotNull(accountTemplateModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new AccountQuoteResponse { AccountTemplate = accountTemplateModel });
            }
            return data;
        }

        //Get account quote details by omsTemplateId.
        public virtual string GetCartTemplate(int omsTemplateId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountTemplateModel accountTemplateModel = _service.GetAccountTemplate(omsTemplateId, Expands, Filters);

                if (HelperUtility.IsNotNull(accountTemplateModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new AccountQuoteResponse { AccountTemplate = accountTemplateModel });
            }
            return data;
        }

        #endregion
    }
}
