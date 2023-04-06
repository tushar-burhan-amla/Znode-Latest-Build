using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public class ZNodeShoppingCartV2 : ZnodeShoppingCart
    {
        #region Private Variables        
        private readonly IPublishProductHelper publishProductHelper;
        private readonly ZnodeOrderHelperV2 orderHelper;
        #endregion

        #region Constructor
        public ZNodeShoppingCartV2() : base()
        {
            publishProductHelper = ZnodeDependencyResolver.GetService<IPublishProductHelper>();
            orderHelper = new ZnodeOrderHelperV2();
        }
        #endregion

        //save the shoppingcart items in the database.
        public int SaveV2(ShoppingCartModel shoppingCart)
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

                //Save all shopping cart line items.
                AddToCartModel addToCartModel = new AddToCartModel
                {
                    PortalId = shoppingCart.PortalId,
                    CookieMappingId = shoppingCart.CookieMappingId,
                    Coupons = shoppingCart.Coupons,
                    LocaleId = shoppingCart.LocaleId,
                    PublishedCatalogId = shoppingCart.PublishedCatalogId,
                    ShoppingCartItems = shoppingCart.ShoppingCartItems,
                    UserDetails = shoppingCart.UserDetails,
                    UserId = shoppingCart.UserId,
                    Quantity = shoppingCart.ShoppingCartItems.Sum(x => x.Quantity),
                    ZipCode = shoppingCart.ShippingAddress?.PostalCode
                };
                orderHelper.SaveAllCartLineItemsInDatabase(savedCartId, addToCartModel);

                return cookieMappingId;
            }
            return 0;
        }
    }
}
