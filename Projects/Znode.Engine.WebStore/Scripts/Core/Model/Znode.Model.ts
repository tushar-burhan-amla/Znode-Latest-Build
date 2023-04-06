declare module Znode.Core {
    interface BaseModel {
        uri: string;
        properties: { [key: string]: string };
    }
    interface MultiSelectDDlModel extends BaseModel {
        Value: any;
        IsAjax: boolean;
        IsMultiple: boolean;
        Controller: string;
        Action: string;
        ItemIds: string[];
        SuccessCallBack: any;
        flag: boolean;
    }

    interface ServerConfigurationDDlModel extends BaseModel {
        ServerId: number;
        PartialViewName: string;
        SuccessCallBack: any;
        flag: boolean;
    }

    type ProductDetailModel = {
        ProductId: number;
        MainProductId: number;
        SKU: string;
        MainProductSKU: string;
        Quantity: string;
        MaxQuantity: number;
        MinQuantity: number;
        DecimalValue: number;
        DecimalPoint: number;
        InventoryRoundOff: number;
        QuantityError: string;
    }

    type AddOnDetails = {
        SKU: string;
        ParentSKU: string;
        Quantity: string;
        ParentProductId: number;
    }

    type AddressModel = {
        AddressId: number;
        FirstName: string;
        Address1: string;
        Address2: string;
        LastName: string;
        CityName: string;
        StateName: string;
        PostalCode: string;
        CountryName: string;
        AddressType: string;
        PhoneNumber: string;
        EmailAddress: string;
        AspNetUserId: string;
        UserId: number;
    }

    type PublishProductModel = {
        sku: string;
        type: string;
        PublishProductId: string;
        MinQuantity: string;
        PriceView: boolean;
        ObsoleteClass: string;
    };

    type ReturnOrderLineItemModel = {
        Guid: string;
        ShippedQuantity: string;
        ExpectedReturnQuantity: string;
        RmaReasonForReturnId: number;
        RmaReasonForReturn: string;
        ProductId: number;
        OmsOrderLineItemsId: number;
        TotalLineItemPrice: number;
        RmaReturnLineItemsId: number;
    }

    type CalculateOrderReturnModel = {
        OrderNumber: string;
        CultureCode: string;
        ReturnCalculateLineItemList: ReturnOrderLineItemModel[];
    }

    type OrderReturnModel = {
        OrderNumber: string;
        ReturnNumber: string;
        CultureCode: string;
        Notes: string;
        ReturnLineItems: Array<ReturnOrderLineItemModel>;
    }

    //Region Enhanced Ecommerce
    type EcommerceProductDataModel = {
        id: string;
        name: string;
        category: string;
        brand: string;
        price: string;
    }

    type EcommerceProductListDataModel = {
        id: string;
        name: string;
        category: string;
        brand: string;
        price: string;
        list: string;
    }

    type EcommerceCartItemDataModel = {
        id: string;
        name: string;
        brand: string;
        variant: string;
        quantity: string;
        price: string;
    }

    type EcommercePurchaseItemsDataModel = {
        id: string;
        sku: string;
        name: string;
        quantity: string;
        price: string;
        total: number;
        description: string;
    }
    type EcommercePurchaseActionFieldModel = {
        id: string;
        affiliation: string;
        revenue: string;
        tax: string;
        shipping: string;
        currency: string;
    }

    type ProductImageDetailModel = {
        SKU: string;
        PublishProductId: number;
        Name: string;
        ImageName: string;
        AlternateImageName: string;
    }

    type StockNotificationModel = {
        SKU: string;
        ParentSKU: string;
        EmailId: string
        Quantity: number
    }

    //End Region
}

declare module System.Collections.Generic {
    interface KeyValuePair<TKey, TValue> {
        key: TKey;
        value: TValue;
    }
}
declare module System {
    interface Guid {
    }
    interface Tuple<T1, T2> {
        item1: T1;
        item2: T2;
    }
}

module Constant {
    export const GET = "GET";
    export const json = "json";
    export const Function = "function";
    export const string = "string";
    export const object = "object";
    export const Post = "post"
    export const ZnodeCustomerShipping = "ZnodeCustomerShipping";
    export const AmericanExpressCardCode = "AMEX";
    export const gocoderGoogleAPI = $("#gocoderGoogleAPI").val();//To be fetched from config file
    export const gocoderGoogleAPIKey = $("#gocoderGoogleAPIKey").val();//To be fetched from config file 
    export const Cloudflare = "cloudflare";
    export const CMSDefaultPageSize = "16";
    export const CMSDefaultPageNumber = "1";
    export const PercentOffShipping = "ZnodeCartPromotionPercentOffShipping";
    export const PercentOffShippingWithCarrier = "ZnodeCartPromotionPercentOffShippingWithCarrier";
    export const AmountOffShipping = "ZnodeCartPromotionAmountOffShipping";
    export const AmountOffShippingWithCarrier = "ZnodeCartPromotionAmountOffShippingWithCarrier";
    export const RememberedSearchTerms = "RememberedSearchTerms";
    export const CyberSource = "cybersource";
    export const BrainTree = "braintree";
}

module ErrorMsg {
    export const CallbackFunction = "Callback is not defined. No request made.";
    export const APIEndpoint = "API Endpoint not available: ";
    export const InvalidFunction = "invalid function name : ";

}

