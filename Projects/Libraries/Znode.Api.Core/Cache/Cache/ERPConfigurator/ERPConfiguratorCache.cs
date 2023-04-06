using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ERPConfiguratorCache : BaseCache, IERPConfiguratorCache
    {
        #region Private Variables
        private readonly IERPConfiguratorService _service;
        #endregion

        #region Public Constructor
        public ERPConfiguratorCache(IERPConfiguratorService eRPConfiguratorService)
        {
            _service = eRPConfiguratorService;
        }
        #endregion

        #region Public Methods

        //Get ERP Configurator From Cache
        public virtual string GetERPConfiguratorList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                ERPConfiguratorListModel eRPConfiguratorList = _service.GetERPConfiguratorList(Expands, Filters, Sorts, Page);
                if (eRPConfiguratorList?.ERPConfiguratorList?.Count > 0)
                {
                    ERPConfiguratorListResponse response = new ERPConfiguratorListResponse { ERPConfigurator = eRPConfiguratorList.ERPConfiguratorList };
                    response.MapPagingDataFromModel(eRPConfiguratorList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get all ERP Configurator Classes which are not present in database.
        public virtual string GetAllERPConfiguratorClassesNotInDatabase(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ERPConfiguratorListModel list = _service.GetAllERPConfiguratorClassesNotInDatabase();

                //If list count is greater than 0 then Create a list response for ERP Configurator and insert into cache.
                if (list?.ERPConfiguratorList?.Count > 0)
                {
                    ERPConfiguratorListResponse response = new ERPConfiguratorListResponse { ERPConfiguratorClasses = list.ERPConfiguratorList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get eRPConfigurator by eRPConfigurator id.
        public virtual string GetERPConfigurator(int eRPConfiguratorId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ERPConfiguratorModel eRPConfiguratorModel = _service.GetERPConfigurator(eRPConfiguratorId);
                if (IsNotNull(eRPConfiguratorModel))
                {
                    ERPConfiguratorResponse response = new ERPConfiguratorResponse { ERPConfigurator = eRPConfiguratorModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}