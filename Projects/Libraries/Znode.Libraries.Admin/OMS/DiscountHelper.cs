using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
namespace Znode.Libraries.Admin
{
    public class DiscountHelper : IDiscountHelper
    {
        #region Private Variables
        protected readonly IZnodeOrderHelper orderHelper;
        #endregion

        #region Constructor

        public DiscountHelper()
        {
            orderHelper = ZnodeDependencyResolver.GetService<IZnodeOrderHelper>();
        }
        #endregion

        #region Split Order Discount

        #region Public Method
        /// <summary>
        /// SplitOrderLevelDiscount on each lineitem
        /// </summary>
        /// <param name="allAppliedPromoCouponList">Applied Promotion Coupon list for entire order </param>
        /// <param name="OrderLineItems">List of OrderLineItems</param>
        /// <returns></returns>
        public virtual List<OrderDiscountModel> SplitDiscount(List<OrderDiscountModel> orderDiscounts, List<OrderLineItemModel> OrderLineItems)
        {
            try
            {
                if (HelperUtility.IsNotNull(orderDiscounts) && orderDiscounts.Count > 0)
                {
                    // Filter Order level coupons from existing PromotionCoupon list
                   // SplitOrderLevelDiscount(allAppliedPromoCouponList, OrderLineItems);
                    SplitShippingLevelDiscount(orderDiscounts, OrderLineItems);

                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return orderDiscounts;
        }

        #endregion

        #region Protected Method
        /// <summary>
        /// Calculate and Distribute Order level discounts on line item wise.
        /// Calculate and Distribute Order level discounts on line item wise.
        /// </summary>
        /// <param name="allAppliedPromoCouponList"></param>
        /// <param name="OrderLineItems"></param>
        /// <param name="orderLevelDiscount"></param>
        protected virtual void SplitOrderLevelDiscount(List<OrderDiscountModel> allAppliedPromoCouponList, List<OrderLineItemModel> OrderLineItems)
        {
            List<OrderDiscountModel> orderLevelDiscount = allAppliedPromoCouponList.FindAll(x => x.OmsOrderLineItemId == null && (x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel || x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.CSRLevel)).OrderBy(x => x.DiscountLevelTypeId).ToList();
            if (HelperUtility.IsNotNull(orderLevelDiscount) && orderLevelDiscount?.Count > 0)
            {
                foreach (OrderDiscountModel discount in orderLevelDiscount)
                {
                    SetDiscountForEachOrderLevelDiscount(allAppliedPromoCouponList, discount, OrderLineItems);
                }
            }
        }

        /// <summary>
        /// Split Discount for each order level discount(e.g AmtOfOrder/PercentOffOrder/CSR) on line item
        /// </summary>
        /// <param name="allAppliedPromoCouponList"></param>
        /// <param name="discount"></param>
        /// <param name="OrderLineItems"></param>
        protected virtual void SetDiscountForEachOrderLevelDiscount(List<OrderDiscountModel> allAppliedPromoCouponList, OrderDiscountModel discount, List<OrderLineItemModel> OrderLineItems)
        {
            if (HelperUtility.IsNotNull(discount))
            {
                allAppliedPromoCouponList.RemoveAll(x => x.DiscountCode.Equals(discount.DiscountCode, StringComparison.InvariantCultureIgnoreCase) && x.OmsOrderLineItemId != null);
                OrderDiscountModel splitOrderDiscount = null;
                splitOrderDiscount = new OrderDiscountModel();
                foreach (OrderLineItemModel orderLineItem in OrderLineItems)
                {
                    splitOrderDiscount = SetLineItemOrderDiscount(orderLineItem, discount);
                    allAppliedPromoCouponList.Add(splitOrderDiscount);
                }
            }

        }

        /// <summary>
        /// Set discount for lineitem
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discount"></param>
        /// <param name="discountMultiplier"></param>
        /// <returns></returns>
        protected virtual OrderDiscountModel SetLineItemOrderDiscount(OrderLineItemModel lineItem, OrderDiscountModel discount)
        {
            decimal unitPrice = GetUnitPrice(lineItem);
            OrderDiscountModel splitOrderDiscount = new OrderDiscountModel();
            splitOrderDiscount.OmsDiscountTypeId = discount.OmsDiscountTypeId;
            splitOrderDiscount.OmsOrderDetailsId = discount.OmsOrderDetailsId;
            splitOrderDiscount.OmsOrderLineItemId = lineItem.OmsOrderLineItemsId;
            splitOrderDiscount.DiscountAmount = GetDiscountAmount(lineItem,  unitPrice,  discount);
            splitOrderDiscount.PerQuantityDiscount = splitOrderDiscount.DiscountAmount / lineItem.Quantity;
            splitOrderDiscount.DiscountCode = discount.DiscountCode;
            splitOrderDiscount.ParentOmsOrderLineItemsId = lineItem.ParentOmsOrderLineItemsId; //GetParentOmsLineItemId(lineItem);
            splitOrderDiscount.Description = discount.Description;
            splitOrderDiscount.DiscountMultiplier = discount.DiscountMultiplier;
            splitOrderDiscount.DiscountLevelTypeId = discount.DiscountLevelTypeId;
            splitOrderDiscount.PromotionName = discount.PromotionName;
            splitOrderDiscount.PromotionTypeId = discount.PromotionTypeId;
            return splitOrderDiscount;
        }



        protected virtual decimal GetDiscountAmount(OrderLineItemModel orderLineItem, decimal unitPrice, OrderDiscountModel discount)
        {
            decimal discountAmount = 0;
            if (discount.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)
            {
                discountAmount = ((unitPrice * orderLineItem.Quantity - GetLineItemDiscount(orderLineItem)) * discount.DiscountMultiplier).GetValueOrDefault();
                orderLineItem.LineItemOrderDiscountAmount = orderLineItem.LineItemOrderDiscountAmount + discountAmount;
            }
            else
            {
                discountAmount = ((unitPrice * orderLineItem.Quantity - orderLineItem.DiscountAmount - orderLineItem.LineItemOrderDiscountAmount) * discount.DiscountMultiplier).GetValueOrDefault();
            }
            return discountAmount;
        }


        /// <summary>
        /// Get the Unit price for lineItem
        /// </summary>
        /// <param name="orderLineItem"></param>
        /// <returns></returns>
        protected virtual decimal GetUnitPrice(OrderLineItemModel orderLineItem)
        {
            decimal unitPrice = orderLineItem.Price;
            if (orderLineItem?.OrderLineItemCollection?.Count > 0)
            {
                foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
                {
                    if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns))
                        unitPrice = unitPrice + childLineItems.Price;
                    else if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable))
                        unitPrice = childLineItems.Price;
                    else if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group))
                        unitPrice = childLineItems.Price;
                }
            }
            return unitPrice;
        }


        protected virtual decimal? GetLineItemDiscount(OrderLineItemModel orderLineItem)
        {
            decimal? discountAmount = orderLineItem.DiscountAmount;
            if (orderLineItem?.OrderLineItemCollection?.Count > 0)
            {
                foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
                {
                    if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns))
                        discountAmount = discountAmount + childLineItems.DiscountAmount;
                    else if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable))
                        discountAmount = childLineItems.DiscountAmount;
                    else if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group))
                        discountAmount = childLineItems.DiscountAmount;
                }
            }
            return discountAmount;
        }


       


        /// <summary>
        /// Distribute Shipping level discounts on line item wise.
        /// </summary>
        /// <param name="allAppliedPromoCouponList"></param>
        /// <param name="OrderLineItems"></param>
        /// <param name="orderLevelDiscount"></param>
        protected virtual void SplitShippingLevelDiscount(List<OrderDiscountModel> allAppliedPromoCouponList, List<OrderLineItemModel> OrderLineItems)
        {
            List<OrderDiscountModel> shippingLevelDiscount = allAppliedPromoCouponList.FindAll(x => x.OmsOrderLineItemId == null && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.ShippingLevel).ToList();
            if (HelperUtility.IsNotNull(shippingLevelDiscount) && shippingLevelDiscount?.Count > 0)
            {
                foreach (OrderDiscountModel discount in shippingLevelDiscount)
                {
                    SetDiscountForEachShippingLevelDiscount(allAppliedPromoCouponList, discount, OrderLineItems);
                }
            }
        }

        /// <summary>
        /// Split Discount for each Shipping level discount on line item
        /// </summary>
        /// <param name="allAppliedPromoCouponList"></param>
        /// <param name="discount"></param>
        /// <param name="OrderLineItems"></param>
        protected virtual void SetDiscountForEachShippingLevelDiscount(List<OrderDiscountModel> allAppliedPromoCouponList, OrderDiscountModel discount, List<OrderLineItemModel> OrderLineItems)
        {
            if (HelperUtility.IsNotNull(discount))
            {
                allAppliedPromoCouponList.RemoveAll(x => x.DiscountCode.Equals(discount.DiscountCode, StringComparison.InvariantCultureIgnoreCase) && x.OmsOrderLineItemId != null);
                OrderDiscountModel splitShippingDiscount = null;
                splitShippingDiscount = new OrderDiscountModel();
                foreach (OrderLineItemModel orderLineItem in OrderLineItems)
                {
                    splitShippingDiscount = SetLineItemShippingDiscount(orderLineItem, discount);
                    if (splitShippingDiscount.DiscountAmount > 0)
                        allAppliedPromoCouponList.Add(splitShippingDiscount);
                }
            }

        }

        /// <summary>
        /// Set shipping discount for lineitem
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discount"></param>
        /// <param name="discountMultiplier"></param>
        /// <returns></returns>
        protected virtual OrderDiscountModel SetLineItemShippingDiscount(OrderLineItemModel lineItem, OrderDiscountModel discount)
        {
            decimal unitPrice = GetUnitPrice(lineItem);
            OrderDiscountModel splitOrderDiscount = new OrderDiscountModel();
            splitOrderDiscount.OmsDiscountTypeId = discount.OmsDiscountTypeId;
            splitOrderDiscount.OmsOrderDetailsId = discount.OmsOrderDetailsId;
            splitOrderDiscount.OmsOrderLineItemId = lineItem.OmsOrderLineItemsId;
            splitOrderDiscount.DiscountAmount = lineItem.ShippingCost *  discount.DiscountMultiplier;
            splitOrderDiscount.PerQuantityDiscount = splitOrderDiscount.DiscountAmount / lineItem.Quantity;
            splitOrderDiscount.DiscountCode = discount.DiscountCode;
            splitOrderDiscount.ParentOmsOrderLineItemsId = lineItem.ParentOmsOrderLineItemsId;// GetParentOmsLineItemId(lineItem);
            splitOrderDiscount.Description = discount.Description;
            splitOrderDiscount.DiscountMultiplier = discount.DiscountMultiplier;
            splitOrderDiscount.DiscountLevelTypeId = discount.DiscountLevelTypeId;
            splitOrderDiscount.PromotionName = discount.PromotionName;
            splitOrderDiscount.Sku = lineItem.Sku;
            splitOrderDiscount.GroupId = lineItem.GroupId;
            splitOrderDiscount.PromotionMessage = discount.PromotionMessage;
            return splitOrderDiscount;
        }

        #endregion

        #endregion


    }
}
