class PIMAttributeGroup extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    DeletePIMAttributeGroup(contollerName, control): any {
        var pimAttributeGroupIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (pimAttributeGroupIds.length > 0) {
            this._endPoint.DeletePIMAttributeGroup(pimAttributeGroupIds, contollerName, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateGroupCode(): any {
        $("#GroupCode").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            PIMAttributeGroup.prototype.ValidateAttributeGroupCode();
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
            Endpoint.prototype.IsAttributeGroupCodeExist($("#GroupCode").val(), $("#IsCategory").val(), $("#PimAttributeGroupId").val(), function (response) {
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
}