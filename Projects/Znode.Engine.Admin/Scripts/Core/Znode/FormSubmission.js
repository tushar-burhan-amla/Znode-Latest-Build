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
var FormSubmission = /** @class */ (function (_super) {
    __extends(FormSubmission, _super);
    function FormSubmission() {
        return _super.call(this) || this;
    }
    FormSubmission.prototype.Init = function () {
    };
    //Method to make Form Submission Export ajax request
    FormSubmission.prototype.FormSubmissionExport = function (e) {
        e.preventDefault();
        var currentTarget = e.currentTarget;
        var exportTypeId = currentTarget.getAttribute("data-exportTypeId");
        var exportType = currentTarget.getAttribute("data-exporttype");
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.FormSubmissionExport(exportType, function (response) {
            ZnodeBase.prototype.HideLoader();
            if (!response.HasError) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Export has been initiated." + " <a href='/Export/List'>Click here</a> or redirect to the Exports screen to view more details.", 'success', isFadeOut, fadeOutTime);
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "error", true, 5000);
        });
    };
    return FormSubmission;
}(ZnodeBase));
//# sourceMappingURL=FormSubmission.js.map