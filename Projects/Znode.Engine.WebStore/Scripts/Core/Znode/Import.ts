class Import extends ZnodeBase {
    constructor() {
        super();
    }

    ValidateImportFile(): boolean {        
        if ($("#ImportData").val() == "") {
            $("#importErrorFileTypeAndSize").html(ZnodeBase.prototype.getResourceByKeyName("FileNotPresentError"));
            return false;
        }
    }

    //Delete shipping logs by log ids.
    DeleteImportLogs(control): any {
        var importProcessLogId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (importProcessLogId.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.DeleteImportLogs(importProcessLogId, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Delete user logs by log ids.
    DeleteUserImportLogs(control): any {
        var importProcessLogId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (importProcessLogId.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.DeleteImportLogs(importProcessLogId, function (res) {
                ZnodeBase.prototype.HideLoader();
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeUserImportProcessLog").find("#refreshGrid"), res);
            });
        }
    }
}
