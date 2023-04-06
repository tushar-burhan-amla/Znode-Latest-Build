using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ERPTaskSchedulerCache : BaseCache, IERPTaskSchedulerCache
    {
        #region Private Variables
        private readonly IERPTaskSchedulerService _service;
        #endregion

        #region Public Constructor
        public ERPTaskSchedulerCache(IERPTaskSchedulerService eRPConfiguratorService)
        {
            _service = eRPConfiguratorService;
        }
        #endregion

        #region Public Methods
        //Get ERPTaskSchedulers From Cache
        public virtual string GetERPTaskSchedulerList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                ERPTaskSchedulerListModel eRPConfiguratorList = _service.GetERPTaskSchedulerList(Expands, Filters, Sorts, Page);
                if (eRPConfiguratorList?.ERPTaskSchedulerList?.Count > 0)
                {
                    ERPTaskSchedulerListResponse response = new ERPTaskSchedulerListResponse { ERPTaskScheduler = eRPConfiguratorList.ERPTaskSchedulerList };
                    response.MapPagingDataFromModel(eRPConfiguratorList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get eRPConfigurator by eRPConfigurator id.
        public virtual string GetERPTaskScheduler(int erpTaskSchedulerId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ERPTaskSchedulerModel eRPConfiguratorModel = _service.GetERPTaskScheduler(erpTaskSchedulerId);
                if (IsNotNull(eRPConfiguratorModel))
                {
                    ERPTaskSchedulerResponse response = new ERPTaskSchedulerResponse { ERPTaskScheduler = eRPConfiguratorModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}