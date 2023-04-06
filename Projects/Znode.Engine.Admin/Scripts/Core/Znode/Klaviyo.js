var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Klaviyo = /** @class */ (function (_super) {
    __extends(Klaviyo, _super);
    function Klaviyo() {
        return _super.call(this) || this;
    }
    Klaviyo.prototype.Init = function () {
    };
    Klaviyo.prototype.ShowProviderHtml = function () {
        var providerName = $('#ddlProviderTypes option:selected').html();
        var portalId = $('#hdnportalId').val();
        var currentProviderId = $('#ddlProviderTypes  option:selected')[0].id;
        if (currentProviderId == "defaultType") {
            $("#klaviyoprovidertypeform-container").hide();
            $("#klaviyoprovidertypeform-container").html('');
        }
        else {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetProviderTypeKlaviyoForm(providerName, portalId, currentProviderId, function (res) {
                $("#klaviyoprovidertypeform-container").show();
                $("#klaviyoprovidertypeform-container").html(res);
                ZnodeBase.prototype.HideLoader();
            });
        }
    };
    return Klaviyo;
}(ZnodeBase));
//# sourceMappingURL=Klaviyo.js.map