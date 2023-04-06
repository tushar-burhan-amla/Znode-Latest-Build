using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Libraries.Klaviyo.Helper;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Engine.Klaviyo.IClient;

namespace Znode.Engine.WebStore.Agents
{
    public class ProductAgent : BaseAgent, IProductAgent
    {
        #region Private Variables
        private readonly ICustomerReviewClient _reviewClient;
        private readonly IPublishProductClient _productClient;
        private readonly IWebStoreProductClient _webstoreProductClient;
        private readonly ISearchClient _searchClient;
        private readonly IHighlightClient _highlightClient;
        private readonly IPublishCategoryClient _publishCategoryClient;
        private readonly IWebstoreHelper _webstoreHelper;
        #endregion

        #region Public Constructor
        public ProductAgent(ICustomerReviewClient reviewClient, IPublishProductClient productClient, IWebStoreProductClient webstoreProductClient, ISearchClient searchClient, IHighlightClient highlightClient, IPublishCategoryClient publishCategoryClient)
        {
            _reviewClient = GetClient<ICustomerReviewClient>(reviewClient);
            _productClient = GetClient<IPublishProductClient>(productClient);
            _webstoreProductClient = GetClient<IWebStoreProductClient>(webstoreProductClient);
            _searchClient = GetClient<ISearchClient>(searchClient);
            _highlightClient = GetClient<IHighlightClient>(highlightClient);
            _publishCategoryClient = GetClient<IPublishCategoryClient>(publishCategoryClient);
            _webstoreHelper = GetService<IWebstoreHelper>();
        }
        #endregion

        #region Public Method
        //Create customer review for product.
        public virtual ProductReviewViewModel CreateReview(ProductReviewViewModel reviewModel)
        {
            try
            {
                UserViewModel userModel = new UserAgent(GetClient<CountryClient>(), GetClient<WebStoreUserClient>(), GetClient<WishListClient>(), GetClient<UserClient>(), GetClient<PublishProductClient>(), GetClient<CustomerReviewClient>(), GetClient<OrderClient>(), GetClient<GiftCardClient>(), GetClient<AccountClient>(), GetClient<AccountQuoteClient>(), GetClient<OrderStateClient>(), GetClient<PortalCountryClient>(), GetClient<ShippingClient>(), GetClient<PaymentClient>(), GetClient<CustomerClient>(), GetClient<StateClient>(), GetClient<PortalProfileClient>()).GetUserViewModelFromSession();
                reviewModel.UserId = HelperUtility.IsNotNull(userModel) ? (int?)userModel.UserId : null;
                reviewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
                CustomerReviewModel productReviewModel = _reviewClient.CreateCustomerReview(reviewModel?.ToModel<CustomerReviewModel>());
                return HelperUtility.IsNotNull(productReviewModel) ? productReviewModel.ToViewModel<ProductReviewViewModel>() : new ProductReviewViewModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (ProductReviewViewModel)GetViewModelWithErrorMessage(reviewModel, Admin_Resources.SaveErrorMessage);
            }
        }
        // Get product details by Product Id
        public virtual ProductViewModel GetProduct(int productId)
        {
            return GetProduct(productId, false);
        }
        // Get product details associated with category
        public virtual ProductViewModel GetProduct(int productId, bool isCategoryAssociated)
        {
            if (productId > 0)
            {
                //set user id for profile base pricing.
                _productClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

                _productClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
                _productClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
                _productClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                PublishProductModel model = _productClient.GetPublishProduct(productId, SetProductFilter(isCategoryAssociated), GetProductExpands());
                if (HelperUtility.IsNotNull(model))
                {
                    ProductViewModel viewModel = model.ToViewModel<ProductViewModel>();

                    string minQuantity = viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity);

                    viewModel.MinQuantity = minQuantity;

                    decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);

                    viewModel.IsCallForPricing = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (model.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();

                    viewModel.ProductType = viewModel.Attributes.CodeFromSelectValue(ZnodeConstant.ProductType);
                    viewModel.IsConfigurable = model.IsConfigurableProduct;

                    if (string.Equals(viewModel?.ProductType, ZnodeConstant.BundleProduct, StringComparison.InvariantCultureIgnoreCase))
                    {
                        UpdateBundleProductInventoryQuantity(viewModel);
                        CheckBundleProductsInventory(viewModel, 0, _productClient.UserId);
                    }
                    else
                        //Check Main Product inventory
                        CheckInventory(viewModel, quantity);

                    //Check any default addon product is selected or not.
                    string addOnSKU = string.Empty;
                    List<string> addOnProductSKU = viewModel.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.FirstOrDefault(x => x.IsDefault)?.SKU)?.ToList();

                    if (addOnProductSKU?.Count > 0)
                    {
                        addOnSKU = string.Join(",", addOnProductSKU.Where(x => !string.IsNullOrEmpty(x)));
                    }

                    if ((!string.IsNullOrEmpty(addOnSKU) && (HelperUtility.IsNotNull(viewModel.Quantity) && viewModel.Quantity > 0)) || (!string.IsNullOrEmpty(addOnSKU) && (Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct))))
                        //Check Associated addon inventory.
                        CheckAddOnInvenTory(viewModel, addOnSKU, quantity);

                    if (!viewModel.IsCallForPricing)
                        GetProductFinalPrice(viewModel, viewModel.AddOns, quantity, addOnSKU);

                    viewModel.ParentProductId = productId;
                    viewModel.IsConfigurable = HelperUtility.IsNotNull(viewModel.Attributes?.Find(x => x.ConfigurableAttribute?.Count > 0));

                    viewModel.ParentProductImageSmallPath = model?.ParentProductImageSmallPath;

                    UpdateRecentViewedProducts(viewModel);
                    AddToRecentlyViewProduct(viewModel.ConfigurableProductId > 0 ? viewModel.ConfigurableProductId : viewModel.PublishProductId);

                    Helper.SetProductCartParameter(viewModel);

                    if (viewModel.IsConfigurable)
                        GetConfigurableValues(model, viewModel);

