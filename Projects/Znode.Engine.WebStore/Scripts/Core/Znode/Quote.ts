class Quote extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {

    }
    SubmitQuote(): any {
        Checkout.prototype.ShowLoader();
        //Set recipient name if recipient textbox is available
        Checkout.prototype.SaveRecipientNameAddressData('shipping', function (response) {
            if (!Quote.prototype.IsCheckoutDataValid()) {
                ZnodeBase.prototype.HideLoader();
            }
            else {
                if (!Quote.prototype.ShippingErrorMessage()) {
                    Checkout.prototype.HideLoader();
                    return false;
                }

                if ($("#dynamic-allowesterritories").length > 0) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AllowedTerritories"), "error", false, 0);
                    Checkout.prototype.HideLoader();
                    return false;
                }
                else {
                    var userType = ZnodeBase.prototype.GetParameterValues("mode");
                    if (userType == undefined) {
                        userType = "";
                    }
                    userType = (userType != "") ? userType.replace("#", "") : userType;

                    Quote.prototype.SubmitPlaceQuoteForm();
                }
            }
        });
    }

    SubmitPlaceQuoteForm(): void {
        var data = {};

        //Get all the selected values required to submit order.
        Quote.prototype.SetQuoteFormData(data);

        //Create form to submit order.
        var form = Quote.prototype.CreateSubmitQuoteForm(data);

        // submit form
        form.submit();
        form.remove();
    }

    //Get all the selected values required to submit order.
    SetQuoteFormData(data): any {
        data["ShippingAddressId"] = $("#shipping-content").find("#AddressId").val();
        data["BillingAddressId"] = $("#billing-content").find("#AddressId").val();
        data["ShippingId"] = $("input[name='ShippingOptions']:checked").val();//shipping type id
        data["ShippingCode"] = $("input[name='ShippingOptions']:checked").attr("data-shippingcode");
        data["AdditionalInstruction"] = $("#AdditionalInstruction").val();
        data["FreeShipping"] = $("#cartFreeShipping").val();
        data["InHandDate"] = $("#InHandDate").val();
        data["PortalId"] = $("#hdnPortalId").val();
        data["UserId"] = $("#UserId").val();
        data["ShippingConstraintCode"] = $("input[name='ShippingConstraintCode']:checked").val();
        data["JobName"] = $("#JobName").val();
        data["AccountNumber"] = $("#AccountNumber").val();
        data["ShippingMethod"] = $("#ShippingMethod").val();
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    //Create form to submit order.
    CreateSubmitQuoteForm(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/Quote/SubmitQuote");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        form.attr("__RequestVerificationToken", $('[name=__RequestVerificationToken]').val());
        Quote.prototype.AddFormFields(form, data);
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
        }
    }


    public IsCheckoutDataValid(): boolean {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        if (Total != "" && Total != null && Total != 'undefined') {
            Total = Total.replace(',', '');
        }

        var isValid: boolean = true;
        var shippingOptionValue = $("input[name='ShippingOptions']:checked").val();
        var isBillingAddresOptional = $("#IsBillingAddressOptional").val();

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

    ShippingErrorMessage(): any {
        var shippingErrorMessage = $("#ShippingErrorMessage").val();
        var shippingHasError = $("#ValidShippingSetting").val();

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
        return true;
    }

    GetPaymentOptions(): any {
        $("#errorPayment").html("");
        $('#payment-view-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");

        Endpoint.prototype.PaymentOptions(true, true, function (data) {
            var paymentOptionsHtml = data;
            Endpoint.prototype.AmazonPaymentOptions(true, function (response) {
                $("#billing-content").after(response.html);
                setTimeout(function () {
                    $("#payment-view-content").html(paymentOptionsHtml);
                    $("#btnConvertQuoteToOrder").show();
                }, 2000);
            });
        });
    }

    ClosePopup(): any {
        $("#errorPayment").html("");
        $("#payment-view-popup-ipad").find(".close").click();
        ZnodeBase.prototype.HideLoader();
    }

    ConvertQuoteToOrder(): any {
        var paymentCode = $("#hdnGatwayName").val().toLowerCase();
        if (paymentCode == Constant.CyberSource) {
            if ($('ul#creditCardTab ').find('li').find('a.active').attr('href') == "#savedCreditCard-panel" && $('ul#creditCardTab ').find('li.active').find('a').attr('href') == "#savedCreditCard-panel") {

                Quote.prototype.SubmitIframePayment("");

            }
            else {
                $("#pay-button").click();
            }
        }
        else {
            Quote.prototype.ConvertQuoteToOrderQuotes();
        }
    }
    ConvertQuoteToOrderQuotes(): any {
        if (!Quote.prototype.IsQuoteDataValid()) {
            Checkout.prototype.isPayMentInProcess = false;
        }
        else {
            var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
            var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);
            var userType = ZnodeBase.prototype.GetParameterValues("mode");
            if (userType == undefined) {
                userType = "";
            }
            userType = (userType != "") ? userType.replace("#", "") : userType;
            switch (paymentType.toLowerCase()) {
                case "cod":
                    Quote.prototype.ClosePopup();
                    Quote.prototype.SubmitQuoteForm();
                    break;
                case "credit_card":
                    if (Quote.prototype.IsValidCreditCardDetails()) {
                        var paymentCode = $("#hdnGatwayName").val();

                        if (paymentCode == Constant.CyberSource) {
                            Quote.prototype.SubmitIframePayment($("#CardDataToken").val());
                        }
                        else {
                            Quote.prototype.SubmitPayment();
                        }
                    }
                    break;
                case "ach":
                    Quote.prototype.SubmitACHPayment();
                    break;
                default:
                    // global data
                    if (Checkout.prototype.CheckValidPODocument()) {
                        Quote.prototype.SubmitQuoteForm();
                    }
                    else {
                        Checkout.prototype.HideLoader();
                        return false;
                    }
                    break;
            }
        }
    }

    //Submit ACH Payment
    SubmitACHPayment(): any {
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
            Quote.prototype.ClosePopup();
            Checkout.prototype.ShowPaymentProcessDialog();

            Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {

                Checkout.prototype.isPayMentInProcess = currentStatus;
                if (!response.Billing.HasError) {
                    if ($("#ajaxProcessPaymentError").html() == undefined) {
                    } else {
                        $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                    }


                    var { BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId } = Quote.prototype.GetOrderDetails(response);
                    var { cardNumber, cardExpirationMonth, cardExpirationYear, cardHolderName } = Quote.prototype.GetCardDetails();

                    var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? true : false;
                    var guid = $('#GUID').val();
                    var discount = $('#Discount').val();
                    var ShippingCost = $('#ShippingCost').val();
                    var SubTotal = $('#SubTotal').val();
                    var cardType = $("#hdnGatwayName").val() == "cardconnect" ? Checkout.prototype.DetectCardTypeForCardConnect(cardNumber) : Checkout.prototype.DetectCardType(cardNumber);
                    var orderNumber = response.orderNumber;
                    if (cardNumber != "") {
                        $("#hdnCreditCardNumber").val(cardNumber.slice(-4));
                    }

                    var paymentSettingId = $('#PaymentSettingId').val();
                    var paymentCode = $('#hdnPaymentCode').val();
                    var CustomerPaymentProfileId = $('#CustomerPaymentProfileId').val();
                    var CustomerProfileId = $('#CustomerProfileId').val();
                    var CardDataToken = $('#CardDataToken').val();
                    var gatewayCode = $("#hdnGatwayName").val();

                    if (Total.indexOf(',') > -1) {
                        Total.replace(',', '');
                    }

                    //Get Payment model
                    var payment = Quote.prototype.GetPaymentACHModel(guid, gatewayCode, BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCost, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId, SubTotal, Total, discount, cardNumber, CustomerPaymentProfileId, CustomerProfileId, CardDataToken, cardType, paymentSettingId, IsAnonymousUser, paymentCode, orderNumber, cardExpirationYear, cardExpirationMonth, cardHolderName);

                    //Validate Payment Profile And Proceed for Convert To Order
                    Quote.prototype.ValidatePaymentAndPayInvoiceACH(payment, paymentSettingId, paymentCode, gatewayCode);
                }
            });
        }


    }

    //AuthorizeNet Payment
    SubmitAuthorizeNetPayment(querystr: any) {
        var transactionResponse = JSON.parse(querystr);
        var Total = transactionResponse.totalAmount;
        var transactionId = transactionResponse.transId;
        var creditCardNumber = transactionResponse.accountNumber;
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);
        var orderInvoiceNumber = transactionResponse.orderInvoiceNumber;

        if (Checkout.prototype.IsOrderTotalGreaterThanZero($("#Total").val())) {
            Quote.prototype.ClosePopup();
            Checkout.prototype.ShowPaymentProcessDialog();
            var submitPaymentViewModel = Quote.prototype.GetAuthorizeNetPaymentModel(paymentType, transactionId, creditCardNumber, orderInvoiceNumber);
            var token = $("[name='__RequestVerificationToken']").val();
            $.ajax({
                type: "POST",
                url: "/quote/ConvertQuoteToOrder",
                async: true,
                data: { __RequestVerificationToken: token, convertToOrderViewModel: submitPaymentViewModel },
                success: function (response) {
                    if (response.error != null && response.error != "" && response.error != 'undefined') {
                        Checkout.prototype.HidePaymentProcessDialog();
                        $("#layout-account-orderhistory").html('');
                        $("#layout-account-orderhistory").html(response.receiptHTML);
                        var message = Checkout.prototype.GetPaymentErrorMsg(response);
                        Quote.prototype.ClearPaymentAndDisplayMessage(message);
                        Checkout.prototype.HideLoader();
                        return false;
                    }
                    else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                        //Quote.prototype.CanclePayment();
                        Checkout.prototype.HidePaymentProcessDialog();
                        //This will focus to the top of screen.
                        $(this).scrollTop(0);
                        $('body, html').animate({ scrollTop: 0 }, 'fast');
                        $(".cartcount").html('0');
                        $("#messageBoxContainerId").hide();
                        $(".cartAmount").html('');
                        window.location.href = "/Checkout/OrderCheckoutReceipt";
                    }
                },
                error: function () {
                    Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
            });
        }
        else
            ZnodeBase.prototype.HideLoader();
    }

    GetAuthorizeNetPaymentModel(paymentType: any, transactionId: any, creditCardNumber: any, orderInvoiceNumber: any) {
        return {
            OmsQuoteId: $("#QuoteId").val(),
            UserId: $("#hdnUserId").val(),
            PaymentDetails: {
                PaymentSettingId: $('#PaymentSettingId').val(),
                GatewayCode: $("#hdnGatwayName").val(),
                PaymentCode: $('#hdnPaymentCode').val(),
                paymentType: paymentType,
                TransactionId: transactionId,
                CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                CustomerProfileId: $('#CustomerProfileId').val(),
                IsSaveCreditCard: $("#AuthNetSaveCreditCard").is(':checked'),
                CreditCardNumber: (creditCardNumber).slice(-4),
                CardType: 'credit_card',
                PaymentAmount: $("#Total").val(),
                OrderId: orderInvoiceNumber
            }

        };
    }

    ValidatePaymentAndPayInvoiceACH(payment: { "GUID": any; "GatewayType": any; "BillingCity": any; "BillingCountryCode": any; "BillingFirstName": any; "BillingLastName": any; "BillingPhoneNumber": any; "BillingPostalCode": any; "BillingStateCode": any; "BillingStreetAddress1": any; "BillingStreetAddress2": any; "BillingEmailId": any; "ShippingCost": any; "ShippingCity": any; "ShippingCountryCode": any; "ShippingFirstName": any; "ShippingLastName": any; "ShippingPhoneNumber": any; "ShippingPostalCode": any; "ShippingStateCode": any; "ShippingStreetAddress1": any; "ShippingStreetAddress2": any; "ShippingEmailId": any; "SubTotal": any; "Total": any; "Discount": any; "PaymentToken": any; "CardNumber": any; "CardExpirationMonth": any; "CardExpirationYear": any; "GatewayCurrencyCode": any; "CustomerPaymentProfileId": any; "CustomerProfileId": any; "CardDataToken": any; "CardType": any; "PaymentSettingId": any; "IsAnonymousUser": boolean; "IsSaveCreditCard": boolean; "CardHolderName": any; "CustomerGUID": any; "PaymentCode": any; "OrderId": any; }, paymentSettingId: any, paymentCode: any, gatewayCode: any) {
        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
        var creditCardNumber: string = $('#CredidCardNumber').val();
        $("#div-CreditCard").hide();
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);

        submitCard(payment, function (response) {
            if (response.GatewayResponse == undefined) {
                if (response.indexOf("Unauthorized") > 0) {
                    Checkout.prototype.HidePaymentProcessDialog();
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                    Checkout.prototype.HideLoader();
                    Checkout.prototype.isPayMentInProcess = false;
                }
            }
            else {
                var isSuccess = response.GatewayResponse.IsSuccess;
                if (isSuccess) {
                    Quote.prototype.ClosePopup();
                    var submitPaymentViewModel = Quote.prototype.GetSubmitPaymentViewACHModel(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    var token = $("[name='__RequestVerificationToken']").val();
                    $.ajax({
                        type: "POST",
                        url: "/quote/ConvertQuoteToOrder",
                        async: true,
                        data: { __RequestVerificationToken: token, convertToOrderViewModel: submitPaymentViewModel },
                        success: function (response) {
                            Checkout.prototype.isPayMentInProcess = false;
                            if (response.error != null && response.error != "" && response.error != 'undefined') {
                                Checkout.prototype.HidePaymentProcessDialog();
                                $("#layout-account-orderhistory").html('');
                                $("#layout-account-orderhistory").html(response.receiptHTML);
                                var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                Quote.prototype.ClearPaymentAndDisplayMessage(message);
                                Checkout.prototype.HideLoader();
                                return false;
                            }
                            else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                Quote.prototype.CanclePayment();
                                Checkout.prototype.HidePaymentProcessDialog();
                                //This will focus to the top of screen.
                                $(this).scrollTop(0);
                                $('body, html').animate({ scrollTop: 0 }, 'fast');
                                $(".cartcount").html('0');
                                $("#messageBoxContainerId").hide();
                                $(".cartAmount").html('');
                                window.location.href = "/Checkout/OrderCheckoutReceipt";
                            }
                        },
                        error: function () {
                            Checkout.prototype.HidePaymentProcessDialog();
                            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                            Checkout.prototype.HideLoader();
                            return false;
                        }
                    });
                }
                else {
                    Checkout.prototype.HidePaymentProcessDialog();
                    Quote.prototype.PaymentFailedProcess(response, gatewayCode);
                }
            }
        });
    }

    GetSubmitPaymentViewACHModel(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsQuoteId: $("#QuoteId").val(),
            UserId: $("#hdnUserId").val(),
            PaymentDetails: {
                PaymentSettingId: paymentSettingId,
                PaymentCode: paymentCode,
                CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                CustomerGuid: response.GatewayResponse.CustomerGUID,
                PaymentToken: $("input[name='CCdetails']:checked").val(),
                paymentType: paymentType,
                CreditCardNumber: creditCardNumber.slice(-4),
                RemainingAmount: parseFloat($("#Total").val()),
                PaymentAmount: parseFloat($("#paymentAmount").val()),
                IsACHPayment: true
            }
        };
    }

    public IsQuoteDataValid(): boolean {
        var isValid: boolean = true;
        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();

        if (paymentOptionValue == null || paymentOptionValue == "") {
            isValid = false;
            $("#errorPayment").html(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"));
            Checkout.prototype.HidePaymentLoader();
        }
        return isValid;
    }

    SubmitQuoteForm(): void {
        Checkout.prototype.ShowLoader();
        var data = {};

        //Get all the selected values required to submit order.
        Quote.prototype.SetPaymentData(data);

        //Create form to submit order.
        var form = Quote.prototype.CreateForm(data);

        // submit form
        form.submit();
        form.remove();
    }

    //Get all the selected values required to submit order.
    SetPaymentData(data): any {
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);
        data["OmsQuoteId"] = $("#QuoteId").val();
        data["UserId"] = $("#hdnUserId").val();
        data["PaymentDetails.PaymentSettingId"] = $("input[name='PaymentOptions']:checked").val();
        data["PaymentDetails.paymentType"] = paymentType;
        data["PaymentDetails.PurchaseOrderNumber"] = $("#txtPurchaseOrderNumber").val();
        data["PaymentDetails.PODocumentName"] = $("#po-document-path").val();
        data["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    }

    //Create form to submit order.
    CreateForm(data): any {
        var form = $('<form/></form>');
        form.attr("action", "/Quote/ConvertQuoteToOrder");
        form.attr("method", "POST");
        form.attr("style", "display:none;");
        form.attr("enctype", "multipart/form-data");
        form.attr("__RequestVerificationToken", $('[name=__RequestVerificationToken]').val());
        Checkout.prototype.AddFormFields(form, data);
        $("body").append(form);
        return form;
    }

    SubmitIframePayment(querystr: any) {  // SubmitCybersourceePayment(querystr: any)
        var Total = $("#Total").val();
        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {

            $("#div-CreditCard").hide();
            var orderNumber = "";
            Endpoint.prototype.GenerateOrderNumber($("#hdnPortalId").val(), function (response) {
                orderNumber = response.orderNumber;
            });
            var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
            var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);

            var submitPaymentViewModel = {

                OmsQuoteId: $("#QuoteId").val(),
                UserId: $("#hdnUserId").val(),
                PaymentDetails: {
                    PaymentSettingId: $('#PaymentSettingId').val(),
                    PaymentCode: $('#hdnPaymentCode').val(),

                    ShippingOptionId: $("[name='ShippingId']").val(),
                    BillingAddressId: $("#billing-content").find("#AddressId").val(),
                    ShippingAddressId: $("#shipping-content").find("#AddressId").val(),
                    PortalId: $("#hdnPortalId").val(),
                    PortalCatalogId: $("#PortalCatalogId").val(),
                    AdditionalInfo: $("#additionalInstructions").val(),
                    EnableAddressValidation: $("input[name='EnableAddressValidation']").val(),
                    RequireValidatedAddress: $("input[name='RequireValidatedAddress']").val(),
                    AccountNumber: $("#AccountNumber").val(),
                    ShippingMethod: $("#ShippingMethod").val(),
                    CardType: 'credit_card',
                    OrderNumber: orderNumber,
                    InHandDate: $("#InHandDate").val(),
                    JobName: $("#JobName").val(),
                    ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                    CyberSourceToken: querystr,
                    paymentType: paymentType,
                    IsSaveCreditCard: $("#SaveCreditCard").is(':checked'),
                    CustomerProfileId: $('#CustomerProfileId').val(),
                    CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                    CustomerGuid: $("#hdnCustomerGUID").val(),
                    PaymentGUID: $("#hdnPaymentGUID").val(),
                    GatewayCode: $("#hdnGatwayName").val(),
                }

            };

            $.ajax({
                type: "POST",
                url: "/quote/ConvertQuoteToOrder",
                async: true,
                data: submitPaymentViewModel,
                success: function (response) {
                    if (response.error != null && response.error != "" && response.error != 'undefined') {
                        Checkout.prototype.HidePaymentProcessDialog();
                        $("#layout-account-orderhistory").html('');
                        $("#layout-account-orderhistory").html(response.receiptHTML);
                        var message = Checkout.prototype.GetPaymentErrorMsg(response);
                        Quote.prototype.ClearPaymentAndDisplayMessage(message);
                        Checkout.prototype.HideLoader();
                        return false;
                    }
                    else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                        Quote.prototype.CanclePayment();
                        Checkout.prototype.HidePaymentProcessDialog();
                        //This will focus to the top of screen.
                        $(this).scrollTop(0);
                        $('body, html').animate({ scrollTop: 0 }, 'fast');
                        $(".cartcount").html('0');
                        $("#messageBoxContainerId").hide();
                        $(".cartAmount").html('');
                        window.location.href = "/Checkout/OrderCheckoutReceipt";
                    }
                },
                error: function () {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
            });
        }
        else
            ZnodeBase.prototype.HideLoader();
    }
    //Submit Payment
    SubmitPayment(): any {
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');

        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            if (Quote.prototype.IsValidCreditCardDetails()) {

                var shippingId = $("#shipping-content").find("#AddressId").val();
                var billingId = $("#billing-content").find("#AddressId").val();
                if ($("#IsBillingAddressOptional").val() == 'true' && (parseInt(billingId) == 0)) {
                    billingId = $("#shipping-content").find("#AddressId").val();
                    $("#billing-content").find("#AddressId").val(billingId);
                }
                var currentStatus = Checkout.prototype.isPayMentInProcess;
                Quote.prototype.ClosePopup();
                Checkout.prototype.ShowPaymentProcessDialog();

                Endpoint.prototype.GetshippingBillingAddress(parseInt($("#hdnPortalId").val()), parseInt(shippingId), parseInt(billingId), function (response) {

                    Checkout.prototype.isPayMentInProcess = currentStatus;
                    if (!response.Billing.HasError) {
                        if ($("#ajaxProcessPaymentError").html() == undefined) {
                        } else {
                            $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                        }


                        var { BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId } = Quote.prototype.GetOrderDetails(response);
                        var { cardNumber, cardExpirationMonth, cardExpirationYear, cardHolderName } = Quote.prototype.GetCardDetails();

                        var IsAnonymousUser = $("#hdnAnonymousUser").val() == 0 ? true : false;
                        var guid = $('#GUID').val();
                        var discount = $('#Discount').val();
                        var ShippingCost = $('#ShippingCost').val();
                        var SubTotal = $('#SubTotal').val();
                        var cardType = $("#hdnGatwayName").val() == "cardconnect" ? Checkout.prototype.DetectCardTypeForCardConnect(cardNumber) : $("#hdnGatwayName").val() === Constant.BrainTree ? $("#hdnBraintreeCardType").val() : Checkout.prototype.DetectCardType(cardNumber);
                        var orderNumber = response.orderNumber;
                        if (cardNumber != "") {
                            $("#hdnCreditCardNumber").val(cardNumber.slice(-4));
                        }

                        if ($("#addNewCreditCard-panel").attr("class").indexOf("active") != -1 && $("#hdnGatwayName").val() != Constant.BrainTree) {
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

                        //Get Payment model
                        var payment = Quote.prototype.GetPaymentModel(guid, gatewayCode, BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCost, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId, SubTotal, Total, discount, cardNumber, CustomerPaymentProfileId, CustomerProfileId, CardDataToken, cardType, paymentSettingId, IsAnonymousUser, paymentCode, orderNumber, cardExpirationYear, cardExpirationMonth, cardHolderName);

                        //Validate Payment Profile And Proceed for Convert To Order
                        Quote.prototype.ValidatePaymentProfileAndConvertToOrder(payment, paymentSettingId, paymentCode, gatewayCode);
                    }
                });
            }

        }
    }

    ValidatePaymentProfileAndConvertToOrder(payment: { "GUID": any; "GatewayType": any; "BillingCity": any; "BillingCountryCode": any; "BillingFirstName": any; "BillingLastName": any; "BillingPhoneNumber": any; "BillingPostalCode": any; "BillingStateCode": any; "BillingStreetAddress1": any; "BillingStreetAddress2": any; "BillingEmailId": any; "ShippingCost": any; "ShippingCity": any; "ShippingCountryCode": any; "ShippingFirstName": any; "ShippingLastName": any; "ShippingPhoneNumber": any; "ShippingPostalCode": any; "ShippingStateCode": any; "ShippingStreetAddress1": any; "ShippingStreetAddress2": any; "ShippingEmailId": any; "SubTotal": any; "Total": any; "Discount": any; "PaymentToken": any; "CardNumber": any; "CardExpirationMonth": any; "CardExpirationYear": any; "GatewayCurrencyCode": any; "CustomerPaymentProfileId": any; "CustomerProfileId": any; "CardDataToken": any; "CardType": any; "PaymentSettingId": any; "IsAnonymousUser": boolean; "IsSaveCreditCard": boolean; "CardHolderName": any; "CustomerGUID": any; "PaymentCode": any; "OrderId": any; }, paymentSettingId: any, paymentCode: any, gatewayCode: any) {
        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
        var creditCardNumber: string = $("#hdnGatwayName").val() === Constant.BrainTree ? $('#hdnBraintreecardNumber').val() : $('#CredidCardNumber').val();
        $("#div-CreditCard").hide();
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);

        submitCard(payment, function (response) {
            if (response.GatewayResponse == undefined) {
                if (response.indexOf("Unauthorized") > 0) {
                    Checkout.prototype.HidePaymentProcessDialog();
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                    Checkout.prototype.HideLoader();
                    Checkout.prototype.isPayMentInProcess = false;
                }
            }
            else {
                var isSuccess = response.GatewayResponse.IsSuccess;
                if (isSuccess) {
                    Quote.prototype.ClosePopup();
                    var submitPaymentViewModel = Quote.prototype.GetSubmitPaymentViewModel(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    var token = $("[name='__RequestVerificationToken']").val();
                    $.ajax({
                        type: "POST",
                        url: "/quote/ConvertQuoteToOrder",
                        async: true,
                        data: { __RequestVerificationToken: token, convertToOrderViewModel: submitPaymentViewModel },
                        success: function(response) {
                            Checkout.prototype.isPayMentInProcess = false;
                            if (response.error != null && response.error != "" && response.error != 'undefined') {
                                Checkout.prototype.HidePaymentProcessDialog();
                                var message = Checkout.prototype.GetPaymentErrorMsg(response);
                                Quote.prototype.ClearPaymentAndDisplayMessage(message);
                                Checkout.prototype.HideLoader();
                                return false;
                            }
                            else if (response.receiptHTML != null && response.receiptHTML != "" && response.receiptHTML != 'undefined') {
                                Quote.prototype.CanclePayment();
                                Checkout.prototype.HidePaymentProcessDialog();
                                //This will focus to the top of screen.
                                $(this).scrollTop(0);
                                $('body, html').animate({ scrollTop: 0 }, 'fast');
                                $(".cartcount").html('0');
                                $("#messageBoxContainerId").hide();
                                $(".cartAmount").html('');
                                window.location.href = "/Checkout/OrderCheckoutReceipt";
                            }
                        },
                        error: function() {
                            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                            Checkout.prototype.HideLoader();
                            return false;
                        }
                    });
                }
                else {
                    Checkout.prototype.HidePaymentProcessDialog();
                    Quote.prototype.PaymentFailedProcess(response, gatewayCode);
                }
            }
        });
    }

    PaymentFailedProcess(response: any, gatewayCode: any) {
        Checkout.prototype.isPayMentInProcess = false;
        var errorMessage = response.GatewayResponse.ResponseText;
        if (errorMessage == undefined) {
            errorMessage = response.GatewayResponse.GatewayResponseData;
        }
        if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
            Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacementCardDataMissing"));
        }
        else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
            Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
            $("#div-CreditCard").show();
        }
        else if (errorMessage != null && errorMessage != undefined && errorMessage.indexOf('customer') > 0) {
            Checkout.prototype.ClearPaymentAndDisplayMessage(errorMessage);
        }
        else {
            switch (gatewayCode.toLowerCase()) {
                case "payflow":
                    if (response.GatewayResponse.ResponseText)
                        Checkout.prototype.ClearPaymentAndDisplayMessage(response.GatewayResponse.ResponseText);
                    else
                        Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacement"));
                    break;
                default:
                    Checkout.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlacement"));
            }
        }
        Checkout.prototype.HideLoader();
    }

    GetSubmitPaymentViewModel(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsQuoteId: $("#QuoteId").val(),
            UserId: $("#hdnUserId").val(),
            PaymentDetails: {
                PaymentSettingId: paymentSettingId,
                PaymentCode: paymentCode,
                CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                CustomerGuid: response.GatewayResponse.CustomerGUID,
                PaymentToken: $("input[name='CCdetails']:checked").val(),
                paymentType: paymentType,
                CreditCardNumber: $("#hdnGatwayName").val() == "cardconnect" ? $('#CardDataToken').val().slice(-4) : creditCardNumber.slice(-4)
            }
        };
    }

    GetPaymentModel(guid: any, gatewayCode: any, BillingCity: any, BillingCountryCode: any, BillingFirstName: any, BillingLastName: any, BillingPhoneNumber: any, BillingPostalCode: any, BillingStateCode: any, BillingStreetAddress1: any, BillingStreetAddress2: any, BillingEmailId: any, ShippingCost: any, ShippingCity: any, ShippingCountryCode: any, ShippingFirstName: any, ShippingLastName: any, ShippingPhoneNumber: any, ShippingPostalCode: any, ShippingStateCode: any, ShippingStreetAddress1: any, ShippingStreetAddress2: any, ShippingEmailId: any, SubTotal: any, Total: any, discount: any, cardNumber: any, CustomerPaymentProfileId: any, CustomerProfileId: any, CardDataToken: any, cardType: any, paymentSettingId: any, IsAnonymousUser: boolean, paymentCode: any, orderNumber: any, cardExpirationYear: any, cardExpirationMonth: any, cardHolderName: any) {
        return {
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
            "CardExpirationMonth": cardExpirationMonth,
            "CardExpirationYear": cardExpirationYear,
            "GatewayCurrencyCode": $('#hdnCurrencyCode').val(),
            "CustomerPaymentProfileId": CustomerPaymentProfileId,
            "CustomerProfileId":gatewayCode === Constant.BrainTree ? null : CustomerProfileId,
            "CardDataToken": CardDataToken,
            "CardType": cardType,
            "PaymentSettingId": paymentSettingId,
            "IsAnonymousUser": IsAnonymousUser,
            "IsSaveCreditCard": gatewayCode === Constant.BrainTree ? $("#hdnBraintreeIsVault").val() : $("#SaveCreditCard").is(':checked'), //Set default value to true for Braintree.
            "CardHolderName": cardHolderName,
            "CustomerGUID": $("#hdnCustomerGUID").val(),
            "PaymentCode": paymentCode,
            "OrderId": orderNumber,
            "PaymentMethodNonce": $('#hdnBraintreeNonce').val()
        };
    }

    GetPaymentACHModel(guid: any, gatewayCode: any, BillingCity: any, BillingCountryCode: any, BillingFirstName: any, BillingLastName: any, BillingPhoneNumber: any, BillingPostalCode: any, BillingStateCode: any, BillingStreetAddress1: any, BillingStreetAddress2: any, BillingEmailId: any, ShippingCost: any, ShippingCity: any, ShippingCountryCode: any, ShippingFirstName: any, ShippingLastName: any, ShippingPhoneNumber: any, ShippingPostalCode: any, ShippingStateCode: any, ShippingStreetAddress1: any, ShippingStreetAddress2: any, ShippingEmailId: any, SubTotal: any, Total: any, discount: any, cardNumber: any, CustomerPaymentProfileId: any, CustomerProfileId: any, CardDataToken: any, cardType: any, paymentSettingId: any, IsAnonymousUser: boolean, paymentCode: any, orderNumber: any, cardExpirationYear: any, cardExpirationMonth: any, cardHolderName: any) {
        return {
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
            "CardNumber": cardNumber,
            "CardExpirationMonth": cardExpirationMonth,
            "CardExpirationYear": cardExpirationYear,
            "GatewayCurrencyCode": $('#hdnCurrencyCode').val(),
            "CustomerPaymentProfileId": CustomerPaymentProfileId,
            "CustomerProfileId": CustomerProfileId,
            "CardDataToken": CardDataToken,
            "CardType": cardType,
            "PaymentSettingId": paymentSettingId,
            "IsAnonymousUser": IsAnonymousUser,
            "IsSaveCreditCard": $("#SaveACHAccount").is(':checked'),
            "CardHolderName": cardHolderName,
            "CustomerGUID": $("#hdnCustomerGUID").val(),
            "PaymentCode": paymentCode,
            "OrderId": orderNumber,
            "IsACHPayment": true
        };
    }

    GetCardDetails() {
        var cardNumber: string
        var cardExpirationMonth: string
        var cardExpirationYear: string
        var cardHolderName: string

        if ($("#hdnGatwayName").val() == "cardconnect") {
            cardNumber = $('#CardDataToken').val();
            cardExpirationMonth = $("#CardExpirationDate").val().substring(4);
            cardExpirationYear = $("#CardExpirationDate").val().substring(0, 4);
            cardHolderName = $("#cardconnectCardHolderName").val();
        }
        else if ($("#hdnGatwayName").val() == Constant.BrainTree) {
            cardNumber = $('#hdnBraintreecardNumber').val();
            cardExpirationMonth = $("#hdnBraintreeCardExpirationMonth").val();
            cardExpirationYear = $("#hdnBraintreeCardExpirationYear").val();
            cardHolderName = $("#hdnBraintreeCardHolderName").val();
        }
        else {
            cardNumber = $("#div-CreditCard [data-payment='number']").val().split(" ").join("");
            cardExpirationMonth = $("#div-CreditCard [data-payment='exp-month']").val();
            cardExpirationYear = $("#div-CreditCard [data-payment='exp-year']").val();
            cardHolderName = $("#div-CreditCard [data-payment='cardholderName']").val();
        }

        return { cardNumber, cardExpirationMonth, cardExpirationYear, cardHolderName};
    }

    GetOrderDetails(response: any) {
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
        return { BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId };
    }

    IsValidCreditCardDetails(): boolean {
        var isValid: boolean = true;
        var paymentCode = $('#hdnGatwayName').val();
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
            if (paymentCode == Constant.CyberSource) {
                isValid = Checkout.prototype.ValidateCardConnectDataToken();
            }
            else if (paymentCode != "cardconnect" && paymentCode != Constant.BrainTree) {
                isValid = Checkout.prototype.ValidateCreditCardDetails();
            }
            else if (paymentCode === Constant.BrainTree) {
                isValid = Quote.prototype.ValidateBrainTreeCardDetails();
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
            return false;
        }
        return isValid;
    }

    ClearPaymentAndDisplayMessage(message): any {
        Quote.prototype.CanclePayment();
        $("#errorPayment").html(message);
    }

    CanclePayment(): any {
        $("#div-CreditCard").hide();
        $("#div-CreditCard [data-payment='number']").val('');
        $("#div-CreditCard [data-payment='cvc']").val('');
        $("#div-CreditCard [data-payment='exp-month']").val('');
        $("#div-CreditCard [data-payment='exp-year']").val('');
        $("#div-CreditCard [data-payment='cardholderName']").val('');
        $("input[name='PaymentOptions']:checked").prop('checked', false);
    }


    ConvertToOrderWithPaypalPayment(): any {
        var loderState = false;
        Checkout.prototype.ShowPaymentLoader();
        var Total = $("#Total").val();
        Total = Total.replace(',', '.');
        if (Total != "" && Total != null && Total != 'undefined') {
            Total = Total.replace(',', '');
        }
        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();
        if (paymentOptionValue == null || paymentOptionValue == "") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"), "error", false, 0);
            loderState = true;
        } else {
            if ($("#dynamic-allowesterritories").length > 0) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AllowedTerritories"), "error", false, 0);
                Checkout.prototype.HidePaymentLoader();
                return false;
            }
            return Quote.prototype.ProcessPayPalPayment();
        }

        if (loderState)
            Checkout.prototype.HideLoader();
    }

    ProcessPayPalPayment(): any {

        var Total = $("#Total").val();
        var url = [];
        if (Checkout.prototype.IsOrderTotalGreaterThanZero(Total)) {
            Endpoint.prototype.GetPaymentDetails($('#PaymentSettingId').val(), false, function (response) {
                Checkout.prototype.SetPaymentDetails(response.Response);
                if (response.error != null && response.error != "" && response.error != 'undefined') {
                    Checkout.prototype.HidePaymentLoader();
                    $("#errorPayment").html(response.error);
                }
                else if (!response.HasError) {
                    url = Quote.prototype.PayPalPayment();
                }
                else {
                    ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder");
                }
            });
        }
        return false;
    }

    public PayPalPayment(): any {
        Checkout.prototype.ShowPaypalPaymentProcessDialog();
        var urlhost = document.location.origin;
        let paymentCode: string = $("#hdnPaymentCode").val();
        var PaymentSettingId = $('#PaymentSettingId').val();
        var cancelUrl = urlhost + "/quote/GetQuote?omsQuoteId=" + $("#hdnOmsQuoteId").val() + "";
        var returnUrl = urlhost + "/quote/PaypalRequest?PaymentSettingId=" + PaymentSettingId + "&paymentCode=" + paymentCode + "" + "&quoteId=" + $("#QuoteId").val();
        var token = $("[name='__RequestVerificationToken']").val();

        var submitPaymentViewModel = {
            OmsQuoteId: $("#QuoteId").val(),
            UserId: $("#hdnUserId").val(),
            PaymentDetails: {
                PaymentSettingId: PaymentSettingId,
                PaymentCode: paymentCode,
                PaymentType: "PayPalExpress",
                PayPalReturnUrl: returnUrl,
                PayPalCancelUrl: cancelUrl,
            }
        }

        var paypalDetails = [];
        $.ajax({
            type: "POST",
            url: "/quote/ConvertQuoteToOrder",
            data: { __RequestVerificationToken: token, convertToOrderViewModel: submitPaymentViewModel },
            async: false,
            success: function (response) {
                if (response.error != null && response.error != "" && response.error != 'undefined') {
                    Quote.prototype.ClearPaymentAndDisplayMessage(response.error);
                    Checkout.prototype.HidePaymentLoader();
                    $("#div-PaypalExpress").hide();
                    return false;
                } else if (response.responseText != null && response.responseText != "" && response.responseText != 'undefined') {
                    $("#div-PaypalExpress").hide();
                    if (response.responseText != undefined && response.responseText.indexOf('Message=') >= 0) {
                        Checkout.prototype.HidePaymentLoader();
                        $("#errorPayment").html(ZnodeBase.prototype.getResourceByKeyName("SelectCOD"));
                    } else if (response.responseText.indexOf("http") != -1) {
                        Quote.prototype.ClosePopup();
                        window.location.href = response.responseText;
                    }
                    else {
                        Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                        Checkout.prototype.HidePaymentLoader();
                        $("#div-PaypalExpress").hide();
                        return false;
                    }
                } else {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    Checkout.prototype.HidePaymentLoader();
                    $("#div-PaypalExpress").hide();
                    return false;
                }
            },
            error: function () {
                Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                Checkout.prototype.HidePaymentLoader();
                $("#div-PaypalExpress").hide();
                return false;
            }
        });
        return paypalDetails;
    }

    //Process amazon payment.
    public ProcessAmazonPayment(total, paymentSettingId, paymentCode): any {
        var url = [];
        if (Checkout.prototype.IsOrderTotalGreaterThanZero(total)) {
            Endpoint.prototype.GetPaymentDetails(paymentSettingId, false, function (response) {
                Checkout.prototype.SetPaymentDetails(response);
                if (!response.HasError) {
                    $("#ajaxProcessPaymentError").html(ZnodeBase.prototype.getResourceByKeyName("ProcessingPayment"));
                    Checkout.prototype.ShowAmazonPaymentProcessDialog();
                    url = Quote.prototype.AmazonPayPayment(paymentSettingId, paymentCode);
                }
                else {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    Checkout.prototype.HideAmazonPaymentProcessDialog();
                }
            });
        }
        if (url != null)
            return url;
        Checkout.prototype.HideAmazonPaymentProcessDialog();

        return false;
    }

    //Call :Amazon Pay
    public AmazonPayPayment(paymentSettingId, paymentCode): any {
        var urlhost = document.location.origin;
        var PaymentSettingId = paymentSettingId;
        var amazonOrderReferenceId = $("#hdnOrderReferenceId").val();
        var returnUrl = urlhost + "/quote/AmazonRequest?quoteId=" + $("#QuoteId").val() + "&amazonOrderReferenceId="
            + amazonOrderReferenceId + "&PaymentType=AmazonPay" + "&PaymentSettingId="
            + PaymentSettingId + "&paymentCode=" + paymentCode;

        return Quote.prototype.ConvertToOrderWithAmazonPay(PaymentSettingId, paymentCode, urlhost, returnUrl, amazonOrderReferenceId);
    }


    ConvertToOrderWithAmazonPay(PaymentSettingId: any, paymentCode: any, urlhost: string, returnUrl: string, amazonOrderReferenceId: string ) {
        var amazonPayDetails = [];
        var submitPaymentViewModel = this.GetModelForConvertToOrderWithAmazonPay(PaymentSettingId, paymentCode, urlhost, returnUrl, amazonOrderReferenceId)

        $.ajax({
            type: "POST",
            url: "/Quote/ConvertQuoteToOrder",
            data: submitPaymentViewModel,
            async: false,
            success: function(response) {
                if (response.error != null && response.error != "" && response.error != 'undefined') {
                    Quote.prototype.ClearPaymentAndDisplayMessage(response.error);
                    Checkout.prototype.HideAmazonPaymentProcessDialog();
                    $("#div-PaypalExpress").hide();
                    return false;
                }
                else if (response.responseText != null && response.responseText != "" && response.responseText != 'undefined') {
                    $("#div-PaypalExpress").hide();
                    if (response.responseText != undefined && response.responseText.indexOf('Message=') >= 0) {
                        Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("SelectCOD"));
                    }
                    else if (response.responseText == "True") {
                        window.location.href = returnUrl + "&captureId=" + response.responseToken + "&orderNumber=" + response.orderNumber;
                    }
                    else {
                        Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    }
                } else {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessPayment"));
                    Checkout.prototype.HideAmazonPaymentProcessDialog();
                    $("#div-PaypalExpress").hide();
                    return false;
                }
            },
            error: function() {
                Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                Checkout.prototype.HideAmazonPaymentProcessDialog();
                return false;
            }
        });
        return amazonPayDetails;
    }

    private GetModelForConvertToOrderWithAmazonPay(PaymentSettingId: any, paymentCode: any, urlhost: string, returnUrl: string, amazonOrderReferenceId: string ) {
        var quoteId = $("#QuoteId").val();
        var cancelUrl = urlhost + "/quote/GetQuote?omsQuoteId=" + quoteId;
        return {
            OmsQuoteId: $("#QuoteId").val(),
            UserId: $("#hdnUserId").val(),
            Total: $("#Total").val(),
            SubTotal: $('#SubTotal').val(),
            PaymentDetails: {
                PaymentSettingId: PaymentSettingId,
                PaymentCode: paymentCode,
                AmazonPayReturnUrl: returnUrl,
                AmazonPayCancelUrl: cancelUrl,
                AmazonOrderReferenceId: amazonOrderReferenceId,
                PaymentType: "AmazonPay",
                IsFromAmazonPay: true,
            }
        };
    }

    SubmitBraintreeQuote(payload,isVault) {
        $('#BraintreeSubmitButton').prop("disabled", true);
        $('#BraintreeCancelButton').prop("disabled", true);
        var cardDetails = payload.details;
        $('#hdnBraintreecardNumber').val(cardDetails.lastFour);
        $("#hdnBraintreeCardExpirationMonth").val(cardDetails.expirationMonth);
        $("#hdnBraintreeCardExpirationYear").val(cardDetails.expirationYear);
        $("#hdnBraintreeCardHolderName").val(cardDetails.cardholderName);
        $("#hdnBraintreeCardType").val(cardDetails.cardType);
        $("#hdnBraintreeNonce").val(payload.nonce);
        $("#hdnBraintreecode").val(Constant.BrainTree);
        $("#hdnBraintreeIsVault").val(isVault);
        Quote.prototype.ConvertQuoteToOrder();
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
}