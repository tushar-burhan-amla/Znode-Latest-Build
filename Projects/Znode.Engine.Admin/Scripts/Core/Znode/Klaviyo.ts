class Klaviyo extends ZnodeBase {
    constructor() {
        super();
    }
    Init(): void {
    }
    ShowProviderHtml() {
        let providerName: string = $('#ddlProviderTypes option:selected').html();
        let portalId: number = $('#hdnportalId').val();
        let currentProviderId: string = $('#ddlProviderTypes  option:selected')[0].id;
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
    }
}