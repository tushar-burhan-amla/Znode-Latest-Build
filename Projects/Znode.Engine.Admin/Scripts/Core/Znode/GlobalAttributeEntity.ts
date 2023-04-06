class GlobalAttributeEntity extends ZnodeBase {
    _attributeId: number;

    constructor() {
        super();
    }

    public HideShowExpandOption(control: any, controller: string, action: string): void {
        $(control).find("i").toggleClass("z-add z-minus");
        $(control).closest("thead").next("tbody .attributeData").toggle();

        if ($(control).find("i").hasClass("z-minus")) {
            this._attributeId = <number>$(control).data("attributegroupid");
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetAssociatedGlobalAttributes(this._attributeId, function (response) {
                if (response) {
                    $(control).closest("thead").next("tbody .attributeData").html(response).show();
                    ZnodeBase.prototype.HideLoader();
                }
                else {
                    $(control).closest("thead").next("tbody .attributeData").html("No attributes associated to be displayed.").show();
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
    }

    GetAttributeGroupEntity(): void {
        let entityId: number = $('#entityList').val();
        $("#hdnEntityId").val(entityId);
        $("#divEntityToGroup").html("");
        if (entityId > 0) {
            Endpoint.prototype.GetTabStructure(entityId, function (response) {
                if (response != "" && response != null && typeof (response) != "undefined") {
                    $("#divEntityToGroup").append(response);
                }
            });
        }
    }

    //This method is used to show popup to edit display order
    EditDisplayOrder(groupID: number, groupCode: string, displayOrder: number) {
        $("#errorSpamtxtDisplayOrder").removeClass("error-msg field-validation-valid").hide();
        $("#txtDisplayOrder").removeClass('input-validation-error');
        $('#divEditDisplayOrderPopup').show();
        $('#txtGroupCode').val(groupCode);
        $('#txtDisplayOrder').val(displayOrder);
        $('#hdnGroupCodeId').val(groupID);
        $('#divEditDisplayOrderPopup').modal({ backdrop: 'static', keyboard: false });
    }

    //This method is used to update display order of attribute group in attribute entity
    UpdateDisplayOrder(control) {
        var displayOrder = $('#txtDisplayOrder').val();
        var globalattributeGroupId = $('#hdnGroupCodeId').val();
        var globalAttributeEntityId = $('#entityList').val();
        if (!GlobalAttributeEntity.prototype.ValidateDisplayOrder(displayOrder))
            return;
        Endpoint.prototype.UpdateGlobalAttributeGroupDisplayOrder(globalattributeGroupId, displayOrder, globalAttributeEntityId, function (response) {
            $('#divEditDisplayOrderPopup').modal("hide");
            if (response.status) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                GlobalAttributeEntity.prototype.GetAttributeGroupEntity();
            }
            else {
                ZnodeBase.prototype.HideLoader();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
    }

    //This method will unassign the global attribute group.
    UnAssignAttributeGroup(groupId: number, control) {
        if (groupId > 0) {
            let entityId: number = $('#entityList').val();
            Endpoint.prototype.UnAssignGlobalAttributeGroup(groupId, entityId, function (response) {
                if (response.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "success", isFadeOut, fadeOutTime);
                    GlobalAttributeEntity.prototype.GetAttributeGroupEntity();
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "error", isFadeOut, fadeOutTime);
                }
            });
        }
    }

    //Validate display order.
    ValidateDisplayOrder(displayOrder): boolean {
        if (displayOrder === "" && displayOrder.length === 0) {
            Products.prototype.ShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredDisplayOrder"), $("#txtDisplayOrder"), $("#errorSpamtxtDisplayOrder"));
            return false;
        }
        else if (!/^([1-9][0-9]{0,2}|999)$/.test(displayOrder)) {
            $("#errorSpamtxtDisplayOrder").removeClass("error-msg field-validation-valid").show();
            Products.prototype.ShowErrorMessage(ZnodeBase.prototype.getResourceByKeyName("InValidDisplayOrderRange"), $("#txtDisplayOrder"), $("#errorSpamtxtDisplayOrder"));
            return false;
        }
        else
            return true;
    }
}