class Payment extends ZnodeBase {

    constructor() {
        super();
    }
    Init(): void {
        Payment.prototype.OnLoad();
    }

    OnLoad(): void {
        $(document).off("change", "#ddlPaymentTypes").on("change", "#ddlPaymentTypes", function () {
            Payment.prototype.GetPaymentTypesForm($('#ddlPaymentTypes option:selected').attr('data-type'), null);
        });

        let paymentType: string = $('#ddlPaymentTypes option:selected').attr('data-type').toLowerCase();
        Payment.prototype.SetGatewayMode();
        if (paymentType == "credit_card") {
            Payment.prototype.DisableCybersource();
            $(document).off("change", "#ddlPaymentGetways").on("change", "#ddlPaymentGetways", function () {
                Payment.prototype.DisableCybersource();
                Payment.prototype.SetGatewayMode();
                Payment.prototype.GetPaymentGetwayForm($('#ddlPaymentGetways').val(), null);
            });
        }

        if (paymentType == "credit_card" || paymentType == "paypal_express" || paymentType == "amazon_pay") {
            $(document).off("change", "#ddlTestMode").on("change", "#ddlTestMode", function () {
                Payment.prototype.GetPaymentSettingCredentials();
            });
        }

        $(document).off("change", "#IsPoDocUploadEnable").on("change", "#IsPoDocUploadEnable", function () {
            Payment.prototype.ToggleIsPODocRequired();
        });

        $(document).off("change", "#IsPoDocRequire").on("change", "#IsPoDocRequire", function () {
            var IsPODocRequired = $("#IsPoDocRequire");
            IsPODocRequired.is(":checked") ? IsPODocRequired.val("true") : IsPODocRequired.val("false");
        });

        if (paymentType == "purchase_order") {
            $("#IsPoDocUploadEnable").trigger("change");
            $("#IsPoDocRequire").trigger("change");
        }

        Payment.prototype.PaymentCodeValidation();
        Payment.prototype.PaymentDisplayNameValidation();
    }

    DisableCybersource() {
        if ($("#ddlPaymentGetways").val() == 'cybersource') {
            $('#IsActive').prop('checked', false)
            $('#IsActive').prop('disabled', true)
        }
        else {
            $('#IsActive').prop('checked', true)
            $('#IsActive').prop('disabled', false)
        }
    }


