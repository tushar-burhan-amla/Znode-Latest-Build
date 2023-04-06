declare var coupons: any;
class Cart extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        Cart.prototype.RestrictEnterButton();
    }

    RestrictEnterButton(): any {
        $('.frmCartQuantity').on('keyup keypress', function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });
    }

    //To show the Saved Items block after changing the quantity in the main cart.
    ShowSaveForLaterBlock(): any {
        $.ajax({ url: "/SaveForLater/GetSavedCartForLater", type: "get" }).done(function (t) { $("#SaveForLater-Container").html(t); });
    }

    UpdateCartQauntity(control, isSubmitChekout = false): boolean {
        var productId: number = parseInt($(control).attr("data-cart-productId"));
        var minQuantity: number = parseInt($(control).attr("data-cart-minquantity"));
        var maxQuantity: number = parseInt($(control).attr("data-cart-maxquantity"));
        var errorMessageField = $(control).parent().find("#quantity_error_msg_" + productId);
        var errorQuantityMessageField = $(control).parent().find("#avl_quantity_error_msg_" + productId);
        errorMessageField.text('');
        errorQuantityMessageField.text('');
        var inventoryRoundOff: number = parseInt($(control).attr("data-inventoryRoundOff"));
        var selectedQty: string = $(control).val();
        var decimalPoint: number = 0;
        var decimalValue: number = 0;
        let result: boolean = false;

        if (selectedQty.split(".")[1] != null) {
            decimalPoint = selectedQty.split(".")[1].length;
            decimalValue = parseInt(selectedQty.split(".")[1]);
        }

        $(document).on("blur", "#cart-quantity", function () {
            if ($(this).val() == '' || $(this).val() < minQuantity || $(this).val() > maxQuantity) {
                $("#btnSaveCart").addClass('disable-anchor');
                $(this).focus();
            }
        });

        if (this.CheckDecimalValue(decimalPoint, decimalValue, inventoryRoundOff, productId, errorMessageField)) {
            if (this.CheckIsNumeric(selectedQty, productId, errorMessageField)) {
                if (this.CheckMinMaxQuantity(parseInt(selectedQty), minQuantity, maxQuantity, productId, errorMessageField)) {
                    $("#btnCompleteCheckout").removeClass('disable-anchor');
                    $("#paypal-express-checkout").removeClass('disable-anchor');
                    $("#btnSaveCart").removeClass('disable-anchor');
                    $("#checkOut-link").removeAttr('href');
                    $("#cartTransfer-link").removeAttr('href');
                    $("#requestQuote-link").removeAttr('href');
                    result = true;

                    if (isSubmitChekout === undefined || isSubmitChekout === false)
                        $(control).closest("form").submit();
                }
            }
        }
        return result;
    }

    UpdateTemplateQauntity(control, isSubmitChekout = false): boolean {
        var productId: number = parseInt($(control).attr("data-cart-productId"));
        var minQuantity: number = parseInt($(control).attr("data-cart-minquantity"));
        var maxQuantity: number = parseInt($(control).attr("data-cart-maxquantity"));
        var defaultinventorycount: string = $(control).attr("data-cart-defaultinventorycount");
        var errorMessageField = $(control).parent().find("#quantity_error_msg_" + productId);
        var isActive: string = $(control).attr("data-cart-isactive");
        var isObsolete: string = $(control).attr("data-cart-isobsolete");
        var outOfStock: string = $(control).attr("data-cart-outofstock");
        var errorQuantityMessageField = $(control).parent().find("#avl_quantity_error_msg_" + productId);
        errorMessageField.text('');
        errorQuantityMessageField.text('');
        var inventoryRoundOff: number = parseInt($(control).attr("data-inventoryRoundOff"));
        var selectedQty: string = $(control).val();
        var decimalPoint: number = 0;
        var decimalValue: number = 0;
        let result: boolean = false;

        if (this.CheckOutOfStockIsObsoletAndIsActive(Number(defaultinventorycount), Number(selectedQty), outOfStock, isObsolete, isActive, errorMessageField)) {
            return result;
        }

        if (selectedQty.split(".")[1] != null) {
            decimalPoint = selectedQty.split(".")[1].length;
            decimalValue = parseInt(selectedQty.split(".")[1]);
        }

        $(document).on("blur", "#cart-quantity", function () {
            if ($(this).val() == '' || $(this).val() < minQuantity || $(this).val() > maxQuantity) {
                $(this).focus();
            }
        });

        if (this.CheckDecimalValue(decimalPoint, decimalValue, inventoryRoundOff, productId, errorMessageField)) {
            if (this.CheckIsNumeric(selectedQty, productId, errorMessageField)) {
                if (this.CheckMinMaxQuantity(parseInt(selectedQty), minQuantity, maxQuantity, productId, errorMessageField)) {
                    $("#btnCompleteCheckout").removeClass('disable-anchor');
                    $("#paypal-express-checkout").removeClass('disable-anchor');
                    $("#btnSaveCart").removeClass('disable-anchor');
                    $("#checkOut-link").removeAttr('href');
                    $("#cartTransfer-link").removeAttr('href');
                    $("#requestQuote-link").removeAttr('href');
                    result = true;

                    $.ajax({
                        url: "/User/GetCartItems",
                        data: { "productId": productId, "selectedQty": selectedQty },
                        type: 'POST',
                        async: false,
                        success: function (response) {
                            if (response.status) {
                                $("#checkOut-link").addClass('disable-anchor');
                                $("#cartTransfer-link").addClass('disable-anchor');
                                $("#requestQuote-link").addClass('disable-anchor');
                                $("#btnSaveCart").addClass('disable-anchor');
                                result = false;
                            }

                        }, error: function (e) {
                            alert("Status: " + e);
                        }
                    }).done(function () {
                        if ((isSubmitChekout === undefined || isSubmitChekout === false))
                            $(control).closest("form").submit();
                    });
                }
            }
        }
        return result;
    }

    CheckOutOfStockIsObsoletAndIsActive(defaultinventorycount: number, selectedQty: number, outOfStock: string, isObsolete: string, isActive: string, errorMessageField: any): boolean {
        if (selectedQty > defaultinventorycount && outOfStock != "DontTrackInventory") {
            errorMessageField.text(outOfStock);
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return true;
        }
        if (isObsolete === 'True') {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("isObsolete"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return true;
        }
        if (isActive === 'False') {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return true;
        }
        return false;
    }

    CheckDecimalValue(decimalPoint: number, decimalValue: number, inventoryRoundOff: number, productId: number, errorMessageField: any): boolean {
        if (isNaN(decimalValue) && decimalValue != 0 && decimalPoint > inventoryRoundOff) {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityHaving") + inventoryRoundOff + ZnodeBase.prototype.getResourceByKeyName("XNumbersAfterDecimalPoint"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return false;
        }
        if (isNaN(decimalValue)) {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return false;
        }
        return true;
    }

    CheckIsNumeric(selectedQty: string, productId: number, errorMessageField: any): boolean {
        var matches = selectedQty.match(/^-?[\d.]+(?:e-?\d+)?$/);
        if (matches == null) {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return false;
        }
        return true;
    }
    CheckMinMaxQuantity(selectedQty: number, minQuantity: number, maxQuantity: number, productId: number, errorMessageField: any): boolean {
        if (selectedQty < minQuantity || selectedQty > maxQuantity) {
            errorMessageField.text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + minQuantity + ZnodeBase.prototype.getResourceByKeyName("To") + maxQuantity + ZnodeBase.prototype.getResourceByKeyName("FullStop"));
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#btnSaveCart").addClass('disable-anchor');
            return false;
        }
        return true;
    }

    GetShippingId(control): void {
        Checkout.prototype.ShowLoader();
        var shippingId = $(control).data('shippingid');

        if (typeof shippingId == undefined || shippingId == null || shippingId == "")
            shippingId = 0;

        var zipCode = $("#zipcode").val();
        $("#checkOut-link").attr('href', '/Checkout/Index?ShippingId=' + shippingId);
        Endpoint.prototype.GetCart(shippingId, zipCode, function (response) {
            $(".order-summary").html(response);
            var orderTotal = $("[data-test-selector='hdgOrderGrandTotal']").html();
            (orderTotal != undefined && orderTotal!="")? $("#dynamic-cart-order-total").html(orderTotal) : "";
            Checkout.prototype.HideLoader();
        })
    }

    GetShippingEstimates(): any {
        var zipCode = $("#zipcode").val();
        var zipCodeRegexp = new RegExp("^[- +()]*[0-9][- +()0-9]*$");
        if (zipCode == undefined || zipCode == "") {
            $("#zipcodeerrormessage").text(ZnodeBase.prototype.getResourceByKeyName("ZipCodeError"));
        }
        else if (!zipCodeRegexp.test(zipCode)) {
            $("#zipcodeerrormessage").text(ZnodeBase.prototype.getResourceByKeyName("NumericZipCodeError"));
        }
        else {
            $("#zipcodeerrormessage").text("");
            Cart.prototype.ShowHideCancelButton();
            $("#shippingOptionsContainer").html(ZnodeBase.prototype.getResourceByKeyName("ZipCodeMessage"));
            Endpoint.prototype.GetShippingEstimates(zipCode, function (res) {
                if (res != null && res != "") {
                    var dynamicHtml = "";
                    if (res.shippingOptions != "" && res.shippingOptions != null) {
                        dynamicHtml = "<div class='col-xs-12 nopadding'>";
                        for (var arrayCounter = 0; arrayCounter < res.shippingOptions.length; ++arrayCounter) {
                            var shippingCost = res.shippingOptions[arrayCounter].FormattedShippingRate;
                            var shippingDesc = res.shippingOptions[arrayCounter].Description;
                            var shippingCode = res.shippingOptions[arrayCounter].ShippingCode;
                            var approximateArrival = res.shippingOptions[arrayCounter].ApproximateArrival;
                            var isSelected = res.shippingOptions[arrayCounter].IsSelected;
                            var shippingId = res.shippingOptions[arrayCounter].ShippingId;
                            //commented this line to hide due date as per requirement , Uncomment this line to show Due date on shipping
                            //if (approximateArrival != undefined && approximateArrival.length > 0) {
                            //    shippingDesc = shippingDesc + " (Due on " + approximateArrival + ")";
                            //}
                            if (shippingCode.toLowerCase() != "FreeShipping".toLowerCase()) {
                                if (isSelected) {
                                    dynamicHtml += "<div class='form-group'><div class='col-xs-6 nopadding'><div class='styled-input mr-0'><input type='radio' checked='" + isSelected + "' onclick='Cart.prototype.GetShippingId(this)' name='shippingOptions' data-shippingId='" + shippingId + "' data-shippingCode='" + shippingCode + "' id='" + shippingId + "'/><label for='" + shippingId + "'>" + shippingDesc + "</label></div></div><div class='col-xs-6 nopadding'>" + shippingCost + "</div></div>";
                                }
                                else {
                                    dynamicHtml += "<div class='form-group'><div class='col-xs-6 nopadding'><div class='styled-input'><input type='radio' onclick='Cart.prototype.GetShippingId(this)' name='shippingOptions' data-shippingId='" + shippingId + "' data-shippingCode='" + shippingCode + "' id='" + shippingId + "'/><label for='" + shippingId + "'>" + shippingDesc + "</label></div></div><div class='col-xs-4 nopadding'>" + shippingCost + "</div></div>";
                                }
                            }
                        }
                        dynamicHtml += "</div>";
                        $("#shippingOptionsContainer").html("");
                        $("#shippingOptionsContainer").html(dynamicHtml);
                        if (res.shippingOptions.filter(function (a) { return a.IsSelected == true }).length > 0) {
                            Cart.prototype.GetShippingId($("#shippingOptionsContainer input[type='radio']:checked"));
                        }
                    } else {
                        $("#shippingOptionsContainer").html(ZnodeBase.prototype.getResourceByKeyName("NoShippingOptionsFound"));
                    }
                }
            });
        }
    }



    ShowHideCancelButton(): void {
        if ($("#zipcode").val().length > 0) {
            $("#CancleShippingEstimator").show();
        } else {
            $("#CancleShippingEstimator").hide();
        }
    }
    ClearShippingEstimates(): void {
        $("#zipcode").val('');
        $("#zipcodeerrormessage").html('');
        $("#shippingOptionsContainer input[type='radio']:checked").prop('checked', false)

        Cart.prototype.GetShippingId($("#shippingOptionsContainer input[type='radio']:checked"));

        $("#shippingOptionsContainer").html('');
        $("#CancleShippingEstimator").hide();
    }

    ValidateProductQuantity(): void {
        let finalResult: boolean = false;
        let productId: number;

        $("div.cart-products table tbody tr").each(function () {
            let qtyTextbox = $(this).find('input[name="Quantity"]');

            productId = parseInt($(qtyTextbox).attr("data-cart-productId"));
            finalResult = Cart.prototype.UpdateCartQauntity(qtyTextbox, true);

            if (finalResult === false) {
                $("#quantity_error_msg_" + productId).text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + $(qtyTextbox).attr("data-cart-minquantity") + ZnodeBase.prototype.getResourceByKeyName("To") + $(qtyTextbox).attr("data-cart-maxquantity") + ZnodeBase.prototype.getResourceByKeyName("FullStop"));
                $("#checkOut-link").addClass('disable-anchor');
                $("#checkOut-link").removeAttr('href');
                $("#cartTransfer-link").addClass('disable-anchor');
                $("#cartTransfer-link").removeAttr('href');
                return finalResult;
            }
        });

        if (finalResult) {
            location.href = window.location.protocol + "//" + window.location.host + "/checkout/index";
        }
    }

    //TODO: Quote - Need to move this in sepearet ts i.e Quote.ts and chnage the logic as well
    ValidateProductQuantityForQuote(): void {
        let finalResult: boolean = false;
        let productId: number;

        $("div.cart-products table tbody tr").each(function () {
            let qtyTextbox = $(this).find('input[name="Quantity"]');

            productId = parseInt($(qtyTextbox).attr("data-cart-productId"));
            finalResult = Cart.prototype.UpdateCartQauntity(qtyTextbox, true);

            if (finalResult === false) {
                $("#quantity_error_msg_" + productId).text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + $(qtyTextbox).attr("data-cart-minquantity") + ZnodeBase.prototype.getResourceByKeyName("To") + $(qtyTextbox).attr("data-cart-maxquantity") + ZnodeBase.prototype.getResourceByKeyName("FullStop"));
                $("#checkOut-link").addClass('disable-anchor');
                $("#checkOut-link").removeAttr('href');
                $("#requestQuote-link").addClass('disable-anchor');
                $("#requestQuote-link").removeAttr('href');
                return finalResult;
            }
        });

        if (finalResult) {
            location.href = window.location.protocol + "//" + window.location.host + "/quote/index";
        }
    }

    EnableDisableCheckoutButton(): void {
        if ($('#hdnInsufficientQuantity').val() == 'True') {
            $("#checkOut-link").addClass('disable-anchor');
            $("#cartTransfer-link").addClass('disable-anchor');
            $("#btnCompleteCheckout").addClass('disable-anchor');
            $("#paypal-express-checkout").addClass('disable-anchor');
            $("#checkOut-link").removeAttr('href');
            $("#cartTransfer-link").removeAttr('href');
            $("#requestQuote-link").addClass('disable-anchor');
            $("#requestQuote-link").removeAttr('href');
            $("#btnSaveCart").addClass('disable-anchor');
        }
        ZnodeBase.prototype.HideLoader();
        //Calling the Saved Items block after hide loader
        if ($('#IsAuthenticatedUser').val() == 'True' && $("#EnableSaveForLater").val() == 'True')
            Cart.prototype.ShowSaveForLaterBlock();
    }
    EnableDisableMenuIcon(): void {        
        if ($("#others-menu .dropdown").length === 0) {            
            $(".nav-drop-icon").hide();
        }
        else {            
            $(".nav-drop-icon").show();
        }

        $(".control_next").on("click", function () {
            $("#others-menu").toggle();
        });

        ZnodeBase.prototype.HideLoader();
    }

    ShowCouponsMessages(Coupons:any): void {
        var htmlString = "";
        coupons = Coupons;
        if (coupons.length > 0) {
            for (var dataIndex = 0; dataIndex < coupons.length; dataIndex++) {
                var style = coupons[dataIndex].CouponApplied ? "success-msg padding-top" : "error-msg";
                var message = coupons[dataIndex].PromotionMessage;
                var couponCode = coupons[dataIndex].Code;
                Checkout.prototype.RemoveDiscountMessages();
                htmlString = htmlString + "<p class='text-break " + style + "'>" + "<a class='zf-close' onclick='Cart.prototype.RemoveAppliedCoupon(" + dataIndex + ")' style='cursor:pointer;color:#cc0000;padding-right:3px;' title='Remove Coupon Code'></a>" + "<b>" + couponCode + "</b>" + " - " + message + "</p>";
            }
            htmlString = htmlString + "</div>";
            $("#couponMessageContainer").html("");
            $("#couponMessageContainer").html(htmlString);
        }
    }

    RemoveAppliedCoupon(couponIndex): any {
        var _code = coupons[couponIndex].Code;
        coupons = new Array();
        Checkout.prototype.RemoveCoupon(_code);
    }

    EditSavedCartName(): any {
        var templateName = $("#SavedCartName").val();
        var templateid = $("#OmsTemplateId").val();
        var regex = new RegExp("^[A-Za-z0-9 ]+$");
        $('#validTemplateName').text("");
        $("#checkOuts-link").prop("disabled", false);
        $("#checkout-link").prop("disabled", false);

        if (templateName != null && templateName != undefined && templateName != "") {
            if (regex.test(templateName)) {
                if (!(templateName.length > 100)) {
                    Endpoint.prototype.EditSavedCartName(templateName, templateid, function (response) {
                        if (response.status == false) {
                            $("#checkOuts-link").prop("disabled", true);
                            $("#checkout-link").prop("disabled", true);
                            $('#validTemplateName').text(ZnodeBase.prototype.getResourceByKeyName("DuplicateCart"));
                        }
                        else {
                            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessSavedCart"), "success", false, 0);
                        }
                    });
                }
                else {
                    $("#checkOuts-link").prop("disabled", true);
                    $("#checkout-link").prop("disabled", true);
                    $('#validTemplateName').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSavedcartName"));
                }
            }
            else {
                $("#checkOuts-link").prop("disabled", true);
                $("#checkout-link").prop("disabled", true);
                $('#validTemplateName').text(ZnodeBase.prototype.getResourceByKeyName("alphanumeric"));
            }
        }
        else {
            $("#checkOuts-link").prop("disabled", true);
            $("#checkout-link").prop("disabled", true);
            $('#validTemplateName').text(ZnodeBase.prototype.getResourceByKeyName("NoCart"));
        }
    }

    AddProductToCartForSaveCart(): any {
        var flag = true;
        var cartItemCount = $("#hdnTemplateCartItemCount").val();
        var templateId = $("#OmsTemplateId").val();
        if (cartItemCount > 0) {
            if (templateId > 0) {
                flag = true;
                window.location.href = "/SavedCart/AddProductToCartForSaveCart?omsTemplateId=" + templateId;
            }
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorAtLeastOneSaveCart"), "error", false, 0);
            flag = false;
        }
        return flag;
    }

    SetMoveToCart(): any {
        $("[data-swhgcontainer='ZnodeSavedCart'] tbody tr").each(function () {
            var items = parseFloat($(this).find('td.z-items').text());
            if (items != undefined && items != null && items <= 0) {
                $(this).find('.cart-icon').attr("disabled", true).css({ "pointer-events": "none" });
            }
        });
    }
}

$(document).on("keypress", "#cart-quantity", function (e) {
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});

$(document).on("cut copy paste", "#cart-quantity", function(e) {
    e.preventDefault();
});
