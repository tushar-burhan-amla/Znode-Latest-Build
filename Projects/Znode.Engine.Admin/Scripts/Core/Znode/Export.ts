class Export extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
        this._endPoint = new Endpoint();
    }
     //Delete Export process Log details.
    DeleteExportLogs(control): any {
        let exportProcessLogId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (exportProcessLogId.length > 0) {
            Endpoint.prototype.DeleteExportLogs(exportProcessLogId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }
    SetActiveDeActive(): any {
        $("[data-swhgcontainer='ZnodeExportProcessLog'] tbody tr").each(function () {
            var items = $(this).find('td.IsCompletedorNot').text();
            if (items != "Completed") {
                $(this).find('.z-download').attr("disabled", true).css({ "pointer-events": "none", "opacity": "0.5"  });
                $(this).find('.z-delete').attr("disabled", true).css({ "pointer-events": "none" , "opacity" : "0.5" });
                $(this).find('.grid-row-checkbox').attr("disabled", true);
            }
        });
    }

    
}
