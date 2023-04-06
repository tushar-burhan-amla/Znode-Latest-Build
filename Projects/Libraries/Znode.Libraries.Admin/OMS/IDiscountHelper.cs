using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Admin
{
    public interface IDiscountHelper
    {
        /// <summary>
        /// SplitOrderLevelDiscount on each lineitem
        /// </summary>
        /// <param name="allAppliedPromoCouponList">Applied Promotion Coupon list for entire order </param>
        /// <param name="OrderLineItems">List of OrderLineItems</param>
        /// <returns></returns>
        List<OrderDiscountModel> SplitDiscount(List<OrderDiscountModel> orderDiscounts, List<OrderLineItemModel> OrderLineItems);

    }
}
