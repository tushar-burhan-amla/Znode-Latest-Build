
class Dashboard extends ZnodeBase {
    _endPoint: Endpoint;
    _Model: any;

    constructor() {
        super();
        this._endPoint = new Endpoint();
    }
    GetSelectedPortal()
    {
        var portalIds = $("#ddlPortal").val();
        var durationId = $("#ddlDuration").val();
        if (portalIds == 0) {
            var values = $.map($('#ddlPortal option'), function (e) { return e.value; });
            portalIds = values.join(',');
        }
        return portalIds;
    }
    DisplayLowInventoryProductReport() {
        var url = window.location.protocol + "//" + window.location.host + "/MyReports/GetDashboardReport?reportPath=InventoryReorder&reportName=Inventory Re-order&portalIds=" + Dashboard.prototype.GetSelectedPortal() + "&durationId=" + $("#ddlDuration").val();
        window.open(url, '_blank');
    }

    GetSelectedDashboardPortal(item: any): any {
        if (item != undefined) {
            $("#hdnPortal").val(item.Id);
        }
        Dashboard.prototype.GetSalesDetailsBasedOnSelectedPortalAndAccount($("#hdnPortal").val(), $("#hdnAccount").val());
    }

    GetSelectedDashboardAccount(item: any): any {
        if (item != undefined) {
            $("#hdnAccount").val(item.Id);
        }
        Dashboard.prototype.GetSalesDetailsBasedOnSelectedPortalAndAccount($("#hdnPortal").val(), $("#hdnAccount").val());
    }

    GetSalesDetailsBasedOnSelectedPortalAndAccount(portalId: number, accountId: number): any {
        Endpoint.prototype.SalesDetailsBasedOnSelectedPortalAndAccount(portalId, accountId, function (response) {
            $("#QuotesView").html(response.quotes);
            $("#OrdersView").html(response.orders);
            $("#ReturnsView").html(response.returns);
            $("#TopAccountView").html(response.topaccounts);
            $("#SalesView").html(response.sales);
        });
        //Get low inventory products on the basis of selected portal.
        Dashboard.prototype.GetDashboardLowInventoryProductCountOnSelectedPortal();
    }

    GetDashboardLowInventoryProductCountOnSelectedPortal() {
        Endpoint.prototype.DashboardLowInventoryProductCountOnSelectedPortal(parseInt($("#ddlPortal").val()), function (response) {
            $("#LowInventoryCount").html(response.html);           
        });
    }

    SetLink() {
        var _newUrl = MediaManagerTools.prototype.UpdateQueryString("portalId", $("#ddlPortal").val(), window.location.href);
        window.history.pushState({ path: _newUrl }, '', _newUrl);
    }
}