class OrderTabs extends ZnodeBase {
    constructor() {
        super();
    }

    _endPoint: Endpoint;
    Init(): any {
    }


    ContinueOrder(isPageLoad: boolean = false): any {
        let activeTab: string = $("#OrderAsidePannel>li.active").attr("id");
        var nextTab = $("#OrderAsidePannel>li.active").next('li');
        let isValidate: boolean;
        if (OrderTabs.prototype.CheckValidationOnDiv(activeTab))
            return false;

        switch (activeTab) {
            case "customer-tab-link":
                //Validate Details of Customer tab
                isValidate = OrderTabs.prototype.ValidateCustomerDetails(isPageLoad);
                break;
            case "shipping-cart-tab-link":
                //Validate Details of Cart and Shipping tab
                isValidate = OrderTabs.prototype.ValidateShippingAndCartDetails();
                if (isValidate === false)
                    break;
                isValidate = Order.prototype.IsValidCSRDiscountApplied();
                break;
            case "payment-tab-link":
                //Validate details of payment tab
                if (Order.prototype.IsQuote())
                {
                    isValidate = true;
                }
                else
                {
                    isValidate = OrderTabs.prototype.ValidatePaymentDetails();
                }
                break;
            case "review-placeorder-tab-link":
                if (Order.prototype.IsQuote()) {
                    isValidate = true;
                }
                else {
                    isValidate = true;
                    $("#btnPlaceOrder").hide();
                }
                break;
        }

        if (isValidate) {
            if (nextTab.length > 0) {
                nextTab.removeClass("disabled");
                nextTab.find('a').trigger('click');
                if (Order.prototype.IsQuote() && activeTab == "shipping-cart-tab-link") {
                    nextTab.next('li').removeClass("disabled");
                }
            }
        }
        return isValidate;
    }

    CheckValidationOnDiv(activeTab: string): boolean {
        var activeDiv: string;
        var IsInvalid: boolean = false;
        switch (activeTab) {
            case "customer-tab-link":
                activeDiv = "CustomerDiv";
                break;
            case "shipping-cart-tab-link":
                activeDiv = "divShoppingCart";
                break;
            case "payment-tab-link":
                activeDiv = "paymentMethodsDiv";
                break;
            case "review-placeorder-tab-link":
                activeDiv = "ReviewDiv";
                break;
        }
        $('div#' + activeDiv + '').find("[data-val-required]").each(function () {
            if ($(this).attr('type') != 'hidden' && $(this).val() === "") {
                if ($("#hdnTotalOrderAmount").val() > 0.00 && ($("#hdnOverDueAmount").val() >= 0.00)) {
                    $('#' + $(this).attr('id')).addClass('input-validation-error');
                    $('#' + $(this).attr('id')).attr('style', 'border: 1px solid rgb(195, 195, 195)');
                    $('span#' + $(this).attr('name') + 'Error').removeClass('field-validation-valid');
                    $('span#' + $(this).attr('name') + 'Error').addClass('field-validation-error');
                    $('span#' + $(this).attr('name') + 'Error').text($(this).attr('data-val-required'));
                    IsInvalid = true;
                }
                else {
                    IsInvalid = false;
                }
            }
            else if (activeDiv == 'ShoppingCartDiv') {
                if ($("#hdnIsAnyProductOutOfStock").val() != undefined && $("#hdnIsAnyProductOutOfStock").val().toLowerCase() == 'true' || $("#hdnIsAnyProductOutOfStock").val() == true) {
                    IsInvalid = true;
                }
                else {
                    $('tr#cart-row-div').each(function () {
                        if ($('#quantity_error_msg_' + $(this).find('#CartQuantity').attr('data-cart-externalid') + '').text().trim() != '') {
                            IsInvalid = true;
                        }
                    });
                }
            }
            else if ($(this).attr('type') != 'hidden' && typeof ($(this).attr('class')) != 'undefined') {
                if ($(this).attr('class').indexOf("input-validation-error") > 0) {
                    IsInvalid = true;
                }
            }
            else if ($("[name='SaveCard-CVV']:visible").length > 0) {
                var cardtype = $("[name='SaveCard-CVV']:visible").attr('data-cardtype');
                var cvvNumber: string = $("[name='SaveCard-CVV']:visible").val();
                if (cardtype == Constant.AmericanExpressCardCode) {
                    if (!cvvNumber || cvvNumber.length < 4) {
                        OrderTabs.prototype.ValidationOfCVV();
                        Order.prototype.ShowHideErrorCVV(false);
                        Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
                        IsInvalid = true;
                    }
                } else if (!cvvNumber || (cvvNumber.length <= 2 || cvvNumber.length > 4)) {
                    IsInvalid = true;
                    Order.prototype.ShowHideErrorCVV(false);
                    Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
                    OrderTabs.prototype.ValidationOfCVV();
                } else {
                    IsInvalid = false;
                }
            }
        });
        return IsInvalid;
    }

