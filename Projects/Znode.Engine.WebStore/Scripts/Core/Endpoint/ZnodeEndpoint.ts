class Endpoint extends ZnodeBase {

    constructor() {
        super();
    }
    GetProductDetails(productId, isQuickView, publishState, localeId, profileId, accountId, catalogId, callbackMethod) {
        super.ajaxRequest("/Product/GetProductQuickView", Constant.GET, { "id": productId, "isQuickView": isQuickView, "publishState": publishState, "localeId": localeId, "profileId": profileId, "accountId": accountId, "catalogId": catalogId }, callbackMethod, "html");
    }

    GetProductOutOfStockDetails(productId, callbackMethod) {
        super.ajaxRequest("/Product/GetProductOutOfStockDetails", Constant.GET, { "productId": productId }, callbackMethod, "json");
    }

    GetProductListBySKU(sku, callbackMethod) {
        super.ajaxRequest("/Product/GetProductListBySKU", Constant.GET, { "sku": sku }, callbackMethod, "json");
    }

    AddToWishList(sku, addOnSKUs, callbackMethod, isRedirectToLogin = false) {
        super.ajaxRequest("/Product/AddToWishList", Constant.GET, { "productSKU": sku, "addOnProductSKUs": addOnSKUs, "isRedirectToLogin": isRedirectToLogin }, callbackMethod, "json");
    }

    AddToWishListPLP(sku, addOnSKUs, callbackMethod, isRedirectToLogin = false) {
        super.ajaxRequest("/Product/AddToWishListPLP", Constant.GET, { "productSKU": sku, "addOnProductSKUs": addOnSKUs, "isRedirectToLogin": isRedirectToLogin }, callbackMethod, "json");
    }

    GetProductPrice(sku, parentProductSKU, quantity, selectedAddOnIds, parentProductId, callbackMethod) {
        super.ajaxRequest("/Product/GetProductPrice", Constant.GET, { "productSKU": sku, "parentProductSKU": parentProductSKU, "quantity": quantity, "addOnIds": selectedAddOnIds, "parentProductId": parentProductId }, callbackMethod, "json", false);
    }

    GetProduct(parameters, callbackMethod) {
        super.ajaxRequest("/Product/GetConfigurableProduct", Constant.Post, { "model": parameters }, callbackMethod, "html");
    }

    CheckGroupProductInventory(mainProductId, sku, quantity, callbackMethod) {
        super.ajaxRequest("/Product/CheckGroupProductInventory", Constant.GET, { "mainProductId": mainProductId, "productSKU": sku, "quantity": quantity }, callbackMethod, "json", false);
    }

    GlobalLevelProductComapre(productId, categoryId, callbackMethod) {
        super.ajaxRequest("/Product/GlobalLevelCompareProduct", Constant.GET, { "productId": productId, "categoryId": categoryId }, callbackMethod, "json");
    }

    GetProductComparison(callbackMethod) {
        super.ajaxRequest("/Product/ViewProductComparison", Constant.GET, {}, callbackMethod, "json");
    }

    RemoveProduct(productId, control, callbackMethod) {
        super.ajaxRequest("/Product/RemoveProductFormSession", Constant.GET, { "productId": productId, "control": control }, callbackMethod, "json");
    }

    GetCompareProductList(callbackMethod) {
        super.ajaxRequest("/Product/GetCompareProductList", Constant.GET, {}, callbackMethod, "json");
    }

    GetRecentlyViewProduct(productId, callbackMethod) {
        super.ajaxRequest("/Product/GetRecentViewProducts", Constant.GET, { "productId": productId }, callbackMethod, "json");
    }

    UpdateCartQUantity(guid: string, quantity: any, productid: any, callbackMethod) {
        super.ajaxRequest("/cart/UpdateCartQuantity", Constant.Post, { "guid": guid, "quantity": quantity, "productId": productid }, callbackMethod, "html");
    }

    RemoveProductFromWishList(wishlistId, callbackMethod) {
        super.ajaxRequest("/User/Wishlist", Constant.Post, { "wishid": wishlistId }, callbackMethod, "json");
    }

    getView(url, callbackMethod) {
        super.ajaxRequest(url, Constant.GET, {}, callbackMethod, "html");
    }

    SignUpForNewsLetter(emailId, callbackMethod) {
        super.ajaxRequest("/Home/SignUpForNewsLetter", Constant.Post, { "emailId": emailId }, callbackMethod, "json");
    }

    SendMail(callbackMethod) {
        super.ajaxRequest("/Product/SendComparedProductMail", Constant.GET, {}, callbackMethod, "html");
    }

    GetSenMailNotification(url: string, senderMailId: string, recieverMailId: string, callbackMethod) {
        super.ajaxRequest(url, Constant.Post, { "senderMailId": senderMailId, "recieverMailId": recieverMailId }, callbackMethod, "json");
    }

    RemoveCouponCode(couponCode: string, callbackMethod: any) {
        super.ajaxRequest("/Checkout/RemoveCoupon", Constant.GET, { "couponCode": couponCode }, callbackMethod, "json");
    }

    RemoveGiftCard(discountCode: string, callbackMethod: any) {
        super.ajaxRequest("/Checkout/ApplyDiscount", Constant.Post, { "discountCode": discountCode, "isGiftCard": true }, callbackMethod, "json");
    }

    UpdateQuoteStatus(quoteId: string, status: number, callbackMethod) {
        super.ajaxRequest("/User/UpdateQuoteStatus", Constant.GET, { "quoteId": quoteId, "status": status }, callbackMethod, "json");
    }

    DeleteTemplate(omsTemplateId: string, callbackMethod) {
        super.ajaxRequest("/User/DeleteTemplate", Constant.GET, { "omsTemplateId": omsTemplateId }, callbackMethod, "json");
    }

    GetPaymentDetails(paymentSettingId, isAsync: boolean, callbackMethod) {
        super.ajaxRequest("/Checkout/GetPaymentDetails", Constant.GET, { "paymentSettingId": paymentSettingId}, callbackMethod, "json", isAsync);
    }

    GetPaymentDetailsForInvoice(paymentSettingId, isAsync: boolean, isUsedForOfflinePayment: boolean, remainingOrderAmount: number, callbackMethod) {
        super.ajaxRequest("/Checkout/GetPaymentDetails", Constant.GET, { "paymentSettingId": paymentSettingId, "isUsedForOfflinePayment": isUsedForOfflinePayment, "remainingOrderAmount": remainingOrderAmount}, callbackMethod, "json", isAsync);
    }

    GetPaymentDetailsForQuotes(paymentSettingId, isAsync: boolean, quoteNumber: string, callbackMethod) {
        super.ajaxRequest("/Checkout/GetPaymentDetailsForQuotes", Constant.GET, { "paymentSettingId": paymentSettingId, "quoteNumber": quoteNumber }, callbackMethod, "json", isAsync);
    }

    GetBillingAddressDetail(portalId, billingAddressId, shippingAddressId, callbackMethod) {
        super.ajaxRequest("/Checkout/GetBillingAddressDetail", Constant.GET, { "portalId": portalId, "billingAddressId": billingAddressId, "shippingAddressId": shippingAddressId }, callbackMethod, "json", true);
    }
    

    ShippingOptions(isAsync: boolean, isQuoteRequest: boolean, isPendingOrderRequest: boolean, callbackMethod) {
        super.ajaxRequest("/Checkout/ShippingOptions", Constant.GET, { "isQuote": isQuoteRequest, "isPendingOrderRequest": isPendingOrderRequest }, callbackMethod, "html", isAsync);
    }

    GetXml(id, callbackMethod) {
        super.ajaxRequest("/XMLGenerator/View", Constant.GET, { "id": id }, callbackMethod, "html");
    }

    ProcessPayPalPayment(submitPaymentViewModel: any, callbackMethod) {
        super.ajaxRequest("/Checkout/submitorder", Constant.Post, { "submitPaymentViewModel": submitPaymentViewModel }, callbackMethod, "json");
    }

    DeleteQuoteLineItem(omsQuoteLineItemId: any, omsQuoteId: any, quoteLineItemCount: any, orderStatus: any, roleName: any, token: any, callbackMethod) {
        super.ajaxRequest("/User/DeleteQuoteLineItem", Constant.Post, { "omsQuoteLineItemId": omsQuoteLineItemId, "omsQuoteId": omsQuoteId, "quoteLineItemCount": quoteLineItemCount, "orderStatus": orderStatus, "roleName": roleName, "__RequestVerificationToken": token }, callbackMethod, "json");
    }

    CreateQuote(submitQuoteViewModel: any, callbackMethod) {
        super.ajaxRequest("/User/CreateQuote", Constant.Post, { "submitQuoteViewModel": submitQuoteViewModel }, callbackMethod, "json");
    }

    GetPurchanseOrder(paymentType: string, paymentSettingId: number, callbackMethod) {
        super.ajaxRequest("/Checkout/GetPaymentProvider", Constant.GET, { "paymentType": paymentType, "paymentSettingId": paymentSettingId }, callbackMethod, "html");
    }

    GetShippingEstimates(zipCode: string, callbackMethod) {
        super.ajaxRequest("/cart/GetShippingEstimates", Constant.GET, { "zipCode": zipCode }, callbackMethod, "json");
    }

    GetBreadCrumb(categoryId: number, categoryIds: string, checkFromSession: boolean, callbackMethod) {
        super.ajaxRequest("/Product/GetBreadCrumb", Constant.GET, { "categoryId": categoryId, "productAssociatedCategoryIds": categoryIds, "checkFromSession": checkFromSession }, callbackMethod, "json");
    }

    GetSaveCreditCardCount(customerGUID: string, callbackMethod) {
        super.ajaxRequest("/Checkout/GetSaveCreditCardCount", Constant.GET, { "customerGUID": customerGUID }, callbackMethod, "html");
    }

    GetAjaxHeaders(callbackMethod: any): any {
        super.ajaxRequest("/Checkout/GetAjaxHeaders", Constant.GET, {}, callbackMethod, "json", true);
    }

    GetPaymentAppHeader(callbackMethod: any): any {
        super.ajaxRequest("/Checkout/GetPaymentAppHeader", Constant.GET, {}, callbackMethod, "json", true);
    }

    public CallPriceApi(products: string, callbackMethod: any): void {
        super.ajaxRequest("/Product/GetProductPrice", Constant.Post, { "products": products }, callbackMethod, "json");
    }

    public CallInventoryPriceApi(products: any, callbackMethod: any): void {
        super.ajaxRequest("/Product/GetPriceWithInventory", Constant.Post, { "productSku": products }, callbackMethod, "json");
    }

    public IsAsyncPrice(callbackMethod: any): any {
        super.ajaxRequest("/Product/IsAsyncPrice", Constant.Post, {}, callbackMethod, "json");
    }

    IsTemplateNameExist(templateName: string, omsTemplateId: number, callbackMethod) {
        super.ajaxRequest("/User/IsTemplateNameExist", Constant.Post, { "templateName": templateName, "omsTemplateId": omsTemplateId }, callbackMethod, "json", false);
    }

    GetAutoCompleteItemProperties(productId: number, callbackMethod) {
        super.ajaxRequest("/Product/GetAutoCompleteItemProperties", Constant.GET, { "productId": productId }, callbackMethod, "json", false)
    }

    GetProductDetailsBySKU(sku: any, callbackMethod) {
        super.ajaxRequest("/Product/GetProductDetailsBySKU", Constant.GET, { "sku": sku }, callbackMethod, "json", false)
    }

    public GetSiteMapCategory(pageSize: number, pageLength: number, callbackMethod: any): any {
        super.ajaxRequest("/SiteMap/SiteMapList", Constant.GET, { "pageSize": pageSize, "pageLength": pageLength }, callbackMethod, "json");
    }

    public GetUserCommentList(blogNewsId: any, callbackMethod) {
        super.ajaxRequest("/BlogNews/GetUserCommentList", Constant.GET, { "blogNewsId": blogNewsId }, callbackMethod, "html");
    }

    public GetPublishedProductList(pageIndex: number, pageSize: number, callbackMethod) {
        super.ajaxRequest("/SiteMap/GetPublishProduct", Constant.GET, { "pageIndex": pageIndex, "pageSize": pageSize }, callbackMethod, "json");
    }

    public GetAmazonPayAddress(shippingOptionId: number, shippingAddressId: number, shippingCode: string, paymentSettingId: number, paymentApplicationSettingId: number, amazonOrderReferenceId: string, total: number, callbackMethod) {
        super.ajaxRequest("/Checkout/GetAmazonAddress", Constant.GET, { "shippingOptionId": shippingOptionId, "shippingAddressId": shippingAddressId, "shippingCode": shippingCode, "paymentSettingId": paymentSettingId, "paymentApplicationSettingId": paymentApplicationSettingId, "amazonOrderReferenceId": amazonOrderReferenceId, "total": total, callbackMethod }, callbackMethod, "json");
    }
    AmazonShippingOptions(OrderReferenceId: string, paymentSettingId: number, total: string, accesstoken: string, accountNumber: string, shippingMethod: string, callbackMethod) {
        super.ajaxRequest("/Checkout/AmazonShippingOptions", Constant.GET, { "amazonOrderReferenceId": OrderReferenceId, "paymentSettingId": paymentSettingId, "total": total, "accesstoken": accesstoken, "accountNumber": accountNumber, "shippingMethod": shippingMethod }, callbackMethod, "html", true);
    }

    GetCartCount(callbackMethod) {
        super.ajaxRequest("/Home/GetCartCount", Constant.GET, "", callbackMethod, "html", true);
    }

    GetCartCountByProductId(productId: number, callbackMethod) {
        super.ajaxRequest("/cart/GetCartCount", Constant.GET, { "productId": productId }, callbackMethod, "json", true);
    }

    IsAttributeValueUnique(attributeCodeValues: string, id: number, isCategory: boolean, callbackMethod) {
        super.ajaxRequest("/PIM/ProductAttribute/IsAttributeValueUnique", Constant.GET, { "attributeCodeValues": attributeCodeValues, "id": id, "isCategory": isCategory }, callbackMethod, "json", false);
    }

    ValidationView(url: string, attributeTypeId: number, callbackMethod: any) {
        super.ajaxRequest(url, Constant.GET, { "AttributeTypeId": attributeTypeId }, callbackMethod, "html");
    }

    IsGlobalAttributeCodeExist(attributeCode: string, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/IsAttributeCodeExist", Constant.GET, { "attributeCode": attributeCode }, callbackMethod, "json", false);
    }

    IsGlobalAttributeDefaultValueCodeExist(attributeId: number, attributeDefaultValueCode: string, defaultvalueId: number, callbackMethod) {
        super.ajaxRequest("/GlobalAttribute/IsAttributeDefaultValueCodeExist", Constant.GET, { "attributeId": attributeId, "attributeDefaultValueCode": attributeDefaultValueCode, "defaultValueId": defaultvalueId }, callbackMethod, "json", false);
    }

    IsGlobalAttributeValueUnique(attributeCodeValues: string, id: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/FormBuilder/FormAttributeValueUnique", Constant.GET, { "AttributeCodeValues": attributeCodeValues, "Id": id, "EntityType": entityType }, callbackMethod, "json", false);
    }

    GetRecommendedAddress(_addressModel: Znode.Core.AddressModel, callbackMethod) {
        super.ajaxRequest("/User/GetRecommendedAddress", Constant.Post, { "addressViewModel": _addressModel }, callbackMethod, "json", false);
    }

    ImportPost(callbackMethod) {
        super.ajaxRequest("/User/ImportShippingAddress", Constant.Post, {}, callbackMethod, "json");
    }

    GetBrandData(callbackMethod) {
        super.ajaxRequest("/Brand/GetBrandListPopUp", Constant.GET, {}, callbackMethod, "html");
    }

    GetAddressDetails(addressId, callbackMethod) {
        super.ajaxRequest("/Checkout/GetAddressById", Constant.GET, { "addressId": addressId }, callbackMethod, "json", false);
    }

    GetAndSelectAddressDetails(addressId, addressType, callbackMethod, isCalculateCart: boolean = true) {
        super.ajaxRequest("/Checkout/GetAddressById", Constant.GET, { "addressId": addressId, "addressType": addressType, "isCalculateCart": isCalculateCart }, callbackMethod, "json", false);
    }


    RefreshAddressOptions(addressType, callbackMethod, isCalculateCart: boolean = true) {
        super.ajaxRequest("/Checkout/RefreshAddressOptions", Constant.GET, { "addressType": addressType, "isCalculateCart": isCalculateCart }, callbackMethod, "html", false);
    }



    GetApproverList(accountId: number, userId: number, callbackMethod) {
        super.ajaxRequest("/User/GetApproverList", Constant.GET, { "accountId": accountId, "userId": userId }, callbackMethod, "json");
    }

    UpdateSearchAddress(addressViewModel: any, callbackMethod) {
        super.ajaxRequest("/Checkout/UpdateSearchAddress", Constant.Post, { "addressViewModel": addressViewModel }, callbackMethod, "json");
    }

    IsUserNameExist(userName: string, portalId: number, callbackMethod) {
        super.ajaxRequest("/User/IsUserNameExists", Constant.GET, {
            "userName": userName, "portalId": $("#PortalId").val(),
        }, callbackMethod, "json", false);
    }

    GetPermissionList(accountId, accountPermissionAccessId, callbackMethod) {
        super.ajaxRequest("/User/GetPermissionList", Constant.GET, { accountId: accountId, accountPermissionId: accountPermissionAccessId }, callbackMethod, "json", false);
    }

    DeleteAccountCustomers(id: string, callbackMethod) {
        super.ajaxRequest("/User/CustomerDelete", Constant.GET, { "userId": id }, callbackMethod, "json");
    }

    CustomerEnableDisableAccount(accountid: number, id: string, isEnable: boolean, callbackMethod) {
        super.ajaxRequest("/User/CustomerEnableDisableAccount", Constant.GET, { "accountId": accountid, "userId": id, "isLock": isEnable, "isRedirect": false }, callbackMethod, "json");
    }

    SingleResetPassword(id: Number, callbackMethod) {
        super.ajaxRequest("/User/SingleResetPassword", Constant.GET, { "userId": id }, callbackMethod, "json");
    }

    CustomerAccountResetPassword(accountid: number, id: string, callbackMethod) {
        super.ajaxRequest("/User/BulkResetPassword", Constant.GET, { "accountid": accountid, "userId": id }, callbackMethod, "json");
    }

    SelectBrand(brandId: number, callbackMethod) {
        super.ajaxRequest("/CustomBrand/SelectBrand", Constant.GET, { "brandId": brandId }, callbackMethod, "json");
    }
    SearchBrand(searchKeyword: string, callbackMethod) {
        super.ajaxRequest("/CustomBrand/SearchBrandData", Constant.GET, { "searchKeyword": searchKeyword }, callbackMethod, "html");
    }

    DeleteImportLogs(importProcessLogId: string, callbackMethod) {
        super.ajaxRequest("/Import/DeleteLogs", Constant.GET, { "importProcessLogId": importProcessLogId }, callbackMethod, "json");
    }

    IsGlobalValueUnique(attributeCodeValues: string, id: number, entityType: string, callbackMethod) {
        super.ajaxRequest("/CustomUser/IsGlobalAttributeValueUnique", Constant.Post, { "AttributeCodeValues": attributeCodeValues, "Id": id, "EntityType": entityType }, callbackMethod, "json", false);
    }

    GetStates(countryCode, callbackMethod) {
        super.ajaxRequest("/User/GetStates", Constant.GET, { "countryCode": countryCode }, callbackMethod, "json");
    }

    GetCart(shippingId: number, zipCode: string, callbackMethod) {
        super.ajaxRequest("/cart/GetCalculatedShipping", Constant.GET, { "shippingId": shippingId, "zipCode": zipCode }, callbackMethod, "html", true);
    }

    GetshippingBillingAddress(portalId: number, shippingId: number, billingId: number, callbackMethod) {
        super.ajaxRequest("/Checkout/GetshippingBillingAddress", "get", { "portalId": portalId, "shippingId": shippingId, "billingId": billingId }, callbackMethod, "json", true);
    }

    GetUserApproverList(omsQuoteId: number, callbackMethod) {
        super.ajaxRequest("/User/GetUserApproverList", Constant.GET, { "omsQuoteId": omsQuoteId }, callbackMethod, "html");
    }

    SetPrimaryAddress(selectedAddressId: number, addressType: string, callbackMethod) {
        super.ajaxRequest("/User/SetPrimaryBillingShippingAddress", Constant.GET, { "addressId": selectedAddressId, "addressType": addressType }, callbackMethod, "json");
    }

    GetValidateUserBudget(callbackMethod) {
        super.ajaxRequest("/User/ValidateUserBudget", Constant.GET, {}, callbackMethod, "json", false);
    }

    SetAddressRecipientNameInCart(firstName, lastName, addressType, callbackMethod) {
        super.ajaxRequest("/Checkout/SetAddressRecipientNameInCart", Constant.GET, { "firstName": firstName, "lastName": lastName, "addressType": addressType }, callbackMethod, "json", false);
    }

    GenerateOrderNumber(portalId: number, callbackMethod) {
        super.ajaxRequest("/Checkout/GenerateOrderNumber", Constant.GET, { "portalId": portalId }, callbackMethod, "json", false)
    }

    //Region B2B Theme
    Login(returnUrl: string, callbackMethod) {
        super.ajaxRequest("/User/Login", "get", { "returnUrl": returnUrl }, callbackMethod, "html", false);
    }

    Logoff(callback) {
        super.ajaxRequest("/User/Logout", "get", {}, callback, "html", false);
    }
    GetAccountMenus(callbackMethod) {
        super.ajaxRequest("/User/GetAccountMenus", "get", {}, callbackMethod, "html", false);
    }

    ForgotPassword(callbackMethod) {
        super.ajaxRequest("/User/ForgotPassword", "get", {}, callbackMethod, "html", false);
    }

    RemoveFromWishList(wishListId, callbackMethod) {
        super.ajaxRequest("/User/Wishlist", "get", { "wishid": wishListId }, callbackMethod, "json");
    }

    GetCategoryBreadCrumb(categoryId, callbackMethod) {
        super.ajaxRequest("/Category/GetBreadCrumb/" + categoryId, Constant.GET, {}, callbackMethod, "json");
    }


    PaymentOptions(isAsync: boolean, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/Checkout/PaymentOptions", Constant.GET, { "isQuote": isQuote }, callbackMethod, "html", isAsync);
    }

    GetLoginUserAddress(userid: number, quoteId: number, callbackMethod) {
        super.ajaxRequest("/Checkout/AccountAddress", Constant.GET, { "userid": userid, "quoteId": quoteId }, callbackMethod, "html", false);
    }

    GetcartReview(shippingOptionId: number, shippingAddressId: number, shippingCode: string, callbackMethod) {
        super.ajaxRequest("/Checkout/GetCartDetails", Constant.GET, { "shippingOptionId": shippingOptionId, "shippingAddressId": shippingAddressId, "shippingCode": shippingCode }, callbackMethod, "json", false);
    }

    ChangeUserProfile(profileId, callbackMethod) {
        super.ajaxRequest("/User/ChangeUserProfile", Constant.GET, { profileId: profileId }, callbackMethod, "json", false);
    }

    GetBarcodeScanner(callbackMethod) {
        super.ajaxRequest("/Home/GetBarcodeScanner", Constant.GET, "", callbackMethod, "html");
    }

    GetProductDetail(searchTerm, enableSpecificSearch, callbackMethod) {
        super.ajaxRequest("/Product/GetProductDetail", Constant.GET, { "searchTerm": searchTerm, "enableSpecificSearch": enableSpecificSearch }, callbackMethod, "html");
    }

    SaveSearchReportData(model, callbackMethod) {
        super.ajaxRequest("/SearchReport/SaveSearchReportData", "post", { "model": model }, callbackMethod, "json");
    }

    GetSearchCMSPages(searchTerm, pageNumber, pageSize, callbackMethod) {
        super.ajaxRequest("/Search/GetSearchContentPage", "post", { "searchTerm": searchTerm, "pageNumber": pageNumber, "pageSize": pageSize }, callbackMethod, "html");
    }

    GetOrderDetailsForReturn(orderNumber: string, callbackMethod) {
        super.ajaxRequest("/RMAReturn/GetOrderDetailsForReturn", Constant.GET, { "orderNumber": orderNumber }, callbackMethod, "html", false);
    }

    DeleteOrderReturn(returnNumber: string, callbackMethod) {
        super.ajaxRequest("/RMAReturn/DeleteOrderReturn", Constant.Post, { "returnNumber": returnNumber }, callbackMethod, "json", false);
    }

    SubmitOrderReturn(url,orderReturnModel, callbackMethod) {
        super.ajaxRequest(url, Constant.Post, { "returnViewModel": orderReturnModel }, callbackMethod, "json", false);
    }

    CalculateOrderReturn(url, calculateOrderReturnModel, callbackMethod) {
        super.ajaxRequest(url, Constant.Post, { "calculateOrderReturnModel": calculateOrderReturnModel }, callbackMethod, "json", false);
    }

    SaveOrderReturn(orderReturnModel, callbackMethod) {
        super.ajaxRequest("/RMAReturn/SaveOrderReturn", Constant.Post, { "returnViewModel": orderReturnModel }, callbackMethod, "json", false);
    }

    PrintReturnReceipt(url, returnNumber, isReturnDetailsReceipt, callbackMethod) {
        super.ajaxRequest(url, Constant.GET, { "returnNumber": returnNumber, "isReturnDetailsReceipt": isReturnDetailsReceipt }, callbackMethod, "html", false);
    }    

    public GetAllLocationInventory(productId: any, callbackMethod: any): void {
        super.ajaxRequest("/Product/GetProductInventory", Constant.GET, { "productId": productId }, callbackMethod, "json");
    }

    public ShowProductAllLocationInventory(productId: any, callbackMethod: any): void {
        super.ajaxRequest("/Product/ShowProductAllLocationInventory", Constant.GET, { "productId": productId }, callbackMethod, "html");
    } 
    

    public DisplayAddToCartNotification(product, callbackMethod) {
        super.ajaxRequest("/Product/ShowAddToCartNotification", "post", { "product": JSON.parse(product) }, callbackMethod, "html");
    }

    public IsTemplateItemsModified(templateId, callbackMethod) {
        super.ajaxRequest("/User/IsTemplateItemsModified", Constant.GET, { "omsTemplateId": templateId }, callbackMethod, "json");
    }

    RemoveVoucher(code: string, callbackMethod: any) {
        super.ajaxRequest("/Checkout/RemoveVoucher", Constant.GET, { "voucherNumber": code }, callbackMethod, "json");
    }

    GetHighlightInfoByCode(highlightCode, publishProductId, productSku, callbackMethod) {
        super.ajaxRequest("/Product/GetHighlightInfoByCode", Constant.GET, { "highLightCode": highlightCode, "productId": publishProductId, "sku": productSku }, callbackMethod, "json");
    }

    public AddProductsToQuickOrder(multipleItems: string, callbackMethod) {
        super.ajaxRequest("/Product/AddProductsToQuickOrder", Constant.Post, { "multipleItems": multipleItems }, callbackMethod, "json", true)
    }

    AmazonPaymentOptions(isQuotes, callbackMethod: any) {
        super.ajaxRequest("/Quote/AmazonPaymentOptions", Constant.GET, { "isQuotes": isQuotes}, callbackMethod, "json");
    }

    public ValidateGuestUserReturn(orderNumber: any, callbackMethod: any): void {
        super.ajaxRequest("/User/ValidateGuestUserReturn", Constant.GET, { "orderNumber": orderNumber }, callbackMethod, "json");
    }

    public CheckOrderEligibilityForReturn(orderNumber: any, callbackMethod: any): void {
        super.ajaxRequest("/User/CheckOrderEligibilityForReturn", Constant.GET, { "orderNumber": orderNumber }, callbackMethod, "json");
    }

    GetImage(_productImageDetails: Znode.Core.ProductImageDetailModel, callbackMethod) {
        super.ajaxRequest("/Product/GetConfigurableProductVariantImage", Constant.Post, { "productDetails": _productImageDetails }, callbackMethod, "json", false);
    }

    public GetOrderAndPaymentDetails(omsOrderId: any, portalId: any, callbackMethod): void {
        super.ajaxRequest("/Checkout/GetOrderAndPaymentDetails", Constant.GET, { "omsOrderId": omsOrderId, "portalId": portalId }, callbackMethod, "json");
    }

    SubmitStockRequest(stockRequestModel: Znode.Core.StockNotificationModel, callbackMethod) {
        super.ajaxRequest("/Product/SubmitStockRequest", Constant.Post, { "stockNotificationViewModel": stockRequestModel }, callbackMethod, "json");
    }

    CheckConfigurableChildProductQuantity(parentProductId, childSKUs, childQuantities, callbackMethod) {
        super.ajaxRequest("/Product/CheckConfigurableChildProductInventory", Constant.GET, { "parentProductId": parentProductId, "childSKUs": childSKUs, "childQuantities": childQuantities }, callbackMethod, "json", false);
    }

    GetAuthorizeNetToken(paymentTokenModel, callbackMethod) {
            super.ajaxRequest("/checkout/GetAuthorizeNetToken", Constant.Post, { "paymentTokenModel": paymentTokenModel }, callbackMethod, "json");

    }

    GetPaymentGatewayToken(paymentTokenModel, callbackMethod: any) {
        super.ajaxRequest("/Checkout/GetPaymentGatewayToken", Constant.Post, { "paymentTokenModel": paymentTokenModel }, callbackMethod, "json");
    }
   
    SavedNewcart(templatename: string,callbackMethod : any) {
        super.ajaxRequest("/savedcart/createsavedcart", Constant.Post, { "templatename": templatename }, callbackMethod, "json", false);
    }

    GetSavedCartList(callbackMethod: any) {
        super.ajaxRequest("/savedcart/GetTemplate", Constant.GET, {}, callbackMethod, "json", false);
    }

    EditSaveCart(omsTemplateId, callbackMethod: any) {
        super.ajaxRequest("/savedcart/EditSaveCart", Constant.Post, { "omsTemplateId": omsTemplateId }, callbackMethod, "json", false);
    }

    EditSavedCartName(templateName, templateid, callbackMethod: any)
    {
        super.ajaxRequest("/savedcart/EditSaveCartName", Constant.GET, { "templateName": templateName, "templateid": templateid}, callbackMethod, "json", false);
    }

   
    DownloadMediaById(mediaId : any, callbackMethod: any)
    {
        super.ajaxRequest("/home/DownloadMediaById/", Constant.GET, { "mediaId": mediaId }, callbackMethod, "json", false);
    }

    DownloadMediaByGuid(mediaGuid : string, callbackMethod: any) {
        super.ajaxRequest("/home/DownloadMediaByGuid/", Constant.GET, { "mediaGuid": mediaGuid, }, callbackMethod, "json", false)
    }

    GetIframeViewWithToken(paymentTokenModel, partialView, callbackMethod) {
        super.ajaxRequest("/checkout/GetIframeViewWithToken", Constant.Post, { "paymentTokenModel": paymentTokenModel, "partialView": partialView }, callbackMethod, "json");
    }
    //End Region
}