declare var enabledPaymentProviders;
declare var savedUserCCDetails;
declare var savedUserACHAccountDetails;
declare var iframeUrl;
declare var coupons: any;
declare var vouchers: any;
declare var dataLayer: any;
declare var PaymentauthHeader: any;
declare function submitCard(model: any, callBack: any): any;
class Checkout extends ZnodeBase {
    public isPayMentInProcess: boolean = false;
    constructor() {
        super();
    }

    Init() {
        Checkout.prototype.SelectShippingOption();
        $("#applyCoupon").submit();
        $("#promocode").removeClass("promotion-block");
        $("#RequiredCouponErrorMessage").html('');
        $("#applyGiftCard").submit();
        $("#RequiredgiftCardErrorMessage").html('');
        $("#giftCard").removeClass("promotion-block");
        $(".cart-item-remove").remove();
    }

    SelectShippingOption(): void {
        var shippingId = $("#ShippingId").val();
        if (shippingId != undefined && shippingId > 0) {
            $("input[name='ShippingOptions']").each(function () {
                if ($(this).val() == shippingId) {
                    $(this).prop('checked', 'checked');
                    Checkout.prototype.CalculateShipping("");
                }
            });
        }
    }

    SelectShippingOptionForAmaoznPay(): boolean
    {
        var shippingOption = true;
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        if ((shippingOptionValue == null || shippingOptionValue == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            $('[id*="Amazon_Pay"]').prop("checked", false);
            $("#payWithAmazonDiv").hide();
            shippingOption = false;
            
        }
        
        var CustomShipping = (Checkout.prototype.IsCheckoutDataValidForAmazonPay());
        if (shippingOption && CustomShipping) {
            return true;
        }
        else
        {
            if ($('[id*="Amazon_Pay"]').prop("checked", true)) {
                $('[id*="Amazon_Pay"]').prop("checked", false);
                $("#payWithAmazonDiv").hide();
            }
            return false;
        }
    }
    
    ShippingOptions(isCalculateCart: boolean = true): void {
        Checkout.prototype.HideShippingDiv();
        $("#loaderId").html(" <div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
        var isQuoteRequest = $('#IsQuoteRequest').val();
        var isPendingOrderRequest = $('#IsPendingOrderRequest').val();
        Endpoint.prototype.ShippingOptions(true, isQuoteRequest, isPendingOrderRequest, function (response) {
            $("#loaderId").html("");
            if (response == null || response == undefined || response == "") {
                $(".shipping-method").html(ZnodeBase.prototype.getResourceByKeyName("InvalidAddressSelection"));
            }
            else {
                $(".shipping-method").html(response);
                Checkout.prototype.DisableShippingForFreeShippingAndDownloadableProduct();
                /* Call calculate method to calculate tax while changing address*/
                if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() != null) {
                    Checkout.prototype.CalculateShipping($("#hndShippingclassName").val(), isCalculateCart);
                }
                else {
                    Checkout.prototype.CalculateShipping("", isCalculateCart);
                }
            }
        });
    }

    GetOrderAndPaymentDetails(omsOrderId) {
        $("#payment-view-popup-ipad").addClass('show');
        $("#payment-view-popup-ipad").attr('style', 'padding-right: 21px; display: block;');
        $("#order-view-content").html('')
        $('#payment-view-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");

        $("#errorPayment").html("");

        Endpoint.prototype.GetOrderAndPaymentDetails(omsOrderId, $("#hdnPortalId").val(), function (response) {
            $("#order-view-content").html(response.orderHtml);
            $("#payment-view-content").html(response.paymentOptionHtml);
            $("#applyGiftCardDiv").hide();
            $("#paymentMethodDiv").hide();
            $("#paymentType").hide();
        });
    }

    GetOrderAndPaymentDetailsForInvoice(data: any): any {
        data.attr("href", "#");
        var omsOrderId: number = (data.attr("data-parameter").split('=')[1]).split('&')[0];

        Checkout.prototype.GetOrderAndPaymentDetails(omsOrderId);

    }

    MakePaymentAndGetOrderAndPaymentDetails(): any {
        var omsOrderId: number = $("#OmsOrderId").val();
        Checkout.prototype.GetOrderAndPaymentDetails(omsOrderId);
    }

    SetOfflinePayment(): any {
        $("[data-swhgcontainer='ZnodeWebStoreOrder'] tbody tr").each(function () {
            var paymentType = ((($(this).find('.zf-dollar').attr("data-parameter").toString()).split('PaymentType=')[1]).split('&')[0]).toLocaleLowerCase();
            var remainingOrderAmount = ((($(this).find('.zf-dollar').attr("data-parameter").toString()).split('RemainingOrderAmount=')[1])).toLocaleLowerCase();
            if (!Checkout.prototype.GetPaymentOptionListToHide().includes(paymentType)) {
                $(this).find('.zf-dollar').hide();
            }
            else {
                if (parseFloat(remainingOrderAmount) <= 0) {
                    $(this).find('.zf-dollar').attr("disabled", true);
                    $(this).find('.zf-dollar').css({ "pointer-events": "none" });
                }
            }
        });
    }

    public SavedNewcart(): any {
        var templatename = $("#SavedCartName").val();
        var regex = new RegExp("^[A-Za-z0-9 ]+$");
        if (templatename != "" && templatename != undefined && templatename != null) {
            if (regex.test(templatename)) {
                if (!(templatename.length > 100)) {
                    Endpoint.prototype.SavedNewcart(templatename, function (response) {
                        if (response.status == true) {

                            $("#savecartmodal").modal('hide');
                            window.location.href = "/SavedCart/SavedCartList";
                        }
                        else {
                            $('#errorname').text(ZnodeBase.prototype.getResourceByKeyName("DuplicateCart"));
                        }
                    });
                }
                else {
                    $('#errorname').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSavedcartName"));
                }
            }
            else {
                $('#errorname').text(ZnodeBase.prototype.getResourceByKeyName("alphanumeric"));
            }
        }
        else {
            $('#errorname').text(ZnodeBase.prototype.getResourceByKeyName("NoCart"));
        }

    }

    public GetSavedCartList(): any {
        Endpoint.prototype.GetSavedCartList(function (response) {
            $("#SavedCarts-view-content").html(response.templateHtml);
        });
    }

    public Saved(): any {
        var omsTemplateId = $('input[name="PaymentOptions"]:checked').val();
        $("#selecterrorname").text("");

        if (omsTemplateId != null && omsTemplateId != undefined && omsTemplateId != "") {
            Endpoint.prototype.EditSaveCart(omsTemplateId, function (response) {
                if (response.status == true) {

                    $("#savecartmodal").modal('hide');
                    window.location.href = "/SavedCart/SavedCartList";
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessSavedCart"), "success", false, 0);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessSavedCart"), "error", false, 0);
                }
            });
        }
        else {
            $('#selecterrorname').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectcartName"));
        }
    }

    GetPaymentOptionListToHide(): any {
        var paymentOptionList = ['purchase_order', 'invoice me'];
        return paymentOptionList;
    }

    PaymentOptions(): void {
        Endpoint.prototype.PaymentOptions(true, false, function (response) {
            $("#allPaymentOptionsDiv").html(response);
            Checkout.prototype.DisableShippingForFreeShippingAndDownloadableProduct();
        });
    }

    ShowTaxDetails(): void {
        if ($("#TaxSummary").is(':visible') && $(".ava-tax-msg").is(':visible')) {
            $("#TaxSummary").hide()
            $(".ava-tax-msg").hide()
        }
        else {
            $("#TaxSummary").show()
            $(".ava-tax-msg").show()
        }
    }

    SubmitOrder(): any {
        var paymentCode = $('#hdnGatwayName').val();
        if (paymentCode == Constant.CyberSource) {
            let isValid: boolean = false;
            isValid = Checkout.prototype.ValidateCyberSourceCard(event);
            if (isValid) {
                if ($('ul#creditCardTab ').find('li').find('a.active').attr('href') == "#savedCreditCard-panel" && $('ul#creditCardTab ').find('li.active').find('a').attr('href') == "#savedCreditCard-panel") {
                    Checkout.prototype.SubmitCyberSourcePayment("");

                }
                else {
                    $("#pay-button").click();
                }
            }
        }
        else {
            Checkout.prototype.SubmitOrderCheckout();
        }

    };

