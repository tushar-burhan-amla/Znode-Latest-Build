class Sms extends ZnodeBase {
    constructor() {
        super();
    }
    Init(): void {
    }

    ShowProviderHtml() {
        let providerName: string = $('#ddlProviderTypes option:selected').html();
        let portalId: number = $('#hdnportalId').val();
        let currentProviderId :string = $('#ddlProviderTypes  option:selected')[0].id;
        if (currentProviderId == "defaultType") {
            $("#providertypeform-container").hide();
            $("#providertypeform-container").html('');
        }
        else {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetProviderTypeForm(providerName, portalId, currentProviderId, function (res) {
                $("#providertypeform-container").show();
                $("#providertypeform-container").html(res);
                ZnodeBase.prototype.HideLoader();
            });
        }

    }
   
}