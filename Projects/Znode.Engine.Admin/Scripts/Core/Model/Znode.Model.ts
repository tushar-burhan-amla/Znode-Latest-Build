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
        SKU: string;
        MainProductSKU: string;
        Quantity: string;
        DecimalValue: number;
        DecimalPoint: number;
        InventoryRoundOff: number;
        QuantityError: string;
        MainProductId: number;
    }

    type OrderLineItemModel = {
        OrderId: number;
        Guid: string;
        Quantity: string;
        CustomQuantity: string;
        UnitPrice: number;
        TrackingNumber: string;
        OrderLineItemStatusId: number;
        ReasonForReturnId: number;
        OrderLineItemStatus: string;
        ReasonForReturn: string;
        ProductId: number;
        IsShippingReturn: boolean;
        PartialRefundAmount: string;
        OrderLineItemShippingCost: number;
        IsOrderLineItemShipping: boolean;
        OriginalOrderLineItemShippingCost: number;
    }

    type CartModel = {
        PublishProductId: number;
        SKU: string;
        ProductName: string;
        Quantity: number;
        ProductType: string;
    }

    type PortalPaymentApproverViewModel = {
        ApprovalUserIds: string[];
        PaymentSettingIds: string[];
        PortalPaymentGroupId: number;
    }

    type ApproverDetailsViewModel = {
        EnableApprovalManagement: boolean;
        ApprovalUserIds: string[];
        PortalApprovalTypeId: number;
        PortalApprovalLevelId: number;
        OrderLimit: number;
        PortalId: number;
        PortalPaymentGroupId: number;
        PortalApprovalId: number;
        PaymentTypeIds: string[];
        PortalPaymentUserApproverList: Array<PortalPaymentApproverViewModel>
    }

    type ProductParameterModel = {
        PublishProductId: number;
        LocaleId: number;
        PortalId: number;
        UserId: number;
        OMSOrderId: number;
    }

    type AddOnDetails = {
        ParentSKU: string;
        SKU: string;
        Quantity: string;
        ParentProductId: number;
    }

    type ReturnOrderLineItemModel = {
        Guid: string;
        ReturnedQuantity: string;
        OmsOrderLineItemsId: number;
        RmaReturnLineItemsId: number;
        IsShippingReturn: boolean;
        RmaReturnStateId: number;
        RefundAmount: number;
        ReturnStatus: string;
        ShippedQuantity: number;
        ProductId: number;
        ExpectedReturnQuantity: number;
        RmaReasonForReturnId: number;
        RmaReasonForReturn: string;
        TotalLineItemPrice: number;
    }

    type QuoteLineItemModel = {
        OmsQuoteId: number;
        Guid: string;
        Quantity: string;
        UnitPrice: number;
        ProductId: number;
        ShippingCost: number;
        IsShippingEdit: boolean;
    }

    type ConvertQuoteToOrderViewModel = {
        OmsQuoteId: number;
        UserId: number;
        AdditionalInstructions: string;
        PaymentDetails: PaymentDetails;
    }
    type PaymentDetails = {
        PaymentSettingId: number;
        paymentType: string;
        PurchaseOrderNumber: string;
        PODocumentName: string;
    }

    type CalculateOrderReturnModel = {
        OrderNumber: string;
        CultureCode: string;
        ReturnCalculateLineItemList: ReturnOrderLineItemModel[];
        PortalId: number;
        UserId: number;
    }

    type OrderReturnModel = {
        OrderNumber: string;
        ReturnNumber: string;
        CultureCode: string;
        Notes: string;
        ReturnLineItems: Array<ReturnOrderLineItemModel>;
        PortalId: number;
        UserId: number;
        OmsOrderDetailsId: number;
    }

    type ConfigurationModel = {
        CMSMappingId: number;
        CMSWidgetsId: number;
        WidgetKey: string;
        TypeOFMapping: string;
        DisplayName: string;
        WidgetName: string;
        FileName: string;
        ContainerKey: string;
    }

    type UserDetailsViewModel = {
        UserName: string;
        UserId: number;
        PortalId: number;
    }
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
    export const innerLoderHtml = "<div class='loader-inner' style='margin:0 auto;text-align:center;padding:20px;'><img src='../Content/Images/throbber.gif' alt='Loading' class='dashboard-loader' /></div>"
    export const configurableProduct = "ConfigurableProduct";
    export const simpleProduct = "SimpleProduct";
    export const groupedProduct = "GroupedProduct";
    export const bundleProduct = "BundleProduct";
    export const addOns = "AddOns";
    export const ZnodeCustomerShipping = "ZnodeCustomerShipping";
    export const GuestUser = "Guest User";
    export const gocoderGoogleAPI = $("#gocoderGoogleAPI").val();//To be fetched from config file
    export const gocoderGoogleAPIKey = $("#gocoderGoogleAPIKey").val();//To be fetched from config file 
    export const inventory = "Inventory";
    export const category = "Category";
    export const seo = "SEO";
    export const defaultAdmin = "YWRtaW5Aem5vZGUuY29t";
    export const CATALOG = "Catalog";
    export const image = "Image";
    export const AmericanExpressCardCode = "AMEX";
    export const shippingSettings = "ShippingSettings";
    export const productSetting = "ProductSetting";
    export const productDetails = "ProductDetails";
    export const storelist = "Storelist";
    export const creditCardNoHidden = "XXXX-XXXX-XXXX-";
    export const creditCardPaymentCode = "credit_card";
    export const Cloudflare = "cloudflare";
    export const categoryXMLSitemap = "CategoryXMLSitemap";
    export const contentPagesXMLSitemap = "ContentPagesXMLSitemap";
    export const productXMLSitemap = "ProductXMLSitemap";
    export const allXMLSitemap = "AllXMLSitemap";
    export const googleProductFeed = "GoogleProductFeed";
    export const bingProductFeed = "BingProductFeed";
    export const shoppingProductFeed = "ShoppingProductFeed";
    export const analyticsChartStartDate = "30daysAgo";
    export const analyticsChartEndDate = "today";
    export const analyticsTableChartMaxResults = 10;
    export const xmlProductFeed = "XmlProductFeed";
    export const CyberSource = "cybersource";
    export const BrainTree = "braintree";
    export const AdminOrderCreate = "AdminOrderCreate";
    export const AdminOrderManage = "AdminOrderManage";
}

