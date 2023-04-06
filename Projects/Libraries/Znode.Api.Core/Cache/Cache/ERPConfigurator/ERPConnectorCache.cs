using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ERPConnectorCache : BaseCache, IERPConnectorCache
    {
        #region Private Variables
        private readonly IERPConnectorService _service;
        #endregion

        #region Constructor
        public ERPConnectorCache(IERPConnectorService erpConnectorService)
        {
            _service = erpConnectorService;
        }
        #endregion

        #region Public Methods
        public virtual string GetERPConnectorControlList(ERPConfiguratorModel erpConfiguratorModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                ERPConnectorControlListModel erpConnectorControlList = _service.GetERPConnectorControls(erpConfiguratorModel);
                if (!Equals(erpConnectorControlList?.ERPConnectorControlList, null))
                {
                    //Create response.
                    ERPConnectorControlListResponse response = new ERPConnectorControlListResponse { ERPConnectorControls = erpConnectorControlList.ERPConnectorControlList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(erpConnectorControlList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
