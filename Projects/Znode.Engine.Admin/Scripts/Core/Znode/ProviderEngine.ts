class ProviderEngine extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        if (window.location.href.indexOf('PromotionTypeList')>0 )
        {
            $.cookie('_backURL', '', { path: '/' }); 
        }
    }


    GetPromotionTypeByClassName(): any {
        var className = $("#ddlPromotionType").val();
        Endpoint.prototype.GetPromotionTypeDetails(className, function (response) {
            if (response != null) {
                $("#Name").val(response.Name);
                $("#ClassName").val(response.ClassName);
                $("#Description").val(response.Description);
                $("#ClassType").val(response.ClassType);
            }
        });
    }

    GetTaxRuleTypeByClassName(): any {
        var className = $("#ddlTaxType").val();
        Endpoint.prototype.GetTaxRuleTypeDetails(className, function (response) {
            if (response != null) {
                $("#Name").val(response.Name);
                $("#ClassName").val(response.ClassName);
                $("#Description").val(response.Description);
            }
        });
    }

    GetShippingTypeByClassName(): any {
        var className = $("#ddlShippingType").val();
        Endpoint.prototype.GetShippingTypeDetails(className, function (response) {
            if (response != null) {
                $("#Name").val(response.Name);
                $("#ClassName").val(response.ClassName);
                $("#Description").val(response.Description);
            }
        });
    }

    DeleteTaxRuleType(control): any {
        var taxRuletypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        var url = "/ProviderEngine/DeleteTaxRuleType";
        if (taxRuletypeIds.length > 0) {
            Endpoint.prototype.DeleteTaxRuleTypes(taxRuletypeIds, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
            });
        }
    }

    DeleteShippingType(control): any {
        var shippingTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        var url = "/ProviderEngine/DeleteShippingType";
        if (shippingTypeIds.length > 0) {
            Endpoint.prototype.DeleteShippingTypes(shippingTypeIds, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
            });
        }
    }

    DeletePromotionType(control): any {
        var promotionTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        var url = "/ProviderEngine/DeletePromotionType";
        if (promotionTypeIds.length > 0) {
            Endpoint.prototype.DeletePromotionTypes(promotionTypeIds, function (res) {
                DynamicGrid.prototype.RefreshGrid(control, res);
            });
        }
    }

    EnableTaxRuleType(control): any {
        var taxruleTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxruleTypeIds.length > 0) {
            Endpoint.prototype.EnableTaxRuleTypes(taxruleTypeIds, true, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/TaxRuleTypeList";
            });
        }
    }

    DisableTaxRuleType(control): any {
        var taxruleTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (taxruleTypeIds.length > 0) {
            Endpoint.prototype.EnableTaxRuleTypes(taxruleTypeIds, false, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/TaxRuleTypeList";
            });
        }
    }

    EnableShippingType(control): any {
        var shippingTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (shippingTypeIds.length > 0) {
            Endpoint.prototype.EnableShippingTypes(shippingTypeIds, true, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/ShippingTypeList";
            });
        }
    }

    DisableShippingType(control): any {
        var shippingTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (shippingTypeIds.length > 0) {
            Endpoint.prototype.EnableShippingTypes(shippingTypeIds, false, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/ShippingTypeList";
            });
        }
    }

    EnablePromotionType(control): any {
        var promotionTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (promotionTypeIds.length > 0) {
            Endpoint.prototype.EnablePromotionTypes(promotionTypeIds, true, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/PromotionTypeList";
            });
        }
    }

    DisablePromotionType(control): any {
        var promotionTypeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (promotionTypeIds.length > 0) {
            Endpoint.prototype.EnablePromotionTypes(promotionTypeIds, false, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/ProviderEngine/PromotionTypeList";
            });
        }
    }
}