    ConfirmCancelOrderPopUp(): void {
        $('#PopUpConfirmCancelOrder').modal('show');
    }

    ValidatePortal(): boolean {
        if (($("#hdnPortalId").val() == 0)) {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").parent("div").addClass('input-validation-error');
            return false;
        }
        return true;
    }

    ValidationOfCVV(): void {
        $("[name='SaveCard-CVV']:visible").css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
        $("[name='SaveCard-CVV']:visible").parent().find("span.field-validation-error").length <= 0 ?
            $("[name='SaveCard-CVV']:visible").parent().append("<span class='field-validation-error error-cvv'>" + ZnodeBase.prototype.getResourceByKeyName("CVVErrorMessage") + "</span>") :
            $("[name='SaveCard-CVV']:visible").parent().find("span.field-validation-error").show();
    }

    public OnBlurPurchaseOrderNumber(): void {
        var poOrderNumber = $("#PurchaseOrderNumber").val();
        if (poOrderNumber != "" && poOrderNumber != null && poOrderNumber != "undefined") {
            $("#cart-ponumber-status").hide();
            $(poOrderNumber).removeClass('input-validation-error');
        }
    }

    ConfirmCancelOrder(): void {
        var actionName = $("#ActionName").val();
        if (actionName != "" && actionName != null && actionName != undefined) {
            if ( actionName == "OrderView") {
                window.location.href = '/Order/List';
            }
            else if (actionName == "AccountOrderView") {
                var accountId = $('#hdnAccountId').val();
                if (accountId == 0) {
                    accountId = $('#ReorderAccountId').val();
                    window.location.href = "/Account/AccountUserOrderList?accountId=" + accountId;
                }
                else if (accountId != null && accountId > 0)
                    window.location.href = "/Account/AccountUserOrderList?accountId=" + accountId;
            }
            else if (actionName == "CustomerOrderView") {
                var userId =window.location.href.split('&')[0].split('=')[1];
                    window.location.href = "/Customer/GetOrderList?userId=" + userId;
            }
        }
        else {
            window.location.href = '/Order/List';
        }
    }

