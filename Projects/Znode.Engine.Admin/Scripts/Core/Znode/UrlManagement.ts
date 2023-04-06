class UrlManagement extends ZnodeBase {

    constructor() {
        super();
    }


    Init() {
    }
    ValidateDomainNameField(object): any {
        var isValid = true;
        if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            if ($(object).val() == '')
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DomainNameIsRequired"), 'error', isFadeOut, fadeOutTime);

            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    EnableDomain(): any {
        var domainIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (domainIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.EnableDisableAdminAPIDomain(domainIds, true, function (res) {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/UrlManagement/List";
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisableDomain(): any {
        var domainIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (domainIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.EnableDisableAdminAPIDomain(domainIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/UrlManagement/List";
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DeleteMultipleUrl(control): any {
        var urlIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (urlIds.length > 0) {
            Endpoint.prototype.DeleteUrl(urlIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }
}