                    if(Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.DisplayVariantsOnGrid)) && string.Equals(viewModel?.ProductType, ZnodeConstant.ConfigurableProduct, StringComparison.InvariantCultureIgnoreCase))
                        viewModel.IsConfigurable = model.IsConfigurableProduct;


                    //check if the product is added in wishlist for the logged in user. If so, binds its wishlistId
                    BindProductWishListDetails(viewModel);

                    if (PortalAgent.CurrentPortal.IsKlaviyoEnable)
                    {
                        // Created the task to post the data to klaviyo
                        HttpContext httpContext = HttpContext.Current;

                        Task.Run(() =>
                        {
                            PostDataToKlaviyo(viewModel, httpContext);
                        });
                    }

                    return viewModel;
                }
            }
            throw new ZnodeException(ErrorCodes.ProductNotFound, WebStore_Resources.ErrorProductNotFound);
        }
        // To post the data to klaiyo
        protected virtual void PostDataToKlaviyo(ProductViewModel viewModel, HttpContext httpContext)
        {
            HttpContext.Current = httpContext;
            IKlaviyoClient _klaviyoClient = GetComponentClient<IKlaviyoClient>(GetService<IKlaviyoClient>());
            KlaviyoProductDetailModel klaviyoProductDetailModel = MapProductModelToKlaviyoModel(viewModel);
            bool isTrackKlaviyo = _klaviyoClient.TrackKlaviyo(klaviyoProductDetailModel);
        }

        // Map Product model to Klaviyo Model
        protected virtual KlaviyoProductDetailModel MapProductModelToKlaviyoModel(ProductViewModel viewModel)
        {
            EcommerceDataViewModel ecommerceDataViewModel = viewModel.GetEcommerceDetailsOfProduct();
            KlaviyoProductDetailModel klaviyoProductDetailModel = new KlaviyoProductDetailModel();
            klaviyoProductDetailModel.OrderLineItems = new List<OrderLineItemDetailsModel>();
            klaviyoProductDetailModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            klaviyoProductDetailModel.FirstName = userViewModel?.FirstName;
            klaviyoProductDetailModel.LastName = userViewModel?.LastName;
            klaviyoProductDetailModel.Email = userViewModel?.Email;
            klaviyoProductDetailModel.StoreName = PortalAgent.CurrentPortal.Name;
            klaviyoProductDetailModel.PropertyType = (int) KlaviyoEventType.ProductEvent;
            klaviyoProductDetailModel.OrderLineItems.Add(new OrderLineItemDetailsModel { ProductName = ecommerceDataViewModel.Name, SKU = viewModel.SKU, Quantity = viewModel.Quantity, UnitPrice = Convert.ToDecimal(viewModel?.UnitPrice), Image = viewModel.ImageSmallPath });
            return klaviyoProductDetailModel;
        }

        //This method only returns the brief details of a published product .
        public virtual ProductViewModel GetProductBrief(int productID)
        {
            if (productID > 0)
            {
                //set user id for profile base pricing.
                _productClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

                _productClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
                _productClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
                _productClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
                PublishProductModel model = _productClient.GetPublishProductBrief(productID, GetRequiredFilters(), GetProductExpands(false, ExpandKeys.ProductTemplate, ExpandKeys.SEO, ExpandKeys.ProductReviews, ExpandKeys.Brand));
                if (HelperUtility.IsNotNull(model))
                {
                    ProductViewModel viewModel = model.ToViewModel<ProductViewModel>();

                    viewModel.IsCallForPricing = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (model.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();
                    viewModel.ProductType = viewModel.Attributes.CodeFromSelectValue(ZnodeConstant.ProductType);

                    AddToRecentlyViewProduct(viewModel.ConfigurableProductId > 0 ? viewModel.ConfigurableProductId : viewModel.PublishProductId);

                    viewModel.ParentProductId = productID;
                    viewModel.IsConfigurable = HelperUtility.IsNotNull(viewModel.Attributes?.Find(x => x.ConfigurableAttribute?.Count > 0));

                    Helper.SetProductCartParameter(viewModel);

                    if (viewModel.IsConfigurable)
                        GetConfigurableValues(model, viewModel);

                    //check if the product is added in wishlist for the logged in user. If so, binds its wishlistId
                    BindProductWishListDetails(viewModel);

                    return viewModel;
                }
            }
            throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorProductNotFound);
        }

        //This method only returns the extended details of a published product.
        public virtual ShortProductViewModel GetExtendedProductDetails(int productID, string[] expandKeys)
        {
            if (productID > 0 && HelperUtility.IsNotNull(expandKeys) && expandKeys.Length > 0)
            {
                //set user id for profile base pricing.
                _productClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

                _productClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
                _productClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
                _productClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());

                PublishProductDTO model = _productClient.GetExtendedPublishProductDetails(productID, GetRequiredFilters(), GetProductExpands(false, expandKeys));
                if (HelperUtility.IsNotNull(model))
                {
                    ShortProductViewModel viewModel = model.ToViewModel<ShortProductViewModel>();

                    string minQuantity = viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity);

                    decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);

                    viewModel.IsCallForPricing = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (model.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();

                    viewModel.MiscellaneousDetails.ProductType = viewModel.Attributes.CodeFromSelectValue(ZnodeConstant.ProductType);

                    //Check Main Product inventory
                    CheckInventory(viewModel, quantity);

                    //Get Addon SKu from required addons.
                    string addonSKu = string.Join(",", viewModel.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.FirstOrDefault(x => x.IsDefault)?.SKU));

                    if ((!string.IsNullOrEmpty(addonSKu) && (HelperUtility.IsNotNull(viewModel.InventoryDetails.Quantity) && viewModel.InventoryDetails.Quantity > 0)) || (!string.IsNullOrEmpty(addonSKu) && (Equals(viewModel.MiscellaneousDetails.ProductType, ZnodeConstant.GroupedProduct))))
                        //Check Associated addon inventory.
                        CheckAddOnInventory(viewModel, addonSKu, quantity);

                    if (!viewModel.IsCallForPricing)
                        GetProductFinalPrice(viewModel, viewModel.AddOns, quantity, addonSKu);

                    viewModel.ParentProductId = productID;
                    viewModel.IsConfigurable = HelperUtility.IsNotNull(viewModel.Attributes?.Find(x => x.ConfigurableAttribute?.Count > 0));

                    AddToRecentlyViewProduct(viewModel.MiscellaneousDetails.ConfigurableProductId > 0 ? viewModel.MiscellaneousDetails.ConfigurableProductId : viewModel.PublishProductId);

                    Helper.SetProductCartParameter(viewModel);

                    if (viewModel.IsConfigurable)
                        GetConfigurableValues(model, viewModel);

                    //check if the product is added in wishlist for the logged in user. If so, binds its wishlistId
                    BindProductWishListDetails(viewModel);

                    return viewModel;
                }
            }
            throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorProductNotFound);
        }

        //Get Product Final price incliding addons price
        protected virtual void GetProductFinalPrice(ShortProductViewModel viewModel, List<AddOnViewModel> addOns, decimal minQuantity, string addOnIds)
        {
            if (!viewModel.IsCallForPricing)
            {
                viewModel.Pricing.UnitPrice = viewModel.Pricing.SalesPrice > 0 ? viewModel.Pricing.SalesPrice : viewModel.Pricing.RetailPrice;
                //Apply tier price if any.
                if (viewModel.Pricing.TierPriceList?.Count > 0 && viewModel.Pricing.TierPriceList.Where(x => minQuantity >= x.MinQuantity)?.Count() > 0)
                    viewModel.Pricing.ProductPrice = viewModel.Pricing.TierPriceList.FirstOrDefault(x => minQuantity >= x.MinQuantity && minQuantity < x.MaxQuantity)?.Price;
                else
                    viewModel.Pricing.ProductPrice = (minQuantity > 0 && HelperUtility.IsNotNull(viewModel.Pricing.SalesPrice)) ? viewModel.Pricing.SalesPrice * minQuantity : viewModel.Pricing.PromotionalPrice > 0 ? viewModel.Pricing.PromotionalPrice * minQuantity : viewModel.Pricing.RetailPrice;

                if (addOns?.Count > 0)
                {
                    decimal? addonPrice = 0.00M;

                    if (!string.IsNullOrEmpty(addOnIds))
                    {
                        foreach (string addOn in addOnIds.Split(','))
                        {
                            AddOnValuesViewModel addOnValue = addOns.SelectMany(
                                       y => y.AddOnValues.Where(x => x.SKU == addOn))?.FirstOrDefault();
                            if (HelperUtility.IsNotNull(addOnValue))
                                addonPrice = addonPrice + (HelperUtility.IsNotNull(addOnValue.SalesPrice) ? addOnValue.SalesPrice : addOnValue.RetailPrice);

                        }
                    }
                    viewModel.Pricing.ProductPrice = addonPrice > 0 ? viewModel.Pricing.ProductPrice + addonPrice : viewModel.Pricing.ProductPrice;

                    //Check add on price.
                    if (HelperUtility.IsNull(addonPrice))
                    {
                        viewModel.InventoryDetails.ShowAddToCart = false;
                        viewModel.InventoryDetails.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorAddOnPrice;
                    }
                }
                //Check product final price.
                if (HelperUtility.IsNull(viewModel.Pricing.ProductPrice) && (!Equals(viewModel.MiscellaneousDetails.ProductType, ZnodeConstant.GroupedProduct)))
                {
                    viewModel.InventoryDetails.ShowAddToCart = false;
                    viewModel.InventoryDetails.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorPriceNotAssociate;
                }
            }
            else
                viewModel.InventoryDetails.ShowAddToCart = false;
        }

        //Get Product Final price incliding addons price
        protected virtual void GetProductFinalPrice(ProductViewModel viewModel, List<AddOnViewModel> addOns, decimal minQuantity, string addOnIds)
        {
            bool isDisplayVariantsOnGrid = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.DisplayVariantsOnGrid)) && viewModel.IsConfigurable;

            bool isAddonPriceAvailable = true;
            if (!viewModel.IsCallForPricing)
            {
                viewModel.UnitPrice = viewModel.SalesPrice > 0 ? viewModel.SalesPrice : viewModel.RetailPrice;
                //Apply tier price if any.
                if (viewModel.TierPriceList?.Count > 0 && viewModel.TierPriceList.Where(x => minQuantity >= x.MinQuantity)?.Count() > 0)
                    viewModel.ProductPrice = viewModel.TierPriceList.FirstOrDefault(x => minQuantity >= x.MinQuantity && minQuantity < x.MaxQuantity)?.Price;
                else
                    viewModel.ProductPrice = (minQuantity > 0 && HelperUtility.IsNotNull(viewModel.SalesPrice)) ? viewModel.SalesPrice * minQuantity : viewModel.PromotionalPrice > 0 ? viewModel.PromotionalPrice * minQuantity : viewModel.RetailPrice;

                if (addOns?.Count > 0)
                {
                    decimal? addonPrice = 0.00M;

                    if (!string.IsNullOrEmpty(addOnIds))
                    {
                        foreach (string addOn in addOnIds.Split(','))
                        {
                            AddOnValuesViewModel addOnValue = addOns.SelectMany(
                                       y => y.AddOnValues.Where(x => x.SKU == addOn))?.FirstOrDefault();
                            if (HelperUtility.IsNotNull(addOnValue))
                                addonPrice = addonPrice + Convert.ToDecimal(HelperUtility.IsNotNull(addOnValue.SalesPrice) ? addOnValue?.SalesPrice : addOnValue?.RetailPrice);

                            if (HelperUtility.IsNull(addOnValue?.SalesPrice) && HelperUtility.IsNull(addOnValue?.RetailPrice))
                                isAddonPriceAvailable = false;
                        }
                    }
                    viewModel.ProductPrice = addonPrice > 0 ? viewModel.ProductPrice + addonPrice : viewModel.ProductPrice;

                    //Check add on price.
                    if (!isAddonPriceAvailable)
                    {
                        viewModel.ShowAddToCart = false;
                        viewModel.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorAddOnPrice;
                    }
                }
                //Check product final price.
                if (HelperUtility.IsNull(viewModel.ProductPrice) && (!Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct) && !isDisplayVariantsOnGrid))
                {
                    viewModel.ShowAddToCart = false;
                    viewModel.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorPriceNotAssociate;
                }
            }
            else
                viewModel.ShowAddToCart = false;
        }

        //Get Product details by product id.
        public virtual List<AutoComplete> GetProductList(string sku)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, "True");
                filter.Add(WebStoreEnum.ProductIndex.ToString(), FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());
                filter.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, GetCatalogId().GetValueOrDefault().ToString());
                filter.Add(WebStoreEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());


                KeywordSearchModel keyWordSearchModel = _searchClient.GetKeywordSearchSuggestion(new SearchRequestModel { Keyword = sku.Trim(), LocaleId = PortalAgent.LocaleId, PortalId = PortalAgent.CurrentPortal.PortalId, CatalogId = GetCatalogId().GetValueOrDefault(), PageSize = 10 }, null, filter, null, 0, 0);

                ProductListViewModel listViewModel = new ProductListViewModel { Products = keyWordSearchModel?.Products?.ToViewModel<ProductViewModel>().ToList() };

                List<AutoComplete> _autoComplete = new List<AutoComplete>();

                if ((listViewModel.Products?.Count > 0))
                {
                    //Assign AutoCompleteLabel and Cart quantity to each product
                    listViewModel.Products.ForEach(item =>
                    {
                        AutoComplete _item = new AutoComplete();
                        _item.Name = HttpUtility.HtmlDecode(string.Format(WebStore_Resources.AutoCompleteLabelQuickOrder,
                                         item.Name, string.Empty, item.SKU, string.Empty,
                                            string.Empty, '"' + item.ImageSmallThumbnailPath + '"'));

                        _item.Id = item.PublishProductId;
                        _item.DisplayText = item.SKU;
                        _item.Properties.Add("MaxQuantity", Attributes.Value(item.Attributes, ZnodeConstant.MaximumQuantity));
                        _autoComplete.Add(_item);
                    });
                }
                return _autoComplete;
            }
            else
                return new List<AutoComplete>();
        }

        //This method is Add valid SKUs to Quick Order Grid
        public virtual QuickOrderViewModel AddProductsToQuickOrder(string multipleItems)
        {
            QuickOrderViewModel quickOrderModel = new QuickOrderViewModel() { ProductSKUText = multipleItems };
            ValidateProductSKUs(MergeDuplicateSKUs(ExtractValidSKUs(quickOrderModel.ProductSKUText)), quickOrderModel);
            if (quickOrderModel.ValidSKUCount > 0)
            {
                RemoveValidProductSKUs(quickOrderModel);
            }
            if (quickOrderModel.ProductDetail.Count > 50)
            {
                quickOrderModel.ProductDetail.RemoveRange(50, quickOrderModel.ProductDetail.Count-50);
                quickOrderModel.ValidSKUCount = 50;
            }
            SetQuickOrderNotification(quickOrderModel);
            return quickOrderModel;
        }

        //This method to download quick order template
        public virtual void DownloadQuickOrderTemplate(HttpResponseBase response)
        {
            IDownloadHelper helper = GetService<IDownloadHelper>();
            DownloadModel downloadModel = new DownloadModel();
            downloadModel.data = GetDummyQuickOrderProductList();
            helper.ExportDownload(downloadModel.data, "2", response, ",", $"{WebStoreConstants.TemplateQuickOrder}{WebStoreConstants.CSV}", false, false);
        }


        //This method merge duplicate skus
        protected virtual List<QuickOrderCartViewModel> MergeDuplicateSKUs(List<QuickOrderCartViewModel> quickOrderModel)
        {
            if (quickOrderModel?.Count > 0)
            {
                List<string> duplicateSKUs = quickOrderModel.GroupBy(x => x.SKU).Where(g => g.Count() > 1).Select(m => m.Key)?.ToList();
                if (duplicateSKUs?.Count() > 0)
                {
                    List<QuickOrderCartViewModel> mergeProductSKUs = quickOrderModel.Where(x => duplicateSKUs.Contains(x.SKU)).GroupBy(x => x.SKU).Select(p => new QuickOrderCartViewModel() { SKU = p.Key, Quantity = p.Sum(m => m.Quantity) })?.ToList();
                    if(mergeProductSKUs?.Count > 0)
                    {
                        quickOrderModel.RemoveAll(m => duplicateSKUs.Contains(m.SKU));
                        quickOrderModel.AddRange(mergeProductSKUs);
                    }
                }
            }
            return quickOrderModel;
        }

        //This method is to set quick order notification
        protected virtual void SetQuickOrderNotification(QuickOrderViewModel quickOrderModel)
        {
            string message = string.Empty;
            quickOrderModel.IsSuccess = quickOrderModel.ValidSKUCount > 0;
            if (quickOrderModel.ValidSKUCount > 0)
            {
                message = string.Format(quickOrderModel.ValidSKUCount == 1 ? WebStore_Resources.ValidProductCount : WebStore_Resources.ValidProductsCount, quickOrderModel.ValidSKUCount.ToString());
            }
            if (quickOrderModel.InvalidSKUCount > 0)
            {
                message = !string.IsNullOrEmpty(message) ? $"{message}<br/>" : message;
                message += string.Format(quickOrderModel.InvalidSKUCount == 1 ? WebStore_Resources.InvalidProductCount : WebStore_Resources.InvalidProductsCount, quickOrderModel.InvalidSKUCount.ToString());
            }
            if(quickOrderModel.ValidSKUCount == 0 && quickOrderModel.InvalidSKUCount == 0 )
            {
                message = WebStore_Resources.ErrorProcessingRequest;
            }
            quickOrderModel.NotificationMessage = message;
        }

        //This method is for extract sku in valid form
        protected virtual List<QuickOrderCartViewModel> ExtractValidSKUs(string productSKUText)
        {
            List<QuickOrderCartViewModel> model = new List<QuickOrderCartViewModel>();
            if (!string.IsNullOrEmpty(productSKUText))
            {
                Regex rg = new Regex(@"^(?<sku>[a-zA-Z0-9_-]*)[\s|,]*(?<quantity>[0-9]*)[\s]*$");
                string[] skuList = new Regex(@"\r\n|\n|\r", RegexOptions.Singleline).Split(productSKUText);
                for (int index = 0; index < skuList?.Length; ++index)
                {
                    string sku = "";
                    int quantity = 0;
                    Match match = rg.Match(skuList[index]);
                    if (match?.Success == true)
                    {
                        string[] groupNames = rg.GetGroupNames();
                        foreach (string gName in groupNames)
                        {
                            if (gName.Contains("sku"))
                            {
                                sku = match.Groups[gName]?.Value;
                            }
                            else if (gName.Contains("quantity"))
                            {
                                if (!Int32.TryParse(match.Groups[gName]?.Value, out quantity))
                                {
                                    quantity = 1;
                                }
                            }
                        }
                        if (sku?.Length > 0)
                        {
                            model.Add(new QuickOrderCartViewModel() { SKU = sku, Quantity = quantity });
                        }
                    }
                }
            }
            return model;
        }

        //This method is to validate sku against catalog and get details
        protected virtual void ValidateProductSKUs(List<QuickOrderCartViewModel> productSKUs, QuickOrderViewModel quickOrderModel)
        {
            if (productSKUs?.Count > 0 && IsNotNull(quickOrderModel))
            {
                IQuickOrderClient quickOrderClient = GetClient<IQuickOrderClient>(GetService<IQuickOrderClient>());
                string productSku = string.Join(",", productSKUs.Select(m => m.SKU));
                FilterCollection filters = GetRequiredFilters();
                quickOrderModel.ProductDetail = quickOrderClient.GetQuickOrderProductList(filters, new QuickOrderParameterModel { SKUs = productSku }).Products?.ToViewModel<QuickOrderProductViewModel>()?.ToList();
                int uniqueId = new Random().Next();
                quickOrderModel.ValidSKUCount = 0;
                List<string> skus = quickOrderModel.ProductDetail?.Select(m => m.SKU).Distinct()?.ToList();
                List<QuickOrderCartViewModel> skuQuantities = GetOrderedItemQuantity(skus);
                quickOrderModel.ProductDetail?.ForEach(product =>
                {
                    for (int i = 0; i < product.PublishBundleProducts.Count(); i++)
                    {
                        product.PublishBundleProducts[i].Quantity = product.QuantityOnHand;
                    }
                    product.UniqueId = uniqueId;
                    product.Quantity = productSKUs.FirstOrDefault(m => string.Equals(m.SKU, product.SKU, StringComparison.InvariantCultureIgnoreCase))?.Quantity;
                    if (product.ProductType == ZnodeConstant.BundleProduct)
                    {
                        UpdateBundleProductInventoryQuantity(product);

                        bool isChildDisablePurchasing = false;
                        if (product.PublishBundleProducts != null)
                        {
                            isChildDisablePurchasing = product.PublishBundleProducts.Any(x => x.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.Code == ZnodeConstant.DisablePurchasing);
                        }
                        product.InventoryCode = isChildDisablePurchasing ? ZnodeConstant.DisablePurchasing : product.InventoryCode;
                    }
                    product.CartQuantity = skuQuantities?.FirstOrDefault(m => string.Equals(m.SKU, product.SKU, StringComparison.InvariantCultureIgnoreCase))?.Quantity ?? 0;
                    if (product.IsActive)
                    {
                        uniqueId = uniqueId + 1;
                    }
                });
                quickOrderModel.ValidSKUCount = quickOrderModel.ProductDetail.Count(m => m.IsActive);
                quickOrderModel.InvalidSKUCount = productSKUs.Count - quickOrderModel.ValidSKUCount;
                quickOrderModel.IsSuccess = true;
            }
            else
            {
                quickOrderModel.IsSuccess = false;
            }
        }

        //This method remove valid skus from item number field
        protected virtual void RemoveValidProductSKUs(QuickOrderViewModel quickOrderModel)
        {
            if (quickOrderModel?.ProductDetail?.Count > 0)
            {
                List<string> skus = quickOrderModel.ProductDetail.Select(m => m.SKU)?.ToList();
                List<string> skuList = new Regex(@"\r\n|\n|\r", RegexOptions.Singleline).Split(quickOrderModel.ProductSKUText).ToList();
                if (skus?.Count > 0 && skuList?.Count > 0)
                {
                    skuList.RemoveAll(m => skus.Any(x => x?.Length > 0 && m.Contains(x)));
                }
                quickOrderModel.ProductSKUText = string.Join(Environment.NewLine, skuList);
            }
        }

        //This method return random quick order product basic details
        protected virtual List<dynamic> GetDummyQuickOrderProductList()
        {
            IQuickOrderClient quickOrderClient = GetClient<IQuickOrderClient>(GetService<IQuickOrderClient>());
            QuickOrderProductListModel productList = quickOrderClient.GetDummyQuickOrderProductList(GetRequiredFilters(), 1, 2);
            List<dynamic> products = new List<dynamic>();
            productList?.Products?.ForEach(m => {
                products.Add(new { SKU = m.SKU, Quantity = 1 });
            });
            return products;
        }

        //Gets selected sku details.
        public AutoComplete GetAutoCompleteProductProperties(int productId)
        {
            //Get published product by product ID.
            _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            PublishProductModel product = _productClient.GetPublishProduct(productId, GetRequiredFilters(), GetProductInventoryExpands());

            return HelperUtility.IsNotNull(product) ? MapProductDetails(product, productId) : new AutoComplete();
        }

        protected virtual AutoComplete MapProductDetails(PublishProductModel product, int productId = 0)
        {
            AutoComplete _item = new AutoComplete();

            ProductViewModel productViewModel = product?.ToViewModel<ProductViewModel>();

            List<AttributeValidationViewModel> attributeValidation = GetattributeValidation(productViewModel, productId > 0? productId:(product?.ProductId).GetValueOrDefault());

            bool? isCallForPricing = false;
            if (!Convert.ToBoolean(product?.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.FirstOrDefault()?.AttributeValues))
                isCallForPricing = product?.Promotions.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing);
            else
                isCallForPricing = Convert.ToBoolean(product?.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.FirstOrDefault()?.AttributeValues);

            List<GroupProductViewModel> associatedProductList = GetGroupProductList(productId > 0 ? productId : (product?.ProductId).GetValueOrDefault());
            if (associatedProductList.Any())
            {
                _item.Properties.Add("GroupProductSKUs", string.Join(",", associatedProductList.Select(x => x.SKU)));
                _item.Properties.Add("GroupProductsQuantity", associatedProductList.Select(x => x.SKU).Count().ToString());
            }

            _item.DisplayText = product.SKU;
            _item.Id = product.PublishProductId;
            if (productViewModel.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code == ZnodeConstant.BundleProduct)
            {
                UpdateBundleProductInventoryQuantity(productViewModel);
                product.Quantity = productViewModel.Quantity;
                // Returns bool if child product out of stock setting is DisablePurchasing
                bool isChildDisablePurchasing = productViewModel.PublishBundleProducts
                                     .Any(x => x.Attributes.SelectAttributeList(ZnodeConstant.OutOfStockOptions).FirstOrDefault().Code == ZnodeConstant.DisablePurchasing);
                _item.Properties.Add("InventoryCode", isChildDisablePurchasing ? ZnodeConstant.DisablePurchasing : GetOutOfStockOptionsAttributeList(productViewModel)?.FirstOrDefault().Code);
            }
            else
            {
                _item.Properties.Add("InventoryCode", GetOutOfStockOptionsAttributeList(productViewModel)?.FirstOrDefault().Code);
            }
            _item.Properties.Add("InventoryMessage", GetInventoryMessage(productViewModel));

            _item.Properties.Add("CartQuantity", GetOrderedItemQuantity(product.SKU));
            _item.Properties.Add("ProductName", product.Name);
            _item.Properties.Add("Quantity", product.Quantity);
            _item.Properties.Add("ProductType", productViewModel.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code);
            _item.Properties.Add("CallForPricing", isCallForPricing);
            _item.Properties.Add("TrackInventory", productViewModel.Attributes.Where(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.AttributeValues);
            _item.Properties.Add("OutOfStockMessage", string.IsNullOrEmpty(product.OutOfStockMessage) ? WebStore_Resources.TextOutofStock : product.OutOfStockMessage);
            _item.Properties.Add("MaxQuantity", productViewModel.Attributes.Where(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.FirstOrDefault()?.AttributeValues);
            _item.Properties.Add("MinQuantity", productViewModel.Attributes.Where(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.FirstOrDefault()?.AttributeValues);
            _item.Properties.Add("RetailPrice", product.RetailPrice);
            _item.Properties.Add("ImagePath", product.ImageSmallPath);
            _item.Properties.Add("IsPersonalisable", productViewModel.Attributes.Where(x => x.IsPersonalizable == true).Select(x => x.IsPersonalizable).FirstOrDefault());
            _item.Properties.Add("IsRequired", attributeValidation?.Where(x => x.IsRequired == true)?.Select(x => x.IsRequired)?.FirstOrDefault().ToString()?.ToLower());
            _item.Properties.Add("ConfigurableProductSKUs", product.ConfigurableProductSKU);
            _item.Properties.Add("AutoAddonSKUs", string.Join(",", product.AddOns.Where(x => x.IsAutoAddon)?.Select(y => y.AutoAddonSKUs)));
            _item.Properties.Add("IsObsolete", productViewModel.Attributes.Where(x => x.AttributeCode == ZnodeConstant.IsObsolete)?.FirstOrDefault()?.AttributeValues);
            _item.Properties.Add("IsActive", product.IsActive);
            return _item;
        }
        //Get inventory message.
        public virtual string GetInventoryMessage(ProductViewModel model)
        {
            string inventoryMessage = string.Empty;

            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(model);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;

            if (inventorySetting?.Count > 0)
            {
                if (HelperUtility.IsNotNull(model.Quantity) && model.Quantity > 0)
                {
                    bool AllowBackOrder = false;
                    bool TrackInventory = false;
                    string minQuantity = model?.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                    decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);
                    decimal combinedQuantity = quantity + GetOrderedItemQuantity(model.SKU);

                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                    if (model.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                        inventoryMessage = !string.IsNullOrEmpty(model.OutOfStockMessage) ? model.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                    else if (model.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                        inventoryMessage = !string.IsNullOrEmpty(model.BackOrderMessage) ? model.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;

                    if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(minQuantity), Convert.ToDecimal(model.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                        inventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, minQuantity, model.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                }
                else if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                    inventoryMessage = !string.IsNullOrEmpty(model.InStockMessage) ? model.InStockMessage : WebStore_Resources.TextInstock;
                else
                    inventoryMessage = !string.IsNullOrEmpty(model.OutOfStockMessage) ? model.OutOfStockMessage : WebStore_Resources.TextOutofStock;
            }
            return inventoryMessage;
        }

        public virtual ProductViewModel GetProductPriceAndInventory(string productSKU, string quantity, string addOnIds, string parentproductSKU = "", int parentProductId = 0)
        {
            decimal selectedQuantity = 0;
            decimal.TryParse(quantity, out selectedQuantity);

            //Get Product By product sku
            _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ProductViewModel productModel = _productClient.GetPublishProductBySKU(new ParameterProductModel { SKU = productSKU, ParentProductSKU = parentproductSKU, ParentProductId = parentProductId }, GetProductExpands(), GetRequiredFilters())?.ToViewModel<ProductViewModel>();

            if (!string.IsNullOrEmpty(parentproductSKU) && !string.Equals(productSKU, parentproductSKU, StringComparison.InvariantCultureIgnoreCase) && HelperUtility.IsNotNull(productModel))
            {
                string sku = productModel.SKU;
                productModel.SKU = parentproductSKU;
                productModel.ConfigurableProductSKU = productSKU;
            }

            if (HelperUtility.IsNotNull(productModel))
            {
                Helper.SetProductCartParameter(productModel);

                productModel.IsCallForPricing = Convert.ToBoolean(productModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (productModel.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();

                if (productModel.PublishBundleProducts.Count > 0)
                {
                    UpdateBundleProductInventoryQuantity(productModel);
                }

                //Check product inventory.
                if (productModel.PublishBundleProducts.Count > 0)
                    CheckBundleProductsInventory(productModel, 0, _productClient.UserId, selectedQuantity);
                else
                    CheckInventory(productModel, selectedQuantity);

                //Check Add on inventory.
                if (productModel.ShowAddToCart)
                {
                    CheckAddOnInvenTory(productModel, addOnIds, selectedQuantity);
                }

                GetProductFinalPrice(productModel, productModel.AddOns, selectedQuantity, addOnIds);
                return productModel;
            }
            else
                return new ProductViewModel();
        }

        // Get Configurable Product variant image.
        public virtual ProductViewModel GetProductImage(ProductDetailViewModel productDetailViewModel)
        {
            ProductViewModel productViewModel = new ProductViewModel();

            if(HelperUtility.IsNotNull(productDetailViewModel))
            {
                PortalViewModel portalViewModel = PortalAgent.CurrentPortal;
                productViewModel.AlternateImages = new List<ProductAlterNateImageViewModel>();
                productViewModel.PublishProductId = productDetailViewModel.PublishProductId;
                productViewModel.SKU = productDetailViewModel.SKU;
                productViewModel.Name = productDetailViewModel.Name;
                productViewModel.ImageLargePath = $"{portalViewModel.ImageLargeUrl}{productDetailViewModel.ImageName}";
                productViewModel.ImageMediumPath = $"{portalViewModel.ImageMediumUrl}{productDetailViewModel.ImageName}";
                string[] images = productDetailViewModel.AlternateImageName?.Split(',')?.ToArray();

                if (HelperUtility.IsNotNull(images) && images?.Count() > 0)
                {
                    foreach(string image in images)
                    {
                        ProductAlterNateImageViewModel productAlterNateImageViewModel = new ProductAlterNateImageViewModel();
                        productAlterNateImageViewModel.OriginalImagePath = $"{portalViewModel.MediaServerUrl}{image}";
                        productAlterNateImageViewModel.ImageThumbNailPath = $"{portalViewModel.ImageThumbnailUrl}{image}";
                        productViewModel.AlternateImages.Add(productAlterNateImageViewModel);
                    }
                }
            }
            return productViewModel;
        }

        //Updated the available quantity in inventory of bundle product based on child product availability
        protected virtual void UpdateBundleProductInventoryQuantity(ProductViewModel productModel)
        {
            List<int> AllowInventory = new List<int>();
            var userId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();
            if (IsNotNull(productModel) && IsNotNull(productModel.PublishBundleProducts))
            {
                productModel.PublishBundleProducts?.ForEach(product =>
                {
                    if (IsNotNull(product))
                    {
                        CheckBundleProductInventory(product, product.Quantity, 0, userId);
                        if (product.AllowBackOrder != true && product.TrackInventory == true)
                        {
                            AllowInventory.Add(Convert.ToInt32(product.Quantity / product.AssociatedQuantity));
                        }
                    }
                });
                if (AllowInventory.Count > 0)
                    productModel.Quantity = AllowInventory.OrderBy(x => x).FirstOrDefault();
                else
                {
                    productModel.Quantity = Convert.ToInt32(productModel?.Quantity);
                }
            }
        }

        //Updated the available quantity in inventory of bundle product based on child product availability
        protected virtual void UpdateBundleProductInventoryQuantity(QuickOrderProductViewModel productModel)
        {
            List<int> AllowInventory = new List<int>();
            var userId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();
            productModel?.PublishBundleProducts?.ForEach(product=> {
                CheckBundleProductInventory(product, product.Quantity, 0, userId);
                if (product.AllowBackOrder != true && product.TrackInventory == true){
                    AllowInventory.Add(Convert.ToInt32(product.Quantity / product.AssociatedQuantity));
                }
            });
            if (AllowInventory.Count > 0)
                productModel.QuantityOnHand = AllowInventory.OrderBy(x => x).FirstOrDefault();
            else
            {
                productModel.QuantityOnHand = Convert.ToInt32(productModel.QuantityOnHand);
                productModel.ChildTrackInventory = "true";
            }
        }

        // Set bundle product inventorymessage  and ShowAddToCart based on the child product
        public virtual void CheckBundleProductsInventory(ProductViewModel viewModel, int omsOrderId = 0, int? userId = 0, decimal selectedQuantity = 0)
        {
            bool isBundleInventoryMessageUpdated = false;
            foreach (var publishBundleProduct in viewModel.PublishBundleProducts)
            {
                string minQuantity = publishBundleProduct.Attributes?.Find(x => x.AttributeCode == ZnodeConstant.MinimumQuantity).AttributeValues;
                decimal quantity = selectedQuantity != 0 ? selectedQuantity : Convert.ToDecimal(minQuantity);
                CheckBundleProductInventory(publishBundleProduct, quantity, 0, userId);
                if (!isBundleInventoryMessageUpdated && !publishBundleProduct.ShowAddToCart)
                {
                    viewModel.InventoryMessage = selectedQuantity > 0 ? WebStore_Resources.ExceedingAvailableWithoutQuantity : publishBundleProduct.InventoryMessage;
                    viewModel.ShowAddToCart = publishBundleProduct.ShowAddToCart;
                    isBundleInventoryMessageUpdated = true;
                }
            }
            if (string.IsNullOrEmpty(viewModel.InventoryMessage))
            {
                string parentMinQuantity = viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                decimal? parentQuantity = selectedQuantity != 0 ? selectedQuantity : Convert.ToDecimal(parentMinQuantity);
                BundleProductParentCheckInventory(viewModel, parentQuantity, 0, userId);
            }
        }

        // Check bundle product inventory
        public virtual void BundleProductParentCheckInventory(ProductViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Add sku of child product in SKU property to check inventory of child product.

            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(viewModel);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;

            decimal selectedQuantity = quantity.GetValueOrDefault();

            decimal cartQuantity = GetOrderedItemQuantity(viewModel.SKU);

            decimal combinedQuantity = selectedQuantity + cartQuantity;

            if (IsNotNull(viewModel?.Quantity))
            {
                if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                {
                    viewModel.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }
            }
        }

        private static List<AttributesSelectValuesViewModel> GetOutOfStockOptionsAttributeListForBundle(AssociatedPublishBundleProductViewModel products)
            => products.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        public virtual void CheckBundleProductInventory(AssociatedPublishBundleProductViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeListForBundle(viewModel);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;
            decimal selectedQuantity = quantity.GetValueOrDefault();
            decimal cartQuantity = GetOrderedItemQuantity(viewModel.SKU);
            decimal combinedQuantity = (selectedQuantity + cartQuantity) * viewModel.AssociatedQuantity;

            ZnodeLogging.LogMessage("selectedQuantity, cartQuantity and combinedQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SelectedQuantity = selectedQuantity, CartQuantity = cartQuantity, CombinedQuantity = combinedQuantity });

            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                {
                    viewModel.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }
            }
            if (IsNotNull(viewModel?.Quantity))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;

                if (inventorySetting?.Count > 0)
                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else if (viewModel.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                    viewModel.ShowAddToCart = true;
                    return;
                }
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                viewModel.ShowAddToCart = true;
            }
            else
            {
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                viewModel.ShowAddToCart = false;
                return;
            }
        }

        /// <summary>
        /// Get price and inventory for multiple SKUs
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public virtual List<ProductPriceViewModel> GetPriceWithInventory(List<ProductPriceViewModel> products)
        {
            List<ProductPriceViewModel> result = new List<ProductPriceViewModel>();
            ParameterInventoryPriceListModel inventoryPriceModels = new ParameterInventoryPriceListModel();
            inventoryPriceModels.parameterInventoryPriceModels = new List<ParameterInventoryPriceModel>();
            int portalId = PortalAgent.CurrentPortal.PortalId;
            foreach (ProductPriceViewModel productPriceView in products)
            {
                inventoryPriceModels.parameterInventoryPriceModels.Add(new ParameterInventoryPriceModel() { SKU = productPriceView.sku, ProductId = productPriceView.PublishProductId, PortalId = portalId, ProductType = productPriceView.type, PriceView = productPriceView.PriceView, ObsoleteClass = productPriceView.ObsoleteClass, MinQuantity = productPriceView.MinQuantity });
            }
            if (products?.Count > 0)
            {
                result.AddRange(_productClient.GetPriceWithInventory(inventoryPriceModels)?.ProductList?.ToViewModel<ProductPriceViewModel>());
            }
            return GetFinalProductPrice(result);
        }

        //Get Ordered Item Quantity.
        public virtual decimal GetOrderedItemQuantity(string sku)
        {
            ShoppingCartModel shoppingCartModel = GetShoppingCartModel();
            decimal? cartQuantity = 0.00M;

            if (shoppingCartModel?.ShoppingCartItems?.Count > 0)
            {
                cartQuantity = (
                                 from ShoppingCartItemModel item in shoppingCartModel.ShoppingCartItems
                                 where !Equals(item, null) && !Equals(item.AddOnProductSKUs, null)
                                 where string.Equals(sku, item.SKU, StringComparison.OrdinalIgnoreCase) || string.Equals(sku, item.ConfigurableProductSKUs, StringComparison.OrdinalIgnoreCase) || item.AddOnProductSKUs.Split(',').Contains(sku) || item.BundleProductSKUs.Split(',').Contains(sku)
                                 select item.Quantity
                               ).Sum();
            }
            return cartQuantity.GetValueOrDefault();
        }

        //Get order quantity of collection of sku
        public virtual List<QuickOrderCartViewModel> GetOrderedItemQuantity(List<string> skus)
        {
            ShoppingCartModel shoppingCartModel = GetShoppingCartModel();
            List<QuickOrderCartViewModel> skuQuantity = new List<QuickOrderCartViewModel>();
            if (shoppingCartModel?.ShoppingCartItems?.Count > 0 && skus?.Count > 0)
            {
                skuQuantity = (
                                 from ShoppingCartItemModel item in shoppingCartModel.ShoppingCartItems
                                 where !Equals(item, null) && !Equals(item.AddOnProductSKUs, null)
                                 where skus.Contains(item.SKU) || skus.Contains(item.ConfigurableProductSKUs) || !string.IsNullOrEmpty(item.AddOnProductSKUs?.Split(',').FirstOrDefault(m=> m?.Length > 0 && skus.Contains(m)))
                                 select new QuickOrderCartViewModel() { Quantity = item.Quantity, SKU = skus.Contains(item.SKU) ? item.SKU : (skus.Contains(item.ConfigurableProductSKUs) ? item.ConfigurableProductSKUs : item.AddOnProductSKUs?.Split(',')?.FirstOrDefault(m => m?.Length > 0 && skus.Contains(m))) }
                               )?.ToList();
                if(skuQuantity?.Count > 0)
                {
                    List<string> duplicateSKUs = skuQuantity.GroupBy(x => x.SKU).Where(g => g.Count() > 1).Select(m => m.Key)?.ToList();
                    if (duplicateSKUs?.Count() > 0)
                    {
                        List<QuickOrderCartViewModel> mergeSKUs = skuQuantity.Where(x => duplicateSKUs.Contains(x.SKU)).GroupBy(x => x.SKU).Select(p => new QuickOrderCartViewModel() { SKU = p.Key, Quantity = p.Sum(m => m.Quantity) } )?.ToList();
                        if(mergeSKUs?.Count > 0)
                        {
                            skuQuantity.RemoveAll(m => duplicateSKUs.Contains(m.SKU));
                            skuQuantity.AddRange(mergeSKUs);
                        }
                    }
                }
            }
            return skuQuantity;
        }

        //Get Ordered Item Quantity.
        protected virtual ShoppingCartModel GetShoppingCartModel()
        {
            ICartAgent cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<AccountQuoteClient>(), GetClient<UserClient>());
            ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                                 cartAgent.GetCartFromCookie();

            if (IsNotNull(shoppingCartModel) && (IsNull(shoppingCartModel?.ShoppingCartItems) || shoppingCartModel?.ShoppingCartItems?.Count == 0))
            {
                var shoppingCartItems = cartAgent.GetCartFromCookie()?.ShoppingCartItems;
                shoppingCartModel.ShoppingCartItems = (shoppingCartItems?.Count > 0) ? shoppingCartItems : new List<ShoppingCartItemModel>(); ;
            }
            return shoppingCartModel;
        }

        private decimal GetGroupProductOrderedItemQuantity(string sku)
        {
            CartViewModel cart = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<AccountQuoteClient>(), GetClient<UserClient>()).GetCart(false, false);
            decimal? cartQuantity = 0.00M;

            if (cart?.ShoppingCartItems?.Count > 0)
            {
                cartQuantity = (
                        from CartItemViewModel item in cart.ShoppingCartItems
                        from groupProduct in item.GroupProducts
                        where !Equals(item, null) && !Equals(item.AddOnProductSKUs, null)
                        where string.Equals(sku, item.SKU, StringComparison.OrdinalIgnoreCase) || string.Equals(sku, item.ConfigurableProductSKUs, StringComparison.OrdinalIgnoreCase) || item.AddOnProductSKUs.Split(',').Contains(sku)
                        || string.Equals(sku, groupProduct.Sku, StringComparison.OrdinalIgnoreCase)
                        select groupProduct.Quantity
                        ).Sum();
            }
            return cartQuantity.GetValueOrDefault();
        }

        //Get list of bundle product.
        public virtual List<BundleProductViewModel> GetBundleProduct(int productId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, productId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, GetCatalogId().GetValueOrDefault().ToString());
            List<BundleProductViewModel> bundleProducts = _productClient.GetBundleProducts(filters).BundleProducts?.ToViewModel<BundleProductViewModel>()?.ToList();

            return bundleProducts?.Count > 0 ? bundleProducts : new List<BundleProductViewModel>();
        }

        //Get list of bundle product.
        public virtual List<GroupProductViewModel> GetGroupProductList(int productId, bool checkInventory = true)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, productId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add(ZnodePortalCatalogEnum.PublishCatalogId.ToString(), FilterOperators.Equals, GetCatalogId().GetValueOrDefault().ToString());

            _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            WebStoreGroupProductListModel groupProducts = _productClient.GetGroupProductList(filters);

            if (groupProducts?.GroupProducts?.Count > 0)
            {
                List<GroupProductViewModel> groupProductList = groupProducts.GroupProducts.ToViewModel<GroupProductViewModel>().ToList();

                if (groupProductList?.Count > 0)
                {
                    foreach (GroupProductViewModel groupProduct in groupProductList)
                    {
                        //assign parent product of all group products.
                        groupProduct.ParentPublishProductId = productId;
                        groupProduct.IsCallForPricing = Convert.ToBoolean(groupProduct.Attributes.Value(ZnodeConstant.CallForPricing));

                        if (checkInventory)
                            //Check inventory of all group products.
                            CheckGroupInventory(groupProduct, Convert.ToDecimal(groupProduct.Attributes?.Value(ZnodeConstant.MinimumQuantity)));

                        if (Equals(groupProduct.RetailPrice, null))
                        {
                            groupProduct.ShowAddToCart = false;
                            groupProduct.InventoryMessage = WebStore_Resources.ErrorPriceNotAssociate;
                        }
                    }
                }

                return groupProductList;
            }
            return new List<GroupProductViewModel>();
        }

        //Get list of product highlist.
        public virtual List<HighlightsViewModel> GetProductHighlights(int productId, string highLightsCodes)
        {
            HighlightListModel highLights = _webstoreProductClient.GetProductHighlights(new ParameterProductModel { HighLightsCodes = highLightsCodes, LocaleId = PortalAgent.LocaleId }, productId, PortalAgent.LocaleId);
            return highLights?.HighlightList?.Count > 0 ? highLights?.HighlightList.ToViewModel<HighlightsViewModel>().ToList() : new List<HighlightsViewModel>();
        }

        //Get list of product highlist.
        public virtual HighlightsViewModel GetHighlightInfo(int highLightId, int productId, string sku)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString()));

            filters.Add(new FilterTuple(FilterKeys.SKU.ToString(), FilterOperators.Equals, sku));
            //Get Highlight by highlightId.
            return _highlightClient.GetHighlight(highLightId, filters, productId).ToViewModel<HighlightsViewModel>();
        }

        //Get list of highlist by Code.
        public virtual HighlightsViewModel GetHighlightInfoByCode(string highLightCode, string sku)
        {
            HighlightsViewModel highlightDetailCache = Helper.GetFromCache<HighlightsViewModel>($"HighlightDetailList{highLightCode.ToLower()}");
            if(IsNull(highlightDetailCache))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString()));

                filters.Add(new FilterTuple(FilterKeys.SKU.ToString(), FilterOperators.Equals, sku));
                //Get Highlight by highlightId.
                HighlightsViewModel highlightsViewModel = _highlightClient.GetHighlightByCode(highLightCode, filters).ToViewModel<HighlightsViewModel>();
                Helper.AddIntoCache(highlightsViewModel, $"HighlightDetailList{highLightCode.ToLower()}", "CurrentPortalCacheDuration");

                return highlightsViewModel;
            }
            return highlightDetailCache;
        }

        //Get list of product Attributes.
        public virtual ConfigurableAttributeViewModel GetProductAttribute(int productId, ParameterProductModel model, List<AttributesViewModel> attribute, bool isDefaultAssoicatedProduct)
        {
            if (productId > 0)
            {
                ConfigurableAttributeViewModel configurableData = new ConfigurableAttributeViewModel();

                //Get configurable attributes.
                ConfigurableAttributeListModel attributes = _productClient.GetProductAttribute(productId, model);

                if (attributes?.Attributes?.Count > 0)
                {
                    //Get the selected configurable attributes.
                    List<AttributesViewModel> viewModel = attributes?.Attributes.ToViewModel<AttributesViewModel>().ToList();
                    foreach (AttributesViewModel configurableAttribute in viewModel)
                    {
                        foreach (ProductAttributesViewModel productAttribute in configurableAttribute.ConfigurableAttribute)
                        {
                            configurableAttribute.SelectedAttributeValue = new[] { (model.SelectedAttributes[configurableAttribute.AttributeCode]) };
                            if (isDefaultAssoicatedProduct && productAttribute.AttributeValue == model.SelectedValue)
                                productAttribute.IsDisabled = true;
                        }
                    }

                    //Remove all configurable attributes and add newly assign configurable attributes.
                    foreach (PublishAttributeModel configurableAttribute in attributes.Attributes)
                        attribute.RemoveAll(x => x.AttributeCode == configurableAttribute.AttributeCode);

                    attribute.AddRange(viewModel);

                    configurableData.ConfigurableAttributes = attribute;

                    return configurableData;
                }
            }
            return null;
        }

        //Get Configurable product.
        public virtual ProductViewModel GetConfigurableProduct(ParameterProductModel model)
        {
            if (model.ParentProductId > 0)
            {
                Dictionary<string, string> SelectedAttributes = GetAttributeValues(model.Codes, model.Values);

                model.LocaleId = PortalAgent.LocaleId;
                model.PublishCatalogId = GetCatalogId().GetValueOrDefault();
                model.PortalId = PortalAgent.CurrentPortal.PortalId;
                model.SelectedAttributes = SelectedAttributes;

                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                PublishProductModel publishProductModel = _productClient.GetConfigurableProduct(model, GetProductExpands());

                if (HelperUtility.IsNotNull(model))
                    if (HelperUtility.IsNotNull(publishProductModel))
                    {
                        ProductViewModel viewModel = publishProductModel.ToViewModel<ProductViewModel>();
                        ParameterProductModel productAttribute = null;
                        ConfigurableAttributeViewModel configurableData = null;

                        //If Product is default configurable product.
                        if (publishProductModel.IsDefaultConfigurableProduct)
                        {
                            //Default product attribute.
                            AttributesViewModel defaultAttribute = viewModel.Attributes?.FirstOrDefault(x => x.IsConfigurable);
                            //Get parameter model.
                            productAttribute = GetConfigurableParameterModel(model.ParentProductId, model.SelectedCode, model.SelectedValue, SelectedAttributes);
                            //Get product aatribute values.
                            configurableData = GetProductAttribute(model.ParentProductId, productAttribute,
                                                  viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute.Count > 0).ToList(), publishProductModel.IsDefaultConfigurableProduct);
                            //Set message id combination does not exist.
                            configurableData.CombinationErrorMessage = WebStore_Resources.ProductCombinationErrorMessage;
                            viewModel.IsDefaultConfigurableProduct = publishProductModel.IsDefaultConfigurableProduct;
                        }
                        else
                        {
                            //Get parameter model.
                            productAttribute = GetConfigurableParameterModel(model.ParentProductId, model.SelectedCode, model.SelectedValue, SelectedAttributes);
                            //Get product aatribute values.
                            configurableData = GetProductAttribute(model.ParentProductId, productAttribute,
                            viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute.Count > 0).ToList(), publishProductModel.IsDefaultConfigurableProduct);
                        }

                        //Map Product configurable product data.
                        MapConfigurableProductData(model.ParentProductId, model.SKU, viewModel, configurableData);

                        Helper.SetProductCartParameter(viewModel);
                        return viewModel;
                    }
            }
            return new ProductViewModel { Attributes = new List<AttributesViewModel>() };
        }
        //Check Group Product Inventory
        public virtual GroupProductViewModel CheckGroupProductInventory(int mainProductId, string productSKU, string quantity)
        {
            if (!string.IsNullOrEmpty(productSKU) && !string.IsNullOrEmpty(quantity))
            {
                //Get sku's and quantity of associated group products.
                string[] groupProducts = productSKU.Split(',');
                string[] groupProductsQuantity = quantity.Split('_');

                if (mainProductId > 0 && groupProducts?.Length > 0 && groupProductsQuantity?.Length > 0)
                {
                    List<GroupProductViewModel> groupProductList = GetGroupProductList(mainProductId, false);
                    if (groupProductList?.Count > 0 && groupProductsQuantity?.Length > 0)
                    {
                        for (int index = 0; index < groupProductsQuantity.Length; index++)
                        {
                            GroupProductViewModel groupProduct = groupProductList.FirstOrDefault(x => x.SKU == groupProducts[index]);
                            if (!string.IsNullOrEmpty(groupProductsQuantity[index]) && HelperUtility.IsNotNull(groupProduct))
                            {
                                decimal groupProductQuantity;
                                decimal.TryParse(groupProductsQuantity[index], out groupProductQuantity);
                                //Check the inventory of group product.
                                CheckGroupInventory(groupProduct, groupProductQuantity);
                            }
                        }

                        if (groupProductList.Where(x => groupProducts.Contains(x.SKU)).Any(x => !x.ShowAddToCart))
                        {
                            string message = string.Empty;
                            List<GroupProductViewModel> outofStockProductList = groupProductList.Where(x => !x.ShowAddToCart && groupProducts.Contains(x.SKU)).ToList();
                            foreach (GroupProductViewModel item in outofStockProductList)
                                message = item.InventoryMessage;

                            return new GroupProductViewModel() { ShowAddToCart = false, InventoryMessage = message };
                        }
                    }
                }
            }
            return new GroupProductViewModel() { ShowAddToCart = true };
        }

        //Add Product for Compare
        public virtual bool GlobalAddProductToCompareList(int productId, int categoryId, out string message, out int errorCode)
        {
            bool isCompare = false;
            message = string.Empty;
            errorCode = 0;
            List<ProductCompareViewModel> compareProductList = new List<ProductCompareViewModel>();
            List<ProductCompareViewModel> products = GetFromSession<List<ProductCompareViewModel>>(ZnodeConstant.CompareProducts);

            PortalViewModel portal = PortalAgent.CurrentPortal;
            //If Product is not available for compare first add for compare.
            if (HelperUtility.IsNull(products))
            {
                if (Equals(portal.ProductCompareType, ZnodeConstant.GlobalLevel))
                {
                    categoryId = 0;
                    CompareProduct(productId, categoryId, compareProductList);
                    isCompare = true;
                }
                else
                {
                    CompareProduct(productId, categoryId, compareProductList);
                    isCompare = true;
                }
            }
            //If Add Product to comparison list.
            else if (HelperUtility.IsNotNull(products) && products.Count < ZnodeConstant.CompareProductLimit)
            {
                bool isProductExists = false;
                bool isCategoryChanged = false;
                compareProductList = products;

                if (Equals(portal.ProductCompareType, ZnodeConstant.GlobalLevel))
                {
                    if (compareProductList.Any(x => x.ProductId == productId))
                    {
                        isProductExists = true;
                        errorCode = Convert.ToInt32(ProductCompareErrorCode.ProductExist);
                    }
                }
                else
                {
                    if (!compareProductList.Any(x => x.CategoryId == categoryId))
                    {
                        isCategoryChanged = true;
                        errorCode = Convert.ToInt32(ProductCompareErrorCode.CategoryChanged);
                    }
                    else if (compareProductList.Any(x => x.ProductId == productId))
                    {
                        isProductExists = true;
                        errorCode = Convert.ToInt32(ProductCompareErrorCode.ProductExist);
                    }
                }

                //Check if category changed or not.
                if (isCategoryChanged)
                    RemoveInSession(ZnodeConstant.CompareProducts);
                else
                {
                    if (!isProductExists)
                    {
                        if (HelperUtility.IsNull(compareProductList))
                            compareProductList = products;

                        CompareProduct(productId, categoryId, compareProductList);
                        isCompare = true;
                    }
                }
            }
            else
                errorCode = Convert.ToInt32(ProductCompareErrorCode.MaxProductLimit);

            message = GetErrorMessage(errorCode);
            return isCompare;
        }

        //Get no of product available in session.
        public virtual bool GetCompareProducts()
        {
            if (GetFromSession<List<ProductCompareViewModel>>(ZnodeConstant.CompareProducts).Count <= 1)
                return true;
            else
                return false;
        }

        //Remove All products from Comparison
        public virtual void DeleteComparableProducts() =>
            RemoveInSession(ZnodeConstant.CompareProducts);

        //Get product details for comparison
        public virtual List<ProductViewModel> GetCompareProductsDetails(bool isProductDetails)
        {
            List<ProductCompareViewModel> products = GetFromSession<List<ProductCompareViewModel>>(ZnodeConstant.CompareProducts);
            if (products?.Count > 0)
            {
                string productIds = string.Join(",", products.Select(x => x.ProductId));

                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                PublishProductListModel productList = _productClient.GetPublishProductList(GetExpandsForProductCompare(isProductDetails), GetRequiredFilters(), null, null, null, new ParameterKeyModel { Ids = productIds });

                List<ProductViewModel> productViewModelList = productList?.PublishProducts?.Count > 0 ? productList?.PublishProducts?.GroupBy(x => x.PublishProductId).Select(y => y.First()).ToViewModel<ProductViewModel>().ToList() : new List<ProductViewModel>();

                if (isProductDetails)
                {
                    foreach (ProductViewModel product in productViewModelList)
                    {
                        Helper.SetProductCartParameter(product);

                        //get the minimum selectable quantity to check inventory.
                        string minimumProductQauntity = product?.Attributes.Value(ZnodeConstant.MinimumQuantity);
                        decimal selectedQauntity = !string.IsNullOrEmpty(minimumProductQauntity) ? Convert.ToDecimal(minimumProductQauntity) : 0;

                        //Check inventory of product.
                        CheckInventory(product, selectedQauntity);

                        product.IsCallForPricing = Convert.ToBoolean(product.Attributes?.Value(ZnodeConstant.CallForPricing)) || (product.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();

                        //Get add on product skus and Check inventory of each add on.
                        string addonSKu = string.Join(",", product.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.First().SKU));
                        if (!string.IsNullOrEmpty(addonSKu))
                            CheckAddOnInvenTory(product, addonSKu, selectedQauntity);

                        if (!Equals(product.ProductType, ZnodeConstant.GroupedProduct) && !Equals(product.ProductType, ZnodeConstant.ConfigurableProduct))
                        {
                            GetProductFinalPrice(product, product.AddOns, selectedQauntity, addonSKu);

                            if (HelperUtility.IsNull(product.ProductPrice))
                            {
                                product.ShowAddToCart = false;
                                product.InventoryMessage = WebStore_Resources.ErrorPriceNotAssociate;
                            }
                        }
                    }
                }
                return productViewModelList;
            }
            else
                return new List<ProductViewModel>();
        }

        //Send compare Product mail.
        public virtual bool SendComparedProductMail(ProductCompareViewModel viewModel)
        {
            viewModel.CatalogId = GetCatalogId().GetValueOrDefault();
            viewModel.LocaleId = PortalAgent.LocaleId;
            viewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            viewModel.IsShowPriceAndInventoryToLoggedInUsersOnly = GlobalAttributeHelper.IsShowPriceAndInventoryToLoggedInUsersOnly();
            viewModel.WebstoreDomainName = HttpContext.Current.Request.Url.Authority;
            viewModel.WebstoreDomainScheme = HttpContext.Current.Request.Url.Scheme;
            List<ProductCompareViewModel> products = GetFromSession<List<ProductCompareViewModel>>(ZnodeConstant.CompareProducts);
            viewModel.ProductIds = string.Join(",", products.Select(x => x.ProductId));
            return _productClient.SendComparedProductMail(viewModel.ToModel<ProductCompareModel>());
        }

        //Send compare Product mail.
        public virtual bool SendMailToFriend(EmailAFriendViewModel viewModel)
        {
            viewModel.CatalogId = GetCatalogId().GetValueOrDefault();
            viewModel.LocaleId = PortalAgent.LocaleId;
            viewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            return _productClient.SendEmailToFriend(viewModel.ToModel<EmailAFriendListModel>());
        }

        //Remove Single product from comparison
        public virtual bool RemoveProductFormSession(int productId)
        {
            bool isCompare = false;
            List<ProductCompareViewModel> comparableProducts = GetFromSession<List<ProductCompareViewModel>>(ZnodeConstant.CompareProducts);
            if (comparableProducts?.Count != 0)
            {
                foreach (ProductCompareViewModel item in comparableProducts)
                {
                    if (item.ProductId == productId)
                    {
                        comparableProducts.Remove(item);
                        isCompare = true;
                        break;
                    }
                }
            }
            return isCompare;
        }

        //Get List OF Recently View Product
        public virtual List<RecentViewModel> GetRecentProductList(int productId)
        {
            List<RecentViewModel> storedValues = new List<RecentViewModel>();
            List<RecentViewModel> recentViewProductCookie = GetFromSession<List<RecentViewModel>>(ZnodeConstant.RecentViewProduct);

            if (recentViewProductCookie != null || recentViewProductCookie?.Count > 0)
            {
                List<RecentViewModel> deserializedObject = GetFromSession<List<RecentViewModel>>(ZnodeConstant.RecentViewProduct);
                if (deserializedObject != null && deserializedObject.Any())
                {
                    PublishProductClient client = new PublishProductClient();

                    List<RecentViewProductModel> productData = client.GetActiveProducts(string.Join(",", deserializedObject.Select(x => x.PublishProductId).ToList()), GetCatalogId().GetValueOrDefault(), PortalAgent.LocaleId, 0);

                    if (HelperUtility.IsNotNull(productData) && productData.Any())
                    {
                        deserializedObject.RemoveAll(x => !string.Join(",", productData?.Select(y => y.ZnodeProductId.ToString())).Contains(x.PublishProductId.ToString()));

                        foreach (RecentViewModel item in deserializedObject)
                        {
                            if (productData.Where(x => x.ZnodeProductId == item.PublishProductId).Count() > 0 && item.PublishProductId != productId)
                            {
                                item.HighlightsList = _webstoreHelper.GetHighlightListFromAttributes(item.Attributes, item.SKU, item.PublishProductId);
                                item.ImageSmallPath = HttpUtility.UrlDecode(item.ImageSmallPath);
                                storedValues.Add(item);
                            }
                        }
                    }
                    if (PortalAgent.CurrentPortal.IsAddToCartOptionForProductSlidersEnabled)
                    {
                        if (HelperUtility.IsNotNull(storedValues) && storedValues.Count > 0)
                        {
                            storedValues.ForEach(y =>
                            {
                                y.ProductViewModel = GetProduct(y.PublishProductId);
                                y.ProductViewModel.IsAddToCartOptionForProductSlidersEnabled = true;
                            });
                        }
                    }                               
                }
            }
            return storedValues;
        }

        //Get Product Details For Review.
        public virtual ProductReviewViewModel GetProductForReview(int productID, string productName, decimal? rating)
        {
            _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            PublishProductModel model = _productClient.GetPublishProduct(productID, GetRequiredFilters(), new ExpandCollection { ExpandKeys.SEO });
            if (HelperUtility.IsNotNull(model))
            {
                //Updated rating added againest that product.
                List<RecentViewModel> recentViewProductCookie = GetFromSession<List<RecentViewModel>>(ZnodeConstant.RecentViewProduct);
                if (IsNotNull(recentViewProductCookie))
                {
                    foreach (RecentViewModel recentViewModel in recentViewProductCookie.Where(x => x.PublishProductId == productID))
                    {
                        recentViewModel.Rating = Convert.ToDecimal(rating);
                    }
                    SaveInSession(ZnodeConstant.RecentViewProduct, recentViewProductCookie);
                }
                ProductReviewViewModel viewModel = model.ToViewModel<ProductReviewViewModel>();
                if (model.ConfigurableProductId > 0)
                {
                    viewModel.PublishProductId = model.ConfigurableProductId;
                    viewModel.ProductName = model.ParentConfiguarableProductName;
                }
                return viewModel;
            }
            else
                return new ProductReviewViewModel { PublishProductId = productID, ProductName = productName };
        }

        //Get product url by product sku.
        public virtual string GetProductUrl(string productSKU, UrlHelper urlHelper)
        {
            if (!string.IsNullOrEmpty(productSKU))
            {
                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                ProductViewModel productModel = _productClient.GetPublishProductBySKU(new ParameterProductModel { SKU = productSKU }, GetProductExpands(), GetRequiredFilters())?.ToViewModel<ProductViewModel>();
                if (HelperUtility.IsNotNull(productModel))
                {
                    if (!string.IsNullOrEmpty(productModel.SEOUrl))
                        return urlHelper.RouteUrl("SeoSlug", new { slug = productModel.SEOUrl.ToLower() });

                    return urlHelper.RouteUrl("product-details", new { id = productModel.ConfigurableProductId > 0 ? productModel.ConfigurableProductId : productModel.PublishProductId });
                }
            }
            return string.Empty;
        }

        public virtual void CheckAddOnInvenTory(ProductViewModel model, string addOnIds, decimal quantity)
        {
            string[] selectedAddOn = !string.IsNullOrEmpty(addOnIds) ? addOnIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
            bool AllowBackOrder = false;
            bool TrackInventory = false;

            if (selectedAddOn?.Length > 0)
            {
                foreach (string addOnSKU in selectedAddOn)
                {
                    AddOnViewModel addOn = null;
                    if (!string.IsNullOrEmpty(addOnSKU))
                        addOn =
                            model.AddOns.FirstOrDefault(
                                x => x.AddOnValues.Any(y => y.SKU == addOnSKU));

                    if (HelperUtility.IsNotNull(addOn))
                    {
                        AddOnValuesViewModel addOnValue = addOn.AddOnValues.FirstOrDefault(y => y.SKU == addOnSKU);

                        if (HelperUtility.IsNotNull(addOnValue))
                        {
                            decimal selectedQuantity = quantity > 0 ? quantity : Convert.ToDecimal(model.Attributes?.Value(ZnodeConstant.MinimumQuantity));

                            decimal cartQuantity = GetOrderedItemQuantity(addOnSKU);

                            decimal combinedQuantity = selectedQuantity + cartQuantity;

                            List<AttributesSelectValuesViewModel> inventorySetting = addOnValue.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

                            if (inventorySetting?.Count > 0)
                            {
                                string inventorySettingCode = inventorySetting?.FirstOrDefault()?.Code;

                                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    model.InventoryMessage = !string.IsNullOrEmpty(model.InStockMessage) ? model.InStockMessage : WebStore_Resources.TextInstock;
                                    model.ShowAddToCart = true;
                                    continue;
                                }
                                TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

                                if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                                {
                                    model.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity), addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                                    model.ShowAddToCart = false;
                                    return;
                                }

                                if ((addOnValue.Quantity < combinedQuantity || HelperUtility.IsNull(addOnValue.Quantity)) && !AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryMessage = WebStore_Resources.TextOutOfStockAddon;
                                    model.ShowAddToCart = false;
                                    addOn.IsOutOfStock = true;
                                    addOn.InventoryMessage = model.InventoryMessage;
                                    return;
                                }

                                if (HelperUtility.IsNull(addOnValue.Quantity) && AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryMessage = WebStore_Resources.TextOutOfStockAddon;
                                    model.ShowAddToCart = false;
                                    addOn.IsOutOfStock = true;
                                    addOn.InventoryMessage = model.InventoryMessage;
                                    return;
                                }
                                else if (addOnValue.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryMessage = !string.IsNullOrEmpty(model.BackOrderMessage) ? model.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                                    model.ShowAddToCart = true;
                                    continue;
                                }
                            }
                            return;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(model.InventoryMessage) && model.ShowAddToCart)
            {
                model.InventoryMessage = !string.IsNullOrEmpty(model.InStockMessage) ? model.InStockMessage : WebStore_Resources.TextInstock;
                model.ShowAddToCart = true;
            }
        }

        public virtual void CheckAddOnInventory(ShortProductViewModel model, string addOnIds, decimal quantity)
        {
            string[] selectedAddOn = !string.IsNullOrEmpty(addOnIds) ? addOnIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
            bool AllowBackOrder = false;
            bool TrackInventory = false;

            if (selectedAddOn?.Length > 0)
            {
                foreach (string addOnSKU in selectedAddOn)
                {
                    AddOnViewModel addOn = null;
                    if (!string.IsNullOrEmpty(addOnSKU))
                        addOn =
                            model.AddOns.FirstOrDefault(
                                x => x.AddOnValues.Any(y => y.SKU == addOnSKU));

                    if (HelperUtility.IsNotNull(addOn))
                    {
                        AddOnValuesViewModel addOnValue = addOn.AddOnValues.FirstOrDefault(y => y.SKU == addOnSKU);

                        if (HelperUtility.IsNotNull(addOnValue))
                        {
                            decimal selectedQuantity = quantity > 0 ? quantity : Convert.ToDecimal(model.Attributes?.Value(ZnodeConstant.MinimumQuantity));

                            decimal cartQuantity = GetOrderedItemQuantity(addOnSKU);

                            decimal combinedQuantity = selectedQuantity + cartQuantity;

                            List<AttributesSelectValuesViewModel> inventorySetting = addOnValue.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

                            if (inventorySetting?.Count > 0)
                            {
                                string inventorySettingCode = inventorySetting?.FirstOrDefault()?.Code;

                                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    model.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(model.StoreSettings.InStockMessage) ? model.StoreSettings.InStockMessage : WebStore_Resources.TextInstock;
                                    model.InventoryDetails.ShowAddToCart = true;
                                    continue;
                                }
                                TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

                                if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                                {
                                    model.InventoryDetails.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity), addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                                    model.InventoryDetails.ShowAddToCart = false;
                                    return;
                                }

                                if ((addOnValue.Quantity < combinedQuantity || HelperUtility.IsNull(addOnValue.Quantity)) && !AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryDetails.InventoryMessage = WebStore_Resources.TextOutOfStockAddon;
                                    model.InventoryDetails.ShowAddToCart = false;
                                    addOn.IsOutOfStock = true;
                                    addOn.InventoryMessage = model.InventoryDetails.InventoryMessage;
                                    return;
                                }

                                if (HelperUtility.IsNull(addOnValue.Quantity) && AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryDetails.InventoryMessage = WebStore_Resources.TextOutOfStockAddon;
                                    model.InventoryDetails.ShowAddToCart = false;
                                    addOn.IsOutOfStock = true;
                                    addOn.InventoryMessage = model.InventoryDetails.InventoryMessage;
                                    return;
                                }
                                else if (addOnValue.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(model.StoreSettings.BackOrderMessage) ? model.StoreSettings.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                                    model.InventoryDetails.ShowAddToCart = true;
                                    continue;
                                }
                            }
                            return;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(model.InventoryDetails.InventoryMessage) && model.InventoryDetails.ShowAddToCart)
            {
                model.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(model.StoreSettings.InStockMessage) ? model.StoreSettings.InStockMessage : WebStore_Resources.TextInstock;
                model.InventoryDetails.ShowAddToCart = true;
            }
        }

        //Check inventory for product.
        public virtual void CheckInventory(ProductViewModel viewModel, decimal? quantity)
        {
            bool isDisplayVariantsOnGrid = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.DisplayVariantsOnGrid)) && viewModel.IsConfigurable;
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(viewModel);
            string inventorySettingCode = inventorySetting?.FirstOrDefault()?.Code;
            decimal combinedQuantity;

            if (!ValidateMinMaxQuantity(viewModel, quantity, out combinedQuantity))
            {
                viewModel.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                viewModel.ShowAddToCart = false;
                return;
            }

            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase) && !Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct) && !isDisplayVariantsOnGrid)
            {
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                viewModel.ShowAddToCart = true;
                return;
            }

            if (HelperUtility.IsNotNull(viewModel) && isDisplayVariantsOnGrid)
            {
                ValidateInventoryForConfigurableProducts(viewModel, combinedQuantity);
                return;
            }
            if (HelperUtility.IsNotNull(viewModel) && HelperUtility.IsNotNull(viewModel.Quantity))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;


                if (inventorySetting?.Count > 0)
                {
                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                    if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory && !Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct))
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                        viewModel.ShowAddToCart = false;
                        return;
                    }
                    else if (AllowBackOrder && TrackInventory && viewModel?.Quantity <= 0)
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                        viewModel.ShowAddToCart = true;
                        return;
                    }
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                }
            }
            else if (HelperUtility.IsNotNull(viewModel) && (Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct) || isDisplayVariantsOnGrid))
            {
                viewModel.InventoryMessage = string.Empty;
                viewModel.ShowAddToCart = true;
            }
            else
            {
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                viewModel.ShowAddToCart = false;
                return;
            }
        }

        //Check inventory for product.
        public virtual void CheckInventory(ShortProductViewModel viewModel, decimal? quantity)
        {
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(viewModel);
            string inventorySettingCode = inventorySetting?.FirstOrDefault()?.Code;

            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase) && !Equals(viewModel.MiscellaneousDetails.ProductType, ZnodeConstant.GroupedProduct))
            {
                viewModel.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(viewModel.StoreSettings.InStockMessage) ? viewModel.StoreSettings.InStockMessage : WebStore_Resources.TextInstock;
                viewModel.InventoryDetails.ShowAddToCart = true;
                return;
            }
            if (HelperUtility.IsNotNull(viewModel) && HelperUtility.IsNotNull(viewModel.InventoryDetails.Quantity))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;
                decimal selectedQuantity = quantity.GetValueOrDefault();

                string sku = string.IsNullOrEmpty(viewModel.ConfigurableProductSKU) ? viewModel.SKU : viewModel.ConfigurableProductSKU;

                decimal cartQuantity = GetOrderedItemQuantity(sku);

                decimal combinedQuantity = selectedQuantity + cartQuantity;

                if (inventorySetting?.Count > 0)
                {
                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                    if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                    {
                        viewModel.InventoryDetails.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                        viewModel.InventoryDetails.ShowAddToCart = false;
                        return;
                    }

                    if (viewModel.InventoryDetails.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory && !Equals(viewModel.MiscellaneousDetails.ProductType, ZnodeConstant.GroupedProduct))
                    {
                        viewModel.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(viewModel.StoreSettings.OutOfStockMessage) ? viewModel.StoreSettings.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                        viewModel.InventoryDetails.ShowAddToCart = false;
                        return;
                    }
                    else if (viewModel.InventoryDetails.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                    {
                        viewModel.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(viewModel.StoreSettings.BackOrderMessage) ? viewModel.StoreSettings.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;
                        viewModel.InventoryDetails.ShowAddToCart = true;
                        return;
                    }
                    viewModel.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(viewModel.StoreSettings.InStockMessage) ? viewModel.StoreSettings.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.InventoryDetails.ShowAddToCart = true;
                }
            }
            else if (HelperUtility.IsNotNull(viewModel) && (Equals(viewModel.MiscellaneousDetails.ProductType, ZnodeConstant.GroupedProduct)))
            {
                viewModel.InventoryDetails.InventoryMessage = string.Empty;
                viewModel.InventoryDetails.ShowAddToCart = true;
            }
            else
            {
                viewModel.InventoryDetails.InventoryMessage = !string.IsNullOrEmpty(viewModel.StoreSettings.OutOfStockMessage) ? viewModel.StoreSettings.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                viewModel.InventoryDetails.ShowAddToCart = false;
                return;
            }
        }

        //Get message for group product.
        public virtual string GetGroupProductMessage(List<GroupProductViewModel> list)
        {
            List<decimal?> priceList = new List<decimal?>();
            //Add sales price and retail price in list.
            priceList.AddRange(list.Where(x => x.SalesPrice != null)?.Select(y => y.SalesPrice));
            priceList.AddRange(list.Where(x => x.RetailPrice != null)?.Select(y => y.RetailPrice));
            //Order list in asending order.
            decimal? price = priceList.OrderBy(x => x.Value).FirstOrDefault();
            //Currency code for price format.
            string currencyCode = list.FirstOrDefault(x => x.SalesPrice == price || x.RetailPrice == price)?.CultureCode;

            return string.Format(WebStore_Resources.GroupProductMessage, Helper.FormatPriceWithCurrency(price, currencyCode));
        }

        //Gets the breadcrumb string.
        public virtual string GetBreadCrumb(int categoryId, string[] productAssociatedCategoryIds, bool checkFromSession)
        {
            //Generate a cache key based on the parameter.
            string cacheKey = string.Concat("ProductBreadCrumb{0},{1},{2}", categoryId,(productAssociatedCategoryIds != null && productAssociatedCategoryIds?.Count() > 0) ? string.Join(",", productAssociatedCategoryIds):"", checkFromSession);

            if (HelperUtility.IsNull(HttpRuntime.Cache[cacheKey]))
            {
                string breadCrumb = GenerateBreadCrumb(categoryId,productAssociatedCategoryIds, checkFromSession);
                Helper.AddIntoCache(breadCrumb, cacheKey, "CurrentPortalCacheDuration");
                return breadCrumb;
            }
            else
                return Helper.GetFromCache<string>(cacheKey);
        }

        //Generate the breadcrumb based on the parameter.
        protected virtual string GenerateBreadCrumb(int categoryId, string[] productAssociatedCategoryIds, bool checkFromSession)
        {
            if (checkFromSession)
            {
                categoryId = GetFromSession<int>(string.Format(WebStoreConstants.LastSelectedCategoryForPortal, PortalAgent.CurrentPortal.PortalId));
                if ((!productAssociatedCategoryIds?.Contains(categoryId.ToString())).GetValueOrDefault())
                    categoryId = Convert.ToInt32(productAssociatedCategoryIds[0]);
                else if (HelperUtility.IsNull(productAssociatedCategoryIds))
                    return $"<a href='/' {WebStore_Resources.LinkHomeIcon}></a>";
            }
            FilterCollection filters = GetRequiredFilters();
            filters.Add(WebStoreEnum.IsGetParentCategory.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.IsBindImage.ToString(), FilterOperators.Equals, ZnodeConstant.FalseValue);
            CategoryViewModel category = _publishCategoryClient.GetPublishCategory(categoryId, filters, new ExpandCollection { ZnodeConstant.SEO })?.ToViewModel<CategoryViewModel>();
            return $"<a href='/' {WebStore_Resources.LinkHomeIcon}></a> {GetBreadCrumbHtml(category)}";
        }

        //Get see more string from breadcrumb url.
        public virtual string GetSeeMoreString(string breadCrumb)
        {
            string seeMore = breadCrumb.Substring(breadCrumb.LastIndexOf("<a"));
            seeMore = string.IsNullOrEmpty(seeMore) ? string.Empty : seeMore.Insert(seeMore.IndexOf(">") + 1, WebStore_Resources.TextSeeMore);
            seeMore = seeMore.Insert(seeMore.IndexOf(">"), WebStore_Resources.SeeMoreClass);
            return seeMore;
        }

        public virtual List<ProductPriceViewModel> GetProductPrice(List<ProductPriceViewModel> products)
        {
            List<ProductPriceViewModel> result = new List<ProductPriceViewModel>();
            if (products?.Count > 0)
            {
                //Get price for products through ajax async call.
                result.AddRange(GroupedProducts(products.FindAll(x => x.type == ZnodeConstant.GroupedProduct).ToList())?.ToViewModel<ProductPriceViewModel>());
                result.AddRange(ConfigurableProducts(products.FindAll(x => x.type == ZnodeConstant.ConfigurableProduct).ToList())?.ToViewModel<ProductPriceViewModel>());
                result.AddRange(OtherProducts(products)?.ToViewModel<ProductPriceViewModel>());

                foreach (ProductPriceViewModel product in result)
                {
                    product.DisplaySalesPrice = Helper.FormatPriceWithCurrency(product.SalesPrice, product.CurrencyCode);
                    product.DisplayRetailPrice = Helper.FormatPriceWithCurrency(product.RetailPrice, product.CurrencyCode);
                }
            }
            return result;
        }

        //Get all product reviews.
        public virtual ProductAllReviewListViewModel GetAllReviews(int productId, string sortingChoice, int? pageSize, int pageNo)
        {
            FilterCollection filters;
            SortCollection sorts;
            GetFilterAndSortForProductReviews(productId, ref sortingChoice, ref pageSize, ref pageNo, out filters, out sorts);

            CustomerReviewListModel userReviews = _reviewClient.GetCustomerReviewList(Convert.ToString(PortalAgent.LocaleId), null, filters, sorts, pageNo, pageSize);
            ProductAllReviewListViewModel viewModel = new ProductAllReviewListViewModel();
            viewModel.ProductId = productId;

            if (userReviews?.CustomerReviewList?.Count > 0)
            {
                viewModel.ProductName = userReviews.CustomerReviewList.FirstOrDefault()?.ProductName;
                viewModel.AllReviewsList = userReviews.CustomerReviewList.ToViewModel<ProductReviewViewModel>().ToList();
                viewModel.TotalPages = userReviews.TotalPages.GetValueOrDefault();
            }
            return viewModel;
        }



        //Get the SEO Details for the Publish Product, Publish Category and Content Pages.
        public virtual SEOViewModel GetSEODetails(int itemId, string seoCode)
        {
            var currentPortal = PortalAgent.CurrentPortal;
            if (string.IsNullOrEmpty(seoCode))
                seoCode = "NaN";
            var _object = GetClient<SEOSettingClient>();
            SEOViewModel seoDetailViewModel = new SEOViewModel();
            if (itemId > 0)
                seoDetailViewModel = _object.GetPublishSEODetail(itemId, ZnodeConstant.Product, currentPortal.LocaleId, currentPortal.PortalId, seoCode)?.ToViewModel<SEOViewModel>();

            return seoDetailViewModel;
        }

        //Get product details by Sku.
        public virtual AutoComplete GetProductDetailsBySKU(string sku)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                ISearchAgent searchAgent = GetService<ISearchAgent>();
                int pageNumber = 1;
                int pageSize = 1;
                SearchRequestViewModel searchRequestModel = SetSearchRequestModel(sku, pageNumber, pageSize);
                PublishProductModel product = _productClient.GetProductDetailsBySKU(searchAgent.GetKeywordSearchModel(searchRequestModel), GetRequiredFilters(), GetProductInventoryExpands(), searchAgent.GetSortForSearch(searchRequestModel.Sort));

                return HelperUtility.IsNotNull(product) ? MapProductDetails(product) : new AutoComplete();
            }
            return null;
        }

        //Get products if matches Search Term.
        public virtual ProductListViewModel GetProductSearch(string searchTerm, bool enableSpecificSearch = false, int pageNum = 1, int pageSize = 1, int sortValue = 0)
        {
            ISearchAgent searchAgent = DependencyResolver.Current.GetService<ISearchAgent>();
            ProductListViewModel productList = new ProductListViewModel();
            try
            {
                FilterCollection filters = GetRequiredFilters();
                if (enableSpecificSearch)
                {
                    SetUPCProductAttributeFilter(filters, searchTerm);
                    searchTerm = string.Empty;
                }
                SearchRequestViewModel searchRequestModel = SetSearchRequestModel(searchTerm, pageNum, pageSize);
                KeywordSearchModel searchResult = searchAgent.FullTextSearch(searchRequestModel, new ExpandCollection(), filters, searchAgent.GetSortForSearch(searchRequestModel.Sort));
                MapSearchProducts(searchResult, productList);
                productList.Products?.ForEach(x =>
                {
                    x.HighlightLists = _webstoreHelper.GetHighlightListFromAttributes(x.Attributes, x.SKU, x.PublishProductId);
                });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return productList;
        }

        //Set search request model properties.
        public virtual SearchRequestViewModel SetSearchRequestModel(string searchtearm, int pageNum, int pageSize)
        {
            SearchRequestViewModel requestModel = new SearchRequestViewModel();
            requestModel.Category = "";
            requestModel.CatalogId = GetCatalogId().GetValueOrDefault();
            requestModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            requestModel.CategoryId = 0;
            requestModel.LocaleId = PortalAgent.LocaleId;
            requestModel.ProfileId = Helper.GetProfileId();
            requestModel.PageNumber = pageNum;
            requestModel.PageSize = pageSize;
            requestModel.SearchTerm = searchtearm;
            requestModel.FilterList = new Dictionary<string, List<string>>();
            requestModel.UseSuggestion = false;
            requestModel.BrandName = "";
            requestModel.IsFacetList = false;
            requestModel.Sort = 0;
            return requestModel;
        }

        //This method is Add valid SKUs to Quick Order Grid
        public virtual QuickOrderViewModel AddProductsToQuickOrderUsingExcel(HttpPostedFileBase filename)
        {
            string mutipleItems = string.Empty;

            DataTable dataSKU = HelperMethods.GetImportDetails(filename);

            DataColumnCollection columns = dataSKU.Columns;
            //need to check use
            if (!columns.Contains("ExternalId"))
            {
                dataSKU.Columns.Add("ExternalId", typeof(string));
            }

            QuickOrderViewModel quickOrderModel = new QuickOrderViewModel() { ProductSKUText = mutipleItems };

            try
            {

                List<QuickOrderCartViewModel> quickOrderCartViewModels = ExtractValidSKUsUsingDataTable(dataSKU);

                ValidateProductSKUs(MergeDuplicateSKUs(quickOrderCartViewModels), quickOrderModel);
                if (quickOrderModel.ValidSKUCount > 0)
                {
                    RemoveValidProductSKUs(quickOrderCartViewModels, quickOrderModel, dataSKU);
                }
                else
                {
                    List<string> skuList = quickOrderCartViewModels.Select(x => x.SKU)?.ToList();
                    quickOrderModel.ProductSKUText = string.Join(Environment.NewLine, skuList);
                }
                if (quickOrderModel.ProductDetail.Count > 50)
                {
                    quickOrderModel.ProductDetail.RemoveRange(50, quickOrderModel.ProductDetail.Count - 50);
                    quickOrderModel.ValidSKUCount = 50;
                }
                SetQuickOrderNotification(quickOrderModel);
                return quickOrderModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return quickOrderModel;
            }


        }
        //This method remove valid skus from item number field
        protected void RemoveValidProductSKUs(List<QuickOrderCartViewModel> quickOrderCartViewModels, QuickOrderViewModel quickOrderModel, DataTable dataSKU)
        {
            if (quickOrderModel?.ProductDetail?.Count > 0)
            {
                List<string> skus = quickOrderModel.ProductDetail.Select(m => m.SKU)?.ToList();
                List<string> skuList = quickOrderCartViewModels.Select(x => x.SKU)?.ToList();
                if (skus?.Count > 0 && skuList?.Count > 0)
                {
                    skuList.RemoveAll(m => skus.Any(x => x?.Length > 0 && m.Equals(x)) || m == String.Empty);
                }
                quickOrderModel.ProductSKUText = string.Join(Environment.NewLine, skuList);
            }
        }
        //This method is for extract sku in valid form
        protected virtual List<QuickOrderCartViewModel> ExtractValidSKUsUsingDataTable(DataTable dtSKUs)
        {
            List<QuickOrderCartViewModel> model = new List<QuickOrderCartViewModel>();

            foreach (DataRow row in dtSKUs.Rows)
            {
                decimal qty = 1;
                bool isNumeric = decimal.TryParse((row.ItemArray.Count() > 1 ? Convert.ToString(row.ItemArray[1]) : "1"), out qty);
                if (!string.IsNullOrEmpty(Convert.ToString(row.ItemArray[0])))
                {
                    model.Add(new QuickOrderCartViewModel() { SKU = Convert.ToString(row.ItemArray[0]), Quantity = (isNumeric ? Convert.ToDecimal(qty) : 1) });
                }
            }
            return model;
        }
        //Set SKU filter value.
        protected void SetUPCProductAttributeFilter(FilterCollection filters, string productAttributeValue)
        {
            if (IsNotNull(productAttributeValue) && IsNotNull(filters))
            {
                if (filters.Exists(x => string.Equals(x.Item1, FilterKeys.UPC.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    //If SKU is already present in filters, remove it.
                    filters.RemoveAll(x => string.Equals(x.Item1, FilterKeys.UPC.ToString(), StringComparison.InvariantCultureIgnoreCase));

                    //Add New productAttributeValue into filters.
                    filters.Add(new FilterTuple(FilterKeys.UPC, FilterOperators.Equals, productAttributeValue));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.UPC, FilterOperators.Equals, productAttributeValue));
            }
        }

        #region B2B Theme
        //check if the product is added in wishlist for the logged in user. If so, binds its wishlistId
        protected virtual void BindProductWishListDetails(ProductViewModel viewModel)
        {
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
            if (userViewModel?.WishList?.Count > 0)
            {
                viewModel.IsAddedInWishList = userViewModel.WishList.Any(x => x.SKU == viewModel.SKU);
                if (viewModel.IsAddedInWishList)
                    viewModel.WishListId = userViewModel.WishList.FirstOrDefault(x => x.SKU == viewModel.SKU).UserWishListId;
            }
        }

        protected virtual void BindProductWishListDetails(ShortProductViewModel viewModel)
        {
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
            if (userViewModel?.WishList?.Count > 0)
            {
                viewModel.IsAddedInWishList = userViewModel.WishList.Any(x => x.SKU == viewModel.SKU);
                if (viewModel.IsAddedInWishList)
                    viewModel.WishListId = userViewModel.WishList.FirstOrDefault(x => x.SKU == viewModel.SKU).UserWishListId;
            }
        }

        //Get Product details by product id.
        public virtual ProductInventoryDetailViewModel GetProductInventory(int productID, bool isAllLocationsInventoryFlag)
        {
            if (productID > 0)
            {
                //set user id for profile base pricing.
                _productClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

                _productClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
                _productClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
                _productClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                FilterCollection productFilters = GetRequiredFilters();
                SetAllLocationInventoryFilter(productFilters, isAllLocationsInventoryFlag);
                ProductInventoryDetailModel model = _productClient.GetProductInventory(productID, productFilters);
                if (HelperUtility.IsNotNull(model))
                {
                    ProductInventoryDetailViewModel viewModel = model.ToViewModel<ProductInventoryDetailViewModel>();
                    return viewModel;
                }
            }
            throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorProductNotFound);
        }

        // Get associated Configurable products.
        public virtual List<ConfigurableProductViewModel> GetAssociatedConfigurableVariants(int productId)
        {
            PublishProductListModel publishProductListModel = _productClient.GetAssociatedConfigurableVariants(productId);
            List<ConfigurableProductViewModel> configurableProductViewModelList = publishProductListModel?.ConfigurableProducts?.Count > 0 ? publishProductListModel?.ConfigurableProducts?.ToViewModel<ConfigurableProductViewModel>().ToList() : new List<ConfigurableProductViewModel>();
            foreach (ConfigurableProductViewModel product in configurableProductViewModelList)
            {
                string minQuantity = product.ProductAttributes?.Value(ZnodeConstant.MinimumQuantity);
                decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);
                ProductViewModel productViewModel = product.ToModel<ProductViewModel, ConfigurableProductViewModel>();
                productViewModel.Attributes = product.ProductAttributes;
                CheckInventory(productViewModel, quantity);
                product.InventoryMessage = productViewModel.InventoryMessage;
                product.ShowAddToCart = productViewModel.ShowAddToCart;
            }
            return configurableProductViewModelList;
        }

        // Submit Stock Request.
        public virtual bool SubmitStockRequest(StockNotificationViewModel stockNotificationViewModel)
        {
            if(IsNotNull(stockNotificationViewModel))
            {
                stockNotificationViewModel.PortalID = PortalAgent.CurrentPortal.PortalId;
                stockNotificationViewModel.CatalogId = PortalAgent.CurrentPortal.PublishCatalogId;
                return _productClient.SubmitStockRequest(stockNotificationViewModel?.ToModel<StockNotificationModel>());
            }
            return false;
        }

        // Check configurable child product inventory.
        public virtual ConfigurableProductViewModel CheckConfigurableChildProductInventory(int parentProductId, string childSKUs, string childQuantities)
        {
            if (!string.IsNullOrEmpty(childSKUs) && !string.IsNullOrEmpty(childQuantities))
            {
                // Get skus and quantity of associated configurable products.
                string[] childProductSKUs = childSKUs.Split(',');
                string[] childProductQuantities = childQuantities.Split('_');

                if (parentProductId > 0 && childProductSKUs?.Length > 0 && childProductQuantities?.Length > 0)
                {
                    PublishProductListModel publishProductListModel = _productClient.GetAssociatedConfigurableVariants(parentProductId);
                    List<ConfigurableProductViewModel> childProductList = publishProductListModel?.ConfigurableProducts?.Count > 0 ? publishProductListModel?.ConfigurableProducts?.ToViewModel<ConfigurableProductViewModel>().ToList() : new List<ConfigurableProductViewModel>();
                    if (childProductList?.Count > 0 && childProductQuantities?.Length > 0)
                    {
                        for (int index = 0; index < childProductQuantities.Length; index++)
                        {
                            ConfigurableProductViewModel childProduct = childProductList?.FirstOrDefault(x => x.SKU == childProductSKUs[index]);
                            if (!string.IsNullOrEmpty(childProductQuantities[index]) && HelperUtility.IsNotNull(childProduct))
                            {
                                decimal configurableChildProductQuantity;
                                decimal.TryParse(childProductQuantities[index], out configurableChildProductQuantity);
                                ProductViewModel productViewModel = childProduct?.ToModel<ProductViewModel, ConfigurableProductViewModel>();
                                productViewModel.Attributes = childProduct?.ProductAttributes;
                                // Check the inventory of associated configurable product.
                                CheckInventory(productViewModel, configurableChildProductQuantity);
                                childProduct.InventoryMessage = productViewModel?.InventoryMessage;
                                childProduct.ShowAddToCart = productViewModel.ShowAddToCart;
                            }
                        }

                        // Check inventory message for associated configurable product.
                        string inventoryMessage = InventoryMessageForConfigurableProducts(childProductList, childProductSKUs);
                        if(!string.IsNullOrEmpty(inventoryMessage))
                        {
                            return new ConfigurableProductViewModel() { ShowAddToCart = false, InventoryMessage = inventoryMessage };
                        }
                    }
                }
            }

            return new ConfigurableProductViewModel() { ShowAddToCart = true };
        }


        // Validate Inventory.
        protected virtual void ValidateInventoryForConfigurableProducts(ProductViewModel productViewModel, decimal combinedQuantity)
        {
            if (HelperUtility.Between(combinedQuantity, Convert.ToDecimal(productViewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(productViewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
            {
                productViewModel.InventoryMessage = string.Empty;
                productViewModel.ShowAddToCart = true;
            }
            else
            {
                productViewModel.InventoryMessage = !string.IsNullOrEmpty(productViewModel.OutOfStockMessage) ? productViewModel.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                productViewModel.ShowAddToCart = false;
            }
        }

        //Set All Locations filter value.
        protected void SetAllLocationInventoryFilter(FilterCollection filters, bool isAllLocationsInventoryFlag)
        {
            //bind the store level inventory flag to get the product inventory of default and all locations set for the current store.
            bool isGetAllLocationsInventory = isAllLocationsInventoryFlag ? true : Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, ZnodeConstant.DisplayAllWarehousesStock, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);
            if (isGetAllLocationsInventory && IsNotNull(filters))
            {
                if (filters.Exists(x => string.Equals(x.Item1, ZnodeConstant.IsGetAllLocationsInventory, StringComparison.InvariantCultureIgnoreCase)))
                {
                    //If SKU is already present in filters, remove it.
                    filters.RemoveAll(x => string.Equals(x.Item1, ZnodeConstant.IsGetAllLocationsInventory, StringComparison.InvariantCultureIgnoreCase));

                    //Add New productAttributeValue into filters.
                    filters.Add(new FilterTuple(ZnodeConstant.IsGetAllLocationsInventory, FilterOperators.Equals, isGetAllLocationsInventory.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeConstant.IsGetAllLocationsInventory, FilterOperators.Equals, isGetAllLocationsInventory.ToString()));
            }
        }
        #endregion
        #endregion

        #region Private Methods

        //Get product expands.
        public ExpandCollection GetProductExpands(bool isProductDetails = false)
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.ProductTemplate);
            expands.Add(ExpandKeys.AddOns);
            expands.Add(ExpandKeys.SEO);
            expands.Add(ExpandKeys.Brand);
            expands.Add(ExpandKeys.ProductReviews);

            if (isProductDetails)
                expands.Add(ExpandKeys.ConfigurableAttribute);
            return expands;
        }

        //Get product expands.
        public ExpandCollection GetProductExpands(bool isProductDetails = false, params string[] expandKeys)
        {
            ExpandCollection expands = new ExpandCollection();

            foreach (string expand in expandKeys)
            {
                expands.Add(expand);
            }

            if (isProductDetails)
                expands.Add(ExpandKeys.ConfigurableAttribute);
            return expands;
        }

        //Get product expands.
        private ExpandCollection GetExpandsForProductCompare(bool isProductDetails = false)
        {
            ExpandCollection expands = new ExpandCollection();
            if (isProductDetails)
            {
                expands.Add(ExpandKeys.Promotions);
                expands.Add(ExpandKeys.Inventory);
                expands.Add(ExpandKeys.ProductReviews);
                expands.Add(ExpandKeys.Pricing);
                expands.Add(ExpandKeys.ProductTemplate);
                expands.Add(ExpandKeys.AddOns);
                expands.Add(ExpandKeys.SEO);
                expands.Add(ExpandKeys.ConfigurableAttribute);
            }
            else
            {
                expands.Add(ExpandKeys.Promotions);
                expands.Add(ExpandKeys.Pricing);
            }
            return expands;
        }
        //Get product inventory expands.
        private ExpandCollection GetProductInventoryExpands()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.AddOns);
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.ConfigurableAttribute);
            return expands;
        }

        //Generate filters to get products for quick order functionality through autocomplete.
        private FilterCollection GetFilterForProductAutoComplete(string skus)
        {
            PortalViewModel currentPortal = PortalAgent.CurrentPortal;

            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(currentPortal))
            {
                filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, currentPortal.PortalId.ToString());
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
                filters.Add(ZnodePriceEnum.SKU.ToString(), FilterOperators.Like, skus);
            }
            return filters;
        }

        //Set AllowBackOrder and TrackInventory.
        public static void TrackInventoryData(ref bool AllowBackOrder, ref bool TrackInventory, string inventorySetting)
        {
            switch (inventorySetting)
            {
                case ZnodeConstant.DisablePurchasing:
                    AllowBackOrder = false;
                    TrackInventory = true;
                    break;
                case ZnodeConstant.AllowBackOrdering:
                    AllowBackOrder = true;
                    TrackInventory = true;
                    break;

                case ZnodeConstant.DontTrackInventory:
                    AllowBackOrder = false;
                    TrackInventory = false;
                    break;
            }
        }

        //Get filter and sort for product reviews.
        private void GetFilterAndSortForProductReviews(int productId, ref string sortingChoice, ref int? pageSize, ref int pageNo, out FilterCollection filters, out SortCollection sorts)
        {
            filters = new FilterCollection();
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId));
            filters.Add(ZnodePublishProductEnum.PublishProductId.ToString(), FilterOperators.Equals, Convert.ToString(productId));
            filters.Add(ZnodeCMSCustomerReviewEnum.Status.ToString(), FilterOperators.Is, "A");

            if (string.IsNullOrEmpty(sortingChoice))
                sortingChoice = WebStoreConstants.NewestFirst;

            pageSize = pageSize == 0 ? pageSize = 16 : pageSize;
            pageSize = pageSize == -1 ? pageSize = null : pageSize;
            pageNo = pageNo == 0 ? pageNo = 1 : pageNo;
            SaveInSession<int?>(WebStoreConstants.PageSizeValue, pageSize);
            //Sorting For Brand List.
            sorts = new SortCollection();
            switch (sortingChoice)
            {
                case WebStoreConstants.NewestFirst:
                    sorts.Add(ZnodeCMSCustomerReviewEnum.CreatedDate.ToString(), SortDirections.Descending);
                    break;
                case WebStoreConstants.OldestFirst:
                    sorts.Add(ZnodeCMSCustomerReviewEnum.CreatedDate.ToString(), SortDirections.Ascending);
                    break;
                case WebStoreConstants.HighestRatingFirst:
                    sorts.Add(ZnodeCMSCustomerReviewEnum.Rating.ToString(), SortDirections.Descending);
                    break;
                case WebStoreConstants.LowestRatingFirst:
                    sorts.Add(ZnodeCMSCustomerReviewEnum.Rating.ToString(), SortDirections.Ascending);
                    break;
            }
        }

        //Get attribute values and code.
        public Dictionary<string, string> GetAttributeValues(string codes, string values)
        {
            //Attribute Code And Value 
            string[] Codes = codes.Split(',');
            string[] Values = values.Split(',');
            Dictionary<string, string> SelectedAttributes = new Dictionary<string, string>();

            //Add code and value pair
            for (int i = 0; i < Codes.Length; i++)
                SelectedAttributes.Add(Codes[i], Values[i]);
            return SelectedAttributes;
        }

        //Get Error Message For Product Compare
        private string GetErrorMessage(int errorCode)
        {
            string message = string.Empty;
            switch (errorCode)
            {
                case 1:
                    message = WebStore_Resources.ProductCategoryChangeErrorMessage;
                    break;
                case 2:
                    message = WebStore_Resources.ProductCompareProductExistErrorMessage;
                    break;
                case 3:
                    message = WebStore_Resources.ProductCompareLimitReachedErrorMessage;
                    break;
                default:
                    message = WebStore_Resources.ProductCompareSuccessMessage;
                    break;

            }
            return message;
        }

        private void CompareProduct(int productId, int categoryId, List<ProductCompareViewModel> compareProducts)
        {
            ProductCompareViewModel compareProduct = new ProductCompareViewModel();
            compareProduct.CategoryId = categoryId;
            compareProduct.ProductId = productId;
            compareProducts.Add(compareProduct);
            SaveInSession(ZnodeConstant.CompareProducts, compareProducts);
        }

        //Check if product already exist in recently view product list.
        private bool IsProductExistInList(List<string> productIds, string productId)
        {
            if (productIds?.Count > 0)
                return productIds.Contains(productId) ? true : false;

            return false;
        }

        //Add Product to recently view list.
        private void AddToRecentlyViewProduct(int productId)
        {
            if (productId > 0)
            {
                List<string> productIds = GetFromSession<List<string>>(ZnodeConstant.RecentlyViewProducts);
                if (!IsProductExistInList(productIds, Convert.ToString(productId)))
                    SetMaxRecentProductInSession(Convert.ToString(productId));
            }
        }

        //Check for max limit of recently view product.
        private void SetMaxRecentProductInSession(string productId)
        {
            //List of product ids from cookies
            List<string> productIds = GetFromSession<List<string>>(ZnodeConstant.RecentlyViewProducts);

            if (HelperUtility.IsNull(productIds))
                productIds = new List<string>();

            productIds.Add(productId);

            int maxItemToDisplay = /*MvcDemoConstants.MaxRecentViewItemToDisplay*/15;

            //If exceed of max limit of recently view product remove last product from list.
            if (productIds.Count > maxItemToDisplay)
                for (int count = 0; count < productIds.Count - maxItemToDisplay; count++)
                    productIds.RemoveAt(0);

            if (productIds.Count > 0)
                SaveInSession(ZnodeConstant.RecentlyViewProducts, productIds);
        }

        //Check group product quantity.
        private void CheckGroupInventory(GroupProductViewModel viewModel, decimal? quantity)
        {
            if (HelperUtility.IsNotNull(viewModel))
            {
                List<AttributesSelectValuesViewModel> inventorySetting = viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);
                string inventorySettingCode = inventorySetting.FirstOrDefault().Code;
                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : WebStore_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }

                if (HelperUtility.IsNotNull(viewModel.Quantity))
                {
                    bool AllowBackOrder = false;
                    bool TrackInventory = false;
                    decimal selectedQuantity = quantity.GetValueOrDefault();

                    decimal cartQuantity = GetGroupProductOrderedItemQuantity(viewModel.SKU);

                    decimal combinedQuantity = selectedQuantity + cartQuantity;

                    if (inventorySetting?.Count > 0)
                    {
                        TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                        if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                        {
                            viewModel.InventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantityGroupProduct, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity), viewModel.Name);
                            viewModel.ShowAddToCart = false;
                            return;
                        }

                        if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                        {
                            viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? $"{viewModel.OutOfStockMessage}" : $"{WebStore_Resources.TextOutofStock}";
                            viewModel.ShowAddToCart = false;
                            return;
                        }
                        else if (viewModel.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                        {
                            viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? $"{viewModel.BackOrderMessage}" : $"{WebStore_Resources.TextBackOrderMessage}";
                            viewModel.ShowAddToCart = true;
                            return;
                        }

                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? $"{viewModel.InStockMessage}" : $"{WebStore_Resources.TextInstock}";
                        viewModel.ShowAddToCart = true;
                    }
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? $"{viewModel.OutOfStockMessage}" : $"{WebStore_Resources.TextOutofStock}";
                    viewModel.ShowAddToCart = false;
                    return;
                }
            }
        }

        //Get group product quantity according to quantity.
        private void GetGroupProductFinalPrice(GroupProductViewModel viewModel, decimal minQuantity)
        {
            viewModel.SalesPrice = viewModel.SalesPrice > 0 ? viewModel.SalesPrice * minQuantity : viewModel.RetailPrice;
            viewModel.RetailPrice = viewModel.SalesPrice < 1 ? viewModel.RetailPrice * minQuantity : viewModel.RetailPrice;
        }

        private void GetConfigurableValues(PublishProductModel model, ProductViewModel viewModel)
        {
            viewModel.ConfigurableData = new ConfigurableAttributeViewModel();
            //Select Is Configurable Attributes list
            viewModel.ConfigurableData.ConfigurableAttributes = viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute?.Count > 0).ToList();
            //Assign select attribute values.
            if (model.IsDefaultConfigurableProduct)
                viewModel.ConfigurableData.ConfigurableAttributes.ForEach(x => x.SelectedAttributeValue = new[] { x?.AttributeValues });
            else
                viewModel.ConfigurableData.ConfigurableAttributes.ForEach(x => x.SelectedAttributeValue = new[] { x.ConfigurableAttribute?.FirstOrDefault()?.AttributeValue });
        }

        private void GetConfigurableValues(PublishProductDTO model, ShortProductViewModel viewModel)
        {
            viewModel.ConfigurableData = new ConfigurableAttributeViewModel();
            //Select Is Configurable Attributes list
            viewModel.ConfigurableData.ConfigurableAttributes = viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute?.Count > 0).ToList();
            //Assign select attribute values.
            viewModel.ConfigurableData.ConfigurableAttributes.ForEach(x => x.SelectedAttributeValue = new[] { x.ConfigurableAttribute?.FirstOrDefault()?.AttributeValue });
        }

        /// <summary>
        /// To update recent view products
        /// </summary>
        /// <param name="viewModel"></param>
        private void UpdateRecentViewedProducts(ProductViewModel viewModel)
        {
            RecentViewModel recentViewModel = new RecentViewModel()
            {
                ImageSmallPath = viewModel.ConfigurableProductId > 0 ? HttpUtility.UrlEncode(viewModel.ParentProductImageSmallPath) : HttpUtility.UrlEncode(viewModel.ImageSmallPath),
                PublishProductId = viewModel.ConfigurableProductId > 0 ? viewModel.ConfigurableProductId : viewModel.PublishProductId,
                SalesPrice = viewModel.SalesPrice,
                Name = viewModel.ConfigurableProductId > 0 ? viewModel.ParentConfiguarableProductName : viewModel.Name,
                SEOUrl = viewModel.SEOUrl,
                SKU = viewModel.SKU,
                ProductType = viewModel.ProductType,
                CultureCode = viewModel.CultureCode,
                PromotionalPrice = viewModel.PromotionalPrice,
                UOM = Attributes.ValueFromSelectValue(viewModel?.Attributes, ZnodeConstant.UOM),
                RetailPrice = viewModel.RetailPrice,
                Rating = viewModel.Rating,
                Attributes = viewModel.Attributes,
                Promotions = viewModel.Promotions

            };

            int maxItemToDisplay = 15;
            List<RecentViewModel> storedValues = new List<RecentViewModel>();
            try
            {
                List<RecentViewModel> recentViewProductCookie = GetFromSession<List<RecentViewModel>>("RecentViewProduct");

                if (recentViewProductCookie == null || recentViewProductCookie?.Count == 0)
                {
                    storedValues.Add(recentViewModel);
                    SaveInSession(ZnodeConstant.RecentViewProduct, storedValues);
                }
                else
                {
                    int publishProductId = viewModel.ConfigurableProductId > 0 ? viewModel.ConfigurableProductId : viewModel.PublishProductId;
                    storedValues = recentViewProductCookie;

                    if (!storedValues.Where(x => x.PublishProductId == publishProductId).Any())
                    {
                        storedValues.Insert(0, recentViewModel);
                        if (storedValues.Count() > maxItemToDisplay)
                        {
                            storedValues.RemoveAt(storedValues.Count - 1);
                        }
                        SaveInSession(ZnodeConstant.RecentViewProduct, storedValues);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Error);
            }
        }

        //Map configurable product data.
        public virtual void MapConfigurableProductData(int productId, string sku, ProductViewModel viewModel, ConfigurableAttributeViewModel configurableData)
        {
            viewModel.ConfigurableData = new ConfigurableAttributeViewModel();
            //Assign list of configurable attribute
            viewModel.ConfigurableData.ConfigurableAttributes = configurableData.ConfigurableAttributes;
            //List od swatch images
            viewModel.ConfigurableData.SwatchImages = configurableData.SwatchImages;
            viewModel.ConfigurableData.CombinationErrorMessage = configurableData.CombinationErrorMessage;

            string minQuantity = viewModel?.Attributes?.Value(ZnodeConstant.MinimumQuantity);
            decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);

            //Check any default addon product is selected or not.
            string addOnSKU = string.Empty;
            List<string> addOnProductSKU = viewModel.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.FirstOrDefault(x => x.IsDefault)?.SKU)?.ToList();

            if (addOnProductSKU?.Count > 0)
            {
                addOnSKU = string.Join(",", addOnProductSKU.Where(x => !string.IsNullOrEmpty(x)));
            }

            viewModel.IsCallForPricing = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (viewModel.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();

            //Check Product Inventory
            CheckInventory(viewModel, quantity);

            if (!string.IsNullOrEmpty(addOnSKU))
                //Check Associated addon inventory.
                CheckAddOnInvenTory(viewModel, addOnSKU, quantity);

            GetProductFinalPrice(viewModel, viewModel.AddOns, quantity, addOnSKU);

            if (HelperUtility.IsNull(viewModel.ProductPrice))
            {
                viewModel.ShowAddToCart = false;
                viewModel.InventoryMessage = WebStore_Resources.ErrorPriceNotAssociate;
            }

            viewModel.ParentProductId = productId;
            viewModel.IsConfigurable = true;
            if (viewModel.IsDefaultConfigurableProduct)
                viewModel.ShowAddToCart = false;
        }



        /// <summary>
        /// To get recent viewed products
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<RecentViewModel> GetRecentProductView(int productId)
        {
            List<RecentViewModel> storedValues = new List<RecentViewModel>();
            List<RecentViewModel> recentViewProductCookie = GetFromSession<List<RecentViewModel>>(ZnodeConstant.RecentViewProduct);

            if (recentViewProductCookie != null || recentViewProductCookie?.Count > 0)
            {
                List<RecentViewModel> deserializedObject = GetFromSession<List<RecentViewModel>>(ZnodeConstant.RecentViewProduct);
                if (deserializedObject != null && deserializedObject.Any())
                {


                    List<RecentViewProductModel> productData = _productClient.GetActiveProducts(string.Join(",", deserializedObject.Select(x => x.PublishProductId).ToList()), GetCatalogId().GetValueOrDefault(), PortalAgent.LocaleId, 0);
                    deserializedObject.RemoveAll(x => !string.Join(",", productData.Select(y => y.ZnodeProductId.ToString())).Contains(x.PublishProductId.ToString()));
                    if (HelperUtility.IsNotNull(productData) && productData.Any())
                    {
                        foreach (RecentViewModel item in deserializedObject)
                        {
                            if (productData.Where(x => x.ZnodeProductId == item.PublishProductId).Count() > 0 && item.PublishProductId != productId)
                            {
                                item.ImageSmallPath = HttpUtility.UrlDecode(item.ImageSmallPath);
                                storedValues.Add(item);
                            }
                        }
                    }
                }
            }
            return storedValues;
        }


        //Get parameter model for configurable attibutr.
        private ParameterProductModel GetConfigurableParameterModel(int productId, string selectedCode, string selectedValue, Dictionary<string, string> SelectedAttributes)
        {
            ParameterProductModel productAttribute = new ParameterProductModel();
            productAttribute.ParentProductId = productId;
            productAttribute.LocaleId = PortalAgent.LocaleId;
            productAttribute.SelectedAttributes = SelectedAttributes;
            productAttribute.PortalId = PortalAgent.CurrentPortal.PortalId;
            productAttribute.SelectedCode = selectedCode;
            productAttribute.SelectedValue = selectedValue;
            return productAttribute;
        }

        //Gets the breadcrumb for the product.
        private void GetProductBreadCrumb(int categoryId, ProductViewModel viewModel, int productId)
        {
            string breadCrumbHtml = string.Empty;

            CategoryViewModel category = (categoryId > 0) ? viewModel.CategoryHierarchy.FirstOrDefault(categoryItem => categoryItem.CategoryId == categoryId) : viewModel.CategoryHierarchy?.FirstOrDefault();

            string categoryBreadCrumb = GetBreadCrumbHtml(category);
            if (!string.IsNullOrEmpty(categoryBreadCrumb))
                viewModel.BreadCrumbHtml = $"{categoryBreadCrumb} / {viewModel.Name}";
        }

        //Gets bread html for category.
        private string GetBreadCrumbHtml(CategoryViewModel category, bool isParent = false)
        {
            if (HelperUtility.IsNotNull(category))
            {
                string breadCrumb = $"<a href='/{(string.IsNullOrEmpty(category.SEODetails?.SEOUrl) ? "category/" + category.CategoryId : category.SEODetails.SEOUrl)}'>{category.CategoryName}</a>";
                if (category.ParentCategory?.Count > 0)
                    breadCrumb = GetBreadCrumbHtml(category.ParentCategory[0], true) + " / " + breadCrumb;
                return breadCrumb;
            }
            return string.Empty;
        }

        //Get Out Of Stock Options Attribute List.
        private List<AttributesSelectValuesViewModel> GetOutOfStockOptionsAttributeList(ProductViewModel viewModel)
            => viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        //Get Out Of Stock Options Attribute List.
        private List<AttributesSelectValuesViewModel> GetOutOfStockOptionsAttributeList(ShortProductViewModel viewModel)
            => viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        //Get Price details for group products.
        private List<ProductInventoryPriceModel> GroupedProducts(List<ProductPriceViewModel> products)
        {
            if (products?.Count > 0)
            {
                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                return _productClient.GetProductPrice(new ParameterInventoryPriceModel { Parameter = string.Join(",", products?.Select(x => x.sku)), ProductType = ZnodeConstant.GroupedProduct, CatalogId = PortalAgent.CurrentPortal.PublishCatalogId, LocaleId = PortalAgent.LocaleId, PortalId = PortalAgent.CurrentPortal.PortalId })?.ProductList;
            }
            return new List<ProductInventoryPriceModel>();
        }

        //Get Price details for configurable products.
        private List<ProductInventoryPriceModel> ConfigurableProducts(List<ProductPriceViewModel> products)
        {
            if (products?.Count > 0)
                return _productClient.GetProductPrice(new ParameterInventoryPriceModel { Parameter = string.Join(",", products?.Select(x => x.sku)), ProductType = ZnodeConstant.ConfigurableProduct, CatalogId = PortalAgent.CurrentPortal.PublishCatalogId, LocaleId = PortalAgent.LocaleId, PortalId = PortalAgent.CurrentPortal.PortalId })?.ProductList;
            return new List<ProductInventoryPriceModel>();
        }

        //Get Price details for products.
        private List<ProductInventoryPriceModel> OtherProducts(List<ProductPriceViewModel> products)
        {
            if (products?.Count > 0)
            {
                products.RemoveAll(x => x.type == ZnodeConstant.GroupedProduct);
                products.RemoveAll(x => x.type == ZnodeConstant.ConfigurableProduct);
                _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                return _productClient.GetProductPrice(new ParameterInventoryPriceModel { Parameter = string.Join(",", products?.Select(x => x.sku)), PortalId = PortalAgent.CurrentPortal.PortalId })?.ProductList;
            }
            return new List<ProductInventoryPriceModel>();
        }

        //Map search products.
        private void MapSearchProducts(KeywordSearchModel searchResult, ProductListViewModel productList)
        {
            //Get Product list data.
            if (searchResult?.Products?.Count > 0)
            {
                productList.Products = searchResult.Products?.ToViewModel<ProductViewModel>()?.ToList();
            }
        }

        //Validate min and max quantity of product
        private bool ValidateMinMaxQuantity(ProductViewModel viewModel, decimal? quantity, out decimal combinedQuantity)
        {
            bool isDisplayVariantsOnGrid = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.DisplayVariantsOnGrid)) && viewModel.IsConfigurable;

            bool result = false;
            if (HelperUtility.IsNotNull(viewModel))
            {
                decimal selectedQuantity = quantity.GetValueOrDefault();
                string sku = string.IsNullOrEmpty(viewModel.ConfigurableProductSKU) ? viewModel.SKU : viewModel.ConfigurableProductSKU;
                decimal cartQuantity = GetOrderedItemQuantity(sku);
                combinedQuantity = selectedQuantity + cartQuantity;
                if (!Equals(viewModel.ProductType, ZnodeConstant.GroupedProduct) && !isDisplayVariantsOnGrid && !HelperUtility.Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                    result = false;
                else
                    result = true;
            }
            else
            {
                combinedQuantity = quantity.GetValueOrDefault();
                result = true;
            }
            return result;
        }

        //Get product price  
        protected virtual List<ProductPriceViewModel> GetFinalProductPrice(List<ProductPriceViewModel> productsPriceAndInventory)
        {
            if (HelperUtility.IsNull(productsPriceAndInventory)) return productsPriceAndInventory;

            decimal? ProductPrice = 0;

            foreach (ProductPriceViewModel priceViewModel in productsPriceAndInventory)
            {
                if (HelperUtility.IsNull(priceViewModel) || string.IsNullOrEmpty(priceViewModel.MinQuantity))
                    continue;

                if (priceViewModel?.TierPriceList.Count > 0 && priceViewModel?.TierPriceList.Where(x => Convert.ToDecimal(priceViewModel.MinQuantity) >= x.MinQuantity)?.Count() > 0)
                    ProductPrice = priceViewModel.TierPriceList.FirstOrDefault(x => Convert.ToDecimal(priceViewModel.MinQuantity) >= x.MinQuantity && Convert.ToDecimal(priceViewModel.MinQuantity) < x.MaxQuantity)?.Price;
                else
                    ProductPrice = (Convert.ToDecimal(priceViewModel.MinQuantity) > 0 && HelperUtility.IsNotNull(priceViewModel.SalesPrice)) ? priceViewModel.SalesPrice * Convert.ToDecimal(priceViewModel.MinQuantity) : priceViewModel?.PromotionalPrice > 0 ? priceViewModel?.PromotionalPrice * Convert.ToDecimal(priceViewModel.MinQuantity) : priceViewModel.RetailPrice;

                productsPriceAndInventory.FirstOrDefault(x => x.sku == priceViewModel.sku).ProductPrice = ProductPrice;
            }
            return productsPriceAndInventory;
        }

        #endregion

        #region Protected Methods

        // This method only return the filter for category 
        protected virtual FilterCollection SetProductFilter(bool isCategoryAssociated)
        {
            FilterCollection filters = GetRequiredFilters();
            if (isCategoryAssociated)
            {
                filters.Add(WebStoreEnum.ZnodeCategoryIds.ToString(), FilterOperators.NotEquals, "0");
            }
            return filters;
        }

        // This method return inventory message for configurable child product.
        protected virtual string InventoryMessageForConfigurableProducts(List<ConfigurableProductViewModel> childProductList, string[] childProductSKUs)
        {
            string message = string.Empty;
            if (childProductList.Where(x => childProductSKUs.Contains(x.SKU)).Any(x => !x.ShowAddToCart))
            {
                List<ConfigurableProductViewModel> outOfStockProductList = childProductList?.Where(x => !x.ShowAddToCart && childProductSKUs.Contains(x.SKU)).ToList();
                return message = outOfStockProductList.FirstOrDefault().InventoryMessage;
            }
            return message;
        }

        // Get the product attributes.
        protected List<AttributeValidationViewModel> GetattributeValidation(ProductViewModel productViewModel, int productId)
        {
            if (productId > 0 &&  IsNotNull(productViewModel))
            {
                Dictionary<string, string> personalizeValues = productViewModel.Attributes.Where(x => x.IsPersonalizable == true)?.Select(x => new { x.AttributeCode, x.AttributeName })?.ToDictionary(y => y.AttributeCode, y => y.AttributeName);
                IAttributeAgent attributeAgent = new AttributeAgent(GetClient<PIMAttributeClient>());
                List<AttributeValidationViewModel> attributeValidations = attributeAgent.GetAttributeValidationByCodes(productId, personalizeValues);
                return attributeValidations;
            }
            return null;
        }
        #endregion
    }
}
