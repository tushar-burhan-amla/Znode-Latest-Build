using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Services.Maps.V2;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using System.Diagnostics;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class ShoppingCartServiceV2 : ShoppingCartService, IShoppingCartServiceV2
    {
        #region Private Variables

        private readonly IPublishProductHelper _publishProductHelper;
        private readonly IOrderServiceV2 _orderService;
        private readonly IPublishProductServiceV2 _publishProductService;

        private readonly string MINIMUM_QUANTITY = "MinimumQuantity";
        private readonly string MAXIMUM_QUANTITY = "MaximumQuantity";

        #endregion

        #region Ctor

        public ShoppingCartServiceV2() : base()
        {
            _publishProductHelper = new PublishProductHelper();
            _publishProductService = new PublishProductServiceV2();
            _orderService = new OrderServiceV2();
        }

        #endregion

        #region Public Methods

        //To Create shopping cart.
        public virtual ShoppingCartModelV2 CreateCart(ShoppingCartModelV2 cartModel)
        {
            if (IsNull(cartModel))
                throw new ZnodeException(ErrorCodes.NullModel, "Shopping Cart Model can not be null.");

            ShoppingCartModel shoppingCart = new ShoppingCartModel();
            shoppingCart = cartModel.ToEntity<ShoppingCartModel>();
            if (shoppingCart.OmsOrderId > 0 && shoppingCart.ShoppingCartItems?.Count > 0)
                shoppingCart.ShoppingCartItems.ForEach(x => x.OmsOrderId = shoppingCart.OmsOrderId > 0 ? shoppingCart.OmsOrderId : null);

            CheckParentChildSkuForConfigurableProduct(cartModel);


            int shippingId = 0;
            string countryCode = string.Empty;
            if (IsNotNull(shoppingCart.Shipping))
            {
                shippingId = shoppingCart.Shipping.ShippingId < 1 ? shoppingCart.ShippingId : Convert.ToInt32(shoppingCart.Shipping.ShippingId);
                countryCode = string.IsNullOrEmpty(shoppingCart?.ShippingAddress?.CountryName) ? shoppingCart.Shipping.ShippingCountryCode : shoppingCart?.ShippingAddress?.CountryName;
            }

            //Set Shipping Address To shopping cart payment model.
            ZnodeLogging.LogMessage("ShippingAddress with Id to set in payment model: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = shoppingCart?.ShippingAddress?.AddressId });
            SetShippingAddressToPayment(shoppingCart);

            if (!string.IsNullOrEmpty(shoppingCart.RemoveAutoAddonSKU))
            {
                ZnodeLogging.LogMessage("RemoveAutoAddonSKU to remove associated auto addon: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, shoppingCart?.RemoveAutoAddonSKU);
                RemoveAssociatedAutoAddon(shoppingCart);
            }

            CheckPricingForCartItems(shoppingCart);

            //To save cart line items in savedcart table and get cookieMappingId
            int cookieMappingId = new ZNodeShoppingCartV2().SaveV2(shoppingCart);
            
            if (cookieMappingId > 0)
            {
                CartParameterModel cartParamModel = new CartParameterModel
                {
                    CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString()),
                    LocaleId = shoppingCart.LocaleId,
                    PortalId = shoppingCart.PortalId,
                    PublishedCatalogId = shoppingCart.PublishedCatalogId,
                    UserId = shoppingCart.UserId,
                    ShippingId = shippingId,
                    ShippingCountryCode = countryCode,
                    OmsOrderId = shoppingCart.OmsOrderId
                };

                var updatedCart = GetShoppingCartDetails(cartParamModel, shoppingCart);
                updatedCart.ProfileId = shoppingCart.ProfileId;
                ValidateShoppingCart(cartModel, updatedCart);
                return CalculateV2(updatedCart);
            }
            return null;
        }

        public virtual void CheckParentChildSkuForConfigurableProduct(ShoppingCartModelV2 cartModel)
        {
            foreach (var shoppingCartItems in cartModel.ShoppingCartItems)
            {

                IZnodeRepository<ZnodePimProductTypeAssociation> _pimProductTypeAssociation = new ZnodeRepository<ZnodePimProductTypeAssociation>();
                IZnodeRepository<View_LoadManageProductInternal> _view_LoadManageProductInternal = new ZnodeRepository<View_LoadManageProductInternal>();

                if (!string.IsNullOrEmpty(shoppingCartItems.ChildProductSKU) && !string.IsNullOrEmpty(shoppingCartItems.SKU))
                {

                    List<int?> pimProductId = (from pimProductTypeAssociation in _pimProductTypeAssociation.Table
                                               where _view_LoadManageProductInternal.Table.Where(x => x.AttributeCode == ZnodeConstant.ProductSKU && x.AttributeValue == shoppingCartItems.SKU).Any(f => f.PimProductId == pimProductTypeAssociation.PimParentProductId)
                                               select pimProductTypeAssociation.PimProductId).ToList();
                    string pimProductIds = string.Join(",", pimProductId);

                    if (pimProductId.Count <= 0) return;

                    FilterCollection filters = new FilterCollection();
                    filters.Add(new FilterTuple(View_PimProductAttributeValueEnum.PimProductId.ToString(), FilterOperators.In, pimProductIds));
                    filters.Add(new FilterTuple(ZnodeMediaAttributeEnum.AttributeCode.ToString(), FilterOperators.Is, ZnodeConstant.ProductSKU.ToString()));

                    EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                    string associatedChildProduct = string.Join(",", _view_LoadManageProductInternal.GetEntityList(whereClause.WhereClause, whereClause.FilterValues)?.Select(x => x.AttributeValue)?.ToList());
                    bool IsAssociatedChildProduct = associatedChildProduct.Contains(shoppingCartItems.ChildProductSKU);
                    if (!IsAssociatedChildProduct)
                        throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.InvalidDataForConfigurableProduct);
                    
                }

            }
        }

        //To remove cart item(s) based on comma separated list of SavedCartLineItemIds       
        public virtual ShoppingCartModelV2 RemoveSavedCartItems(RemoveCartItemModelV2 model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                bool status = false;

                ZnodeLogging.LogMessage("SavedCartLineItemIds to remove personalized attribute list and saved cart items: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model?.SavedCartLineItemIds);
                RemovePersonalizedAttribute(model.SavedCartLineItemIds);
                IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemRepository = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsSavedCartLineItemEnum.ParentOmsSavedCartLineItemId.ToString(), ProcedureFilterOperators.In, model.SavedCartLineItemIds));
                status = _savedCartLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

                filters.Clear();
                filters.Add(new FilterTuple(ZnodeOmsSavedCartLineItemEnum.OmsSavedCartLineItemId.ToString(), ProcedureFilterOperators.In, model.SavedCartLineItemIds));

                //Delete saved cart item.
                status = _savedCartLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessSavedCartLineItemDelete : Admin_Resources.ErrorSavedCartLineItemDelete);
                return CalculateV2(GetShoppingCartDetails(model.ToEntity<CartParameterModel>()));
            }
            catch (Exception)
            {
                ZnodeLogging.LogMessage(Admin_Resources.RemoveCart, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
                return null;
            }   
        }

        //Calculate the shopping cart (For direct API call)
        public virtual ShoppingCartModelV2 CalculateV2(ShoppingCartCalculateRequestModelV2 model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get shopping cart
            ShoppingCartModel _shoppingCart = GetShoppingCart(ShoppingCartMapV2.ToCartParameterModel(model));

            if (IsNull(_shoppingCart))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShoppingCartModelNotNull);

            if (_shoppingCart.ShoppingCartItems.Count.Equals(0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.NoItemPresentInShoppingCart);

            //Get User details
            _shoppingCart.UserDetails = (model.UserId == 0 && model.IsGuest) ? new UserModel() : _orderService.GetUserDetails(Convert.ToInt32(model.UserId));

            //Get Shipping details
            _shoppingCart.Shipping = _orderService.GetShipping(model.ShippingOptionId);

            //Get billing address
            _shoppingCart.BillingAddress = model.BillingAddressId == 0 ? model.BillingAddress?.ToEntity<AddressModel>() : _orderService.GetAddress(Convert.ToInt32(model.UserId), model.BillingAddressId, true);

            //Get Shipping Address
            _shoppingCart.ShippingAddress = model.ShippingAddressId == 0 ? model.ShippingAddress?.ToEntity<AddressModel>() : _orderService.GetAddress(Convert.ToInt32(model.UserId), model.ShippingAddressId, false);

            ZnodeLogging.LogMessage("UserDetails,Shipping, BillingAddress and ShippingAddress with Ids respectively: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { _shoppingCart?.UserDetails?.UserId,
                     _shoppingCart?.Shipping?.ShippingId, _shoppingCart?.BillingAddress?.AddressId, _shoppingCart?.ShippingAddress?.AddressId });

            //prepare shopping cart
            ShoppingCartMapV2.CreateOrderToShoppingCartModel(model, _shoppingCart);

            //Map payment details
            ShoppingCartMapV2.MapPayment(model, _shoppingCart);

            if (model.IsGuest || string.IsNullOrEmpty(_shoppingCart.UserDetails?.AspNetUserId))
                _shoppingCart.UserId = 0;

            return Calculate(_shoppingCart).ToEntity<ShoppingCartModelV2>();
        }

        //To Create shopping cart.
        public virtual ShoppingCartModelV2 GetShoppingCartV2(CartParameterModel model) => GetShoppingCart(model)?.ToEntity<ShoppingCartModelV2>();
        #endregion

        #region Protected Methods

        protected virtual void ValidateShoppingCart(ShoppingCartModelV2 cartModel, ShoppingCartModel shoppingCart)
        {
            if (IsNull(cartModel))
                throw new ZnodeException(ErrorCodes.NullModel, "Shopping Cart Model can not be null.");

            //shoppingCart = cartModel.ToEntity<ShoppingCartModel>();

            CartParameterModel cartParamModel = new CartParameterModel
            {
                LocaleId = shoppingCart.LocaleId,
                PortalId = shoppingCart.PortalId,
                PublishedCatalogId = shoppingCart.PublishedCatalogId,
                UserId = shoppingCart.UserId,
                OmsOrderId = shoppingCart.OmsOrderId
            };

            StringBuilder builder = new StringBuilder();
            //var products = GetPublishedProducts(shoppingCart);
            foreach (ShoppingCartItemModel item in shoppingCart.ShoppingCartItems)
            {
                if (item.Quantity < 1)
                {
                    builder.Append("Invalid product quantity. Quantity must be greater than zero.");
                    break;
                }

                CheckCartlineItemInventory(item, shoppingCart.PortalId, cartParamModel);

                if (item.InsufficientQuantity)
                {
                    builder.Append($"{item.SKU} product is not available.{Environment.NewLine}");
                    continue;
                }

                if (!string.IsNullOrEmpty(item.ConfigurableProductSKUs))
                {
                    ValidateConfigurableProducts(shoppingCart, builder, item);
                }

                if (item.Quantity > item.MaxQuantity|| item.Quantity < item.MinQuantity)
                {
                    builder.Append($"{item.SKU} product quantity should be between {item.MinQuantity} to {item.MaxQuantity}.{Environment.NewLine}");
                    continue;
                }
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                throw new ZnodeException(ErrorCodes.OutOfStockException, builder.ToString(), HttpStatusCode.BadRequest);
            }
        }
        
        protected void ValidateConfigurableProducts(ShoppingCartModel shoppingCart, StringBuilder builder, ShoppingCartItemModel item)
        {
            ParameterProductModel parameterModel = new ParameterProductModel
            {
                SKU = item.SKU,
                ParentProductSKU = item.SKU
            };

            NameValueCollection expands = new NameValueCollection {
                { ZnodeConstant.AssociatedProducts, ZnodeConstant.AssociatedProducts }
            };

            FilterCollection filters = new FilterCollection
            {
                { WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, shoppingCart.PublishedCatalogId.ToString() },
                { ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, shoppingCart.LocaleId.ToString() },
                { ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, shoppingCart.PortalId.ToString() },
                { WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue }
            };

            PublishProductModel publishProduct = _publishProductService.GetPublishProductBySkuV2(parameterModel, expands, filters);
            if (HelperUtility.IsNull(publishProduct))
            {
                builder.Append($"Invalid Product SKU: {item.SKU}{Environment.NewLine}");
                return;
            }

            var isExist = publishProduct.AssociatedProducts.Any(x => string.Equals(x.SKU, item.ConfigurableProductSKUs, StringComparison.InvariantCultureIgnoreCase));

            if (isExist)
            {
                //Check if the requested quantity is allowed in the Product setting.
                int minQuantity = Convert.ToInt32(publishProduct.Attributes.Where(x => x.AttributeCode == MINIMUM_QUANTITY).Select(y => y.AttributeValues).FirstOrDefault());
                int maxQuantity = Convert.ToInt32(publishProduct.Attributes.Where(x => x.AttributeCode == MAXIMUM_QUANTITY).Select(y => y.AttributeValues).FirstOrDefault());

                if (item.Quantity > maxQuantity || item.Quantity < minQuantity)
                {
                    builder.Append($"Requested quantity({item.Quantity}) {(item.Quantity < minQuantity ? "recedes" : "exceeds")} the permissible quantity({maxQuantity}) for SKU: {item.ConfigurableProductSKUs}.{Environment.NewLine}");
                }
            }
            else
            {
                builder.Append($"Invalid Child Product SKU ({item.ConfigurableProductSKUs}) for Product {item.SKU}{Environment.NewLine}");
            }
        }

        //Remove personalized attribute 
        protected bool RemovePersonalizedAttribute(string SavedCartLineItemIds)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(SavedCartLineItemIds))
            {
                IZnodeRepository<ZnodeOmsPersonalizeCartItem> _personalizeCartItem = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsPersonalizeCartItemEnum.OmsSavedCartLineItemId.ToString(), ProcedureFilterOperators.In, SavedCartLineItemIds));

                //Delete all Personalised attribute list if exists 
                result = _personalizeCartItem.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            }
            return result;
        }

        //Calculate cart using default Shopping cart model 
        protected ShoppingCartModelV2 CalculateV2(ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get shopping cart
            ShoppingCartModel _shoppingCart = model;

            if (IsNull(_shoppingCart))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShoppingCartModelNotNull);

            if (model.UserId > 0)
            {
                ZnodeLogging.LogMessage("UserId to get user details, billing and shipping address: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model?.UserId);
                //Get User details
                _shoppingCart.UserDetails = _orderService.GetUserDetails(Convert.ToInt32(model.UserId));

                //Get billing address
                _shoppingCart.BillingAddress = _orderService.GetAddress(Convert.ToInt32(model.UserId));

                //Get Shipping Address
                _shoppingCart.ShippingAddress = _orderService.GetAddress(Convert.ToInt32(model.UserId), false);
                ZnodeLogging.LogMessage("UserDetails, BillingAddress and ShippingAddress with Ids respectively: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { _shoppingCart?.UserDetails?.UserId,
                    _shoppingCart?.BillingAddress?.AddressId, _shoppingCart?.ShippingAddress?.AddressId });
            }

            //Get Shipping details
            ZnodeLogging.LogMessage("ShippingId to get Shipping: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model?.ShippingId);
            _shoppingCart.Shipping = _orderService.GetShipping(model.ShippingId) ?? new OrderShippingModel();

            //prepare shopping cart
            CreateOrderModelV2 createOrderModelV2 = model.ToEntity<CreateOrderModelV2>();
            ShoppingCartMapV2.CreateOrderToShoppingCartModel(createOrderModelV2, _shoppingCart);

            //Map payment details
            ShoppingCartMapV2.MapPayment(createOrderModelV2, _shoppingCart);

            return Calculate(_shoppingCart).ToEntity<ShoppingCartModelV2>();
        }

        //Check if the items which are being added to the cart have pricing associated to them.
        protected void CheckPricingForCartItems(ShoppingCartModel shoppingCart)
        {
            List<string> skusToAdd = shoppingCart.ShoppingCartItems
                                                 .Where(x => x.OmsSavedcartLineItemId == 0)
                                                 .Select(s => string.IsNullOrEmpty(s.ConfigurableProductSKUs) ? s.SKU : s.ConfigurableProductSKUs).ToList();

            if (skusToAdd.Count > 0)
            {
                string priceSKU = string.Join(",", _publishProductHelper.GetPricingBySKUs(skusToAdd, shoppingCart.PortalId, shoppingCart.UserId.GetValueOrDefault())
                    .Select(y => y.SKU).ToArray());

                shoppingCart.ShoppingCartItems.RemoveAll(x => !priceSKU.Contains(x.SKU) && !priceSKU.Contains(x.ConfigurableProductSKUs));
            }
        }
        #endregion
    }
}
