class PIMAttributeFamily extends ZnodeBase {
    constructor() {
        super();
    }
    DeletePIMFamily(control, contollerName): any {
        var pimAttributeFamilyIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (pimAttributeFamilyIds.length > 0) {
            Endpoint.prototype.DeletePIMFamily(pimAttributeFamilyIds, contollerName, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidatePIMAttributeFamily(): any {
        $("#FamilyCode").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            PIMAttributeFamily.prototype.ValidatePIMAttributeFamilyCode();
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidatePIMAttributeFamilyCode(): boolean {
        var isValid = true;
        if ($("#FamilyCode").val() == '') {
            $("#FamilyCode").addClass("input-validation-error");
            $("#errorSpanFamilyCode").addClass("error-msg");
            $("#errorSpanFamilyCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAttributeFamilyCodeRequired"));
            $("#errorSpanFamilyCode").show();
            ZnodeBase.prototype.HideLoader();
        }
        else {
            Endpoint.prototype.IsAttributeFamilyCodeExist($("#FamilyCode").val(), $("#IsCategory").val(), $("#PimAttributeFamilyId").val(), function (response) {
                if (!response) {
                    $("#FamilyCode").addClass("input-validation-error");
                    $("#errorSpanFamilyCode").addClass("error-msg");
                    $("#errorSpanFamilyCode").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistFamilyCode"));
                    $("#errorSpanFamilyCode").show();
                    isValid = false;
                }
            });
        }

        if (!CommonHelper.prototype.Validate())
            isValid = false;
        return isValid;
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

    //This method is used to update display order of attribute group in attribute family
    UpdateDisplayOrder(control) {
        var displayOrder = $('#txtDisplayOrder').val();
        var pimattributeGroupId = $('#hdnGroupCodeId').val();
        var pimAttributeFamilyId = $('#PimAttributeFamilyId').val();
        if (!PIMAttributeFamily.prototype.ValidateDisplayOrder(displayOrder))
            return;
        Endpoint.prototype.UpdateAttributeGroupDisplayOrder(pimattributeGroupId, displayOrder, pimAttributeFamilyId, function (response) {
            $('#divEditDisplayOrderPopup').modal("hide");
            if (response.status)
                window.location.reload();
            else {
                ZnodeBase.prototype.HideLoader();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
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

    //Get Unassigned attributes.
    GetUnAssignedAttributes(contollerName, groupID: number, familyID: number): any {
        var pimattributeGroupId = groupID;
        var pimAttributeFamilyId = $('#PimAttributeFamilyId').val();
        if (pimattributeGroupId > 0 && pimAttributeFamilyId > 0) {
            Endpoint.prototype.GetUnAssigedAttributes(pimattributeGroupId, pimAttributeFamilyId, contollerName, function (response) {
                $("#UnAssignedAttributesForGroup_" + pimattributeGroupId).html(response);
            });
        }
    }
}