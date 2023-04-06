class Profiles extends ZnodeBase {
    _endPoint: Endpoint;
    Init(): any {
        $(document).on("UpdateGrid", Profiles.prototype.RefreshGridOnEdit);
    }
    DeleteProfiles(control): any {
        var profileId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (profileId.length > 0) {
            Endpoint.prototype.DeleteProfiles(profileId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }


    AssociateCatalogs(profileId, catalogId) {
        if (catalogId.length > 0) {
            Endpoint.prototype.AssociateCatalogToProfile(profileId, catalogId, function (res) {
                if (res.status) {
                    Endpoint.prototype.GetProfileCatalogList(profileId, function (res) {
                        DynamicGrid.prototype.ClearCheckboxArray();
                        $("#ZnodeProfileAssociatedCatalogList").html(res);
                    });
                }
                $("#divUnassociatedCatalogListPopup").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
            ZnodeBase.prototype.RemoveAsidePopupPanel();
        }
        else {
            $("#divAssociatedProfileCatalogsError").show();
        }
    }

    GetUnassociatedCatalog(profileId) {
        DynamicGrid.prototype.ClearCheckboxArray();
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Profiles/GetProfileUnAssociatedCatalogList?profileId=' + profileId, 'divUnassociatedCatalogListPopup');
    }

    GetUnassociatedShipping(profileId) {
        DynamicGrid.prototype.ClearCheckboxArray();
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Profiles/GetUnAssociatedShippingList?profileId=' + profileId + '&portalId=' + $("#PortalId").val(), 'divUnassociatedShippingListPopup');
    }

    AssociateShipping(profileId) {
        var shippingIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (shippingIds.length > 0) {
            Endpoint.prototype.AssociateShipping(profileId, shippingIds, function (res) {
                if (res.status) {
                    Endpoint.prototype.GetAssociatedShippingList(profileId, $("#PortalId").val(), function (res) {
                        DynamicGrid.prototype.ClearCheckboxArray();
                        $("#ZnodeAssociatedShippingListToProfile").html(res);
                    });
                }
                $("#divUnassociatedShippingListPopup").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
            ZnodeBase.prototype.RemoveAsidePopupPanel();
        }
        else {
            $("#divAssociatedProfileShippingError").show();
        }
    }

    UnAssociateAssociatedShipping(control): any {
        var shippingId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (shippingId.length > 0) {
            Endpoint.prototype.UnAssociateAssociatedShipping(shippingId, $("#ProfileId").val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssociatePaymentSetting(portalId: number): void {
        ZnodeBase.prototype.ShowLoader();
        var linkTaxClassIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('ZnodePayment');
        if (linkTaxClassIds.length > 0)
            Endpoint.prototype.AssociatePaymentSettingForProfiles(linkTaxClassIds, portalId, function (res) {
                Endpoint.prototype.GetAssociatedPaymentListForProfiles(portalId, $("#PortalId").val(), function (response) {
                    $("#AssociatedPaymentListToProfile").html('');
                    $("#AssociatedPaymentListToProfile").html(response);
                    GridPager.prototype.UpdateHandler();
                });
                $("#DivGetUnAssociatedPaymentSettingListForStore").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
                ZnodeBase.prototype.RemoveAsidePopupPanel();
            });
        else {
            $('#associatedTaxClass').show();
            ZnodeBase.prototype.HideLoader();
        }
    }

    RemoveAssociatedPaymentSetting(control: any): void {
        var taxClassIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxClassIds.length > 0) {
            Endpoint.prototype.RemoveAssociatedPaymentSettingForProfiles(taxClassIds, $("#ProfileId").val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateProfile(object: any, defaultInventoryRoundOff: string, message, colname: string): boolean {
        var regex = new RegExp('^\\d{0,}(\\.\\d{0,})?$');
        switch (colname) {
            case "Weighting":
                return Profiles.prototype.ValidateWeighting(object, regex, message);
            case "ProfileName":
                return Profiles.prototype.ValidateProfileName(object, regex, message);
            case "DefaultExternalAccountNo":
                return Profiles.prototype.ValidateProfileCode(object, regex, message);
        }
    }

    RefreshGridOnEdit(): any {
        if ($('#AssociatedPaymentListToProfile').length > 0)
            Profiles.prototype.ProfilePaymentUpdateResult();
        else if ($('#ZnodeAssociatedShippingListToProfile').length > 0)
            Profiles.prototype.ProfileShippingUpdateResult();
    }

    ProfilePaymentUpdateResult(): any {
        $("[update-container-id='AssociatedPaymentListToProfile'] #refreshGrid").click();
    }

    ProfileShippingUpdateResult(): any {
        $("[update-container-id='ZnodeAssociatedShippingListToProfile'] #refreshGrid").click();
    }

    private ValidateWeighting(object: any, regex: any, message: string): boolean {
        var isValid: boolean = true;
        var qtyValue: string = $(object).val();
        if (!qtyValue) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterWighting"), 'error', isFadeOut, fadeOutTime);
            $(object).addClass("input-validation-error");
            isValid = false;
        }
        else if (!regex.test(qtyValue)) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorOnlyNumericValue"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (parseInt(qtyValue, 10) > 0 && parseInt(qtyValue, 10) > 999999) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorNumberRange"), 'error', isFadeOut, fadeOutTime);
            $(object).addClass("input-validation-error");
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    private ValidateProfileName(object: any, regex: any, message: string): boolean {
        var isValid: boolean = true;
        var qtyValue: string = $(object).val();
        if (!qtyValue) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorProfileName"), 'error', isFadeOut, fadeOutTime);
            $(object).addClass("input-validation-error");
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    private ValidateProfileCode(object: any, regex: any, message: string): boolean {
        var isValid: boolean = true;
        return isValid;
    }
}
