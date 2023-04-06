class CustomEndpoint extends ZnodeBase {

    constructor() {
        super();
    }

    GetStateByCountryCode(countryCode: string, callbackMethod) {
        super.ajaxRequest("/CustomAccount/GetStateByCountryCode", "get", { "countryCode": countryCode }, callbackMethod, "json", false);
    }

    DeleteDataCaptureDefaultValues(defaultvalueId: number, callbackMethod: any) {
        super.ajaxRequest("/CustomStoreDataCapture/DeleteDefaultValues",
            "get",
            {
                 "defaultvalueId": defaultvalueId
            },
            callbackMethod,
            "json");
    }

    SaveGlobalAttributeDefaultValues(
        data: any, attributeId: number, defaultvaluecode: string, defaultvalueId: number, displayOrder: number, isDefault: boolean, callbackMethod: any) {
        super.ajaxRequest("/CustomStoreDataCapture/SaveDefaultValues",
            "get",
            {
                "model": JSON.stringify(data),
                "attributeId": attributeId,
                "defaultvalueId": defaultvalueId,
                "defaultvaluecode": defaultvaluecode,
                "displayOrder": displayOrder,
                "isdefault": isDefault
            },
            callbackMethod,
            "json");
    }

    GetPortalCustomizationSettings(portalId: number, callbackMethod) {
        super.ajaxRequest("/CustomOrder/GetProductCustomizationSetting", "get", { "portalId": portalId }, callbackMethod, "json");
    }

    GetProduct(parameters, callbackMethod) {
        super.ajaxRequest("/CustomOrder/GetConfigurableProduct",
            "post",
            { "model": parameters },
            callbackMethod,
            "html");
    }

    DeleteCartItem(guidId: string, orderId: number, isQuote: boolean, callbackMethod) {
        super.ajaxRequest("/CustomOrder/RemoveShoppingCartItem", "post", { "guidId": guidId, "orderId": orderId, "isQuote": isQuote }, callbackMethod, "json");
    }
}