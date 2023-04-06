using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Core.ViewModels;
using Znode.Engine.Exceptions;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Agents;
using Znode.Libraries.Klaviyo.Helper;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Engine.Klaviyo.IClient;

namespace Znode.Engine.WebStore.Agents
{
    public class CartAgent : BaseAgent, ICartAgent
    {
        #region protected member
        protected readonly IShoppingCartClient _shoppingCartsClient;
        protected readonly IPublishProductClient _publishProductClient;
        protected readonly IAccountQuoteClient _accountQuoteClient;
        protected readonly IUserClient _userClient;
        protected List<UpdatedProductQuantityModel> updatedProducts = new List<UpdatedProductQuantityModel>();
        #endregion

        #region Constructor
        public CartAgent(IShoppingCartClient shoppingCartsClient, IPublishProductClient publishProductClient, IAccountQuoteClient accountQuoteClient, IUserClient userClient)
        {
            _shoppingCartsClient = GetClient<IShoppingCartClient>(shoppingCartsClient);
            _publishProductClient = GetClient<IPublishProductClient>(publishProductClient);
            _accountQuoteClient = GetClient<IAccountQuoteClient>(accountQuoteClient);
            _userClient = GetClient<IUserClient>(userClient);
        }
        #endregion

        #region Public methods

        /// <summary>
        /// This method is used for creating a single shopping cart from the list of cartitems and passing it to the shoppingcart client.
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        public virtual CartViewModel CreateCart(List<CartItemViewModel> cartItems)
        {
            if (IsNotNull(cartItems))
            {
                //Get shopping cart data from session.
                ShoppingCartModel shoppingCartModel = IsNull(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)) && GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems?.Count > 0 ? GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) : new ShoppingCartModel();
                //Bind cartitems to shoppingcart.
                AddCartItemsToShoppingCart(cartItems, shoppingCartModel);

                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());

                ShoppingCartModel shoppingCart = _shoppingCartsClient.CreateCart(shoppingCartModel);

                //Bind the last CartItemViewModel properties to ShoppingCartModel.
                BindCartItemData(cartItems[cartItems.Count - 1], shoppingCart);

                //Assign values which are required for the estimated shipping calculation.
                shoppingCart = GetEstimatedShippingDetails(shoppingCart);

                SaveInCookie(WebStoreConstants.CartCookieKey, shoppingCart.CookieMappingId.ToString(), ZnodeConstant.MinutesInAYear);

                SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCart);
                //To clear the session for cartcount session key.
                ClearCartCountFromSession();

                return shoppingCart?.ToViewModel<CartViewModel>();
            }
            return null;
        }


        //Create Shopping Cart.
        public virtual CartViewModel CreateCart(CartItemViewModel cartItem)
        {
            if (IsNotNull(cartItem))
            {
                //Get shopping cart data from session.

                //Bind Portal related data to Shopping cart model.
                ShoppingCartModel shoppingCartModel = IsNull(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)) && GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems?.Count > 0 ? GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) : new ShoppingCartModel();
                //shoppingCartModel.Coupons = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.Coupons?.Count > 0 ? GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.Coupons : new List<CouponModel>();
                shoppingCartModel = GetShoppingCart(cartItem, shoppingCartModel);
                //Create new cart.
                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                ShoppingCartModel shoppingCart = _shoppingCartsClient.CreateCart(shoppingCartModel);

                //Bind CartItemViewModel properties to ShoppingCartModel.
                BindCartItemData(cartItem, shoppingCart);

                //assign values which are required for the estimated shipping calculation.
                shoppingCart = GetEstimatedShippingDetails(shoppingCart);

                SaveInCookie(WebStoreConstants.CartCookieKey, shoppingCart.CookieMappingId.ToString(),ZnodeConstant.MinutesInAYear);

                shoppingCart?.ShoppingCartItems.Where(x => x.SKU == cartItem.SKU).Select(x => { x.ProductType = cartItem.ProductType; return x; })?.ToList();

                SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCart);
                //To clear the session for CartCount session key.
                ClearCartCountFromSession();

                return shoppingCart?.ToViewModel<CartViewModel>();
            }
            return null;
        }

        //Add product in cart.
        public virtual AddToCartViewModel AddToCartProduct(AddToCartViewModel cartItem)
        {
            if (IsNotNull(cartItem))
            {
                try
                {
                    cartItem.CookieMappingId = GetFromCookie(WebStoreConstants.CartCookieKey);
                    cartItem.PublishedCatalogId = GetCatalogId().GetValueOrDefault();
                    ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems?.Count > 0 ? GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) : new ShoppingCartModel();

                    cartItem.Coupons = shoppingCartModel?.Coupons?.Count > 0 ? shoppingCartModel?.Coupons : new List<CouponModel>();

                    //Create new cart.
                    cartItem = _shoppingCartsClient.AddToCartProduct(GetShoppingCartValues(cartItem)?.ToModel<AddToCartModel>()).ToViewModel<AddToCartViewModel>();

                    SaveInCookie(WebStoreConstants.CartCookieKey, cartItem.CookieMappingId.ToString(),ZnodeConstant.MinutesInAYear);

                    cartItem?.ShoppingCartItems.Where(x => x.SKU == cartItem.SKU).Select(x => { x.ProductType = cartItem.ProductType; return x; })?.ToList();

                    cartItem.ShippingId = (shoppingCartModel?.ShippingId).GetValueOrDefault();

                    shoppingCartModel = MapAddToCartToShoppingCart(cartItem, shoppingCartModel);

                    SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCartModel);

                    if (cartItem.HasError == false)
                        ClearCartCountFromSession();

                    // Created the task to post the data to klaviyo
                    HttpContext httpContext = HttpContext.Current;

                    //for guest user no data has been set to Klaviyo.
                    if (PortalAgent.CurrentPortal.IsKlaviyoEnable && cartItem.UserId > 0)
                        Task.Run(() =>
                        {
                            PostDataToKlaviyo(cartItem, httpContext);
                        });
                    
                   return cartItem;
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                    return (AddToCartViewModel)GetViewModelWithErrorMessage(new AddToCartViewModel(), ex.ErrorMessage);
                }
            }
            return null;
        }

        // Get Cart method to check Session or Cookie to get the existing shopping cart.
        public virtual CartViewModel GetCart(bool isCalculateTaxAndShipping = true, bool isCalculateCart = true, bool isCalculatePromotionAndCoupon = true)
        {
            ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                                     GetCartFromCookie();

            if (IsNull(shoppingCartModel))
            {
                return new CartViewModel()
                {
                    HasError = true,
                    ErrorMessage = WebStore_Resources.OutofStockMessage
                };
            }

            if (IsNotNull(shoppingCartModel) && (IsNull(shoppingCartModel?.ShoppingCartItems) || shoppingCartModel?.ShoppingCartItems?.Count == 0))
            {
                List<ShoppingCartItemModel> shoppingartItems = GetCartFromCookie()?.ShoppingCartItems;
                shoppingCartModel.ShoppingCartItems = (shoppingartItems?.Count > 0) ? shoppingartItems : new List<ShoppingCartItemModel>(); ;
            }

            if (shoppingCartModel.ShoppingCartItems?.Count == 0)
            {
                return new CartViewModel();
            }

            if (!(HttpContext.Current.User.Identity.IsAuthenticated) && shoppingCartModel?.ShippingOptions?.Count > 0)
            {
                shoppingCartModel.Shipping = new OrderShippingModel() { ShippingId = shoppingCartModel.ShippingId > 0 ? shoppingCartModel.ShippingId : 0, ShippingCountryCode = WebStoreConstants.ShippingCountryCode };
                shoppingCartModel.Payment = new PaymentModel();
                RemoveTaxFromCart(shoppingCartModel);
            }
            else
            {
                //Remove Shipping and Tax calculation From Cart.
                RemoveShippingTaxFromCart(shoppingCartModel);

                //To remove the tax getting calculated when we come back from checkout page to cart page
                shoppingCartModel.ShippingAddress = new AddressModel();
                shoppingCartModel.ShippingAddress.PostalCode = GetFromSession<String>(ZnodeConstant.ShippingEstimatedZipCode);
            }

            shoppingCartModel.ShippingAddress.CountryName = "us";
            shoppingCartModel.CultureCode = PortalAgent.CurrentPortal.CultureCode;
            shoppingCartModel.CurrencyCode = PortalAgent.CurrentPortal.CurrencyCode;
            shoppingCartModel.IsCalculatePromotionAndCoupon = isCalculatePromotionAndCoupon;
            shoppingCartModel.IsCalculateVoucher = false;
            shoppingCartModel.IsPendingOrderRequest = false;
            //Calculate cart
            shoppingCartModel = CalculateCart(shoppingCartModel, isCalculateTaxAndShipping, isCalculateCart);

            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, shoppingCartModel);
            //Set currency details.
            return SetCartCurrency(shoppingCartModel);
        }

        //Update quantity of cart item.
        public virtual CartViewModel UpdateCartItemQuantity(string guid, string quantity, int productId)
        {
            // Get shopping cart from session.
            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            decimal newQuantity = ModifyQuantityValue(quantity);
            if (IsNotNull(shoppingCart))
            {
                //Get shopping cart item on the basis of guid.
                shoppingCart = GetShoppingCartItemByExternalId(shoppingCart, guid, newQuantity, productId);

                //Update shopping cart item quantity.
                if (IsNotNull(shoppingCart))
                {
                    return UpdatecartItemQuantity(shoppingCart, guid, newQuantity, productId);
                }
            }
            return new CartViewModel();
        }

        //Update quantity of cart item.
        public virtual AddToCartViewModel UpdateQuantityOfCartItem(string guid, string quantity, int productId)
        {
            // Get shopping cart from session.
            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            decimal newQuantity = ModifyQuantityValue(quantity);

            if (IsNotNull(shoppingCart))
            {
                //Update quantity and update the cart.
                if (productId > 0)
                {
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(x => x.GroupProducts?.Where(y => y.ProductId == productId)?.Select(z => { z.Quantity = newQuantity; return z; })?.FirstOrDefault()).ToList();
                }
                else
                {
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.Quantity = Convert.ToDecimal(newQuantity.ToInventoryRoundOff()); return y; }).ToList();
                }

                ShoppingCartItemModel shoppingCartItemModel = shoppingCart.ShoppingCartItems?.FirstOrDefault(w => w.ExternalId == guid);
                shoppingCartItemModel?.AssociatedAddOnProducts?.ForEach(x => x.Quantity = Convert.ToDecimal(newQuantity.ToInventoryRoundOff()));
                if (IsNotNull(shoppingCartItemModel))
                {
                    AddToCartViewModel addToCartViewModel = new AddToCartViewModel();
                    addToCartViewModel.CookieMappingId = GetFromCookie(WebStoreConstants.CartCookieKey);
                    addToCartViewModel.GroupId = shoppingCartItemModel.GroupId;
                    addToCartViewModel.AddOnProductSKUs = shoppingCartItemModel.AddOnProductSKUs;
                    addToCartViewModel.AutoAddonSKUs = shoppingCartItemModel.AutoAddonSKUs;
                    addToCartViewModel.SKU = shoppingCartItemModel.SKU;
                    addToCartViewModel.ParentOmsSavedcartLineItemId = shoppingCartItemModel.ParentOmsSavedcartLineItemId;
                    addToCartViewModel.AssociatedAddOnProducts = shoppingCartItemModel.AssociatedAddOnProducts;
                    addToCartViewModel.Custom1 = shoppingCartItemModel.Custom1;
                    addToCartViewModel.Custom2 = shoppingCartItemModel.Custom2;
                    addToCartViewModel.Custom3 = shoppingCartItemModel.Custom3;
                    addToCartViewModel.Custom4 = shoppingCartItemModel.Custom4;
                    addToCartViewModel.Custom5 = shoppingCartItemModel.Custom5;

                    if (!string.IsNullOrEmpty(shoppingCartItemModel.BundleProductSKUs))
                    {
                        addToCartViewModel.OmsSavedCartLineItemId = shoppingCartItemModel.OmsSavedcartLineItemId;
                        addToCartViewModel.Quantity = shoppingCartItemModel.Quantity;
                        GetSelectedBundleProductsForAddToCart(addToCartViewModel, shoppingCartItemModel.BundleProductSKUs);
                    }
                    else if (shoppingCartItemModel.GroupProducts?.Count > 0)
                    {
                        shoppingCartItemModel.Quantity = shoppingCartItemModel.GroupProducts.FirstOrDefault().Quantity;
                        addToCartViewModel.ShoppingCartItems.Add(shoppingCartItemModel);
                    }

                    else if (!string.IsNullOrEmpty(shoppingCartItemModel.ConfigurableProductSKUs))
                    {
                        addToCartViewModel.OmsSavedCartLineItemId = shoppingCartItemModel.OmsSavedcartLineItemId;
                        addToCartViewModel.Quantity = shoppingCartItemModel.Quantity;
                        GetSelectedConfigurableProductsForAddToCart(addToCartViewModel, shoppingCartItemModel.ConfigurableProductSKUs);
                    }
                    else if (!string.IsNullOrEmpty(shoppingCartItemModel?.AddOnProductSKUs))
                    {
                        addToCartViewModel.OmsSavedCartLineItemId = shoppingCartItemModel.OmsSavedcartLineItemId;
                        addToCartViewModel.Quantity = shoppingCartItemModel.Quantity;
                        GetSelectedAddOnProductsForAddToCart(addToCartViewModel, shoppingCartItemModel.AddOnProductSKUs);
                    }
                    else
                        addToCartViewModel.ShoppingCartItems.Add(shoppingCartItemModel);

                    AddToCartModel addToCartModel = addToCartViewModel.ToModel<AddToCartModel>();
                    addToCartModel.PublishedCatalogId = shoppingCart.PublishedCatalogId;
                    addToCartModel.Coupons = shoppingCart.Coupons;
                    addToCartModel.ZipCode = shoppingCart?.ShippingAddress?.PostalCode;
                    addToCartModel.UserId = shoppingCart.UserId;
                    addToCartModel.PortalId = shoppingCart.PortalId;
                    addToCartModel.LocaleId = shoppingCart.LocaleId;
                    addToCartModel = _shoppingCartsClient.AddToCartProduct(addToCartModel);
                    addToCartModel.ShippingId = shoppingCart.ShippingId;
                    //Save items updated quantities with sku in list.
                    UpdateQuantityItem(shoppingCartItemModel);

                    // ShoppingCartItems set null to get from cart from database in GetCart
                    shoppingCart.ShoppingCartItems = null;

                    SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCart);

                    SaveInCookie(WebStoreConstants.CartCookieKey, addToCartModel.CookieMappingId,ZnodeConstant.MinutesInAYear);

                    //To clear the session for CartCount session key.
                    ClearCartCountFromSession();
                }
            }
            return new AddToCartViewModel();
        }

        protected virtual void UpdateQuantityItem(ShoppingCartItemModel shoppingCartItemModel)
        {
            UpdatedProductQuantityModel updatedProduct = updatedProducts.FirstOrDefault(x =>
                                    x.SKU == shoppingCartItemModel.SKU &&
                                    x.AddOnProductSKUs == shoppingCartItemModel.AddOnProductSKUs &&
                                    x.AutoAddonSKUs == shoppingCartItemModel.AutoAddonSKUs &&
                                    x.ConfigurableProductSKUs == shoppingCartItemModel.ConfigurableProductSKUs &&
                                    x.BundleProductSKUs == shoppingCartItemModel.BundleProductSKUs &&
                                    x.PersonaliseValuesDetail == shoppingCartItemModel.PersonaliseValuesDetail);
            if (IsNotNull(updatedProduct))
            {
                updatedProduct.Quantity = shoppingCartItemModel.Quantity;
            }
            else
            {
                updatedProducts.Add(new UpdatedProductQuantityModel()
                {
                    SKU = shoppingCartItemModel.SKU,
                    AddOnProductSKUs = shoppingCartItemModel.AddOnProductSKUs,
                    AutoAddonSKUs = shoppingCartItemModel.AutoAddonSKUs,
                    BundleProductSKUs = shoppingCartItemModel.BundleProductSKUs,
                    ConfigurableProductSKUs = shoppingCartItemModel.ConfigurableProductSKUs,
                    Quantity = shoppingCartItemModel.Quantity,
                    PersonaliseValuesDetail = shoppingCartItemModel.PersonaliseValuesDetail
                });
            }
        }

        //Remove cart item from shopping cart.
        public virtual bool RemoveCartItem(string guiId)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            if (cartModel?.ShoppingCartItems?.Count < 0) { return false; }

            if (cartModel.ShoppingCartItems.Count.Equals(1))
            {
                //Remove all cart items if shopping cart having only one item.              
                return RemoveAllCartItems();
            }

            ShoppingCartItemModel cartItem = cartModel.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == guiId);
            if (Equals(cartItem, null)) { return false; }

            //to remove child item if deleting the parent product having autoAddon associated with it
            if (!string.IsNullOrEmpty(cartItem.AutoAddonSKUs))
            {
                cartModel.RemoveAutoAddonSKU = cartItem.AutoAddonSKUs;
                cartModel.IsParentAutoAddonRemoved = string.Equals(cartItem.SKU.Trim(), cartItem.AutoAddonSKUs.Trim(), StringComparison.OrdinalIgnoreCase) ? false : true;
            }

            // Remove item and update the cart in Session and API.
            cartModel.ShoppingCartItems.Remove(cartItem);

            _shoppingCartsClient.RemoveCartLineItem(Convert.ToInt32(cartItem.OmsSavedcartLineItemId));

            //Save shopping cart in session.
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            ClearCartCountFromSession();
            return true;

        }
        //Remove cart items from shopping cart.
        public virtual bool RemoveCartItems(string[] guiId)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            if (cartModel?.ShoppingCartItems?.Count < 0) { return false; }

            if (cartModel.ShoppingCartItems.Count.Equals(1))
            {
                //Remove all cart items if shopping cart having only one item.              
                return RemoveAllCartItems();
            }

            ShoppingCartItemModel[] cartItems = cartModel.ShoppingCartItems.Where(x => guiId.Contains(x.ExternalId)).ToArray();
            if (Equals(cartItems, null) || cartItems.Count() != guiId.Length) { return false; }

            foreach (ShoppingCartItemModel cartItem in cartItems)
            {
                //to remove child item if deleting the parent product having autoAddon associated with it
                if (!string.IsNullOrEmpty(cartItem.AutoAddonSKUs))
                {
                    cartModel.RemoveAutoAddonSKU = cartItem.AutoAddonSKUs;
                    cartModel.IsParentAutoAddonRemoved = string.Equals(cartItem.SKU.Trim(), cartItem.AutoAddonSKUs.Trim(), StringComparison.OrdinalIgnoreCase) ? false : true;
                }

                // Remove item and update the cart in Session and API.
                cartModel.ShoppingCartItems.Remove(cartItem);
            }

            cartModel.IsMerged = true;
            //Create new cart.
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.CreateCart(cartModel);

            //Save shopping cart in session.
            SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCartModel);
            ClearCartCountFromSession();
            return true;
        }

        //Remove all cart items from shoppingCart.
        public virtual bool RemoveAllCartItems()
        {
            // Get cart from Session.
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            if (IsNotNull(cart))
            {
                RemoveCookie(WebStoreConstants.CartCookieKey);
                _shoppingCartsClient.RemoveAllCartItem(new CartParameterModel { UserId = cart.UserId.GetValueOrDefault(), CookieMappingId = cart.CookieMappingId, PortalId = PortalAgent.CurrentPortal.PortalId });
                SaveInSession(WebStoreConstants.CartModelSessionKey, new ShoppingCartModel());
                ClearCartCountFromSession();
            }
            return true;
        }

        //Remove all cart items from shoppingCart.
        public virtual bool RemoveAllCartItems(int omsOrderId = 0)
        {
            // Get cart from Session.
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            if (IsNotNull(cart))
            {
                RemoveCookie(WebStoreConstants.CartCookieKey);
                if (omsOrderId < 1)
                    _shoppingCartsClient.RemoveAllCartItem(new CartParameterModel { UserId = cart?.UserId.GetValueOrDefault(), CookieMappingId = cart?.CookieMappingId, PortalId = PortalAgent.CurrentPortal.PortalId });
                //To clear the session for CartCount session key.
                ClearCartCountFromSession();
                SaveInSession(WebStoreConstants.CartModelSessionKey, new ShoppingCartModel());
            }
            return true;
        }

        //Get count of cart items in shopping cart.
        public virtual decimal GetCartCount()
        {
            decimal? cartCount = GetFromSession<decimal?>(WebStoreConstants.CartCount);

            if (IsNull(cartCount))
            {
                cartCount = ReturnCartCount();
                SaveInSession<decimal?>(WebStoreConstants.CartCount, cartCount);
            }

            return cartCount.GetValueOrDefault();
        }

        //To remove the cart count key from session.
        public virtual void ClearCartCountFromSession() => RemoveInSession(WebStoreConstants.CartCount);

        //Get count of cart items in shopping cart After Login.
        //In above GetCartCount(), User's Authentication status doesn't get changes in same request and hence it return 0 cart count for first time which unable to redirect on cart page. 
        public virtual decimal GetCartCountAfterLogin(bool cartCountAfterLogin)
        {
            return ReturnCartCount(cartCountAfterLogin);
        }

        //Get count of cart items in shopping cart.
        public virtual decimal GetCartCount(int productId)
        {
            //Get Shopping cart from session or cookie.
            ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                       GetCartFromCookie();
            decimal? count = shoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.ProductId == productId)?.Quantity;

            return count == null ? 0 : Convert.ToDecimal(Helper.GetRoundOffQuantity(count.Value));
        }

        //Get template cart model session.
        public virtual TemplateViewModel GetTemplateCartModelSession()
        {
            //Get template cart model session.
            TemplateViewModel cartModel = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());

            //Return cartModel.
            return cartModel;
        }

        //Get cart info from cookie.
        public virtual ShoppingCartModel GetCartFromCookie()
        {
            ShoppingCartModel shoppingCartModel = null;
            if (GetCartCount() <= 0)
                return shoppingCartModel;
            try
            {
                string cookieValue = GetFromCookie(WebStoreConstants.CartCookieKey);
                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                shoppingCartModel = _shoppingCartsClient.GetShoppingCart(new CartParameterModel
                {
                    CookieMappingId = cookieValue,
                    LocaleId = PortalAgent.LocaleId,
                    PortalId = PortalAgent.CurrentPortal.PortalId,
                    PublishedCatalogId = GetCatalogId().GetValueOrDefault(),
                    UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId
                });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return shoppingCartModel;
        }

        //Calculate shipping.
        public virtual CartViewModel CalculateShipping(int shippingOptionId, int shippingAddressId, string shippingCode, string additionalInstruction = "", bool isQuoteRequest = false, bool isCalculateCart = true, bool isPendingOrderRequest = false, string jobName = "")
        {
                ShoppingCartModel cartModel = IsShoppingCartInSession(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)) ?
                      GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey): GetCartFromCookie();

                if (shippingOptionId != cartModel.Shipping.ShippingId)
                {
                    cartModel.ShippingHandlingCharges = 0;
                }

                if (IsNull(cartModel?.Shipping))
                {
                    cartModel.Shipping = new OrderShippingModel();
                }

                cartModel.IsCalculatePromotionAndCoupon = isQuoteRequest ? false : true;
                cartModel.IsQuoteOrder = isQuoteRequest;
                //bind shipping related data to ShoppingCartModel.
                cartModel.AdditionalInstructions = additionalInstruction;
                cartModel.JobName = jobName;
                cartModel.Shipping.ShippingId = shippingOptionId > 0 ? shippingOptionId : cartModel.Shipping.ShippingId;
                cartModel.ShippingId = shippingOptionId > 0 ? shippingOptionId : cartModel.ShippingId;
                cartModel.Shipping.ResponseCode = (IsNotNull(shippingCode) && shippingCode != string.Empty) ? shippingCode : cartModel.Shipping.ResponseCode;
                cartModel.ShippingAddress = (IsNotNull(cartModel?.ShippingAddress) && !string.IsNullOrEmpty(cartModel?.ShippingAddress?.PostalCode)) ? cartModel.ShippingAddress : DependencyResolver.Current.GetService<IUserAgent>().GetAddressList()?.AddressList?.FirstOrDefault(x => x.AddressId == shippingAddressId)?.ToModel<AddressModel>();
                cartModel.Shipping.ShippingCountryCode = IsNull(cartModel?.ShippingAddress?.CountryName) ? "us" : cartModel.ShippingAddress.CountryName;
                // When Amazon Pay Checkout Screen initializing shipping should not be calculated based on selected shipping in webstore checkout page as address box different
                if (shippingOptionId == 0)
                {
                    cartModel.ShippingHandlingCharges = 0.0M;
                    cartModel.Shipping.ShippingId = shippingOptionId;
                    cartModel.Shipping.ResponseCode = "0";
                }

                if (IsNull(cartModel.UserDetails))
                    cartModel.UserDetails = new UserModel() { BusinessIdentificationNumber = GetBusinessIdentificationNoFromSession() };
                else
                    cartModel.UserDetails.BusinessIdentificationNumber = GetBusinessIdentificationNoFromSession();
                //Calculate cart.
                cartModel = CalculateCart(cartModel, true, isCalculateCart);
                cartModel.ShoppingCartItems.ForEach(x => x.Product.ShippingCost = x.Product.ShippingCost > 0 ? x.Product.ShippingCost : x.ShippingCost);
                CartViewModel cartViewModel = cartModel?.ToViewModel<CartViewModel>();

                if (IsNotNull(cartViewModel))
                {
                    cartViewModel.AdditionalInstruction = cartModel.AdditionalInstructions;
                    cartModel.Shipping.ShippingDiscount = Convert.ToDecimal(cartViewModel?.Shipping?.ShippingDiscount);
                    cartModel.ShippingCost = cartViewModel.ShippingCost;
                    cartModel.TaxCost = cartViewModel.TaxCost;
                    cartViewModel.IsQuoteRequest = isQuoteRequest;
                    SetPortalApprovalType(cartViewModel);

                    cartViewModel.IsSinglePageCheckout = PortalAgent.CurrentPortal.IsEnableSinglePageCheckout;

                    if (!string.IsNullOrEmpty(cartViewModel.ShippingResponseErrorMessage))
                    {
                        cartViewModel.HasError = true;
                        cartViewModel.IsValidShippingSetting = false;
                        cartViewModel.ErrorMessage = cartViewModel.ShippingResponseErrorMessage;
                    }
                    else if (cartViewModel.ShoppingCartItems.Where(w => w.IsAllowedTerritories == false).ToList().Count > 0)
                    {
                        cartViewModel.HasError = true;
                    }

                    //Get User details from session.
                    UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                    if (cartModel.OmsQuoteId > 0)
                        cartViewModel.IsLastApprover = _accountQuoteClient.IsLastApprover(cartModel.OmsQuoteId);
                    else
                        cartViewModel.IsLastApprover = cartModel?.IsLastApprover ?? false;

                    cartViewModel.IsLevelApprovedOrRejected = cartModel?.IsLevelApprovedOrRejected ?? false;
                    cartViewModel.OmsQuoteId = cartModel?.OmsQuoteId ?? 0;

                    //Bind User details to ShoppingCartModel.
                    cartViewModel.CustomerPaymentGUID = userViewModel?.CustomerPaymentGUID;
                    cartViewModel.UserId = IsNotNull(userViewModel) ? userViewModel.UserId : 0;
                    cartViewModel.OrderStatus = cartModel.OrderStatus;
                    cartViewModel.BudgetAmount = (userViewModel?.BudgetAmount).GetValueOrDefault();
                    cartViewModel.PermissionCode = userViewModel?.PermissionCode;
                    cartViewModel.RoleName = userViewModel?.RoleName;

                    if (IsNotNull(cartViewModel?.ShoppingCartItems) && cartViewModel.ShoppingCartItems.Count > 0)
                    {
                        cartViewModel.ShoppingCartItems = cartViewModel.ShoppingCartItems.OrderBy(c => c.GroupSequence).ToList();
                    }
                }

                IWebstoreHelper _webstoreHelper = GetService<IWebstoreHelper>();
                cartViewModel.InHandDate = _webstoreHelper.GetInHandDate();
                cartViewModel.ShippingConstraints = _webstoreHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete);
                SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);

                return cartViewModel;
        }

        // To get the Business Identification Number from user session.
        protected virtual string GetBusinessIdentificationNoFromSession()
        {
            return GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.BusinessIdentificationNumber;
        }

        public virtual bool MergeCart()
        {
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                           GetCartFromCookie();

            //GetCurrent user's Id.
            int userId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId;

            if (HttpContext.Current.Request.Url.AbsolutePath.ToUpper() != "/CHECKOUT/INDEX"
               && cart?.UserId != 0
               && userId != cart?.UserId)
            {
                //Nullify edit mode cart
                cart = null;
            }
            //Get shopping cart by userId.
            _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ShoppingCartModel cartModel = _shoppingCartsClient.GetShoppingCart(new CartParameterModel
            {
                UserId = userId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                LocaleId = PortalAgent.LocaleId,
                PublishedCatalogId = GetCatalogId().GetValueOrDefault()
            });

            //Check if cart persistent.
            CheckCartPersistent(userId, cartModel, cart);

            //to set user profile Id in shopping cart 
            cartModel.ProfileId = Helper.GetProfileId();

            bool status = false;
            //Update cart
            if (cart?.ShoppingCartItems?.Count > 0)
            {
                status = UpdateCart(ref cartModel);
            }

            //Save cart in session.
            if (status)
            {
                SaveInSession(WebStoreConstants.CartMerged, true);
            }

            cartModel.ShippingId = (cart?.ShippingId).GetValueOrDefault();
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            RemoveCookie(WebStoreConstants.CartCookieKey);
            return status;
        }

        /// <summary>
        /// Merge Cart after login
        /// </summary>
        /// <returns></returns>
        public virtual bool MergeGuestUserCart()
        {
            bool status = false;

            //if session is null or session's ShoppingCartItems are null then getting cart from cookie
            ShoppingCartModel cart = IsShoppingCartInSession(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)) ?
                GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) : GetCartFromCookie();

            //Merge cart
            if (cart?.ShoppingCartItems?.Count() > 0)
            {
                ClearCartCountFromSession();
                RemoveInSession(WebStoreConstants.CartModelSessionKey);
                if (cart.PublishedCatalogId == GetCatalogId(true).GetValueOrDefault())
                    status = _shoppingCartsClient.MergeGuestUsersCart(GetFiltersForMergeCart(cart.CookieMappingId, Convert.ToInt32(cart.ShoppingCartItems.FirstOrDefault()?.OmsSavedcartLineItemId)));
            }

            //Get shopping cart by userId.
            _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ShoppingCartModel cartModel = _shoppingCartsClient.GetShoppingCart(new CartParameterModel
            {
                UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                LocaleId = PortalAgent.LocaleId,
                PublishedCatalogId = GetCatalogId(true).GetValueOrDefault()
            });

            cartModel.ShippingId = (cart?.ShippingId).GetValueOrDefault();
            cartModel.Coupons = cart?.Coupons;
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            RemoveCookie(WebStoreConstants.CartCookieKey);
            ClearCartCountFromSession();
            return status;
        }

        /// <summary>
        /// This method is use for avoiding the merge admin cart and user cart at the time of impersonation. 
        /// </summary>
        /// <returns> bool status </returns>
        public virtual bool MergeUserCartAfterImpersonationLogin()
        {
            bool status = false;

            //if session is null or session's ShoppingCartItems are null then getting cart from cookie
            ShoppingCartModel cart = IsShoppingCartInSession(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)) ?
                GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) : GetCartFromCookie();

            //Get shopping cart by userId.
            _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ShoppingCartModel cartModel = _shoppingCartsClient.GetShoppingCart(new CartParameterModel
            {
                UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                LocaleId = PortalAgent.LocaleId,
                PublishedCatalogId = GetCatalogId(true).GetValueOrDefault()
            });

            cartModel.ShippingId = (cart?.ShippingId).GetValueOrDefault();
            cartModel.Coupons = cart?.Coupons;
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            RemoveCookie(WebStoreConstants.CartCookieKey);
            ClearCartCountFromSession();
            return status;
        }
        protected virtual FilterCollection GetFiltersForMergeCart(string cookieMappingId, int omsSavedCartLineItemId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("UserId", FilterOperators.Equals, GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add("CookieMappingId", FilterOperators.Equals, cookieMappingId);
            filters.Add("OmsSavedCartLineItemId", FilterOperators.Equals, omsSavedCartLineItemId.ToString());
            return filters;
        }

        //Apply & validate Coupon code.
        public virtual CartViewModel ApplyDiscount(string discountCode, bool isGiftCard)
        {
            //Get shopping cart form session or through cookie.
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                           GetCartFromCookie();

            if (IsNull(cartModel))
            {
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);
            }

            cartModel.UserId = GetUserId();
            //Apply giftcard if "isGiftCard" is true else apply coupon if any. 
            return isGiftCard ? ApplyVoucher(discountCode.Trim(), cartModel)
                              : ApplyCoupon(discountCode.Trim(), cartModel);
        }
        //Removes the applied coupon from the cart.
        public virtual CartViewModel RemoveCoupon(string couponCode)
        {
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                       GetCartFromCookie();

            bool isShippingBasedCouponRemoved = false;

            ShoppingCartModel oldShoppingCartModel = cart;

            if (IsNull(cart))
            {
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);
            }

            if (IsNotNull(cart.Coupons))
            {
                isShippingBasedCouponRemoved = IsShippingBasedCoupon(cart.Coupons, couponCode);
                cart.Coupons.RemoveAll(x => x.Code == couponCode);
            }

            //Calculate coupon with cart.
            ShoppingCartModel _cartModel = _shoppingCartsClient.Calculate(cart);
            _cartModel.ShippingOptions = cart.ShippingOptions;

            //Assign value to property to use for loading shipping option
            _cartModel.IsShippingBasedCoupon = isShippingBasedCouponRemoved;
            MapQuantityOnHandAndSeoName(oldShoppingCartModel, _cartModel);
            _cartModel.ShoppingCartItems.Select(x => { x.ShippingCost = oldShoppingCartModel.ShoppingCartItems.FirstOrDefault(y => y.ProductId == x.ProductId).ShippingCost;
                return x; }).ToList();

            //Required for session state SQLServer.
            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, _cartModel);

            return _cartModel?.ToViewModel<CartViewModel>();
        }


        // Get Cart method to check Session or Cookie to get the existing shopping cart.
        public virtual CartViewModel SetQuoteCart(int omsQuoteId)
        {
            ShoppingCartModel availableCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                     GetCartFromCookie();

            if (availableCart.OmsQuoteId != omsQuoteId)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString());
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());

                //Get account quote.
                _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                AccountQuoteModel accountQuoteModel = _accountQuoteClient.GetAccountQuote(new ExpandCollection() { ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString() }, filters);

                ShoppingCartModel shoppingCartModel = accountQuoteModel.ShoppingCart;

                UserViewModel userDetails = _userClient.GetUserAccountData(accountQuoteModel.UserId, new ExpandCollection { ExpandKeys.Profiles }).ToViewModel<UserViewModel>();

                AccountQuoteViewModel accountQuoteViewModel = BindDataToAccountQuoteViewModel(userDetails, accountQuoteModel);
                shoppingCartModel.IsLevelApprovedOrRejected = accountQuoteViewModel?.IsLevelApprovedOrRejected ?? false;
                shoppingCartModel.IsLastApprover = accountQuoteViewModel?.IsLastApprover ?? false;

                shoppingCartModel.OmsQuoteId = omsQuoteId;

                if (IsNull(shoppingCartModel))
                {
                    return new CartViewModel()
                    {
                        HasError = true,
                        ErrorMessage = WebStore_Resources.OutofStockMessage
                    };
                }

                if (shoppingCartModel.ShoppingCartItems?.Count == 0)
                {
                    return new CartViewModel();
                }

                shoppingCartModel.QuotePaymentSettingId = accountQuoteModel.PaymentSettingId;

                ShoppingCartModel shoppingCartModelForCreatecart = shoppingCartModel;
                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                _shoppingCartsClient.CreateCart(shoppingCartModelForCreatecart);

                SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCartModel);

                //Set currency details.
                return SetCartCurrency(shoppingCartModel);
            }
            else
            {
                return SetCartCurrency(availableCart);
            }
        }

        #region Template
        //Create Shopping Cart.
        public virtual TemplateViewModel AddToTemplate(TemplateCartItemViewModel cartItem)
        {
            if (IsNotNull(cartItem))
            {
                //Get shopping cart data from session.
                TemplateViewModel shoppingCartModel = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());
                cartItem.IsQuickOrderPad = true;
                //Bind Portal related data to Shopping cart model.
                ClearCartCountFromSession();
                return GetTemplateCart(cartItem, shoppingCartModel);
            }
            return new TemplateViewModel();
        }

        //Set template cart model to null.
        public virtual void SetTemplateCartModelSessionToNull()
        {
            SaveInSession<TemplateViewModel>(GetTemplateCartModelSessionKey(), null);
        }

        //Create template.
        public virtual bool CreateTemplate(TemplateViewModel templateViewModel)
        {
            AccountTemplateModel accountTemplateModel = templateViewModel.ToModel<AccountTemplateModel>();
            if (IsNotNull(accountTemplateModel))
            {
                TemplateViewModel cartItems = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());

                //Validate Template ID.
                if (accountTemplateModel?.OmsTemplateId > 0 && cartItems?.OmsTemplateId > 0)
                {
                    if (!HelperUtility.IsValidIdInQueryString(accountTemplateModel.OmsTemplateId, cartItems.OmsTemplateId))
                    {
                        templateViewModel.HasError = true;
                        templateViewModel.ErrorMessage = WebStore_Resources.HttpCode_401_AccessDeniedMsg;
                        return false;
                    }
                }

                accountTemplateModel.TemplateCartItems = cartItems?.TemplateCartItems.ToModel<TemplateCartItemModel>()?.ToList();
                SetTemplateModel(accountTemplateModel, cartItems.TemplateType);
                templateViewModel = _accountQuoteClient.CreateTemplate(accountTemplateModel)?.ToViewModel<TemplateViewModel>();
                return IsNotNull(templateViewModel);
            }
            return false;
        }

        //Get template.
        public virtual TemplateViewModel GetTemplate(int omsTemplateId, bool isClearAll)
        {
            //Get user details from session
            UserViewModel userViewModel = GetUserViewModelFromSession();

            if (omsTemplateId > 0)
            {
                try
                {
                    

                    ExpandCollection expands = new ExpandCollection();
                    expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());
                    expands.Add(ExpandKeys.Pricing);

                    FilterCollection filters = GetRequiredFilters();

                    //Get the account template model.
                    _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                    AccountTemplateModel accountTemplateModel = _accountQuoteClient.GetAccountTemplate(omsTemplateId, expands, filters);

                    if (IsNotNull(accountTemplateModel))
                    {
                        //Validate UserID.
                        if (!HelperUtility.IsValidIdInQueryString(accountTemplateModel.CreatedBy, userViewModel.UserId))
                        {
                        return (TemplateViewModel)GetViewModelWithErrorMessage(new TemplateViewModel(), WebStore_Resources.HttpCode_401_AccessDeniedMsg);
                        }
                   
                        //Maps the model to view model.
                        TemplateViewModel viewModel = accountTemplateModel.ToViewModel<TemplateViewModel>();          
                        viewModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                        viewModel.CultureCode = PortalAgent.CurrentPortal?.CultureCode;
                        //Maps the cart items.
                        viewModel.TemplateCartItems = accountTemplateModel.TemplateCartItems?.ToViewModel<TemplateCartItemViewModel>()?.ToList();

                        SetTemplateCartItemModel(viewModel.TemplateCartItems, accountTemplateModel.TemplateType);

                        TemplateViewModel TemplateViewModel = null;
                        if (accountTemplateModel.TemplateType==ZnodeConstant.SavedCart) 
                        {
                            TemplateViewModel = GetFromSession<TemplateViewModel>(GetSavedCartModelSessionKey());
                            CheckTemplateDataForSavedCart(viewModel);
                        } 
                        else 
                            TemplateViewModel = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());


                        //Get the template line items from session.
                        if (IsNotNull(TemplateViewModel) && omsTemplateId == TemplateViewModel.OmsTemplateId && TemplateViewModel.IsQuickOrderPad)
                        {
                            viewModel.TemplateCartItems = TemplateViewModel.TemplateCartItems;
                        }
                        if (IsNotNull(TemplateViewModel) && omsTemplateId == TemplateViewModel.OmsTemplateId && TemplateViewModel.TemplateType == ZnodeConstant.OrderTemplate && !isClearAll)
                        {
                            viewModel.TemplateCartItems = TemplateViewModel.TemplateCartItems;

                        }
                        if (accountTemplateModel.TemplateType == WebStoreConstants.SavedCart)
                        {
                            //Save in session.
                            SaveInSession<TemplateViewModel>(GetSavedCartModelSessionKey(), viewModel);
                        }
                        else
                        {
                            //Save in session.
                            SaveInSession<TemplateViewModel>(GetTemplateCartModelSessionKey(), viewModel);
                        }
                        return viewModel;
                    }
                    return new TemplateViewModel();
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return null;
                }
            }
            if (isClearAll == true)
                RemoveInSession(ZnodeConstant.TemplateShoppingCart + userViewModel.UserId);
            return new TemplateViewModel();
        }

        //Remove cart item from template.
        public virtual bool RemoveTemplateCartItem(string guiId, string templateType = null)
        {
            TemplateViewModel templateViewModel = null; 
            if (templateType == ZnodeConstant.SavedCart)
            {
                templateViewModel = GetFromSession<TemplateViewModel>(GetSavedCartModelSessionKey());
            }
            else 
            {
                templateViewModel = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());
            }

            if (templateViewModel?.TemplateCartItems?.Count > 0)
            {
                if (templateViewModel.TemplateCartItems.Count.Equals(1))
                {
                    //Remove all cart items if shopping cart having only one item.              
                    return RemoveAllTemplateCartItems(templateType);
                }

                TemplateCartItemViewModel cartItem = templateViewModel.TemplateCartItems.FirstOrDefault(x => x.ExternalId == guiId);

                if (IsNull(cartItem))
                {
                    return false;
                }

                if (cartItem.OmsTemplateLineItemId == 0 )
                {
                    // Remove item and update the cart in Session and API.
                    templateViewModel.TemplateCartItems.Remove(cartItem);
                    templateViewModel.CurrencyCode = PortalAgent.CurrentPortal.CurrencyCode;
                    templateViewModel.CultureCode = PortalAgent.CurrentPortal.CultureCode;
                    //Save shopping cart in session.
                    SaveInSession(GetTemplateCartModelSessionKey(), templateViewModel);
                    ClearCartCountFromSession();
                    return true;
                }

                // Remove item and update the cart in Session and API.
                templateViewModel.TemplateCartItems.Remove(cartItem);
                templateViewModel.CurrencyCode = PortalAgent.CurrentPortal.CurrencyCode;
                templateViewModel.CultureCode = PortalAgent.CurrentPortal.CultureCode;
                //Save shopping cart in session.
                SaveInSession(GetTemplateCartModelSessionKey(), templateViewModel);
                ClearCartCountFromSession();

                //Delete template cart item.
                if (cartItem.OmsTemplateId > 0 && cartItem.OmsTemplateLineItemId > 0)
                {
                    return _accountQuoteClient.DeleteCartItem(new AccountTemplateModel { OmsTemplateId = cartItem.OmsTemplateId, OmsTemplateLineItemId = cartItem.OmsTemplateLineItemId.ToString(), TemplateType = templateType});
                }

                return true;
            }
            return false;
        }

        //Remove all cart items from template.
        public virtual bool RemoveAllTemplateCartItems(string templateType = null)
        {
            TemplateViewModel templateViewModel = null;
            if (templateType == ZnodeConstant.SavedCart)
            {
                // Get cart from Session.
                templateViewModel = GetFromSession<TemplateViewModel>(GetSavedCartModelSessionKey());
            }
            else
            {
                // Get cart from Session.
                templateViewModel = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());
            }
            if (IsNotNull(templateViewModel))
            {
                //Delete template cart item.
                DeleteCartItem(templateViewModel);
                SaveInSession(GetTemplateCartModelSessionKey(), new TemplateViewModel());
                ClearCartCountFromSession();
                return true;
            }
            return false;
        }

        //Get template list.
        public virtual TemplateListViewModel GetTemplateList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, string templateType = null)
        {
            //Set Filter for template list.
            filters = SetFilterForTemplateList(filters, templateType);

            AccountTemplateListModel tempateList = _accountQuoteClient.GetTemplateList(null, filters, sortCollection, pageIndex, recordPerPage);
            TemplateListViewModel templateListViewModel = new TemplateListViewModel { List = tempateList?.AccountTemplates?.ToViewModel<TemplateViewModel>().ToList(), RoleName = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.RoleName };

            //Set tool menu for template on grid list view.
            SetTemplateListToolMenu(templateListViewModel);

            SetListPagingData(templateListViewModel, tempateList);

            return templateListViewModel?.List?.Count > 0 ? templateListViewModel : new TemplateListViewModel() { List = new List<TemplateViewModel>(), RoleName = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.RoleName };
        }

        //Method to delete template.
        public virtual bool DeleteTemplate(string omsTemplateId)
        {
            try
            {
                return _accountQuoteClient.DeleteTemplate(new ParameterModel { Ids = omsTemplateId.ToString() });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Method to delete  template cart item.
        public virtual bool DeleteCartItem(TemplateViewModel templateViewModel)
        {
            string _omsTemplateLineItemIds = string.Empty;
            try
            {
                if (templateViewModel?.TemplateCartItems?.Count > 0)
                {
                    _omsTemplateLineItemIds = string.Join(",", templateViewModel.TemplateCartItems.Select(x => x.OmsTemplateLineItemId));
                }

                ClearCartCountFromSession();
                return _accountQuoteClient.DeleteCartItem(new AccountTemplateModel { OmsTemplateId = templateViewModel.OmsTemplateId, OmsTemplateLineItemId = _omsTemplateLineItemIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Add multiple products to the cart.
        public virtual string AddMultipleProductsToCartTemplate(List<TemplateCartItemViewModel> cartItems)
        {
            string errorMessage = string.Empty;
            try
            {
                foreach (TemplateCartItemViewModel cartItem in cartItems)
                {
                    cartItem.IsQuickOrderPad = true;
                    AddToTemplate(cartItem);
                }

                return errorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return ex.Message;
            }
        }

        //Update quantity of template cart item.
        public virtual TemplateViewModel UpdateTemplateItemQuantity(string guid, decimal quantity, int productId)
        {
            // Get shopping cart from session.
            TemplateViewModel templateCart = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());

            if (IsNotNull(templateCart))
            {
                //Get shopping cart item on the basis of guid.
                templateCart = GetTemplateItemByExternalId(templateCart, guid, quantity, productId);

                //Update shopping cart item quantity.
                if (IsNotNull(templateCart))
                {
                    ClearCartCountFromSession();
                    return UpdateTemplateItemQuantity(templateCart, guid, quantity, productId);
                }
            }
            return new TemplateViewModel();
        }

        public virtual ShoppingCartModel GetCartItems() => GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

        //Add the template in cart.
        public virtual string AddTemplateToCart(int omsTemplateId)
        {
            string errorMessage = string.Empty;
            if (omsTemplateId > 0)
            {
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());

                FilterCollection filters = GetRequiredFilters();
                //Get the account template model.
                _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                AccountTemplateModel accountTemplateModel = _accountQuoteClient.GetAccountTemplate(omsTemplateId, expands, filters);
                errorMessage = CheckTemplateData(accountTemplateModel);
                if (accountTemplateModel?.TemplateCartItems?.Count > 0 && string.IsNullOrEmpty(errorMessage))
                {
                    ClearCartCountFromSession();
                    return AddMultipleProductsToCart(accountTemplateModel.TemplateCartItems.ToViewModel<CartItemViewModel>().ToList());
                }
            }
            return errorMessage;
        }

        //Add Product to Cart for saved items
        public virtual string AddProductToCartForLater(int omsTemplateId, int omsTemplateLineItemId)
        {
            string errorMessage = string.Empty;
            if (omsTemplateId > 0 && omsTemplateLineItemId > 0)
            {
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());

                FilterCollection filters = GetRequiredFilters();
                //Get the account template model.
                AccountTemplateModel accountTemplateModel = _accountQuoteClient.GetAccountTemplate(omsTemplateId, expands, filters);

                if (HelperUtility.IsNotNull(accountTemplateModel) && accountTemplateModel.TemplateCartItems.Count > 0)
                {
                    accountTemplateModel?.TemplateCartItems?.RemoveAll(x => x.OmsTemplateLineItemId != omsTemplateLineItemId);

                    ClearCartCountFromSession();
                    errorMessage = AddMultipleProductsToCart(accountTemplateModel.TemplateCartItems.ToViewModel<CartItemViewModel>().ToList());
                }
            }
            return errorMessage;
        }

        public string CheckTemplateData(AccountTemplateModel accountTemplateModel)
        {
            ShoppingCartModel cart = GetCartItems();
            TemplateViewModel viewModel = accountTemplateModel.ToViewModel<TemplateViewModel>();

            viewModel.TemplateCartItems = accountTemplateModel.TemplateCartItems?.ToViewModel<TemplateCartItemViewModel>()?.ToList();

            if (IsNotNull(viewModel) && IsNotNull(accountTemplateModel))
            {
                SetTemplateCartItemModel(viewModel.TemplateCartItems, accountTemplateModel.TemplateType);

                foreach (var item in viewModel.TemplateCartItems)
                {
                    item.ErrorMessage = string.Empty;

                    if (!item.IsActive)
                        return WebStore_Resources.InActive + "/" + item.ProductId;
                    if (item.IsObsolete)
                        return WebStore_Resources.IsObsoleteMessage + "/" + item.ProductId;

                    if (item.Quantity > item.MaxQuantity || item.Quantity < item.MinQuantity)
                        return string.Format(WebStore_Resources.WarningSelectedQuantity,item.MinQuantity, item.MaxQuantity)+ "/" + item.ProductId;

                    if (item.AllowBackOrder && HelperUtility.IsNotNull(item.DefaultInventoryCount))
                    {
                        return null;
                    }

                    if(!string.Equals(item.OutOfStockMessage, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (HelperUtility.IsNotNull(item.DefaultInventoryCount) && item.Quantity > Convert.ToInt32(item.DefaultInventoryCount))
                        {
                            return item.OutOfStockMessage + "/" + item.ProductId;
                        }
                    }

                    foreach (var cartItemData in cart.ShoppingCartItems)
                    {
                        if (item.GroupProducts.Count > 0)
                        {
                            foreach (var groupProduct in item.GroupProducts)
                            {
                                if (groupProduct.ProductId == cartItemData.ProductId)
                                {
                                    int groupProductQuantity = (int)(cartItemData.Quantity + groupProduct.Quantity);

                                    if (groupProductQuantity > int.Parse(item.DefaultInventoryCount))
                                        return item.OutOfStockMessage + "/" + item.ProductId;

                                    if (groupProductQuantity > groupProduct.MaxQuantity)
                                        return string.Format(WebStore_Resources.WarningSelectedQuantity, item.MinQuantity, item.MaxQuantity) + "/" + groupProduct.ProductId;
                                }

                            }
                        }

                        if (item.ProductId.Equals(cartItemData.ProductId.ToString()))
                        {
                            int quantity = (int)(cartItemData.Quantity + item.Quantity);
                            if (quantity > item.MaxQuantity)
                                return string.Format(WebStore_Resources.WarningSelectedQuantity, item.MinQuantity, item.MaxQuantity) + "/" + item.ProductId;

                            if (quantity > int.Parse(item?.DefaultInventoryCount == null ? "0" : item?.DefaultInventoryCount) && !string.Equals(item?.OutOfStockMessage, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase))
                                return item.OutOfStockMessage + "/" + item.ProductId;
                        }
                    }
                }
            }
            return null;
        }

        public void CheckTemplateDataForSavedCart(TemplateViewModel templateViewModel)
        {
            if (IsNotNull(templateViewModel) && templateViewModel.TemplateCartItems.Count > 0)
            {
                ShoppingCartModel cart = GetCartItems();
                
                foreach (var item in templateViewModel.TemplateCartItems)
                {
                    item.ErrorMessage = string.Empty;

                    if (!item.IsActive)
                        item.ErrorMessage = WebStore_Resources.ProductCombinationErrorMessage;
                    if (item.IsObsolete)
                        item.ErrorMessage = WebStore_Resources.IsObsoleteMessage;

                    if (item.Quantity > item.MaxQuantity || item.Quantity < item.MinQuantity)
                        item.ErrorMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, item.MinQuantity, item.MaxQuantity);

                    if (item.AllowBackOrder && HelperUtility.IsNotNull(item.DefaultInventoryCount))
                    {
                        item.ErrorMessage = string.Empty;
                    }
                    else if (!string.Equals(item.OutOfStockMessage, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (HelperUtility.IsNotNull(item.DefaultInventoryCount) && item.Quantity > Convert.ToInt32(item.DefaultInventoryCount))
                        {
                            item.ErrorMessage = item.OutOfStockMessage;
                        }
                    }

                    foreach (var cartItemData in cart.ShoppingCartItems)
                    {
                        if (item.GroupProducts.Count > 0)
                        {
                            foreach (var groupProduct in item.GroupProducts)
                            {
                                if (groupProduct.ProductId == cartItemData.ProductId)
                                {
                                    int groupProductQuantity = (int)(cartItemData.Quantity + groupProduct.Quantity);

                                    if (groupProductQuantity > int.Parse(item.DefaultInventoryCount))
                                        item.ErrorMessage = item.OutOfStockMessage;

                                    if (groupProductQuantity > groupProduct.MaxQuantity)
                                        item.ErrorMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, item.MinQuantity, item.MaxQuantity);
                                }

                            }
                        }

                        if (item.ProductId.Equals(cartItemData.ProductId.ToString()))
                        {
                            int quantity = (int)(cartItemData.Quantity + item.Quantity);
                            if (quantity > item.MaxQuantity)
                                item.ErrorMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, item.MinQuantity, item.MaxQuantity) + "/" + item.ProductId;

                            if (quantity > int.Parse(item.DefaultInventoryCount))
                                item.ErrorMessage = item.OutOfStockMessage + "/" + item.ProductId;
                        }
                    }
                }
            }
        }
        #endregion

        //Add multiple products to the cart.
        public virtual string AddMultipleProductsToCart(List<CartItemViewModel> cartItems)
        {
            string errorMessage = string.Empty;
            try
            {
                CreateCart(cartItems);
                return errorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return ex.Message;
            }
        }

        //Check quantity for cart item.
        public virtual void CheckCartQuantity(ProductViewModel viewModel, decimal? quantity)
        {
            if (IsNotNull(viewModel))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;
                decimal selectedQuantity = quantity.GetValueOrDefault();

                List<AttributesSelectValuesViewModel> inventorySetting = viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);
                if (inventorySetting?.Count > 0)
                {
                    ProductAgent.TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

                    if (viewModel.Quantity < selectedQuantity && !AllowBackOrder && TrackInventory)
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                        viewModel.ShowAddToCart = false;
                        return;
                    }
                    else if (viewModel.Quantity < selectedQuantity && AllowBackOrder && TrackInventory)
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                        viewModel.ShowAddToCart = true;
                        return;
                    }

                    if (!Between(selectedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                    {
                        viewModel.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                        viewModel.ShowAddToCart = false;
                        return;
                    }
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                }
            }
        }

        //Get Shipping estimates for the provided zip code.
        public virtual ShippingOptionListViewModel GetShippingEstimates(string zipCode)
        {
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                        GetCartFromCookie();

            if (IsNull(cart))
            {
                return (ShippingOptionListViewModel)GetViewModelWithErrorMessage(new ShippingOptionListViewModel(), WebStore_Resources.ErrorNoCartItemsFound);
            }

            cart.PublishStateId = DefaultSettingHelper.GetCurrentOrDefaultAppType(PortalAgent.CurrentPortal.PublishState);
            ShippingListModel lstModel = _shoppingCartsClient.GetShippingEstimates(zipCode, cart);

            ShippingOptionListViewModel lstViewModel = new ShippingOptionListViewModel { ShippingOptions = lstModel?.ShippingList?.ToViewModel<ShippingOptionViewModel>().ToList() };

            if (cart.Shipping?.ShippingId > 0)
            {
                lstViewModel.ShippingOptions?.Where(x => x.ShippingId == cart.Shipping.ShippingId)?.Select(y => { y.IsSelected = true; return y; })?.ToList();
            }

            lstViewModel = GetCurrencyFormattedRates(lstViewModel);
            if (IsNotNull(cart.ShippingAddress))
            {
                cart.ShippingAddress.PostalCode = zipCode;
                SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, cart);
            }
            SaveInSession<String>(ZnodeConstant.ShippingEstimatedZipCode, zipCode);
            return lstViewModel;
        }

        //Insert estimated shipping id in session
        public virtual void AddEstimatedShippingIdToCartViewModel(int shippingId)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                      GetCartFromCookie();
            cartModel.ShippingId = shippingId;
            cartModel.Shipping.ShippingId = shippingId;
            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, cartModel);
        }

        //Insert estimated shipping details in session
        public virtual void AddEstimatedShippingDetailsToCartViewModel(int shippingId, string zipCode)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                      GetCartFromCookie();
            cartModel.ShippingId = shippingId;
            cartModel.Shipping.ShippingId = shippingId;

            if (IsNotNull(cartModel.ShippingAddress))
            {
                cartModel.ShippingAddress.PostalCode = zipCode;
            }
            else
            {
                cartModel.ShippingAddress = new AddressModel { PostalCode = zipCode };
            }

            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, cartModel);
            SaveInSession<String>(ZnodeConstant.ShippingEstimatedZipCode, zipCode);
        }

        //Create cartItem for Group type product.
        public virtual void GetSelectedGroupedProducts(CartItemViewModel cartItem, ShoppingCartModel shoppingCart)
        {
            //Get skus and quantity of associated group products.
            string[] groupProducts = string.IsNullOrEmpty(cartItem.GroupProductSKUs) ? cartItem.GroupProducts?.Select(x => x.Sku)?.ToArray() : cartItem.GroupProductSKUs?.Split(',');
            string[] groupProductsQuantity = string.IsNullOrEmpty(cartItem.GroupProductsQuantity) ? cartItem.GroupProducts?.Select(x => Convert.ToString(x.Quantity))?.ToArray() : cartItem.GroupProductsQuantity?.Split('_');

            for (int index = 0; index < groupProducts?.Length; index++)
            {
                bool isNewExtIdRequired = false;
                if (!Equals(index, 0))
                {
                    isNewExtIdRequired = true;
                }

                ShoppingCartItemModel cartItemModel = BindGroupProducts(groupProducts[index], groupProductsQuantity[index], cartItem, isNewExtIdRequired);
                if (IsNotNull(cartItemModel))
                {
                    shoppingCart.ShoppingCartItems.Add(cartItemModel);
                }
            }
        }

        //Create cartItem for Group type product.
        public virtual void GetSelectedGroupedProductsForAddToCart(AddToCartViewModel cartItem)
        {
            //Get skus and quantity of associated group products.
            string[] groupProducts = string.IsNullOrEmpty(cartItem.GroupProductSKUs) ? cartItem.GroupProducts?.Select(x => x.Sku)?.ToArray() : cartItem.GroupProductSKUs?.Split(',');
            string[] groupProductsQuantity = string.IsNullOrEmpty(cartItem.GroupProductsQuantity) ? cartItem.GroupProducts?.Select(x => Convert.ToString(x.Quantity))?.ToArray() : cartItem.GroupProductsQuantity?.Split('_');

            cartItem.SKU = cartItem.SKU;
            cartItem.AddOnProductSKUs = cartItem.AddOnProductSKUs;
            string addOnProductSKUs = cartItem.AutoAddonSKUs;

            for (int index = 0; index < groupProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                if (CheckConfigurableProductForAddToCart(cartItem))
                {
                    cartItem = BindConfigurableProductsForAddToCart(groupProducts[index], groupProductsQuantity[index], cartItem, isNewExtIdRequired);
                }
                else
                {
                    cartItem.GroupProducts = new List<AssociatedProductModel> { new AssociatedProductModel { Sku = groupProducts[index], Quantity = decimal.Parse(groupProductsQuantity[index]) } };
                    cartItem.Quantity = decimal.Parse(groupProductsQuantity[index]);
                }

                if (IsNotNull(cartItem))
                {
                    GetSelectedAddOnProductsForAddToCart(cartItem, addOnProductSKUs);
                }
            }
        }

        //Create cartItem for Group type product.
        public virtual void GetSelectedBundleProductsForAddToCart(AddToCartViewModel cartItem, string bundleProductSKUs = null)
        {
            //Get skus and quantity of associated group products.
            string[] bundleProducts = !string.IsNullOrEmpty(bundleProductSKUs) ? bundleProductSKUs.Split(',') : !string.IsNullOrEmpty(cartItem.BundleProductSKUs) ? cartItem.BundleProductSKUs?.Split(',') : null;

            cartItem.SKU = cartItem.SKU;
            string addOnProductSKUs = cartItem.AddOnProductSKUs;
            cartItem.AutoAddonSKUs = cartItem.AutoAddonSKUs;

            for (int index = 0; index < bundleProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                cartItem.BundleProductSKUs = bundleProducts[index];
                cartItem.Quantity = cartItem.Quantity;

                if (IsNotNull(cartItem))
                {
                    GetSelectedAddOnProductsForAddToCart(cartItem, addOnProductSKUs);
                }
            }
        }

        //Create cartItem for configurable type product.
        public virtual void GetSelectedConfigurableProductsForAddToCart(AddToCartViewModel cartItem, string configurableProductSkus = null)
        {
            //Get skus and quantity of associated group products.
            string[] configurableProducts = !string.IsNullOrEmpty(configurableProductSkus) ? configurableProductSkus.Split(',') : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) ? cartItem.ConfigurableProductSKUs?.Split(',') : null;

            cartItem.SKU = cartItem.SKU;
            string addOnProductSKUs = cartItem.AddOnProductSKUs;
            cartItem.AutoAddonSKUs = cartItem.AutoAddonSKUs;

            for (int index = 0; index < configurableProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                cartItem.ConfigurableProductSKUs = configurableProducts[index];
                cartItem.Quantity = cartItem.Quantity;

                if (IsNotNull(cartItem))
                {
                    GetSelectedAddOnProductsForAddToCart(cartItem, addOnProductSKUs);
                }
            }
        }

        //Get the list of add-ons for cart line item on the basis of savedCartLineItemId.
        protected virtual List<AssociatedProductModel> GetAddOnsValueCartLineItem(List<ShoppingCartItemModel> childShoppingCartItems, int? savedCartLineItemId)
        {
            List<AssociatedProductModel> list = new List<AssociatedProductModel>();
            List<ShoppingCartItemModel> lineItem = childShoppingCartItems.Where(y => y.ParentOmsSavedcartLineItemId == savedCartLineItemId && y.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns)).ToList();
            foreach (ShoppingCartItemModel item in lineItem)
                list.Add(new AssociatedProductModel { Sku = item.SKU, Quantity = item.Quantity, OrderLineItemRelationshipTypeId = Convert.ToInt32(item.OrderLineItemRelationshipTypeId) });
            return list;
        }

        public virtual void GetSelectedAddOnProductsForAddToCart(AddToCartViewModel cartItem, string addOnSKUS = null)
        {
            string[] addOnProducts = !string.IsNullOrEmpty(addOnSKUS) ? addOnSKUS.Split(',') : cartItem.AddOnProductSKUs?.Split(',');

            if (IsNull(cartItem.AssociatedAddOnProducts) && IsNotNull(addOnProducts))
            {
                cartItem.AssociatedAddOnProducts = new List<AssociatedProductModel>();
                for (int index = 0; index < addOnProducts.Length; index++)
                {
                    cartItem.AssociatedAddOnProducts.Add(new AssociatedProductModel { Sku = addOnProducts[index], AddOnQuantity = cartItem.Quantity, Quantity = cartItem.Quantity, OrderLineItemRelationshipTypeId = 1 });
                }
            }
     
            if (IsNotNull(cartItem.AssociatedAddOnProducts) && cartItem.AssociatedAddOnProducts.Count > 0)
            {
                for (int index = 0; index < cartItem.AssociatedAddOnProducts.Count(); index++)
                {
                    bool isNewExtIdRequired = !Equals(index, 0);
                    cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                    cartItem.AddOnProductSKUs = cartItem.AssociatedAddOnProducts[index].Sku;
                    cartItem.Quantity = cartItem.Quantity;
                    cartItem.SKU = cartItem.SKU;
                    cartItem.AddOnQuantity = cartItem.AssociatedAddOnProducts[index].AddOnQuantity;

                    if (IsNotNull(cartItem))
                        cartItem.ShoppingCartItems.Add(cartItem.ToModel<ShoppingCartItemModel>());
                }
            }
            else
                cartItem.ShoppingCartItems.Add(cartItem.ToModel<ShoppingCartItemModel>());
        }

        //Get the Group products
        public virtual ShoppingCartItemModel BindGroupProducts(string simpleProduct, string simpleProductQty, CartItemViewModel cartItem, bool isNewExtIdRequired)
        {
            return new ShoppingCartItemModel
            {
                ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId,
                SKU = cartItem.SKU,
                Quantity = decimal.Parse(simpleProductQty),
                AddOnProductSKUs = cartItem.AddOnProductSKUs,
                AutoAddonSKUs = cartItem.AutoAddonSKUs,
                PersonaliseValuesDetail = cartItem.PersonaliseValuesDetail,
                GroupProducts = new List<AssociatedProductModel> { new AssociatedProductModel { Sku = simpleProduct, Quantity = decimal.Parse(simpleProductQty) } }
            };
        }

        //Bind CartItemViewModel properties to ShoppingCartModel.
        public virtual void BindCartItemData(CartItemViewModel cartItem, ShoppingCartModel shoppingCartModel)
        {
            shoppingCartModel.OmsQuoteId = cartItem.OmsQuoteId;
            shoppingCartModel.OrderStatus = cartItem.OrderStatus;
            shoppingCartModel.ShippingId = cartItem.ShippingId;
            shoppingCartModel.ShippingAddressId = cartItem.ShippingAddressId;
            shoppingCartModel.BillingAddressId = cartItem.BillingAddressId;
            shoppingCartModel.SelectedAccountUserId = cartItem.SelectedAccountUserId > 0 ? cartItem.SelectedAccountUserId : shoppingCartModel.UserId.GetValueOrDefault();
        }

        //Get attribute values and code.
        public virtual void PersonalisedItems(CartItemViewModel cartItem)
        {
            //Attribute Code And Value 
            string[] Codes = cartItem.PersonalisedCodes?.Split(',');
            string[] Values = cartItem.PersonalisedValues?.Split('`');

            //To DO

            //if (Convert.ToBoolean(cartItem.PersonalisedValues?.Contains('`')))
            //{
            //    Values = cartItem.PersonalisedValues?.Split('`');
            //}
            //else
            //{
            //    Values = cartItem.PersonalisedValues?.Split(',');
            //}

            Dictionary<string, object> SelectedAttributes = new Dictionary<string, object>();
            if (IsNotNull(Values))
            {
                //Add code and value pair
                for (int i = 0; i < Codes.Length; i++)
                    SelectedAttributes.Add(Codes[i], Values[i]);
            }
            cartItem.PersonaliseValuesList = SelectedAttributes;
        }

        //Get attribute values and code.
        public virtual void PersonalisedItemsForAddToCart(AddToCartViewModel cartItem)
        {
            string[] Codes = cartItem.PersonalisedCodes?.Split(',');
            string[] Values = cartItem.PersonalisedValues?.Split('`');

            if (Convert.ToBoolean(cartItem.PersonalisedValues?.Contains('`')))
                Values = cartItem.PersonalisedValues?.Split('`');
            else
                Values = cartItem.PersonalisedValues?.Split(',');

            Dictionary<string, object> SelectedAttributes = new Dictionary<string, object>();
            if (IsNotNull(Values))
            {
                //Add code and value pair
                for (int i = 0; i < Codes.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Values[i]))
                    {
                        SelectedAttributes.Add(Codes[i], Values[i]);
                    }
                }
            }
            cartItem.PersonaliseValuesList = SelectedAttributes;
        }

        //Check if cart persistent.
        public virtual void CheckCartPersistent(int userId, ShoppingCartModel cartModel, ShoppingCartModel sessionCart)
        {
            if (IsNotNull(cartModel) && sessionCart?.ShoppingCartItems?.Count > 0)
            {
                List<ShoppingCartItemModel> cartItems = sessionCart.ShoppingCartItems;
                List<ShoppingCartItemModel> groupProducts = new List<ShoppingCartItemModel>();
                foreach (ShoppingCartItemModel guestCartItem in cartItems)
                {
                    foreach (ShoppingCartItemModel loginCartModel in cartModel.ShoppingCartItems)
                    {
                        if (Equals(loginCartModel.ProductType, ZnodeConstant.GroupedProduct) && Equals(guestCartItem.ProductType, ZnodeConstant.GroupedProduct))
                        {
                            if (Equals(loginCartModel.GroupProducts.FirstOrDefault().Sku, guestCartItem.GroupProducts.FirstOrDefault().Sku)
                                && Equals(loginCartModel.AddOnProductSKUs, guestCartItem.AddOnProductSKUs))
                            {
                                loginCartModel.GroupProducts.FirstOrDefault().Quantity = guestCartItem.GroupProducts.FirstOrDefault().Quantity;
                            }
                            else
                            {
                                string sku = guestCartItem.GroupProducts.FirstOrDefault().Sku;
                                string addonSku = guestCartItem.AddOnProductSKUs;
                                bool isCartItemAvailable = CheckIfGrouProductAvailable(sku, addonSku, cartModel.ShoppingCartItems);
                                int availableInGroupProduct = groupProducts.Where(gp => gp.GroupProducts.FirstOrDefault().Sku == sku && gp.AddOnProductSKUs == addonSku).ToList().Count;
                                if (!isCartItemAvailable && availableInGroupProduct == 0)
                                {
                                    groupProducts.Add(guestCartItem);
                                }
                            }
                        }
                        else
                        {
                            if (guestCartItem.PersonaliseValuesDetail?.Count > 0 || loginCartModel.PersonaliseValuesDetail?.Count > 0)
                            {
                                //Check if personalized values are identical for guest user cart item and logged in user cart item model.
                                if (loginCartModel.ConfigurableProductSKUs == guestCartItem.ConfigurableProductSKUs && guestCartItem.PersonaliseValuesDetail.Any(x => loginCartModel.PersonaliseValuesDetail.Any(y => y.PersonalizeCode == x.PersonalizeCode) && loginCartModel.PersonaliseValuesDetail.Any(y => y.PersonalizeValue == x.PersonalizeValue)))
                                {
                                    loginCartModel.Quantity = guestCartItem.Quantity;
                                    loginCartModel.PersonaliseValuesList = guestCartItem.PersonaliseValuesList;
                                    loginCartModel.PersonaliseValuesDetail = guestCartItem.PersonaliseValuesDetail;
                                    break;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(loginCartModel.ConfigurableProductSKUs) && !string.IsNullOrEmpty(loginCartModel.ConfigurableProductSKUs))
                                {
                                    UpdateConfigurableProduct(guestCartItem, loginCartModel);
                                }
                                else if (Equals(loginCartModel.SKU, guestCartItem.SKU))
                                {
                                    UpdateProductDetails(guestCartItem, loginCartModel);
                                }
                            }
                        }
                    }


                    try
                    {
                        //If product with different SKU exist, then cartCount is 0 and in that case add the item to cart.
                        int? cartCount = cartModel?.ShoppingCartItems?.Where(cartModelItem => ((!string.IsNullOrEmpty(cartModelItem.ConfigurableProductSKUs)
                            ? cartModelItem.ConfigurableProductSKUs
                            : (string.IsNullOrEmpty(cartModelItem.AddOnProductSKUs)
                                ? cartModelItem.SKU
                                : cartModelItem.AddOnProductSKUs)) == (!string.IsNullOrEmpty(guestCartItem.ConfigurableProductSKUs)
                                    ? guestCartItem.ConfigurableProductSKUs
                                    : (string.IsNullOrEmpty(guestCartItem.AddOnProductSKUs)
                                        ? guestCartItem.SKU
                                        : guestCartItem.AddOnProductSKUs))) && (cartModelItem.PersonaliseValuesDetail?.Any(x => (cartModelItem.PersonaliseValuesDetail?.Any(y => y.PersonalizeCode == x.PersonalizeCode)).GetValueOrDefault() && (guestCartItem.PersonaliseValuesDetail?.Any(y => y.PersonalizeValue == x.PersonalizeValue)).GetValueOrDefault())).GetValueOrDefault())?.Count();


                        if (cartCount == 0)
                        {
                            cartModel.ShoppingCartItems.Add(guestCartItem);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                //check if group products are available if so insert it in cart model
                if (groupProducts?.Count > 0)
                {
                    cartModel.ShoppingCartItems.AddRange(groupProducts);
                }

                //Check if cart persistent.
                if (cartModel?.ShoppingCartItems?.Count > 0 && PortalAgent.CurrentPortal.PersistentCartEnabled)
                {
                    cartModel.UserDetails = new UserModel() { UserId = userId };
                }

                if (IsNull(cartModel.ShippingAddress))
                {
                    cartModel.ShippingAddress = new AddressModel { PostalCode = sessionCart.ShippingAddress?.PostalCode };
                }

                //Get coupons if already applied.
                if (sessionCart?.Coupons?.Count > 0)
                {
                    //Remove Invalid coupon code.
                    foreach (CouponModel coupon in sessionCart.Coupons)
                    {
                        if (coupon.CouponApplied)
                        {
                            cartModel.Coupons.Add(coupon);
                        }
                    }
                }
            }
        }

        //add cart items to cart.
        public virtual bool UpdateCart(ref ShoppingCartModel cartModel)
        {
            try
            {
                cartModel.IsMerged = true;
                cartModel.ShoppingCartItems.ForEach(x => x.IsProductEdit = true);
                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                cartModel = _shoppingCartsClient.CreateCart(cartModel);
                return IsNotNull(cartModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //add cart items to cart.
        public virtual ShoppingCartModel UpdateCartDetails(ShoppingCartModel cartModel)
        {
            try
            {
                cartModel.IsMerged = true;
                _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                return _shoppingCartsClient.CreateCart(cartModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Bind AddToCartNotification Model
        public virtual AddToCartNotificationViewModel BindAddToCartNotification(AddToCartViewModel cartItem)
        {
            AddToCartNotificationViewModel model = new AddToCartNotificationViewModel();
            if(IsNotNull(cartItem))
            {
                model.ProductId = cartItem.ProductId;
                if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs))
                {
                    model.SKU = cartItem.ConfigurableProductSKUs;
                    model.ProductType = ZnodeConstant.ConfigurableProduct;
                }
                else if (!string.IsNullOrEmpty(cartItem.BundleProductSKUs))
                {
                    model.SKU = cartItem.SKU;
                    model.ProductType = ZnodeConstant.BundleProduct;
                }
                else if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs))
                {
                    model.SKU = cartItem.GroupProductSKUs;
                    model.ProductType = ZnodeConstant.GroupedProduct;
                }
                else
                {
                    model.SKU = cartItem.SKU;
                    model.ProductType = ZnodeConstant.SimpleProduct;
                }
                model.ProductName = cartItem.ProductName;
                model.Quantity = cartItem.Quantity;
                model.ProductImage = cartItem.Image;
                model.IsEnabled = CheckAddToCartNotificationFlag();
            }
            return model;
        }

        //Check Add to cart notification flag has enabled against current theme
        protected virtual bool CheckAddToCartNotificationFlag()
        {
            bool flag = false;
            string selectedThemes = ZnodeWebstoreSettings.EnableEnhancedAddToCartNotificationForThemes;
            if(!string.IsNullOrEmpty(selectedThemes))
            {
                string[] themes = selectedThemes.Split(',');
                flag = !string.IsNullOrEmpty(themes?.FirstOrDefault(theme => string.Equals(PortalAgent.CurrentPortal.Theme, theme, StringComparison.InvariantCultureIgnoreCase) || string.Equals(PortalAgent.CurrentPortal.ParentTheme, theme, StringComparison.InvariantCultureIgnoreCase)));
            }
            return flag;
        }


        //Removes the applied voucher from the cart.
        public virtual CartViewModel RemoveVoucher(string voucherNumber)
        {
            ShoppingCartModel cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                       GetCartFromCookie();

            if (IsNull(cart))
            {
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);
            }

            if (IsNotNull(cart.Vouchers))
            {
                if (cart.Vouchers.Count == 1)
                    cart.IsAllVoucherRemoved = true;
                    cart.IsPendingOrderRequest = false;
                cart.Vouchers.RemoveAll(x => x.VoucherNumber == voucherNumber);
            }

            //Calculate voucher with cart.
            ShoppingCartModel _cartModel = _shoppingCartsClient.Calculate(cart);

            //Required for session state SQLServer.
            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, _cartModel);

            return _cartModel?.ToViewModel<CartViewModel>();
        }

        #endregion

        #region AmazonPay
        public virtual CartViewModel CalculateAmazonShipping(int shippingOptionId, int shippingAddressId, string shippingCode, AddressViewModel addressViewModel)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            if (IsNull(cartModel?.Shipping))
            {
                cartModel.Shipping = new OrderShippingModel();
            }

            //bind shipping related data to ShoppingCartModel.
            cartModel.Shipping.ShippingId = shippingOptionId;
            cartModel.Shipping.ResponseCode = shippingCode;
            cartModel.ShippingAddress = addressViewModel.ToModel<AddressModel>();

            //Calculate cart.
            CartViewModel cartViewModel = CalculateCart(cartModel)?.ToViewModel<CartViewModel>();

            if (IsNotNull(cartViewModel) && cartModel.Shipping.ShippingId > 0)
            {
                cartViewModel.IsSinglePageCheckout = PortalAgent.CurrentPortal.IsEnableSinglePageCheckout;

                if (!string.IsNullOrEmpty(cartViewModel.ShippingResponseErrorMessage))
                {
                    cartViewModel.HasError = true;
                    cartViewModel.IsValidShippingSetting = false;
                    cartViewModel.ErrorMessage = cartViewModel.ShippingResponseErrorMessage;
                }
                else if (cartViewModel.ShoppingCartItems.Where(w => w.IsAllowedTerritories == false).ToList().Count > 0)
                {
                    cartViewModel.HasError = true;
                }
            }
            //Get User details from session.
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            //Bind User details to ShoppingCartModel.
            cartViewModel.CustomerPaymentGUID = userViewModel?.CustomerPaymentGUID;
            cartViewModel.UserId = IsNotNull(userViewModel) ? userViewModel.UserId : 0;
            cartViewModel.OrderStatus = cartModel.OrderStatus;
            cartViewModel.BudgetAmount = (userViewModel?.BudgetAmount).GetValueOrDefault();
            cartViewModel.PermissionCode = userViewModel?.PermissionCode;
            cartViewModel.RoleName = userViewModel?.RoleName;
            return cartViewModel;
        }
        #endregion

        #region protected  Methods 

        /// <summary>
        /// Get cartItems data into shoppingCart.
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        protected virtual ShoppingCartModel AddCartItemsToShoppingCart(List<CartItemViewModel> cartItems, ShoppingCartModel shoppingCart)
        {
            foreach (CartItemViewModel cartItem in cartItems)
            {
                if (!string.IsNullOrEmpty(cartItem.PersonalisedCodes))
                {
                    PersonalisedItems(cartItem);
                }

                //Check if cartItem contains group product and add it to shopping cart.
                if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) && !string.IsNullOrEmpty(cartItem.GroupProductsQuantity) || cartItem?.GroupProducts?.Count > 0)
                {
                    GetSelectedGroupedProducts(cartItem, shoppingCart);
                }
                else
                {
                    shoppingCart.ShoppingCartItems.Add(cartItem?.ToModel<ShoppingCartItemModel>());
                }

            }
            shoppingCart.PortalId = PortalAgent.CurrentPortal.PortalId;
            shoppingCart.LocaleId = PortalAgent.LocaleId;
            shoppingCart.PublishedCatalogId = GetCatalogId().GetValueOrDefault();
            shoppingCart.UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId;
            shoppingCart.ProfileId = Helper.GetProfileId();
            return shoppingCart;
        }


        //Bind shopping cart values.
        public virtual ShoppingCartModel GetShoppingCart(CartItemViewModel cartItem, ShoppingCartModel shoppingCart)
        {
            if (!string.IsNullOrEmpty(cartItem.PersonalisedCodes))
            {
                PersonalisedItems(cartItem);
            }

            //Check if cartitem contains group product and add it to shopping cart.
            if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) && !string.IsNullOrEmpty(cartItem.GroupProductsQuantity) || cartItem?.GroupProducts?.Count > 0)
            {
                GetSelectedGroupedProducts(cartItem, shoppingCart);
            }
            else
            {
                shoppingCart.ShoppingCartItems.Add(cartItem?.ToModel<ShoppingCartItemModel>());
            }

            shoppingCart.PortalId = PortalAgent.CurrentPortal.PortalId;
            shoppingCart.LocaleId = PortalAgent.LocaleId;
            shoppingCart.PublishedCatalogId = GetCatalogId().GetValueOrDefault();
            shoppingCart.UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId;
            shoppingCart.ProfileId = Helper.GetProfileId();
            shoppingCart = GetEstimatedShippingDetails(shoppingCart);
            return shoppingCart;
        }

        //Bind shopping cart values.
        public virtual AddToCartViewModel GetShoppingCartValues(AddToCartViewModel cartItem)
        {
            if (!string.IsNullOrEmpty(cartItem.PersonalisedCodes))
            {
                PersonalisedItemsForAddToCart(cartItem);
            }

            //Check if cart item contains group product and add it to shopping cart.
            if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) && !string.IsNullOrEmpty(cartItem.GroupProductsQuantity) || cartItem?.GroupProducts?.Count > 0)
            {
                GetSelectedGroupedProductsForAddToCart(cartItem);
            }
            if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) && !string.IsNullOrEmpty(cartItem.ConfigurableProductQuantity))
            {
                GetSelectedConfigurableProductsForAddToCart(cartItem);
            }
            else if (!string.IsNullOrEmpty(cartItem.BundleProductSKUs))
            {
                GetSelectedBundleProductsForAddToCart(cartItem);
            }
            else
            {
                GetSelectedAddOnProductsForAddToCart(cartItem);
            }

            cartItem.PortalId = PortalAgent.CurrentPortal.PortalId;
            cartItem.LocaleId = PortalAgent.LocaleId;
            cartItem.PublishedCatalogId = GetCatalogId().GetValueOrDefault();
            cartItem.UserId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId;
            return cartItem;
        }

        //To check cartitem has configurable product or not.
        protected virtual bool CheckConfigurableProductForAddToCart(AddToCartViewModel cartItem)
        {
            return (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) &&
                           !string.IsNullOrEmpty(cartItem.GroupProductsQuantity) &&
                           string.Equals(cartItem.ProductType, ZnodeConstant.ConfigurableProduct, StringComparison.CurrentCultureIgnoreCase)) ? true : false;
        }

        //to bind configurable products
        protected virtual AddToCartViewModel BindConfigurableProductsForAddToCart(string configurableSKU, string quantity, AddToCartViewModel cartItem, bool isNewExtIdRequired)
        {
            cartItem.Quantity = Convert.ToDecimal(quantity);
            cartItem.ConfigurableProductSKUs = configurableSKU;
            return cartItem;
        }

        //Bind template cart values.
        protected virtual TemplateViewModel GetTemplateCart(TemplateCartItemViewModel cartItem, TemplateViewModel shoppingCart)
        {
            if (string.Equals(cartItem.ProductType, ZnodeConstant.GroupedProduct, StringComparison.CurrentCultureIgnoreCase))
            {
                GetGroupProducts(cartItem, shoppingCart);
            }

            //Check if Shopping cart is null.
            if (IsNull(shoppingCart))
            {
                shoppingCart = new TemplateViewModel() { TemplateCartItems = new List<TemplateCartItemViewModel>() };
            }

            shoppingCart.HasError = !Equals(shoppingCart.TemplateCartItems.FirstOrDefault(x => x.ProductId == cartItem.ProductId), null);

            if (shoppingCart.HasError)
            {
                return shoppingCart;
            }

            if (!string.IsNullOrEmpty(cartItem.PersonalisedCodes))
            {
                PersonalisedItems(cartItem);
            }

            cartItem.OmsTemplateId = shoppingCart.OmsTemplateId;

            //Check if cartitem contains group product and add it to shopping cart.
            if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) && !string.IsNullOrEmpty(cartItem.GroupProductsQuantity))
            {
                GetSelectedGroupedTemplateProducts(cartItem, shoppingCart);
            }
            else
            {
                ProductViewModel product = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>()).GetProductPriceAndInventory(cartItem.SKU, cartItem.Quantity.ToString(), cartItem.AddOnProductSKUs);
                if (IsNotNull(product))
                {
                    //Set product details.
                    SetProductDetails(cartItem, product);
                    shoppingCart.TemplateCartItems?.Add(cartItem);
                }
            }

            shoppingCart.UserId = Convert.ToInt32(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId);
            shoppingCart.CurrencyCode = PortalAgent.CurrentPortal.CurrencyCode;
            shoppingCart.CultureCode = PortalAgent.CurrentPortal.CultureCode;
            shoppingCart.IsQuickOrderPad = cartItem.IsQuickOrderPad;
            shoppingCart.TemplateName = cartItem.TemplateName;
            SaveInSession(GetTemplateCartModelSessionKey(), shoppingCart);
            return shoppingCart;
        }

        //Get Bundle Product list.
        protected virtual List<BundleProductViewModel> GetBundleProduct(int productId)
        {
            IProductAgent _productAgent = new ProductAgent(null, _publishProductClient, null, null, null, null);

            //Get group product list.
            List<BundleProductViewModel> bundleProductList = _productAgent.GetBundleProduct(productId);
            return bundleProductList;
        }

        //Get group product list.
        protected virtual void GetGroupProducts(TemplateCartItemViewModel cartItem, TemplateViewModel shoppingCart)
        {
            IProductAgent _productAgent = new ProductAgent(null, _publishProductClient, null, null, null, null);

            //Get group product list.
            List<GroupProductViewModel> groupProductList = _productAgent.GetGroupProductList(Convert.ToInt32(cartItem.ProductId));

            //Convert List<GroupProductViewModel> to List<AssociatedProductModel>.
            cartItem.GroupProducts = ToAssociatedProductListModel(groupProductList, cartItem.Quantity);

            shoppingCart?.TemplateCartItems?.Select(x => x.GroupProducts = cartItem.GroupProducts);

            //Calculate and set total price.
            foreach (AssociatedProductModel item in cartItem.GroupProducts)
            {
                cartItem.ExtendedPrice += item.UnitPrice.GetValueOrDefault() * (item.Quantity);
            }
        }

        //Convert List<GroupProductViewModel> to List<AssociatedProductModel>.
        protected virtual List<AssociatedProductModel> ToAssociatedProductListModel(List<GroupProductViewModel> groupProductList, decimal quantity)
        {
            List<AssociatedProductModel> groupProducts = new List<AssociatedProductModel>();
            foreach (GroupProductViewModel groupProduct in groupProductList)
            {
                groupProducts.Add(new AssociatedProductModel
                {
                    Quantity = quantity,
                    ProductId = groupProduct.PublishProductId,
                    Sku = groupProduct.SKU,
                    ProductName = groupProduct.Name,
                    UnitPrice = groupProduct.RetailPrice,
                    CurrencyCode = groupProduct.CurrencyCode,
                    CultureCode = groupProduct.CultureCode,
                    InStockMessage = groupProduct.InStockMessage,
                    InventoryMessage = groupProduct.InventoryMessage,
                    OutOfStockMessage = groupProduct.OutOfStockMessage,
                    BackOrderMessage = groupProduct.BackOrderMessage,
                    MaximumQuantity = Convert.ToDecimal(groupProduct.Attributes?.Value(ZnodeConstant.MaximumQuantity)),
                    MinimumQuantity = Convert.ToDecimal(groupProduct.Attributes?.Value(ZnodeConstant.MinimumQuantity))
                }
                    );
            }
            return groupProducts;
        }

        //Get cart description.
        protected virtual string GetCartDescription(ProductViewModel product)
        {
            string cartdescription = string.Empty;
            if (IsNotNull(product) && product.PublishProductId > 0)
            {                
                List<BundleProductViewModel> bundleProductList = null;

                if (string.Equals(product.ProductType, ZnodeConstant.BundleProduct, StringComparison.CurrentCultureIgnoreCase))
                {
                    bundleProductList = GetBundleProduct(product.PublishProductId);
                }

                if (IsNotNull(bundleProductList))
                {
                    foreach (BundleProductViewModel bundle in bundleProductList)
                    {
                        cartdescription += $"{ bundle.SKU } - { bundle.Name } <br/>";
                    }
                }
                //Binds the cart description.
                foreach (AddOnViewModel addon in product?.AddOns)
                {
                    if (IsNotNull(addon))
                    {
                        if (addon.IsRequired)
                        {
                            cartdescription += $"{ addon.GroupName } : { String.Join("<br />", addon.AddOnValues?.Select(p => p.Name)) } ";
                        }
                    }
                }               
            }
            return cartdescription;
        }
           

        //Check quantity of cartitem.
        protected virtual ShoppingCartModel GetShoppingCartItemByExternalId(ShoppingCartModel cartModel, string guid, decimal quantity, int productId)
        {
            //Get shopping cart item having guid.
            ShoppingCartItemModel cartItem = cartModel?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == guid);

            if (IsNotNull(cartItem))
            {
                //Get selected sku.
                string sku = productId > 0 ? cartItem.GroupProducts?.Where(x => x.ProductId == productId)?.FirstOrDefault()?.Sku
                                    : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) ? cartItem.ConfigurableProductSKUs : cartItem.SKU;
                ProductViewModel ProductDetails = CheckQuantity(sku, quantity, cartItem.AddOnProductSKUs);
                if (!ProductDetails.ShowAddToCart)
                {
                    if (IsNotNull(cartModel.ShoppingCartItems.FirstOrDefault(c => c.SKU == sku)))
                    {
                        cartModel.ShoppingCartItems.FirstOrDefault(c => c.SKU == sku).InsufficientQuantity = true;
                    }
                }
            }
            return cartModel;
        }

        //Check quantity of cartitem.
        public virtual TemplateViewModel GetTemplateItemByExternalId(TemplateViewModel cartModel, string guid, decimal quantity, int productId)
        {
            //Get shopping cart item having guid.
            TemplateCartItemViewModel cartItem = cartModel?.TemplateCartItems?.FirstOrDefault(x => x.ExternalId == guid);

            if (IsNotNull(cartItem))
            {
                int indexOfSelectedElement = cartModel.TemplateCartItems.IndexOf(cartItem);
                if (cartItem?.GroupProducts?.Count() > 0)
                {
                    cartItem.ExtendedPrice = 0;

                    //Calculate and set Extended Price of group product.
                    foreach (AssociatedProductModel item in cartItem.GroupProducts)
                    {
                        cartItem.ExtendedPrice += item.UnitPrice.GetValueOrDefault() * (item.ProductId == productId ? quantity : item.Quantity);
                    }

                    cartModel.TemplateCartItems.RemoveAll(x => x.ExternalId == guid);
                    cartModel.TemplateCartItems.Insert(indexOfSelectedElement, cartItem);
                    return cartModel;
                }

                //Get selected sku.
                string sku = productId > 0 ? cartItem.GroupProducts?.Where(x => x.ProductId == productId)?.FirstOrDefault()?.Sku
                                    : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) ? cartItem.ConfigurableProductSKUs : cartItem.SKU;

                //Get available quantity of cart item.
                ProductViewModel product = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>()).GetProductPriceAndInventory(sku, quantity.ToString(), cartItem.AddOnProductSKUs);

                //check if available quantity against selected quantity.
                CheckCartQuantity(product, quantity);

                if (product.ShowAddToCart)
                {
                    cartItem.UnitPrice = IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal(product.PromotionalPrice) : IsNotNull(product.UnitPrice) ? Convert.ToDecimal(product.UnitPrice.ToPriceRoundOff()) : 0;
                    cartItem.ExtendedPrice = IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal((product.PromotionalPrice * quantity).ToPriceRoundOff()) : IsNotNull(product.UnitPrice) ? Convert.ToDecimal((product.UnitPrice * quantity).ToPriceRoundOff()) : 0;
                    cartModel.TemplateCartItems.RemoveAll(x => x.ExternalId == guid);
                    cartModel.TemplateCartItems.Insert(indexOfSelectedElement, cartItem);
                    return cartModel;
                }
            }
            return null;
        }

        //It check session template cart items values with database values and return true if both are match
        public virtual bool IsTemplateItemsModified(int omsTemplateId)
        {
            bool flag = false;
            if (omsTemplateId > 0)
            {
                try
                {
                    ExpandCollection expands = new ExpandCollection();
                    expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());
                    expands.Add(ExpandKeys.Pricing);
                    FilterCollection filters = GetRequiredFilters();
                    //Get the account template model.
                    _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                    AccountTemplateModel accountTemplateModel = _accountQuoteClient.GetAccountTemplate(omsTemplateId, expands, filters);
                    if (IsNotNull(accountTemplateModel))
                    {
                        //Maps the model to view model.
                        TemplateViewModel viewModel = accountTemplateModel.ToViewModel<TemplateViewModel>();
                        viewModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                        viewModel.CultureCode = PortalAgent.CurrentPortal?.CultureCode;
                        //Maps the cart items.
                        viewModel.TemplateCartItems = accountTemplateModel.TemplateCartItems?.ToViewModel<TemplateCartItemViewModel>()?.ToList();

                        SetTemplateCartItemModel(viewModel.TemplateCartItems);

                        TemplateViewModel templateViewModelSession = GetFromSession<TemplateViewModel>(GetTemplateCartModelSessionKey());

                        //Get the template line items from session.
                        if (IsNotNull(templateViewModelSession) && omsTemplateId == templateViewModelSession.OmsTemplateId && templateViewModelSession?.TemplateCartItems?.Count == viewModel?.TemplateCartItems?.Count)
                        {
                            for (int index = 0; index < templateViewModelSession?.TemplateCartItems?.Count && index < viewModel?.TemplateCartItems?.Count; ++index)
                            {
                                if (templateViewModelSession.TemplateCartItems[index].SKU == viewModel.TemplateCartItems[index].SKU && templateViewModelSession.TemplateCartItems[index].Quantity == viewModel.TemplateCartItems[index].Quantity)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return flag;
                }
            }
            return flag;
        }

        //Set Filter for template list.
        protected virtual FilterCollection SetFilterForTemplateList(FilterCollection filters,string templateType = null)
        {
            if (IsNull(filters))
            {
                filters = new FilterCollection();
            }

            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == FilterKeys.PortalId);
            filters.RemoveAll(x => x.Item1 == FilterKeys.TemplateType);

            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId.ToString()));
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            if (templateType == null)
            {
                filters.Add(FilterKeys.TemplateType, FilterOperators.Is, WebStoreConstants.OrderTemplate);
            }
            else 
            {
                filters.Add(FilterKeys.TemplateType, FilterOperators.Is, WebStoreConstants.SavedCart);
            }

            return filters;
        }

        //Check available quantity.
        protected virtual ProductViewModel CheckQuantity(string sku, decimal quantity, string addOnSkus)
        {
            //Get available quantity of cart item.
            ProductViewModel product = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>()).GetProductPriceAndInventory(sku, quantity.ToString(), addOnSkus);

            //check if available quantity against selected quantity.
            CheckCartQuantity(product, quantity);

            return product;
        }

        //Get total quantity on hand by sku.
        protected virtual decimal GetQuantityOnHandBySku(string sku, int portalId)
        {
            _publishProductClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ProductInventoryPriceListModel productInventory = _publishProductClient.GetProductPriceAndInventory(new ParameterInventoryPriceModel { Parameter = sku, PortalId = portalId });
            return (productInventory?.ProductList?.Where(w => w.SKU == sku).FirstOrDefault().Quantity).GetValueOrDefault();
        }

        //Update shopping cart item quantity.
        protected virtual CartViewModel UpdatecartItemQuantity(ShoppingCartModel shoppingCart, string guid, decimal quantity, int productId)
        {
            //Update quantity and update the cart.
            if (productId > 0)
            {
                shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(x => x.GroupProducts?.Where(y => y.ProductId == productId)?.Select(z => { z.Quantity = quantity; return z; })?.FirstOrDefault()).ToList();
            }
            else
            {
                shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.Quantity = Convert.ToDecimal(quantity.ToInventoryRoundOff()); return y; }).ToList();
            }

            shoppingCart.IsMerged = true;
            _shoppingCartsClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.CreateCart(shoppingCart);

            //assign values which are required for the estimated shipping calculation.
            if (IsNotNull(shoppingCartModel))
            {
                shoppingCartModel.Shipping = shoppingCart.Shipping?.ShippingId > 0 ? shoppingCart.Shipping : shoppingCartModel.Shipping;
                shoppingCartModel.ShippingAddress = !string.IsNullOrEmpty(shoppingCart.ShippingAddress?.PostalCode) ? shoppingCart.ShippingAddress : shoppingCartModel.ShippingAddress;
            }

            SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCartModel);
            ClearCartCountFromSession();
            return SetCartCurrency(shoppingCartModel);
        }

        //Update template cart item quantity.
        public virtual TemplateViewModel UpdateTemplateItemQuantity(TemplateViewModel shoppingCart, string guid, decimal quantity, int productId)
        {
            //Update quantity and update the cart.
            if (productId > 0)
            {
                shoppingCart.TemplateCartItems?.Where(w => w.ExternalId == guid)?.Select(x => x.GroupProducts?.Where(y => y.ProductId == productId)?.Select(z => { z.Quantity = quantity; return z; })?.FirstOrDefault()).ToList();
            }
            else
            {
                shoppingCart.TemplateCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.Quantity = quantity; return y; }).ToList();
            }

            shoppingCart.IsQuickOrderPad = true;
            shoppingCart.CurrencyCode = PortalAgent.CurrentPortal.CurrencyCode;
            shoppingCart.CultureCode = PortalAgent.CurrentPortal.CultureCode;
            if (shoppingCart.TemplateType == ZnodeConstant.SavedCart) 
                SaveInSession(GetSavedCartModelSessionKey(), shoppingCart); 
            else 
                SaveInSession(GetTemplateCartModelSessionKey(), shoppingCart);

            return shoppingCart;
        }

        //Calculate cart.
        public virtual ShoppingCartModel CalculateCart(ShoppingCartModel shoppingCartModel, bool isCalculateTaxAndShipping = true, bool isCalculateCart= true)
        {
                if (IsNotNull(shoppingCartModel))
                {
                    shoppingCartModel.LocaleId = PortalAgent.LocaleId;
                    shoppingCartModel.IsCalculateTaxAndShipping = isCalculateTaxAndShipping;
                    string billingEmail = shoppingCartModel.BillingEmail;
                    int shippingId = shoppingCartModel.ShippingId;
                    int billingAddressId = shoppingCartModel.BillingAddressId;
                    int shippingAddressId = shoppingCartModel.ShippingAddressId;
                    int selectedAccountUserId = shoppingCartModel.SelectedAccountUserId;
                    shoppingCartModel.ProfileId = Helper.GetProfileId();
                    if (isCalculateCart)
                    {
                        ShoppingCartModel oldShoppingCartModel = shoppingCartModel;
                        shoppingCartModel = _shoppingCartsClient.Calculate(shoppingCartModel);
                        shoppingCartModel.ShippingOptions = oldShoppingCartModel.ShippingOptions;
                        //Bind required data to ShoppingCartModel.
                        BindShoppingCartData(shoppingCartModel, billingEmail, shippingId, shippingAddressId, billingAddressId, selectedAccountUserId);
                        MapQuantityOnHandAndSeoName(oldShoppingCartModel, shoppingCartModel);
                    }
                }
                SaveInSession(WebStoreConstants.CartModelSessionKey, shoppingCartModel);
                return shoppingCartModel;
        }

        //Bind required data to ShoppingCartModel.
        protected virtual void BindShoppingCartData(ShoppingCartModel shoppingCartModel, string billingEmail, int shippingId, int shippingAddressId, int billingAddressId, int selectedAccountUserId)
        {
            shoppingCartModel.BillingEmail = billingEmail;
            shoppingCartModel.ShippingId = shippingId;
            shoppingCartModel.ShippingAddressId = shippingAddressId;
            shoppingCartModel.BillingAddressId = billingAddressId;
            shoppingCartModel.SelectedAccountUserId = selectedAccountUserId;
        }

        //Create cartItem for Group type product.
        protected virtual void GetSelectedGroupedTemplateProducts(TemplateCartItemViewModel cartItem, TemplateViewModel shoppingCart)
        {
            //Get sku's and quantity of associated group products.
            string[] groupProducts = cartItem.GroupProductSKUs.Split(',');
            string[] groupProductsQuantity = cartItem.GroupProductsQuantity.Split('_');
            List<AssociatedProductModel> groupProductsList = cartItem.GroupProducts;
            if (groupProductsQuantity.Count() <= 0)
            {
                groupProductsQuantity = cartItem.GroupProductsQuantity.Split(',');
            }

            cartItem.GroupProducts = new List<AssociatedProductModel>();

            //Get the associated group product skus and their quantities.
            for (int index = 0; index < groupProducts.Length; index++)
            {
                if (!string.IsNullOrEmpty(groupProductsQuantity[index]))
                {
                    cartItem.GroupProducts.Add(new AssociatedProductModel
                    {
                        Quantity = Convert.ToDecimal(groupProductsQuantity[index]),
                        Sku = groupProducts[index],
                        ProductName = groupProductsList.FirstOrDefault(x => x.Sku == groupProducts[index])?.ProductName,
                        UnitPrice = groupProductsList.FirstOrDefault(x => x.Sku == groupProducts[index])?.UnitPrice,
                        ProductId = Convert.ToInt32(groupProductsList.FirstOrDefault(x => x.Sku == groupProducts[index])?.ProductId)
                    });
                }
            }
            shoppingCart.TemplateCartItems.Add(cartItem);
        }

        //Set currency details for shopping cart and cart items.
        protected virtual CartViewModel SetCartCurrency(ShoppingCartModel cartModel)
        {
            if (IsNotNull(cartModel))
            {
                CartViewModel cartViewModel = cartModel.ToViewModel<CartViewModel>();

                // cartViewModel.ShoppingCartItems?.ForEach(item => item.GroupProducts.ForEach(y => y.Quantity = (y.Quantity)));

                string currencyCode = PortalAgent.CurrentPortal.CurrencyCode;
                string cultureCode = PortalAgent.CurrentPortal.CultureCode;
                cartViewModel.CultureCode = cultureCode;
                cartViewModel.CurrencyCode = currencyCode;
                cartViewModel.ZipCode = cartModel.ShippingAddress?.PostalCode;
                cartViewModel.ShoppingCartItems.Select(x => { x.CurrencyCode = currencyCode; x.CultureCode = cultureCode; return x; })?.ToList();
                cartViewModel.TaxCost = 0;
                return cartViewModel;
            }
            return new CartViewModel();
        }
        //get calculated shopping cart
        public virtual CartViewModel CalculateCart()
        {
            ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            ShoppingCartModel model = CalculateCart(shoppingCartModel, false, true);

            return SetCartCurrency(model);
        }

        //Check whether approval routing is required for the current user quote.
        protected virtual bool IsRequireApprovalRouting(int quoteId, decimal quoteTotal)
        {
            bool IsRequireApprovalRouting = false;

            UserApproverListViewModel model = GetUserApproverList(0, true);
            if (model?.UserApprover?.Count > 0)
            {
                int firstLevelOrder = model.UserApprover.Min(x => x.ApproverOrder);
                decimal? firstLevelBudgetStart = model.UserApprover.Where(x => x.ApproverOrder == firstLevelOrder)?.Select(x => x.FromBudgetAmount)?.FirstOrDefault();
                if (IsNotNull(firstLevelBudgetStart) && quoteTotal > firstLevelBudgetStart)
                {
                    IsRequireApprovalRouting = true;
                }
            }
            return IsRequireApprovalRouting;
        }

        //Get user view model from session.
        public virtual UserViewModel GetUserViewModelFromSession()
        {
            return GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
        }

        //Get the list of user approvers.
        public virtual UserApproverListViewModel GetUserApproverList(int omsQuoteId, bool showAllApprovers)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));
            filters.Add(new FilterTuple(ZnodeOmsTemplateEnum.UserId.ToString(), FilterOperators.Equals, GetUserViewModelFromSession()?.UserId.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.ShowAllApprovers.ToString().ToLower(), FilterOperators.Equals, showAllApprovers.ToString()));

            UserApproverListModel listModel = _accountQuoteClient.GetUserApproverList(null, filters, null, null, null);
            return new UserApproverListViewModel { UserApprover = listModel?.UserApprovers?.ToViewModel<UserApproverViewModel>()?.ToList() };
        }

        //Map On Hand Quantity And Seo Name
        public virtual void MapQuantityOnHandAndSeoName(ShoppingCartModel sourceShoppingCartModel, ShoppingCartModel destShoppingCartModel)
        {
            foreach (ShoppingCartItemModel sourceItem in sourceShoppingCartModel.ShoppingCartItems)
            {
                //Add Product Inventory check needs to be introduced.
                if (!string.IsNullOrEmpty(sourceItem.AddOnProductSKUs))
                {
                    sourceItem.QuantityOnHand = 0;
                }
                //If product is an configurable product.
                if (!string.IsNullOrEmpty(sourceItem.ConfigurableProductSKUs))
                {
                    MapQuantityOnHandAndSeoNameForConfigurableProduct(sourceItem, destShoppingCartModel);
                }
                //If product is an group product.
                else if (IsNotNull(sourceItem.GroupProducts) && sourceItem.GroupProducts.Count > 0)
                {
                    MapQuantityOnHandAndSeoNameForGroupProduct(sourceItem, destShoppingCartModel);
                }
                //If product is an simple/bundle product.
                else
                {
                    MapQuantityOnHandAndSeoNameForSimpleAndBundleProduct(sourceItem, destShoppingCartModel);
                }
            }
        }

        // Create cart for configurable Products.
        public virtual void GetSelectedConfigurableProductsForAddToCart(AddToCartViewModel cartItem)
        {
            
            string[] configurableProducts = cartItem.ConfigurableProductSKUs?.Split(',')?.Where(s => !string.IsNullOrWhiteSpace(s))?.ToArray();
            string[] configurableProductsQuantity = cartItem.ConfigurableProductQuantity?.Split('_')?.Where(s => !string.IsNullOrWhiteSpace(s))?.ToArray();

            for (int index = 0; index < configurableProducts?.Length; index++)
            {
                cartItem.Quantity = Convert.ToDecimal(configurableProductsQuantity[index]);
                cartItem.ConfigurableProductSKUs = configurableProducts[index];
             
                if (IsNotNull(cartItem))
                {
                    cartItem.ShoppingCartItems.Add(cartItem.ToModel<ShoppingCartItemModel>());
                }
            }

            cartItem.ConfigurableProductSKUs = String.Join(",", configurableProducts);
        }

        //Update quantity and personalize value of a product.
        protected virtual void UpdateProductDetails(ShoppingCartItemModel guestCartItem, ShoppingCartItemModel loginCartModel)
        {
            loginCartModel.PersonaliseValuesList = guestCartItem.PersonaliseValuesList;
        }

        //Update quantity and personalize value for configurable product.
        protected virtual void UpdateConfigurableProduct(ShoppingCartItemModel guestCartItem, ShoppingCartItemModel loginCartModel)
        {
            if (Equals(loginCartModel.ConfigurableProductSKUs, guestCartItem.ConfigurableProductSKUs))
            {
                UpdateProductDetails(guestCartItem, loginCartModel);
            }
        }

        protected virtual bool CheckIfGrouProductAvailable(string sku, string addonSku, List<ShoppingCartItemModel> shoppingCartItems)
        {
            if (shoppingCartItems.Count > 0)
            {
                return Convert.ToBoolean(shoppingCartItems.Where(sc => sc?.GroupProducts?.FirstOrDefault()?.Sku == sku && sc.AddOnProductSKUs == addonSku).ToList().Count);
            }
            else
            {
                return false;
            }
        }

        //Apply coupon code.
        protected virtual CartViewModel ApplyCoupon(string couponCode, ShoppingCartModel cartModel)
        {
            ShoppingCartModel oldShoppingCartModel = cartModel;

            //Get coupons that are applied to cart.
            List<CouponModel> promocode = GetCoupons(cartModel);

            List<CouponViewModel> invalidCoupons = new List<CouponViewModel>();

            //Bind new coupon to shopping cart model to apply.
            if (!string.IsNullOrEmpty(couponCode))
            {
                GetNewCouponToApply(cartModel, couponCode);
            }
            else
            {
                invalidCoupons.Add(new CouponViewModel { Code = couponCode, PromotionMessage = WebStore_Resources.ErrorEmptyCouponCode, CouponApplied = false, CouponValid = false });
            }

            //Get invalid coupon list form cart.
            List<CouponModel> invalidCouponslist = GetInvalidCouponList(cartModel);

            RemoveInvalidCoupon(cartModel);
            //Calculate coupon for the cart.
            ShoppingCartModel calculatedCart = _shoppingCartsClient.Calculate(cartModel);

            if (IsNotNull(calculatedCart) && IsNotNull(cartModel))
            {
                cartModel.Coupons = calculatedCart.Coupons;
                cartModel.Total = calculatedCart.Total;
                cartModel.Discount = calculatedCart.Discount;
                cartModel.ShippingCost = calculatedCart.ShippingCost;
                cartModel.GiftCardAmount = calculatedCart.GiftCardAmount;
                cartModel.TaxCost = calculatedCart.TaxCost;
                cartModel.OrderLevelDiscountDetails = calculatedCart.OrderLevelDiscountDetails;
                cartModel.OrderTotalWithoutVoucher = calculatedCart.OrderTotalWithoutVoucher;
                cartModel.Shipping = calculatedCart.Shipping;
                cartModel.ShippingDiscount = calculatedCart.ShippingDiscount;

                if(IsNotNull(cartModel.ShoppingCartItems) && IsNotNull(calculatedCart.ShoppingCartItems))
                    SetCalculatedDetailsInLineItem(cartModel.ShoppingCartItems, calculatedCart.ShoppingCartItems);
            }

            if (IsNotNull(invalidCouponslist))
                cartModel.Coupons?.AddRange(invalidCouponslist);

            MapQuantityOnHandAndSeoName(oldShoppingCartModel, cartModel);
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);

            CartViewModel cartViewModel = calculatedCart?.ToViewModel<CartViewModel>();
            //Bind User details to ShoppingCartModel.
            cartViewModel.CustomerPaymentGUID = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.CustomerPaymentGUID;
            return cartViewModel;
        }

        //To set orders discount details in shopping cart items.
        protected virtual void SetCalculatedDetailsInLineItem(List<ShoppingCartItemModel> sessionCartModelItems, List<ShoppingCartItemModel> calculatedCartModelItems)
        {
            foreach (ShoppingCartItemModel item in calculatedCartModelItems)
            {
                if (IsNotNull(item.Product) && IsNotNull(sessionCartModelItems.FirstOrDefault(x => x.Product.PublishProductId == item.Product.PublishProductId)?.Product))
                {
                    MapDetailsInLineItem(sessionCartModelItems.FirstOrDefault(x => x.Product.PublishProductId == item.Product.PublishProductId), item);
                }
            }
        }

        //To map calculated cart line item details.
        protected virtual void MapDetailsInLineItem(ShoppingCartItemModel sessionLineItemDetails, ShoppingCartItemModel calculatedLineItemDetails)
        {
            sessionLineItemDetails.Product.OrdersDiscount = calculatedLineItemDetails.Product.OrdersDiscount;
            sessionLineItemDetails.Product.PST = calculatedLineItemDetails.Product.PST;
            sessionLineItemDetails.Product.GST = calculatedLineItemDetails.Product.GST;
            sessionLineItemDetails.Product.VAT = calculatedLineItemDetails.Product.VAT;
            sessionLineItemDetails.Product.HST = calculatedLineItemDetails.Product.HST;
            sessionLineItemDetails.Product.DiscountAmount = calculatedLineItemDetails.Product.DiscountAmount;
            sessionLineItemDetails.Product.SalesTax = calculatedLineItemDetails.Product.SalesTax;
            sessionLineItemDetails.Product.ShippingCost = calculatedLineItemDetails.Product.ShippingCost > 0 ? calculatedLineItemDetails.Product.ShippingCost : calculatedLineItemDetails.ShippingCost;
            sessionLineItemDetails.PromotionalPrice = calculatedLineItemDetails.PromotionalPrice;
        }

        //Add to promoCodes when shopping cart having coupons already.
        protected virtual List<CouponModel> GetCoupons(ShoppingCartModel cartModel)
        {
            return cartModel.Coupons?.Where(x => x.CouponApplied)?.Select(y => y)?.ToList();
        }

        //Get new coupon code and added to cart.
        protected virtual void GetNewCouponToApply(ShoppingCartModel cartModel, string couponCode)
        {
            //Check if any coupon is already appled to cart and add new coupon.
            bool isCouponNotAvailableInCart = Equals(cartModel.Coupons?.Where(p => Equals(p.Code, couponCode)).ToList().Count, 0);

            //Add coupon code if not present in cart.
            if (isCouponNotAvailableInCart)
            {
                cartModel.Coupons.Add(new CouponModel { Code = couponCode });
            }
        }

        //Apply gift card.
        [Obsolete]
        protected virtual CartViewModel ApplyGiftCard(string giftCardNumber, ShoppingCartModel cartModel)
        {
            ShoppingCartModel oldShoppingCartModel = cartModel;

            cartModel.GiftCardNumber = giftCardNumber;
            //calculate price of shopping cart after gift card applied.
            ShoppingCartModel calculatedCart = _shoppingCartsClient.Calculate(cartModel);
            cartModel.Coupons = calculatedCart?.Coupons;
            cartModel.Total = calculatedCart?.Total;
            cartModel.Discount = Convert.ToDecimal(calculatedCart?.Discount);
            cartModel.ShippingCost = Convert.ToDecimal(calculatedCart?.ShippingCost);
            cartModel.GiftCardAmount=calculatedCart.GiftCardAmount;
            cartModel.GiftCardBalance=calculatedCart.GiftCardBalance;
            cartModel.GiftCardMessage = calculatedCart.GiftCardMessage;
            cartModel.GiftCardNumber = calculatedCart.GiftCardNumber;
            cartModel.GiftCardValid = calculatedCart.GiftCardValid;
            MapQuantityOnHandAndSeoName(oldShoppingCartModel, cartModel);
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);

            CartViewModel cartViewModel = calculatedCart?.ToViewModel<CartViewModel>();
            if (IsNotNull(cartViewModel))
            {
                cartViewModel.SuccessMessage = cartViewModel.GiftCardMessage;
            }
            //Bind User details to ShoppingCartModel.
            cartViewModel.CustomerPaymentGUID = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.CustomerPaymentGUID;
            return cartViewModel;
        }

        //Set template model.
        public virtual void SetTemplateModel(AccountTemplateModel accountTemplateModel, string TemplateType)
        {
            accountTemplateModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            accountTemplateModel.LocaleId = PortalAgent.LocaleId;
            accountTemplateModel.PublishedCatalogId = PortalAgent.CurrentPortal.PublishCatalogId;
            accountTemplateModel.UserId = Convert.ToInt32(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId);
            if (TemplateType == WebStoreConstants.SavedCart)
                accountTemplateModel.TemplateType = TemplateType;
            else
                accountTemplateModel.TemplateType = WebStoreConstants.OrderTemplate;
        }

        //Set the Tool Menus for Template List Grid View.
        protected virtual void SetTemplateListToolMenu(TemplateListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = $"User.prototype.DeleteTemplate(this)", ControllerName = "User", ActionName = "DeleteTemplate" });
            }
        }

        //Sets the template cart item model.
        protected virtual void SetTemplateCartItemModel(List<TemplateCartItemViewModel> templateCartItems, string templateType = null)
        {
            //Check for null check.
            if (templateCartItems?.Count > 0)
            {
                IProductAgent productAgent = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>());
                ProductViewModel product;
                foreach (var cartItem in templateCartItems)
                {
                    if (cartItem.GroupProducts?.Count > 0)
                    {
                        foreach (AssociatedProductModel item in cartItem.GroupProducts)
                        {
                            product = productAgent.GetProductPriceAndInventory(item.Sku, item.Quantity.ToString(), "");
                            if (IsNotNull(product))
                                SetProductDetailsForGroupProduct(item, product);

                            if(templateType != ZnodeConstant.OrderTemplate)
                                cartItem.ExtendedPrice += item.UnitPrice.GetValueOrDefault() * (item.Quantity);
                        }
                    }

                    //Sets the properties.
                    product = productAgent.GetProductPriceAndInventory(cartItem.SKU, cartItem.Quantity.ToString(), cartItem.AddOnProductSKUs);
                    if (IsNotNull(product) && templateType == ZnodeConstant.SavedCart)
                        SetProductDetails(cartItem, product, ZnodeConstant.SavedCart);

                    else
                        SetProductDetails(cartItem, product);
                }
            }
        }

        protected  void SetProductDetailsForGroupProduct (AssociatedProductModel associatedProduct, ProductViewModel product)
        {
            if (IsNotNull(product))
            {
                associatedProduct.ProductId = product.PublishProductId;
                associatedProduct.UnitPrice = IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal(product.PromotionalPrice) : IsNotNull(product.UnitPrice) ? Convert.ToDecimal(product.UnitPrice.ToPriceRoundOff()) : 0;
                associatedProduct.ProductName = product.Name;
                associatedProduct.MaxQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                associatedProduct.MinQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MinimumQuantity));
                associatedProduct.OutOfStockMessage = string.Format(WebStore_Resources.ExceedingAvailableQuantity, product.DefaultInventoryCount);
                associatedProduct.IsActive = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsActive));
                associatedProduct.IsObsolete = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsObsolete));
                associatedProduct.DefaultInventoryCount = product.DefaultInventoryCount;

            }
        }

        //Maps the product details in TemplateCartItemViewModel.
        protected virtual void SetProductDetails(TemplateCartItemViewModel cartItem, ProductViewModel product ,string templateType = null)
        {
            if (IsNotNull(product) && product.PublishProductId > 0)
            {
                ISaveForLaterAgent cartAgent = new SaveForLaterAgent(GetClient<PublishProductClient>(), GetClient<SaveForLaterClient>(), GetClient<ShoppingCartClient>());
                cartAgent.SetUnitPriceWithAddon(cartItem, product);

                if (templateType != ZnodeConstant.SavedCart || (cartItem.BundleProductSKUs?.Length) > 0)
                {
                    cartItem.CartDescription = GetCartDescription(product);
                }
                //Maps the data properly.
                cartItem.ProductId = product.ConfigurableProductId > 0 ? product.ConfigurableProductId.ToString() : product.PublishProductId.ToString();
                cartItem.UnitPrice = IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal(product.PromotionalPrice) : IsNotNull(product.UnitPrice) ? Convert.ToDecimal(product.UnitPrice.ToPriceRoundOff()) : 0;
                cartItem.ExtendedPrice = cartItem?.GroupProducts?.Count() > 0 ? Convert.ToDecimal(cartItem.ExtendedPrice.ToPriceRoundOff()) : IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal((product?.PromotionalPrice.GetValueOrDefault() * cartItem.Quantity).ToPriceRoundOff()) : Convert.ToDecimal((product?.UnitPrice.GetValueOrDefault() * cartItem.Quantity).ToPriceRoundOff());
                cartItem.ProductName = product.Name;
                cartItem.ImagePath = product.ImageSmallPath;
                cartItem.SEODescription = product.SEODescription;
                cartItem.SEOKeywords = product.SEOKeywords;
                cartItem.SEOTitle = product.SEOTitle;
                cartItem.SEOUrl = product.SEOUrl;
                cartItem.MaxQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                cartItem.MinQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MinimumQuantity));
                cartItem.AllowBackOrder = product.Attributes.Where(y => y.AttributeCode == ZnodeConstant.OutOfStockOptions).FirstOrDefault().SelectValues.Where(x => x.Code == ZnodeConstant.AllowBackOrdering).Any();
                if(product.DefaultInventoryCount == null)
                    cartItem.OutOfStockMessage = !string.Equals(product.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.Code, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase) ? string.Format(WebStore_Resources.ExceedingAvailableQuantity, "0") : ZnodeConstant.DontTrackInventory;
                else
                    cartItem.OutOfStockMessage = !string.Equals(product.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.Code, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase) ? string.Format(WebStore_Resources.ExceedingAvailableQuantity, product.DefaultInventoryCount) : ZnodeConstant.DontTrackInventory;

                if ((templateType == ZnodeConstant.SavedCart || templateType == ZnodeConstant.SaveForLater) && product.Quantity > 0)
                {
                    product.DefaultInventoryCount = Convert.ToString(product.Quantity);
                    cartItem.OutOfStockMessage = null;
                }
                cartItem.IsActive = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsActive));
                cartItem.IsObsolete = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsObsolete));
                cartItem.DefaultInventoryCount = product.DefaultInventoryCount == null ? "0" : product?.DefaultInventoryCount;
                if (!string.IsNullOrEmpty(product?.ConfigurableProductSKU) && templateType != ZnodeConstant.SavedCart)
                {
                    cartItem.SKU = product.SKU;
                    cartItem.ConfigurableProductSKUs = string.Join(",", product?.Attributes?.Where(x => x.AttributeCode == "SKU").Select(x => x.AttributeValues).Distinct());
                    cartItem.CartDescription = string.Join("<br>", product?.Attributes?.Where(x => x.IsConfigurable).Select(x => x.AttributeName + " - " + x.AttributeValues));
                }       
            }
        }

        //Remove Shipping and Tax calculation From Cart.
        protected virtual void RemoveShippingTaxFromCart(ShoppingCartModel shoppingCartModel)
        {
            shoppingCartModel.ShippingAddress = IsNotNull(shoppingCartModel.ShippingAddress) ? shoppingCartModel.ShippingAddress : null;
            shoppingCartModel.BillingAddress = null;
            shoppingCartModel.Shipping = new OrderShippingModel() { ShippingId = shoppingCartModel.ShippingId > 0 ? shoppingCartModel.ShippingId : 0, ShippingCountryCode = "us" };
            shoppingCartModel.Payment = new PaymentModel();
            shoppingCartModel.BillingEmail = null;
            RemoveTaxFromCart(shoppingCartModel);
            ResetShippingCostForCartItemsAndAddOns(shoppingCartModel);
        }

        protected virtual void ResetShippingCostForCartItemsAndAddOns(ShoppingCartModel shoppingCartModel)
        {
            shoppingCartModel.ShippingCost = 0;
            shoppingCartModel.ShippingHandlingCharges = 0;
            shoppingCartModel.ShippingDiscount = 0;
            shoppingCartModel.OrderLevelShipping = 0;
            foreach (ShoppingCartItemModel cartItem in shoppingCartModel.ShoppingCartItems)
            {
                // Reset each line item shipping cost
                cartItem.ShippingCost = 0;
                if (HelperUtility.IsNotNull(cartItem.Product))
                {
                    cartItem.Product.ShippingCost = 0;
                }
            }
        }

        //Remove Tax calculation From Cart.
        protected virtual void RemoveTaxFromCart(ShoppingCartModel shoppingCartModel)
        {
            shoppingCartModel.TaxCost = 0;
            shoppingCartModel.SalesTax = 0;
            shoppingCartModel.TaxRate = 0;
            shoppingCartModel.Vat = 0;
            shoppingCartModel.Pst = 0;
            shoppingCartModel.Hst = 0;
            shoppingCartModel.Gst = 0;
            shoppingCartModel.OrderLevelTaxes = 0;
        }

        //Convert the shipping rate to currency formatted rate.
        protected virtual ShippingOptionListViewModel GetCurrencyFormattedRates(ShippingOptionListViewModel lstViewModel)
        {
            ShippingOptionListViewModel model = new ShippingOptionListViewModel();
            List<ShippingOptionViewModel> lstModel = new List<ShippingOptionViewModel>();
            string cultureCode = PortalAgent.CurrentPortal.CultureCode;
            if (lstViewModel?.ShippingOptions?.Count > 0)
            {
                foreach (ShippingOptionViewModel shipping in lstViewModel?.ShippingOptions)
                {
                    shipping.FormattedShippingRate = Helper.FormatPriceWithCurrency(shipping.ShippingRate, cultureCode);
                    lstModel.Add(shipping);
                }
            }

            model.ShippingOptions = lstModel;
            return model;
        }

        //to remove invalid coupon form cart 
        protected virtual void RemoveInvalidCoupon(ShoppingCartModel cartModel)
        {
            if (cartModel.Coupons?.Count > 0)
            {
                cartModel.Coupons.RemoveAll(x => !x.CouponApplied && !string.IsNullOrEmpty(x.PromotionMessage));
            }
        }

        //Get template cart model session key.
        protected virtual string GetTemplateCartModelSessionKey()
        {
            //GetCurrent user's Id.
            int userId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId;
            return WebStoreConstants.TemplateCartModelSessionKey + userId;
        }

        public virtual string GetSavedCartModelSessionKey()
        {
            //GetCurrent user's Id.
            int userId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey).UserId;
            return WebStoreConstants.SavedCartModelSessionKey + userId;
        }

        //Convert string type to decimal for quantity.
        protected virtual decimal ModifyQuantityValue(string quantity)
        {
            decimal newQuantity = 0;
            if (quantity.Contains(","))
            {
                newQuantity = Convert.ToDecimal((quantity).Replace(",", "."));
            }
            else
            {
                newQuantity = decimal.Parse(quantity, CultureInfo.InvariantCulture);
            }

            return newQuantity;
        }

        //Map AddToCartViewModel to ShoppingCartModel
        protected virtual ShoppingCartModel MapAddToCartToShoppingCart(AddToCartViewModel addToCartViewModel, ShoppingCartModel shoppingCartModel)
        {
            if (IsNotNull(addToCartViewModel?.Coupons) && IsNotNull(shoppingCartModel?.ShoppingCartItems))
            {
                shoppingCartModel.Coupons = new List<CouponModel>();
                shoppingCartModel.Coupons.AddRange(addToCartViewModel.Coupons);
            }
            if (IsNotNull(addToCartViewModel?.ShoppingCartItems) && IsNotNull(shoppingCartModel?.ShoppingCartItems))
            {
                shoppingCartModel.ShoppingCartItems = null;
            }
            shoppingCartModel.ExternalId = addToCartViewModel.ExternalId;
            shoppingCartModel.PublishedCatalogId = addToCartViewModel.PublishedCatalogId;
            shoppingCartModel.LocaleId = addToCartViewModel.LocaleId;
            shoppingCartModel.UserId = addToCartViewModel.UserId;
            shoppingCartModel.PortalId = addToCartViewModel.PortalId;
            shoppingCartModel.CookieMappingId = addToCartViewModel.CookieMappingId;
            shoppingCartModel.ShippingId = addToCartViewModel.ShippingId;
            return shoppingCartModel;
        }


        //Get details from shoppincart session which are required to calculate Estimated shipping cost.
        protected virtual ShoppingCartModel GetEstimatedShippingDetails(ShoppingCartModel shoppingCart)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            if (IsNotNull(cartModel))
            {
                shoppingCart.ShippingAddress = IsNotNull(cartModel.ShippingAddress) ? cartModel.ShippingAddress : shoppingCart.ShippingAddress;
                shoppingCart.Shipping = cartModel.Shipping?.ShippingId > 0 ? cartModel.Shipping : shoppingCart.Shipping;
                shoppingCart.ShippingId = cartModel.ShippingId;
            }
            return shoppingCart;
        }


        //Bind user details to AccountQuoteViewModel.
        protected virtual AccountQuoteViewModel BindDataToAccountQuoteViewModel(UserViewModel userDetails, AccountQuoteModel accountQuoteModel)
        {
            AccountQuoteViewModel accountQuoteViewModel = accountQuoteModel?.ToViewModel<AccountQuoteViewModel>();
            if (IsNotNull(accountQuoteViewModel))
            {
                accountQuoteViewModel.CurrencyCode = DefaultSettingHelper.DefaultCurrency;
                accountQuoteViewModel.RoleName = userDetails?.RoleName;
                accountQuoteViewModel.PermissionCode = userDetails?.PermissionCode;
                accountQuoteViewModel.CartItemCount = Convert.ToString(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems.Count()) ??
                          GetFromCookie(WebStoreConstants.CartCookieKey);
                PortalViewModel portal = PortalAgent.CurrentPortal;
                accountQuoteViewModel.CurrencyCode = portal?.CurrencyCode;

                if (accountQuoteModel.ShoppingCart?.ShoppingCartItems?.Count() > 0)
                {
                    //If inventory is out of stock set error message.
                    if (accountQuoteModel.ShoppingCart.ShoppingCartItems.Any(x => x.InsufficientQuantity))
                    {
                        accountQuoteViewModel.OutOfStockMessage = portal?.OutOfStockMessage;
                    }
                }
                accountQuoteViewModel.LoggedInUserId = GetUserId();
                accountQuoteViewModel.IsLastApprover = _accountQuoteClient.IsLastApprover(accountQuoteModel.OmsQuoteId);
                accountQuoteModel.ShoppingCart.Shipping.ShippingName = accountQuoteModel.ShippingName;
            }
            return accountQuoteViewModel;
        }

        //Get count of cart items in shopping cart on cartCountAfterLogin flag status
        protected virtual decimal ReturnCartCount(bool cartCountAfterLogin = false)
        {
            string cookieValue = GetFromCookie(WebStoreConstants.CartCookieKey);
            //If user is guest and CookieMappingId is not available, then method should return 0.
            if (string.IsNullOrEmpty(cookieValue) && (!cartCountAfterLogin && HttpContext.Current.User.Identity.IsAuthenticated == false))
                return 0;

            string count = _shoppingCartsClient.GetCartCount(new CartParameterModel
            {
                CookieMappingId = cookieValue,
                LocaleId = PortalAgent.LocaleId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                PublishedCatalogId = GetCatalogId().GetValueOrDefault(),
                UserId = cartCountAfterLogin ? (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId) : ((HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated) ? GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId : null)
            });
            if (string.IsNullOrEmpty(count)) count = "0";

            return Convert.ToDecimal(Helper.GetRoundOffQuantity(count));
        }

        //Set portal approval type in the cart model
        protected virtual void SetPortalApprovalType(CartViewModel cartViewModel)
        {
            if (PortalAgent.CurrentPortal.EnableApprovalManagement && IsNotNull(cartViewModel))
            {
                PortalApprovalModel portalApprovalModel = GetClient<PortalClient>().GetPortalApproverDetailsById(PortalAgent.CurrentPortal.PortalId);
                cartViewModel.ApprovalType = portalApprovalModel?.PortalApprovalTypeName;
            }
        }

        //Check if Shopping cart session is null or not
        protected virtual bool IsShoppingCartInSession(ShoppingCartModel model)
        {
            if(IsNull(model) || IsNull(model?.ShoppingCartItems) || model?.ShoppingCartItems?.Count < 1 )
            {
                return false;
            }
            return true;
        }

        //Apply Voucher code.
        protected virtual CartViewModel ApplyVoucher(string voucherNumber, ShoppingCartModel cartModel)
        {
            RemoveInvalidVouchers(cartModel);
            //Bind new coupon to shopping cart model to apply.
            if (!string.IsNullOrEmpty(voucherNumber))
            {
                AddNewVoucherToApply(cartModel, voucherNumber);
            }
            //Calculate coupon for the cart.
            ShoppingCartModel calculatedCart = _shoppingCartsClient.Calculate(cartModel);
            cartModel.Vouchers = calculatedCart?.Vouchers;
            cartModel.GiftCardAmount = calculatedCart.GiftCardAmount;
            cartModel.Total = Convert.ToDecimal(calculatedCart.Total.ToPriceRoundOff());
            cartModel.OrderTotalWithoutVoucher = Convert.ToDecimal(calculatedCart.OrderTotalWithoutVoucher.ToPriceRoundOff());
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);

            CartViewModel cartViewModel = calculatedCart?.ToViewModel<CartViewModel>();
            //Bind User details to ShoppingCartModel.
            cartViewModel.CustomerPaymentGUID = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.CustomerPaymentGUID;
            return cartViewModel;
        }

        //Get new voucher code and added to cart.
        protected virtual void AddNewVoucherToApply(ShoppingCartModel cartModel, string voucherNumber)
        {
            //Check if any voucher is already appled to cart and add new coupon.
            bool isCouponNotAvailableInCart = Equals(cartModel.Vouchers?.Where(p => Equals(p.VoucherNumber, voucherNumber)).ToList().Count, 0);

            //Add voucher code if not present in cart.
            if (isCouponNotAvailableInCart)
            {
                cartModel.Vouchers.Add(new VoucherModel { VoucherNumber = voucherNumber });
            }
        }

        // Remove invalid vouchers
        protected virtual void RemoveInvalidVouchers(ShoppingCartModel cartModel)
        {
            if (cartModel.Vouchers?.Count > 0)
            {
                cartModel.Vouchers.RemoveAll(x => !x.IsVoucherApplied && !string.IsNullOrEmpty(x.VoucherMessage));
            }
        }

        //Check whether shipping coupon is removed or not.
        protected virtual bool IsShippingBasedCoupon(List<CouponModel> coupons, string couponCode)
        {
            string couponPromotionType = coupons.FirstOrDefault(x => string.Equals(x.Code, couponCode, StringComparison.InvariantCultureIgnoreCase))?.CouponPromotionType;

            if (!string.IsNullOrEmpty(couponPromotionType) && (string.Equals(couponPromotionType, ZnodeConstant.AmountOffShipping, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(couponPromotionType, ZnodeConstant.AmountOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(couponPromotionType, ZnodeConstant.PercentOffShipping, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(couponPromotionType, ZnodeConstant.PercentOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }
            return false;
        }

        //Get invalid coupon list form cart.
        protected virtual List<CouponModel> GetInvalidCouponList(ShoppingCartModel cartModel)
        {
            if (cartModel.Coupons?.Count > 0)
            {
                List<CouponModel> invalidCoupons = new List<CouponModel>();
                return invalidCoupons = cartModel.Coupons.Where(x => !x.CouponApplied && !string.IsNullOrEmpty(x.PromotionMessage))?.ToList();
            }
            return null;
        }

        //Map quantity on hand and seo name for configurable product.
        private void MapQuantityOnHandAndSeoNameForConfigurableProduct(ShoppingCartItemModel sourceItem, ShoppingCartModel destShoppingCartModel)
        {
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.ConfigurableProductSKUs.Length > 0 && x.ConfigurableProductSKUs == sourceItem.ConfigurableProductSKUs).QuantityOnHand = sourceItem.QuantityOnHand;
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.ConfigurableProductSKUs.Length > 0 && x.ConfigurableProductSKUs == sourceItem.ConfigurableProductSKUs).SeoPageName = sourceItem.SeoPageName;
        }

        //Map quantity on hand and seo name for group product.
        private void MapQuantityOnHandAndSeoNameForGroupProduct(ShoppingCartItemModel sourceItem, ShoppingCartModel destShoppingCartModel)
        {
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.GroupProducts.Count() > 0 && x.GroupProducts.FirstOrDefault().Sku == sourceItem.GroupProducts.FirstOrDefault().Sku).QuantityOnHand = sourceItem.QuantityOnHand;
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.GroupProducts.Count() > 0 && x.GroupProducts.FirstOrDefault().Sku == sourceItem.GroupProducts.FirstOrDefault().Sku).SeoPageName = sourceItem.SeoPageName;
        }

        //Map quantity on hand and seo name for simple and bundle product.
        private void MapQuantityOnHandAndSeoNameForSimpleAndBundleProduct(ShoppingCartItemModel sourceItem, ShoppingCartModel destShoppingCartModel)
        {
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.SKU == sourceItem.SKU).QuantityOnHand = sourceItem.QuantityOnHand;
            destShoppingCartModel.ShoppingCartItems.FirstOrDefault(x => x.SKU == sourceItem.SKU).SeoPageName = sourceItem.SeoPageName;
        }

        // To mark specified shipping option as selected in shopping cart model and to specify shipping calculations should be skipped or not.
        // This method can be overridden to change its behavior.
        protected virtual void SetShippingOptionSelectedById(ShoppingCartModel shoppingCart, int shippingOptionId)
        {
            if (shippingOptionId > 0 && IsNotNull(shoppingCart?.ShippingOptions) && shoppingCart?.ShippingOptions.Count > 0)
            {
                // If this property is marked as true then call to shipping agent will be skipped.
                shoppingCart.SkipShippingCalculations = true;
                shoppingCart.ShippingOptions.ForEach(x => x.IsSelected = false);
                shoppingCart.ShippingOptions.FirstOrDefault(x => x.ShippingId == shippingOptionId).IsSelected = true;
            }
        }

        // To post the data to klaviyo
        protected virtual void PostDataToKlaviyo(AddToCartViewModel cartItem, HttpContext httpContext)
        {
            HttpContext.Current = httpContext;
            IKlaviyoClient _klaviyoClient = GetComponentClient<IKlaviyoClient>(GetService<IKlaviyoClient>());
            KlaviyoProductDetailModel klaviyoProductDetailModel = MapShoppingCartModelToKlaviyoModel(cartItem);
            bool isTrackKlaviyo = _klaviyoClient.TrackKlaviyo(klaviyoProductDetailModel);
        }

        // Map the order model to klaviyo model
        protected virtual KlaviyoProductDetailModel MapShoppingCartModelToKlaviyoModel(AddToCartViewModel cartItem)
        {
            KlaviyoProductDetailModel klaviyoProductDetailModel = new KlaviyoProductDetailModel();
            klaviyoProductDetailModel.OrderLineItems = new List<OrderLineItemDetailsModel>();
            klaviyoProductDetailModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (HelperUtility.IsNull(userViewModel))
                userViewModel = _userClient.GetUserDetailById(cartItem.UserId.GetValueOrDefault(), cartItem.PortalId)?.ToViewModel<UserViewModel>();
            klaviyoProductDetailModel.FirstName = userViewModel?.FirstName;
            klaviyoProductDetailModel.LastName = userViewModel?.LastName;
            klaviyoProductDetailModel.Email = userViewModel?.Email;
            klaviyoProductDetailModel.StoreName = PortalAgent.CurrentPortal.Name;
            klaviyoProductDetailModel.PropertyType = (int)KlaviyoEventType.AddToCartEvent;
            klaviyoProductDetailModel.OrderLineItems.Add(new OrderLineItemDetailsModel { ProductName = cartItem.ShoppingCartItems.FirstOrDefault().ProductName, SKU = cartItem?.SKU, Quantity = cartItem.Quantity, UnitPrice = cartItem.ShoppingCartItems.FirstOrDefault().UnitPrice, Image = cartItem.Image });
            return klaviyoProductDetailModel;
        }
        #endregion
    }
}

