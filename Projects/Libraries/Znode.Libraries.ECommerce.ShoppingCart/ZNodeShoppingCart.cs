using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Promotions;
using Znode.Engine.Shipping;
using Znode.Engine.Taxes;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using ZnodeEntityType = Znode.Libraries.ECommerce.Entities;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    /// <summary>
    /// Represents Shopping cart and shopping cart items
    /// </summary>
    [Serializable()]
    public class ZnodeShoppingCart : Entities.ZnodeShoppingCart, IZnodeShoppingCart
    {
        #region Private Variables        
        private Dictionary<string, decimal> SKUQuantity;
        private List<IZnodePortalCart> portalCarts = new List<IZnodePortalCart>();
        private readonly IPublishProductHelper publishProductHelper;
        private readonly IZnodeOrderHelper orderHelper;
        private readonly IZnodeQuoteHelper quotehelper;
        private int _catalogVersionId = 0;
        protected readonly IOrderInventoryManageHelper _orderInventoryManageHelper;
        private readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity;
        private readonly IZnodeRepository<ZnodePublishBundleProductEntity> _publishBundleProductEntity;

        #endregion

        #region Constructor
        public ZnodeShoppingCart()
        {
            publishProductHelper = GetService<IPublishProductHelper>();
            orderHelper = GetService<IZnodeOrderHelper>();
            _orderInventoryManageHelper = GetService<IOrderInventoryManageHelper>();
            quotehelper = GetService<IZnodeQuoteHelper>();
            _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
            _publishBundleProductEntity = new ZnodeRepository<ZnodePublishBundleProductEntity>(HelperMethods.Context);
        }
        #endregion

        #region Public Properties
        // Get Portal based cart items.
        public List<IZnodePortalCart> PortalCarts
        {
            get
            {
                if (!portalCarts.Any() ||
                     !Equals(portalCarts.Sum(x => x.ShoppingCartItems.Count), ShoppingCartItems.Count) ||
                     !Equals(portalCarts.Sum(x => x.ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(s => s.Quantity)),
                     ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(s => s.Quantity)))
                {
                    int portalId = GetHeaderPortalId();
                    portalCarts =
                   ShoppingCartItems.Cast<ZnodeShoppingCartItem>()
                       .Select(x => IsNotNull(x.Product) && x.Product.PortalID > 0 ? x.Product.PortalID : portalId)
                       .Distinct()
                        .Select(x => GetService<IZnodePortalCart>(
                           new ZnodeNamedParameter("portalId", x),
                       new ZnodeNamedParameter("addressId", x),
                        new ZnodeNamedParameter("shoppingCartItems", ShoppingCartItems)
                       )).ToList();
                }

                var portalCart = portalCarts.FirstOrDefault();

                if (!Equals(portalCart, null) && !Equals(Coupons, null))
                {
                    for (int couponIndex = 0; couponIndex < Coupons.Count; couponIndex++)
                    {
                        if (portalCart.Coupons.Count > 0 && !Equals(portalCart.Coupons[couponIndex].Coupon, Coupons[couponIndex].Coupon))
                        {
                            if (!string.IsNullOrEmpty(Coupons[couponIndex].Coupon))
                            {
                                portalCart.AddCouponCode(Coupons[couponIndex].Coupon);
                            }
                            else
                            {
                                portalCart.AddCouponCode(string.Empty);
                            }
                        }
                    }
                }
                return portalCarts;
            }
        }

        /// <summary>
        /// Gets or sets the Shoppingcart Items
        /// </summary>
        [XmlIgnore()]
        public new ZnodeGenericCollection<ZnodeShoppingCartItem> ShoppingCartItems
        {
            get
            {
                ZnodeGenericCollection<ZnodeShoppingCartItem> _cartItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
                foreach (Znode.Libraries.ECommerce.Entities.ZnodeShoppingCartItem item in base.ShoppingCartItems)
                {
                    _cartItems.Add((ZnodeShoppingCartItem)item);
                }

                return _cartItems;
            }

            set
            {
                foreach (ZnodeShoppingCartItem item in value)
                {
                    base.ShoppingCartItems.Add(item);
                }
            }
        }

        public bool IsMultipleShipToAddress => PortalCarts.SelectMany(x => x.AddressCarts).Count() > PortalCarts.Count;

        /// <summary>
        /// Gets or sets the Shoppingcart Items
        /// </summary>
        [XmlIgnore()]
        public ZnodeGenericCollection<ZnodeShoppingCartItem> SplittedShoppingCartItems
        {
            get
            {
                ZnodeGenericCollection<ZnodeShoppingCartItem> _cartItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();

                foreach (Znode.Libraries.ECommerce.Entities.ZnodeShoppingCartItem item in ShoppingCartItems)
                {
                    int i = 0;
                    while (i < item.Quantity)
                    {
                        _cartItems.Add((ZnodeShoppingCartItem)item);
                        i++;
                    }
                }
                return _cartItems;
            }
        }

        // Gets the total cost of items in the shopping cart before shipping and taxes
        public override decimal SubTotal => ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ExtendedPrice);

        private decimal? discount = null;

        // Gets total discount of applied to the items in the shopping cart.
        public override decimal Discount
        {
            get
            {
                if (IsNotNull(discount))
                    return discount.GetValueOrDefault();

                decimal totalDiscount = OrderLevelDiscount + ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ExtendedPriceDiscount + x.DiscountAmount);
                decimal totalrefundAmount = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.PartialRefundAmount) ?? 0;

                totalDiscount = totalDiscount + totalrefundAmount;

                return totalDiscount;
            }

            set
            {
                discount = value;
            }
        }

        // Gets the total cost before voucher consideration and after shipping, taxes and promotions       
        public override decimal OrderTotalWithoutVoucher => GetService<IZnodeOrderHelper>().GetPriceRoundOff((SubTotal - Discount) + ShippingCost + ShippingHandlingCharges + ReturnCharges + OrderLevelTaxes + ImportDuty - CSRDiscount + GetAdditionalPrice() - (ShippingCost > 0 ? Shipping.ShippingDiscount : 0));

        // Gets Order total which is Amount to be paid shown after voucher consideration
        public override decimal Total => OrderTotalWithoutVoucher - GiftCardAmount;

        //Get or Set CookieMappingId this value is in encrypted format 
        public int CookieMappingId { get; set; }

        //Get or Set total of all additional cost associated with each cartline item if any
        public override decimal TotalAdditionalCost { get => GetAdditionalPrice(); set { } }

        public virtual List<OrderWarehouseLineItemsModel> LowInventoryProducts { get; set; }

        public virtual List<Entities.ZnodeShoppingCartItem> ZnodeShoppingCartItemCollection
        {
            get
            {
                return base.ShoppingCartItems;
            }
            set
            {
                base.ShoppingCartItems = value;
            }
        }       

        #endregion

        #region Public Methods           

        /// <summary>
        /// Calculates final pricing, shipping and taxes in the cart.
        /// </summary>
        // Pass profile ID as null to the overload
        public virtual void Calculate()
        {
            Calculate(null);
        }

        public virtual void Calculate(int? profileId, bool isCalculateTaxAndShipping = true, bool isCalculatePromotionAndCoupon = true)
        {
            // Clear previous error message
            ClearPreviousErrorMessages();
            bool isCouponAvailable;

            //// TaxRules
            if (isCalculateTaxAndShipping)
            {
                if (!this.SkipShippingCalculations || CheckFreeShipping())
                {
                    //Initialize  ZnodeShippingManager and calculate shipping cost.
                    IZnodeShippingManager shippingManager = GetService<IZnodeShippingManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
                    shippingManager.Calculate();
                }
            }

            IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", this), new ZnodeNamedParameter("profileId", profileId));

            if (isCalculatePromotionAndCoupon)
            {
                cartPromoManager.Calculate();
            }
            else
            {
                cartPromoManager.ClearAllPromotionsAndCoupons();
                cartPromoManager.SetPromotionalPrice();
            }


            List<ShoppingCartDiscountModel> productDiscount = new List<ShoppingCartDiscountModel>();
            // Promotions calculation starts

            //to save previous discount amount of product before calculating Promotions
            GetDiscountFromShoppingCart(productDiscount);

            isCouponAvailable = productDiscount.Count > 0;

            if (isCouponAvailable || IsAnyPromotionApplied)
            {
                productDiscount.Clear();
                GetDiscountFromShoppingCart(productDiscount);
            }

            if (productDiscount.Count > 0)
            {
                SetShoppingCartDiscount(productDiscount);
            }

            GiftCardAmount = 0;
            IsGiftCardApplied = false;
            GiftCardMessage = string.Empty;
            CSRDiscountApplied = false;
            CSRDiscountMessage = string.Empty;
            //to apply csr discount amount
            if (CSRDiscountAmount > 0)
            {
                AddCSRDiscount(CSRDiscountAmount);
            }

            //calculate PercentOffShipping promotion after shipping is calculated in order to get the calculated shipping cost
            //if (isCalculatePromotionAndCoupon)
            //{
            //    if (cartPromoManager.CartPromotionCache.Any(x => x.PromotionType.ClassName == ZnodeConstant.PercentOffShipping || x.PromotionType.ClassName == ZnodeConstant.PercentOffShippingWithCarrier))
            //        cartPromoManager.Calculate();
            //}

            //// TaxRules
            if (isCalculateTaxAndShipping)
            {
                // TaxRules
                ZnodeTaxManager taxManager = new ZnodeTaxManager(this);
                taxManager.Calculate(this);
            }

            if (IsCalculateVoucher && isCalculatePromotionAndCoupon && !IsPendingOrderRequest)
            {
                AddVouchers(this.OrderId);
            }
            if (IsPendingOrderRequest)
            {
                CheckInvalidVoucher();
            }
        }

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if the order should be submitted. False if there is anything that will prevent the order from submitting correctly.</returns>
        public virtual bool PreSubmitOrderProcess(out string isInventoryInStockMessage, out Dictionary<int, string> minMaxSelectableQuantity)
        {
            bool returnVal;
            PreSubmitOrderInitVariables(out isInventoryInStockMessage, out returnVal);

            // ShippingRules
            returnVal = PreOrderSubmitProcessShippingRules(returnVal);

            // TaxRules
            returnVal = PreOrderSubmitProcessTaxRules(returnVal);
          
            // Promotions
            returnVal = PreOrderSubmitProcessPromotions(returnVal);

            // Coupon 
            returnVal = PreOrderSubmitProcessCouponAvailableCheck(returnVal);

      
          
            minMaxSelectableQuantity = IsValidMinAndMaxSelectedQuantity();
            returnVal = IsNotNull(minMaxSelectableQuantity) && minMaxSelectableQuantity.Count > 0 ? false : returnVal;
            return returnVal;
        }

        // Process anything that must be done after the order is submitted.
        public virtual void CancelTaxOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            // TaxRules
            IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            taxManager.CancelOrderRequest(shoppingCartModel);
        }

        // Process anything that must be done after the order is submitted.
        public virtual void ReturnOrderLineItem(ShoppingCartModel shoppingCartModel)
        {
            // TaxRules
            IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            taxManager.ReturnOrderLineItem(shoppingCartModel);
        }

        /// <summary>
        /// Process anything that must be done after the order has been submitted.
        /// </summary>
        public virtual void PostSubmitOrderProcess(int orderId = 0, bool isGuest = true, bool isTaxCostUpdated = true)
        {
            // ShippingRules
            IZnodeShippingManager shippingManager = GetService<IZnodeShippingManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            shippingManager.PostSubmitOrderProcess();

            // TaxRules

            IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            taxManager.PostSubmitOrderProcess(this, isTaxCostUpdated);

        }
        public virtual void SubmitTax(bool isTaxCostUpdated = true)
        {
            // TaxRules
            IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            taxManager.PostSubmitOrderProcess(this, isTaxCostUpdated);
        }

        /// <summary>
        /// // Reduce the quantity of available coupons if it is applied to order
        /// </summary>
        public virtual void ReduceCouponsQuantity(int orderId = 0)
        {
            if (!Equals(Coupons, null))
            {
                foreach (Entities.ZnodeCoupon coupon in Coupons.Where(x => x.CouponApplied))
                {
                    // Reduce the quantity of available coupons                        
                    orderHelper.UpdateCouponQuantity(coupon.Coupon, orderId, coupon.IsExistInOrder);

                }
            }
        }

        // Adds a coupon code to the shopping cart.
        public virtual void AddCouponCode(string CouponCode)
        {
            Entities.ZnodeCoupon coupon = new Entities.ZnodeCoupon()
            {
                Coupon = CouponCode,
                CouponApplied = false,
                CouponValid = false
            };
            Coupons.Add(coupon);
        }

        /// <summary>
        /// Add Gift Card to the shopping cart.
        /// </summary>
        /// <param name="giftCardNumber">Unique gift card number.</param>  
        [Obsolete("This method is marked as obsolete from Znode version 9.6.1, instead of this AddVouchers method should be used.")]
        public virtual bool AddGiftCard(string giftCardNumber, int? orderId = null)
        {
            bool success = false;
            string response; //Gets set to GiftCard Message
            string invalidGiftCard = $"Invalid Voucher : {giftCardNumber}";
            string invalidAccountAssociation = $"Voucher '{giftCardNumber}' is not associated with this account.";
            string invalidStoreAssociation = $"Voucher '{giftCardNumber}' is not associated with this store.";

            if (string.IsNullOrEmpty(giftCardNumber))
            {
                IsGiftCardApplied = false;
                GiftCardNumber = string.Empty;
                GiftCardAmount = 0;
            }

            int? userId = GetUserId();
            GiftCardModel giftCard = orderHelper.GetVoucherByCardNumber(giftCardNumber, orderId);

            // Reset previous value.
            GiftCardAmount = 0;
            GiftCardBalance = 0;

            if (giftCard != null)
            {
                if (giftCard.PortalId != 0 && giftCard.PortalId != this.PortalId)
                {
                    response = invalidStoreAssociation;
                    IsGiftCardApplied = false;
                }
                else if (string.IsNullOrEmpty(giftCard.CardNumber))
                {
                    response = invalidGiftCard;
                    IsGiftCardApplied = false;
                }
                else
                {
                    decimal availableBalance = Convert.ToDecimal(giftCard.RemainingAmount);
                    decimal remainingBalance = 0;
                    if (availableBalance > 0)
                    {
                        // Validate the giftcard expiration date.
                        if (giftCard?.ExpirationDate < DateTime.Today.Date)
                        {
                            IsGiftCardValid = false;
                            IsGiftCardApplied = false;
                            response = "Voucher expired.";
                            GiftCardMessage = response;
                            return success;
                        }

                        if (Total > availableBalance)
                        {
                            // Set all available balance to Gift Card Amount.
                            GiftCardAmount = availableBalance;
                            remainingBalance = 0;
                        }
                        else if (Total <= availableBalance)
                        {
                            remainingBalance = Convert.ToDecimal(giftCard.RemainingAmount) - this.Total;
                            GiftCardAmount = Total;
                        }

                        response = string.Format(Admin_Resources.GiftCardAppliedMessage, ZnodeCurrencyManager.FormatPriceWithCurrency(remainingBalance, CultureCode));
                        GiftCardNumber = giftCardNumber;
                        IsGiftCardValid = true;
                        IsGiftCardApplied = true;
                        GiftCardBalance = remainingBalance;
                        success = true;
                    }
                    else
                    {
                        IsGiftCardValid = false;
                        IsGiftCardApplied = false;
                        response = "No balance amount on Voucher";
                    }
                }
            }
            else
            {
                GiftCardNumber = string.Empty;
                IsGiftCardValid = false;
                IsGiftCardApplied = false;
                response = invalidGiftCard;
            }

            GiftCardMessage = response;
            return success;
        }

        /// <summary>
        /// Add Vouchers
        /// </summary>
        /// <param name="orderId"> Order Id</param>
        /// <returns>True/ False</returns>
        public virtual bool AddVouchers(int? orderId = null)
        {
            bool success = false; string response;
            decimal voucherAmountTotal = 0; decimal voucherAmountUsed = 0;
            if (Vouchers.Count == 0 && !IsAllVoucherRemoved)
                AddUserVouchers();


            if (Vouchers?.Count > 0 && Total > 0)
            {
                ClearAllAppliedVouchers();
                foreach (ZnodeVoucher voucher in Vouchers)
                {
                    decimal availableBalance = voucher.IsExistInOrder ? voucher.VoucherAmountUsed : Convert.ToDecimal(voucher.VoucherBalance);
                    string invalidGiftCard = Admin_Resources.ErrorInvalidVoucher;
                    decimal remainingBalance = 0;
                    voucher.CultureCode = CultureCode;
                    if (!string.IsNullOrEmpty(voucher.VoucherNumber) && voucher.PortalId != 0 && voucher.PortalId == this.PortalId)
                    {
                        if (availableBalance > 0)
                        {
                            if (Total > availableBalance)
                            {
                                //Set all available balance to Gift Card Amount. 
                                voucherAmountTotal = voucherAmountTotal + availableBalance;
                                GiftCardAmount = voucherAmountTotal;
                                remainingBalance = voucher.IsExistInOrder ? voucher.VoucherBalance : 0;
                                voucherAmountUsed = availableBalance;
                            }
                            else if (Total <= availableBalance)
                            {
                                remainingBalance = voucher.IsExistInOrder ? voucher.VoucherBalance : Convert.ToDecimal(voucher.VoucherBalance) - this.Total;
                                GiftCardAmount = voucherAmountTotal + Total;
                                voucherAmountUsed = voucher.IsExistInOrder ? availableBalance : availableBalance - remainingBalance;
                                response = string.Format(Admin_Resources.GiftCardAppliedMessage, ZnodeCurrencyManager.FormatPriceWithCurrency(remainingBalance, CultureCode));
                                MapAppliedVoucherResponce(voucher, response, remainingBalance, voucherAmountUsed, true, true);
                                RemoveInvalidUserVouchers();
                                return true;
                            }
                            response = string.Format(Admin_Resources.GiftCardAppliedMessage, ZnodeCurrencyManager.FormatPriceWithCurrency(remainingBalance, CultureCode));
                            MapAppliedVoucherResponce(voucher, response, remainingBalance, voucherAmountUsed, true, true);

                        }
                        else
                        {
                            response = Admin_Resources.ErrorZeroBalanceAmountVoucher;
                            MapAppliedVoucherResponce(voucher, response, remainingBalance, 0, false, false);
                        }
                    }
                    else
                        MapAppliedVoucherResponce(voucher, invalidGiftCard, 0, 0, false, false);
                }
            }
            return success;
        }

        // Set user valid vouchers.
        protected virtual void AddUserVouchers()
        {
            List<GiftCardModel> giftCardModels = orderHelper.GetUserVouchers(Convert.ToInt32(GetUserId()), this.PortalId);
            foreach (GiftCardModel giftCard in giftCardModels)
            {
                Entities.ZnodeVoucher voucher = new Entities.ZnodeVoucher()
                {
                    VoucherNumber = giftCard.CardNumber,
                    VoucherBalance = Convert.ToDecimal(giftCard.RemainingAmount),
                    ExpirationDate = giftCard.ExpirationDate,
                    VoucherName = giftCard.Name,
                    PortalId = giftCard.PortalId,
                    UserId = giftCard.UserId,
                    IsVoucherApplied = false,
                    IsVoucherValid = true
                };
                Vouchers.Add(voucher);
            }
        }

        // Remove invalid or not applied vouchers. 
        protected void RemoveInvalidUserVouchers()
        {
            if (Vouchers?.Count > 0)
            {
                Vouchers.RemoveAll(x => !x.IsVoucherValid && string.IsNullOrEmpty(x.VoucherMessage));
            }

        }

        //Clear all applied vouchers. 
        protected void ClearAllAppliedVouchers()
        {
            //to clear all promotions and coupons applied 
            if (Vouchers?.Count > 0)
            {
                int userId = GetUserId().GetValueOrDefault();
                foreach (ZnodeVoucher voucher in Vouchers)
                {
                    GiftCardModel giftCard = orderHelper.GetByCardNumber(voucher.VoucherNumber);
                    if (IsNotNull(giftCard))
                    {
                        if (((IsNull(giftCard.UserId) || giftCard.UserId == userId) && giftCard.RestrictToCustomerAccount && !voucher.IsExistInOrder) || (!giftCard.RestrictToCustomerAccount && !voucher.IsExistInOrder))
                        {
                            voucher.VoucherBalance = voucher.IsExistInOrder ? voucher.VoucherAmountUsed : Convert.ToDecimal(giftCard.RemainingAmount);
                            voucher.ExpirationDate = giftCard.ExpirationDate;
                            voucher.VoucherName = giftCard.Name;
                            voucher.PortalId = giftCard.PortalId;
                            voucher.UserId = giftCard.UserId;
                        }
                    }
                    voucher.IsVoucherValid = true;
                    voucher.IsVoucherApplied = false;
                    voucher.VoucherMessage = string.Empty;
                    voucher.VoucherAmountUsed = voucher.IsExistInOrder ? voucher.VoucherAmountUsed : 0;
                    voucher.OrderVoucherAmount = voucher.OrderVoucherAmount;
                }

                Vouchers = Vouchers.OrderBy(c => c.ExpirationDate).ThenBy(c => c.VoucherBalance).ToList();
            }
        }

        // Map applied voucher response to model.
        protected void MapAppliedVoucherResponce(ZnodeVoucher voucher, string cardMessage, decimal remainingBalance, decimal voucherAmountUsed, bool isVoucherValid, bool isVoucherApplied)
        {
            if (IsNotNull(voucher))
            {
                voucher.VoucherMessage = cardMessage;
                voucher.IsVoucherValid = isVoucherValid;
                voucher.IsVoucherApplied = isVoucherApplied;
                voucher.VoucherBalance = remainingBalance;
                voucher.VoucherAmountUsed = voucherAmountUsed;
            }
        }

        /// <summary>
        ///  Add CSR Discount to the shopping cart.
        /// </summary>
        /// <param name="discountAmount"></param>
        /// <returns></returns>
        public virtual bool AddCSRDiscount(decimal discountAmount)
        {
            bool success = false;
            string response = string.Empty;
            if (discountAmount > 0)
            {
                decimal totalAmount = Total;

                if (SubTotal < (discountAmount + Discount))
                {
                    response = Api_Resources.ErrorDiscountExceeded;
                    success = false;
                }
                else
                {
                    if (totalAmount > discountAmount)
                    {
                        CSRDiscount = discountAmount;
                    }
                    else if (totalAmount <= discountAmount && CSRDiscount == 0)
                    {
                        CSRDiscount = Total;
                    }
                    CSRDiscountAmount = CSRDiscount;
                    if(CSRDiscountEdited)
                       response = $"Discount of {ZnodeCurrencyManager.FormatPriceWithCurrency(CSRDiscountAmount, CultureCode)} applied successfully.";
                    success = true;
                }

            }
            else
            {
                CSRDiscountAmount = 0;
                response = "Unable to apply discount.";
                success = false;
            }
            CSRDiscountMessage = response;
            CSRDiscountApplied = success;
            return success;
        }

        //to check inventory of products, addons, bundle, group and configurable product in the shopping cart if inventory set to 'disable purchasing for out of stock product'. 
        public virtual bool IsInventoryInStock()
        {
            //Initialize SKU quantity per line items
            PreOrderSubmitProcessInItSKUQuantity();

            //Get inventory list
            List<InventorySKUModel> inventoryList = GetInventoryList();

            //Check quantity with in-stock inventory
            bool isInventoryInStock = CheckWithInStockInventory(inventoryList);

            //Clear variable
            SKUQuantity = null;

            return isInventoryInStock;
        }

        /// <summary>
        /// Check the specified quantity with in-stock inventory
        /// </summary>
        /// <param name="isInventoryInStock"></param>
        /// <param name="inventoryList"></param>
        /// <returns></returns>
        public bool CheckWithInStockInventory(List<InventorySKUModel> inventoryList)
        {
            bool isInventoryInStock = true;
            try
            {
                if (inventoryList?.Count > 0)
                {
                    foreach (InventorySKUModel inventory in inventoryList)
                    {
                        if (ShoppingCartItems?.Cast<ZnodeShoppingCartItem>()
                                             ?.FirstOrDefault(o => o.SKU == inventory.SKU)
                                             ?.Product
                                             ?.InventoryTracking == ZnodeConstant.DontTrackInventory)
                        {
                            isInventoryInStock = isInventoryInStock && true;
                        }
                        else
                        {
                            if (inventory.Quantity < GetQuantityBySKU(inventory.SKU))
                            {
                                isInventoryInStock = isInventoryInStock && false;
                            }
                        }
                    }
                }
                SKUQuantity = null;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                SKUQuantity = null;
                throw ex;
            }
            return isInventoryInStock;

        }

        /// <summary>
        /// Get inventory list
        /// </summary>
        /// <returns></returns>
        public List<InventorySKUModel> GetInventoryList()
        {
            List<InventorySKUModel> inventoryList = new List<InventorySKUModel>();
            try
            {
                List<string> skus = SKUQuantity.Select(x => x.Key).ToList();
                int? portalId = this.PortalId ?? ZnodeConfigManager.SiteConfig.PortalId;

                inventoryList = publishProductHelper.GetInventoryBySKUs(skus, portalId.GetValueOrDefault());

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                SKUQuantity = null;
                throw ex;
            }
            return inventoryList;
        }

        /// <summary>
        /// Initialize the SKU Quantity dictionary
        /// </summary>
        public void PreOrderSubmitProcessInItSKUQuantity()
        {
            try
            {
                SKUQuantity = new Dictionary<string, decimal>();

                // to check inventory loop through the order line items
                foreach (ZnodeShoppingCartItem item in ShoppingCartItems)
                {
                    AddProductInDictionary(item);
                }

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                SKUQuantity = null;
                throw ex;
            }
        }

        protected bool PreOrderSubmitProcessInventoryCheck(bool returnVal, out string isInventoryInStockMessage)
        {
            returnVal = IsInventoryInStock();
            isInventoryInStockMessage = !returnVal ? "Unable to place the order as product is not available." : "";
            return returnVal;
        }

        /// <summary>
        /// Check if the coupon is available
        /// </summary>
        /// <param name="returnVal"></param>
        /// <returns></returns>
        public bool PreOrderSubmitProcessCouponAvailableCheck(bool returnVal)
        {
            if (!Equals(Coupons, null))
            {
                foreach (Entities.ZnodeCoupon coupon in Coupons)
                    returnVal &= IsCouponQuantityAvailable(coupon.Coupon);
            }

            return returnVal;
        }

        /// <summary>
        /// Process promotions for the pre-order submit process
        /// </summary>
        /// <param name="returnVal"></param>
        /// <returns></returns>
        public bool PreOrderSubmitProcessPromotions(bool returnVal)
        {
            IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", this), new ZnodeNamedParameter("profileId", null));
            returnVal &= cartPromoManager.PreSubmitOrderProcess();
            return returnVal;
        }

        /// <summary>
        /// Process the tax rules for pre order submit 
        /// </summary>
        /// <param name="returnVal"></param>
        /// <returns></returns>
        public bool PreOrderSubmitProcessTaxRules(bool returnVal)
        {
            IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            returnVal &= taxManager.PreSubmitOrderProcess(this);
            return returnVal;
        }

        /// <summary>
        /// Pre order submit process the shipping rule.
        /// </summary>
        /// <param name="returnVal"></param>
        /// <returns></returns>
        public bool PreOrderSubmitProcessShippingRules(bool returnVal)
        {
            IZnodeShippingManager shippingManager = GetService<IZnodeShippingManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            returnVal &= shippingManager.PreSubmitOrderProcess();
            return returnVal;
        }

        /// <summary>
        /// Initializes the variables used in preorder submit
        /// </summary>
        /// <param name="isInventoryInStockMessage"></param>
        /// <param name="returnVal"></param>
        public void PreSubmitOrderInitVariables(out string isInventoryInStockMessage, out bool returnVal)
        {
            isInventoryInStockMessage = string.Empty;
            // Clear previous error message
            ClearPreviousErrorMessages();

            returnVal = true;
        }

        public virtual Dictionary<int, string> IsValidMinAndMaxSelectedQuantity()
        {
            try
            {
                StringBuilder errorMessage = new StringBuilder();
                Dictionary<int, string> errorDictionary = new Dictionary<int, string>();
                foreach (ZnodeShoppingCartItem item in ShoppingCartItems)
                {
                    foreach (ZnodeProductBaseEntity productBaseEntity in item.Product.ZNodeGroupProductCollection)
                    {
                        if (productBaseEntity.SelectedQuantity < productBaseEntity.MinQty || productBaseEntity.SelectedQuantity > productBaseEntity.MaxQty)
                        {
                            errorMessage.Append(productBaseEntity.Name);
                            errorMessage.Append(",");
                        }

                    }
                }
                string message = Convert.ToString(errorMessage);
                if (!string.IsNullOrEmpty(message))
                {
                    errorDictionary.Add(ErrorCodes.MinAndMaxSelectedQuantityError, string.Format(Admin_Resources.ErrorSelectedQuantity, message.TrimEnd(',')));
                }

                return errorDictionary ?? new Dictionary<int, string>();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in IsValidMinAndMaxSelectedQuantity method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }
        
        //save the shoppingcart items in the database.
        public virtual int Save(ShoppingCartModel shoppingCart, string groupIdProductAttribute = "", GlobalSettingValues groupIdPersonalizeAttribute = null)
        {
            if (IsNotNull(shoppingCart))
            {
                int cookieId = !string.IsNullOrEmpty(shoppingCart.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(shoppingCart.CookieMappingId)) : 0;
                //Get CookieMappingId
                int cookieMappingId = cookieId == 0 ? orderHelper.GetCookieMappingId(shoppingCart.UserId, shoppingCart.PortalId) : cookieId;

                //Get SavedCartId
                int savedCartId = orderHelper.GetSavedCartId(ref cookieMappingId);

                //If the new cookie Mapping Id gets generated, then it should assign back within the requested model.
                shoppingCart.CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString());

                foreach (ShoppingCartItemModel cartItem in shoppingCart.ShoppingCartItems)
                {
                    BindCartProductDetails(cartItem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId, groupIdProductAttribute, groupIdPersonalizeAttribute);
                }


                //Save all shopping cart line items.
                orderHelper.SaveAllCartLineItems(savedCartId, shoppingCart);

                return cookieMappingId;
            }
            return 0;
        }
        //Save the shopping cart items in the database.
        public virtual AddToCartModel SaveAddToCartData(AddToCartModel cartModel, string groupIdProductAttribute = "", GlobalSettingValues groupIdPersonalizeAttribute = null)
        {
            if (IsNotNull(cartModel))
            {
                //Get CookieMappingId               
                int cookieMappingId = (!string.IsNullOrEmpty(cartModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cartModel.CookieMappingId)) : 0) == 0 ? orderHelper.GetCookieMappingId(cartModel.UserId, cartModel.PortalId) : !string.IsNullOrEmpty(cartModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cartModel.CookieMappingId)) : 0;

                foreach (ShoppingCartItemModel cartItem in cartModel.ShoppingCartItems)
                {
                    BindCartProductDetails(cartItem, cartModel.PublishedCatalogId, cartModel.LocaleId, groupIdProductAttribute, groupIdPersonalizeAttribute);
                }

                //Save all shopping cart line items.
                AddToCartStatusModel addToCartStatusModel = orderHelper.SaveAllCartLineItemsInDatabase(cartModel, cookieMappingId);

                cartModel.CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString());
                cartModel.CartCount = addToCartStatusModel.CartCount;
                cartModel.HasError = !addToCartStatusModel.Status;

                return cartModel;
            }
            return null;
        }

        // Get list of all the bundle products child list with associated quantities
        public virtual List<AssociatedPublishedBundleProductModel> BindBundleProductChildByParentSku(string bundleProductSkus, int publishCatalogId, int localeId)
        {
            int portalId = GetPortalId();
            int catalogVersionId = GetCatalogVersionId(publishCatalogId,localeId);
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { bundleProductSkus = bundleProductSkus, catalogVersionId = catalogVersionId, localeId = localeId, publishCatalogId = publishCatalogId });
            IZnodeViewRepository<AssociatedPublishedBundleProductModel> objStoredProc = new ZnodeViewRepository<AssociatedPublishedBundleProductModel>();
            objStoredProc.SetParameter("@SKU", bundleProductSkus, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@versionid", catalogVersionId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@localeId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@publishCatalogId", publishCatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            List<AssociatedPublishedBundleProductModel> publishBundleChildModels = objStoredProc.ExecuteStoredProcedureList("Znode_GetBundleProductAssociatedChildQuantity @SKU,@versionid,@localeId,@publishCatalogId,@PortalId")?.ToList();
            publishBundleChildModels?.ForEach(x => x.Attributes = string.IsNullOrEmpty(x.Attribute) ? null : JsonConvert.DeserializeObject<List<PublishAttributeModel>>(x.Attribute));

            SetBundleInventoryData(publishBundleChildModels);

            return publishBundleChildModels;
        }

        //to load shoppingCart from database by cookieMappingId
        public virtual ZnodeShoppingCart LoadFromDatabase(CartParameterModel cartParameterModel, List<string> expands = null)
        {
            cartParameterModel.CookieId = !string.IsNullOrEmpty(cartParameterModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cartParameterModel.CookieMappingId)) : 0;

            //AccountQuoteLineItemModel contains properties of Account Quotes and Saved cart line items.
            List<AccountQuoteLineItemModel> cartLineItems;

            if (cartParameterModel.OmsQuoteId > 0)
            {
                cartLineItems = GetAccountQuoteLineItems(cartParameterModel);
            }
            else
            {
                //Check if cookieMappingId is null or 0.
                if (string.IsNullOrEmpty(cartParameterModel.CookieMappingId) || cartParameterModel.CookieId == 0)
                {
                    List<ZnodeOmsCookieMapping> cookieMappings = orderHelper.GetCookieMappingList(cartParameterModel);
                    cartParameterModel.CookieId = Convert.ToInt32(cookieMappings?.FirstOrDefault()?.OmsCookieMappingId);
                    cartParameterModel.CookieMappingId = new ZnodeEncryption().EncryptData(cartParameterModel.CookieId.ToString());
                }

                //Get saved cart line items.
                cartLineItems = GetSavedCartLineItems(cartParameterModel);
            }

            List<PublishedConfigurableProductEntityModel> configEntities;
            List<string> skus = cartLineItems.Select(x => x.SKU.ToLower())?.Distinct().ToList();

            if (IsNull(expands))
                expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing, ZnodeConstant.Inventory, ZnodeConstant.AddOns, ZnodeConstant.SEO };

            int catalogVersionId = publishProductHelper.GetCatalogVersionId(cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId);

            List<PublishProductModel> cartLineItemsProductData = publishProductHelper.GetDataForCartLineItems(skus, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId, expands, cartParameterModel.UserId.GetValueOrDefault(), cartParameterModel.PortalId, catalogVersionId, out configEntities, omsOrderId: cartParameterModel.OmsOrderId.GetValueOrDefault());

            List<TaxClassRuleModel> lstTaxClassSKUs = GetTaxRules(skus);

            List<ZnodePimDownloadableProduct> lstDownloadableProducts = new ZnodeRepository<ZnodePimDownloadableProduct>().Table.Where(x => skus.Contains(x.SKU)).ToList();


            //Set Portal Id in Context Header, to avoid loop based calls.
            SetPortalIdInRequestHeader();

            //Get the Saved Cart Line Item ids, to avoid loop based calls.
            List<int?> lstCartLineIds = GetSavedCartLineItemIds(cartLineItems);

            List<PersonaliseValueModel> lstPersonalizedValues = GetService<IZnodeOrderHelper>()?.GetPersonalizedValueCartLineItem(lstCartLineIds);
            //clear existing items in shopping cart           
            foreach (AccountQuoteLineItemModel cartLineItem in cartLineItems)
            {
                bool isConfigurableExists = false;
                if (cartParameterModel.OmsQuoteId > 0)
                {
                    isConfigurableExists = ((cartLineItem.ParentOmsQuoteLineItemId < 1 || cartLineItem.ParentOmsQuoteLineItemId == null) && cartLineItems.Any(lineItem => lineItem?.ParentOmsQuoteLineItemId == cartLineItem.OmsQuoteLineItemId && (lineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles))))
                       ? true
                       : cartLineItems.Any(lineItem => lineItem?.OmsQuoteLineItemId == cartLineItem.ParentOmsQuoteLineItemId && ((cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable)) || (cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)) || (cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Simple))));
                }
                else
                {
                    isConfigurableExists = (cartLineItem.ParentOmsSavedCartLineItemId < 1 && cartLineItems.Any(lineItem => lineItem?.ParentOmsSavedCartLineItemId == cartLineItem.OmsSavedCartLineItemId && ((lineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles)))))
                   ? true
                   : cartLineItems.Any(lineItem => lineItem?.OmsSavedCartLineItemId == cartLineItem.ParentOmsSavedCartLineItemId
                   && ((cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable))
                   || (cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Simple))
                   || (cartLineItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group))
                   ));
                }

                //add new items from savedcartlineitem in shopping cart                              
                if (isConfigurableExists)
                    this.AddToShoppingCartV2(cartLineItem, cartLineItems, cartParameterModel, cartLineItemsProductData, catalogVersionId, lstTaxClassSKUs, lstDownloadableProducts, configEntities, lstPersonalizedValues);
            }
            return this;
        }

        //to load shoppingCart from database by orderId
        public virtual ShoppingCartModel LoadCartFromOrder(CartParameterModel model, int? catalogVersionId = null)
        {
            ShoppingCartModel cartModel;
            List<ZnodeOmsOrderLineItem> parentDetails = null;
            //Check if OrderId is null or 0.
            if (IsNull(model.OmsOrderId) || model.OmsOrderId == 0)
            {
                return null;
            }

            //Get order line items from ZnodeOmsOrderLineItem by orderId.
            IZnodeOrderHelper helper = GetService<IZnodeOrderHelper>();
            ZnodeOmsOrderDetail orderDetails = helper.GetOrderById(model.OmsOrderId.GetValueOrDefault());
            cartModel = orderDetails?.ToModel<ShoppingCartModel>() ?? new ShoppingCartModel();
            cartModel.IsOldOrder = model.IsOldOrder;
            List<ZnodeOmsOrderLineItem> allOrderLineItems = helper.GetOrderLineItemByOrderId(cartModel.OmsOrderDetailsId);
            List<ZnodeOmsOrderLineItem> orderLineItems = allOrderLineItems?
                                                     .Where(m => m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns)
                                                     && m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles)).ToList();
            List<OrderDiscountModel> allLineItemDiscountList = helper.GetOmsOrderDiscountList(model.OmsOrderId.GetValueOrDefault());


            List<PublishedProductEntityModel> productList = GetPublishProductBySKUs(orderLineItems?.Select(x => x.Sku).ToList(), model.PublishedCatalogId, model.LocaleId, catalogVersionId)?.ToModel<PublishedProductEntityModel>()?.ToList();
            cartModel.ShoppingCartItems = new List<ShoppingCartItemModel>();
            parentDetails = orderLineItems.Where(o => o.ParentOmsOrderLineItemsId == null).ToList();
            SetParentLineItemDetails(parentDetails, productList);

            foreach (ZnodeOmsOrderLineItem lineItem in orderLineItems.Where(orderLineItem => orderLineItem.ParentOmsOrderLineItemsId.HasValue))
            {
                ShoppingCartItemModel item = lineItem.ToModel<ShoppingCartItemModel>();
                PublishedProductEntityModel product = productList?.FirstOrDefault(x => x.SKU == item.SKU);
                item.PerQuantityShippingCost = GetPerQtyShippingCost(item.ShippingCost, item.Quantity);
                item.OmsOrderId = model.OmsOrderId.GetValueOrDefault();
                if (lineItem.ParentOmsOrderLineItemsId == 0)
                {
                    item.ProductType = ZnodeConstant.BundleProduct;
                }
                item.ParentOmsOrderLineItemsId = lineItem.ParentOmsOrderLineItemsId;
                item.IsActive = (product?.IsActive).GetValueOrDefault();
                item.ShipSeperately = lineItem.ShipSeparately.GetValueOrDefault();
                item.ImportDuty = (lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.ImportDuty).GetValueOrDefault();
                //to set configurable/group product quantity for cart line item
                SetConfigurableOrGroupProductQuantity(item, new List<ZnodeOmsOrderLineItem>() { lineItem }, productList);
                CalculateLineItemPrice(item, allOrderLineItems);
                SetAssociateProductType(item, allOrderLineItems);
                SetProductImage(item, model.PublishedCatalogId, model.LocaleId, model.OmsOrderId.GetValueOrDefault());
                GetLineItemEditStatus(lineItem.ZnodeOmsOrderState, item);
                GetLineItemReason(lineItem.ZnodeRmaReasonForReturn, item);
                item.TrackingNumber = lineItem.TrackingNumber;
                if (IsNotNull(lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns))
                {
                    ZnodeOmsTaxOrderLineDetail znodeOmsTaxOrderLineDetail = IsNull(lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()) && (lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group) ? orderLineItems.Where(orderLineItem => orderLineItem.OmsOrderLineItemsId == lineItem.ParentOmsOrderLineItemsId)?.ToList()?.FirstOrDefault()?.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault() : lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault();
                    item.TaxCost = IsNotNull(znodeOmsTaxOrderLineDetail) ? item.TaxCost + (znodeOmsTaxOrderLineDetail.SalesTax + znodeOmsTaxOrderLineDetail.VAT + znodeOmsTaxOrderLineDetail.GST + znodeOmsTaxOrderLineDetail.HST + znodeOmsTaxOrderLineDetail.PST).GetValueOrDefault() : 0;
                    item.TaxTransactionNumber = znodeOmsTaxOrderLineDetail?.TaxTransactionNumber;
                    item.TaxRuleId = (znodeOmsTaxOrderLineDetail?.TaxRuleId).GetValueOrDefault();
                }
                else
                {
                    item.TaxCost = item.TaxCost > 0 ? item.TaxCost : (lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.SalesTax + lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.VAT + lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.GST + lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.HST + lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.PST).GetValueOrDefault();
                    item.TaxTransactionNumber = lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.TaxTransactionNumber;
                    item.TaxRuleId = (lineItem.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.TaxRuleId).GetValueOrDefault();
                }
                SetPersonalizedAttributes(lineItem, item);
                item.DownloadableProductKey = GetProductKey(item.SKU, item.Quantity, item.OmsOrderLineItemsId);
                if (IsNotNull(item.ParentOmsQuoteLineItemId))
                {
                    item.SKU = parentDetails?.Where(o => o.ParentOmsOrderLineItemsId == item.ParentOmsQuoteLineItemId)?.Select(o => o.Sku).FirstOrDefault();
                }

                SetGroupAndConfigurableParentProductDetails(parentDetails, lineItem, item);
                orderHelper.SetPerQuantityDiscount(allLineItemDiscountList, item);
                SetInventoryData(item, product);

                cartModel.ShoppingCartItems.Add(item);
            }
            if (cartModel.OmsOrderDetailsId > 0)
            {
                SetOrderDiscount(cartModel);
                if (IsNotNull(orderDetails))
                {
                    orderDetails.DiscountAmount = cartModel.Discount;

                    if (orderDetails.DiscountAmount > 0)
                    {
                        cartModel.Discount = orderDetails.DiscountAmount.GetValueOrDefault();
                    }

                    if (!string.IsNullOrEmpty(orderDetails.CouponCode))
                    {
                        SetOrderCoupons(cartModel, orderDetails.CouponCode);
                    }
                    if (!string.IsNullOrEmpty(cartModel.GiftCardNumber))
                    {
                        SetOrderVouchers(cartModel, cartModel.GiftCardNumber);
                    }
                }


            }
            return cartModel;
        }

        public virtual void SetParentLineItemDetails(List<ZnodeOmsOrderLineItem> parentDetails, List<PublishedProductEntityModel> productList)
        {
            if (productList.Count > 0)
            {
                parentDetails?.ForEach(item =>
                {
                    string productType = productList.FirstOrDefault(x => x.SKU == item.Sku)?.Attributes?
                   .Where(x => x.AttributeCode == ZnodeConstant.ProductType)?
                   .Select(x => x.SelectValues?
                   .Select(m => m.Code)?.FirstOrDefault())?.FirstOrDefault();

                    if (productType == ZnodeConstant.BundleProduct)
                    {
                        item.ParentOmsOrderLineItemsId = 0;
                    }
                });
            }
        }

        //Set Parent Product Name and Discount for Group Product and Configurable Product
        public virtual void SetGroupAndConfigurableParentProductDetails(List<ZnodeOmsOrderLineItem> parentDetails, ZnodeOmsOrderLineItem lineItem, ShoppingCartItemModel item)
        {
            if ((item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) && item.GroupProducts?.Count > 0) || (item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable) && !string.IsNullOrEmpty(item.ConfigurableProductSKUs)))
            {
                ZnodeOmsOrderLineItem parentOrderLineItem = parentDetails?.Where(p => p.OmsOrderLineItemsId == lineItem.ParentOmsOrderLineItemsId).FirstOrDefault();
                decimal? productDiscountAmount = parentDetails?.FirstOrDefault(p => p.OmsOrderLineItemsId == lineItem.ParentOmsOrderLineItemsId)?.DiscountAmount;
                if (HelperUtility.IsNotNull(parentOrderLineItem))
                {
                    item.ProductName = parentOrderLineItem.ProductName;
                    //Set Parent SKU for group Product.
                    item.SKU = parentOrderLineItem.Sku;
                    item.PerQuantityShippingCost = GetPerQtyShippingCost(parentOrderLineItem.ShippingCost.GetValueOrDefault(), item.Quantity);
                    //Bind value to shipping cost field on child item from parent item as shipping cost value saved againt it
                    item.ShippingCost = parentOrderLineItem.ShippingCost.GetValueOrDefault();
                    item.ProductDiscountAmount = productDiscountAmount.GetValueOrDefault();
                }


            }
        }

        //Get Download product key of product
        public virtual string GetProductKey(string sku, decimal quantity, int omsOrderLineItemsId)
        {
            string productKey = string.Empty;
            IZnodeRepository<ZnodePimDownloadableProduct> _pimDownloadableProduct = new ZnodeRepository<ZnodePimDownloadableProduct>();
            bool IsDownloadableSKU = _pimDownloadableProduct.Table.Any(x => x.SKU == sku);

            if (IsDownloadableSKU && quantity > 0)
            {
                IZnodeRepository<ZnodePimDownloadableProductKey> _pimDownloadableProductKey = new ZnodeRepository<ZnodePimDownloadableProductKey>();
                IZnodeRepository<ZnodeOmsDownloadableProductKey> _omsDownloadableProductKey = new ZnodeRepository<ZnodeOmsDownloadableProductKey>();
                var productKeyDetails =
                    from omsDownloadableProductKey in _omsDownloadableProductKey.Table
                    join pimDownloadableProductKey in _pimDownloadableProductKey.Table on omsDownloadableProductKey.PimDownloadableProductKeyId equals pimDownloadableProductKey.PimDownloadableProductKeyId
                    join pimDownloadableProduct in _pimDownloadableProduct.Table on pimDownloadableProductKey.PimDownloadableProductId equals pimDownloadableProduct.PimDownloadableProductId
                    where pimDownloadableProduct.SKU == sku && pimDownloadableProductKey.IsUsed && omsDownloadableProductKey.OmsOrderLineItemsId == omsOrderLineItemsId
                    select new { keys = pimDownloadableProductKey.DownloadableProductKey }.keys;

                productKey = string.Join(",", productKeyDetails);
            }

            return productKey;
        }

        public virtual string GetProductKey(string sku, decimal quantity, int omsOrderLineItemsId, List<ZnodePimDownloadableProduct> lstDownloadableProducts)
        {
            string productKey = string.Empty;
            bool IsDownloadableSKU = lstDownloadableProducts.Any(x => x.SKU == sku);

            if (IsDownloadableSKU && quantity > 0)
            {
                IZnodeRepository<ZnodePimDownloadableProductKey> _pimDownloadableProductKey = new ZnodeRepository<ZnodePimDownloadableProductKey>();
                IZnodeRepository<ZnodeOmsDownloadableProductKey> _omsDownloadableProductKey = new ZnodeRepository<ZnodeOmsDownloadableProductKey>();
                IZnodeRepository<ZnodePimDownloadableProduct> _pimDownloadableProduct = new ZnodeRepository<ZnodePimDownloadableProduct>();

                var productKeyDetails =
                    from omsDownloadableProductKey in _omsDownloadableProductKey.Table
                    join pimDownloadableProductKey in _pimDownloadableProductKey.Table on omsDownloadableProductKey.PimDownloadableProductKeyId equals pimDownloadableProductKey.PimDownloadableProductKeyId
                    join pimDownloadableProduct in _pimDownloadableProduct.Table on pimDownloadableProductKey.PimDownloadableProductId equals pimDownloadableProduct.PimDownloadableProductId
                    where pimDownloadableProduct.SKU == sku && pimDownloadableProductKey.IsUsed && omsDownloadableProductKey.OmsOrderLineItemsId == omsOrderLineItemsId
                    select new { keys = pimDownloadableProductKey.DownloadableProductKey }.keys;

                productKey = string.Join(",", productKeyDetails);
            }

            return productKey;
        }

        public virtual void SetPersonalizedAttributes(ZnodeOmsOrderLineItem orderLineItem, ShoppingCartItemModel cartItem)
        {
            cartItem.PersonaliseValuesList = new Dictionary<string, object>();
            if (IsNotNull(orderLineItem))
            {
                if (IsNotNull(orderLineItem.ZnodeOmsPersonalizeItems) && orderLineItem.ZnodeOmsPersonalizeItems.Count > 0)
                {
                    foreach (var item in orderLineItem.ZnodeOmsPersonalizeItems)
                    {
                        cartItem.PersonaliseValuesList.Add(item.PersonalizeCode, item.PersonalizeValue);
                    }
                }
                else
                {
                    if (orderLineItem.OmsOrderDetailsId > 0)
                    {
                        Dictionary<string, object> personaliseValuesList = orderHelper.GetPersonalizedValueOrderLineItem(Convert.ToInt32(orderLineItem.ParentOmsOrderLineItemsId), false, 0);
                        foreach (var item in personaliseValuesList)
                        {
                            cartItem.PersonaliseValuesList.Add(item.Key, item.Value);
                        }
                    }
                }
            }
        }

        //to add saved cart line item to shopping Cart
        public virtual void AddToShoppingCart(AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems, CartParameterModel cartParameterModel)
        {
            if (string.IsNullOrEmpty(cartLineItemModel.SKU))
            {
                return;
            }

            string parentSKUProductName = string.Empty;

            var configurableLineItem = new List<AccountQuoteLineItemModel>();
            configurableLineItem.Add(cartLineItemModel);

            List<AccountQuoteLineItemModel> shoppingCartLineItems = cartLineItemModel?.OmsQuoteId > 0 ? (
                   cartLineItemModel.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) ?
                   cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId)?.ToList() :
                   cartLineItems.Where(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId)?.ToList()) :
                   cartLineItemModel.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) ?
                   cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId)?.ToList() :
                   cartLineItems.Where(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId)?.ToList();


            this.PortalId = cartParameterModel.PortalId > 0 ? cartParameterModel.PortalId : GetHeaderPortalId();

            List<AccountQuoteLineItemModel> bundleLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.Bundles);
            List<AccountQuoteLineItemModel> configurableLineItems = BindProductType(configurableLineItem, ZnodeCartItemRelationshipTypeEnum.Configurable);
            List<AccountQuoteLineItemModel> groupLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.Group);
            List<AccountQuoteLineItemModel> addonLineItems = new List<AccountQuoteLineItemModel>();

            if (groupLineItems?.Count > 0)
            {
                foreach (AccountQuoteLineItemModel item in cartLineItems)
                {
                    if (item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns) && (item.OmsQuoteId > 0 ? groupLineItems.Any(y => y.OmsQuoteLineItemId == item.ParentOmsQuoteLineItemId) : groupLineItems.Any(y => y.OmsSavedCartLineItemId == item.ParentOmsSavedCartLineItemId)))
                    {
                        addonLineItems.Add(item);
                    }
                }
                parentSKUProductName = groupLineItems?.Count > 0 ? cartLineItemModel?.ProductName : string.Empty;
            }
            else
            {
                addonLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.AddOns);
            }

            List<AssociatedProductModel> addOnProducts = new List<AssociatedProductModel>();

            if (addonLineItems?.Count > 0)
                addonLineItems.ForEach(doc => addOnProducts.Add(new AssociatedProductModel { Sku = doc.SKU, Quantity = doc.Quantity }));

            ZnodeShoppingCartItem cartLineItem = new ZnodeShoppingCartItem(null);
            cartLineItem.OmsOrderId = cartLineItemModel.OmsOrderId;
            cartLineItem.OmsSavedCartLineItemId = cartLineItemModel.OmsSavedCartLineItemId;
            cartLineItem.ParentOmsSavedCartLineItemId = cartLineItemModel.ParentOmsSavedCartLineItemId;
            cartLineItem.CustomText = cartLineItemModel.CustomText;

            string parentSKu = cartParameterModel.OmsQuoteId > 0 ? GetQuoteParentSKU(cartLineItemModel, cartLineItems) : GetParentSKU(cartLineItemModel, cartLineItems);
            if (cartParameterModel.OmsQuoteId > 0)
            {
                //Map AccountQuoteLineItemModel to AccountQuoteLineItemModel.
                ToZNodeShoppingCartItem(cartLineItemModel, cartLineItem, parentSKu);
            }

            List<AssociatedProductModel> groupProduct = new List<AssociatedProductModel>();

            if (groupLineItems?.Count > 0)
            {
                groupLineItems.ForEach(doc => groupProduct.Add(new AssociatedProductModel { Sku = doc.SKU, Quantity = doc.Quantity, OmsSavedCartLineItemId = doc.OmsSavedCartLineItemId, ProductName = doc.ProductName }));
            }

            //Get cartitem having configurable product sku.
            AccountQuoteLineItemModel cartItem = cartLineItem.OmsQuoteId > 0 ? cartLineItems.FirstOrDefault(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId && x.OrderLineItemRelationshipTypeId == 3)
                                 : cartLineItems.FirstOrDefault(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId && x.OrderLineItemRelationshipTypeId == 3);

            if (IsNotNull(cartItem))
            {
                cartLineItemModel = cartItem;
            }

            BindProductDetails(cartLineItem, new PublishProductModel
            {
                SKU = cartLineItemModel.SKU,
                ParentPublishProductId = GetParentProductId(parentSKu, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId, cartParameterModel.OmsOrderId.GetValueOrDefault()),
                Quantity = cartLineItemModel.Quantity,
                LocaleId = cartParameterModel.LocaleId,
                PublishedCatalogId = cartParameterModel.PublishedCatalogId,
                AddonProductSKUs = string.Join(",", addonLineItems.Select(b => b.SKU)),
                AssociatedAddOnProducts = addOnProducts,
                BundleProductSKUs = string.Join(",", bundleLineItems.Select(b => b.SKU)),
                ConfigurableProductSKUs = string.Join(",", configurableLineItems.Select(b => b.SKU)),
                GroupProductSKUs = groupProduct,
            }, parentSKu, cartParameterModel.UserId.GetValueOrDefault(), 0, null, parentSKUProductName, cartParameterModel.ProfileId);


            if (cartLineItemModel.OmsSavedCartLineItemId.Equals(0))
            {
                cartLineItem.PersonaliseValuesDetail = GetService<IZnodeOrderHelper>()?.GetPersonalizedValueCartLineItem(
                    (cartLineItemModel.ParentOmsQuoteLineItemId.GetValueOrDefault() > 0) ? cartLineItemModel.ParentOmsQuoteLineItemId.GetValueOrDefault() : cartLineItemModel.OmsQuoteLineItemId);

                cartLineItem.GroupId = (cartLineItemModel.ParentOmsQuoteLineItemId.GetValueOrDefault() > 0)
                  ? cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId).Select(x => x.GroupId).FirstOrDefault()
                  : cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId).Select(x => x.GroupId).FirstOrDefault();
            }
            else
            {
                cartLineItem.PersonaliseValuesDetail = GetService<IZnodeOrderHelper>()?.GetPersonalizedValueCartLineItem(
                     (cartLineItemModel.ParentOmsSavedCartLineItemId > 0) ? cartLineItemModel.ParentOmsSavedCartLineItemId : cartLineItemModel.OmsSavedCartLineItemId);

                cartLineItem.GroupId = (cartLineItemModel.ParentOmsSavedCartLineItemId > 0)
                    ? cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId).Select(x => x.GroupId).FirstOrDefault()
                    : cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId).Select(x => x.GroupId).FirstOrDefault();
            }

            cartLineItem.OrderLineItemRelationshipTypeId = cartLineItemModel.OrderLineItemRelationshipTypeId;

            if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon))
            {
                cartLineItem.AutoAddonSKUs = cartLineItemModel.AutoAddon;
            }

            BindCustomData(cartLineItemModel, cartLineItem);

            if (IsNotNull(cartLineItem.Product))
            {
                base.ShoppingCartItems.Add(cartLineItem);
            }
        }

        //to add item to ZnodeShoppingCart from api model
        public virtual void AddtoShoppingCart(ShoppingCartItemModel model, AddressModel shippingAddress, int localeId, int publishedCatalogId, int userId, int omsOrderId)
        {
            if (string.IsNullOrEmpty(model.SKU))
            {
                return;
            }

            this.PortalId = IsNotNull(this.PortalId) ? this.PortalId : GetHeaderPortalId();

            ZnodeShoppingCartItem znodeCartItem = new ZnodeShoppingCartItem(shippingAddress)
            {
                ExternalId = model.ExternalId,
                Quantity = model.Quantity,
                ShippingCost = model.ShippingCost,
                ShippingOptionId = model.ShippingOptionId,
                ParentProductId = model.ParentProductId,
                InsufficientQuantity = model.InsufficientQuantity,
                CustomUnitPrice = model.CustomUnitPrice,
                PartialRefundAmount = model.PartialRefundAmount,
                OrderStatusId = model.OmsOrderStatusId,
                OrderStatus = model.OrderLineItemStatus,
                TrackingNumber = model.TrackingNumber,
                IsEditStatus = model.IsEditStatus,
                IsActive = model.IsActive,
                IsItemStateChanged = model.IsItemStateChanged,
                IsSendEmail = model.IsSendEmail,
                OmsOrderId = model.OmsOrderId,
                OmsOrderLineItemId = model.OmsOrderLineItemsId,
                AutoAddonSKUs = model.AutoAddonSKUs,
                Custom1 = model.Custom1,
                Custom2 = model.Custom2,
                Custom3 = model.Custom3,
                Custom4 = model.Custom4,
                Custom5 = model.Custom5,
                ShipSeperately = model.ShipSeperately,
                CustomText = model.CustomText,
                OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault(),
                GroupId = model.GroupId,
                AdditionalCost = model.AdditionalCost,
                AssociatedAddOnProducts = model.AssociatedAddOnProducts,
                ParentOmsOrderLineItemsId = model.ParentOmsOrderLineItemsId,
                ProductLevelTax = model.ProductLevelTax
            };

            //If Quote Id is greater than zero, bind ShoppingCartItemModel properties to ZNodeShoppingCartItem.
            if (model.OmsQuoteId > 0)
            {
                BindShoppingCartItemModel(model, znodeCartItem, string.Empty);
            }

            int addressId = 0;

            // Cart level shipping address
            if (IsNotNull(shippingAddress))
            {
                addressId = shippingAddress.AddressId;
            }

            if (IsNotNull(model.MultipleShipToAddress) && model.MultipleShipToAddress.Any())
            {
                foreach (OrderShipmentModel shipToAddress in model.MultipleShipToAddress)
                {
                    if (shipToAddress.AddressId.Equals(0))
                    {
                        shipToAddress.AddressId = addressId;
                    }

                    ZnodeOrderShipment znodeOrderShipment = new ZnodeOrderShipment(shipToAddress.AddressId, shipToAddress.Quantity, znodeCartItem.GUID, shipToAddress.ShippingOptionId.GetValueOrDefault(0), shipToAddress.ShippingName);
                    znodeCartItem.OrderShipments.Add(znodeOrderShipment);
                }
            }
            else
            {
                // Cart item level shipping address
                if (IsNotNull(model.ShippingAddress))
                {
                    addressId = model.ShippingAddress.AddressId;
                }

                ZnodeOrderShipment orderShipment = new ZnodeOrderShipment(addressId, model.Quantity, znodeCartItem.GUID);

                znodeCartItem.OrderShipments.Add(orderShipment);
            }
            string parentSKU = model?.GroupProducts?.Count > 0 || !string.IsNullOrEmpty(model.AutoAddonSKUs) ? string.Empty : model.SKU;
            BindProductDetails(znodeCartItem, new PublishProductModel
            {
                SKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : model.SKU,
                Quantity = model.Quantity,
                LocaleId = localeId,
                ParentPublishProductId = model.ParentProductId > 0 && (!string.IsNullOrEmpty(model.ConfigurableProductSKUs) || !string.IsNullOrEmpty(model.AutoAddonSKUs)) ? model.ParentProductId : GetParentProductId(model.SKU, publishedCatalogId, localeId, omsOrderId),
                PublishedCatalogId = publishedCatalogId,
                AddonProductSKUs = !string.IsNullOrEmpty(model.AddOnProductSKUs) ? model.AddOnProductSKUs : string.Empty,
                BundleProductSKUs = !string.IsNullOrEmpty(model.BundleProductSKUs) ? model.BundleProductSKUs : string.Empty,
                ConfigurableProductSKUs = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : string.Empty,
                GroupProductSKUs = model?.GroupProducts?.Count > 0 ? model.GroupProducts : new List<AssociatedProductModel>(),
                AssociatedAddOnProducts = model?.AssociatedAddOnProducts?.Count > 0 ? model.AssociatedAddOnProducts : new List<AssociatedProductModel>()
            }, parentSKU, userId, omsOrderId, model.CustomUnitPrice);
            znodeCartItem.PersonaliseValuesList = model.PersonaliseValuesList;
            znodeCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();
            znodeCartItem.PersonaliseValuesDetail = model.PersonaliseValuesDetail;
            base.ShoppingCartItems.Add(znodeCartItem);
        }


        //to add item to ZnodeShoppingCart from api model
        public virtual void AddtoShoppingBag(ShoppingCartModel shoppingCartItems)
        {
            List<ZnodeShoppingCartItem> shoppingCartItemList = new List<ZnodeShoppingCartItem>();

            IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemRepository = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
            List<int?> lstParentIds = shoppingCartItems.ShoppingCartItems.Select(x => x.ParentOmsSavedcartLineItemId).ToList();
            List<ZnodeOmsSavedCartLineItem> lstCartDetails = _savedCartLineItemRepository.Table.Where(x => lstParentIds.Contains(x.OmsSavedCartLineItemId)).ToList();

            foreach (ShoppingCartItemModel model in shoppingCartItems.ShoppingCartItems.OrderBy(c => c.GroupSequence))
            {
                if (string.IsNullOrEmpty(model.SKU))
                {
                    return;
                }

                ZnodeShoppingCartItem znodeCartItem = MapShoppingCartItemModel(model, model.ShippingAddress);

                MapShoppingCartOtherData(model, model.ShippingAddress, znodeCartItem);

                ZnodeOmsSavedCartLineItem cartDetails = lstCartDetails?.FirstOrDefault(x => x.OmsSavedCartLineItemId == model.ParentOmsSavedcartLineItemId);
                string parentSKU = model?.GroupProducts?.Count > 0 ? cartDetails?.SKU : model.SKU;
                string parentSKUProductName = model?.GroupProducts?.Count > 0 ? cartDetails?.ProductName : string.Empty;
                if (string.IsNullOrEmpty(parentSKUProductName))
                {
                    parentSKUProductName = model?.ProductName;
                }

                //If Quote Id is greater than zero, bind ShoppingCartItemModel properties to ZNodeShoppingCartItem.
                if (model.OmsQuoteId > 0)
                {
                    BindShoppingCartItemModel(model, znodeCartItem, parentSKU);
                }

                BindProductDetails(znodeCartItem, GetPublishProductModel(model, shoppingCartItems.LocaleId, shoppingCartItems.PublishedCatalogId, shoppingCartItems.OmsOrderId.GetValueOrDefault()), parentSKU, shoppingCartItems.UserId.GetValueOrDefault(), shoppingCartItems.OmsOrderId.GetValueOrDefault(), model.CustomUnitPrice, parentSKUProductName, shoppingCartItems.ProfileId.GetValueOrDefault());

                znodeCartItem.PersonaliseValuesDetail = model.PersonaliseValuesDetail;
                znodeCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();

                shoppingCartItemList.Add(znodeCartItem);
            }

            //List of child product sku available in cart
            List<string> cartProductActualSkus = shoppingCartItemList.Where(x => x.Product != null)
                                                                     .Select(x => x.Product?.Attributes
                                                                                           ?.FirstOrDefault(o => o.AttributeCode.Equals(ZnodeConstant.ProductSKU, StringComparison.CurrentCultureIgnoreCase))
                                                                                           ?.AttributeValue).ToList();
            if (IsNotNull(cartProductActualSkus))
            {
                //Validate inventory using mapped product SKU
                List<InventorySKUModel> inventory = publishProductHelper.GetInventoryBySKUs(cartProductActualSkus,
                    shoppingCartItems.PortalId);

                //Update quantity on hand of the cart item
                shoppingCartItemList.Where(cartItem => cartItem.Product != null && cartItem.Product
                                                                                           ?.Attributes
                                                                                           ?.Any(o => o.AttributeCode.Equals(ZnodeConstant.ProductSKU, StringComparison.CurrentCultureIgnoreCase)) == true)
                                                                                           ?.ToList()
                .ForEach(znodeCartItem =>
                {
                    string productSku = znodeCartItem.Product.Attributes
                                                             .FirstOrDefault(o => o.AttributeCode.Equals(ZnodeConstant.ProductSKU, StringComparison.CurrentCultureIgnoreCase))
                                                             .AttributeValue;
                    znodeCartItem.Product.QuantityOnHand = inventory.Any(sku => sku.SKU == productSku) ? inventory.FirstOrDefault(sku => sku.SKU == productSku).Quantity : 0;
                    base.ShoppingCartItems.Add(znodeCartItem);
                });
            }
        }

        //to bind custom data from shopping cart item to ZNodeShoppingCartItem
        public virtual void BindCustomData(AccountQuoteLineItemModel model, ZnodeShoppingCartItem cartItem)
        {
            cartItem.Custom1 = model.Custom1;
            cartItem.Custom2 = model.Custom2;
            cartItem.Custom3 = model.Custom3;
            cartItem.Custom4 = model.Custom4;
            cartItem.Custom5 = model.Custom5;
        }

        //to load shoppingCart from database by QuoteId
        public virtual ShoppingCartModel LoadCartFromQuote(CartParameterModel model, int? catalogVersionId = null)
        {
            ShoppingCartModel cartModel;
            List<ZnodeOmsQuoteLineItem> parentDetails = null;
            //Check if QuoteId is null or 0.
            if (IsNull(model.OmsQuoteId) || model.OmsQuoteId == 0)
            {
                return null;
            }

            //Get quote line items from ZnodeOmsOrderLineItem by QuoteId.
            ZnodeOmsQuote quote = quotehelper.GetQuoteById(model.OmsQuoteId);

            cartModel = quote?.ToModel<ShoppingCartModel>() ?? new ShoppingCartModel();

            List<ZnodeOmsQuoteLineItem> allquoteLineItems = quotehelper.GetQuoteLineItemByQuoteId(cartModel.OmsQuoteId);
            List<ZnodeOmsQuoteLineItem> quoteLineItems = allquoteLineItems?
                                                     .Where(m => m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns)
                                                     && m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles)).ToList();

            List<ZnodeOmsQuoteLineItem> bundlesQuoteLineItems = allquoteLineItems?
                                                     .Where(m => m.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles)).ToList();

            List<string> skus = quoteLineItems?.Select(x => x.SKU).ToList();
            if (bundlesQuoteLineItems?.Count > 0)
            {
                bundlesQuoteLineItems.ForEach(bundlesQuoteLineItem => {
                    skus?.AddRange(bundlesQuoteLineItem.SKU?.Split(','));
                });
            }


            List<PublishedProductEntityModel> productList = GetPublishProductBySKUs(skus, model.PublishedCatalogId, model.LocaleId, catalogVersionId)?.ToModel<PublishedProductEntityModel>()?.ToList();

            cartModel.ShoppingCartItems = new List<ShoppingCartItemModel>();
            parentDetails = quoteLineItems.Where(o => o.ParentOmsQuoteLineItemId == null).ToList();
            SetParentLineItemDetailsForQuote(parentDetails, productList);

            //Get the Quote Line Item ids, to avoid loop based calls.
            List<int?> lstCartLineIds = GetQuoteCartLineItemIds(quoteLineItems);
            List<PersonaliseValueModel> lastPersonalizedValues = GetService<IZnodeQuoteHelper>()?.GetPersonalisedValueCartLineItem(lstCartLineIds);
            foreach (ZnodeOmsQuoteLineItem lineItem in quoteLineItems.Where(quoteLineItem => quoteLineItem.ParentOmsQuoteLineItemId.HasValue))
            {
                ShoppingCartItemModel item = lineItem.ToModel<ShoppingCartItemModel>();
                if (lineItem.ParentOmsQuoteLineItemId == 0)
                {
                    item.ProductType = ZnodeConstant.BundleProduct;
                }
                if (item.ProductType != ZnodeConstant.BundleProduct)
                {
                    PublishedProductEntityModel product = productList?.FirstOrDefault(x => x.SKU == item.SKU);

                    item.IsActive = (product?.IsActive).GetValueOrDefault();

                    //to set configurable/group product quantity for cart line item
                    SetConfigurableOrGroupProductQuantityForQuote(item, new List<ZnodeOmsQuoteLineItem>() { lineItem }, productList);
                    CalculateLineItemPriceForQuote(item, allquoteLineItems);
                    SetAssociateQuoteProductType(item, allquoteLineItems);
                    SetProductImage(item, model.PublishedCatalogId, model.LocaleId, model.OmsOrderId.GetValueOrDefault());

                    SetGroupAndConfigurableParentProductDetailsOfQuote(parentDetails, lineItem, item);

                    SetInventoryData(item, product);

                    //get personalize attributes by OmsQuoteLineItemId
                    if (lastPersonalizedValues.Count > 0)
                    {
                        List<PersonaliseValueModel> childLinePersonalizedValues = lastPersonalizedValues.Where(x => x.OmsSavedCartLineItemId == item.OmsQuoteLineItemId).ToList();
                        item.PersonaliseValuesDetail = childLinePersonalizedValues?.Count > 0 ? childLinePersonalizedValues : lastPersonalizedValues.Where(x => x.OmsSavedCartLineItemId == item.ParentOmsQuoteLineItemId).ToList();
                    }
                }
                else
                {
                    ZnodeOmsQuoteLineItem childLine = allquoteLineItems.FirstOrDefault(x => x.ParentOmsQuoteLineItemId == item.OmsQuoteLineItemId);
                    IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
                    item.BundleProducts = znodeShoppingCarts.BindBundleProductChildByParentSku(item.SKU, model.PublishedCatalogId, model.LocaleId);
                    SetBundleProductQuantityForQuote(item, childLine, productList);
                    CalculateBundleLineItemPriceForQuote(item, allquoteLineItems);
                    SetAssociateQuoteProductType(item, allquoteLineItems);
                    SetProductImage(item, model.PublishedCatalogId, model.LocaleId, model.OmsOrderId.GetValueOrDefault());

                }
                cartModel.ShoppingCartItems.Add(item);
            }
            return cartModel;
        }

        public virtual void SetParentLineItemDetailsForQuote(List<ZnodeOmsQuoteLineItem> parentDetails, List<PublishedProductEntityModel> productList)
        {
            if (productList.Count > 0)
            {
                parentDetails?.ForEach(item =>
                {
                    string productType = productList.FirstOrDefault(x => x.SKU == item.SKU)?.Attributes?
                   .Where(x => x.AttributeCode == ZnodeConstant.ProductType)?
                   .Select(x => x.SelectValues?
                   .Select(m => m.Code)?.FirstOrDefault())?.FirstOrDefault();

                    if (productType == ZnodeConstant.BundleProduct)
                    {
                        item.ParentOmsQuoteLineItemId = 0;
                    }
                });
            }
        }

        //Set product image attribute to Publish Product Model if already not set
        protected virtual void SetProductImageAttribute(PublishedProductEntityModel productInfo, ShoppingCartItemModel cartModel)
        {
            PublishProductModel product = cartModel.Product ?? new PublishProductModel();
            string productImage = productInfo?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, ZnodeConstant.ProductImage, StringComparison.InvariantCultureIgnoreCase))?.AttributeValues;
            if (!string.IsNullOrEmpty(productImage))
            {
                product.Attributes = product.Attributes ?? new List<PublishAttributeModel>();
                PublishAttributeModel imageAttribute = product.Attributes.FirstOrDefault(item => string.Equals(item.AttributeCode, ZnodeConstant.ProductImage, StringComparison.InvariantCultureIgnoreCase));
                if (IsNull(imageAttribute))
                {
                    product.Attributes.Add(new PublishAttributeModel() { AttributeCode = ZnodeConstant.ProductImage, AttributeName = ZnodeConstant.ProductImage, AttributeValues = productImage });
                }
            }
            cartModel.Product = product;
        }
        #endregion        

        #region Private Methods

        //Set Custom Tier Price.
        protected virtual void SetCustomTierPrice(ZnodeShoppingCartItem cartItem, AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems)
        {
            decimal totalQuantity = cartLineItemModel.OmsQuoteId > 0 ? cartLineItems.Where(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId).Sum(x => x.Quantity) : cartLineItems.Where(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId).Sum(x => x.Quantity);
            if (HelperUtility.IsNotNull(cartItem?.Product?.ZNodeTieredPriceCollection))
            {
                // var dd = cartItem.Product.ZNodeTieredPriceCollection.               
                decimal? finalPrice = null;
                foreach (ZnodeProductTierEntity productTieredPrice in cartItem.Product.ZNodeTieredPriceCollection)
                {
                    //check if tier quantity is valid or not.
                    if (totalQuantity >= productTieredPrice.MinQuantity && totalQuantity < productTieredPrice.MaxQuantity)
                    {
                        finalPrice = productTieredPrice.Price;
                        break;
                    }
                }
                cartItem.Product.CustomPrice = (cartItem.OmsQuoteId > 0 && cartItem.CustomUnitPrice > 0) ? cartItem.CustomUnitPrice : finalPrice;
            }
        }

        protected virtual string GetQuoteParentSKU(AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems)
        {
            if (cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable))?.Count() > 0)
            {
                return cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)).Select(x => x.SKU).FirstOrDefault();
            }
            else if (cartLineItems.Where(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId && (x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))?.Count() > 0)
            {
                return cartLineItemModel.SKU;
            }
            else if (cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))?.Count() > 0)
            {
                return cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)).Select(x => x.SKU).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && cartLineItemModel.Quantity < 1)
            {
                return cartLineItemModel.SKU;
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && cartLineItemModel.AutoAddon.Contains(","))
            {
                return cartLineItemModel.SKU;
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && Equals(cartLineItemModel.AutoAddon, cartLineItemModel.SKU))
            {
                return cartLineItems.FirstOrDefault(x => !string.IsNullOrEmpty(x.AutoAddon) && x.AutoAddon.Contains(cartLineItemModel.SKU) && !Equals(x.SKU, cartLineItemModel.AutoAddon)).SKU;
            }

            return string.Empty;
        }

        protected virtual string GetParentSKU(AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems)
        {
            if (cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable))?.Count() > 0)
            {
                return cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)).Select(x => x.SKU).FirstOrDefault();
            }
            else if (cartLineItems.Where(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId && (x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))?.Count() > 0)
            {
                return cartLineItemModel.SKU;
            }
            else if (cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))?.Count() > 0)
            {
                return cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId && (cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || cartLineItemModel.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)).Select(x => x.SKU).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && cartLineItemModel.Quantity < 1)
            {
                return cartLineItemModel.SKU;
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && cartLineItemModel.AutoAddon.Contains(","))
            {
                return cartLineItemModel.SKU;
            }
            else if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon) && Equals(cartLineItemModel.AutoAddon, cartLineItemModel.SKU))
            {
                return cartLineItems.FirstOrDefault(x => !string.IsNullOrEmpty(x.AutoAddon) && x.AutoAddon.Contains(cartLineItemModel.SKU) && !Equals(x.SKU, cartLineItemModel.AutoAddon)).SKU;
            }

            return string.Empty;

        }

        protected virtual void GetLineItemReason(ZnodeRmaReasonForReturn rmaReasonForReturn, ShoppingCartItemModel cartLineItem)
        {
            if (IsNotNull(rmaReasonForReturn) && IsNotNull(cartLineItem))
            {
                cartLineItem.RmaReasonForReturnId = rmaReasonForReturn.RmaReasonForReturnId;
                cartLineItem.RmaReasonForReturn = rmaReasonForReturn.Name;
            }
        }

        protected virtual void GetLineItemEditStatus(ZnodeOmsOrderState omsOrderState, ShoppingCartItemModel cartLineItem)
        {
            if (IsNotNull(omsOrderState) && IsNotNull(cartLineItem))
            {
                cartLineItem.OrderLineItemStatus = omsOrderState.Description;
                cartLineItem.IsEditStatus = omsOrderState.IsEdit;
                cartLineItem.IsSendEmail = omsOrderState.IsSendEmail;
                cartLineItem.OmsOrderStatusId = omsOrderState.OmsOrderStateId;
            }
        }

        // Calculates total of all additional cost associated with each cartline item if any
        public virtual decimal GetAdditionalPrice()
        {
            decimal additionalPrice = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Where(x => IsNotNull(x.AdditionalCost)))
            {
                additionalPrice = additionalPrice + cartItem.AdditionalCost.Sum(x => x.Value) * cartItem.Quantity;
            }


            return additionalPrice;

        }

        // To set order discount amount
        public virtual void SetOrderDiscount(ShoppingCartModel cartModel)
        {
            List<OrderDiscountModel> discountList = orderHelper.GetOrderDiscountAmount(cartModel.OmsOrderDetailsId);

            if (discountList?.Count > 0)
            {
                OrderDiscountModel CSRDiscountModel = discountList.FirstOrDefault(x => x.DiscountCode == OrderDiscountTypeEnum.CSRDISCOUNT.ToString() && x.OmsOrderLineItemId == null);
                // decimal csrDiscount = (discountList.FirstOrDefault(x => x.DiscountType == OrderDiscountTypeEnum.CSRDISCOUNT.ToString())?.DiscountAmount).GetValueOrDefault();
                if (HelperUtility.IsNotNull(CSRDiscountModel))
                {
                    cartModel.CSRDiscountAmount = CSRDiscountModel.DiscountAmount.GetValueOrDefault();
                    cartModel.Discount = (cartModel.Discount - cartModel.CSRDiscountAmount);
                }

                decimal giftCardDiscount = (discountList.FirstOrDefault(x => x.DiscountType == OrderDiscountTypeEnum.GIFTCARD.ToString())?.DiscountAmount).GetValueOrDefault();
                
                cartModel.GiftCardAmount = giftCardDiscount;
                cartModel.GiftCardNumber = orderHelper.GetDiscountCode(cartModel.OmsOrderDetailsId, OrderDiscountTypeEnum.GIFTCARD);
                cartModel.GiftCardApplied = true;                
            }
        }

        //Get Saved Cart Line Items.
        protected virtual List<AccountQuoteLineItemModel> GetSavedCartLineItems(CartParameterModel cartParameterModel)
        {
            int cookieMappingId = cartParameterModel.CookieId.GetValueOrDefault();
            //Get saved cart Id on the basis of cookieMappingId.
            int savedCartId = orderHelper.GetSavedCartId(ref cookieMappingId, cartParameterModel.PortalId, cartParameterModel.UserId);

            //If the new cookie Mapping Id gets generated, then it should assign back within the requested model.
            cartParameterModel.CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString());
            cartParameterModel.CookieId = cookieMappingId;

            return orderHelper.GetSavedCartLineItem(savedCartId, cartParameterModel.OmsOrderId.GetValueOrDefault())?.ToModel<AccountQuoteLineItemModel>()?.ToList();
        }

        //Get Account Quote Line Items.
        protected virtual List<AccountQuoteLineItemModel> GetAccountQuoteLineItems(CartParameterModel cartParameterModel)
        {
            IZnodeRepository<ZnodeOmsQuoteLineItem> _omsQuoteLineItemRepository = new ZnodeRepository<ZnodeOmsQuoteLineItem>();
            return _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteId == cartParameterModel.OmsQuoteId)?.ToModel<AccountQuoteLineItemModel>()?.ToList();
        }

        // Get product details  
        protected virtual void BindProductDetails(ZnodeShoppingCartItem znodeCartItem, PublishProductModel productModel, string parentSKu = null, int userId = 0, int omsOrderId = 0, decimal? unitPrice = null, string parentSKUProductName = null, int profileId = 0)
        {
            int catalogVersionId = GetCatalogVersionId(productModel.PublishedCatalogId, productModel.LocaleId);
            PublishProductModel product = GetPublishProductBySKU(productModel.SKU, productModel.PublishedCatalogId, productModel.LocaleId, catalogVersionId, omsOrderId)?.ToModel<PublishProductModel>();
            if (IsNotNull(product) && IsNotNull(znodeCartItem))
            {
                bool isGroupProduct = productModel.GroupProductSKUs.Count > 0;
                string countryCode = znodeCartItem.ShippingAddress?.CountryName;
                PublishProductModel publishProduct = product;
                publishProduct.GroupProductSKUs = productModel.GroupProductSKUs;
                publishProduct.ConfigurableProductId = productModel.ParentPublishProductId;
                ZnodeProduct baseProduct = GetProductDetails(publishProduct, this.PortalId.GetValueOrDefault(), productModel.LocaleId, catalogVersionId, znodeCartItem.ShippingAddress?.CountryName, isGroupProduct, parentSKu, userId, omsOrderId, parentSKUProductName, profileId);
                znodeCartItem.ProductCode = product.Attributes.Where(x => x.AttributeCode == ZnodeConstant.ProductCode)?.FirstOrDefault()?.AttributeValues;
                znodeCartItem.ProductType = product.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code;
                znodeCartItem.Product = new ZnodeProductBase(baseProduct, znodeCartItem.ShippingAddress, unitPrice);
                znodeCartItem.Product.ZNodeAddonsProductCollection = GetZnodeProductAddons(productModel, productModel.PublishedCatalogId, productModel.LocaleId, baseProduct.AddOns, countryCode, userId, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Product.ZNodeBundleProductCollection = GetZnodeProductBundles(productModel, productModel.PublishedCatalogId, productModel.LocaleId, countryCode, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Product.ZNodeConfigurableProductCollection = GetZnodeProductConfigurables(productModel.ConfigurableProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, countryCode, productModel.ParentPublishProductId, userId, omsOrderId, profileId, productModel.Quantity.GetValueOrDefault(), catalogVersionId);
                znodeCartItem.Product.ZNodeGroupProductCollection = GetZnodeProductGroup(productModel.GroupProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, countryCode, userId, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Quantity = GetProductQuantity(znodeCartItem, productModel.Quantity.GetValueOrDefault());
                znodeCartItem.ParentProductId = productModel.ParentPublishProductId;
                znodeCartItem.UOM = baseProduct.UOM;
                znodeCartItem.ParentProductSKU = znodeCartItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)
                                       ? znodeCartItem.ParentProductSKU : product.SKU;
                znodeCartItem.Product.SKU = !string.IsNullOrEmpty(parentSKu) && (!string.IsNullOrEmpty(productModel.ConfigurableProductSKUs) || isGroupProduct) ? parentSKu : product.SKU;
                znodeCartItem.Image = znodeCartItem.Product.ZNodeGroupProductCollection?.Count > 0 ? znodeCartItem.Product.ZNodeGroupProductCollection[0].Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValue : product.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                znodeCartItem.Product.Container = GetAttributeValueByCode(znodeCartItem, product, ZnodeConstant.ShippingContainer);
                znodeCartItem.Product.Size = GetAttributeValueByCode(znodeCartItem, product, ZnodeConstant.ShippingSize);
                znodeCartItem.Product.PackagingType = product.Attributes.Where(x => x.AttributeCode == ZnodeConstant.PackagingType)?.FirstOrDefault()?.SelectValues[0]?.Value;
                znodeCartItem.Product.DownloadableProductKey = GetProductKey(znodeCartItem.Product.SKU, znodeCartItem.Quantity, znodeCartItem.OmsOrderLineItemId);
                znodeCartItem.AssociatedAddOnProducts = productModel.AssociatedAddOnProducts;
                SetInventoryData(znodeCartItem.Product);
            }
        }


        protected virtual void BindCartProductDetails(ShoppingCartItemModel cartModel, int publishCatalogId, int localeId, string groupIdProductAttribute = "", GlobalSettingValues groupIdPersonalizeAttribute = null)
        {
            int catalogVersionId = GetCatalogVersionId(publishCatalogId);
            PublishedProductEntityModel product = GetPublishProductBySKU(string.IsNullOrEmpty(cartModel.ConfigurableProductSKUs) ? cartModel.SKU : cartModel.ConfigurableProductSKUs, publishCatalogId, localeId, catalogVersionId)?.ToModel<PublishedProductEntityModel>();
            if (IsNotNull(product))
            {
                SetProductImageAttribute(product, cartModel);
                cartModel.Description = GetShortDescription(product, cartModel, catalogVersionId);
                cartModel.ProductName = cartModel.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) ? cartModel.ProductName : product.Name;
                cartModel.GroupId = GenerateGroupId(cartModel, product, groupIdProductAttribute, groupIdPersonalizeAttribute);
            }
        }

        protected virtual string GetShortDescription(PublishedProductEntityModel product, ShoppingCartItemModel cartModel, int catalogVersionId)
        {
            string shortDescription = string.Empty;
            List<PublishedConfigurableProductEntityModel> configurableProducts = publishProductHelper.GetConfigurableProductEntity(cartModel.ParentProductId, catalogVersionId);
            shortDescription = string.IsNullOrEmpty(cartModel.ConfigurableProductSKUs)
                ? GetAttributeValueByCode(product, ZnodeConstant.ShortDescription)
                : (configurableProducts?.Count > 0
                    ? string.Join("<br>", product?.Attributes?.Where(x => x.IsConfigurable && (configurableProducts?.FirstOrDefault()?.ConfigurableAttributeCodes?.Contains(x.AttributeCode)).GetValueOrDefault()).OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value).Distinct())
                    : string.Join("<br>", product?.Attributes?.Where(x => x.IsConfigurable)?.OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value)?.Distinct())
                  );
            return shortDescription;
        }

        protected virtual string GetShortDescription(PublishedProductEntityModel product, ShoppingCartItemModel cartModel)
        {
            string shortDescription = string.Empty;
            shortDescription = string.IsNullOrEmpty(cartModel.ConfigurableProductSKUs) ? GetAttributeValueByCode(product, ZnodeConstant.ShortDescription) :
                string.Join("<br>", product?.Attributes?.Where(x => x.IsConfigurable).OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value).Distinct());

            return shortDescription;
        }

        protected virtual void SetInventoryData(ZnodeProductBase product, ZnodeShoppingCartItem znodeCartItem = null, int omsOrderId = 0)
        {
            if (omsOrderId > 0)
            {
                product.InventoryTracking = znodeCartItem.InventoryTracking;
                SetProductAllowBackOrderAndTrackInventory(znodeCartItem.InventoryTracking, product);
            }
            else if (IsNotNull(product))
            {
                string inventorySettingCode = product.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValueCode;
                SetProductAllowBackOrderAndTrackInventory(inventorySettingCode, product);
            }
        }

        protected virtual string GenerateGroupId(ShoppingCartItemModel cartItemModel, PublishedProductEntityModel product, string groupIdProductAttribute, GlobalSettingValues groupIdPersonalizeAttribute)
        {
            string groupId = null;

            string[] groupIdAttrCombination = !string.IsNullOrEmpty(groupIdProductAttribute) ? groupIdProductAttribute.Split('|') : null;
            if (IsNotNull(groupIdAttrCombination) && groupIdAttrCombination.Any())
            {
                groupId = cartItemModel.SKU;
                foreach (string item in groupIdAttrCombination)
                {
                    string attrValue = GetAttributeValueCodeByCode(product, item);
                    groupId = string.IsNullOrEmpty(attrValue) ? groupId : $"{groupId}|{attrValue}";
                }
                if (IsNotNull(groupIdPersonalizeAttribute) && !string.IsNullOrEmpty(groupId))
                {
                    groupId = SetPersonalizeGroupId(cartItemModel, groupIdPersonalizeAttribute, groupId);
                }
            }
            return groupId;
        }

        protected virtual string SetPersonalizeGroupId(ShoppingCartItemModel cartItemModel, GlobalSettingValues groupIdPersonalizeAttribute, string groupId)
        {
            return $"{groupId}|{GetPersonalizeDesignId(cartItemModel, groupIdPersonalizeAttribute)}";
        }

        protected virtual string GetPersonalizeDesignId(ShoppingCartItemModel cartItemModel, GlobalSettingValues groupIdPersonalizeAttribute)
        {
            string designId = string.Empty;
            if (cartItemModel?.PersonaliseValuesDetail?.Count > 0)
            {
                designId = cartItemModel.PersonaliseValuesDetail.Select(x => x.DesignId)?.FirstOrDefault();
            }
            else if (cartItemModel?.PersonaliseValuesList?.Count > 0 && (HelperUtility.IsNotNull(cartItemModel.PersonaliseValuesList[groupIdPersonalizeAttribute.Value1])))
            {
                dynamic customData = JsonConvert.DeserializeObject(Convert.ToString(cartItemModel.PersonaliseValuesList[groupIdPersonalizeAttribute.Value1]));
                designId = customData[groupIdPersonalizeAttribute.Value2];
            }
            return designId;
        }

        public virtual string GetAttributeValueByCode(ZnodeShoppingCartItem znodeCartItem, PublishedProductEntityModel product, string code)
        {
            return znodeCartItem?.Product?.ZNodeGroupProductCollection?.Count > 0 ? znodeCartItem?.Product?.ZNodeGroupProductCollection[0]?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.AttributeValue : product?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.SelectValues[0]?.Value;
        }

        public virtual string GetAttributeValueByCode(ZnodeShoppingCartItem znodeCartItem, PublishProductModel product, string code)
            => znodeCartItem?.Product?.ZNodeGroupProductCollection?.Count > 0 ? znodeCartItem?.Product?.ZNodeGroupProductCollection[0]?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.AttributeValue : product?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.SelectValues?.FirstOrDefault()?.Value;

        protected virtual string GetAttributeValueByCode(PublishedProductEntityModel product, string code)
        {
            return product?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.AttributeValues;
        }

        protected virtual string GetAttributeValueCodeByCode(PublishedProductEntityModel product, string code)
        {
            return product?.Attributes?.FirstOrDefault(x => x.AttributeCode == code)?.AttributeValues;
        }


        //to get product quantity return zero if group or configurable product
        public virtual decimal GetProductQuantity(ZnodeShoppingCartItem cartItem, decimal quantity)
        {
            if (IsNotNull(cartItem) && cartItem.Product.ZNodeGroupProductCollection?.Count > 0)
                return cartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity;
            return quantity;
        }

        protected virtual ZnodeGenericCollection<ZnodeShoppingCartItem> GetAddressCartItems(int x, int portalId)
        {
            var items = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Where(y => (y.Product.PortalID > 0 ? y.Product.PortalID : portalId) == x).ToList();
            ZnodeGenericCollection<ZnodeShoppingCartItem> returnItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
            items.ForEach(y => returnItems.Add(y));
            return returnItems;
        }

        protected virtual ZnodeGenericCollection<ZnodeShoppingCartItem> GetAddressCartItems(int x, int portalId, ZnodeGenericCollection<ZnodeShoppingCartItem> shoppingCartItems)
        {
            var items = shoppingCartItems.Cast<ZnodeShoppingCartItem>().Where(y => (y.Product.PortalID > 0 ? y.Product.PortalID : portalId) == x).ToList();
            ZnodeGenericCollection<ZnodeShoppingCartItem> returnItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
            items.ForEach(y => returnItems.Add(y));
            return returnItems;
        }

        //To get Addon products collection 
        protected virtual ZnodeGenericCollection<ZnodeProductBaseEntity> GetZnodeProductAddons(PublishProductModel productModel, int publishedCatalogId, int localeId, List<WebStoreAddOnModel> addOns = null, string countryCode = null, int userId = 0, int omsOrderId = 0, int profileId = 0, int catalogVersionId = 0)
        {
            ZnodeGenericCollection<ZnodeProductBaseEntity> addonsCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
            if (IsNull(addOns))
            {
                addOns = publishProductHelper.GetAddOnsData(productModel.PublishProductId, productModel.ParentPublishProductId, productModel.PortalId, localeId, catalogVersionId, userId, productModel.ZnodeCatalogId, omsOrderId: omsOrderId);
            }

            if (IsNull(productModel.AssociatedAddOnProducts))
            {
                productModel.AssociatedAddOnProducts = new List<AssociatedProductModel>();
                productModel.AddonProductSKUs?.Split(',').ToList().ForEach(x => productModel.AssociatedAddOnProducts.Add(new AssociatedProductModel { Sku = x, Quantity = Convert.ToDecimal(productModel.Quantity), OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.AddOns }));
            }

            if (IsNotNull(productModel.AssociatedAddOnProducts))
            {
                foreach (AssociatedProductModel item in productModel.AssociatedAddOnProducts)
                {
                    foreach (string sku in item.Sku?.Split(','))
                    {
                        PublishProductModel product = GetPublishProductBySKU(sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId)?.ToModel<PublishProductModel>();

                        if (IsNotNull(product))
                        {
                            ZnodeProduct addonproduct = GetProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, catalogVersionId, countryCode, false, "", userId, omsOrderId, null, profileId);
                            addonproduct.AddonGroupName = GetAddonGroupName(addOns, addonproduct.ProductID);
                            addonproduct.SelectedQuantity = item.Quantity;
                            addonproduct.OrdersDiscount = item.OrdersDiscount;
                            addonsCollection.Add(new ZnodeProductTypeEntity(addonproduct));
                        }
                    }
                }
            }
            return addonsCollection;
        }

        //To get bundle products collection 
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
       " This method mark as obsolete as required PublishProductModel instead of only bundleProductSKUs as a parameter" +
       " Please use overload of this method having PublishProductModel as a parameters")]
        protected virtual ZnodeGenericCollection<ZnodeProductBaseEntity> GetZnodeProductBundles(string bundleProductSKUs, int publishedCatalogId, int localeId, string countryCode = null, int omsOrderId = 0, int profileId = 0, int catalogVersionId = 0)
        {
            ZnodeGenericCollection<ZnodeProductBaseEntity> bundleCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();

            if (!string.IsNullOrEmpty(bundleProductSKUs))
            {
                List<string> bundles = bundleProductSKUs.Split(',').ToList<string>();
                foreach (string item in bundles)
                {
                    PublishProductModel product = GetPublishProductBySKU(item, publishedCatalogId, localeId, catalogVersionId, omsOrderId)?.ToModel<PublishProductModel>();

                    if (IsNotNull(product))
                    {
                        bundleCollection.Add(new ZnodeProductTypeEntity(GetProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, catalogVersionId, countryCode, false, string.Empty, 0, omsOrderId, null, profileId)));
                    }
                }
            }
            return bundleCollection;
        }

        //To get bundle products collection 
        protected virtual ZnodeGenericCollection<ZnodeProductBaseEntity> GetZnodeProductBundles(PublishProductModel productModel, int publishedCatalogId, int localeId, string countryCode = null, int omsOrderId = 0, int profileId = 0, int catalogVersionId = 0)
        {
            ZnodeGenericCollection<ZnodeProductBaseEntity> bundleCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();

            if (HelperUtility.IsNotNull(productModel) && !string.IsNullOrEmpty(productModel.BundleProductSKUs))
            {
                List<string> bundles = productModel.BundleProductSKUs.Split(',').ToList<string>();
                List<PublishProductModel> productList = GetPublishProductBySKUs(bundles, publishedCatalogId, localeId, catalogVersionId)?.ToModel<PublishProductModel>()?.ToList();
                List<AssociatedPublishedBundleProductModel> publishBundleChildModel = BindBundleProductChildByParentSku(productModel.SKU, publishedCatalogId, localeId);
                List<ZnodeOmsOrderLineItem> orderLineItems = omsOrderId != 0 ? orderHelper.GetOrderLineItemByOmsOrderId(omsOrderId) : null;
                foreach (string item in bundles)
                {
                    PublishProductModel product = productList?.FirstOrDefault(x => x.SKU == item);
                    if (IsNotNull(product))
                    {
                        int bundleChildProductQuantity = 0;
                        if (omsOrderId == 0)
                        {
                            bundleChildProductQuantity = Convert.ToInt32(publishBundleChildModel?.Where(x => x.SKU.ToLower() == item.ToLower()).Select(x => x.AssociatedQuantity).FirstOrDefault() ?? 1);
                        }
                        else
                        {
                            bundleChildProductQuantity = Convert.ToInt32(orderLineItems?.Where(x => x.Sku.ToLower() == item.ToLower()).FirstOrDefault().BundleQuantity);
                        }
                        bundleCollection.Add(new ZnodeProductTypeEntity(GetProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, catalogVersionId, countryCode, false, productModel.SKU, 0, omsOrderId, null, profileId, isBundleProduct: true, bundleProductChildQuantity: bundleChildProductQuantity)));
                    }
                }
            }
            return bundleCollection;
        } 

        //To get bundle configurable collection 
        protected virtual ZnodeGenericCollection<ZnodeProductBaseEntity> GetZnodeProductConfigurables(string configurableProductSKUs, int publishedCatalogId, int localeId, string countryCode = null, int parentProductId = 0, int userId = 0, int omsOrderId = 0, int profileId = 0, decimal productQuantity = 0, int catalogVersionId = 0, List<PublishProductModel> cartLineItemsProductData = null, List<TaxClassRuleModel> lstTaxClassSKUs = null, List<PublishedConfigurableProductEntityModel> configEntities = null)
        {
            ZnodeGenericCollection<ZnodeProductBaseEntity> configurableCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
            if (!string.IsNullOrEmpty(configurableProductSKUs))
            {
                List<string> configurable = configurableProductSKUs.Split(',').ToList<string>();
                foreach (string item in configurable)
                {
                    PublishProductModel product = cartLineItemsProductData.FirstOrDefault(x => x.SKU == item);
                    if (IsNotNull(product))
                    {
                        product.ParentPublishProductId = parentProductId;
                        ZnodeProduct configureproduct = GetProductDetailsV2(product, this.PortalId.GetValueOrDefault(), localeId, countryCode, false, "", userId, omsOrderId, null, 0, lstTaxClassSKUs, configEntities);
                        configureproduct.SelectedQuantity = productQuantity;
                        configurableCollection.Add(new ZnodeProductTypeEntity(configureproduct));

                    }
                }
            }
            return configurableCollection;
        }

        //To get group product collection 
        protected virtual ZnodeGenericCollection<ZnodeProductBaseEntity> GetZnodeProductGroup(List<AssociatedProductModel> groupProducts, int publishedCatalogId, int localeId, string countryCode = null, int userId = 0, int omsOrderId = 0, int profileId = 0, int catalogVersionId = 0)
        {
            ZnodeGenericCollection<ZnodeProductBaseEntity> groupProductCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();


            foreach (AssociatedProductModel item in groupProducts)
            {
                PublishProductModel product = GetPublishProductBySKU(item.Sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId)?.ToModel<PublishProductModel>();

                if (IsNotNull(product))
                {
                    ZnodeProduct groupProduct = GetProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, catalogVersionId, countryCode, false, "", userId, omsOrderId, null, profileId);
                    groupProduct.SelectedQuantity = item.Quantity;
                    groupProduct.OmsSavedCartLineItemId = item.OmsSavedCartLineItemId;
                    groupProduct.OrdersDiscount = item.OrdersDiscount;
                    groupProductCollection.Add(new ZnodeProductTypeEntity(groupProduct));
                }
            }
            return groupProductCollection;
        }

        //To get tier price collection 
        public virtual ZnodeGenericCollection<ZnodeProductTierEntity> GetZnodeProductTierPrice(PublishProductModel publishProduct)
        {
            ZnodeGenericCollection<ZnodeProductTierEntity> tierPriceCollection = new ZnodeGenericCollection<ZnodeProductTierEntity>();

            if (publishProduct?.TierPriceList?.Count > 0)
            {
                foreach (PriceTierModel tierPrice in publishProduct.TierPriceList)
                {
                    tierPriceCollection.Add(new ZnodeProductTierEntity { Price = tierPrice.Price.GetValueOrDefault(), TierQuantity = Convert.ToInt32(tierPrice.Quantity.GetValueOrDefault()), MaxQuantity = tierPrice.MaxQuantity.GetValueOrDefault(), MinQuantity = tierPrice.MinQuantity.GetValueOrDefault() });
                }
            }
            return tierPriceCollection;
        }

        //to get addon get addon group name
        protected virtual string GetAddonGroupName(List<WebStoreAddOnModel> addOns, int productId)
        {
            string addonGroupName = string.Empty;
            try
            {
                addonGroupName = (from addOnList in addOns.ToList()
                                  from addonValues in addOnList.AddOnValues.ToList()
                                  where Equals(addonValues.PublishProductId, productId)
                                  select addOnList.GroupName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return addonGroupName;
        }

        //to checks if the coupon quantity is available.
        public virtual bool IsCouponQuantityAvailable(string couponCode)
        {
            bool isCouponQuantityAvailable = true;
            if (orderHelper.GetCouponAvailableQuantity(couponCode) <= 0)
            {
                int orderId = this.OrderId.GetValueOrDefault();
                if (orderId > 0)
                {
                    if (!orderHelper.IsExistingOrderCoupon(orderId, couponCode))
                    {
                        this.Coupons.Remove(this.Coupons.FirstOrDefault(x => x.Coupon == couponCode));
                        _ErrorMessage.Append("Sorry, this Coupon is not available.");
                        isCouponQuantityAvailable = false;
                    }
                }
                else
                {
                    this.Coupons.Remove(this.Coupons.FirstOrDefault(x => x.Coupon == couponCode));
                    _ErrorMessage.Append("Sorry, this Coupon is not available.");
                    isCouponQuantityAvailable = false;
                }
            }
            return isCouponQuantityAvailable;
        }

        // Check inventory and min/max quantity.
        public virtual void CheckInventoryAndMinMaxQuantity(out string isInventoryInStockMessage, out Dictionary<int, string> minMaxSelectableQuantity)
        {
            isInventoryInStockMessage = string.Empty;
            isInventoryInStockMessage = !IsInventoryInStock() ? "Unable to place the order as product is not available." : "";

            minMaxSelectableQuantity = IsValidMinAndMaxSelectedQuantity();
        }

        //to add product and its associated items in dictionary
        protected virtual void AddProductInDictionary(ZnodeShoppingCartItem item)
        {
            string parentProductSku = item.Product.SKU;
            decimal quantity = item.Quantity;

            if (item.Product.ZNodeConfigurableProductCollection.Count > 0)
            {
                parentProductSku = string.Empty;
            }

            if (item.Product.ZNodeGroupProductCollection.Count > 0)
            {
                parentProductSku = string.Empty;
                quantity = 0;
            }

            if (!string.IsNullOrEmpty(parentProductSku) && quantity > 0)
            {
                AddSKUInDictionary(item.Product, quantity);
            }

            foreach (ZnodeProductBaseEntity addon in item.Product.ZNodeAddonsProductCollection)
            {
                AddSKUInDictionary(addon, quantity);
            }


            foreach (ZnodeProductBaseEntity config in item.Product.ZNodeConfigurableProductCollection)
            {
                AddSKUInDictionary(config, quantity);
            }

            foreach (ZnodeProductBaseEntity group in item.Product.ZNodeGroupProductCollection)
            {
                AddSKUInDictionary(group, group.SelectedQuantity);
            }
        }

        //to check dont track inventory set to true
        protected virtual bool IsDisablePurchasing(string inventoryTracking)
        {
            return (inventoryTracking.ToLower() == ZnodeConstant.DisablePurchasing.ToString().ToLower());
        }

        //to add sku and product quantity to dictionary if inventory tracking of the product is set to disablepurchasing
        protected virtual void AddSKUInDictionary(ZnodeProductBaseEntity product, decimal quantity)
        {
            if (IsDisablePurchasing(product.InventoryTracking))
            {
                AddUpdateSKUQuantity(product.SKU, quantity);
            }
        }

        //to add all products distinct sku and its total quantity in dictionary, function will add the quantities of the products in the cart, will return the exact number of total quantity of the sku in cart.     
        protected virtual void AddUpdateSKUQuantity(string sku, decimal quantity)
        {
            if (SKUQuantity?.Count > 0)
            {
                if (SKUQuantity.ContainsKey(sku))
                {
                    decimal currentQuantity = GetQuantityBySKU(sku);
                    currentQuantity += quantity;
                    SKUQuantity.Remove(sku);
                    SKUQuantity.Add(sku, currentQuantity);
                }
                else
                {
                    SKUQuantity.Add(sku, quantity);
                }
            }
            else
            {
                SKUQuantity?.Add(sku, quantity);
            }
        }

        /// <summary>
        /// This function will return total quantity of SKU in current cart.
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Return latest SKU quantity</returns>
        protected virtual decimal GetQuantityBySKU(string sku)
        {
            decimal quantity = 0;
            foreach (KeyValuePair<string, decimal> pair in SKUQuantity)
            {
                if (pair.Key.Equals(sku))
                {
                    quantity = pair.Value;
                    break;
                }
            }
            return quantity;
        }

        //Bind Product entity value.
        protected virtual ZnodeProduct GetProductDetails(PublishProductModel publishProduct, int portalId, int localeId, int catalogVersionId, string countryCode = null, bool isGroupProduct = false, string parentSKU = "", int userId = 0, int omsOrderId = 0, string parentSKUProductName = null, int profileId = 0, bool isBundleProduct = false, int bundleProductChildQuantity = 0)
        {
            if ((IsNull(publishProduct)))
            {
                return null;
            }

            publishProduct.ParentSEOCode = parentSKU;

            publishProductHelper.GetDataFromExpands(portalId, new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing, ZnodeConstant.AddOns, ZnodeConstant.SEO }, publishProduct, localeId, "", userId, catalogVersionId, null, profileId, omsOrderId);
            bool isProductHasRetailPrice = true;
            if (IsNull(publishProduct.RetailPrice))
            {
                isProductHasRetailPrice = false;
                GetParentProductPriceDetails(publishProduct, portalId, localeId, parentSKU, userId, profileId);
            }
            if(isBundleProduct && !isProductHasRetailPrice)
            {
                publishProduct.SKU = !String.IsNullOrEmpty(publishProduct.ConfigurableProductSKU) ? publishProduct.ConfigurableProductSKU : publishProduct.SKU;
            }

            List<AttributesSelectValuesModel> inventorySetting = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Vendor)?.SelectValues;
            string vendorCode = inventorySetting?.Count > 0 ? inventorySetting.FirstOrDefault().Code : string.Empty;

            ZnodeProduct product = new ZnodeProduct
            {
                ProductID = publishProduct.PublishProductId,
                SEOURL = publishProduct.SEOUrl,
                Name = isGroupProduct ? parentSKUProductName : publishProduct.Name,
                SKU = isGroupProduct ? parentSKU : publishProduct.SKU,
                SalePrice = publishProduct.SalesPrice,
                RetailPrice = publishProduct.RetailPrice.GetValueOrDefault(),
                QuantityOnHand = publishProduct.Quantity.GetValueOrDefault(),
                ZNodeTieredPriceCollection = GetZnodeProductTierPrice(publishProduct),
                TaxClassID = GetTaxClassId(publishProduct.SKU, countryCode),
                AddOns = publishProduct.AddOns,
                IsPriceExist = isGroupProduct ? true : IsProductPriceExist(publishProduct.SalesPrice, publishProduct.RetailPrice),
                VendorCode = vendorCode,
                IsActive = publishProduct.IsActive,
                ProductCategoryIds = new int[] { publishProduct.ZnodeCategoryIds },
                AllowedTerritories = GetProductAttributeAllowedTerritoriesValue(publishProduct, ZnodeConstant.AllowedTerritories),
                SelectedQuantity = bundleProductChildQuantity
            };
            if (publishProduct.Attributes?.Count > 0)
            {
                product.AllowBackOrder = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.AllowBackOrdering);
                product.FreeShippingInd = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.FreeShipping);
                product.ShipSeparately = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.ShipSeparately);
                product.MinQty = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MinimumQuantity);
                product.MaxQty = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MaximumQuantity);
                product.InventoryTracking = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues?.FirstOrDefault()?.Code ?? string.Empty;
                product.ShippingRuleTypeCode = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingCost)?.SelectValues?.FirstOrDefault()?.Code;
                product.BrandCode = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Brand)?.SelectValues?.FirstOrDefault()?.Code ?? string.Empty;
                product.Height = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Height);
                product.Width = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Width);
                product.Length = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Length);
                product.Weight = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Weight);
                product.UOM = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.UOM)?.SelectValues?.FirstOrDefault()?.Value;
                product.Container = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingContainer)?.SelectValues[0]?.Value;
                product.Size = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingSize)?.SelectValues[0]?.Code;
                product.PackagingType = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PackagingType)?.SelectValues[0]?.Value;
            }

            //to apply product promotional price
            product.ApplyPromotion();
            //set configurable product attributes
            if (publishProduct?.ParentPublishProductId > 0)
            {
                List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(publishProduct.ParentPublishProductId, _catalogVersionId);
                if (IsNotNull(configEntity))
                {
                    product.Description = !string.IsNullOrEmpty(product.Description)
                                          ? product.Description
                                          : (configEntity?.Count > 0
                                          ? string.Join("<br>", publishProduct?.Attributes?.Where(x => x.IsConfigurable && (configEntity?.FirstOrDefault()?.ConfigurableAttributeCodes?.Contains(x.AttributeCode)).GetValueOrDefault()).OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value).Distinct())
                                          : string.Join("<br>", publishProduct?.Attributes?.Where(x => x.IsConfigurable)?.OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value)?.Distinct()));
                }
            }

            //to set product attributes 
            SetProductAttributes(product, publishProduct);
            return product;
        }

        public virtual bool GetBooleanProductAttributeValue(PublishProductModel publishProduct, string attributeCode)
        {
            string attributeValue = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;

            return !string.IsNullOrEmpty(attributeValue) ? Convert.ToBoolean(attributeValue) : false;
        }

        public virtual decimal GetDecimalProductAttributeValue(PublishProductModel publishProduct, string attributeCode)
        {
            string attributeValue = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;

            return !string.IsNullOrEmpty(attributeValue) ? Convert.ToDecimal(attributeValue) : 0;
        }

        // Get allowed territories value from attribute.
        protected virtual string GetProductAttributeAllowedTerritoriesValue(PublishProductModel publishProduct, string attributeCode)
        {
            string attributeValue = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValues;
            return !string.IsNullOrEmpty(attributeValue) ? attributeValue : string.Empty;
        }

        //Get price data of main product for configurable product.
        public virtual void GetParentProductPriceDetails(PublishProductModel publishProduct, int portalId, int localeId, string parentSKU = "", int userId = 0, int profileId = 0)
        {
            string configSKU = publishProduct.SKU;
            publishProduct.SKU = parentSKU;
            publishProduct.ConfigurableProductSKU = configSKU;
            publishProductHelper.GetDataFromExpands(portalId, GetCartExpands(parentSKU), publishProduct, localeId, "", userId, null, null, profileId);

            if (IsNull(publishProduct.SalesPrice) && IsNull(publishProduct.RetailPrice))
            {
                publishProductHelper.GetDataFromExpands(portalId, GetCartExpands(string.Empty), publishProduct, localeId, "", userId, null, null, profileId);
            }
            else
            {
                publishProductHelper.GetDataFromExpands(portalId, new List<string> { ZnodeConstant.Promotions, ZnodeConstant.AddOns, ZnodeConstant.SEO }, publishProduct, localeId, "", userId, null, null, profileId);
            }
            publishProduct.SKU =  string.IsNullOrEmpty(publishProduct.SKU) ? configSKU : publishProduct.SKU;
        }

        //Get Expands for Cart Data.
        public virtual List<string> GetCartExpands(string parentSKU)
        {
            return !string.IsNullOrEmpty(parentSKU) ? new List<string> { ZnodeConstant.Inventory }
                       : new List<string> { ZnodeConstant.Pricing };
        }

        // Get tax class id by sku and country code.
        protected virtual int GetTaxClassId(string sKU, string countryCode)
            => publishProductHelper.GetTaxClassId(sKU, countryCode);


        // Get tax class id by sku and country code.
        protected virtual List<TaxClassRuleModel> GetTaxRules(List<string> sKUs)
            => publishProductHelper.GetTaxRules(sKUs);



        //To child items as per producttype for saved line items
        protected virtual List<AccountQuoteLineItemModel> BindProductType(List<AccountQuoteLineItemModel> childItems, ZnodeCartItemRelationshipTypeEnum enumProductType)
        {
            return childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)enumProductType)?.ToList();
        }

        //to calculate unit price and extended price
        protected virtual void CalculateLineItemPrice(ShoppingCartItemModel lineItem, List<ZnodeOmsOrderLineItem> childItems)
        {
            if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)) || IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)))
            {
                lineItem.UnitPrice = lineItem.UnitPrice > 0 ? lineItem.UnitPrice : childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable || x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group).Sum(x => x.Price);
            }

            if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)))
            {
                lineItem.UnitPrice += childItems
                                    .Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                                    && x.ParentOmsOrderLineItemsId == lineItem.OmsOrderLineItemsId
                                    ).Sum(x => x.Price);
            }

            lineItem.ExtendedPrice = lineItem.UnitPrice * lineItem.Quantity;
            lineItem.CustomUnitPrice = lineItem.UnitPrice;
            //to set externalid of line item
            lineItem.ExternalId = Guid.NewGuid().ToString();
        }

        //to set product type data for shoppingcart line item 
        protected virtual void SetAssociateProductType(ShoppingCartItemModel lineItem, List<ZnodeOmsOrderLineItem> childItems)
        {
            lineItem.AddOnProductSKUs = string.Join(",", childItems
                                        .Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                                         && x.ParentOmsOrderLineItemsId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.Sku));
            lineItem.BundleProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles
                                         && x.ParentOmsOrderLineItemsId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.Sku));
            lineItem.ConfigurableProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable && x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.Sku));
            lineItem.GroupProducts = childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group
                                                   && x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId)?.ToModel<AssociatedProductModel>()?.ToList();
        }

        //to set set product image
        protected virtual void SetProductImage(ShoppingCartItemModel lineItem, int publishedCatalogId, int localeId, int omsOrderId = 0)
        {
            int catalogversionId = GetCatalogVersionId(publishedCatalogId, localeId);
            PublishedProductEntityModel product = new PublishedProductEntityModel();

            if (lineItem?.GroupProducts?.Count > 0)
            {
                product = GetPublishProductBySKU(lineItem.GroupProducts.FirstOrDefault().Sku, publishedCatalogId, localeId, catalogversionId, omsOrderId)?.ToModel<PublishedProductEntityModel>();
            }
            else
            {
                product = GetPublishProductBySKU(!string.IsNullOrEmpty(lineItem.ConfigurableProductSKUs) ? lineItem.ConfigurableProductSKUs : lineItem.SKU, publishedCatalogId, localeId, catalogversionId, omsOrderId)?.ToModel<PublishedProductEntityModel>();
            }
            if (IsNotNull(product))
            {
                lineItem.ImagePath = product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;

                lineItem.MinQuantity = Convert.ToDecimal(product.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.FirstOrDefault()?.AttributeValues);
                lineItem.MaxQuantity = Convert.ToDecimal(product.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.FirstOrDefault()?.AttributeValues);
                lineItem.UOM = product.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.UOM)?.SelectValues?.FirstOrDefault()?.Value;
                if (lineItem?.GroupProducts?.Count > 0)
                {
                    lineItem.GroupProducts.Where(w => w.Sku == lineItem.GroupProducts.FirstOrDefault().Sku).ToList().ForEach(s => s.ProductId = product.ZnodeProductId);
                }
            }
        }

        //to remove product discount by id
        protected virtual bool RemoveProductDiscountById(List<ShoppingCartDiscountModel> productDiscount, int productId, int parentProductId)
        {
            var removeitem = productDiscount.FirstOrDefault(discount => (discount.ProductId == productId) && (discount.ParentProductId == parentProductId));
            productDiscount.Remove(removeitem);
            return true;
        }

        //to get discount from shoppingcart
        protected virtual void GetDiscountFromShoppingCart(List<ShoppingCartDiscountModel> productDiscount)
        {
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCartItems)
            {
                if (IsNotNull(cartItem?.Product))
                {
                    if (cartItem.Product.DiscountAmount > 0.0M)
                    {
                        productDiscount.Add(new ShoppingCartDiscountModel { ProductId = cartItem.Product.ProductID, ParentProductId = 0, DiscountAmount = cartItem.Product.DiscountAmount });
                    }

                    if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0 || cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                    {
                        RemoveProductDiscountById(productDiscount, cartItem.Product.ProductID, 0);
                    }

                    foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        if (addon.DiscountAmount > 0.0M)
                        {
                            productDiscount.Add(new ShoppingCartDiscountModel { ProductId = addon.ProductID, ParentProductId = cartItem.Product.ProductID, DiscountAmount = addon.DiscountAmount });
                        }
                    }

                    foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                    {
                        if (configurable.DiscountAmount > 0.0M)
                        {
                            productDiscount.Add(new ShoppingCartDiscountModel { ProductId = configurable.ProductID, ParentProductId = cartItem.Product.ProductID, DiscountAmount = configurable.DiscountAmount });
                        }
                    }
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (group.DiscountAmount > 0.0M)
                        {
                            productDiscount.Add(new ShoppingCartDiscountModel { ProductId = group.ProductID, ParentProductId = cartItem.Product.ProductID, DiscountAmount = group.DiscountAmount });
                        }
                    }
                }
            }
        }

        //to set discount to shoppingcart
        protected virtual void SetShoppingCartDiscount(List<ShoppingCartDiscountModel> productDiscount)
        {
            SetProductDiscount(productDiscount);

            foreach (ZnodeShoppingCartItem cartItem in this.ShoppingCartItems)
            {
                int parentProductId = cartItem.Product.ProductID;
                if (cartItem.Product.ZNodeAddonsProductCollection.Count > 0)
                {
                    SetProductTypeDiscount(productDiscount, cartItem.Product.ZNodeAddonsProductCollection, parentProductId);
                }

                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    SetProductTypeDiscount(productDiscount, cartItem.Product.ZNodeConfigurableProductCollection, parentProductId);
                }

                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    SetProductTypeDiscount(productDiscount, cartItem.Product.ZNodeGroupProductCollection, parentProductId);
                }
            }
        }

        //to set product discount
        protected virtual void SetProductDiscount(List<ShoppingCartDiscountModel> productDiscount)
        {
            foreach (ShoppingCartDiscountModel item in productDiscount)
            {
                foreach (ZnodeShoppingCartItem cartItem in this.ShoppingCartItems)
                {
                    if (Equals(item.ProductId, cartItem.Product.ProductID) && Equals(item.ParentProductId, 0) && item.DiscountAmount > 0.0M)
                    {
                        cartItem.Product.DiscountAmount = item.DiscountAmount;
                    }
                }
            }
        }

        //to set product type discount
        protected virtual void SetProductTypeDiscount(List<ShoppingCartDiscountModel> productDiscount, ZnodeGenericCollection<ZnodeProductBaseEntity> childproduct, int parentProductId)
        {
            foreach (ShoppingCartDiscountModel item in productDiscount)
            {
                foreach (ZnodeProductBaseEntity childItem in childproduct)
                {
                    if (Equals(item.ProductId, childItem.ProductID) && Equals(item.ParentProductId, parentProductId) && item.DiscountAmount > 0.0M)
                    {
                        childItem.DiscountAmount = item.DiscountAmount;
                    }
                }
            }
        }

        //to update giftcard details
        [Obsolete("The Gift card functionality was replaced by voucher so no longer use of this method")]
        protected virtual void UpdateGiftCard(int? userId, decimal giftCardAmount, bool isGuest = true)
        {
            GiftCardModel giftCard = new GiftCardModel();
            if (isGuest == false)
                giftCard.UserId = userId;
            giftCard.Amount = giftCardAmount;
            giftCard.CardNumber = this.GiftCardNumber;
            orderHelper.UpdateGiftCard(giftCard);
        }

        //Update Applied voucher details
        protected virtual void UpdateVoucher(int? userId, decimal giftCardAmount, bool isGuest = true)
        {
            GiftCardModel giftCard = new GiftCardModel();
            if (this.Vouchers?.Count > 0)
            {
                foreach (ZnodeVoucher voucher in this.Vouchers)
                {
                    if (voucher.IsVoucherApplied)
                    {
                        if (!isGuest)
                            giftCard.UserId = userId;
                        giftCard.Amount = voucher.VoucherAmountUsed;
                        giftCard.CardNumber = voucher.VoucherNumber;
                        if (voucher.IsExistInOrder)
                            giftCard.RemainingAmount = voucher.VoucherBalance;
                        orderHelper.UpdateGiftCard(giftCard, voucher.IsExistInOrder);
                    }
                }
            }
        }

        //to get login user id
        public virtual int? GetUserId()
        {
            int? userId = base.UserId;
            if ((IsNull(userId) || userId == 0) && IsNotNull(this.UserAddress?.UserId))
            {
                userId = this.UserAddress?.UserId;
            }
            return userId;
        }

        //Map AccountQuoteLineItemModel to AccountQuoteLineItemModel.
        protected virtual void ToZNodeShoppingCartItem(AccountQuoteLineItemModel quoteLineItemModel, ZnodeShoppingCartItem cartLineItem, string parentSKu)
        {
            cartLineItem.OmsQuoteId = quoteLineItemModel.OmsQuoteId;
            cartLineItem.OmsQuoteLineItemId = quoteLineItemModel.OmsQuoteLineItemId;
            cartLineItem.ParentOmsQuoteLineItemId = quoteLineItemModel.ParentOmsQuoteLineItemId;
            cartLineItem.OrderLineItemRelationshipTypeId = quoteLineItemModel.OrderLineItemRelationshipTypeId;
            cartLineItem.CustomText = quoteLineItemModel.CustomText;
            cartLineItem.CartAddOnDetails = quoteLineItemModel.CartAddOnDetails;
            cartLineItem.SKU = string.IsNullOrEmpty(parentSKu) ? quoteLineItemModel.SKU : parentSKu;
            cartLineItem.Quantity = quoteLineItemModel.Quantity;
            cartLineItem.CustomUnitPrice = quoteLineItemModel.Price;
            cartLineItem.ParentOmsSavedCartLineItemId = quoteLineItemModel.ParentOmsSavedCartLineItemId;
            cartLineItem.ShippingCost = Convert.ToDecimal(quoteLineItemModel.ShippingCost);
        }

        //Map ShoppingCartItemModel to AccountQuoteLineItemModel.
        protected virtual void BindShoppingCartItemModel(ShoppingCartItemModel model, ZnodeShoppingCartItem znodeCartItem, string parentSKU)
        {
            znodeCartItem.OmsQuoteId = model.OmsQuoteId;
            znodeCartItem.OmsQuoteLineItemId = model.OmsQuoteLineItemId;
            znodeCartItem.ParentOmsQuoteLineItemId = model.ParentOmsQuoteLineItemId;
            znodeCartItem.OrderLineItemRelationshipTypeId = model.OrderLineItemRelationshipTypeId;
            znodeCartItem.CustomText = model.CustomText;
            znodeCartItem.CartAddOnDetails = model.CartAddOnDetails;
            //znodeCartItem.SKU = parentSKU;  // This Line was making Parent Sku bind into shopping cart items
            znodeCartItem.Quantity = model.Quantity;
            znodeCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();
            znodeCartItem.ParentOmsSavedCartLineItemId = model.ParentOmsSavedcartLineItemId.GetValueOrDefault();
        }

        protected virtual int GetParentProductId(string sku, int publishedCatalogId, int localeId, int omsOrderId = 0)
        {
            int catalogVersionId = GetCatalogVersionId(publishedCatalogId, localeId);
            return (GetPublishProductBySKU(sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId)?.ZnodeProductId).GetValueOrDefault();
        }

        //to check product price exist returns true if both sale & retail price is null
        protected virtual bool IsProductPriceExist(decimal? saleprice, decimal? retailPrice)
        {
            return !(IsNull(saleprice) && IsNull(retailPrice));
        }

        //to set product attributes in  ZNodeProduct from PublishProductModel
        public virtual void SetProductAttributes(ZnodeProduct product, PublishProductModel publishProduct)
        {
            if (IsNotNull(product) && (publishProduct?.Attributes?.Count > 0))
            {
                publishProduct.Attributes.ForEach(item =>
                {
                    if (!product.Attributes.Any(x => x.AttributeCode == item.AttributeCode))
                    {
                        if (!string.IsNullOrEmpty(item.AttributeValues))
                        {
                            product.Attributes.Add(new OrderAttributeModel { AttributeCode = item.AttributeCode, AttributeValue = item.AttributeValues });
                        }
                        else if (item?.SelectValues?.Count > 0)
                        {
                            product.Attributes.Add(new OrderAttributeModel { AttributeCode = item.AttributeCode, AttributeValue = item.SelectValues?.FirstOrDefault().Value, AttributeValueCode = item.SelectValues?.FirstOrDefault().Code });
                        }
                    }
                });
            }
        }

        //to get catalog version Id by  published catalog Id
        protected virtual int GetCatalogVersionId(int publishedCatalogId, int localeId = 0)
        {
            if (_catalogVersionId.Equals(0))
            {
                _catalogVersionId = publishProductHelper.GetCatalogVersionId(publishedCatalogId, localeId);
            }

            return _catalogVersionId;
        }

        //to set Configurable/group product quantity
        protected virtual void SetConfigurableOrGroupProductQuantity(ShoppingCartItemModel item, List<ZnodeOmsOrderLineItem> childItems, List<PublishedProductEntityModel> productList)
        {
            if (item.Quantity == 0)//to set Configurable product quantity
            {
                if (Convert.ToDecimal(childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)?.Select(s => s.Quantity)?.FirstOrDefault()) > 0)
                {
                    SetChildItemData(item, (int)ZnodeCartItemRelationshipTypeEnum.Configurable, childItems, productList);
                }
                else
                {
                    SetChildItemData(item, (int)ZnodeCartItemRelationshipTypeEnum.Group, childItems, productList);
                }
            }
            else if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)))
            {
                item.AddOnLineItemId = childItems.FirstOrDefault().OmsOrderLineItemsId;
                SetChildItemData(item, (int)ZnodeCartItemRelationshipTypeEnum.AddOns, childItems, productList);
            }
        }

        protected virtual void SetChildItemData(ShoppingCartItemModel item, int relationTypeId, List<ZnodeOmsOrderLineItem> childItems, List<PublishedProductEntityModel> productList)
        {
            ZnodeOmsOrderLineItem child = childItems.Where(x => x.OrderLineItemRelationshipTypeId == relationTypeId)?.FirstOrDefault();
            if (IsNotNull(child))
            {
                PublishedProductEntityModel childProduct = productList?.FirstOrDefault(x => x.SKU == child.Sku);
                SetInventoryData(item, childProduct);
                item.IsActive = (productList?.FirstOrDefault(x => x.SKU == child.Sku)?.IsActive).GetValueOrDefault();
                item.Quantity = child.Quantity.GetValueOrDefault();
                item.ChildProductId = child.OmsOrderLineItemsId;
                item.ShipSeperately = relationTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns ? child.ShipSeparately.GetValueOrDefault() : item.ShipSeperately;
                item.TaxCost = (child.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.SalesTax).GetValueOrDefault() + (child.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.GST).GetValueOrDefault() + (child.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.HST).GetValueOrDefault() + (child.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.PST).GetValueOrDefault() + (child.ZnodeOmsTaxOrderLineDetails?.FirstOrDefault()?.VAT).GetValueOrDefault();
                item.ShippingCost = child.ShippingCost.GetValueOrDefault();
            }
        }

        protected virtual void SetInventoryBundleData(ShoppingCartItemModel item)
        {
            foreach (var product in item.BundleProducts)
            {
                List<AttributesSelectValuesModel> inventorySettingList = product.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues;
                string inventorySettingCode = inventorySettingList?.FirstOrDefault().Code;
                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    product.TrackInventory = false;
                }
                else if (string.Equals(ZnodeConstant.AllowBackOrdering, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    product.AllowBackOrder = true;
                    product.TrackInventory = false;
                }
                else
                {
                    product.TrackInventory = true;
                }
            }
        }
        protected virtual void SetInventoryData(ShoppingCartItemModel item, PublishedProductEntityModel product)
        {
            if (IsNotNull(product))
            {
                List<PublishedSelectValuesEntityModel> inventorySettingList = product.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues;
                string inventorySettingCode = inventorySettingList?.FirstOrDefault().Code;
                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.TrackInventory = false;
                }
                else if (string.Equals(ZnodeConstant.AllowBackOrdering, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.AllowBackOrder = true;
                    item.TrackInventory = false;
                }
                else
                {
                    item.TrackInventory = true;
                }
            }
        }

        //to set order coupons applied at initial level
        protected virtual void SetOrderCoupons(ShoppingCartModel model, string couponCodes)
        {
            if (IsNull(model?.Coupons))
                model.Coupons = new List<CouponModel>();

            string[] coupons = couponCodes.Split(new string[] { ZnodeConstant.CouponCodeSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (coupons.Length > 0)
            {
                List<CouponModel> couponPromotionMessage = orderHelper.GetCouponPromotionMessages(coupons, model.IsOldOrder, model.OmsOrderId??0);
                foreach (CouponModel coupon in couponPromotionMessage)
                {
                    model?.Coupons.Add(new CouponModel { Code = coupon.Code, PromotionMessage = string.IsNullOrEmpty(coupon.PromotionMessage) ? GlobalSetting_Resources.MessageCouponAccepted : coupon.PromotionMessage, CouponApplied = true, CouponValid = true, IsExistInOrder = true });
                }
            }
        }

        //to set order coupons applied at initial level
        protected virtual void SetOrderVouchers(ShoppingCartModel model, string voucherCodes)
        {
            if (IsNull(model?.Vouchers))
                model.Vouchers = new List<VoucherModel>();
            string[] vouchers = voucherCodes?.Split(new string[] { ZnodeConstant.CouponCodeSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (vouchers?.Length > 0)
            {
                List<VoucherModel> voucherList = orderHelper.GetVoucherDetailByCodes(String.Join(",", vouchers), model.OmsOrderDetailsId);
                if(voucherList?.Count > 0)
                {
                    foreach (VoucherModel voucher in voucherList)
                    {
                        if(voucher.ExpirationDate <= DateTime.Today.Date && !voucher.IsActive)
                            voucher.VoucherBalance = 0;
                        model?.Vouchers.Add(new VoucherModel { VoucherNumber = voucher.VoucherNumber, IsVoucherApplied = true, IsVoucherValid = true, VoucherAmountUsed = voucher.VoucherAmountUsed, VoucherBalance = voucher.VoucherBalance, ExpirationDate = voucher.ExpirationDate, VoucherName = voucher.VoucherName, IsExistInOrder = true, PortalId = model.PortalId, CultureCode = model.CultureCode, OrderVoucherAmount= voucher.OrderVoucherAmount });
                    }
                    model.IsCalculateVoucher = true;
                }               
            }
        }

        //Map shopping cart item model.
        protected virtual ZnodeShoppingCartItem MapShoppingCartItemModel(ShoppingCartItemModel model, AddressModel shippingAddress)
        {
            this.PortalId = IsNotNull(this.PortalId) ? this.PortalId : GetHeaderPortalId();

            return new ZnodeShoppingCartItem(shippingAddress)
            {
                Description = string.IsNullOrEmpty(model.CartDescription) || string.IsNullOrWhiteSpace(model.CartDescription) ? model.Description : model.CartDescription,
                ExternalId = model.ExternalId,
                Quantity = model.Quantity,
                ShippingCost = model.ShippingCost,
                ShippingOptionId = model.ShippingOptionId,
                ParentProductId = model.ParentProductId,
                InsufficientQuantity = model.InsufficientQuantity,
                CustomUnitPrice = model.CustomUnitPrice,
                PartialRefundAmount = model.PartialRefundAmount,
                OrderStatusId = model.OmsOrderStatusId,
                OrderStatus = model.OrderLineItemStatus,
                TrackingNumber = model.TrackingNumber,
                IsEditStatus = model.IsEditStatus,
                IsActive = model.IsActive,
                IsItemStateChanged = model.IsItemStateChanged,
                IsSendEmail = model.IsSendEmail,
                OmsOrderId = model.OmsOrderId,
                OmsOrderLineItemId = model.OmsOrderLineItemsId,
                AutoAddonSKUs = model.AutoAddonSKUs,
                Custom1 = model.Custom1,
                Custom2 = model.Custom2,
                Custom3 = model.Custom3,
                Custom4 = model.Custom4,
                Custom5 = model.Custom5,
                ParentProductSKU = model.GroupProducts?.Count > 0 ? model.GroupProducts.FirstOrDefault()?.Sku : model.ParentProductSKU,
                SKU = model.ParentProductId == 0 ? model.SKU : (model.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple ? (model.ConfigurableProductSKUs ?? model.AddOnProductSKUs ?? model.BundleProductSKUs) : model.SKU),
                GroupId = model.GroupId,
                OrderLineItemRelationshipTypeId = model.OrderLineItemRelationshipTypeId,
                OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault(),
                ShipSeperately = model.ShipSeperately,
                Sequence = model.Sequence,
                GroupSequence = model.GroupSequence,
                ParentOmsSavedCartLineItemId = model.ParentOmsSavedcartLineItemId.GetValueOrDefault(),
                CustomText = model.CustomText,
                AdditionalCost = model.AdditionalCost,
                ParentOmsOrderLineItemsId = model.ParentOmsOrderLineItemsId,
                PerQuantityShippingCost = model.PerQuantityShippingCost,
                PerQuantityShippingDiscount = model.PerQuantityShippingDiscount,
                ProductLevelTax = model.ProductLevelTax,
                PaymentStatusId = model.PaymentStatusId,
                ShipDate = model.ShipDate,
                TaxRuleId = model.TaxRuleId,
                InventoryTracking = model.InventoryTracking,
                InitialPrice = model.InitialPrice,
                IsPriceEdit = model.IsPriceEdit,
                InitialShippingCost = model.InitialShippingCost,
                CustomShippingCost = model.CustomShippingCost,
                PersonaliseValuesDetail = model.PersonaliseValuesDetail,
                PersonaliseValuesList = model.PersonaliseValuesList
            };
        }

        //Map Shopping Cart other model.
        public virtual void MapShoppingCartOtherData(ShoppingCartItemModel model, AddressModel shippingAddress, ZnodeShoppingCartItem znodeCartItem)
        {
            int addressId = 0;

            // Cart level shipping address
            if (IsNotNull(shippingAddress))
            {
                addressId = shippingAddress.AddressId;
            }

            if (IsNotNull(model.MultipleShipToAddress) && model.MultipleShipToAddress.Any())
            {
                foreach (OrderShipmentModel shipToAddress in model.MultipleShipToAddress)
                {
                    if (shipToAddress.AddressId.Equals(0))
                    {
                        shipToAddress.AddressId = addressId;
                    }

                    ZnodeOrderShipment znodeOrderShipment = new ZnodeOrderShipment(shipToAddress.AddressId, shipToAddress.Quantity, znodeCartItem.GUID, shipToAddress.ShippingOptionId.GetValueOrDefault(0), shipToAddress.ShippingName);
                    znodeCartItem.OrderShipments.Add(znodeOrderShipment);
                }
            }
            else
            {
                // Cart item level shipping address
                if (IsNotNull(model.ShippingAddress))
                {
                    addressId = model.ShippingAddress.AddressId;
                }

                ZnodeOrderShipment orderShipment = new ZnodeOrderShipment(addressId, model.Quantity, znodeCartItem.GUID);
                znodeCartItem.OrderShipments.Add(orderShipment);
            }
        }

        protected virtual PublishProductModel GetPublishProductModel(ShoppingCartItemModel model, int localeId, int publishedCatalogId, int omsOrderId, string configParentSKU = "")
        {
            return new PublishProductModel
            {
                SKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : model.SKU,
                Quantity = model.Quantity,
                LocaleId = localeId,
                ParentPublishProductId = model.ParentProductId > 0 && (!string.IsNullOrEmpty(model.ConfigurableProductSKUs) || !string.IsNullOrEmpty(model.AutoAddonSKUs) || !string.IsNullOrEmpty(model.AddOnProductSKUs)) ? model.ParentProductId : GetParentProductId(!string.IsNullOrEmpty(configParentSKU) ? configParentSKU : model.SKU, publishedCatalogId, localeId, omsOrderId),
                PublishedCatalogId = publishedCatalogId,
                AddonProductSKUs = !string.IsNullOrEmpty(model.AddOnProductSKUs) ? model.AddOnProductSKUs : string.Empty,
                BundleProductSKUs = !string.IsNullOrEmpty(model.BundleProductSKUs) ? model.BundleProductSKUs : string.Empty,
                ConfigurableProductSKUs = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : string.Empty,
                GroupProductSKUs = model?.GroupProducts?.Count > 0 ? model.GroupProducts : new List<AssociatedProductModel>(),
                AssociatedAddOnProducts = model.AssociatedAddOnProducts
            };
        }

        public virtual PublishProductListModel GetPublishProductModelList(List<ShoppingCartItemModel> model, int localeId, int publishedCatalogId, int omsOrderId)
        {
            PublishProductListModel listModel = new PublishProductListModel();
            listModel.PublishProducts = model.Select(x => new PublishProductModel
            {
                SKU = !string.IsNullOrEmpty(x.ConfigurableProductSKUs) ? x.ConfigurableProductSKUs : x.SKU,
                Quantity = x.Quantity,
                LocaleId = localeId,
                ParentPublishProductId = x.ParentProductId > 0 && (!string.IsNullOrEmpty(x.ConfigurableProductSKUs) || !string.IsNullOrEmpty(x.AutoAddonSKUs)) ? x.ParentProductId : GetParentProductId(x.SKU, publishedCatalogId, localeId, omsOrderId),
                PublishedCatalogId = publishedCatalogId,
                AddonProductSKUs = !string.IsNullOrEmpty(x.AddOnProductSKUs) ? x.AddOnProductSKUs : string.Empty,
                BundleProductSKUs = !string.IsNullOrEmpty(x.BundleProductSKUs) ? x.BundleProductSKUs : string.Empty,
                ConfigurableProductSKUs = !string.IsNullOrEmpty(x.ConfigurableProductSKUs) ? x.ConfigurableProductSKUs : string.Empty,
                GroupProductSKUs = x?.GroupProducts?.Count > 0 ? x.GroupProducts : new List<AssociatedProductModel>()
            }).ToList();

            return listModel;
        }

        protected void AddToShoppingCartV2(AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems, CartParameterModel cartParameterModel, List<PublishProductModel> publishProductModel, int catalogVersionId, List<TaxClassRuleModel> lstTaxClassSKUs = null, List<ZnodePimDownloadableProduct> lstDownloadableProducts = null, List<PublishedConfigurableProductEntityModel> configEntities = null, List<PersonaliseValueModel> lstPersonalizedValues = null)
        {
            if (string.IsNullOrEmpty(cartLineItemModel.SKU))
                return;

            string parentSKUProductName = string.Empty;

            var configurableLineItem = new List<AccountQuoteLineItemModel>();
            configurableLineItem.Add(cartLineItemModel);

            List<AccountQuoteLineItemModel> shoppingCartLineItems = cartLineItemModel?.OmsQuoteId > 0 ? (
                   cartLineItemModel.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) ?
                   cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId)?.ToList() :
                   cartLineItems.Where(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId)?.ToList()) :
                   cartLineItemModel.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) ?
                   cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId)?.ToList() :
                   cartLineItems.Where(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId)?.ToList();


            this.PortalId = cartParameterModel.PortalId > 0 ? cartParameterModel.PortalId : GetHeaderPortalId();

            List<AccountQuoteLineItemModel> bundleLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.Bundles);
            List<AccountQuoteLineItemModel> configurableLineItems = BindProductType(configurableLineItem, ZnodeCartItemRelationshipTypeEnum.Configurable);
            List<AccountQuoteLineItemModel> groupLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.Group);
            List<AccountQuoteLineItemModel> addonLineItems = new List<AccountQuoteLineItemModel>();

            if (groupLineItems?.Count > 0)
            {
                foreach (AccountQuoteLineItemModel item in cartLineItems)
                {
                    if (item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns) && (item.OmsQuoteId > 0 ? groupLineItems.Any(y => y.OmsQuoteLineItemId == item.ParentOmsQuoteLineItemId) : groupLineItems.Any(y => y.OmsSavedCartLineItemId == item.ParentOmsSavedCartLineItemId)))
                        addonLineItems.Add(item);
                }

                parentSKUProductName = groupLineItems?.Count > 0 ? cartLineItemModel?.ProductName : string.Empty;
            }
            else
                addonLineItems = BindProductType(shoppingCartLineItems, ZnodeCartItemRelationshipTypeEnum.AddOns);

            List<AssociatedProductModel> addOnProducts = new List<AssociatedProductModel>();

            if (addonLineItems?.Count > 0)
                addonLineItems.ForEach(doc => addOnProducts.Add(new AssociatedProductModel { Sku = doc.SKU, Quantity = doc.Quantity }));

            ZnodeShoppingCartItem cartLineItem = new ZnodeShoppingCartItem(null);
            cartLineItem.OmsOrderId = cartLineItemModel.OmsOrderId;
            cartLineItem.OmsSavedCartLineItemId = cartLineItemModel.OmsSavedCartLineItemId;
            cartLineItem.ParentOmsSavedCartLineItemId = cartLineItemModel.ParentOmsSavedCartLineItemId;
            cartLineItem.CustomText = cartLineItemModel.CustomText;
            cartLineItem.CreatedBy = cartLineItemModel.CreatedBy;
            cartLineItem.CreatedDate = cartLineItemModel.CreatedDate;
            cartLineItem.ModifiedBy = cartLineItemModel.ModifiedBy;
            cartLineItem.ModifiedDate = cartLineItemModel.ModifiedDate;
            cartLineItem.CustomUnitPrice = cartLineItemModel.CustomUnitPrice;
            string parentSKu = cartParameterModel.OmsQuoteId > 0 ? GetQuoteParentSKU(cartLineItemModel, cartLineItems) : GetParentSKU(cartLineItemModel, cartLineItems);
            if (cartParameterModel.OmsQuoteId > 0)
            {
                //Map AccountQuoteLineItemModel to AccountQuoteLineItemModel.
                ToZNodeShoppingCartItem(cartLineItemModel, cartLineItem, parentSKu);
            }

            List<AssociatedProductModel> groupProduct = new List<AssociatedProductModel>();

            if (groupLineItems?.Count > 0)
                groupLineItems.ForEach(doc => groupProduct.Add(new AssociatedProductModel { Sku = doc.SKU, Quantity = doc.Quantity, OmsSavedCartLineItemId = doc.OmsSavedCartLineItemId, ProductName = doc.ProductName }));

            //Get cartitem having configurable product sku.
            AccountQuoteLineItemModel cartItem = cartLineItem.OmsQuoteId > 0 ? cartLineItems.FirstOrDefault(x => x.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId && x.OrderLineItemRelationshipTypeId == 3)
                                 : cartLineItems.FirstOrDefault(x => x.ParentOmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId && x.OrderLineItemRelationshipTypeId == 3);

            if (IsNotNull(cartItem))
                cartLineItemModel = cartItem;

            DateTime dtBindProductStartTime, dtBindProductEndTime;
            TimeSpan BindProducttotalTime;
            dtBindProductStartTime = DateTime.Now;
            ZnodeLogging.LogMessage("Cart - NewBindProductDetails - Start Time=" + dtBindProductStartTime.ToString(), "Cart", TraceLevel.Info);

            BindProductDetailsV2(cartLineItem, new PublishProductModel
            {
                SKU = cartLineItemModel.SKU,
                ParentPublishProductId = publishProductModel.FirstOrDefault(x => x.SKU.Equals(parentSKu, StringComparison.InvariantCultureIgnoreCase))?.PublishProductId ?? 0,
                Quantity = cartLineItemModel.Quantity,
                LocaleId = cartParameterModel.LocaleId,
                PublishedCatalogId = cartParameterModel.PublishedCatalogId,
                AddonProductSKUs = string.Join(",", addonLineItems.Select(b => b.SKU)),
                AssociatedAddOnProducts = addOnProducts,
                BundleProductSKUs = string.Join(",", bundleLineItems.Select(b => b.SKU)),
                ConfigurableProductSKUs = string.Join(",", configurableLineItems.Select(b => b.SKU)),
                GroupProductSKUs = groupProduct,
        }, publishProductModel.FirstOrDefault(x => x.SKU == cartLineItemModel.SKU), parentSKu, cartParameterModel.UserId.GetValueOrDefault(), 0, null, parentSKUProductName, cartParameterModel.ProfileId, catalogVersionId, publishProductModel, lstTaxClassSKUs, lstDownloadableProducts, configEntities);


            dtBindProductEndTime = DateTime.Now;
            BindProducttotalTime = dtBindProductEndTime.Subtract(dtBindProductStartTime);
            ZnodeLogging.LogMessage("Cart - NewBindProductDetails - Total Time=" + BindProducttotalTime.ToString(), "Cart", TraceLevel.Info);

            if (cartLineItemModel.OmsSavedCartLineItemId.Equals(0))
            {
                cartLineItem.PersonaliseValuesDetail = GetService<IZnodeOrderHelper>()?.GetPersonalizedQuoteValueCartLineItem(cartLineItemModel.OmsQuoteLineItemId);

                cartLineItem.GroupId = (cartLineItemModel.ParentOmsQuoteLineItemId.GetValueOrDefault() > 0)
                  ? cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.ParentOmsQuoteLineItemId).Select(x => x.GroupId).FirstOrDefault()
                  : cartLineItems.Where(x => x.OmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId).Select(x => x.GroupId).FirstOrDefault();
            }
            else
            {
                //This will have only child cart line item id. For simple product it will have the parent ID itself.
                int cartLineItemId = cartLineItemModel.OmsSavedCartLineItemId;

                if (IsNotNull(bundleLineItems) && bundleLineItems.Count > 0)
                {
                    int? cartItemId = bundleLineItems.FirstOrDefault(x => x.ParentOmsSavedCartLineItemId == cartLineItemId)?.OmsSavedCartLineItemId;
                    cartLineItemId = cartItemId > 0 ? cartItemId.GetValueOrDefault() : cartLineItemId;
                    List<PersonaliseValueModel> childLinePersonalizedValues = lstPersonalizedValues?.Where(x => x.OmsSavedCartLineItemId == cartLineItemId).ToList();
                    cartLineItem.PersonaliseValuesDetail = childLinePersonalizedValues?.Count > 0 ? childLinePersonalizedValues : lstPersonalizedValues?.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId).ToList();
                }
                else
                {
                    List<PersonaliseValueModel> childLinePersonalizedValues = lstPersonalizedValues?.Where(x => x.OmsSavedCartLineItemId == cartLineItemId).ToList();
                    cartLineItem.PersonaliseValuesDetail = childLinePersonalizedValues?.Count > 0 ? childLinePersonalizedValues : lstPersonalizedValues?.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId).ToList();
                }
                cartLineItem.GroupId = (cartLineItemModel.ParentOmsSavedCartLineItemId > 0)
                    ? cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.ParentOmsSavedCartLineItemId).Select(x => x.GroupId).FirstOrDefault()
                    : cartLineItems.Where(x => x.OmsSavedCartLineItemId == cartLineItemModel.OmsSavedCartLineItemId).Select(x => x.GroupId).FirstOrDefault();
            }

            cartLineItem.OrderLineItemRelationshipTypeId = cartLineItemModel.OrderLineItemRelationshipTypeId;

            if (!string.IsNullOrEmpty(cartLineItemModel.AutoAddon))
                cartLineItem.AutoAddonSKUs = cartLineItemModel.AutoAddon;

            BindCustomData(cartLineItemModel, cartLineItem);

            if (cartParameterModel?.OmsQuoteId > 0 && cartLineItem?.ProductType == ZnodeConstant.BundleProduct)
            {
                //update the Quantity, price and shipping as per the child entries.
                BindBundleProductData(cartLineItem, bundleLineItems, cartLineItemModel);
            }

            if (IsNotNull(cartLineItem.Product))
                base.ShoppingCartItems.Add(cartLineItem);

        }

        protected virtual void BindBundleProductData(ZnodeShoppingCartItem cartLineItem, List<AccountQuoteLineItemModel> bundleLineItems, AccountQuoteLineItemModel cartLineItemModel)
        {
            AccountQuoteLineItemModel associatedBundle = bundleLineItems?.FirstOrDefault(d => d.ParentOmsQuoteLineItemId == cartLineItemModel.OmsQuoteLineItemId && d.Quantity != 0 && d.Price != 0);
            if (IsNotNull(associatedBundle))
            {
                cartLineItem.Quantity = associatedBundle.Quantity;
                cartLineItem.CustomUnitPrice = associatedBundle.Price;
                cartLineItem.ShippingCost = IsNotNull(associatedBundle.ShippingCost) ? associatedBundle.ShippingCost.Value : cartLineItem.ShippingCost;
            }
        }

        protected virtual void BindProductDetailsV2(ZnodeShoppingCartItem znodeCartItem, PublishProductModel productModel, PublishProductModel publishProduct, string parentSKu = null, int userId = 0, int omsOrderId = 0, decimal? unitPrice = null, string parentSKUProductName = null, int profileId = 0, int catalogVersionId = 0, List<PublishProductModel> publishProducts = null, List<TaxClassRuleModel> lstTaxClassSKUs = null, List<ZnodePimDownloadableProduct> lstDownloadableProducts = null, List<PublishedConfigurableProductEntityModel> configEntities = null)
        {
            if (IsNotNull(publishProduct) && IsNotNull(znodeCartItem))
            {
                bool isGroupProduct = productModel.GroupProductSKUs.Count > 0;
                string countryCode = znodeCartItem.ShippingAddress?.CountryName;
                publishProduct.GroupProductSKUs = productModel.GroupProductSKUs;
                publishProduct.ConfigurableProductId = productModel.ParentPublishProductId;
                publishProduct.ParentPublishProductId = productModel.ParentPublishProductId;
                productModel.PublishProductId = publishProduct.PublishProductId;
                publishProduct.SEOUrl = productModel.ParentPublishProductId > 0 ? publishProducts.FirstOrDefault(x => x.PublishProductId == productModel.ParentPublishProductId)?.SEOUrl : publishProduct.SEOUrl;
                ZnodeProduct baseProduct = GetProductDetailsV2(publishProduct, this.PortalId.GetValueOrDefault(), productModel.LocaleId, znodeCartItem.ShippingAddress?.CountryName, isGroupProduct, parentSKu, userId, omsOrderId, parentSKUProductName, profileId, lstTaxClassSKUs, configEntities);
                znodeCartItem.ProductCode = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductCode)?.AttributeValues;
                znodeCartItem.ProductType = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code;
                znodeCartItem.Product = new ZnodeProductBase(baseProduct, znodeCartItem.ShippingAddress, unitPrice);
                znodeCartItem.Product.ZNodeAddonsProductCollection = GetZnodeProductAddons(productModel, productModel.PublishedCatalogId, productModel.LocaleId, baseProduct.AddOns, countryCode, userId, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Product.ZNodeBundleProductCollection = GetZnodeProductBundles(productModel,productModel.PublishedCatalogId, productModel.LocaleId, countryCode, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Product.ZNodeConfigurableProductCollection = GetZnodeProductConfigurables(productModel.ConfigurableProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, countryCode, productModel.ParentPublishProductId, userId, omsOrderId, profileId, productModel.Quantity.GetValueOrDefault(), catalogVersionId, publishProducts, lstTaxClassSKUs, configEntities);
                znodeCartItem.Product.ZNodeGroupProductCollection = GetZnodeProductGroup(productModel.GroupProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, countryCode, userId, omsOrderId, profileId, catalogVersionId);
                znodeCartItem.Quantity = GetProductQuantity(znodeCartItem, productModel.Quantity.GetValueOrDefault());
                znodeCartItem.ParentProductId = productModel.ParentPublishProductId;
                znodeCartItem.UOM = baseProduct.UOM;
                znodeCartItem.ParentProductSKU = znodeCartItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)
                                       ? znodeCartItem.ParentProductSKU : publishProduct.SKU;
                znodeCartItem.Product.SKU = !string.IsNullOrEmpty(parentSKu) && (!string.IsNullOrEmpty(productModel.ConfigurableProductSKUs) || isGroupProduct) ? parentSKu : publishProduct.SKU;
                znodeCartItem.Image = znodeCartItem.Product.ZNodeGroupProductCollection?.Count > 0 ? znodeCartItem.Product.ZNodeGroupProductCollection[0].Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValue : publishProduct.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                znodeCartItem.Product.Container = GetAttributeValueByCode(znodeCartItem, publishProduct, ZnodeConstant.ShippingContainer);
                znodeCartItem.Product.Size = GetAttributeValueByCode(znodeCartItem, publishProduct, ZnodeConstant.ShippingSize);
                znodeCartItem.Product.PackagingType = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PackagingType)?.SelectValues[0]?.Value;
                znodeCartItem.Product.DownloadableProductKey = GetProductKey(znodeCartItem.Product.SKU, znodeCartItem.Quantity, znodeCartItem.OmsOrderLineItemId, lstDownloadableProducts);
                znodeCartItem.AssociatedAddOnProducts = productModel.AssociatedAddOnProducts;
                znodeCartItem.Product.SelectedQuantity = znodeCartItem.Quantity;
                znodeCartItem.Product.OrdersDiscount = publishProduct.OrdersDiscount;
                znodeCartItem.Product.HST = publishProduct.HST;
                znodeCartItem.Product.PST = publishProduct.PST;
                znodeCartItem.Product.GST = publishProduct.GST;
                znodeCartItem.Product.VAT = publishProduct.VAT;
                znodeCartItem.Product.ImportDuty = publishProduct.ImportDuty;
                znodeCartItem.Product.SalesTax = publishProduct.SalesTax;
                znodeCartItem.Product.DiscountAmount = publishProduct.DiscountAmount;
               znodeCartItem.Product.SelectedQuantity = znodeCartItem.Quantity;
               SetInventoryData(znodeCartItem.Product, znodeCartItem, omsOrderId);
            }
        }
        protected virtual ZnodeProduct GetProductDetailsV2(PublishProductModel publishProduct, int portalId, int localeId, string countryCode = null, bool isGroupProduct = false, string parentSKU = "", int userId = 0, int omsOrderId = 0, string parentSKUProductName = null, int profileId = 0, List<TaxClassRuleModel> lstTaxClassSKUs = null, List<PublishedConfigurableProductEntityModel> configEntities = null, bool isBundleProduct = false)
        {
            if ((IsNull(publishProduct)))
                return null;

            publishProduct.ParentSEOCode = parentSKU;
            bool isProductHasRetailPrice = true;
            if (IsNull(publishProduct.RetailPrice))
            {
                isProductHasRetailPrice = false;
                GetParentProductPriceDetails(publishProduct, portalId, localeId, parentSKU, userId, profileId);
            }
            if (isBundleProduct && !isProductHasRetailPrice)
            {
                publishProduct.SKU = !String.IsNullOrEmpty(publishProduct.ConfigurableProductSKU) ? publishProduct.ConfigurableProductSKU : publishProduct.SKU;
            }

            List<AttributesSelectValuesModel> inventorySetting = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Vendor)?.SelectValues;
            string vendorCode = inventorySetting?.Count > 0 ? inventorySetting.FirstOrDefault().Code : string.Empty;

            ZnodeProduct product = new ZnodeProduct
            {
                ProductID = publishProduct.PublishProductId,
                SEOURL = publishProduct.SEOUrl,
                Name = isGroupProduct ? parentSKUProductName : publishProduct.Name,
                SKU = isGroupProduct ? parentSKU : publishProduct.SKU,
                SalePrice = publishProduct.SalesPrice,
                RetailPrice = publishProduct.RetailPrice.GetValueOrDefault(),
                QuantityOnHand = publishProduct.Quantity.GetValueOrDefault(),
                ZNodeTieredPriceCollection = GetZnodeProductTierPrice(publishProduct),
                TaxClassID = GetTaxClassBySKU(publishProduct.SKU, countryCode, lstTaxClassSKUs),
                AddOns = publishProduct.AddOns,
                IsPriceExist = isGroupProduct ? true : IsProductPriceExist(publishProduct.SalesPrice, publishProduct.RetailPrice),
                VendorCode = vendorCode,
                IsActive = publishProduct.IsActive,
                ProductCategoryIds = new int[] { publishProduct.ZnodeCategoryIds },
                AllowedTerritories = GetProductAttributeAllowedTerritoriesValue(publishProduct, ZnodeConstant.AllowedTerritories)
            };
            if (publishProduct.Attributes?.Count > 0)
            {
                product.AllowBackOrder = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.AllowBackOrdering);
                product.FreeShippingInd = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.FreeShipping);
                product.ShipSeparately = GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.ShipSeparately);
                product.MinQty = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MinimumQuantity);
                product.MaxQty = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MaximumQuantity);
                product.InventoryTracking = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues?.FirstOrDefault()?.Code ?? string.Empty;
                product.ShippingRuleTypeCode = publishProduct.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ShippingCost)?.FirstOrDefault()?.SelectValues?.FirstOrDefault()?.Code;
                product.BrandCode = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Brand)?.SelectValues?.FirstOrDefault()?.Code ?? string.Empty;
                product.Height = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Height);
                product.Width = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Width);
                product.Length = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Length);
                product.Weight = GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Weight);
                product.UOM = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.UOM)?.SelectValues?.FirstOrDefault()?.Value;
                product.Container = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingContainer)?.SelectValues?.FirstOrDefault()?.Value;
                product.Size = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingSize)?.SelectValues?.FirstOrDefault()?.Code;
                product.PackagingType = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PackagingType)?.SelectValues?.FirstOrDefault()?.Value;
            }

            //to apply product promotional price
            product.ApplyPromotion();
            //set configurable product attributes
            if (publishProduct?.ParentPublishProductId > 0)
            {
                List<PublishedConfigurableProductEntityModel> configEntity = configEntities?.Where(x => x.ZnodeProductId == publishProduct.ParentPublishProductId).ToList();

                if (IsNotNull(configEntity) && configEntity?.Count > 0)
                {
                    product.Description = string.IsNullOrEmpty(product.Description)
                                          ? (configEntity?.Count > 0
                                          ? string.Join("<br>", publishProduct?.Attributes?.Where(x => x.IsConfigurable && (configEntity?.FirstOrDefault()?.ConfigurableAttributeCodes?.Contains(x.AttributeCode)).GetValueOrDefault()).OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value).Distinct())
                                          : string.Join("<br>", publishProduct?.Attributes?.Where(x => x.IsConfigurable)?.OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value)?.Distinct()))
                                          : product.Description;
                }
                else
                {
                    product.Description = string.IsNullOrEmpty(product.Description) ? string.Join("<br>", publishProduct?.Attributes?.Where(x => x.IsConfigurable)?.OrderBy(x => x.DisplayOrder).Select(x => x.AttributeName + " - " + x.SelectValues?.FirstOrDefault()?.Value)?.Distinct()) : product.Description; ;
                }

            }
            //to set product attributes 
            SetProductAttributes(product, publishProduct);
            return product;
        }
        public virtual void AddtoShoppingBagV2(ShoppingCartModel shoppingCartItems, List<PublishProductModel> cartLineItemsProductData, int catalogVersionId = 0, List<TaxClassRuleModel> lstTaxClassSKUs = null, List<ZnodePimDownloadableProduct> lstDownloadableProducts = null, List<PublishedConfigurableProductEntityModel> configEntities=null, List<string> expands = null)
        {
            List<ZnodeShoppingCartItem> shoppingCartItemList = new List<ZnodeShoppingCartItem>();
            //Set Portal Id in Context Header, to avoid loop based calls.
            SetPortalIdInRequestHeader();
            List<ZnodeOmsSavedCartLineItem> cartDetails = orderHelper.GetParentSavedCartLineItem(shoppingCartItems.ShoppingCartItems.Select(x => x.ParentOmsSavedcartLineItemId.GetValueOrDefault())?.Distinct()?.ToList());
            foreach (ShoppingCartItemModel model in shoppingCartItems.ShoppingCartItems.OrderBy(c => c.GroupSequence))
            {
                if (string.IsNullOrEmpty(model.SKU))
                    return;
                
                BindShoppingCartDataV2(shoppingCartItems, cartLineItemsProductData, catalogVersionId, lstTaxClassSKUs, lstDownloadableProducts, configEntities, shoppingCartItemList, cartDetails, model);
            }

            //List of child product sku available in cart
            List<string> cartProductActualSkus = shoppingCartItemList?.Where(x => x.ParentProductSKU != null)?.Select(x => x.ParentProductSKU)?.ToList();

            if (IsNotNull(cartProductActualSkus))
            {
                List<InventorySKUModel> inventory = new List<InventorySKUModel>();
                //Validate inventory using mapped product SKU
                if (IsNotNull(expands) && expands.Contains(ZnodeConstant.Inventory))
                    inventory = publishProductHelper.GetInventoryBySKUs(cartProductActualSkus,shoppingCartItems.PortalId);

                //Update quantity on hand of the cart item
                shoppingCartItemList.Where(cartItem => cartItem.Product != null && cartItem.Product
                                                                                            ?.Attributes
                                                                                            ?.Any(o => o.AttributeCode.Equals(ZnodeConstant.ProductSKU, StringComparison.CurrentCultureIgnoreCase)) == true)
                                                                                            ?.ToList()
                .ForEach(znodeCartItem =>
                {
                    string productSku = znodeCartItem?.ParentProductSKU;
                    znodeCartItem.Product.QuantityOnHand = inventory.Any(sku => sku.SKU == productSku) ? inventory.FirstOrDefault(sku => sku.SKU == productSku).Quantity : 0;
                    base.ShoppingCartItems.Add(znodeCartItem);
                });
            }
        }
        private void BindShoppingCartDataV2(ShoppingCartModel shoppingCartItems, List<PublishProductModel> cartLineItemsProductData, int catalogVersionId, List<TaxClassRuleModel> lstTaxClassSKUs, List<ZnodePimDownloadableProduct> lstDownloadableProducts, List<PublishedConfigurableProductEntityModel> configEntities, List<ZnodeShoppingCartItem> shoppingCartItemList, List<ZnodeOmsSavedCartLineItem> cartDetails, ShoppingCartItemModel model)
        {
            ZnodeShoppingCartItem znodeCartItem = MapShoppingCartItemModel(model, model.ShippingAddress);
            MapShoppingCartOtherData(model, model.ShippingAddress, znodeCartItem);
            ZnodeOmsSavedCartLineItem parentCartItem = cartDetails?.FirstOrDefault(x => x.OmsSavedCartLineItemId == model.ParentOmsSavedcartLineItemId);
            string parentSKU = model.SKU;
            string parentSKUProductName = string.Empty;
            string configParentSKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.SKU : "";
            model.SKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : model?.GroupProducts?.Count > 0 ? model?.GroupProducts[0]?.Sku : model.SKU;

            if (IsNotNull(parentCartItem) && model?.GroupProducts?.Count > 0)
            {
                parentSKU = parentCartItem.SKU;
                parentSKUProductName = parentCartItem.ProductName;
            }
            if (string.IsNullOrEmpty(parentSKUProductName))
                parentSKUProductName = model?.ProductName;

            //If Quote Id is greater than zero, bind ShoppingCartItemModel properties to ZNodeShoppingCartItem.
            if (model.OmsQuoteId > 0)
                BindShoppingCartItemModel(model, znodeCartItem, parentSKU);
            BindProductDetailsV2(znodeCartItem, GetPublishProductModel(model, shoppingCartItems.LocaleId, shoppingCartItems.PublishedCatalogId, shoppingCartItems.OmsOrderId.GetValueOrDefault(), configParentSKU), cartLineItemsProductData.FirstOrDefault(x => x.SKU == model.SKU), parentSKU, shoppingCartItems.UserId.GetValueOrDefault(), shoppingCartItems.OmsOrderId.GetValueOrDefault(), model.CustomUnitPrice, parentSKUProductName, shoppingCartItems.ProfileId.GetValueOrDefault(), catalogVersionId, cartLineItemsProductData, lstTaxClassSKUs, lstDownloadableProducts, configEntities);

            znodeCartItem.PromotionalPrice = model.PromotionalPrice;
            znodeCartItem.PersonaliseValuesDetail = model.PersonaliseValuesDetail;
            znodeCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();

            shoppingCartItemList.Add(znodeCartItem);
        }
        protected virtual int GetTaxClassBySKU(string sku, string countryCode, List<TaxClassRuleModel> lstTaxClassSKUs)
        {
            int? taxClassId = lstTaxClassSKUs?.FirstOrDefault(x => x.SKU == sku && (x.DestinationCountryCode == countryCode || x.DestinationCountryCode == null))?.TaxClassId;

            return taxClassId ?? 0;
        }
        //Set Portal Id in Request header, to avoid multiple loop based calls, in Promotion helper by using ZnodeConfigManager SiteConfig.
        public virtual void SetPortalIdInRequestHeader()
        {
            //if site config is null then get the portal id from domain
            int portalId = ZnodeConfigManager.SiteConfig != null ? ZnodeConfigManager.SiteConfig.PortalId : GetPortalId();
            if (!Equals(HttpContext.Current, null) && !Equals(HttpContext.Current.Request, null) && !Equals(HttpContext.Current.Request.Headers, null))
                HttpContext.Current.Request.Headers.Add("Znode-Cart-PortalId", portalId.ToString());
        }
        //Get the saved Cart Line Item Ids, to avoid loop based call.
        protected virtual List<int?> GetSavedCartLineItemIds(List<AccountQuoteLineItemModel> cartLineItems)
        {
            List<int?> lstDDD = new List<int?>();

            cartLineItems?.ForEach(item =>
            {
                //This will have only child cart line item id. For simple product it will have the parent ID itself.
                lstDDD.Add(item.OmsSavedCartLineItemId);
            });
            return lstDDD;
        }
        public virtual int GetHeaderPortalId()
        {
            const string headerCartPortalId = "Znode-Cart-PortalId";
            int portalId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerCartPortalId], out portalId);
            return portalId > 0 ? portalId : ZnodeConfigManager.SiteConfig != null ? ZnodeConfigManager.SiteConfig.PortalId : GetPortalId();
        }

        //Get Published product by SKU
        protected virtual ZnodePublishProductEntity GetPublishProductBySKU(string sku, int publishedCatalogId, int localeId, int? catalogVersionId, int omsOrderId = 0)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                List<ZnodePublishProductEntity> publishProducts = _publishProductEntity.Table.Where(x => x.SKULower == sku.ToLower() && x.ZnodeCatalogId == publishedCatalogId && x.LocaleId == localeId && x.VersionId == catalogVersionId).ToList();
                return omsOrderId < 1 ? publishProducts.FirstOrDefault(x => x.IsActive) : publishProducts.FirstOrDefault();
            }
            return null;
        }

        //Get published products by list of SKUs
        public virtual List<ZnodePublishProductEntity> GetPublishProductBySKUs(List<string> sku, int publishedCatalogId, int localeId, int? catalogVersionId = null)
        {
            FilterCollection filters = new FilterCollection();

            if (sku?.Count > 0)
                filters.Add(FilterKeys.SKU, FilterOperators.In, string.Join(",", sku.Select(x => $"\"{x}\"")));
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString());
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, publishedCatalogId.ToString());
            if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                filters.Add(FilterKeys.VersionId, FilterOperators.Equals, catalogVersionId.Value.ToString());

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
            List<ZnodePublishProductEntity> publishProducts = _publishProductEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues).ToList();

            return publishProducts;
        }
        #region Quote 
        //to set Configurable/group product quantity for quote product
        protected virtual void SetConfigurableOrGroupProductQuantityForQuote(ShoppingCartItemModel item, List<ZnodeOmsQuoteLineItem> childItems, List<PublishedProductEntityModel> productList)
        {
            if (item.Quantity == 0)//to set Configurable product quantity
            {
                if (Convert.ToDecimal(childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)?.Select(s => s.Quantity)) > 0)
                {
                    SetChildItemDataForQuote(item, (int)ZnodeCartItemRelationshipTypeEnum.Configurable, childItems, productList);
                }
                else
                {
                    SetChildItemDataForQuote(item, (int)ZnodeCartItemRelationshipTypeEnum.Group, childItems, productList);
                }
            }
            else if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)))
            {
                item.AddOnLineItemId = childItems.FirstOrDefault().OmsQuoteLineItemId;
                SetChildItemDataForQuote(item, (int)ZnodeCartItemRelationshipTypeEnum.AddOns, childItems, productList);
            }
        }

        //to set Bundle product quantity for quote product
        protected virtual void SetBundleProductQuantityForQuote(ShoppingCartItemModel item, ZnodeOmsQuoteLineItem childItem, List<PublishedProductEntityModel> productList)
        {
            SetInventoryBundleData(item);
            bool isActive = true;
            if (item.BundleProducts.Count > 0)
            {                
                foreach (var BundleProduct in item.BundleProducts)
                {
                    if (productList?.FirstOrDefault(x => x.SKU == BundleProduct.SKU)?.IsActive == false)
                    {
                        isActive = false;
                        break;
                    }
                }                
            }
            item.IsActive = isActive;
            item.Quantity = childItem.Quantity.GetValueOrDefault();
            item.ShippingCost = childItem.ShippingCost.GetValueOrDefault();
            //item.ChildProductId = child.OmsQuoteLineItemId;
        }

        protected virtual void SetChildItemDataForQuote(ShoppingCartItemModel item, int relationTypeId, List<ZnodeOmsQuoteLineItem> childItems, List<PublishedProductEntityModel> productList)
        {
            ZnodeOmsQuoteLineItem child = childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == relationTypeId);
            if (IsNotNull(child))
            {
                PublishedProductEntityModel childProduct = productList?.FirstOrDefault(x => x.SKU == child.SKU);
                SetInventoryData(item, childProduct);
                item.IsActive = (productList?.FirstOrDefault(x => x.SKU == child.SKU)?.IsActive).GetValueOrDefault();
                item.Quantity = child.Quantity.GetValueOrDefault();
                item.ChildProductId = child.OmsQuoteLineItemId;
            }
        }

        //to calculate unit price and extended price for quote lineitems
        protected virtual void CalculateLineItemPriceForQuote(ShoppingCartItemModel lineItem, List<ZnodeOmsQuoteLineItem> childItems)
        {
            if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)) || 
                IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))||
                IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles))
                )
            {
                lineItem.UnitPrice = lineItem.UnitPrice > 0 ? lineItem.UnitPrice :
                    childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable
                    || x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group).Sum(x => x.Price).GetValueOrDefault();
            }

            if (IsNotNull(childItems.FirstOrDefault(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)))
            {
                lineItem.UnitPrice += childItems
                                    .Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                                    && x.ParentOmsQuoteLineItemId == lineItem.OmsQuoteLineItemId
                                    ).Sum(x => x.Price).GetValueOrDefault();
            }
            lineItem.ExtendedPrice = lineItem.UnitPrice * lineItem.Quantity;
            //to set externalid of line item
            lineItem.ExternalId = Guid.NewGuid().ToString();
        }

        //to calculate unit price and extended price for quote lineitems
        protected virtual void CalculateBundleLineItemPriceForQuote(ShoppingCartItemModel lineItem, List<ZnodeOmsQuoteLineItem> childItems)
        {
            if (IsNotNull(childItems))
            {
                var record = childItems.FirstOrDefault(x => x.ParentOmsQuoteLineItemId == lineItem.OmsQuoteLineItemId && x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles && x.Price != null);
                lineItem.UnitPrice = record != null ? record.Price.GetValueOrDefault() : 0;
                lineItem.ExtendedPrice = lineItem.UnitPrice * lineItem.Quantity;
                //to set externalid of line item
                lineItem.ExternalId = Guid.NewGuid().ToString();
            }
        }

        //to set product type data for shoppingcart line item 
        protected virtual void SetAssociateQuoteProductType(ShoppingCartItemModel lineItem, List<ZnodeOmsQuoteLineItem> childItems)
        {
            lineItem.AddOnProductSKUs = string.Join(",", childItems
                                        .Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                                         && x.ParentOmsQuoteLineItemId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.SKU));
            lineItem.BundleProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles
                                         && x.ParentOmsQuoteLineItemId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.SKU));
            lineItem.ConfigurableProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable && x.OmsQuoteLineItemId == lineItem.OmsOrderLineItemsId).AsEnumerable().Select(b => b.SKU));
            lineItem.GroupProducts = childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group
                                                   && x.OmsQuoteLineItemId == lineItem.OmsOrderLineItemsId)?.ToModel<AssociatedProductModel>()?.ToList();
        }

        //Set Parent Product Name for Group Product
        public virtual void SetGroupAndConfigurableParentProductDetailsOfQuote(List<ZnodeOmsQuoteLineItem> parentDetails, ZnodeOmsQuoteLineItem lineItem, ShoppingCartItemModel item)
        {
            if ((item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group) && item.GroupProducts?.Count > 0) || (item.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable) && !string.IsNullOrEmpty(item.ConfigurableProductSKUs)))
            {
                string parentName = parentDetails?.FirstOrDefault(p => p.OmsQuoteLineItemId == lineItem.ParentOmsQuoteLineItemId)?.ProductName;
                if (!string.IsNullOrEmpty(parentName))
                {
                    item.ProductName = parentName;
                }
                //Set Parent SKU for group Product.
                item.SKU = parentDetails?.FirstOrDefault(p => p.OmsQuoteLineItemId == lineItem.ParentOmsQuoteLineItemId)?.SKU;
            }
        }


        //Get the Quote Line Item Ids, to avoid loop based call.
        private List<int?> GetQuoteCartLineItemIds(List<ZnodeOmsQuoteLineItem> cartLineItems)
        {
            List<int?> cartLineItemIds = new List<int?>();

            cartLineItems?.ForEach(item =>
            {
                //This will have only child cart line item id. For simple product it will have the parent ID itself.
                cartLineItemIds.Add(item.OmsQuoteLineItemId);
            });
            return cartLineItemIds;
        }

        protected virtual decimal GetPerQtyShippingCost(decimal lineItemShippingCost, decimal totalLineItemQuantity )
        {
            if (lineItemShippingCost > 0 && totalLineItemQuantity > 0)
                return lineItemShippingCost / totalLineItemQuantity;
            else
                return 0;
        }

        #endregion Quote

        // To check is free shipping applicable or not.
        protected virtual bool CheckFreeShipping()
        {
            bool? isFreeShipingApplied = false;
            foreach (ZnodeShoppingCartItem shoppingCartItem in this?.ShoppingCartItems)
            {
                isFreeShipingApplied = shoppingCartItem?.Product?.FreeShippingInd;
                if (isFreeShipingApplied == false)
                    break;
            }

            this.FreeShipping = isFreeShipingApplied.GetValueOrDefault();

            if (this.FreeShipping == true)
                this.OrderLevelShipping = 0;

            return isFreeShipingApplied.GetValueOrDefault();
        }
        //To set AllowBackOrder and TrackInventoryInd as per inventory setting Code
        private void SetProductAllowBackOrderAndTrackInventory(string inventorySettingCode, ZnodeProductBase product)
        {
            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
            {
                product.TrackInventoryInd = false;
            }
            else if (string.Equals(ZnodeConstant.AllowBackOrdering, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
            {
                product.AllowBackOrder = true;
                product.TrackInventoryInd = false;
            }
            else
            {
                product.TrackInventoryInd = true;
            }
        }
        #endregion

        //To set the flag of inventory for child bundle product
        protected virtual void SetBundleInventoryData(List<AssociatedPublishedBundleProductModel> associatedBundleProducts)
        {
            foreach (var childBundleProduct in associatedBundleProducts)
            {
                string outOfStockOptionCode = childBundleProduct?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues?.FirstOrDefault()?.Code;
                if (string.Equals(ZnodeConstant.DisablePurchasing, outOfStockOptionCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    childBundleProduct.TrackInventory = true;
                }

                if (string.Equals(ZnodeConstant.AllowBackOrdering, outOfStockOptionCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    childBundleProduct.AllowBackOrder = true;
                    childBundleProduct.TrackInventory = false;
                }
            }
        }

        // Map applied voucher response to model.
        protected void CheckInvalidVoucher()
        {
            if (Vouchers.Count == 0 && !IsAllVoucherRemoved)
                AddUserVouchers();

            if (Vouchers?.Count > 0 && Total > 0)
            {
                ClearAllAppliedVouchers();
                foreach (ZnodeVoucher voucher in Vouchers)
                {
                    string invalidGiftCard = Admin_Resources.ErrorInvalidVoucher;
                    voucher.CultureCode = CultureCode;

                    MapAppliedVoucherResponce(voucher, invalidGiftCard, 0, 0, false, false);
                }
            }
        }

    }
}



