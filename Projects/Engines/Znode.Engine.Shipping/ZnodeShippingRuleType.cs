using System.ComponentModel;

namespace Znode.Engine.Shipping
{	
    public enum ZnodeShippingRuleTypes
    {
        [Description("FlatRatePerItem")]
        FlatRatePerItem,

        [Description("QuantityBasedRate")]
        RateBasedOnQuantity,

        [Description("WeightBasedRate")]
        RateBasedOnWeight,

        [Description("FixedRatePerItem")]
        FixedRatePerItem,
    }

    public enum ZnodeShippingHandlingChargesBasedON
    {
        SubTotal,
        Shipping,
        Amount
    }
}
