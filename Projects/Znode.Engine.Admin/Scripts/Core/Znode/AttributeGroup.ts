class AttributeGroup extends ZnodeBase {
    _endpoint: Endpoint;

    constructor() {
        super();
        this._endpoint = new Endpoint();
    }

    DeleteAttributeGroup(control): any {
        var mediaAttributeGroupIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (mediaAttributeGroupIds.length > 0) {
            Endpoint.prototype.DeleteMediaAttributeGroup(mediaAttributeGroupIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    Init() {
        AttributeGroup.prototype.ValidateGroupCode();
        $("#GroupCodeText").on('keypress blur change', function (e) {
            $("#GroupCode").val($("#GroupCodeText").val());
        });

        $("#dvSave").on("click",function () {
            if ($("#GroupCodeLable").text() == "") {
                if ($("#GroupCodeText").val()) {
                    $("#GroupCodeErrormessage").html("");
                    return true;
                } else {
                    $("#GroupCodeErrormessage").html("Please enter Group Code.");
                    return false;
                }
            }
        });
    }

    ValidateGroupCode(): any {
        $("#GroupCode").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            AttributeGroup.prototype.ValidateMediaAttributeGroupCode();
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidateMediaAttributeGroupCode(): boolean {
        var isValid = true;
        if ($("#GroupCode").val() == '') {
            $("#GroupCode").addClass("input-validation-error");
            $("#errorSpanGroupCode").addClass("error-msg");
            $("#errorSpanGroupCode").text(ZnodeBase.prototype.getResourceByKeyName("ErrorAttributeGroupCodeRequired"));
            $("#errorSpanGroupCode").show();
            ZnodeBase.prototype.HideLoader();
        }
        else {
            Endpoint.prototype.IsMediaAttributeGroupCodeExist($("#GroupCode").val(), $("#MediaAttributeGroupId").val(), function (response) {
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