    SubmitOrderCheckout(): any {
        Checkout.prototype.ShowLoader();
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);
        //Set recipient name if recipient textbox is available
        Checkout.prototype.SaveRecipientNameAddressData('shipping', function (response) {
            Checkout.prototype.isPayMentInProcess = true;
            Checkout.prototype.HidePaymentLoader();
            if (!Checkout.prototype.IsCheckoutDataValid()) {
                Checkout.prototype.isPayMentInProcess = false;
                ZnodeBase.prototype.HideLoader();
                Checkout.prototype.HideModal();
            }
            else {
                if (!Checkout.prototype.ShippingErrorMessage()) {
                    Checkout.prototype.HideLoader();
                    Checkout.prototype.isPayMentInProcess = false;
                    return false;
                }

                if ($("#dynamic-allowesterritories").length > 0) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AllowedTerritories"), "error", false, 0);
                    Checkout.prototype.isPayMentInProcess = false;
                    Checkout.prototype.HideLoader();
                    return false;
                }

                else {
                    var isNotGuest: boolean = (parseInt($('#hdnAnonymousUser').val()) > 0);
                    var isApprovalRequired = "";
                    var isOABRequired = "";
                    if ($("input[name='PaymentOptions']:checked").length > 0) {
                        isApprovalRequired = $("input[name='PaymentOptions']:checked").attr("data-isApprovalRequired").toLowerCase();
                        isOABRequired = $("input[name='PaymentOptions']:checked").attr("data-isOABRequired").toLowerCase();
                    }
                    var isStoreLevelAppoverOn = $("#EnableApprovalRouting").val().toLowerCase();
                    var isStoreLevelApproverType = $("#ApprovalType").val().toLowerCase();
                    var userType = ZnodeBase.prototype.GetParameterValues("mode");
                    if (userType == undefined) {
                        userType = "";
                    }
                    userType = (userType != "") ? userType.replace("#", "") : userType;
                    switch (paymentType.toLowerCase()) {
                        case "credit_card":
                            if (userType != "guest") {
                                if (Checkout.prototype.SetFlagForApprovalRouting(isApprovalRequired, isOABRequired, isStoreLevelAppoverOn) && isStoreLevelApproverType != "payment") {
                                    Checkout.prototype.SubmitForApproval();
                                }
                                else {
                                    Checkout.prototype.SubmitPayment();
                                }
                            }
                            else {
                                Checkout.prototype.SubmitPayment();
                            }

                            break;
                        case "cod":
                            $("#btnCompleteCheckout").prop("disabled", false);
                            $("#btnCompleteCheckout").show();
                            $('#txtPurchaseOrderNumber').val('');
                            Checkout.prototype.PaymentStoreApproval(userType, isApprovalRequired, isOABRequired, isStoreLevelAppoverOn, isStoreLevelApproverType, isNotGuest);
                            break;
                        case "ach":
                            $("#btnCompleteCheckout").prop("disabled", false);
                            if (userType != "guest") {
                                if (Checkout.prototype.SetFlagForApprovalRouting(isApprovalRequired, isOABRequired, isStoreLevelAppoverOn) && isStoreLevelApproverType != "payment") {
                                    Checkout.prototype.SubmitForApproval();
                                }
                                else {
                                    Checkout.prototype.SubmitPaymentForACH();
                                }
                            }
                            else {
                                Checkout.prototype.SubmitPaymentForACH();
                            }

                            break;
                        default:
                            $("#btnCompleteCheckout").prop("disabled", false);
                            $("#btnCompleteCheckout").show();
                            // global data
                            if (Checkout.prototype.CheckValidPODocument()) {
                                Checkout.prototype.PaymentStoreApproval(userType, isApprovalRequired, isOABRequired, isStoreLevelAppoverOn, isStoreLevelApproverType, isNotGuest);
                            }
                    }
                }
            }
        });
        if (!(paymentType.toLowerCase() == "ach")) {
            $("#btnCompleteCheckout").prop("disabled", true);
        }
    };


    ShippingErrorMessage(isLoaderHide: boolean = true): any {
        var shippingErrorMessage = $("#ShippingErrorMessage").val();
        var shippingHasError = $("#ValidShippingSetting").val();
        this.ShowGiftCardMessage();
        if (isLoaderHide)
            Checkout.prototype.HidePaymentLoader();

        if (shippingHasError != null && shippingHasError != "" && shippingHasError != 'undefined' && shippingHasError.toLowerCase() == "false" && shippingErrorMessage != null && shippingErrorMessage != "" && shippingErrorMessage != 'undefined') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(shippingErrorMessage, "error", false, 0);
            return false;
        }
        else if (shippingErrorMessage != null && shippingErrorMessage != "" && shippingErrorMessage != 'undefined') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(shippingErrorMessage, "error", false, 0);
            return true;
        }
        Checkout.prototype.DisablePaymentOnZeroOrderTotal();
        Checkout.prototype.ToggleFreeShipping();
        Checkout.prototype.ChangeSubmitOrderButtonText();
        Checkout.prototype.ShowHidePayPalButton();
        return true;
    }

    ShowHidePayPalButton(): any {
        var paymentOption: string = $("input[name='PaymentOptions']:checked").attr("id");
        paymentOption = Checkout.prototype.GetPaymentType(paymentOption);
        if (paymentOption == "paypal_express")
            $("#btnCompleteCheckout").hide();
        $("#btnConvertQuoteToOrder").hide();
    }

    InvoiceMe(): any {
        if (!$("#div-InvoiceMe input:checkbox").prop('checked')) {
            if (!$('#BillingAccountNumber').val())
                Checkout.prototype.EnableButton();
            else
                if (!$('#BillingAccountNumber').val())
                    Checkout.prototype.EnableButton();
                else
                    Checkout.prototype.DisableButton();
        }
        else
            Checkout.prototype.DisableButton();
    }

    EnableButton(): any {
        $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", true);
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Please select the checkbox to proceed", "error", true, 10000);
    }

    DisableButton(): any {
        $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false)
    }

    //Get all the selected values required to submit order.
    SetOrderFormData(data): any {
        data["__RequestVerificationToken"] = $("input[name=__RequestVerificationToken]").val();
        data["ShippingOptionId"] = $("input[name='ShippingOptions']:checked").val();
        data["PaymentSettingId"] = $("input[name='PaymentOptions']:checked").val();
        data["ShippingAddressId"] = $("#shipping-content").find("#AddressId").val();
        data["BillingAddressId"] = $("#billing-content").find("#AddressId").val();
        data["AdditionalInstruction"] = $("#AdditionalInstruction").val();
        data["PurchaseOrderNumber"] = $("#txtPurchaseOrderNumber").val();
        data["PODocumentName"] = $("#po-document-path").val();
        data["AccountNumber"] = $("#AccountNumber").val();
        data["ShippingMethod"] = $("#ShippingMethod").val();
        data["InHandDate"] = $("#InHandDate").val();
        data["JobName"] = $("#JobName").val();
        data["ShippingConstraintCode"] = $("input[name='ShippingConstraintCode']:checked").val();
    }

    ShowLoaderForExistingCustomerLogin(): any {
        if ($("#login_username").val() != "" && $("#login_password").val() != "") {
            $("#loader-content-backdrop-login").show();
        }
    }

    DisableShippingForFreeShippingAndDownloadableProduct(): any {
        if ($("#cartFreeShipping").val() == "True" && $("#hdnIsFreeShipping").val() == "True") {
            $('input[name="ShippingOptions"]').prop('checked', false);
            $('input[name="ShippingOptions"]').next('label').addClass('disable-radio');
            $('[id*="FreeShipping_"]').attr("checked", "checked");
            var form = $("#form0");
            var shippingOptionId = $('[id*="FreeShipping_"]').val();
            var shippingAddressId = $("#shipping-content").find("#AddressId").val();
            var shippingCode = $('[id*="FreeShipping_"]').attr("data-shippingCode")
            $("#hndShippingclassName").val('ZnodeShippingCustom');
            if (shippingOptionId != null || shippingOptionId != undefined || shippingOptionId != "") {
                if (form.attr('action').match("shippingOptionId")) {
                    var url = form.attr('action').split('?')[0];
                    form.attr('action', "")
                    form.attr('action', url);
                }

                form.attr('action', form.attr('action') + "?shippingOptionId=" + shippingOptionId + "&shippingAddressId=" + shippingAddressId + "&shippingCode=" + shippingCode + "");
                form.submit();
            }
            $("#message-freeshipping").show();
        }
        else {
            $('input[name="ShippingOptions"]').next('label').removeClass('disable-radio')
        }
    }

    //Create form to submit order.
    CreateForm(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/Checkout/SubmitOrder");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        Checkout.prototype.AddFormFields(form, data);
        $("body").append(form);
        return form;
    }

    AddFormFields(form, data): any {
        if (data != null) {
            $.each(data, function (name, value) {
                if (value != null) {
                    var input = $("<input></input>").attr("type", "hidden").attr("name", name).val(value);
                    form.append(input);
                }
            });
            if ($("#PODocument") != null && $("#PODocument").val() != "") {
                form.append($("#PODocument"));
            }
        }
    }

    CalculateShipping(ShippingclassName: string, isCalculateCart: boolean = true): any {
        var form = $("#form0")
        if (form.length > 0) {
            var shippingOptionId = $("input[name='ShippingOptions']:checked").val();
            if (!(shippingOptionId > 0) && $('[id*="FreeShipping_"]').attr("checked")) {
                shippingOptionId = $('[id*="FreeShipping_"]').val();
            }
            var shippingAddressId = $("#shipping-content").find("#AddressId").val();
            var shippingCode = $("input[name='ShippingOptions']:checked").attr("data-shippingCode");
            var additionalInstruction = $('#AdditionalInstruction').val();
            var isQuoteRequest = $('#IsQuoteRequest').val();
            var jobName = $("#JobName").val();
            var isPendingOrderRequest = $('#IsPendingOrderRequest').val();
            $("#hndShippingclassName").val(ShippingclassName);
            $("#messageBoxContainerId").hide();
            if (ShippingclassName.toLowerCase() == (Constant.ZnodeCustomerShipping).toLowerCase()) {
                $("#customerShippingDiv").show();
            }
            else {
                $("#customerShippingDiv").hide();
            }
            if (shippingOptionId != null || shippingOptionId != "") {
                if (form.attr('action').match("shippingOptionId")) {
                    var url = form.attr('action').split('?')[0];
                    form.attr('action', "")
                    form.attr('action', url);
                }

                form.attr('action', form.attr('action') + "?shippingOptionId=" + shippingOptionId + "&shippingAddressId=" + shippingAddressId + "&shippingCode=" + shippingCode + "&additionalInstruction=" + additionalInstruction + "" + "&isQuoteRequest=" + isQuoteRequest + "&isCalculateCart=" + isCalculateCart + "&isPendingOrderRequest=" + isPendingOrderRequest + "&jobName=" + jobName);
                form.submit();
            }
            if ($('[id*="Amazon_Pay"]').prop("checked", true)) {
                $('[id*="Amazon_Pay"]').prop("checked", false);
                $("#payWithAmazonDiv").hide();
            }
            $(".payment-options-radio").prop("checked", false);
            $(".credit-card-container, .payment-details-block").hide();
        }
        $("#btnCompleteCheckout").prop("disabled", false);
    }

    SetUserCreationStatusMessage(response): any {
        var newUrl = window.location.href.replace('?mode=guest', '');
        if (response.hasError) {
            $("#ExistingUserError").html(response.message);
        }
        else if (response.status != undefined && response.status != null && !response.status) {
            $("#error-content").html(response.error);
        }
        else {
            window.location.href = newUrl;
        }
    }

    CheckDiscountCodeValue(codeType): any {
        Checkout.prototype.ShowLoader();
        var discountCode = "";
        if (codeType == "coupon") {
            discountCode = $("#promocode").val();
            if (discountCode == null || discountCode == "") {
                $("#promocode").addClass("promotion-block");
                $("#giftCard").removeClass("promotion-block");
                $("#RequiredgiftCardErrorMessage").html('');
                $("#RequiredCouponErrorMessage").html(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredCoupon"));
                Checkout.prototype.HideLoader();
                return false;
            }
            else
                $("#RequiredCouponErrorMessage").html('');
        }
        else if (codeType == "giftcard") {
            discountCode = $("#giftCard").val();
            if (discountCode == null || discountCode == "") {
                $("#giftCard").addClass("promotion-block");
                $("#promocode").removeClass("promotion-block");
                $("#RequiredCouponErrorMessage").html('');
                $("#RequiredgiftCardErrorMessage").html(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredVoucher"));
                Checkout.prototype.HideLoader();
                return false;
            }

            else if (parseFloat($("#hdnTotalOrderAmount").val().replace(',', '.')) <= 0.00) {
                $("#RequiredgiftCardErrorMessage").html(ZnodeBase.prototype.getResourceByKeyName("ErrorNoVoucherApplied"));
                Checkout.prototype.HideLoader();
                return false;
            }

            else if (typeof vouchers !== "undefined" && vouchers !== null) {
                if (vouchers !== null) {
                    var found = vouchers.some(el => el.VoucherNumber === discountCode && el.IsVoucherApplied == true);
                    if (found == true) {
                        $("#RequiredgiftCardErrorMessage").html(ZnodeBase.prototype.getResourceByKeyName("ErrorVoucherAlreadyApplied"));
                        Checkout.prototype.HideLoader();
                        return false;
                    }
                }
            }

            else
                $("#RequiredgiftCardErrorMessage").html('');
        }
    }

    DisplayAppliedDiscountCode(data): any {
        $("#summary-details").html(data.html);
        if ($("#CartOrdersummary").html() != "undefined") {

            $("#CartOrdersummary").html("");
            $("#CartOrdersummary").html("<div class='head-text'><h1>Order Summary</h1></div>" + data.html);
        }
        if ($('#dynamic-order-total'))
            $('#dynamic-cart-order-total').html($('#dynamic-order-total')[0].innerText);

        if ($('#hdnEncryptedTotalAmount'))
            $('#hdnEncryptedTotalAmount').val(data.encryptedTotalAmount);

        var htmlString = "<div class='col-xs-12 nopadding'>";
        if (data.isGiftCard) {
            vouchers = data.vouchers;
            Checkout.prototype.BindVoucherHtml(vouchers);
        }
        else {
            $("#cartFreeShipping").val(data.freeshipping);
            coupons = data.coupons;
            var validShippingPromotion = 0;
            var isShippingBasedCouponRemoved = data.isShippingBasedCoupon;

            for (var dataIndex = 0; dataIndex < coupons.length; dataIndex++) {
                var style = coupons[dataIndex].CouponApplied ? "success-msg padding-top" : "error-msg";
                var message = coupons[dataIndex].PromotionMessage;
                var couponCode = coupons[dataIndex].Code;

                if (coupons[dataIndex].CouponValid && (coupons[dataIndex].CouponPromotionType == Constant.AmountOffShipping
                    || coupons[dataIndex].CouponPromotionType == Constant.AmountOffShippingWithCarrier
                    || coupons[dataIndex].CouponPromotionType == Constant.PercentOffShipping
                    || coupons[dataIndex].CouponPromotionType == Constant.PercentOffShippingWithCarrier)) {
                    validShippingPromotion++;
                }

                Checkout.prototype.RemoveDiscountMessages();
                htmlString = htmlString + "<p class='text-break " + style + "'>" + "<a class='zf-close' onclick='Checkout.prototype.RemoveAppliedCoupon(" + dataIndex + ")' style='cursor:pointer;color:#cc0000;padding-right:3px;' title='Remove Coupon Code'></a>" + "<b>" + couponCode + "</b>" + " - " + message + "</p>";
            }
            htmlString = htmlString + "</div>";
            $("#couponMessageContainer").html("");
            $("#couponMessageContainer").html(htmlString);
            $("#promocode").removeClass("promotion-block");
        }
        Checkout.prototype.ChangeSubmitOrderButtonText();
        Checkout.prototype.DisablePaymentOnZeroOrderTotal();

        //Load shipping method if any shipping coupon is applied
        if (!data.isGiftCard && validShippingPromotion > 0) {
            Checkout.prototype.LoadShippingOptionsOnCouponAction();
        }

        //Load shipping method if any shipping coupon is removed
        if (!data.isGiftCard && isShippingBasedCouponRemoved) {
            Checkout.prototype.LoadShippingOptionsOnCouponAction();
        }

        Checkout.prototype.ToggleFreeShipping();
        Checkout.prototype.HideLoader();
    }

    BindVoucherHtml(vouchers): any {
        if (vouchers != null) {
            $("#RequiredgiftCardErrorMessage").html('');
            $("#giftCardMessageContainer").html("");
            for (var voucherIndex = 0; voucherIndex < vouchers.length; voucherIndex++) {
                var message = vouchers[voucherIndex].VoucherMessage;
                var isVoucherApplied = vouchers[voucherIndex].IsVoucherApplied;
                var voucherNumber = vouchers[voucherIndex].VoucherNumber;
                Checkout.prototype.AppendGiftCardMessage(message, isVoucherApplied, voucherIndex, voucherNumber, vouchers[voucherIndex].VoucherName, vouchers[voucherIndex].ExpirationDate, vouchers[voucherIndex].VoucherAmountUsed);
                if (isVoucherApplied == false) {
                    $("#giftCard").val(vouchers[voucherIndex].VoucherNumber);
                }
                $("#giftCard").removeClass("promotion-block");
            }
        }
    }

    RemoveDiscountMessages(): void {
        if ($("#couponMessageContainer .success-msg") != null) {
            $("couponMessageContainer .success-msg").each(function () { $(this).remove() });
        }

        if ($("couponMessageContainer .error-msg") != null) {
            $("couponMessageContainer .error-msg").each(function () { $(this).remove() });
        }
    }

    DisablePaymentOnZeroOrderTotal(): any {
        if ($("#hdnTotalOrderAmount").val().replace(',', '.') > 0.00) {
            $('input[name="PaymentOptions"]').next('label').removeClass('disable-radio')
        }
        else {
            Checkout.prototype.ShowHidePaymentOption('cod');
            $('input[name="PaymentOptions"]').prop('checked', false);
            $('input[name="PaymentOptions"]').next('label').addClass('disable-radio')
        }
    }

    RemoveAppliedCoupon(couponIndex): any {
        var _code = coupons[couponIndex].Code;
        coupons = new Array();
        Checkout.prototype.RemoveCoupon(_code);
    }

    RemoveCoupon(code: string): void {
        Checkout.prototype.ShowLoader();
        Endpoint.prototype.RemoveCouponCode(code, function (response) {
            Checkout.prototype.DisplayAppliedDiscountCode(response);
            $("#promocode").val("");
            Checkout.prototype.DisablePaymentOnZeroOrderTotal();
        })
    }

    RemoveGiftCard(): any {
        Checkout.prototype.ShowLoader();
        Endpoint.prototype.RemoveGiftCard("", function (response) {
            Checkout.prototype.DisplayAppliedDiscountCode(response);
            $("#giftCard").val("");
            $("#giftCardMessageContainer").html("");
            Checkout.prototype.DisablePaymentOnZeroOrderTotal();
        })
    }

    RemoveVoucher(code: string): any { 
        Checkout.prototype.ShowLoader();
        Endpoint.prototype.RemoveVoucher(code, function (response) {
            Checkout.prototype.DisplayAppliedDiscountCode(response);
            $("#giftCard").val("");
            Checkout.prototype.DisablePaymentOnZeroOrderTotal();
        })
    }

    RemoveAppliedVoucher(voucherIndex): any {
        var _code = vouchers[voucherIndex].VoucherNumber;
        vouchers = new Array();
        Checkout.prototype.RemoveVoucher(_code);
    }

    SetAddressErrorNotificationMessage(data) {
        if (data.status) {
            if (data.error == "")
                $("#dvBillingShippingContainer").load();
            if (data.error == ZnodeBase.prototype.getResourceByKeyName("AccessDeniedMessage"))
                window.location.href = '/404';
            $("#frmEditAddress_" + data.addressType).find("#AddressError").html(data.error);
            ZnodeBase.prototype.HideLoader();
        }
        else {
            $("#dvShippingOptions h3").next().html("<div id='loaderId'></div>");
            ZnodeBase.prototype.ShowLoader();
            var hostUrl: string = window.location.origin;
            setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
            //Save address and show loader for same shipping billing addresses.
            if ($("#is_both_billing_shipping").is(":checked")) {
                $("#Edit-Address-content_shipping").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
                $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress", "");
            }
            else if ($("#sameAsShipping").is(":checked")) {
                $("#Edit-Address-content_billing").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
                $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress", "");
            }
            //Save address and show loader for different shipping and billing addresses.
            else {
                if (data.addressType.toLowerCase() == "shipping".toLowerCase()) {
                    $("#shipping-address-content").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
                    $("#shipping-content").load(hostUrl + "/checkout/accountaddress" + " #shipping-content>*", "");
                }
                if (data.addressType.toLowerCase() == "billing".toLowerCase()) {
                    $("#billing-content").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
                    $("#BillingAddressContainer").load(hostUrl + "/checkout/accountaddress" + " #BillingAddressContainer>*", "");
                }
            }
            Checkout.prototype.RefreshAddressOptions(data.addressType, false);
            Checkout.prototype.ShippingOptions();
        }
    }

    ToggleBillingAddressBlock(): any {
        if ($("#IsBillingAddressOptional").val() == "true") {
            $("#BillingAddressContainer").hide();
            $("#shippingOptionSrNo").html("2");
            $("#paymentOptionSrNo").html("3");
            $("#cartReviewSerialNo").html("4");
            $("#same-as-billing").hide();
        } else {
            $("#BillingAddressContainer").show();
            $("#shippingOptionSrNo").html("3");
            $("#paymentOptionSrNo").html("4");
            $("#cartReviewSerialNo").html("5");
            $("#same-as-billing").show();
        }
    }

    public SaveChanges(event, id): any {
        var addressViewModel = {
            "AddressId": $("#shipping-content").find("#AddressId").val(),
            "Address1": $("#recommended-address1-" + id + "").text(),
            "Address2": $("#recommended-address2-" + id + "").text(),
            "CityName": $("#recommended-address-city-" + id + "").text(),
            "PostalCode": $("#recommended-address-postalcode-" + id + "").text().trim(),
            "StateName": $("#recommended-address-state-" + id + "").text(),
            "CountryName": $("#recommended-address-country-" + id + "").text()
        }
        Endpoint.prototype.UpdateSearchAddress(addressViewModel,
            function (response) {
                $("#SearchForLocationAddress").html("");
                $("#SearchForLocationAddress").html(response.html);
            });
        $('#custom-modal').modal('hide');
        $("#btnSaveAddress").closest("form").submit();
        return true;
    }

    ShowPaymentOptions(data): any {    
        $("#errorPayment").html("");
        var paymentId = data;
        var isApprovalRequired = $(paymentId).attr('data-isApprovalRequired').toLowerCase();
        var isOABRequired = $(paymentId).attr('data-isOABRequired').toLowerCase();
        var userId = $('#UserId').val();
        var isUsedForOfflinePayment = $("#IsUserForOfflinePayment").val();
        if ($("#IsBillingAddressOptional").length > 0) {
            $("#IsBillingAddressOptional").val("false");
        } else {
            $("#BillingAddressContainer").append("<input type='hidden' value='false' id='IsBillingAddressOptional' />");
        }
        Checkout.prototype.ChangeSubmitOrderButtonText();
        if (paymentId != null && paymentId != "" && paymentId != "undefined") {
            $("#messageBoxContainerId").hide();

            //Hiding Card Connect iframe while changing the payment method
            $('#divCardconnect #iframebody').hide();
            var controlId = paymentId.id;
            controlId = Checkout.prototype.GetPaymentType(controlId);
            switch (controlId.toLowerCase()) {
                case "cod":
                    $("#btnCompleteCheckout").show();
                    $("#btnConvertQuoteToOrder").show();
                    $('#divAuthorizeNetIFrame').hide();
                    $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false);
                    Checkout.prototype.ShowLoader();
                    Endpoint.prototype.GetPaymentDetails(paymentId.value, false, function (response) {
                        Checkout.prototype.BindOrderSummaryForOrder(response);

                        $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                        var billingId = $("#billing-content").find("#AddressId").val();
                        if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                            billingId = $("#shipping-content").find("#AddressId").val();
                            $("#billing-content").find("#AddressId").val(billingId);
                        }
                        HideBillingAddress();
                        Checkout.prototype.HideLoader();
                    });

                    Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                    break;
                case "purchase_order":
                    $("#btnCompleteCheckout").show();
                    $("#btnConvertQuoteToOrder").show();
                    $('#divAuthorizeNetIFrame').hide();
                    $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false);

                    Checkout.prototype.ShowLoader();
                    Endpoint.prototype.GetPaymentDetails(paymentId.value, false, function (response) {
                        Checkout.prototype.BindOrderSummaryForOrder(response);

                        $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                        var billingId = $("#billing-content").find("#AddressId").val();
                        if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                            billingId = $("#shipping-content").find("#AddressId").val();
                            $("#billing-content").find("#AddressId").val(billingId);
                        }
                        HideBillingAddress();
                        Checkout.prototype.HideLoader();

                    });
                    Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                    Checkout.prototype.GetPurchaseOrderHtml(controlId.toLowerCase());
                    break;
                case "credit_card":
                    $("#btnCompleteCheckout").show();
                    $("#btnConvertQuoteToOrder").show();
                    $("#payWithAmazonDiv").hide();
                    $("#div-InvoiceMe").hide();
                    $("#div-PaypalExpress").hide();
                    $("#div-ACHAccount").hide();
                    $("#divpurchase-order").hide();
                    $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false);

                    var Total = $("#Total").val();
                    if (!Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
                        return false;
                    }
                    $("#paymentAmount").attr("disabled", "disabled");
                    $("#paymentAmount").val($("#AmountDue").html())
                    $("#PaymentSettingId").val(paymentId.value);
                    $("#hdnGatwayName").val('');
                    $("#hdnEncryptedTotalAmount").val('');

                    Checkout.prototype.ShowLoader();
                    var quoteNumber = $("#QuoteNumber").val();

                    if (quoteNumber != undefined && quoteNumber != null) {
                        Endpoint.prototype.GetPaymentDetailsForQuotes(paymentId.value, true, quoteNumber, function (response) {
                            Checkout.prototype.ShowPaymentDetails(response, controlId);
                            if (!response.HasError) {
                                $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                HideBillingAddress();
                            }
                        });
                    }
                    else {
                        if (isUsedForOfflinePayment == "True") {
                            Endpoint.prototype.GetPaymentDetailsForInvoice(paymentId.value, true, isUsedForOfflinePayment, $("#paymentAmount").val(), function (response) {
                                Checkout.prototype.BindOrderSummaryForOrder(response);
                                Checkout.prototype.ShowPaymentDetails(response, controlId);
                                if (!response.HasError) {
                                    $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                    HideBillingAddress();
                                }
                            });
                        }
                        else {
                            Endpoint.prototype.GetPaymentDetails(paymentId.value, true, function (response) {
                                Checkout.prototype.BindOrderSummaryForOrder(response);
                                Checkout.prototype.ShowPaymentDetails(response, controlId);
                                if (!response.HasError) {
                                    $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                    HideBillingAddress();
                                }
                            });
                        }
                    }

                    break;
                case "paypal_express":
                    $("#PaymentSettingId").val(paymentId.value);
                    $("#btnCompleteCheckout").hide();
                    $("#btnConvertQuoteToOrder").hide();
                    Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                    Checkout.prototype.ShowLoader();
                    Endpoint.prototype.GetPaymentDetails(paymentId.value, false, function (response) {
                        Checkout.prototype.BindOrderSummaryForOrder(response);
                        if (!response.HasError) {
                            $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                            HideBillingAddress();
                            Checkout.prototype.SetPaymentDetails(response.Response);
                            var Total = $("#Total").val();
                            if (!Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
                                return false;
                            }
                        }
                        Checkout.prototype.HideLoader();
                    });
                    break;
                case "amazon_pay":
                    var ShippingMethod = Checkout.prototype.SelectShippingOptionForAmaoznPay();
                    if (ShippingMethod) {
                        $("#PaymentSettingId").val(paymentId.value);
                        $("#btnCompleteCheckout").hide();
                        $("#btnConvertQuoteToOrder").hide();
                        var CustomerAccountNumber = $("#AccountNumber").val()
                        $("#hdnAccountNumberShipping").val(CustomerAccountNumber);
                        localStorage.setItem("AccountNumber", CustomerAccountNumber);
                        var CustomerShippingMethod = $("#ShippingMethod").val();
                        $("#hdnShippingMethod").val(CustomerShippingMethod);
                        localStorage.setItem("ShippingMethod", CustomerShippingMethod);
                        Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                        Checkout.prototype.ShowLoader();
                        Endpoint.prototype.GetPaymentDetails(paymentId.value, false, function (response) {
                            Checkout.prototype.BindOrderSummaryForOrder(response);
                            if (!response.HasError) {
                                $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                HideBillingAddress();
                                Checkout.prototype.SetPaymentDetails(response);
                                var Total = $("#Total").val();
                                if (!Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
                                    return false;
                                }
                            }
                            Checkout.prototype.HideLoader();
                        });
                        break;
                    }
                    else {
                        break;
                    }
                case "ach":
                    $("#btnCompleteCheckout").show();
                    $("#btnConvertQuoteToOrder").show();
                    $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false);

                    var Total = $("#Total").val();
                    if (!Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
                        return false;
                    }
                    $("#paymentAmount").attr("disabled", false);

                    $("#PaymentSettingId").val(paymentId.value);
                    $("#hdnGatwayName").val('');
                    $("#hdnEncryptedTotalAmount").val('');

                    Checkout.prototype.ShowLoader();
                    var quoteNumber = $("#QuoteNumber").val();

                    if (quoteNumber != undefined && quoteNumber != null) {
                        Endpoint.prototype.GetPaymentDetailsForQuotes(paymentId.value, true, quoteNumber, function (response) {
                            Checkout.prototype.ShowACHPaymentDetails(response, controlId);
                            if (!response.HasError) {
                                $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                HideBillingAddress();
                            }
                        });
                    }
                    else {
                        if (isUsedForOfflinePayment == "True") {
                            Endpoint.prototype.GetPaymentDetailsForInvoice(paymentId.value, true, isUsedForOfflinePayment, $("#paymentAmount").val(), function (response) {
                                Checkout.prototype.BindOrderSummaryForOrder(response);
                                Checkout.prototype.ShowACHPaymentDetails(response, controlId);
                                if (!response.HasError) {
                                    $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                    HideBillingAddress();
                                }
                            });
                        }
                        else {
                            Endpoint.prototype.GetPaymentDetails(paymentId.value, true, function (response) {
                                Checkout.prototype.BindOrderSummaryForOrder(response);
                                Checkout.prototype.ShowACHPaymentDetails(response, controlId);
                                if (!response.HasError) {
                                    $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                                    HideBillingAddress();
                                }
                            });
                        }
                    }

                    break;
                default:
                    $("#btnCompleteCheckout").show();
                    $("#btnConvertQuoteToOrder").show();
                    Checkout.prototype.ShowLoader();
                    Endpoint.prototype.GetPaymentDetails(paymentId.value, false, function (response) {
                        Checkout.prototype.BindOrderSummaryForOrder(response);
                        if (!response.HasError) {
                            $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);

                            var billingId = $("#billing-content").find("#AddressId").val();
                            if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                                billingId = $("#shipping-content").find("#AddressId").val();
                                $("#billing-content").find("#AddressId").val(billingId);
                            }
                            HideBillingAddress();
                        }
                        Checkout.prototype.HideLoader();
                    });

                    Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                    break;
            }
            if ($("#btnConvertQuoteToOrder").attr("data-isquote") == "true") { isOABRequired = "false"; }

            if (isOABRequired == "true" && userId != '0') {
                $("#div-InvoiceMe").show();
                $("#div-CreditCard").hide();
                $("#div-PaypalExpress").hide();
                $("#divpurchase-order").hide();
                $("#payWithAmazonDiv").hide();
                $("#btnCompleteCheckout").show();
                Checkout.prototype.RemoveCreditCardValidationOnPaymentMethodChange();
                Checkout.prototype.InvoiceMe();
            }

        }
    }


    ShowPaymentDetails(response: any, controlId: any) {
        if (!response.HasError) {
            $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);
            Checkout.prototype.SetPaymentDetails(response.Response);
            Checkout.prototype.CreditCardPayment(controlId);
        }
        Checkout.prototype.HideLoader();
    }

    public CreditCardPaymentCyberSource(controlId): void {
        var paymentTokenModel = {
            PaymentSettingId: parseInt($('#PaymentSettingId').val()),
            PaymentCode: $('#hdnPaymentCode').val(),
            Total: $("#hdnTotalOrderAmount").val(),
            paymentGatewayId: $("#hdnPaymentGatewayId").val(),

        }
        Endpoint.prototype.GetPaymentGatewayToken(paymentTokenModel, function (response) {

            $("#divCreditCardCyberSource").html(response.html);
        })
        if ($("#hdnGatwayName").val() == Constant.CyberSource) {
            $("#creditCard").hide()
        }
    }

    ShowACHPaymentDetails(response: any, controlId: any) {
        if (!response.HasError) {
            $("#IsBillingAddressOptional").val(response.IsBillingAddressOptional);
            Checkout.prototype.SetPaymentDetails(response.Response);
            Checkout.prototype.ACHPayment(controlId);
        }
        Checkout.prototype.HideLoader();
    }

    ShowHidePaymentOption(paymentType) {
        switch (paymentType.toLowerCase()) {
            case "cod":
                $("#div-CreditCard").hide();
                $("#div-PaypalExpress").hide();
                $("#divpurchase-order").hide();
                $("#payWithAmazonDiv").hide();
                $("#div-InvoiceMe").hide();
                $("#div-ACHAccount").hide();
                $("#submitandpaybutton").hide();
                $("#divAuthorizeNetIFrame").html('');
                Checkout.prototype.RemoveCreditCardValidationOnPaymentMethodChange();
                break;
            case "purchase_order":
                $("#div-CreditCard").hide();
                $("#div-PaypalExpress").hide();
                $("#payWithAmazonDiv").hide();
                $("#div-InvoiceMe").hide();
                $("#div-ACHAccount").hide();
                $("#submitandpaybutton").hide();
                $("#divAuthorizeNetIFrame").html('');
                Checkout.prototype.RemoveCreditCardValidationOnPaymentMethodChange();
                break;
            case "credit_card":
                $("#div-CreditCard").show();
                if ($("#hdnGatwayName").val() == "cardconnect") {
                    $("#divCardconnect").show();
                    $("#creditCard").hide();
                    $("#divCreditCardCyberSource").hide();
                    $('#divAuthorizeNetIFrame').hide();
                    $("#submitandpaybutton").hide();
                    $("#divAuthorizeNetIFrame").html('');
                }
                else if ($("#hdnGatwayName").val() == Constant.CyberSource) {
                    $("#divCreditCardCyberSource").show();
                    $("#creditCard").hide();
                    $("#divCardconnect").hide();
                    $('#divAuthorizeNetIFrame').hide();
                    $('#paymentProviders').hide();
                    $("#submitandpaybutton").hide();
                    $("#divAuthorizeNetIFrame").html('');
                }
                else if ($("#hdnGatwayName").val() == "authorizenet") {
                    $('#paymentProviders').hide();
                    $("#divCreditCardCyberSource").hide();
                    $("#divCardconnect").hide()
                    $('#divAuthorizeNetIFrame').show();
                    $("#creditCard").hide();
                    $("#divAuthorizeNetIFramePrvoider").show();
                    $("#submitandpaybutton").attr("onclick", "Checkout.prototype.AuthorizeNetPayment()");
                }
                else if ($("#hdnGatwayName").val() == Constant.BrainTree) {
                    $('#paymentProviders').hide();
                    $("#divCreditCardCyberSource").hide();
                    $("#divCardconnect").hide()
                    $('#divAuthorizeNetIFrame').hide();
                    $("#creditCard").hide();
                    $("#divAuthorizeNetIFramePrvoider").show();
                    $("#submitandpaybutton").attr("onclick", "Checkout.prototype.BrainTreePayment()");
                }
                else {
                    $("#divCardconnect").hide()
                    $("#creditCard").show()
                    $('#divAuthorizeNetIFrame').hide();
                    $("#divCreditCardCyberSource").hide();
                    $("#divAuthorizeNetIFramePrvoider").hide();
                    $("#submitandpaybutton").hide();
                    $("#divAuthorizeNetIFrame").html('');
                }
                $("#div-PaypalExpress").hide();
                $("#divpurchase-order").hide();
                $("#payWithAmazonDiv").hide();
                $("#div-InvoiceMe").hide();
                $("#div-ACHAccount").hide();
                break;
            case "paypal_express":
                $("#div-CreditCard").hide();
                $("#divpurchase-order").hide();
                $("#div-PaypalExpress").show();
                $("#payWithAmazonDiv").hide();
                $("#div-InvoiceMe").hide();
                $("#div-ACHAccount").hide();
                $("#submitandpaybutton").hide();
                $("#divAuthorizeNetIFrame").html('');
                Checkout.prototype.RemoveCreditCardValidationOnPaymentMethodChange();
                break
            case "amazon_pay":
                $("#div-CreditCard").hide();
                $("#divpurchase-order").hide();
                $("#div-PaypalExpress").hide();
                $("#payWithAmazonDiv").show();
                $("#div-InvoiceMe").hide();
                $("#div-ACHAccount").hide();
                $("#submitandpaybutton").hide();
                $("#divAuthorizeNetIFrame").html('');
                Checkout.prototype.RemoveCreditCardValidationOnPaymentMethodChange();
                break
            case "ach":
                $("#div-ACHAccount").show();
                $("#div-CreditCard").hide();
                $("#submitandpaybutton").hide();
                $("#divAuthorizeNetIFrame").html('');
                if ($("#hdnGatwayName").val() == "cardconnect") {
                    $("#divCardconnect").show()
                    $("#creditCard").hide()
                    if ($("#btnClosePopup").attr('style') === 'display: none;' && $("#btnPayInvoice").attr('style') === 'display: none;') { $("#btnClosePopup").show(); $("#btnPayInvoice").show() }
                }
                else {
                    $("#divCardconnect").hide()
                    $("#creditCard").show()
                }
                $("#div-PaypalExpress").hide();
                $("#divpurchase-order").hide();
                $("#payWithAmazonDiv").hide();
                $("#div-InvoiceMe").hide();
                break;

            default:

        }
    }

    SetPaymentDetails(response): any {
        if (!response.HasError) {
            $("#hdnGatwayName").val(response.GatewayCode);
            $("#paymentProfileId").val(response.PaymentProfileId);
            $("#hdnPaymentCode").val(response.PaymentCode);
            $("#hdnEncryptedTotalAmount").val(response.Total);
            $("#hdnPaymentGatewayId").val(response.PaymentGatewayId);
        }
    }

    ClearPaymentAndDisplayMessage(message): any {
        Checkout.prototype.CanclePayment();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(message, "error", isFadeOut, fadeOutTime);
    }

    CanclePayment(): any {
        Checkout.prototype.HidePaymentProcessDialog();
        $("#div-CreditCard").hide();
        $("#div-CreditCard [data-payment='number']").val('');
        $("#div-CreditCard [data-payment='cvc']").val('');
        $("#div-CreditCard [data-payment='exp-month']").val('');
        $("#div-CreditCard [data-payment='exp-year']").val('');
        $("#div-CreditCard [data-payment='cardholderName']").val('');
        $("input[name='PaymentOptions']:checked").prop('checked', false);
        $("#payment-view-popup-ipad").modal('hide');
        if ($('#btnClosePopup').length > 0 && $('#btnPayInvoice').length > 0) {
            var closePopup = $('#btnClosePopup').attr('style');
            var payInvoice = $('#btnPayInvoice').attr('style');
            var displayNone = "display: none";
            if (closePopup.indexOf(displayNone) != -1 && (payInvoice).indexOf(displayNone) != -1) {
                $("#btnClosePopup").show();
                $("#btnPayInvoice").show();
            }
        }
    }

    SetCreditCardValidations(): any {
        $('input[data-payment="exp-month"]').on("keypress", function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });

        $('input[data-payment="exp-month"]').on("focusout", function (e) {
            var monthVal = $('input[data-payment="exp-month"]').val();
            if (monthVal.length == 1 && (monthVal >= 1 || monthVal <= 9)) {
                monthVal = 0 + monthVal;
                $('input[data-payment="exp-month"]').val(monthVal);
            }
        });
        $('input[data-payment="exp-year"]').on("keypress", function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
        $('input[data-payment="cvc"]').on("keypress", function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
    }

    SubmitAuthorizePayment(querystr: any) {
        var Total = $("#Total").val();
        var transactionResponse = JSON.parse(querystr);
        var orderNumber = $("#OrderNumber").val();
        if (!Checkout.prototype.IsCheckoutDataValid()) {
            Checkout.prototype.isPayMentInProcess = false;
            ZnodeBase.prototype.HideLoader();
            Checkout.prototype.HideModal();
        }
        else {
            if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
                var shippingId = $("#shipping-content").find("#AddressId").val();
                var billingId = $("#billing-content").find("#AddressId").val();
                if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                    billingId = $("#shipping-content").find("#AddressId").val();
                    $("#billing-content").find("#AddressId").val(billingId);
                }
                var currentStatus = Checkout.prototype.isPayMentInProcess;
                Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {                    
                    Checkout.prototype.isPayMentInProcess = currentStatus;
                    if (!response.Billing.HasError) {
                        if ($("#ajaxProcessPaymentError").html() == undefined) {
                        } else {
                            $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                        }
                        $("#AuthorizeNetModal").modal('hide');
                        Checkout.prototype.ShowPaymentProcessDialog();
                        var submitPaymentViewModel = {
                            PaymentSettingId: parseInt($('#PaymentSettingId').val()),
                            PaymentApplicationSettingId: parseInt($('#PaymentSettingId').val()),
                            PaymentCode: $('#hdnPaymentCode').val(),
                            PaymentType: 'credit_card',
                            ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                            BillingAddressId: $("#billing-content").find("#AddressId").val(),
                            ShippingOptionId: $("input[name='ShippingOptions']:checked").val(),
                            AdditionalInstruction: $("#AdditionalInstruction").val(),
                            Total: $("#Total").val(),
                            SubTotal: transactionResponse.totalAmount,
                            AccountNumber: $("#AccountNumber").val(),
                            ShippingMethod: $("#ShippingMethod").val(),
                            OrderNumber: orderNumber,
                            InHandDate: $("#InHandDate").val(),
                            JobName: $("#JobName").val(),
                            ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                            TransactionId: transactionResponse.transId,
                            IsSaveCreditCard: $("#AuthNetSaveCreditCard").is(':checked'),
                            CreditCardNumber: (transactionResponse.accountNumber).slice(-4),
                            CustomerProfileId: $('#CustomerProfileId').val(),
                            CustomerPaymentProfileId: $('#CustomerPaymentProfileId').val(),
                            GatewayCode: $("#hdnGatwayName").val()
                        };
                        var token = $("[name='__RequestVerificationToken']").val();
                        $.ajax({
                            type: "POST",
                            url: "/checkout/submitorder",
                            async: true,
                            data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
                            success: function (response) {
                                Checkout.prototype.isPayMentInProcess = false;
                                if (response.error != null && response.error != "" && response.error != 'undefined') {
                                    var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(message);
                                    Checkout.prototype.HideLoader();

                                    $("#submitandpaybutton").hide();
                                    return false;
                                } else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                    //This will focus to the top of screen.
                                    $(this).scrollTop(0);
                                    $('body, html').animate({ scrollTop: 0 }, 'fast');
                                    $(".cartcount").html('0');
                                    $("#messageBoxContainerId").hide();
                                    $(".cartAmount").html('');
                                    window.location.href = "/checkout/ordercheckoutreceipt";
                                }
                            },
                            error: function () {
                                Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                                Checkout.prototype.HideLoader();
                                $("#submitandpaybutton").hide();
                                return false;
                            }
                        });
                    }
                });
            }
        }
    }
    SubmitPayment(): any {

        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        var paymentCode = $('#hdnGatwayName').val();

        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            var isValid = true;
            if (!$("#radioCCList").is(':visible')) {
                $('input[data-payment="number"],input[data-payment="exp-month"],input[data-payment="exp-year"],input[data-payment="cvc"]').each(function () {
                    if ($.trim($(this).val()) == '') {
                        isValid = false;
                        $(this).css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                    } else {
                        $(this).css({
                            "border": "1px solid black",
                            "background": ""
                        });
                    }
                });

                if (paymentCode != "cardconnect" && paymentCode != Constant.BrainTree) {
                    isValid = Checkout.prototype.ValidateCreditCardDetails();
                } else if (paymentCode === Constant.BrainTree) {
                    isValid = Checkout.prototype.ValidateBrainTreeCardDetails();
                }
                else {

                    isValid = Checkout.prototype.ValidateCardConnectDataToken() && Checkout.prototype.ValidateCardConnectCardHolderName();
                }
            }
            else {
                isValid = Checkout.prototype.ValidateCVV();
            }
            if (isValid == false) {
                Checkout.prototype.isPayMentInProcess = false;
                Checkout.prototype.HidePaymentProcessDialog();
                Checkout.prototype.HideLoader();
                if ($("#hdnGatwayName").val() !== "cardconnect") {
                    $('#payment-provider-content')[0].scrollIntoView(true);
                }
                return false;
            }

            if (isValid) {
                var shippingId = $("#shipping-content").find("#AddressId").val();

                var billingId = $("#billing-content").find("#AddressId").val();
                if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                    billingId = $("#shipping-content").find("#AddressId").val();
                    $("#billing-content").find("#AddressId").val(billingId);
                }
                var currentStatus = Checkout.prototype.isPayMentInProcess;
                Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {
                    Checkout.prototype.isPayMentInProcess = currentStatus;
                    if (!response.Billing.HasError) {
                        if ($("#ajaxProcessPaymentError").html() == undefined) {
                        } else {
                            $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                        }                       
                        $("#BrainTreeModal").modal('hide'); //Hide braintree modal
                        Checkout.prototype.ShowPaymentProcessDialog();

                        var BillingCity = response.Billing.CityName;
                        var BillingCountryCode = response.Billing.CountryName;
                        var BillingFirstName = response.Billing.FirstName;
                        var BillingLastName = response.Billing.LastName;
                        var BillingPhoneNumber = response.Billing.PhoneNumber;
                        var BillingPostalCode = response.Billing.PostalCode;
                        var BillingStateCode = response.Billing.StateName;
                        if (response.Billing.StateCode != undefined && response.Billing.StateCode != null && response.Billing.StateCode != "") {
                            BillingStateCode = response.Billing.StateCode;
                        }
                        var BillingStreetAddress1 = response.Billing.Address1;
                        var BillingStreetAddress2 = response.Billing.Address2;
                        var BillingEmailId = response.Billing.EmailAddress;

                        var ShippingCity = response.Shipping.CityName;
                        var ShippingCountryCode = response.Shipping.CountryName;
                        var ShippingFirstName = response.Shipping.FirstName;
                        var ShippingLastName = response.Shipping.LastName;
                        var ShippingPhoneNumber = response.Shipping.PhoneNumber;
                        var ShippingPostalCode = response.Shipping.PostalCode;
                        var ShippingStateCode = response.Shipping.StateName;
                        var ShippingStreetAddress1 = response.Shipping.Address1;
                        var ShippingStreetAddress2 = response.Shipping.Address2;
                        var ShippingEmailId = response.Shipping.EmailAddress;

                        var cardNumber: string
                        var CardExpirationMonth: string
                        var CardExpirationYear: string
                        var CardHolderName: string

                        if ($("#hdnGatwayName").val() == "cardconnect") {
                            cardNumber = $('#CardDataToken').val();
                            CardExpirationMonth = $("#CardExpirationDate").val().substring(4);
                            CardExpirationYear = $("#CardExpirationDate").val().substring(0, 4);
                            CardHolderName = $("#cardconnectCardHolderName").val();
                        }
                        else if ($("#hdnGatwayName").val() === Constant.BrainTree) {
                            cardNumber = $('#hdnBraintreecardNumber').val();
                            CardExpirationMonth = $("#hdnBraintreeCardExpirationMonth").val();
                            CardExpirationYear = $("#hdnBraintreeCardExpirationYear").val();
                            CardHolderName = $("#hdnBraintreeCardHolderName").val();
                        }
                        else {
                            cardNumber = $("#div-CreditCard [data-payment='number']").val().split(" ").join("");
                            CardExpirationMonth = $("#div-CreditCard [data-payment='exp-month']").val();
                            CardExpirationYear = $("#div-CreditCard [data-payment='exp-year']").val();
                            CardHolderName = $("#div-CreditCard [data-payment='cardholderName']").val();
                        }

                        var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? true : false;

                        var guid = $('#GUID').val();

                        var discount = $('#Discount').val();
                        var ShippingCost = $('#ShippingCost').val();

                        var SubTotal = $('#SubTotal').val();

                        var cardType = $("#hdnGatwayName").val() == "cardconnect" ? Checkout.prototype.DetectCardTypeForCardConnect(cardNumber) : $("#hdnGatwayName").val() === Constant.BrainTree ? $('#hdnBraintreeCardType').val() : Checkout.prototype.DetectCardType(cardNumber);
                        var orderNumber = response.orderNumber;
                        if (cardNumber != "") {
                            $("#hdnCreditCardNumber").val(cardNumber.slice(-4));
                        }
                        var gatewayCode = $("#hdnGatwayName").val();
                        if (gatewayCode != Constant.CyberSource && gatewayCode != Constant.BrainTree) {
                            if ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1) {
                                if (cardType.toLowerCase() != $("input[name='PaymentProviders']:checked").val().toLowerCase()) {
                                    Checkout.prototype.HidePaymentProcessDialog();
                                    var message = ZnodeBase.prototype.getResourceByKeyName("SelectedCardType") + $("input[name='PaymentProviders']:checked").val().toLowerCase() + ZnodeBase.prototype.getResourceByKeyName("SelectCardNumberAndCardType");

                                    if (message != undefined) {
                                        Checkout.prototype.ShowErrorPaymentDialog(message);
                                    }
                                    Checkout.prototype.HideLoader();
                                    return false;
                                }
                            }
                        }
                        var paymentSettingId = $('#PaymentSettingId').val();
                        var paymentCode = $('#hdnPaymentCode').val();

                        var CustomerPaymentProfileId = $('#CustomerPaymentProfileId').val();
                        var CustomerProfileId = $('#CustomerProfileId').val();
                        var CardDataToken = $('#CardDataToken').val();
                        var gatewayCode = $("#hdnGatwayName").val();
                        if (gatewayCode.toLowerCase() == 'payflow') {
                            if ($("#hdnEncryptedTotalAmount").val() != undefined && $("#hdnEncryptedTotalAmount").val() != null) {
                                Total = $("#hdnEncryptedTotalAmount").val();
                            }
                        }
                        if (Total.indexOf(',') > -1) {
                            Total.replace(',', '');
                        }
                        var companyName = response.Shipping.CompanyName;
                        var payment = {
                            "GUID": guid,
                            "GatewayType": gatewayCode,
                            "BillingCity": BillingCity,
                            "BillingCountryCode": BillingCountryCode,
                            "BillingFirstName": BillingFirstName,
                            "BillingLastName": BillingLastName,
                            "BillingPhoneNumber": BillingPhoneNumber,
                            "BillingPostalCode": BillingPostalCode,
                            "BillingStateCode": BillingStateCode,
                            "BillingStreetAddress1": BillingStreetAddress1,
                            "BillingStreetAddress2": BillingStreetAddress2,
                            "BillingEmailId": BillingEmailId,
                            "ShippingCost": ShippingCost,
                            "ShippingCity": ShippingCity,
                            "ShippingCountryCode": ShippingCountryCode,
                            "ShippingFirstName": ShippingFirstName,
                            "ShippingLastName": ShippingLastName,
                            "ShippingPhoneNumber": ShippingPhoneNumber,
                            "ShippingPostalCode": ShippingPostalCode,
                            "ShippingStateCode": ShippingStateCode,
                            "ShippingStreetAddress1": ShippingStreetAddress1,
                            "ShippingStreetAddress2": ShippingStreetAddress2,
                            "ShippingEmailId": ShippingEmailId,
                            "SubTotal": SubTotal,
                            "Total": Total,
                            "Discount": discount,
                            "PaymentToken": ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1) ? "" : $("input[name='CCListdetails']:checked").val(),
                            "CardNumber": cardNumber,
                            "CardExpirationMonth": CardExpirationMonth,
                            "CardExpirationYear": CardExpirationYear,
                            "GatewayCurrencyCode": $('#hdnCurrencyCode').val(),
                            "CustomerPaymentProfileId": CustomerPaymentProfileId,
                            "CustomerProfileId": gatewayCode === Constant.BrainTree ? null : CustomerProfileId,
                            "CardDataToken": CardDataToken,
                            "CardType": cardType,
                            "PaymentSettingId": paymentSettingId,
                            "IsAnonymousUser": IsAnonymousUser,
                            "IsSaveCreditCard": gatewayCode === Constant.BrainTree ? $("#hdnBraintreeIsVault").val() : $("#SaveCreditCard").is(':checked'), //Set default value to true for Braintree.
                            "CardHolderName": CardHolderName,
                            "CustomerGUID": $("#hdnCustomerGUID").val(),
                            "PaymentCode": paymentCode,
                            "OrderId": orderNumber,
                            "CompanyName": companyName
                        };
                        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
                        if (gatewayCode === Constant.BrainTree) {
                            payment["PaymentMethodNonce"] = $('#hdnBraintreeNonce').val();
                        }
                        $("#div-CreditCard").hide();
                        $("#divAuthorizeNetIFramePrvoider").hide();
                        submitCard(payment, function (response) {
                            if (response.GatewayResponse == undefined) {
                                if (response.indexOf("Unauthorized") > 0) {
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                                    Checkout.prototype.HideLoader();
                                    Checkout.prototype.isPayMentInProcess = false;
                                }
                            } else {
                                var isSuccess = response.GatewayResponse.IsSuccess;
                                if (isSuccess) {
                                    var submitPaymentViewModel = {
                                        PaymentSettingId: paymentSettingId,
                                        PaymentCode: paymentCode,
                                        CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                                        CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                                        CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                                        CustomerGuid: response.GatewayResponse.CustomerGUID,
                                        PaymentToken: $("input[name='CCdetails']:checked").val(),
                                        ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                                        BillingAddressId: $("#billing-content").find("#AddressId").val(),
                                        ShippingOptionId: $("input[name='ShippingOptions']:checked").val(),
                                        AdditionalInstruction: $("#AdditionalInstruction").val(),
                                        CreditCardNumber: $("#hdnCreditCardNumber").val(),
                                        CardSecurityCode: payment["CardSecurityCode"],
                                        Total: $("#Total").val(),
                                        SubTotal: $('#SubTotal').val(),
                                        AccountNumber: $("#AccountNumber").val(),
                                        ShippingMethod: $("#ShippingMethod").val(),
                                        OrderNumber: orderNumber,
                                        InHandDate: $("#InHandDate").val(),
                                        JobName: $("#JobName").val(),
                                        ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                                        GatewayCode: $("#hdnGatwayName").val()
                                    };
                                    if (gatewayCode === Constant.BrainTree) {                  
                                        submitPaymentViewModel["CardType"] = payment["CardType"];
                                    }
                                    var token = $("[name='__RequestVerificationToken']").val();
                                    $.ajax({
                                        type: "POST",
                                        url: "/checkout/submitorder",
                                        async: true,
                                        data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
                                        success: function (response) {
                                            Checkout.prototype.isPayMentInProcess = false;
                                            if (response.error != null && response.error != "" && response.error != 'undefined') {
                                                var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                                Checkout.prototype.ClearPaymentAndDisplayMessage(message);
                                                Checkout.prototype.HideLoader();
                                                return false;
                                            } else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                                Checkout.prototype.CanclePayment();
                                                //This will focus to the top of screen.
                                                $(this).scrollTop(0);
                                                $('body, html').animate({ scrollTop: 0 }, 'fast');
                                                $(".cartcount").html('0');
                                                $("#messageBoxContainerId").hide();
                                                $(".cartAmount").html('');
                                                window.location.href = "/checkout/ordercheckoutreceipt";
                                            }
                                        },
                                        error: function () {
                                            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                                            Checkout.prototype.HideLoader();
                                            return false;
                                        }
                                    });
                                }
                                else {
                                    Checkout.prototype.isPayMentInProcess = false;
                                    var errorMessage = response.GatewayResponse.ErrorMessage;
                                    if (errorMessage == undefined) {
                                        errorMessage = response.GatewayResponse.GatewayResponseData;
                                    }

                                    if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacementCardDataMissing"));
                                    } else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
                                        $("#div-CreditCard").show();
                                    } else if (errorMessage != null && errorMessage != undefined && errorMessage.indexOf('customer') > 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                    } else {
                                        switch (gatewayCode.toLowerCase()) {
                                            case "payflow":
                                                if (response.GatewayResponse.ResponseText)
                                                    Checkout.prototype.ClearPaymentAndDisplayMessage(response.GatewayResponse.ResponseText);
                                                else
                                                    Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                                break;
                                            case Constant.BrainTree:
                                                if (response.GatewayResponse.ResponseText) {
                                                    Checkout.prototype.ClearPaymentAndDisplayMessage(response.GatewayResponse.ResponseText);
                                                    $("#divAuthorizeNetIFramePrvoider").show();
                                                }
                                                else
                                                    Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                                    $("#BrainTreeModal").modal('hide');
                                                    $("#divAuthorizeNetIFramePrvoider").hide();
                                                break;
                                            default:
                                                Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                        }
                                    }
                                    Checkout.prototype.HideLoader();
                                }
                            }
                        });
                    }
                });
            }

        }
    }

    SubmitCyberSourcePayment(querystr: any) {
        var Total = $("#Total").val();

        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            var shippingId = $("#shipping-content").find("#AddressId").val();
            var billingId = $("#billing-content").find("#AddressId").val();
            if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                billingId = $("#shipping-content").find("#AddressId").val();
                $("#billing-content").find("#AddressId").val(billingId);
            }
            var currentStatus = Checkout.prototype.isPayMentInProcess;
            Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {
                Checkout.prototype.isPayMentInProcess = currentStatus;
                if (!response.Billing.HasError) {
                    if ($("#ajaxProcessPaymentError").html() == undefined) {
                    } else {
                        $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                    }
                    Checkout.prototype.ShowPaymentProcessDialog();

                    var orderNumber = response.orderNumber;
                    var submitPaymentViewModel = {
                        PaymentSettingId: parseInt($('#PaymentSettingId').val()),
                        PaymentApplicationSettingId: parseInt($('#PaymentSettingId').val()),
                        PaymentCode: $('#hdnPaymentCode').val(),
                        PaymentType: 'credit_card',
                        ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                        BillingAddressId: $("#billing-content").find("#AddressId").val(),
                        ShippingOptionId: $("input[name='ShippingOptions']:checked").val(),
                        AdditionalInstruction: $("#AdditionalInstruction").val(),
                        Total: $("#Total").val(),
                        SubTotal: $("#Total").val(),  //Need TO check
                        AccountNumber: $("#AccountNumber").val(),
                        ShippingMethod: $("#ShippingMethod").val(),
                        OrderNumber: orderNumber,
                        InHandDate: $("#InHandDate").val(),
                        JobName: $("#JobName").val(),
                        ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                        CyberSourceToken: querystr,
                        IsSaveCreditCard: $("#SaveCreditCard").is(':checked'),
                        CustomerProfileId: $('#CustomerProfileId').val(),
                        CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                        CustomerGuid: $("#hdnCustomerGUID").val(),
                        PaymentGUID: $("#hdnPaymentGUID").val(),
                        GatewayCode: $("#hdnGatwayName").val(),
                        CardType: 'credit_card',
                        CardHolderName: $("#cyscardholderName").val(),
                    };
                    var token = $("[name='__RequestVerificationToken']").val();
                    $.ajax({
                        type: "POST",
                        url: "/checkout/submitorder",
                        async: true,
                        data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
                        success: function (response) {
                            Checkout.prototype.isPayMentInProcess = false;
                            if (response.error != null && response.error != "" && response.error != 'undefined') {
                                var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                Checkout.prototype.ClearPaymentAndDisplayMessage(message);
                                Checkout.prototype.HideLoader();
                                return false;
                            } else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                Checkout.prototype.CanclePayment();
                                //This will focus to the top of screen.
                                $(this).scrollTop(0);
                                $('body, html').animate({ scrollTop: 0 }, 'fast');
                                $(".cartcount").html('0');
                                $("#messageBoxContainerId").hide();
                                $(".cartAmount").html('');
                                window.location.href = "/checkout/ordercheckoutreceipt";
                            }
                        },
                        error: function () {
                            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                            Checkout.prototype.HideLoader();
                            return false;
                        }
                    });
                }
            });
        }
    }

    SubmitPaymentForACH(): any {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        var isValid: boolean = true;

        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {

            if ($("#addNewACHAccount-panel").attr("class").indexOf("active") != -1) {
                isValid = Checkout.prototype.ValidateCardConnectDataToken();
            }

            if (isValid == false) {
                Checkout.prototype.isPayMentInProcess = false;
                Checkout.prototype.HidePaymentProcessDialog();
                Checkout.prototype.HideLoader();
                return false;
            }

            var shippingId = $("#shipping-content").find("#AddressId").val();

            var billingId = $("#billing-content").find("#AddressId").val();
            if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                billingId = $("#shipping-content").find("#AddressId").val();
                $("#billing-content").find("#AddressId").val(billingId);
            }
            var currentStatus = Checkout.prototype.isPayMentInProcess;
            Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {
                Checkout.prototype.isPayMentInProcess = currentStatus;
                if (!response.Billing.HasError) {
                    if ($("#ajaxProcessPaymentError").html() == undefined) {
                    } else {
                        $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                    }
                    Checkout.prototype.ShowPaymentProcessDialog();

                    var BillingCity = response.Billing.CityName;
                    var BillingCountryCode = response.Billing.CountryName;
                    var BillingFirstName = response.Billing.FirstName;
                    var BillingLastName = response.Billing.LastName;
                    var BillingPhoneNumber = response.Billing.PhoneNumber;
                    var BillingPostalCode = response.Billing.PostalCode;
                    var BillingStateCode = response.Billing.StateName;
                    if (response.Billing.StateCode != undefined && response.Billing.StateCode != null && response.Billing.StateCode != "") {
                        BillingStateCode = response.Billing.StateCode;
                    }
                    var BillingStreetAddress1 = response.Billing.Address1;
                    var BillingStreetAddress2 = response.Billing.Address2;
                    var BillingEmailId = response.Billing.EmailAddress;

                    var ShippingCity = response.Shipping.CityName;
                    var ShippingCountryCode = response.Shipping.CountryName;
                    var ShippingFirstName = response.Shipping.FirstName;
                    var ShippingLastName = response.Shipping.LastName;
                    var ShippingPhoneNumber = response.Shipping.PhoneNumber;
                    var ShippingPostalCode = response.Shipping.PostalCode;
                    var ShippingStateCode = response.Shipping.StateName;
                    var ShippingStreetAddress1 = response.Shipping.Address1;
                    var ShippingStreetAddress2 = response.Shipping.Address2;
                    var ShippingEmailId = response.Shipping.EmailAddress;


                    var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? true : false;

                    var guid = $('#GUID').val();

                    var discount = $('#Discount').val();
                    var ShippingCost = $('#ShippingCost').val();

                    var SubTotal = $('#SubTotal').val();

                    var orderNumber = response.orderNumber;
                    var paymentSettingId = $('#PaymentSettingId').val();
                    var paymentCode = $('#hdnPaymentCode').val();

                    var CustomerPaymentProfileId = $('#CustomerPaymentProfileId').val();
                    var CustomerProfileId = $('#CustomerProfileId').val();
                    var CardDataToken = $('#CardDataToken').val();
                    var gatewayCode = $("#hdnGatwayName").val();

                    var cardNumber: string

                    if ($("#hdnGatwayName").val() == "cardconnect") {
                        cardNumber = $('#CardDataToken').val();

                    }

                    if (cardNumber != "") {
                        $("#hdnCreditCardNumber").val(cardNumber.slice(-4));
                    }
                    var companyName = response.Shipping.CompanyName;
                    var payment = {
                        "GUID": guid,
                        "GatewayType": gatewayCode,
                        "BillingCity": BillingCity,
                        "BillingCountryCode": BillingCountryCode,
                        "BillingFirstName": BillingFirstName,
                        "BillingLastName": BillingLastName,
                        "BillingPhoneNumber": BillingPhoneNumber,
                        "BillingPostalCode": BillingPostalCode,
                        "BillingStateCode": BillingStateCode,
                        "BillingStreetAddress1": BillingStreetAddress1,
                        "BillingStreetAddress2": BillingStreetAddress2,
                        "BillingEmailId": BillingEmailId,
                        "ShippingCost": ShippingCost,
                        "ShippingCity": ShippingCity,
                        "ShippingCountryCode": ShippingCountryCode,
                        "ShippingFirstName": ShippingFirstName,
                        "ShippingLastName": ShippingLastName,
                        "ShippingPhoneNumber": ShippingPhoneNumber,
                        "ShippingPostalCode": ShippingPostalCode,
                        "ShippingStateCode": ShippingStateCode,
                        "ShippingStreetAddress1": ShippingStreetAddress1,
                        "ShippingStreetAddress2": ShippingStreetAddress2,
                        "ShippingEmailId": ShippingEmailId,
                        "SubTotal": SubTotal,
                        "Total": Total,
                        "Discount": discount,
                        "PaymentToken": ($("#addNewACHAccount-panel").attr("class").indexOf("active") != -1) ? "" : $("input[name='CCListdetails']:checked").val(),
                        "GatewayCurrencyCode": $('#hdnCurrencyCode').val(),
                        "CustomerPaymentProfileId": CustomerPaymentProfileId,
                        "CustomerProfileId": CustomerProfileId,
                        "CardDataToken": CardDataToken,
                        "PaymentSettingId": paymentSettingId,
                        "IsAnonymousUser": IsAnonymousUser,
                        "IsSaveCreditCard": $("#SaveACHAccount").is(':checked'),//changeee
                        "CardHolderName": "abcd",
                        "CustomerGUID": $("#hdnCustomerGUID").val(),
                        "PaymentCode": paymentCode,
                        "OrderId": orderNumber,
                        "CompanyName": companyName,
                        "CardNumber": cardNumber,
                        "IsACHPayment": true
                    };

                    submitCard(payment, function (response) {
                        if (response.GatewayResponse == undefined) {
                            if (response.indexOf("Unauthorized") > 0) {
                                Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                                Checkout.prototype.HideLoader();
                                Checkout.prototype.isPayMentInProcess = false;
                            }
                        } else {
                            var isSuccess = response.GatewayResponse.IsSuccess;
                            if (isSuccess) {
                                var submitPaymentViewModel = {
                                    PaymentSettingId: paymentSettingId,
                                    PaymentCode: paymentCode,
                                    CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                                    CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                                    CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                                    CustomerGuid: response.GatewayResponse.CustomerGUID,
                                    CreditCardNumber: $("#hdnCreditCardNumber").val(),
                                    PaymentToken: $("input[name='CCdetails']:checked").val(),
                                    ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                                    BillingAddressId: $("#billing-content").find("#AddressId").val(),
                                    ShippingOptionId: $("input[name='ShippingOptions']:checked").val(),
                                    AdditionalInstruction: $("#AdditionalInstruction").val(),
                                    Total: $("#Total").val(),
                                    SubTotal: $('#SubTotal').val(),
                                    AccountNumber: $("#AccountNumber").val(),
                                    ShippingMethod: $("#ShippingMethod").val(),
                                    OrderNumber: orderNumber,
                                    InHandDate: $("#InHandDate").val(),
                                    JobName: $("#JobName").val(),
                                    IsACHPayment: true,
                                    ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                                };
                                var token = $("[name='__RequestVerificationToken']").val();
                                $.ajax({
                                    type: "POST",
                                    url: "/checkout/submitorder",
                                    async: true,
                                    data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
                                    success: function (response) {
                                        Checkout.prototype.isPayMentInProcess = false;
                                        if (response.error != null && response.error != "" && response.error != 'undefined') {
                                            var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                            Checkout.prototype.ClearPaymentAndDisplayMessage(message);
                                            Checkout.prototype.HideLoader();
                                            return false;
                                        } else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                            Checkout.prototype.CanclePayment();
                                            //This will focus to the top of screen.
                                            $(this).scrollTop(0);
                                            $('body, html').animate({ scrollTop: 0 }, 'fast');
                                            $(".cartcount").html('0');
                                            $("#messageBoxContainerId").hide();
                                            $(".cartAmount").html('');
                                            window.location.href = "/checkout/ordercheckoutreceipt";
                                        }
                                    },
                                    error: function () {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                                        Checkout.prototype.HideLoader();
                                        return false;
                                    }
                                });
                            }
                            else {
                                Checkout.prototype.isPayMentInProcess = false;
                                var errorMessage = response.GatewayResponse.ErrorMessage;
                                if (errorMessage == undefined) {
                                    errorMessage = response.GatewayResponse.GatewayResponseData;
                                }

                                if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacementCardDataMissing"));
                                } else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
                                    $("#div-CreditCard").show();
                                } else if (errorMessage != null && errorMessage != undefined && errorMessage.indexOf('customer') > 0) {
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                } else {
                                    switch (gatewayCode.toLowerCase()) {
                                        case "payflow":
                                            if (response.GatewayResponse.ResponseText)
                                                Checkout.prototype.ClearPaymentAndDisplayMessage(response.GatewayResponse.ResponseText);
                                            else
                                                Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                            break;
                                        default:
                                            Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                    }
                                }
                                Checkout.prototype.HideLoader();
                            }
                        }
                    });
                }
            });

        }
    }

    GetPaymentErrorMsg(response): string {
        var errorCode = response["error"] ? response["error"].toLowerCase().split(",") : "";

        if ($.inArray("code: E00027".toLowerCase(), errorCode) >= 0)
            return ZnodeBase.prototype.getResourceByKeyName("ErrorCodeE00027");
        return response["error"];
    }

    SubmitQuotePayment(permissionCode): any {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            var isValid = true;
            if (!$("#radioCCList").is(':visible')) {
                $('input[data-payment="number"],input[data-payment="exp-month"],input[data-payment="exp-year"],input[data-payment="cvc"]').each(function () {
                    if ($.trim($(this).val()) == '') {
                        isValid = false;
                        $(this).css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                    } else {
                        $(this).css({
                            "border": "1px solid black",
                            "background": ""
                        });
                    }
                });
                isValid = Checkout.prototype.ValidateCreditCardDetails();
            }
            else {
                isValid = Checkout.prototype.ValidateCVV();
            }

            if (isValid == false) {
                Checkout.prototype.isPayMentInProcess = false;
                Checkout.prototype.HidePaymentProcessDialog();
                Checkout.prototype.HideLoader();
                return false;
            }

            if (isValid) {
                var currentStatus = Checkout.prototype.isPayMentInProcess;
                Endpoint.prototype.GetBillingAddressDetail($("#hdnPortalId").val(), $("#billing-content").find("#AddressId").val(), $("#shipping-content").find("#AddressId").val(), function (response) {
                    Checkout.prototype.isPayMentInProcess = currentStatus;
                    if (!response.HasError) {
                        if ($("#ajaxProcessPaymentError").html() == undefined) {
                        } else {
                            $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                        }
                        Checkout.prototype.ShowPaymentProcessDialog();

                        var BillingCity = response.data.BillingAddress.CityName;
                        var BillingCountryCode = response.data.BillingAddress.CountryName;
                        var BillingFirstName = response.data.BillingAddress.FirstName;
                        var BillingLastName = response.data.BillingAddress.LastName;
                        var BillingPhoneNumber = response.data.BillingAddress.PhoneNumber;
                        var BillingPostalCode = response.data.BillingAddress.PostalCode;
                        var BillingStateCode = response.data.BillingAddress.StateName;
                        if (response.data.BillingAddress.StateCode != undefined && response.data.BillingAddress.StateCode != null && response.data.BillingAddress.StateCode != "") {
                            BillingStateCode = response.data.BillingAddress.StateCode;
                        }
                        var BillingStreetAddress1 = response.data.BillingAddress.Address1;
                        var BillingStreetAddress2 = response.data.BillingAddress.Address2;
                        var BillingEmailId = response.data.BillingAddress.EmailAddress;

                        var ShippingCity = response.data.ShippingAddress.CityName;
                        var ShippingCountryCode = response.data.ShippingAddress.CountryName;
                        var ShippingFirstName = response.data.ShippingAddress.FirstName;
                        var ShippingLastName = response.data.ShippingAddress.LastName;
                        var ShippingPhoneNumber = response.data.ShippingAddress.PhoneNumber;
                        var ShippingPostalCode = response.data.ShippingAddress.PostalCode;
                        var ShippingStateCode = response.data.ShippingAddress.StateName;
                        if (response.data.ShippingAddress.StateCode != undefined && response.data.ShippingAddress.StateCode != null && response.data.ShippingAddress.StateCode != "") {
                            ShippingStateCode = response.data.ShippingAddress.StateCode;
                        }
                        var ShippingStreetAddress1 = response.data.ShippingAddress.Address1;
                        var ShippingStreetAddress2 = response.data.ShippingAddress.Address2;
                        var ShippingEmailId = response.data.ShippingAddress.EmailAddress;

                        var cardNumber = $("#div-CreditCard [data-payment='number']").val().split(" ").join("");

                        var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? true : false;

                        var guid = $('#GUID').val();

                        var discount = $('#Discount').val();
                        var ShippingCost = $('#ShippingCost').val();

                        var SubTotal = $('#SubTotal').val();

                        var cardType = Checkout.prototype.DetectCardType(cardNumber);
                        var cardExpirationMonth = $("#div-CreditCard [data-payment='exp-month']").val();
                        var cardExpirationYear = $("#div-CreditCard [data-payment='exp-year']").val();
                        var orderNumber = response.orderNumber;
                        var gatewayCode = $("#hdnGatwayName").val();
                        if (cardNumber != "") {
                            $("#hdnCreditCardNumber").val(cardNumber.slice(-4));
                        }

                        if (gatewayCode.toLowerCase() == 'payflow') {
                            if ($("#hdnEncryptedTotalAmount").val() != undefined && $("#hdnEncryptedTotalAmount").val() != null) {
                                Total = $("#hdnEncryptedTotalAmount").val();
                            }
                        }

                        if ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1) {
                            if (cardType.toLowerCase() != $("input[name='PaymentProviders']:checked").val().toLowerCase()) {
                                Checkout.prototype.HidePaymentProcessDialog();
                                Checkout.prototype.HideLoader();
                                var message = ZnodeBase.prototype.getResourceByKeyName("SelectedCardType") + $("input[name='PaymentProviders']:checked").val().toLowerCase() + ZnodeBase.prototype.getResourceByKeyName("SelectCardNumberAndCardType");

                                if (message != undefined) {
                                    Checkout.prototype.ShowErrorPaymentDialog(message);
                                }
                                Checkout.prototype.isPayMentInProcess = false;
                                return false;
                            }
                        }

                        var paymentSettingId = $('#PaymentSettingId').val();
                        var paymentCode = $('#hdnPaymentCode').val();

                        var CustomerPaymentProfileId = $('#CustomerPaymentProfileId').val();
                        var CustomerProfileId = $('#CustomerProfileId').val();
                        var CardDataToken = $('#CardDataToken').val();
                        var gatewayCode = $("#hdnGatwayName").val();
                        if (gatewayCode.toLowerCase() == 'payflow') {
                            if ($("#hdnEncryptedTotalAmount").val() != undefined && $("#hdnEncryptedTotalAmount").val() != null) {
                                Total = $("#hdnEncryptedTotalAmount").val();
                            }
                        }
                        if (Total.indexOf(',') > -1) {
                            Total.replace(',', '');
                        }
                        var payment = {
                            "GUID": guid,
                            "GatewayType": gatewayCode,
                            "BillingCity": BillingCity,
                            "BillingCountryCode": BillingCountryCode,
                            "BillingFirstName": BillingFirstName,
                            "BillingLastName": BillingLastName,
                            "BillingPhoneNumber": BillingPhoneNumber,
                            "BillingPostalCode": BillingPostalCode,
                            "BillingStateCode": BillingStateCode,
                            "BillingStreetAddress1": BillingStreetAddress1,
                            "BillingStreetAddress2": BillingStreetAddress2,
                            "BillingEmailId": BillingEmailId,
                            "ShippingCity": ShippingCity,
                            "ShippingCountryCode": ShippingCountryCode,
                            "ShippingFirstName": ShippingFirstName,
                            "ShippingLastName": ShippingLastName,
                            "ShippingPhoneNumber": ShippingPhoneNumber,
                            "ShippingPostalCode": ShippingPostalCode,
                            "ShippingStateCode": ShippingStateCode,
                            "ShippingStreetAddress1": ShippingStreetAddress1,
                            "ShippingStreetAddress2": ShippingStreetAddress2,
                            "ShippingEmailId": ShippingEmailId,
                            "ShippingCost": ShippingCost,
                            "SubTotal": SubTotal,
                            "Total": Total,
                            "Discount": discount,
                            "CardSecurityCode": $("#div-CreditCard [data-payment='cvc']").val(),
                            "CardNumber": cardNumber,
                            "CardExpirationMonth": cardExpirationMonth,
                            "CardExpirationYear": cardExpirationYear,
                            "GatewayCurrencyCode": $('#hdnCurrencySuffix').val(),
                            "CustomerPaymentProfileId": CustomerPaymentProfileId,
                            "CustomerProfileId": CustomerProfileId,
                            "CardDataToken": CardDataToken,
                            "CardType": cardType,
                            "PaymentSettingId": paymentSettingId,
                            "IsAnonymousUser": IsAnonymousUser,
                            "IsSaveCreditCard": $("#SaveCreditCard").is(':checked'),
                            "CardHolderName": $("#div-CreditCard [data-payment='cardholderName']").val(),
                            "CustomerGUID": $("#hdnCustomerGUID").val(),
                            "PaymentToken": ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1) ? "" : $("input[name='CCListdetails']:checked").val(),
                            "PaymentCode": paymentCode,
                            "OrderId": orderNumber
                        };

                        $("#div-CreditCard").hide();

                        submitCard(payment, function (response) {
                            if (response.GatewayResponse == undefined) {
                                if (response.indexOf("Unauthorized") > 0) {
                                    Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                                    Checkout.prototype.HideLoader();
                                }
                                Checkout.prototype.isPayMentInProcess = false;
                            } else {
                                var isSuccess = response.GatewayResponse.IsSuccess;
                                if (isSuccess) {
                                    Checkout.prototype.isPayMentInProcess = false;
                                    var submitQuoteViewModel = {
                                        PaymentCode: paymentCode,
                                        CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                                        CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                                        CustomerGuid: response.GatewayResponse.CustomerGUID,
                                        PaymentToken: $("input[name='CCdetails']:checked").val(),
                                        ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                                        BillingAddressId: $("#billing-content").find("#AddressId").val(),
                                        ShippingOptionId: $("input[name='ShippingOptions']:checked").val(),
                                        AdditionalInstruction: $("#AdditionalInstruction").val(),
                                        CreditCardNumber: $("#hdnCreditCardNumber").val(),
                                        Total: $("#Total").val(),
                                        SubTotal: $('#SubTotal').val(),
                                        AccountNumber: $("#AccountNumber").val(),
                                        ShippingMethod: $("#ShippingMethod").val(),
                                        OmsOrderState: "PENDING APPROVAL",
                                        OldOrderStatus: $('#OrderStatus').val() ? $('#OrderStatus').val() : "",
                                        QuoteId: $('#QuoteId').val(),
                                        ShippingId: $("input[name='ShippingOptions']:checked").val(),
                                        AdditionalNotes: $("#AdditionalInstruction").val(),
                                        PaymentSettingId: ($("input[name='PaymentOptions']").length > 0) ? $("input[name='PaymentOptions']:checked").val() : null,
                                        CardType: cardType,
                                        CreditCardExpMonth: cardExpirationMonth,
                                        CreditCardExpYear: cardExpirationYear,
                                        IsPendingPayment: false,
                                        OrderNumber: orderNumber,
                                        InHandDate: $("#InHandDate").val(),
                                        JobName: $("#JobName").val(),
                                        shippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                                        ShippingOptionCode: $("input[name='ShippingOptions']:checked").attr("data-shippingCode")
                                    };

                                    Checkout.prototype.ShowLoader();
                                    if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "REJECTED") {
                                        return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                                    }

                                    if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "DRAFT") {
                                        return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                                    }

                                    if (permissionCode.toUpperCase() == "ARA" || permissionCode.toUpperCase() == "SRA" || permissionCode.toUpperCase() == "DNRA") {
                                        submitQuoteViewModel.OldOrderStatus = null;
                                        return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                                    }
                                    if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "PENDING PAYMENT") {
                                        submitQuoteViewModel.QuoteId = $('#OmsQuoteId').val();
                                        return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                                    }
                                    if (permissionCode.toUpperCase() == "ARAPAYMENT") {
                                        submitQuoteViewModel.OldOrderStatus = null;
                                        submitQuoteViewModel.OmsOrderState = "PENDING PAYMENT"
                                        return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                                    }
                                }
                                else {
                                    var errorMessage = response.GatewayResponse.ResponseText;
                                    if (errorMessage == undefined) {
                                        errorMessage = response.GatewayResponse.GatewayResponseData;
                                    }

                                    if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacementCardDataMissing"));
                                    } else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
                                        $("#div-CreditCard").show();
                                    } else if (errorMessage != null && errorMessage != undefined && errorMessage.indexOf('customer') > 0) {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                    } else {
                                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacement"));
                                    }
                                    Checkout.prototype.HideLoader();
                                }
                            }
                        });
                    }
                });
            }

        }
    }

    IsOrderTotalGreaterThanZero(total): any {
        if (total != "" && total != null && total != 'undefined') {
            total = total.replace(',', '');
        }

        if (total > 0.00) {
            return true;
        } else {
            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("SelectCODForZeroOrderTotal"));
        }
    }

    Mod10(ccNum): boolean {
        var valid = "0123456789";  // Valid digits in a credit card number
        var len = ccNum.length;  // The length of the submitted cc number
        var iCCN = parseInt(ccNum);  // integer of ccNum
        var sCCN = ccNum.toString();  // string of ccNum

        sCCN = sCCN.replace(/^\s+|\s+$/g, '');  // strip spaces

        var iTotal = 0;  // integer total set at zero
        var bNum = true;  // by default assume it is a number
        var bResult = false;  // by default assume it is NOT a valid cc
        var temp;  // temp variable for parsing string
        var calc;  // used for calculation of each digit

        // Determine if the ccNum is in fact all numbers
        for (var j = 0; j < len; j++) {
            temp = "" + sCCN.substring(j, j + 1);
            if (valid.indexOf(temp) == -1) {
                bNum = false;
            }
        }

        // if it is NOT a number, you can either alert to the fact, or just pass a failure
        if (!bNum) {
            bResult = false;
        }

        // Determine if it is the proper length
        if ((len == 0) && (bResult)) {  // nothing, field is blank AND passed above # check
            bResult = false;
        } else {  // ccNum is a number and the proper length - let's see if it is a valid card number
            if (len >= 15) {  // 15 or 16 for Amex or V/MC
                for (var i = len; i > 0; i--) {  // LOOP throught the digits of the card
                    calc = Math.floor(iCCN) % 10;  // right most digit
                    calc = Math.floor(parseInt(calc));  // assure it is an integer
                    iTotal += calc;  // running total of the card number as we loop - Do Nothing to first digit
                    i--;  // decrement the count - move to the next digit in the card
                    iCCN = iCCN / 10;                               // subtracts right most digit from ccNum
                    calc = Math.floor(iCCN) % 10;    // NEXT right most digit
                    calc = calc * 2;                                       // multiply the digit by two
                    // Instead of some screwy method of converting 16 to a string and then parsing 1 and 6 and then adding them to make 7,
                    // I use a simple switch statement to change the value of calc2 to 7 if 16 is the multiple.
                    switch (calc) {
                        case 10: calc = 1; break;       //5*2=10 & 1+0 = 1
                        case 12: calc = 3; break;       //6*2=12 & 1+2 = 3
                        case 14: calc = 5; break;       //7*2=14 & 1+4 = 5
                        case 16: calc = 7; break;       //8*2=16 & 1+6 = 7
                        case 18: calc = 9; break;       //9*2=18 & 1+8 = 9
                        default: calc = calc;           //4*2= 8 &   8 = 8  -same for all lower numbers
                    }
                    iCCN = iCCN / 10;  // subtracts right most digit from ccNum
                    iTotal += calc;  // running total of the card number as we loop
                }  // END OF LOOP
                if ((iTotal % 10) == 0) {  // check to see if the sum Mod 10 is zero
                    bResult = true;  // This IS (or could be) a valid credit card number.
                } else {
                    bResult = false;  // This could NOT be a valid credit card number
                }
            }
        }
        return bResult; // Return the results
    }
    DetectCardTypeForCardConnect(number): any {
        var firstDigit = number.toString().substring(1, 2)
        if (firstDigit == 5)
            return 'MASTERCARD'
        else if (firstDigit == 3)
            return 'AMEX'
        else if (firstDigit == 6)
            return 'DISCOVER'
        else if (firstDigit == 4)
            return 'VISA'
        else
            return 'undefined'
    }

    DetectCardType(number): any {
        var re = {
            electron: /^(4026|417500|4405|4508|4844|4913|4917)\d+$/,
            maestro: /^(5018|5020|5038|5612|5893|6304|6759|6761|6762|6763|0604|6390)\d+$/,
            dankort: /^(5019)\d+$/,
            interpayment: /^(636)\d+$/,
            unionpay: /^(62|88)\d+$/,
            visa: /^4[0-9]{12}(?:[0-9]{3})?$/,
            mastercard: /^5[1-5]\d{14}$|^2(?:2(?:2[1-9]|[3-9]\d)|[3-6]\d\d|7(?:[01]\d|20))\d{12}$/,
            amex: /^3[47][0-9]{13}$/,
            diners: /^3(?:0[0-5]|[68][0-9])[0-9]{11}$/,
            discover: /^6(?:011|5[0-9]{2})[0-9]{12}$/,
            jcb: /^(?:2131|1800|35\d{3})\d{11}$/
        };
        if (re.electron.test(number)) {
            return 'ELECTRON';
        } else if (re.maestro.test(number)) {
            return 'MAESTRO';
        } else if (re.dankort.test(number)) {
            return 'DANKORT';
        } else if (re.interpayment.test(number)) {
            return 'INTERPAYMENT';
        } else if (re.unionpay.test(number)) {
            return 'UNIONPAY';
        } else if (re.visa.test(number)) {
            return 'VISA';
        } else if (re.mastercard.test(number)) {
            return 'MASTERCARD';
        } else if (re.amex.test(number)) {
            return 'AMEX';
        } else if (re.diners.test(number)) {
            return 'DINERS';
        } else if (re.discover.test(number)) {
            return 'DISCOVER';
        } else if (re.jcb.test(number)) {
            return 'JCB';
        } else {
            return undefined;
        }
    }


    PayPalPaymentProcess(): any {
        var Total = $("#Total").val();
        var url = [];
        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            Endpoint.prototype.GetPaymentDetails($('#PaymentSettingId').val(), false, function (response) {
                Checkout.prototype.BindOrderSummaryForOrder(response);
                Checkout.prototype.SetPaymentDetails(response.Response);
                if (!response.HasError) {
                    url = Checkout.prototype.PayPalPayment();
                }
                else {
                    Checkout.prototype.HidePaymentLoader();
                }
            });
        }
        return false;
    }

    ValidateCreditCardDetails(): any {

        var isValid = true;
        var cardType = $('input[name="PaymentProviders"]:checked').val();
        if (!Checkout.prototype.Mod10($('input[data-payment="number"]').val().split(" ").join(""))) {
            isValid = false;
            $('#errornumber').show();
            Checkout.prototype.PaymentError("number");
        }
        else {
            $('#errornumber').hide();
            Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="number"]');
        }

        if (!Checkout.prototype.ValidateCreditCardExpirationDetails(event)) {
            isValid = false;
        }
        if ($('input[data-payment="cvc"]').val() == '') {
            $('#errorcvc').show();
        }
        else {
            $('#errorcvc').hide();
            Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');          
        }

        if ($('input[data-payment="cvc"]').val().length < 3) {
            isValid = false;
            $('#errorcardnumber').show();
            Checkout.prototype.PaymentError("cvc");
        } else {
            if (cardType == Constant.AmericanExpressCardCode && $('input[data-payment="cvc"]').val().length == 4) {
                Checkout.prototype.ShowHideErrorCVV(false);
                Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
            } else if (cardType != Constant.AmericanExpressCardCode && $('input[data-payment="cvc"]').val().length == 3) {
                Checkout.prototype.ShowHideErrorCVV(false)
                Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
            }
            else {
                isValid = false;
                Checkout.prototype.ShowHideErrorCVV(true);
                Checkout.prototype.PaymentError("cvc");
            }
        }

        if ($('input[data-payment="cardholderName"]').val().trim() == '' || $('input[data-payment="cardholderName"]').val().trim().length > 100) {
            isValid = false;
            $('#errorcardholderName').show();
            Checkout.prototype.PaymentError("cardholderName");
        }
        else {
            $('#errorcardholderName').hide();
            Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cardholderName"]');
        }
        return isValid;
    }

    ValidateCyberSourceCardNameHolder(): any {
        var isValidCard = true;
        Checkout.prototype.RemovePaymentErrorCyberSource("#cyscardholderName");
        $('#errorcybersourcecardholdername').hide();

        //Card Holder Name Validation
        if ($('#cyscardholderName').val().trim() == '' || $('#cyscardholderName').val().trim().length > 100) {
            isValidCard = false;
            $('#errorcybersourcecardholdername').show();
            Checkout.prototype.PaymentErrorCyberSource("#cyscardholderName");
        }
        else {
            $('#errorcybersourcecardholdername').hide();
            Checkout.prototype.RemovePaymentErrorCyberSource("#cyscardholderName");
        }

        return isValidCard;
    }

    ValidateCyberSourceCard(event): any {
        var isValidCardName = true;
        var isValidCardMonthAndYear = true;
        var isValidCard = true;
        isValidCardName = Checkout.prototype.ValidateCyberSourceCardNameHolder();
        isValidCardMonthAndYear = Checkout.prototype.ValidateCreditCardExpirationDetailsCyberSource(event);

        if (!isValidCardName && !isValidCardMonthAndYear)
            isValidCard = false;

        return isValidCard;
    }

    ValidateCreditCardExpirationDetailsCyberSource(event): any {
        var isValidCard = true;
        var isValidMonth = true;
        var isValidYear = true;

        Checkout.prototype.RemovePaymentErrorCyberSource('#expMonth');
        Checkout.prototype.RemovePaymentErrorCyberSource('#expYear');
        $('#errorexpmonth').hide();
        $('#errorexpyear').hide();
        $('#errorexpirymonthandyear').hide();
        var currentMonth = (new Date).getMonth() + 1;
        var currentYear = (new Date).getFullYear();

        //Year Validation
        if (Number($('input[data-payment="expiry-year"]').val()) == currentYear && Number($('input[data-payment="expiry-month"]').val()) < currentMonth) {
            isValidYear = false;
            isValidMonth = false;
        }
        if (!/^[0-9]+$/.test($('input[data-payment="expiry-year"]').val())) {
            isValidYear = false;
        }
        if (!/^[0-9]+$/.test($('input[data-payment="expiry-month"]').val())) {
            isValidMonth = false;
        }
        if (Number($('input[data-payment="expiry-month"]').val()) > 12 || Number($('input[data-payment="expiry-month"]').val()) < 1) {
            isValidMonth = false;
        }
        if (Number($('input[data-payment="expiry-year"]').val()) < currentYear) {
            isValidYear = false;
        }

        // Month/Year Validation
        if (!isValidMonth && !isValidYear && event?.target?.id?.toLowerCase() == "btncompletecheckout") {
            isValidCard = false;
            $('#errorexpirymonthandyear').show();
            Checkout.prototype.PaymentErrorCyberSource("#expMonth");
            Checkout.prototype.PaymentErrorCyberSource("#expYear");
        }
        else if (!isValidMonth && ((event?.target?.id?.toLowerCase() == "expmonth") || event?.target?.id?.toLowerCase() == "btncompletecheckout")) {
            isValidCard = false;
            $('#errorexpmonth').show();
            Checkout.prototype.PaymentErrorCyberSource("#expMonth");
        }
        else if (!isValidYear && ((event?.target?.id?.toLowerCase() == "expyear") || event?.target?.id?.toLowerCase() == "btncompletecheckout")) {
            isValidCard = false;
            $('#errorexpyear').show();
            Checkout.prototype.PaymentErrorCyberSource("#expYear");
        }
        if (isValidCard == true)
            $("#btnCompleteCheckout").attr("disabled", false);
        return isValidCard;
    }

    ValidateCreditCardExpirationDetails(event): any {
        var isValidCard = true;
        var isValidMonth = true;
        var isValidYear = true;

        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="exp-month"]');
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="exp-year"]');
        $('#errormonth').hide();
        $('#erroryear').hide();
        var currentMonth = (new Date).getMonth() + 1;
        var currentYear = (new Date).getFullYear();
        if ($('input[data-payment="exp-year"]').val() == currentYear && $('input[data-payment="exp-month"]').val() < currentMonth) {
            isValidYear = false;
            isValidMonth = false;
        }
        if (!/^[0-9]+$/.test($('input[data-payment="exp-year"]').val())) {
            isValidYear = false;
        }
        if (!/^[0-9]+$/.test($('input[data-payment="exp-month"]').val())) {
            isValidMonth = false;
        }
        if ($('input[data-payment="exp-month"]').val() > 12 || $('input[data-payment="exp-month"]').val() < 1) {
            isValidMonth = false;
        }
        if ($('input[data-payment="exp-year"]').val() < currentYear) {
            isValidYear = false;
        }
        if (!isValidMonth && !isValidYear && event?.target?.id?.toLowerCase() == "btncompletecheckout") {
            isValidCard = false;
            $('#errormonthandyear').show();
            Checkout.prototype.PaymentError("exp-month");
            Checkout.prototype.PaymentError("exp-year");
        }
        else if (!isValidMonth && ((event?.target?.id?.toLowerCase() == "creditcardexpmonth") || event?.target?.id?.toLowerCase() == "btncompletecheckout")) {
            isValidCard = false;
            $('#errormonth').show();
            Checkout.prototype.PaymentError("exp-month");
        }
        else if (!isValidYear && ((event?.target?.id?.toLowerCase() == "creditcardexpyear") || event?.target?.id?.toLowerCase() == "btncompletecheckout")) {
            isValidCard = false;
            $('#erroryear').show();
            Checkout.prototype.PaymentError("exp-year");
        }
        if (isValidCard == true)
            $("#btnCompleteCheckout").attr("disabled", false);
        return isValidCard;
    }

    ShowHideErrorCVV(isShow: boolean): void {
        isShow ? $('#errorcvc').show() : $('#errorcvc').hide();
        isShow ? $('#errorcardnumber').show() : $('#errorcardnumber').hide();
    }
    RemoveCreditCardValidationCSS(control): any {
        $(control).css('border', '1px solid #c3c3c3');
        $(control).css('background', '');
    }

    PaymentError(control): any {
        $('input[data-payment=' + control + ']').css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
    }
    PaymentErrorCyberSource(control): any {
        $(control).css('border', '1px solid red');
        $(control).css('background', '#FFCECE');
    }

    RemovePaymentErrorCyberSource(control): any {
        $(control).css('border', '1px solid #c3c3c3');
        $(control).css('background', '');
    }

    RemoveCreditCardValidationOnPaymentMethodChange(): any {
        $('#errornumber').hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="number"]');
        $('#errormonth').hide();
        $('#erroryear').hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="exp-year"]');
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="exp-month"]');
        $('#errorcardnumber').hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
        $('#errorcvc').hide();
    }

    SubmitForApproval(): any {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        if (Total != "" && Total != null && Total != 'undefined') {
            Total = Total.replace(',', '');
        }
        if ($("#EnableUserOrderAnnualLimit").val() && $("#EnableUserOrderAnnualLimit").val().toLowerCase() == "true" && parseInt($("#AnnualOrderLimit").val()) > 0 && (parseInt($("#AnnualBalanceOrderAmount").val()) - parseInt(Total) <= 0)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AnnualOrderLimitFailed") + $("#AnnualOrderLimitWithCurrency").val(), "error", isFadeOut, fadeOutTime);
            Checkout.prototype.HideLoader();
            return false;
        }

        if ($("#EnablePerOrderlimit").val() && $("#EnablePerOrderlimit").val().toLowerCase() == "true" && parseInt($("#PerOrderLimit").val()) > 0 && parseInt($("#PerOrderLimit").val()) <= parseInt(Total)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PerOrderLimitFailed") + $("#PerOrderLimitWithCurrency").val(), "error", isFadeOut, fadeOutTime);
            Checkout.prototype.HideLoader();
            return false;
        }

        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        var isBillingAddresOptional = $("#IsBillingAddressOptional").val();

        $("#errorAccountNumber").hide();
        $("#errorShippingMethod").hide();
        $("#expeditedShippingWarningDiv").removeClass("error");

        Checkout.prototype.HidePaymentLoader();
        if (($("#shipping-content .address-name").text().trim() == "")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredShippingAddress"), "error", false, 0);
            Checkout.prototype.HideLoader();
        }
        else if (($("#billing-content .address-name").text().trim() == "") && isBillingAddresOptional != 'true') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredBillingAddress"), "error", false, 0);
            Checkout.prototype.HideLoader();
        }
        else if ((shippingOptionValue == null || shippingOptionValue == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "") && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorAccountNumber").show();
            $("#errorShippingMethod").show();
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "")) {
            $("#errorAccountNumber").show();
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorShippingMethod").show();
            Checkout.prototype.HideLoader();
        }
        else if ($("#expeditedShippingWarningDiv").is(':visible') && $("#expeditedCheckbox").is(':checked') === false) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ConfirmShippingMethod"), "error", false, 0);
            $("#expeditedShippingWarningDiv").addClass("error");
            Checkout.prototype.HideLoader();
        }
        else if (paymentOptionValue == null || paymentOptionValue == "") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"), "error", false, 0);
            Checkout.prototype.HideLoader();
        }
        else {
            var isOABRequired = $("input[name='PaymentOptions']").length > 0 ? $("input[name='PaymentOptions']:checked").attr("data-isOABRequired").toLowerCase() : "false";
            var isPendingPayment = false;
            if (isOABRequired == "true" && ($('#BillingAccountNumber').val() == undefined || $('#BillingAccountNumber').val() == "")) {
                $("#PermissionCode").val('ARAPAYMENT');
                isPendingPayment = true;
            }
            var portalPaymentGroupId = $("input[name='PaymentOptions']").length > 0 ? $("input[name='PaymentOptions']:checked").attr("data-PaymentGroupId").toLowerCase() : 0;
            var permissionCode = $('#PermissionCode').val() ? $('#PermissionCode').val() : "ARA";
            var paymentOption: string = $("input[name='PaymentOptions']:checked").attr("id");
            paymentOption = Checkout.prototype.GetPaymentType(paymentOption);
            if (paymentOption == "credit_card") {
                Checkout.prototype.SubmitQuotePayment(permissionCode);
            } else {
                var submitQuoteViewModel = {
                    OmsOrderState: ZnodeBase.prototype.getResourceByKeyName("PendingApproval"),
                    OldOrderStatus: $('#OrderStatus').val() ? $('#OrderStatus').val() : "",
                    QuoteId: $('#QuoteId').val(),
                    ShippingId: $("input[name='ShippingOptions']:checked").val(),
                    AdditionalNotes: $("#AdditionalInstruction").val(),
                    PaymentSettingId: ($("input[name='PaymentOptions']").length > 0) ? $("input[name='PaymentOptions']:checked").val() : null,
                    IsPendingPayment: isPendingPayment,
                    PortalPaymentGroupId: $("input[name='PaymentOptions']:checked").attr('data-paymentgroupid'),
                    ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                    PurchaseOrderNumber: $("#txtPurchaseOrderNumber").val(),
                    PODocumentName: $("#po-document-path").val(),
                    InHandDate: $("#InHandDate").val(),
                    shippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                    JobName: $("#JobName").val(),
                    AdditionalInstruction: $("#AdditionalInstruction").val(),
                    ShippingMethod: $("#ShippingMethod").val(),
                    ShippingOptionCode: $("input[name='ShippingOptions']:checked").attr("data-shippingCode"),
                    AccountNumber: $("#AccountNumber").val(),
                };
                if ((submitQuoteViewModel.ShippingId == null || submitQuoteViewModel.ShippingId == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
                    Checkout.prototype.DisplaySelectOptionMessage();
                    Checkout.prototype.HideLoader();
                    return false;
                }
                Checkout.prototype.ShowLoader();
                if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "REJECTED") {
                    return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                }

                if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "DRAFT") {
                    return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                }

                if (permissionCode.toUpperCase() == "ARA" || permissionCode.toUpperCase() == "SRA" || permissionCode.toUpperCase() == "DNRA") {
                    submitQuoteViewModel.OldOrderStatus = null;
                    return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                }
                if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "PENDING PAYMENT") {
                    submitQuoteViewModel.QuoteId = $('#OmsQuoteId').val();
                    return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                }
                if (permissionCode.toUpperCase() == "ARAPAYMENT") {
                    submitQuoteViewModel.OldOrderStatus = null;
                    submitQuoteViewModel.OmsOrderState = "PENDING PAYMENT"
                    return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
                }

            }
        }
    }

    SubmitForDraft(): any {
        var roleName = $('#RoleName').val();
        var submitQuoteViewModel = {
            OmsOrderState: "DRAFT",
            OldOrderStatus: $('#OrderStatus').val() ? $('#OrderStatus').val() : "",
            QuoteId: $('#QuoteId').val(),
            ShippingId: $("input[name='ShippingOptions']:checked").val(),
            AdditionalNotes: $("#AdditionalInstruction").val(),
            PaymentSettingId: ($("input[name='PaymentOptions']").length > 0) ? $("input[name='PaymentOptions']:checked").val() : null
        };
        if ((submitQuoteViewModel.ShippingId == null || submitQuoteViewModel.ShippingId == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            return Checkout.prototype.DisplaySelectOptionMessage();
        }
        Checkout.prototype.ShowLoader();
        if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "REJECTED" || (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "IN REVIEW" && roleName.toLowerCase() == "administrator") || (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "PENDING APPROVAL" && roleName.toLowerCase() == "administrator")) {
            return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
        }
        else if (submitQuoteViewModel.OldOrderStatus.toUpperCase() == "APPROVED" || submitQuoteViewModel.OldOrderStatus.toUpperCase() == "DRAFT") {
            return Checkout.prototype.CreateQuoteRedirectToHistory(submitQuoteViewModel);
        }
        else {
            submitQuoteViewModel.OldOrderStatus = null;
            return Checkout.prototype.CreateQuoteRedirectToReceipt(submitQuoteViewModel);
        }
    }

    DisplaySelectOptionMessage(): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", false, 0);
        return false;
    }

    CreateQuoteRedirectToReceipt(submitQuoteViewModel: any): any {
        if (submitQuoteViewModel.QuoteId != "" && parseInt(submitQuoteViewModel.QuoteId) > 0) {
            return Checkout.prototype.CreateQuoteRedirectToQuoteApprovalHistory(submitQuoteViewModel);
        } else {
            Endpoint.prototype.CreateQuote(submitQuoteViewModel, function (response) {
                if (response.status) {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/Checkout/QuoteReceipt?quoteId=" + response.omsQuoteId + "&IsPendingPayment=" + submitQuoteViewModel.IsPendingPayment;
                } else {
                    Checkout.prototype.HideLoader();
                }
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", isFadeOut, fadeOutTime);
            })
        }
    }

    CreateQuoteRedirectToHistory(submitQuoteViewModel: any): any {
        Endpoint.prototype.CreateQuote(submitQuoteViewModel, function (response) {
            if (response.status) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/User/QuoteHistory";
            } else {
                Checkout.prototype.HideLoader();
            }
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", isFadeOut, fadeOutTime);
        })
    }

    CreateQuoteRedirectToQuoteApprovalHistory(submitQuoteViewModel: any): any {

        if ($("#hdnIsLastApprover").val().toString() != "True") {
            submitQuoteViewModel.OmsOrderState = 'APPROVED';
        }
        Endpoint.prototype.CreateQuote(submitQuoteViewModel, function (response) {
            if (response.status) {
                if ($("#hdnIsLastApprover").val().toString() == "True") {
                    Checkout.prototype.ConvertQuoteToOrderCallbackQuoteList();
                } else {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/User/QuoteApprovalHistory";
                }
            } else {
                Checkout.prototype.HideLoader();
            }
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", isFadeOut, fadeOutTime);
        })
    }

    GetPurchaseOrderHtml(paymentType: string): any {
        var paymentSettingId: number = $("input[name='PaymentOptions']:checked").val();
        Endpoint.prototype.GetPurchanseOrder(paymentType, paymentSettingId, function (response) {
            $("#payment-provider-content").html(response);
            $('form').removeData('validator');
            $('form').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse('form');
        });
    }

    SubmitCheckOutForm(): void {
        var data = {};

        //Get all the selected values required to submit order.
        Checkout.prototype.SetOrderFormData(data);

        //Create form to submit order.
        var form = Checkout.prototype.CreateForm(data);

        // submit form
        form.submit();
        form.remove();
    }

    CheckValidPODocument(): boolean {
        var purchaseOrderNumber: string = $('#txtPurchaseOrderNumber').val()
        if (purchaseOrderNumber != null) {
            if (purchaseOrderNumber.length < 1) {
                $('#txtPurchaseOrderNumber').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                $('#errorpurchaseorder').show();
                $('#errorpurchaseorder').text(ZnodeBase.prototype.getResourceByKeyName('ErrorRequiredPurchaseOrder'));
                $(window).scrollTop(0);
                $(document).scrollTop(0);
                return false;
            }
            else if (purchaseOrderNumber.length > 50) {
                $('#txtPurchaseOrderNumber').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                $('#errorpurchaseorder').show();
                $('#errorpurchaseorder').text(ZnodeBase.prototype.getResourceByKeyName('ErrorPurchaseOrderLength'));
                $(window).scrollTop(0);
                $(document).scrollTop(0);
                return false;
            }
            else if ($("#IsPoDocRequire").val() == "True") {
                if ($("#PODocument").val() == null || $("#PODocument").val() == "") {
                    $("#errorFileTypeAndSize").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFileRequired"));
                    $(window).scrollTop(0);
                    $(document).scrollTop(0);
                    return false;
                }
            }
            return true;
        }
        else
            return true;
    }

    public HidePONumberValidateMessage(): void {
        var purchaseOrderNumber: string = $('#txtPurchaseOrderNumber').val()
        if (purchaseOrderNumber != null) {
            if (purchaseOrderNumber.length > 0) {
                $('#errorpurchaseorder').text("");
                $('#txtPurchaseOrderNumber').removeAttr('style');
                $('#errorpurchaseorder').hide();
                $("#btnCompleteCheckout").attr("disabled", false);
            }
        }
    }

    ShowGiftCardMessage(): void {
        if ($("#giftCard").val() != undefined && $("#giftCard").val().trim().length > 0
            && $("#cartGiftCardMessage").val() != undefined && $("#cartGiftCardMessage").val().trim().length > 0) {
            let msg: string = $("#cartGiftCardMessage").val();
            let isGiftCardApplied: boolean = $("#cartGiftCardApplied").val();
        }
    }

    AppendGiftCardMessage(msg: string, giftCardApplied: any, voucherIndex: number, voucherNumber: string, voucherName: string, expirationDate: string, voucherAmountUsed: string): void {
        let htmlString: any = "<div class='col-xs-12 nopadding'>";

        htmlString = giftCardApplied == true || giftCardApplied == "True" ? htmlString = htmlString + "<p class='text-break padding-top'>" + "<a class='zf-close' onclick='Checkout.prototype.RemoveAppliedVoucher(" + voucherIndex + ");'></a><span class='voucher-amount'>" + voucherAmountUsed + "</span> " + voucherName + " Expires :(" + expirationDate + ")</p>"
            : htmlString = htmlString + "<p class='text-break error-msg'>" + "<a class='zf-close' onclick='Checkout.prototype.RemoveAppliedVoucher(" + voucherIndex + ");' style='cursor:pointer;color:#cc0000;padding-right:3px;'></a>" + msg + " </p>";

        htmlString = htmlString + "</div>";

        if (giftCardApplied == true || giftCardApplied == "True")
            htmlString = htmlString + "<p class='voucher-no'> " + voucherNumber + "</p>";
        if (msg != null && msg != "") {
            if (!($("#giftCard").val() == '' && $("#IsPendingOrderRequest").val().toLowerCase() == 'true')) {
                $("#giftCardMessageContainer").append(htmlString);
            }
        }
    }

    UploadPODocument(file, callback): any {
        CommonHelper.prototype.GetAjaxHeaders(function (response) {
            var data = new FormData();
            data.append("file", file[0]);
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", response.Authorization);
                    xhr.setRequestHeader("Znode-UserId", response.ZnodeAccountId);
                    xhr.setRequestHeader("Token", response.Token);
                    if (response != null && response != undefined && response.DomainName != null && response.DomainName != undefined) {
                        response.DomainName = response.DomainName.replace(/^https?:\/\//, '');
                        response.DomainName = response.DomainName.replace(/^http?:\/\//, '');
                    }
                    xhr.setRequestHeader("Znode-DomainName", response.DomainName);
                },
                url: response.ApiUrl + "/apiupload/uploadpodocument?filePath=~/Data/Media/PODocument",
                contentType: false,
                dataType: "json",
                processData: false,
                data: data,
                success: function (data) {
                    callback(data);
                },
                error: function (error) {
                    var jsonValue = JSON.parse(error.responseText);
                }
            });
        })
    }

    RemovePoDocument(file, callback): any {
        CommonHelper.prototype.GetAjaxHeaders(function (response) {
            var data = new FormData();
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", response.Authorization);
                    xhr.setRequestHeader("Znode-UserId", response.ZnodeAccountId);
                    xhr.setRequestHeader("Znode-DomainName", response.DomainName);
                    xhr.setRequestHeader("Token", response.Token);
                },
                url: response.ApiUrl + "/apiupload/removepodocument?filePath=~/Data/Media/PODocument&file=" + file,
                contentType: false,
                dataType: "json",
                data: data,
                processData: false,
                success: function (data) {
                    callback(data);
                },
                error: function (error) {
                    var jsonValue = JSON.parse(error.responseText);
                }
            });
        })
    }

    public ShowPaymentLoader(): void {
        $("#Single-loader-content-backdrop").show();
    }

    public HidePaymentLoader(): void {
        $("#Single-loader-content-backdrop").hide();
    }

    public CreditCardPayment(controlId): boolean {
        if ($("#hdnGatwayName").val() != undefined && $("#hdnGatwayName").val().length > 0) {
            var gatewayCode = $("#hdnGatwayName").val();
            var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
            $('#Save-ach-card').hide();
            $('#ACHPayment').hide();
            if (gatewayCode == "authorizenet") {
                $('#paymentProviders').hide();
                $('#divAuthorizeNetIFrame').show();
                $('#div-CreditCard').hide();
                $('#btnClosePopup').hide();
                $('#btnConvertQuoteToOrder').hide();
                $("#btnPayInvoice").hide();
            }
            else if (gatewayCode == Constant.BrainTree) {
                $('#paymentProviders').hide();
                $('#divAuthorizeNetIFrame').hide();
                $('#div-CreditCard').hide();
                $('#btnClosePopup').hide();
                $('#btnConvertQuoteToOrder').hide();
                $("#btnPayInvoice").hide();
            }
            else {
                $('#paymentProviders').show();
                $('#divAuthorizeNetIFrame').hide();
                $('#div-CreditCard').show();
                $('#btnClosePopup').show();
                $('#btnConvertQuoteToOrder').show();
                $('#submitandpaybutton').hide();
                $("#btnPayInvoice").show();
            }

            if (gatewayCode.toLowerCase() == "payflow") {
                $('#Save-credit-card').hide();
            }

            var profileId = null;
            if ($("#paymentProfileId").val().length > 0) {
                profileId = $("#paymentProfileId").val();
            }

            var paymentCreditCardModel = {
                gateway: gatewayCode,
                profileId: profileId,
                paymentCode: $('#hdnPaymentCode').val(),
                customerGUID: $("#hdnCustomerGUID").val(),
                publishStateId: $("#hdnPublishStateId").val()
            };
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", $("#hdnPaymentApiResponseHeader").val());
                },
                url: Config.PaymentScriptUrl,
                data: paymentCreditCardModel,
                success: function (response) {
                    if ($("#hdnGatwayName").val() == Constant.CyberSource) {
                        $("#creditCard").hide();
                        Checkout.prototype.CreditCardPaymentCyberSource(controlId);
                        PaymentauthHeader = $("#hdnPaymentApiResponseHeader").val();

                        $("#divCreditCardCyberSource").show();

                        Checkout.prototype.AppendResponseToHTML(response);
                    }
                    else {
                        Checkout.prototype.AppendResponseToHTML(response);
                    }
                    Checkout.prototype.SetCreditCardValidations();
                    var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? false : true;
                    if ($("#hdnAnonymousUser").val() == "true" || gatewayCode.toLowerCase() == "payflow" || !IsAnonymousUser || gatewayCode.toLowerCase() == Constant.BrainTree || gatewayCode.toLowerCase() == "authorizenet") {
                        $("#Save-credit-card").hide();
                    }
                    else {
                        $("#Save-credit-card").show();
                    }

                    if ($("#hdnGatwayName").val() == "cardconnect") {
                        $("#iframebody").attr("src", iframeUrl + "&css=" + encodeURIComponent(Checkout.prototype.GetCardConnectIframeCSS()))
                    }

                    if (gatewayCode == "authorizenet")
                        Checkout.prototype.AuthorizeIframeValidationsAndRendering(gatewayCode, shippingOptionValue, controlId);

                    if (gatewayCode === Constant.BrainTree)
                        Checkout.prototype.BraintreeIframeValidationsAndRendering(gatewayCode, shippingOptionValue, controlId);

                    if (enabledPaymentProviders != '') {
                        var payProvidersHtml = '';
                        var toSplittedPayProviders = enabledPaymentProviders.split(",");
                        for (var iPayProviders = 0; iPayProviders < toSplittedPayProviders.length; iPayProviders++) {
                            payProvidersHtml += "<div class='col-xs-6 col-sm-3 p-0 nopadding save-cart'><label class='input-radio_label'><input class='input-radioButton' id=radioPaymentProviders" + iPayProviders + " type=radio name=PaymentProviders onclick = 'Checkout.prototype.ClearNewlyAddedCreditCardDetailsOnToggle()' value=" + toSplittedPayProviders[iPayProviders] + " /><span class='input-radioButton_appearance'></span><span id=radioPaymentProviders" + iPayProviders + " class='input-radioButton-label'><img src=../../Content/images/" + toSplittedPayProviders[iPayProviders] + ".png class='img-responsive' style='float:right;' /></span></label></div>";
                        }

                        $('#paymentProviders').html("<ul>" + payProvidersHtml + "</ul>");
                        $("#" + $('input[name="PaymentProviders"]')[0].id).prop("checked", true);

                    }
                    if (savedUserCCDetails != '' && gatewayCode != Constant.BrainTree) { //To avoid the Saved CC Details from Znode, we get default vault from Braintree itself so we need to skip this condition.
                        $('#radioCCList').show();
                        $('#radioCCList').html('');
                        var iCCOrder = 0;
                        var creditCardHtml = ""
                        if (gatewayCode != Constant.CyberSource) {

                            $.each(JSON.parse(savedUserCCDetails), function () {
                                creditCardHtml += "<div class='col-sm-12 nopadding styled-input'><input onclick=Checkout.prototype.OnSavedCreditCardClick(" + this['CreditCardLastFourDigit'].split(" ")[3] + "); id=radioSavedCreditCard" + iCCOrder + " type=radio name=CCListdetails value=" + this['PaymentGUID'] + " /><label class='lbl padding-8' for=radioSavedCreditCard" + iCCOrder + ">" + this['CreditCardLastFourDigit'] + "</label></div>";
                                iCCOrder++;
                            });
                        }
                        else {

                            $.each(JSON.parse(savedUserCCDetails), function () {
                                creditCardHtml += "<div class='col-12 p-0 styled-input'><input onclick=Checkout.prototype.OnSavedCreditCardClickCyberSource(" + this['CreditCardLastFourDigit'].split(" ")[3] + "," + "'" + this['PaymentGUID'] + "'" + "); id=radioSavedCreditCard" + iCCOrder + " type=radio name=CCListdetails value=" + this['PaymentGUID'] + " /><label class='lbl padding-8' for=radioSavedCreditCard" + iCCOrder + ">" + this['CreditCardLastFourDigit'] + "</label></div>";
                                iCCOrder++;
                            });
                        }
                        $('#radioCCList').append("<div class='col-sm-12 nopadding'>" + creditCardHtml + "</div>");
                        Checkout.prototype.ToggleCreditCardTab(true);
                        var savedCCRadio = $("#radioSavedCreditCard0");
                        if (savedCCRadio.length > 0) {
                            savedCCRadio.prop('checked', 'true');
                            var cardData = JSON.parse(savedUserCCDetails)[0];

                            if (cardData.CardType == Constant.AmericanExpressCardCode)
                                savedCCRadio.parent().append(Checkout.prototype.GetCVVHtmlForAmericanExpress(cardData.CardType))
                            else
                                savedCCRadio.parent().append(Checkout.prototype.GetCVVHtml(cardData.CardType))

                            savedCCRadio.click();
                        }
                        Checkout.prototype.BindEvent();

                    } else {
                        Checkout.prototype.ToggleCreditCardTab(false);
                        Checkout.prototype.RestrictCopyPasteEvent();
                    }
                    if (gatewayCode != "authorizenet" || gatewayCode != Constant.BrainTree) {
                        Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());
                    }
                    if ($("#hdnGatwayName").val() == Constant.CyberSource) {
                        $("#creditCard").hide();
                        $("#paymentProviders").hide();
                    }

                    $("#divOrderSavePage").hide();
                    return false;
                },
                error: function (error) {
                    if ($("#QuoteId").val() > 0) {
                        Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentApplication"));
                    }
                    else {
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentApplication"));
                        $("#submitandpaybutton").hide();
                    }
                    return false;
                }
            });
        } else {
            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentAsNoGatewayAvailable"));
            $("#submitandpaybutton").hide();
            return false;
        }
    }
    public AuthorizeIframeValidationsAndRendering(gatewayCode, shippingOptionValue, controlId) {
        if ((shippingOptionValue == undefined) && ($("#QuoteId").val() == 0)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            $("input:radio[name='PaymentOptions']").each(function (i) {
                this.checked = false;
            });
            Checkout.prototype.HideLoader();
        }
        else {
            if (($("#QuoteId").val() > 0) || $("#IsFromInvoice").val()) {
                Checkout.prototype.AuthorizeNetPayment(controlId);
            }
            else {
                $("#submitandpaybutton").show();
                $("#btnCompleteCheckout").hide();
            }
        }
    }
    public AuthorizeNetPayment(controlId): void {
        $("#divAuthorizeNetIFrame").html('');
        var urlhost = document.location.origin;
        var iFrameUrl = urlhost + "/Checkout/AuthorizeIframeCommunicator";
        var orderNumber: number = 0;
        if ($("#OrderNumber").val() == null || $("#OrderNumber").val() == undefined) {
            Endpoint.prototype.GenerateOrderNumber(parseInt($("#hdnPortalId").val()), function (response) {
                orderNumber = response.orderNumber;
            });
        } else {
            orderNumber = $("#OrderNumber").val();
        }

        var paymentTokenModel = {
            PaymentSettingId: parseInt($('#PaymentSettingId').val()),
            PaymentCode: $("#hdnPaymentCode").val(),
            Total: $("#Total").val(),
            IFrameUrl: iFrameUrl,
            CustomerProfileId: $('#CustomerProfileId').val(),
            CustomerGUID: $("#hdnCustomerGUID").val(),
            GatewayCode: $("#hdnGatwayName").val(),
            UserId: $("#UserId").val(),
            IsIframeRenderInPopup: $("#QuoteId").val() == 0 ? true : false,
            OrderNumber: orderNumber
        }

        Endpoint.prototype.GetAuthorizeNetToken(paymentTokenModel, function (response) {
            $("#divAuthorizeNetIFrame").show();
            $("#divAuthorizeNetIFrame").html(response.html);
            $("#CustomerProfileId").val(response.customerProfileId);

            $("#AuthorizeNetModal").modal('show');
            $("#btnCompleteCheckout").hide();
            $("#btnPayInvoice").hide();
            Checkout.prototype.HideLoader();
        })
    }

    public ACHPayment(controlId): boolean {
        if ($("#hdnGatwayName").val() != undefined && $("#hdnGatwayName").val().length > 0) {
            var gatewayCode = $("#hdnGatwayName").val();
            $('#Save-credit-card').hide();
            $('#paymentProviders').hide();

            $('#Save-ach-card').show();
            $('#ACHPayment').show();

            var profileId = null;
            if ($("#paymentProfileId").val().length > 0) {
                profileId = $("#paymentProfileId").val();
            }

            var paymentCreditCardModel = {
                gateway: gatewayCode,
                profileId: profileId,
                paymentCode: $('#hdnPaymentCode').val(),
                customerGUID: $("#hdnCustomerGUID").val(),
                publishStateId: $("#hdnPublishStateId").val()
            };
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", $("#hdnPaymentApiResponseHeader").val());
                },
                url: Config.PaymentScriptUrlForACH,
                data: paymentCreditCardModel,
                success: function (response) {
                    Checkout.prototype.AppendResponseToHTML(response);
                    if ($("#hdnAnonymousUser").val() == 0) {
                        $("#Save-ach-card").hide();
                    }
                    if ($("#hdnGatwayName").val() == "cardconnect") {
                        $("#iframebodyACH").attr("src", iframeUrl + "&css=" + encodeURIComponent(Checkout.prototype.GetACHCardConnectIframeCSS()))
                    }
                    if (savedUserACHAccountDetails != '') {
                        $('#radioACHList').show();
                        $('#radioACHList').html('');
                        var iCCOrder = 0;
                        var creditCardHtml = "";
                        $.each(JSON.parse(savedUserACHAccountDetails), function () {
                            creditCardHtml += "<div class='col-sm-12 nopadding styled-input'><input onclick=Checkout.prototype.OnSavedAchAccountClick(" + this['CreditCardLastFourDigit'].split(" ")[3] + "); id=radioSavedCreditCard" + iCCOrder + " type=radio name=CCListdetails value=" + this['PaymentGUID'] + " /><label for=radioSavedCreditCard" + iCCOrder + ">" + this['CreditCardLastFourDigit'] + "</label></div>";
                            iCCOrder++;
                        });
                        $('#radioACHList').append("<div class='col-sm-12 nopadding'>" + creditCardHtml + "</div>");
                        Checkout.prototype.ToggleACHAccountTab(true);
                        var savedACHRadio = $("#radioSavedCreditCard0");
                        if (savedACHRadio.length > 0) {
                            savedACHRadio.prop('checked', 'true');

                            savedACHRadio.click();
                        }
                    }
                    else {
                        Checkout.prototype.ToggleACHAccountTab(false);
                        Checkout.prototype.RestrictCopyPasteEvent();
                    }

                    Checkout.prototype.ShowHidePaymentOption(controlId.toLowerCase());

                    $("#divOrderSavePage").hide();
                    return false;
                },
                error: function (error) {
                    if ($("#QuoteId").val() > 0) {
                        Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentApplication"));
                    }
                    else {
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentApplication"));
                    }
                    return false;
                }
            });
        } else {
            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorPaymentAsNoGatewayAvailable"));
            return false;
        }
    }

    GetCardConnectIframeCSS(): string {
        return "input{width:100%;max-width:280px;height:34px;border:1px solid #9E9E9E;border-radius:2px;border-width:thin; outline: medium none;background-color:#FAFAFA; padding: 0 8px;margin-bottom: 10px;margin-top: 5px;}select{width: 90px;height: 34px;border: 1px solid #9E9E9E;border-radius: 2px;border-width: thin; outline: medium none;background-color: #FAFAFA;padding: 0 8px;margin-bottom: 10px;margin-top: 5px;}#cccvvfield{width:70px}label{font-weight: bold; font-size: 14px;color: #454545;font-family: Roboto-Regular,Arial,Sans-serif;}";
    }

    GetACHCardConnectIframeCSS(): string {
        return "input{width:100%;max-width:280px;height:34px;border:1px solid #9E9E9E;border-radius:2px;border-width:thin; outline: medium none;background-color:#FAFAFA; padding: 0 8px;margin-bottom: 10px;margin-top: 5px;}select{width: 70px;height: 34px;border: 1px solid #9E9E9E;border-radius: 2px;border-width: thin; outline: medium none;background-color: #FAFAFA;padding: 0 8px;margin-bottom: 10px;margin-top: 5px;}#cccvvfield{width:70px}label{font-weight: bold; font-size: 14px;color: #454545;font-family: Roboto-Regular,Arial,Sans-serif;}";
    }

    Getiframe(): void {
        setTimeout(function () {
            $('#iframebody').show()
        }, 300);
    }

    GetiframeACH(): void {
        setTimeout(function () {
            $('#iframebodyACH').show()
        }, 300);
    }

    //Validate CVV Code
    ValidateCVV(): boolean {     
        var cardtype = $("[name='SaveCard-CVV']:visible").attr('data-cardtype');
        var cvvNumber: string = $("[name='SaveCard-CVV']:visible").val();
        if (cardtype == Constant.AmericanExpressCardCode) {
            if (!cvvNumber || cvvNumber.length < 4) {
                Checkout.prototype.ValidationOfCVV();
                return false;
            }
        }
        if (!cvvNumber || (cvvNumber.length <= 2 || cvvNumber.length > 4)) {
            Checkout.prototype.ValidationOfCVV();
            return false;
        }
        $("[name='SaveCard-CVV']:visible").parent().find("span").hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvv"]');
        return true;
    }

    ValidationOfCVV(): void {
        $("[name='SaveCard-CVV']:visible").css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
        $("[name='SaveCard-CVV']:visible").parent().find("span").length <= 0 ?
            $("[name='SaveCard-CVV']:visible").parent().append("<span class='field-validation-error error-cvv'>" + "Please enter a Valid CVV Code." + "</span>") : $("[name='SaveCard-CVV']:visible").parent().find("span").show();
        $(window).scrollTop(0);
        $(document).scrollTop(0);
    }

    BindEvent(): void {
        $("#radioCCList input[type='radio']").on("change", Checkout.prototype.AppendCVVHtml);
        //restrict inputs
        $(document).on("keypress", 'input[data-payment="cvv"]', function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
        Checkout.prototype.RestrictCopyPasteEvent();
    }

    RestrictCopyPasteEvent(): void {
        //restrict cut-copy-paste
        $('input[data-payment="cvv"]').add('#CredidCardCVCNumber').on("cut copy paste", function (e) {
            e.preventDefault();
        });
    }
    //Append CVV Code HTML
    AppendCVVHtml(event): any {
        var currentElement = event.currentTarget;
        var cardtype = "";
        var cardData = JSON.parse(savedUserCCDetails);

        $.each(cardData, function (element, value) {
            if (currentElement.value == value["PaymentGUID"])
                cardtype = value["CardType"];
        });

        $('.error-cvv').hide();
        $('[name=SaveCard-CVV]').hide()
        if ($(currentElement).parent().find('[name=SaveCard-CVV]').length > 0) {
            $(currentElement).parent().find('[name=SaveCard-CVV]').show()
        }
        else {
            if (cardtype == Constant.AmericanExpressCardCode)
                $(currentElement).parent().append(Checkout.prototype.GetCVVHtmlForAmericanExpress(cardtype));
            else
                $(currentElement).parent().append(Checkout.prototype.GetCVVHtml(cardtype));
        }
    }
    //Get CVV Code HTML
    GetCVVHtml(cardtype: string = ""): string {
        return "<input class='form-control' id='CredidCardCVCNumberSaved' name='SaveCard-CVV' data-cardtype=" + cardtype + " data-payment='cvv' type='password' placeholder='Enter CVV' maxlength='3'  style='width:25%;margin-left:2%;'/>";
    }

    //Get CVV Code HTML
    GetCVVHtmlForAmericanExpress(cardtype: string = ""): string {
        return "<input class='form-control' id='CredidCardCVCNumberSaved' name='SaveCard-CVV' data-cardtype=" + cardtype + " data-payment='cvv' type='password'  placeholder='Enter CVV' maxlength='4'  style='width:25%;margin-left:2%;'/>";
    }

    ToggleCreditCardTab(show: boolean): void {
        $('#credit-card-div').show();
        if (show) {
            jQuery('#creditCardTab').show()
            jQuery('.single-page-checkout .credit-card-container .section-heading').show();
            if ($("#creditCardTab li:eq(0)").children().hasClass('active')) {
                jQuery('#savedCreditCard-panel').addClass('active in');
                jQuery('#addNewCreditCard-panel').removeClass('active in');
                $("#creditCardTab li:eq(0)").first().addClass('active');
                $("#creditCardTab li:eq(1)").first().removeClass('active');
            }
            else {
                jQuery('#savedCreditCard-panel').removeClass('active in');
                jQuery('#addNewCreditCard-panel').addClass('active in');
                $("#creditCardTab li:eq(0)").first().removeClass('active');
                $("#creditCardTab li:eq(1)").first().addClass('active');
            }
            $('#divAddNewCCDetails').show();
            if ($("#hdnGatwayName").val() == "cardconnect") {
                $("#divCardconnect").show()
                $("#creditCard").hide()
            }
            else {
                $("#divCardconnect").hide()
                $("#creditCard").show()
            }
        }
        else {
            jQuery('#creditCardTab').hide()
            jQuery('.single-page-checkout .credit-card-container .section-heading').hide();
            jQuery('#savedCreditCard-panel').removeClass('active in');
            jQuery('#addNewCreditCard-panel').addClass('active in');
            $("#creditCardTab li:eq(0)").first().removeClass('active');
            $("#creditCardTab li:eq(1)").first().addClass('active');
            $('#divAddNewCCDetails').hide();
        }
    }

    ToggleACHAccountTab(show: boolean): void {
        $('#ach-account-div').show();
        if (show) {
            jQuery('#ACHAccountTab').show()
            jQuery('.single-page-checkout .credit-card-container .section-heading').show();
            if ($("#ACHAccountTab li:eq(0)").children().hasClass('active')) {
                jQuery('#savedACHAccount-panel').addClass('active in');
                jQuery('#addNewACHAccount-panel').removeClass('active in');
                $("#ACHAccountTab li:eq(0)").first().addClass('active');
                $("#ACHAccountTab li:eq(1)").first().removeClass('active');
            }
            else {
                jQuery('#savedACHAccount-panel').removeClass('active in');
                jQuery('#addNewACHAccount-panel').addClass('active in');
                $("#ACHAccountTab li:eq(0)").first().removeClass('active');
                $("#ACHAccountTab li:eq(1)").first().addClass('active');
            }
            $('#divAddNewCCDetails').show();
            if ($("#hdnGatwayName").val() == "cardconnect") {
                $("#divCardconnectACH").show()
                $("#ACHCard").hide()
            }
            else {
                $("#divCardconnect").hide()
                $("#ACHCard").show()
            }
        }
        else {
            jQuery('#ACHAccountTab').hide()
            jQuery('.single-page-checkout .credit-card-container .section-heading').hide();
            jQuery('#savedACHAccount-panel').removeClass('active in');
            jQuery('#addNewACHAccount-panel').addClass('active in');
            $("#ACHAccountTab li:eq(0)").first().removeClass('active');
            $("#ACHAccountTab li:eq(1)").first().addClass('active');
            $('#divAddNewCCDetails').hide();
            if ($("#hdnGatwayName").val() == "cardconnect") {
                $("#divCardconnectACH").show()
                $("#ACHCard").show()
            }
        }
    }

    //Append javascript Response to Html.
    private AppendResponseToHTML(response): void {
        $("#payment-provider-content script").remove();
        //if script element is already present then add html to response
        if ($("#payment-provider-content").find("script").length > 0) {
            $("#payment-provider-content").find("script").html(response);
            return;
        }
        PaymentauthHeader = $("#hdnPaymentApiResponseHeader").val();
        $("#payment-provider-content").append("<script>" + response + "</script>");
    }

    public OnSavedCreditCardClick(cardNo): any {
        $("#hdnCreditCardNumber").val(cardNo);
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvv"]');
        Checkout.prototype.ClearNewlyAddedCreditCardDetailsOnToggle();
    }
    OnSavedCreditCardClickCyberSource(cardNo, PaymentGUID): any {
        $("#hdnCreditCardNumber").val(cardNo);
        $("#hdnPaymentGUID").val(PaymentGUID);

        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvv"]');
    }
    public CardConnectPayment(token): any {
        $('#CardDataToken').val(token.message);
        $('#CardExpirationDate').val(token.expiry);
        $("#ErrorMessage").val(token.validationError);

    }


    ValidateCardConnectDataToken(): boolean {
        var token = $("#CardDataToken").val()
        if (token != null && token != "" && token != 'undefined')
            return true;
        else {
            if ($("#ErrorMessage").val() != "")
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#ErrorMessage").val(), "error", isFadeOut, fadeOutTime);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorCardDetails"), "error", isFadeOut, fadeOutTime);
            return false;
        }
    }
    ErrorDisplayCyberSourcePayment() {
        if ($("#ErrorMessage").val() == "")
            $("#ErrorMessage").val(ZnodeBase.prototype.getResourceByKeyName("ErrorCardDetails"));
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#ErrorMessage").val(), "error", isFadeOut, fadeOutTime);
    }


    ValidateCardConnectCardHolderName(): boolean {
        var name = $("#cardconnectCardHolderName").val()
        if (name != null && name != "" && name != 'undefined' && name.trim().length <= 100)
            return true;
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#errorcardconnectcardholderName").text(), "error", isFadeOut, fadeOutTime);
            return false;
        }
    }

    public OnSavedAchAccountClick(cardNo): any {
        $("#hdnCreditCardNumber").val(cardNo);
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvv"]');
        Checkout.prototype.ClearNewlyAddedCreditCardDetailsOnToggle();
    }

    public PayPalPayment(): any {
        Checkout.prototype.ShowPaypalPaymentProcessDialog();
        var urlhost = document.location.origin;
        var ShippingOptionId = $("input[name='ShippingOptions']:checked").val();
        var AdditionalInstruction = $("#AdditionalInstruction").val();
        var ShippingAddressId = $("#shipping-content").find("#AddressId").val();
        var BillingAddressId = $("#billing-content").find("#AddressId").val();
        let paymentCode: string = $("#hdnPaymentCode").val();
        var PaymentSettingId = $('#PaymentSettingId').val();
        var InHandDate = $("#InHandDate").val();
        var jobName = $("#JobName").val();
        var orderNumber: number = 0;
        Endpoint.prototype.GenerateOrderNumber(parseInt($("#hdnPortalId").val()), function (response) {
            orderNumber = response.orderNumber;
        });
        var shippingConstraintCode = $("input[name='ShippingConstraintCode']:checked").val();
        var cancelUrl = urlhost + "/checkout/index";
        var returnUrl = urlhost + "/checkout/SubmitPaypalOrder?ShippingAddressId=" + ShippingAddressId + "&BillingAddressId=" + BillingAddressId + "&ShippingOptionId=" + ShippingOptionId + "&AdditionalInstruction=" + AdditionalInstruction + "&PaymentSettingId=" + PaymentSettingId + "&paymentCode=" + paymentCode + "&orderNumber=" + orderNumber + "&inHandDate=" + InHandDate + "&jobName=" + jobName + "&shippingConstraintCode=" + shippingConstraintCode + "";

        var submitPaymentViewModel = {
            PaymentSettingId: PaymentSettingId,
            PaymentCode: paymentCode,
            ShippingAddressId: ShippingAddressId,
            BillingAddressId: BillingAddressId,
            ShippingOptionId: ShippingOptionId,
            AdditionalInstruction: AdditionalInstruction,
            PayPalReturnUrl: returnUrl,
            PayPalCancelUrl: cancelUrl,
            PaymentType: "PayPalExpress",
            Total: $("#Total").val(),
            SubTotal: $('#SubTotal').val(),
            AccountNumber: $("#AccountNumber").val(),
            ShippingMethod: $("#ShippingMethod").val(),
            OrderNumber: orderNumber,
            InHandDate: InHandDate,
            JobName: jobName,
            ShippingConstraintCode: shippingConstraintCode
        };
        var token = $("[name='__RequestVerificationToken']").val();
        var paypalDetails = [];
        $.ajax({
            type: "POST",
            url: "/checkout/submitorder",
            data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
            async: false,
            success: function (response) {
                if (response.error != null && response.error != "" && response.error != 'undefined') {
                    Checkout.prototype.ClearPaymentAndDisplayMessage(response.error);
                    Checkout.prototype.HidePaymentLoader();
                    $("#div-PaypalExpress").hide();
                    return false;
                } else if (response.responseText != null && response.responseText != "" && response.responseText != 'undefined') {
                    $("#div-PaypalExpress").hide();

                    if (response.responseText != undefined && response.responseText.indexOf('Message=') >= 0) {
                        var errorMessage = response.responseText.substr(response.responseText.indexOf('=') + 1);
                        Checkout.prototype.HidePaymentLoader();
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("SelectCOD"));

                    } else if (response.responseText.indexOf("http") != -1) {
                        window.location.href = response.responseText;
                    }
                    else {
                        Checkout.prototype.HidePaymentLoader();
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    }
                }
            },
            error: function () {
                Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                Checkout.prototype.HidePaymentLoader();
                return false;
            }
        });
        return paypalDetails;
    }

    public ClearNewlyAddedCreditCardDetailsOnToggle(): any {
        $("#CredidCardNumber").val('');
        $("#CreditCardExpMonth").val('');
        $("#CreditCardExpYear").val('');
        $("#CredidCardCVCNumber").val('');
        $("#CardHolderName").val('');
    }

    public ToggleFreeShipping(): void {
        let freeshipping: string = $("#cartFreeShipping").val();
        if (freeshipping != null) {
            if (freeshipping.toLowerCase() == "true") {
                $("#message-freeshipping").show();
            }
            else {
                $("#message-freeshipping").hide();
            }
        }
    }

    public ShowPaymentProcessDialog(): void {
        $("#PaymentModal").modal({
            "backdrop": "static",
            "keyboard": true,
            "show": true
        });
    }

    public ShowPaypalPaymentProcessDialog(): void {
        $("#PaypalPaymentModal").modal({
            "backdrop": "static",
            "keyboard": true,
            "show": true
        });
    }
    public ShowAmazonPaymentProcessDialog(): void {
        $("#AmazonPaymentModal").modal({
            "backdrop": "static",
            "keyboard": true,
            "show": true
        });
    }

    public HideAmazonPaymentProcessDialog(): void {
        $(".modal-backdrop").remove();
        $("#AmazonPaymentModal").modal('hide');
        $("body").removeClass("modal-open");
    }

    public HidePaymentProcessDialog(): void {
        $(".modal-backdrop").remove();
        $("#PaymentModal").modal('hide');
        $("body").removeClass("modal-open");
    }

    public ShowErrorPaymentDialog(message): void {
        $("#ErrorPaymentModal").modal({
            "backdrop": "static",
            "keyboard": true,
            "show": true
        }).find('p').html(message);
    }

    public HideChangeAddressLink(): void {
        var accountId: number = $("#accountId").val();
        var roleName: string = $("#RoleName").val();
        var addressCount: number = $("#AddressCount").val();
        if ((roleName.toLowerCase() == "manager" || roleName.toLowerCase() == "user") && addressCount == 1 && accountId > 0) { $(".address-change").hide(); $(".create-new-address").hide(); }

    }

    //Disable fields, when Manager/ User changing address in checkout page.
    public DisableFields(): void {
        var roleName: string = $("#RoleName").val();

        if (roleName != null && roleName != undefined && roleName != "") {

            if (roleName.toLowerCase() == "manager" || roleName.toLowerCase() == "user") {
                $(".edit-address-form :input:not(:button):not(:checkbox)")
                    .attr("readonly", true);

                $('.address_country').attr("disabled", true);
                $('.address_state').attr("disabled", true);
                $("#asdefault_billing").attr('disabled', 'disabled');
                $("#asdefault_shipping").attr('disabled', 'disabled');

                if ($('#AddressId').val() <= 0)
                    $("#btnSaveAddress")
                        .attr("disabled", true);
            }
        }
    }

    public ChangeCartReviewSequence(): void {
        if ($("#allPaymentOptionsDiv").length == 0) {
            $('.shopping-cart .title span').text('4');
        }
    }

    public ScrollTop(): void {
        $(window).scrollTop(0);
        $(document).scrollTop(0);
    }

    public ModifyQuertyString(): void {
        var query = window.location.search.substring(1);
    }

    public PutDataIntoDatalayer(Data: any): void {
        dataLayer.push(Data);
    }

    //AmazonPay Methods.

    //Call on change  on shipping and address.
    public CalculateAmazonShipping(ShippingclassName: string, amazonOrderReferenceId: string): any {
        var form = $("#form0")
        var shippingOptionId = $("input[name='ShippingOptions']:checked").val();
        var shippingAddressId = $("#shipping-content").find("#AddressId").val();
        var shippingCode = $("input[name='ShippingOptions']:checked").attr("data-shippingCode");
        let paymentCode = $("#hdnPaymentCode").val();
        $("#hndShippingclassName").val(ShippingclassName);
        var paymentSettingId = $("#hdnPaymentSettingId").val();
        var total = 0;
        $("#messageBoxContainerId").hide();
        if (ShippingclassName.toLowerCase() == (Constant.ZnodeCustomerShipping).toLowerCase()) {
            $("#customerShippingDiv").show();
        }
        else {
            $("#customerShippingDiv").hide();
        }
        if ((shippingOptionId == null || shippingOptionId != undefined || shippingOptionId != "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            //shippingOptionId = 0;   //commented because shipping charges not applied on shopping cart.
            if (form.attr('action') != undefined && form.attr('action').match("shippingOptionId")) {
                var url = form.attr('action').split('?')[0];
                form.attr('action', "")
                form.attr('action', url);
            }

            form.attr('action', form.attr('action') + "?shippingOptionId=" + shippingOptionId + "&shippingAddressId=" + shippingAddressId + "&shippingCode=" + shippingCode + "&amazonOrderReferenceId=" + $("#hdnOrderReferenceId").val() + "&paymentSettingId=" + paymentSettingId + "&total=" + total + "");
            form.submit();
        }
    }

    //Load shipping option for amazon pay checkout page.
    public AmazonShippingOptions(OrderReferenceId: string, paymentSettingId: number, total: string, accessToken: string, accountNumber: string, shippingMethod: string): void {
        $("#loaderId").html(" <div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
        Endpoint.prototype.AmazonShippingOptions(OrderReferenceId, paymentSettingId, total, accessToken, localStorage.getItem("AccountNumber"), localStorage.getItem("ShippingMethod"), function (response) {
            $(".shipping-method").html(response);
            $("#loaderId").html("");

        });
    }

    public AmazonPayOnReadyShippingCalculate(selectedShippingClassName: string): any {
        if ($('input[name=ShippingOptions]:checked', '#form0').val() != undefined && selectedShippingClassName != null) {
            Checkout.prototype.CalculateAmazonShipping(selectedShippingClassName, '');
        }
    }

    //Process amazon payment.
    public AmazonPayProcess(total, paymentSettingId, paymentCode): any {
        var url = [];
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        if ((shippingOptionValue == null || shippingOptionValue == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        var currentStatus = true;
        if (!Checkout.prototype.IsCheckoutDataValidForAmazonPay()) {
            Checkout.prototype.isPayMentInProcess = false;
            ZnodeBase.prototype.HideLoader();
            currentStatus = false;
        }

        if (Checkout.prototype.IsOrderTotalGreaterThanZero(total) && currentStatus) {

            Endpoint.prototype.GetPaymentDetails(paymentSettingId, false, function (response) {
                Checkout.prototype.BindOrderSummaryForOrder(response);
                Checkout.prototype.SetPaymentDetails(response);
                if (!response.HasError) {
                    $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                    Checkout.prototype.ShowAmazonPaymentProcessDialog();
                    url = Checkout.prototype.AmazonPayPayment(paymentSettingId, paymentCode);
                }
            });
        }
        if (url != null)
            return url;
        Checkout.prototype.HidePaymentProcessDialog();

        return false;
    }

    //Call :Amazon Pay
    public AmazonPayPayment(paymentSettingId, paymentCode): any {
        var orderNumber: number = 0;
        Endpoint.prototype.GenerateOrderNumber(parseInt($("#hdnPortalId").val()), function (response) {
            orderNumber = response.orderNumber;
        });

        var urlhost = document.location.origin;
        var ShippingOptionId = $("input[name='ShippingOptions']:checked").val();
        var AdditionalInstruction = $("#AdditionalInstruction").val() == undefined ? "" : $("#AdditionalInstruction").val();
        var ShippingAddressId = $("#shipping-content").find("#AddressId").val();
        var BillingAddressId = $("#billing-content").find("#AddressId").val();
        var PaymentType = "AmazonPay";
        var PaymentSettingId = paymentSettingId;
        var amazonOrderReferenceId = $("#hdnOrderReferenceId").val();
        var cancelUrl = urlhost + "/checkout/index";
        var InHandDate = $("#InHandDate").val();
        var jobName = $("#JobName").val();
        var shippingConstraintCode = $("input[name='ShippingConstraintCode']:checked").val();
        var returnUrl = urlhost + "/checkout/SubmitAmazonOrder?amazonOrderReferenceId=" + amazonOrderReferenceId + "&PaymentType=AmazonPay&ShippingOptionId=" + ShippingOptionId + "&PaymentSettingId=" + PaymentSettingId + "&paymentCode=" + paymentCode + "&AdditionalInstruction=" + AdditionalInstruction + "&orderNumber=" + orderNumber + "&inHandDate=" + InHandDate + "&jobName=" + jobName + "&shippingConstraintCode=" + shippingConstraintCode + "";

        var submitPaymentViewModel = {
            PaymentSettingId: PaymentSettingId,
            PaymentCode: paymentCode,
            ShippingAddressId: ShippingAddressId,
            BillingAddressId: BillingAddressId,
            ShippingOptionId: ShippingOptionId,
            AdditionalInstruction: AdditionalInstruction,
            AmazonPayReturnUrl: returnUrl,
            AmazonPayCancelUrl: cancelUrl,
            AmazonOrderReferenceId: amazonOrderReferenceId,
            PaymentType: PaymentType,
            Total: $("#Total").val(),
            SubTotal: $('#SubTotal').val(),
            AccountNumber: $("#AccountNumber").val(),
            ShippingMethod: $("#ShippingMethod").val(),
            InHandDate: InHandDate,
            OrderNumber: orderNumber,
            IsFromAmazonPay: true,
            JobName: jobName,
            ShippingConstraintCode: shippingConstraintCode
        };
        var token = $("[name='__RequestVerificationToken']").val();
        var amazonPayDetails = [];
        $.ajax({
            type: "POST",
            url: "/checkout/submitorder",
            data: { __RequestVerificationToken: token, submitOrderViewModel: submitPaymentViewModel },
            async: false,
            success: function (response) {
                if (response.error != null && response.error != "" && response.error != 'undefined') {
                    Checkout.prototype.ClearPaymentAndDisplayMessage(response.error);
                    $("#div-PaypalExpress").hide();
                    return false;
                }
                else if (response.responseText != null && response.responseText != "" && response.responseText != 'undefined') {
                    $("#div-PaypalExpress").hide();

                    if (response.responseText != undefined && response.responseText.indexOf('Message=') >= 0) {
                        var errorMessage = response.responseText.substr(response.responseText.indexOf('=') + 1);
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("SelectCOD"));

                    } else if (response.responseText == "True") {
                        window.location.href = returnUrl + "&captureId=" + response.responseToken;
                    }
                    else {
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    }
                }
            },
            error: function () {
                Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                Checkout.prototype.HideAmazonPaymentProcessDialog();
                return false;
            }
        });
        return amazonPayDetails;
    }

    //End Amazon Pay Methods. 
    public GetPaymentType(id: string): string {
        var paymentType = $("#" + id).attr("data-payment-type");
        if (paymentType != undefined) {
            return paymentType.toLowerCase();
        } else {
            return id;
        }
    }

    ConvertQuoteToOrder(): void {
        var data = {};
        //Get all the selected values required to submit order.
        Checkout.prototype.SetOrderDataFromQuote(data);
        //Create form to convert quote to order.
        var form = Checkout.prototype.ConvertToOrder(data);
        // submit form
        form.submit();
        form.remove();
    }

    ConvertQuoteToOrderCallbackQuoteList(): void {
        var data = {};
        //Get all the selected values required to submit order.
        Checkout.prototype.SetOrderDataFromQuote(data);
        //Create form to convert quote to order.
        var form = Checkout.prototype.ConvertToOrderCallbackQuoteList(data);
        // submit form
        form.submit();
        form.remove();
    }

    MultilevelApprove(): void {
        var data = {};
        //Get all the selected values required to submit order.
        Checkout.prototype.SetOrderDataFromQuote(data);
        //Create form to convert quote to order.
        var form = Checkout.prototype.MultilevelApproveForm(data);
        // submit form
        form.submit();
        form.remove();
    }

    SetOrderDataFromQuote(data): any {
        data["ShippingId"] = $("#ShippingMethod").val();
        data["PaymentSettingId"] = $("#PaymentSettingId").val();
        data["ShippingAddressId"] = $("#ShippingAddressId").val();
        data["BillingAddressId"] = $("#BillingAddressId").val();
        data["AdditionalInstruction"] = $("#AdditionalInstruction").val();
        data["PurchaseOrderNumber"] = $("#PurchaseOrderNumber").val();
        data["PODocumentName"] = $("#PODocumentName").val();
        data["AccountNumber"] = $("#AccountNumber").val();
        data["ShippingMethod"] = $("#ShippingMethod").val();
        data["UserId"] = $("#UserId").val();
        data["OmsQuoteId"] = $("#OmsQuoteId").val();
        data["Comments"] = $("#Comments").val();
    }

    //Create form to convert quote to order.
    ConvertToOrder(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/User/ConvertToOrder");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        Checkout.prototype.AddFormFields(form, data);
        $("body").append(form);
        return form;
    }

    //Create form to convert quote to order.
    ConvertToOrderCallbackQuoteList(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/User/ConvertToOrderCallbackQuoteList");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        Checkout.prototype.AddFormFields(form, data);
        $("body").append(form);
        return form;
    }

    //Create form to convert quote to order.
    MultilevelApproveForm(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/User/UpdateQuote");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        Checkout.prototype.AddFormFields(form, data);
        $("body").append(form);
        return form;
    }

    SetFlagForApprovalRouting(isApprovalRequired: string, isOABRequired: string, isStoreLevelAppoverOn: string): boolean {
        if (isStoreLevelAppoverOn == "true" || isApprovalRequired == "true" || isOABRequired == "true") {
            if (isOABRequired == "true")
                return true;
            else {
                let orderLimit: number = parseFloat($('#OrderLimit').val());
                let orderTotal: number = parseFloat($("#hdnTotalOrderAmount").val().replace(',', '.'));
                if (orderLimit == 0 || orderTotal >= orderLimit)
                    return true;
                else
                    return false;
            }
        }
        else
            return false;
    }

    GetLoginUserAddress() {
        var quoteId: number = parseInt($("#QuoteId").val());
        Endpoint.prototype.GetLoginUserAddress(0, quoteId, function (response) {
            $("#address-popup-content").html(response);
        });
    }

    GetCartReview() {
        Endpoint.prototype.GetcartReview(0, 0, "", function (response) {
            $("#divShoppingCart").html(response.html);
            $(".headerSubtotal").html(response.cartTotal);
        });
    }

    //Bind click events for billing/shipping checkboxes.
    public BindAddToAddressBookCheckbox(): void {
        $("input[name=DontAddUpdateAddress]").on("click", function () {
            if ($(this).prop("checked") == true) {
                if ($(".billingShippingCheckBox[name=IsDefaultBilling]").length > 0) {
                    $(".billingShippingCheckBox[name=IsDefaultBilling]").prop("checked", false);
                    $(".set-as-default-address-IsDefaultBilling").fadeOut(400);
                }
                if ($(".billingShippingCheckBox[name=IsDefaultShipping]").length > 0) {
                    $(".billingShippingCheckBox[name=IsDefaultShipping]").prop("checked", false);
                    $(".set-as-default-address-IsDefaultShipping").fadeOut(400);
                }
            } else {
                if ($(".billingShippingCheckBox[name=IsDefaultBilling]").length > 0) {

                    if ($(".billingShippingCheckBox[name=IsDefaultBilling]").parent(".input-checkbox_label").hasClass("checkbox-disable")) {
                        $(".billingShippingCheckBox[name=IsDefaultBilling]").prop("checked", true);
                    }

                    $(".set-as-default-address-IsDefaultBilling").fadeIn(400);
                }
                if ($(".billingShippingCheckBox[name=IsDefaultShipping]").length > 0) {

                    if ($(".billingShippingCheckBox[name=IsDefaultShipping]").parent(".input-checkbox_label").hasClass("checkbox-disable")) {
                        $(".billingShippingCheckBox[name=IsDefaultShipping]").prop("checked", true);
                    }

                    $(".set-as-default-address-IsDefaultShipping").fadeIn(400);
                }
            }
        });
    }

    public IsCheckoutDataValidForAmazonPay(): boolean {

        var isValid: boolean = true;
        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        var isBillingAddresOptional = $("#IsBillingAddressOptional").val();
        $("#errorAccountNumber").hide();
        $("#errorShippingMethod").hide();
        $("#expeditedShippingWarningDiv").removeClass("error");

        if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "") && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorAccountNumber").show();
            $("#errorShippingMethod").show();
            isValid = false;
            $('#AccountNumber').focus();
            Checkout.prototype.HideLoader();
            return isValid;
        }
        if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "")) {
            $("#errorAccountNumber").show();
            $('#AccountNumber').focus();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorShippingMethod").show();
            $('#ShippingMethod').focus();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        return isValid;
    }

    OnShippingAddressSelect(item: any): any {
        var addressId: number = item.id;
        Checkout.prototype.ShowLoader();
        let isSuggestedAddress: boolean = false;
        Endpoint.prototype.GetAndSelectAddressDetails(addressId, "shipping",
            function (response) {
                Checkout.prototype.ShippingOptions();
                if (response.model != "" && typeof response.model != "undefined" && response.model != null) {
                    Checkout.prototype.BindDisplayAddressData(response.model, "shipping");
                    if ($(".shipping-address-content .address-details").css('display') == 'none') {
                        $(".shipping-address-content .address-details").fadeIn(400);
                    }
                }
                Checkout.prototype.HideLoader();
            });
    }

    OnBillingAddressSelect(item: any): any {
        var addressId: number = item.id;
        Checkout.prototype.ShowLoader();
        let isSuggestedAddress: boolean = false;
        Endpoint.prototype.GetAndSelectAddressDetails(addressId, "billing",
            function (response) {
                Checkout.prototype.ShippingOptions();
                if (response.model != "" && typeof response.model != "undefined" && response.model != null) {
                    Checkout.prototype.BindDisplayAddressData(response.model, "billing");
                }
                Checkout.prototype.HideLoader();
            });
    }

    OnAddressSelectionCancel(addressId: number, addressType: string, userId: number) {
        $("#dvShippingOptions h3").next().html("<div id='loaderId'></div>");
        ZnodeBase.prototype.ShowLoader();
        var hostUrl: string = window.location.origin;
        setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
        //Save address and show loader for same shipping billing addresses.
        if ($("#is_both_billing_shipping").is(":checked")) {
            $("#Edit-Address-content_shipping").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/throbber.gif' alt= 'Loading' class='dashboard-loader' /></div>");
            if (addressId != 0) {
                if (userId > 0) {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + "&userid=" + userId + "&isAddressFromSession=true");
                }
                else {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType, "&isAddressFromSession=true");
                }
            }
            else {
                $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressType=" + addressType, "&isAddressFromSession=true");
            }
        }
        else if ($("#sameAsShipping").is(":checked")) {
            $("#Edit-Address-content_billing").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/throbber.gif' alt= 'Loading' class='dashboard-loader' /></div>");
            if (addressId != 0) {
                if (userId > 0) {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + "&userid=" + userId + "&isAddressFromSession=true");
                }
                else {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType, "&isAddressFromSession=true");
                }
            }
            else {
                $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressType=" + addressType, "&isAddressFromSession=true");
            }
        }
        //Save address and show loader for different shipping and billing addresses.
        else {
            if (addressType == "shipping".toLowerCase()) {
                $("#shipping-address-content").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/throbber.gif' alt= 'Loading' class='dashboard-loader' /></div>");
                if (addressId != 0) {
                    if (userId > 0) {
                        $("#shipping-content").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + " #shipping-content>*", "&isAddressFromSession=true");
                    }
                    else {
                        $("#shipping-content").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + " #shipping-content>*", "&isAddressFromSession=true");
                    }
                }
                else {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressType=" + addressType, "&isAddressFromSession=true");
                }
            }
            if (addressType == "billing".toLowerCase()) {
                $("#billing-content").html("<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/throbber.gif' alt= 'Loading' class='dashboard-loader' /></div>");
                if (addressId != 0) {
                    if (userId > 0) {
                        $("#BillingAddressContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + "&userid=" + userId + " #BillingAddressContainer>*", "&isAddressFromSession=true");
                    }
                    else {
                        $("#BillingAddressContainer").load(hostUrl + "/checkout/accountaddress?addressId=" + addressId + "&addressType=" + addressType + " #BillingAddressContainer>*", "&isAddressFromSession=true");
                    }
                }
                else {
                    $("#dvBillingShippingContainer").load(hostUrl + "/checkout/accountaddress?addressType=" + addressType, "&isAddressFromSession=true");
                }
            }
        }
        //Checkout.prototype.RefreshAddressOptions(addressType);
    }

    OnShippingAddressSelectionChange(): any {
        var addressId: number = $("#ShippingAddress").val();
        Checkout.prototype.ShowLoader();
        let isSuggestedAddress: boolean = false;
        Endpoint.prototype.GetAndSelectAddressDetails(addressId, "shipping",
            function (response) {                
                Checkout.prototype.ShippingOptions();
                if (response.model != "" && typeof response.model != "undefined" && response.model != null) {
                    Checkout.prototype.BindDisplayAddressData(response.model, "shipping");
                    if ($(".shipping-address-content .address-details").css('display') == 'none') {
                        $(".shipping-address-content .address-details").fadeIn(400);
                    }
                }
                $("#shipping-address-content .address-change a").attr('href', $("#shipping-address-content .address-change a").attr('href').replace('AddressId=' + $("#shipping-address-content .address-change a").attr("data-addressid"), 'AddressId=' + addressId));
                $("#shipping-address-content .address-change a").attr("data-addressid", addressId)
                $('#ShippingAddress option[value=' + addressId + ']').attr('selected', 'selected');
                Checkout.prototype.HideShippingDiv();
            }, false);
        }

    RefreshAddressOptions(type: string, isCalculateCart: boolean = true): any {
        Endpoint.prototype.RefreshAddressOptions(type,
            function (response) {
                if (type === "shipping") {
                    $("#billing-content-selectoptions").html(response);
                    Checkout.prototype.HideShippingDiv();
                    var selectedvalue = $(".billing-address-content .address-block #AddressId").val();
                    $("#BillingAddress option[value=" + selectedvalue + "]").attr("selected", "selected");
                }
                else {
                    $("#shipping-content-selectoptions").html(response);
                    Checkout.prototype.HideShippingDiv();
                    var selectedvalue = $(".shipping-address-content .address-block #AddressId").val();
                    $("#ShippingAddress option[value=" + selectedvalue + "]").attr("selected", "selected");

                }
            }, isCalculateCart);
    }

    OnBillingAddressSelectionChange(): any {
        var addressId: number = $("#BillingAddress").val();
        Checkout.prototype.ShowLoader();
        let isSuggestedAddress: boolean = false;
        Endpoint.prototype.GetAndSelectAddressDetails(addressId, "billing",
            function (response) {
                Checkout.prototype.ShippingOptions();
                if (response.model != "" && typeof response.model != "undefined" && response.model != null) {
                    Checkout.prototype.BindDisplayAddressData(response.model, "billing");
                }
                $("#billing-content .address-change a").attr('href', $("#billing-content .address-change a").attr('href').replace('AddressId=' + $("#billing-content .address-change a").attr("data-addressid"), 'AddressId=' + addressId));
                $("#billing-content .address-change a").attr("data-addressid", addressId)
                $('#BillingAddress option[value=' + addressId + ']').attr('selected', 'selected');
                Checkout.prototype.HideShippingDiv();
            }, false);
    }
    SetReceipentNameAddressData(addressType): void {
        if ($("." + addressType + "-address-content .address-block .address-recipient").length > 0) {
            var receipentNameControl = $("." + addressType + "-address-content .address-block .address-recipient");
            var firstName: string = "";
            var lastName: string = "";
            var receipentName: string = $(receipentNameControl).val().trim();

            if (receipentName.split(" ").length > 1) {
                firstName = receipentName.split(" ")[0];
                lastName = receipentName.substring(receipentName.indexOf(" "), receipentName.length);

            } else if (receipentName.split(" ").length == 1) {
                firstName = receipentName.split(" ")[0];
                lastName = "";
            }

            var addressUsername: string = Checkout.prototype.GetValueOrEmptyString(firstName.trim()) + " " + Checkout.prototype.GetValueOrEmptyString(lastName.trim());
            if (addressUsername.trim() != "") {
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", firstName.trim());
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", lastName.trim());
                $("." + addressType + "-address-content .address-block .address-firstlastname").html(addressUsername);
            }
        }
    }

    SaveRecipientNameAddressData(addressType: string, callback): any {
        if ($("#shipping-content .address-recipient").length > 0) {
            Checkout.prototype.ShowLoader();
            Endpoint.prototype.SetAddressRecipientNameInCart(
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname")
                , $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname")
                , addressType
                , function (response) {
                    if (response.AddressId != "" && typeof response.AddressId != "undefined" && response.AddressId != null) {
                        $("." + addressType + "-address-content .address-block .address-firstlastname").removeClass("text-warning");
                    }
                    Checkout.prototype.HideLoader();
                    callback(response);
                });
        } else {
            callback(null);
        }
    }

    BindDisplayAddressData(addressModel, addressType): void {
        var receipentNameControl = $("." + addressType + "-address-content .address-block .address-recipient");
        $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", addressModel.FirstName);
        $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", addressModel.LastName);
        $("." + addressType + "-address-content .address-block .address-company-name").attr("data-address-companyname", addressModel.CompanyName);
        $("." + addressType + "-address-content .address-block .address-street").attr("data-address-address1", addressModel.Address1);
        $("." + addressType + "-address-content .address-block .address-citystate").attr("data-address-cityname", addressModel.CityName);
        $("." + addressType + "-address-content .address-block .address-citystate").attr("data-address-statecode", addressModel.StateName);
        $("." + addressType + "-address-content .address-block .address-citystate").attr("data-address-countryname", addressModel.CountryName);
        $("." + addressType + "-address-content .address-block .address-citystate").attr("data-address-postalcode", addressModel.PostalCode);

        $("." + addressType + "-address-content .address-block .address-phonenumber").attr("data-address-phonenumber", addressModel.PhoneNumber);
        $("." + addressType + "-address-content .address-block .address-emailaddress").attr("data-address-emailaddress", addressModel.EmailAddress);

        if (parseInt($('#QuoteId').val()) > 0) {
            //Editing quote mode.
            if ((Checkout.prototype.GetValueOrEmptyString(addressModel.FirstName) + " " + Checkout.prototype.GetValueOrEmptyString(addressModel.LastName)).trim() == "") {
                //Address model has empty shopper name.
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", $(".billing-address-content .address-block .address-firstlastname").attr("data-address-fname"));
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", $(".billing-address-content .address-block .address-firstlastname").attr("data-address-lname"));
                $("." + addressType + "-address-content .address-block .address-firstlastname").html(
                    Checkout.prototype.GetValueOrEmptyString($(".billing-address-content .address-block .address-firstlastname").attr("data-address-fname")) + " " +
                    Checkout.prototype.GetValueOrEmptyString($(".billing-address-content .address-block .address-firstlastname").attr("data-address-lname")));
            } else {
                //Address model has proper shopper name.
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", addressModel.FirstName);
                $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", addressModel.LastName);
                $("." + addressType + "-address-content .address-block .address-firstlastname").html(
                    Checkout.prototype.GetValueOrEmptyString(addressModel.FirstName) + " " +
                    Checkout.prototype.GetValueOrEmptyString(addressModel.LastName));
            }
        } else {
            $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", addressModel.FirstName);
            $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", addressModel.LastName);
            $("." + addressType + "-address-content .address-block .address-firstlastname").html(
                Checkout.prototype.GetValueOrEmptyString(addressModel.FirstName) + " " +
                Checkout.prototype.GetValueOrEmptyString(addressModel.LastName));
        }

        if ($(receipentNameControl).length > 0) {
            var recipientName: string = (Checkout.prototype.GetValueOrEmptyString(addressModel.FirstName) + " " + Checkout.prototype.GetValueOrEmptyString(addressModel.LastName)).trim();
            if (recipientName != "") {
                $(receipentNameControl).val(recipientName);
            } else {
                if (parseInt($('#QuoteId').val()) > 0) {
                    //Editing quote mode.
                    $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", $(".billing-address-content .address-block .address-firstlastname").attr("data-address-fname"));
                    $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", $(".billing-address-content .address-block .address-firstlastname").attr("data-address-lname"));
                    $("." + addressType + "-address-content .address-block .address-firstlastname").html(
                        Checkout.prototype.GetValueOrEmptyString($(".billing-address-content .address-block .address-firstlastname").attr("data-address-fname")) + " " +
                        Checkout.prototype.GetValueOrEmptyString($(".billing-address-content .address-block .address-firstlastname").attr("data-address-lname")));
                } else {
                    //If name is not available in the address then use logged in user name
                    $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-fname", $("." + addressType + "-address-content .address-shopper-firstname").val());
                    $("." + addressType + "-address-content .address-block .address-firstlastname").attr("data-address-lname", $("." + addressType + "-address-content .address-shopper-lastname").val());
                    $("." + addressType + "-address-content .address-block .address-firstlastname").html(
                        Checkout.prototype.GetValueOrEmptyString($("." + addressType + "-address-content .address-shopper-firstname").val()) + " " +
                        Checkout.prototype.GetValueOrEmptyString($("." + addressType + "-address-content .address-shopper-lastname").val()));
                }
            }
        }

        $("." + addressType + "-address-content .address-block .address-company-name").html(Checkout.prototype.GetValueOrEmptyString(addressModel.CompanyName));

        $("." + addressType + "-address-content .address-block .address-street").html(
            Checkout.prototype.GetValueOrEmptyString(addressModel.Address1) + "<br />" +
            Checkout.prototype.GetValueOrEmptyString(addressModel.Address2));

        $("." + addressType + "-address-content .address-block .address-citystate").html(
            Checkout.prototype.GetValueOrEmptyString(addressModel.CityName) + ", " +
            Checkout.prototype.GetValueOrEmptyString(addressModel.StateName) + ", " +
            Checkout.prototype.GetValueOrEmptyString(addressModel.CountryName) + " " +
            Checkout.prototype.GetValueOrEmptyString((addressModel.PostalCode)));
        $("." + addressType + "-address-content .address-block .address-phonenumber").html("Ph: " + Checkout.prototype.GetValueOrEmptyString(addressModel.PhoneNumber));

        $("." + addressType + "-address-content .address-block #AddressId").val(addressModel.AddressId);
        $("." + addressType + "-address-content .address-block #accountId").val(addressModel.AccountId);

    }

    GetValueOrEmptyString(value: string): string {
        if (value != "" && typeof value != "undefined" && value != null) {
            return value.trim();
        } else {
            return "";
        }
    }

    ChangeSubmitOrderButtonText(): any {
        var isApprovalRequired = "false";
        var isOABRequired = "false";
        var paymentOption: string = "";
        if ($("input[name='PaymentOptions']:checked").length > 0) {
            isApprovalRequired = $("input[name='PaymentOptions']:checked").attr("data-isApprovalRequired").toLowerCase();
            isOABRequired = $("input[name='PaymentOptions']:checked").attr("data-isOABRequired").toLowerCase();
            paymentOption = $("input[name='PaymentOptions']:checked").attr("data-payment-type").toLowerCase();
            if (paymentOption == "Amazon_Pay".toLowerCase()) {
                $(".btnCompleteCheckout").hide();
                return;
            }
        }
        if (parseInt($('#QuoteId').val()) > 0) { } else {
            var isStoreLevelAppoverOn = "false";
            var approvalType = $("#ApprovalType").val();
            if (approvalType != "Payment" && $("#EnableApprovalRouting").val() != undefined)
            {
                isStoreLevelAppoverOn = $("#EnableApprovalRouting").val().toLowerCase();
            }
            var isNotGuest: boolean = (parseInt($('#hdnAnonymousUser').val()) > 0);
            if (Checkout.prototype.SetFlagForApprovalRouting(isApprovalRequired, isOABRequired, isStoreLevelAppoverOn) && isNotGuest) {
                $(".btnCompleteCheckout").html('<i class="zf-checkout"></i> Submit For Approval');
            }
            else {
                $(".btnCompleteCheckout").html('<i class="zf-checkout"></i> Place Order');
            }
        }
    }
    public IsCheckoutDataValid(): boolean {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        if (Total != "" && Total != null && Total != 'undefined') {
            Total = Total.replace(',', '');
        }

        var isValid: boolean = true;
        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        var isBillingAddresOptional = $("#IsBillingAddressOptional").val();
        $("#errorAccountNumber").hide();
        $("#errorShippingMethod").hide();
        $("#expeditedShippingWarningDiv").removeClass("error");

        if ($("#shipping-content .address-recipient").length == 0 && ($("#shipping-content .address-name").text().trim() == "")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredShippingAddress"), "error", false, 0);
            isValid = false;
            Checkout.prototype.HideLoader();
        } else if (($("#shipping-content .address-recipient").length > 0) && ($("#shipping-content .address-recipient").val().trim() == "")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredRecipientName"), "error", false, 0);
            $("#shipping-content .address-recipient").focus();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        if ($("#billing-content .address-citystate").length < 1 && isBillingAddresOptional != 'true') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredBillingAddress"), "error", false, 0);
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        if ($("#billing-content .address-citystate").length > 1 && ($("#billing-content .address-citystate").attr("data-address-postalcode").trim() == "") && isBillingAddresOptional != 'true') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredBillingAddress"), "error", false, 0);
            isValid = false;
            Checkout.prototype.HideLoader();
        } else if ((shippingOptionValue == null || shippingOptionValue == "") && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "") && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorAccountNumber").show();
            $("#errorShippingMethod").show();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "")) {
            $("#errorAccountNumber").show();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && ($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorShippingMethod").show();
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#expeditedShippingWarningDiv").is(':visible') && $("#expeditedCheckbox").is(':checked') === false) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ConfirmShippingMethod"), "error", false, 0);
            $("#expeditedShippingWarningDiv").addClass("error");
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if (paymentOptionValue == null || paymentOptionValue == "") {
            if ($("#hdnTotalOrderAmount").val().replace(',', '.') > 0.00) {
                isValid = false;
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"), "error", false, 0);
                Checkout.prototype.HideLoader();
            }
            else {
                Checkout.prototype.SubmitCheckOutForm();
            }
        }
        else if ($("#EnableUserOrderAnnualLimit").val() && $("#EnableUserOrderAnnualLimit").val().toLowerCase() == "true" && parseInt($("#AnnualOrderLimit").val()) > 0 && (parseInt($("#AnnualBalanceOrderAmount").val()) - parseInt(Total) <= 0)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AnnualOrderLimitFailed") + $("#AnnualOrderLimitWithCurrency").val(), "error", false, 0);
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        else if ($("#EnablePerOrderlimit").val() && $("#EnablePerOrderlimit").val().toLowerCase() == "true" && parseInt($("#PerOrderLimit").val()) > 0 && parseInt($("#PerOrderLimit").val()) <= parseInt(Total)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PerOrderLimitFailed") + $("#PerOrderLimitWithCurrency").val(), "error", false, 0);
            isValid = false;
            Checkout.prototype.HideLoader();
        }
        return isValid;
    }

    // Data Layer Start - OrdersViewModel This data modal is required for prepareDataLayer function.
    prepareDataLayer(data): any {
        var cartModel = {};
        var ecommerce = {};
        var purchase = {};

        var actionField = {};
        actionField["id"] = data.OmsOrderId;
        actionField["affiliation"] = "Online Store";
        actionField["revenue"] = data.Total;
        actionField["tax"] = data.TaxCost;
        actionField["shipping"] = data.ShippingCost;
        actionField["coupon"] = Checkout.prototype.getValueForDataLayer(data.CouponCode);
        purchase["actionField"] = actionField;

        var shoppingCart = new Array();
        $.each(data.OrderLineItems, function (v, e) {
            var shoppingCartModel = {};
            shoppingCartModel["id"] = e.OmsOrderLineItemsId;
            shoppingCartModel["sku"] = e.Sku;
            shoppingCartModel["name"] = e.ProductName;
            shoppingCartModel["price"] = e.Price;
            shoppingCartModel["quantity"] = e.Quantity;
            shoppingCartModel["salePrice"] = e.Price;
            shoppingCartModel["total"] = e.Price;
            shoppingCartModel["description"] = e.Description;
            shoppingCart.push(shoppingCartModel);
        });

        purchase["products"] = shoppingCart;
        ecommerce["purchase"] = purchase;
        cartModel["ecommerce"] = ecommerce;
        return cartModel;
    }
    // Data Layer End

    getValueForDataLayer(item): any {
        var itemValue = item != undefined ? item : "";
        return itemValue;
    }

    EditAddressSuccess(): void {
        if ($("#IsBillingAddressOptional").val() == "true") {
            $("#same-as-billing").hide();
        }
        else {
            $("#same-as-billing").show();
        }
        $('form').removeData('validator');
        $('form').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('form'); $('#IsDefaultShipping').rules('remove');
        $('#IsDefaultBilling').rules('remove');
    }

    ShowHideBillingAddressDiv() {
        if ($("#is_both_billing_shipping").is(":checked") && $("#is_both_billing_shipping").attr('data-addressType') == "shipping") {
            $("#BillingAddressContainer").hide();
        }
        else {
            $("#BillingAddressContainer").show();
        }
    }

    ShowHideShippingAddressDiv() {
        if ($("#sameAsShipping").is(":checked") && $("#sameAsShipping").attr('data-addressType') == "billing") {
            $("#shipping-content").hide();
            $("#IsSameAsBillingAddress").val("True");
        }
        else {
            $("#shipping-content").show();
            $("#IsSameAsBillingAddress").val("False");
        }
    }

    HideShippingDiv(): any {
        $("#shippingDiv").html("");
    }

    HideVoucherHistoryGridColumn(): any {
        var indexOfRow: number = $('#grid tbody tr:eq(0)').find('.OrderId').index() + 1;
        var indexOfOmsUserId: number = $('#grid tbody tr:eq(0)').find('.OmsUserId').index() + 1;
        $('th:nth-child(' + indexOfRow + ')').hide();
        $('th:nth-child(' + indexOfOmsUserId + ')').hide();
        $('#grid tbody tr').find('.OrderId').hide();
        $('#grid tbody tr').find('.OmsUserId').hide();
        var loginUserId = $("#hdnLoginUserId").val();

        $('#grid tbody tr').each(function () {
            if ($(this).find('.OmsUserId').text() != loginUserId) {
                $(this).find('.zf-view').hide();
                var ordernumber = $(this).find('.OrderNumber').find("a").html();
                $(this).find('.OrderNumber').html(ordernumber);
            }
        });
    }

    //Load shipping method on coupon apply/remove.
    LoadShippingOptionsOnCouponAction(): void {
        $("#loaderId").html(" <div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src= '../Content/Images/loading.svg' alt= 'Loading' class='dashboard-loader' /></div>");
        var isQuoteRequest = $('#IsQuoteRequest').val();
        var isPendingOrderRequest = $('#IsPendingOrderRequest').val();
        $("#shippingDiv").html("");
        Endpoint.prototype.ShippingOptions(true, isQuoteRequest, isPendingOrderRequest, function (response) {
            $("#loaderId").html("");
            if (response == null || response == undefined || response == "") {
                $(".shipping-method").html(ZnodeBase.prototype.getResourceByKeyName("InvalidAddressSelection"));
            }
            else {
                $(".shipping-method").html(response);
                Checkout.prototype.DisableShippingForFreeShippingAndDownloadableProduct();
                Checkout.prototype.ToggleFreeShipping();
            }
        });
    }

    // Validate Customer Shipping method 
    ValidateCustomerShipping(): void {
        if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && !($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "") && !($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#btnCompleteCheckout").attr("disabled", false);
            $("#errorAccountNumber").hide();
            $("#errorShippingMethod").hide();
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && !($("#AccountNumber").val() == undefined || $("#AccountNumber").val() == "")) {
            $("#errorAccountNumber").hide();
            Checkout.prototype.HideLoader();
        }
        else if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && !($("#ShippingMethod").val() == undefined || $("#ShippingMethod").val() == "")) {
            $("#errorShippingMethod").hide();
            Checkout.prototype.HideLoader();
        }
    }
    BindOrderSummaryForOrder(data): any {
        $("#summary-details").html(data.html);
        $("#dynamic-order-total").html(data.total);

        var coupons = data.coupons;
        var htmlString = "<div class='col-xs-12 nopadding'>";

        for (var dataIndex = 0; dataIndex < coupons.length; dataIndex++) {
            var style = coupons[dataIndex].CouponApplied ? "success-msg padding-top" : "error-msg";
            var message = coupons[dataIndex].PromotionMessage;
            var couponCode = coupons[dataIndex].Code;

            Checkout.prototype.RemoveDiscountMessages();
            htmlString = htmlString + "<p class='text-break " + style + "'>" + "<a class='zf-close' onclick='Checkout.prototype.RemoveAppliedCoupon(" + dataIndex + ")' style='cursor:pointer;color:#cc0000;padding-right:3px;' title='Remove Coupon Code'></a>" + "<b>" + couponCode + "</b>" + " - " + message + "</p>";
        }
        htmlString = htmlString + "</div>";
        $("#couponMessageContainer").html("");
        $("#couponMessageContainer").html(htmlString);
        $("#promocode").removeClass("promotion-block");

        vouchers = data.vouchers;
        Checkout.prototype.BindVoucherHtml(vouchers);
    }

    HideAuthorizeIframe(IsIframeRenderInPopup): any {
        if (IsIframeRenderInPopup == "True") {
            $('#AuthorizeNetModal').modal('hide');
            $('#submitandpaybutton').hide();
        }
        else {
            $('#iframe_holder').hide();
            if ($("#IsFromInvoice").val()) {
                $("#btnPayInvoice").show();
                $("#btnClosePopup").show();
            }

        }
        $("input:radio[name='PaymentOptions']").each(function (i) {
            this.checked = false;
        });
    }

    PaymentStoreApproval(userType, isApprovalRequired, isOABRequired, isStoreLevelAppoverOn, isStoreLevelApproverType, isNotGuest): any {
        if (userType != "guest") {
            if (Checkout.prototype.SetFlagForApprovalRouting(isApprovalRequired, isOABRequired, isStoreLevelAppoverOn) && isNotGuest) {                
                if (isStoreLevelApproverType.toLowerCase() == "store") {
                    Checkout.prototype.SubmitForApproval();
                }
                else if (isApprovalRequired.toLowerCase() == "true" && isStoreLevelApproverType.toLowerCase() != "store") {
                    Checkout.prototype.SubmitForApproval();
                }
                else if (isOABRequired.toLowerCase() == "true") {
                    Checkout.prototype.SubmitForApproval();
                }
                else {
                    Checkout.prototype.SubmitCheckOutForm();
                }
            }
            else {
                Checkout.prototype.SubmitCheckOutForm();
            }
        }
        else {
            Checkout.prototype.SubmitCheckOutForm();
            Checkout.prototype.HideLoader();
            return false;
        }
    };

    // To validate and render Braintree Iframe.
    BraintreeIframeValidationsAndRendering(gatewayCode, shippingOptionValue, controlId) {
        if ((shippingOptionValue == undefined) && ($("#QuoteId").val() == 0)) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", isFadeOut, fadeOutTime);
            $("input:radio[name='PaymentOptions']").each(function (i) {
                this.checked = false;
            });
            Checkout.prototype.HideLoader();
        }
        else {
            if (($("#QuoteId").val() > 0) || $("#IsFromInvoice").val()) {
                Checkout.prototype.BrainTreePayment(controlId);
            }
            else {
                $("#submitandpaybutton").show();
                $("#btnCompleteCheckout").hide();
            }
        }
    }

    // To generate braintree hosted fields
    BrainTreePayment(controlId): void {
        $("#divAuthorizeNetIFrame").html('');
        var orderNumber: number = 0;
        if ($("#OrderNumber").val() == null || $("#OrderNumber").val() == undefined) {
            Endpoint.prototype.GenerateOrderNumber(parseInt($("#hdnPortalId").val()), function (response) {
                orderNumber = response.orderNumber;
            });
        } else {
            orderNumber = $("#OrderNumber").val();
        }

        var paymentTokenModel = {
            PaymentSettingId: parseInt($('#PaymentSettingId').val()),
            PaymentCode: $("#hdnPaymentCode").val(),
            Total: $("#Total").val(),
            CustomerProfileId: $('#CustomerProfileId').val(),
            CustomerGUID: $("#hdnCustomerGUID").val(),
            GatewayCode: $("#hdnGatwayName").val(),
            UserId: $("#UserId").val(),
            IsIframeRenderInPopup: $("#QuoteId").val() == 0 ? true : false,
            OrderNumber: orderNumber
        }

        Endpoint.prototype.GetIframeViewWithToken(paymentTokenModel, '_HostedFieldsBrainTree', function (response) {
            if (response.isSuccess) {
                $("#divAuthorizeNetIFrame").show();
                $("#divAuthorizeNetIFrame").html(response.html);
                $("#CustomerProfileId").val(response.customerProfileId);
                $("#BrainTreeModal").modal('show');
                $("#btnCompleteCheckout").hide();
                $("#btnPayInvoice").hide();
                $("#divAuthorizeNetIFramePrvoider").show();
                Checkout.prototype.HideLoader();
            }
            else {
                var message = Checkout.prototype.GetPaymentErrorMsg(response);
                Checkout.prototype.ClearPaymentAndDisplayMessage(message);
                $("#divAuthorizeNetIFramePrvoider").hide();
            }
        })
    }

    // Submit braintree order
    SubmitBraintreeOrder(payload, isVault) {
        $('#SubmitButton').prop("disabled", true);
        $('#CancelButton').prop("disabled", true);
        var cardDetails = payload.details;
        $('#hdnBraintreecardNumber').val(cardDetails.lastFour);
        $("#hdnBraintreeCardExpirationMonth").val(cardDetails.expirationMonth);
        $("#hdnBraintreeCardExpirationYear").val(cardDetails.expirationYear);
        $("#hdnBraintreeCardHolderName").val(cardDetails.cardholderName);
        $("#hdnBraintreeCardType").val(cardDetails.cardType);
        $("#hdnBraintreeNonce").val(payload.nonce);
        $("#hdnBraintreeIsVault").val(isVault);
        Checkout.prototype.SubmitOrder();
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

    //Hide the Modal if billing address not provided
    HideModal() {
        if ($("#BrainTreeModal").hasClass('show')) {
            $("#BrainTreeModal").modal('hide');
        }
        if ($("#AuthorizeNetModal").hasClass('show')) {
            $("#AuthorizeNetModal").modal('hide');
        }
    }
}

