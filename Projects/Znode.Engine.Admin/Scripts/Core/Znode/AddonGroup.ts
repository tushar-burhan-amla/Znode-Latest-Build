var _data: any;
var addonId: number;
var selectedTab: string;
class AddonGroup extends ZnodeBase {
    _endPoint: Endpoint;
    _Model: any;

    constructor() {
        super();
    }

    Init() {
    }

    DdlCultureChange() {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
        $.cookie("_addOnCulture", $("#ddlCultureSpan").attr("data-value"), { expires: expiresTime });
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];
        window.location.reload();
    }

    CultureChange() {
        $.cookie("_addOnCulture", $("#ddlAddonLocale").val());
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];

        if (orignalUrl.indexOf('PimAddonGroupId;') > -1)
            window.location.replace(orignalUrl + "?PimAddonGroupId=" + $("#PimAddonGroupId").val());
        else
            window.location.reload();
    }

    AddSKUs(): void {
        var productIds = DynamicGrid.prototype.GetMultipleSelectedIds('UnassociatedProducts');
        if (productIds != "" && productIds != null && productIds != undefined) {
            AddonGroup.prototype.AssociateAddonGroupSKU(productIds, parseInt($("#PimAddonGroupId").val(), 10));
            ZnodeBase.prototype.CancelUpload('divAssociateAddonGroupProduct');
            DynamicGrid.prototype.ClearCheckboxArray();
        }
        else {
            $("#AssociateAddonGroupError").show();
        }
    }

    AssociateAddonGroupSKU(associatedProductIds: string, addonGroupId: number): void {
        var model: Object = { "ParentId": addonGroupId, "AssociatedIds": associatedProductIds };
        Endpoint.prototype.AssociateAddonGroupProducts(model, function (response) {
            if (response.status) {
                AddonGroup.prototype.GetAssociatedProducts($("#associatedProducts"), addonGroupId, $("#AddonGroupName").val(), parseInt($("#LocaleId").val()));
            } else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper("An error occured while associating addon group products.", "error", true, 5000);
            }
            $("#divAssociateAddonGroupProduct").html("");
        });
    }

    //Method for Delete Addon Groups
    DeleteMultipleAddonGroupProduct(control) {
        var addonGroupProductIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (addonGroupProductIds.length > 0) {
            Endpoint.prototype.DeleteAddonGroupProducts(addonGroupProductIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    GetAssociatedProducts(containerDiv: any, addonGroupId: number, addonGroupName: string, localeId: number) {
        Endpoint.prototype.GetAssociatedAddonProducts(addonGroupId, addonGroupName, localeId, function (res) {
            containerDiv.html(res);
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Addon group products associated successfully.", "success", true, 5000);
        });
    }
}