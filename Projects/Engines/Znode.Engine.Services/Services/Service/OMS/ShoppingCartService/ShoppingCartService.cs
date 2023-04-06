using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Shipping;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using Znode.Engine.Promotions;
using static Znode.Engine.Shipping.FedEx.FedExEnum;

namespace Znode.Engine.Services
{
    public class ShoppingCartService : BaseService, IShoppingCartService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeOmsCookieMapping> _cookieMappingRepository;
        private readonly IZnodeRepository<ZnodeOmsSavedCart> _omsSavedRepository;
        private readonly IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemService;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _orderLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItemRelationshipType> _lineItemRelationshipType;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodeAccount> _accountRepository;
        private IPublishedProductDataService publishedProductData;

        private readonly IZnodeRepository<ZnodeState> _stateRepository;
        private readonly IPublishProductHelper publishProductHelper;
        private readonly IZnodeOrderHelper orderHelper;
        public static string SKU { get; } = "sku";
        public static string Width { get; } = "width";
        public static string Height { get; } = "height";
        private readonly IShoppingCartMap _shoppingCartMap;
        private readonly IShoppingCartItemMap _shoppingCartItemMap;
        public readonly string[] _upsLTLCode = { "308", "309", "310" };
        #endregion

        #region Constructor
        public ShoppingCartService()
        {
            _cookieMappingRepository = new ZnodeRepository<ZnodeOmsCookieMapping>();
            _omsSavedRepository = new ZnodeRepository<ZnodeOmsSavedCart>();
            _savedCartLineItemService = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _lineItemRelationshipType = new ZnodeRepository<ZnodeOmsOrderLineItemRelationshipType>();
            _stateRepository = new ZnodeRepository<ZnodeState>();
            publishProductHelper = GetService<IPublishProductHelper>();
            orderHelper = GetService<IZnodeOrderHelper>();
            _orderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _shoppingCartMap = GetService<IShoppingCartMap>();
            _shoppingCartItemMap = GetService<IShoppingCartItemMap>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _accountRepository = new ZnodeRepository<ZnodeAccount>();
            publishedProductData = GetService<IPublishedProductDataService>();
        }
        #endregion

        #region Public Methods
        //To get shopping cart.
        public virtual ShoppingCartModel GetShoppingCart(CartParameterModel cartParameterModel)
        {
            if(IsNotNull(cartParameterModel.OmsOrderId) && cartParameterModel.OmsOrderId > 0)
            {
                return GetCartByOrderId(cartParameterModel);
            }
            else if (IsNotNull(cartParameterModel.OmsQuoteId) && cartParameterModel.OmsQuoteId > 0)
            {
                return LoadCartForQuote(cartParameterModel);
            }
            else
            {
                return GetShoppingCartDetails(cartParameterModel);
            }
        }
         
