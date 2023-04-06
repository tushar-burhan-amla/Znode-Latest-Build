class RMAConfiguration extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    constructor() {
        super();
    }
    Init() {
    }

    CreateReasonForReturn(): any {
        Endpoint.prototype.CreateReasonForReturn(function (res) {
            $("#reasonForReturnPopup").modal("show");
            $("#reasonForReturnPopup").html(res);
        });
    }

    EditReasonForReturn(): any {
        $("#grid tbody tr td").find(".z-edit").click(function (e) {
            e.preventDefault();
            var rmaReasonForReturnId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditReasonForReturn(rmaReasonForReturnId, function (res) {
                if (res != "") {
                    $("#reasonForReturnPopup").modal("show");
                    $("#reasonForReturnPopup").html(res);
                }
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), 'error', isFadeOut, fadeOutTime);
            });
        });
    }

    AddReasonForReturnResult(response: any) {
        if (response.status) {
            Endpoint.prototype.GetReasonForReturnList(function (res) {
                $("#reasonForReturnPopup").html(res);
                $("#reasonForReturnPopup").modal("hide");
            });
        }
        $("#reasonForReturnPopup").modal("hide");
        WebSite.prototype.RemovePopupOverlay();
        window.location.href = window.location.protocol + "//" + window.location.host + "/RMAConfiguration/GetReasonForReturnList";
    }

    DeleteReasonForReturn(control): any {
        var rmaReasonForReturnId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (rmaReasonForReturnId.length > 0) {
            Endpoint.prototype.DeleteReasonForReturn(rmaReasonForReturnId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    EditRequestStatus(): any {
        $("#grid tbody tr td").find(".z-edit").click(function (e) {
            e.preventDefault();
            var rmaRequestStatusId = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            Endpoint.prototype.EditRequestStatus(rmaRequestStatusId, function (res) {
                if (res != "") {
                    $("#requestStatusPopup").modal("show");
                    $("#requestStatusPopup").html(res);
                }
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), 'error', isFadeOut, fadeOutTime);
            });
        });
    }

    EditRequestStatusResult(response: any) {
        if (response.status) {
            Endpoint.prototype.GetRequestStatusList(function (res) {
                $("#requestStatusPopup").html(res);
                $("#requestStatusPopup").modal("hide");
            });
        }
        $("#requestStatusPopup").modal("hide");
        WebSite.prototype.RemovePopupOverlay();
        window.location.href = window.location.protocol + "//" + window.location.host + "/RMAConfiguration/GetRequestStatusList";
    }

    DeleteRequestStatus(control): any {
        var rmaRequestStatusId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (rmaRequestStatusId.length > 0) {
            Endpoint.prototype.DeleteRequestStatus(rmaRequestStatusId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }
    ValidateNameField(object): any {
        var isValid = true;
        if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            if ($(object).val() == '')
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ReasonForReturnIsRequired"), 'error', isFadeOut, fadeOutTime);

            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }
}