    //Validate Details of Customer tab
    ValidateCustomerDetails(isPageLoad: boolean = false): boolean {
        let isValidate: boolean = true;
        if ($("#txtCustomerName").val() == "" || typeof $("#hdnUserId").val() == 'undefined' || $("#hdnUserId").val() == '0') {
            OrderTabs.prototype.ValidatePortal();
            $('span#CustomerNameError').removeClass('field-validation-valid');
            $('span#CustomerNameError').addClass('field-validation-error');
            $('#txtCustomerName').addClass('input-validation-error');
            $('span#CustomerNameError').text(ZnodeBase.prototype.getResourceByKeyName("ErrorCustomerSelect"));
            isValidate = false;
        }
        else if (typeof $("#ShippingAddress_AddressId").val() == 'undefined' || $("#ShippingAddress_AddressId").val() == '0') {
            if (!isPageLoad) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorShippingAddress"), 'error', isFadeOut, fadeOutTime);
            }
            isValidate = false;
        }
        else if (typeof $("#BillingAddress_AddressId").val() == 'undefined' || $("#BillingAddress_AddressId").val() == '0') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorBillingAddress"), 'error', isFadeOut, fadeOutTime);
            isValidate = false;
        }

        return isValidate;
    }

    //Validate Details of Cart and Shipping tab
    ValidateShippingAndCartDetails(): boolean {
        let isValidate: boolean = true;
        if (typeof $("#hdnShoppingCartCount").val() == 'undefined' || $("#hdnShoppingCartCount").val() == '0') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorItemNotFountInCart"), 'error', isFadeOut, fadeOutTime);
            isValidate = false;
        }
        else if ($("#hdnShippingHasError").val().toLowerCase() == "true" && $("#hdnShippingErrorMessage").val() != "" && $("#hdnShippingErrorMessage").val() != null && $("#hdnShippingErrorMessage").val() != 'undefined') {
            let shippingErrorMessage: string = $("#hdnShippingErrorMessage").val();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(shippingErrorMessage, 'error', isFadeOut, 10000);
            isValidate = false;
        }
        else if ($('#shippingMethodDiv input[name="ShippingId"]').length == 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectShippingMethod"), 'error', isFadeOut, fadeOutTime);
            isValidate = false;
        }
        else {
            if ($("#shippingMethodDiv #ShippingDetails :radio:checked").length == 0 && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectShippingMethod"), 'error', isFadeOut, fadeOutTime);
                isValidate = false;
            }
            else if ($("#hndShippingclassName").val() != 'undefined' && $("#hndShippingclassName").val().trim() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping) {
                if ($("#ShippingListViewModel_AccountNumber").val() == undefined || $("#ShippingListViewModel_AccountNumber").val() == "") {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterAccountNumber"), 'error', isFadeOut, fadeOutTime);
                    isValidate = false;
                }
                else if ($("#ShippingListViewModel_ShippingMethod").val() == 'undefined' && $("#ShippingListViewModel_ShippingMethod").val().trim == undefined || $("#ShippingListViewModel_ShippingMethod").val() == "") {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterShippingMethod"), 'error', isFadeOut, fadeOutTime);
                    isValidate = false;
                }
            }
        }

        return isValidate;
    }

    //Validate details of payment tab
    ValidatePaymentDetails(): boolean {
        let isValidate: boolean = true;
        if ($("#hdnTotalOrderAmount").val() == 'undefined' || $("#hdnTotalOrderAmount").val() > 0.00 && ($("#hdnOverDueAmount").val() >= 0.00)) {
            if ($("#ddlPaymentTypes option:selected").val() == '') {
                $('#' + $(this).attr('id')).addClass('input-validation-error');
                $('#' + $(this).attr('id')).attr('style', 'border: 1px solid rgb(195, 195, 195)');
                $('span#valPaymentTypes').removeClass('field-validation-valid');
                $('span#valPaymentTypes').addClass('field-validation-error');
                $('span#valPaymentTypes').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectPaymentType"));
                isValidate = false;
            }
            if ($("#ddlPaymentTypes option:selected").attr("id") != null && $("#ddlPaymentTypes option:selected").attr("id") != undefined) {
                if ($("#ddlPaymentTypes option:selected").attr("data-payment-type").toLowerCase() == 'credit_card') {
                    if ($("#hdnGatewayCode").val() == "cardconnect") {
                        if ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1) {
                            if (Order.prototype.ValidateCardConnectDataToken() && Order.prototype.ValidateCardConnectCardHolderName()) {
                                var cardType = Order.prototype.DetectCardTypeForCardConnect($('#CardDataToken').val());
                                isValidate = Order.prototype.ValidateCardType(cardType);
                            }
                            else {
                                isValidate = false;
                                ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#ErrorMessage").val(), "error", isFadeOut, fadeOutTime);
                            }
                        }
                        else
                            isValidate = Order.prototype.ValidateCardCVV();
                    }
                    else if ($("#hdnGatewayCode").val().toLowerCase() == "cybersource") {
                            isValidate = true;
                    }
                    else if ($("#hdnGatewayCode").val().toLowerCase() == Constant.BrainTree) {
                        isValidate = Order.prototype.ValidateBrainTreeCardDetails();
                    }
                    else {
                        if (!Order.prototype.ValidateDetails("false"))
                            isValidate = false;
                    }

                } else if ($("#ddlPaymentTypes option:selected").attr("data-payment-type").toLowerCase() == 'purchase_order') {
                    let poControl: any = $("#PurchaseOrderNumber");
                    let purchaseOrderNumber: string = poControl.val();
                    if (purchaseOrderNumber == "" || purchaseOrderNumber == null || purchaseOrderNumber == 'undefined') {
                        $("#cart-ponumber-status").show();
                        poControl.addClass('input-validation-error');
                        isValidate = false;
                    }
                }
                else if ($("#ddlPaymentTypes option:selected").attr("data-payment-type").toLowerCase() == 'ach') {
                    if ($("#addNewACHAccount-panel").attr("class").indexOf("active") != -1) {
                        if (Order.prototype.ValidateCardConnectDataToken()) {
                            isValidate = true;
                        }
                        else {
                            isValidate = false;
                            ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#ErrorMessage").val(), "error", isFadeOut, fadeOutTime);
                        }
                    }
                }
                else {
                    $("#cart-ponumber-status").hide();
                }
            }
        }
        return isValidate;
    }

    //Validate the braintree fields is null or not
    ValidateBrainTreeCardDetails() {
        if (($('#hdnBraintreecardNumber').val() == "" && ($('#hdnBraintreecardNumber').val().length <= 0 || $('#hdnBraintreecardNumber').val().length > 4)) &&
            $("#hdnBraintreeCardExpirationMonth").val() == "" && $("#hdnBraintreeCardExpirationYear").val() == "" &&
            $("#hdnBraintreeCardHolderName").val() == "" && $("#hdnBraintreeCardType").val() == "" && $("#hdnBraintreeNonce").val() == "") {
            return false;
        } else {
            return true;
        }
    }

    //This method will direct open the shopping cart tab instead of customer tab in create order
    ContinueToCartTab(): any {
        if ($("#hdnActionName").val() == "reordercompleteorder") {
            let activeTab: string = $("#OrderAsidePannel>li.active").attr("id");
            var nextTab = $("#OrderAsidePannel>li.active").next('li');

            nextTab.removeClass("disabled");
            nextTab.find('a').trigger('click');
            if (Order.prototype.IsQuote() && activeTab == "shipping-cart-tab-link") {
                nextTab.next('li').removeClass("disabled");
            }
        }
    }
}