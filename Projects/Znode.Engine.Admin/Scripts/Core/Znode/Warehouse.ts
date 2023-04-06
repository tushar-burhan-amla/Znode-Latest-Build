class Warehouse extends ZnodeBase {
    _endPoint: Endpoint;
    _Model: any;


    constructor() {
        super();
    }

    Init() {
        Account.prototype.BindStates();
    }

    DeleteWarehouse(control): any {
        var warehouseIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (warehouseIds.length > 0) {
            Endpoint.prototype.DeleteWarehouse(warehouseIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteMultipleSKUInventory(control): any {
        var inventoryId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (inventoryId.length > 0) {
            Endpoint.prototype.DeleteMultipleAssociatedSkus(inventoryId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Restrict multiple click while associating inventory to warehouse.
    RestrictDoubleClickForInventory(): any {
        $("#UnassociatedInventory td a").click(function (e) {
            $('#associateinventorylist').hide(700);
        });
    }

    //Hide filter if no record found.
    HideInventoryFilter(): any {
        if ($("#UnassociatedInventory").find("tr").length == 0) {
            $("#UnassociatedInventory").find(".filter-component").hide();
        }
    }
}