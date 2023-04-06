using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class CheckoutMap
    {
        public static IZnodeCheckout ToZnodeCheckout(UserAddressModel user, ZnodeShoppingCart shoppingCart)
        {
            int? portalId = shoppingCart.PortalId;
            var znodeCheckout = ZnodeDependencyResolver.GetService<IZnodeCheckout>();

            znodeCheckout.UserAccount = user;
            znodeCheckout.ShoppingCart = shoppingCart.PortalCarts.FirstOrDefault();

            if (IsNotNull(znodeCheckout.ShoppingCart))
            {
                znodeCheckout.ShoppingCart.UserAddress = user;
                znodeCheckout.ShoppingCart.PortalId = portalId;
                znodeCheckout.ShoppingCart.PortalID = portalId.GetValueOrDefault();
                znodeCheckout.ShoppingCart.Token = shoppingCart.Token;
                znodeCheckout.ShoppingCart.PayerId = shoppingCart.PayerId;
                znodeCheckout.ShoppingCart.OrderId = shoppingCart.OrderId;
                znodeCheckout.ShoppingCart.ReturnItemList = shoppingCart.ReturnItemList;
                znodeCheckout.ShoppingCart.OrderDate = shoppingCart.OrderDate;
                znodeCheckout.ShoppingCart.IsAllowWithOtherPromotionsAndCoupons = shoppingCart.IsAllowWithOtherPromotionsAndCoupons;
                znodeCheckout.ShoppingCart.PublishStateId = shoppingCart.PublishStateId;
                znodeCheckout.ShoppingCart.ProfileId = shoppingCart.ProfileId;
                znodeCheckout.ShoppingCart.InvalidOrderLevelShippingDiscount = shoppingCart?.InvalidOrderLevelShippingDiscount;
                znodeCheckout.ShoppingCart.InvalidOrderLevelShippingPromotion = shoppingCart?.InvalidOrderLevelShippingPromotion;
                znodeCheckout.ShoppingCart.IsShippingDiscountRecalculate = shoppingCart.IsShippingDiscountRecalculate;
                znodeCheckout.ShoppingCart.IsCalculateTaxAfterDiscount = shoppingCart.IsCalculateTaxAfterDiscount;
                znodeCheckout.ShoppingCart.IsOldOrder = shoppingCart.IsOldOrder;
                znodeCheckout.ShoppingCart.OrderLevelShipping = shoppingCart.OrderLevelShipping;
                if (IsNotNull(shoppingCart.PersonaliseValuesList))
                    znodeCheckout.ShoppingCart.PersonaliseValuesList = shoppingCart.PersonaliseValuesList;
            }

            return znodeCheckout;
        }
    }
}
