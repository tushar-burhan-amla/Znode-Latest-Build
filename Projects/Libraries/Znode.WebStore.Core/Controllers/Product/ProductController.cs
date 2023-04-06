using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Znode.Engine.Api.Models;
using Znode.Engine.Core.ViewModels;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.WebStore.Controllers
{
    public class ProductController : BaseController
    {
        #region Private Readonly members
        private readonly IProductAgent _productAgent;
        private readonly IUserAgent _accountAgent;
        private readonly IAttributeAgent _attributeAgent;
        private readonly ICartAgent _cartAgent;
        private const string ComparableProductsView = "_ComparableProducts";
        private const string ProductComparePopup = "_ProductComparePopup";
        private const string SendMailPopUp = "_SendMailView";
        private readonly IWidgetDataAgent _widgetDataAgent;
        private readonly IRecommendationAgent _recommendationAgent;
        #endregion

        #region Public Constructor
        public ProductController(IProductAgent productAgent, IUserAgent userAgent, ICartAgent cartAgent, IWidgetDataAgent widgetDataAgent, IAttributeAgent attributeAgent, IRecommendationAgent recommendationAgent)
        {
            _productAgent = productAgent;
            _accountAgent = userAgent;
            _cartAgent = cartAgent;
            _widgetDataAgent = widgetDataAgent;
            _attributeAgent = attributeAgent;
            _recommendationAgent = recommendationAgent;
        }
        #endregion

        [HttpGet]
        public virtual ActionResult WriteReview(int Id, string name)
        => View("WriteReview", _productAgent.GetProductForReview(Id, name, null));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult WriteReview(ProductReviewViewModel reviewModel)
        {
            if (ModelState.IsValid)
            {
                ProductReviewViewModel model = _productAgent.CreateReview(reviewModel);
                if (!model.HasError && model.CMSCustomerReviewId > 0)
                {
                    ModelState.Clear();
                    ProductReviewViewModel viewModel = _productAgent.GetProductForReview(reviewModel.PublishProductId, reviewModel.ProductName, reviewModel.Rating);
                    TempData[WebStoreConstants.Notifications] = (model.Status != WebStoreConstants.ReviewStatus) ? GenerateNotificationMessages(WebStore_Resources.SuccessWriteReview, NotificationType.success) : GenerateNotificationMessages(WebStore_Resources.SuccessWriteReviewWhenApproverRequired, NotificationType.success);
                    return View("WriteReview", viewModel);
                }
                else
                {
                    TempData[WebStoreConstants.Notifications] = GenerateNotificationMessages(WebStore_Resources.ErrorWriteReview, NotificationType.error);
                    return View("WriteReview", _productAgent.GetProductForReview(reviewModel.PublishProductId, reviewModel.ProductName, null));
                }
            }
            TempData[WebStoreConstants.Notifications] = GenerateNotificationMessages(WebStore_Resources.ErrorWriteReview, NotificationType.error);
            return View("WriteReview", _productAgent.GetProductForReview(reviewModel.PublishProductId, reviewModel.ProductName, null));
        }



        [HttpGet]
        public virtual ActionResult Details(int id = 0, int? parentOmsSavedCartLineItemId = 0, string seo = "", bool isQuickView = false)
        {
            ViewBag.ProductId = id;
            ViewBag.Seo = seo;
            ViewBag.IsQuickView = isQuickView;

            SEOViewModel seoModel = _productAgent.GetSEODetails(id, seo);
            ViewBag.Title = string.IsNullOrEmpty(seoModel.SEOTitle) ? PortalAgent.CurrentPortal.WebsiteTitle : seoModel.SEOTitle;
            ViewBag.Keywords = seoModel?.SEOKeywords;
            ViewBag.Description = seoModel?.SEODescription;
            ViewBag.CanonicalURL = seoModel?.CanonicalURL;
            ViewBag.RobotTag = string.IsNullOrEmpty(seoModel.RobotTag) || seoModel?.RobotTag?.ToLower() == "none" ? string.Empty : seoModel?.RobotTag.Replace("_", ",");

            return View("ProductDetails");
        }

        [HttpGet]
        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "id;publishState")]
        public virtual ActionResult BriefContent(int id = 0, string seo = "", bool isQuickView = false, string publishState = "PRODUCTION")
        {
            ViewBag.ProductId = id;
            ViewBag.Seo = seo;
            ViewBag.IsQuickView = isQuickView;
            ViewBag.IsLite = true;

            ProductViewModel product = _productAgent.GetProductBrief(id);
            if (IsNull(product))
                return Redirect("/404");

            product.ProductTemplateName = string.IsNullOrEmpty(product.ProductTemplateName) || Equals(product.ProductTemplateName, ZnodeConstant.ProductDefaultTemplate) ? "Details" : product.ProductTemplateName;

            string templateName = product.ProductTemplateName + "_Lite";

            return PartialView(templateName, product);
        }

        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "id;publishState;profileId;localeId;accountId;catalogId")]
        public virtual ActionResult DetailsContent(int id = 0, string seo = "", bool isQuickView = false, string publishState = "PRODUCTION", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0)
        {
            ViewBag.IsLite = false;
            ProductViewModel product = _productAgent.GetProduct(id,true);
            if (IsNull(product))
                return Redirect("/404");

            product.ProductTemplateName = string.IsNullOrEmpty(product.ProductTemplateName) || Equals(product.ProductTemplateName, ZnodeConstant.ProductDefaultTemplate) ? "Details" : product.ProductTemplateName;

            return PartialView(product.ProductTemplateName, product);
        }

        [HttpGet]
        [ZnodePageCache(Duration = 3600, Location = OutputCacheLocation.Server, VaryByParam = "id;expands;publishState")]
        public virtual ActionResult ExtendedContent(int id = 0, string expands = "", string seo = "", bool isQuickView = false, string publishState = "PRODUCTION")
        {
            string[] expandKeys = !string.IsNullOrEmpty(expands) ? expands.Split(',') : null;

            ShortProductViewModel product = _productAgent.GetExtendedProductDetails(id, expandKeys);
            if (IsNull(product))
                return new HttpNotFoundResult("The supplied id or one of the supplied expands is not valid.");

            return Json(product, "application/json", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ZnodePageCache(Duration = 3600, Location = OutputCacheLocation.Server, VaryByParam = "id;expands;publishState")]
        public virtual ActionResult AlternateProductImages(int id = 0, string expands = "", string seo = "", bool isQuickView = false, string publishState = "PRODUCTION")
        {
            string[] expandKeys = !string.IsNullOrEmpty(expands) ? expands.Split(',') : null;
            ShortProductViewModel product = _productAgent.GetExtendedProductDetails(id, expandKeys);
            if (IsNull(product.ProductImage))
                return new HttpNotFoundResult("One of the supplied expands is not valid.");

            return PartialView("_AlternateImages", product);
        }

        [HttpGet]
        [ZnodePageCache(Duration = 3600, Location = OutputCacheLocation.Server, VaryByParam = "id;expands;publishState")]
        public virtual ActionResult AlternateProductImagesZoomEffect(int id = 0, string expands = "", string seo = "", bool isQuickView = false, string publishState = "PRODUCTION")
        {
            string[] expandKeys = !string.IsNullOrEmpty(expands) ? expands.Split(',') : null;
            ShortProductViewModel product = _productAgent.GetExtendedProductDetails(id, expandKeys);
            if (IsNull(product.ProductImage))
                return new HttpNotFoundResult("One of the supplied expands is not valid.");

            return PartialView("_AlternateImagesZoomEffect", product);
        }

        [HttpGet]
        [ZnodePageCache(Duration = 3600, Location = OutputCacheLocation.Server, VaryByParam = "id;expands;publishState")]
        public virtual ActionResult ProductInfo(int id = 0, string expands = "", string seo = "", bool isQuickView = false, string publishState = "PRODUCTION")
        {
            string[] expandKeys = !string.IsNullOrEmpty(expands) ? expands.Split(',') : null;
            ShortProductViewModel product = _productAgent.GetExtendedProductDetails(id, expandKeys);
            if (IsNull(product.ProductImage))
                return new HttpNotFoundResult("One of the supplied expands is not valid.");

            return PartialView("_ProductInfoLite", product);
        }

        //Render Quick View Partial through widget.
        [HttpGet]
        [ZnodePageCache(Duration = 3600, Location = OutputCacheLocation.Server, VaryByParam = "id;publishState;localeId;profileId;accountId;catalogId")]
        public virtual ActionResult GetProductQuickView(int id, bool isQuickView, string seo = "", string publishState = "PRODUCTION", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0)
        {
            ProductViewModel product = new ProductViewModel();
            product.PublishProductId = id;
            product.IsQuickView = isQuickView;
            return View("_QuickViewProduct", product);
        }

        //Get List Of Recommended Products
        [HttpGet]
        public virtual ActionResult GetRecommendedProducts(string widgetCode, string productSku = "") // "" means we're on the cart page
        {
            List<ProductViewModel> productList = _recommendationAgent.GetRecommendedProducts(widgetCode, productSku);
            ViewBag.ProductCount = 3;
            return ActionView("_RecommendedProduct", productList);
        }

        [HttpPost]
        public virtual ActionResult AddToCart(CartItemViewModel cartItem)
        {
            _cartAgent.CreateCart(cartItem);
            return RedirectToAction<CartController>(x => x.Index());
        }

        //Add product in the cart.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public virtual ActionResult AddToCartProduct(AddToCartViewModel cartItem, bool IsRedirectToCart = true)
        {
            AddToCartViewModel ShoppingCart = _cartAgent.AddToCartProduct(cartItem);

            if (IsRedirectToCart)
                return RedirectToAction<CartController>(x => x.Index());
            ShoppingCartItemModel productDetail = ShoppingCart?.ShoppingCartItems?.FirstOrDefault(x => x.SKU == cartItem.SKU);
            string imagePath = $"{PortalAgent.CurrentPortal.ImageThumbnailUrl}{productDetail?.Product?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues}";
            if (IsNotNull(cartItem) && !string.IsNullOrEmpty(imagePath))
            {
                cartItem.Image = imagePath;
            }
            return Json(new { status = ShoppingCart?.HasError, cartCount = ShoppingCart?.CartCount, Product = productDetail, ImagePath = imagePath, CartNotification = _cartAgent.BindAddToCartNotification(cartItem) }, JsonRequestBehavior.AllowGet);
        }

        //This method is Add valid SKUs to Quick Order Grid
        [HttpPost]
        public virtual ActionResult AddProductsToQuickOrder(string multipleItems)
        {
            QuickOrderViewModel returnModel = _productAgent.AddProductsToQuickOrder(multipleItems);

            if(IsNotNull(returnModel))
            {
                return Json(new
                {
                    response = new { IsSuccess = returnModel.IsSuccess, ValidSKUCount = returnModel.ValidSKUCount, InvalidSKUCount = returnModel.InvalidSKUCount, ProductSKUText = returnModel.ProductSKUText },
                    rowsHtml = RenderRazorViewToString("_MultipleQuickOrderRow", returnModel),
                    notificationHtml = returnModel.NotificationMessage
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                response = new { IsSuccess = false, ValidSKUCount = 0, InvalidSKUCount = 0, ProductSKUText = "" },
                rowsHtml = "",
                notificationHtml = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual FileResult DownloadQuickOrderTemplate(string fileType)
        {
            string fileName = string.Empty;
            string path = String.Empty;
            if (fileType != WebStoreConstants.CSV)
            {
                path = Server.MapPath("~/Data/QuickOrder/") + WebStoreConstants.TemplateQuickOrderExcel + WebStoreConstants.XLSX;
                fileName = WebStoreConstants.TemplateQuickOrderExcel;
            }
            else
            {
                path = Server.MapPath("~/Data/QuickOrder/") + WebStoreConstants.TemplateQuickOrder + WebStoreConstants.CSV;
                fileName = WebStoreConstants.TemplateQuickOrder;
            }
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes,"application/ocet-stream", fileName + fileType);
        }


        [HttpPost]
        public virtual ActionResult AddMultipleProductsToCart(List<CartItemViewModel> cartItems)
        {
            string errorMessage = _cartAgent.AddMultipleProductsToCart(cartItems);
            return Json(new
            {
                isSuccess = string.IsNullOrEmpty(errorMessage),
                message = string.IsNullOrEmpty(errorMessage) ? WebStore_Resources.MultipleCartSuccessMessage : errorMessage,
                cartCount = _cartAgent.GetCartCount(),
            }, JsonRequestBehavior.AllowGet);
        }

        //Method to get out of stock details for a product.
        public virtual JsonResult GetProductOutOfStockDetails(int productId)
        {
            //TO Do: Implement Logic for check out of stock
            ProductViewModel product = _productAgent.GetProduct(productId);
            if (!product.ShowAddToCart)
            {
                //Show Inventory Error message.
                string errorMessage = product.InventoryMessage;
                return Json(new { status = false, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
            if (IsNull(product))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        //Get Product List By SKU.
        public virtual ActionResult GetProductListBySKU(string query)
        => Json(_productAgent.GetProductList(query), JsonRequestBehavior.AllowGet);

        //Get Selected product item properties.
        public virtual ActionResult GetAutoCompleteItemProperties(int productId)
        => Json(_productAgent.GetAutoCompleteProductProperties(productId), JsonRequestBehavior.AllowGet);

        //Get Selected product item properties by sku.
        public virtual ActionResult GetProductDetailsBySKU(string sku)
        => Json(_productAgent.GetProductDetailsBySKU(sku), JsonRequestBehavior.AllowGet);

        //Get the view of Quick Order pad.
        public virtual ActionResult QuickOrderPad()
        {
            SessionHelper.SaveDataInSession<int>("AutoIndex", Convert.ToInt32(WebStoreConstants.DefaultQuickOrderPadRows));
            return View("_QuickOrderPadView");
        }

        //Add to wishlist method.
        [HttpGet]
        public virtual ActionResult AddToWishList(string productSKU, string addOnProductSKUs = null, bool isRedirectToLogin = false)
        {
            return AddToWishListDetails(productSKU, addOnProductSKUs, isRedirectToLogin);
        }

        [HttpGet]
        protected virtual ActionResult AddToWishListDetails(string productSKU, string addOnProductSKUs, bool isRedirectToLogin, string redirectUrl = "")
        {
            if (!Request.IsAuthenticated)
            {
                if (isRedirectToLogin)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorLoginWishlist));
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(new { status = false, isRedirectToLogin = true, link = $"/User/Login?returnurl={HttpUtility.UrlEncode($"/Product/AddToWishList?productSKU={productSKU}")}", message = WebStore_Resources.ErrorLoginWishlist }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction<HomeController>(x => x.Index());
                }
            }
            bool flag = _accountAgent.CreateWishList(productSKU, addOnProductSKUs);
            if (Request.IsAjaxRequest())
                return Json(new { status = flag, message = flag ? WebStore_Resources.SuccessProductAddWishlist : WebStore_Resources.ErrorProductAddWishlist, style = flag ? "success-msg" : "error-msg", link = "/User/Wishlist", wishListId = GetWishListIdForProduct(productSKU) }, JsonRequestBehavior.AllowGet);

            SetNotificationMessage(flag ? GetSuccessNotificationMessage(WebStore_Resources.SuccessProductAddWishlist) : GetErrorNotificationMessage(WebStore_Resources.ErrorProductAddWishlist));
            if(!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return Redirect(_productAgent.GetProductUrl(productSKU, Url));
        }

        //Add to wishlist method.
        [HttpGet]
        public virtual ActionResult AddToWishListPLP(string productSKU, string addOnProductSKUs = null, bool isRedirectToLogin = false)
        {
            return AddToWishListDetails(productSKU, addOnProductSKUs, isRedirectToLogin, Convert.ToString(Request.UrlReferrer));
        }

        //Get product price.
        [HttpGet]
        public virtual ActionResult GetProductPrice(string productSKU = "", string parentProductSKU = "", string quantity = "", string addOnIds = "", int parentProductId = 0)
        {
            ProductViewModel viewModel = _productAgent.GetProductPriceAndInventory(productSKU, quantity, addOnIds, parentProductSKU, parentProductId);

            return Json(new
            {
                success = viewModel.ShowAddToCart,
                message = viewModel.InventoryMessage,
                Quantity = viewModel.Quantity,
                data = new
                {
                    style = viewModel.ShowAddToCart ? "success" : "error",
                    price = Helper.FormatPriceWithCurrency(viewModel.ProductPrice, viewModel.CultureCode),
                    sku = viewModel?.SKU,
                    productId = parentProductId > 0 ? parentProductId : viewModel.PublishProductId,
                    addOnMessage = viewModel?.AddOns?.Count > 0 ? viewModel.AddOns?.Select(x => x.InventoryMessage)?.FirstOrDefault() : null,
                    isOutOfStock = viewModel?.AddOns?.Count > 0 ? viewModel.AddOns?.Select(x => x.IsOutOfStock)?.FirstOrDefault() : null,
                    productType = !string.IsNullOrEmpty(viewModel?.ProductType) ? viewModel?.ProductType : string.Empty,
                }
            }, JsonRequestBehavior.AllowGet);
        }

        //Get associated bundle products.
        [HttpGet]
        public virtual ActionResult GetBundleProduct(int productId, bool isObsolete = false, List<AssociatedPublishBundleProductViewModel> associatedPublishBundleViewModel = null)
        {
            //To display lead time on PDP for bundle product.
            ViewBag.isObsolete = isObsolete;
            if(associatedPublishBundleViewModel?.Count > 0)
            {
                return View("_BundleProducts", associatedPublishBundleViewModel);
            }
            return View("_BundleProducts", _productAgent.GetBundleProduct(productId));
        }


        //Get Product highlights.
        [ChildActionOnly]
        public virtual ActionResult GetProductHighlights(int productId, string highLightsCodes = "", string sku = "")
        {
            List<HighlightsViewModel> viewModel = _productAgent.GetProductHighlights(productId, highLightsCodes);

            //Assign product id to highlight list.
            viewModel.ForEach(x => x.PublishProductId = productId);
            //Assign SKU to highlight list.
            viewModel.ForEach(x => x.SKU = sku);
            return View("_ProductHighLights", viewModel);
        }

        //Get highlight data.
        public virtual ActionResult GetHighlightInfo(int highLightId, int productId, string sku = "")
        {
            HighlightsViewModel highlight = _productAgent.GetHighlightInfo(highLightId, productId, sku);
            highlight = IsNotNull(highlight) ? highlight : new HighlightsViewModel();
            highlight.PublishProductId = productId;
            return View("HighlightInfo", highlight);
        }

        //Get highlight data by Highlight Code.
        [HttpGet]
        public virtual ActionResult GetHighlightInfoByCode(string highLightCode, int productId, string sku = "")
        {
            HighlightsViewModel highlight = _productAgent.GetHighlightInfoByCode(highLightCode, sku);
            highlight = IsNotNull(highlight) ? highlight : new HighlightsViewModel();
            highlight.PublishProductId = productId;
            if(Request.IsAjaxRequest())
            {
                return Json(new
                {
                    status = true,
                    DisplayPopup = highlight.DisplayPopup,
                    HyperLink = highlight.Hyperlink
                }, JsonRequestBehavior.AllowGet);
            }
            return View("HighlightInfoPLP", highlight);
        }

        [HttpPost]
        //Get configurable data.
        public virtual ActionResult GetConfigurableProduct(ParameterProductModel model)
        {
            ProductViewModel product = _productAgent.GetConfigurableProduct(model);
            product.IsQuickView = model.IsQuickView;
            product.IsProductEdit = !Equals(ViewBag.IsProductEdit, null) ? ViewBag.IsProductEdit : false;
            product.ProductTemplateName = string.IsNullOrEmpty(product.ProductTemplateName) || Equals(product.ProductTemplateName, ZnodeConstant.ProductDefaultTemplate) ? "Details" : product.ProductTemplateName;
            if (model.IsQuickView)
                return ActionView("_QuickViewProductView", product);

            return ActionView(product.ProductTemplateName, product);
        }

        //Get Associated products to product.
        public virtual ActionResult GetGroupProductList(int productId, bool isCallForPricing, bool isObsolete = false, bool isQuickView = false)
        {
            List<GroupProductViewModel> associatedProductList = _productAgent.GetGroupProductList(productId);
            //Price message for group product.
            ViewBag.GroupProductMessage = _productAgent.GetGroupProductMessage(associatedProductList);
            ViewBag.IsMainProductCallForPricing = isCallForPricing;
            ViewBag.IsObsolete = isObsolete;
            ViewBag.isQuickView = isQuickView;
            return ActionView("_GroupProductList", associatedProductList);
        }

        //Check Group Product Inventory.
        public virtual ActionResult CheckGroupProductInventory(int mainProductId, string productSKU = "", string quantity = "")
            => Json(_productAgent.CheckGroupProductInventory(mainProductId, productSKU, quantity), JsonRequestBehavior.AllowGet);

        #region Product Compare
        //Add product to comparison
        [HttpGet]
        public virtual ActionResult GlobalLevelCompareProduct(int productId = 0, int categoryId = 0)
        {
            string message = string.Empty;
            int errorCode;
            List<ProductViewModel> products = new List<ProductViewModel>();
            bool status = _productAgent.GlobalAddProductToCompareList(productId, categoryId, out message, out errorCode);
            if (status)
                products = _productAgent.GetCompareProductsDetails();

            return Json(new
            {
                success = status,
                count = products.Count,
                data = new
                {
                    html = IsNull(products) ? string.Empty : RenderRazorViewToString(ComparableProductsView, products),
                    popuphtml = RenderRazorViewToString(ProductComparePopup,
                    new ProductComparePopUpViewModel { Message = message, ErrorCode = errorCode, ProductId = productId, CategoryId = categoryId, HideViewComparisonButton = products.Count == 1 }),
                }
            }, JsonRequestBehavior.AllowGet);
        }

        //Check Product no of product available for comparison.
        [HttpGet]
        public virtual JsonResult ViewProductComparison()
        {
            bool status = _productAgent.GetCompareProducts();
            string message = string.Empty;
            if (status)
                message = WebStore_Resources.ProductCompareQuantityErrorMessage;

            return Json(new
            {
                success = status,
                message = message,
                data = new
                {
                    popuphtml = RenderRazorViewToString(ProductComparePopup, new ProductComparePopUpViewModel { Message = message }),
                }
            }, JsonRequestBehavior.AllowGet);
        }

        //Get product comparison details.
        public virtual ActionResult ViewComparison()
        {
            ProductCompareViewModel compareProducts = new ProductCompareViewModel();
            compareProducts.ProductList = _productAgent.GetCompareProductsDetails(true);
            if (compareProducts.ProductList.Count <= 0)
                return RedirectToAction("Index", "Home");

            return View("ViewComparison", compareProducts);
        }

        //Remove All the products from comparison
        [HttpGet]
        public virtual ActionResult DeleteComparableProducts()
        {
            _productAgent.DeleteComparableProducts();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public virtual ActionResult RemoveProductFormSession(int productId, string control)
        {
            bool status = false;
            string message = string.Empty;
            ProductCompareViewModel compareProducts = new ProductCompareViewModel();
            if (productId > 0)
                status = _productAgent.RemoveProductFormSession(productId);

            if (Equals(control, "Product"))
            {
                compareProducts.ProductList = _productAgent.GetCompareProductsDetails();
                if (IsNull(compareProducts.ProductList) || compareProducts.ProductList.Count <= 1)
                    message = WebStore_Resources.ProductCompareErrorMessage;
                return Json(new
                {
                    success = status,
                    message = message,
                    count = compareProducts.ProductList.Count,
                    data = new
                    {
                        html = IsNull(compareProducts.ProductList) ? string.Empty : RenderRazorViewToString("_ProductComparisonList", compareProducts.ProductList),
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                compareProducts.ProductList = _productAgent.GetCompareProductsDetails();
                return Json(new
                {
                    success = status,
                    count = compareProducts.ProductList.Count,
                    data = new
                    {
                        html = IsNull(compareProducts.ProductList) ? string.Empty : RenderRazorViewToString("_ComparableProducts", compareProducts.ProductList),
                    }
                }, JsonRequestBehavior.AllowGet);
            }

        }

        //Get Compare Product List
        [HttpGet]
        public virtual ActionResult GetCompareProductList()
        {
            List<ProductViewModel> products = _productAgent.GetCompareProductsDetails();
            return Json(new
            {
                count = products.Count,
                data = new
                {
                    html = IsNull(products) ? string.Empty : RenderRazorViewToString(ComparableProductsView, products),
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult SendComparedProductMail()
        {
            ViewBag.IsCompare = true;
            return PartialView(SendMailPopUp, new ProductCompareViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SendComparedProductMail(ProductCompareViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool isMailSend = _productAgent.SendComparedProductMail(viewModel);
                return Json(new
                {
                    Type = isMailSend ? "success" : "error",
                    Message = isMailSend ? WebStore_Resources.SendMailMessage : WebStore_Resources.ErrorEmailSend
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Type = false ? "success" : "error",
                Message = false ? WebStore_Resources.SendMailMessage : WebStore_Resources.ErrorEmailSend
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Get List Of Recently View Product.
        [HttpGet]
        public virtual ActionResult GetRecentViewProducts(int productId = 0)
        {

            List<RecentViewModel> productList = _productAgent.GetRecentProductList(productId);
            ViewBag.ProductCount = 3;
            return ActionView("_RecentlyViewProduct", productList);
        }

        //Get link products.
        public virtual ActionResult GetLinkProducts(int productId)
        {
            List<LinkProductViewModel> model = _widgetDataAgent.GetLinkProductList(new WidgetParameter { CMSMappingId = productId });
            return PartialView("_LinkProductList", model);
        }

        //Generate new row for quick order pad.
        public virtual ActionResult QuickOrder()
        {
            int index = Convert.ToInt32(SessionHelper.GetDataFromSession<int>("AutoIndex")) + 1;
            SessionHelper.SaveDataInSession<int>("AutoIndex", index);
            return PartialView("_MultipleQuickOrders");
        }

        //Get attribute validations list.
        public virtual ActionResult GetPersonalisedAttributes(int productId = 0, Dictionary<string, string> PersonliseValues = null)
        => ActionView("_PersonalisedAttribute", _attributeAgent.GetAttributeValidationByCodes(productId, PersonliseValues));

        [HttpGet]
        public virtual ActionResult AllReviews(int id)
        {
            string sortingChoice = Request.QueryString["Sort"];
            int pageSize = 0;
            int pageNo = 0;
            Int32.TryParse(Request.QueryString["PageSize"], out pageSize);
            Int32.TryParse(Request.QueryString[WebStoreConstants.PageNumber], out pageNo);
            ProductAllReviewListViewModel reviewModelList = _productAgent.GetAllReviews(id, sortingChoice, pageSize, pageNo);
            return View("AllReviews", reviewModelList);
        }
        //Email To Friend
        public virtual ActionResult EmailToFriend() => View("_EmailToFriend", new EmailAFriendViewModel());

        //
        public virtual void SendMailToFriend(EmailAFriendViewModel emailAFriendViewModel)
        {
            emailAFriendViewModel.ProductUrl = Request.UrlReferrer.OriginalString;
            _productAgent.SendMailToFriend(emailAFriendViewModel);
        }

        //Gets the breadcrumb.
        public virtual ActionResult GetBreadCrumb(int categoryId, string productAssociatedCategoryIds, bool checkFromSession)
        {
            productAssociatedCategoryIds = string.IsNullOrEmpty(productAssociatedCategoryIds) ? null : productAssociatedCategoryIds;
            string breadCrumb = _productAgent.GetBreadCrumb(categoryId, productAssociatedCategoryIds?.Split(','), checkFromSession);
            string seeMore = _productAgent.GetSeeMoreString(breadCrumb);
            return Json(new
            {
                breadCrumb = breadCrumb ?? string.Empty,
                seeMore = seeMore ?? string.Empty,
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult GetProductPrice(object products)
        {
            var _data = JsonConvert.DeserializeObject<List<ProductPriceViewModel>>(((string[])products)[0]);

            List<ProductPriceViewModel> result = _productAgent.GetProductPrice(_data);

            return Json(new { status = true, data = result });
        }

        public virtual JsonResult IsAsyncPrice() => Json(new { status = Helper.IsAsyncPrice });

        [HttpPost]
        public JsonResult GetPriceWithInventory(List<ProductPriceViewModel> productSku)
        {
            if(HelperUtility.IsNull(productSku)) return Json(new { status = false, data = new List<PriceOrInventoryPartialViewModel>() });

            List<ProductPriceViewModel> productPriceViewModel = _productAgent.GetPriceWithInventory(productSku);
            List<PriceOrInventoryPartialViewModel> partialViewModels = new List<PriceOrInventoryPartialViewModel>();
            foreach (ProductPriceViewModel priceViewModel in productPriceViewModel)
            {
                if (!partialViewModels.Any(x => x.SKU == priceViewModel?.sku))
                {
                    partialViewModels.Add(new PriceOrInventoryPartialViewModel() { SKU = priceViewModel?.sku, HtmlText = IsNull(priceViewModel) ? "" : RenderRazorViewToString("_ProductPriceAndInventory", priceViewModel), TierPriceText = IsNull(priceViewModel) ? "" : (priceViewModel.PriceView ? RenderRazorViewToString("_ProductTierPrice", priceViewModel) : "") });
                }
            }
            return Json(new { status = true, data = partialViewModels });
        }

        [HttpPost]
        public virtual ActionResult AddProductsToQuickOrderUsingExcel()
        {

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase importFile = files[i];
                    QuickOrderViewModel returnModel = _productAgent.AddProductsToQuickOrderUsingExcel(importFile);

                    if (IsNotNull(returnModel))
                    {
                        return Json(new
                        {
                            response = new { IsSuccess = returnModel.IsSuccess, ValidSKUCount = returnModel.ValidSKUCount, InvalidSKUCount = returnModel.InvalidSKUCount, ProductSKUText = returnModel.ProductSKUText },
                            rowsHtml = RenderRazorViewToString("_MultipleQuickOrderRow", returnModel),
                            notificationHtml = returnModel.NotificationMessage
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new
            {
                response = new { IsSuccess = false, ValidSKUCount = 0, InvalidSKUCount = 0, ProductSKUText = "" },
                rowsHtml = "",
                notificationHtml = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual JsonResult GetProductDetail(string searchTerm, bool enableSpecificSearch = false)
        {
            bool isRecordFound = false;
            ProductListViewModel productList = _productAgent.GetProductSearch(searchTerm, enableSpecificSearch);

            if (productList?.Products?.Count > 0)
                isRecordFound = true;

            return Json(new
            {
                Type = isRecordFound ? "success" : "error",
                Message = !isRecordFound ? WebStore_Resources.BarcodeInvalidMessage : string.Empty,
                Data = isRecordFound ? productList?.Products?.FirstOrDefault() : null

            }, JsonRequestBehavior.AllowGet);
        }

        #region B2B Theme
        //returns the wishlistId of the recently added product to wishlist for logged in user 
        public virtual int GetWishListIdForProduct(string productSKU)
        {
            UserViewModel userModel = SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            if (IsNull(userModel)) return 0;

            return userModel?.WishList?.FirstOrDefault(x => x.SKU == productSKU)?.UserWishListId ?? 0;
        }
        [HttpGet]
        public virtual JsonResult GetProductInventory(int productId)
        {
            return Json(new { status = true, data = _productAgent.GetProductInventory(productId, true) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult ShowProductAllLocationInventory(int productId)
        {
            return PartialView("_ProductAllLocationInventory", _productAgent.GetProductInventory(productId, true));
        }

        //Display AddToCart notification.
        [HttpPost]
        public virtual ActionResult ShowAddToCartNotification(AddToCartNotificationViewModel product)
        {
            return PartialView("_AddToCartNotification", product);
        }

        // Get associated configurable variants.
        [HttpGet]
        public virtual ActionResult GetAssociatedConfigurableVariants(int productId, bool isObsolete = false, bool isQuickView = false)
        {
            ViewBag.isObsolete = isObsolete;
            ViewBag.isQuickView = isQuickView;
            List<ConfigurableProductViewModel> productList = _productAgent.GetAssociatedConfigurableVariants(productId);
            return ActionView("_ConfigurableProducts", productList);
        }

    // Get variant image of the configurable product. 
    [HttpPost]
        public virtual ActionResult GetConfigurableProductVariantImage(ProductDetailViewModel productDetails)
        {
            ProductViewModel productViewModel = _productAgent.GetProductImage(productDetails);
            return Json(new
            {
                html = RenderRazorViewToString("_ProductImageZoomEffect", productViewModel),
                JsonRequestBehavior.AllowGet
            });
        }

        // Submit stock request.
        [HttpPost]
        public virtual ActionResult SubmitStockRequest(StockNotificationViewModel stockNotificationViewModel)
        {
            bool status = _productAgent.SubmitStockRequest(stockNotificationViewModel);
            return Json(new
            {
                status = status
            });
        }

        // Check configurable child product inventory.
        [HttpGet]
        public virtual ActionResult CheckConfigurableChildProductInventory(int parentProductId, string childSKUs, string childQuantities)
            => Json(_productAgent.CheckConfigurableChildProductInventory(parentProductId, childSKUs, childQuantities), JsonRequestBehavior.AllowGet);

        #endregion


    }

}