$("#CredidCardNumber").on("blur", function (event) {
    let CredidCardNumber: string = $('input[data-payment="number"]').val().split(" ").join("");
    if (!Checkout.prototype.Mod10(CredidCardNumber) && CredidCardNumber != "") {
        $('#errornumber').show();
        Checkout.prototype.PaymentError("number");
        return false;
    }
    else {
        $('#errornumber').hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="number"]');
        $("#btnCompleteCheckout").attr("disabled", false);
    }
});

$("#cardconnectCardHolderName").on("keyup", function () {
    var IsValid = Checkout.prototype.ValidateCardConnectCardHolderName();
    if (IsValid) {
        $("#btnCompleteCheckout").attr("disabled", false);
    }
    else {
        $("#btnCompleteCheckout").attr("disabled", true);
        return false;
    }
});

$("#CreditCardExpMonth").on("focusout", function (event) {
    Checkout.prototype.ValidateCreditCardExpirationDetails(event);
});


$(document).on("focusout", '#expMonth', function (e) {
    Checkout.prototype.ValidateCreditCardExpirationDetailsCyberSource(event);
});

$(document).on("focusout", '#expYear', function (e) {
    Checkout.prototype.ValidateCreditCardExpirationDetailsCyberSource(event);
});

$(document).on("focusout", '#cyscardholderName', function () {
    Checkout.prototype.ValidateCyberSourceCardNameHolder();
});

