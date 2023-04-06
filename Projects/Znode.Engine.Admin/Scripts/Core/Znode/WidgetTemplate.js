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
var WidgetTemplate = /** @class */ (function (_super) {
    __extends(WidgetTemplate, _super);
    function WidgetTemplate() {
        return _super.call(this) || this;
    }
    WidgetTemplate.prototype.DeleteWidgetTemplate = function (control) {
        var containerTemplateId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var fileName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn('File Name');
        if (containerTemplateId.length > 0) {
            Endpoint.prototype.DeleteWidgetTemplate(containerTemplateId, fileName, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    };
    WidgetTemplate.prototype.ValidateTemplateCode = function () {
        var isValid = true;
        var widgetTemplateId = $('#ContainerTemplateId').val();
        if (widgetTemplateId < 1 && $("#Code").val() != '') {
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
    };
    return WidgetTemplate;
}(ZnodeBase));
//# sourceMappingURL=WidgetTemplate.js.map