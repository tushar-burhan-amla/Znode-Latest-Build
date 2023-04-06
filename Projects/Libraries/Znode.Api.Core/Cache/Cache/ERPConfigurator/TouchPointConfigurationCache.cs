using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class TouchPointConfigurationCache : BaseCache, ITouchPointConfigurationCache
    {
        #region Private Variables
        private readonly ITouchPointConfigurationService _service;
        #endregion

        #region Public Constructor
        public TouchPointConfigurationCache(ITouchPointConfigurationService touchPointConfigurationService)
        {
            _service = touchPointConfigurationService;
        }
        #endregion

        #region Public Methods

        //Get TouchPointConfigurations From Cache
        public virtual string GetTouchPointConfigurationList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                TouchPointConfigurationListModel touchPointConfigurationList = _service.GetTouchPointConfigurationList(Expands, Filters, Sorts, Page);
                if (touchPointConfigurationList?.TouchPointConfigurationList?.Count > 0)
                {
                    TouchPointConfigurationListResponse response = new TouchPointConfigurationListResponse { TouchPointConfiguration = touchPointConfigurationList.TouchPointConfigurationList };
                    response.MapPagingDataFromModel(touchPointConfigurationList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get TouchPointConfigurations From Cache
        public virtual string SchedulerLogList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                TouchPointConfigurationListModel schedulerLogList = _service.SchedulerLogList(Expands, Filters, Sorts, Page);
                if (schedulerLogList?.SchedulerLogList?.Count > 0)
                {
                    TouchPointConfigurationListResponse response = new TouchPointConfigurationListResponse { SchedulerLog = schedulerLogList.SchedulerLogList };
                    response.MapPagingDataFromModel(schedulerLogList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}