$("#CreditCardExpYear").on("focusout", function (event) {
    Checkout.prototype.ValidateCreditCardExpirationDetails(event);
});

$("#CreditCardExpYear").on("paste", function (event) {
    Checkout.prototype.ValidateCreditCardExpirationDetails(event);
});

$("#CreditCardExpMonth").on("paste", function (event) {
    Checkout.prototype.ValidateCreditCardExpirationDetails(event);
});

$(document).on('blur', "input#CredidCardCVCNumberSaved", function () {
    var IsValid = Checkout.prototype.ValidateCVV();
    if (IsValid) {
        $("#btnCompleteCheckout").attr("disabled", false);
    }
});

$("#CredidCardCVCNumber").on("blur", function (event) {
    var cardType = $('input[name="PaymentProviders"]:checked').val();
    if ($('input[data-payment="cvc"]').val().length < 3 && $('input[data-payment="cvc"]').val() != "") {
        $('#errorcvc').show();
        Checkout.prototype.PaymentError("cvc");
        Checkout.prototype.ScrollTop();
        return false;
    } else if (cardType == Constant.AmericanExpressCardCode && $('input[data-payment="cvc"]').val().length < 4 && $('input[data-payment="cvc"]').val() != "") {
        $('#errorcvc').show();
        Checkout.prototype.PaymentError("cvc");
        Checkout.prototype.ScrollTop();
        return false;
    }
    else {
        $('#errorcvc').hide();
        $("#btnCompleteCheckout").attr("disabled", false);
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
    }
});

