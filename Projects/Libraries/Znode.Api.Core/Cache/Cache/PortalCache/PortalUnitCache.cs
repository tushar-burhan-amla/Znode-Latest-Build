using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PortalUnitCache : BaseCache, IPortalUnitCache
    {
        #region Private Variable
        private readonly IPortalUnitService _service;
        #endregion

        #region Constructor
        public PortalUnitCache(IPortalUnitService portalUnitService)
        {
            _service = portalUnitService;
        }
        #endregion

        #region Public Method
        public virtual string GetPortalUnit(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalUnitModel portalUnit = _service.GetPortalUnit(portalId);
                if (!Equals(portalUnit, null))
                {
                    PortalUnitResponse portalUnitResponse = new PortalUnitResponse { PortalUnit = portalUnit };
                    data = InsertIntoCache(routeUri, routeTemplate, portalUnitResponse);
                }
            }
            return data;
        }
        #endregion
    }
}