module ErrorMsg {
    export const CallbackFunction = "Callback is not defined. No request made.";
    export const APIEndpoint = "API Endpoint not available: ";
    export const InvalidFunction = "invalid function name : ";
    export const ErrorMessageForCategoryCode = "Alphanumeric values are allowed,Must contain at least one alphabet in CategoryCode."
}

module Enum {
    export enum OrderStatusDropdown {
        PENDING = "5",
        RECEIVED = "11",
        SUBMITTED = "10",
        SHIPPED = "20",
        RETURNED = "30",
        CANCELED = "40",
        PENDINGAPPROVAL = "50",
        APPROVED = "60",
        REJECTED = "70",
        INREVIEW = "80",
        DRAFT = "90",
        ORDERED = "100",
        PARTIALREFUND = "110",
        SENDING = "120",
        ORDERRECEIVED = "130",
        FAILED = "140",
        INPROGRESS = "150",
        INPRODUCTION = "160",
        WAITINGTOSHIP = "170",
        INVOICED = "180",
        PENDINGPAYMENT = "190",
        EXPIRED = "200"
    }

    export enum ReturnStatusDropdown {
        SUBMITTED = "10",
        INREVIEW = "30",
        RECEIVED = "40",
        APPROVED = "50",
        REJECTED = "60",
        PARTIALLYAPPROVED = "70",
        REFUNDPROCESSED = "80",
    }

    export enum ProductFeedType {
        XmlSiteMap,
        Google,
        Bing,
        Xml,
    }

    export enum ProductFeedSiteMapType {
        Category,
        Content,
        Product,
        ALL
    }  
}
