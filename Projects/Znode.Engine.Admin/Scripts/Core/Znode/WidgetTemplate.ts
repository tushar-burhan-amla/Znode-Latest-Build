class WidgetTemplate extends ZnodeBase {
    constructor() {
        super();
    }

    
    DeleteWidgetTemplate(control): any {
        var containerTemplateId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var fileName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn('File Name');
        if (containerTemplateId.length > 0) {
            Endpoint.prototype.DeleteWidgetTemplate(containerTemplateId, fileName, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateTemplateCode(): boolean {
        var isValid = true;
        var widgetTemplateId = $('#ContainerTemplateId').val()

        if (widgetTemplateId < 1 && $("#Code").val() !='') {
            Endpoint.prototype.IsContainerTemplateExist($("#Code").val(), function (response) {
                if (response.data) {
                    $("#Code").addClass("input-validation-error");
                    $("#valCode").addClass("error-msg");
                    $("#valCode").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistContainerTemplate"));
                    $("#valCode").show();
                    isValid = false;
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
        return isValid;
    }
}