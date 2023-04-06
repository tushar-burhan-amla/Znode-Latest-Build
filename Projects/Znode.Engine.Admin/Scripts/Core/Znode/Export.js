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
var Export = /** @class */ (function (_super) {
    __extends(Export, _super);
    function Export() {
        var _this = _super.call(this) || this;
        _this._endPoint = new Endpoint();
        return _this;
    }
    //Delete Export process Log details.
    Export.prototype.DeleteExportLogs = function (control) {
        var exportProcessLogId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (exportProcessLogId.length > 0) {
            Endpoint.prototype.DeleteExportLogs(exportProcessLogId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    };
    Export.prototype.SetActiveDeActive = function () {
        $("[data-swhgcontainer='ZnodeExportProcessLog'] tbody tr").each(function () {
            var items = $(this).find('td.IsCompletedorNot').text();
            if (items != "Completed") {
                $(this).find('.z-download').attr("disabled", true).css({ "pointer-events": "none", "opacity": "0.5" });
                $(this).find('.z-delete').attr("disabled", true).css({ "pointer-events": "none", "opacity": "0.5" });
                $(this).find('.grid-row-checkbox').attr("disabled", true);
            }
        });
    };
    return Export;
}(ZnodeBase));
//# sourceMappingURL=Export.js.map