class Brand extends ZnodeBase {

    constructor() {
        super();
    }

    Init() {
        if ($("#BrandId").val() > 0)
            $.cookie("_brandCulture", $("#ddlBrandLocale").val());
    }

    //Delete brand.
    DeleteBrand(control): void {
        var brandIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (brandIds.length > 0) {
            Endpoint.prototype.DeleteBrand(brandIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Associate product to a brand.
    AssociateBrandProducts(): void {
        ZnodeBase.prototype.ShowLoader();
        var linkProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('AssociatedProductList');

        if (linkProductIds.length > 0)
            Endpoint.prototype.BrandAssociateProductList($("#BrandCode").val(), linkProductIds, function (res) {
                Endpoint.prototype.AssociatedBrandProductList($("#BrandId").val(), $("#BrandCode").val(), $("#BrandName").val(), $("#LocaleId").val(), function (response) {
                    $("#BrandAssociatedProductList").html('');
                    $("#BrandAssociatedProductList").html(response);
                    GridPager.prototype.UpdateHandler();
                });
                $("#divBrandProductListPopup").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
                ZnodeBase.prototype.RemoveAsidePopupPanel();
            });
        else {
            Brand.prototype.DisplayNotificationMessagesForBrand(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneProduct"), "error", isFadeOut, fadeOutTime);
            ZnodeBase.prototype.HideLoader();
        }
    }

    //Unassociate product from a brand.
    UnassociateBrandProduct(control): void {
        var brandProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (brandProductIds.length > 0) {
            Endpoint.prototype.BrandUnAssociateProductList(brandProductIds, $("#BrandCode").val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Dispaly notification messages
    DisplayNotificationMessagesForBrand(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        var element: any = $(".taxClassMessageBoxContainer");
        $(".taxClassMessageBoxContainer").removeAttr("style");
        $(window).scrollTop(0);
        $(document).scrollTop(0);
        if (element.length) {
            if (message !== "" && message != null) {
                element.html("<p>" + message + "</p>");
                element.find('p').addClass('error-msg');

                if (isFadeOut == null || typeof isFadeOut === "undefined") isFadeOut = true;
                if (fadeOutMilliSeconds == null || typeof fadeOutMilliSeconds === "undefined") fadeOutMilliSeconds = 10000;

                if (isFadeOut == true) {
                    setTimeout(function () {
                        element.fadeOut().empty();
                    }, fadeOutMilliSeconds);
                }
            }
        }
    }

    //Set cookies when locales dropdown change.
    DdlCultureChange(): void {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
        $.cookie("_brandCulture", $("#ddlCultureSpan").attr("data-value"), { expires: expiresTime }); // expires after 2 hours
        window.location.reload();
    }

    //Set cookies value when locale dropdown change.
    CultureChangeOnEdit(): void {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
        $.cookie("_brandCulture", $("#ddlBrandLocale").val(), { expires: expiresTime }); // expires after 2 hours
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];

        if (url.indexOf('BrandId') > -1)
            window.location.replace(orignalUrl + "?BrandId=" + $("#BrandId").val());
        else
            window.location.reload();
    }

    //Show/hide save cancel button.
    ShowHideSaveCancelButton(): void {
        if ($("#divBrandProductListPopup").find("tr").length > 0)
            $("#divSave").show();
        else
            $("#divSave").hide();
    }

    //Active and Inactive brand.
    ActiveInactiveBrand(isActive: boolean): void {
        var brandIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (brandIds.length > 0) {
            Endpoint.prototype.ActiveInactiveBrand(brandIds, isActive, function (response) {
                $("#ZnodeBrandDetails #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else 
            $('#NoCheckboxSelected').modal('show');
    }

    //Check duplicate brand SEOFriendlyPageName.
    ValidateBrandSEOFriendlyPageName(): boolean {
        var isValid = true;
        if ($("#SEOFriendlyPageName").val() != undefined && $("#SEOFriendlyPageName").val().trim().length>0)
            Endpoint.prototype.IsBrandSEOFriendlyPageNameExist($("#SEOFriendlyPageName").val(), $("#CMSSEODetailId").val(), function (response) {
                if (!response) {
                    $("#SEOFriendlyPageName").addClass("input-validation-error");
                    $("#errorSEOFriendlyPageName").addClass("error-msg");
                    $("#errorSEOFriendlyPageName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistBrandSEOFriendlyPageName"));
                    $("#errorSEOFriendlyPageName").show();
                    isValid = false;
                    ZnodeBase.prototype.HideLoader();
                }
            });
        return isValid;
    }

    //Associate product to a brand.
    AssociateBrandPortal(): void {
        ZnodeBase.prototype.ShowLoader();
        var linkPortalIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('ZnodeStorePortal');

        if (linkPortalIds.length > 0)
            Endpoint.prototype.BrandAssociatePortalList($("#BrandId").val(), linkPortalIds, function (res) {
                location.reload();
                ZnodeBase.prototype.CancelUpload("divBrandStoreListPopup");
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                DynamicGrid.prototype.ClearCheckboxArray();
            });
        else {
            Brand.prototype.DisplayNotificationMessagesForBrand(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneStore"), "error", isFadeOut, fadeOutTime);
            ZnodeBase.prototype.HideLoader();
        }
    }

    CheckUniqueBrandCode(control): void {
        Endpoint.prototype.CheckUniqueBrandCode($(control).val(), function (res) {
            if (res.result) {
                $("#errorSpanForUnique").text(ZnodeBase.prototype.getResourceByKeyName("ErrorCodeAlreadyExist")).addClass("error-msg").show();
                $(control).focus();
                return false;
            }
            else {
                $("#errorSpanForUnique").hide();
            }
        });
    }

    //Get Brand Name
    GetBrandName(control: any): void {
        var BrandCode = $(control).val();
        var localeId: number = $("#ddlBrandLocale").val();
        Endpoint.prototype.GetBrandName(BrandCode, localeId, function (res) {
            if (res.result) {
                $("#BrandName").val(res.result);
            }      
        });
    }
}