    GetPaymentTypesForm(paymentName: string, paymentSettingModel: Object): void {
        let payment_Name: string = $("#PaymentTypeCode").val();//"PaymentTypeCode" has the Payment name.
        if (payment_Name != undefined && payment_Name != "") {
            $("#ddlPaymentTypes").val(payment_Name);
            paymentName = $('#ddlPaymentTypes option:selected').attr('data-type');
        }
        let paymentCode: string = $('#ddlPaymentTypes option:selected').val();
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetPaymentTypeForm(paymentName, paymentSettingModel, paymentCode, function (res) {
            $("#paymenttypeform-container").show();
            $("#paymenttypeform-container").html(res);
            let gatewayCode: string = $('#ddlPaymentGetways').val();
            if (gatewayCode != undefined && gatewayCode != "") {
                Payment.prototype.GetPaymentGetwayForm($('#ddlPaymentGetways').val(), null);
            }
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidatePayment(): boolean {
        var IsValid: boolean = true;
        IsValid = Payment.prototype.IsCardTypeSelected();
        if (IsValid == true) {
            IsValid = Payment.prototype.ValidatePaymentCode();
        }
        if (IsValid) {
            $("#PaymentTypeCode").val($("#ddlPaymentTypes option:selected").val());
            $("#GatewayCode").val($("#ddlPaymentGetways option:selected").val());
        }
        return IsValid;
    }

    IsCardTypeSelected(): boolean {
        $("#paymentcardtype").show();
        var paymentType = $('#ddlPaymentTypes option:Selected').val().toLowerCase();
        if (paymentType == "creditcard") {
            var gatewayCode = $('#ddlPaymentGetways').val();
            if (gatewayCode == "cybersource" || gatewayCode == "authorizenet" || gatewayCode == "braintree") {
                $("#paymentcardtype").hide();
                return true;
            }

            if ($("#EnableVisa").is(":checked") == false
                && $("#EnableMasterCard").is(":checked") == false
                && $("#EnableAmericanExpress").is(":checked") == false
                && $("#EnableDiscover").is(":checked") == false) {
                $("#AcceptedCardsValidation").show();
                return false;
            }
        }
        return true;

    }

    IsMatchRegularExpressionString(str: string, regax: string): boolean {
        if (str.match(regax)) {
            return true;
        }
        else {
            return false;
        }
    }

    GetPaymentGetwayForm(gatewayname: string, paymentSettingModel: Object): void {
        if (gatewayname != "" && gatewayname != "0") {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetPaymentGetwayForm(gatewayname, paymentSettingModel, function (res) {
                $("#PaymentGetwayForm-container").html(res);
                $("#PaymentGetwayForm-container").show();
                Payment.prototype.ResetGatewayForm();
                Payment.prototype.GetPaymentSettingCredentials();
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    GetPaymentSettingCredentials(): void {
        var paymentCode = $("#PaymentCode").val();
        if (paymentCode != undefined && paymentCode != "") {
            ZnodeBase.prototype.ShowLoader();
            var testMode = $("#ddlTestMode").val();
            var gatewayCode = $('#ddlPaymentGetways').val();
            let paymentType: string = $('#ddlPaymentTypes option:selected').attr('data-type').toLowerCase();
            let paymentTypeCode: string = $('#ddlPaymentTypes option:selected').val();
            if (paymentType == "amazon_pay") { gatewayCode = "amazon_pay";}

            Endpoint.prototype.GetPaymentSettingCredentials(paymentCode, testMode, gatewayCode, paymentTypeCode, function (response) {
                $("#PaymentGetwayForm-container").html(response);
                $("#PaymentGetwayForm-container").show();
                Payment.prototype.OnLoad();
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    DeleteMultiplePaymentSettings(control): any {
        var paymentSettingIds = MediaManagerTools.prototype.unique();
        if (paymentSettingIds.length > 0) {
            Endpoint.prototype.DeleteMultiplePaymentSettings(paymentSettingIds.join(","), function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }
    ToggleIsPODocRequired(): void {
        var divIsPODocRequired = $("#divIsPODocRequired");
        var EnablePODocUpload = $("#IsPoDocUploadEnable").is(":checked");
        if (EnablePODocUpload) {
            divIsPODocRequired.show();
            $("#IsPoDocUploadEnable").val("true");
            var IsPODocRequired = $("#IsPoDocRequire");
            if (IsPODocRequired != null && typeof IsPODocRequired != 'undefined') {
                IsPODocRequired.is(":checked") ? IsPODocRequired.val("true") : IsPODocRequired.val("false");
            }
        }
        else {
            divIsPODocRequired.hide();
            $("#IsPoDocRequire").prop("checked", false);
            $("#IsPoDocRequire").val("false");
            $("#IsPoDocUploadEnable").val("false");
        }
    }

    PaymentCodeValidation(): any {
        $('input[id$="PaymentCode"]').blur(function () {
            Payment.prototype.PaymentCodeExist();
        });
    }

    PaymentCodeExist(): void {
        Endpoint.prototype.IsPaymentCodeExist($("#PaymentCode").val(), $("#PaymentSettingId").val(), function (response) {
            if (!response) {
                Payment.prototype.SetPaymentValidation("PaymentCode", "errorSpanPaymentCode", "AlreadyExistPaymentCode");
                return false;
            }
        });
    }

    PaymentDisplayNameValidation(): any {
        $('input[id$="PaymentDisplayName"]').blur(function () {
            Payment.prototype.IsDuplicatePaymentName();
        });
    }

    ShowBillingAddressOptional(): any {
        $("#divIsBillingAddressOptional").show();
    }

    IsDuplicatePaymentName(): void {
        Endpoint.prototype.IsPaymentDisplayNameExists($("#PaymentDisplayName").val(), $("#PaymentSettingId").val(), function (response) {
            if (!response) {
                Payment.prototype.SetPaymentValidation("PaymentDisplayName", "errorSpanPaymentDisplayName", "AlreadyExistPaymentDisplayName");
                return false;
            }
        });
    }

    ValidatePaymentCode(): boolean {
        var isValid: boolean = true;
        if ($("#PaymentCode").val() == '') {
            this.SetPaymentValidation("PaymentCode", "errorSpanPaymentCode", "PaymentCodeRequiredError");
            isValid = false;
        }
        else if ($("#PaymentDisplayName").val() == '') {
            this.SetPaymentValidation("PaymentDisplayName", "errorSpanPaymentDisplayName", "PaymentCodeRequiredError");
            isValid = false;
        }
        else {
            Endpoint.prototype.IsPaymentCodeExist($("#PaymentCode").val(), $("#PaymentSettingId").val(), function (response) {
                if (!response) {
                    Payment.prototype.SetPaymentValidation("PaymentCode", "errorSpanPaymentCode", "AlreadyExistPaymentCode");
                    isValid = false;
                }
            });
            Endpoint.prototype.IsPaymentDisplayNameExists($("#PaymentDisplayName").val(), $("#PaymentSettingId").val(), function (response) {
                if (!response) {
                    Payment.prototype.SetPaymentValidation("PaymentDisplayName", "errorSpanPaymentDisplayName", "AlreadyExistPaymentDisplayName");
                    isValid = false;
                }
            });
        }
        return isValid;
    }

    ResetGatewayForm(): void {
        var gatewayCode = $('#ddlPaymentGetways').val();
        if (gatewayCode == "cybersource" || gatewayCode == "authorizenet" || gatewayCode == "braintree") {
            $("#paymentcardtype").hide();
        }
        else {
            $("#paymentcardtype").show();
        }
        $("#paymentPreAuthorize").show();
        if (parseInt($("#PaymentSettingId").val()) < 1) {
            $('#PreAuthorize').attr('checked', false);
            $('#EnableVisa').attr('checked', false);
            $('#EnableMasterCard').attr('checked', false);
            $('#EnableAmericanExpress').attr('checked', false);
            $('#EnableDiscover').attr('checked', false);
        }
    }

    private SetPaymentValidation(controlId, validationControlId, errorKey): void {
        $("#" + controlId).addClass("input-validation-error");
        $("#" + validationControlId).addClass("error-msg");
        $("#" + validationControlId).text(ZnodeBase.prototype.getResourceByKeyName(errorKey));
        $("#" + validationControlId).show();
    }

    SetGatewayMode(): any {
        var gatewayCode = $('#ddlPaymentGetways').val();
        if (gatewayCode == "cybersource" || gatewayCode == "paypal") {
            $("#divGatewayMode").hide();
            $("#divGatewayModeHelpText").removeClass("d-none");           
        }
        else {
            $("#divGatewayMode").show();
            $("#divGatewayModeHelpText").addClass("d-none");           
        }
    }
}