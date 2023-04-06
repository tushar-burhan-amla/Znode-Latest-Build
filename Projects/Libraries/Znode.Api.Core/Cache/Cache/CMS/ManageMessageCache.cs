using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ManageMessageCache : BaseCache, IManageMessageCache
    {
        #region Private Variable
        private readonly IManageMessageService _service;
        #endregion

        #region Constructor
        public ManageMessageCache(IManageMessageService manageMessageService)
        {
            _service = manageMessageService;
        }       
        #endregion

        #region Public Methods
        //Get Manage Message list.
        public virtual string GetManageMessages(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ManageMessageListModel list = _service.GetManageMessages(Expands, Filters, Sorts, Page);
                if (list?.ManageMessages?.Count > 0)
                {
                    ManageMessageListResponse response = new ManageMessageListResponse { ManageMessages = list.ManageMessages };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get ManageMessage details.
        public virtual string GetManageMessage(ManageMessageMapperModel manageMessageMapperModel, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ManageMessageModel manageMessageModel = _service.GetManageMessage(manageMessageMapperModel);
                if (IsNotNull(manageMessageModel))
                {
                    ManageMessageResponse response = new ManageMessageResponse { ManageMessage = manageMessageModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}