class Diagnostics extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init():any {
        Endpoint.prototype.showDiagnosticsTrace(function(response){
            $("#trace").html(response);
        });
    }

    MaintenaceCleanDataPopup() {
        $("#maintenancecleardata").modal('show');
    }

    //To delete all the published data from elastic and sql publish entity
    ClearAllPublishedData() {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("TextClearPublishData"), 'success', isFadeOut, fadeOutTime);

        Endpoint.prototype.ClearAllPublishedData(function (response) {
            if (response != null) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            }
        });
    }
}
