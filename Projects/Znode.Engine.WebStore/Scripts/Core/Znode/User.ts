declare var amazon: any;
class User extends ZnodeBase {
    public isPaymentInProcess: boolean = false;
    constructor() {
        super();
    }
    Init() {
        User.prototype.RemoveIconWishlist();
        User.prototype.LoadQuote();
        User.prototype.RestrictEnterButton();
        User.prototype.BindStates(null);
        User.prototype.RemoveCaptchaValidationMessage();
    }

    RestrictEnterButton(): void {
        $('#frmUpdateQuoteQuantity').on('keyup keypress', function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });
    }

    LogOffUser(): void {
        if (typeof amazon !== 'undefined') {
            ZnodeBase.prototype.ShowLoader();
            amazon.Login.logout();
            window.setInterval(() => User.prototype.LogOff(), 1800);
            ZnodeBase.prototype.HideLoader();
        }
        else
            User.prototype.LogOff();
    }

    RemoveIconWishlist(): any {
        $("#layout-account-wishlist .wishlist-item-remove a").on("click", function (ev) {
            ev.preventDefault();
            User.prototype.RemoveWishlistItem(this);
        });
    }

    RemoveWishlistItem(el): any {
        var clicked = $(el);
        var wishlistId = clicked.data("id");
        var wishListCount = parseInt($("#wishlistcount").text());

        Endpoint.prototype.RemoveProductFromWishList(wishlistId, function (res) {
            if (res.success) {
                clicked.closest(".wishlist-item").remove();
                $("#wishlistcount").html(res.data.total);
                if (res.data.total == 0) {
                    $('#subTextWishList').text('');
                    $('#subTextWishList').text(ZnodeBase.prototype.getResourceByKeyName("MessageNoProductsInWishlist"));
                }
            }
            else {
            }
        });
    }

    UpdateQuoteStatus(control, statusId): any {
        var quoteIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (quoteIds.length > 0 && statusId > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.UpdateQuoteStatus(quoteIds, statusId, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    GenerateInvoice(): any {

        var arrIds = [];

        var ids = new Array();
        $(".grid-row-checkbox:checked").each(function () {
            ids.push($.trim($(this).attr('id').split('_')[1]));
        });

        if (ids.length > 0) {
            for (let entry of ids) {
                arrIds.push(entry.replace("rowcheck_", ""));
            }
        }

        if (arrIds != undefined && arrIds.length > 0) {
            $("#orderIds").val(arrIds);
            setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
            return true;
        }
        else {
            $("#SuccessMessage").html("");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneOrder"), "error", isFadeOut, fadeOutTime);
            return false;
        }
    }

    LoadQuote(): any {
        $("#btnBottomReview").on('click', function () {
            $("#OrderStatus").val('IN REVIEW');
        });

        $("#btnBottomApprove").on('click', function () {
            $("#OrderStatus").val('APPROVED');
        });

        $("#btnBottomReject").on('click', function () {
            $("#OrderStatus").val('REJECTED');
        });

        $("#btnTopReview").on('click', function () {
            $("#OrderStatus").val('IN REVIEW');
        });

        $("#btnTopApprove").on('click', function () {
            $("#OrderStatus").val('APPROVED');
        });

        $("#btnTopReject").on('click', function () {
            $("#OrderStatus").val('REJECTED');
        });
    }

    UpdateQuoteLineItemQuantity(control): boolean {
        var sku: string = $(control).attr("data-cart-sku")
        var minQuantity: number = parseInt($(control).attr("min-Qty"));
        var maxQuantity: number = parseInt($(control).attr("max-Qty"));
        $("#quantity_error_msg_" + sku).text('');
        var inventoryRoundOff: number = parseInt($(control).attr("data-inventoryRoundOff"));
        var selectedQty: string = $(control).val();
        var decimalPoint: number = 0;

        var decimalValue: number = 0
        if (selectedQty.split(".")[1] != null) {
            decimalPoint = selectedQty.split(".")[1].length;
            decimalValue = parseInt(selectedQty.split(".")[1]);
        }
        if (this.CheckDecimalValue(decimalPoint, decimalValue, inventoryRoundOff, sku)) {
            if (this.CheckIsNumeric(selectedQty, sku)) {
                if (this.CheckMinMaxQuantity(parseInt(selectedQty), minQuantity, maxQuantity, sku)) {
                    $(control).closest("form").submit();
                }
            }
        }
        return false;
    }

    CheckDecimalValue(decimalPoint: number, decimalValue: number, inventoryRoundOff: number, sku: string): boolean {
        if (decimalValue != 0 && decimalPoint > inventoryRoundOff) {
            $("#quantity_error_msg_" + sku).text(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityHaving") + inventoryRoundOff + ZnodeBase.prototype.getResourceByKeyName("XNumbersAfterDecimalPoint"));
            return false;
        }
        return true;
    }

    CheckIsNumeric(selectedQty: string, sku: string): boolean {
        var matches = selectedQty.match(/^-?[\d.]+(?:e-?\d+)?$/);
        if (matches == null) {
            $("#quantity_error_msg_" + sku).text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            return false;
        }
        return true;
    }
    CheckMinMaxQuantity(selectedQty: number, minQuantity: number, maxQuantity: number, sku: string): boolean {
        if (selectedQty < minQuantity || selectedQty > maxQuantity) {
            $("#quantity_error_msg_" + sku).text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + minQuantity + ZnodeBase.prototype.getResourceByKeyName("To") + maxQuantity + ZnodeBase.prototype.getResourceByKeyName("FullStop"));
            return false;
        }
        return true;
    }

    DeleteCurrentAddress(): any {
        var url = $("#deleteCurrentAddress").attr('data-url');
        $("#frmEditAddress_billing").attr('action', url);
        $("#frmEditAddress_billing").submit();
    }

    DeleteTemplate(control): any {
        var templateIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (templateIds.length > 0) {
            Endpoint.prototype.DeleteTemplate(templateIds, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    ProcessContinueOnClick(): any {
        if (parseInt($("#InventoryOutOfStockCount").val()) == parseInt($("#ShoppingCartItemsCount").val())) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("QuoteItemsOutOfStockErrorMsg"), "error", isFadeOut, 0);
            return false;
        }

        var cartItemcount = $("#CartItemCount").val();
        if (parseInt(cartItemcount) > 0) {
            var omsQuoteLineItemId = $("#omsQuoteLineItemId").val();
            $("#QuoteConfirmPopup").modal('show');
        }
        else {
            User.prototype.ProcessQuote();
        }
    }

    ProcessQuote(): any {
        $("#FormQuoteView").attr('action', "/User/ProcessQuote").submit();
    }

    DeleteQuoteLineItem(): any {
        var omsQuoteLineItemId = $("#OmsQuoteLineItemId").val();
        var omsQuoteId = $("#OmsQuoteId").val();
        var orderStatus = $("#OrderStatus").val();
        var roleName = $("#RoleName").val();
        var token = $('input[name="__RequestVerificationToken"]', $('#FormQuoteView')).val();

        Endpoint.prototype.DeleteQuoteLineItem(omsQuoteLineItemId, omsQuoteId, 1, orderStatus, roleName, token, function (res) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
            window.location.href = window.location.protocol + "//" + window.location.host + "/User/QuoteHistory";
        });
    }

    DeleteDraft(): any {
        $("#DraftConfirmPopup").modal('show');
    }

    ValidateCreateEditTemplate(): any {
        var templateName = $("#TemplateName").val();
        var isValid: boolean = true;
        if (!templateName) {
            $("#validTemplateName").html(ZnodeBase.prototype.getResourceByKeyName("RequiredTemplateName"))
            $("#validTemplateName").addClass("error-msg");
            $("#validTemplateName").show();
            isValid = false;
        }
        Endpoint.prototype.IsTemplateNameExist(templateName, $("#OmsTemplateId").val(), function (response) {
            if (!response) {
                $("#validTemplateName").html(ZnodeBase.prototype.getResourceByKeyName("TemplateNameAlreadyExist"))
                $("#validTemplateName").addClass("error-msg");
                $("#validTemplateName").show();
                isValid = false;
            }
        });
        if (isValid)
            $("#frmCreateEditTemplate").submit();
        else
            return false;
    }

    SetManageQuoteUrl(): any {
        $("#grid tbody tr td").find(".zf-view").each(function () {
            var orderStatus = $(this).attr("data-parameter").split('&')[1].split('=')[1];
            var newhref = $(this).attr("href");
            if (newhref.length > 0) {
                if (orderStatus.toLowerCase() == "ordered") {
                    var omsQuoteId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
                    newhref = window.location.protocol + "//" + window.location.host + "/User/OrderReceipt?OmsOrderId=" + omsQuoteId;
                }
                else {
                    newhref = window.location.protocol + "//" + window.location.host + newhref;
                }
            }
            $(this).attr('href', newhref);
        });
    }

    SetQuoteIdLinkURL(): any {
        $("#grid tbody tr .linkQuoteId").each(function () {
            var orderStatus = $(this).children().attr("href").split('&')[1].split('=')[1];
            var newhref = $(this).children().attr("href");
            if (newhref.length > 0) {
                if (orderStatus.toLowerCase() == "ordered") {
                    var omsQuoteId = $(this).children().attr("href").split('&')[0].split('=')[1];
                    newhref = window.location.protocol + "//" + window.location.host + "/User/OrderReceipt?OmsOrderId=" + omsQuoteId;
                }
                else {
                    newhref = window.location.protocol + "//" + window.location.host + newhref;
                }
            }
            $(this).children().attr('href', newhref);
        });
    }

    HideAddressChangeLink(): any {
        $("#FormQuoteView").find('.address-change').hide();
        $("#FormQuoteView").find('.change-address').hide();
    }

    //Saved Credit Card Region
    ShowCardPaymentOptions(customerGUID): any {
        Endpoint.prototype.GetSaveCreditCardCount(customerGUID, function (count) {
            $("#creditCardCount").html($("#creditCardCount").html().replace("0", count.toString()));
        });
    }

    ShowPaymentOptions(data, CustomerPaymentGUID, isAccount): any {
        Endpoint.prototype.GetPaymentDetails(data, true, function (response) {
            if (!response.HasError) {
                Endpoint.prototype.GetSaveCreditCardCount(CustomerPaymentGUID, function (count) {
                    $("#creditCardCount").html($("#creditCardCount").html().replace("0", count.toString()));
                });
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorContactPaymentApp"), "error", false, 0);
            }
        });
    }

    HideGridColumnForPODocument(isEnableReturnRequest): void {
        //Hide return icon if create return is disable for store
        if (!isEnableReturnRequest) {
            $('#grid .zf-return').hide();
        }
        //Hide PODocument path column
        $('#grid tbody tr').each(function () {
            var podoc = $(this).find('td').last();
            if (podoc.hasClass('z-podocument')) {

                //Get the PoDocument path if exist and create a hyper link to download PODocument.
                var filePath = podoc.text();

                if (filePath != "" && typeof filePath != "undefined") {
                    $(this).find('td').each(function () {
                        if ($(this).hasClass("z-paymenttype")) {
                            if ($(this).text().toLocaleLowerCase() == "purchase_order") {
                                $(this).text("");
                                $(this).append($('<div>').html("<a href='" + podoc.text() + "' target='_blank'>Purchase Order</a>"));
                            }
                        }
                    })
                }
            }
        })
    }

    RemoveCaptchaValidationMessage(): any {
        $("#CaptchaInputText").on("keyup keypress", function () {
            $("#CaptchaInputText").addClass("input-validation-valid").removeClass("input-validation-error");
            $("#CaptchaInputText").next().next("span").attr("class", "field-validation-valid");
            $("#CaptchaInputText").next().next("span").html("");
        });
    }


    public PrintOrderDetails(e): void {
        var printContents = $("#userorderdetails").html();
        var originalContents = document.body.innerHTML;
        var orderNumber = $("#OrderNumber").val();
        var emailAddress = $("#EmailAddress").val();
        document.body.innerHTML = printContents;

        window.print();

        document.body.innerHTML = originalContents;
        $("#OrderNumber").val(orderNumber);
        $("#EmailAddress").val(emailAddress);
        $.validator.unobtrusive.parse($("#frmOrderDetails"));
    }

    public LoginMethod(): void {
        let actualurl: string = window.location.href;
        //If actual url does not contain return url then only append return url.
        if (actualurl.indexOf("returnUrl") == -1) {
            actualurl = decodeURIComponent(actualurl);

            let returnUrl: string = decodeURIComponent(actualurl.replace(document.location.origin, ''));
            returnUrl = encodeURIComponent(returnUrl);
            if (returnUrl != "/User/Login")
                window.location.href = window.location.protocol + "//" + window.location.host + '/User/Login?returnUrl=' + returnUrl;
        }
        else
            window.location.href = window.location.protocol + "//" + window.location.host + '/User/Login';
    }

    public AppendLoaderOnSubmit(): void {
        $("#error-content").empty();
        if ($("#login_password").val() != "" && $("#login_username").val() != "") {
            if ($(".field-validation-error").eq(0).html() == "") {
                ZnodeBase.prototype.ShowLoader();
            }           
        }
        ZnodeBase.prototype.HideLoader();
        $("#valueCaptchaError").html("");  
    }

    BindAddressModel(addressType): Znode.Core.AddressModel {
        var stateName = $("#frmEditAddress_" + addressType).find('#txtStateCode[disabled]').length > 0 ? $("#frmEditAddress_" + addressType).find("#SelectStateName option:selected").val() : $("#frmEditAddress_" + addressType).find("#txtStateCode").val();
        var _addressModel: Znode.Core.AddressModel = {
            Address1: $("#frmEditAddress_" + addressType).find("input[name=Address1]").val(),
            Address2: $("#frmEditAddress_" + addressType).find("input[name=Address2]").val(),
            AddressId: parseInt($("#frmEditAddress_" + addressType).find("#AddressId").val()),
            CityName: $("#frmEditAddress_" + addressType).find("input[name=CityName]").val(),
            FirstName: $("#frmEditAddress_" + addressType).find("input[name=FirstName]").val(),
            LastName: $("#frmEditAddress_" + addressType).find("input[name=LastName]").val(),
            PostalCode: $("#frmEditAddress_" + addressType).find("input[name=PostalCode]").val().replace(/ /g, ""),
            StateName: stateName,
            CountryName: $("#frmEditAddress_" + addressType).find('select[name="CountryName"]').val(),
            AddressType: addressType,
            PhoneNumber: $("#frmEditAddress_" + addressType).find("input[name=PhoneNumber]").val(),
            EmailAddress: $("#frmEditAddress_" + addressType).find("input[name=EmailAddress]").val(),
            AspNetUserId: $("#frmEditAddress_" + addressType).find("input[name=AspNetUserId]").val(),
            UserId: $("#frmEditAddress_" + addressType).find("input[name=UserId]").val()
        };
        return _addressModel;
    }

    public ValidateAddressForm(addressType): boolean {
        var _addressType = $("#frmEditAddress_" + addressType);
        var isValid = false;
        var postalCode = _addressType.find('#address_postalcode').val().replace(/ /g, "");
        $("input[name=PostalCode]").val(postalCode);        
        if (User.prototype.IsValidZipCode(postalCode, _addressType)) {
            _addressType.find('#valid-postalcode').hide();
            isValid = true;
        } else {
            _addressType.find('#valid-postalcode').show();
            isValid = false;
        }
        return isValid;
    }


    public IsValidZipCode(zipCode, _addressType): boolean {
        var countryCode = _addressType.find('#ShippingAddressModel_CountryCode').val();
        //Currently few country regex available.If want to validate for other country add regex in 'ZipCodeRegex'
        var zipCodeRegexp = ZipCodeRegex[countryCode];
        if (zipCodeRegexp) {
            var regexp = new RegExp(zipCodeRegexp);
            return regexp.test(zipCode);
        }
        return true;
    }

    public SaveChanges(event, id, addressType): any {
        event ? event.preventDefault() : "";
        if (id != "" && typeof id != "undefined" && id != null) {
            $("#frmEditAddress_" + addressType).find("input[name=Address1]").val($("#recommended-address1-" + id + "").text());
            $("#frmEditAddress_" + addressType).find("input[name=Address2]").val($("#recommended-address2-" + id + "").text());
            $("#frmEditAddress_" + addressType).find("input[name=CityName]").val($("#recommended-address-city-" + id + "").text());
            $("#frmEditAddress_" + addressType).find("input[name=PostalCode]").val($("#recommended-address-postalcode-" + id + "").text());
            $("#frmEditAddress_" + addressType).find('#txtStateCode[disabled]').length > 0 ? $("#frmEditAddress_" + addressType).find("select[name=StateName]").val($("#recommended-address-state-" + id + "").text()) : $("#frmEditAddress_" + addressType).find("input[name=StateName]").val($("#recommended-address-state-" + id + "").text());
            $("#formChange").val("true");
        }
        $('#custom-modal').modal('hide');
        $("#frmEditAddress_" + addressType).find("#btnSaveAddress").closest("form").submit();
        return true;
    }

    public RecommendedAddress(addressType): boolean {
        if (!$("#frmEditAddress_" + addressType).valid())
            return false;

        if (!User.prototype.ValidateAddressForm(addressType))
            return false;
        
        ZnodeBase.prototype.ShowLoader();
        var addressModel = User.prototype.BindAddressModel(addressType);
        let isSuggestedAddress: boolean = false;
        Endpoint.prototype.GetRecommendedAddress(addressModel, function (response) {
            var htmlString = response.html;
            if (htmlString != "" && typeof htmlString != "undefined" && htmlString != null) {
                $('#custom-modal').find('#custom-content').empty();
                $('#custom-modal').find('#custom-content').append(htmlString);

                $("#user-entered-address").empty();
                let enteredaddress: string = "<div class='address-street'><div id='enteredAddress1'>" + addressModel.Address1 + "</div>";
                if (addressModel.Address2 != "" && typeof addressModel.Address2 != "undefined" && addressModel.Address2 != null) {
                    enteredaddress += "<div id='enteredAddress2'>" + addressModel.Address2 + "</div> ";
                }
                enteredaddress += "<div class='address-citystate'><span id='enteredCity'>" + addressModel.CityName + "</span> <span id='enteredState'>" + addressModel.StateName + "</span> <span id='enteredPostalCode'>" + addressModel.PostalCode + "</span> <div id='enteredCountry'>" + addressModel.CountryName + "</div></div>";

                $("#user-entered-address").append(enteredaddress);
                User.prototype.ShowHideRecommendedPopUp(addressType);

                ZnodeBase.prototype.HideLoader();
                isSuggestedAddress = false;
                $(".address-popup").modal("hide");
            }
            else {
                isSuggestedAddress = true;
            }
        });
        return isSuggestedAddress;
    }

    //If the recommended address matches completely then it will hide popup and save the address.
    public ShowHideRecommendedPopUp(addressType): void {
        let isShowRecommendedAddress: boolean = true;
        isShowRecommendedAddress = User.prototype.MatchAddress();
        if (isShowRecommendedAddress) {
            $('#custom-modal').empty();
            return User.prototype.SaveChanges(null, null, addressType);
        }
        $('#custom-modal').modal('show');
    }

    //Match entered address with recommended address.
    public MatchAddress(): boolean {
        let isMatchedAddress: boolean = true;
        for (var i = 1; i < $("#custom-modal .address-details").length; i++) {
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredAddress1", "#recommended-address1-" + i, isMatchedAddress);
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredAddress2", "#recommended-address2-" + i, isMatchedAddress);
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredCity", "#recommended-address-city-" + i, isMatchedAddress);
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredState", "#recommended-address-state-" + i, isMatchedAddress);
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredCountry", "#recommended-address-country-" + i, isMatchedAddress);
            isMatchedAddress = User.prototype.ValidateRecommendedAddress("#enteredPostalCode", "#recommended-address-postalcode-" + i, isMatchedAddress);
        }
        return isMatchedAddress;
    }

    public ValidateRecommendedAddress(selector, recommendedAddressSelector, isMatchedAddress): boolean {
        if (!($(selector).text().trim().toLowerCase() == $(recommendedAddressSelector).text().trim().toLowerCase())) {
            $(recommendedAddressSelector).addClass("address-error");
            isMatchedAddress = false;
        }
        return isMatchedAddress;
    }

    public HideShowAddressPopUP(): void {
        $("#AddressError").html("")
        $("#custom-modal").modal("hide");
    }

    OnUserTypeSelection(): any {
        var selectedRole = $("#ddlUserType option:selected").text();
        if (selectedRole == null && selectedRole == "") {
            $('#ddlRole').children('option:not(:first)').remove();
            $('#divRole').hide();
            return false;
        }

        if (selectedRole == "User") {
            $('#divRole').show();
            Endpoint.prototype.GetPermissionList($("#AccountId").val(), $("#AccountPermissionAccessId").val(), function (response) {
                $('#permission_options').html("");
                $('#permission_options').html(response);
                $("#ddlPermission").attr("onchange", "User.prototype.OnPermissionSelection();");
            });
            $("#ddlPermission").change();
        }
        else {
            $('#divRole').hide();
            $('#approvalNamesDiv').hide();
            $('#maxBudgetDiv').hide();
            $("#BudgetAmount").val("");
        }
    }

    OnPermissionSelection(): any {
        var permission = $("#ddlPermission option:selected").attr('data-permissioncode');
        var $sel = $("#divRole");
        var value = $sel.val();
        var text = $("option:selected", $sel).text();
        $('#PermissionCode').val(permission);
        $('#PermissionsName').val(text);
        if (permission != undefined && permission == 'ARA') {
            User.prototype.ShowApprovalList();
            $('#maxBudgetDiv').hide();
        }
        else if (permission != undefined && permission == 'SRA') {
            User.prototype.ShowApprovalList();
            $('#maxBudgetDiv').show();
        }
        else {
            $('#approvalNamesDiv').hide();
            $('#maxBudgetDiv').hide();
            $("#BudgetAmount").val("");
        }
    }

    OnUserProfileSelection(): any {
        var profileId = $("#ddlUserProfile option:selected").val();
        if (profileId != undefined && profileId > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.ChangeUserProfile(profileId, function (response) {
                if (response.status) {
                    window.location.reload();
                }
            });
        }
    }

    ShowApprovalList(): any {
        var accountId = $("#AccountId").val();
        var userId = parseInt($("#UserId").val(), 10);
        Endpoint.prototype.GetApproverList(accountId, userId, function (response) {
            var approvalUserId = $("#ApprovalUserId").val();
            $("#ddlApproverList").html("");
            $('#ddlApproverList').find('option').remove().end();
            $('#ddlApproverList').children('option:not(:first)').remove();
            for (var i = 0; i < response.length; i++) {
                if (response[i].Value == approvalUserId)
                    var opt = new Option(response[i].Text, response[i].Value, false, true);
                else
                    var opt = new Option(response[i].Text, response[i].Value);

                $('#ddlApproverList').append(opt);
            }
            $('#approvalNamesDiv').show();
        });
    }

    ValidateUserNameExists(): boolean {
        if ($("#divAddCustomerAsidePanel #UserName").val() != '') {
            Endpoint.prototype.IsUserNameExist($("#divAddCustomerAsidePanel #UserName").val(), $("#PortalId").val(), function (response) {
                if (!response) {
                    $("#UserName").addClass("input-validation-error");
                    $("#errorUserName").addClass("error-msg");
                    $("#errorUserName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistUserName"));
                    $("#errorUserName").show();
                    $("#loading-div-background").hide();
                    return false;
                }
            });
        }
        return User.prototype.ValidateBudgetAmount();
    }

    ValidateBudgetAmount(): boolean {
        if ($("#BudgetAmount").is(':visible')) {
            if ($("#BudgetAmount").val() == null || $("#BudgetAmount").val() == "") {
                $("#errorRequiredAccountPermissionAccessId").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorBudgetAmount")).addClass("field-validation-error").show();
                $("#BudgetAmount").addClass('input-validation-error');
                return false;
            }
        }
        return true;
    }

    SubmitCustomerCreateEditForm() {
        return User.prototype.ValidationForUser();
    }

    ValidateAccountsCustomer(): any {
        $("#frmCreateEditCustomerAccount").submit(function () {
            return User.prototype.ValidationForUser();
        });
    }

    ValidationForUser() {
        var flag: boolean = true;
        var _AllowGlobalLevelUserCreation = $("#AllowGlobalLevelUserCreation").val();
        if (_AllowGlobalLevelUserCreation == "False" && $("#AccountName").val() == "" && $("#hdnPortalId").val() == "") {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").addClass('input-validation-error');
            flag = false;
        }

        if ($("#hdnRoleName").val() == "User") {
            if ($("#BudgetAmount").is(':visible')) {
                if ($("#BudgetAmount").val() == null || $("#BudgetAmount").val() == "") {
                    $("#errorRequiredAccountPermissionAccessId").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorBudgetAmount")).addClass("field-validation-error").show();
                    $("#BudgetAmount").addClass('input-validation-error');
                    flag = false;
                }
            }
            if ($("#ddlApproverList").is(':visible')) {
                if ($("#ddlApproverList").val() == null || $("#ddlApproverList").val() == "") {
                    $("#errorRequiredApprovalUserId").html("<span>" + ZnodeBase.prototype.getResourceByKeyName("SelectApprovalUserId") + "</span>");
                    $("#ddlApproverList").addClass('input-validation-error');
                    flag = false;
                }
            }
        }
        if (!$("#BudgetAmount").is(':visible')) {
            $("#BudgetAmount").val("");
        }
        if ($("#Email").is(':visible') && $("#Email").val() == '') {
            $("#errorRequiredEmail").text('').text(ZnodeBase.prototype.getResourceByKeyName("EmailAddressIsRequired")).removeClass('field-validation-valid').addClass("field-validation-error").show();
            $("#Email").removeClass('valid').addClass('input-validation-error');
            flag = false;
        }
        return flag;
    }

    CancelUpload(targetDiv) {
        if ($(".add-to-cart-popover").html() != null && $(".add-to-cart-popover").html() != undefined && $(".add-to-cart-popover").html() != "")
            $(".add-to-cart-popover").remove();
        $("#" + targetDiv).hide(700);
        $("#" + targetDiv).html("");
        $("body").css('overflow', 'auto');
        User.prototype.RemovePopupOverlay();
    }

    RemovePopupOverlay(): any {
        //Below code is used to close the overlay of popup, as it was not closed in server because container is updated by Ajax call
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
        $("body").css('overflow', 'auto');
    }

    DeleteMultipleAccountCustomer(control): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.DeleteAccountCustomers(accountIds, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountUser").find("#refreshGrid"), res);
            });
        }
    }

    EnableCustomerAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerEnableDisableAccount($("#AccountId").val(), accountIds, true, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountUser").find("#refreshGrid"), res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("EnableMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisableCustomerAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerEnableDisableAccount($("#AccountId").val(), accountIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountUser").find("#refreshGrid"), res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DisableMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    CustomerResetPassword(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.CustomerAccountResetPassword($("#AccountId").val(), accountIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeAccountUser").find("#refreshGrid"), res);
                ZnodeBase.prototype.HideLoader();
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessResetPassword"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName(res.message), 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    GetUserPermissionList(): void {
        if ($("#hdnRoleName").val() == "User") {
            Endpoint.prototype.GetPermissionList($("#AccountId").val(), $("#AccountPermissionAccessId").val(), function (response) {
                $('#permission_options').html("");
                $('#permission_options').html(response);
                $("#ddlPermission").attr("onchange", "User.prototype.OnPermissionSelection();");
            });
        }
    }

    ShowHidePermissionDiv(): any {
        if ($("#hdnRoleName").val() != "User") {
            $("#permissionsToHide").hide();
        }
        else {
            $("#permissionsToHide").show();
        }
    }

    ResetPasswordCustomer() {
        var accountId = $("#AccountId").val();
        window.location.href = window.location.protocol + "//" + window.location.host + "/user/singleresetpassword?accountId=" + accountId;
    }

    ResetPasswordUsers() {
        var userId = $("#divAddCustomerAsidePanel #UserId").val();
        if (userId == undefined && $("#UserId").val() != undefined)
            userId = $("#UserId").val();
        else
            userId = User.prototype.GetUserIdFromQueryString();   //if #UserId and #divAddCustomerAsidePanel is undefine then UserId will get from query string

        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.SingleResetPassword(userId, function (res) {
            ZnodeBase.prototype.HideLoader();
            var errorType = 'error';
            if (res.status) {
                errorType = 'success';
            }
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, errorType, isFadeOut, fadeOutTime);
        });
    }

    //Get User ID from Query String
    GetUserIdFromQueryString() {
        var hdnResetPasswordURL = $("#hdnResetPasswordURL").val();
        var paramStr = decodeURIComponent(hdnResetPasswordURL).substring(1);
        var paramSegs = paramStr.split('&');
        for (var i = 0; i < paramSegs.length; i++) {
            var paramSeg = paramSegs[i].split('=');
        }
        var userId = paramSeg[1];
        return userId
    }

    public BindStates(addresstype): void {
        if (addresstype == null || addresstype == '') {
            $(".addressType").each(function () {
                addresstype = $(this).val();
                if (addresstype.toLowerCase() == "shipping")
                    User.prototype.BindStatestoShippingAddress();
                else if (addresstype.toLowerCase() == "billing")
                    User.prototype.BindStatestoBillingAddress();
                else {
                    let countryCode: string = ($('select[name="CountryName"]').val() != undefined) ? $('select[name="CountryName"]').val() : $('select[name="Address.CountryName"]').val();
                    if (countryCode.toLowerCase() != '' && countryCode.toLowerCase() != undefined) {
                        Endpoint.prototype.GetStates(countryCode, function (response) {
                            var stateName = $('#SelectStateName');
                            stateName.empty();
                            $("#txtStateCode").attr("disabled", "disabled");
                            $("#txtStateCode").val('');
                            $.each(response.states, function (key, value) {
                                stateName.append('<option value="' + value.Value + '">' + value.Text + '</option>');
                            });

                            let code: string = $("#hdn_StateCode").val();
                            $("#SelectStateName option").filter(function () {
                                return ($(this).val() == code);
                            }).prop('selected', true);
                        });
                    }
                    else {
                        $("#txtStateCode").prop("disabled", false);
                    }
                    $("#dev-statecode-textbox").hide();
                    $("#dev-statecode-select").show();
                }
            });
        }
        else if (addresstype.toLowerCase() == "shipping")
            User.prototype.BindStatestoShippingAddress();
        else if (addresstype.toLowerCase() == "billing")
            User.prototype.BindStatestoBillingAddress();
    }

    BindStatestoShippingAddress(): void {
        let countryCode: string = ($("#frmEditAddress_shipping").find('select[name="CountryName"]').val() != undefined) ? $("#frmEditAddress_shipping").find('select[name="CountryName"]').val() : $("#frmEditAddress_shipping").find('select[name="Address.CountryName"]').val();
        if (countryCode.toLowerCase() != '' && countryCode.toLowerCase() != undefined) {
            Endpoint.prototype.GetStates(countryCode, function (response) {
                var stateName = $("#frmEditAddress_shipping").find('#SelectStateName');
                stateName.empty();
                $("#frmEditAddress_shipping").find("#txtStateCode").attr("disabled", "disabled");
                $("#frmEditAddress_shipping").find("#txtStateCode").val('');
                $.each(response.states, function (key, value) {
                    stateName.append('<option value="' + value.Value + '">' + value.Text + '</option>');
                });

                let code: string = $("#frmEditAddress_shipping").find("#hdn_StateCode").val();
                $("#frmEditAddress_shipping").find("#SelectStateName option").filter(function () {
                    return ($(this).val() == code);
                }).prop('selected', true);
            });
        }
        else {
            $("#frmEditAddress_shipping").find("#txtStateCode").prop("disabled", false);
        }
        $("#frmEditAddress_shipping").find("#dev-statecode-textbox").hide();
        $("#frmEditAddress_shipping").find("#dev-statecode-select").show();
    }
    BindStatestoBillingAddress(): void {
        let countryCode: string = ($("#frmEditAddress_billing").find('select[name="CountryName"]').val() != undefined) ? $("#frmEditAddress_billing").find('select[name="CountryName"]').val() : $("#frmEditAddress_billing").find('select[name="Address.CountryName"]').val();
        if (countryCode.toLowerCase() != '' && countryCode.toLowerCase() != undefined) {
            Endpoint.prototype.GetStates(countryCode, function (response) {
                var stateName = $("#frmEditAddress_billing").find('#SelectStateName');
                stateName.empty();
                $("#frmEditAddress_billing").find("#txtStateCode").attr("disabled", "disabled");
                $("#frmEditAddress_billing").find("#txtStateCode").val('');
                $.each(response.states, function (key, value) {
                    stateName.append('<option value="' + value.Value + '">' + value.Text + '</option>');
                });
                let code: string = $("#frmEditAddress_billing").find("#hdn_StateCode").val();
                $("#frmEditAddress_billing").find("#SelectStateName option").filter(function () {
                    return ($(this).val() == code);
                }).prop('selected', true);
            });
        }
        else {
            $("#frmEditAddress_billing").find("#txtStateCode").prop("disabled", false);
        }
        $("#frmEditAddress_billing").find("#dev-statecode-textbox").hide();
        $("#frmEditAddress_billing").find("#dev-statecode-select").show();
    }

    SetPrimaryAddress(event: any, type: string): any {
        var selectedAddressId: number = event.value;
        User.prototype.ShowLoader();
        Endpoint.prototype.SetPrimaryAddress(selectedAddressId, type, function (response) {
            if (type == "shipping") {
                $("#defaultShippingAddressDiv").html(response.html);
            }
            if (type == "billing") {
                $("#defaultBillingAddressDiv").html(response.html);
            }
            User.prototype.HideLoader();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? "success" : "error", isFadeOut, fadeOutTime);
        });
    }

    GetUserApproverList(omsQuoteId: any): any {
        Endpoint.prototype.GetUserApproverList(omsQuoteId, function (response) {
            $("#user-approver-popup-content").html(response);
        });
    }

    public LoginInPopup(): void {
        let actualurl: string = window.location.href;
        let returnUrl: string;
        if ($("#returnUrl").val() != undefined && $("#returnUrl").val() != null) {
            returnUrl = $("#returnUrl").val();
        }
        //If actual url does not contain return url then only append return url.
        if (actualurl.indexOf("returnUrl") == -1) {
            actualurl = decodeURIComponent(actualurl);
            returnUrl = decodeURIComponent(actualurl.replace(document.location.origin, ''));
            if (returnUrl == "/User/Login")
                returnUrl = "";
        }
        Endpoint.prototype.Login(returnUrl, function (response) {
            $("#sign-in-nav").html(response);
            $(".accountmenus").addClass("OpenNav");
            $(".loginNow").addClass("OpenNav");
        });
    }

    public GetAccountMenus(): void {
        Endpoint.prototype.GetAccountMenus(function (response) {
            ZnodeBase.prototype.HideLoader();
            $("#accountMenusDiv").html(response);
            $("#accountMenusDiv .dropdown-menu").attr("style", "display:block");
            $("#accountMenusDiv .dropdown-menu").show();
        });
    }

    public ForgotPassword(): void {
        Endpoint.prototype.ForgotPassword(function (response) {
            $("#sign-in-nav").html(response);
        });
    }

    public GetResult(data): void {
        if (data.status == false || data.status == undefined || data.status == "undefined") {
            if (data.error == '' || data.error == undefined || data.error == "undefined") {
                $("#sign-in-nav").html(data);
                $("#CaptchaInputText").addClass("input-validation-error");
                $("#CaptchaInputText").next().next("span").attr("class", "field-validation-error");
                $("#CaptchaInputText").val("");
            }
            $("#error-content").html(data.error);
            $("#login_password").val("");
            $("#div-captcha").html(data.captchaHtml);
            ZnodeBase.prototype.HideLoader();
        }
        else if (data.status == true) {
            if (data.hasOwnProperty('isResetPassword') && data.isResetPassword == true) {
                window.location.href = "/User/ResetWebstorePassword";
            }
            else if (data.link != null) {
                if (data.link == "/User/Wishlist") {
                    localStorage.setItem("Status", data.status)
                    window.location.reload();
                }
                if (data.link !== null && data.link !== '') {
                    if (data.link.indexOf(window.location.origin) >= 0)
                        window.location.href = data.link;
                    else if (data.link.match("^/")) {
                        window.location.href = window.location.origin + data.link;
                    }
                    else {
                        if (data.link.indexOf('/') == 0)
                            window.location.href = window.location.origin + data.link;
                        else
                            window.location.href = window.location.origin + '/' + data.link;
                   }
                }
                else
                    window.location.href = window.location.pathname;
            }
            else if (window.location.href.indexOf('/User/signup') >= 0) {
                window.location.href = '/';
            }
            else {
                window.location.reload();
            }
        }
        else {
            if (window.location.href.indexOf('/User/signup') >= 0) {
                window.location.href = '/';
            }
            else {
                window.location.reload();
            }
        }
    }

    public LogOff(): void {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.Logoff(function (reponse) {
            window.location.href = window.location.protocol + "//" + window.location.host;
            ZnodeBase.prototype.HideLoader();
        });
    }

    public RedirectToLogin(data): void {
        $("#sign-in-nav").html(data);
        ZnodeBase.prototype.HideLoader();
    }

    public RemoveValidationMessage(addressType): void {
        var _addressType = $("#frmEditAddress_" + addressType);
        _addressType.find('#valid-postalcode').hide();
    }

    public LoginOnPasswordReset(): void {
        ZnodeBase.prototype.HideLoader();
        window.location.reload();
    }

    AddToCartOnCreateTemplate(): any {
        var flag = true;
        var cartItemCount = $("#hdnTemplateCartItemCount").val();
        var templateId = $("#OmsTemplateId").val();
        if (cartItemCount > 0) {
            if (templateId > 0) {
                ZnodeBase.prototype.ShowLoader();
                Endpoint.prototype.IsTemplateItemsModified(templateId, function (response) {
                    ZnodeBase.prototype.HideLoader();
                    if (response.status == true) {
                        flag = true;
                        window.location.href = "/User/AddTemplateToCart?omsTemplateId=" + templateId;
                    }
                    else {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSaveOrderTemplate"), "error", false, 0);
                        flag = false;
                    }

                });
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSaveOrderTemplate"), "error", false, 0);
                flag = false;
            }
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorAtLeastOneProductTemplate"), "error", false, 0);
            flag = false;
        }
        return flag;
    }

    public IsInvoicePaymentValid(): boolean {
        var isValid: boolean = true;
        var paymentOptionValue = $("input[name='PaymentOptions']:checked").val();

        if (paymentOptionValue == null || paymentOptionValue == "") {
            isValid = false;
            $("#errorPayment").html(ZnodeBase.prototype.getResourceByKeyName("SelectPaymentOption"));
            Checkout.prototype.HidePaymentLoader();
        }
        return isValid;
    }

    PayInvoice(): any {

        var paymentCode = $('#hdnGatwayName').val();

        if (paymentCode == Constant.CyberSource) {
            if ($('ul#creditCardTab ').find('li').find('a.active').attr('href') == "#savedCreditCard-panel" && $('ul#creditCardTab ').find('li.active').find('a').attr('href') == "#savedCreditCard-panel") {

                User.prototype.SubmitCyberSourcePayment("");

            }
            else {
                $("#pay-button").click();
            }
        }
        else {
            User.prototype.PayInvoiceManagement();
        }
    }
    

    PayInvoiceManagement(): any {
        if (!User.prototype.IsInvoicePaymentValid()) {
            User.prototype.isPaymentInProcess = false;
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
                case "ach":
                    User.prototype.SubmitACHPayment();
                    break;
                case "credit_card":
                    User.prototype.SubmitPayment();
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
                        var cardType = $("#hdnGatwayName").val() == "cardconnect" ? Checkout.prototype.DetectCardTypeForCardConnect(cardNumber) : $("#hdnGatwayName").val() === Constant.BrainTree ? $('#hdnBraintreeCardType').val() : Checkout.prototype.DetectCardType(cardNumber);
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
                        User.prototype.ValidatePaymentAndPayInvoice(payment, paymentSettingId, paymentCode, gatewayCode);
                    }
                });
            }

        }
    }
    SubmitCyberSourcePayment(querystr: any): any {
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
                        var payment = Quote.prototype.GetPaymentModel(guid, gatewayCode, BillingCity, BillingCountryCode, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingPostalCode, BillingStateCode, BillingStreetAddress1, BillingStreetAddress2, BillingEmailId, ShippingCost, ShippingCity, ShippingCountryCode, ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingPostalCode, ShippingStateCode, ShippingStreetAddress1, ShippingStreetAddress2, ShippingEmailId, SubTotal, Total, discount, cardNumber, CustomerPaymentProfileId, CustomerProfileId, CardDataToken, "", paymentSettingId, IsAnonymousUser, paymentCode, orderNumber, cardExpirationYear, cardExpirationMonth, cardHolderName);

                        User.prototype.ValidatePaymentAndPayInvoiceCyberSource(payment, paymentSettingId, paymentCode, gatewayCode, querystr);

                    }
                });
            }

        }
    }
    //Submit AuthorizeNet Payment 
    SubmitAuthorizeNetPayment(querystr: any): any {
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
            var submitPaymentViewModel = User.prototype.GetAuthorizeNetPaymentModel(paymentType, transactionId, creditCardNumber, orderInvoiceNumber);
            var token = $("[name='__RequestVerificationToken']").val();
                $.ajax({
                    type: "POST",
                    url: "/user/PayInvoice",
                    async: true,
                    data: submitPaymentViewModel,
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
                            window.location.href = "/User/OrderReceiptForOfflinePayment?OmsOrderId=" + $("#OmsOrderId").val();;
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
        else
            ZnodeBase.prototype.HideLoader();
    }

    //GetAuthorizeNetPaymentModel
    GetAuthorizeNetPaymentModel(paymentType: any, transactionId: any, creditCardNumber: any,
        orderInvoiceNumber: any) {
        return {
            OmsOrderId: $("#OmsOrderId").val(),
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
                        User.prototype.ValidatePaymentAndPayInvoiceACH(payment, paymentSettingId, paymentCode, gatewayCode);
                    }
                });
            }

        
    }

    ValidatePaymentAndPayInvoice(payment: { "GUID": any; "GatewayType": any; "BillingCity": any; "BillingCountryCode": any; "BillingFirstName": any; "BillingLastName": any; "BillingPhoneNumber": any; "BillingPostalCode": any; "BillingStateCode": any; "BillingStreetAddress1": any; "BillingStreetAddress2": any; "BillingEmailId": any; "ShippingCost": any; "ShippingCity": any; "ShippingCountryCode": any; "ShippingFirstName": any; "ShippingLastName": any; "ShippingPhoneNumber": any; "ShippingPostalCode": any; "ShippingStateCode": any; "ShippingStreetAddress1": any; "ShippingStreetAddress2": any; "ShippingEmailId": any; "SubTotal": any; "Total": any; "Discount": any; "PaymentToken": any; "CardNumber": any; "CardExpirationMonth": any; "CardExpirationYear": any; "GatewayCurrencyCode": any; "CustomerPaymentProfileId": any; "CustomerProfileId": any; "CardDataToken": any; "CardType": any; "PaymentSettingId": any; "IsAnonymousUser": boolean; "IsSaveCreditCard": boolean; "CardHolderName": any; "CustomerGUID": any; "PaymentCode": any; "OrderId": any; }, paymentSettingId: any, paymentCode: any, gatewayCode: any) { 
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
                    var submitPaymentViewModel = User.prototype.GetSubmitPaymentViewModel(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    $.ajax({
                        type: "POST",
                        url: "/user/PayInvoice",
                        async: true,
                        data: submitPaymentViewModel,
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
                                window.location.href = "/User/OrderReceiptForOfflinePayment?OmsOrderId=" + $("#OmsOrderId").val();;
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
    } ValidatePaymentAndPayInvoiceCyberSource(payment: { "GUID": any; "GatewayType": any; "BillingCity": any; "BillingCountryCode": any; "BillingFirstName": any; "BillingLastName": any; "BillingPhoneNumber": any; "BillingPostalCode": any; "BillingStateCode": any; "BillingStreetAddress1": any; "BillingStreetAddress2": any; "BillingEmailId": any; "ShippingCost": any; "ShippingCity": any; "ShippingCountryCode": any; "ShippingFirstName": any; "ShippingLastName": any; "ShippingPhoneNumber": any; "ShippingPostalCode": any; "ShippingStateCode": any; "ShippingStreetAddress1": any; "ShippingStreetAddress2": any; "ShippingEmailId": any; "SubTotal": any; "Total": any; "Discount": any; "PaymentToken": any; "CardNumber": any; "CardExpirationMonth": any; "CardExpirationYear": any; "GatewayCurrencyCode": any; "CustomerPaymentProfileId": any; "CustomerProfileId": any; "CardDataToken": any; "CardType": any; "PaymentSettingId": any; "IsAnonymousUser": boolean; "IsSaveCreditCard": boolean; "CardHolderName": any; "CustomerGUID": any; "PaymentCode": any; "OrderId": any; }, paymentSettingId: any, paymentCode: any, gatewayCode: any, querystr: any) {
        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
        var creditCardNumber: string = $('#CredidCardNumber').val();
        $("#div-CreditCard").hide();
        var paymentOptionId: string = $("input[name='PaymentOptions']:checked").attr("id");
        var paymentType = Checkout.prototype.GetPaymentType(paymentOptionId);

        Quote.prototype.ClosePopup();
        var submitPaymentViewModel = User.prototype.GetSubmitPaymentViewModelCybersource(paymentSettingId, paymentCode, paymentType, creditCardNumber, querystr);
        $.ajax({
            type: "POST",
            url: "/user/PayInvoice",
            async: true,
            data: submitPaymentViewModel,
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
                    window.location.href = "/User/OrderReceiptForOfflinePayment?OmsOrderId=" + $("#OmsOrderId").val();;
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
                    var submitPaymentViewModel = User.prototype.GetSubmitPaymentViewACHModel(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    $.ajax({
                        type: "POST",
                        url: "/user/PayInvoice",
                        async: true,
                        data: submitPaymentViewModel,
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
                                window.location.href = "/User/OrderReceiptForOfflinePayment?OmsOrderId=" + $("#OmsOrderId").val();;
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
                    Quote.prototype.PaymentFailedProcess(response, gatewayCode);
                }
            }
        });
    }

    GetSubmitPaymentViewModel(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsOrderId: $("#OmsOrderId").val(),
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
                PaymentGatewayId: $("#hdnPaymentGatewayId").val(),
                PaymentGatewayName: $("#hdnGatwayName").val()
            }
        };
    } GetSubmitPaymentViewModelCybersource(paymentSettingId: any, paymentCode: any, paymentType: string, creditCardNumber: string, querystr: string) {
        return {
            OmsOrderId: $("#OmsOrderId").val(),
            UserId: $("#hdnUserId").val(),
            PaymentDetails: {
                PaymentSettingId: paymentSettingId,
                PaymentCode: paymentCode,
                CustomerProfileId: $('#CustomerProfileId').val(),
                CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                CustomerShippingAddressId: $('#CustomerShippingAddressId').val(),
                CustomerGuid: $("#hdnCustomerGUID").val(),
                PaymentGUID: $("#hdnPaymentGUID").val(),
                PaymentToken: $("input[name='CCdetails']:checked").val(),
                paymentType: paymentType,
                CreditCardNumber: creditCardNumber.slice(-4),
                RemainingAmount: parseFloat($("#Total").val()),
                PaymentAmount: parseFloat($("#paymentAmount").val()),
                PaymentGatewayId: $("#hdnPaymentGatewayId").val(),
                PaymentGatewayName: $("#hdnGatwayName").val(),
                CyberSourceToken: querystr,
                GatewayCode: $("#hdnGatwayName").val(),
                IsSaveCreditCard: $("#SaveCreditCard").is(':checked')
            }
        };
    }

    GetSubmitPaymentViewACHModel(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsOrderId: $("#OmsOrderId").val(),
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

    public ValidateInvoiceAmount(): any {
        var paymentAmount = parseFloat($("#paymentAmount").val());
        var amountDue = parseFloat($("#AmountDue").text());
        var roundoff = $("#priceRoundOff").val();

        if (isNaN(paymentAmount) || paymentAmount == null || paymentAmount == undefined) {
            $('#errorPaymentAmount').empty();
            User.prototype.EnableDisableSubmitPayment(false);
            $("#paymentAmount").val(amountDue.toFixed(roundoff));
        }

        else if (amountDue < paymentAmount) {
            $("#errorPaymentAmount").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAmountDueMessage"));
            User.prototype.EnableDisableSubmitPayment(true);
        }
        else if (paymentAmount == 0) {
            $("#errorPaymentAmount").text(ZnodeBase.prototype.getResourceByKeyName("ErrorpaymentAmountMessage"));
            User.prototype.EnableDisableSubmitPayment(true);
        }
        else {
            User.prototype.EnableDisableSubmitPayment(false);
            $('#errorPaymentAmount').empty();
            $("#paymentAmount").val(paymentAmount.toFixed(roundoff));
        }
    }

    private EnableDisableSubmitPayment(isDisabled: boolean) {
        $('#btnPayInvoice').prop("disabled", isDisabled);
    }

    //Move to cart for Saved Items
    MoveToCartForSavedLaterItem(omsTemplateLineItemId): any
    {
        var flag = true;
        var cartItemCount = $("#hdnSavedCartItemCount").val();
        var templateId = $("#OmsTemplateId").val();
        if (cartItemCount > 0) {
            if (templateId > 0 && omsTemplateLineItemId > 0) {
                window.location.href = "/SaveForLater/AddProductToCart?omsTemplateId=" + templateId + "&omsTemplateLineItemId=" + omsTemplateLineItemId;
            }
            else
            {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSaveOrderTemplate"), "error", false, 0);
                flag = false;
            }
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AddToCartErrorMessage"), "error", false, 0);
            flag = false;
        }
        return flag;
    }

    SubmitBraintreeInvoice(payload,isVault) {
        var cardDetails = payload.details;
        $('#hdnBraintreecardNumber').val(cardDetails.lastFour);
        $("#hdnBraintreeCardExpirationMonth").val(cardDetails.expirationMonth);
        $("#hdnBraintreeCardExpirationYear").val(cardDetails.expirationYear);
        $("#hdnBraintreeCardHolderName").val(cardDetails.cardholderName);
        $("#hdnBraintreeCardType").val(cardDetails.cardType);
        $("#hdnBraintreeNonce").val(payload.nonce);
        $("#hdnBraintreeIsVault").val(isVault);
        User.prototype.PayInvoice();
    }
}

$('#custom-modal').on('hidden.bs.modal', function () {
    if ($("#custom-modal .close, .popup").length > 1) {
        $('body').addClass('modal-open');
    }
});

$('.address-popup').on('hidden.bs.modal', function () {
    $('body').addClass('modal-open');
});

