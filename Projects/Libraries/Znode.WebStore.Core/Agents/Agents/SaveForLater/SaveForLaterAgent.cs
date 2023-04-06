using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.WebStore.Core.Agents
{ 
    public class SaveForLaterAgent : BaseAgent, ISaveForLaterAgent
    {

        #region Protected Readonly Variables
        protected readonly IPublishProductClient _publishProductClient;
        protected readonly ISaveForLaterClient _saveForLaterClient;
        protected readonly IShoppingCartClient _shoppingCartsClient;
        protected readonly ISavedCartAgent _savedCartAgent;
        #endregion

        #region Public Constructor
        public SaveForLaterAgent(IPublishProductClient publishProductClient, ISaveForLaterClient saveForLaterClient, IShoppingCartClient shoppingCartClient)
        {
            _publishProductClient = GetClient<IPublishProductClient>(publishProductClient);
            _saveForLaterClient = GetClient<ISaveForLaterClient>(saveForLaterClient);
            _shoppingCartsClient = GetClient<IShoppingCartClient>(shoppingCartClient);
        }
        #endregion

        #region Public Methods

        //Create cart for later
        public virtual TemplateViewModel CreateCartForLater(string guiId)
        {
            try
            {
                //Get shopping cart model from the session
                List<ShoppingCartItemModel> shoppingListModel = new List<ShoppingCartItemModel>();

                ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
                ShoppingCartItemModel cartItem = cartModel.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == guiId);

                if (HelperUtility.IsNotNull(cartItem))
                {
                    shoppingListModel.Add(cartItem);
                }

                //Creating new AccountTemplate view model to bind the data
                AccountTemplateModel templateModel = new AccountTemplateModel();
                SetTemplateModelData(templateModel);

                //Set Template cart item model data
                templateModel.TemplateCartItems = shoppingListModel?.ToViewModel<TemplateCartItemModel>().ToList();

                if (!string.IsNullOrEmpty(cartItem?.BundleProductSKUs))
                    SetSelectedBundleProductsForAddToCart(cartItem, templateModel, cartItem.BundleProductSKUs);

                //Create Cart for later
                AccountTemplateModel accountTemplateModel = _saveForLaterClient.CreateSaveForLater(templateModel);

                return HelperUtility.IsNotNull(accountTemplateModel) ? accountTemplateModel.ToViewModel<TemplateViewModel>() : null;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        public virtual void SetSelectedBundleProductsForAddToCart(ShoppingCartItemModel cartItem, AccountTemplateModel templateModel, string bundleProductSKUs = null)
        {
            //Get skus and quantity of associated group products.
            string[] bundleProducts = !string.IsNullOrEmpty(bundleProductSKUs) ? bundleProductSKUs.Split(',') : !string.IsNullOrEmpty(cartItem.BundleProductSKUs) ? cartItem.BundleProductSKUs?.Split(',') : null;
   
            for (int index = 0; index < bundleProducts?.Length; index++)
            {
                bool isNewExtIdRequired = !Equals(index, 0);
                cartItem.BundleProductSKUs = bundleProducts[index];
                cartItem.Quantity = cartItem.Quantity;
                templateModel.TemplateCartItems.Add(cartItem.ToViewModel<TemplateCartItemModel>());
            }
            templateModel.TemplateCartItems.Remove(templateModel.TemplateCartItems.FirstOrDefault(a => a.ProductCode == cartItem?.ProductCode));
        }

        //Get the saved cart for later
        public virtual TemplateViewModel GetSavedCartForLater()
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                //Get user details from session
                UserViewModel userViewModel = GetUserViewModelFromSession();

                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());
                expands.Add(ExpandKeys.Pricing);

                FilterCollection filters = GetRequiredFilters();

                //Get the account template model.
                AccountTemplateModel accountTemplateModel = _saveForLaterClient.GetSaveForLaterTemplate(userViewModel.UserId, ZnodeConstant.SaveForLater, expands, filters);

                //Validate UserID.
                if (!HelperUtility.IsValidIdInQueryString(accountTemplateModel.CreatedBy, userViewModel.UserId))
                {
                    return (TemplateViewModel)GetViewModelWithErrorMessage(new TemplateViewModel(), WebStore_Resources.HttpCode_401_AccessDeniedMsg);
                }

                if (HelperUtility.IsNotNull(accountTemplateModel))
                {
                    //Maps the model to view model.
                    TemplateViewModel viewModel = accountTemplateModel.ToViewModel<TemplateViewModel>();
                    viewModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                    viewModel.CultureCode = PortalAgent.CurrentPortal?.CultureCode;
                    //Maps the cart items.
                    viewModel.TemplateCartItems = accountTemplateModel.TemplateCartItems?.ToViewModel<TemplateCartItemViewModel>()?.ToList();

                    SetTemplateCartItemModel(viewModel.TemplateCartItems);

                    ZnodeLogging.LogMessage("Execution Done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
            return new TemplateViewModel();
        }

        //Remove cart item from template.
        public virtual bool RemoveSingleCartLineItem(int omsTemplateId, int omsTemplateLineItemId)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (omsTemplateId > 0 && omsTemplateLineItemId > 0)
                {
                    return _saveForLaterClient.DeleteCartItem(new AccountTemplateModel { OmsTemplateId = omsTemplateId, OmsTemplateLineItemId = omsTemplateLineItemId.ToString() });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return false;
        }

        //Remove all cart items from saved cart later.
        public virtual bool RemoveAllTemplateCartItems(int omsTemplateId, bool isFromSavedCart = false)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return _saveForLaterClient.DeleteAllCartItems(omsTemplateId, isFromSavedCart);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return false;
        }

        #endregion

        #region Protected Methods
        //Sets the template cart item model.
        protected virtual void SetTemplateCartItemModel(List<TemplateCartItemViewModel> templateCartItems)
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
                            if (HelperUtility.IsNotNull(product))
                            {
                                SetProductDetailsForGroupProduct(item, product);
                                cartItem.ImagePath = product.ImageSmallPath;
                            }
                        }                        
                    }

                    //Sets the properties.
                    if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs))
                        product = productAgent.GetProductPriceAndInventory(cartItem.ConfigurableProductSKUs, cartItem.Quantity.ToString(), "", cartItem.SKU);
                    else
                        product = productAgent.GetProductPriceAndInventory(cartItem.SKU, cartItem.Quantity.ToString(), cartItem.AddOnProductSKUs, cartItem.SKU);
                    if (HelperUtility.IsNotNull(product))
                    {
                        SetProductDetails(cartItem, product);
                    }
                }
            }
        }

        //Set product details for the group product
        protected void SetProductDetailsForGroupProduct(AssociatedProductModel associatedProduct, ProductViewModel product)
        {
            if (HelperUtility.IsNotNull(product))
            {
                associatedProduct.ProductId = product.PublishProductId;
                associatedProduct.UnitPrice = HelperUtility.IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal(product.PromotionalPrice) : HelperUtility.IsNotNull(product.UnitPrice) ? Convert.ToDecimal(product.UnitPrice.ToPriceRoundOff()) : 0;
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
        protected virtual void SetProductDetails(TemplateCartItemViewModel cartItem, ProductViewModel product)
        {
            if (HelperUtility.IsNotNull(product))
            {
                SetUnitPriceWithAddon(cartItem, product);
                //Maps the data property.
                cartItem.ProductId = product.ConfigurableProductId > 0 ? product.ConfigurableProductId.ToString() : product.PublishProductId.ToString();
                cartItem.UnitPrice = HelperUtility.IsNotNull(product.UnitPrice) ? Convert.ToDecimal(product.UnitPrice.ToPriceRoundOff()) : HelperUtility.IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal(product.PromotionalPrice) : 0;
                cartItem.ExtendedPrice = cartItem?.GroupProducts?.Count() > 0 ? Convert.ToDecimal(cartItem.ExtendedPrice.ToPriceRoundOff()) : HelperUtility.IsNotNull(product.PromotionalPrice) ? Convert.ToDecimal((product?.PromotionalPrice.GetValueOrDefault() * cartItem.Quantity).ToPriceRoundOff()) : Convert.ToDecimal((product?.UnitPrice.GetValueOrDefault() * cartItem.Quantity).ToPriceRoundOff());
                cartItem.ImagePath = cartItem?.GroupProducts?.Count() > 0 ? cartItem.ImagePath :  product.ImageSmallPath;
                cartItem.SEODescription = product.SEODescription;
                cartItem.SEOKeywords = product.SEOKeywords;
                cartItem.SEOTitle = product.SEOTitle;
                cartItem.SEOUrl = product.SEOUrl;
                cartItem.MaxQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                cartItem.MinQuantity = Convert.ToDecimal(product.Attributes?.Value(ZnodeConstant.MinimumQuantity));
                cartItem.OutOfStockMessage = !string.Equals(product.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.Code, ZnodeConstant.DontTrackInventory, StringComparison.InvariantCultureIgnoreCase) ? string.Format(WebStore_Resources.ExceedingAvailableQuantity, product.DefaultInventoryCount) : ZnodeConstant.DontTrackInventory;
                cartItem.IsActive = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsActive));
                cartItem.IsObsolete = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.IsObsolete));
                cartItem.DefaultInventoryCount = product.DefaultInventoryCount;
                if (!string.IsNullOrEmpty(cartItem?.ConfigurableProductSKUs))
                {      
                    cartItem.SKU = product.SKU;
                    cartItem.ProductAttributes = (List<PublishAttributeModel>)product.Attributes.ToModel<PublishAttributeModel>();
                }
            }
        }

        //Get cart description.
        //Flagged
        protected virtual string GetCartDescription(ProductViewModel product, TemplateCartItemViewModel cartItem)
        {
            string cartdescription = string.Empty;
            List<BundleProductViewModel> bundleProductList = null;

            if (string.Equals(product.ProductType, ZnodeConstant.BundleProduct, StringComparison.CurrentCultureIgnoreCase))
                bundleProductList = GetBundleProduct(product.PublishProductId);

            if (HelperUtility.IsNotNull(bundleProductList))
            {
                foreach (BundleProductViewModel bundle in bundleProductList)
                    cartdescription += $"{ bundle.SKU } - { bundle.Name } <br/>";
            }
            //Binds the cart description.
            if (HelperUtility.IsNotNull(product?.AddOns))
            {
                foreach (string cartaddon in cartItem.AddOnProductSKUs.Split(','))
                {
                    foreach (AddOnViewModel addon in product?.AddOns)
                    {
                        if (HelperUtility.IsNotNull(addon))
                        {
                            var productAddon = addon.AddOnValues.Find(x => x.SKU == cartaddon);
                            if (HelperUtility.IsNotNull(productAddon))
                                cartdescription += $"{ addon.GroupName } : { String.Join("<br />", productAddon.Name) } <br> ";
                        }
                    }
                }
            }

            return cartdescription;
        }

        //Get Bundle Product list.
        protected virtual List<BundleProductViewModel> GetBundleProduct(int productId)
        {
            IProductAgent _productAgent = new ProductAgent(null, _publishProductClient, null, null, null, null);

            //Get group product list.
            List<BundleProductViewModel> bundleProductList = _productAgent.GetBundleProduct(productId);
            return bundleProductList;
        }

        //Set Cart Item model.
        protected virtual void SetTemplateModelData(AccountTemplateModel accountTemplateModel)
        {
            accountTemplateModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            accountTemplateModel.LocaleId = PortalAgent.LocaleId;
            accountTemplateModel.PublishedCatalogId = PortalAgent.CurrentPortal.PublishCatalogId;
            accountTemplateModel.UserId = Convert.ToInt32(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId);
            accountTemplateModel.TemplateType = ZnodeConstant.SaveForLater;
            accountTemplateModel.TemplateName = ZnodeConstant.SaveForLater;
        }

        //Set Addon product price.
        public virtual void SetUnitPriceWithAddon(TemplateCartItemViewModel cartItem, ProductViewModel product)
        {
            List<AddOnViewModel> addOns = product?.AddOns;

            if (HelperUtility.IsNotNull(cartItem) && HelperUtility.IsNotNull(product))
            { 
                string addOnIds = cartItem?.AddOnProductSKUs;
                decimal? addonPrice = 0.00M;
                if (addOns?.Count > 0)
                {
                   
                    if (!string.IsNullOrEmpty(addOnIds))
                    {
                        foreach (string addOn in addOnIds.Split(','))
                        {
                            AddOnValuesViewModel addOnValue = addOns.SelectMany(y => y.AddOnValues.Where(x => x.SKU == addOn))?.FirstOrDefault();
                            if (HelperUtility.IsNotNull(addOnValue))
                                addonPrice = addonPrice + Convert.ToDecimal(HelperUtility.IsNotNull(addOnValue.SalesPrice) ? addOnValue?.SalesPrice : addOnValue?.RetailPrice);
                        }
                    };
                }
                    if (product.TierPriceList?.Count > 0 && product.TierPriceList.Where(x => cartItem.Quantity >= x.MinQuantity)?.Count() > 0)
                        product.UnitPrice = product.TierPriceList.FirstOrDefault(x => cartItem.Quantity >= x.MinQuantity && cartItem.Quantity < x.MaxQuantity)?.Price;
                    
                    product.UnitPrice = (product.UnitPrice + addonPrice.GetValueOrDefault());

                    decimal? groupProductPrice = cartItem.GroupProducts?.Select(x => x.UnitPrice)?.FirstOrDefault();
                    if (cartItem.GroupProducts?.Count > 0)
                    {
                        cartItem.GroupProducts?.ForEach(x =>
                        {
                            x.UnitPrice = (x.UnitPrice + addonPrice);
                        });
                    }
                }
            }        
        #endregion
    }
}
