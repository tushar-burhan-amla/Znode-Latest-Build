using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PublishHistoryCache : BaseCache, IPublishHistoryCache
    {
        #region Private Variables
        private readonly IPublishHistoryService _publishHistoryService;
        #endregion

        #region Constructor
        public PublishHistoryCache(IPublishHistoryService service)
        {
            _publishHistoryService = service;
        }
        #endregion

        #region Public Methods
        //Get the list of Publish History.
        public string GetPublishHistoryList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PublishHistoryListModel list = _publishHistoryService.GetPublishHistoryList(Expands, Filters, Sorts, Page);
                if (list?.PublishHistoryList?.Count > 0)
                {
                    PublishHistoryListResponse response = new PublishHistoryListResponse { PublishHistory = list.PublishHistoryList, TotalResults = list.TotalResults, TotalPages = list.TotalPages, PageIndex = list.PageIndex, PageSize = list.PageSize };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
