var isFadeOut = true;
var fadeOutTime = 10000;

class RMAReturn extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        var returnNumber: string = $("#hdnReturnNumber").val();
        if (returnNumber != null && returnNumber != "") {
            RMAReturn.prototype.ShowHideButtons(true, true, true);
        }
    }

    //Get Order Details For Return
    public GetOrderDetailsForReturn(item: any): void {
        RMAReturn.prototype.ShowHideButtons(true, true, true);
        var orderNumber: string = item.text;
        ZnodeBase.prototype.ShowLoader();
        $("#divSelectReturnId").html("");
        if (orderNumber != undefined && orderNumber != "" && orderNumber != null) {
            Endpoint.prototype.GetOrderDetailsForReturn(orderNumber, function (response) {
                $("#divSelectReturnId").html(response);
            });
        }
        ZnodeBase.prototype.HideLoader();
    }

    //Validate Return Line Item
    public ValidateReturnLineItem(guid: string, isFromGuestUser: boolean = false) {
        ZnodeBase.prototype.ShowLoader();
        var shippedQuantity = $("#shippedquantity_" + guid).val();
        var expectedReturnQuantity = $("#expectedReturnQuantity_" + guid).val();
        var expectedReturnQuantityError: string = "#expectedReturnQuantity_error_msg_" + guid;
        $(expectedReturnQuantityError).html("");
        $("#returnTotalPrice_" + guid).html("");
        RMAReturn.prototype.ClearTotalSummary();
        if (expectedReturnQuantity == null || expectedReturnQuantity == "") {
            $("#expectedReturnQuantity_" + guid).val("0");
            expectedReturnQuantity = "0";
        }
        if (expectedReturnQuantity == "0") {
            if ($("#returnOrderDetails").length > 0) {
                $("#returnOrderDetails").hide();
                var cultureCode = $("#hdnCurrencyCode").val();
                $("#returnSubtotalId").html(cultureCode + ZnodeBase.prototype.getResourceByKeyName("ZeroValueInDecimal"));
                $("#returnTotalId").html(cultureCode + ZnodeBase.prototype.getResourceByKeyName("ZeroValueInDecimal"));
            }
        }
        if (expectedReturnQuantity != null && expectedReturnQuantity != "") {
            if (this.CheckExpectedReturnQuantityValidations(shippedQuantity, expectedReturnQuantity, expectedReturnQuantityError)) {
                $(expectedReturnQuantityError).html("");
                $(expectedReturnQuantityError).hide();
                RMAReturn.prototype.CalculateOrderReturn(isFromGuestUser);
            }
            else {
                RMAReturn.prototype.ShowHideButtons(false, false, false);
            }
        }
        ZnodeBase.prototype.HideLoader();
    }

    //Delete order return on basis of return number.
    public DeleteOrderReturn(): void {
        var returnNumber: string = $("#hdnReturnNumber").val();
        if (returnNumber != undefined && returnNumber != "" && returnNumber != null) {
            RMAReturn.prototype.DisableButtons(true, true, true);
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.DeleteOrderReturn(returnNumber, function (response) {
                if (response.status) {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/RMAReturn/GetReturnList";
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
                    RMAReturn.prototype.DisableButtons(false, false, false);
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
    }

    //Calculate order return
    public CalculateOrderReturn(isFromGuestUser: boolean = false): void {
        ZnodeBase.prototype.ShowLoader();        
        var guidArray = this.GetGreaterThanZeroReturnQuantityLineItemGuid();
        if (guidArray != undefined && guidArray.length > 0) {
            RMAReturn.prototype.ClearTotalSummary();
            var calculateOrderReturnModel = RMAReturn.prototype.BindCalculateOrderReturnData(guidArray);
            var Url: string = this.SetCalculateOrderReturnUrl(isFromGuestUser);
            Endpoint.prototype.CalculateOrderReturn(Url, calculateOrderReturnModel, function (response) {
                RMAReturn.prototype.BindCalculatedDataResponse(response);
            });
        }
        else {
            var returnNumber: string = $("#hdnReturnNumber").val();
            if (returnNumber != null && returnNumber != "") {
                RMAReturn.prototype.ShowHideButtons(true, false, false);
            }
            else {
                RMAReturn.prototype.ShowHideButtons(false, false, false);
            }
        }
        ZnodeBase.prototype.HideLoader();
    }

    //Save order return.
    public SaveOrderReturn(isSubmitReturn: boolean, isFromGuestUser: boolean = false): void {
        ZnodeBase.prototype.ShowLoader();
        var returnNumber: string = $("#hdnReturnNumber").val();
        var guidArray = [];
        if (returnNumber != undefined && returnNumber != "" && returnNumber != null) {
            if (!isSubmitReturn) {
                if (this.IsAnyChangeInEditReturn() == false) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("MessageChangesSavedSuccessfully"), "success", isFadeOut, fadeOutTime);
                    ZnodeBase.prototype.HideLoader();
                    return;
                }
            }
            guidArray = this.GetReturnQuantityLineItemGuid();
        }
        else {
            guidArray = this.GetGreaterThanZeroReturnQuantityLineItemGuid();
        }

        if (guidArray != undefined && guidArray.length > 0) {
            var isValidOrderLineItem = this.IsValidReturnLineItem(guidArray, true);
            if (isValidOrderLineItem != undefined && isValidOrderLineItem) {
                RMAReturn.prototype.DisableButtons(true, true, true);
                var orderReturnModel = RMAReturn.prototype.BindOrderReturnData(guidArray);
                if (isSubmitReturn) {
                    var url: string = this.SetSubmitOrderReturnUrl(isFromGuestUser);
                    Endpoint.prototype.SubmitOrderReturn(url, orderReturnModel, function (response) {
                        RMAReturn.prototype.DisableButtons(true, true, true);
                        ZnodeBase.prototype.ShowLoader();
                        if (response.hasError) {
                            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.errorMessage, "error", isFadeOut, fadeOutTime);
                            RMAReturn.prototype.DisableButtons(false, false, false);
                        }
                        else {
                            if (isFromGuestUser)
                                window.location.href = window.location.protocol + "//" + window.location.host + "/User/GetReturnDetails?returnNumber=" + response.returnNumber + "&isReturnDetailsReceipt=" + false;
                            else
                                window.location.href = window.location.protocol + "//" + window.location.host + "/RMAReturn/GetReturnDetails?returnNumber=" + response.returnNumber + "&isReturnDetailsReceipt=" + false;
                        }
                    });
                }
                else {
                    Endpoint.prototype.SaveOrderReturn(orderReturnModel, function (response) {
                        if (response.hasError) {
                            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.errorMessage, "error", isFadeOut, fadeOutTime);
                            RMAReturn.prototype.DisableButtons(true, false, false);
                        }
                        else {
                            window.location.href = window.location.protocol + "//" + window.location.host + "/RMAReturn/ManageOrderReturn?returnNumber=" + response.returnNumber;
                        }
                    });
                }
            }
        }
        ZnodeBase.prototype.HideLoader();
    }

    //Print return receipt
    public PrintReturnReceipt(returnNumber: string, isFromGuestUser: boolean = false): any {
        if (returnNumber != null && returnNumber != "") {
            var isReturnDetailsReceipt: boolean = $("#hdnIsReturnsDetailsReceipt").val();
            var url: string = this.SetPrintReturnReceiptUrl(isFromGuestUser);
            Endpoint.prototype.PrintReturnReceipt(url, returnNumber, isReturnDetailsReceipt, function (response) {
                var originalContents = document.body.innerHTML;

                if (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1) {
                    setTimeout(function () { document.body.innerHTML = response }, 1);
                    setTimeout(function () { window.print(); }, 10);
                    setTimeout(function () { document.body.innerHTML = originalContents }, 20);
                    setTimeout(function () { ZnodeBase.prototype.HideLoader(); }, 30);
                }

                else {
                    document.body.innerHTML = response;
                    window.print();
                    document.body.innerHTML = originalContents;
                }
            });
        }
    }
    //Bind calculate order return data
    private BindCalculateOrderReturnData(guidArray: any[]): any {
        if (guidArray.length > 0) {
            var returnOrderLineItemModel = [];
            guidArray.forEach(function (guid) {
                returnOrderLineItemModel.push(RMAReturn.prototype.BindSingleReturnItemModel(guid));
            });
            var _calculateOrderReturnModel: Znode.Core.CalculateOrderReturnModel = {
                OrderNumber: $("#hdnOrderNumber").val(),
                CultureCode: $("#hdnCultureCode").val(),
                ReturnCalculateLineItemList: returnOrderLineItemModel,
            }
            return _calculateOrderReturnModel;
        }
    }

    //Bind order return data
    private BindOrderReturnData(guidArray: any[]): any {
        if (guidArray.length > 0) {
            var returnOrderLineItemModel = [];
            guidArray.forEach(function (guid) {
                returnOrderLineItemModel.push(RMAReturn.prototype.BindSingleReturnItemModel(guid));
            });
            var _orderReturnModel: Znode.Core.OrderReturnModel = {
                OrderNumber: $("#hdnOrderNumber").val(),
                ReturnNumber: $("#hdnReturnNumber").val(),
                CultureCode: $("#hdnCultureCode").val(),
                Notes: $("#ReturnNote").val(),
                ReturnLineItems: returnOrderLineItemModel,
            }
            return _orderReturnModel;
        }
    }

    //Check is valid return line item
    private IsValidReturnLineItem(guidArray: any[], isValidOrderLineItem: boolean): boolean {
        guidArray.forEach(function (guid) {
            var shippedQuantity = $("#shippedquantity_" + guid).val();
            var expectedReturnQuantity = $("#expectedReturnQuantity_" + guid).val();
            var expectedReturnQuantityError: string = "#expectedReturnQuantity_error_msg_" + guid;
            var status = RMAReturn.prototype.CheckExpectedReturnQuantityValidations(shippedQuantity, expectedReturnQuantity, expectedReturnQuantityError);
            if (status == undefined || status == false) {
                isValidOrderLineItem = false;
            }
        });
        return isValidOrderLineItem;
    }

    //Get Greater Than Zero Return Quantity Line Item Guid
    private GetGreaterThanZeroReturnQuantityLineItemGuid() {
        var guidArray = [];
        $("#returnOrderTable tbody tr").each(function () {
            var guid = $(this).attr("data-return-lineitem");
            if ($("#expectedReturnQuantity_" + guid).val() > 0) {
                guidArray.push(guid);
            }
        });
        return guidArray;
    }

    //Get Update Return Quantity Line Item Guid
    private GetReturnQuantityLineItemGuid() {
        var guidArray = [];
        $("#returnOrderTable tbody tr").each(function () {
            var guid = $(this).attr("data-return-lineitem");
            if (($("#RmaReturnLineItemsId_" + guid).val() == 0 && $("#expectedReturnQuantity_" + guid).val() > 0) || ($("#RmaReturnLineItemsId_" + guid).val() > 0)) {
                guidArray.push(guid);
            }
        });
        return guidArray;
    }

    //Get Update Return Quantity Line Item Guid
    private IsAnyChangeInEditReturn(): boolean {
        var status = false;
        if ($("#hdnOldReturnNote").val() != $("#ReturnNote").val()) {
            return true;
        }

        $("#returnOrderTable tbody tr").each(function () {
            var guid = $(this).attr("data-return-lineitem");
            if ($("#expectedReturnQuantity_" + guid).val() != $("#oldExpectedReturnQuantity_" + guid).val() || ($("#RmaReturnLineItemsId_" + guid).val() > 0 && $("#ddlReasonList_" + guid).val() != $("#oldReasonForReturnId_" + guid).val())) {
                status = true
            }
        });
        return status;
    }

    //Bind Single Return Item Model
    private BindSingleReturnItemModel(guid: string): Znode.Core.ReturnOrderLineItemModel {
        var _returnOrderLineItemModel: Znode.Core.ReturnOrderLineItemModel = {
            ShippedQuantity: $("#shippedquantity_" + guid).val(),
            ProductId: parseInt($("#shippedquantity_" + guid).attr("data-return-productid")),
            Guid: guid,
            ExpectedReturnQuantity: $("#expectedReturnQuantity_" + guid).val(),
            RmaReasonForReturnId: $("#ddlReasonList_" + guid).val(),
            RmaReasonForReturn: $("#ddlReasonList_" + guid + " :selected").text(),
            OmsOrderLineItemsId: parseInt($("#OmsOrderLineItemsId_" + guid).val()),
            RmaReturnLineItemsId: parseInt($("#RmaReturnLineItemsId_" + guid).val()),
            TotalLineItemPrice: 0,
        };
        return _returnOrderLineItemModel;
    }

    //Check Expected Return Quantity Validations
    private CheckExpectedReturnQuantityValidations(shippedQuantity: string, expectedReturnQuantity: string, expectedReturnQuantityError: string): boolean {
        var returnQty: number = parseFloat(expectedReturnQuantity);
        var shippedQty: number = parseFloat(shippedQuantity);

        if (expectedReturnQuantity != "") {
            if (!RMAReturn.prototype.CheckIsNumeric(expectedReturnQuantity, expectedReturnQuantityError)) {
                return false;
            }
        }
        else {
            $(expectedReturnQuantityError).text(ZnodeBase.prototype.getResourceByKeyName("ErrorInvalidReturnShippedQuantity"));
            $(expectedReturnQuantityError).show();
            return false;
        }
        if (returnQty != 0 && (returnQty < 1 || returnQty > shippedQty)) {
            $(expectedReturnQuantityError).text(ZnodeBase.prototype.getResourceByKeyName("ErrorInvalidReturnShippedQuantity"));
            $(expectedReturnQuantityError).show();
            return false;
        }
        return true;
    }

    //Check Expected Return Quantity Is Numeric or not
    private CheckIsNumeric(expectedReturnQuantity: string, expectedReturnQuantityError: string): boolean {
        var matches = expectedReturnQuantity.match(/^[0-9]*$/);
        if (matches == null) {
            $(expectedReturnQuantityError).text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            $(expectedReturnQuantityError).addClass("error-msg")
            $(expectedReturnQuantityError).show();
            return false;
        }
        return true;
    }

    //Disable Delete, Save and Submit Buttons based on flag
    private DisableButtons(isDeleteButtonDisable: boolean, isSaveButtonDisable: boolean, isSubmitButtonDisable: boolean) {
        $("#btnDeleteReturn").prop("disabled", isDeleteButtonDisable);
        $("#btnSaveReturn").prop("disabled", isSaveButtonDisable);
        $("#btnSubmitReturn").prop("disabled", isSubmitButtonDisable);
    }

    //Show Delete, Save and Submit Buttons based on flag
    private ShowHideButtons(isDeleteButtonShow: boolean, isSaveButtonShow: boolean, isSubmitButtonShow: boolean) {
        if (isDeleteButtonShow) { $("#btnDeleteReturn").show(); }
        else { $("#btnDeleteReturn").hide(); }

        if (isSaveButtonShow) { $("#btnSaveReturn").show(); }
        else { $("#btnSaveReturn").hide(); }

        if (isSubmitButtonShow) { $("#btnSubmitReturn").show(); }
        else { $("#btnSubmitReturn").hide(); }
    }

    //Bind calculated data response
    private BindCalculatedDataResponse(response: any): void {
        $("#divReturnCalculation").html("").html(response.html);
        var calculateLineItemList = response.calculateLineItemList;

        if (calculateLineItemList != undefined && calculateLineItemList.length > 0) {
            for (var index = 0; index < calculateLineItemList.length; index++) {
                $("#returnTotalPrice_" + calculateLineItemList[index].Guid).html(calculateLineItemList[index].TotalLineItemPriceWithCurrency);
                $("#expectedReturnQuantity_error_msg_" + calculateLineItemList[index].Guid).html(calculateLineItemList[index].ErrorMessage);
            }
            var returnNumber: string = $("#hdnReturnNumber").val();
            if (returnNumber != null && returnNumber != "") {
                RMAReturn.prototype.ShowHideButtons(true, true, true);
            }
            else {
                RMAReturn.prototype.ShowHideButtons(false, true, true);
            }
            for (var index = 0; index < calculateLineItemList.length; index++) {
                if (calculateLineItemList[index].ErrorMessage != null)
                    RMAReturn.prototype.ShowHideButtons(false, false, false);
            }
        }
        if (response.hasError) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.errorMessage, "error", isFadeOut, fadeOutTime);
            RMAReturn.prototype.ShowHideButtons(false, false, false);
        }
    }

    //Clear Total Summary
    private ClearTotalSummary() {
        var defaultPrice: string = $("#hdnDefaultPrice").val();
        $("#returnSubtotalId").html(defaultPrice);
        $("#returnTaxCostId").html("+ "+defaultPrice+"");
        $("#returnShippingCostId").html("+ " + defaultPrice +"");
        $("#returnTotalId").html(defaultPrice);
        $("#returnDiscountAmountId").html("- " + defaultPrice +"");
        $("#returnCSRDiscountAmountId").html("- " + defaultPrice +"");
        $("#returnShippingDiscountAmountId").html("- " + defaultPrice +"");
        $("#returnChargesAmountId").html("- " + defaultPrice +"");


    }

    public ValidateGuestReturn(orderNumber: string): boolean {
        if (orderNumber != undefined && orderNumber != null) {
            Endpoint.prototype.ValidateGuestUserReturn(orderNumber, function (response) {
                if (response.hasError) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.errorMessage, "error", isFadeOut, fadeOutTime);
                    return false;
                }
                else { 
                    if (response.isGuestUser)
                        window.location.href = window.location.protocol + "//" + window.location.host + "/User/GetOrderDetailsForReturn?orderNumber=" + orderNumber.trim();
                    else
                        window.location.href = window.location.protocol + "//" + window.location.host + "/RMAReturn/GetOrderDetailsForReturn?orderNumber=" + orderNumber.trim();
                }
            });
        }
        return true;
    }

    public CheckOrderEligibilityForReturn() {
        var orderNumber: string = $("#OrderNumber").val();
        if (orderNumber != undefined && orderNumber != null) {
            Endpoint.prototype.CheckOrderEligibilityForReturn(orderNumber.trim(), function (response) {
                if (response.isEligible) {
                    $("#btnCreateReturn").prop("disabled", false);
                }
                else {
                    $("#btnCreateReturn").prop("disabled", true);
                }
            });
        }
    }

    private SetCalculateOrderReturnUrl(isFromGuestUser: boolean) {
        var Url: string = "";
        if (isFromGuestUser)
            Url = "/User/CalculateOrderReturn";
        else
            Url = "/RMAReturn/CalculateOrderReturn";
        return Url;
    }

    private SetSubmitOrderReturnUrl(isFromGuestUser: boolean) {
        var Url: string = "";
        if (isFromGuestUser)
            Url = "/User/SubmitOrderReturn";
        else
            Url = "/RMAReturn/SubmitOrderReturn";
        return Url;
    }

    private SetPrintReturnReceiptUrl(isFromGuestUser: boolean) {
        var Url: string = "";
        if (isFromGuestUser)
            Url = "/User/PrintReturnReceipt";
        else
            Url = "/RMAReturn/PrintReturnReceipt";
        return Url;
    }


}