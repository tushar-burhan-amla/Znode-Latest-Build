using System.ComponentModel;

namespace Znode.Engine.Shipping
{
    public enum ZnodeShippingControl
    {
        Profile,
        ServiceCodes,
        DisplayName,
        InternalCode,
        HandlingCharge,
        Countries
    }

    public enum UPSLTLServiceCode
    {
        [Description("308")]
        FreightLTL = 308,
        [Description("309")]
        FreightLTLGuaranteed,
        [Description("310")]
        FreightLTLUrgent,
    }
}

