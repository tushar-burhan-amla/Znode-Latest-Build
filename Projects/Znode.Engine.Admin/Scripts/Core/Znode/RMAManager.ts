class RMAManager extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    subTax: number = 0;
    constructor() {
        super();
    }

    Init(): any {
        RMAManager.prototype.CalculateSubTotal();
        RMAManager.prototype.GetRMAItemTotal();
        RMAManager.prototype.RemoveRMAGridIcone();
    }

    GetRMAItemTotal(): any {
        $('#rmaGrid tbody td select[name=ddlRmaQuantity]').on("change", function (e) {
            var currencyCode = $("#CurrencyCode").val();
            var cultureCode = $("#CultureCode").val();
            var row = $(this).closest('tr');
            var qty = $(this).val();
            var price = $(row).find(".BasePrice").text();
            var total = parseFloat(price.replace(/[^0-9\.]+/g, "")) * Number(qty);
            var totalWithDecimal = total % 1 != 0 ? total : total + 0.00;

            Endpoint.prototype.GetConvertedDecimalValues(total, cultureCode, function (response) {
                $(row).find(".Total").text(response._currencyValue);
                RMAManager.prototype.RMACalculation();
            });
        });
    }

    CalculateSubTotal(): any {
        $('#rmaGrid tbody td input:checkbox[name=IsReturnable]').on("click", function (e) {
            RMAManager.prototype.RMACalculation();
        });
    }

    CreateRMARequest(): any {
        var checkedItems = $('#rmaGrid tr').filter(':has(:checkbox:checked)').find('td');
        if (checkedItems.length < 1) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectRMAItem"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        var items = RMAManager.prototype.GetGridData();

        var tax: number = RMAManager.prototype.subTax;

        var subTotal = $("#hdnSubtotal").val();

        var total: number = Number(subTotal.replace(/[^0-9\.]+/g, "")) + tax;

        var requestModel = {
            RMARequestId: 0,
            RequestNumber: $("#requestNumber").val(),
            TaxCost: tax,
            SubTotal: subTotal,
            Total: total,
            Comments: $('textarea#Comments').val(),
            Flag: "",
            EmailRMAReport: $("#emailRMA").prop('checked'),
            RMARequestItems: items
        };
        Endpoint.prototype.createRMARequest(requestModel, function (response) {
            if (response.success) {
                window.location.href = "/rmamanager/rmalist/null?messageShowStatus=true";
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
    }

    SelectAll(): any {
        if ($("input:checkbox[name=chkall]").prop('checked')) {
            $("input:checkbox[name=IsReturnable]").prop('checked', true);
        }
        else {
            $("input:checkbox[name=IsReturnable]").prop('checked', false);
        }
        RMAManager.prototype.RMACalculation();
    }

    GetGridData(): any {
        var items = new Array();
        var orderId = $("#OrderId").val();
        var subTotal = 0;
        $('#rmaGrid tr').not('thead tr').each(function () {
            var checkbox = $(this).find('input:checkbox')[0];
            if ($(this).find('input:checkbox').prop('checked')) {
                var omsOrderLineItemsId = $(this).find(".OmsOrderLineItemsId").text();
                var quantity = $(this).find("[name=ddlRmaQuantity] :selected").val();
                var rmaReasonForReturnId = $(this).find("[name=ddlReasonForReturn] :selected").val();
                var price = $(this).find(".BasePrice").text();
                items.push({ OmsOrderId: orderId, OmsOrderLineItemsId: omsOrderLineItemsId, Quantity: quantity, Price: Number(price.replace(/[^0-9\.]+/g, "")), RmaReasonForReturnId: rmaReasonForReturnId });
                subTotal += Number(price.replace(/[^0-9\.]+/g, "")) * parseFloat(quantity);
            }
        });
        $("#hdnSubtotal").val(subTotal);
        return items;
    }

    RMACalculation(): any {
        RMAManager.prototype.subTax = 0;
        if ($('input:checkbox[name=IsReturnable]:checked').length == $('input:checkbox[name=IsReturnable]').length)
            $("input:checkbox[name=chkall]").prop('checked', true);
        else
            $("input:checkbox[name=chkall]").prop('checked', false);

        Order.prototype.ShowLoader();
        var mode = $("#hdnMode").val();
        var subTotal = 0;
        var total = 0;
        var currencyCode = $("#CurrencyCode").val();
        var cultureCode = $("#CultureCode").val();
        $('#rmaGrid tr').not('thead tr').each(function (e) {
            if ($(this).find('input:checkbox').prop('checked')) {
                var quantity = $(this).find("[name=ddlRmaQuantity] :selected").val();
                var price = $(this).find(".BasePrice").text();
                subTotal += Number(price) * parseFloat(quantity);
                var taxPrice = $(this).find(".Tax").text();
                RMAManager.prototype.subTax += Number(taxPrice.replace(/[^0-9\.]+/g, "")) * parseFloat(quantity);
            }

        });
        total = (RMAManager.prototype.subTax + subTotal);
        if (mode.toLowerCase() != "view") {
            Endpoint.prototype.GetConvertedDecimalValues(subTotal, cultureCode, function (response) {
                $("#dynamic-sub-total").html(response._currencyValue);
            });
            Endpoint.prototype.GetConvertedDecimalValues(RMAManager.prototype.subTax, cultureCode, function (response) {
                $("#dynamic-tax-amount").html(response._currencyValue);
            });
            Endpoint.prototype.GetConvertedDecimalValues(total, cultureCode, function (response) {
                $("#dynamic-total").html(response._currencyValue);
                Order.prototype.HideLoader();
            });
        }
    }


    //Print RMA Request
    PrintContent(): any {
        var RequestNumber = $("#requestNumber").val();
        var OrderId = $("#OmsOrderDetailsId").val();
        var OmsOrderId = $("#OMSOrderId").val();
        var StoreName = $("#StoreName").val();
        var CustomerName = $("#CustomerName").val();
        var SalesPhoneNumber = $("#SalesPhoneNumber").val(); 
        var OrderNumber = $("#orderNumber").val();
        var divContents = $("#rmalist").html();
        var $s = $(divContents).find('#orderid').remove();

        var printWindow = window.open('', '', 'height=500,width=900');
        printWindow.document.write('<html><head><title>RMA Request Report</title><style type="text/css">.rma-print{font-family:arial;font-size:13px;margin:0;color:#474747;} header{background-color:#5b6770;height:40px;} header .logo img{height:20px;padding:0;} header .logo{padding:8px 10px 10px;} header .logo .z-multifront {border-left:1px solid #ba5650;height:18px;margin-left:8px;padding-left:8px;vertical-align:bottom;} .title-container{box-shadow:0 5px 5px -5px #dcdcdc;padding:10px 20px;} .title-container:after{content:"";display:block;clear:both;} .title-container h1{font-size:20px; line-height:35px;font-weight:normal;float:left;margin:0;} .btn-text{background-color:#891e17;color:#fff;min-width:90px;border:0;text-transform:capitalize;border-radius:2px;font-size:12px;height:24px;line-height:23px;float:right;cursor:pointer;} textarea{background-color:#fff;border:0;resize:none;color:#474747;font-family:arial;font-size:13px;padding:0;} h3{font-size:14px;color:#323230;} .form-group h3{text-transform:uppercase;} .table{border:1px solid #ddd;width:100%;margin-bottom:25px;border-spacing:0;} .table thead tr th{border-bottom:1px solid #c3c3c3;padding:5px 3px;text-align:left;font-size:13px;font-weight:normal} .table tbody tr td{padding:5px 3px;border:0;font-size:13px;} .form-group{margin:0 0 8px;} .form-group:after{content:"";display:block;clear:both;} .form-group .control-label{float:left;width:170px;} .form-group .control-md{float:left;width:35%;} .col-sm-3{float:left;width:25%;} .col-sm-9{float:left;width:75%;} .cost-container{margin:0 0 20px;} .gift-card-status{border-bottom:1px solid #c3c3c3;padding-bottom:5px;margin-bottom:20px;} .pull-right{float:right;} .page-container{padding:20px;}@media print{.btn-text{display:none;} .page-container{padding:0;} .title-container{padding:10px 0 8px; border-bottom:1px solid #c3c3c3; margin-bottom:20px;box-shadow:none;} .title-container h1{float:none;}}</style>');
        printWindow.document.write('</head><body class="rma-print"><div class="col-sm-12 top-shift"><div class="label-background"><h3 class="left">' + StoreName + '</h3><h3 class="right">' + SalesPhoneNumber + '</h3></div></div><div class="col-sm-12 title-container"><h1>Return Merchandise Authorization</h1><div class="pull-right"><button class="btn-text" type="button" onclick="window.print()" id="printbtn">Print</button></div></div>');
        printWindow.document.write('<section class="col-md-12 page-container"><div class="col-sm-12"><strong> Dear ' + CustomerName + ', </strong><p> Please be advised your request for return has been approved.Your Return Merchandise Authorization (RMA) number is: ' + RequestNumber + ' and your order number is ' + OrderNumber + '</p></div>');
        printWindow.document.write(divContents);
        printWindow.document.write('<div><p>Regards</p></div><div>' + StoreName + '</div></div></div></section></body></html>');
        printWindow.document.close();
    }


    //To Do: Working on it.
    IssueGiftCard(): any {
        var Ids = $('#rmaGrid tr').not('thead tr').filter(':has(:checkbox:checked)').find('td');
        if (Ids.length < 1) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneProduct"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        var items = RMAManager.prototype.GetGiftCardGridData();

        var orderLineItems = "";
        for (var i = 0; i < items.length; i++) {
            orderLineItems = orderLineItems + items[i].OrderLineItemId + ",";
        }
        orderLineItems = orderLineItems.substr(0, orderLineItems.length - 1);
        var quantities = "";
        for (var i = 0; i < items.length; i++) {
            quantities = quantities + items[i].Quantity + ",";
        }
        quantities = quantities.substr(0, quantities.length - 1);
        var tax = RMAManager.prototype.subTax;
        var subTotal = Number($("#hdnSubtotal").val().replace(/[^0-9\.]+/g, "")).toFixed(2);
        $("#OrderLineItems").val(orderLineItems);
        $("#Quantities").val(quantities);
        $("#Mode").val("rma");
        $("#Comments").val($('textarea#Comments').val());
        $("#CurrencyCode").val($('.CurrencyCode')[0].innerHTML.trim());
        $("#CultureCode").val($('.CultureCode')[0].innerHTML.trim());
        var total = (parseFloat(subTotal) + tax).toFixed(2);
        $("#Total").val(total);
        if (total == "0.00") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorTotalCannotlesszero"), "error", isFadeOut, fadeOutTime);
            return false;
        }
        var orderId = $("#OmsOrderDetailsId").val();
    }

    GetGiftCardGridData(): any {
        var items = new Array();
        var orderId = $("#OrderId").val();
        var subTotal = 0;
        $('#rmaGrid tr').not('thead tr').each(function () {
            if ($(this).find('input:checkbox').prop('checked')) {
                var orderLineItemId = $(this).find(".OmsOrderLineItemsId").text();
                var quantity = $(this).find("[name=ddlRmaQuantity] :selected").val();
                var reasonForReturnId = 0;
                var price = $(this).find(".BasePrice").text();
                items.push({ OrderId: orderId, OrderLineItemId: orderLineItemId, Quantity: quantity, Price: Number(price.replace(/[^0-9\.]+/g, "")), ReasonForReturnId: reasonForReturnId });
                subTotal += Number(price.replace(/[^0-9\.]+/g, "")) * parseFloat(quantity);
            }
        });
        $("#hdnSubtotal").val(subTotal);
        return items;
    }

    RemoveRMAGridIcone(): any {
        $('#grid tbody tr').each(function () {
            if ($(this).find('.requestStatus').text() == 'Authorised' || $(this).find('.requestStatus').text() == '') {
                $(this).find('.z-append').parents('li').remove();
            }
            else {
                $(this).find('.z-edit').parents('li').remove();
            }
        });
    }
}