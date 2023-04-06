using Znode.Engine.Api.Models;
using Znode.Engine.Shipping.Ups;

namespace Znode.Engine.Shipping
{
    public interface IZnodeShippingUps
    {
        void GetUPSCredentialsSetting(PortalShippingModel portalShippingModel, UpsAgent ups);
        void MapShipItemsDimensionsForEstimateRate(UpsAgent ups);
    }
}