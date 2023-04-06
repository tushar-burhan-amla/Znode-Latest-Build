declare function autocompletewrapper(control: any, controlValue: any): any;
class QuickOrderPad extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        var productId = ZSearch.prototype.GetQueryStringParameterByName("ProductId");
        if (productId == "0") {
            $('#btnQuickOrderPad').attr('disabled', 'disabled');
            $('#btnQuickOrderClearAll').prop('disabled', 'disabled');
        }

        QuickOrderPad.prototype.QuickOrderPadAutoComplete();
        QuickOrderPad.prototype.GenerateNewRow();
        QuickOrderPad.prototype.AddMultipleOrdersToCart();
        QuickOrderPad.prototype.ClearAll();
        QuickOrderPad.prototype.RemoveRow();
        QuickOrderPad.prototype.ShowRemoveItemBox();
        QuickOrderPad.prototype.SetQuantity();
    }

    QuickOrderPadAutoComplete() {

    }
    OnItemSelect(item): any {
        var act = document.activeElement;
        $('#btnQuickOrderClearAll').prop('disabled', false);
        QuickOrderPad.prototype.SetAutoCompleteItemProperties(act, item.id);
    }

    OnQuantityChange(control) {
        if ($("#inventoryMessage_" + control.id.split("_")[1]).html() == '' || $("#inventoryMessage_" + control.id.split("_")[1]).html() == undefined) {
            if ($("#txtQuickOrderPadQuantity_" + control.id.split("_")[1]).val() != '0') {
                $('#btnQuickOrderClearAll').prop('disabled', false);
                QuickOrderPad.prototype.ValidateQuickOrderItems();
            }
            else {
            $('#btnQuickOrderPad').prop('disabled', true);
            QuickOrderPad.prototype.ValidateQuickOrderItems();
            }
        }
    }

    MapProductDetails(control, res) {
        $(control).val(res.DisplayText)
        $('.quick-order-pad-autocomplete').val(res.DisplayText);
        $(control).attr("data_qo_sku", res.DisplayText);
        $(control).attr("data_qo_product_id", res.Id);
        $(control).attr("data_qo_product_name", res.Properties.ProductName);
        $(control).attr("data_qo_cart_quantity", res.Properties.CartQuantity);
        $(control).attr("data_qo_quantity_on_hand", res.Properties.Quantity);
        $(control).attr("data_qo_product_type", res.Properties.ProductType);
        $(control).attr("data_qo_addon_product", res.Properties.AddOnProductSkus);
        $(control).attr("data_qo_retail_price", res.Properties.RetailPrice);
        $(control).attr("data_qo_group_product_sku", res.Properties.GroupProductSKUs);
        $(control).attr("data_qo_group_product_qty", res.Properties.GroupProductsQuantity);
        $(control).attr("data_qo_configurable_product_sku", res.Properties.ConfigurableProductSKUs);
        $(control).attr("data_qo_autoaddonskus", res.Properties.AutoAddonSKUs);
        $(control).attr("data_qo_inventorycode", res.Properties.InventoryCode);
        $(control).attr("data_qo_isobsolete", res.Properties.IsObsolete);
        if (res.Properties.CallForPricing != undefined) {
            $(control).attr("data_qo_call_for_pricing", res.Properties.CallForPricing);
        }
        else { $(control).attr("data_qo_call_for_pricing", ''); }
        if (res.Properties.TrackInventory != undefined) {
            $(control).attr("data_qo_track_inventory", res.Properties.TrackInventory);
        }
        else { $(control).attr("data_qo_track_inventory", ''); }
        if (res.Properties.OutOfStockMessage != undefined) {
            $(control).attr("data_qo_out_stock_message", res.Properties.OutOfStockMessage);
        }
        if (res.Properties.MaxQuantity != undefined) {
            $(control).attr("data_qo_max_quantity", res.Properties.MaxQuantity);
        }
        if (res.Properties.MinQuantity != undefined) {
            $(control).attr("data_qo_min_quantity", res.Properties.MinQuantity);
        }
        if (res.Properties.IsActive != undefined) {
            $(control).attr("data_qo_isactive", res.Properties.IsActive);
        }
        if ($(control).val() == $(control).attr("data_qo_sku") && $(control).val() != '' && $(control).val() != undefined && control.id == undefined) {
            $("#txtQuickOrderPadQuantity_" + control[0].id.split("_")[1]).val("1");
        }
        if ($(control).val() == $(control).attr("data_qo_sku") && $(control).val() != '' && $(control).val() != undefined) {
            $("#txtQuickOrderPadQuantity_" + control.id.split("_")[1]).val("1");
        }
        else {
            $("#txtQuickOrderPadQuantity_" + control.id.split("_")[1]).val("0");
        }
    }


    SetAutoCompleteItemProperties(control, productId) {
        Endpoint.prototype.GetAutoCompleteItemProperties(productId, function (res) {
            QuickOrderPad.prototype.MapProductDetails(control, res);
            QuickOrderPad.prototype.ValidateQuickOrderItems();
            QuickOrderPad.prototype.CheckDuplicateSKUs();
        });
    }

    DownloadQuickOrderTemplate(fileType: string) {
        window.location.href = window.location.protocol + "//" + window.location.host + "/Product/DownloadQuickOrderTemplate?fileType=" + fileType;
    }

    ValidateQuickOrderItems() {
        var showAddToCart = true;
        var existVal = 0;
        $('.quickOrderPadAddToCart').prop('disabled', false);
        $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
            if (this.id.length > 0 && !isNaN(parseInt($(this).attr("data_qo_product_id"))) && $(this).attr("data_qo_product_id") != "" && $(this).attr("data_qo_product_id") != "0") {
                $('p[for="' + 'inventoryquickorderMessage_' + this.id.split("_")[1] + '"]').html('');
                $('p[for="' + 'txtQuickOrderPadQuantity_' + this.id.split("_")[1] + '"]').html('');
                let result = QuickOrderPad.prototype.ValidateAutoCompleteItemProperties(this);
                if (result.showAddToCart && showAddToCart) {
                    showAddToCart = true;
                }
                else {
                    showAddToCart = false;
                }
            }
            if ($(this).val() !== "") {
                existVal = 1;
            }
        });

        if (!showAddToCart || existVal === 0) {
            $('#btnQuickOrderPad').attr('disabled', 'disabled');
        }
    }

    IsValidProductSKU(control) {
        if ($(control).val() != $(control).attr("data_qo_sku") && $(control).val() != "") {
            $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
            $('#btnQuickOrderClearAll').prop('disabled', false);
            return false;
        }
        else
        {
            QuickOrderPad.prototype.ShowRemoveItemBox();
        }
    }
    CheckUpdateSKU(control, e) {
        if (e.keyCode === 13 || e.keyCode === 9 || e.type === "blur") {
            var skuProductId = $(control).val();
            if (skuProductId != "" && skuProductId != undefined) {
                Endpoint.prototype.GetProductDetailsBySKU(skuProductId, function (res) {
                    if (res.DisplayText != null && res.Id > 0) {
                        $('p[for="' + control.id + '"]').html("");
                        var act = control;
                        QuickOrderPad.prototype.MapProductDetails(act, res);
                    }
                    $('#btnQuickOrderClearAll').prop('disabled', false);
                    QuickOrderPad.prototype.ValidateQuickOrderItems();
                    QuickOrderPad.prototype.CheckDuplicateSKUs();
                });
            }

            if ($(control).val() != $(control).attr("data_qo_sku") && ($(control).val() == "")) {
                var QuickPadRow = control.id.split('_')[1];
                if (QuickPadRow <= 5) {
                    QuickOrderPad.prototype.ClearSelectedData(control.id.split('_')[1]);
                    QuickOrderPad.prototype.ShowRemoveItemBox();
                    QuickOrderPad.prototype.IsValidProductSKUQuick(control);
                    return false;
                }
                else {
                    QuickOrderPad.prototype.ClearSelectedData(control.id.split('_')[1]);
                    QuickOrderPad.prototype.ShowRemoveItemBoxForNewRow(control.id);
                    QuickOrderPad.prototype.IsValidateQuantity();
                }
            }
            if ($(control).val() == $(control).attr("data_qo_sku") && ($(control).val() == "")) {
                QuickOrderPad.prototype.ValidateQuickOrderItems();
            }
        }
    }

    IsValidateQuantity() {
        var textBoxIdsArray = ($("*[id*='txtQuickOrderPadQuantity_']")).map(function () {
            return this.id;
        }).get();

        var textBoxSplittedIdsArray = [];
        for (let textBoxIdIndex = 0; textBoxIdIndex < textBoxIdsArray.length; textBoxIdIndex++)
        {
            let textBoxSplittedId = (textBoxIdsArray[textBoxIdIndex].split("_")[1])
            textBoxSplittedIdsArray.push(textBoxSplittedId);
        }

        for (let textBoxSplittedIdIndex = 0; textBoxSplittedIdIndex < textBoxSplittedIdsArray.length; textBoxSplittedIdIndex++) {
            if ($("#txtQuickOrderPadQuantity_" + textBoxSplittedIdsArray[textBoxSplittedIdIndex]).length != 0)
            {
                if ($("#txtQuickOrderPadQuantity_" + textBoxSplittedIdsArray[textBoxSplittedIdIndex]).val() != '0')
                {
                    $('#btnQuickOrderClearAll').prop('disabled', false);
                    $('#btnQuickOrderPad').prop('disabled', false);
                }
            }
        }
    }
    IsValidProductSKUQuick(control) {
        if ($(control).val() != $(control).attr("data_qo_sku") && ($(control).val() != "")){
            $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
            $('#btnQuickOrderClearAll').prop('disabled', false);
            return false;
        }

        if ($(control).val() == "") {
            QuickOrderPad.prototype.ClearSelectedData(control.id.split('_')[1]);
            QuickOrderPad.prototype.ShowRemoveItemBox();

        }
    }   

    AddProductsToQuickOrder() {
        var mutipleItems = $("#txtAddMultipleItems").val();
        var blankRowIndex = 0;
        if (mutipleItems.length > 0) {
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.AddProductsToQuickOrder(mutipleItems, function (res) {
                if (res.rowsHtml.length > 0 && res.response.IsSuccess) {
                    $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
                        if (this.id.length > 0 && isNaN(parseInt($(this).attr("data_qo_product_id"))) && ($(this).attr("data_qo_product_id") == "") && $(this).val() == "") {
                            var index = this.id.split('_')[1];
                            QuickOrderPad.prototype.ClearSelectedData(index);
                            $("#removeRow_" + index).hide();
                            $('#form-group-' + index).remove();
                            blankRowIndex = blankRowIndex + 1;
                        }
                    });
                    $("#quickorderdiv").append(res.rowsHtml);
                    if (res.hasOwnProperty("response") && res.response.hasOwnProperty("ValidSKUCount")) {
                        blankRowIndex = blankRowIndex - (parseFloat(res.response.ValidSKUCount) );
                        if (blankRowIndex > 0) {
                            QuickOrderPad.prototype.LoadBlankQuickGridRow(blankRowIndex);
                        }
                    }
                    if (res.response.IsSuccess) {
                        $('#btnQuickOrderClearAll').prop('disabled', false);
                    }
                    QuickOrderPad.prototype.ValidateQuickOrderItems();
                    QuickOrderPad.prototype.MergeDuplicateSKUs();
                    QuickOrderPad.prototype.LimitQuickOrderUptoFiftyRecords(res);
                    $("#txtAddMultipleItems").val(res.response.ProductSKUText);

                }
                if (res.notificationHtml.length > 0) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.notificationHtml, res.response.IsSuccess ? "success" : "error", isFadeOut, fadeOutTime);
                }
                ZnodeBase.prototype.HideLoader();
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorItemNumberField"), "error", isFadeOut, fadeOutTime);
        }
    }

    UploadQuickOrderFile(files): any {
        var filesSelected = $('#selectfile').val();
        if (files.length > 0 && filesSelected != '') {
            ZnodeBase.prototype.ShowLoader();
            $('#selectfile').val(files[0].name);

            var fileData = new FormData();
            fileData.append(files[0].name, files[0]);


            var fileExtension = files[0].name.split('.').pop().toLowerCase();
            if (fileExtension != "") {
                if ($.inArray(fileExtension, ['csv', 'xls', 'xlsx']) == -1) {
                    ZnodeBase.prototype.HideLoader();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("InvalidCSVFileType"), "error", isFadeOut, fadeOutTime);
                    return false;
                }
            }

            $.ajax({
                url: '/Product/AddProductsToQuickOrderUsingExcel',
                type: 'post',
                data: fileData,
                dataType: 'json',
                contentType: false,
                processData: false,
                beforeSend: function () {
                    ZnodeBase.prototype.ShowLoader();
                },
                success: function (res) {
                    ZnodeBase.prototype.HideLoader();
                    $("#file-input").replaceWith($("#file-input").val('').clone(true));
                    $('#selectfile').val('');
                    if (res.rowsHtml.length > 0 && res.response.IsSuccess) {

                        $('#quick-order-pad-content [data-autocomplete-url]').each(function () {

                            if (this.id.length > 0 && $(this).val() == "") {
                                var index = this.id.split('_')[1];
                                QuickOrderPad.prototype.ClearSelectedData(index);
                                $("#removeRow_" + index).hide();
                                $('#form-group-' + index).remove();
                            }
                        });
                        $("#quickorderdiv").append(res.rowsHtml);
                        $("#txtAddMultipleItems").val(res.response.ProductSKUText);
                        if (res.response.IsSuccess) {
                            $('#btnQuickOrderClearAll').prop('disabled', false);
                        }
                        QuickOrderPad.prototype.ValidateQuickOrderItems();
                        QuickOrderPad.prototype.MergeDuplicateSKUs();
                        QuickOrderPad.prototype.LimitQuickOrderUptoFiftyRecords(res);
                    }
                    else {
                        $("#txtAddMultipleItems").val(res.response.ProductSKUText);
                    }
                    if (res.notificationHtml.length > 0) {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.notificationHtml, res.response.IsSuccess ? "success" : "error", isFadeOut, fadeOutTime);
                    }
                },
                error: function (msg) {
                }
            });
        }
    }

    LoadBlankQuickGridRow(index) {
        var rowIndex;
        var i;
        for (i = 1; i <= index; i++) {
            rowIndex = i + 99;          
            $("#quickorderdiv").append('<div id="quickorderdiv"><div class="form-group" id="form-group-' + rowIndex + '"><div class="row no-gutters align-items-center"><div class="col-8"><input class="typeahead" data-autocomplete-id-field="Id" data-autocomplete-url="/product/getproductlistbysku" data-onselect-function="QuickOrderPad.prototype.OnItemSelect" data_autocomplete_url="/product/getproductlistbysku" data_is_first="true" data_onselect_function="QuickOrderPad.prototype.OnItemSelect" data_qo_addon_product="" data_qo_autoaddonskus="" data_qo_call_for_pricing="" data_qo_cart_quantity="0" data_qo_configurable_product_sku="" data_qo_group_product_qty="0" data_qo_group_product_sku="" data_qo_in_stock_message="" data_qo_inventorycode="" data_qo_isactive="false" data_qo_isobsolete="" data_qo_max_quantity="" data_qo_min_quantity="" data_qo_out_stock_message="" data_qo_product_id="" data_qo_product_name="" data_qo_product_type="" data_qo_quantity_on_hand="" data_qo_retail_price="" data_qo_sku="" data_qo_track_inventory="" id="txtQuickOrderPadSku_' + rowIndex + '" name="Name" onchange="QuickOrderPad.prototype.IsValidProductSKU(this)" onKeyPress = "QuickOrderPad.prototype.CheckUpdateSKU(this,event)" onblur= "QuickOrderPad.prototype.CheckUpdateSKU(this,event)"  placeholder="Enter SKU" type="text" value="" /></div><div class="col-3 px-2 px-md-3"><input class="quantity quick-order-pad-quantity text-right" id="txtQuickOrderPadQuantity_' + rowIndex + '" maxlength="4" name="txtQuickOrderPadQuantity_' + rowIndex + '" parentcontrol="txtQuickOrderPadSku_' + rowIndex + '" placeholder="Qty" type="text" value="0" /></div><div class="col-1"><div id="removeRow_' + rowIndex + '" class="remove_row remove-item" title="Clear"><i class="close-icon"></i></div></div><p id="inventoryMessage_' + rowIndex + '" for="txtQuickOrderPadSku_' + rowIndex + '" class="col-xs-12 nopadding error-msg"></p></div></div></div>');
            autocompletewrapper($("#txtQuickOrderPadSku_" + rowIndex), $("#txtQuickOrderPadSku_" + rowIndex).data('onselect-function'));

        }
        QuickOrderPad.prototype.SetQuantity();
        QuickOrderPad.prototype.RemoveRow();
    }

    DisplayQuickOrderNotification(response) {
        if (response != null && response != undefined) {
            var element: any = $(".messageBoxContainer");
            $(".messageBoxContainer").removeAttr("style");
            $(window).scrollTop(0);
            $(document).scrollTop(0);
            if (element.length) {
                if (response !== "" && response != null) {
                    element.html(response);
                    setTimeout(function () {
                        element.fadeOut().empty();
                    }, fadeOutTime);
                }
            }
        }
    }
    GetFirstOrderRowId(): number {
        var index = 1;
        var flag = true;
        $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
            if (this.id.length > 0 && flag) {
                index = this.id.split('_')[0];
                flag = false;
            }
        });
        return index;
    }

    CheckDuplicateSKUs() {
        var listOfSKUs = [];
        $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
            if ($(this).attr("data_qo_sku") != "" && this.id.length > 0) {
                listOfSKUs.push($(this).attr("data_qo_sku"));
            }
        });
        var duplicateSKUsCountObj = {};
        $.each(listOfSKUs, function (key, value) {
            var numOccr = $.grep(listOfSKUs, function (elem) {
                return elem === value;
            }).length;
            duplicateSKUsCountObj[value] = numOccr
        });
        $.each(duplicateSKUsCountObj, function (key, value) {
            if (value > 1 && $('#quick-order-pad-content [data_qo_sku="' + key + '"]').length > 1) {
                var controlId = $('#quick-order-pad-content [data_qo_sku="' + key + '"]')[$('#quick-order-pad-content [data_qo_sku="' + key + '"]').length - 1].id;
                $('p[for="' + controlId + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorDuplicatedSKU"));
            }
        });
    }

    LimitQuickOrderUptoFiftyRecords(res) {
        var count = 0;
        if ($('.form-group').length > 51) {
            for (let i = 51; i < $(".form-group").length; i++) {
                $(".form-group")[i].remove();
                count++;
            }
            res.notificationHtml = ZnodeBase.prototype.getResourceByKeyName("QuickOrderLimit");
            res.response.IsSuccess = false;
        }
    }

    MergeDuplicateSKUs() {
        var listOfSKUs = [];
        $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
            if ($(this).attr("data_qo_sku") != "" && this.id.length > 0) {
                listOfSKUs.push($(this).attr("data_qo_sku"));
            }
        });
        var duplicateSKUsCountObj = {};
        $.each(listOfSKUs, function (key, value) {
            var numOccr = $.grep(listOfSKUs, function (elem) {
                return elem === value;
            }).length;
            duplicateSKUsCountObj[value] = numOccr
        });
        $.each(duplicateSKUsCountObj, function (key, value) {
            if (value > 1 && $('#quick-order-pad-content [data_qo_sku="' + key + '"]').length > 1) {
                var quantity = 0;
                $.each($('#quick-order-pad-content [data_qo_sku="' + key + '"]'), function (key1, value1) {
                    if (value1.id != undefined && value1.id.length > 0) {
                        console.log("Duplicate id" + value1.id);
                        var selectedRow = value1.id.split('_')[1];
                        quantity = quantity + parseInt($("#txtQuickOrderPadQuantity_" + selectedRow).val());

                        if (value1.id != $('#quick-order-pad-content [data_qo_sku="' + key + '"]')[$('#quick-order-pad-content [data_qo_sku="' + key + '"]').length - 1].id) {
                            QuickOrderPad.prototype.ClearSelectedData(selectedRow);
                            $("#removeRow_" + selectedRow).hide();
                            $('#form-group-' + selectedRow).remove();
                        }
                        else {
                            $("#txtQuickOrderPadQuantity_" + selectedRow).val(quantity);
                            if (parseFloat($(value1).attr("data_qo_max_quantity")) > 0 && parseFloat($(value1).attr("data_qo_max_quantity")) < quantity + parseFloat($(value1).attr("data_qo_cart_quantity"))) {
                                $('p[for="' + value1.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorDuplicateSKUWithMaxCartQuantity"));
                            }
                            else if (parseFloat($(this).attr("data_qo_min_quantity")) > 0 && parseFloat($(this).attr("data_qo_min_quantity")) > quantity + parseFloat($(value1).attr("data_qo_cart_quantity"))) {
                                $('p[for="' + value1.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMinCartQuantity"));
                            }
                            else {
                                $('p[for="' + value1.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorDuplicatedSKU"));
                            }
                        }
                    }
                });
            }
        });
    }


    ValidateAutoCompleteItemProperties(control) {
        var track_inventory = $(control).attr("data_qo_track_inventory");
        var quantity_on_hand = $(control).attr("data_qo_quantity_on_hand");
        var call_for_pricing = $(control).attr("data_qo_call_for_pricing");
        var product_type = $(control).attr("data_qo_product_type");
        var retail_price = $(control).attr("data_qo_retail_price");
        var inventorySettingQuantity = $(control).attr("data_qo_quantity_on_hand");
        var autoAddOnProductSkus = $(control).attr("data_qo_autoaddonskus");
        var inventoryCode = $(control).attr("data_qo_inventorycode");
        var isObsolete = $(control).attr("data_qo_isobsolete");
        var isActive = $(control).attr("data_qo_isactive");
        var childTrackInventory = $(control).attr("data_qo_child_track_inventory");
        var isSuccess = true;
        var showAddToCart = true;
        var lastIndex = control.id.split("_")[1];
        $('p[for="' + control.id + '"]').html("");        
        if (control.id.length > 0 && !isNaN(parseInt($(control).attr("data_qo_product_id"))) && $(control).attr("data_qo_product_id") != "" && $(control).attr("data_qo_product_id") != "0") {
            let groupProductQty: string;
            let groupProductSKUs: string = $(control).attr("data_qo_group_product_sku");
            let configurableProductSKUs: string = $(control).attr("data_qo_configurable_product_sku");

            if (groupProductSKUs != undefined) {
                groupProductQty = new Array(groupProductSKUs.split(",").length + 1).join($('input[parentcontrol=' + control.id + ']').val() + "_").replace(/\_$/, '');
            }

            var quantity = parseFloat($("input[parentcontrol=" + control.id + "]").val());           
            
            if ($(control).attr("data_is_first") == "false") {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
                isSuccess = false;                
            }
            else if ($(control).val() != $(control).attr("data_qo_sku") && $(control).val() != '' && $(control).val() != undefined) { 
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if (isActive == "false") {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorInValidSKU"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if (isObsolete == "true") {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ObsoleteProductErrorMessage"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if ((track_inventory == "DisablePurchasing") && parseInt(quantity_on_hand) <= 0) {
                $('p[for="' + control.id + '"]').html($(control).attr("data_qo_out_stock_message"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if ((track_inventory == "DisablePurchasing") && parseInt(quantity_on_hand) == parseInt($(control).attr("data_qo_cart_quantity"))) {
                $('p[for="' + control.id + '"]').html($(control).attr("data_qo_out_stock_message"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if ((inventoryCode.toLowerCase().trim() != "donttrackinventory" && inventoryCode.toLowerCase().trim() != "allowbackordering") && (inventorySettingQuantity == "" || inventorySettingQuantity == undefined || inventorySettingQuantity == "0")) {
                if (childTrackInventory != "true") {
                    $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorOutOfStockMessage"));
                    isSuccess = false;
                    showAddToCart = false;
                }
            }
            else if ((retail_price == "" || retail_price == undefined) && (groupProductSKUs === undefined || groupProductSKUs.trim() === '')) {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorPriceNotSet"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if (call_for_pricing == "true") {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("CallForPricing"));
                isSuccess = false;
                showAddToCart = false;
            }

            else if (((quantity % 1) != 0 || (quantity) <= 0) && isSuccess) {        
                $('p[for="' + 'txtQuickOrderPadQuantity_' + lastIndex + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidQuantity"));
                isSuccess = false;
                showAddToCart = false;
            }
            else if ((isNaN(quantity) || quantity.toString() == "") && isSuccess) {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorWholeNumber"));
                isSuccess = false;
            }
            else if (parseFloat($(control).attr("data_qo_max_quantity")) > 0 && parseFloat($(control).attr("data_qo_max_quantity")) < quantity + parseFloat($(control).attr("data_qo_cart_quantity"))) {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMaxCartQuantity"));
                isSuccess = false;
            }
            else if (parseFloat($(control).attr("data_qo_min_quantity")) > 0 && parseFloat($(control).attr("data_qo_min_quantity")) > quantity) {
                $('p[for="' + control.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectedQuantityLessThanMinSpecifiedQuantity"));
                isSuccess = false;
            }
            else if ((track_inventory == "DisablePurchasing") && (quantity + parseInt($(control).attr("data_qo_cart_quantity"))) > parseInt($(control).attr("data_qo_quantity_on_hand"))) {
                $('p[for="' + control.id + '"]').html("Only " + (parseInt(quantity_on_hand) - parseInt($(control).attr("data_qo_cart_quantity"))) + " quantity are available for Add to cart/Shipping");
                isSuccess = false;
                showAddToCart = false;
            }
            else if (inventoryCode != "" && (inventoryCode.toLowerCase().trim() == "donttrackinventory" || inventoryCode.toLowerCase().trim() == "allowbackordering")) {
                isSuccess = true;
                showAddToCart = true;
            }
            else if (parseFloat($(control).attr("data_qo_max_quantity")) == 0 && parseFloat($(control).attr("data_qo_min_quantity")) == 0) {
                isSuccess = true;
                showAddToCart = true;
            }           
        }
        if (isSuccess == false) {
            $('#btnQuickOrderClearAll').prop('disabled', false);
        }
        return { isSuccess, showAddToCart };
    }

    GenerateNewRow(): any {
        var id = parseInt($('#indexId').val()) + 1;
        $('#defaultValue-add-new-row').on('click', function () {
            $("#quickorderdiv").append('<div class="form-group" id="form-group-' + id + '"><div class="col-xs-8 col-sm-9 nopadding"><input class="typeahead tt-input" data-autocomplete-id-field="Id" data-autocomplete-url="/Product/GetProductListBySKU" data-onselect-function="QuickOrderPad.prototype.OnItemSelect" data_autocomplete_url="/Product/GetProductListBySKU" data_is_first="true" data_onselect_function="QuickOrderPad.prototype.OnItemSelect" data_qo_call_for_pricing="" data_qo_cart_quantity="" data_qo_in_stock_message="" data_qo_max_quantity="" data_qo_min_quantity="" data_qo_out_stock_message="" data_qo_product_id="" data_qo_product_name="" data_qo_quantity_on_hand="" data_qo_sku="" data_qo_track_inventory="" id="Name" name="Name" placeholder="Enter SKU" type="text" value="" autocomplete="off" spellcheck="false" dir="auto" style="position: relative; vertical-align: top; background-color: transparent;"><p id="inventoryMessage_' + id + '" for="txtQuickOrderPadSku_' + id + '" class="col-xs-12 nopadding error-msg"></p></div><div class="col-xs-3 col-sm-2"><input class="quantity quick-order-pad-quantity text-right" parentcontrol= "txtQuickOrderPadSku_' + id + '"  id="txtQuickOrderPadQuantity_' + id + '" maxlength="4" name="txtQuickOrderPadQuantity_' + id + '" placeholder="Qty" type="number" value="0" /></div><div class="col-xs-1 nopadding"><div id="removeRow_' + id + '" class="remove_row remove-item" title="Clear"><i class="zf-close"></i></div></div></div>');
            id++;

            QuickOrderPad.prototype.QuickOrderPadAutoComplete();
            QuickOrderPad.prototype.SetQuantity();
            QuickOrderPad.prototype.RemoveRow();
        });
    }

    ClearAll(): any {
        $("#btnQuickOrderClearAll").on("click", function () {
            var index = 0;
            var formIndex = 0;
            $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
                index++;
                $(this).val("");
                $(".quick-order-pad-quantity").val('0');
                $('p[for="' + this.id + '"]').html("");
                $('p[for="' + 'txtQuickOrderPadQuantity_' + this.id.split('_')[1] + '"]').html("");
                $('#btnQuickOrderPad').attr('disabled', 'disabled');

                var skuId = this.id.split('_')[1];               
                var element = $('#txtQuickOrderPadSku_' + skuId);
                element.attr("data_qo_isactive", "");
                element.attr("data_qo_isobsolete", "");
                element.attr("data_qo_product_type", "");
                element.attr("data_qo_retail_price", "");
                element.attr("data_qo_call_for_pricing", "");
                element.attr("data_qo_cart_quantity", "");
                element.attr("data_qo_max_quantity", "");
                element.attr("data_qo_min_quantity", "");
                element.attr("data_qo_out_stock_message", "");
                element.attr("data_qo_product_id", "");
                element.attr("data_qo_product_name", "");
                element.attr("data_qo_quantity_on_hand", "");
                element.attr("data_qo_sku", "");
                element.removeAttr("data_qo_inventorycode");
                element.attr("txtQuickOrderPadSku_" + skuId, "txtQuickOrderPadSku_" + index);
                $('#btnQuickOrderClearAll').prop('disabled', 'disabled');
            });

            $('.form-group').each(function () {
                formIndex++;
                //As we only have to remove the extra added rows
                if (formIndex > 6) {
                    $(this).remove();
                }

            })
            EnabledAddRowButton();
        });
    }
    RemoveRow(): any {
        $(document).off("click", ".remove_row")
        $(document).on("click", ".remove_row", function () {
            var removeId = $(this).attr("id");
            var selectedRow = removeId.split('_')[1];
            QuickOrderPad.prototype.ClearSelectedData(selectedRow);
            $(this).hide();
            if (selectedRow != QuickOrderPad.prototype.GetFirstOrderRowId() + "") {
                $('#form-group-' + selectedRow).remove();
            }
            var _existVal = 0;
            $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
                if ($(this).val() !== "") {
                    _existVal = 1;
                }
            });
            QuickOrderPad.prototype.ValidateQuickOrderItems();
            if (_existVal === 0) {
                $('#btnQuickOrderPad').attr('disabled', 'disabled');
            }
            EnabledAddRowButton();
        });
    }

    ShowRemoveItemBox(): any {
        $('#quick-order-pad-content [data-autocomplete-url]').on("focusout", function () {
            var removeId = $(this).attr("id");
            var selectedRow = removeId.split('_')[1];
            if ($(this).val() == $(this).attr("data_qo_sku") && $(this).val() != '' && $(this).val() != undefined) {
                if ($(this).val() <= 1) {
                    $('#txtQuickOrderPadQuantity_' + selectedRow + '').val('1');
                }
            }
            else {
                $('#txtQuickOrderPadQuantity_' + selectedRow + '').val('0');
            }
            if ($(this).val() != "") {
                $("#removeRow_" + selectedRow).show();
            }
            else {
                QuickOrderPad.prototype.ClearSelectedData(selectedRow);
                var _existVal = 0;
                $('#quick-order-pad-content [data-autocomplete-url]').each(function () {
                    if ($(this).val() !== "") {
                        _existVal = 1;
                    }
                });
                if (_existVal === 0) {
                    $('#btnQuickOrderPad').attr('disabled', 'disabled');
                }
            }
            $('p[for="' + 'txtQuickOrderPadQuantity_' + selectedRow + '"]').html('');
        });
    }

    ShowRemoveItemBoxForNewRow(Quantitypadidnewrow): any {
        var removeId = Quantitypadidnewrow;
        var selectedRow = removeId.split('_')[1];
        if ($(removeId).val() == $(removeId).attr("data_qo_sku") && $(removeId).val() != '' && $(removeId).val() != undefined) {
                if ($(this).val() <= 1) {
                    $('#txtQuickOrderPadQuantity_' + selectedRow + '').val('1');
                }
            }
            else {
                $('#txtQuickOrderPadQuantity_' + selectedRow + '').val('0');
        }
        if ($(removeId).val() != "" && $(removeId).val() != undefined) {
                $("#removeRow_" + selectedRow).show();
            }
            else {
                QuickOrderPad.prototype.ClearSelectedData(selectedRow);
                var _existVal = 0;
            $(Quantitypadidnewrow).each(function () {
                    if ($(removeId).val() !== "") {
                        _existVal = 1;
                    }
                });
                if (_existVal === 0) {
                    $('#btnQuickOrderPad').attr('disabled', 'disabled');
                }
            }
            $('p[for="' + 'txtQuickOrderPadQuantity_' + selectedRow + '"]').html('');
    }

    

    ClearSelectedData(selectedRow): any {
        $('#txtQuickOrderPadSku_' + selectedRow).val("");
        $('#txtQuickOrderPadQuantity_' + selectedRow).val('0');
        $('#inventoryMessage_' + selectedRow).html("");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_sku", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_skuId", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_product_id", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_product_name", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_cart_quantity", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_quantity_on_hand", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_in_stock_message", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_out_stock_message", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_min_quantity", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_max_quantity", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_qo_call_for_pricing", "");
        $('#txtQuickOrderPadSku_' + selectedRow).attr("data_is_first", "true");
        $('#btnQuickOrderClearAll').prop('disabled', 'disabled');

        var textBoxIdsArray = ($("*[id*='txtQuickOrderPadQuantity_']")).map(function () {
            return this.id;
        }).get();

        var textBoxSplittedIdsArray = [];
        for (let textBoxIdIndex = 0; textBoxIdIndex < textBoxIdsArray.length; textBoxIdIndex++)
        {
            let textBoxSplittedId = (textBoxIdsArray[textBoxIdIndex].split("_")[1])
            textBoxSplittedIdsArray.push(textBoxSplittedId);
        }

        for (let textBoxSplittedIdIndex = 0; textBoxSplittedIdIndex < textBoxSplittedIdsArray.length; textBoxSplittedIdIndex++)
        {
            if ($("#txtQuickOrderPadQuantity_" + textBoxSplittedIdsArray[textBoxSplittedIdIndex]).length != 0)
            {
                if ($("#txtQuickOrderPadQuantity_" + textBoxSplittedIdsArray[textBoxSplittedIdIndex]).val() != '0')
                {
                    $('#btnQuickOrderClearAll').prop('disabled', false);
                }
            }          
        }
    }

    MergeDuplicateCartItems(cartItems): any {
        var listOfSKUs = [];
        $.each(cartItems, function (key, cartItem) {
            listOfSKUs.push(cartItem.Sku);
        });
        var duplicateSKUsCountObj = [];
        $.each(listOfSKUs, function (key, value) {
            var numOccr = $.grep(listOfSKUs, function (elem) {
                return elem === value;
            }).length;
            if (numOccr > 1) {
                duplicateSKUsCountObj.push(value);
            }
        });
        var mergeCartItems = [];
        var duplicateCartItems = [];
        $.each(cartItems, function (key, cartItem) {
            if (duplicateSKUsCountObj.indexOf(cartItem.Sku) > -1) {
                var isAvailable = false;
                $.each(duplicateCartItems, function (key, duplicateCartItem) {
                    if (duplicateCartItem.Sku == cartItem.Sku) {
                        duplicateCartItem.Quantity = duplicateCartItem.Quantity + cartItem.Quantity;
                        isAvailable = true;
                    }
                });
                if (!isAvailable) {
                    duplicateCartItems.push(cartItem);
                }
            }
            else {
                mergeCartItems.push(cartItem);
            }
        });
        $.each(duplicateCartItems, function (key, duplicateCartItem) {
            if ($('#quick-order-pad-content [data_qo_sku="' + duplicateCartItem.Sku + '"]').length > 1) {
                var controlId = $('#quick-order-pad-content [data_qo_sku="' + duplicateCartItem.Sku + '"]')[$('#quick-order-pad-content [data_qo_sku="' + duplicateCartItem.Sku + '"]').length - 1].id;

                if (duplicateCartItem.MinQuantity > duplicateCartItem.Quantity) {
                    $('p[for="' + controlId + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMaxCartQuantity"));
                    mergeCartItems = [];
                    return false;
                }
                else if (duplicateCartItem.MaxQuantity < duplicateCartItem.Quantity) {
                    $('p[for="' + controlId + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMinCartQuantity"));
                    mergeCartItems = [];
                    return false;
                }
                else {
                    mergeCartItems.push(duplicateCartItem);
                }
            }
        });
        return mergeCartItems;
    }

    AddMultipleOrdersToCart(): any {
        var cartItems = [];
        var isSuccess = true;

        $('#btnQuickOrderPad').on("click", function () {
            cartItems.length = 0;
            var index = 0;
            isSuccess = true;                       
            $('#quick-order-pad-content [data-autocomplete-url]').each(function () {                
                var track_inventory = $(this).attr("data_qo_track_inventory");
                var quantity_on_hand = $(this).attr("data_qo_quantity_on_hand");
                var call_for_pricing = $(this).attr("data_qo_call_for_pricing");
                var product_type = $(this).attr("data_qo_product_type");
                var retail_price = $(this).attr("data_qo_retail_price");
                var inventorySettingQuantity = $(this).attr("data_qo_quantity_on_hand");
                var autoAddOnProductSkus = $(this).attr("data_qo_autoaddonskus");
                var inventoryCode = $(this).attr("data_qo_inventorycode");
                var isObsolete = $(this).attr("data_qo_isobsolete");
                var maxQuantity = parseFloat($(this).attr("data_qo_max_quantity")) > parseFloat($(this).attr("data_qo_cart_quantity")) ? parseFloat($(this).attr("data_qo_max_quantity")) - parseFloat($(this).attr("data_qo_cart_quantity")) : parseFloat($(this).attr("data_qo_cart_quantity")) - parseFloat($(this).attr("data_qo_max_quantity"));
                var minQuantity = parseFloat($(this).attr("data_qo_min_quantity"));
                var childTrackInventory = $(this).attr("data_qo_child_track_inventory");
                index++;
                var lastIndex = this.id.split("_")[1];
                $('p[for="' + this.id + '"]').html("");
                if (this.id.length > 0 && !isNaN(parseInt($(this).attr("data_qo_product_id"))) && $(this).attr("data_qo_product_id") != "" && $(this).attr("data_qo_product_id") != "0") {
                    let groupProductQty: string;
                    let groupProductSKUs: string = $(this).attr("data_qo_group_product_sku");
                    let configurableProductSKUs: string = $(this).attr("data_qo_configurable_product_sku");

                    if (groupProductSKUs != undefined) {
                        groupProductQty = new Array(groupProductSKUs.split(",").length + 1).join($('input[parentcontrol=' + this.id + ']').val() + "_").replace(/\_$/, '');
                    }

                    var quantity = parseFloat($("input[parentcontrol=" + this.id + "]").val());

                    if ($(this).attr("data_is_first") == "false") {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
                        isSuccess = false;
                        return false;
                    }

                    if ($(this).val() != $(this).attr("data_qo_sku")) {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
                        isSuccess = false;
                        return false;
                    }

                    if ((quantity % 1) != 0 || (quantity) <= 0) {
                        $('p[for="' + 'txtQuickOrderPadQuantity_' + lastIndex + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidQuantity"));
                        isSuccess = false;
                        return false;
                    }

                    if (isSuccess){
                        $('p[for="' + 'txtQuickOrderPadQuantity_' + lastIndex + '"]').html('');
                    }

                    if (isNaN(quantity) || quantity.toString() == "") {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorWholeNumber"));
                        isSuccess = false;
                        return false;
                    }

                    if (call_for_pricing == "true") {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("CallForPricing"));
                        isSuccess = false;
                        return false;
                    }

                    if ((track_inventory == "DisablePurchasing")) {
                        if (parseInt(quantity_on_hand) <= 0) {
                            $('p[for="' + this.id + '"]').html($(this).attr("data_qo_out_stock_message"));
                            isSuccess = false;
                            return false;
                        }
                    }

                    if (parseFloat($(this).attr("data_qo_max_quantity")) > 0 && parseFloat($(this).attr("data_qo_max_quantity")) < quantity + parseFloat($(this).attr("data_qo_cart_quantity"))) {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMaxCartQuantity"));
                        isSuccess = false;
                        return false;
                    }

                    if (parseFloat($(this).attr("data_qo_min_quantity")) > 0 && parseFloat($(this).attr("data_qo_min_quantity")) > quantity) {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorMinCartQuantity"));
                        isSuccess = false;
                        return false;
                    }

                    if ((track_inventory == "DisablePurchasing")) {
                        if (parseInt(quantity_on_hand) == parseInt($(this).attr("data_qo_cart_quantity"))) {
                            $('p[for="' + this.id + '"]').html($(this).attr("data_qo_out_stock_message"));
                            isSuccess = false;
                            return false;
                        }
                    }

                    if ((track_inventory == "DisablePurchasing")) {
                        if ((quantity + parseInt($(this).attr("data_qo_cart_quantity"))) > parseInt($(this).attr("data_qo_quantity_on_hand"))) {
                            $('p[for="' + this.id + '"]').html("Only " + (parseInt(quantity_on_hand) - parseInt($(this).attr("data_qo_cart_quantity"))) + " quantity are available for Add to cart/Shipping");
                            isSuccess = false;
                            return false;
                        }
                    }

                    if ((retail_price == "" || retail_price == undefined) && (groupProductSKUs === undefined || groupProductSKUs.trim() === '')) {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorPriceNotSet"));
                        isSuccess = false;
                        return false;
                    }
                    if (isObsolete == "true") {
                        $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ObsoleteProductErrorMessage"));
                        isSuccess = false;
                        return false;
                    }
                    if (inventoryCode != "" && (inventoryCode.toLowerCase().trim() == "donttrackinventory" || inventoryCode.toLowerCase().trim() == "allowbackordering")) {
                        isSuccess = true;
                    }
                    if ((inventoryCode.toLowerCase().trim() != "donttrackinventory" && inventoryCode.toLowerCase().trim() != "allowbackordering") && (inventorySettingQuantity == "" || inventorySettingQuantity == undefined || inventorySettingQuantity == "0")) {
                        if (childTrackInventory != "true") {
                            $('p[for="' + this.id + '"]').html($(this).attr("data_qo_out_stock_message"));
                            isSuccess = false;
                            return false;
                        }
                    }
                    if (parseFloat($(this).attr("data_qo_max_quantity")) == 0 && parseFloat($(this).attr("data_qo_min_quantity")) == 0) {
                        isSuccess = true;
                    }
                    if (isSuccess) {
                        var quickOrderModel;
                        if (window.location.pathname.toLowerCase().indexOf("/user/createtemplate") >= 0 || window.location.pathname.toLowerCase().indexOf("/user/edittemplate") >= 0 || window.location.pathname.toLowerCase().indexOf("/user/quickorderpadtemplate") >= 0) {
                            quickOrderModel =
                            {
                                "ProductId": $(this).attr("data_qo_product_id"),
                                "ProductName": $(this).attr("data_qo_product_name"),
                                "Sku": $(this).attr("data_qo_sku"),
                                "Quantity": quantity,
                                "ProductType": $(this).attr("data_qo_product_type"),
                                "GroupProductSKUs": groupProductSKUs,
                                "GroupProductsQuantity": groupProductQty,
                                "ConfigurableProductSKUs": configurableProductSKUs,
                                "AutoAddonSKUs": autoAddOnProductSkus,
                                "TemplateName": $("#TemplateName").val(),
                                "MaxQuantity": maxQuantity,
                                "MinQuantity": minQuantity
                            };
                            cartItems.push(quickOrderModel);

                        } else {
                            quickOrderModel =
                            {
                                "Sku": $(this).attr("data_qo_sku"),
                                "Quantity": quantity,
                                "ProductType": $(this).attr("data_qo_product_type"),
                                "GroupProductSKUs": groupProductSKUs,
                                "GroupProductsQuantity": groupProductQty,
                                "ConfigurableProductSKUs": configurableProductSKUs,
                                "AutoAddonSKUs": autoAddOnProductSkus,
                            };
                            cartItems.push(quickOrderModel);
                        }
                    }
                }
                else if ($(this).val() != "") {
                    $('p[for="' + this.id + '"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));

                    if (this.id.length > 0 && this.id.split('_').length > 1) {
                        index = this.id.split('_')[1];
                    }
                    $("#removeRow_" + index).show();
                    isSuccess = false;
                    return false;
                }
            });
            if (isSuccess) {
                cartItems = QuickOrderPad.prototype.MergeDuplicateCartItems(cartItems);
                isSuccess = cartItems.length > 0;
            }
            if (window.location.pathname.toLowerCase().indexOf("/user/createtemplate") >= 0 || window.location.pathname.toLowerCase().indexOf("/user/edittemplate") >= 0 || window.location.pathname.toLowerCase().indexOf("/user/quickorderpadtemplate") >= 0) {
                if (isSuccess) {
                    $('#btnQuickOrderPad').attr('disabled', 'disabled');
                    $.ajax({
                        url: "/user/addmultipleproductstocarttemplate/",
                        type: "post",
                        data: JSON.stringify(cartItems),
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            if (data.isSuccess) {
                                if (data.omsTemplateId > 0)
                                    window.location.href = "/user/edittemplate?omstemplateid=" + data.omsTemplateId + "";
                                else
                                    window.location.href = "/user/createtemplate/";
                            }
                            else {
                                $('#lblNotificationMessage').addClass('error-msg');
                                $('#lblNotificationMessage').html(data.message);
                            }
                        },
                        error: function (msg) {
                        }
                    });
                }
            }
            else {
                if (isSuccess) {
                    $('#btnQuickOrderPad').attr('disabled', 'disabled');
                    ZnodeBase.prototype.ShowLoader();
                    if ($("#isEnhancedEcommerceEnabled").val() == "True") {
                        GoogleAnalytics.prototype.SendAddToCartsFromMultipleQuickOrder(cartItems);
                        
                    }

                    $.ajax({
                        url: "/product/addmultipleproductstocart/",
                        type: "post",
                        data: JSON.stringify(cartItems),
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            if (data.isSuccess) {
                                ZnodeBase.prototype.HideLoader();
                                window.location.href = "/cart/index";
                            }
                            else {
                                if ($("#lblNotificationMessage").length > 0) {
                                    $('#lblNotificationMessage').addClass('error-msg');
                                    $('#lblNotificationMessage').html(data.message);
                                }
                                else {
                                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, 'error', isFadeOut, fadeOutTime);
                                }
                            }
                        },
                        error: function (msg) {
                        }
                    });
                }
            }
        });
    }

    SetQuantity(): any {
        $(document).off("focusout", ".quick-order-pad-quantity")
        $(document).on("focusout", ".quick-order-pad-quantity", function (ev) {
            var removeId = $(this).attr("id");
            var selectedRow = removeId.split('_')[1];
            if ($("#" + removeId + "").val() != "") {
                if ($("#" + removeId + "").val() >= 0) {
                    $("#" + removeId + "").val(parseInt($("#" + removeId + "").val()));
                }
                else {
                    $("#" + removeId + "").val($("#" + removeId + "").val().replace(/[^\d].+/, ""));
                    if ((ev.which < 48 || ev.which > 57)) {
                        $("#" + removeId + "").val(1);
                    }
                }
            }
        });
    }
}

$(window).on("load", function () {
    var quickOrderPad = new QuickOrderPad();
    quickOrderPad.Init();
});


$('#btnAddNewSKU').on("click", function () {
    if ($('.form-group').length > 50) {
        $('#btnAddNewSKU').css({ "cursor": "not-allowed" });      
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("QuickOrderLimit"), "error", isFadeOut, fadeOutTime);
        return false;
    }
});
function EnabledAddRowButton() {
    if ($('.form-group').length <= 50) {
        $('#btnAddNewSKU').css({ "cursor": "" });
    }
}
