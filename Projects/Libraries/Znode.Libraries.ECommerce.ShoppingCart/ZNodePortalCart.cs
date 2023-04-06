using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Engine.Shipping;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Libraries.ECommerce.ShoppingCart
{
    [Serializable()]
    public class ZnodePortalCart : ZnodeShoppingCart, IZnodePortalCart
    {
        #region Constructor
        public ZnodePortalCart() : base() { }

        public ZnodePortalCart(int portalId, int addressId, ZnodeGenericCollection<ZnodeShoppingCartItem> shoppingCartItems) : base()
        {
            PortalID = portalId;
            PortalId = portalId;
            ShoppingCartItems = GetAddressCartItems(addressId, portalId, shoppingCartItems);
        }
        #endregion

        List<IZnodeMultipleAddressCart> _addressCarts = GetServices<IZnodeMultipleAddressCart>().ToList();

        #region Properties
        public int PortalID { get; set; }

        // Get Address based cart items.
        public virtual List<IZnodeMultipleAddressCart> AddressCarts
        {
            get
            {
                var noAddressCarts = !_addressCarts.Any();
                var countNotEqual = _addressCarts.Count() != ShoppingCartItems.Cast<ZnodeShoppingCartItem>()
                                           .SelectMany(x => x.OrderShipments.Select(y => y.AddressID))
                                           .Distinct()
                                           .Count();

                var quantityNotEqual = _addressCarts.Sum(x => x.ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(s => s.Quantity)) != ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(s => s.Quantity);

                if (noAddressCarts || countNotEqual || quantityNotEqual)
                {
                    var allOrderShipments = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().SelectMany(x => x.OrderShipments).ToList();

                    var userAddresses = this.UserAddress;

                    _addressCarts = allOrderShipments.GroupBy(x => new { x.AddressID, x.ShippingID },
                                                             (orderShipmentKey, orderShipmentKeyGroup) => GetService<IZnodeMultipleAddressCart>(
                          new ZnodeNamedParameter("addressID", orderShipmentKey.AddressID),
                      new ZnodeNamedParameter("shipping", GetShippingMethod(orderShipmentKey.ShippingID, orderShipmentKeyGroup.FirstOrDefault())),
                       new ZnodeNamedParameter("shoppingCartItems", GetAddressCartItems(orderShipmentKey.AddressID, userAddresses))))
                                                               .ToList();
                }
                return _addressCarts;
            }
        }

        private decimal? shippingCost = null;

        public override decimal ShippingCost
        {
            get
            {
                if (IsNotNull(shippingCost))
                    return shippingCost.GetValueOrDefault();

                var addressCarts = AddressCarts.Where(x => x.Shipping != null).ToList();
                decimal totalShippingCost = addressCarts.Sum(x => x.ShippingCost);
                return !Equals(CustomShippingCost, null) ? CustomShippingCost.GetValueOrDefault() : totalShippingCost;
            }

            set
            {
                shippingCost = value;
            }
        }

        private decimal? taxCost = null;

        //Gets the totalTaxCost 
        public override decimal TaxCost
        {
            get
            {
                if (IsNotNull(taxCost))
                    return taxCost.GetValueOrDefault();

                decimal totalTaxCost = AddressCarts.Sum(x => x.OrderLevelTaxes);
                return !Equals(CustomTaxCost, null) ? CustomTaxCost.GetValueOrDefault() : totalTaxCost;
            }

            set
            {
                taxCost = value;
            }
        }


        private decimal? subTotal = null;

        // Gets the total cost of items in the shopping cart before shipping and taxes
        public override decimal SubTotal
        {
            get
            {
                if (IsNotNull(subTotal))
                    return subTotal.GetValueOrDefault();

                return _addressCarts.SelectMany(x => x.ShoppingCartItems.Cast<ZnodeShoppingCartItem>()).Sum(item => item.ExtendedPrice);
            }
            
            set
            {
                subTotal = value;
            }
        }



        private decimal? orderTotalWithoutVoucher = null;

        // Gets the total cost before voucher consideration and after shipping, taxes and promotions
        public override decimal OrderTotalWithoutVoucher
        {

            get
            {
                if (IsNotNull(orderTotalWithoutVoucher))
                    return orderTotalWithoutVoucher.GetValueOrDefault();

                return (SubTotal - Discount) + ShippingCost + ImportDuty + ShippingHandlingCharges + ReturnCharges + (!Equals(CustomTaxCost, null) ? 0 : TaxCost) - CSRDiscount + GetAdditionalPrice() - Shipping.ShippingDiscount;
            }

            set
            {
                orderTotalWithoutVoucher = value;
            }
        }

        private decimal? total = null;

        // Gets Order total which is Amount to be paid shown after voucher consideration
        public override decimal Total
        {

            get
            {
                if (IsNotNull(total))
                    return total.GetValueOrDefault();

                return OrderTotalWithoutVoucher - GiftCardAmount;
            }

            set
            {
                total = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates final pricing, shipping and taxes in the cart.
        /// </summary>
        public override void Calculate() => Calculate(null);

        /// <summary>
        /// Calculates final pricing, shipping and taxes in the cart.
        /// </summary>
        public override void Calculate(int? profileId, bool isCalculateTaxAndShipping = true, bool isCalculatePromotionAndCoupon = true)
        {
            // Clear previous messages
            this._ErrorMessage = new StringBuilder();

            // Promotions
            if (isCalculatePromotionAndCoupon)
            {
                IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", this), new ZnodeNamedParameter("profileId", profileId));
                cartPromoManager.Calculate();
            }

            var addressShippingPayments = _addressCarts.Select(x => new { x.AddressID, x.AddressCartID, x.Shipping, x.Payment }).ToList();

            int? portalId = _addressCarts?.Select(s => s.PortalId)?.FirstOrDefault();

            _addressCarts = GetServices<IZnodeMultipleAddressCart>().ToList();

            addressShippingPayments.ForEach(x =>
            {
                var item = AddressCarts.FirstOrDefault(y => y.AddressID == x.AddressID);
                if (!Equals(item, null))
                {
                    item.AddressCartID = x.AddressCartID;
                    item.Shipping = x.Shipping;
                    item.Payment = x.Payment;
                    item.PortalId = portalId;
                    item.Coupons = this.Coupons;
                    item.UserId = this.UserId;
                    item.CurrencyCode = this.CurrencyCode;
                    item.CultureCode = this.CultureCode;
                    item.OrderId = this.OrderId;
                    item.ProfileId = IsNull(profileId) ? this.ProfileId : profileId; 
                    item.PublishStateId = this.PublishStateId;
                    item.IsAllowWithOtherPromotionsAndCoupons = this.IsAllowWithOtherPromotionsAndCoupons;
                    item.ReturnItemList = this.ReturnItemList;
                    item.IsShippingRecalculate = this.IsShippingRecalculate;
                    item.InvalidOrderLevelShippingDiscount = this?.InvalidOrderLevelShippingDiscount;
                    item.InvalidOrderLevelShippingPromotion = this?.InvalidOrderLevelShippingPromotion;
                    item.IsShippingDiscountRecalculate = this.IsShippingDiscountRecalculate;
                    item.IsCalculateTaxAfterDiscount = this.IsCalculateTaxAfterDiscount;
                    item.CSRDiscountAmount = this.CSRDiscountAmount;
                    item.IsOldOrder = this.IsOldOrder;
                    item.OrderLevelShipping = this.OrderLevelShipping;
                    item.CustomShippingCost = this.CustomShippingCost;
                    item.IsQuoteToOrder = this.IsQuoteToOrder;
                    item.IsRemoveShippingDiscount = this.IsRemoveShippingDiscount;
                    item.IsPendingOrderRequest = this.IsPendingOrderRequest;
                    item.Calculate();
                }
            });

            //to apply csr discount amount
            if (CSRDiscountAmount > 0)
                AddCSRDiscount(CSRDiscountAmount);

            GiftCardAmount = 0;

            if (IsCalculateVoucher && !IsPendingOrderRequest)
                AddVouchers(this.OrderId);
           
        }

        #endregion

        #region Private Methods

        protected virtual ZnodeShippings GetShippingMethod(int shippingId, ZnodeOrderShipment orderShipment)
        {

            if (shippingId > 0 && !Equals(orderShipment, null))
            {
                return new ZnodeShippings()
                {
                    ShippingID = shippingId,
                    ShippingName = orderShipment.ShippingName
                };
            }
            return new ZnodeShippings();

        }

        protected virtual ZnodeGenericCollection<ZnodeShoppingCartItem> GetAddressCartItems(int addressId, UserAddressModel userAddress)
        {
            List<AddressModel> addresses = userAddress?.Addresses ?? null;

            if (IsNotNull(addresses))
            {
                AddressModel address = addresses.FirstOrDefault(x => x.AddressId == addressId);
                var items = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Where(y => y.OrderShipments.Any(z => z.AddressID == addressId)).Select(c => c.Clone()).ToList();
                ZnodeGenericCollection<ZnodeShoppingCartItem> returnItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
                items.ForEach(y =>
                {
                    y.Product.AddressToShip = address;
                    y.Quantity = y.OrderShipments.Where(z => z.AddressID == addressId).Sum(s => s.Quantity);
                    returnItems.Add(y);
                });
                return returnItems;
            }

            return new ZnodeGenericCollection<ZnodeShoppingCartItem>();
        }
        #endregion
    }
}

