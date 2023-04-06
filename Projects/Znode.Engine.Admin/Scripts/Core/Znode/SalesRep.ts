class SalesRep extends ZnodeBase {
    _endPoint: Endpoint;
    _multiFastItemdata: Array<string>;

    constructor() {
        super();
        User.prototype._multiFastItemdata = new Array<string>();
    }

    Init() {       
        User.prototype.SetIsSelectAllPortalOnInit();     
    }

    EnableUserAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.EnableDisableSalesRepAccount(accountIds, true, function (res) {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/SalesRep/List";
            });
            ZnodeBase.prototype.HideLoader();
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisableUserAccount(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.EnableDisableSalesRepAccount(accountIds, false, function (res) {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/SalesRep/List";
            });
            ZnodeBase.prototype.HideLoader();
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    UserResetPassword(): any {
        var accountIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (accountIds.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.UserResetSalesRepPassword(accountIds, function (res) {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/SalesRep/List";
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

}