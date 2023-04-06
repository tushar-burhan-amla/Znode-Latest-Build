using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreMessageCache : BaseCache, IWebStoreMessageCache
    {
        #region Private Variable
        private readonly IManageMessageService _service;
        #endregion

        #region Constructor
        public WebStoreMessageCache(IManageMessageService service)
        {
            _service = service;
        }
        #endregion

        //Get message by Message Key, Area and Portal Id.
        public virtual string GetMessage(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ManageMessageModel message = _service.GetMessage(Expands, Filters);
                if (HelperUtility.IsNotNull(message))
                {
                    WebStoreMessageResponse response = new WebStoreMessageResponse { Message = message };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get message list by locale id and Portal Id.
        public virtual string GetMessages(string routeUri, string routeTemplate, int localeId)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ManageMessageListModel list = _service.GetMessages(Expands, Filters, localeId);
                if (list?.ManageMessages?.Count > 0)
                {
                    WebStoreMessageListResponse response = new WebStoreMessageListResponse { Messages = list.ManageMessages };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
    }
}