        //To get shopping cart count
        public virtual string GetCartCount(CartParameterModel cartParameterModel)
        {
            int mappingId = !string.IsNullOrEmpty(cartParameterModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cartParameterModel.CookieMappingId)) : 0;

            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            objStoredProc.SetParameter("@OmsCookieMappingId", mappingId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", cartParameterModel.UserId ?? 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", cartParameterModel.PortalId, ParameterDirection.Input, DbType.Int32);
            string result = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsSavedCartLineItemCount @OmsCookieMappingId,@UserId,@PortalId").FirstOrDefault();
            return result;          
        }
        

        //To Create shopping cart.
        public virtual ShoppingCartModel CreateCart(ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(shoppingCart))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ShoppingCartModelNotNull);

            ZnodeLogging.LogMessage("OmsOrderId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCart.OmsOrderId);
            ZnodeLogging.LogMessage("ShoppingCartItems count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCart.ShoppingCartItems?.Count);
            if (shoppingCart.OmsOrderId > 0 && shoppingCart.ShoppingCartItems?.Count > 0)
                shoppingCart.ShoppingCartItems.ForEach(x => x.OmsOrderId = shoppingCart.OmsOrderId > 0 ? shoppingCart.OmsOrderId : null);

            int shippingId = 0;
            string countryCode = string.Empty;
            if (HelperUtility.IsNotNull(shoppingCart.Shipping))
            {
                shippingId = shoppingCart.Shipping.ShippingId < 1 ? shoppingCart.ShippingId : Convert.ToInt32(shoppingCart.Shipping.ShippingId);
                countryCode = string.IsNullOrEmpty(shoppingCart?.ShippingAddress?.CountryName) ? shoppingCart.Shipping.ShippingCountryCode : shoppingCart?.ShippingAddress?.CountryName;
            }

            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { shippingId = shippingId, countryCode = countryCode });
            //Set Shipping Address To shopping cart payment model.
            SetShippingAddressToPayment(shoppingCart);
            ZnodeLogging.LogMessage("Country code:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, countryCode);
            IZnodeShoppingCart znodeShoppingCart = GetService<IZnodeShoppingCart>();
            //Edit order flow in admin is not using this merging.
            if (shoppingCart.UserId > 0 && shoppingCart.IsMerged && shoppingCart.UserId == GetLoginUserId())
                MergeShoppingCartItems(shoppingCart);
            ZnodeLogging.LogMessage("User Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCart.UserId);
            if (!string.IsNullOrEmpty(shoppingCart.RemoveAutoAddonSKU))
                RemoveAssociatedAutoAddon(shoppingCart);

            //To save cart line items in saved cart table and get cookieMappingId
            int cookieMappingId = znodeShoppingCart.Save(shoppingCart, DefaultGlobalConfigSettingHelper.DefaultGroupIdProductAttribute, DefaultGlobalConfigSettingHelper.DefaultGroupIdPersonalizeAttribute);
            ZnodeLogging.LogMessage("CookieMappingId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, cookieMappingId);
            if (cookieMappingId > 0)
                return GetShoppingCartDetails(new CartParameterModel
                {
                    CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString()),
                    LocaleId = shoppingCart.LocaleId,
                    PortalId = shoppingCart.PortalId,
                    PublishedCatalogId = shoppingCart.PublishedCatalogId,
                    UserId = shoppingCart.UserId,
                    ShippingId = shippingId,
                    ShippingCountryCode = countryCode,
                    OmsOrderId = shoppingCart.OmsOrderId,
                    ProfileId = GetProfileId()
                }, shoppingCart);
            return null;
        }

        //Add product in the cart.
        public virtual AddToCartModel AddToCartProduct(AddToCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(shoppingCart))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.AddToCartModelNotNull);

            //Validate all the SKUs 
            ValidateSKUDetails(shoppingCart);

            AddToCartModel cartModel = GetService<IZnodeShoppingCart>().SaveAddToCartData(shoppingCart, DefaultGlobalConfigSettingHelper.DefaultGroupIdProductAttribute, DefaultGlobalConfigSettingHelper.DefaultGroupIdPersonalizeAttribute);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartModel;
        }

        public virtual PublishProductModel GetPublishProduct(AddToCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int? catlogVersionId = GetCatalogVersionId(shoppingCart.PublishedCatalogId);

            ShoppingCartItemModel lineItem = shoppingCart.ShoppingCartItems.FirstOrDefault();

            string sku = string.IsNullOrEmpty(lineItem.ConfigurableProductSKUs) ? lineItem.SKU : lineItem.ConfigurableProductSKUs;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { catlogVersionId = catlogVersionId, ProductId = lineItem.ProductId, sku = sku });

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return GetService<IPublishedProductDataService>().GetPublishProductBySKU(sku, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId, catlogVersionId)?.ToModel<PublishProductModel>();
        }

        //Calculate the tax, shipping, discount for cart.
        public virtual ShoppingCartModel Calculate(ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(shoppingCartModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ShoppingCartModelNotNull);

            //remove duplicate coupons with same name but different cases.
            ZnodeLogging.LogMessage("Coupons details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCartModel.Coupons);

            shoppingCartModel.IsAllowWithOtherPromotionsAndCoupons = DefaultGlobalConfigSettingHelper.IsAllowWithOtherPromotionsAndCoupons;
            RemoveDuplicateCoupons(shoppingCartModel.Coupons);
           
            SetShippingStateCode(shoppingCartModel);
            List<string> expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing };
            ZnodeShoppingCart znodeShoppingCart = _shoppingCartMap.ToZnodeShoppingCart(shoppingCartModel, null, expands);

            
            SetAllowTerritories(znodeShoppingCart, shoppingCartModel.BillingAddress);
            znodeShoppingCart.PublishStateId = znodeShoppingCart.PublishStateId == 0 ? shoppingCartModel.PublishStateId : znodeShoppingCart.PublishStateId;
            znodeShoppingCart.Calculate(znodeShoppingCart.ProfileId, shoppingCartModel.IsCalculateTaxAndShipping, shoppingCartModel.IsCalculatePromotionAndCoupon);
            ShoppingCartModel calculatedModel = _shoppingCartMap.ToModel(znodeShoppingCart, GetService<IImageHelper>());
            if (calculatedModel?.ShoppingCartItems?.FirstOrDefault(x => x.ProductType == ZnodeConstant.BundleProduct) != null)
                BindBundleProductChilds(calculatedModel, znodeShoppingCart);
            calculatedModel.ShippingAddress = shoppingCartModel.ShippingAddress;
            calculatedModel.BillingAddress = shoppingCartModel.BillingAddress;
            calculatedModel.CurrencyCode = shoppingCartModel.CurrencyCode;
            calculatedModel.CultureCode = shoppingCartModel.CultureCode;
            if(IsNotNull(calculatedModel?.Shipping))
                calculatedModel.Shipping.ShippingDiscountType = znodeShoppingCart?.Shipping?.ShippingDiscountType;
            if (IsNotNull(calculatedModel) && IsNotNull(calculatedModel.ShoppingCartItems))
            {
                List<string> skus = calculatedModel.ShoppingCartItems?.Select(x => x.SKU).ToList();

                List<ZnodePimDownloadableProduct> lstDownloadableProducts = null;

                if (IsNotNull(shoppingCartModel.OmsOrderId) && shoppingCartModel.OmsOrderId > 0)
                    lstDownloadableProducts = new ZnodeRepository<ZnodePimDownloadableProduct>().Table.Where(x => skus.Contains(x.SKU)).ToList();

                //Get the discount amount of each cartline item.
                foreach (ShoppingCartItemModel shoppingCartItem in calculatedModel.ShoppingCartItems)
                {
                    List<ShoppingCartItemModel> cartItem = shoppingCartModel.ShoppingCartItems.Where(product => Equals(product.ProductId, shoppingCartItem.ProductId))?.ToList();
                    decimal modelCartItemDiscountAmount = IsNotNull(cartItem.FirstOrDefault()) ? cartItem.FirstOrDefault().ProductDiscountAmount : 0;

                    if (shoppingCartItem.ProductDiscountAmount <= 0.0M && modelCartItemDiscountAmount > 0.0M)
                        shoppingCartItem.ProductDiscountAmount = shoppingCartModel.ShoppingCartItems.FirstOrDefault(product => Equals(product.ProductId, shoppingCartItem.ProductId)).ProductDiscountAmount;
                    shoppingCartItem.CurrencyCode = shoppingCartModel.CurrencyCode;
                    shoppingCartItem.CultureCode = shoppingCartModel.CultureCode;
                    if (IsNotNull(shoppingCartModel.OmsOrderId) && shoppingCartModel.OmsOrderId > 0)
                    {
                        bool IsDownloadableSKU = lstDownloadableProducts.Any(x => x.SKU == shoppingCartItem.SKU);
                        if (IsDownloadableSKU)
                        {
                            int? parentOmsOrderLineItemsId = _orderLineItemRepository.Table.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId && x.IsActive).ParentOmsOrderLineItemsId;
                            if (parentOmsOrderLineItemsId > 0)
                            {
                                shoppingCartItem.DownloadableProductKey = GetProductKey(shoppingCartItem.SKU, Convert.ToInt32(parentOmsOrderLineItemsId));
                            }
                        }
                    }
                    shoppingCartItem.CultureCode = shoppingCartModel.CultureCode;
                }

                bool? isTrackInventory = shoppingCartModel.ShoppingCartItems?.FirstOrDefault()?.TrackInventory;

                if (IsNotNull(isTrackInventory))
                    calculatedModel.ShoppingCartItems?.ForEach(item => { item.TrackInventory = isTrackInventory.GetValueOrDefault(); });

            }
            //to set order over due amount
            if (IsNotNull(shoppingCartModel.OmsOrderId) && shoppingCartModel.OmsOrderId > 0)
                SetOrderOverDueAmount(calculatedModel);
            ZnodeLogging.LogMessage("Oms Quote Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCartModel.OmsQuoteId);
            if (shoppingCartModel.OmsQuoteId > 0)
            {
                calculatedModel.OmsQuoteId = shoppingCartModel.OmsQuoteId;
                calculatedModel.OrderStatus = shoppingCartModel.OrderStatus;
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return calculatedModel;
        }

        protected virtual void SetInventoryData(List<AssociatedPublishedBundleProductModel> products)
        {
            if (IsNotNull(products) && products.Count > 0)
            {
                foreach(AssociatedPublishedBundleProductModel product in products)
                {
                    string inventorySettingCode = product.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues?.FirstOrDefault()?.Code;
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
        }

        protected virtual void BindBundleProductChilds(ShoppingCartModel shoppingCartModel, ZnodeShoppingCart znodeShoppingCart)
        {
            string bundleProductSkus = string.Join(",", shoppingCartModel?.ShoppingCartItems?.Where(x => x.ProductType == ZnodeConstant.BundleProduct).Select(y => y.SKU));
            List<ZnodeOmsOrderLineItem> orderListItem = new List<ZnodeOmsOrderLineItem>();
            if (znodeShoppingCart.OrderId > 0)
            {
                orderListItem = orderHelper.GetOrderLineItemByOmsOrderId(znodeShoppingCart.OrderId.Value);
            }

            if (!string.IsNullOrEmpty(bundleProductSkus))
            {
                List<AssociatedPublishedBundleProductModel> associatedPublishedBundleProductModels = znodeShoppingCart.BindBundleProductChildByParentSku(bundleProductSkus, shoppingCartModel.PublishedCatalogId, shoppingCartModel.LocaleId);

                SetInventoryData(associatedPublishedBundleProductModels);
                shoppingCartModel.ShoppingCartItems?.ForEach(product =>
                {
                    product.BundleProducts = associatedPublishedBundleProductModels?.Where(y => y.ParentBundleSKU == product.SKU)?.ToList();
                    if (znodeShoppingCart.OrderId > 0 && product.BundleProducts != null)
                    {
                        product.BundleProducts?.ForEach(bProduct =>
                        {
                            bProduct.AssociatedQuantity = orderListItem.FirstOrDefault(d => d.Sku == bProduct.SKU)?.BundleQuantity;
                        });
                    }
                });
            }

        }

        //Get Download product key of product
        private string GetProductKey(string sku, int omsOrderLineItemsId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { sku = sku, omsOrderLineItemsId = omsOrderLineItemsId });
            string productKey = string.Empty;
            IZnodeRepository<ZnodePimDownloadableProduct> _pimDownloadableProduct = new ZnodeRepository<ZnodePimDownloadableProduct>();
            IZnodeRepository<ZnodePimDownloadableProductKey> _pimDownloadableProductKey = new ZnodeRepository<ZnodePimDownloadableProductKey>();
            IZnodeRepository<ZnodeOmsDownloadableProductKey> _omsDownloadableProductKey = new ZnodeRepository<ZnodeOmsDownloadableProductKey>();

            var productKeyDetails =
                from omsDownloadableProductKey in _omsDownloadableProductKey.Table
                join pimDownloadableProductKey in _pimDownloadableProductKey.Table on omsDownloadableProductKey.PimDownloadableProductKeyId equals pimDownloadableProductKey.PimDownloadableProductKeyId
                join pimDownloadableProduct in _pimDownloadableProduct.Table on pimDownloadableProductKey.PimDownloadableProductId equals pimDownloadableProduct.PimDownloadableProductId
                where pimDownloadableProduct.SKU == sku && pimDownloadableProductKey.IsUsed && omsDownloadableProductKey.OmsOrderLineItemsId == omsOrderLineItemsId
                select new { keys = pimDownloadableProductKey.DownloadableProductKey }.keys;

            productKey = string.Join(",", productKeyDetails);
            ZnodeLogging.LogMessage("Product Key:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, productKey);
            return productKey;
        }

        //To get ShoppingCart by cookieId 
        public virtual ShoppingCartModel GetShoppingCartDetails(CartParameterModel cartParameterModel, ShoppingCartModel cartModel = null)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            cartParameterModel.ProfileId = cartModel?.ProfileId ?? GetProfileId();
            ZnodeLogging.LogMessage("Profile Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, cartParameterModel?.ProfileId);

            SetPublishCatalogId(cartParameterModel);

            List<string> expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing, ZnodeConstant.AddOns, ZnodeConstant.SEO };
            ZnodeShoppingCart znodeShoppingCart = GetService<IZnodeShoppingCart>().LoadFromDatabase(cartParameterModel, expands);

            znodeShoppingCart.IsAllowWithOtherPromotionsAndCoupons = DefaultGlobalConfigSettingHelper.IsAllowWithOtherPromotionsAndCoupons;
            //to map cart model to znode shopping cart  
            BindCartModel(cartModel, znodeShoppingCart);

            IImageHelper imageHelper = GetService<IImageHelper>();

            //Map Libraries.ECommerce.ShoppingCart to ShoppingCartModel.
            ShoppingCartModel shoppingCartModel = _shoppingCartMap.ToModel(znodeShoppingCart, imageHelper);

            //Map null data with requesting cart model
            MapNullDataWithRequestingCartModel(cartModel, shoppingCartModel);

            ZnodeLogging.LogMessage("Coupon count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, cartModel?.Coupons?.Count);
            //Get coupons if already applied.
            if (cartModel?.Coupons?.Count > 0)
                shoppingCartModel.Coupons = cartModel.Coupons;

            ZnodeLogging.LogMessage("Shipping Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, cartParameterModel?.ShippingId);
            if (HelperUtility.IsNotNull(cartParameterModel.ShippingId))
            {
                IZnodeRepository<ZnodeShipping> _shippingRepository = new ZnodeRepository<ZnodeShipping>();

                //Check if Shipping is null or not,If null then get the shipping 
                //on the basis of ShippingId form ShoppingCartModel.
                ZnodeShipping shipping = _shippingRepository.Table.FirstOrDefault(x => x.ShippingId == cartParameterModel.ShippingId);

                if (HelperUtility.IsNotNull(shipping))
                {
                    shoppingCartModel.Shipping = new OrderShippingModel
                    {
                        ShippingId = shipping.ShippingId,
                        ShippingDiscountDescription = shipping.Description,
                        ShippingCountryCode = string.IsNullOrEmpty(cartParameterModel.ShippingCountryCode) ? string.Empty : cartParameterModel.ShippingCountryCode
                    };
                }
            }

            ZnodeLogging.LogMessage("ShoppingCartItem:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCartModel?.ShoppingCartItems);
            //Check product inventory of the product for all type of product in cart-line item.
            if (HelperUtility.IsNotNull(shoppingCartModel?.ShoppingCartItems))
            {
                if (cartModel?.ShoppingCartItems?.Count > 0)
                {
                    shoppingCartModel?.ShoppingCartItems.ForEach(cartItem =>
                    {
                        var lineItem = cartModel.ShoppingCartItems
                                    .Where(cartLineItem => cartLineItem.SKU == cartItem.SKU && cartLineItem.AddOnProductSKUs == cartItem.AddOnProductSKUs)?.FirstOrDefault();
                        cartItem.OmsOrderLineItemsId = (lineItem?.OmsOrderLineItemsId).GetValueOrDefault();
                        cartItem.CartDescription = string.IsNullOrEmpty(cartItem.CartDescription) ? lineItem?.CartDescription : cartItem.CartDescription;

                    });
                }
                BindBundleProductChildBySKU(cartParameterModel, shoppingCartModel);
                //Single call instead multiple inventory check.
                CheckBaglineItemInventory(shoppingCartModel, cartParameterModel);
            }

            //Bind cookieMappingId, PortalId, LocaleId, CatalogId, UserId.
            BindCartData(shoppingCartModel, cartParameterModel);

            if (IsNotNull(shoppingCartModel) && cartModel?.OmsOrderId > 0)
                shoppingCartModel.BillingAddress = cartModel.BillingAddress;

            IPIMAttributeService _pimAttributeService = GetService<IPIMAttributeService>();
            //Binding Shoppingcart items' personalize attribute names.
            //TODO: _pimAttributeService.GetAttributeLocale will be called for each personalized attr of each shopping cart item.
            // So this code may perform slow if cart items are more quantity.
            shoppingCartModel.ShoppingCartItems.Where(s => s.PersonaliseValuesDetail != null && s.PersonaliseValuesDetail.Count > 0).ToList()
                .ForEach(x => x.PersonaliseValuesDetail
                    .ForEach(p => p.PersonalizeName = _pimAttributeService.GetAttributeLocale(p.PersonalizeCode, shoppingCartModel.LocaleId)));

            if (cartModel?.Vouchers?.Count > 0)
                shoppingCartModel.Vouchers = cartModel.Vouchers;

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCartModel;
        }

        protected virtual void BindBundleProductChildBySKU(CartParameterModel cartParameterModel, ShoppingCartModel shoppingCartModel)
        {
            string bundleParentProductSkus = string.Join(",", shoppingCartModel?.ShoppingCartItems.Where(x => x.ProductType == ZnodeConstant.BundleProduct).Select(y => y.SKU));
            if (!string.IsNullOrEmpty(bundleParentProductSkus))
            {
                IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
                List<AssociatedPublishedBundleProductModel> publishBundleChildModel = znodeShoppingCarts.BindBundleProductChildByParentSku(bundleParentProductSkus, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId);

                shoppingCartModel?.ShoppingCartItems.ForEach(ShoppingCartItem => {
                    ShoppingCartItem.BundleProducts = new List<AssociatedPublishedBundleProductModel>();
                    ShoppingCartItem.BundleProducts.AddRange(publishBundleChildModel.Where(x => x.ParentBundleSKU == ShoppingCartItem.SKU).Select(x => new AssociatedPublishedBundleProductModel()
                    {
                        SKU = x.SKU,
                        Quantity = x.AssociatedQuantity * ShoppingCartItem.Quantity
                    }));
                });
            }            
        }

        //to delete saved cart from data base for this user
        public virtual bool RemoveSavedCartItems(int? userId, int? cookieMappingId, int? portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, cookieMappingId = cookieMappingId });
            bool status = false;
            //To delete saved cart line items
            status = DeleteSavedCartItems(userId.GetValueOrDefault(), cookieMappingId.GetValueOrDefault(), portalId.GetValueOrDefault());
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessSavedCartLineItemDelete : Admin_Resources.ErrorSavedCartLineItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //to delete saved cart line item from data base 
        public virtual bool RemoveSavedCartLineItem(int omsSavedCartLineItemId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsSavedCartLineItemId = omsSavedCartLineItemId });
            bool status = false;
            if (omsSavedCartLineItemId > 0)
            {
                List<ZnodeOmsSavedCartLineItem> savedCartLineItems = _savedCartLineItemService.Table.Where(o => o.ParentOmsSavedCartLineItemId == omsSavedCartLineItemId || o.OmsSavedCartLineItemId == omsSavedCartLineItemId)
                                                                      ?.ToList();
                if (IsNotNull(savedCartLineItems))
                {
                    int omsSavedCartId = savedCartLineItems.Count > 0 ? savedCartLineItems.FirstOrDefault().OmsSavedCartId : 0;

                    //Get parent ids
                    List<int?> parentIds = savedCartLineItems.Where(o => (o.OmsSavedCartLineItemId == omsSavedCartLineItemId || o.ParentOmsSavedCartLineItemId == omsSavedCartLineItemId) && o.ParentOmsSavedCartLineItemId != null)
                                                                           ?.Select(o => o.ParentOmsSavedCartLineItemId)?.Distinct()
                                                                           ?.ToList();
                    ZnodeLogging.LogMessage("parentIds:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, parentIds);

                    int omsSaveCartLineItemId = SetOmsSaveCartLineItemId(parentIds, omsSavedCartLineItemId, savedCartLineItems);

                    //Remove personalized attribute 
                    RemovePersonalizedAttribute(parentIds, omsSaveCartLineItemId);

                    //Get child line item count
                    int childCount = parentIds.Join(_savedCartLineItemService.Table,
                                              o => o,
                                              ob => ob.ParentOmsSavedCartLineItemId,
                                              (o, ob) => o)
                                              .Count();
                    ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { childCount = childCount });

                    //GEt line items to delete, if there is only one child then remove parent line as well otherwise do not delete the parent.
                    IQueryable<ZnodeOmsSavedCartLineItem> lineItems = GetCartLineItems(omsSavedCartLineItemId, childCount, parentIds);

                    ZnodeLogging.LogMessage("Removing line item :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, lineItems);
                    status = _savedCartLineItemService.Delete(lineItems);

                    if (status && omsSavedCartId > 0)
                    {
                        var savedCart = _omsSavedRepository.Table.FirstOrDefault(x => x.OmsSavedCartId == omsSavedCartId);
                        savedCart.ModifiedDate = DateTime.Now;
                        _omsSavedRepository.Update(savedCart);
                    }
                    ZnodeLogging.LogMessage(status ? "Saved cart line item deleted successfully." : "Failed to delete saved cart line item.", string.Empty, TraceLevel.Info);
                }
            }

            return status;
        }

        //Remove personalized attribute 
        protected virtual bool RemovePersonalizedAttribute(List<int?> omsSavedCartLineItemId, int childSavedCartLineItemId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsSavedCartLineItemId = omsSavedCartLineItemId, childSavedCartLineItemId = childSavedCartLineItemId });

            bool result = false;
            if (omsSavedCartLineItemId.Any())
            {
                IZnodeRepository<ZnodeOmsPersonalizeCartItem> _personalizeCartItem = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
                FilterCollection filters = new FilterCollection
                {
                    new FilterTuple(ZnodeOmsPersonalizeCartItemEnum.OmsSavedCartLineItemId.ToString(), ProcedureFilterOperators.In, string.Join(",", omsSavedCartLineItemId) + "," + childSavedCartLineItemId)
                };

                //Delete all Personalized attribute list if exists 
                result = _personalizeCartItem.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            }
            ZnodeLogging.LogMessage("Delete all Personalized attribute list if exists flag:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, result);
            return result;
        }

        // to set shipping state code
        public virtual void SetShippingStateCode(ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(shoppingCartModel?.ShippingAddress))
                shoppingCartModel.ShippingAddress.StateCode = GetStateCode(shoppingCartModel?.ShippingAddress?.StateName, shoppingCartModel?.ShippingAddress?.CountryName);
        }

        public virtual ShippingListModel GetShippingEstimates(string zipCode, ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Base - GetShippingEstimates", "Custom", TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorShoppingCartModelNull);

            if (string.IsNullOrEmpty(zipCode))
                return null;

            List<ShippingModel> listwithRates = new List<ShippingModel>();
            try
            {
                List<ShippingModel> list = GetService<IShippingService>().GetShippingListByUserDetails(model.UserId.HasValue ? model.UserId.GetValueOrDefault() : GetLoginUserId(), model.PortalId);
                
                ZnodeLogging.LogMessage("Shipping list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count);
                if (list?.Count > 0)
                {
                    if (Equals(model.ShippingAddress, null))
                    {
                        IZnodeRepository<ZnodeAddress> _addressRepository = new ZnodeRepository<ZnodeAddress>();
                        IZnodeRepository<ZnodeUserAddress> _addressUserRepository = new ZnodeRepository<ZnodeUserAddress>();
                        var shippingAddress = (from p in _addressRepository.Table
                                               join q in _addressUserRepository.Table
                                               on p.AddressId equals q.AddressId
                                               where (q.UserId == model.UserId) && (p.IsDefaultShipping)
                                               select new AddressModel
                                               {
                                                   StateName = p.StateName,
                                                   CountryName = p.CountryName,
                                                   PostalCode = p.PostalCode
                                               }).FirstOrDefault();
                        model.ShippingAddress = (shippingAddress != null) ? shippingAddress : new AddressModel();
                    }
                    string countryCode = model?.ShippingAddress?.CountryName;
                    string stateCode = model?.ShippingAddress?.StateCode;
                    if (string.IsNullOrEmpty(stateCode))
                        stateCode = GetStateCode(model?.ShippingAddress?.StateName);

                    ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose,new { countryCode = countryCode , stateCode = stateCode });

                    if (!string.IsNullOrEmpty(countryCode) && !string.IsNullOrEmpty(stateCode))
                        list = GetShippingByCountryAndStateCode(countryCode, stateCode, list);

                    if (!string.IsNullOrEmpty(model.ShippingAddress?.PostalCode))
                        list = GetShippingByZipCode(model.ShippingAddress.PostalCode, list);

                    //check shipping type and call that service to get the rates. Add the rates in the list.
                    if (Equals(model.ShippingAddress, null))
                        model.ShippingAddress = new AddressModel();

                    model.ShippingAddress.PostalCode = zipCode;
                    model.ShippingAddress.StateCode = stateCode;
                    model.ShippingAddress.CountryName = countryCode;
                    model.ShippingAddress.Address1 = string.IsNullOrEmpty(model.ShippingAddress.Address1) ? string.Empty : model.ShippingAddress.Address1;
                    model.ShippingAddress.CityName = string.IsNullOrEmpty(model.ShippingAddress.CityName) ? string.Empty : model.ShippingAddress.CityName;
                    List<string> expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing };
                    
                    ZnodeShoppingCart znodeShoppingCart = GetService<IZnodeShippingCartMap>().ToZnodeShippingShoppingCart(model, expands);

                    List<ShippingModel> upsList = GetUpsShippingTypeList(list);
                    List<ShippingModel> fedexList = GetFedExShippingTypeList(list);
                    list?.RemoveAll(r => (r.ShippingTypeName?.ToLower() == ZnodeConstant.UPS.ToLower() || (r.ShippingTypeName?.ToLower() == ZnodeConstant.FedEx.ToLower() && r.ShippingCode != ServiceType.FEDEX_GROUND.ToString())) && r.ShippingCode != ServiceType.FEDEX_FREIGHT_ECONOMY.ToString() && r.ShippingCode != ServiceType.FEDEX_FREIGHT_PRIORITY.ToString() && !_upsLTLCode.Contains(r.ShippingCode));
                    bool isCalculatePromotionForShippingEstimates = ZnodeWebstoreSettings.IsCalculatePromotionForShippingEstimate;

                    //Call the respective shipping classes to get the shipping rates.
                    foreach (ShippingModel item in list)
                    {
                        model.Shipping.ShippingId = item.ShippingId;
                        model.Shipping.ShippingName = item.ShippingCode;
                        model.Shipping.ShippingCountryCode = string.IsNullOrEmpty(countryCode) ? item.DestinationCountryCode : countryCode;

                        znodeShoppingCart.Shipping = ShippingMap.ToZnodeShipping(model.Shipping);
                        IZnodeShippingManager shippingManager = GetService<IZnodeShippingManager>(new ZnodeTypedParameter(typeof(Znode.Libraries.ECommerce.Entities.ZnodeShoppingCart), znodeShoppingCart));
                        shippingManager.Calculate();

                        // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
                        if (isCalculatePromotionForShippingEstimates && znodeShoppingCart.IsCalculatePromotionAndCoupon)
                            CalculatePromotionForShippingEstimate(znodeShoppingCart);

                        //If the shipping amount less than shipping promotion discount then only apply add handling charges in shipping rate.
                        item.ShippingRate = (znodeShoppingCart.ShippingCost >= znodeShoppingCart?.Shipping?.ShippingDiscount) ?
                                              znodeShoppingCart.ShippingCost + znodeShoppingCart.Shipping.ShippingHandlingCharge -  znodeShoppingCart?.Shipping?.ShippingDiscount
                                            : znodeShoppingCart.Shipping.ShippingHandlingCharge;

                        if (item.ShippingRate >= 0 && znodeShoppingCart?.Shipping?.ShippingDiscount > 0)
                            item.ShippingRateWithoutDiscount = znodeShoppingCart.ShippingCost + znodeShoppingCart.Shipping.ShippingHandlingCharge;

                        item.ApproximateArrival = znodeShoppingCart.ApproximateArrival;

                        item.HandlingCharge = znodeShoppingCart.ShippingHandlingCharges;
                        if (Equals(znodeShoppingCart?.Shipping?.ResponseCode, "0"))
                            listwithRates.Add(item);
                    }

                    //UPS shipping type is exclude from above loop execution to avoid ups api call in loop.
                    //Below ups execution consolidated calculated shipping rates and 'EstimateDate' of ups shipping type on page load.
                    //It also removes not applicable ups shipping types which above loop shows with high shipping values.
                    //As below execution call ups api, it also calculate shipping value for ups and also discounted shipping value .
                    //Need to implemente same for fedex and usps shipping types for which API are already present.
                    GetListWithRates( listwithRates, upsList, model, znodeShoppingCart);
                    
                    //FedEx shipping type is exclude from above loop execution to avoid ups api call in loop.
                    GetListWithRates( listwithRates, fedexList, model, znodeShoppingCart);              
                    return new ShippingListModel { ShippingList = listwithRates.OrderBy(o => o.DisplayOrder).ToList() };
                }
                else
                    return new ShippingListModel { ShippingList = new List<ShippingModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorShippingOptionGet, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex);
                return new ShippingListModel { ShippingList = listwithRates?.Count() > 0 ? listwithRates.OrderBy(o => o.DisplayOrder).ToList() : new List<ShippingModel>() };
            }
        }

        /// <summary>
        /// Get the shipping list which is not filtered on the basis of country, state or zip code.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual List<ShippingModel> GetShippingList(ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int? profileId = 0;
            int userId = 0;
            if (!Equals(model, null))
            {
                profileId = IsNull(model?.ProfileId) ? GetProfileId() : model?.ProfileId.Value;
                userId = model.UserId.HasValue ? model.UserId.GetValueOrDefault() : GetLoginUserId();
            }
            userId = userId.Equals(0) ? -1 : userId;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, profileId = profileId });
            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.One));

            string publishStatus = Enum.GetName(typeof(ZnodePublishStatesEnum), model.PublishStateId > 0 ? model.PublishStateId : PublishStateId);
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { publishStatus = publishStatus });

            if (publishStatus == ZnodePublishStatesEnum.PRODUCTION.ToString())
                filters.Add(new FilterTuple(ZnodeConstant.PublishState, FilterOperators.In, publishStatus));

            PageListModel pageListModel = new PageListModel(filters, null, null);
            ZnodeLogging.LogMessage("WhereClause:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", model?.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT, @ProfileId,@PortalId, @UserId", 4, out pageListModel.TotalRowCount)?.ToList();
        }

        //if any item from cart is out of stock so it return total no of item which is "out stock item".
        public virtual int IsItemOutOfStock(ShoppingCartItemModel shoppingCartItem, List<string> skus, decimal selectedQuantity, List<InventorySKUModel> inventoryList, int insufficientQuantity, PublishedProductEntityModel products)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string minimumQuantity = "";
            string maximumQuantity = "";
            if (!string.Equals(shoppingCartItem.ProductType, ZnodeConstant.BundleProduct, StringComparison.OrdinalIgnoreCase))
            {
                selectedQuantity = shoppingCartItem.Quantity;
                minimumQuantity = products.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues;
                maximumQuantity = products.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.AttributeValues;
            }
            
            //Get inventory setting for product.
            List<PublishedSelectValuesEntityModel> inventorySettingList = products.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.SelectValues;
            List<string> sku = skus.Where(m => m == products.SKU).ToList();

            string inventorySetting = inventorySettingList?.Count > 0 ? inventorySettingList.FirstOrDefault().Code : ZnodeConstant.DontTrackInventory;
            ZnodeLogging.LogMessage("Parameter :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { minimumQuantity = minimumQuantity, maximumQuantity = maximumQuantity, InventorySetting = inventorySetting });

            switch (inventorySetting)
            {
                case ZnodeConstant.DisablePurchasing:
                    shoppingCartItem.InsufficientQuantity = IsInsufficientQuantity(sku, selectedQuantity, inventoryList);
                    if (shoppingCartItem.InsufficientQuantity)
                        insufficientQuantity++;
                    break;

                case ZnodeConstant.AllowBackOrdering:
                    shoppingCartItem.InsufficientQuantity = false;
                    break;

                case ZnodeConstant.DontTrackInventory:
                    shoppingCartItem.InsufficientQuantity = false;
                    break;

                default:
                    //Between true if want to include min and max number in comparison.
                    shoppingCartItem.InsufficientQuantity = string.IsNullOrEmpty(minimumQuantity) ? false : !HelperUtility.Between(Convert.ToDecimal(shoppingCartItem.Quantity), Convert.ToDecimal(minimumQuantity), Convert.ToDecimal(maximumQuantity), true);
                    if (shoppingCartItem.InsufficientQuantity)
                        insufficientQuantity++;
                    break;
            }
            ZnodeLogging.LogMessage("Insufficient Quantity:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, insufficientQuantity);
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return insufficientQuantity;
        }

        //Check item has Insufficient Quantity or not.
        protected virtual bool IsInsufficientQuantity(List<string> skus, decimal selectedQuantity, List<InventorySKUModel> inventoryList)
        {
            if (HelperUtility.IsNotNull(skus) && HelperUtility.IsNotNull(inventoryList))
            {
                foreach (InventorySKUModel inventoryItem in inventoryList)
                {
                    foreach (var item in skus)
                    {
                        if (string.Equals(item, inventoryItem.SKU, StringComparison.OrdinalIgnoreCase) && inventoryItem.Quantity < selectedQuantity)
                            return true;
                    }
                }
                return false;
            }
            return true;
        }


        #endregion

        #region protected virtual Method    

        protected virtual void MergeShoppingCartItems(ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ShoppingCartItemModel> shoppingCartItems = GetShoppingCartItems(cartModel);

            List<ShoppingCartItemModel> parentShoppingCartItems = shoppingCartItems?.Where(x => HelperUtility.IsNull(x.OrderLineItemRelationshipTypeId))?.ToList();
            List<ShoppingCartItemModel> childShoppingCartItems = shoppingCartItems?.Where(x => HelperUtility.IsNotNull(x.OrderLineItemRelationshipTypeId))?.ToList();
            ZnodeLogging.LogMessage("List details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { parentShoppingCartItems = parentShoppingCartItems?.Count(), childShoppingCartItems = childShoppingCartItems?.Count() });
            if (parentShoppingCartItems?.Count > 0 && childShoppingCartItems?.Count > 0)
            {
                List<ShoppingCartItemModel> childCartItems = new List<ShoppingCartItemModel>();

                foreach (ShoppingCartItemModel parentItem in parentShoppingCartItems)
                {
                    List<ShoppingCartItemModel> childItems = childShoppingCartItems.Where(x => x.ParentOmsSavedcartLineItemId == parentItem.OmsSavedcartLineItemId && x.OrderLineItemRelationshipTypeId != 1).ToList();

                    if (childItems?.Count > 0)
                    {
                        foreach (var item in childItems)
                        {
                            BindChildLineItem(shoppingCartItems, orderHelper, parentItem, item);
                        }
                        childCartItems.InsertRange((childCartItems.Count > 0 ? childCartItems.Count - 1 : 0), childItems);
                    }
                    else
                    {
                        parentItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedValueCartLineItem(parentItem.OmsSavedcartLineItemId.GetValueOrDefault());
                        childCartItems.Add(parentItem);
                    }
                }
                ZnodeLogging.LogMessage("List details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { childCartItems = childCartItems?.Count() });

                parentShoppingCartItems = childCartItems;
            }
            else
            {
                parentShoppingCartItems?.ForEach(x =>
                {
                    x.PersonaliseValuesDetail = orderHelper.GetPersonalizedValueCartLineItem(x.OmsSavedcartLineItemId.GetValueOrDefault());
                });
            }
            cartModel.ShoppingCartItems.InsertRange(0, parentShoppingCartItems.OrderBy(x => x.Sequence));
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        public virtual void BindChildLineItem(List<ShoppingCartItemModel> shoppingCartItems, IZnodeOrderHelper orderHelper, ShoppingCartItemModel parentItem, ShoppingCartItemModel item)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            GetAddOnSKU(shoppingCartItems, item);

            item.ConfigurableProductSKUs = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable) ? item.SKU : null;
            item.BundleProductSKUs = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles) ? item.SKU : null;
            item.GroupProducts = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                                ? new List<AssociatedProductModel> { new AssociatedProductModel { Sku = item.SKU, Quantity = item.Quantity } } : null;
            item.SKU = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns) ? item.SKU : parentItem.SKU;
            item.PersonaliseValuesDetail = orderHelper.GetPersonalizedValueCartLineItem(item.OmsSavedcartLineItemId.GetValueOrDefault());
            item.AssociatedAddOnProducts = GetAddOnsValueCartLineItem(shoppingCartItems, item.OmsSavedcartLineItemId);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get the list of add-ons for cart line item on the basis of savedCartLineItemId.
        private List<AssociatedProductModel> GetAddOnsValueCartLineItem(List<ShoppingCartItemModel> childShoppingCartItems, int? savedCartLineItemId)
        {
            List<AssociatedProductModel> list = new List<AssociatedProductModel>();
            var lineItem = childShoppingCartItems.Where(y => y.ParentOmsSavedcartLineItemId == savedCartLineItemId && y.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns)).ToList();
            foreach (var item in lineItem)
                list.Add(new AssociatedProductModel { Sku = item.SKU, Quantity = item.Quantity, OrderLineItemRelationshipTypeId = Convert.ToInt32(item.OrderLineItemRelationshipTypeId) });
            ZnodeLogging.LogMessage("List of AddOn:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list);
            return list;

        }

        private void GetAddOnSKU(List<ShoppingCartItemModel> shoppingCartItems, ShoppingCartItemModel item)
        {
            item.AddOnProductSKUs = shoppingCartItems.Where(m => m.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                && m.ParentOmsSavedcartLineItemId == item.ParentOmsSavedcartLineItemId).Select(m => m.SKU).FirstOrDefault();

            if (string.IsNullOrEmpty(item.AddOnProductSKUs))
                item.AddOnProductSKUs = shoppingCartItems.Where(m => m.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                 && m.ParentOmsSavedcartLineItemId == item.OmsSavedcartLineItemId).Select(m => m.SKU).FirstOrDefault();
        }

        public List<ShoppingCartItemModel> GetShoppingCartItems(ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return (from savedCartLineItem in _savedCartLineItemService.Table
                    join savedCart in _omsSavedRepository.Table on savedCartLineItem.OmsSavedCartId equals savedCart.OmsSavedCartId
                    join cookieMapping in _cookieMappingRepository.Table on savedCart.OmsCookieMappingId equals cookieMapping.OmsCookieMappingId
                    where cookieMapping.UserId == cartModel.UserId && cookieMapping.PortalId == cartModel.PortalId
                    select savedCartLineItem)?.ToModel<ShoppingCartItemModel>()?.ToList();
        }

        //Remove duplicate coupons with same name but different cases.
        protected virtual void RemoveDuplicateCoupons(List<CouponModel> Coupons)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<String> AppliedCoupons = new List<String>();

            Coupons?.Where(x => x.CouponApplied).ToList().ForEach(x => { AppliedCoupons.Add(x.Code); });
            if (AppliedCoupons.Count > 0)
            {
                foreach (string appliedCouponCode in AppliedCoupons)
                    Coupons?.RemoveAll(x => string.Equals(x.Code, appliedCouponCode, StringComparison.InvariantCultureIgnoreCase) && !x.CouponApplied);
            }
            ZnodeLogging.LogMessage("Coupons details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, Coupons);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        protected virtual void SetAllowTerritories(ZnodeShoppingCart znodeShoppingCart, AddressModel billingAddress)
        {
            foreach (ZnodeShoppingCartItem item in znodeShoppingCart.ShoppingCartItems)
            {
                if (HelperUtility.IsNotNull(billingAddress))
                {
                    item.IsAllowedTerritories = !string.IsNullOrEmpty(item?.Product?.AllowedTerritories) && !string.IsNullOrEmpty(znodeShoppingCart.Shipping.ShippingCountryCode) ? item.Product.AllowedTerritories.Split(',').ToList().Contains(znodeShoppingCart.Shipping.ShippingCountryCode) && item.Product.AllowedTerritories.Split(',').ToList().Contains(billingAddress?.CountryName) : true;

                    if (item.Product?.ZNodeGroupProductCollection?.Count > 0)
                        AllowedTerritoriesForGroupConfigureAddonsProduct(znodeShoppingCart, item, billingAddress);

                    if (item.Product?.ZNodeConfigurableProductCollection?.Count > 0)
                        AllowedTerritoriesForGroupConfigureAddonsProduct(znodeShoppingCart, item, billingAddress);

                    if (item.Product?.ZNodeAddonsProductCollection?.Count > 0)
                        AllowedTerritoriesForGroupConfigureAddonsProduct(znodeShoppingCart, item, billingAddress);
                }

            }
        }

        protected virtual void AllowedTerritoriesForGroupConfigureAddonsProduct(ZnodeShoppingCart shoppingCart, ZnodeShoppingCartItem item, AddressModel billingAddress)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string shippingCountryCode = shoppingCart.Shipping.ShippingCountryCode;
            ZnodeLogging.LogMessage("Shipping country code:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shippingCountryCode);
            if (HelperUtility.IsNotNull(billingAddress))
            {
                foreach (Znode.Libraries.ECommerce.Entities.ZnodeProductBaseEntity productBaseEntity in item.Product.ZNodeGroupProductCollection)
                    item.IsAllowedTerritories = !string.IsNullOrEmpty(productBaseEntity.AllowedTerritories) && !string.IsNullOrEmpty(shippingCountryCode) ? productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(shippingCountryCode) && productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(billingAddress.CountryName) : true;

                foreach (Znode.Libraries.ECommerce.Entities.ZnodeProductBaseEntity productBaseEntity in item.Product.ZNodeAddonsProductCollection)
                    item.IsAllowedTerritories = !string.IsNullOrEmpty(productBaseEntity.AllowedTerritories) && !string.IsNullOrEmpty(shippingCountryCode) ? productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(shippingCountryCode) && productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(billingAddress.CountryName) : true;

                foreach (Znode.Libraries.ECommerce.Entities.ZnodeProductBaseEntity productBaseEntity in item.Product.ZNodeConfigurableProductCollection)
                    item.IsAllowedTerritories = !string.IsNullOrEmpty(productBaseEntity.AllowedTerritories) && !string.IsNullOrEmpty(shippingCountryCode) ? productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(shippingCountryCode) && productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(billingAddress.CountryName) : true;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

        }

        //Remove personalized attribute 
        protected virtual bool RemovePersonalizedAttribute(int saveCartId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("saveCartId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, saveCartId);
            bool result = false;
            //Get Saved personalized attribute 
            List<int> OmsSavedCartLineItemId = GetOmsSavedCartLineItemId(saveCartId);
            ZnodeLogging.LogMessage("OmsSavedCartLineItemId list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, OmsSavedCartLineItemId?.Count());
            if (OmsSavedCartLineItemId?.Count > 0)
            {
                IZnodeRepository<ZnodeOmsPersonalizeCartItem> _personalizeCartItem = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsPersonalizeCartItemEnum.OmsSavedCartLineItemId.ToString(), ProcedureFilterOperators.In, string.Join(",", OmsSavedCartLineItemId)));

                //Delete all Personalized attribute list if exists 
                result = _personalizeCartItem.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return result;
        }

        //To check product inventory
        public virtual void CheckCartlineItemInventory(ShoppingCartItemModel shoppingCartItem, int portalId, CartParameterModel cartParameterModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, portalId);
            List<string> skus = new List<string>();
            string allSku;
            //Get all associated skus of items.
            GetAllItemsSku(shoppingCartItem, skus, out allSku);
            decimal selectedQuantity = shoppingCartItem.Quantity;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { selectedQuantity = selectedQuantity, skus = skus });
            //Get the inventory list of skus.
            List<InventorySKUModel> inventoryList = publishProductHelper.GetInventoryBySKUs(skus, portalId);
            ZnodeLogging.LogMessage("inventoryList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, inventoryList?.Count());
   
            List<PublishedProductEntityModel> inventorySetting = publishedProductData.GetPublishProductBySKUs(skus, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId, GetCatalogVersionId())?.ToModel<PublishedProductEntityModel>()?.ToList();
            //Get insufficient Quantity Count of current cart item.
            int insufficientQuantityCount = GetCartInsufficientQuantityCount(shoppingCartItem, skus, ref selectedQuantity, inventoryList, inventorySetting);
            ZnodeLogging.LogMessage("Insufficient quantity count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, insufficientQuantityCount);
            //if insufficient Quantity Count is greater then 0 then current cart is out of stock.
            if (insufficientQuantityCount > 0)
                shoppingCartItem.InsufficientQuantity = true;
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //To check product inventory 
        public virtual void CheckBaglineItemInventory(ShoppingCartModel shoppingCartModel, CartParameterModel cartParameterModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ShoppingCartItemModel> shoppingCartItemList = HelperUtility.IsNotNull(shoppingCartModel) ? shoppingCartModel.ShoppingCartItems : new List<ShoppingCartItemModel>();
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { shoppingCartItemListCount = shoppingCartItemList });

            List<string> productSkus = new List<string>();
            List<string> itemListSkus = new List<string>();
            //Get all associated skus of items.
            GetAllItemsSku(shoppingCartItemList, productSkus, out itemListSkus);
            List<InventorySKUModel> inventoryList = itemListSkus.Count > 0 ? publishProductHelper.GetInventoryBySKUs(itemListSkus, shoppingCartModel.PortalId) : new List<InventorySKUModel>();
            List<PublishedProductEntityModel> inventoryProducts = publishedProductData.GetPublishProductBySKUs(itemListSkus, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId, GetCatalogVersionId(cartParameterModel.PublishedCatalogId))?.ToModel<PublishedProductEntityModel>()?.ToList();

            shoppingCartItemList.ForEach(shoppingCartItem =>
            {
                string allSku = string.Empty;
                List<string> skus = new List<string>();
                GetAllItemsSku(shoppingCartItem, productSkus, out allSku);
                skus.AddRange(productSkus);
                skus = skus?.Distinct().ToList();

            //Get inventory setting details from publish products.
                List<PublishedProductEntityModel> inventorySetting = inventoryProducts.Where(x => allSku.Split(',').Contains(x.SKU))?.ToList();
                decimal selectedQuantity = new decimal();

            //Get insufficient Quantity Count of current cart item.
            int insufficientQuantityCount = GetCartInsufficientQuantityCount(shoppingCartItem, skus, ref selectedQuantity, inventoryList, inventorySetting);

                if (IsNotNull(inventoryList) && inventoryList.Count > 0)
                {
                    shoppingCartItem.QuantityOnHand = SetQuantityOnHand(shoppingCartItem, inventoryList);
                }

                //if insufficient Quantity Count is greater then 0 then current cart is out of stock.
                if (insufficientQuantityCount > 0)
                    shoppingCartItem.InsufficientQuantity = true;
            });
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }        

        public virtual IZnodeCheckout GetCartAndMapToCheckout(UserAddressModel user, ShoppingCartModel shoppingCart)
        {
            int? portalId = shoppingCart.PortalId;
            var znodeCheckout = GetService<IZnodeCheckout>();
            ZnodeShoppingCart znodeCart = (ZnodeShoppingCart)GetService<IZnodeShoppingCart>();

            znodeCart = GetPortalCartToMapCheckoutData(znodeCart, znodeCheckout, shoppingCart);
            znodeCheckout.UserAccount = user;
            znodeCheckout.ShoppingCart = znodeCart.PortalCarts.FirstOrDefault();

            if (IsNotNull(znodeCheckout.ShoppingCart))
            {
                znodeCheckout.ShoppingCart.UserAddress = user;
                znodeCheckout.ShoppingCart.PortalId = portalId;
                znodeCheckout.ShoppingCart.PortalID = portalId.GetValueOrDefault();
                znodeCheckout.ShoppingCart.Token = shoppingCart.Token;
                znodeCheckout.ShoppingCart.OrderId = shoppingCart.OmsOrderId;
                znodeCheckout.ShoppingCart.ReturnItemList = shoppingCart.ReturnItemList;
                znodeCheckout.ShoppingCart.OrderDate = shoppingCart.OrderDate;
                znodeCheckout.ShoppingCart.IsAllowWithOtherPromotionsAndCoupons = shoppingCart.IsAllowWithOtherPromotionsAndCoupons;
                znodeCheckout.ShoppingCart.PublishStateId = shoppingCart.PublishStateId;
                znodeCheckout.ShoppingCart.ProfileId = shoppingCart.ProfileId;
                znodeCheckout.ShoppingCart.Discount = shoppingCart.Discount;
                znodeCheckout.ShoppingCart.CSRDiscountAmount = shoppingCart.CSRDiscountAmount;
                znodeCheckout.ShoppingCart.TaxCost = shoppingCart.TaxCost;
                znodeCheckout.ShoppingCart.ImportDuty = shoppingCart.ImportDuty.GetValueOrDefault();
                znodeCheckout.ShoppingCart.TaxRate = shoppingCart.TaxRate;
                znodeCheckout.ShoppingCart.ShippingCost = shoppingCart.ShippingCost;
                znodeCheckout.ShoppingCart.ShippingHandlingCharges = shoppingCart.ShippingHandlingCharges;
                znodeCheckout.ShoppingCart.OrderLevelDiscountDetails = shoppingCart.OrderLevelDiscountDetails;
                znodeCheckout.ShoppingCart.CookieMappingId = string.IsNullOrEmpty(shoppingCart.CookieMappingId) ? 0 : Convert.ToInt32(new ZnodeEncryption().DecryptData(shoppingCart.CookieMappingId));

                znodeCheckout.ShoppingCart.IsPendingOrderRequest = shoppingCart.IsPendingOrderRequest;
                znodeCheckout.ShoppingCart.AvataxIsSellerImporterOfRecord = shoppingCart.AvataxIsSellerImporterOfRecord;

                var personaliseValuesList = shoppingCart.ShoppingCartItems.Count > 0 ? shoppingCart?.ShoppingCartItems?[0].PersonaliseValuesList : new System.Collections.Generic.Dictionary<string, object>();

                if (IsNotNull(personaliseValuesList))
                    znodeCheckout.ShoppingCart.PersonaliseValuesList = personaliseValuesList;
            }

            return znodeCheckout;
        }
        
        //Get insufficient Quantity Count of current cart item.
        private int GetCartInsufficientQuantityCount(ShoppingCartItemModel shoppingCartItem, List<string> skus, ref decimal selectedQuantity, List<InventorySKUModel> inventoryList, List<PublishedProductEntityModel> inventorySetting)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int insufficientQuantityCount = 0;
            foreach (var products in inventorySetting)
            {

                //Check for group product inventory.
                if (shoppingCartItem?.GroupProducts?.Count > 0)
                {

                    foreach (AssociatedProductModel associatedProduct in shoppingCartItem.GroupProducts)
                    {
                        if (string.Equals(associatedProduct.Sku, products.SKU))
                        {
                            selectedQuantity = associatedProduct.Quantity;

                            //Get insufficient Quantity Count of current item.
                            insufficientQuantityCount = IsItemOutOfStock(shoppingCartItem, skus, selectedQuantity, inventoryList, insufficientQuantityCount, products);
                        }
                    }
                }
                //Check for Configurable product inventory.
                else if (!string.IsNullOrEmpty(shoppingCartItem?.ConfigurableProductSKUs))
                {
                    List<InventorySKUModel> inventoryListForConfigProd = inventoryList.Where(w => w.SKU == shoppingCartItem.ConfigurableProductSKUs).ToList();
                    if (string.Equals(products.SKU, shoppingCartItem.ConfigurableProductSKUs))
                    {
                        insufficientQuantityCount = IsItemOutOfStock(shoppingCartItem, skus, selectedQuantity, inventoryListForConfigProd, insufficientQuantityCount, products);
                    }
                }
                //Check for Bundle product inventory.
                else if (!string.IsNullOrEmpty(shoppingCartItem?.BundleProductSKUs))
                {
                    //Check for inventory of a child product only.
                    if (string.Equals(products?.SKU, shoppingCartItem.SKU))
                        break;
                    AssociatedPublishedBundleProductModel publishModel = shoppingCartItem?.BundleProducts?.FirstOrDefault(x => x.SKU == products?.SKU);
                    selectedQuantity = publishModel != null ? publishModel.Quantity.Value : 0;
                    if(selectedQuantity > 0.0M)
                    {
                        List<InventorySKUModel> inventoryListForBundleProd = inventoryList.Where(w => w.SKU == products?.SKU).ToList();
                        insufficientQuantityCount = IsItemOutOfStock(shoppingCartItem, skus, selectedQuantity, inventoryListForBundleProd, insufficientQuantityCount, products);

                    }
                    if (shoppingCartItem.InsufficientQuantity)
                        break;
                }
                else if (shoppingCartItem?.AssociatedAddOnProducts?.Count > 0)
                {
                    foreach (string AddonSku in skus)
                    {
                        if (string.Equals(products.SKU, AddonSku))
                        {
                            List<InventorySKUModel> inventoryListForAddOnProd = inventoryList.Where(w => w.SKU == AddonSku).ToList();
                            insufficientQuantityCount = IsItemOutOfStock(shoppingCartItem, skus, selectedQuantity, inventoryListForAddOnProd, insufficientQuantityCount, products);
                        }
                    }
                }
                //Check for Simple product inventory.
                //Get insufficient Quantity Count of current item.
                if (string.Equals(products.SKU, shoppingCartItem.SKU))
                {
                    insufficientQuantityCount = IsItemOutOfStock(shoppingCartItem, skus, selectedQuantity, inventoryList, insufficientQuantityCount, products);
                }
            }
            ZnodeLogging.LogMessage("Insufficient quantity count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, insufficientQuantityCount);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return insufficientQuantityCount;
        }

        //Get all skus of current shopping cart.
        protected virtual void GetAllItemsSku(ShoppingCartItemModel shoppingCartItem, List<string> skus, out string allSku)
        {
            //Add sku in sku List.
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);

            bool isSimpleProduct = true;

            //Set skus of Bundle Product.
            if (!string.IsNullOrEmpty(shoppingCartItem.BundleProductSKUs))
                SetBundleProductSKUs(shoppingCartItem, skus, out isSimpleProduct);
            //Set skus of Configurable Product.
            else if (!string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs))
                SetConfigurableProductSKUs(shoppingCartItem, skus, out isSimpleProduct);
            //Set Skus of Group Products.
            else if (HelperUtility.IsNotNull(shoppingCartItem.GroupProducts) && shoppingCartItem.GroupProducts.Count > 0)
                SetGroupProductsSkus(shoppingCartItem, skus, out isSimpleProduct);

            //Set skus of AddOn Product.
            if (!string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs))
                SetAddOnProductSKUs(shoppingCartItem, skus, out isSimpleProduct);

            allSku = string.Join(",", skus);
            //Set skus of simple Product.
            if (isSimpleProduct)
            {
                allSku = string.IsNullOrWhiteSpace(allSku) ? shoppingCartItem.SKU : $"{allSku},{ shoppingCartItem.SKU }";
                skus.Add(shoppingCartItem.SKU);
            }
            skus = skus.Distinct().ToList();


        }

        //Get all skus of current shopping cart.
        protected virtual void GetAllItemsSku(List<ShoppingCartItemModel> shoppingCartItemList, List<string> skus, out List<string> allSkus)
        {
            //Add sku in sku List.

            shoppingCartItemList.ForEach(shoppingCartItem =>
            {
                bool isSimpleProduct = true;

            //Set skus of Bundle Product.
            if (!string.IsNullOrEmpty(shoppingCartItem.BundleProductSKUs))
            {
                    SetBundleProductSKUs(shoppingCartItem, skus, out isSimpleProduct);
                    skus.Remove(shoppingCartItem.SKU);
            }
            
            //Set skus of Configurable Product.
            else if (!string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs))
                    SetConfigurableProductSKUs(shoppingCartItem, skus, out isSimpleProduct);
            //Set Skus of Group Products.
            else if (HelperUtility.IsNotNull(shoppingCartItem.GroupProducts) && shoppingCartItem.GroupProducts.Count > 0)
                    SetGroupProductsSkus(shoppingCartItem, skus, out isSimpleProduct);

            //Set skus of AddOn Product.
            if (!string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs))
                    SetAddOnProductSKUs(shoppingCartItem, skus, out isSimpleProduct);


                //Set skus of simple Product.
                if (isSimpleProduct)
                {
                    skus.Add(shoppingCartItem.SKU);
                }
                skus = skus.Distinct().ToList();
            });

            allSkus = skus;
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);

        }

        //Set Skus of Group Products.
        protected virtual void SetGroupProductsSkus(ShoppingCartItemModel shoppingCartItem, List<string> skus, out bool isSimpleProduct)
        {
            isSimpleProduct = false;
            skus.AddRange(shoppingCartItem.GroupProducts.Select(x => x.Sku)?.ToList());
            string.Join(",", shoppingCartItem.GroupProducts.Select(x => x.Quantity));
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);
        }

        //Set skus of Configurable Product.
        protected virtual void SetConfigurableProductSKUs(ShoppingCartItemModel shoppingCartItem, List<string> skus, out bool isSimpleProduct)
        {
            isSimpleProduct = false;
            skus.AddRange(shoppingCartItem.ConfigurableProductSKUs.Split(',')?.ToList());
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);

        }

        //Set skus of Bundle Product.
        protected virtual void SetBundleProductSKUs(ShoppingCartItemModel shoppingCartItem, List<string> skus, out bool isSimpleProduct)
        {
            isSimpleProduct = false;
            skus.AddRange(shoppingCartItem.BundleProductSKUs.Split(',')?.ToList());
            skus.Add(shoppingCartItem.SKU);
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);

        }

        //Set skus of AddOn Product.
        protected virtual void SetAddOnProductSKUs(ShoppingCartItemModel shoppingCartItem, List<string> skus, out bool isSimpleProduct)
        {
            ZnodeLogging.LogMessage("List of skus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, skus);
            isSimpleProduct = true;
            skus.AddRange(shoppingCartItem.AddOnProductSKUs.Split(',')?.ToList());
        }

        //Bind cookieMappingId, PortalId, LocaleId, CatalogId, UserId.
        protected virtual void BindCartData(ShoppingCartModel shoppingCartModel, CartParameterModel cartParameterModel)
        {
            shoppingCartModel.CookieMappingId = cartParameterModel.CookieMappingId;
            shoppingCartModel.PortalId = cartParameterModel.PortalId;
            shoppingCartModel.UserId = cartParameterModel.UserId.GetValueOrDefault();
            shoppingCartModel.PublishedCatalogId = cartParameterModel.PublishedCatalogId;
            shoppingCartModel.LocaleId = cartParameterModel.LocaleId;
            shoppingCartModel.OmsOrderId = cartParameterModel.OmsOrderId;
            shoppingCartModel.ShippingId = cartParameterModel.ShippingId.GetValueOrDefault();
            //Get the active currency code of portal.
            CurrencyModel currencyModel = HelperUtility.IsNotNull(shoppingCartModel.OmsOrderId) && shoppingCartModel.OmsOrderId > 0 ?
                                     GetOrderCurrency(shoppingCartModel.OmsOrderId) :
                                     GetPortalCurrency(shoppingCartModel.PortalId);
            shoppingCartModel.CurrencyCode = currencyModel?.CurrencyCode;
            shoppingCartModel.CurrencySuffix = currencyModel?.Symbol;
            shoppingCartModel.CultureCode = currencyModel?.CultureCode;
            shoppingCartModel.Custom1 = cartParameterModel.Custom1;
            shoppingCartModel.Custom2 = cartParameterModel.Custom2;
            shoppingCartModel.Custom3 = cartParameterModel.Custom3;
            shoppingCartModel.Custom4 = cartParameterModel.Custom4;
            shoppingCartModel.Custom5 = cartParameterModel.Custom5;
            shoppingCartModel.IsOldOrder = cartParameterModel.IsOldOrder;
            shoppingCartModel.ShoppingCartItems.Select(x => { x.CultureCode = currencyModel?.CultureCode; x.CurrencyCode = currencyModel?.CurrencyCode; return x; })?.ToList();
        }

        // Get saved carts by  
        protected virtual int GetOrderSavedCartId(int userId, int cookieMappingId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, cookieMappingId = cookieMappingId });
            if (cookieMappingId < 1)
                //Get cookie mappings by user id.
                cookieMappingId = Convert.ToInt32(_cookieMappingRepository.Table.FirstOrDefault(x => x.UserId == userId)?.OmsCookieMappingId);

            if (cookieMappingId > 0)
                //Get save cart items by cookie mapping id.
                return Convert.ToInt32(_omsSavedRepository.Table.FirstOrDefault(x => x.OmsCookieMappingId == cookieMappingId)?.OmsSavedCartId);
            return 0;
        }
        //Get Personalized attributes by saved cart id
        protected virtual List<int> GetOmsSavedCartLineItemId(int omsSavedCartId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsSavedCartId = omsSavedCartId });

            if (omsSavedCartId > 0)
            {
                return _savedCartLineItemService.Table.Where(x => x.OmsSavedCartId == omsSavedCartId).Select(x => x.OmsSavedCartLineItemId).ToList();
            }
            return new List<int>();
        }
        //To get ShoppingCart by orderId
        public virtual ShoppingCartModel GetCartByOrderId(CartParameterModel cartParameterModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel shoppingCartModel = GetService<IZnodeShoppingCart>().LoadCartFromOrder(cartParameterModel, GetCatalogVersionId(cartParameterModel.PublishedCatalogId));
            shoppingCartModel.IsAllowWithOtherPromotionsAndCoupons = DefaultGlobalConfigSettingHelper.IsAllowWithOtherPromotionsAndCoupons;
            //Bind cookieMappingId, PortalId, LocaleId, CatalogId, UserId.
            BindCartData(shoppingCartModel, cartParameterModel);

            if (shoppingCartModel?.ShoppingCartItems?.Count > 0)
                //Bind image path to product image.
                BindImagePath(shoppingCartModel);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCartModel;
        }

        protected virtual void BindImagePath(ShoppingCartModel shoppingCartModel)
        {
            if (!Equals(shoppingCartModel, null) && !Equals(shoppingCartModel.ShoppingCartItems, null) && shoppingCartModel.ShoppingCartItems.Count > 0)
            {
                IImageHelper objImage = GetService<IImageHelper>();
                //Concatenate image path and product image.
                foreach (ShoppingCartItemModel shoppingItem in shoppingCartModel.ShoppingCartItems)
                    shoppingItem.ImagePath = _shoppingCartItemMap.GetImagePath(shoppingItem.ImagePath, shoppingCartModel.PortalId, objImage);
            }
        }

        //to create ShoppingCart without using saved cart line item this function is used while editing order in that case we dont save/update cart line data ZnodeOmsSavedCartLineItem
        protected virtual ShoppingCartModel CreateCartFromShoppingCartModel(ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            CartParameterModel cartParameterModel = new CartParameterModel
            {
                LocaleId = model.LocaleId,
                PortalId = model.PortalId,
                PublishedCatalogId = model.PublishedCatalogId,
                UserId = model.UserId,
                OmsOrderId = model.OmsOrderId,
                ProfileId = HelperUtility.IsNotNull(model.ProfileId) ? model.ProfileId.GetValueOrDefault() : GetProfileId()
            };
            List<string> expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing };
            ZnodeShoppingCart znodeShoppingCart = _shoppingCartMap.ToZnodeShoppingCart(model, null, expands);
           
            ShoppingCartModel shoppingCartModel = _shoppingCartMap.ToModel(znodeShoppingCart, GetService<IImageHelper>());

            //Bind cookieMappingId, PortalId, LocaleId, CatalogId, UserId.
            BindCartData(shoppingCartModel, cartParameterModel);
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return shoppingCartModel;

        }

        //Set Portal currency details.
        protected virtual CurrencyModel GetPortalCurrency(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId });
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            IZnodeRepository<ZnodeCulture> _cultureRepository = new ZnodeRepository<ZnodeCulture>();
            IZnodeRepository<ZnodeCurrency> _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            //Get Portal currency data.            
            CurrencyModel currencyModel = (from asl in _portalUnitRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(PortalFilter(portalId).ToFilterDataCollection()).WhereClause)
                                           join portalCulture in _cultureRepository.GetEntityList(string.Empty) on asl.CultureId equals portalCulture.CultureId
                                           join portalCurrency in _currencyRepository.GetEntityList(string.Empty) on asl.CurrencyId equals portalCurrency.CurrencyId
                                           select new CurrencyModel()
                                           {
                                               CurrencyCode = portalCurrency.CurrencyCode,
                                               CultureCode = portalCulture.CultureCode,
                                               Symbol = portalCulture.Symbol
                                           }
                                   )?.FirstOrDefault();
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return currencyModel;
        }

        //Get Order currency data.
        protected virtual CurrencyModel GetOrderCurrency(int? orderId)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId });
            IZnodeRepository<ZnodeCulture> _cultureRepository = new ZnodeRepository<ZnodeCulture>();
            IZnodeRepository<ZnodeCurrency> _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            return (from orderDetails in _orderDetailRepository.Table
                    join portalCulture in _cultureRepository.Table
                    on orderDetails.CultureCode equals portalCulture.CultureCode
                    join portalCurrency in _currencyRepository.Table on portalCulture.CurrencyId equals portalCurrency.CurrencyId
                    where ((orderDetails.OmsOrderId == orderId) && (orderDetails.IsActive))
                    select new CurrencyModel()
                    {
                        CurrencyCode = portalCurrency.CurrencyCode,
                        CultureCode = portalCulture.CultureCode,
                        Symbol = portalCulture.Symbol
                    }
                                   )?.FirstOrDefault();
        }

        //Generate filters for Portal Id.
        private static FilterCollection PortalFilter(int portalId)
             => new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };

        //Set shipping address in payment model.
        protected virtual void SetShippingAddressToPayment(ShoppingCartModel shoppingCart)
        {

            if (IsNotNull(shoppingCart))
            {
                if (IsNotNull(shoppingCart.Payment))
                    shoppingCart.Payment.ShippingAddress = shoppingCart.ShippingAddress;
                else
                    shoppingCart.Payment = new PaymentModel() { ShippingAddress = shoppingCart.ShippingAddress };
            }
        }

        //to set order over due amount
        protected virtual void SetOrderOverDueAmount(ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(shoppingCartModel.OmsOrderId) && shoppingCartModel.OmsOrderId > 0)
            {
                ZnodeOmsOrderDetail order = _orderDetailRepository.Table.FirstOrDefault(x => x.OmsOrderId == shoppingCartModel.OmsOrderId && x.IsActive);
                shoppingCartModel.OverDueAmount = (shoppingCartModel.Total - order?.Total ?? 0);

                //To handle the penny calculation as due to fraction the amount changes slightly
                if (shoppingCartModel?.OverDueAmount > 0)
                {
                    string overDueAmount = Convert.ToString(shoppingCartModel.OverDueAmount);
                    int overDueAmountSplitLength = ZnodeApiSettings.OrderOverDueAmountRoundOffLength;
                    overDueAmount = !string.IsNullOrEmpty(overDueAmount) ? overDueAmount?.Substring(0, overDueAmount.Length > overDueAmountSplitLength ? overDueAmountSplitLength : overDueAmount.Length) : "0";
                    decimal splittedOverDueAmount = Convert.ToDecimal(overDueAmount);

                    if (splittedOverDueAmount == 0)
                    {
                        shoppingCartModel.OverDueAmount = splittedOverDueAmount;
                    }
                }
                ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
        }

        // Get state code by stateName
        protected virtual string GetStateCode(string stateName, string countryCode)
        {

            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { stateName = stateName, countryCode = countryCode });
            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     && state.CountryCode == countryCode
                                                     select state.StateCode).FirstOrDefault());
                ZnodeLogging.LogMessage("Parameter :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { stateCode = stateCode });

                return stateCode ?? string.Empty;
            }
            ZnodeLogging.LogMessage("Output from GetStateCode method:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, stateName);
            return stateName ?? string.Empty;
        }

        //to delete auto addon product associated with parent product
        protected virtual void RemoveAssociatedAutoAddon(ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(shoppingCart.RemoveAutoAddonSKU) && shoppingCart.IsParentAutoAddonRemoved)
            {
                RemoveParentProductWithAutoAddon(shoppingCart);
            }
            else if (!string.IsNullOrEmpty(shoppingCart.RemoveAutoAddonSKU) && !shoppingCart.IsParentAutoAddonRemoved)
            {
                RemoveChildAddonFromParentProduct(shoppingCart);
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to get cookie mapping Id from shoppingcart
        protected virtual int GetCookieMappingIdFromShoppingCart(ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int cookieId = !string.IsNullOrEmpty(shoppingCart.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(shoppingCart.CookieMappingId)) : 0;
            //Get CookieMappingId
            int cookieMappingId = cookieId == 0 ? orderHelper.GetCookieMappingId(shoppingCart.UserId, shoppingCart.PortalId) : cookieId;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { cookieId = cookieId, cookieMappingId = cookieMappingId });

            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return cookieMappingId;
        }

        //to remove parent product with all auto addon associated with main product
        protected virtual void RemoveParentProductWithAutoAddon(ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string[] autoAddSKUS = shoppingCart.RemoveAutoAddonSKU.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ZnodeLogging.LogMessage("autoAddSKUS list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, autoAddSKUS?.Count());
            if (autoAddSKUS.Length > 0)
            {
                shoppingCart.ShoppingCartItems.RemoveAll(x => autoAddSKUS.Contains(x.AutoAddonSKUs));
                if (shoppingCart?.ShoppingCartItems?.Count == 0)
                {
                    //after removing main product if shopping cart count becomes zero then remove saved cart line item
                    RemoveSavedCartItems(shoppingCart.UserId, GetCookieMappingIdFromShoppingCart(shoppingCart), shoppingCart.PortalId);
                }
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to remove child product addon from parent/main product
        protected virtual void RemoveChildAddonFromParentProduct(ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string[] autoAddSKUS = shoppingCart.RemoveAutoAddonSKU.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ZnodeLogging.LogMessage("autoAddSKUS list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, autoAddSKUS?.Count());

            foreach (string sku in autoAddSKUS)
            {
                foreach (ShoppingCartItemModel item in shoppingCart.ShoppingCartItems)
                {
                    if (!string.IsNullOrEmpty(item.AutoAddonSKUs))
                    {
                        item.AutoAddonSKUs = RemoveSKUFromAutoAddon(sku, item.AutoAddonSKUs);
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to remove sku from auto addon
        protected virtual string RemoveSKUFromAutoAddon(string sku, string autoAddon)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(sku) && !string.IsNullOrEmpty(autoAddon))
            {
                string[] autoAddSKUS = autoAddon.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                autoAddSKUS = autoAddSKUS.Where(val => val != sku).ToArray();
                ZnodeLogging.LogMessage("autoAddSKUS list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, autoAddSKUS?.Count());

                return string.Join(",", autoAddSKUS);
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return autoAddon;
        }

        //to bind shopping cart model to znodeShoppingCart
        protected virtual void BindCartModel(ShoppingCartModel cartModel, ZnodeShoppingCart znodeShoppingCart)
        {
            if (HelperUtility.IsNotNull(cartModel))
            {
                znodeShoppingCart.CustomTaxCost = cartModel.CustomTaxCost;
                znodeShoppingCart.CustomShippingCost = cartModel.CustomShippingCost;
                znodeShoppingCart.Custom1 = cartModel.Custom1;
                znodeShoppingCart.Custom2 = cartModel.Custom2;
                znodeShoppingCart.Custom3 = cartModel.Custom3;
                znodeShoppingCart.Custom4 = cartModel.Custom4;
                znodeShoppingCart.Custom5 = cartModel.Custom5;
            }
        }

        protected virtual List<ShippingModel> GetShippingByCountryAndStateCode(string countryCode, string stateCode, List<ShippingModel> shippinglist)
        {
            return shippinglist?.Where(shipping => (string.Equals(shipping.DestinationCountryCode, countryCode, StringComparison.CurrentCultureIgnoreCase) || shipping.DestinationCountryCode == null || string.Equals(shipping.ShippingCode, ZnodeConstant.FreeShipping, StringComparison.CurrentCultureIgnoreCase))
              && (string.Equals(shipping.StateCode, stateCode, StringComparison.CurrentCultureIgnoreCase) || shipping.StateCode == null))?.ToList() ?? new List<ShippingModel>();
        }

        protected virtual string GetStateCode(string stateName)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     select state.StateCode).FirstOrDefault());
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { stateName = stateName, stateCode = stateCode });

                return stateCode ?? string.Empty;
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return stateName ?? string.Empty;
        }

        protected virtual bool IsValidShippingZipCode(string userZipcode, string shippingOptionZipcode, ShippingModel shipping, List<ShippingModel> filteredShippingList)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            bool result = false;
            //add shipping Option zipcode having "*"
            if (shippingOptionZipcode.Contains("*"))
            {
                string shippingZipCode = shippingOptionZipcode.Replace("*", string.Empty).Trim();
                //shipping Option Zipcode start with the user zipcode then allow to add
                if (userZipcode.Trim().StartsWith(shippingZipCode))
                {
                    filteredShippingList.Add(shipping);
                    result = true;
                }
            }
            //add if shipping Option Zipcode is same as user zipcode then allow to add
            else if (string.Equals(shippingOptionZipcode.Trim(), userZipcode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                filteredShippingList.Add(shipping);
                result = true;
            }
            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return result;
        }

        protected virtual List<ShippingModel> GetShippingByZipCode(string zipcode, List<ShippingModel> shippinglist)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { zipcode = zipcode });

            if (shippinglist?.Count > 0)
            {
                List<ShippingModel> filteredShippingList = new List<ShippingModel>();
                //to check each shipping option have zipcode entered by user
                foreach (ShippingModel shipping in shippinglist)
                {
                    //if shipping option zipcode is null or "*" then allow for all zipcode entered by user 
                    if (string.IsNullOrEmpty(shipping.ZipCode) || shipping.ZipCode.Trim() == "*")
                        filteredShippingList.Add(shipping);
                    else
                    {
                        //if shipping option zipcode contains "," then it will have more than one zipcode allows
                        if (shipping.ZipCode.Contains(","))
                        {
                            string[] allZipCodesAssignToShipping = shipping.ZipCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            //to check each zipcode that entered against shipping  option comma separate
                            foreach (string shippingZipCode in allZipCodesAssignToShipping)
                            {
                                //to check zipcode for each shipping 
                                if (IsValidShippingZipCode(zipcode, shippingZipCode, shipping, filteredShippingList))
                                    break;
                            }
                        }
                        else
                            IsValidShippingZipCode(zipcode, shipping.ZipCode, shipping, filteredShippingList);
                    }
                }
                return filteredShippingList;
            }

            ZnodeLogging.LogMessage("Execution Done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return shippinglist;
        }


        /// <summary>
        /// Get OmsLineItem Detail by omsOrderId
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>List of OmsLineItems</returns>
        public virtual OrderLineItemDataListModel GetOrderLineItemDetails(int omsOrderId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("OmsOrderId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, omsOrderId);
            IZnodeRepository<ZnodeOmsOrderLineItem> _OmsOrderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();

            return new OrderLineItemDataListModel
            {
                OrderLineItemDetails = (from orderDetail in _orderDetailRepository.Table
                                        join orderLineItem in _OmsOrderLineItemRepository.Table on orderDetail.OmsOrderDetailsId equals orderLineItem.OmsOrderDetailsId
                                        where orderDetail.OmsOrderId == omsOrderId
                                        select new OrderLineItemDataModel
                                        {
                                            Sku = orderLineItem.Sku,
                                            OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId,
                                            ParentOmsOrderLineItemsId = orderLineItem.ParentOmsOrderLineItemsId,
                                            Quantity = orderLineItem.Quantity

                                        }).ToList()
            };
        }
        /// <summary>
        /// Merge Cart after login
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual bool MergeGuestUsersCart(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int status = 0;
            //Get parameters from filters
            int userId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, "UserId", StringComparison.CurrentCultureIgnoreCase))?.Item3);
            int portalId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, "PortalId", StringComparison.CurrentCultureIgnoreCase))?.Item3);

            //Get old Saved Cart Id
            int oldSavedCartId = GetOldSavedCartId(filters);

            //Get new Saved Cart Id
            int newSavedCartId = GetSavedCartId(userId, portalId);
            ZnodeLogging.LogMessage("UserId,PortalId,OldSavedCartId,NewSavedCartId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { userId, portalId, oldSavedCartId, newSavedCartId });

            if (oldSavedCartId == newSavedCartId)
                return true;

            IZnodeViewRepository<CartParameterModel> objStoredProc = new ZnodeViewRepository<CartParameterModel>();

            //SP parameters
            objStoredProc.SetParameter("@OmsSavedCartId", newSavedCartId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OldOmsSavedCartId", oldSavedCartId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.ExecuteStoredProcedureList("Znode_MergeOmsSavedCartLineItems @OmsSavedCartId,@OldOmsSavedCartId,@UserId,@status OUT", 3, out status);
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status == 1;
        }

        // Validate all the SKUs 
        public virtual void ValidateSKUDetails(AddToCartModel shoppingCart)
        {

            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(shoppingCart) || IsNull(shoppingCart.ShoppingCartItems) || shoppingCart.ShoppingCartItems.Any(item => string.IsNullOrEmpty(item.SKU)))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            int portalId = shoppingCart.PortalId > 0 ? shoppingCart.PortalId : GetPortalId();
            int publishedCatalogId = shoppingCart.PublishedCatalogId > 0 ? shoppingCart.PublishedCatalogId : GetPublishedCatalogId(portalId);


            List<string> associatedSKUs = GetAssociatedSKUs(shoppingCart);

            if (associatedSKUs?.Count > 0 && publishedCatalogId > 0)
            {
                int? catalogVersionId = GetCatalogVersionId(publishedCatalogId);
                int localeId = shoppingCart.LocaleId > 0 ? shoppingCart.LocaleId : GetDefaultLocaleId();

                List<PublishedProductEntityModel> products = publishedProductData.GetPublishProductBySKUs( associatedSKUs, publishedCatalogId, localeId, catalogVersionId)?.ToModel<PublishedProductEntityModel>()?.ToList();

                List<string> publishedSKUs = products?.Select(x => x.SKU)?.Distinct()?.ToList();

                //Validate the entered SKUs  
                if (associatedSKUs.Except(publishedSKUs).Any())
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidSku);
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

        }

        //Get the list of SKUs
        public virtual List<string> GetAssociatedSKUs(AddToCartModel shoppingCart)
        {

            List<string> SKUs = shoppingCart?.ShoppingCartItems?
                .SelectMany(x => new string[] { x.SKU, x.AddOnProductSKUs, x.BundleProductSKUs, x.ConfigurableProductSKUs, String.Join(",", x?.GroupProducts?.Select(y => y.Sku)?.ToList() ?? new List<String>()) })?
                .Where(x => !string.IsNullOrEmpty(x))?.Distinct()?.ToList();

            List<string> associatedSKUs = new List<string>();
            SKUs?.ForEach(x => { associatedSKUs.AddRange(x.Split(',')); });

            return associatedSKUs?.Distinct()?.ToList();
        }

        protected virtual ZnodeShoppingCart GetPortalCartToMapCheckoutData(ZnodeShoppingCart znodeCart, IZnodeCheckout znodeCheckout, ShoppingCartModel shoppingCart)
        {
            List<string> skus = new List<string>();

            foreach (ShoppingCartItemModel item in shoppingCart.ShoppingCartItems)
            {
                if (!string.IsNullOrEmpty(item.ConfigurableProductSKUs))
                {
                    skus.Add(item.ConfigurableProductSKUs.ToLower());
                }
                else if (item?.GroupProducts?.Count > 0)
                {
                    skus.Add(item.GroupProducts[0]?.Sku.ToLower());
                }
                skus.Add(item.SKU.ToLower());
            }
            skus = skus.Distinct().ToList();

            List<PublishedConfigurableProductEntityModel> configEntities;
            List<string> expands = new List<string> { ZnodeConstant.Pricing };
            int catalogVersionId = GetCatalogVersionId(shoppingCart.PublishedCatalogId, shoppingCart.LocaleId) ?? 0;
            bool includeInactiveProduct = false;

            //If order status is cancelled then allow inactive products for cancellation.
            if (string.Equals(shoppingCart.OrderStatus, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                includeInactiveProduct = true;

            znodeCart.OrderLevelDiscountDetails = shoppingCart.OrderLevelDiscountDetails;
           
            List<PublishProductModel> cartLineItemsProductData = publishProductHelper.GetDataForCartLineItems(skus, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId, expands, shoppingCart.UserId.GetValueOrDefault(), shoppingCart.PortalId, catalogVersionId, out configEntities, includeInactiveProduct, shoppingCart.OmsOrderId.GetValueOrDefault());

            SetOrdersDiscountInLineItem(cartLineItemsProductData, shoppingCart.ShoppingCartItems);

            List<TaxClassRuleModel> lstTaxClassSKUs = publishProductHelper.GetTaxRules(skus);
            List<ZnodePimDownloadableProduct> lstDownloadableProducts = new ZnodeRepository<ZnodePimDownloadableProduct>().Table.Where(x => skus.Contains(x.SKU)).ToList();

         
            znodeCart.AddtoShoppingBagV2(shoppingCart, cartLineItemsProductData, catalogVersionId, lstTaxClassSKUs, lstDownloadableProducts, configEntities, expands);
   
            return znodeCart;
        }

        //To set order discount details for cart line items.
        protected virtual void SetOrdersDiscountInLineItem(List<PublishProductModel> cartLineItemsProductData, List<ShoppingCartItemModel> shoppingCartItems)
        {
            foreach (ShoppingCartItemModel item in shoppingCartItems)
            {
                if (IsNotNull(item.Product) && IsNotNull(cartLineItemsProductData.FirstOrDefault(x => x.PublishProductId == item.Product.PublishProductId)))
                    MapDetailsInLineItem(cartLineItemsProductData.FirstOrDefault(x => x.PublishProductId == item.Product.PublishProductId), item);
            }
        }

        //To map cart line item details in publish product model.
        protected virtual void MapDetailsInLineItem(PublishProductModel publishproduct, ShoppingCartItemModel cartLineItemDetails)
        {
            try
            {
                publishproduct.OrdersDiscount = cartLineItemDetails.Product.OrdersDiscount;
                publishproduct.PST = cartLineItemDetails.Product.PST;
                publishproduct.GST = cartLineItemDetails.Product.GST;
                publishproduct.VAT = cartLineItemDetails.Product.VAT;
                publishproduct.HST = cartLineItemDetails.Product.HST;
                publishproduct.DiscountAmount = cartLineItemDetails.Product.DiscountAmount;
                publishproduct.SalesTax = cartLineItemDetails.Product.SalesTax > 0 ? cartLineItemDetails.Product.SalesTax : cartLineItemDetails.TaxCost;
                publishproduct.ShippingCost = cartLineItemDetails.Product.ShippingCost;
                publishproduct.ImportDuty = cartLineItemDetails.Product.ImportDuty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapDetailsInLineItem method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        private int GetOldSavedCartId(FilterCollection filters)
        {
            int omsSavedCartLineItemId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, "OmsSavedCartLineItemId", StringComparison.CurrentCultureIgnoreCase))?.Item3);

            int oldSavedCartId = Convert.ToInt32(_savedCartLineItemService?.Table?.Where(x => x.OmsSavedCartLineItemId == omsSavedCartLineItemId)?.FirstOrDefault()?.OmsSavedCartId);
            ZnodeLogging.LogMessage("OmsSavedCartLineItemId,OldSavedCartId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { omsSavedCartLineItemId, oldSavedCartId });
            return oldSavedCartId;
        }

        private int GetSavedCartId(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new { userId = userId, portalId = portalId });

            int cookieMappingId = orderHelper.GetCookieMappingId(userId, portalId);
            int newSavedCartId = orderHelper.GetSavedCartId(ref cookieMappingId);
            return newSavedCartId;
        }


        /// <summary>
        /// Map null data with requesting cart model
        /// </summary>
        /// <param name="cartModel">Requesting cart model.</param>
        /// <param name="shoppingCartModel">Cart model loaded from DB.</param>
        protected virtual void MapNullDataWithRequestingCartModel(ShoppingCartModel cartModel, ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(cartModel))
            {
                var cartModelProperties = (cartModel.GetType()).GetProperties();
                foreach (var propertyInfo in (shoppingCartModel.GetType()).GetProperties())
                {
                    if (HelperUtility.IsNull(propertyInfo.GetValue(shoppingCartModel))
                        || (propertyInfo.GetValue(shoppingCartModel)?.Equals(0) ?? false))
                    {
                        propertyInfo.SetValue(shoppingCartModel, propertyInfo.GetValue(cartModel));
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Delete All the saved cart line items.
        private bool DeleteSavedCartItems(int userId, int cookieMappingId,int portalId)
        {
            try
            {

                int status = 0;
                IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
                objStoredProc.SetParameter("@OmsCookieMappingId", cookieMappingId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteSavedCartItem @OmsCookieMappingId,@UserId,@PortalId,@Status OUT", 3, out status);
                return status == 1;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in DeleteSavedCartItems method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
        private void CalculatePromotionForShippingEstimate(ZnodeShoppingCart znodeShoppingCart)
        {
            ZnodeLogging.LogMessage("Execution Started:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", znodeShoppingCart), new ZnodeNamedParameter("profileId", znodeShoppingCart.ProfileId));

            if (cartPromoManager.CartPromotionCache.Any(x => x.PromotionType.ClassName == ZnodeConstant.PercentOffShipping || x.PromotionType.ClassName == ZnodeConstant.PercentOffShippingWithCarrier || x.PromotionType.ClassName == ZnodeConstant.AmountOffShipping || x.PromotionType.ClassName == ZnodeConstant.AmountOffShippingWithCarrier))
                cartPromoManager.Calculate();
            ZnodeLogging.LogMessage("Execution Ended:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

        }

        //To get ShoppingCart by quoteId
        public virtual ShoppingCartModel LoadCartForQuote(CartParameterModel cartParameterModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(cartParameterModel))
            {
                ShoppingCartModel shoppingCartModel = GetService<IZnodeShoppingCart>().LoadCartFromQuote(cartParameterModel, GetCatalogVersionId(cartParameterModel.PublishedCatalogId));

                if (shoppingCartModel?.ShoppingCartItems?.Count > 0)
                {
                    //Bind image path to product image.
                    BindImagePath(shoppingCartModel);
                    //Bind Culturecode to lineitem.
                    shoppingCartModel.ShoppingCartItems.Select(x => { x.CultureCode = shoppingCartModel?.CultureCode; return x; })?.ToList();
                }
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return shoppingCartModel;
            }
            return null;            
            
        }
        #endregion

        #region Private Methods
        //Set the Publish Catalog Id
        private void SetPublishCatalogId(CartParameterModel cartParameterModel)
        {
            if(cartParameterModel.PublishedCatalogId == 0 && cartParameterModel.PortalId > 0)
            {
                int userAccountId = _userRepository.Table.FirstOrDefault(o => o.UserId == cartParameterModel.UserId).AccountId.GetValueOrDefault();
                if (userAccountId != 0)
                {
                    int accountCatalogId = GetAccountCatalogId(userAccountId);
                    cartParameterModel.PublishedCatalogId = accountCatalogId == 0 ? GetPublishedCatalogId(cartParameterModel.PortalId) : accountCatalogId;
                }
                else
                {
                    cartParameterModel.PublishedCatalogId = GetPublishedCatalogId(cartParameterModel.PortalId);
                }
            }
        }

        //Get the Publish Catalog Id on the basis of PortalId
        private int GetPublishedCatalogId(int portalId)
        {
            return portalId > 0 ? _portalCatalogRepository.Table.FirstOrDefault(o => o.PortalId == portalId).PublishCatalogId : 0;
        }

        //Get Catalog Id on the basis of UserAccount
        private int GetAccountCatalogId(int userAccountId)
        {
            return _accountRepository.Table.FirstOrDefault(o => o.AccountId == userAccountId).PublishCatalogId.GetValueOrDefault();
        }

        //Get the OmsSaveCartLineItemId 
        private int SetOmsSaveCartLineItemId(List<int?> parentIds, int omsSavedCartLineItemId, List<ZnodeOmsSavedCartLineItem> savedCartLineItems)
        {
            //Retrieving child line item for bundle product
            if (IsNotNull(parentIds) && parentIds.Contains(omsSavedCartLineItemId))
            {
                return savedCartLineItems.FirstOrDefault(e => e.ParentOmsSavedCartLineItemId != null).OmsSavedCartLineItemId;
            }
            else
            {
                return omsSavedCartLineItemId;
            }
        }

        //Getting Save Cart line Items 
        private IQueryable<ZnodeOmsSavedCartLineItem> GetCartLineItems(int omsSavedCartLineItemId, int childCount, List<int?> parentIds)
        {
            if (IsNotNull(parentIds) && parentIds.Contains(omsSavedCartLineItemId))
            {
                return _savedCartLineItemService.Table.Where(o => o.ParentOmsSavedCartLineItemId == omsSavedCartLineItemId || o.OmsSavedCartLineItemId == omsSavedCartLineItemId);
            }
            else
            {
                //Get line items to delete, if there is only one child then remove parent line as well otherwise do not delete the parent.
                return childCount > 1 ? _savedCartLineItemService.Table.Where(o => o.OmsSavedCartLineItemId == omsSavedCartLineItemId || o.ParentOmsSavedCartLineItemId == omsSavedCartLineItemId)
                                           : _savedCartLineItemService.Table.Where(o => o.OmsSavedCartLineItemId == omsSavedCartLineItemId || o.ParentOmsSavedCartLineItemId == omsSavedCartLineItemId || (parentIds.Contains(o.OmsSavedCartLineItemId)));

            }
        }

        //Set Quantity On Hand (Available Inventory)
        protected virtual decimal SetQuantityOnHand(ShoppingCartItemModel shoppingCartItem, List<InventorySKUModel> inventoryList)
        {
            decimal? quantityOnHand = 0;
            //If shopping cart dont have any item return 0.
            if (IsNull(shoppingCartItem))
            {
                return quantityOnHand.Value;
            }
            //Add Product Inventory check needs to be introduced.
            if (!string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs))
            {
                shoppingCartItem.QuantityOnHand = 0;
            }
            //If product is an configurable product.
            if (!string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs))
            {
                quantityOnHand = SetQuantityOnHandForConfigurableProduct(shoppingCartItem, inventoryList);
            }
            //If product is an group product.
            else if (IsNotNull(shoppingCartItem.GroupProducts) && shoppingCartItem.GroupProducts.Count > 0)
            {
                quantityOnHand = SetQuantityOnHandForGroupProduct(shoppingCartItem, inventoryList);
            }
            //If product is an simple/bundle product.
            else
            {
                quantityOnHand = SetQuantityOnHandForSimpleAndBundleProduct(shoppingCartItem, inventoryList);
            }

            return IsNotNull(quantityOnHand) ? quantityOnHand.Value : 0;
        }

        //Set quantity on hand for configurable product.
        private decimal? SetQuantityOnHandForConfigurableProduct(ShoppingCartItemModel shoppingCartItem, List<InventorySKUModel> inventoryList)
            => inventoryList.FirstOrDefault(x => x.SKU.Equals(shoppingCartItem.ConfigurableProductSKUs))?.Quantity;

        //Set quantity on hand for group product.
        private decimal? SetQuantityOnHandForGroupProduct(ShoppingCartItemModel shoppingCartItem, List<InventorySKUModel> inventoryList)
            => inventoryList.FirstOrDefault(x => x.SKU.Equals(shoppingCartItem.GroupProducts.FirstOrDefault().Sku))?.Quantity;

        //Set quantity on hand for simple and bundle product.
        private decimal? SetQuantityOnHandForSimpleAndBundleProduct(ShoppingCartItemModel shoppingCartItem, List<InventorySKUModel> inventoryList)
            => inventoryList.FirstOrDefault(x => x.SKU.Equals(shoppingCartItem.SKU))?.Quantity;

        protected virtual List<ShippingModel> GetListWithRates(List<ShippingModel> listwithRates, List<ShippingModel> shippingTypeList, ShoppingCartModel model, ZnodeShoppingCart znodeShoppingCart)
        {
            try
            { 
                if (shippingTypeList.Count > 0)
                {
                    ZnodeShippingManager manager = null;
                    ZnodeGenericCollection<IZnodeShippingsType> shippingTypes = new ZnodeGenericCollection<IZnodeShippingsType>();
                    List<ZnodeShippingBag> shippingbagList = new List<ZnodeShippingBag>();
                    foreach (ShippingModel item in shippingTypeList)
                    {
                        znodeShoppingCart.Shipping.ShippingID = item.ShippingId;
                        znodeShoppingCart.Shipping.ShippingName = item.ShippingCode;
                        model.Shipping.ShippingId = item.ShippingId;
                        model.Shipping.ShippingName = item.ShippingCode;
                        model.Shipping.ShippingCountryCode = string.IsNullOrEmpty(model?.ShippingAddress?.CountryName) ? item.DestinationCountryCode : model?.ShippingAddress?.CountryName;
                        znodeShoppingCart.Shipping = ShippingMap.ToZnodeShipping(model.Shipping);
                        manager = new ZnodeShippingManager(znodeShoppingCart, true, shippingTypes, shippingbagList);
                    }

                    List<ShippingModel> ratelist = manager.GetShippingEstimateRate(znodeShoppingCart, model, model?.ShippingAddress?.CountryName, shippingbagList);
                    ZnodeLogging.LogMessage("Rate list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, ratelist?.Count());
                    shippingTypeList = shippingTypeList.Join(ratelist, r => r.ShippingCode, p => p.ShippingCode, (oldShoppingModel, newShoppingModel) => oldShoppingModel).ToList();

                    shippingTypeList?.ForEach(f =>
                    {
                        ShippingModel shippingModel = ratelist?.Where(w => w.ShippingCode == f.ShippingCode && !Equals(w.ShippingRate, 0))?.FirstOrDefault();

                        if (HelperUtility.IsNotNull(shippingModel) && shippingModel?.ShippingRate >= 0)
                        {
                            f.EstimateDate = shippingModel.EstimateDate;
                            f.ShippingRate = znodeShoppingCart.CustomShippingCost ?? shippingModel.ShippingRate;
                            f.ShippingRateWithoutDiscount = shippingModel?.ShippingRateWithoutDiscount > 0 ? shippingModel?.ShippingRateWithoutDiscount : 0;
                            f.HandlingCharge = shippingModel.ShippingHandlingCharge;
                        }
                    });
                    listwithRates.AddRange(shippingTypeList);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Api_Resources.ErrorShippingOptionWithRatesGet, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex);
            }
            return listwithRates;
        }

        //Get FedEx Shipping Type List.
        protected virtual List<ShippingModel> GetFedExShippingTypeList(List<ShippingModel> list)
           => list.Where(w => w.ShippingTypeName.ToLower() == ZnodeConstant.FedEx.ToLower() && w.ShippingCode != ServiceType.FEDEX_GROUND.ToString() && w.ShippingCode != ServiceType.FEDEX_FREIGHT_ECONOMY.ToString() && w.ShippingCode != ServiceType.FEDEX_FREIGHT_PRIORITY.ToString()).ToList();

        //Get UpsList Shipping Type List.
        protected virtual List<ShippingModel> GetUpsShippingTypeList(List<ShippingModel> list)
           => list.Where(w => w.ShippingTypeName.ToLower() == ZnodeConstant.UPS.ToLower()).ToList();

        #endregion
        }
    }
