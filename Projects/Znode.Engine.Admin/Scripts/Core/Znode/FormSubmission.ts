class FormSubmission extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
    }
    Init(): void {

    }
    //Method to make Form Submission Export ajax request
    FormSubmissionExport(e: any) {
        e.preventDefault();
        var currentTarget = e.currentTarget;
        var exportTypeId: string = currentTarget.getAttribute("data-exportTypeId");
        var exportType: string = currentTarget.getAttribute("data-exporttype");
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.FormSubmissionExport(exportType, function (response) {
            ZnodeBase.prototype.HideLoader();
            if (!response.HasError) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Export has been initiated." + " <a href='/Export/List'>Click here</a> or redirect to the Exports screen to view more details.", 'success', isFadeOut, fadeOutTime);
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "error", true, 5000);
        });

    }
}

