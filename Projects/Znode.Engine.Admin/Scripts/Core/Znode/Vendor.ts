class Vendor extends ZnodeBase {

    constructor() {
        super();
    }
    Init() {
        Account.prototype.BindStates();
    }

    //Delete vendor
    DeleteVendor(control): void {
        var pimVendorId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (pimVendorId.length > 0) {
            Endpoint.prototype.DeleteVendor(pimVendorId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Associate products to vendor
    AssociateVendorProducts(): void {
        ZnodeBase.prototype.ShowLoader();
        var ProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds('AssociatedProductList');
        if (ProductIds.length > 0)
            Endpoint.prototype.VendorAssociatedProductList($("#VendorCode").val(), $("#VendorName").val(), ProductIds, function (res) {
                Endpoint.prototype.AssociatedProductList($("#PimVendorId").val(), $("#VendorCode").val(), $("#VendorName").val(), function (response) {
                    $("#AssociatedVendorProductList").html('');
                    $("#AssociatedVendorProductList").html(response);
                    GridPager.prototype.UpdateHandler();
                });
                $("#divVendorProductListPopup").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
                ZnodeBase.prototype.RemoveAsidePopupPanel();
            });
        else {
            Vendor.prototype.DisplayNotificationMessagesForVendor(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneProduct"), "error", isFadeOut, fadeOutTime);
            ZnodeBase.prototype.HideLoader();
        }
    }

    //unassociate products from vendor
    UnassociateVendorProduct(control): void {
        var vendorProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (vendorProductIds.length > 0) {
            Endpoint.prototype.VendorUnAssociateProductList(vendorProductIds, $("#VendorCode").val(), function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Dispaly notification messages
    DisplayNotificationMessagesForVendor(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
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

    //Show/hide save cancel button.
    ShowHideSaveCancelButton(): void {
        if ($("#divVendorProductListPopup").find("tr").length > 0)
            $("#divSave").show();
        else
            $("#divSave").hide();
    }

    //Active/Inactive vendor
    ActiveInactiveVendor(isActive: boolean): void {
        var vendorIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (vendorIds.length > 0) {
            Endpoint.prototype.ActiveInactiveVendor(vendorIds, isActive, function (response) {
                $("#ZnodePimVendor #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }
}
