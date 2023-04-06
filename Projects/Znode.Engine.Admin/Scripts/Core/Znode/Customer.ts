class Customer extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
    }

    Init() {
        GiftCard.prototype.GetActiveCurrencyToStore("");
        User.prototype.Init();
        $("#ReferralStatus").change();
        $("#formCustomerAffiliate").submit(function () {
            return Customer.prototype.ValidationForCustomerAffiliate();
        });
        Customer.prototype.EditCustomerAddress();

        $("#ddl_portal_list").on("change", function () {
            Customer.prototype.PortalChange();
        });
    }

    ValidationForCustomerAffiliate() {
        if ($("#ddl_portal_list").is(':visible')) {
            var portal = $("#ddl_portal_list").val();
            if (portal == null || portal == "") {
                $("#errorRequiredddl_portal_list").html(ZnodeBase.prototype.getResourceByKeyName("SelectPortal"));
                return false;
            }
        }
        if ($("#areaList").is(':visible')) {
            var domain = $("#domains").val();
            if (domain == undefined || domain == null || domain == "") {
                $("#errorRequire_domains").html(ZnodeBase.prototype.getResourceByKeyName("SelectURL"));
                return false;
            }
        }
    }

    //Associate profiles to customer.
    AssociateProfiles() {
        var profileIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (profileIds.length > 0) {
            Endpoint.prototype.AssociateProfilesForCustomer(profileIds, $("#UserId").val(), function (res) {
                $("#associateProfileList").hide();
                window.location.href = window.location.protocol + "//" + window.location.host + "/Customer/GetAssociatedProfileList?userId=" + $("#UserId").val();
            });
        }
        else {
            $('#selectOneProfile').show();
        }
    }

    //Delete associated profiles for customer.
    UnAssociateProfiles(control): any {
        var profileId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var userId = $("#UserId").val();
        if (profileId.length > 0) {
            Endpoint.prototype.UnAssociateProfiles(profileId, userId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    GetUnassociatedProfileList(): any {
        $('#associateProfileList').modal('show');
        if ($("#associateProfileList").find("tr").length == 0) {
            $("#associateProfileList").find(".modal-footer").hide();
        }
    }

    PortalChange() {
        Endpoint.prototype.GetDomains($('#ddl_portal_list').val(), $('#hdnDomainIds').val(), $("#hdn_affiliate_user_id").val(), function (response) {
            $('#domains_to_show').html(response);
        });
    }

    ReferralStatusChange() {
        var referralStatus = $("#ReferralStatus").val();
        if (referralStatus === "A") {
            $("#show_domains").show();
            $("#ddl_portal_list").change();
            if ($('#domains').val() == '' || $('#domains').val() == null)
                Customer.prototype.PortalChange();
        }
        else {
            $("#show_domains").hide();
        }
    }

    DeleteMultipleNotes(control): any {
        var noteId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (noteId.length > 0) {
            Endpoint.prototype.DeleteMultipleCustomerNotes(noteId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleAddress(control): any {
        var IsRoleAdministrator = $("#IsRoleAdministrator").val();
        var userAddressId = "";
        var accountAddressId = "";

        if (IsRoleAdministrator == "True")
            accountAddressId = DynamicGrid.prototype.GetMultipleSelectedIds();
        else
            userAddressId = DynamicGrid.prototype.GetMultipleSelectedIds();

        if (userAddressId.length > 0 || accountAddressId.length > 0) {
            Endpoint.prototype.DeleteMultipleCustomerAddress(userAddressId, accountAddressId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    UnAssociatePriceList(control): any {
        var priceListId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListId.length > 0) {
            Endpoint.prototype.UnAssociatePriceListFromCustomer(priceListId, $('#UserId').val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    EditAssociatedPriceListPrecedence(): any {
        $("#grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            var priceListId = parseInt(decodeURIComponent($(this).attr("data-parameter")).split('&')[0].split('=')[1]);
            var listName = decodeURIComponent($(this).attr("data-parameter")).split('&')[1].split('=')[1];
            Endpoint.prototype.EditAssociatedPriceListPrecedenceForCustomer(priceListId, $('#UserId').val(), listName, function (res) {
                if (!res.status) {
                    $("#priceListPrecedence").modal("hide");
                    return false;
                }
                $("#priceListPrecedence").modal("show");
                $("#priceListPrecedence").html(res.data.html);
            });
        });
    }

    EditAssociatedPriceListPrecedenceResult(data: any) {
        $("#priceListPrecedence").modal("hide");
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.Message, "success", isFadeOut, fadeOutTime);
        ZnodeBase.prototype.RemovePopupOverlay();

        if ($('#UserId').val().length > 0)
            Customer.prototype.AssociatedPriceList();
    }

    AssociatedPriceList() {
        Endpoint.prototype.GetAssociatedPriceListForCustomer($("#UserId").val(), function (response) {
            $("#AssociatedPriceListToCustomer").html('');
            $("#AssociatedPriceListToCustomer").html(response);
            GridPager.prototype.UpdateHandler();
        });
    }

    AssociatePriceListToCustomer() {
        var priceListId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListId.length > 0) {
            Endpoint.prototype.AssociatePriceListForCustomer($("#UserId").val(), priceListId, function (res) {
                $("#DivGetUnAssociatedPriceListForCustomer").hide(700);
                window.location.href = window.location.protocol + "//" + window.location.host + "/Customer/GetAssociatedPriceListForCustomer?userId=" + $("#UserId").val();
            });
        }
        else {
            $('#associatedPriceListId').show();
        }
    }

    SetCommisionValidation() {
        var referralCommissionType = $("#ddlReferralCommisionType option:selected").text();
        var commission = $("#ReferralCommission").val();
        if (commission < 0) {
            $("#valReferralCommission").text('').text(ZnodeBase.prototype.getResourceByKeyName("InvalidBannerSequence")).addClass("field-validation-error").show();
            return false;
        }
        else if (referralCommissionType.trim().toLowerCase() == "percentage") {
            if (parseFloat(commission) > 100) {
                $("#valReferralCommission").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorValidAffiliatePercentage")).addClass("field-validation-error").show();
                return false;
            }
            return true;
        }
        else if (referralCommissionType.trim().toLocaleLowerCase() == "amount") {
            if (parseFloat(commission) > 999999) {
                $("#valReferralCommission").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorValidAffiliateAmount")).addClass("field-validation-error").show();
                return false;
            }
            return true;
        }
        return true;
    }

    SetdefaultProfile(): any {
        var ids = MediaManagerTools.prototype.unique();
        var userId = $("#UserId").val();
        if (ids.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else if (ids.length > 1)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAnyOneToSetAsDefault"), 'error', isFadeOut, fadeOutTime);
        else {
            Endpoint.prototype.SetCustomerDefaultProfile(userId, ids.toString(), function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/Customer/GetAssociatedProfileList?userId=" + $("#UserId").val();
            });
        }
    }

    ValidatePrecedanceField(object): any {
        var regex = new RegExp('^\\d{0,}?$');
        var isValid = true;
        if (isNaN($(object).val())) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (!regex.test($(object).val()) || $(object).val() == 0) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DisplayOrderRange"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("PrecedenceIsRequired"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    HideGuestUserColumn(): void {
        $('#grid tbody tr').each(function () {
            $(this).find("td").each(function () {
                if ($(this).hasClass('grid-action')) {
                    if ($(this).next().children().hasClass("z-active")) {
                        $(this).children().children("ul").children().find(".z-disable").parent().hide();
                    }
                }
            });
            $(this).find("td.Guest").each(function () {
                if ($(this).children("i").hasClass("z-active")) {
                    $(this).next().next().children().children("ul").children().find(".z-disable").parent().hide();
                    $(this).next().next().children().children("ul").children().find(".z-enable").parent().hide();
                }
            });
        });
    }

    GetOrderDetailsByRowSelection(): any {
        $("#grid tbody tr").on("click", function (e) {
            if ($(this).find("td a").attr("href") !== undefined && $(this).find("td a").attr("href") !== "")
                window.location.href = $(this).find("td a").attr("href");
        });
    }

    EditCustomerAddress(): void {
        $("#customerAddressListId #grid tbody tr td").find(".z-edit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            $("#divCustomerAddressPopup").html("");
            var href = $(this).attr('href');
            ZnodeBase.prototype.BrowseAsidePoupPanel(href, 'divCustomerAddressPopup');
        });
    }

    //This method is used to select store from fast select and show it on textbox
    OnSelectStoreAutocompleteDataBind(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: number = item.Id;
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetUserList(portalId, portalName, function (response) {
                $("#userList").html("");
                $("#userList").html(response);
                ZnodeBase.prototype.HideLoader();
            });
        }
    }

    //Toggle the icon of voucher activate deactivate button
    ToggleActivateDeactivateActionClass(control): any {
        $('#grid tbody tr').each(function () {
            var target = $(this).find("td.grid-action").find('.action-ui').find("[data-managelink='Disable']");
            var isActive = $(target).hasClass('z-enable');
            if (isActive) {
                $(target).removeClass('z-enable');
                $(target).addClass('z-disable');
                $(target).attr('title', 'Deactivate');
            } else {
                $(target).removeClass('z-disable');
                $(target).addClass('z-enable');
                $(target).attr('title', 'Activate');
            }
        });
    }
}