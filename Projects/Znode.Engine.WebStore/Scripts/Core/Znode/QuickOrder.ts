class QuickOrder extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        $('#btnQuickOrder').attr('disabled', 'disabled');
        QuickOrder.prototype.ShowHideQuickOrderPopUp();
        QuickOrder.prototype.CloseQuickOrderpopup();
        QuickOrder.prototype.Validation();
        QuickOrder.prototype.RemoveValidationMessage();
        QuickOrder.prototype.SetProperties();
        QuickOrder.prototype.SetQuantity();
    }
    ShowHideQuickOrderPopUp(): any {
        $(".quickordercontainer").on('mouseover mouseenter touch', function () {

            $(this).find(".divQuickOrder").show();
            if ($("#TemplateName:visible").length > 0) {
                $("#quickOrderPadTemplateLink").attr("href", "/User/QuickOrderPadTemplate?templateName=" + $("#TemplateName").val());
            }
        });
        $(".quickordercontainer").on('mouseleave touch', function () {
            if ((!$(this).find("#hdnttxtSKU").is(":focus")) && (!$(this).find(".txtQuickOrderQuantity").is(":focus")) && ($(this).find("#hdnttxtSKU").val() == "")) {
                $(this).find(".divQuickOrder").hide();
            }
        });
    }

    CloseQuickOrderpopup(): any {
        $('.close-quick-order-popup').on('click', function () {
            var _content = $(this).closest(".quick-order-container");
            $(_content).find(".divQuickOrder").hide();
            $(_content).find('.quickOrderAddToCart').attr('disabled', 'disabled');
            $(_content).find('.txtQuickOrderSku').val("");
            $(_content).find('#hdnttxtSKU').val("");
            $(_content).find('.txtQuickOrderQuantity').val("1");
            $(_content).find('#inventorymsg').html("");
            $(_content).find(".divTemplateQuickOrder").hide();
            $(_content).find('.quickOrderAddToTemplate').attr('disabled', 'disabled');
            $(_content).find('.txtTemplateQuickOrderQuantity').val("");
            $(_content).find('.txtTemplateQuickOrderQuantity').val("1");
            $(_content).find('#templateInventorymsg').html("");
        });

    }

    OnItemSelect(item): any {
        var _focus = document.activeElement;
        var _content = $(_focus).closest(".quick-order-container");
        $(_content).find('#hdnttxtSKU').val(item.displaytext);
        $(_content).find('#hdnQuickOrderProductId').val(item.id);
        $(_content).find('#hdnQuickOrderMaxQty').val(item.properties.MaxQuantity);
        $(_content).find('.quickOrderAddToCart').prop('disabled', false);
        if ($("#hdnttxtSKU").val() != '' && $("#hdnttxtSKU").val() != undefined) {
            $("#txtQuickOrderQuantity").val("1");
        }
        else {
            $("#txtQuickOrderQuantity").val("0");
        }

        QuickOrder.prototype.SetQuickOrderMultipleHref();
    }

    SetQuickOrderMultipleHref() {
        if ($("#linkMultiplePartSku").length > 0) {            
            var link = $("#linkMultiplePartSku").attr("href");
            link = QuickOrder.prototype.UpdateQueryStringParameter(link, "ProductId", $($('#hdnttxtSKU').closest('.quick-order-container')).find('#hdnQuickOrderProductId').val());
            $("#linkMultiplePartSku").attr("href", link);
        }
    }

    UpdateQueryStringParameter(uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        else {
            return uri + separator + key + "=" + value;
        }
    }

    OnQuantityChange(item): any {
        //Adding CartCoundByProductId method to get the count of product in cart.
        var cartCount = 0;
        $('.quickOrderAddToCart').attr('disabled', true);
        Endpoint.prototype.GetCartCountByProductId(parseInt($('#hdnQuickOrderProductId').val()), function (response) {
            cartCount = parseInt(response) + parseInt(item.value);
            if (parseInt($('#hdnQuickOrderMaxQty').val()) < cartCount || $('#hdnQuickOrderMaxQty').val() == "") {
                $('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + 1 + ZnodeBase.prototype.getResourceByKeyName("To") + parseInt($('#hdnQuickOrderMaxQty').val()));
                $('.quickOrderAddToCart').attr('disabled', true);
                return false;
            } else $('.quickOrderAddToCart').attr('disabled', false);
        });
    }

    CheckUpdateSKUSinglePage(control, e) {
        if (e.keyCode === 13 || e.keyCode === 9 || e.type === "blur") {
            var skuProductId = $(control).val();
            if (skuProductId != "" && skuProductId != undefined) {
                Endpoint.prototype.GetProductDetailsBySKU(skuProductId, function (res) {
                    QuickOrder.prototype.SetValidationData(control, res);
                    if (res.DisplayText != null && res.Id > 0) {
                        $('p[for="' + control.id + '"]').html("");
                        $('p[for="linkMultiplePartSku"]').html("");
                        $('.quickOrderAddToCart').attr('disabled', false);
                        var act = control;
                        $('#hdnQuickOrderProductId').val(res.Id);
                        $('#hdnQuickOrderMaxQty').val(res.Properties.MaxQuantity);
                    }
                    else {
                        $('p[for="linkMultiplePartSku"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
                        $('#txtQuickOrderQuantity').val(0)
                        $('.quickOrderAddToCart').attr('disabled', true);
                    }

                });
            }
        }
    }

    Validation(): any {
        $(".quickOrderAddToCart").on("click", function () {
            var isValid = false;
            var _content = $(this).closest(".quick-order-container");
            var productId: number = parseInt(_content.find("#hdnQuickOrderProductId").val());
            Endpoint.prototype.GetAutoCompleteItemProperties(productId, function (res) {
                QuickOrder.prototype.SetValidationData(_content, res);
                isValid = QuickOrder.prototype.ValidateAddToCart(_content);
                if (isValid == true && $("#isEnhancedEcommerceEnabled").val() == "True") {
                    GoogleAnalytics.prototype.SendAddToCartsFromQuickOrder();
                }
                return isValid;
            })
            return isValid;
        });
    }

    SetValidationData(_content, response): void {
        $(_content).find('.txtQuickOrderSku').val(response.DisplayText);
        $(_content).find('#hdnQuickOrderSku').val(response.DisplayText);
        $(_content).find('#hdnQuickOrderProductName').val(response.Properties.ProductName);
        $(_content).find('#hdnQuickOrderQuantityOnHand').val(response.Properties.Quantity);
        $(_content).find('#hdnQuickOrderCartQuantity').val(response.Properties.CartQuantity);
        $(_content).find('#hdnQuickOrderProductType').val(response.Properties.ProductType);
        $(_content).find('#hdnRetailPrice').val(response.Properties.RetailPrice);
        $(_content).find('#hdnImagePath').val(response.Properties.ImagePath);
        $(_content).find('#hdnIsPersonisable').val(response.Properties.IsPersonalisable);
        $(_content).find('#hdnAutoAddonSKUs').val(response.Properties.AutoAddonSKUs);
        $(_content).find('#hdnInventoryCode').val(response.Properties.InventoryCode);
        $(_content).find('#hdnIsObsolete').val(response.Properties.IsObsolete);
        $(_content).find('#hdnIsRequired').val(response.Properties.IsRequired);
        if (response.Properties.ConfigurableProductSKUs != undefined) {
            $(_content).find('#hdnConfigurableProductSKUs').val(response.Properties.ConfigurableProductSKUs);
        }
        else {
            $(_content).find('#hdnConfigurableProductSKUs').val("");
        }

        if (response.Properties.GroupProductSKUs != undefined) {
            $(_content).find('#hdnGroupProductSKUs').val(response.Properties.GroupProductSKUs);
        }
        else {
            $(_content).find('#hdnGroupProductSKUs').val("");
        }

        if (response.Properties.GroupProductsQuantity != undefined) {
            $(_content).find('#hdnGroupProductsQuantity').val(new Array(response.Properties.GroupProductSKUs.split(",").length + 1).join($(_content).find('.txtQuickOrderQuantity').val() + "_").replace(/\_$/, ''));
        }
        else {
            $(_content).find('#hdnGroupProductsQuantity').val("");
        }

        if (response.Properties.CallForPricing != undefined) {
            $(_content).find('#hdnQuickOrderCallForPricing').val(response.Properties.CallForPricing);
        }
        else {
            $(_content).find('#hdnQuickOrderCallForPricing').val("false")
        }
        if (response.Properties.TrackInventory != undefined) {
            $(_content).find('#hdnQuickOrderInventoryTracking').val(response.Properties.TrackInventory);
        }
        else {
            $(_content).find('#hdnQuickOrderInventoryTracking').val("");
        }
        if (response.Properties.OutOfStockMessage != undefined) {
            $(_content).find('#hdnQuickOrderOutOfStockMessage').val(ZnodeBase.prototype.getResourceByKeyName("ErrorOutOfStockMessage"));
        }
        if (response.Properties.MaxQuantity != undefined) {
            $(_content).find('#hdnQuickOrderMaxQty').val(response.Properties.MaxQuantity);
        }
        if (response.Properties.MinQuantity != undefined) {
            $(_content).find('#hdnQuickOrderMinQty').val(response.Properties.MinQuantity);
        }
        if (response.Properties.IsPersonalisable != undefined) {
            $(_content).find('#hdnIsPersonisable').val(response.Properties.IsPersonalisable);
        }
    }

    ValidateAddToCart(_content): boolean {
        var quantity = parseFloat($(_content).find('.txtQuickOrderQuantity').val());
        var maxQuantity = parseFloat($(_content).find('#hdnQuickOrderMaxQty').val());
        var trackInventory = $(_content).find('#hdnQuickOrderInventoryTracking').val();
        var productType = $(_content).find('#hdnQuickOrderProductType').val();
        var retailPrice = $(_content).find('#hdnRetailPrice').val();
        var inventorySettingQuantity = $(_content).find('#hdnQuickOrderQuantityOnHand').val();
        var isPersonisable = $(_content).find('#hdnIsPersonisable').val();
        var inventoryCode = $(_content).find('#hdnInventoryCode').val();
        var isObsolete = $(_content).find('#hdnIsObsolete').val();
        var isRequired = $(_content).find('#hdnIsRequired').val();
        $("#hdnTemplateNameQuickOrder").val($("#TemplateName").val());
        if (productType != "" && productType.toLowerCase().trim() != "groupedproduct" && (retailPrice == "")) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorPriceNotSet"));
            return false;
        }
        if (isObsolete == "true") {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ObsoleteProductErrorMessage"));
            $('#btnQuickOrder').attr('disabled', 'disabled');
            return false;
        }
        if (isPersonisable == "true" && isRequired == "true") {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorAddToCartFromPDPOrQuickView"));
            return false;
        }
        if ((inventoryCode) && ((inventoryCode.toLowerCase().trim() == ZnodeBase.prototype.getResourceByKeyName("DontTrackInventory")) || inventoryCode.toLowerCase().trim() == ZnodeBase.prototype.getResourceByKeyName("AllowBackOrdering").toLowerCase())) {
            return true;
        }
        if ($(_content).find('#hdnttxtSKU').val() != $(_content).find('#hdnQuickOrderSku').val()) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidSKU"));
            return false;
        }
        if (parseInt($(_content).find('.txtQuickOrderQuantity').val()) % 1 != 0 || parseInt($(_content).find('.txtQuickOrderQuantity').val()) <= 0) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorValidQuantity"));
            return false;
        }
        if (isNaN($(_content).find('.txtQuickOrderQuantity').val()) || $(_content).find('.txtQuickOrderQuantity').val() == "") {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorWholeNumber"));
            return false;
        }
        if ($(_content).find('#hdnQuickOrderCallForPricing').val() == "true") {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("CallForPricing"));
            return false;
        }
        if ((trackInventory == "DisablePurchasing")) {
            if (parseFloat($(_content).find("#hdnQuickOrderQuantityOnHand").val()) <= 0) {
                $(_content).find('#inventorymsg').html($(_content).find('#hdnQuickOrderOutOfStockMessage').val());
                return false;
            }
        }
        if (parseFloat($(_content).find('#hdnQuickOrderMaxQty').val()) < quantity + parseFloat($(_content).find("#hdnQuickOrderCartQuantity").val())) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectedQuantityExceedsMaxCartQuantity"));
            return false;
        }
        if (parseInt($(_content).find('#hdnQuickOrderMinQty').val()) > quantity) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectedQuantityLessThanMinSpecifiedQuantity"));
            return false;
        }
        if ((trackInventory == "DisablePurchasing")) {
            if (parseFloat($(_content).find('#hdnQuickOrderQuantityOnHand').val()) == parseFloat($(_content).find("#hdnQuickOrderCartQuantity").val())) {
                $(_content).find('#inventorymsg').html($(_content).find('#hdnQuickOrderOutOfStockMessage').val());
                return false;
            }
        }
        if ((trackInventory == "DisablePurchasing")) {
            if (quantity + parseFloat($(_content).find("#hdnQuickOrderCartQuantity").val()) > parseFloat($(_content).find('#hdnQuickOrderQuantityOnHand').val())) {
                $(_content).find('#inventorymsg').html("Only " + (parseFloat($(_content).find('#hdnQuickOrderQuantityOnHand').val()) - parseFloat($(_content).find("#hdnQuickOrderCartQuantity").val())) + " quantities are available for Add to cart/Shipping");
                return false;
            }
        }
        if (productType != "" && productType.toLowerCase().trim() != "groupedproduct" && (inventorySettingQuantity == "" || inventorySettingQuantity == undefined || inventorySettingQuantity == 0)) {
            $(_content).find('#inventorymsg').html($(_content).find('#hdnQuickOrderOutOfStockMessage').val());
            return false;
        }
        if (maxQuantity < quantity) {
            $(_content).find('#inventorymsg').html(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + 1 + ZnodeBase.prototype.getResourceByKeyName("To") + maxQuantity);
            return false;
        }
        return true;
    }

    RemoveValidationMessage(): any {
        $("#hdnttxtSKU").on("focusout", function () {
            var _content = $(this).closest(".quick-order-container");
            $(_content).find('#inventorymsg').text('');
            if ($(_content).find(".txtQuickOrderSku").val() == "") {
                $(_content).find('#inventorymsg').html("");
                $(_content).find('.quickOrderAddToCart').attr('disabled', 'disabled');
                
            }
            if ($("#hdnttxtSKU").val() == '' || $("#hdnttxtSKU").val() == undefined) {
                $(_content).find('#txtQuickOrderQuantity').val('0');
                $("#btnQuickOrder").attr('disabled', true);
                $("#linkMultiplePartSku").attr("href", QuickOrder.prototype.UpdateQueryStringParameter($("#linkMultiplePartSku").attr("href"), "ProductId", "0"));
            }
            else {
                $(_content).find('.txtQuickOrderQuantity').val("1");
                $("#btnQuickOrder").attr('disabled', false);
            }
        });
        $("#txtQuickOrderQuantity").on("focusout", function () { $('#inventorymsg').text(''); });
    }

    SetProperties(): any {
        var _focus = document.activeElement;
        var _content = $(_focus).closest(".quick-order-container");
        $(_content).find('.quickOrderAddToCart').attr('disabled', 'disabled');
        $(_content).find('#txtQuickOrderQuantity').attr('Value', 1);
    }

    SetQuantity(): any {
        $("#txtQuickOrderQuantity").on("focusout", function (ev) {
            if ($(this).val() != "") {
                if ($(this).val() > 0) {
                    $(this).val(parseInt($(this).val()));
                }
                else {
                    $(this).val($(this).val().replace(/[^\d].+/, ""));
                    if ((ev.which < 49 || ev.which > 57)) {
                        $(this).val(0);
                    }
                }
            }
        });
    }
    CloseTemplateQuickOrder() {
        $('.close-quick-order-popup').click();
    }
}

$(document).on("keypress", "#txtQuickOrderQuantity", function (e) {
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});
$('#txtQuickOrderQuantity').on("cut copy paste", function (e) {
    e.preventDefault();
});


$(window).on("load", function () {
    QuickOrder.prototype.ShowHideQuickOrderPopUp();
    QuickOrder.prototype.CloseQuickOrderpopup();
    QuickOrder.prototype.Validation();
    QuickOrder.prototype.RemoveValidationMessage();
    QuickOrder.prototype.SetProperties();
    QuickOrder.prototype.SetQuantity();
});