$("#CardHolderName").on("blur", function (event) {
    if ($('input[data-payment="cardholderName"]').val().trim() == "") {
        $('#errorcardholderName').show();
        Checkout.prototype.PaymentError("cardholderName");
        Checkout.prototype.ScrollTop();
        return false;
    }
    else {
        $('#errorcardholderName').hide();
        Checkout.prototype.RemoveCreditCardValidationCSS('input[data-payment="cardholderName"]');
        $("#btnCompleteCheckout").attr("disabled", false);
    }
});

$("#div-InvoiceMe input:checkbox").on("click", function () {
    if (!$(this).prop('checked')) {
        $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", true);
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Please select the checkbox to proceed", "error", true, 10000);
    }
    else {
        $('[name="singleCheckoutSubmitPayment"], [name="singleCheckoutSubmitQuote"]').prop("disabled", false);
    }
});

$("#divSinglePagePayment #paypal-express-checkout").on("click", function (ev) {
    var loderState = false;
    Checkout.prototype.ShowPaymentLoader();
    var Total = $("#Total").val();
    Total = Total.replace(',', '.');
    if (Total != "" && Total != null && Total != 'undefined') {
        Total = Total.replace(',', '');
    }
    var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();
    var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
    var AccountNumber = $("input[name='AccountNumber']").val();
    var ShippingMethod = $("input[name='ShippingMethod']").val();


    if (($("#shipping-content .address-name").text() == "")) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredShippingAddress"), "error", false, 0);
        loderState = true;
    }
    else if (($("#billing-content .address-name").text() == "")) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredBillingAddress"), "error", false, 0);
        loderState = true;
    }
    else if (shippingOptionValue == null || shippingOptionValue == "" && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectShippingOption"), "error", false, 0);
        loderState = true;
    }
    else if (paymentOptionValue == null || paymentOptionValue == "") {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"), "error", false, 0);
        loderState = true;
    }
    else if ($('#customerShippingDiv').is(':visible') && ((AccountNumber == null || ShippingMethod == null) || (AccountNumber == "" || ShippingMethod == ""))) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("CustomerShippingError"), "error", true, 5000);
        loderState = true;
    }
    else if ($("#EnableUserOrderAnnualLimit").val() && $("#EnableUserOrderAnnualLimit").val().toLowerCase() == "true" && parseInt($("#AnnualOrderLimit").val()) > 0 && (parseInt($("#AnnualBalanceOrderAmount").val()) - parseInt(Total) <= 0)) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AnnualOrderLimitFailed") + $("#AnnualOrderLimitWithCurrency").val(), "error", false, 0);
        loderState = true;
    }
    else if ($("#EnablePerOrderlimit").val() && $("#EnablePerOrderlimit").val().toLowerCase() == "true" && parseInt($("#PerOrderLimit").val()) > 0 && parseInt($("#PerOrderLimit").val()) <= parseInt(Total)) {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PerOrderLimitFailed") + $("#PerOrderLimitWithCurrency").val(), "error", false, 0);
        loderState = true;
    }
    else {
        if (!Checkout.prototype.ShippingErrorMessage(loderState)) {
            return false;
        }

        if ($("#dynamic-allowesterritories").length > 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AllowedTerritories"), "error", false, 0);
            Checkout.prototype.HidePaymentLoader();
            return false;
        }
        return Checkout.prototype.PayPalPaymentProcess();
    }

    if (loderState)
        Checkout.prototype.HidePaymentLoader();
});

function HideBillingAddress() {
    if ($("#IsBillingAddressOptional").val() == 'true')
        $("#BillingAddressContainer").attr("hidden", "hidden");
    else
        $("#BillingAddressContainer").removeAttr("hidden");
}
