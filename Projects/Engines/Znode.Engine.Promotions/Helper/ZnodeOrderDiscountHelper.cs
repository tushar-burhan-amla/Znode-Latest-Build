using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeOrderDiscountHelper : IZnodeOrderDiscountHelper
    {
        #region Variables

        // TODO Will have created task to get it from DI
        protected readonly IZnodePromotionHelper promotionHelper = null;
        protected ZnodeShoppingCart _shoppingCart;
        protected ZnodeGenericCollection<IZnodeCartPromotionType> _cartPromotions;

        private List<PromotionModel> activeCoupons = null;
        private ZnodePromotionBag promotionBag;
        #endregion

        #region Constructor
        public ZnodeOrderDiscountHelper(ZnodeShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
            _cartPromotions = new ZnodeGenericCollection<IZnodeCartPromotionType>();
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates all Coupons and Promotion discount on for particular order data.
        /// </summary>
        public virtual void OrderCalculate()
        {
            List<OrderPromotionModel> listOfDiscountInOrder = GetOrderDiscountList((int)_shoppingCart.OrderId);
           
            CalculateLineItemDiscount(listOfDiscountInOrder);

            CalculateShippingDiscount(listOfDiscountInOrder);

            if (_shoppingCart.IsShippingRecalculate || _shoppingCart.IsShippingDiscountRecalculate)
                CalculateShippingPromotion(listOfDiscountInOrder);
            CalculateOrderLevelPromoAndCoupons(listOfDiscountInOrder);
            CalculateShippingLevelPromoAndCoupons(listOfDiscountInOrder);
            SetPromotionCouponSequence(listOfDiscountInOrder);
            CalculateDiscountForNewCoupon(listOfDiscountInOrder);
        }

        public virtual bool IsApplicablePromotion(ZnodeShoppingCartItem cartItem, string discountCode)
        {
            return true;
        }
        
        // This function is used to check the condition that sub total of order is greater than the sum of the orderdiscount, line item discount and CSR discount
        public virtual bool IsDiscountApplicable()
        {
            if (( _shoppingCart.OrderId > 0 ? _shoppingCart.Discount : (_shoppingCart.ShoppingCartItems.Sum(x => x.DiscountAmount) + _shoppingCart.OrderLevelDiscount)
                    + (_shoppingCart.CSRDiscountApplied ? _shoppingCart.CSRDiscountAmount : 0)) < _shoppingCart.SubTotal      
               )
            {
                return true;
            }
            return false;
        }

        public virtual void SetPromotionCouponSequence(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> promoCouponList = GetApplicablePromoCouponList(listOfDiscountInOrder);
            foreach (OrderPromotionModel orderPromotionModel in promoCouponList)
            {
                _shoppingCart.GetDiscountAppliedSequence(orderPromotionModel.DiscountCode);
            }
        }

        // This function is used to check the condition that shipping of order is greater than the sum of the shippingdiscount
        public virtual bool IsShippingDiscountApplicable()
        {
            if(_shoppingCart.Shipping.ShippingID > 0 && _shoppingCart.Shipping.ShippingDiscount > 0)
            {
                if ( _shoppingCart.Shipping.ShippingDiscount < _shoppingCart.ShippingCost)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Protected Method

        protected virtual void CalculateLineItemDiscount(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                SetLineItemDiscount(cartItem, listOfDiscountInOrder);
            }
        }

        protected virtual void CalculateShippingDiscount(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            OrderPromotionModel shippingDiscount = null;
            if (!_shoppingCart.IsRemoveShippingDiscount)
                shippingDiscount = GetShippingApplicableDiscountList(listOfDiscountInOrder)?.FirstOrDefault();

            //If IsRemoveShippingDiscount is true if orderline item  shipping edit from manager order so remove shipping discount
            if (HelperUtility.IsNotNull(shippingDiscount) && _shoppingCart.IsRemoveShippingDiscount)
            {
                bool? isExist = _shoppingCart.InvalidOrderLevelShippingDiscount?.Exists(x => string.Equals(x.DiscountCode, shippingDiscount.DiscountCode, StringComparison.InvariantCultureIgnoreCase));

                if (!Convert.ToBoolean(isExist))
                {
                    _shoppingCart.InvalidOrderLevelShippingDiscount?.Add(shippingDiscount);
                }          
            }

            if (IsInvalidShippingDiscount(shippingDiscount))
            {
                if (HelperUtility.IsNotNull(shippingDiscount) && _shoppingCart.ShoppingCartItems.Count > 0)
                {
                    _shoppingCart.Shipping.ShippingDiscount = _shoppingCart.ShoppingCartItems.Sum(x => x.PerQuantityShippingDiscount * x.Quantity);
                    _shoppingCart.Shipping.ShippingDiscountApplied = true;
                    _shoppingCart.Shipping.ShippingDiscountDescription = shippingDiscount.DiscountCode;
                    _shoppingCart.Shipping.ShippingDiscountType = shippingDiscount.OmsDiscountTypeId;
                    _shoppingCart.ShippingDiscount = _shoppingCart.Shipping.ShippingDiscount;
                    _shoppingCart.ShippingHandlingCharges = _shoppingCart.Shipping.ShippingHandlingCharge;
                }
            }
        }

        // This method pull the list of promotion and coupons applied to order.
        protected virtual List<OrderPromotionModel> GetOrderDiscountList(int omsOrderId)
        {
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrderDiscount> _omsOrderDiscountRepository = new ZnodeRepository<ZnodeOmsOrderDiscount>();
                IZnodeRepository<ZnodeOmsDiscountType> _omsDiscountTypeRepository = new ZnodeRepository<ZnodeOmsDiscountType>();
                IZnodeRepository<ZnodeOmsOrderDetail> _omsOrderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
                IZnodeRepository<ZnodeOmsOrderLineItem> _omsOrderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();

                List<OrderPromotionModel> promotions = (from _orderDiscount in _omsOrderDiscountRepository.Table
                                join _orderdetail in _omsOrderDetailRepository.Table on _orderDiscount.OmsOrderDetailsId equals _orderdetail.OmsOrderDetailsId
                                into orderDiscount from orderDetail in orderDiscount.DefaultIfEmpty()
                                join orderDiscountItem in _omsOrderLineItemRepository.Table on _orderDiscount.OmsOrderLineItemId equals orderDiscountItem.OmsOrderLineItemsId into lineItemDetails
                                from lineitems in lineItemDetails.DefaultIfEmpty()
                                where orderDetail.OmsOrderId == omsOrderId && orderDetail.IsActive == true
                                select new OrderPromotionModel{
                                PromotionName = _orderDiscount.PromotionName,
                                DiscountCode = _orderDiscount.DiscountCode,
                                PerQuantityDiscount = (decimal)_orderDiscount.PerQuantityDiscount,
                                TotalDiscountAmount = (decimal)_orderDiscount.DiscountAmount,
                                OmsDiscountTypeId = (int)_orderDiscount.OmsDiscountTypeId,
                                OmsOrderLineItemId = _orderDiscount.OmsOrderLineItemId,
                                OmsOrderDetailsId = (int)_orderDiscount.OmsOrderDetailsId,
                                ParentOmsOrderLineItemsId = _orderDiscount.ParentOmsOrderLineItemsId,
                                DiscountMultiplier = _orderDiscount.DiscountMultiplier,
                                DiscountLevelTypeId = _orderDiscount.DiscountLevelTypeId,
                                PromotionTypeId = _orderDiscount.PromotionTypeId,
                                Sku = lineitems.Sku,
                                DiscountAppliedSequence = _orderDiscount.DiscountAppliedSequence,
                                PromotionMessage = _orderDiscount.PromotionMessage
                                }).OrderBy(x=> x.Sku).ToList();           

                return promotions ?? new List<OrderPromotionModel>();
            }
            return null;
        }

        // This method is used to set discount to cart item saved in db.
        protected virtual void SetLineItemDiscount(ZnodeShoppingCartItem cartItem, List<OrderPromotionModel> listOfDiscountInOrder)
        {
            string sku = string.IsNullOrEmpty(cartItem.SKU) ? cartItem.ParentProductSKU : cartItem.SKU;

            List<OrderPromotionModel> applicablePromoList = null;

            if (cartItem.Product.ZNodeGroupProductCollection.Count == 0 && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0)
            {
                applicablePromoList = GetLineItemApplicableDiscountList(sku, cartItem, listOfDiscountInOrder);
                SetParentChildDiscount(cartItem.Product, cartItem.Quantity, applicablePromoList);
            }

            if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
            {
                applicablePromoList?.Clear();

                foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
                {
                    applicablePromoList = GetLineItemApplicableDiscountList(config.SKU, cartItem, listOfDiscountInOrder, true);
                    SetParentChildDiscount(cartItem.Product, cartItem.Quantity, applicablePromoList, config, true);
                }
            }


            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
            {
                applicablePromoList?.Clear();
                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    applicablePromoList = GetLineItemApplicableDiscountList(group.SKU, cartItem, listOfDiscountInOrder);
                    SetParentChildDiscount(group, cartItem.Quantity, applicablePromoList);
                }
            }

            if (cartItem.Product.ZNodeAddonsProductCollection.Count > 0)
            {
                applicablePromoList?.Clear();
                foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                {
                    applicablePromoList = GetLineItemApplicableDiscountList(addon.SKU, cartItem, listOfDiscountInOrder, isAddOnProduct: true);
                    SetParentChildDiscount(addon, cartItem.Quantity, applicablePromoList);
                }
            }

        }

        private void SetParentChildDiscount(ZnodeProductBaseEntity product, decimal quantity, List<OrderPromotionModel> applicablePromoList, ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false)
        {
            foreach (OrderPromotionModel lineItemPromotion in applicablePromoList)
            {
                OrderDiscountTypeEnum discountType = GetOrderDiscountType(lineItemPromotion.OmsDiscountTypeId);
                SetLineItemAndOrderDiscount(product, quantity, lineItemPromotion, discountType, childProduct, isConfigurableProduct);
                if (lineItemPromotion.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE)
                    _shoppingCart.IsAnyPromotionApplied = true;
            }
        }

        // Set quantity wised discount in cart item.

        protected virtual void SetLineItemAndOrderDiscount(ZnodeProductBaseEntity product, decimal quantity, OrderPromotionModel orderPromotionModel, OrderDiscountTypeEnum orderDiscountType, ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false)
        {
            if (product.FinalPrice > 0.0M)
            {
                if (orderPromotionModel.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel)
                {
                    if (HelperUtility.IsNull(childProduct))
                    {
                        product.DiscountAmount += orderPromotionModel.PerQuantityDiscount.GetValueOrDefault();
                        product.OrdersDiscount = SetOrderDiscountDetails(orderPromotionModel, product.OrdersDiscount);
                    }
                    else
                    {
                        if (isConfigurableProduct)
                        {
                            product.DiscountAmount += orderPromotionModel.PerQuantityDiscount.GetValueOrDefault();
                            product.OrdersDiscount = SetOrderDiscountDetails(orderPromotionModel, product.OrdersDiscount);
                        }
                        else
                        {
                            childProduct.DiscountAmount += orderPromotionModel.PerQuantityDiscount.GetValueOrDefault();
                            childProduct.OrdersDiscount = SetOrderDiscountDetails(orderPromotionModel, childProduct.OrdersDiscount);
                        }
                       
                    }
                }
                else if (orderPromotionModel.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)
                {
                    product.OrderDiscountAmount += orderPromotionModel.PerQuantityDiscount.GetValueOrDefault() * quantity;
                    product.OrdersDiscount = SetOrderDiscountDetails(orderPromotionModel, product.OrdersDiscount);
                }
            }
        }

        /// <summary>
        /// Set Order level promotion/coupons
        /// </summary>
        /// <param name="listOfDiscountInOrder"></param>
        protected virtual void CalculateOrderLevelPromoAndCoupons(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> orderLevelPromotionList = GetOrderApplicableDiscountList(listOfDiscountInOrder);
            if (HelperUtility.IsNotNull(orderLevelPromotionList) && orderLevelPromotionList.Count > 0)
            {
                List<ReturnOrderLineItemModel> returnItemList = _shoppingCart?.ReturnItemList?.Where(x => x.RmaReturnLineItemStatus == null)?.ToList();
                foreach (OrderPromotionModel orderPromotionModel in orderLevelPromotionList)
                {
                    OrderDiscountTypeEnum discountType = GetOrderDiscountType(orderPromotionModel.OmsDiscountTypeId);
                    SetRemainingOrderDiscountAmount(orderPromotionModel, listOfDiscountInOrder, returnItemList);
                    _shoppingCart.OrderLevelDiscountDetails = SetOrderDiscountDetails(orderPromotionModel, _shoppingCart.OrderLevelDiscountDetails);
                    _shoppingCart.OrderLevelDiscount = _shoppingCart.OrderLevelDiscount + orderPromotionModel.TotalDiscountAmount;
                }
            }
        }
        /// <summary>
        /// Set Shipping level promotion/coupons
        /// </summary>
        /// <param name="listOfDiscountInOrder"></param>
        protected virtual void CalculateShippingLevelPromoAndCoupons(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> shippingDiscountList = null;
            if (!_shoppingCart.IsRemoveShippingDiscount)
                shippingDiscountList = GetShippingApplicableDiscountList(listOfDiscountInOrder);

            if (HelperUtility.IsNotNull(shippingDiscountList) && shippingDiscountList.Count > 0)
            {
                List<ReturnOrderLineItemModel> returnItemList = _shoppingCart?.ReturnItemList?.Where(x => x.RmaReturnLineItemStatus == null)?.ToList();
                foreach (OrderPromotionModel shippingdiscountmodel in shippingDiscountList)
                {
                    OrderDiscountTypeEnum discountType = GetOrderDiscountType(shippingdiscountmodel.OmsDiscountTypeId);
                    SetRemainingOrderDiscountAmount(shippingdiscountmodel, listOfDiscountInOrder, returnItemList);
                    _shoppingCart.OrderLevelDiscountDetails = SetOrderDiscountDetails(shippingdiscountmodel, _shoppingCart.OrderLevelDiscountDetails);
                }
            }
        }

        /// <summary>
        /// Get Discount Type base on discountTypeId
        /// </summary>
        /// <param name="discountTypeId"></param>
        /// <returns></returns>
        protected virtual OrderDiscountTypeEnum GetOrderDiscountType(int discountTypeId)
        {
            OrderDiscountTypeEnum discountType = OrderDiscountTypeEnum.PROMOCODE;
            if (discountTypeId == Convert.ToInt16(OrderDiscountTypeEnum.COUPONCODE))
                discountType = OrderDiscountTypeEnum.COUPONCODE;
            else if (discountTypeId == Convert.ToInt16(OrderDiscountTypeEnum.CSRDISCOUNT))
                discountType = OrderDiscountTypeEnum.CSRDISCOUNT;
            else if (discountTypeId == Convert.ToInt16(OrderDiscountTypeEnum.GIFTCARD))
                discountType = OrderDiscountTypeEnum.GIFTCARD;
            else if (discountTypeId == Convert.ToInt16(OrderDiscountTypeEnum.PARTIALREFUND))
                discountType = OrderDiscountTypeEnum.PARTIALREFUND;
            return discountType;
        }


        /// <summary>
        /// Calculate Remaining discount in case of return Order.
        /// </summary>
        /// <param name="orderPromotionModel">Order level Promotion model</param>
        /// <param name="orderPromotionList">List of all Promotion/Coupons</param>
        protected virtual void SetRemainingOrderDiscountAmount(OrderPromotionModel orderPromotionModel, List<OrderPromotionModel> orderPromotionList, List<ReturnOrderLineItemModel> returnItemList)
        {
            CalculateReturnOrderDiscount(orderPromotionModel, orderPromotionList, returnItemList);

        }

        // Deduct Order discount amount for return quantity. 
        protected virtual void CalculateReturnOrderDiscount(OrderPromotionModel orderPromotionModel, List<OrderPromotionModel> orderPromotionList, List<ReturnOrderLineItemModel> returnItemList)
        {
            decimal? returnDiscAmt = 0;
            if (HelperUtility.IsNotNull(returnItemList) && returnItemList.Count > 0)
            {
                foreach (ReturnOrderLineItemModel returnListItem in returnItemList)
                {
                    decimal?  returnLineItemDiscount = orderPromotionList?.Where(x => x.DiscountCode == orderPromotionModel.DiscountCode && x.OmsOrderLineItemId != null && (x.OmsOrderLineItemId == returnListItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnListItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnListItem.OmsOrderLineItemsId))?.Sum(x => x.PerQuantityDiscount);
                    returnDiscAmt = returnDiscAmt + (returnLineItemDiscount * returnListItem.Quantity);
                }                
                orderPromotionModel.TotalDiscountAmount = orderPromotionModel.TotalDiscountAmount - returnDiscAmt.GetValueOrDefault();
            }
        }

        //calculate remaining discount amount on line item in case of delete from manage order screen.
        protected virtual void CalculateOrderDiscountForDeleteLineItem(OrderPromotionModel orderPromotionModel, List<OrderPromotionModel> orderPromotionList)
        {
            List<OrderPromotionModel> orderDiscountList = orderPromotionList.FindAll(x => x.DiscountCode == orderPromotionModel.DiscountCode && x.OmsOrderLineItemId != null);
            orderPromotionModel.TotalDiscountAmount = orderPromotionModel.TotalDiscountAmount - CalculateRemainingDiscountOnCartItemDelete(orderDiscountList);
        }

        #endregion region

        #region Calculate new coupon code discount
        protected virtual void CalculateDiscountForNewCoupon(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            int cartPromoIndex = 0, cartPromoCouponIndex = 0;
            List<Libraries.ECommerce.Entities.ZnodeCoupon> removeShippingCouponList = new List<Libraries.ECommerce.Entities.ZnodeCoupon>();
            _cartPromotions.Clear();
            activeCoupons = GetPromotionsByCouponCodes(GetCartCoupons());
            foreach (Libraries.ECommerce.Entities.ZnodeCoupon znodeCoupon in _shoppingCart.Coupons)
            {
                PromotionModel activePromotionModel = activeCoupons?.FirstOrDefault(x => string.Equals(x.PromoCode, znodeCoupon.Coupon, StringComparison.OrdinalIgnoreCase));

                //To check the orderline item shipping edit if yes then add all shipping promotion in removed list
                if (IsOrderLineItemShippingEdit(activePromotionModel, znodeCoupon, removeShippingCouponList))
                {
                    if (HelperUtility.IsNotNull(activePromotionModel))
                    {
                        AddPromotionType(activePromotionModel, ZnodePromotionBag(activePromotionModel, _shoppingCart.PortalId, _shoppingCart.ProfileId));
                        string couponMessage = string.IsNullOrEmpty(activePromotionModel.PromotionMessage) ? GlobalSetting_Resources.MessageCouponAccepted : activePromotionModel.PromotionMessage;
                        if (znodeCoupon.IsExistInOrder == false)
                            ApplyNewCouponDiscount(cartPromoIndex, cartPromoCouponIndex, znodeCoupon);
                        else
                            SetCouponDetails(cartPromoCouponIndex, true, true, couponMessage);                            
                    }
                    else if (znodeCoupon.IsExistInOrder == true)
                    {
                        // If coupon is used while placing order but it is update/modified by the admin then activePromotionModel will not contain the already coupon.
                        OrderPromotionModel orderPromotionModel = listOfDiscountInOrder.Where(x => string.Equals(x.DiscountCode, znodeCoupon.Coupon, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        string couponMessage = string.IsNullOrEmpty(orderPromotionModel.PromotionMessage) ? GlobalSetting_Resources.MessageCouponAccepted : orderPromotionModel.PromotionMessage;
                        SetCouponDetails(cartPromoCouponIndex, true, true, couponMessage);
                    }
                    else
                    {
                        string couponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? Api_Resources.InvalidCouponCode : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                        SetCouponDetails(cartPromoCouponIndex, false, false, couponMessage);
                    }
                    cartPromoIndex++;
                    cartPromoCouponIndex++;
                }
            }

            //Remove the shipping related coupons and promotion if shipping is edit for orderline item
            RemoveShippingCouponFromCart(removeShippingCouponList);
        }

        protected virtual ZnodePromotionBag ZnodePromotionBag(PromotionModel promotion, int? currentPortalId, int? currentProfileId)
        {
            promotionBag = new ZnodePromotionBag();
            promotionBag.PromotionId = promotion.PromotionId;
            promotionBag.PortalId = currentPortalId;
            promotionBag.ProfileId = currentProfileId;
            promotionBag.Discount = promotion.Discount.GetValueOrDefault();
            promotionBag.MinimumOrderAmount = promotion.OrderMinimum.GetValueOrDefault(0);
            promotionBag.RequiredProductMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredProductId = promotion.ReferralPublishProductId.GetValueOrDefault(0);
            promotionBag.DiscountedProductQuantity = promotion.PromotionProductQuantity.GetValueOrDefault(1);
            promotionBag.RequiredBrandMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCatalogMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCategoryMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.IsCouponAllowedWithOtherCoupons = promotion.IsAllowedWithOtherCoupons;
            promotionBag.PromoCode = promotion.PromoCode;
            promotionBag.IsUnique = promotion.IsUnique;
            promotionBag.PromotionName = promotion.Name;
            promotionBag.PromotionTypeId = promotion.PromotionTypeId;
            if (promotion.IsCouponRequired.GetValueOrDefault())
            {
                promotionBag.Coupons = promotionHelper.GetPromotionCoupons(promotion.PromotionId, GetCartCoupons());
                promotionBag.PromotionMessage = promotion.PromotionMessage;
            }
            return promotionBag;
        }

        protected virtual void AddPromotionType(PromotionModel promotion, ZnodePromotionBag promotionBag)
        {
            try
            {
                IZnodeCartPromotionType cartPromo = GetPromotionTypeInstance<IZnodeCartPromotionType>(promotion);

                if (HelperUtility.IsNotNull(cartPromo))
                {
                    cartPromo.Precedence = promotion.DisplayOrder.GetValueOrDefault();

                    if (HelperUtility.IsNotNull(promotionBag.Coupons) && promotionBag.Coupons.Count > 0)
                        cartPromo.IsPromoCouponAvailable = true;

                    cartPromo.Bind(_shoppingCart, promotionBag);
                    _cartPromotions.Add(cartPromo);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while instantiating promotion type: " + promotion?.PromotionType + " " + ex.ToString(), ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Verbose);
            }
        }

        // This method used to get promotion details by coupon codes
        protected virtual List<PromotionModel> GetPromotionsByCouponCodes(string couponCodes)
        {
            List<PromotionModel> promotions = null;
            if (!string.IsNullOrEmpty(couponCodes))
            {
                IZnodeRepository<ZnodePromotionType> _promotionTypeRepository = new ZnodeRepository<ZnodePromotionType>();
                IZnodeRepository<ZnodePromotion> _promotionRepository = new ZnodeRepository<ZnodePromotion>();
                IZnodeRepository<ZnodePromotionCoupon> _promotionCouponRepository = new ZnodeRepository<ZnodePromotionCoupon>();
                string[] couponList = couponCodes.ToLower().Split(',');

                promotions = (from _promo in _promotionRepository.Table
                              join _promoType in _promotionTypeRepository.Table on _promo.PromotionTypeId equals _promoType.PromotionTypeId
                              join _promoCoupon in _promotionCouponRepository.Table on _promo.PromotionId equals _promoCoupon.PromotionId
                              where couponList.Contains(_promoCoupon.Code.ToLower())
                              select new PromotionModel
                              {
                                  PromotionId = _promo.PromotionId,
                                  PromoCode = _promoCoupon.Code,
                                  Name = _promo.Name,
                                  Description = _promo.Description,
                                  PromotionTypeId = _promo.PromotionTypeId,
                                  Discount = _promo.Discount,
                                  StartDate = _promo.StartDate,
                                  EndDate = _promo.EndDate,
                                  OrderMinimum = _promo.OrderMinimum,
                                  QuantityMinimum = _promo.QuantityMinimum,
                                  IsCouponRequired = _promo.IsCouponRequired,
                                  DisplayOrder = _promo.DisplayOrder,
                                  IsUnique = _promo.IsUnique,
                                  PortalId = _promo.PortalId,
                                  ProfileId = _promo.ProfileId,
                                  PromotionProductQuantity = _promo.PromotionProductQuantity,
                                  ReferralPublishProductId = _promo.ReferralPublishProductId,
                                  PromotionMessage = _promo.PromotionMessage,
                                  IsAllowedWithOtherCoupons = _promo.IsAllowedWithOtherCoupons,
                                  PromotionType = new PromotionTypeModel
                                  {
                                      ClassName = _promoType.ClassName,
                                      ClassType = _promoType.ClassType,
                                      IsActive = _promoType.IsActive,
                                      Name = _promoType.Name,
                                      Description = _promoType.Description,
                                      PromotionTypeId = _promoType.PromotionTypeId
                                  }
                              })?.ToList();
            }
            return promotions ?? new List<PromotionModel>();
        }

        protected virtual void CalculateCouponDiscount(List<PromotionModel> activePromotions, int cartPromoIndex, int cartPromoCouponIndex)
        {
            if (cartPromoCouponIndex == 0)
            {
                // coupon apply condition from Admin panel Manage Order
                ApplyCouponDiscount(activePromotions, cartPromoIndex, cartPromoCouponIndex);
            }
            else
            {
                if (_shoppingCart.CartAllowsMultiCoupon && _shoppingCart.Coupons[cartPromoCouponIndex].AllowsMultipleCoupon)
                {
                    ApplyCouponDiscount(activePromotions, cartPromoIndex, cartPromoCouponIndex);
                }
                else
                {
                    string couponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? Api_Resources.CouponMessage : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                    SetCouponDetails(cartPromoCouponIndex, false, false, couponMessage);
                }
                    
            }
        }

        private void ApplyCouponDiscount(List<PromotionModel> activePromotions, int cartPromoIndex, int cartPromoCouponIndex)
        {
            if (!IsDiscountApplicable())
            {
                string couponMessage = Api_Resources.NoFurtherCouponMessage;
                SetCouponDetails(cartPromoCouponIndex, false, false, couponMessage);
            }
            else
            {
                CalculateCouponPromotion(cartPromoIndex, cartPromoCouponIndex, activePromotions);
            }
        }

        //Calculate each and every Coupon promotion applied on cart.
        protected virtual void CalculateCouponPromotion(int cartPromoIndex, int cartPromoCouponIndex, List<PromotionModel> allPromotions)
        {
            _cartPromotions[cartPromoIndex].Calculate(cartPromoCouponIndex, allPromotions);

            if (!Equals(_shoppingCart.Coupons, null) && _shoppingCart.Coupons.Count > 0 && !Equals(_shoppingCart.Coupons[cartPromoCouponIndex], null) && _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied)
            {
                if (cartPromoCouponIndex == 0)
                    _shoppingCart.ClearPreviousErrorMessages();
                _shoppingCart.Coupons[cartPromoCouponIndex].IsExistInOrder = false;
                _shoppingCart.AddPromoDescription += _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
            }
            else
            {
                string couponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? Api_Resources.InvalidCouponCode : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                SetCouponDetails(cartPromoCouponIndex, false, false, couponMessage);
            }
        }

        //Create and return instance for promotion classes
        protected virtual T GetPromotionTypeInstance<T>(PromotionModel promotionModel) where T : IZnodeCartPromotionType
        {
            if (HelperUtility.IsNotNull(promotionModel))
            {
                string className = promotionModel.GetClassName();
                return GetPromotionTypeInstance<T>(className);
            }
            else
                return default(T);
        }

        //to get cart coupons
        protected virtual string GetCartCoupons()
        {
            return string.Join(",", _shoppingCart.Coupons.Select(coupon => coupon.Coupon));
        }

        //Get the instance of specific promotion type
        protected virtual T GetPromotionTypeInstance<T>(string className) where T : IZnodeCartPromotionType
        {
            if (!string.IsNullOrEmpty(className))
                return (T)GetKeyedService<IZnodePromotionsType>(className);
            else
                return default(T);
        }

        protected virtual void ApplyNewCouponDiscount(int cartPromoIndex, int cartPromoCouponIndex, Libraries.ECommerce.Entities.ZnodeCoupon znodeCoupon)
        {
            List<PromotionModel> newCoupons = activeCoupons.FindAll(x => string.Equals(x.PromoCode, znodeCoupon.Coupon, StringComparison.OrdinalIgnoreCase));
            newCoupons = promotionHelper.GetActivePromotions(_shoppingCart.PortalId, _shoppingCart.ProfileId, newCoupons);
            if (HelperUtility.IsNotNull(newCoupons))
                CalculateCouponDiscount(newCoupons, cartPromoIndex, cartPromoCouponIndex);
        }

        private void SetCouponDetails(int cartPromoCouponIndex, bool couponValid, bool couponApplied, string couponMessage)
        {
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = couponValid;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = couponApplied;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = couponMessage;
        }


        // This function is used to calculate remaining discount amount on line item in case of delete from manage order screen.
        protected virtual decimal CalculateRemainingDiscountOnCartItemDelete(List<OrderPromotionModel> orderPromotionList)
        {
            decimal remainingDiscountAmount = 0;
            if (HelperUtility.IsNotNull(orderPromotionList) && orderPromotionList.Count > 0)
            {
                foreach (OrderPromotionModel lineitemDiscount in orderPromotionList)
                {
                    if (!_shoppingCart.ShoppingCartItems.Any(x => x.OmsOrderLineItemId == lineitemDiscount.OmsOrderLineItemId || x.ParentOmsOrderLineItemsId == lineitemDiscount.OmsOrderLineItemId))
                        remainingDiscountAmount = remainingDiscountAmount + lineitemDiscount.TotalDiscountAmount;
                }
            }
            return remainingDiscountAmount;
        }

        //to add order line item discount
        protected virtual List<OrderDiscountModel> SetOrderDiscountDetails(OrderPromotionModel orderPromotionModel, List<OrderDiscountModel> orderDiscount)
        {
            if (orderPromotionModel.TotalDiscountAmount == 0)
                return orderDiscount;

            if (HelperUtility.IsNull(orderDiscount) || orderDiscount?.Count == 0)
                orderDiscount = new List<OrderDiscountModel>();

            orderDiscount.Add(new OrderDiscountModel
            {
                OmsDiscountTypeId = orderPromotionModel.OmsDiscountTypeId,
                DiscountAmount = orderPromotionModel.TotalDiscountAmount,
                OriginalDiscount = orderPromotionModel.PerQuantityDiscount,
                DiscountCode = orderPromotionModel.DiscountCode,
                PromotionName = orderPromotionModel.PromotionName,
                DiscountMultiplier = orderPromotionModel.DiscountMultiplier,
                DiscountLevelTypeId = orderPromotionModel.DiscountLevelTypeId,
                PromotionTypeId = orderPromotionModel.PromotionTypeId,
                DiscountAppliedSequence= orderPromotionModel.DiscountAppliedSequence,
                PromotionMessage = orderPromotionModel.PromotionMessage
            });
            return orderDiscount;
        }

        //Return list of discount applicable on line item
        protected virtual List<OrderPromotionModel> GetLineItemApplicableDiscountList(string sku,ZnodeShoppingCartItem cartItem, List<OrderPromotionModel> listOfDiscountInOrder, bool isConfigurableProduct =false, bool isAddOnProduct = false)
        {
            List<OrderPromotionModel> lineItemApplicableDiscount = new List<OrderPromotionModel>();
            if (!string.IsNullOrEmpty(sku))
                lineItemApplicableDiscount.AddRange(listOfDiscountInOrder?.FindAll(x => x.Sku == sku && (x.OmsOrderLineItemId == cartItem.OmsOrderLineItemId || x.OmsOrderLineItemId == cartItem.ParentOmsOrderLineItemsId ||x.ParentOmsOrderLineItemsId == cartItem.ParentOmsOrderLineItemsId) && (x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel || x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)));


            // In Ship separate condition few discount is not gets due to mismatched of SKU to handle this condition have used Parent Product SKU.
            if(isConfigurableProduct)
            {
                lineItemApplicableDiscount.AddRange(listOfDiscountInOrder?.FindAll(x => x.Sku == cartItem.Product.SKU && (x.OmsOrderLineItemId == cartItem.OmsOrderLineItemId || x.OmsOrderLineItemId == cartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == cartItem.ParentOmsOrderLineItemsId) && (x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel || x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)));
            }
            else if(isAddOnProduct)
            {

                // In Omsline item for addon Product not matched with Parent Product hence checked here OmsOrderLineItemId and ParentOmsOrderLineItemsId should be contain atleast in one.
                lineItemApplicableDiscount.AddRange(listOfDiscountInOrder?.FindAll(x => x.Sku == sku && (x.OmsOrderLineItemId == cartItem.OmsOrderLineItemId || x.ParentOmsOrderLineItemsId == cartItem.OmsOrderLineItemId) && (x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel || x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)));
            }

            lineItemApplicableDiscount = lineItemApplicableDiscount.OrderBy(x => x.DiscountAppliedSequence).ToList(); 
            return lineItemApplicableDiscount;
        }

        // To get the OmslineItem in case of ship separate because in this case OmsOrderLineItemId and parentOmsOrderLineItemsId get 0 hence we are fetching it from 
        protected virtual ZnodeShoppingCartItem GetOmsOrderLineItem(ZnodeShoppingCartItem cartItem)
        {
            return _shoppingCart.ShoppingCartItems.Where(x => x.ParentProductId == cartItem.ParentProductId && x.ParentProductSKU == cartItem.ParentProductSKU && x.ParentOmsOrderLineItemsId.HasValue)?.FirstOrDefault();
        }
        //Return list of discount applicable on order
        protected virtual List<OrderPromotionModel> GetOrderApplicableDiscountList(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> orderApplicableDiscount = listOfDiscountInOrder.FindAll(x => x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel && x.OmsOrderLineItemId == null).ToList();
            return orderApplicableDiscount;
        }

        protected virtual List<OrderPromotionModel> GetApplicablePromoCouponList(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> orderApplicableDiscount = listOfDiscountInOrder.FindAll(x => x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel || x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel).OrderBy(x=>x.DiscountAppliedSequence).Distinct().ToList();
            return orderApplicableDiscount;
        }

        //Return list of Shipping applicable on order
        protected virtual List<OrderPromotionModel> GetShippingApplicableDiscountList(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            List<OrderPromotionModel> shippingApplicableDiscount = listOfDiscountInOrder.FindAll(x => x.OmsOrderLineItemId == null && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.ShippingLevel);
            return shippingApplicableDiscount;
        }

        protected virtual void CalculateShippingPromotion(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            ClearShippingDiscount(listOfDiscountInOrder);
            List<PromotionModel> activePromotions = promotionHelper.GetActiveShippingPromotions(_shoppingCart.PortalId, _shoppingCart.ProfileId);

            //If IsRemoveShippingDiscount is true if orderline item  shipping edit from manager order so remove shipping promotion
            if (HelperUtility.IsNotNull(activePromotions) && _shoppingCart.IsRemoveShippingDiscount)
            {
                foreach (PromotionModel promotion in activePromotions)
                {
                    bool? isExist = _shoppingCart.InvalidOrderLevelShippingPromotion?.Exists(x => string.Equals(x.PromoCode, promotion.PromoCode, StringComparison.InvariantCultureIgnoreCase));

                    if (!Convert.ToBoolean(isExist))
                    {
                        _shoppingCart.InvalidOrderLevelShippingPromotion?.Add(promotion);
                    }
                }
            }

            if (HelperUtility.IsNotNull(_shoppingCart.InvalidOrderLevelShippingPromotion) && _shoppingCart.InvalidOrderLevelShippingPromotion.Count == 0)
            {
                //For Manage Order we are not validating Allow promotions and coupons together and Promotions with exceptions condition. 
                //Admin User can apply Promotion or Coupons together irrespective of values of Allow promotions and coupons together and Promotions with exceptions
                foreach (PromotionModel item in activePromotions?.Where(x => x.IsCouponRequired == false))
                {
                    item.IsAllowWithOtherPromotionsAndCoupons = true;
                }
                BuildPromotionsList(activePromotions, _shoppingCart.PortalId, _shoppingCart.ProfileId);
                for (int cartPromoIndex = 0; cartPromoIndex < _cartPromotions.Count; cartPromoIndex++)
                {
                    CalculatePromotionDiscount(activePromotions, cartPromoIndex);
                }
            }
        }

        protected virtual void CalculatePromotionDiscount(List<PromotionModel> activePromotions, int cartPromoIndex)
        {
            _cartPromotions[cartPromoIndex].Calculate(null, activePromotions);
        }

        protected virtual void BuildPromotionsList(List<PromotionModel> promotionsList, int? currentPortalId, int? currentProfileId)
        {
            foreach (PromotionModel promotion in promotionsList ?? new List<PromotionModel>())
            {
                var promoBag = ZnodePromotionBag(promotion, currentPortalId, currentProfileId);
                AddPromotionType(promotion, promoBag);
            }
        }

        protected virtual void ClearShippingDiscount(List<OrderPromotionModel> listOfDiscountInOrder)
        {
            string shippingDiscountCode = listOfDiscountInOrder.Find(x => x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.ShippingLevel)?.DiscountCode;
            if (!string.IsNullOrEmpty(shippingDiscountCode))
            {
                Znode.Libraries.ECommerce.Entities.ZnodeCoupon shippingCoupon = _shoppingCart.Coupons.Find(x => x.Coupon == shippingDiscountCode && x.IsExistInOrder == true);
                    if (shippingCoupon != null)
                    _shoppingCart.Coupons.Remove(shippingCoupon);
            }

            _shoppingCart.Shipping.ShippingDiscount = 0;

        }


        //To check the orderline item shipping edit if yes then add all shipping promotion in removed list
        protected virtual bool IsOrderLineItemShippingEdit(PromotionModel activePromotionModel, Libraries.ECommerce.Entities.ZnodeCoupon shoppingCartCoupon, List<Libraries.ECommerce.Entities.ZnodeCoupon> removeShippingCouponList)
        {
            if(_shoppingCart.IsRemoveShippingDiscount)
            {
                if (HelperUtility.IsNotNull(activePromotionModel))
                {
                    string promotionType = activePromotionModel.PromotionType.ClassName;

                    if (!string.IsNullOrEmpty(promotionType) && (string.Equals(promotionType, ZnodeConstant.AmountOffShipping, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionType, ZnodeConstant.AmountOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionType, ZnodeConstant.PercentOffShipping, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionType, ZnodeConstant.PercentOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        removeShippingCouponList.Add(shoppingCartCoupon);
                        return false;
                    }
                }
                else
                    return false;
            }
            return true;
        }

        //Remove the shipping related coupons and promotion if shipping is edit for orderline item
        protected virtual void RemoveShippingCouponFromCart(List<Libraries.ECommerce.Entities.ZnodeCoupon> removeShippingCouponList)
        {            
            if (HelperUtility.IsNotNull(removeShippingCouponList) && removeShippingCouponList?.Count > 0)
            {
                foreach (Libraries.ECommerce.Entities.ZnodeCoupon znodeCoupon in removeShippingCouponList)
                {
                    _shoppingCart.Coupons.Remove(znodeCoupon);
                }
            }
        }

        //Check the discount is removed because of orderline shipping is edit.
        protected virtual bool IsInvalidShippingDiscount(OrderPromotionModel shippingDiscount)
        {
            if(HelperUtility.IsNotNull(shippingDiscount) && HelperUtility.IsNotNull(_shoppingCart.InvalidOrderLevelShippingDiscount) && _shoppingCart.InvalidOrderLevelShippingDiscount.Count > 0)
            {
                OrderPromotionModel orderPromotionModel = _shoppingCart.InvalidOrderLevelShippingDiscount?.FirstOrDefault(x => string.Equals(x.DiscountCode, shippingDiscount.DiscountCode, StringComparison.InvariantCultureIgnoreCase));

                if (HelperUtility.IsNotNull(orderPromotionModel))
                {
                    return false;
                }
            }
            return true;
        }              

        #endregion
    }

}

