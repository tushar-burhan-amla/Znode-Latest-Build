class GiftCard extends ZnodeBase {
    _endPoint: Endpoint;
    ADD_CUSTOMER_URL: string;
    ADD_CUSTOMER_ROOT_URL: string;
    constructor() {
        super();
        this._endPoint = new Endpoint();
        GiftCard.prototype.ADD_CUSTOMER_ROOT_URL = "/Order/AddNewCustomer?portalid=";
        GiftCard.prototype.ADD_CUSTOMER_URL = GiftCard.prototype.ADD_CUSTOMER_ROOT_URL;
    }

    Init() {
        GiftCard.prototype.GetActiveCurrencyToStore("");
        GiftCard.prototype.ValidateGiftCard();
        GiftCard.prototype.AddCustomEventListener();
    }

    AddCustomEventListener(): void {
        $(document).off("PARTIAL_LOADED").on("PARTIAL_LOADED", function (event, url) {
            if (GiftCard.prototype.ADD_CUSTOMER_URL == url)
                $("form#frmCreateCustomer").attr("data-ajax-success", "GiftCard.prototype.AddCustomerSuccessCallback")

            $("#grid").find("tr").addClass('preview-link');
            $('#DropDownId1').find('input#UserId').closest('li').remove(); /*To Remove CustomerId search Field from column dropdown*/
        });
    }

    ValidateGiftCard(): any {
        $("#UserId").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            GiftCard.prototype.ValidateGiftCardUserId();
            GiftCard.prototype.ValidatePortal();
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidatePortal(): boolean {
        if ($("#PortalId").val() == "" || $("#PortalId").val() == 0 || $("#txtPortalName").val() == "") {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").parent("div").addClass('input-validation-error');
            return false;
        }
        else {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).removeClass("field-validation-error");
            $("#txtPortalName").parent("div").removeClass('input-validation-error');
            return true;
        }
    }

    ValidateStartDate(): boolean {
        var mindate = new Date($("#StartDate").val().replace(/-/g, "/"));
        var maxdate = new Date($("#ExpirationDate").val().replace(/-/g, "/"));
        if ((mindate > maxdate)) {
            $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorStartDate"));
            return false;
        }
        else {
            $("#spamDate").html("");
            return true;
        }
    }
    //Add Customer Callback Function - 
    AddCustomerSuccessCallback(response): boolean {
        var html = $.parseHTML(response);
        if ($($(html).find('.duplicateusererrormessage')[0]).val() != null && typeof $($(html).find('.duplicateusererrormessage')[0]).val() != 'undefined' && $($(html).find('.duplicateusererrormessage')[0]).val() != "") {
            $($($(html).find('.showduplicateusererrormessage')[0])[0]).text($($(html).find('.duplicateusererrormessage')[0]).val());
            $("#ShowDuplicateUserErrorMessage").text($($(html).find('.duplicateusererrormessage')[0]).val());
            return false;
        }
        if (response.indexOf("field-validation-error") < 0) {
            if ($(html).find("#hdnHasError").val() == "False") {
                ZnodeBase.prototype.CancelUpload('customerDetails');
                var userId = $($(html).find('#hdnCreatedUserId')[0]).val();
                $("#CustomerName").val(GiftCard.prototype.GetCustomerName($($(html).find('#FirstName')[0]).val().concat(" ", $($(html).find('#LastName')[0]).val()), $($(html).find('#UserName')[0]).val()));
                $('#CustomerNameError').text('');
                $('#UserId').val(userId);
            }
            else {
                $("#error-create-customer").html("");
                $("#error-create-customer").html($(html).find("#hdnErrorMessage").val());
            }
        }
        else {
            $("#divtaxProductListPopup").html(response);
        }

    }
    ValidateGiftCardUserId(): any {
        if (!GiftCard.prototype.ValidatePortal())
            return false;

        var isValid = true;
        if ($("#UserId").val() != '') {
            Endpoint.prototype.IsUserIdForGiftCardExist($("#UserId").val(), $("#PortalId").val(), function (response) {
                if (!response) {
                    $("#CustomerName").addClass("input-validation-error");
                    $("#valCustomerName").addClass("error-msg");
                    $("#valCustomerName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistGiftCustomerName"));
                    $("#valCustomerName").show();
                    isValid = false;
                }
                else
                    $("#CustomerName").addClass("valid")
            });
        }
        return isValid;
    }

    DeleteGiftCard(control): any {
        var giftCardId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (giftCardId.length > 0) {
            Endpoint.prototype.DeleteGiftCard(giftCardId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ShowTextBox(): any {
        if ($('#EnableToCustomerAccount').prop("checked") == true) {
            $('#ShowUserId').show();
            $('#sendMail').show();
        } else {
            $("#valCustomerName").text('').removeClass("field-validation-valid");
            $("#UserId").val('');
            $('#ShowUserId').hide();
            $('#sendMail').hide();
        }
    }

    ValidateUserId(): any {
        if (!GiftCard.prototype.ValidatePortal())
            return false;
        else if (!GiftCard.prototype.ValidateStartDate()) {
            return false;
        }
          var RemainingAmount = $("#RemainingAmount").val();
        if (RemainingAmount != undefined && RemainingAmount.length < 1) {
            $("#ErrorRemainingAmount").text('').removeClass("field-validation-error");
            $("#ErrorRemainingAmount").text("Remaining Amount cannot be empty").addClass("field-validation-error");
            $("#ErrorRemainingAmount").show();
            return false;
        }
        if ($("#EnableToCustomerAccount").prop('checked') == true) {
            var CustomerName = $("#CustomerName").val();
            if (CustomerName.length < 1) {
                $("#valCustomerName").text('').removeClass("field-validation-error");
                $("#valCustomerName").text(ZnodeBase.prototype.getResourceByKeyName("RequiredCustomerName")).addClass("field-validation-error");
                $("#valCustomerName").show();
                return false;
            }
            else if (!GiftCard.prototype.ValidateGiftCardUserId()) {
                return false;
            }
            else if ($("#CustomerName").hasClass("valid")) {
                $("#valCustomerName").hide();
                return true;
            }
            else {
                $("#valCustomerName").show();
                return false;
            }
        }

        else
            return true;
    }

    GetListBasedOnSelection(): any {
        var dropDownSelection = $("#ExpiredGiftCard").val();
        var isExcludeExpired = true;

        if (dropDownSelection == "All")
            isExcludeExpired = false;

        Endpoint.prototype.GiftCardList(isExcludeExpired, function (response) {
            $("#ZnodeGiftCard").html(response);
        });
    }

    //Get customer list based on portal id.
    GetCustomerList(): any {
        var portalId: number = $("#PortalId").val();
        if (portalId > 0) {
            ZnodeBase.prototype.BrowseAsidePoupPanel('/GiftCard/GetCustomerList?PortalId=' + portalId, 'customerDetails');
        }
        else {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").addClass('input-validation-error');
            return false;
        }
    }
    //Add new Customer on AsidePanel Popup
    AddNewUser(): any {
        var portalId: number = $("#PortalId").val();
        if (portalId > 0) {
            $("#ZnodeUserPortalList").html("");
            $("#ZnodeOrderCustomer").html("");
            GiftCard.prototype.ADD_CUSTOMER_URL = GiftCard.prototype.ADD_CUSTOMER_ROOT_URL + portalId;
            ZnodeBase.prototype.BrowseAsidePoupPanel(GiftCard.prototype.ADD_CUSTOMER_URL, 'customerDetails');
        }
        else {
            $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("SelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").parent("div").addClass('input-validation-error');
            return false;
        }
    }

    //Set customer id for which gift card is created.
    SetCustomerId(): void {
        $("#grid").find("tr").on("click", function () {
            var userId: string = $(this).find("td")[0].innerHTML;
            var fullName= GiftCard.prototype.GetCustomerName($(this).find('.fullname').text(), $(this).find('.username').text())
            $('#CustomerName').val(fullName);
            $('#UserId').val(userId.match(/>(.*?)</)[1]);
            $('#customerDetails').hide(700);
            Products.prototype.HideErrorMessage($("#UserId"), $("#valCustomerName"));
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }

    //Get Customer Name 
    GetCustomerName(fullname, userName): string{
        return fullname ? userName.concat(' ', "|", ' ', fullname) : userName;
    }

    GetActiveCurrencyToStore(currencyCode): void {
        if (currencyCode != "") {
            Endpoint.prototype.GetCurrencyDetailsByCode(currencyCode, function (response) {
                $("span[for='CurrencySymbol']").html(response.currencyViewModel.Symbol);
                $("label[for='CurrencyName']").html(response.currencyViewModel.CurrencyName);
            });
        }
        else {
            var portalId = $("#PortalId").val();
            if (portalId != undefined && portalId != "" && portalId != 0 ) {
                Endpoint.prototype.GetActiveCurrencyToStore(parseInt(portalId), function (response) {
                    $("span[for='CurrencySymbol']").html(response.currencyViewModel.Symbol);
                    $("label[for='CurrencyName']").html(response.currencyViewModel.CultureName);
                    $("#CurrencyCode").val(response.currencyViewModel.CultureCode);
                    $("#CultureCode").val(response.currencyViewModel.CultureCode);
                });

            }
            else {
                portalId = "0";
            }
        }
    }

    //This method is used to get portal list on aside panel.
    GetPortalList(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/GiftCard/GetPortalList', 'divStoreListAsidePanel');
    }

    //To Do: To bind portal information
    OnSelectPortalResult(item: any): any {
        $("#valCustomerName").text('').text("").removeClass("field-validation-error").hide();
        $("#UserId").removeClass('input-validation-error');

        Store.prototype.OnSelectStoreAutocompleteDataBind(item);
        GiftCard.prototype.GetActiveCurrencyToStore("");
    }

    //This method is used to select portal from list and show it on textbox.
    GetPortalDetail(): void {
        $("#grid").find("tr").on("click", function () {
            let portalName: string = $(this).find("td[class='storecolumn']").text();
            let portalId: string = $(this).find("td")[0].innerHTML;
            $('#txtPortalName').val(portalName);
            $('#PortalId').val(portalId);
            $('#UserId').val('');
            $("#valCustomerName").text('').text("").removeClass("field-validation-error").hide();
            $("#UserId").removeClass('input-validation-error');

            GiftCard.prototype.GetActiveCurrencyToStore("");

            $("#errorRequiredStore").text('').text("").removeClass("field-validation-error").hide();
            $("#txtPortalName").removeClass('input-validation-error');
            $('#divStoreListAsidePanel').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }

    ActivateVouchers(control): any {
        var voucherIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (voucherIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.ActivateDeactivateVouchers(voucherIds, true, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ActivateVoucherMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DeactivateVouchers(control): any {
        var voucherIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (voucherIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.ActivateDeactivateVouchers(voucherIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
                if (res.status == true)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DeactivateVoucherMessage"), 'success', isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    GetVoucherHistoryList(voucherId: any): any {
        var portalId: number = $("#PortalId").val();
        if (portalId > 0 && voucherId > 0) {
            Endpoint.prototype.GetVoucherHistoryList(voucherId, portalId, function (response) {
                $("#DivVoucherHistory").html('');
                $('#DivVoucherHistory').append(response);
            });
        }

    }

    //Toggle the icon of voucher activate deactivate button
    ToggleActivateDeactivateActionClass(control): any {
        $('#grid tbody tr').each(function () {
            var target = $(this).find("td.grid-action").find('.action-ui').find("[data-managelink='Activate']");
            var isActive = $(target).hasClass('z-enable');
            if (isActive) {
                $(target).removeClass('z-enable');
                $(target).addClass('z-disable');
                $(target).attr('title', 'Deactivate');
            } else {
                $(target).removeClass('z-disable');
                $(target).addClass('z-enable');
            }
        });
    }
}
