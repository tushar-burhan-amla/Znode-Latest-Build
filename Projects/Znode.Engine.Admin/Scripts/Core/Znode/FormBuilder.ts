class FormBuilder extends ZnodeBase {

    constructor() {
        super();
    }

    Init() {
        FormBuilder.prototype.DisableControls();
    }

    DeleteFormBuilder(control): any {
        var formBuilderIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (formBuilderIds.length > 0) {
            Endpoint.prototype.DeleteFormBuilder(formBuilderIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Check unique form code.
    IsFormCodeExist(): boolean {
        let result: boolean = true;
        let formId: number = $("#FormBuilderId").val();
        let formCode: string = $('#FormCode').val();
        if ((typeof (formId) == "undefined" || formId < 1) && (typeof (formCode) != "undefined" && formCode != "")) {
            Endpoint.prototype.IsFormCodeExist(formCode, function (res) {
                if (res.data) {
                    $("#errorSpanFomCode").addClass("error-msg");
                    $("#errorSpanFomCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorCodeAlreadyExist"));
                    $("#errorSpanFomCode").show();
                    result = false;
                }
            });
        }

        return result;
    }

    UpdateAttributeDisplayOrder(attributeId: number, moveToUp: boolean) {
        if (attributeId > 0) {
            ZnodeBase.prototype.ShowLoader();
            let formBuilderId: number = $("#FormBuilderId").val();
            var model = { "FormBuilderId": formBuilderId, "AttributeId": attributeId, "IsNavigateUpward": moveToUp };
            Endpoint.prototype.UpdateAttributeDisplayOrder(model, function (res) {
                if (res.HasNoError) {
                    FormBuilder.prototype.GetFormBuilderAttributeGroup(formBuilderId);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "error", isFadeOut, fadeOutTime);
                }
            });
            ZnodeBase.prototype.HideLoader();
        }
        FormBuilder.prototype.DisableControls();
    }

    UpdateGroupDisplayOrder(groupId: number, moveToUp: boolean) {
        if (groupId > 0) {
            ZnodeBase.prototype.ShowLoader();
            let formBuilderId: number = $("#FormBuilderId").val();
            var model = { "FormBuilderId": formBuilderId, "GroupId": groupId, "IsNavigateUpward": moveToUp };
            Endpoint.prototype.UpdateGroupDisplayOrder(model, function (res) {
                if (res.HasNoError) {
                    FormBuilder.prototype.GetFormBuilderAttributeGroup(formBuilderId);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "error", isFadeOut, fadeOutTime);
                }
            });
            ZnodeBase.prototype.HideLoader();
        }
    }

    UnAssignAttribute(attributeId: number) {
        if (attributeId > 0) {
            ZnodeBase.prototype.ShowLoader();
            let formBuilderId: number = $("#FormBuilderId").val();
            Endpoint.prototype.UnAssignAttribute(formBuilderId, attributeId, function (res) {
                if (res.HasNoError) {
                    FormBuilder.prototype.GetFormBuilderAttributeGroup(formBuilderId);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "success", isFadeOut, fadeOutTime);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "error", isFadeOut, fadeOutTime);
                }
            });
            ZnodeBase.prototype.HideLoader();
        }
    }

    UnAssignGroup(groupId: number) {
        if (groupId > 0) {
            ZnodeBase.prototype.ShowLoader();
            let formBuilderId: number = $("#FormBuilderId").val();
            Endpoint.prototype.UnAssignGroup(formBuilderId, groupId, function (res) {
                if (res.HasNoError) {
                    FormBuilder.prototype.GetFormBuilderAttributeGroup(formBuilderId);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "success", isFadeOut, fadeOutTime);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.Message, "error", isFadeOut, fadeOutTime);
                }
            });
            ZnodeBase.prototype.HideLoader();
        }
    }

    GetFormBuilderAttributeGroup(formBuilderId: number): void {
        $("#frombuilderControls").html("");
        if (formBuilderId > 0) {
            Endpoint.prototype.GetFormBuilderAttributeGroup(formBuilderId, function (response) {
                if (response != "" && response != null && typeof (response) != "undefined") {
                    $("#frombuilderControls").append(response);
                    $('#frombuilderControls button[id="UploadMultiple"]').attr("disabled", "disabled");
                }
            });
        }
    }

    //Disable  multifile upload control attribute.
    DisableControls(): void {
        $('button[id="UploadMultiple"]').attr("disabled", "disabled");
    }
}