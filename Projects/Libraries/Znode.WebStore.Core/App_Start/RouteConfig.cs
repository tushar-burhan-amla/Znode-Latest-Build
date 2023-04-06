using System.Web.Mvc;
using System.Web.Routing;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            bool runAllManagedModules = ZnodeWebstoreSettings.RunAllManagedModules;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("bundles/{*bundle}");

            routes.MapMvcAttributeRoutes();

            #region Blog/News
            routes.MapRoute("blog-details", "blog/{blogNewsId}", new { controller = "blognews", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), blogNewsId = @"[0-9]+$" });
            routes.MapRoute("news-details", "news/{blogNewsId}", new { controller = "blognews", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), blogNewsId = @"[0-9]+$" });
            #endregion

            #region Brand
            routes.MapRoute("brand-details", "brand/{brand}/{brandId}", new { controller = "brand", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), brand = @"[\w- ]+$", brandId = @"[0-9]+$" });
            routes.MapRoute("brand-detailsid", "brand/{brandId}", new { controller = "brand", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), brandId = @"[0-9]+$" });
            #endregion

            #region Cart
            routes.MapRoute("cart-index", "cart", new { controller = "cart", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("cart-promotionbar", "cart/cartpromotionbar", new { controller = "cart", action = "CartPromotionBar" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("cart-calculate", "cart/calculatecart", new { controller = "cart", action = "CalculateCart" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("cart-update", "cart/updatecartquantity", new { controller = "cart", action = "UpdateCartQuantity" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("cart-updatecart", "cart/updatequantityofcartitem", new { controller = "cart", action = "UpdateQuantityOfCartItem" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("cart-removeitem", "cart/removecartitem", new { controller = "cart", action = "RemoveCartItem" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("cart-removeall", "cart/removeallcartitem", new { controller = "cart", action = "RemoveAllCartItem" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("cart-shippingestimate", "cart/getshippingestimates", new { controller = "cart", action = "GetShippingEstimates" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("cart-calculateshipping", "cart/getcalculatedshipping", new { controller = "cart", action = "GetCalculatedShipping" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("cart-count", "cart/getcartcount", new { controller = "cart", action = "GetCartCount" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region CaseRequest
            routes.MapRoute("caserequest-contactus", "caserequest/contactus", new { controller = "caserequest", action = "ContactUs" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("caserequest-feedbackform", "caserequest/customerfeedback", new { controller = "caserequest", action = "CustomerFeedback" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("caserequest-feedbacksuccess", "caserequest/customerfeedbacksuccess", new { controller = "caserequest", action = "CustomerFeedbackSuccess" }, new { httpMethod = new HttpMethodConstraint("POST") });
            #endregion

            #region Category
            routes.MapRoute("category-breadcrumb", "category/getbreadcrumb/{categoryId}", new { controller = "category", action = "GetBreadCrumb" }, new { httpMethod = new HttpMethodConstraint("GET"), categoryId = @"[0-9]+$" });
            routes.MapRoute("category-details", "category/{category}/{categoryId}", new { controller = "category", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), category = @"[\w- ]+$", categoryId = @"[0-9]+$" });
            routes.MapRoute("seocategory-details", "category/{seo}/{category}", new { controller = "category", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), seo = @"[\w-]+$", category = @"[\w- ]+$" });
            routes.MapRoute("category-detailsId", "category/{categoryId}", new { controller = "category", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), categoryId = @"[0-9]+$" });
            #endregion

            #region Checkout
            routes.MapRoute("checkout-address", "checkout/accountaddress", new { controller = "checkout", action = "AccountAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-refreshaddress", "checkout/refreshaddressoptions", new { controller = "checkout", action = "RefreshAddressOptions" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-shippingoption", "checkout/shippingoptions", new { controller = "checkout", action = "ShippingOptions" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-paymentoption", "checkout/paymentoptions", new { controller = "checkout", action = "PaymentOptions" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-cartreview", "checkout/cartreview", new { controller = "checkout", action = "CartReview" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-discount", "checkout/applydiscount", new { controller = "checkout", action = "ApplyDiscount" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("checkout-getaddress", "checkout/getaddress", new { controller = "checkout", action = "GetAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-editaddress", "checkout/editaddress", new { controller = "checkout", action = "EditAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-changeaddress", "checkout/changeaddress", new { controller = "checkout", action = "ChangeAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-updateaddress", "checkout/updateaddress", new { controller = "checkout", action = "UpdateAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-shippingaddress", "checkout/displayshippingaddress", new { controller = "checkout", action = "DisplayShippingAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getpaymentprovider", "checkout/getpaymentprovider", new { controller = "checkout", action = "GetPaymentProvider" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-removecouponcode", "checkout/removecoupon", new { controller = "checkout", action = "RemoveCoupon" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-paymentdetails", "checkout/getpaymentdetails", new { controller = "checkout", action = "GetPaymentDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-creditcarddetails", "checkout/getpaymentcreditcarddetails", new { controller = "checkout", action = "GetPaymentCreditCardDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-creditcardcount", "checkout/getsavecreditcardcount", new { controller = "checkout", action = "GetSaveCreditCardCount" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getbillingaddressdetail", "checkout/getbillingaddressdetail", new { controller = "checkout", action = "GetBillingAddressDetail" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-submitpaypalorder", "checkout/submitpaypalorder", new { controller = "checkout", action = "SubmitPaypalOrder" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-checkoutreceipt", "checkout/ordercheckoutreceipt", new { controller = "checkout", action = "OrderCheckoutReceipt" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-quotereceipt", "checkout/quotereceipt", new { controller = "checkout", action = "QuoteReceipt" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getshippingbillingaddress", "checkout/getshippingbillingaddress", new { controller = "checkout", action = "GetshippingBillingAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getsearchbillinglocation", "checkout/getsearchbillinglocation", new { controller = "checkout", action = "GetSearchBillingLocation" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getsearchshippinglocation", "checkout/getsearchshippinglocation", new { controller = "checkout", action = "GetSearchShippingLocation" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-setaddressreceipentname", "checkout/setaddressrecipientnameincart", new { controller = "checkout", action = "SetAddressRecipientNameInCart" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getaddressbyid", "checkout/getaddressbyid", new { controller = "checkout", action = "GetAddressById" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-ordernumber", "checkout/generateordernumber", new { controller = "checkout", action = "GenerateOrderNumber" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-createnewaddress", "checkout/createnewaddress", new { controller = "checkout", action = "CreateNewAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-getaddressdetails", "checkout/getaddressdetails", new { controller = "checkout", action = "GetAddressDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-submitamazonorder", "checkout/submitamazonorder", new { controller = "checkout", action = "SubmitAmazonOrder" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-amazoncartreview", "checkout/amazoncartreview", new { controller = "checkout", action = "AmazonCartReview" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-amazonshippingoptions", "checkout/amazonshippingoptions", new { controller = "checkout", action = "AmazonShippingOptions" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("checkout-amazonpaymentoptions", "checkout/amazonpaymentoptions", new { controller = "checkout", action = "AmazonPaymentOptions" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region ContentPage
            routes.MapRoute("content-details", "contentpage/{contentPageId}", new { controller = "ContentPage", action = "ContentPage" }, new { httpMethod = new HttpMethodConstraint("GET"), contentPageId = @"[0-9]+$" });
            routes.MapRoute("contentPage-pageId", "contentpage/{contentPageId}", new { controller = "ContentPage", action = "ContentPage" }, new { httpMethod = new HttpMethodConstraint("GET"), contentPageId = @"[\w- ]+$" });
            #endregion

            #region Dev
            routes.MapRoute("dev-portalselection", "dev/portalselection", new { controller = "dev", action = "PortalSelection" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region DynamicContent
            routes.MapRoute("dynamiccontent", "dynamiccontent/widget", new { controller = "dynamiccontent", action = "Widget" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("dynamiccontent-dynamicstyles", "dynamiccontent/getdynamicstyles", new { controller = "dynamiccontent", action = "GetDynamicStyles" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region ECert
            routes.MapRoute("ecert-inhandecert", "eCert/list", new { controller = "eCert", action = "AvailableECertList" }, new { httpMethod = new HttpMethodConstraint("GET"), contentPageId = @"[\w- ]+$" });
            #endregion

            #region ErrorPage
            routes.MapRoute("errorpage", "errorpage/index", new { controller = "errorpage", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("errorpage-pagenotfound", "errorpage/pagenotfound", new { controller = "errorpage", action = "PageNotFound" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("error-unauthorized", "errorpage/unauthorizederrorrequest", new { controller = "errorpage", action = "UnAuthorizedErrorRequest" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region FormBuilder
            routes.MapRoute("formbuilder", "formbuilder/get", new { controller = "formbuilder", action = "Get" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region Home
            routes.MapRoute("home", "home/index", new { controller = "home", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-about", "home/about", new { controller = "home", action = "About" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-contact", "home/contact", new { controller = "home", action = "Contact" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-storelocator", "home/storelocator", new { controller = "home", action = "StoreLocator" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("home-changelocale", "home/changelocale", new { controller = "home", action = "ChangeLocale" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-getbarcodescanner", "home/getbarcodescanner", new { controller = "home", action = "GetBarcodeScanner" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-offerbanner", "home/offerbanner", new { controller = "home", action = "OfferBanner" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-widgetcategorylist", "home/widgetcategorylist", new { controller = "home", action = "WidgetCategoryList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-widgetbrandlist", "home/widgetbrandlist", new { controller = "home", action = "WidgetBrandList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-newslettersignup", "home/signupfornewsletter", new { controller = "home", action = "SignUpForNewsLetter" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("home-cartcount", "home/getcartcount", new { controller = "home", action = "GetCartCount" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region Import
            routes.MapRoute("import-logdetails", "import/showlogdetails", new { controller = "import", action = "ShowLogDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("import-userlogdetails", "import/showuserlogdetails", new { controller = "import", action = "ShowUserLogDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region Monitor
            routes.MapRoute("monitor", "monitor/index", new { controller = "monitor", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region Product 

            routes.MapRoute("product-briefcontent", "product/briefcontent", new { controller = "product", action = "BriefContent" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-detailscontent", "product/detailscontent", new { controller = "product", action = "DetailsContent" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-extendedcontent", "product/extendedcontent", new { controller = "product", action = "ExtendedContent" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-images", "product/alternateproductimages", new { controller = "product", action = "AlternateProductImages" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-imageszoom", "product/alternateproductimageszoomeffect", new { controller = "product", action = "AlternateProductImagesZoomEffect" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-productinfo", "product/productinfo", new { controller = "product", action = "ProductInfo" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-productquickview", "product/getproductquickview", new { controller = "product", action = "GetProductQuickView" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-recommended", "product/getrecommendedproducts", new { controller = "product", action = "GetRecommendedProducts" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-listbysku", "product/getproductlistbysku", new { controller = "product", action = "GetProductListBySKU" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-completeproperties", "product/getautocompleteitemproperties", new { controller = "product", action = "GetAutoCompleteItemProperties" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-quickorderpad", "product/quickorderpad", new { controller = "product", action = "QuickOrderPad" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-addtowishlist", "product/addtowishlist", new { controller = "product", action = "AddToWishList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-getproductprice", "product/getproductprice", new { controller = "product", action = "GetProductPrice" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-bundleproduct", "product/getbundleproduct", new { controller = "product", action = "GetBundleProduct" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-highlights", "product/getproducthighlights", new { controller = "product", action = "GetProductHighlights" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-highlightsinfo", "product/gethighlightinfo", new { controller = "product", action = "GetHighlightInfo" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-groupproductlist", "product/getgroupproductlist", new { controller = "product", action = "GetGroupProductList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-groupproductinventory", "product/checkgroupproductinventory", new { controller = "product", action = "CheckGroupProductInventory" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-compareproduct", "product/globallevelcompareproduct", new { controller = "product", action = "GlobalLevelCompareProduct" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-viewproductcomparison", "product/viewproductcomparison", new { controller = "product", action = "ViewProductComparison" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-viewcomparison", "product/viewcomparison", new { controller = "product", action = "ViewComparison" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-deletecomparableproducts", "product/deletecomparableproducts", new { controller = "product", action = "DeleteComparableProducts" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-removeproductformsession", "product/removeproductformsession", new { controller = "product", action = "RemoveProductFormSession" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-getcompareproductlist", "product/getcompareproductlist", new { controller = "product", action = "GetCompareProductList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-sendcomparedproductmail", "product/sendcomparedproductmail", new { controller = "product", action = "SendComparedProductMail" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-recentviewproducts", "product/getrecentviewproducts", new { controller = "product", action = "GetRecentViewProducts" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-getlinkproducts", "product/getlinkproducts", new { controller = "product", action = "GetLinkProducts" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-quickorder", "product/quickorder", new { controller = "product", action = "QuickOrder" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-emailfriend", "product/emailtofriend", new { controller = "product", action = "EmailToFriend" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-breadcrumb", "product/getbreadcrumb", new { controller = "product", action = "GetBreadCrumb" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-getproductdetail", "product/getproductdetail", new { controller = "product", action = "GetProductDetail" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-getwishlistforproduct", "product/getwishlistforproduct", new { controller = "product", action = "GetWishListIdForProduct" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("product-details", "product/{id}", new { controller = "product", action = "Details" }, new { httpMethod = new HttpMethodConstraint("GET"), id = @"[0-9]+$" });
            routes.MapRoute("product-reviews-", "product/allreviews/{id}", new { controller = "product", action = "AllReviews" }, new { httpMethod = new HttpMethodConstraint("GET"), id = @"[\w- ]+$" });
            routes.MapRoute("product-writereviews-", "product/writereview/{id}", new { controller = "product", action = "WriteReview" }, new { httpMethod = new HttpMethodConstraint("GET"), id = @"[0-9]+$" });
            routes.MapRoute("edit-product-details", "product/{id}/{parentOmsSavedCartLineItemId}", new { controller = "product", action = "Details" }, new { httpMethod = new HttpMethodConstraint("GET"), id = @"[0-9]+$" });
            routes.MapRoute("product-details-sku", "product/{id}/{sku}", new { controller = "product", action = "Details" }, new { httpMethod = new HttpMethodConstraint("GET"), id = @"[0-9]+$", sku = @"[\w-]+$" });
            routes.MapRoute("seoproduct-details", "product/{seo}/{product}", new { controller = "product", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET"), seo = @"[\w-]+$", product = @"[\w- ]+$" });

            routes.MapRoute("product-getproductinventory", "product/getproductinventory/{productId}", new { controller = "product", action = "GetProductInventory" }, new { httpMethod = new HttpMethodConstraint("GET"), productId = @"[0-9]+$" });
            routes.MapRoute("product-showproductallLocationinventory", "product/showproductallLocationinventory/{productId}", new { controller = "product", action = "ShowProductAllLocationInventory" }, new { httpMethod = new HttpMethodConstraint("GET"), productId = @"[0-9]+$" });

            routes.MapRoute("product-addproductstoquickorder", "product/addproductstoquickorder", new { controller = "product", action = "addproductstoquickorder" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("product-downloadquickordertemplate", "product/downloadquickordertemplate", new { controller = "product", action = "downloadquickordertemplate" }, new { httpMethod = new HttpMethodConstraint("GET") });

            #endregion

            #region Search
            routes.MapRoute("search-productspaging", "search/productspaging", new { controller = "search", action = "ProductsPaging" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("search", "search/index", new { controller = "search", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("search-seodetails", "search/getseourldetails", new { controller = "search", action = "GetSeoUrlDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("search-getsuggestions", "search/getsuggestions", new { controller = "search", action = "GetSuggestions" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("search-getfacets", "search/getfacets", new { controller = "search", action = "GetFacets" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("search-getsubcategories", "search/getsubcategories", new { controller = "search", action = "GetSubCategories" }, new { httpMethod = new HttpMethodConstraint("GET") });

            #endregion

            #region SiteMap
            routes.MapRoute("sitemap-publishproduct", "sitemap/getpublishproduct", new { controller = "sitemap", action = "GetPublishProduct" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region User
            routes.MapRoute("user-login", "user/login", new { controller = "user", action = "Login" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-impersonationlogin", "user/impersonationlogin", new { controller = "user", action = "ImpersonationLogin" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-loginstatus", "user/loginstatus", new { controller = "user", action = "LoginStatus" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-logout", "user/logout", new { controller = "user", action = "Logout" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-changepassword", "user/changepassword", new { controller = "user", action = "ChangePassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-forgotpassword", "user/forgotpassword", new { controller = "user", action = "ForgotPassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-resetpassword", "user/resetpassword", new { controller = "user", action = "ResetPassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-externallogin", "user/externallogin", new { controller = "user", action = "ExternalLogin" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("user-externallogincallback", "user/externallogincallback", new { controller = "user", action = "ExternalLoginCallback" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("user-redirecttolocal", "user/redirecttolocal", new { controller = "user", action = "RedirectToLocal" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-resetwebstorepassword", "user/resetwebstorepassword", new { controller = "user", action = "ResetWebstorePassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-signup", "user/signup", new { controller = "user", action = "Signup" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-dashboard", "user/dashboard", new { controller = "user", action = "Dashboard" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getaccountmenus", "user/getaccountmenus", new { controller = "user", action = "GetAccountMenus" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-wishlist", "user/wishlist", new { controller = "user", action = "Wishlist" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-orderreceipt", "user/orderreceipt", new { controller = "user", action = "OrderReceipt" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getorderdetails", "user/getorderdetails", new { controller = "user", action = "GetOrderDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-reorderproducts", "user/reorderproducts", new { controller = "user", action = "ReorderProducts" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-reorderlineitem", "user/reorderlineitem", new { controller = "user", action = "ReorderOrderLineItem" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-reorderproductslist", "user/reorderproductslist", new { controller = "user", action = "ReorderProductsList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-addressbook", "user/addressbook", new { controller = "user", action = "AddressBook" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-reviews", "user/reviews", new { controller = "user", action = "Reviews" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-editprofile", "user/editprofile", new { controller = "user", action = "EditProfile" }, new { httpMethod = new HttpMethodConstraint("GET") });
           
            routes.MapRoute("user-getsavedcreditcards", "user/getsavedcreditcards", new { controller = "user", action = "GetSavedCreditCards" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-deletecarddetails", "user/deletecarddetails", new { controller = "user", action = "DeleteCardDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-creditcarddetails", "user/creditcarddetails", new { controller = "user", action = "CreditCardDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-address", "user/address", new { controller = "user", action = "Address" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-deleteaddress", "user/deleteaddress", new { controller = "user", action = "DeleteAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-affiliateinformation", "user/affiliateinformation", new { controller = "user", action = "AffiliateInformation" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-downloadpdf", "user/downloadpdf", new { controller = "user", action = "DownloadPDF" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("user-customerslist", "user/customerslist", new { controller = "user", action = "CustomersList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-customeredit", "user/customeredit", new { controller = "user", action = "CustomerEdit" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-customerdelete", "user/customerdelete", new { controller = "user", action = "CustomerDelete" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-customerenabledisable", "user/customerenabledisableaccount", new { controller = "user", action = "CustomerEnableDisableAccount" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getentityattributedetails", "user/getentityattributedetails", new { controller = "user", action = "GetEntityAttributeDetails" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-singleresetpassword", "user/singleresetpassword", new { controller = "user", action = "SingleResetPassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-bulkresetpassword", "user/bulkresetpassword", new { controller = "user", action = "BulkResetPassword" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getpermissionlist", "user/getpermissionlist", new { controller = "user", action = "GetPermissionList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getapproverlist", "user/getapproverlist", new { controller = "user", action = "GetApproverList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getaccountdepartments", "user/getaccountdepartments", new { controller = "user", action = "GetAccountDepartments" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-userapprovallist", "user/userapprovallist", new { controller = "user", action = "UserApprovalList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-ecertificatebalance", "user/ecertificatebalance", new { controller = "user", action = "eCertificateBalance" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-validateuserbudget", "user/validateuserbudget", new { controller = "user", action = "ValidateUserBudget" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-getstates", "user/getstates", new { controller = "user", action = "GetStates" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-changeuserprofile", "user/changeuserprofile", new { controller = "user", action = "ChangeUserProfile" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-setprimarybillingshippingaddress", "user/setprimarybillingshippingaddress", new { controller = "user", action = "SetPrimaryBillingShippingAddress" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("user-istemplateitemsmodified", "user/istemplateitemsmodified", new { controller = "user", action = "IsTemplateItemsModified" }, new { httpMethod = new HttpMethodConstraint("GET") });


            routes.MapRoute("user-powerbireport", "user/powerbireport", new { controller = "user", action = "PowerBIReport" }, new { httpMethod = new HttpMethodConstraint("GET") });

            #endregion

            #region XMLGenerator
            routes.MapRoute("xmlgenerator-createxml", "xmlgenerator/createxml", new { controller = "xmlgenerator", action = "CreateXML" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-getcolumnlist", "xmlgenerator/getcolumnlist", new { controller = "xmlgenerator", action = "GetColumnList" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-savecolumnxml", "xmlgenerator/savecolumnxml", new { controller = "xmlgenerator", action = "SaveColumnXML" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-editapplicationsetting", "xmlgenerator/editapplicationsetting", new { controller = "xmlgenerator", action = "EditApplicationSetting" }, new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("xml-delete", "xmlgenerator/delete", new { controller = "xmlgenerator", action = "Delete" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-autocompleteentityname", "xmlgenerator/autocompleteentityname", new { controller = "xmlgenerator", action = "AutoCompleteEntityName" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-view", "xmlgenerator/view", new { controller = "xmlgenerator", action = "View" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-edit", "xmlgenerator/edit", new { controller = "xmlgenerator", action = "Edit" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-showhidecolumn", "xmlgenerator/showhidecolumn", new { controller = "xmlgenerator", action = "ShowHidecolumn" }, new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("xml-sortaction", "xmlgenerator/sortaction", new { controller = "xmlgenerator", action = "SortAction" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion

            #region RMA
            routes.MapRoute("rma-getorderdetailsforreturn", "rmareturn/getorderdetailsforreturn", new { controller = "rmareturn", action = "getorderdetailsforreturn" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-getreturnlist", "rmareturn/getreturnlist", new { controller = "rmareturn", action = "getreturnlist" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-checkordereligibilityforreturn", "rmareturn/checkordereligibilityforreturn", new { controller = "rmareturn", action = "checkordereligibilityforreturn" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-getreturndetails", "rmareturn/getreturndetails", new { controller = "rmareturn", action = "getreturndetails" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-manageorderreturn", "rmareturn/manageorderreturn", new { controller = "rmareturn", action = "manageorderreturn" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-printreturnreceipt", "rmareturn/printreturnreceipt", new { controller = "rmareturn", action = "printreturnreceipt" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("rma-saveorderreturn", "rmareturn/saveorderreturn", new { controller = "rmareturn", action = "saveorderreturn" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("rma-deleteorderreturn", "rmareturn/deleteorderreturn", new { controller = "rmareturn", action = "deleteorderreturn" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("rma-submitorderreturn", "rmareturn/submitorderreturn", new { controller = "rmareturn", action = "submitorderreturn" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("rma-calculateorderreturn", "rmareturn/calculateorderreturn", new { controller = "rmareturn", action = "calculateorderreturn" }, new { httpMethod = new HttpMethodConstraint("POST") });
            #endregion

            #region TradeCentric
            routes.MapRoute("punchout-createsession", "punchout/createsession", new { controller = "punchout", action = "createsession" }, new { httpMethod = new HttpMethodConstraint("POST") });
            routes.MapRoute("punchout-initializesession", "punchout/initializesession", new { controller = "punchout", action = "initializesession" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("punchout-transfercart", "punchout/transfercart", new { controller = "punchout", action = "transfercart" }, new { httpMethod = new HttpMethodConstraint("GET") });
            routes.MapRoute("punchout-placeorder", "punchout/placeorder", new { controller = "punchout", action = "placeorder" }, new { httpMethod = new HttpMethodConstraint("POST") });
            #endregion

            #region SAVECART
            routes.MapRoute("savedcart-gettemplate", "savedcart/gettemplate", new { controller = "savedcart", action = "GetTemplate" }, new { httpMethod = new HttpMethodConstraint("GET") });
            #endregion
             routes.MapSEORoute(
                name: "SeoSlug",
                url: runAllManagedModules ? "{*slug}" : "{slug}",
                defaults: new { controller = "", action = "", slug = "", ElementId = "" });

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

        }
    }
}
