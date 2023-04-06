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
var GlobalAttributeFamily = /** @class */ (function (_super) {
    __extends(GlobalAttributeFamily, _super);
    function GlobalAttributeFamily() {
        return _super.call(this) || this;
    }
    GlobalAttributeFamily.prototype.HideShowExpandOption = function (control, controller, action) {
        $(control).find("i").toggleClass("z-add z-minus");
        $(control).closest("thead").next("tbody .attributeData").toggle();
        if ($(control).find("i").hasClass("z-minus")) {
            this._attributeId = $(control).data("attributegroupid");
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
    };
    //This method is used to show popup to edit display order
    GlobalAttributeFamily.prototype.EditDisplayOrder = function (groupID, groupCode, displayOrder) {
        $("#errorSpamtxtDisplayOrder").removeClass("error-msg field-validation-valid").hide();
        $("#txtDisplayOrder").removeClass('input-validation-error');
        $('#divEditDisplayOrderPopup').show();
        $('#txtGroupCode').val(groupCode);
        $('#txtDisplayOrder').val(displayOrder);
        $('#hdnGroupCodeId').val(groupID);
        $('#divEditDisplayOrderPopup').modal({ backdrop: 'static', keyboard: false });
    };
    GlobalAttributeFamily.prototype.DeleteGlobalAttributeFamily = function (control) {
        var globalAttributeFamilyIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (globalAttributeFamilyIds.length > 0) {
            Endpoint.prototype.DeleteGlobalAttributeFamily(globalAttributeFamilyIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    };
    //This method is used to update display order of attribute group 
    GlobalAttributeFamily.prototype.UpdateDisplayOrder = function (control) {
        var displayOrder = $('#txtDisplayOrder').val();
        var groupCode = $('#txtGroupCode').val();
        var familyCode = $('#FamilyCode').val();
        if (!GlobalAttributeEntity.prototype.ValidateDisplayOrder(displayOrder))
            return;
        Endpoint.prototype.UpdateFamilyGroupDisplayOrder(groupCode, displayOrder, familyCode, function (response) {
            $('#divEditDisplayOrderPopup').modal("hide");
            if (response.status) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                GlobalAttributeFamily.prototype.GetAttributeGroupEntity();
            }
            else {
                ZnodeBase.prototype.HideLoader();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
    };
    GlobalAttributeFamily.prototype.GetAttributeGroupEntity = function () {
        var familyCode = $('#FamilyCode').val();
        $("#divGroupToFamily").html("");
        if (familyCode != "") {
            Endpoint.prototype.GetTabStructureForAttributeFamily(familyCode, function (response) {
                if (response != "" && response != null && typeof (response) != "undefined") {
                    $("#divGroupToFamily").append(response);
                }
            });
        }
    };
    GlobalAttributeFamily.prototype.OnSelectEntityAutocompleteDataBind = function (item) {
        if (item != undefined) {
            var entityType = item.text;
            var entityId = item.Id;
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetAttributeFamilyList(entityId, entityType, function (response) {
                $("#ZnodeGlobalAttributeFamilyList").html("");
                $("#ZnodeGlobalAttributeFamilyList").html(response);
                ZnodeBase.prototype.HideLoader();
            });
        }
    };
    //This method will unassign the global attribute group.
    GlobalAttributeFamily.prototype.UnAssignAttributeGroup = function (groupCode, control) {
        if (groupCode != "") {
            var familyCode = $('#FamilyCode').val();
            Endpoint.prototype.UnAssignGlobalAttributeGroupFromFamily(groupCode, familyCode, function (response) {
                if (response.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "success", isFadeOut, fadeOutTime);
                    GlobalAttributeFamily.prototype.GetAttributeGroupEntity();
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "error", isFadeOut, fadeOutTime);
                }
            });
        }
    };
    //Validate display order.
    GlobalAttributeFamily.prototype.ValidateDisplayOrder = function (displayOrder) {
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
    };
    GlobalAttributeFamily.prototype.ValidateFamilyCode = function () {
        var isValid = true;
        var globalAttributeFamilyId = $('#GlobalAttributeFamilyId').val();
        if (globalAttributeFamilyId < 1 && $("#FamilyCode").val() != '') {
            Endpoint.prototype.IsGlobalFamilyCodeExist($('#FamilyCode').val(), function (response) {
                if (response.data) {
                    $("#FamilyCode").addClass("input-validation-error");
                    $("#errorSpanFamilyCode").addClass("error-msg");
                    $("#errorSpanFamilyCode").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistAttributeFamilyCode"));
                    $("#errorSpanFamilyCode").show();
                    isValid = false;
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
        return isValid;
    };
    return GlobalAttributeFamily;
}(ZnodeBase));
//# sourceMappingURL=GlobalAttributeFamily.js.map