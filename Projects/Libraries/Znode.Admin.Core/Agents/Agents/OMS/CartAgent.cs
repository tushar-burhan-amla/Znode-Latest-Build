using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class CartAgent : BaseAgent, ICartAgent
    {
        #region Private member

        private readonly IShoppingCartClient _shoppingCartsClient;
        private readonly IOrderStateClient _orderStateClient;
        private readonly IPortalClient _portalClient;
        private readonly IUserClient _userClient;
        #endregion Private member

        #region Constructor

        public CartAgent(IShoppingCartClient shoppingCartClient, IPublishProductClient publishProductClient, IOrderStateClient orderStateClient, IPortalClient portalClient, IUserClient userClient)
        {
            _shoppingCartsClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
            _portalClient = portalClient;
            _userClient = GetClient<IUserClient>(userClient);
        }

        #endregion Constructor

        #region Public methods

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use AddToCartProduct")]
        //Create/Update new shopping cart.
        public virtual CartViewModel CreateCart(CartItemViewModel cartItem)
        {

            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(cartItem))
            {
                ZnodeLogging.LogMessage("Input parameter CartItemViewModel having:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { UserId = cartItem.UserId });
                SetCreatedByUser(cartItem.UserId);
                ShoppingCartModel cartModel = new ShoppingCartModel();
                //If shopping cart is null then return shoppingCartModel with PortalId, LocaleId, PublishedCatalogId, UserId.
                if (cartItem.OmsOrderId > 0)
                    cartModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + cartItem.OmsOrderId)?.ShoppingCartModel;
                else
                {
                    ShoppingCartModel cartModelForSession = GetCartModelFromSession(cartItem?.UserId);

                    if (cartModelForSession != null)
                    {
                        cartModel.Coupons = cartModelForSession.Coupons?.Count > 0 ? cartModelForSession.Coupons : new List<CouponModel>();
                        cartModel.CSRDiscountAmount = cartModelForSession.CSRDiscountAmount != 0 ? cartModelForSession.CSRDiscountAmount : 0;
                        cartModel.GiftCardNumber = cartModelForSession.GiftCardNumber;
                    }
                }

                cartModel.PortalId = cartItem.PortalId;
                cartModel.LocaleId = cartItem.LocaleId;
                cartModel.PublishedCatalogId = cartItem.CatalogId;
                cartModel.UserId = cartItem.UserId;
                cartModel.OmsOrderId = cartItem.OmsOrderId;

                //Get attribute values and code.
                if (!string.IsNullOrEmpty(cartItem.PersonalisedCodes))
                    PersonalisedItems(cartItem);

                //Create new cart.
                if (!string.IsNullOrEmpty(cartItem.GroupProductSKUs) && !string.IsNullOrEmpty(cartItem.GroupProductsQuantity) || cartItem?.GroupProducts?.Count > 0)
                    GetGroupShoppingCartModel(cartItem, cartModel);
                else
                {
                    cartModel.ShoppingCartItems.Add(cartItem?.ToModel<ShoppingCartItemModel>());
                    if (cartItem.OmsOrderId > 0)
                        AddProductHistory(cartItem);
                }

                if (cartModel.Shipping?.ShippingId < 1)
                    cartModel.ShippingId = cartItem.ShippingId;

                AddressModel shippingAddress = cartModel.ShippingAddress ?? GetCartModelFromSession(cartItem?.UserId)?.ShippingAddress;

                string GiftCardNumber = cartModel.GiftCardNumber;

                decimal CSRDiscountAmount = cartModel.CSRDiscountAmount;
                ZnodeLogging.LogMessage("CSRDiscountAmount and GiftCardNumber:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { CSRDiscountAmount = CSRDiscountAmount, GiftCardNumber = GiftCardNumber });

                if (cartModel.OmsOrderId > 0)
                    cartModel.IsMerged = true;

                if (IsNull(cartModel.ShippingAddress))
                    cartModel.ShippingAddress = shippingAddress;

                ShoppingCartModel newCartModel = _shoppingCartsClient.CreateCart(cartModel);

                PortalModel portal = _portalClient.GetPortal(newCartModel.PortalId, new ExpandCollection { ZnodePortalEnum.ZnodeOmsOrderState.ToString().ToLower() });
                if (IsNotNull(newCartModel))
                {
                    if (cartItem.OmsOrderId > 0)
                        GetUpdatedShoppingCart(cartModel, newCartModel, portal);


                    newCartModel.ShippingAddress = shippingAddress;
                    newCartModel.Payment = new PaymentModel() { ShippingAddress = shippingAddress, PaymentSetting = new PaymentSettingModel() };

                    //Set Gift Card Number and CSR Discount Amount Data For Calculation
                    SetCartDataForCalculation(newCartModel, GiftCardNumber, CSRDiscountAmount);

                    newCartModel = _shoppingCartsClient.Calculate(newCartModel);
                }

                // if persistent cart disabled, we need not call below method, need to check with portal record.
                SaveCartInCookie(newCartModel, portal);
                SaveCartInSession(cartItem.OmsOrderId, newCartModel);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                return cartModel?.ToViewModel<CartViewModel>();
            }
            return new CartViewModel() { HasError = true, ErrorMessage = string.Empty };
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use AddToCartProduct")]
        // Add multiple simple product to cart.
        public virtual CartViewModel AddProductToCart(bool cartItems, int orderId, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cartModel = GetShoppingCartFromSession(orderId, userId);
            SaveLineItemHistorySession(cartModel?.ShoppingCartItems);
            return IsNotNull(cartModel) ? GetCartOrderStatusList(orderId, cartModel) : new CartViewModel();
        }

        // Get Cart method to check Session or Cookie to get the existing shopping cart.
        public virtual CartViewModel GetCart(int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel shoppingCartModel = new ShoppingCartModel();
            if (omsOrderId > 0)
                shoppingCartModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId)?.ShoppingCartModel;
            else
                shoppingCartModel = GetCartModelFromSession(userId);

            if (IsNull(shoppingCartModel))
            {
                return new CartViewModel()
                {
                    HasError = true,
                    ErrorMessage = "Products are out of stock"
                };
            }
            if (shoppingCartModel.ShoppingCartItems.Count == 0)
                return new CartViewModel();

            return shoppingCartModel.ToViewModel<CartViewModel>();
        }

        //Update quantity of cart item.
        public virtual CartViewModel UpdateCartItemQuantity(string guid, decimal quantity, int productId, int shippingId, int? userId = 0, bool isQuote = false)
        {
            // Get shopping cart from session.
            ShoppingCartModel shoppingCart = GetCartModelFromSession(userId, isQuote);

            if (IsNotNull(shoppingCart))
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

                ShoppingCartItemModel shoppingCartItemModel = shoppingCart.ShoppingCartItems?.FirstOrDefault(w => w.ExternalId == guid);
                shoppingCartItemModel?.AssociatedAddOnProducts?.ForEach(x => x.Quantity = Convert.ToDecimal(quantity.ToInventoryRoundOff()));


                if (IsNotNull(shoppingCartItemModel))
                {
                    //save the updated quantity
                    UpdateCartLineItem(shoppingCart, BindCartLineItemData(shoppingCartItemModel));

                    //ShoppingCartItems set null to get from cart from database in GetCart
                    shoppingCart.ShoppingCartItems = null;

                    SaveCartModelInSession(userId, shoppingCart, isQuote);
                }
            }
            return shoppingCart?.ToViewModel<CartViewModel>();
        }

        //Create cartItem for configurable type product.
        public virtual void GetSelectedConfigurableProductsForAddToCart(AddToCartViewModel cartItem, string configurableProductSkus = null)
        {
            //Get sku and quantity of associated group products.
            string[] configurableProducts = !string.IsNullOrEmpty(configurableProductSkus)
                                            ? configurableProductSkus.Split(',')
                                            : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs)
                                            ? cartItem.ConfigurableProductSKUs?.Split(',')
                                            : null;

            for (int index = 0; index < configurableProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                cartItem.ConfigurableProductSKUs = configurableProducts[index];

                if (IsNotNull(cartItem))
                {
                    GetSelectedAddOnProductsForAddToCart(cartItem, cartItem.AddOnProductSKUs);
                }
            }
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use UpdateCartItemQuantity")]
        /// <summary>
        /// Updates the Quantity updated in the shopping cart page.
        /// </summary>
        /// <param name="createOrderViewModel">CreateOrderViewModel</param>
        /// <param name="guid">GUID of the cart item</param>
        /// <param name="quantity">Quantity Selected</param>
        /// <returns>Bool value if the items are updated or not.</returns>
        public virtual CartViewModel UpdateCartItem(string guid, decimal quantity, int productId, int shippingId, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Get cart .            
            ShoppingCartModel cart = GetCartModelFromSession(userId);
            if (IsNull(cart) || cart.ShoppingCartItems?.Count < 1)
                return new CartViewModel();

            // Check if item exists.
            ShoppingCartItemModel cartItem = cart.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == guid);
            if (IsNull(cartItem))
                return new CartViewModel();

            SetCreatedByUser(cart.UserId);
            string sku = string.Empty;

            sku = productId > 0 ? cartItem.GroupProducts?.Where(x => x.ProductId == productId)?.FirstOrDefault()?.Sku
                                   : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) ? cartItem.ConfigurableProductSKUs : cartItem.SKU;
            ZnodeLogging.LogMessage("sku to get productDetails:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { sku = sku });
            //Get inventory of sku and Check Inventory
            PublishProductsViewModel productDetails = CheckQuantity(sku, cartItem.SKU, quantity, cartItem.AddOnProductSKUs, cart.PortalId, productId);

            if (!productDetails.ShowAddToCart)
                cart.ShoppingCartItems.FirstOrDefault(c => c.ExternalId == guid).InsufficientQuantity = true;

            //Update quantity and update the cart.
            if (productId > 0)
                cart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(x => x.GroupProducts?.Where(y => y.ProductId == productId)?.Select(z => { z.Quantity = quantity; return z; })?.FirstOrDefault()).ToList();
            else
                cart.ShoppingCartItems.Where(w => w.ExternalId == guid).Select(c => { c.Quantity = quantity; return c; }).ToList();

            if (cart.Shipping?.ShippingId < 1)
                cart.ShippingId = shippingId;

            cart.IsMerged = true;
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.CreateCart(cart);
            if (IsNotNull(shoppingCartModel))
            {
                shoppingCartModel.ShippingAddress = cart.ShippingAddress;
                shoppingCartModel.Payment = new PaymentModel() { ShippingAddress = shoppingCartModel.ShippingAddress, PaymentSetting = new PaymentSettingModel() };

                //Set Gift Card Number and CSR Discount Amount Data For Calculation
                SetCartDataForCalculation(shoppingCartModel, cart.GiftCardNumber, cart.CSRDiscountAmount);

                shoppingCartModel = _shoppingCartsClient.Calculate(shoppingCartModel);
            }

            SaveCartModelInSession(userId, shoppingCartModel);
            //TODO:OMS
            //SaveLineItemHistorySession(shoppingCartModel.ShoppingCartItems);
            return shoppingCartModel?.ToViewModel<CartViewModel>();
        }

        //Remove all the cart items.
        public virtual void RemoveAllCartItems(int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            if (IsNotNull(cart))
            {
                cart.ShoppingCartItems = new List<ShoppingCartItemModel>();
                cart.GiftCardNumber = string.Empty;
                cart.Coupons = new List<CouponModel>();
                cart.CSRDiscountAmount = 0;
                SaveCartInSession(orderId, cart);
            }
            RemoveInSession(AdminConstants.LineItemHistorySession);
        }

        //Get cart info from cookie.
        public virtual ShoppingCartModel GetCartFromCookie()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get cookie from current request.
            string cookieValue = GetFromCookie(AdminConstants.CartCookieKey);

            if (!string.IsNullOrEmpty(cookieValue))
            {
                try
                {
                    ShoppingCartModel shoppingCartModel = _shoppingCartsClient.GetShoppingCart(new CartParameterModel
                    {
                        CookieMappingId = cookieValue,
                        LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale),
                        PortalId = StoreAgent.CurrentStore.PortalId,
                        UserId = null
                    });
                    if (IsNotNull(shoppingCartModel))
                        return shoppingCartModel;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return null;
                }
            }
            return null;
        }

        //To apply & validate Coupon.
        public virtual CartViewModel ApplyCoupon(string couponCode, int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            if (IsNull(cart))
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);

            List<CouponModel> promocode = GetCoupons(cart);
            ZnodeLogging.LogMessage("promocode list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, promocode?.Count());
            List<CouponViewModel> invalidCoupons = new List<CouponViewModel>();

            //to remove invalid coupon
            RemoveInvalidCoupon(cart);

            if (!string.IsNullOrEmpty(couponCode))
                GetNewCouponToApply(cart, couponCode);
            else
                invalidCoupons.Add(new CouponViewModel { Code = couponCode, PromotionMessage = "Coupon Code cannot be empty.", CouponApplied = false, CouponValid = false });

            //Set login user and selected user in cache.
            SetCreatedByUser(cart.UserId);

            //Set Calculate voucher if voucher applied.
            if(cart.GiftCardAmount > 0 && orderId > 0)
            cart.IsCalculateVoucher = true;

            //Calculate coupon for the cart.
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.Calculate(cart);

            SaveCartInSession(orderId, shoppingCartModel);

            //Get order status list in model & map to CartViewModel
            CartViewModel cartViewModel = GetCartOrderStatusList(orderId, shoppingCartModel);

            //return false if invalidCoupons contains invalid coupons.
            if (!IsCouponApplied(cartViewModel, invalidCoupons))
            {
                //Calculate all valid and invlid coupon.
                cartViewModel = CalculateAllCoupons(orderId, cartViewModel, shoppingCartModel, cart, promocode);

                //Add invalid coupons with error coupon message to the cart and display on view.
                foreach (CouponViewModel invalidCoupon in invalidCoupons)
                    cartViewModel.Coupons.Add(invalidCoupon);

                SaveCartInSession(orderId, shoppingCartModel);
            }
            if (orderId > 0)
                cartViewModel.OrderState = GetOrderModelFromSession(orderId)?.OrderState;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        //Removes the applied coupon from the cart.
        public virtual CartViewModel RemoveCoupon(string couponCode, int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            if (IsNull(cart))
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);

            OrderModel orderModel = new OrderModel();
            if (orderId > 0)
                orderModel = GetOrderModelFromSession(orderId);

            if (IsNotNull(cart.Coupons))
            {
                StringBuilder sb = new StringBuilder();
                //Remove coupon from cart.
                foreach (CouponModel coupon in cart.Coupons)
                {
                    if (Equals(coupon.Code, couponCode))
                    {
                        if (orderId > 0)
                            sb.AppendFormat("{0},", coupon.Code);

                        cart.Coupons.Remove(coupon);
                        break;
                    }
                }

                string coupons = sb?.ToString();
                ZnodeLogging.LogMessage("coupons:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { coupons = coupons });
                if (!string.IsNullOrEmpty(coupons))
                {
                    SaveOrderModelInSession(orderId, orderModel);
                }
            }
            //Set Calculate voucher if voucher applied.
            if (cart.GiftCardAmount > 0 && orderId > 0)
                cart.IsCalculateVoucher = true;

            cart = _shoppingCartsClient.Calculate(cart);

            SaveCartInSession(orderId, cart);

            //Get order status list in model & map to CartViewModel
            CartViewModel cartViewModel = GetCartOrderStatusList(orderId, cart);
            cartViewModel.OrderState = orderModel?.OrderState;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        //Apply GiftCard.
        [Obsolete("This method is marked as obsolete from Znode version 9.6.1, instead of this ApplyVoucher method should be used.")]
        public virtual CartViewModel ApplyGiftCard(string giftCard, int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            cart.GiftCardNumber = giftCard;

            //Set login user and selected user in cache.
            SetCreatedByUser(cart.UserId);

            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.Calculate(cart);

            SaveCartInSession(orderId, shoppingCartModel);

            //Get order status list in model & map to CartViewModel
            CartViewModel cartViewModel = GetCartOrderStatusList(orderId, shoppingCartModel);

            OrderModel orderModel = new OrderModel();
            if (orderId > 0)
                orderModel = GetOrderModelFromSession(orderId);

            if (cartViewModel.GiftCardApplied)
            {
                cartViewModel.SuccessMessage = Admin_Resources.ValidGiftCard;
                if (orderId > 0)
                {
                    RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderGiftCard);
                    OrderHistory(orderModel, ZnodeConstant.OrderGiftCard, string.Empty, giftCard);
                    SaveOrderModelInSession(orderId, orderModel);
                }
            }
            else
            {
                if (orderId > 0)
                    SaveOrderModelInSession(orderId, orderModel);

                cartViewModel = (CartViewModel)GetViewModelWithErrorMessage(cartViewModel, Admin_Resources.ErrorGiftCard);
            }
            cartViewModel.OrderState = orderModel?.OrderState;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }
       
        //Apply Csr Discount.
        public virtual CartViewModel ApplyCsrDiscount(decimal csrDiscount, string csrDesc, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel shoppingCart = GetCartModelFromSession(userId);


            if (shoppingCart?.ShoppingCartItems?.Count < 1 && csrDiscount > 0)
                return new CartViewModel { CSRDiscountMessage = Admin_Resources.CSRValidation };
            else if (shoppingCart?.ShoppingCartItems?.Count < 1)
                return new CartViewModel();

            shoppingCart.CSRDiscountAmount = csrDiscount;
            shoppingCart.CSRDiscountDescription = csrDesc;
            shoppingCart.CSRDiscountEdited = true;
            //Set login user and selected user in cache.
            SetCreatedByUser(shoppingCart.UserId);

            //Set Calculate voucher if voucher applied.
            if (shoppingCart.GiftCardAmount > 0)
                shoppingCart.IsCalculateVoucher = true;

            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.Calculate(shoppingCart);

            SaveCartModelInSession(userId, shoppingCartModel);

            CartViewModel cartViewModel = shoppingCartModel.ToViewModel<CartViewModel>();

            cartViewModel.SuccessMessage = cartViewModel.CSRDiscountMessage;

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use RemoveCartItem")]
        // Removes the items from the shopping cart.
        public virtual CartViewModel RemoveShoppingCartItem(string guid, int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Get cart from Session.
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            if (orderId > 0 && cart.ShoppingCartItems.Count.Equals(1))
            {
                CartViewModel cartViewModel = GetCartOrderStatusList(orderId, cart);
                cartViewModel.HasError = true;
                cartViewModel.ErrorMessage = Admin_Resources.UnableToDeleteSingleItem;
                return cartViewModel;
            }

            if (string.IsNullOrEmpty(guid))
                return GetCartOrderStatusList(orderId, cart);

            SetCreatedByUser(cart.UserId);

            if (cart?.ShoppingCartItems?.Count < 1)
                return new CartViewModel();

            if (cart.ShoppingCartItems.Count.Equals(1))
            {
                //Remove all cart items if shopping cart having only one item.
                _shoppingCartsClient.RemoveAllCartItem(new CartParameterModel { UserId = cart.UserId.GetValueOrDefault(), CookieMappingId = cart.CookieMappingId, OmsOrderId = orderId });

                //Remove all cart from session.
                RemoveAllCartItems(0, userId);
                return new CartViewModel()
                {
                    UserId = Convert.ToInt32(cart.UserId),
                    LocaleId = cart.LocaleId
                };
            }

            // Check if item exists.
            ShoppingCartItemModel item = cart.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == guid);
            if (IsNull(item))
                return new CartViewModel();

            //to remove child item if deleting the parent product having AutoAddon associated with it
            if (!string.IsNullOrEmpty(item.AutoAddonSKUs))
            {
                cart.RemoveAutoAddonSKU = item.AutoAddonSKUs;
                cart.IsParentAutoAddonRemoved = string.Equals(item.SKU.Trim(), item.AutoAddonSKUs.Trim(), StringComparison.OrdinalIgnoreCase) ? false : true;
            }

            // Remove item and update the cart in Session and API.
            cart.ShoppingCartItems.Remove(item);
            //Calculate cart and save in session.
            ShoppingCartModel shoppingCartModel = CartCalculateAndSaveInSession(cart, orderId, true, item);
            if (orderId > 0)
                SaveLineItemHistorySession(shoppingCartModel?.ShoppingCartItems);
            return GetCartOrderStatusList(orderId, shoppingCartModel);
        }


        // Removes the items from the shopping cart.
        public virtual bool RemoveCartItem(string guid, int orderId = 0, int userId = 0, int portalId = 0, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            // Get cart from Session.
            ShoppingCartModel cart = GetCartFromSession(userId, portalId, orderId, isQuote);

            if (orderId > 0 && cart.ShoppingCartItems.Count.Equals(1))
                return false;


            SetCreatedByUser(cart.UserId);

            if (cart?.ShoppingCartItems?.Count < 1)
                return false;

            //Remove all cart items if shopping cart having only one item.
            if (cart.ShoppingCartItems.Count.Equals(1))
            {
                return DeleteAllCartItems(cart, orderId, userId, isQuote);
            }

            // Check if item exists.
            ShoppingCartItemModel item = cart.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == guid);
            if (IsNull(item))
                return false;

            //to remove child item if deleting the parent product having AutoAddon associated with it
            if (!string.IsNullOrEmpty(item.AutoAddonSKUs))
            {
                cart.RemoveAutoAddonSKU = item.AutoAddonSKUs;
                cart.IsParentAutoAddonRemoved = string.Equals(item.SKU.Trim(), item.AutoAddonSKUs.Trim(), StringComparison.OrdinalIgnoreCase) ? false : true;
            }

            // Remove item and update the cart in Session and API.
            cart.ShoppingCartItems.Remove(item);

            bool isRemoved = _shoppingCartsClient.RemoveCartLineItem(Convert.ToInt32(item.OmsSavedcartLineItemId));

            if (isRemoved && (orderId == 0))
            {
                //ShoppingCartItems set null to get from cart from database in GetCart
                cart.ShoppingCartItems = null;

                SaveCartModelInSession(userId, cart, isQuote);
            }
            if (isRemoved && orderId > 0)
            {
                SaveLineItemHistorySession(cart?.ShoppingCartItems);
                SaveCartInSession(orderId, cart, true, item);
            }

            return isRemoved;
        }

        //Remove all cart items from shoppingCart.
        public virtual CartViewModel RemoveAllShoppingCartItems(int userId, int portalId, int orderId = 0, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            // Get cart from Session.
            ShoppingCartModel cart = GetCartFromSession(userId, portalId, orderId, isQuote);

            if (cart?.ShoppingCartItems?.Count > 0)
            {
                DeleteAllCartItems(cart, orderId, userId, isQuote);
            }

            if (IsNotNull(cart))
            {
                return new CartViewModel()
                {
                    UserId = Convert.ToInt32(cart.UserId),
                    LocaleId = cart.LocaleId,
                    PublishedCatalogId = cart.PublishedCatalogId,
                    PortalId = cart.PortalId,
                };
            }

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new CartViewModel();
        }



        //Remove all the cart items.
        public virtual bool DeleteAllCartItems(ShoppingCartModel cart, int orderId = 0, int userId = 0, bool isQuote = false)
        {
            if (_shoppingCartsClient.RemoveAllCartItem(new CartParameterModel { UserId = cart.UserId.GetValueOrDefault(), CookieMappingId = cart.CookieMappingId, OmsOrderId = orderId , PortalId = cart.PortalId }))
            {
                cart.ShoppingCartItems = new List<ShoppingCartItemModel>();
                cart.GiftCardNumber = string.Empty;
                cart.Coupons = new List<CouponModel>();
                cart.CSRDiscountAmount = 0;
                cart.Vouchers = new List<VoucherModel>();
                //Save shopping cart in session.
                SaveCartModelInSession(userId, cart, isQuote);
                return true;
            }
            return false;
        }

        //Get the group products
        public virtual ShoppingCartItemModel BindGroupProducts(string simpleProduct, string simpleProductQty, CartItemViewModel cartItem, bool isNewExtIdRequired, string productName = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartItemModel shoppingCartItemModel = new ShoppingCartItemModel
            {
                ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId,
                SKU = cartItem.SKU,
                AddOnProductSKUs = cartItem.AddOnProductSKUs,
                AutoAddonSKUs = cartItem.AutoAddonSKUs,
                GroupProducts = new List<AssociatedProductModel> { new AssociatedProductModel { Sku = simpleProduct, Quantity = decimal.Parse(simpleProductQty) } }
            };

            AddProductHistory(cartItem, productName, decimal.Parse(simpleProductQty), simpleProduct);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCartItemModel;
        }

        // Remove cart item from cart.
        public virtual CartViewModel RemoveItemFromCart(string productIds, int omsOrderId, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(omsOrderId, userId);
            CartViewModel cartViewModel = new CartViewModel();
            if (IsNotNull(cart))
            {
                if (!string.IsNullOrEmpty(productIds))
                {
                    IDictionary<string, Tuple<string, string>> lineItemHistory = GetFromSession<IDictionary<string, Tuple<string, string>>>(AdminConstants.LineItemHistorySession);

                    foreach (string productId in productIds?.Split(',') ?? new string[0])
                    {
                        List<ShoppingCartItemModel> orderlineitems = cart.ShoppingCartItems?.Where(w => w.ParentProductId == Convert.ToInt32(productId)).ToList();
                        List<OrderLineItemDataModel> lineItemList = _shoppingCartsClient.GetOmsLineItemDetails(omsOrderId)?.OrderLineItemDetails;
                        if (orderlineitems.Any())
                        {
                            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);

                            foreach (ShoppingCartItemModel lineitem in orderlineitems)
                            {
                                Tuple<string, string> currentItemHistory = lineItemHistory?.Where(x => x.Value.Item1.ToString() == OrderAgent.GetProductSKU(lineitem)).Select(s => s.Value).FirstOrDefault();
                                if (IsNull(currentItemHistory))
                                {
                                    if (cart.ShoppingCartItems.Count.Equals(1))
                                        RemoveAllCart(userId);
                                    if (lineitem.OmsOrderLineItemsId == 0)
                                        cart.ShoppingCartItems.Remove(lineitem);
                                    else
                                    {
                                        if (lineItemList?.Count > 0)
                                            UpdateLineItemQuantity(cart, lineitem, lineItemList);
                                    }
                                }
                                else
                                {
                                    SetProductQuantity(lineitem, currentItemHistory);
                                    cart.ShoppingCartItems[cart.ShoppingCartItems.FindIndex(x => x.ExternalId == lineitem.ExternalId)] = lineitem;
                                }

                                if (omsOrderId > 0)
                                    RemoveHistoryForCartLineItem(true, lineitem, orderModel);
                            }

                            if (omsOrderId > 0)
                                SaveInSession(AdminConstants.OMSOrderSessionKey + omsOrderId, orderModel);
                            else
                                SaveCartModelInSession(userId, cart);

                        }
                    }
                }
                cartViewModel = (omsOrderId > 0) ? GetCartOrderStatusList(omsOrderId, CartCalculateAndSaveInSession(cart, omsOrderId)) : CartCalculateAndSaveInSession(cart, omsOrderId)?.ToViewModel<CartViewModel>();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        //Update Line Item Quantity to original Quantity
        public virtual void UpdateLineItemQuantity(ShoppingCartModel cart, ShoppingCartItemModel lineitem, List<OrderLineItemDataModel> lineItemList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(lineitem.GroupProducts) && lineitem.GroupProducts.Count > 0)
            {
                UpdateGroupProductQuantity(lineitem, cart, lineItemList);
            }
            else if (!string.IsNullOrEmpty(lineitem.ConfigurableProductSKUs))
            {
                UpdateConfigurableProduct(lineitem, cart, lineItemList);
            }
            else
            {
                decimal? lineItemOriginalQuantity = lineItemList.FirstOrDefault(m => m.Sku.Equals(lineitem.SKU)).Quantity;

                cart.ShoppingCartItems.ForEach(m => { if (m.SKU == lineitem.SKU) m.Quantity = lineItemOriginalQuantity ?? 0; });
            }
        }

        //Update Configurable Line Item quantity
        public virtual void UpdateConfigurableProduct(ShoppingCartItemModel lineitem, ShoppingCartModel cart, List<OrderLineItemDataModel> lineItemList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            decimal lineItemOriginalQuantity = GetConfigurableLineItemQuantity(lineitem, lineItemList);
            ZnodeLogging.LogMessage("parameter lineItemOriginalQuantity:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { lineItemOriginalQuantity = lineItemOriginalQuantity });
            cart.ShoppingCartItems.ForEach(m =>
            {
                if (!string.IsNullOrEmpty(m.ConfigurableProductSKUs) && m.ConfigurableProductSKUs.Equals(lineitem.ConfigurableProductSKUs))
                    m.Quantity = lineItemOriginalQuantity;
            });
        }

        //Get Configurable Line Item quantity
        public virtual decimal GetConfigurableLineItemQuantity(ShoppingCartItemModel lineitem, List<OrderLineItemDataModel> lineItemList)
            => lineItemList.FirstOrDefault(m => m.Sku.Equals(lineitem.ConfigurableProductSKUs))?.Quantity ?? 0;

        //Update Group Product Quantity
        public virtual void UpdateGroupProductQuantity(ShoppingCartItemModel lineitem, ShoppingCartModel cart, List<OrderLineItemDataModel> lineItemList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            decimal lineItemOriginalQuantity = 0;
            lineitem.GroupProducts.ForEach(m =>
            {
                lineItemOriginalQuantity = GetLineItemOriginalQuantity(lineItemList, m.Sku) ?? 0;
                UpdateCartGroupProductQuantity(cart, lineItemOriginalQuantity, m);
            });
        }

        public virtual decimal? GetLineItemOriginalQuantity(List<OrderLineItemDataModel> lineItemList, string sku)
        => lineItemList.FirstOrDefault(m => m.Sku.Equals(sku))?.Quantity;

        //Update Cart Group Product Quantity
        public virtual void UpdateCartGroupProductQuantity(ShoppingCartModel cart, decimal lineItemOriginalQuantity, AssociatedProductModel groupProduct)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            cart.ShoppingCartItems.ForEach(m =>
            {
                if (m.GroupProducts.Count > 0)
                {
                    m.GroupProducts.ForEach(n =>
                    {
                        if (n.ProductId == groupProduct.ProductId)
                            n.Quantity = lineItemOriginalQuantity;
                    });
                }
            });
        }

        //Remove cart from session.
        public virtual void RemoveCartSession(int? userId = 0)
            => RemoveCartModelInSession(userId);

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use RemoveAllShoppingCartItems")]
        //Remove All Cart.
        public virtual CartViewModel RemoveAllCart(int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Get cart from Session.
            ShoppingCartModel cart = GetShoppingCartFromSession(0);

            if (IsNotNull(cart))
            {
                //Remove all cart items if shopping cart having only one item.
                _shoppingCartsClient.RemoveAllCartItem(new CartParameterModel { UserId = cart.UserId.GetValueOrDefault(), CookieMappingId = cart.CookieMappingId });

                //Remove all cart from session.
                RemoveAllCartItems(0, cart?.UserId);
                UserModel userModel = _userClient.GetUserAccountData(Convert.ToInt32(cart.UserId));
                return new CartViewModel()
                {
                    UserId = Convert.ToInt32(cart.UserId),
                    LocaleId = cart.LocaleId,
                    PublishedCatalogId = cart.PublishedCatalogId,
                    PortalId = cart.PortalId,
                    CustomerName = userModel.FullName
                };
            }
            RemoveInSession(AdminConstants.LineItemHistorySession);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new CartViewModel();
        }

        //Get the updated shopping cart model in case of manage Order.
        public ShoppingCartModel GetUpdatedShoppingCart(ShoppingCartModel sessionCart, ShoppingCartModel newCart, PortalModel portal = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(sessionCart) && IsNotNull(newCart))
            {
                foreach (ShoppingCartItemModel newCartItem in newCart.ShoppingCartItems)
                {
                    foreach (ShoppingCartItemModel sessionCartItem in sessionCart.ShoppingCartItems)
                    {
                        //Check if main product sku is same.
                        if (string.Equals(newCartItem.SKU, sessionCartItem.SKU, StringComparison.OrdinalIgnoreCase))
                        {
                            //Check if group product sku is same.
                            if (IsNotNull(newCartItem.GroupProducts) && newCartItem.GroupProducts.Count > 0)
                            {
                                CheckGroupProductSKUs(sessionCartItem, newCartItem, portal);
                            }
                            //Check if configurable product sku is same.
                            else if (!string.IsNullOrEmpty(newCartItem.ConfigurableProductSKUs) || !string.IsNullOrEmpty(sessionCartItem.ConfigurableProductSKUs) && Equals(newCartItem.ConfigurableProductSKUs, sessionCartItem.ConfigurableProductSKUs))
                            {
                                CheckAddonProductSKUs(sessionCartItem, newCartItem, portal);
                                break;
                            }
                            //Check if bundle product sku is same.
                            else if (!string.IsNullOrEmpty(newCartItem.BundleProductSKUs) || !string.IsNullOrEmpty(sessionCartItem.BundleProductSKUs) && Equals(newCartItem.BundleProductSKUs, sessionCartItem.BundleProductSKUs))
                            {
                                CheckAddonProductSKUs(sessionCartItem, newCartItem, portal);
                                break;
                            }
                            else
                            {
                                //Check if add on product sku is same.
                                CheckAddonProductSKUs(sessionCartItem, newCartItem, portal);
                                break;
                            }
                        }
                        if (Equals(newCartItem.SKU, newCartItem.AutoAddonSKUs))
                            SetAdditionalAutoAddOnLineItemDetails(newCartItem, portal);
                    }
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return newCart;
        }

        //Get the list of associated skus and their quantity for group products.
        public virtual void GetGroupShoppingCartModel(CartItemViewModel cartItem, ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get sku and quantity of associated group products.
            string[] groupProducts = string.IsNullOrEmpty(cartItem.GroupProductSKUs) ? cartItem.GroupProducts?.Select(x => x.Sku)?.ToArray() : cartItem.GroupProductSKUs?.Split(',');
            string[] groupProductsQuantity = string.IsNullOrEmpty(cartItem.GroupProductsQuantity) ? cartItem.GroupProducts?.Select(x => Convert.ToString(x.Quantity))?.ToArray() : cartItem.GroupProductsQuantity?.Split('_');

            for (int index = 0; index < groupProducts?.Length; index++)
            {
                bool isNewExtIdRequired = false;
                if (!Equals(index, 0))
                    isNewExtIdRequired = true;

                ShoppingCartItemModel cartItemModel = BindGroupProducts(groupProducts[index], groupProductsQuantity[index], cartItem, isNewExtIdRequired);
                if (IsNotNull(cartItemModel))
                    shoppingCartModel.ShoppingCartItems.Add(cartItemModel);
            }
        }

        //Set created by and modified by user.
        public void SetCreatedByUser(int? userId)
        {
            if (userId > 0)
            {
                _shoppingCartsClient.LoginAs = _shoppingCartsClient.UserId;
                _shoppingCartsClient.UserId = userId.GetValueOrDefault();
            }
        }

        // Add product cart line history.
        public void AddProductHistory(CartItemViewModel cartItem, string productName = "", decimal quantity = 0m, string sku = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (cartItem.OmsOrderId > 0)
            {
                ZnodeLogging.LogMessage("Input parameters cartItem having:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = cartItem.OmsOrderId });
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + cartItem.OmsOrderId);

                string addedExternalId = string.IsNullOrEmpty(productName) ? cartItem.ExternalId : cartItem.OmsOrderId + productName;
                string addedProductName = string.IsNullOrEmpty(productName) ? cartItem.ProductName : productName;
                decimal addedQuantity = string.IsNullOrEmpty(productName) ? cartItem.Quantity : quantity;
                string addedSKU = string.IsNullOrEmpty(productName) ? cartItem.SKU : sku;
                int lineItemsId = string.IsNullOrEmpty(productName) ? cartItem.OmsOrderLineItemsId : 0;

                OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel() { OrderLineAdd = addedProductName, Quantity = addedQuantity.ToInventoryRoundOff(), SKU = addedSKU, OmsOrderLineItemsId = lineItemsId, TaxCost = cartItem.TaxCost, SubTotal = cartItem.Total.GetValueOrDefault() };

                RemoveKeyFromDictionary(orderModel, addedExternalId, true);
                OrderLineHistory(orderModel, addedExternalId, orderLineItemHistoryModel);

                SaveInSession(AdminConstants.OMSOrderSessionKey + cartItem.OmsOrderId, orderModel);
            }
        }

        //Get attribute values and code.
        public virtual void PersonalisedItems(CartItemViewModel cartItem)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string[] Codes = cartItem.PersonalisedCodes?.Split(',');
            string[] Values = cartItem.PersonalisedValues?.Split('`');

            if (Convert.ToBoolean(cartItem.PersonalisedValues?.Contains('`')))
                Values = cartItem.PersonalisedValues?.Split('`');
            else
                Values = cartItem.PersonalisedValues?.Split(',');
            //}
            Dictionary<string, object> SelectedAttributes = new Dictionary<string, object>();
            if (IsNotNull(Values))
            {
                //Add code and value pair
                for (int index = 0; index < Codes.Length; index++)
                    SelectedAttributes.Add(Codes[index], Values[index]);
            }
            cartItem.PersonaliseValuesList = SelectedAttributes;
        }

        //Add product in the cart.
        public virtual AddToCartViewModel AddToCartProduct(AddToCartViewModel cartItem, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(cartItem))
            {
                cartItem.PublishedCatalogId = cartItem.CatalogId;
                AddToCartViewModel addToCartViewModel = _shoppingCartsClient.AddToCartProduct(GetShoppingCartValues(cartItem)?.ToModel<AddToCartModel>())?.ToViewModel<AddToCartViewModel>();
                GetShoppingCartByUserId(cartItem.UserId, cartItem.PortalId, isQuote);
                return addToCartViewModel;
            }
            return null;
        }

        //Get Cart method to check Session to get the existing shopping cart.
        public virtual CartViewModel GetShoppingCart(int userId, int portalId, int omsOrderId = 0, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            ShoppingCartModel cartModel = GetCartFromSession(userId, portalId, omsOrderId, isQuote);

            if (IsNotNull(cartModel))
                return omsOrderId > 0 ? GetCartOrderStatusList(omsOrderId, cartModel) : cartModel?.ToViewModel<CartViewModel>();

            return new CartViewModel();

        }


        //Calculated shopping cart
        public virtual CartViewModel CalculateShoppingCart(int userId, int portalId, int orderId = 0, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (userId > 0 && portalId > 0)
                {
                    bool isCalculatePromotionAndCoupon = isQuote ? false : true;                   
                    ShoppingCartModel cartModel = CalculateCart(GetCartFromSession(userId, portalId, orderId, isQuote), true, true, isCalculatePromotionAndCoupon);

                    return cartModel?.ToViewModel<CartViewModel>();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
            return null;
        }


        //Calculate cart.
        protected virtual ShoppingCartModel CalculateCart(ShoppingCartModel shoppingCartModel, bool isCalculateTaxAndShipping = true, bool isCalculateCart = true, bool isCalculatePromotionAndCoupon = true)
        {
            if (IsNotNull(shoppingCartModel))
            {
                shoppingCartModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                shoppingCartModel.IsCalculateTaxAndShipping = isCalculateTaxAndShipping;
                shoppingCartModel.IsCalculatePromotionAndCoupon = isCalculatePromotionAndCoupon;
                shoppingCartModel.IsCalculateVoucher = shoppingCartModel.IsQuoteOrder ? false : true;
                if (isCalculateCart)
                    shoppingCartModel = _shoppingCartsClient.Calculate(shoppingCartModel);

            }
            SaveCartModelInSession(shoppingCartModel.UserId, shoppingCartModel);
            return shoppingCartModel;
        }


        //Get cart info from cookie.
        public virtual ShoppingCartModel GetShoppingCartByUserId(int userId, int portalId, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (userId > 0)
            {
                try
                {
                    ShoppingCartModel shoppingCartModel = _shoppingCartsClient.GetShoppingCart(MapCartParameterModel(portalId, userId));
                    shoppingCartModel.IsQuoteOrder = isQuote;
                    shoppingCartModel = MappingFromSessionToDataBase(userId, shoppingCartModel, isQuote);

                    if (IsNotNull(shoppingCartModel))
                    {
                        SaveCartModelInSession(userId, shoppingCartModel, isQuote);

                        return shoppingCartModel;
                    }

                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return null;
                }
            }
            return null;
        }

        protected virtual ShoppingCartModel MappingFromSessionToDataBase(int userId, ShoppingCartModel shoppingCartModel, bool isQuote = false)
        {
            ShoppingCartModel shoppingCartModelFromSession = GetCartModelFromSession(userId, isQuote);

            shoppingCartModel.ShippingAddress = shoppingCartModelFromSession?.ShippingAddress;
            shoppingCartModel.BillingAddress = shoppingCartModelFromSession?.BillingAddress;
            shoppingCartModel.UserId = shoppingCartModelFromSession.UserId.GetValueOrDefault();
            shoppingCartModel.PortalId = shoppingCartModelFromSession.PortalId;
            shoppingCartModel.PublishedCatalogId = shoppingCartModelFromSession.PublishedCatalogId;
            shoppingCartModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            shoppingCartModel.IsTaxCostEdited = shoppingCartModelFromSession.IsTaxCostEdited;
            shoppingCartModel.CustomTaxCost = shoppingCartModelFromSession.CustomTaxCost;
            shoppingCartModel.GiftCardNumber = shoppingCartModelFromSession.GiftCardNumber;
            shoppingCartModel.GiftCardMessage = shoppingCartModelFromSession.GiftCardMessage;
            shoppingCartModel.CSRDiscountAmount = shoppingCartModelFromSession.CSRDiscountAmount;
            shoppingCartModel.CSRDiscountMessage = shoppingCartModelFromSession.CSRDiscountMessage;
            shoppingCartModel.Coupons = shoppingCartModelFromSession.Coupons;
            shoppingCartModel.Discount = shoppingCartModelFromSession.Discount;
            shoppingCartModel.IsAllVoucherRemoved = shoppingCartModelFromSession.IsAllVoucherRemoved;
            shoppingCartModel.Vouchers = shoppingCartModelFromSession.Vouchers;

            return shoppingCartModel;
        }
       
        //Get count of cart items in shopping cart.
        public virtual decimal GetCartCount(int portalId, int userId)
        {
            string count = _shoppingCartsClient.GetCartCount(new CartParameterModel
            {
                PortalId = portalId,
                UserId = userId
            });
            if (string.IsNullOrEmpty(count)) count = "0";
            return Convert.ToDecimal(HelperMethods.GetRoundOffQuantity(count));
        }

        //Get calculated shopping cart
        public virtual CartViewModel GetCalculatedShoppingCart(int userId, int portalId, int orderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                ShoppingCartModel shoppingCartModel = orderId > 0 ? GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId)?.ShoppingCartModel
                                                  : GetCartModelFromSession(userId);

                // If shoppingcart is not present in session, calculatecart method will get the shoppingcart details.
                if (!IsNotNull(shoppingCartModel))
                {
                    ShoppingCartModel cartModel = CalculateCart(GetCartFromSession(userId, portalId, orderId), true, true);

                    return cartModel?.ToViewModel<CartViewModel>();
                }
                return shoppingCartModel?.ToViewModel<CartViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }

        }


        // Remove key from order history.
        public virtual void RemoveKeyFromDictionary(OrderModel orderModel, string key, bool isFromLineItem = false)
        {
            if (IsNotNull(orderModel.OrderHistory) && !isFromLineItem)
            {
                if (orderModel.OrderHistory.ContainsKey(key))
                {
                    orderModel.OrderHistory.Remove(key);
                }
            }
            else
            {
                if (orderModel.OrderLineItemHistory.ContainsKey(key))
                {
                    orderModel.OrderLineItemHistory.Remove(key);
                }
            }
        }


        public virtual ShoppingCartModel GetShoppingCartFromSession(int orderId, int? userId = 0)
       => orderId > 0 ? GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId)?.ShoppingCartModel
                      : GetCartModelFromSession(userId)
                      ?? GetCartFromCookie();

        // Add key and value in order line dictionary.
        public virtual void OrderLineHistory(OrderModel orderModel, string key, OrderLineItemHistoryModel lineHistory) => orderModel.OrderLineItemHistory?.Add(key, lineHistory);

        public virtual OrderModel GetOrderModelFromSession(int orderId) => GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

        public virtual void SaveOrderModelInSession(int orderId, OrderModel orderModel) => SaveInSession(AdminConstants.OMSOrderSessionKey + orderId, orderModel);

        // Add key and value in dictionary.
        public virtual void OrderHistory(OrderModel orderModel, string settingType, string oldValue, string newValue = "") => orderModel.OrderHistory?.Add(settingType, newValue);

        //Update quantity of cart item.
        public virtual CartViewModel UpdateCartItemPrice(string guid, decimal unitPrice, int productId, int shippingId, int? userId = 0)
        {
            // Get shopping cart from session.
            ShoppingCartModel shoppingCart = GetCartModelFromSession(userId);
            shoppingCart.IsQuoteOrder = true;

            if (IsNotNull(shoppingCart))
            {
                string sku = GetProductSku(shoppingCart?.ShoppingCartItems?.FirstOrDefault(w=> w.ExternalId == guid), productId);
                IPublishProductClient _publishProductClient = GetClient<PublishProductClient>();
                ProductInventoryPriceListModel productInventory = _publishProductClient.GetProductPriceAndInventory(new ParameterInventoryPriceModel { Parameter = sku, PortalId = shoppingCart.PortalId });

                decimal cartItemPrice = (productInventory?.ProductList?.FirstOrDefault(w => w.SKU == sku)?.RetailPrice).GetValueOrDefault();

                //Update quantity and update the cart.
                if (productId > 0)
                {
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(x => x.GroupProducts?.Where(y => y.ProductId == productId)?.Select(z => { z.UnitPrice = unitPrice; return z; })?.FirstOrDefault())?.ToList();
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(z => { z.ExtendedPrice = Convert.ToDecimal((z.UnitPrice * z.Quantity).ToInventoryRoundOff()); return z; })?.ToList();
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.IsPriceEdit = true; return y; })?.ToList();

                    if (cartItemPrice == unitPrice) {
                        shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(z => { z.CustomUnitPrice = null; return z; })?.ToList();
                    }
                    else
                        shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(z => { z.CustomUnitPrice = unitPrice; return z; })?.ToList();
                }
                else
                {
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.UnitPrice = Convert.ToDecimal(unitPrice.ToInventoryRoundOff()); return y; })?.ToList();
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.ExtendedPrice = Convert.ToDecimal((y.UnitPrice * y.Quantity).ToInventoryRoundOff()); return y; })?.ToList();
                    shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.IsPriceEdit = true; return y; })?.ToList();

                    if (cartItemPrice == unitPrice) {
                        shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.CustomUnitPrice = null; return y; })?.ToList();
                    }
                    else
                        shoppingCart.ShoppingCartItems?.Where(w => w.ExternalId == guid)?.Select(y => { y.CustomUnitPrice = Convert.ToDecimal(unitPrice.ToInventoryRoundOff()); return y; })?.ToList();
                }
                shoppingCart.SubTotal = Convert.ToDecimal(shoppingCart.ShoppingCartItems?.Sum(x => x.ExtendedPrice));

                SaveCartModelInSession(userId, shoppingCart, true);
            }
            return shoppingCart?.ToViewModel<CartViewModel>();
        }
        #endregion Public methods

        #region Protected Methods

        //Get Shopping cart
        protected virtual ShoppingCartModel GetCartFromSession(int userId, int portalId, int orderId = 0, bool isQuote = false)
        {
            ShoppingCartModel shoppingCartModel = orderId > 0 ? GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId)?.ShoppingCartModel
                                                              : GetFromSession<ShoppingCartModel>(SetCartModelSessionKey(userId, isQuote));

            if (IsNull(shoppingCartModel) || IsNull(shoppingCartModel?.ShoppingCartItems) || shoppingCartModel?.ShoppingCartItems?.Count == 0)
            {
                shoppingCartModel = GetShoppingCartByUserId(userId, portalId, isQuote);
            }
            return shoppingCartModel;
        }

        //Bind shopping cart values.
        protected virtual AddToCartViewModel GetShoppingCartValues(AddToCartViewModel cartItem)
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
            else if (!string.IsNullOrEmpty(cartItem.BundleProductSKUs))
            {
                GetSelectedBundleProductsForAddToCart(cartItem);
            }
            else
            {
                GetSelectedAddOnProductsForAddToCart(cartItem);
            }

            return cartItem;
        }

        //Get attribute values and code.
        protected virtual void PersonalisedItemsForAddToCart(AddToCartViewModel cartItem)
        {
            string[] Codes = cartItem.PersonalisedCodes?.Split(',');
            string[] Values;

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


        //Create cartItem for Group type product.
        protected virtual void GetSelectedGroupedProductsForAddToCart(AddToCartViewModel cartItem)
        {
            //Get sku and quantity of associated group products.
            string[] groupProducts = string.IsNullOrEmpty(cartItem.GroupProductSKUs) ? cartItem.GroupProducts?.Select(x => x.Sku)?.ToArray() : cartItem.GroupProductSKUs?.Split(',');
            string[] groupProductsQuantity = string.IsNullOrEmpty(cartItem.GroupProductsQuantity) ? cartItem.GroupProducts?.Select(x => Convert.ToString(x.Quantity))?.ToArray() : cartItem.GroupProductsQuantity?.Split('_');

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
        protected virtual void GetSelectedBundleProductsForAddToCart(AddToCartViewModel cartItem, string bundleProductSKUs = null)
        {
            //Get sku and quantity of associated group products.
            string[] bundleProducts = !string.IsNullOrEmpty(bundleProductSKUs) ? bundleProductSKUs.Split(',') : !string.IsNullOrEmpty(cartItem.BundleProductSKUs) ? cartItem.BundleProductSKUs?.Split(',') : null;

            string addOnProductSKUs = cartItem.AddOnProductSKUs;

            for (int index = 0; index < bundleProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.ExternalId = isNewExtIdRequired ? Guid.NewGuid().ToString() : cartItem.ExternalId;
                cartItem.BundleProductSKUs = bundleProducts[index];
                cartItem.ProductType = ZnodeConstant.BundleProduct;
                if (IsNotNull(cartItem))
                {
                    GetSelectedAddOnProductsForAddToCart(cartItem, addOnProductSKUs);
                }
            }
        }

        protected virtual void GetSelectedAddOnProductsForAddToCart(AddToCartViewModel cartItem, string addOnSKUS = null)
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
                    cartItem.AddOnQuantity = cartItem.AssociatedAddOnProducts[index].AddOnQuantity;

                    if (IsNotNull(cartItem))
                        cartItem.ShoppingCartItems.Add(cartItem.ToModel<ShoppingCartItemModel>());
                }
            }
            else
                cartItem.ShoppingCartItems.Add(cartItem.ToModel<ShoppingCartItemModel>());
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

        //update the cart line item
        protected virtual void UpdateCartLineItem(ShoppingCartModel shoppingCart, EditCartItemViewModel addToCartViewModel)
        {
            AddToCartModel addToCartModel = addToCartViewModel.ToModel<AddToCartModel>();
            addToCartModel.PublishedCatalogId = shoppingCart.PublishedCatalogId;
            addToCartModel.Coupons = shoppingCart.Coupons;
            addToCartModel.ZipCode = shoppingCart?.ShippingAddress?.PostalCode;
            addToCartModel.UserId = shoppingCart.UserId;
            addToCartModel.PortalId = shoppingCart.PortalId;
            addToCartModel.LocaleId = shoppingCart.LocaleId;
            _shoppingCartsClient.AddToCartProduct(addToCartModel);
        }

        //Bind shopping cart line item data
        protected virtual EditCartItemViewModel BindCartLineItemData(ShoppingCartItemModel shoppingCartItemModel)
        {
            EditCartItemViewModel addToCartViewModel = new EditCartItemViewModel();
            addToCartViewModel.GroupId = shoppingCartItemModel.GroupId;
            addToCartViewModel.AddOnProductSKUs = shoppingCartItemModel.AddOnProductSKUs;
            addToCartViewModel.AutoAddonSKUs = shoppingCartItemModel.AutoAddonSKUs;
            addToCartViewModel.SKU = shoppingCartItemModel.SKU;
            addToCartViewModel.ParentOmsSavedcartLineItemId = shoppingCartItemModel.ParentOmsSavedcartLineItemId;
            addToCartViewModel.AssociatedAddOnProducts = shoppingCartItemModel.AssociatedAddOnProducts;

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

            else
            {
                addToCartViewModel.ShoppingCartItems.Add(shoppingCartItemModel);
            }

            return addToCartViewModel;
        }

        //Get the sku of product of which quantity needs to update.       
        protected virtual string GetProductSku(ShoppingCartItemModel cartItem, int productId)
        {
            string sku = string.Empty;
            if (IsNotNull(cartItem))
            {
                if (productId > 0)
                {
                    sku = cartItem.GroupProducts?.FirstOrDefault(x => x.ProductId == productId)?.Sku;
                }
                else if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs))
                {
                    sku = cartItem.ConfigurableProductSKUs;
                }
                else
                    sku = cartItem.SKU;
            }
            return sku;
        }
        #endregion

        #region Private Methods

        private void SaveLineItemHistorySession(List<ShoppingCartItemModel> ShoppingCartItems)
        {
            ZnodeLogging.LogMessage("ShoppingCartItems list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, ShoppingCartItems?.Count());
            if (ShoppingCartItems.Any())
            {
                IDictionary<string, Tuple<string, string>> lineItemHistory = new Dictionary<string, Tuple<string, string>>();
                foreach (ShoppingCartItemModel orderlineitem in ShoppingCartItems)
                    lineItemHistory.Add(OrderAgent.GetProductSKU(orderlineitem), OrderAgent.GetItemHistory(orderlineitem));
                SaveInSession(AdminConstants.LineItemHistorySession, lineItemHistory);
            }
        }

        private void SetProductQuantity(ShoppingCartItemModel orderlineitem, Tuple<string, string> currentItemHistory)
        {
            //Here, Item1 = SKU, Item2 = Quantity
            if (orderlineitem.GroupProducts.Any())
                orderlineitem.GroupProducts.FirstOrDefault(x => currentItemHistory.Item1.Contains(x.Sku)).Quantity = Convert.ToDecimal(currentItemHistory.Item2);
            else
                orderlineitem.Quantity = Convert.ToDecimal(currentItemHistory.Item2);
        }

        //Check available quantity.
        private PublishProductsViewModel CheckQuantity(string sku, string parentProductSku, decimal quantity, string addOnSkus, int portalId, int parentProductId = 0)
        {
            //Get available quantity of cart item.
            PublishProductsViewModel product = new OrderAgent(GetClient<ShippingClient>(), GetClient<ShippingTypeClient>(), GetClient<StateClient>(), GetClient<CityClient>(), GetClient<ProductsClient>(), GetClient<BrandClient>(),
                GetClient<UserClient>(), GetClient<PortalClient>(), GetClient<AccountClient>(), GetClient<RoleClient>(), GetClient<DomainClient>(), GetClient<OrderClient>(), GetClient<EcommerceCatalogClient>(), GetClient<CustomerClient>(),
                GetClient<PublishProductClient>(), GetClient<MediaConfigurationClient>(), GetClient<PaymentClient>(), GetClient<ShoppingCartClient>(), GetClient<AccountQuoteClient>(), GetClient<OrderStateClient>(), GetClient<PIMAttributeClient>(), GetClient<CountryClient>(), GetClient<AddressClient>()).GetProductPriceAndInventory(sku, parentProductSku, quantity.ToString(), addOnSkus, portalId, parentProductId);

            //check if available quantity against selected quantity.
            //Commenting this method as GetProductPriceAndInventory method already performing the same operation
            //CheckCartQuantity(product, quantity);

            return product;
        }
        
        //Check quantity for cart item.
        private void CheckCartQuantity(PublishProductsViewModel viewModel, decimal? quantity)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(viewModel))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;
                decimal selectedQuantity = quantity.GetValueOrDefault();

                List<AttributesSelectValuesViewModel> inventorySetting = viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);
                ZnodeLogging.LogMessage("inventorySetting list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, inventorySetting?.Count());

                if (inventorySetting?.Count > 0)
                {
                    OrderAgent.TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

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

        //Bind dropdown list for all shoppingcart items.
        public virtual CartViewModel GetCartOrderStatusList(int orderId, ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (orderId > 0)
            {
                if (cartModel?.ShoppingCartItems?.Count > 0)
                {
                    CartViewModel cartViewModel = cartModel?.ToViewModel<CartViewModel>();
                    List<SelectListItem> orderStatusList = BindManageOrderStatus(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderLineItemState.ToString().ToLower(), FilterOperators.Equals, "true"));
                    cartViewModel?.ShoppingCartItems?.ForEach(x => x.ShippingStatusList = orderStatusList);
                    return cartViewModel;
                }
            }
            return cartModel?.ToViewModel<CartViewModel>();
        }

        //Bind Default order status and line item status on the basis of order state and orderlineitemstate
        public virtual List<SelectListItem> BindManageOrderStatus(FilterTuple filter = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            if (filter != null) { filters.Add(filter); }
            ZnodeLogging.LogMessage("Filters to get order states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return StoreViewModelMap.ToOrderStateList(_orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates, true);
        }


        //Bind Order Status dropdown.
        public virtual List<SelectListItem> BindOrderStatus(FilterTuple filter = null)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString().ToLower(), FilterOperators.Equals, "false"));
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            if (IsNotNull(filters)) { filters.Add(filter); }
            return StoreViewModelMap.ToOrderStateList(_orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates, true);
        }



        //Check if group product sku is same.
        private void CheckGroupProductSKUs(ShoppingCartItemModel sessionCartItem, ShoppingCartItemModel newCartItem, PortalModel portal)
        {
            string groupProdSKU = newCartItem.GroupProducts.FirstOrDefault().Sku;
            string mainGroupProdSKU = sessionCartItem.GroupProducts.FirstOrDefault().Sku;
            ZnodeLogging.LogMessage("groupProdSKU & mainGroupProdSKU:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { groupProdSKU = groupProdSKU, mainGroupProdSKU = mainGroupProdSKU });
            if (!string.IsNullOrEmpty(groupProdSKU) && !string.IsNullOrEmpty(mainGroupProdSKU) && Equals(groupProdSKU.ToLower(), mainGroupProdSKU.ToLower()))
                CheckAddonProductSKUs(sessionCartItem, newCartItem, portal);
        }
        //Check if add on product sku is same.
        private void CheckAddonProductSKUs(ShoppingCartItemModel sessionCartItem, ShoppingCartItemModel newCartItem, PortalModel portal)
        {
            if (!string.IsNullOrEmpty(newCartItem.AddOnProductSKUs) && !string.IsNullOrEmpty(sessionCartItem.AddOnProductSKUs))
            {
                string[] sessionAddOns = sessionCartItem.AddOnProductSKUs.Split(',');
                for (int index = 0; index < sessionAddOns.Length; index++)
                {
                    SetAdditionalLineItemDetails(sessionCartItem, newCartItem, portal);
                }
            }
            else
                SetAdditionalLineItemDetails(sessionCartItem, newCartItem, portal);
        }

        //Set custom data for newly create cart items.
        private void SetAdditionalLineItemDetails(ShoppingCartItemModel sessionCartItem, ShoppingCartItemModel newCartItem, PortalModel portal)
        {
            if (sessionCartItem.OmsOrderLineItemsId > 0)
            {
                newCartItem.CustomUnitPrice = sessionCartItem.CustomUnitPrice;
                newCartItem.TrackingNumber = sessionCartItem.TrackingNumber;
                newCartItem.OmsOrderStatusId = sessionCartItem.OmsOrderStatusId;
                newCartItem.IsEditStatus = true;
                newCartItem.IsSendEmail = sessionCartItem.IsSendEmail;
                newCartItem.OrderLineItemStatus = !string.IsNullOrEmpty(sessionCartItem.OrderLineItemStatus) ? sessionCartItem.OrderLineItemStatus : portal?.OrderStatus?.ToUpper();
                newCartItem.OmsOrderLineItemsId = sessionCartItem.OmsOrderLineItemsId;
                newCartItem.ShipSeperately = sessionCartItem.ShipSeperately;
                newCartItem.PartialRefundAmount = sessionCartItem.PartialRefundAmount;
            }
            else
            {
                newCartItem.IsEditStatus = true;
                newCartItem.OrderLineItemStatus = portal?.OrderStatus?.ToUpper() ?? Admin_Resources.TextPendingApproval.ToUpper();
            }
        }

        //Set OrderState, LineItemState etc for newly added cartItem.
        //This method is used only for Manage Order.
        private void SetAdditionalAutoAddOnLineItemDetails(ShoppingCartItemModel newCartItem, PortalModel portal)
        {
            newCartItem.IsEditStatus = true;
            if (IsNotNull(portal))
            {
                newCartItem.OmsOrderStatusId = portal.OrderStatusId;
                newCartItem.OrderLineItemStatus = portal.OrderStatus?.ToUpper();
            }
        }

        //Calculate all valid and invlid coupon.
        private CartViewModel CalculateAllCoupons(int orderId, CartViewModel cartViewModel, ShoppingCartModel shoppingCartModel, ShoppingCartModel cart, List<CouponModel> promocode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //If cart having invalid coupons then remove all coupons from current cart.
            cart.Coupons.Clear();

            //Add promoCodes(already applied coupon) to cart.
            foreach (CouponModel couponModel in promocode)
                cart.Coupons.Add(couponModel);

            //Set login user and selected user in cache.
            SetCreatedByUser(cart.UserId);

            shoppingCartModel = _shoppingCartsClient.Calculate(cart);

            //Get order status list in model & map to CartViewModel
            cartViewModel = GetCartOrderStatusList(orderId, shoppingCartModel);
            SaveCartInSession(orderId, shoppingCartModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        public void SaveCartInSession(int orderId, ShoppingCartModel cartModel, bool isFromDeleteCarItem = false, ShoppingCartItemModel shoppingCartItem = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (orderId > 0)
            {
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

                RemoveHistoryForCartLineItem(isFromDeleteCarItem, shoppingCartItem, orderModel);
                cartModel.ShoppingCartItems?.Where(x => x.OmsOrderLineItemsId < 1)?.Select(y => { y.IsActive = true; return y; })?.ToList();
                orderModel.ShoppingCartModel = cartModel;
                SaveInSession(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
            }
            else
                SaveCartModelInSession(cartModel?.UserId, cartModel);
        }

        // Remove cart line product history.
        private void RemoveHistoryForCartLineItem(bool isFromDeleteCarItem, ShoppingCartItemModel shoppingCartItem, OrderModel orderModel)
        {
            if (isFromDeleteCarItem && IsNotNull(shoppingCartItem))
            {
                OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel() { OrderLineDelete = shoppingCartItem.ProductName, Quantity = shoppingCartItem.Quantity.ToInventoryRoundOff(), SKU = shoppingCartItem.SKU, OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemsId };
                RemoveKeyFromDictionary(orderModel, shoppingCartItem.ExternalId, true);
                OrderLineHistory(orderModel, shoppingCartItem.ExternalId, orderLineItemHistoryModel);
            }
        }

        //Get current coupon list.
        private List<CouponModel> GetCoupons(ShoppingCartModel cartModel)
        {
            //Applied coupons will add to promoCodes list.
            List<CouponModel> promoCodes = new List<CouponModel>();

            if (IsNotNull(cartModel.Coupons))
            {
                //Add to promoCodes when shopping cart having coupons already.
                foreach (CouponModel coupon in cartModel.Coupons)
                {
                    if (coupon.CouponApplied)
                        promoCodes.Add(coupon);
                }
            }
            ZnodeLogging.LogMessage("promoCodes list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, promoCodes?.Count());
            return promoCodes;
        }

        //Get new coupon code and added to cart.
        private void GetNewCouponToApply(ShoppingCartModel cartModel, string couponCode)
        {
            bool isCouponNotAvailableInCart = Equals(cartModel.Coupons.Where(p => Equals(p.Code, couponCode)).ToList().Count, 0);
            if (isCouponNotAvailableInCart)
            {
                CouponModel newCoupon = new CouponModel { Code = couponCode };
                cartModel.Coupons.Add(newCoupon);
            }
        }

        //return false if invalidCoupons contains invalid coupons.
        private bool IsCouponApplied(CartViewModel cartViewModel, List<CouponViewModel> invalidCoupons)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            bool cartCouponsApplied = (invalidCoupons.Count > 0 ? false : true);

            OrderModel orderModel = new OrderModel();
            if (cartViewModel.OmsOrderId > 0)
            {
                orderModel = GetOrderModelFromSession(cartViewModel.OmsOrderId);
                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderCoupon);
            }
            StringBuilder sb = new StringBuilder();
            foreach (CouponViewModel couponViewModel in cartViewModel.Coupons)
            {
                if (couponViewModel.CouponApplied)
                {
                    cartCouponsApplied = true;
                    if (cartViewModel.OmsOrderId > 0 && !couponViewModel.IsExistInOrder)
                        sb.AppendFormat("{0},", couponViewModel.Code);
                }
                else
                {
                    cartCouponsApplied = false;
                    invalidCoupons.Add(couponViewModel);
                }
            }
            string coupons = sb.ToString();
            ZnodeLogging.LogMessage("coupons:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { coupons = coupons });
            if (!string.IsNullOrEmpty(coupons) && cartViewModel.OmsOrderId > 0)
            {
                OrderHistory(orderModel, ZnodeConstant.OrderCoupon, string.Empty, coupons.TrimEnd(','));
                SaveOrderModelInSession(cartViewModel.OmsOrderId, orderModel);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return cartCouponsApplied;
        }

        //to remove invalid coupon form cart
        private void RemoveInvalidCoupon(ShoppingCartModel cartModel)
        {
            if (cartModel.Coupons?.Count > 0)
                cartModel.Coupons.RemoveAll(x => !x.CouponApplied && !string.IsNullOrEmpty(x.PromotionMessage));
        }

        //Set Gift Card Number and CSR Discount Amount Data For Calculation
        public static void SetCartDataForCalculation(ShoppingCartModel cartModel, string GiftCardNumber, decimal CSRDiscountAmount)
        {
            cartModel.GiftCardNumber = GiftCardNumber;
            cartModel.CSRDiscountAmount = CSRDiscountAmount;
        }

        // if persistent cart disabled, we need not call below method, need to check with portal record.
        public void SaveCartInCookie(ShoppingCartModel cartModel, PortalModel portal = null)
        {
            if ((IsNull(cartModel.UserDetails) || cartModel.UserDetails.UserId == 0) && (IsNotNull(portal) && IsNotNull(portal.AvailablePortalFeatures)) ? StoreViewModelMap.ToViewModel(portal).PersistentCartEnabled : StoreAgent.CurrentStore.PersistentCartEnabled)
                SaveInCookie(AdminConstants.CartCookieKey, cartModel.CookieMappingId.ToString());
        }

        //Cart Calculate And Save In Session.
        public virtual ShoppingCartModel CartCalculateAndSaveInSession(ShoppingCartModel cart, int orderId = 0, bool isFromDeleteCarItem = false, ShoppingCartItemModel shoppingCartItem = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            cart.IsMerged = true;
            //Create new cart.
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.CreateCart(cart);

            if (orderId > 0)
                shoppingCartModel = GetUpdatedShoppingCart(cart, shoppingCartModel);

            shoppingCartModel.Payment = new PaymentModel() { ShippingAddress = cart.ShippingAddress, PaymentSetting = new PaymentSettingModel() };

            //Set Gift Card Number and CSR Discount Amount Data For Calculation
            SetCartDataForCalculation(shoppingCartModel, cart.GiftCardNumber, cart.CSRDiscountAmount);

            //Calculate shopping cart.
            shoppingCartModel = _shoppingCartsClient.Calculate(shoppingCartModel);

            shoppingCartModel.UserDetails = cart.UserDetails;
            //Add address in shopping cart
            shoppingCartModel.ShippingAddress = cart.ShippingAddress;
            //Save shopping cart in session.
            SaveCartInSession(orderId, shoppingCartModel, isFromDeleteCarItem, shoppingCartItem);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCartModel;
        }

        //Apply Voucher By voucher Number.
        public virtual CartViewModel ApplyVoucher(string voucherNumber, int orderId = 0, int? userId = 0)
        {
            ShoppingCartModel cartModel = GetShoppingCartFromSession(orderId, userId);
            CartViewModel cartViewModel = new CartViewModel ();
            RemoveInvalidVouchers(cartModel);
            //Bind new voucher to shopping cart model to apply.
            if (!string.IsNullOrEmpty(voucherNumber))
            {
                AddNewVoucherToCart(cartModel, voucherNumber);
            }
            //Set login user and selected user in cache.
            SetCreatedByUser(cartModel.UserId);
            cartModel.GiftCardNumber = string.Empty;
            cartModel.IsCalculateVoucher = true;
            ShoppingCartModel shoppingCartModel = _shoppingCartsClient.Calculate(cartModel);

            if(IsNotNull( shoppingCartModel))
            {
                SaveCartInSession(orderId, shoppingCartModel);
                //Get order status list in model & map to CartViewModel
                cartViewModel = GetCartOrderStatusList(orderId, shoppingCartModel);
            }
            
            OrderModel orderModel = new OrderModel();
            if (orderId > 0)
                orderModel = GetOrderModelFromSession(orderId);

            AddAppliedVoucherToOrderHistory(cartViewModel);

            cartModel.Vouchers = shoppingCartModel.Vouchers;
            cartModel.GiftCardAmount = shoppingCartModel.GiftCardAmount;
            SaveOrderModelInSession(orderId, orderModel);
            return cartViewModel;
        }
     
        //Remove the applied voucher from the cart.
        public virtual CartViewModel RemoveVoucher(string voucherCode, int orderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetShoppingCartFromSession(orderId, userId);

            if (IsNull(cart))
                return (CartViewModel)GetViewModelWithErrorMessage(new CartViewModel(), string.Empty);

            OrderModel orderModel = new OrderModel();
            if (orderId > 0)
                orderModel = GetOrderModelFromSession(orderId);

            if (IsNotNull(cart.Vouchers))
            {
                StringBuilder sb = new StringBuilder();
                //Remove voucher from cart.
                foreach (VoucherModel voucherModel in cart.Vouchers)
                {
                    if (Equals(voucherModel.VoucherNumber, voucherCode))
                    {
                        if (orderId > 0)
                            sb.AppendFormat("{0},", voucherModel.VoucherNumber);
                        if (cart.Vouchers.Count == 1)
                        {
                            cart.GiftCardNumber = string.Empty;
                            cart.IsAllVoucherRemoved = true;
                        }
                        cart.Vouchers.Remove(voucherModel);
                        break;
                    }
                }

                string voucher = sb?.ToString();
                ZnodeLogging.LogMessage("Voucher:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { coupons = voucher });
                if (!string.IsNullOrEmpty(voucher))
                {
                    SaveOrderModelInSession(orderId, orderModel);
                }

                cart.IsCalculateVoucher = true;
                cart = _shoppingCartsClient.Calculate(cart);
                if ( orderId > 0)
                {
                    RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderRemoveVoucher);
                    OrderHistory(orderModel, ZnodeConstant.OrderRemoveVoucher, string.Empty, voucherCode);
                }
   
                SaveCartInSession(orderId, cart);
            }
                               
            //Get order status list in model & map to CartViewModel
            CartViewModel cartViewModel = GetCartOrderStatusList(orderId, cart);
            cartViewModel.OrderState = orderModel?.OrderState;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartViewModel;
        }

        private CartParameterModel MapCartParameterModel(int portalId, int userId)
        {
            CartParameterModel cartParameter = new CartParameterModel();
            cartParameter.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            cartParameter.PortalId = portalId;
            cartParameter.UserId = userId;
            return cartParameter;
        }

        //Get new voucher code and add it to cart.
        protected virtual void AddNewVoucherToCart(ShoppingCartModel cartModel, string voucherNumber)
        {
            //Check if any voucher is already appled to cart and add new coupon.
            bool isVoucherNotAvailableInCart = Equals(cartModel.Vouchers?.Count(p => Equals(p.VoucherNumber, voucherNumber)), 0);

            //Add voucher code if not present in cart.
            if (isVoucherNotAvailableInCart)
            {
                cartModel.Vouchers.Add(new VoucherModel { VoucherNumber = voucherNumber });
            }
        }

        // Remove invalid vouchers
        protected virtual void RemoveInvalidVouchers(ShoppingCartModel cartModel)
        {
            if (cartModel?.Vouchers?.Count > 0)
            {
                cartModel.Vouchers.RemoveAll(x => !x.IsVoucherApplied && !string.IsNullOrEmpty(x.VoucherMessage));
            }
        }

        //Add Vouchers to order History.
        protected virtual void AddAppliedVoucherToOrderHistory(CartViewModel cartViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            OrderModel orderModel = new OrderModel();
            if (cartViewModel?.OmsOrderId > 0)
            {
                orderModel = GetOrderModelFromSession(cartViewModel.OmsOrderId);
                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderVoucher);
            }
            StringBuilder sb = new StringBuilder();
            foreach (VouchersViewModel voucherViewModel in cartViewModel?.Vouchers)
            {
                if (voucherViewModel.IsVoucherApplied)
                {
                    if (cartViewModel?.OmsOrderId > 0 && !voucherViewModel.IsExistInOrder)
                        sb.AppendFormat("{0},", voucherViewModel.VoucherNumber);
                }
            }
            string vouchers = sb.ToString();
            ZnodeLogging.LogMessage("Vouchers:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { vouchers = vouchers });
            if (!string.IsNullOrEmpty(vouchers) && cartViewModel?.OmsOrderId > 0)
            {
                OrderHistory(orderModel, ZnodeConstant.OrderVoucher, string.Empty, vouchers.TrimEnd(','));
                SaveOrderModelInSession(cartViewModel.OmsOrderId, orderModel);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        #endregion Private Methods
    }
}