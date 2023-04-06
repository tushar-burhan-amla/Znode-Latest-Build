class GlobalAttributeGroup extends ZnodeBase {

    constructor() {
        super();
    }

    DeleteGlobalAttributeGroup(control): any {
        var globalAttributeGroupIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (globalAttributeGroupIds.length > 0) {
            Endpoint.prototype.DeleteGlobalAttributeGroup(globalAttributeGroupIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateGroupCode(): any {
        $("#GroupCode").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            GlobalAttributeGroup.prototype.ValidateAttributeGroupCode();
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidateAttributeGroupCode(): boolean {
        var isValid = true;
        if ($("#GroupCode").val() == '') {
            $("#GroupCode").addClass("input-validation-error");
            $("#errorSpanGroupCode").addClass("error-msg");
            $("#errorSpanGroupCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAttributeGroupCodeRequired"));
            $("#errorSpanGroupCode").show();
            ZnodeBase.prototype.HideLoader();
        }
        else {
            Endpoint.prototype.IsGlobalAttributeGroupCodeExist($("#GroupCode").val(), $("#GlobalAttributeGroupId").val(), function (response) {
                if (!response) {
                    $("#GroupCode").addClass("input-validation-error");
                    $("#errorSpanGroupCode").addClass("error-msg");
                    $("#errorSpanGroupCode").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistAttributeGroupCode"));
                    $("#errorSpanGroupCode").show();
                    isValid = false;
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
        if (!CommonHelper.prototype.Validate())
            isValid = false;

        return isValid;
    }


    OnSelectEntityAutocompleteDataBind(item: any): any {
        if (item != undefined) {
            let entityType: string = item.text;
            let entityId: number = item.Id;
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.GetAttributeGroupList(entityId, entityType, function (response) {
                $("#ZnodeGlobalAttributeGroupList").html("");
                $("#ZnodeGlobalAttributeGroupList").html(response);
                
                ZnodeBase.prototype.HideLoader();
            });
        }
    }
}