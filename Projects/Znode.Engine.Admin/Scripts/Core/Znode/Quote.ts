class Quote extends ZnodeBase {
    constructor() {
        super();
    }

    Init(): any {
        Order.prototype.Init();
        ZnodeDateRangePicker.prototype.Init(Quote.prototype.DateTimePickerRange());
    }

    HideConvertToOrderColumn(): void {
        $('#grid tbody tr').each(function () {
            $(this).find("td").each(function () {
                if ($(this).hasClass('grid-action')) {
                    if ($(this).next().children().hasClass("z-active")) {
                        $(this).children().children("ul").children().find(".z-orders").parent().hide();
                    }
                }
            });
            $(this).find("td.IsConvertedToOrder").each(function () {
                if ($(this).children("i").hasClass("z-active")) {
                    $(this).next().children().children("ul").children().find(".z-orders").parent().hide();
                }
            });
        });
        $('#grid th').each(function () {
            if ($.trim($(this).text()) == "Is Converted To Order")
                $(this).hide();
        });
        $('#grid').find(".IsConvertedToOrder").hide();
    }

    //This method is used to select store from fast select and show it on textbox
    OnSelectPortalDataBind(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: number = item.Id;
            $('#txtPortalName').val(portalName);
            $('#hdnPortalId').val(portalId);
        }

        Quote.prototype.GetSearchQuoteListByPortalId();
    }

    //To get the quote list when any selection is made from fast select control.
    GetSearchQuoteListByPortalId(): any {

        Quote.prototype.SetFastSelectFilter("portalid", $("#hdnPortalId").val());
        Quote.prototype.SubmitFormOnFastSelection();
        ZnodeDateRangePicker.prototype.Init(Quote.prototype.DateTimePickerRange());
    }

    DateTimePickerRange(): any {
        var ranges = {
            'All Quotes': [],
            'Last Hour': [],
            'Last Day': [],
            'Last 7 Days': [],
            'Last 30 Days': [],
        }
        return ranges;
    }

    //To set filters related to fast select control present on _FilterComponent.cshtml page.
    SetFastSelectFilter(filterName: string, filterValue: string): any {
        $("#fastSelectFilterName").attr({
            "name": filterName,
            "value": filterValue
        });
        $("#fastSelectFilterOperator").attr({
            "name": "DataOperatorId",
            "value": "1"
        });
    }

    //To submit the form when any selection is made from fast select control.
    SubmitFormOnFastSelection() {
        UpdateContainerId = $("#fastSelectFilterName").closest('form').attr('data-ajax-update').replace("#", "");
        $("#fastSelectFilterName").closest("form").submit();
    }

    GetQuoteId(): number {
        var omsquoteId = $("#hdnManageOmsQuoteId").val();
        if (omsquoteId != null && omsquoteId != "") {
            let quoteId: number = parseInt(omsquoteId);
            if (quoteId > 0) {
                return quoteId;
            }
        }
        return 0;
    }

    public GetPortalId(): number {
        return $("#txtPortalName").attr("data-portalid") != undefined ? parseInt($("#txtPortalName").attr("data-portalid")) : parseInt($("#PortalId").val());
    }

    public GetUserId(): number {
        return $('#hdnUserId').val() == undefined ? parseInt($("#labelCustomerId").text().trim()) : parseInt($("#hdnUserId").val());
    }

    //Save Manage Quote Details
    SubmitForm(data): any {
        $(data).closest("form").submit();
    }

    CancelQuote(): any {
        if ($('#ddlQuoteStatus').val() != Enum.OrderStatusDropdown.CANCELED) {
            var form = $('#QuoteStatus');
            form.find('#SelectedItemValue').val(ZnodeBase.prototype.getResourceByKeyName("Canceled"));
            form.find('#SelectedItemId').val(Enum.OrderStatusDropdown.CANCELED);
            $('#ddlQuoteStatus').val(Enum.OrderStatusDropdown.CANCELED);
            form.submit();
        }
    }

    BindQuoteData(): any {
        Quote.prototype.DisableManageQuoteControls();
        Quote.prototype.GetQuoteShippingList();
        if ($('#hdnIsConvertedToOrder').val() != undefined && $('#hdnIsConvertedToOrder').val() == "True")
            Quote.prototype.FreezeManageQuote(true);
    }

    public OnQuoteStatusChange(obj): void {

        var currentform = $(obj).closest("form").children();
        var selected = $(obj).find('option:selected');

        if (selected.val() == Enum.OrderStatusDropdown.SUBMITTED) {

            var selectedstatus = currentform.find('#SelectedItemValue').val();
            $(obj).val($(obj).find("option:contains('" + selectedstatus + "')").attr('value'));

            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorOnSelectSubmitted"), "error", isFadeOut, fadeOutTime);
        }
        else if (selected.val() == Enum.OrderStatusDropdown.EXPIRED) {

            var selectedstatus = currentform.find('#SelectedItemValue').val();
            $(obj).val($(obj).find("option:contains('" + selectedstatus + "')").attr('value'));

            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorOnSelectExpired"), "error", isFadeOut, fadeOutTime);
        }
        else if (selected.val() == Enum.OrderStatusDropdown.APPROVED) {
            if (!Quote.prototype.ValidateQuote()) {

                var selectedstatus = currentform.find('#SelectedItemValue').val();
                $(obj).val($(obj).find("option:contains('" + selectedstatus + "')").attr('value'));
            } else {
                Quote.prototype.BindSelectedOptionForStatus(obj);
            }
        }
        else {
            if (obj.id == "ddlQuoteStatus") {
                Quote.prototype.BindSelectedOptionForStatus(obj);
            }
        }
    }

    public OnAccountQuoteStatusChange(obj): void {

        var quoteCurrentStatus = $(obj).find('option:selected');

        $("#hdnOmsOrderStateId").val(quoteCurrentStatus.val())

    }

    ValidateQuote(): any {
        var omsQuoteId: number = Quote.prototype.GetQuoteId();
        if ($('#hdnActiveProductCount').val() > 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorInActiveProduct"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        if ($("#hndShippingclassName").val() != undefined && $("#hndShippingclassName").val() == Constant.ZnodeCustomerShipping && omsQuoteId > 0) {
            if ($("#txtShippingMethod").val() == undefined || $("#txtShippingMethod").val() == null || $("#txtShippingMethod").val() == "") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterShippingMethod"), 'error', isFadeOut, fadeOutTime);
                return false;
            }
            if ($("#txtAccountNumber").val() == undefined || $("#txtAccountNumber").val() == null || $("#txtAccountNumber").val() == "") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorEnterAccountNumber"), 'error', isFadeOut, fadeOutTime);
                return false; 
            }
        }
        if ($("#dynamic-allowesterritories").length > 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AllowedTerritories"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        if ($("#hdnIsSaveQuote").val() != undefined && $("#hdnIsSaveQuote").val() == "false") {
            return false;
        }
        return true;
    }

    public OnUpdateQuoteStatus(data): void {
        if (data != undefined && data != null && data.SelectedItemId > 0) {
            $("#ddlQuoteStatus").val(data.SelectedItemId);

            Quote.prototype.DisableManageQuoteControls();

            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.HasError ? data.ErrorMessage : data.SuccessMessage, data.HasError ? "error" : "success", isFadeOut, fadeOutTime);
        }
    }

    BindSelectedOptionForStatus(obj): any {
        var currentform = $(obj).closest("form").children();
        currentform.find('#SelectedItemValue').val($(obj).find('option:selected').text());
        currentform.find('#SelectedItemId').val($(obj).val());
        Quote.prototype.SubmitForm(obj);
    }

    public DisableManageQuoteControls(): void {
        Endpoint.prototype.GetQuoteStateValueById($('#ddlQuoteStatus').find('option:selected').val(), function (response) {
            if ($('#SelectedItemValue').val() == "Approved" || !response.isEdit) {
                Quote.prototype.FreezeManageQuote(true);
            }
            else {
                Quote.prototype.FreezeManageQuote(false);
            }
        });
    }

    public FreezeManageQuote(flag): void {
        if (flag) {
            $("#cancelQuote").attr("disabled", true);
            $("#shippingTypes").attr("disabled", true);
            $("#quoteInformation select").attr("disabled", true);
            $("#quoteInformation input").attr("disabled", true);
            $("#customerInformation a").hide();
            $("#customerInformation input").hide();
            $("#divTotal input").attr("disabled", true);
            $("#chkTaxExempt").attr("disabled", true);
            $('#quoteLineItems select').attr("disabled", true);
            $('#quoteLineItems input').attr("disabled", true);
            $(".sp-actions").hide();
        }
        else {
            $("#cancelQuote").attr("disabled", false);
            $("#shippingTypes").attr("disabled", false);
            $("#quoteInformation select").attr("disabled", false);
            $("#quoteInformation input").attr("disabled", false);
            $("#customerInformation a").show();
            $("#customerInformation input").show();
            $("#divTotal input").attr("disabled", false);
            $("#chkTaxExempt").attr("disabled", false);
            $('#quoteLineItems select').attr("disabled", false);
            $(".sp-actions").show();
        }
    }

    //Get Manage Quote Additional note pop-up
    GetAdditionalNote(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanelWithCallBack('/Quote/GetAdditionalNotes', 'additionalNotes', function (response) {
            $('#notes').val($('#additionalNotes').val());
        });
    }

    //Bind the added note
    SaveAddedNote(): any {
        var notes = $('#notes').val();
        $('#additionalNotes').val(notes);
        ZnodeBase.prototype.CancelUpload('additionalNotes');
    }

    //To show success/error message on the basis of response status
    ChangeShippingAccountNumberSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateShippingAccountNo"), "success", isFadeOut, fadeOutTime);
            $("#spnShippingAccountNumber").hide();
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMessage, "error", isFadeOut, fadeOutTime);
    }

    //To show success/error message on the basis of response status
    ChangeShippingMethodSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateShippingMethod"), "success", isFadeOut, fadeOutTime);
            $("#spnShippingMethod").hide();
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMessage, "error", isFadeOut, fadeOutTime);
    }

    //To show success/error message on the basis of response status
    ChangeInHandDateSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateInHandDate"), "success", isFadeOut, fadeOutTime);
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMessage, "error", isFadeOut, fadeOutTime);
    }

    //To show success/error message on the basis of response status
    ChangeQuoteExpirationDateSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateQuoteExpirationDate"), "success", isFadeOut, fadeOutTime);
        }
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMessage, "error", isFadeOut, fadeOutTime);
    }

    //Get Shipping List for manage Quote
    GetQuoteShippingList(): any {
        Endpoint.prototype.GetQuoteShippingList(Quote.prototype.GetQuoteId(), function (response) {
            var data = response.status;
            var shippingList = data.ShippingList;
            var currentSelectected = $('#hndShippingTypeId').val();
            $('#shippingTypes').empty();
            if (shippingList.filter(obj => obj.ShippingId === parseInt($('#hndShippingTypeId').val())).length < 0)
                $("#hdnIsShippingChange").val('true');

            $.each(shippingList, function (index, element) {
                $('#shippingTypes').append($(`<option value="${element.ShippingId}"  class = "${element.ClassName}" ><text>${element.Description}</text></option>`));
                if (element.ShippingId == currentSelectected)
                    $("#shippingTypes option[value=" + element.ShippingId + "]").attr('selected', 'selected');
            });
            Quote.prototype.CheckIsQuoteOrder();
        });
    }

    //Bind the selected shipping
    GetSelectedShipping(data): any {
        var currentTarget = $(data).find('option:selected');
        if (currentTarget.attr('value') != undefined && currentTarget.attr('class') != undefined) {
            Quote.prototype.ShippingSelectHandler(parseInt(currentTarget.attr('value')), currentTarget.attr('class'));
        }
    }

    //Calculate Shipping In Manage Quote
    ShippingSelectHandler(ShippingId, shippingTypeClass): any {
        $('#hndShippingTypeId').val(ShippingId);
        $("#hndShippingclassName").val(shippingTypeClass);
        $("#AccountNumber").val("");
        if (shippingTypeClass == Constant.ZnodeCustomerShipping) {
            $("#customerShippingDiv").show();
        }
        else {
            $("#customerShippingDiv").hide();
        }
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.CalculateShippingInManage(ShippingId, Quote.prototype.GetQuoteId(), function (response) {
            $("#asidePannelmessageBoxContainerId").hide();
            if (response.shippingErrorMessage != "" && response.shippingErrorMessage != null && response.shippingErrorMessage != "undefined") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.shippingErrorMessage, "error", isFadeOut, fadeOutTime);
                return false;
            }
            //Set shopping cart details of user.
            $("#quoteLineItems").html(response.CartView);
            if ($('#hdnShoppingCartCount').val() == undefined || $('#hdnShoppingCartCount').val() == 0) {
                Quote.prototype.CancelQuote();
            }
            ZnodeBase.prototype.HideLoader();
            Quote.prototype.ShippingErrorMessage();
        });
    }

    //Show Shipping Error Message
    public ShippingErrorMessage(): boolean {
        if ($("#hdnShippiingErrorMessage").length > 0) {
            let shippingErrorMessage: string = $("#hdnShippiingErrorMessage").val();
            if ($("#hdnHasError").val().toLowerCase() == "true" && shippingErrorMessage != "" && shippingErrorMessage != null && shippingErrorMessage != 'undefined') {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(shippingErrorMessage, 'error', isFadeOut, 10000);
                return false;
            }
            return true;
        }
    }

    public ValidateShippingDetails(data): boolean {
        var amount: number = $(data).find('input[type="text"]').val();
        if (!isNaN(amount)) {
            if (amount < 0) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorShippingtAmountNegative"), "error", isFadeOut, fadeOutTime);
                return false;
            }
            else if ($(data).find('input[type="text"]').val() == "") {
                $("#ShippingCost").val($('#hdnShippingCost').val()).removeClass('input-validation-error');
                return false;
            }
            return true;
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"), "error", isFadeOut, fadeOutTime);
            $("#ShippingCost").val($('#hdnShippingCost').val()).removeClass('input-validation-error');
            return false;
        }
    }

    //round off price as per global setting
    public UpdatePriceSuccessCallback(data): any {
        var form = $(data).closest("form");
        var input = $(form).find('input[type="text"]');

        var num = parseFloat(input.val());
        var roundOff = input.attr('data-priceroundoff');
        $("#" + input.attr('id')).val(num.toFixed(parseInt(roundOff)));
        if ($('#hdnShoppingCartCount').val() == undefined || $('#hdnShoppingCartCount').val() == 0) {
            Quote.prototype.CancelQuote();
        }
    }

    //Get Address Detail Popup
    GetCustomerAddressForManange(fromBillingShipping, selectedAddressId, shippingBillingId, omsQuoteId): any {
        $("#customerDetails").html("");
        $("#hdnIsShipping").val(fromBillingShipping);

        var ShippingAddressId: number = fromBillingShipping == 'shipping' ? selectedAddressId : shippingBillingId;
        var BillingAddressId: number = fromBillingShipping == 'billing' ? selectedAddressId : shippingBillingId;
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Quote/GetUserAddressForManageById?selectedAddressId=' + selectedAddressId + '&omsQuoteId=' + omsQuoteId + '&ShippingAddressId=' + ShippingAddressId + '&BillingAddressId=' + BillingAddressId + '&userId=' + $("#hdnUserId").val() + '&portalId=' + Quote.prototype.GetPortalId() + '' + '&fromBillingShipping=' + fromBillingShipping + '', 'addressDetails');
        $("#addressDetails").find('.chkShippingBilling').remove();
    }

    //Map Calculated Shopping cart after address gets updated.
    ChangeAddressSuccessForManage(response): any {
        if (response.errorMessage != "" && response.errorMessage != null && response.errorMessage != "undefined") {
            $("#asidePannelmessageBoxContainerId div").html(response.errorMessage);
            $("#asidePannelmessageBoxContainerId").show();
            return false;
        }
        if (response.shippingErrorMessage != "" && response.shippingErrorMessage != null && response.shippingErrorMessage != "undefined") {
            $("#asidePannelmessageBoxContainerId div").html(response.shippingErrorMessage);
            $("#asidePannelmessageBoxContainerId").show();
            return false;
        }
        if (response.addressView.indexOf("field-validation-error") < 0) {
            ZnodeBase.prototype.CancelUpload('addressDetails');
            $("#customerInformation").html(response.addressView);
            $("#divTotal").html(response.quoteTotal);
            $("#divShoppingCart").html("");
            $("#divShoppingCart").html(response.totalView);
            Quote.prototype.RemoveFormDataValidation();
            Quote.prototype.GetQuoteShippingList();
        }
        else {
            $("#divCustomerAddressPopup").html(response.addressView);
            $(".chkShippingBilling").show();
        }
    }

    //Remove Address Validation
    RemoveFormDataValidation(): any {
        $('form').removeData('validator');
        $('form').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('form');
        $('#IsDefaultShipping').rules('remove');
        $('#IsDefaultBilling').rules('remove');
    }

    CustomerAddressViewHandler(control): any {
        Quote.prototype.ShowHideAddressCheckBoxDiv(control);
        if (control == 'shipping') {
            $("#BillingAddressContainer").hide();
            $('#IsShippingAddressChange').val('true');
        }
        else if (control == 'billing') {
            $("#BillingAddressContainer").show();
            $("#ShippingAddressContainer").hide();
        }
        else {
            $("#addressDetails").find('.chkShippingBilling').show();
        }
        $("#addressDetails").find('#shippingSameAsBillingAddressDiv').remove();
    }

    ShowHideAddressCheckBoxDiv(addressType): any {
        if (addressType == 'shipping') {
            $("#DefaultBillingAddressDiv").hide();
            $("#DefaultShippingAddressDiv").show();
        }
        if (addressType == 'billing') {
            $("#DefaultShippingAddressDiv").hide();
            $("#DefaultBillingAddressDiv").show();
        }
    }  

    //Delete Cart Item in Manage Quote
    DeleteQuoteCartItem(control): void {
        if ($('#hdnShoppingCartCount').val() == "1") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorDeleteLastItem"), "error", isFadeOut, fadeOutTime);
        } else {
            Quote.prototype.ValidateShippingMethod();
            ZnodeBase.prototype.ShowPartialLoader("loader-cart-content");
            var guid: string = $(control).attr('data_cart_externalid');
            let omsQuoteId: number = Quote.prototype.GetQuoteId();
            Endpoint.prototype.DeleteQuoteCartItem(omsQuoteId, guid, function (response) {
                if (response.success) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessDeleteLineItem"), "success", isFadeOut, fadeOutTime);
                    $("#quoteLineItems").html(response.html);
                    //Calculate the shopping cart
                    Quote.prototype.CalculateShoppingCart(omsQuoteId);
                }
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorDeleteLineItem"), "error", isFadeOut, fadeOutTime);

                ZnodeBase.prototype.HidePartialLoader("loader-cart-content");
            });
        }
    }

    //Calculate Shopping Cart of Quote
    CalculateShoppingCart(omsQuoteId: number, showLoader = true) {
        if (showLoader) { ZnodeBase.prototype.ShowPartialLoader("loader-divTotal"); }
        Endpoint.prototype.CalculateQuoteShoppingCart(omsQuoteId, function (response) {

            if (response.html.length > 0) {
                if (omsQuoteId > 0) {
                    $("#divTotal").html(response.html);
                }
                if (showLoader) { ZnodeBase.prototype.HidePartialLoader("loader-divTotal"); }
            }
        });
    }

    //Update CartItem with price and Quantity
    UpdateCartItem(guid: string): any {
        var _quoteLineItemDetail = Quote.prototype.BindCartItemModel(guid);
        var quantityError: string = "#quantity_error_msg_" + guid;
        $('#IsQuoteLineShippingUpdated_' + guid).val("false");

        if (this.CheckQuantityValidations(_quoteLineItemDetail.Quantity, guid, quantityError)) {
            if (this.CheckUnitPriceValidations(_quoteLineItemDetail.UnitPrice.toString(), guid)) {
                if (_quoteLineItemDetail.Quantity != null && _quoteLineItemDetail.Quantity != "") {
                    Endpoint.prototype.UpdateQuoteCartItem(_quoteLineItemDetail, function (response) {
                        Quote.prototype.ClearErrorMessages(guid);
                        Quote.prototype.DisplayUpdatedLineItemData(response, guid);
                        Quote.prototype.UpdateQuoteLineItemShipping(response.lineItemShipping);

                        if (response.hasError) {
                            $(quantityError).html(response.errorMessage);
                            $(quantityError).show();
                            $('#hdnIsSaveQuote').val("false");
                        }
                        else {
                            $(quantityError).html("");
                            $(quantityError).hide();
                            $('#hdnIsSaveQuote').val("true");
                        }
                    });
                }
            }
        }
    }

    //Bind Cart Item Data
    public BindCartItemModel(guid: string): Znode.Core.QuoteLineItemModel {
        var currencySymbol = $("#hdnCurrencySymbol_" + guid).val();
        var _quoteLineItemDetail: Znode.Core.QuoteLineItemModel = {
            OmsQuoteId: Quote.prototype.GetQuoteId(),
            Quantity: ($("#cartQuantity-" + guid).val() == null && $("#cartQuantity-" + guid).val() == "") ? $("#quantity-" + guid).text() : $("#cartQuantity-" + guid).val(),
            UnitPrice: ($("#unitprice-" + guid).val().split(currencySymbol)[1] == undefined || $("#unitprice-" + guid).val().split(currencySymbol)[1] == '') ? $("#unitprice-" + guid).val() : $("#unitprice-" + guid).val().split(currencySymbol)[1].replace(/,/g, ''),
            ProductId: parseInt($("#cartQuantity-" + guid).attr("data-cart-productid")),
            Guid: guid,
            ShippingCost: ($("#shippingcost_" + guid).val().split(currencySymbol)[1] == undefined || $("#shippingcost_" + guid).val().split(currencySymbol)[1] == '') ? $("#shippingcost_" + guid).val() : $("#shippingcost_" + guid).val().split(currencySymbol)[1].replace(/,/g, ''),
            IsShippingEdit: $('#IsQuoteLineShippingUpdated_' + guid).val()
        };
        return _quoteLineItemDetail;
    }

    //Clear error message of item
    public ClearErrorMessages(guid: string): void {
        $("#unit_price_error_msg_" + guid).html("");
        $("#quantity_error_msg_" + guid).html("");
    }

    //To display updated cart item data
    DisplayUpdatedLineItemData(response: any, guid: string) {
        $("#unitprice-" + guid).text(response.unitPrice);
        $("#unitprice-" + guid).val(response.unitPrice);
        $("#shippingcost_" + guid).val(response.shippingCost);
        $("#extendedPrice_" + guid).text(response.extendedPrice);
        $("#quantity-" + guid).text(response.quantity);
        $("#divTotal").html(response.totalView);
        $("#hdnQuoteLineItemShipping_" + guid).val(response.shippingCost);
    }

    //Validate Quantity
    CheckQuantityValidations(quantity: string, guid: string, quantityError: string) {
        var decimalPoint: number = quantity.split(".")[1] != null ? quantity.split(".")[1].length : 0;
        var decimalValue: number = quantity.split(".")[1] != null ? parseInt(quantity.split(".")[1]) : 0;
        var priceRoundOff: number = parseInt($("#cartQuantity-" + guid).attr("data-inventoryRoundOff"));

        if (this.CheckIsNumeric(quantity, quantityError)) {
            if (this.CheckDecimalValue(decimalPoint, decimalValue, priceRoundOff, quantityError)) {
                if (Order.prototype.CheckQuantityGreaterThanZero(parseInt(quantity), quantityError)) {
                    return true;
                }
            }
        }
        return false;
    }

    //Validate Unit Price
    CheckUnitPriceValidations(unitPrice: string, guid: string) {
        var quantityError: string = "#unit_price_error_msg_" + guid;
        var decimalPoint: number = unitPrice.split(".")[1] != null ? unitPrice.split(".")[1].length : 0;
        var decimalValue: number = unitPrice.split(".")[1] != null ? parseInt(unitPrice.split(".")[1]) : 0;
        var priceRoundOff: number = parseInt($("#unit-price-" + guid).attr("data-priceRoundOff"));
        if (unitPrice == "")
            return true;

        if (this.CheckDecimalValue(decimalPoint, decimalValue, priceRoundOff, quantityError, true)) {
            if (parseFloat(unitPrice) > 999999) {
                $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("ErrorPriceRange"));
                $(quantityError).css("class", "error-msg");
                return false;
            }
            return true;
        }
        return false;
    }

    CheckDecimalValue(decimalPoint: number, decimalValue: number, inventoryRoundOff: number, quantityError: string, isPrice: boolean = false): boolean {
        if (decimalValue != 0 && decimalPoint > inventoryRoundOff) {
            if (isPrice)
                $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("EnterPriceHaving") + inventoryRoundOff + ZnodeBase.prototype.getResourceByKeyName("XNumbersAfterDecimalPoint"));
            else
                $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("EnterQuantityHaving") + inventoryRoundOff + ZnodeBase.prototype.getResourceByKeyName("XNumbersAfterDecimalPoint"));
            $(quantityError).css("class", "error-msg");
            $(quantityError).show();
            return false;
        }
        $(quantityError).hide();
        return true;
    }

    CheckIsNumeric(selectedQty: string, quantityError: string): boolean {
        var matches = selectedQty.match(/^-?[\d.]+(?:e-?\d+)?$/);
        if (matches == null) {
            $(quantityError).text(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"));
            $(quantityError).addClass("error-msg")
            $(quantityError).show();
            return false;
        }
        return true;
    }

    public OnTaxExemptChecked(): void {
        if ($("#chkTaxExempt").is(':checked')) {
            $("#PopUpTaxExemptSubmitQuote").modal("show");
        }
        else {
            Quote.prototype.ConfirmTaxExemptQuote();
        }
    }

    public ConfirmTaxExemptQuote(): void {
        var isTaxExempt: boolean = $("#chkTaxExempt").prop("checked");
        ZnodeBase.prototype.ShowPartialLoader("loader-divTotal");
        Endpoint.prototype.UpdateTaxExemptForQuote(Quote.prototype.GetQuoteId(), isTaxExempt, function (response) {
            $("#divTotal").html("");
            $("#divTotal").html(response);
            if (isTaxExempt == true) {
                $("#messageTaxExcempt").html(ZnodeBase.prototype.getResourceByKeyName("TaxExemptMessage"));
            }
            else {
                $("#messageTaxExcempt").html(ZnodeBase.prototype.getResourceByKeyName("MakeTaxExemptMessage"));
            }
            ZnodeBase.prototype.HidePartialLoader("loader-divTotal");
        });
    }

    public CheckTaxExemptOnPageLoad(): void {
        if ($("#hdnIsTaxExempt").val() == "True") {
            $("#containerTaxExempt").show();
            $("#messageTaxExcempt").html("This Quote Tax Exempted");
        }
        $("#divTaxExemptContainer").show();
    }

    PrintManangeQuote(): any {
        Endpoint.prototype.PrintManangeQuote(Quote.prototype.GetQuoteId(), function (response) {
            if (response.success) {
                var originalContents = document.body.innerHTML;

                if (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1) {
                    setTimeout(function () { document.body.innerHTML = response.html }, 1);
                    setTimeout(function () { window.print(); }, 10);
                    setTimeout(function () { document.body.innerHTML = originalContents }, 20);
                    setTimeout(function () { Order.prototype.HideLoader(); }, 30);
                }

                else {
                    document.body.innerHTML = response.html;
                    window.print();
                    document.body.innerHTML = originalContents;
                }
            } else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorShippingCalculation"), "error", isFadeOut, fadeOutTime);
            return false;
        });
    }

    //To show success/error message on the basis of response status
    ChangeJobNameSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateJobName"), "success", isFadeOut, fadeOutTime);
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMassage, "error", isFadeOut, fadeOutTime);
        }
    }

    //To show success/error message on the basis of response status
    ChangeShippingConstraintCodeSuccessCallback(response): any {
        if (response.status) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SuccessUpdateShippingConstraintCode"), "success", isFadeOut, fadeOutTime);
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.ErrorMassage, "error", isFadeOut, fadeOutTime);
        }
    }

    //Confirm Submit Quote.
    public OnConfirmSubmitQuote(): boolean {
        if (!(Quote.prototype.ValidateQuote()))
            return false;

        Quote.prototype.ShowSubmitPopup();
    }

    public SubmitEditQuote(): void {
        let notes: string = "";
        if ($("#additionalNotes").val() != "") {
            notes = $("#additionalNotes").val();
        }
        Endpoint.prototype.UpdateManageQuote(Quote.prototype.GetQuoteId(), notes, function (data) {
            if (!data.status)
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.errorMessage, "error", isFadeOut, fadeOutTime);

            window.location.reload(true);
        });
    }
    public SubmitAccountQuote(flag: boolean): void {
        let notes: string = "";
        let OmsOrderStateId: string = "";
        if ($("#additionalNotes").val() != "") {
            notes = $("#additionalNotes").val();
        }
        if ($("#hdnOmsOrderStateId").val() != "") {
            OmsOrderStateId = $("#hdnOmsOrderStateId").val();
        }
        Endpoint.prototype.UpdateAccountManageQuote(Quote.prototype.GetQuoteId(), notes, OmsOrderStateId,function (data) {
            if (!data.status)
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.errorMessage, "error", isFadeOut, fadeOutTime);
            if (flag != false) {
                window.location.replace("/Quote/AccountQuoteList");
            }
            else {
                window.location.reload(true);
            }
        });
    }

    //Validate Shipping method is exist or not before shipping calculation
    ValidateShippingMethod() {
        if ($('#hdnIsShippingChange').val() == "true") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorShippingCalculation"), "error", isFadeOut, fadeOutTime);
            return false;
        }
    }

    SubmitQuote(): any {
        ZnodeBase.prototype.ShowLoader();
        let isValid: boolean = true;
        isValid = Quote.prototype.ValidateDetails("false");
        if (isValid) {
            ZnodeBase.prototype.ShowLoader();

            Quote.prototype.CreateQuote();
        }

        if (!isValid)
            ZnodeBase.prototype.HideLoader();
    }

    CreateQuote(): void {
        $('#status-message').remove();

        $("#PortalId").val(Order.prototype.GetPortalId());
        $.ajax({
            url: "/Quote/SubmitQuote",
            data: Quote.prototype.SetQuoteCreateViewModel(),
            type: 'POST',
            success: function (data) {
                if (!data.HasError) {
                    window.location.href = "/Quote/QuoteList";
                }
                else {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.ErrorMessage, 'error', isFadeOut, fadeOutTime);
                }
            }
        });
    }
    SetQuoteCreateViewModel(): any {
        var QuoteCreateViewModel = {
            UserId: $('#hdnUserId').val(),
            ShippingId: $('#selectedShippingId').val(),
            EnableAddressValidation: $("#enableAddressValidation").val(),
            AdditionalInstruction: $("#AdditionalInstruction").val(),
            AccountNumber: $("#ShippingListViewModel_AccountNumber").val(),
            ShippingMethod: $("#ShippingListViewModel_ShippingMethod").val(),
            InHandDate: $("#InHandDate").val(),
            QuoteExpirationDate: $("#QuoteExpirationDate").val(),
            JobName: $("#JobName").val(),
            ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val()
        };
        return QuoteCreateViewModel;
    }


    ReviewQuote(): any {
        var isValid = Quote.prototype.ValidateDetails("false");

        if (isValid) {
            ZnodeBase.prototype.ShowLoader();
            $.ajax({
                url: "/Order/GetReviewOrder",
                data: Order.prototype.SetCreateOrderViewModel(),
                type: 'POST',
                success: function (data) {
                    Order.prototype.HideLoader();
                    $("#ReviewDiv").html(data);

                    let shippingId: string = $("input[name='ShippingId']:checked").attr("id");
                    let shippingName: string = $("#shippingMethodDiv input[id='" + shippingId + "']").attr("data-shipping-name");
                    let shippingValue: string = $("#shippingMethodDiv input[id='" + shippingId + "']").attr("data-shippingvalue");
                    let customShippingCost: string = $("#hdnCustomShippingCost").val();
                    shippingName = typeof shippingName == "undefined" || shippingName == "" ? $("input[name='ShippingId']:checked").attr("data-shipping-name") : shippingName;
                    if (customShippingCost == '') {
                        $("#selectedShippingName").html(shippingName + ": " + shippingValue);
                    }
                    else {
                        $("#selectedShippingName").html(shippingName);
                    }
                    $("#customerDivReview").html($("#customerDiv").html());

                    $("#orderNotes").html($("#AdditionalInstruction").val());
                    $("#divQuoteExpirationDate").html($("#QuoteExpirationDate").val());
                    $("#inHandDates").html($("#InHandDate").val());
                    $("#jobName").html($("#JobName").val());
                    $("#shippingConstraintsCode").html($("input[name='ShippingConstraintCode']:checked").attr('data-shipping-name'));

                    GridPager.prototype.Init();
                    if (Order.prototype.IsQuote()) {
                        $("#divPaymentReview").hide();
                        $("#label-review-order").html(ZnodeBase.prototype.getResourceByKeyName("LabelReviewQuote"));
                        $("#label-review-order-sub-msg").html(ZnodeBase.prototype.getResourceByKeyName("ReviewQuoteSubMessage"));
                    }
                }
            });
        }
        return isValid;
    }
    ValidateDetails(isQuote): boolean {
        ZnodeBase.prototype.ShowLoader();
        var isValid = true;
        if ($("#txtCustomerName").val() == '') {
            isValid = false;
            $("#txtCustomerName").css({
                "border": "1px solid red"
            });
            Order.prototype.ClickSelectedTab("customer-tab-link");
        }
        else if ($("[name='ShippingId']").val() == undefined || $("[name='ShippingId']").val() == "" || $("#shippingMethodDiv :radio:checked").length == 0 && ($("#cartFreeShipping").val() != "True" || $("#hdnIsFreeShipping").val() != "True")) {
            if (isQuote != "true") {
                Order.prototype.ClickSelectedTab("shipping-cart-tab-link");
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorShippingNotAvailable"), "error", isFadeOut, fadeOutTime);
                isValid = false;
            }
        }
        else {
            if ($("#ShippingAddress_AddressId").val() == '0') {
                isValid = false;
            };
            if (!isValid) {
                Order.prototype.ClickSelectedTab("customer-tab-link");
            }
        }

        if (!Order.prototype.ShowAllowedTerritoriesError()) {
            isValid = false;
        }
        ZnodeBase.prototype.HideLoader();
        return isValid;
    }

    public ShowSubmitPopup(): void {
        if ($('#SelectedItemValue').val() == "Approved" && $('#hdnIsConvertedToOrder').val() == "False") {
            $('#PopUpConvertToOrder').modal('show');
        } else {
            $('#PopUpConfirmSubmitQuote').modal('show');
        }
    }

    GetPaymentOptions(): any {
        if ($('#SelectedItemValue').val() == "Approved" && $('#hdnIsConvertedToOrder').val() == "False") {
            ZnodeBase.prototype.BrowseAsidePoupPanel('/Quote/GetPaymentMethods?&userId=' + $('#hdnUserId').val() + '&portalId=' + Quote.prototype.GetPortalId() + '', 'paymentStatusPanel');
        }
    }

    public IsQuoteDataValid(): boolean {
        var isValid: boolean = true;
        var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
        var paymentOptionValue = Order.prototype.GetPaymentType(paymentOptionId);

        if (paymentOptionValue == null || paymentOptionValue == "") {
            $('#' + $(this).attr('id')).addClass('input-validation-error');
            $('#' + $(this).attr('id')).attr('style', 'border: 1px solid rgb(195, 195, 195)');
            $('span#valPaymentTypes').removeClass('field-validation-valid');
            $('span#valPaymentTypes').addClass('field-validation-error');
            $('span#valPaymentTypes').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectPaymentType"));
            return false;
        } else {

            $('span#valPaymentTypes').text("");
        }
        return isValid;
    }

    ResetQuoteStatus(): any{
        if ($('#ddlQuoteStatus').find('option:selected').text() != $('#hdnOmsQuoteStatus').val()) {
            var form = $('#QuoteStatus');
            $("#ddlQuoteStatus,#SelectedItemValue,#SelectedItemId, #hdnQuoteStatusQuoteId").attr("disabled", false);
            form.find('#SelectedItemValue').val($('#hdnOmsQuoteStatus').val());
            form.find('#SelectedItemId').val($('#hdnOmsQuoteStateId').val());
            $('#ddlQuoteStatus').val($('#hdnOmsQuoteStateId').val());
            form.submit();
        }
    }
    ConvertQuoteToOrder(): any {
        var paymentCode = $("#hdnGatewayCode").val().toLowerCase();
        if (paymentCode == Constant.CyberSource) {
            if ($('ul#creditCardTab ').find('li').find('a.active').attr('href') == "#savedCreditCard-panel" || $('ul#creditCardTab ').find('li.active').find('a').attr('href') == "#savedCreditCard-panel") {
                Quote.prototype.SubmitIframePayment("");

            }
            else {
                $("#pay-button").click();
            }
        }
        else {
            Quote.prototype.ConvertQuoteToOrderQuotes();
        }

    }
   
    ConvertQuoteToOrderQuotes(): any {
        if (Quote.prototype.IsQuoteDataValid()) {
            var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
            var paymentType = Order.prototype.GetPaymentType(paymentOptionId);
            switch (paymentType.toLowerCase()) {
                case "cod":
                    Quote.prototype.SubmitQuoteForm();
                    ZnodeBase.prototype.CancelUpload('paymentStatusPanel');
                    break;
                case "credit_card":
                        if (Quote.prototype.IsValidCreditCardDetails()) {
                        var paymentCode = $('#hdnGatewayCode').val();

                        if (paymentCode == Constant.CyberSource) {
                            Quote.prototype.SubmitIframePayment($("#CardDataToken").val());
                        }
                        else {
                            Quote.prototype.SubmitPayment();
                        }
                        ZnodeBase.prototype.CancelUpload('paymentStatusPanel');
                    }
                    break;
                case "ach":
                    Quote.prototype.SubmitACHPayment();
                    ZnodeBase.prototype.CancelUpload('paymentStatusPanel');
                    break;
                case "purchase_order":
                    if (Quote.prototype.CheckValidPODocument()) {
                        Quote.prototype.SubmitQuoteForm();
                    }
                    else {
                        ZnodeBase.prototype.HideLoader();
                        return false;
                    }
                    break;
                case "invoice me":
                    Quote.prototype.SubmitQuoteForm();
                    break;
            }
        }
    }

    CheckValidPODocument(): boolean {
        var purchaseOrderNumber: string = $('#PurchaseOrderNumber').val()
        if (purchaseOrderNumber != null) {
            if (purchaseOrderNumber.length < 1) {
                $('#cart-ponumber-status').show();
                $('#cart-ponumber-status').text(ZnodeBase.prototype.getResourceByKeyName('ErrorRequiredPurchaseOrder'));
                $(window).scrollTop(0);
                $(document).scrollTop(0);
                return false;
            }
            else if (purchaseOrderNumber.length > 50) {
                $('#cart-ponumber-status').show();
                $('#cart-ponumber-status').text(ZnodeBase.prototype.getResourceByKeyName('ErrorPurchaseOrderLength'));
                $(window).scrollTop(0);
                $(document).scrollTop(0);
                return false;
            }
            else if ($("#IsPoDocRequire").val() == "True") {
                if ($("#PODocument").val() == null || $("#PODocument").val() == "") {
                    $("#errorFileTypeAndSize").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFileRequired"));
                    $(window).scrollTop(0);
                    $(document).scrollTop(0);
                    return false;
                }
            }
            return true;
        }
        else
            return true;
    }

    SubmitQuoteForm(): void {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.SaveAndConvertQuoteToOrder(Quote.prototype.BindPaymentModel(), function (response) {
            if (response.OrderId !== undefined && response.OrderId > 0) {
                ZnodeBase.prototype.HideLoader();

                window.location.reload(true);

                $("#ajaxBusy").dialog('close');
                var form = $('<form action="QuoteCheckoutReceipt" method="post">' +
                    '<input type="hidden" name="orderId" value="' + response.OrderId + '" />' +
                    '<input type="text" name= "ReceiptHtml" value= "' + response.ReceiptHtml + '" />' +
                    '</form>');
                $('body').append(form);
                $(form).addClass("dirtyignore").submit();
            }
            else {
                $("#ajaxBusy").dialog('close');
                var errorMessage = response.error == undefined || response.error == "" ? ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace") : response.error;
                Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                return false;
            }
        });
    }

    //Bind Cart Item Data
    public BindPaymentModel(): Znode.Core.ConvertQuoteToOrderViewModel {
        var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
        var PaymentDetails: Znode.Core.PaymentDetails = {
            PaymentSettingId: Number($("#ddlPaymentTypes option:selected").val()),
            paymentType: $("#ddlPaymentTypes " + "#" + paymentOptionId).attr("data-payment-type"),
            PurchaseOrderNumber: $("#PurchaseOrderNumber").val(),
            PODocumentName: $("#po-document-path").val(),
        };
        var _convertQuoteToOrderViewModel: Znode.Core.ConvertQuoteToOrderViewModel = {
            OmsQuoteId: Quote.prototype.GetQuoteId(),
            UserId: Quote.prototype.GetUserId(),
            AdditionalInstructions: $("#additionalNotes").val(),
            PaymentDetails: PaymentDetails,
        };
        return _convertQuoteToOrderViewModel;
    }

    //End Amazon Pay Methods. 
    public GetPaymentType(id: string): string {
        var paymentType = $("#" + id).attr("data-payment-type");
        if (paymentType != undefined) {
            return paymentType.toLowerCase();
        } else {
            return id;
        }
    }

    SubmitIframePayment(querystr: any) {  // SubmitCybersourceePayment(querystr: any)
        var Total = $("#hdnTotalAmt").val();
       
        if (Order.prototype.IsOrderTotalGreaterThanZero(Total)) {

            $("#div-CreditCard").hide();
            var orderNumber = "";
            Endpoint.prototype.GenerateOrderNumber($("#hdnPortalId").val(), function (response) {
                orderNumber = response.orderNumber;
            });
            var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
            var paymentType = Order.prototype.GetPaymentType(paymentOptionId);

            var submitPaymentViewModel = {

                OmsQuoteId: Quote.prototype.GetQuoteId(),
                UserId: Quote.prototype.GetUserId(),
                PaymentDetails: {
                    PaymentSettingId: $('#PaymentSettingId').val(),
                    PaymentCode: $('#hdnPaymentCode').val(),
                    UserId: $("#hdnUserId").val(),
                    ShippingOptionId: $("[name='ShippingId']").val(),
                    BillingAddressId: $("#UserAddressDataViewModel_BillingAddress_AddressId").val(),
                    ShippingAddressId: $("#UserAddressDataViewModel_ShippingAddress_AddressId").val(),
                    PortalId: $("#hdnPortalId").val(),
                    PortalCatalogId: $("#PortalCatalogId").val(),
                    AdditionalInfo: $("#additionalInstructions").val(),
                    EnableAddressValidation: $("input[name='EnableAddressValidation']").val(),
                    RequireValidatedAddress: $("input[name='RequireValidatedAddress']").val(),
                    AccountNumber: $("#ShippingListViewModel_AccountNumber").val(),
                    ShippingMethod: $("#ShippingListViewModel_ShippingMethod").val(),
                    CardType: 'credit_card',
                    OrderNumber: orderNumber,
                    InHandDate: $("#InHandDate").val(),
                    JobName: $("#JobName").val(),
                    ShippingConstraintCode: $("input[name='ShippingConstraintCode']:checked").val(),
                    CyberSourceToken: querystr,
                    paymentType: paymentType,
                    IsSaveCreditCard: $("#hdnGatewayCode").val()  === Constant.BrainTree ? $("#hdnBraintreeIsVault").val() : $("#SaveCreditCard").is(':checked'),
                    CustomerProfileId:$("#hdnGatewayCode").val() === Constant.BrainTree ? null : $('#CustomerProfileId').val(),
                    CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                    CustomerGuid: $("#hdnCustomerGUID").val(),
                    PaymentGUID: $("#hdnPaymentGUID").val(),
                    GatewayCode: $("#hdnGatewayCode").val()

                }
                
            };

            $.ajax({
                type: "POST",
                url: "/quote/SaveAndConvertQuoteToOrder",
                async: true,
                data: submitPaymentViewModel,
                success: function (response) {
                    if (response.OrderId !== undefined && response.OrderId > 0) {

                        window.location.reload(true);

                        $("#ajaxBusy").dialog('close');
                        var form = $('<form action="QuoteCheckoutReceipt" method="post">' +
                            '<input type="hidden" name="orderId" value="' + response.OrderId + '" />' +
                            '<input type="text" name= "ReceiptHtml" value= "' + response.ReceiptHtml + '" />' +
                            '</form>');
                        $('body').append(form);
                        $(form).addClass("dirtyignore").submit();
                    }
                    else {
                        $("#ajaxBusy").dialog('close');
                        var errorMessage = response.Data.ErrorMessage == undefined || response.Data.ErrorMessage == "" ? ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace") : response.Data.ErrorMessage;
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                        return false;
                    }
                },
                error: function () {
                    Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
            });
        }
        else
            ZnodeBase.prototype.HideLoader();
    }


    //AuthorizeNet Payment
    SubmitAuthorizeNetPayment(querystr: any) {
        var Total = $("#hdnTotalAmt").val();
        var transactionResponse = JSON.parse(querystr);

        var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
        var paymentType = Order.prototype.GetPaymentType(paymentOptionId);
        var transactionId = transactionResponse.transId;
        var creditCardNumber = transactionResponse.accountNumber;
        var orderInvoiceNumber = transactionResponse.orderInvoiceNumber;
        if (Order.prototype.IsOrderTotalGreaterThanZero(Total)) {
            var submitPaymentViewModel =
            {
                OmsQuoteId: Quote.prototype.GetQuoteId(),
                UserId: Quote.prototype.GetUserId(),
                PaymentDetails: {
                    PaymentSettingId: Number($("#ddlPaymentTypes option:selected").val()),
                    GatewayCode: $("#hdnGatewayCode").val(),
                    PaymentCode: $('#hdnPaymentCode').val(),
                    paymentType: paymentType,
                    TransactionId: transactionId,
                    CustomerPaymentId: $('#CustomerPaymentProfileId').val(),
                    CustomerProfileId: $('#CustomerProfileId').val(),
                    IsSaveCreditCard: $("#SaveCreditCard").is(':checked'),
                    CreditCardNumber: (creditCardNumber).slice(-4),
                    CardType: 'credit_card',
                    OrderId: orderInvoiceNumber
                }

            };
            $.ajax({
                type: "POST",
                url: "/quote/SaveAndConvertQuoteToOrder",
                async: true,
                data: submitPaymentViewModel,
                success: function (response) {
                    if (response.OrderId !== undefined && response.OrderId > 0) {

                        window.location.reload(true);

                        $("#ajaxBusy").dialog('close');
                        var form = $('<form action="QuoteCheckoutReceipt" method="post">' +
                            '<input type="hidden" name="orderId" value="' + response.OrderId + '" />' +
                            '<input type="text" name= "ReceiptHtml" value= "' + response.ReceiptHtml + '" />' +
                            '</form>');
                        $('body').append(form);
                        $(form).addClass("dirtyignore").submit();
                    }
                    else {
                        $("#ajaxBusy").dialog('close');
                        var errorMessage = response.Data.ErrorMessage == undefined || response.Data.ErrorMessage == "" ? ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace") : response.Data.ErrorMessage;
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                        return false;
                    }
                },
                error: function () {
                    Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
            });
        }
        else
            ZnodeBase.prototype.HideLoader();
    }


    //Submit Payment
    SubmitPayment(): any {
        var Total = $("#hdnTotalQuoteAmount").val();

        if (Order.prototype.IsOrderTotalGreaterThanZero(Total)) {
            if (Quote.prototype.IsValidCreditCardDetails()) {

                var paymentSettingId = Number($("#ddlPaymentTypes option:selected").val());
                var paymentCode = $('#hdnPaymentCode').val();

                //Get Payment model
                var payment = Order.prototype.GetPaymentModel();

                //Validate Payment Profile And Proceed for Convert To Order
                Quote.prototype.ValidatePaymentProfileAndConvertToOrder(payment, paymentSettingId, paymentCode);
            }
        }
    }

    //Submit Payment
    SubmitACHPayment(): any {
        var Total = $("#hdnTotalQuoteAmount").val();

        if (Order.prototype.IsOrderTotalGreaterThanZero(Total)) {
            if (Order.prototype.ValidateCardConnectDataToken()) {

                var paymentSettingId = Number($("#ddlPaymentTypes option:selected").val());
                var paymentCode = $('#hdnPaymentCode').val();

                //Get Payment model
                var payment = Order.prototype.GetACHPaymentModel();

                //Validate Payment Profile And Proceed for Convert To Order
                Quote.prototype.ValidatePaymentProfileAndConvertToOrderForACH(payment, paymentSettingId, paymentCode);
            }
        }
    }

    IsValidCreditCardDetails(): boolean {
        var isValid: boolean = true;
        var paymentCode = $('#hdnGatewayCode').val();
        if (!$("#radioCCList").is(':visible')) {
            $('input[data-payment="number"],input[data-payment="exp-month"],input[data-payment="exp-year"],input[data-payment="cvc"]').each(function () {
                if ($.trim($(this).val()) == '') {
                    isValid = false;
                    $(this).css({
                        "border": "1px solid red",
                        "background": "#FFCECE"
                    });
                } else {
                    $(this).css({
                        "border": "1px solid black",
                        "background": ""
                    });
                }
            });
            if (paymentCode == Constant.CyberSource)
                isValid = Order.prototype.ValidateCardConnectDataToken();
            else if (paymentCode != "cardconnect" && paymentCode != Constant.BrainTree)
                isValid = Quote.prototype.ValidateCreditCardDetails();
            else if (paymentCode === Constant.BrainTree) {
                isValid = Quote.prototype.ValidateBrainTreeCardDetails();
            }
            else {
                if (Order.prototype.ValidateCardConnectDataToken() && Order.prototype.ValidateCardConnectCardHolderName()) {
                    var cardType = Order.prototype.DetectCardTypeForCardConnect($('#CardDataToken').val());
                    isValid = Order.prototype.ValidateCardType(cardType);
                }
                else
                    Order.prototype.ShowErrorPaymentDialog($("#ErrorMessage").val());
            }
        }
        else {
            isValid = Order.prototype.ValidateCVV();
        }
        if (isValid == false) {
            return false;
        }
        return isValid;
    }

    ValidateCreditCardDetails(): any {

        var isValid = true;
        var cardType = $('input[name="PaymentProviders"]:checked').val();
        if (!Order.prototype.Mod10($('input[data-payment="number"]').val().split(" ").join(""))) {
            isValid = false;
            $('#errornumber').show();
            Order.prototype.PaymentError("number");
        }
        else {
            $('#errornumber').hide();
            Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="number"]');
        }

        if (!Order.prototype.ValidateCreditCardExpirationDetails()) {
            isValid = false;
        }
        if ($('input[data-payment="cvc"]').val() == '') {
            $('#errorcvc').show();
        }
        else {
            $('#errorcvc').hide();
            Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
        }

        if ($('input[data-payment="cvc"]').val().length < 3) {
            isValid = false;
            $('#errorcardnumber').show();
            Order.prototype.PaymentError("cvc");
        } else {
            if (cardType == Constant.AmericanExpressCardCode && $('input[data-payment="cvc"]').val().length == 4) {
                Order.prototype.ShowHideErrorCVV(false);
                Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
            } else if (cardType != Constant.AmericanExpressCardCode && $('input[data-payment="cvc"]').val().length == 3) {
                Order.prototype.ShowHideErrorCVV(false)
                Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cvc"]');
            }
            else {
                isValid = false;
                Order.prototype.ShowHideErrorCVV(true);
                Order.prototype.PaymentError("cvc");
            }
        }

        if ($('input[data-payment="cardholderName"]').val().trim() == '' || $('input[data-payment="cardholderName"]').val().trim().length > 100) {
            isValid = false;
            $('#errorcardholderName').show();
            Order.prototype.PaymentError("cardholderName");
        }
        else {
            $('#errorcardholderName').hide();
            Order.prototype.RemoveCreditCardValidationCSS('input[data-payment="cardholderName"]');
        }
        if (!isValid) {
            $(window).scrollTop(0);
            $(document).scrollTop(0);
        }
        return isValid;
    }

    ValidatePaymentProfileAndConvertToOrder(payment, paymentSettingId: any, paymentCode: any) {
        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
        $("#div-CreditCard").hide();
        var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
        var paymentType = Order.prototype.GetPaymentType(paymentOptionId);
        var creditCardNumber: string = $("#hdnGatewayCode").val() == "cardconnect" ? $('#CardDataToken').val().slice(-4) : $('#CreditCardNumber').val();

        submitCard(payment, function (response) {
            if (response.GatewayResponse == undefined) {
                if (response.indexOf("Unauthorized") > 0) {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                    ZnodeBase.prototype.HideLoader();
                }
            }
            else {
                var isSuccess = response.GatewayResponse.IsSuccess;
                if (isSuccess) {
                    //Quote.prototype.ClosePopup();
                    var submitPaymentViewModel = Quote.prototype.GetSubmitPaymentViewModel(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    $.ajax({
                        type: "POST",
                        url: "/quote/SaveAndConvertQuoteToOrder",
                        async: true,
                        data: submitPaymentViewModel,
                        success: function (response) {
                            if (response.OrderId !== undefined && response.OrderId > 0) {

                                window.location.reload(true);

                                $("#ajaxBusy").dialog('close');
                                var form = $('<form action="QuoteCheckoutReceipt" method="post">' +
                                    '<input type="hidden" name="orderId" value="' + response.OrderId + '" />' +
                                    '<input type="text" name= "ReceiptHtml" value= "' + response.ReceiptHtml + '" />' +
                                    '</form>');
                                $('body').append(form);
                                $(form).addClass("dirtyignore").submit();
                            }
                            else {
                                $("#ajaxBusy").dialog('close');
                                var errorMessage = response.Data.ErrorMessage == undefined || response.Data.ErrorMessage == "" ? ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace") : response.Data.ErrorMessage;
                                Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                return false;
                            }
                        },
                        error: function () {
                            Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                            ZnodeBase.prototype.HideLoader();
                            return false;
                        }
                    });
                }
                else {
                    var errorMessage = response.GatewayResponse.ResponseText;
                    if (errorMessage == undefined) {
                        errorMessage = response.GatewayResponse.GatewayResponseData;
                    }

                    if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorCardDataMissing"));
                    } else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
                        $("#div-CreditCard").show();
                    } else if (errorMessage.indexOf('customer') > 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                    } else {
                        Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace"));
                    }
                }
            }
        });
    }

    ValidatePaymentProfileAndConvertToOrderForACH(payment, paymentSettingId: any, paymentCode: any) {
        payment["CardSecurityCode"] = payment["PaymentToken"] ? $("[name='SaveCard-CVV']:visible").val() : $("#div-CreditCard [data-payment='cvc']").val();
        $("#div-CreditCard").hide();
        var paymentOptionId: string = $("#ddlPaymentTypes option:selected").attr("id");
        var paymentType = Order.prototype.GetPaymentType(paymentOptionId);
        var creditCardNumber: string = $('#CreditCardNumber').val();

        submitCard(payment, function (response) {
            if (response.GatewayResponse == undefined) {
                if (response.indexOf("Unauthorized") > 0) {
                    Quote.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessCreditCardPayment") + response + ZnodeBase.prototype.getResourceByKeyName("ContactUsToCompleteOrder"));
                    ZnodeBase.prototype.HideLoader();
                }
            }
            else {
                var isSuccess = response.GatewayResponse.IsSuccess;
                if (isSuccess) {
                    //Quote.prototype.ClosePopup();
                    var submitPaymentViewModel = Quote.prototype.GetSubmitPaymentViewModelForACH(paymentSettingId, paymentCode, response, paymentType, creditCardNumber);
                    $.ajax({
                        type: "POST",
                        url: "/quote/SaveAndConvertQuoteToOrder",
                        async: true,
                        data: submitPaymentViewModel,
                        success: function (response) {
                            if (response.OrderId !== undefined && response.OrderId > 0) {

                                window.location.reload(true);

                                $("#ajaxBusy").dialog('close');
                                var form = $('<form action="QuoteCheckoutReceipt" method="post">' +
                                    '<input type="hidden" name="orderId" value="' + response.OrderId + '" />' +
                                    '<input type="text" name= "ReceiptHtml" value= "' + response.ReceiptHtml + '" />' +
                                    '</form>');
                                $('body').append(form);
                                $(form).addClass("dirtyignore").submit();
                            }
                            else {
                                $("#ajaxBusy").dialog('close');
                                var errorMessage = response.Data.ErrorMessage == undefined || response.Data.ErrorMessage == "" ? ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace") : response.Data.ErrorMessage;
                                Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                                return false;
                            }
                        },
                        error: function () {
                            Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorProcessOrder"));
                            ZnodeBase.prototype.HideLoader();
                            return false;
                        }
                    });
                }
                else {
                    var errorMessage = response.GatewayResponse.ResponseText;
                    if (errorMessage == undefined) {
                        errorMessage = response.GatewayResponse.GatewayResponseData;
                    }

                    if (errorMessage != undefined && errorMessage.toLowerCase().indexOf("missing card data") >= 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorCardDataMissing"));
                    } else if (errorMessage != undefined && errorMessage.indexOf("Message=") >= 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage.substr(errorMessage.indexOf("=") + 1));
                        $("#div-CreditCard").show();
                    } else if (errorMessage.indexOf('customer') > 0) {
                        Order.prototype.ClearPaymentAndDisplayMessage(errorMessage);
                    } else {
                        Order.prototype.ClearPaymentAndDisplayMessage(ZnodeBase.prototype.getResourceByKeyName("ErrorOrderPlace"));
                    }
                }
            }
        });
    }



    GetPaymentErrorMsg(response): string {
        var errorCode = response["error"] ? response["error"].toLowerCase().split(",") : "";

        if ($.inArray("code: E00027".toLowerCase(), errorCode) >= 0)
            return ZnodeBase.prototype.getResourceByKeyName("ErrorCodeE00027");
        return response["error"];
    }

    ClearPaymentAndDisplayMessage(message): any {
        Order.prototype.CanclePayment();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(message, "error", isFadeOut, fadeOutTime);
    }

    GetSubmitPaymentViewModel(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsQuoteId: Quote.prototype.GetQuoteId(),
            UserId: Quote.prototype.GetUserId(),
            PaymentDetails: {
                PaymentSettingId: paymentSettingId,
                PaymentCode: paymentCode,
                CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                CustomerGuid: response.GatewayResponse.CustomerGUID,
                PaymentToken: $("input[name='CCdetails']:checked").val(),
                paymentType: paymentType,
                CreditCardNumber: creditCardNumber.slice(-4)
            }
        };
    }

    GetSubmitPaymentViewModelForACH(paymentSettingId: any, paymentCode: any, response: any, paymentType: string, creditCardNumber: string) {
        return {
            OmsQuoteId: Quote.prototype.GetQuoteId(),
            UserId: Quote.prototype.GetUserId(),
            PaymentDetails: {
                PaymentSettingId: paymentSettingId,
                PaymentCode: paymentCode,
                CustomerProfileId: response.GatewayResponse.CustomerProfileId,
                CustomerPaymentId: response.GatewayResponse.CustomerPaymentProfileId,
                CustomerShippingAddressId: response.GatewayResponse.CustomerShippingAddressId,
                CustomerGuid: response.GatewayResponse.CustomerGUID,
                PaymentToken: $("input[name='CCdetails']:checked").val(),
                paymentType: paymentType,
                CreditCardNumber: creditCardNumber.slice(-4),
                IsACHPayment: true
            }
        };
    }

    UpdateQuoteCartItemPrice(control): any {
        var productid = $(control).attr("data-cart-productid");
        var externalId = $(control).attr("data-cart-externalid");
        var shippingid = $("input[name='ShippingId']:checked").val();
        var priceRoundOff = $(control).attr("data-priceRoundOff");
        let userId: number = Order.prototype.GetUserId();
        var cartUnitPtice = $(control).attr("data-cart-unitPrice");

        $("#unit_price_error_msg_" + externalId).text('');
        var updatedUnitPrice = $(control).val();
        if (updatedUnitPrice != undefined && parseFloat(updatedUnitPrice) != parseFloat(cartUnitPtice)) {
            if (updatedUnitPrice.split(".")[1] != null && parseInt(updatedUnitPrice.split(".")[1].length) > parseInt(priceRoundOff)) {
                $("#unit_price_error_msg_" + externalId).text('Please enter Price having ' + priceRoundOff + ' numbers after decimal point.');
                return false;
            }
            var matches = updatedUnitPrice.match(/^-?[\d.]+(?:e-?\d+)?$/);
            if (matches == null) {
                $("#unit_price_error_msg_" + externalId).text('Please enter numeric value');
                return false;
            }
        }
        var guid = $(control).attr("data-cart-externalid");
        ZnodeBase.prototype.ShowPartialLoader("loader-cart-content");
        this.UpdatePrice(guid, updatedUnitPrice, productid, shippingid, userId);
    }

    private UpdatePrice(guid: string, updatedUnitPrice: any, productid: string, shippingid: any, userId: number) {
        Order.prototype.ClearShippingEstimates();
        Endpoint.prototype.UpdateQuoteCartItemPrice(guid, updatedUnitPrice, productid, shippingid, Order.prototype.IsQuote(), userId, function (response) {
            if (response.success) {
                $("#divShoppingCart").html("");
                $("#divShoppingCart").html(response.html);

                let orderId: number = Order.prototype.GetOrderId();
                let portalId: number = Order.prototype.GetPortalId();

                //Calculate the shopping cart
                Order.prototype.CalculateShoppingCart(userId, portalId, orderId, false);
            }
            ZnodeBase.prototype.HidePartialLoader("loader-cart-content");
        });
    }

    //Save order line item shipping
    SaveOrderLineItemShipping(event): any {
        var target = $(event.target);
        var newShippingValue = target.val();
        var extenalId = target.next().next('#hdnExternalId').val();
        var oldShippingValue = target.next('#hdnQuoteLineItemShipping_' + extenalId).val();
        newShippingValue = newShippingValue.replace(/[$]/g, '');
        var matches = newShippingValue.match(/^-?[\d.]+(?:e-?\d+)?/);

        if (matches == null || !newShippingValue || newShippingValue < 0 ) {
            target.val(oldShippingValue);
            event.stopImmediatePropagation();
            return false;
        }

        $('#IsQuoteLineShippingUpdated_' + extenalId).val("true");
        return true;
    }

    UpdateQuoteLineItemShipping(data: any): any {
        if (data) {
            $.each(data, function (index, element) {
                $("#shippingcost_" + element.Item1).val(element.Item2);
                $("#hdnQuoteLineItemShipping_" + element.Item1).val(element.Item2);
            });
        }
    }

    //Get Manage Quote Additional note pop up
    GetQuoteAdditionalNote(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanelWithCallBack('/Quote/GetAdditionalNotes', 'additionalNotes', function (response) {
            $('#notes').val($('#additionalNotes').val());
        });
    }

    /* This method will check whether the quote is old quote or not & will show the notification on edit screen
     to notify the admin that this is old quote. So if any updation do so it will work as per new quote calculation flow
     & may impact the calculation in case of absent data in old quote */
    ShowOldQuoteNotification(isOldQuote): any {

        if (typeof isOldQuote === "undefined" || isOldQuote.toLowerCase() == "false")
            return false;
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("IsAnOldQuoteMessage"), "error", false, 36000000);
            return true;
        }
    }

    CheckIsQuoteOrder(): any {
        var isOldQuote = $("#hdnIsOldQuote").val()

        if (typeof isOldQuote !== "undefined" && isOldQuote.toLowerCase() == "true") {
            var currentTarget = $('#shippingTypes');
            Quote.prototype.GetSelectedShipping(currentTarget);
        }
    }

    ShowTaxDetails(element): void {
        var taxSummary = $(element).closest("#taxTotalDiv").find("#TaxSummary");
        if (taxSummary.is(':visible')) {
            taxSummary.hide();
        }
        else {
            taxSummary.show();
        }
    }
    // Submit Braintree Quote against Quote to Order  
    SubmitBraintreeQuote(payload,isVault) {
        $('#BraintreeSubmitBtn').prop("disabled", true);
        $('#BraintreeCancelBtn').prop("disabled", true);
        var cardDetails = payload.details;
        $('#hdnBraintreecardNumber').val(cardDetails.lastFour);
        $("#hdnBraintreeCardExpirationMonth").val(cardDetails.expirationMonth);
        $("#hdnBraintreeCardExpirationYear").val(cardDetails.expirationYear);
        $("#hdnBraintreeCardHolderName").val(cardDetails.cardholderName);
        $("#hdnBraintreeCardType").val(cardDetails.cardType);
        $("#hdnBraintreeNonce").val(payload.nonce);
        $("#hdnBraintreecode").val(Constant.BrainTree);
        $("#hdnBraintreeIsVault").val(isVault);
        Quote.prototype.ConvertQuoteToOrder();
    }

     //Validate the braintree fields is null or not
    ValidateBrainTreeCardDetails() {
        if (($('#hdnBraintreecardNumber').val() == "" && ($('#hdnBraintreecardNumber').val().length <= 0 || $('#hdnBraintreecardNumber').val().length > 4)) &&
            $("#hdnBraintreeCardExpirationMonth").val() == "" && $("#hdnBraintreeCardExpirationYear").val() == "" &&
            $("#hdnBraintreeCardHolderName").val() == "" && $("#hdnBraintreeCardType").val() == "" && $("#hdnBraintreeNonce").val() == "") {
            return false;
        } else {
            return true;
        }
    }
}
$('#PopUpConvertToOrder').find('#btn-cancel-popup').click(function () {
    Quote.prototype.ResetQuoteStatus();
});