namespace Znode.Engine.Shipping
{
    public interface IZnodeOrderShippingHelper
    {
        /// <summary>
        /// Calculates shipping for particular order data.
        /// </summary>
        void OrderCalculate();

        /// <summary>
        /// Calculates each order line shipping cost for particular order data.
        /// </summary>
        void SetOrderLinePerQuantityShippingCost();

        /// <summary>
        /// Set each order line shipping cost for particular order data.
        /// </summary>
        void SetLineItemShipping();
    }
}
