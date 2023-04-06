using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PublishStateCache : BaseCache, IPublishStateCache
    {
        #region Private Variables
        private readonly IPublishStateService _service;
        #endregion

        #region Constructor
        public PublishStateCache(IPublishStateService publishStateService)
        {
            _service = publishStateService;
        }
        #endregion

        #region Public Methods

        //Get a list of all cities.
        public virtual string GetPublishStateMappingList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PublishStateMappingListModel list = _service.GetPublishStateApplicationTypeMappingList(Filters, Sorts, Page);
                if (list?.PublishStateMappingList?.Count > 0)
                {
                    //Create response.
                    PublishStateMappingListResponse response = new PublishStateMappingListResponse { PublishStateMappings = list.PublishStateMappingList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}