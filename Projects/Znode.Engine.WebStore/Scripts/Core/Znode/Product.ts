var isFromCategoryPage: string;
var isAddToCartGroupProduct: boolean = true;

class Product extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        isFromCategoryPage = localStorage.getItem("isFromCategoryPage");
        Product.prototype.GetProductBreadCrumb(parseInt(window.sessionStorage.getItem("lastCategoryId"), 10), false);
        Product.prototype.ActiveReadReviews();
        Product.prototype.DisableAddToCartForConfigAndGroup();
    }

    GetProductDetails(control): any {
        $('#quick-view-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");
        var productId = control.dataset.value;
        var isQuickView = control.dataset.isquickview;
        let publishState = control.dataset.publishState;
        var localeId: number = parseInt($("#hdnLocaleId").val());
        var profileId: number = parseInt($("#hdnProfileId").val());
        var accountId: number = parseInt($("#hdnAccountId").val());
        var catalogId: number = parseInt($("#hdnCatalogId").val());
        Endpoint.prototype.GetProductDetails(productId, isQuickView, publishState, localeId, profileId, accountId, catalogId, function (res) {
            if (res != null && res != "") {
                $("#quick-view-content").html(res);
                isFromCategoryPage = localStorage.getItem("isFromCategoryPage");
                Product.prototype.GetProductBreadCrumb(parseInt(window.sessionStorage.getItem("lastCategoryId"), 10), true);
            }
        });
    }

    GetProductOutOfStockDetails(control, e): any {
        e.preventDefault();
        var productId = $(control).parent().find("#dynamic-productid").val();
        Endpoint.prototype.GetProductOutOfStockDetails(productId, function (response) {
            if (response.status) {
                Product.prototype.ShowHideWishlistErrorMsg(control, false, response);
                $(control).closest("form").submit();
            }
            else {
                Product.prototype.ShowHideWishlistErrorMsg(control, true, response);
                return false;
            }
        });
    }
    //if true then show wishlist error message otherwise hide error message
    ShowHideWishlistErrorMsg(control, isShow, response): any {
        var errorMessageField = $(control).parent().parent().find("#wishlist-error-msg");
        errorMessageField.text("");
        isShow ? errorMessageField.text(response.errorMessage) : "";
        isShow ? errorMessageField.addClass("error-msg") : errorMessageField.removeClass("error-msg");
        isShow ? $(control).prop("disabled", true) : $(control).prop("disabled", false);
    }

    AddToWishList(control): any {
        var sku = $(control).attr("data-sku");
        var addOnSKUs = Product.prototype.GetSelectedAddons();
        Endpoint.prototype.AddToWishList(sku, addOnSKUs.join(), function (res) {
            if (res.status) {
                $("#accountWishList").attr('href', res.link);
                $("#accountWishList").attr('class', res.style);
                $("#accountWishList").html(res.message);
                $("#accountWishList_" + control.dataset.id).attr('href', res.link);
                $("#accountWishList_" + control.dataset.id).attr('class', res.style);
                $("#accountWishList_" + control.dataset.id).text(res.message);
            } else {
                if (res.isRedirectToLogin) {
                    // document.location = res.link;
                    document.location.href = res.link;
                }
            }
        }, true);
    }

    OnQuantityChange(): boolean {
        var flag = true;
        var cartCount = 0;
        $("#quantity-error-msg").text('');
        var productId: number = parseInt($("#scrollReview form").children("#dynamic-productid").val());
        var _productDetail = Product.prototype.BindProductModelData();
        //To Do:This call is taking time if session does not contain value for cart.
        ////Getting cart count data for the product to perform validation while adding more product quantity in cart.
        //Endpoint.prototype.GetCartCountByProductId(productId, function (response) {
        //    cartCount = parseInt(response);
        //});

        cartCount = parseInt(_productDetail.Quantity);
        if (this.CheckIsNumeric(_productDetail.Quantity, productId, _productDetail.QuantityError)) {
            if (this.CheckDecimalValue(_productDetail.DecimalPoint, _productDetail.DecimalValue, _productDetail.InventoryRoundOff, productId, _productDetail.QuantityError)) {
                if (this.CheckQuantityGreaterThanZero(_productDetail.MaxQuantity, _productDetail.MinQuantity, cartCount, productId, _productDetail.QuantityError)) {
                    flag = false;
                    $("#button-addtocart_" + productId).prop("disabled", false);

                    Product.prototype.UpdateProductVariations(false, _productDetail.SKU, _productDetail.MainProductSKU, _productDetail.Quantity, _productDetail.MainProductId, function (response) {
                        var salesPrice = response.data.price;
                        flag = Product.prototype.UpdateProductValues(response, _productDetail.Quantity);
                        if (flag == true) {
                            flag = Product.prototype.InventoryStatus(response);
                        }
                    });
                }
                else {
                    flag = false;
                }
            }
            else {
                flag = false;
            }
        }
        else {
            flag = false;
        }
        return flag;
    }

    OnAssociatedProductQuantityChange(): boolean {
        $("#QuickViewQuantiyErrorMessage").text('');
        var isAddToCartArray = [];
        $("#dynamic-product-variations .quantity").each(function () {
            var productId: number = parseInt($("#scrollReview form").children("#dynamic-productid").val());
            var _productDetail = Product.prototype.BindProductModel(this, true);
            var _showAddToCart = $("#ShowAddToCart").val();
            if (_productDetail.Quantity != null && _productDetail.Quantity != "") {
                if (Product.prototype.CheckIsNumeric(_productDetail.Quantity, productId, _productDetail.QuantityError)) {
                    if (Product.prototype.CheckDecimalValue(_productDetail.DecimalPoint, _productDetail.DecimalValue, _productDetail.InventoryRoundOff, productId, _productDetail.QuantityError)) {
                        if (Product.prototype.CheckQuantityGreaterThanZero(_productDetail.MaxQuantity, _productDetail.MinQuantity, parseInt(_productDetail.Quantity), productId, _productDetail.QuantityError)) {
                            if (_showAddToCart != "False") {
                                $("#button-addtocart_" + productId).prop("disabled", false);
                            }
                            $(_productDetail.QuantityError).text("");
                            $(_productDetail.QuantityError).removeClass("error-msg");
                            isAddToCartArray.push(true);
                        }
                        else {
                            isAddToCartArray.push(false);
                        }
                    }
                    else {
                        isAddToCartArray.push(false);
                    }
                }
                else {
                    isAddToCartArray.push(false);
                }
            }
            else {
                if (_showAddToCart != "False") {
                    $("#button-addtocart_" + productId).prop("disabled", false);
                }
                $(_productDetail.QuantityError).text("");
                $(_productDetail.QuantityError).removeClass("error-msg");
                isAddToCartArray.push(true);
            }
        });
        isAddToCartGroupProduct = !($.inArray(false, isAddToCartArray) > -1);
        return isAddToCartGroupProduct;
    }

    BindProductModel(control, isGroup: boolean): Znode.Core.ProductDetailModel {
        var _productDetail: Znode.Core.ProductDetailModel = {
            MainProductId: parseInt($(control).attr("data-parentProductId")),
            InventoryRoundOff: parseInt($(control).attr("data-inventoryroundoff")),
            ProductId: parseInt($(control).attr('data-productId')),
            Quantity: $(control).val(),
            MaxQuantity: parseInt($(control).attr("data-maxquantity")),
            MinQuantity: parseInt($(control).attr("data-minquantity")),
            SKU: $(control).attr("data-sku"),
            MainProductSKU: $(control).attr("data-parentsku"),
            DecimalPoint: $(control).val().split(".")[1] != null ? $(control).val().split(".")[1].length : 0,
            DecimalValue: $(control).val().split(".")[1] != null ? $(control).val().split(".")[1] : 0,
            QuantityError: isGroup ? "#quantity-error-msg_" + $(control).attr('data-productId') : "#quantity-error-msg"
        };
        return _productDetail;
    }

    BindProductModelData(): Znode.Core.ProductDetailModel {
        var isGroup = false;
        var _productDetail: Znode.Core.ProductDetailModel = {
            MainProductId: parseInt($("#Quantity").attr("data-parentProductId")),
            InventoryRoundOff: parseInt($("#Quantity").attr("data-inventoryroundoff")),
            ProductId: parseInt($("#Quantity").attr('data-productId')),
            Quantity: $("#Quantity").val(),
            MaxQuantity: parseInt($("#Quantity").attr("data-maxquantity")),
            MinQuantity: parseInt($("#Quantity").attr("data-minquantity")),
            SKU: $("#Quantity").attr("data-sku"),
            MainProductSKU: $("#Quantity").attr("data-parentsku"),
            DecimalPoint: $("#Quantity").val().split(".")[1] != null ? $("#Quantity").val().split(".")[1].length : 0,
            DecimalValue: $("#Quantity").val().split(".")[1] != null ? $("#Quantity").val().split(".")[1] : 0,
            QuantityError: isGroup ? "#quantity-error-msg_" + $("#Quantity").attr('data-productId') : "#quantity-error-msg"
        };
        return _productDetail;
    }

    BindGroupProductModelData(): Znode.Core.AddOnDetails {
        var _productDetail: Znode.Core.AddOnDetails = {
            SKU: $("#dynamic-sku").val(),
            ParentSKU: $("#dynamic-sku").val(),
            Quantity: "0",
            ParentProductId: $("#dynamic-parentproductid").val()
        };
        return _productDetail;
    }

    CheckDecimalValue(decimalPoint: number, decimalValue: number, inventoryRoundOff: number, productId: number, quantityError: string): boolean {
        if (decimalValue != 0 && decimalPoint > inventoryRoundOff) {
            $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityHaving") + inventoryRoundOff + ZnodeBase.prototype.getResourceByKeyName("XNumbersAfterDecimalPoint"));
            $(quantityError).addClass("error-msg");
            return false;
        }
        return true;
    }

    CheckIsNumeric(selectedQty: string, productId: number, quantityError: string): boolean {
        var matches = selectedQty.match(/^-?[\d.]+(?:e-?\d+)?$/);
        if (matches == null) {
            $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            $(quantityError).addClass("error-msg");
            return false;
        }
        return true;
    }

    CheckQuantityGreaterThanZero(maxQuantity: number, minQuantity: number, selectedQty: number, productId: number, quantityError: string): boolean {
        if (selectedQty == 0) {
            $("#dynamic-inventory").text("");
            $(quantityError).addClass("error-msg");
            $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("ErrorProductQuantity"));
            return false;
        }
        if (maxQuantity < selectedQty || minQuantity > selectedQty) {
            $("#dynamic-inventory").text("");
            $(quantityError).addClass("error-msg");
            $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + minQuantity + ZnodeBase.prototype.getResourceByKeyName("To") + maxQuantity);
            return false;
        }
        return true;
    }

    InventoryStatus(response): boolean {
        // In stock
        var invetoryMessge: string;
        var productType: string;
        if (response.data != null)
            productType = response.data.productType;
        if (productType == "" || productType != "BundleProduct" && productType != "SimpleProduct" && productType != "GroupedProduct") {
            invetoryMessge = response.message + '<span class="product-count padding-left-5">(' + response.Quantity + ')</span>';
        } else {
            invetoryMessge = response.message;
        }
        if (response.success) {
            $("#dynamic-inventory").removeClass("error-msg");
            $("#dynamic-inventory").addClass("success-msg");
            // In stock
            $("#dynamic-inventory").show().html(invetoryMessge);
            $("#button-addtocart_" + response.data.productId).prop("disabled", false);
            $("#product-details-quantity input[name='Quantity']").attr('data-change', 'false');
            return true;
        } else {
            $("#dynamic-inventory").removeClass("success-msg");
            $("#dynamic-inventory").addClass("error-msg");
            // Out of stock
            $("#dynamic-inventory").show().html(invetoryMessge);
            return false;
        }
    }

    RefreshPrice(amount): void {
        $("#product_Detail_Price_Div").show();
        $("#layout-product .dynamic-product-price").html(amount);
    }

    OnAddonSelect(control: any): void {
        var _productSKU: Znode.Core.AddOnDetails;
        _productSKU = Product.prototype.GetGroupProductSKUQuantity(control);
        if ($("#dynamic-producttype").val() == "ConfigurableProduct") {
            _productSKU = ConfigurableProduct.prototype.GetConfigurableProductSKUQuantity(control);
        }
        if (_productSKU != null && _productSKU.SKU != null && _productSKU.Quantity != null) {
            Product.prototype.UpdateProductVariations(false, _productSKU.SKU, _productSKU.ParentSKU, _productSKU.Quantity, _productSKU.ParentProductId, function (response) {
                var salesPrice = response.data.price;
                var flag = Product.prototype.UpdateProductValues(response, _productSKU.Quantity);
                Product.prototype.RefreshPrice(salesPrice);
                if (flag) {
                    Product.prototype.InventoryStatus(response);
                }
                Product.prototype.RemoveAddonRequired(control);
            });
        }
        else {
            if ($("#quick-view-popup-ipad").is(":visible")) {
                $("#QuickViewQuantiyErrorMessage").html(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityError"));
            } else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityError"), "error", true, fadeOutTime);
            }
            $("#dynamic-inventory").show().text('');
            $(".AddOn").val('');
            if ($(control).attr('type') == "checkbox") {
                $(control).prop("checked", false);
            }
        }
    }
    CheckGroupProductAddonQuantity(): boolean {
        var flag = true;
        if (Product.prototype.getAddOnIds("").length > 0) {
            var _productSKU = Product.prototype.BindGroupProductModelData();
            if (_productSKU != null && _productSKU.SKU != null && _productSKU.Quantity != null) {
                Product.prototype.UpdateProductVariations(false, _productSKU.SKU, _productSKU.ParentSKU, _productSKU.Quantity, _productSKU.ParentProductId, function (response) {
                    flag = Product.prototype.UpdateProductValues(response, _productSKU.Quantity);
                    if (flag == true) {
                        flag = Product.prototype.InventoryStatus(response);
                    }
                });
            }
            return flag;
        }
        else {
            return flag;
        }
    }

    RemoveAddonRequired(control): void {
        var addOnId = $(control).data("addongroupid");
        var errormsgdivid = $(control).data("errormsgdivid");
        $("#" + errormsgdivid + addOnId).css("display", "none");
    }

    GetGroupProductSKUQuantity(control): Znode.Core.AddOnDetails {
        var _productSKU: Znode.Core.AddOnDetails
        $("input[type=text].quantity").each(function () {
            if ($(this).attr("name") != "Quantity") {
                var groupProductQuantity = $(this).val();
                if (groupProductQuantity != null && groupProductQuantity != "") {
                    _productSKU = {
                        Quantity: groupProductQuantity,
                        SKU: $(this).attr("data-sku"),
                        ParentSKU: $(control).attr("data-sku"),
                        ParentProductId: parseInt($("#dynamic-parentproductid").val()),
                    }
                    return false;
                }
            }
        });
        if (_productSKU == null) {
            _productSKU = {
                Quantity: $("#Quantity").val(),
                SKU: $("#Quantity").attr("data-sku"),
                ParentSKU: $("#Quantity").attr("data-parentsku"),
                ParentProductId: parseInt($("#Quantity").attr("data-parentProductId")),
            }
        }
        return _productSKU;
    }

    getAddOnIds(parentSelector): any {
        var selectedAddOnIds = [];
        if (typeof parentSelector == "undefined") { parentSelector = ""; }
        $(parentSelector + " select.AddOn").each(function () {
            if ($(this).val() != "0") {
                selectedAddOnIds.push($(this).val());
            }
        });

        $(parentSelector + " input.AddOn:checked").each(function () {
            if ($(this).val() != "0") {
                if ($(this).val() === '' || $(this).val() === undefined) {
                    $(this).val($(this).data('value'));
                }
                selectedAddOnIds.push($(this).val());
            }
        });
        return (selectedAddOnIds.join());
    }

    UpdateProductVariations(htmlContainer: boolean, sku: string, parentProductSKU: string, quantity: string, parentProductId: number, callbackMethod): any {
        var selectedAddOnIds = Product.prototype.getAddOnIds("");
        Endpoint.prototype.GetProductPrice(sku, parentProductSKU, quantity, selectedAddOnIds, parentProductId, function (res) {

            if (callbackMethod) {
                callbackMethod(res);
            }
        });
    }

    UpdateProductValues(response, quantity): any {
        var selectedAddOnIds = Product.prototype.getAddOnIds("");
        // Set form values for submit
        $("#dynamic-sku").val(response.data.sku);
        $("#Quantity").val(quantity);
        $("#dynamic-addons").val(selectedAddOnIds);
        $("input[name='AddOnValueIds']").val(selectedAddOnIds);
        $("#dynamic-productName").val(response.data.ProductName);
        if (response.data.addOnMessage != undefined) {
            $("#dynamic-addOninventory").show();
            $("#dynamic-addOninventory").html(response.data.addOnMessage);
            return false;
        }
        else {
            $("#dynamic-addOninventory").hide();
            $("#dynamic-addOninventory").html("");
            return true;
        }
    }

    OnAttributeSelect(control): any {
        var productId = $("#scrollReview form").children("#dynamic-parentproductid").val();
        var Codes = [];
        var Values = [];
        var sku = $("#dynamic-configurableproductskus").val();
        var ParentProductSKU = $("#dynamic-sku").val();
        var selectedCode = $(control).attr('code');
        var selectedValue = $(control).val();
        var existingBreadCrumbHtml = "";
        var existingSeeMoreHtml = "";

        $("select.ConfigurableAttribute").each(function () {
            Values.push($(this).val());
            Codes.push($(this).attr('id'));
        });

        $(" input.ConfigurableAttribute:checked").each(function () {
            Values.push($(this).val());
            Codes.push($(this).attr('code'));
        });
        var catgoryIds: string = $("#categoryIds").val();
        var parameters = {
            "SelectedCode": selectedCode,
            "SelectedValue": selectedValue,
            "SKU": sku,
            "Codes": Codes.join(),
            "Values": Values.join(),
            "ParentProductId": productId,
            "ParentProductSKU": ParentProductSKU,
            "IsQuickView": $("#isQuickView").val(),
            "IsProductEdit": $('#isProductEdit').val(),
            "ParentOmsSavedCartLineItemId": $('#ParentOmsSavedCartLineItemId').val()
        }

        if ($("#breadCrumb") != undefined && $("#breadCrumb").length > 0 && $("#breadCrumb").html().length > 0) {
            existingBreadCrumbHtml = $("#breadCrumb").html();
        }

        if ($("#seeMore") != undefined && $("#seeMore").length > 0 && $("#seeMore").html().length > 0) {
            existingSeeMoreHtml = $("#seeMore").html();
        }

        Endpoint.prototype.GetProduct(parameters, function (res) {
            $("#layout-product").replaceWith(res);
            if (existingBreadCrumbHtml.length > 0) {
                $("#breadCrumb").html(existingBreadCrumbHtml)
                $("#seeMore").html(existingSeeMoreHtml)
            } else {
                $("#categoryIds").val(catgoryIds);
                isFromCategoryPage = localStorage.getItem("isFromCategoryPage");
                Product.prototype.GetProductBreadCrumb(parseInt(window.sessionStorage.getItem("lastCategoryId"), 10), $("#isQuickView").val());
            }
            $("#breadcrumb-productname").html($(".product-name").html())
        });
    }

    IsCategoryLinkClicked() {
        $('#isCategoryLinkClicked').val('true');
    }

    //TO DO:
    GetSuggestionResult(item: any): any {
        if (item.properties["ProductSeoUrl"] !== null && item.properties["ProductSeoUrl"] !== undefined && item.properties["ProductSeoUrl"] !== '') {
            window.location.href = window.location.protocol + "//" + window.location.host + item.properties["ProductSeoUrl"];
        }
    }

    ShowErrorAddonError(isError: boolean, id: string, addOnId: string): boolean {
        if (!isError) {
            $("#" + id + addOnId).css("display", "none");
            return true;
        }
        else {
            $("#" + id + addOnId).removeAttr("style");
            return false;
        }
    }

    SubmitStockRequest(control): any {
        if (!Product.prototype.ValidateStockRequest()) {
            return false;
        }
        var sku: string = $("#Quantity").attr('data-sku');
        if ($("#dynamic-producttype").val() != "SimpleProduct") {
            sku = $("#selectedSKU").val();
        }

        var stockNotificationModel: Znode.Core.StockNotificationModel = {
            SKU: sku,   
            ParentSKU: $("#dynamic-sku").val(),
            EmailId: $("#stockNoticeEmail").val(),
            Quantity: $("#stockNoticeQty").val()
        };
        Endpoint.prototype.SubmitStockRequest(stockNotificationModel, function (res) {
            if (res.status) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessSubmitStockNotice"), 'success', isFadeOut, fadeOutTime);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSubmitStockNotice"), 'error', isFadeOut, fadeOutTime);
            }
        });
        $('#StockNoticeModal').modal('hide');
    }

    BindStockSKU(control): any {
        $("#selectedSKU").val($(control).attr('data-sku'));
    }

    //Bind add on values of Product.
    BindAddOnProductSKU(control, event): any {
        ZnodeBase.prototype.ShowLoader();
        var productType: string = $(control).closest('form').children("#dynamic-producttype").val();
        if (productType == "GroupedProduct") {
            if (isAddToCartGroupProduct == false) {
                ZnodeBase.prototype.HideLoader();
                return false;
            }
            if (!Product.prototype.CheckGroupProductAddonQuantity()) {
                ZnodeBase.prototype.HideLoader();
                return false;
            }
        }
        else if (Product.prototype.OnQuantityChange() == false) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }

        var personalisedForm = $("#frmPersonalised");
        if (personalisedForm.length > 0 && !personalisedForm.valid()) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        var addOnValues = [];
        var bundleProducts = [];
        var groupDontTrackInventory = "";
        var groupProducts = "";
        var groupProductsQuantity = "";
        var personalisedCodes = [];
        var personalisedValues = [];
        var quantity: string = "";
        var flag: boolean = Product.prototype.ValidateAddons();
        var finalFlag: boolean = flag;
        //for checkbox   
     

        //Get selected add on product skus.
        addOnValues = Product.prototype.GetSelectedAddons();

        //Get bundle product skus.
        bundleProducts = Product.prototype.GetSelectedBundelProducts();

        //Get group product skus and their quantity.
        $("input[type=text].quantity").each(function () {
            if ($(this).attr("name") != "Quantity") {
                var quantity = $(this).val();
                if (quantity != null && quantity != "") {
                    groupProducts = groupProducts + $(this).attr("data-sku") + ",";
                    groupProductsQuantity + $(this).val() + "_";
                    groupProductsQuantity = groupProductsQuantity + $(this).val() + "_";
                }
                if (parseInt($(this).attr("data-maxquantity")) < parseInt(quantity))
                    return Product.prototype.CheckQuickViewAndShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("RequiredProductQuantity"));

                if (quantity != null && quantity != "" && $(this).attr("data-inventory") == "False") {
                    groupDontTrackInventory = groupDontTrackInventory + $(this).attr("data-sku") + ",";
                }
            }
        })
        groupProductsQuantity = groupProductsQuantity.substr(0, groupProductsQuantity.length - 1);
        groupProducts = groupProducts.substr(0, groupProducts.length - 1);

        if (productType == "GroupedProduct") {
            if (groupProductsQuantity == null || groupProductsQuantity == "") {
                ZnodeBase.prototype.HideLoader();
                return Product.prototype.CheckQuickViewAndShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("RequiredProductQuantity"));
            }
            else if (!Product.prototype.OnAssociatedProductQuantityChange()) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorProductQuantity"), "error", true, 10000);
                ZnodeBase.prototype.HideLoader();
                return false;
            }
            else {
                var mainProductId: number = parseInt($("#dynamic-parentproductid").val());
                if (!Product.prototype.CheckGroupProductQuantity(mainProductId, groupProducts, groupProductsQuantity, groupDontTrackInventory)) {
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
            }
        }
        else {
            quantity = $("#Quantity").val();
        }
        $("input[IsPersonalizable = True]").each(function () {
            var $label = $("label[for='" + this.id + "']");
            personalisedValues.push($(this).val());
            personalisedCodes.push($label.text());
            if (finalFlag == true) {
                $(this).val("");
            }
        });
        //Map Cart item model values for add to cart.
        Product.prototype.SetCartItemModelValues(addOnValues, bundleProducts, groupProducts, groupProductsQuantity, quantity, personalisedCodes.join(), personalisedValues.join(), control);
        if (finalFlag == true) {
            Product.prototype.DeSelectAddonsOnAddToCart();
        }
        return finalFlag;
    }

    ValidateAddons(): boolean {
        var isAddonsValid: boolean = true;
        var flag: boolean = true;
        $(".chk-product-addons").each(function () {
            var optional = $(this).data("isoptional");
            var displayType = $(this).data("displaytype");
            var checkBoxId = $(this).attr("id");
            var addOnId = $(this).data("addongroupid");
            if (optional == "False") {
                flag = true;
            }
            else {
                var isError = true;
                if (displayType != "") {
                    var addOnDivId = "";
                    var addToCartButtonId = 'button[id^="button-addtocart_"]';
                    displayType = displayType.toLowerCase();
                    if (displayType == "checkbox") {
                        if ($('#' + checkBoxId + ' input[type=checkbox]:checked').length > 0) {
                            isError = false;
                        }
                        else {
                            Product.prototype.ScrollDown(checkBoxId, addToCartButtonId);
                        }
                        addOnDivId = "paraCommentCheckBox-";
                    }
                    if (displayType == "radiobutton") {
                        if ($('#' + checkBoxId + ' input[type=radio]:checked').length > 0) {
                            isError = false;
                        }
                        else {
                            Product.prototype.ScrollDown(checkBoxId, addToCartButtonId);
                        }
                        addOnDivId = "paraCommentRadioButton-";
                    }
                    if (displayType == "dropdown") {
                        var isSelected = $('#' + checkBoxId).find('option:selected').val() == "0" || $('#' + checkBoxId).find('option:selected').val() == undefined ? false : true;
                        if (isSelected) {
                            isError = false;
                        }
                        else {
                            Product.prototype.ScrollDown(checkBoxId, addToCartButtonId);
                        }
                        addOnDivId = "paraCommentDropDown-";
                    }

                    if (!isError) {
                        $("#" + addOnDivId + addOnId).css("display", "none");
                        flag = true;
                    }
                    else {
                        $("#" + addOnDivId + addOnId).removeAttr("style");
                        flag = false;
                    }

                    if (flag == false) {
                        isAddonsValid = flag
                        ZnodeBase.prototype.HideLoader();
                    }

                }
            }

        });
        return isAddonsValid;
    }

    ScrollDown(checkBoxId, addToCartButtonId): any {
        var scrollDown = $([document.documentElement, document.body]).animate({
            scrollTop: $('#' + checkBoxId).offset().top + 150 - $(addToCartButtonId).offset().top
        }, 100);
        return scrollDown;
    }

    CheckGroupProductQuantity(productId: number, groupProductSkus: string, groupProductQuantities: string, groupDontTrackInventory: string): boolean {
        var isSuccess: boolean = true;
        if (groupDontTrackInventory.trim() != "") {
            Endpoint.prototype.CheckGroupProductInventory(productId, groupProductSkus, groupProductQuantities, function (response) {
                if (!response.ShowAddToCart) {
                    Product.prototype.CheckQuickViewAndShowErrorMessage(response.InventoryMessage);
                }
                isSuccess = response.ShowAddToCart;
            });
        }
        return isSuccess;
    }

    CheckQuickViewAndShowErrorMessage(errorMessage): boolean {
        if ($("#quick-view-popup-ipad").is(":visible")) {
            $("#QuickViewQuantiyErrorMessage").html(errorMessage);
        } else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(errorMessage, "error", true, fadeOutTime);
        }
        return false;
    }

    GetSelectedAddons(): any {
        var addOnValues = [];
        $(".AddOn").each(function () {
            var values = "";
            if ($(this).is(":checked")) {
                values = $(this).val();
            }
            else {
                values = $(this).children(":selected").attr("data-addonsku");
            }
            if (values != null && values != "") {
                addOnValues.push(values);
            }
        });
        return addOnValues;
    }

    GetSelectedBundelProducts(): any {
        var bundleProducts = [];
        $(".bundle").each(function () {
            var values = $(this).attr("data-bundlesku");
            bundleProducts.push(values);
        })
        return bundleProducts;
    }

    SetCartItemModelValues(addOnValues, bundleProducts, groupProducts, groupProductsQuantity, quantity, personalisedcodes, personalisedvalues, control): any {
        $(control).closest('form').children("#dynamic-addonproductskus").val(addOnValues);
        $(control).closest('form').children("#dynamic-bundleproductskus").val(bundleProducts);

        if (quantity != null || quantity != "") {
            $(control).closest('form').children("#dynamic-quantity").val(quantity);
        }
        $(control).closest('form').children("#dynamic-personalisedcodes").val(personalisedcodes);
        $(control).closest('form').children("#dynamic-personalisedvalues").val(personalisedvalues);
        $(control).closest('form').children("#dynamic-groupproductskus").val(groupProducts);
        $(control).closest('form').children("#dynamic-groupproductsquantity").val(groupProductsQuantity);
        $(control).closest('form').children("#dynamic-groupproductsquantity").val(groupProductsQuantity);
        $(control).closest('form').append("<input type='hidden' id='dynamic-isproductedit' name='IsProductEdit' value='" + $('#isProductEdit').val() + "' />")
    }

    ActiveReadReviews(): any {
        var url = document.URL.toString();
        var name = '';
        if (!(url.indexOf('#') === -1)) {
            var urlParts = url.split("#");
            name = urlParts[1];
        }
        if (name == "scrollReview") {
            $("#tab-reviews").click();
        }
    }


    SendMailPopUp(): any {
        Endpoint.prototype.SendMail(function (res) {
            $("#btnSendMailPopup").click();
            $("#popUp_sendMail").html(res);
            $("#divSendMail").html(res);
        });
    }

    SendMailResult(data: any) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, data.Type, isFadeOut, fadeOutTime);
        $("#divSendMail").hide();
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        $("#divSendMail").find("button[class=close]").click();
    }

    OnClickSendMail(): void {
        $("#divSendMail").hide();
        ZnodeBase.prototype.ShowLoader();
    }

    EmailToFriendSuccess(): any {
        jQuery('#modelEmailToFriend').trigger('click');
        $("#YourMailId").val("");
        $("#FriendMailId").val("");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessMailSending"), "success", true, 10000);
        ZnodeBase.prototype.HideLoader();
    }
    EmailToFriendBegin(): any {
        $("#ProductName").val($(".product-name").html());
        ZnodeBase.prototype.ShowLoader();
        jQuery('#modelEmailToFriend').trigger('click');
    }
    EmailToFriendFailure() {
        jQuery('#modelEmailToFriend').trigger('click');
        $("#YourMailId").val("");
        $("#FriendMailId").val("");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMailSending"), "error", true, 10000);
        ZnodeBase.prototype.HideLoader();
    }

    GetProductBreadCrumb(categoryId: number, isQuickView: boolean): void {
        var categoryIds: Array<string> = $("#categoryIds").val().split(",");
        if (isFromCategoryPage == "true" && categoryId > 0) {
            if ($.inArray(categoryId.toString(), categoryIds) > -1) {
                Endpoint.prototype.GetBreadCrumb(categoryId, $("#categoryIds").val(), false, function (response) {
                    if (!isQuickView)
                        $("#breadCrumb").html(response.breadCrumb + " / " + "<span id='breadcrumb-productname'>" + $(".product-name").html() + "</span>")
                    $("#seeMore").html(response.seeMore)
                })
            }
            else {
                if (!$("#categoryIds").val()) {
                    Endpoint.prototype.GetBreadCrumb(0, $("#categoryIds").val(), true, function (response) {
                        if (!isQuickView)
                            $("#breadCrumb").html(response.breadCrumb + " / " + "<span id='breadcrumb-productname'>" + $(".product-name").html() + "</span>")
                        $("#seeMore").html(response.seeMore)
                    })
                }
                else {
                    Endpoint.prototype.GetBreadCrumb(parseInt(categoryIds[0], 10), $("#categoryIds").val(), false, function (response) {
                        if (!isQuickView)
                            $("#breadCrumb").html(response.breadCrumb + " / " + "<span id='breadcrumb-productname'>" + $(".product-name").html() + "</span>")
                        $("#seeMore").html(response.seeMore)
                    })
                }
            }
        }
        //new tab
        else if (isFromCategoryPage == "true" && (isNaN(categoryId))) {
            Endpoint.prototype.GetBreadCrumb(0, $("#categoryIds").val(), true, function (response) {
                if (!isQuickView)
                    $("#breadCrumb").html(response.breadCrumb + " / " + "<span id='breadcrumb-productname'>" + $(".product-name").html() + "</span>")
                $("#seeMore").html(response.seeMore)
            })
        }
        else if (isFromCategoryPage != "true") {
            if (!$("#categoryIds").val()) {
                if (!isQuickView)
                    $("#breadCrumb").html("<a href='/'>" + ZnodeBase.prototype.getResourceByKeyName("TextHome") + "</a>" + " / " + $(".product-name").html())
                $("#seeMore").html("")
            }
            else {
                Endpoint.prototype.GetBreadCrumb(parseInt(categoryIds[0], 10), $("#categoryIds").val(), false, function (response) {
                    if (!isQuickView)
                        $("#breadCrumb").html(response.breadCrumb + " / " + "<span id='breadcrumb-productname'>" + $(".product-name").html() + "</span>")
                    $("#seeMore").html(response.seeMore)
                })
            }
        }
    }

    public GetPriceAsync(): void {
        var products = new Array();
        $(".product-details .price-span").each(function () {
            var _control: any = $(this);
            var _product: ProductModel = { sku: _control.data("sku"), type: _control.data("type") };
            products.push(_product);
        })
        if (products.length > 0)
            Product.prototype.CallPriceApi(products);
    }

    public GetPrice(obsoleteClass, minQuantity = "1"): void {
        var products = new Array();
        $(".cloudflareSpan").each(function () {
            var _control: any = $(this);
            var _product: Znode.Core.PublishProductModel = { sku: _control.data("sku"), type: _control.data("type"), PublishProductId: _control.data("id"), MinQuantity: minQuantity, PriceView: true, ObsoleteClass: obsoleteClass };
            products.push(_product);
        })

        $("#CloudflareTierPriceSpan").each(function () {
            var _control: any = $(this);
            var _product: Znode.Core.PublishProductModel = { sku: _control.data("sku"), type: _control.data("type"), PublishProductId: _control.data("id"), MinQuantity: minQuantity, PriceView: true, ObsoleteClass: obsoleteClass };
            products.push(_product);
        })

        Endpoint.prototype.CallInventoryPriceApi(products, function (response) {
            $.each(response.data, function (index, value) {
                $(".cloudflareSpan[data-sku='" + value.SKU + "']").html(value.HtmlText);
                if (value.TierPriceText)
                    $("#CloudflareTierPriceSpan[data-sku='" + value.SKU + "']").html(value.TierPriceText);
            });
        });
    }

    public GetInventory(obsoleteClass): void {
        var products = new Array();
        $(".cloudflareInventorySpan").each(function () {
            var _control: any = $(this);
            var _product: Znode.Core.PublishProductModel = { sku: _control.data("sku"), type: _control.data("type"), PublishProductId: _control.data("id"), MinQuantity: "1", PriceView: false, ObsoleteClass: obsoleteClass};
            products.push(_product);
        })
        Endpoint.prototype.CallInventoryPriceApi(products, function (response) {
            $.each(response.data, function (index, value) {
                $(".cloudflareInventorySpan[data-sku='" + value.SKU + "']").html(value.HtmlText);
            });
        });
    }

    //Redirect to product details page and show add to cart result.
    public DisplayAddToCartResult(data): void {
        ZnodeBase.prototype.HideLoader();
        if (!data.status) {
            // var productData = Product.prototype.GetProductData(this);
            Product.prototype.DisplayStickBar(data);
            $(".cartcount").html(data.cartCount);
            //  ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ProductAddedToCart"), 'success', isFadeOut, fadeOutTime);
        }
        else {
            $('[data-id="stickyBar"]').hide();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AddedToCartErrorMessage"), 'error', isFadeOut, fadeOutTime);
        }
        ZnodeBase.prototype.HideLoader();
    }

    DisplayStickBar(data): void {
        var productData = data.Product;
        var stickyBarDiv = $('[data-id="stickyBar"]');
        stickyBarDiv.find('img').attr('src', data.ImagePath);
        stickyBarDiv.find('.addtocart-label').text("Added to Cart: " + productData.Quantity + " Qty");
        //stickyBarDiv.find('.stickProductName').text(productData.productName);
        stickyBarDiv.find('.stickProductSKU').text("SKU: " + productData.SKU);
        if ($('.bx-align').length > 0) {
            var height = $('.bx-align').height();
            var top = 110;
            var totalTop = top + height;
            $('.static-bar').css({ top: totalTop + 'px' })
        }
        !stickyBarDiv.is(':visible') ? stickyBarDiv.show() : "";

        //Hide Other Stickybar
        $('[data-stick="product-sticky"]').hide();
    }

    DisplayAddToCartMessage(data): void {
        ZnodeBase.prototype.HideLoader();
        $(".quick-view-popup").modal('hide')
        if (!data.status) {
            $(".cartcount").html(data.cartCount);
            if (data.hasOwnProperty("CartNotification") && data.CartNotification.hasOwnProperty("IsEnabled") && data.CartNotification.IsEnabled) {
                Product.prototype.DisplayAddToCartNotification(data.CartNotification);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Added to cart" + " <a href='/cart'>Click here</a> to view your shopping cart and checkout.", 'success', isFadeOut, fadeOutTime);
            }
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AddedToCartErrorMessage"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisplayAddToCartNotification(product) {
        if (product != null && product != undefined) {
            Endpoint.prototype.DisplayAddToCartNotification(JSON.stringify(product), function (response) {
                var element: any = $("#addToCartNotification");
                $("#addToCartNotification").removeAttr("style");
                $(window).scrollTop(0);
                $(document).scrollTop(0);
                if (element.length) {
                    if (response !== "" && response != null) {
                        element.html(response);
                        setTimeout(function () {
                            element.fadeOut().empty();
                        }, fadeOutTime);
                    }
                }
            });
        }
    }

    private CallPriceApi(products: any): void {
        Endpoint.prototype.CallPriceApi(JSON.stringify(products), function (responce) {
            Product.prototype.AssignPricetoProduct(responce.data);
        });
    }

    private AssignPricetoProduct(data): void {
        $.each(data, function (index, value) {
            if (value.DisplaySalesPrice != null && value.DisplaySalesPrice != "") {
                $(".product-details .price-span[data-sku='" + value.sku + "']").html(value.DisplaySalesPrice);
                if (value.DisplayRetailPrice != null && value.DisplayRetailPrice != "") {
                    $(".product-details .price-span[data-sku='" + value.sku + "']").append("<span class='cut-price'>" + value.DisplayRetailPrice + "</span>");
                }
            }
            else if (value.DisplayRetailPrice != null && value.DisplayRetailPrice != "") {
                $(".product-details .price-span[data-sku='" + value.sku + "']").html(value.DisplayRetailPrice);
            }

        });
    }

    //#region B2B
    AddToFavourites(control): any {
        var sku = $(control).attr("data-sku");
        var addOnSKUs = Product.prototype.GetSelectedAddons();
        Endpoint.prototype.AddToWishList(sku, addOnSKUs.join(), function (res) {
            if (res.status) {
                $(".btn-wishlist").addClass('added-to-wishlist');
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "success", true, fadeOutTime);
                $(control).attr("onclick", "Product.prototype.RemoveFromFavourites(" + res.wishListId + ")");
            } else {
                if (res.isRedirectToLogin) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "info", true, fadeOutTime);
                    $('#loginForm').attr('action', '/User/Login?returnUrl=/Product/AddToWishList?productSKU=' + sku);
                    $('.account-signup-link').attr('href', '/User/Signup?returnUrl=/Product/AddToWishList?productSKU=' + sku);
                }
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "error", true, fadeOutTime);
            }
        });
    }

    AddToFavouritesPLP(control): any {
        var sku = $(control).attr("data-sku");
        var addOnSKUs = Product.prototype.GetSelectedAddons();
        Endpoint.prototype.AddToWishListPLP(sku, addOnSKUs.join(), function (res) {
            if (res.status) {
                $("#btnAddToWishList").addClass('added-to-wishlist');
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "success", true, fadeOutTime);
            } else {
                if (res.isRedirectToLogin) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "info", true, fadeOutTime);
                    $('#loginForm').attr('action', '/User/Login?returnUrl=/Product/AddToWishListPLP?productSKU=' + sku);
                    $('.account-signup-link').attr('href', '/User/Signup?returnUrl=/Product/AddToWishListPLP?productSKU=' + sku);
                }
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "error", true, fadeOutTime);
            }
        });
    }

    RemoveFromFavourites(wishListId: number): any {
        if (wishListId > 0) {
            Endpoint.prototype.RemoveFromWishList(wishListId, function (res) {
                if (res.success) {
                    $(".btn-wishlist").removeClass('added-to-wishlist');
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "success", true, fadeOutTime);
                    $('#btnAddToWishList').attr("onclick", "Product.prototype.AddToFavourites(this)");
                } else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "error", true, fadeOutTime);
                }
            });
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorProductRemoveFromWishList"), "error", true, fadeOutTime);
    }

    RegisterDomEvents(): any {
        // Click for write review form stars
        $(document).on("click", "#layout-writereview .setrating label", function () {
            $("#layout-writereview .setrating label").removeClass("full").addClass("empty"); // Reset all to empty            

            var stars = $(this).data("stars");
            $("#Rating").val(stars);
            for (var a = 1; a <= stars; a += 1) {
                $(".star" + a).removeClass("empty").addClass("full");
            }
        });

        $(document).on("keypress", "#product-details-quantity input[name='Quantity']", function (e) {
            $(this).attr("data-change", "true");
            var key = e.keyCode || e.which;
            if ((47 < key) && (key < 58) || key === 8) {
                return true;
            }
            return false;
        });

        $(document).on("cut copy paste", "#product-details-quantity input[name='Quantity'],.product-details-quantity input[class='quantity']", function (e) {
            e.preventDefault();
        });

        //for group product quantity check.
        $(document).on("keypress", ".product-details-quantity input[class='quantity']", function (e) {
            var key = e.keyCode || e.which;
            if ((47 < key) && (key < 58) || key === 8) {
                return true;
            }
            return false;
        });
    }

    DeSelectAddonsOnAddToCart(): void {
        $(".AddOn").each(function () {
            if ($(this).attr('type') == "checkbox" || $(this).attr('type') == "radio") {
                $(this).prop("checked", false);
            }
            else {
                $(this).val("0");
            }
        });
    }

    public DisplayAllLocationInveory() {
        if ($("#PublishProductId").length > 0) {
            var productId = $("#PublishProductId").val();
            $('#inventory-popup-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");
            $(".inventory-popup").first().modal('show');
            $(".inventory-popup .modal-content").css('min-height', '300px');
            $(".inventory-popup .modal-content").css('max-height', '800px');
            $(".inventory-popup .modal-content").css('margin', '0 auto');
            Endpoint.prototype.ShowProductAllLocationInventory(productId, function (res) {
                if (res != null && res != "") {
                    $("#inventory-popup-content").html(res);
                }
                else {
                    $("#inventory-popup-content").html(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessingRequest"));
                }
            });
        }
    }

    public DisplayChildAllLocationInventory(publishProductId: number) {
        if (publishProductId > 0) {
            $('#inventory-popup-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");
            $(".inventory-popup").first().modal('show');
            $(".inventory-popup .modal-content").css('min-height', '300px');
            $(".inventory-popup .modal-content").css('max-height', '800px');
            $(".inventory-popup .modal-content").css('margin', '0 auto');
            Endpoint.prototype.ShowProductAllLocationInventory(publishProductId, function (res) {
                if (res != null && res != "") {
                    $("#inventory-popup-content").html(res);
                }
                else {
                    $("#inventory-popup-content").html(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessingRequest"));
                }
            });
        }
    }

    public GetAllLocationInventory(): void {
        var productId;
        if ($("#PublishProductId").length > 0) {
            productId = $("#PublishProductId").val();
            Endpoint.prototype.GetAllLocationInventory(productId, function (response) {
                var defaultInventoryName;
                var defaultInventoryCount;
                var totalInventoryCount = 0;
                $.each(response.data.Inventory, function (index, value) {
                    if (value.IsDefaultWarehouse) {
                        defaultInventoryName = value.WarehouseName;
                        defaultInventoryCount = value.Quantity;
                    }
                    totalInventoryCount = totalInventoryCount + parseFloat(value.Quantity);
                });
                if ($("#lblDefaultInventoryCount").length > 0) {
                    $("#lblDefaultInventoryCount").html(defaultInventoryCount);
                }
                if ($("#lblDefaultLocationName").length > 0) {
                    $("#lblDefaultLocationName").html(defaultInventoryName);
                }
                if ($("#lblAllInventoryCount").length > 0) {
                    $("#lblAllInventoryCount").html(totalInventoryCount + "");
                }
            });
        }
    }
    
    public GetHighlightInfoByCode(highlightCode, publishProductId, productSku) {
        Endpoint.prototype.GetHighlightInfoByCode(highlightCode, publishProductId, productSku, function (res) {
            if (res.status) {
                if (res.DisplayPopup) {
                    window.open(res.HyperLink, '_blank').focus();
                }
                else {
                    window.location.href = "/Product/GetHighlightInfoByCode?highLightCode=" + highlightCode + "&productId=" + publishProductId + "&sku=" + productSku;
                }
            }
        });
    }

    ValidateStockRequest(): boolean {
        Product.prototype.RemoveStockNoticeValidationCSS("#stockNoticeQty");
        Product.prototype.RemoveStockNoticeValidationCSS("#stockNoticeEmail");
        var isEmailValid: boolean = Product.prototype.ValidateStockRequestEmail();
        var isQtyvalid: boolean = Product.prototype.ValidateStockRequestQty();
        if (!isEmailValid && !isQtyvalid) {
            $("#errorMessage").addClass("error-msg");
            Product.prototype.StockNoticeError("#stockNoticeEmail");
            Product.prototype.StockNoticeError("#stockNoticeQty");
            $("#errorMessage").text(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress"));
            $("#errorMessage").show();
            return false;
        }
        if (!isEmailValid) {
            Product.prototype.StockNoticeError("#stockNoticeEmail");
            $("#errorMessage").addClass("error-msg");
            $("#errorMessage").text(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress"));
            $("#errorMessage").show();
            return false;
        }
        if (!isQtyvalid) {
            Product.prototype.StockNoticeError("#stockNoticeQty");
            $("#errorMessage").addClass("error-msg");
            $("#errorMessage").text(ZnodeBase.prototype.getResourceByKeyName("ErrorQuantity"));
            $("#errorMessage").show();
            return false;
        }
        return true;
    }

    ValidateStockRequestEmail(): boolean {
        var email: string = $("#stockNoticeEmail").val()
        var regex: RegExp = new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/);
        if (email == "" || email == undefined || email == null || !regex.test(email)) {
            return false;
        }
        return true;
    }

    ValidateStockRequestQty(): boolean {
        var quantity: string = $("#stockNoticeQty").val()
        if (quantity == "" || quantity == undefined || quantity == null || quantity == "0") {
            return false;
        }
        return true;
    }

    ClearField(): any {
        Product.prototype.RemoveStockNoticeValidationCSS("#stockNoticeQty");
        Product.prototype.RemoveStockNoticeValidationCSS("#stockNoticeEmail");
        $("#stockNoticeQty").val('');
    }

    StockNoticeError(elementId): any {
        $(elementId).css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
    }

    RemoveStockNoticeValidationCSS(elementId): any {
        $(elementId).removeAttr('style');
        $("#errorMessage").hide();
    }

    DisableAddToCartForConfigAndGroup() {
        if ($("#dynamic-producttype").val() == "ConfigurableProduct") {
            Product.prototype.DisableAddToCartButton(".configurable-attributes-info .configquantity");
        }
        else if ($("#dynamic-producttype").val() == "GroupedProduct") {
            Product.prototype.DisableAddToCartButton(".product-details-quantity .quantity")

        }
    }

    DisableAddToCartButton(selector: string) {
        var productId: number = parseInt($("#scrollReview form").children("#dynamic-productid").val());
        $("#button-addtocart_" + productId).attr("disabled", true);
        $(selector.toString()).each(function () {
            if (!$(this).prop("disabled")) {
                $("#button-addtocart_" + productId).attr("disabled", false);
            }
        });
    }

    HideVideo(): any{
        $("#ProductZoomImage").hide();
        $("#ProductVideo").show();
    }

    SetYoutubeUrl(videoURL): any {
        var video;
        if (videoURL.includes("/watch?v=")) 
            video = "https://www.youtube.com/embed/" + videoURL.substring(videoURL.lastIndexOf('=') + 1);

        else 
            video = "https://www.youtube.com/embed/" + videoURL.substring(videoURL.lastIndexOf('/') + 1);
        
        Product.prototype.SetVideoUrl(video);
    }

    SetDailyMotionUrl(videoURL): any {
        var video = "https://www.dailymotion.com/embed/video/" + videoURL.substring(videoURL.lastIndexOf('/') + 1);
        Product.prototype.SetVideoUrl(video);
    }

    SetGoogleDriveVideoUrl(videoURL): any {
        var video = videoURL.replace("/view?usp=sharing", "/preview");
        if (video.includes("/preview") != true) 
            video = video + "/preview"
            Product.prototype.SetVideoUrl(video);
    }

    SetVideoUrl(videoURL): any {
        $("#PlayVideo").attr("src", videoURL)
        Product.prototype.HideVideo();
    }

    SwapImageAndVideoURL(control, imagePath): any{
        var UrlData = $(control);
        var videoURL = UrlData.data('standard').toString();
        $("#Errormessage").hide();

        if (videoURL.includes("youtube") == true || videoURL.includes("youtu.be") == true)
            Product.prototype.SetYoutubeUrl(videoURL);

        else if (videoURL.includes("dailymotion") == true || videoURL.includes("dai.ly") == true)
            Product.prototype.SetDailyMotionUrl(videoURL);

        else if (videoURL.includes("drive") == true)
            Product.prototype.SetGoogleDriveVideoUrl(videoURL);

        else if (videoURL.includes("mp4") == true || videoURL.includes("ogg") == true || videoURL.includes("webm") == true || videoURL.includes("ogv") == true || videoURL.includes("vimeo") == true)
            Product.prototype.SetVideoUrl(videoURL);

        else if (videoURL.includes(".jpg") != true && videoURL.includes(".png") != true && videoURL.includes(".jpeg") != true && videoURL.includes(".webp") != true && videoURL.includes("/Data/Media/") != true) {
            $("#Errormessage").show();
            $("#ProductVideo").hide();
            $("#ProductZoomImage").hide();
        }

        else {
            $("#ProductVideo").hide();
            $("#ProductZoomImage").show();
            imagePath.swap(UrlData.data('standard'), UrlData.attr('href'));
        }
    }
}

type ProductModel = {
    sku: string;
    type: string;
};

// Click for write review form stars
$(document).on("click", "#layout-writereview .setrating label", function () {
    $("#layout-writereview .setrating label").removeClass("full").addClass("empty"); // Reset all to empty            

    var stars = $(this).data("stars");
    $("#Rating").val(stars);
    for (var a = 1; a <= stars; a += 1) {
        $(".star" + a).removeClass("empty").addClass("full");
    }
    $("Span[for='Rating']").hide();
});

$(document).on("keypress", "#product-details-quantity input[name='Quantity']", function (e) {
    $(this).attr("data-change", "true");
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});

$(document).on("cut copy paste", "#product-details-quantity input[name='Quantity'],.product-details-quantity input[class='quantity']", function (e) {
    e.preventDefault();
});

//for group product quantity check.
$(document).on("keypress", ".product-details-quantity input[class='quantity']", function (e) {
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});

$(document).on("keypress", "#stockNoticeQty", function (e) {
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});

