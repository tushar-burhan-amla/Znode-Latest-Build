class ConfigurableProduct extends Product {
    constructor() {
        super();
    }

    SortColumn(data): any {
        var sortAsc = $(data).hasClass("desc");
        var variantList = $('td.' + $(data).data('variant'));
        var sortedList;
        if (sortAsc) {
            if (variantList.hasClass('swatch')) {
                sortedList = variantList.sort(function (a, b) { return $(a).find('input').val().toUpperCase().localeCompare($(b).find('input').val().toUpperCase()); });
            }
            else {
                sortedList = variantList.sort(function (a, b) { return $(a).text().toUpperCase().localeCompare($(b).text().toUpperCase()); });
            }
            $(data).removeClass("desc");
            $(data).addClass("asc");
            $(data).find(".sort-icons .arrow-drop-down").hide();
            $(data).find(".sort-icons .arrow-drop-up").show();
        }
        else {
            if (variantList.hasClass('swatch')) {
                sortedList = variantList.sort(function (a, b) { return $(b).find('input').val().toUpperCase().localeCompare($(a).find('input').val().toUpperCase()); });
            }
            else {
                sortedList = variantList.sort(function (a, b) { return $(b).text().toUpperCase().localeCompare($(a).text().toUpperCase()); });
            }
            $(data).addClass("desc");
            $(data).removeClass("asc");
            $(data).find(".sort-icons .arrow-drop-down").show();
            $(data).find(".sort-icons .arrow-drop-up").hide();

        }
        $('tbody').html('')
        $.each(sortedList, function () {
            $('tbody').append($(this).parent());
        });
    }

    ValidateConfigurableProduct(control): any {
        var personalisedForm = $("#frmPersonalised");
        if (personalisedForm.length > 0 && !personalisedForm.valid()) {
            $("html, body").animate({ scrollTop: $("#BhPersonalizable").offset().top - 250 });
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        var childSKUs: string = "";
        var childQuantities: string = "";
        var personalisedCodes = [];
        var personalisedValues = [];
        var addOnValues = [];

        if (!ConfigurableProduct.prototype.CheckConfigProductAddonQuantity()) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }

        $("input[type=number].configquantity").each(function () {
            var quantity = $(this).val();
            if (quantity != null && quantity != "") {
                childSKUs = childSKUs + $(this).attr("data-sku") + ",";
                childQuantities + $(this).val() + "_";
                childQuantities = childQuantities + $(this).val() + "_";
            }
        });
        
        childQuantities = childQuantities.substr(0, childQuantities.length - 1);
        childSKUs = childSKUs.substr(0, childSKUs.length - 1);

        var parentProductId: number = parseInt($("#dynamic-parentproductid").val());
        if (!ConfigurableProduct.prototype.CheckConfigurableChildProductQuantity(parentProductId, childSKUs, childQuantities)) {
            ZnodeBase.prototype.HideLoader();
            $("#button-addtocart_" + parentProductId).attr("disabled", true)
            return false;
        }

        $("input[IsPersonalizable = True]").each(function () {
            var $label = $("label[for='" + this.id + "']");
            personalisedValues.push($(this).val());
            personalisedCodes.push($label.text());
        });

        if (!Product.prototype.ValidateAddons()) {
            $("html, body").animate({ scrollTop: $(".chk-product-addons").offset().top - 250 });
            return false;
        }
        addOnValues = Product.prototype.GetSelectedAddons();

        $(control).closest('form').children("#dynamic-configurableproductskus").val(childSKUs);
        $(control).closest('form').children("#dynamic-configurableproductquantity").val(childQuantities);
        $(control).closest('form').children("#dynamic-addonproductskus").val(addOnValues);
        $(control).closest('form').children("#dynamic-personalisedcodes").val(personalisedCodes);
        $(control).closest('form').children("#dynamic-personalisedvalues").val(personalisedValues);
        return true;
    }

    CheckConfigProductAddonQuantity(): boolean {
        var flag = true;
        if (Product.prototype.getAddOnIds("").length > 0) {
            var _productSKU = Product.prototype.BindGroupProductModelData();
            if (_productSKU != null && _productSKU.SKU != null && _productSKU.Quantity != null) {
                Product.prototype.UpdateProductVariations(false, _productSKU.SKU, _productSKU.ParentSKU, "1", _productSKU.ParentProductId, function (response) {
                    flag = Product.prototype.UpdateProductValues(response, _productSKU.Quantity);
                    if (flag == true) {
                        flag = Product.prototype.InventoryStatus(response);
                    }
                });
            }
            return flag;
        }
        else {
            return flag;
        }
    }

    GetProductImage(control): any {
        var row = $(control).closest('tr');
        var publishProductId: number = parseInt($(control).attr('data-productId'));
        var _productImageDetails: Znode.Core.ProductImageDetailModel = {
            SKU: $("#dynamic-sku").val(),
            PublishProductId: $("#dynamic-productid").val(),
            Name: $(row).attr('data-productName'),
            ImageName: $("#image_" + publishProductId).val(),
            AlternateImageName: $("#Alternateimage_" + publishProductId).val(),
        };
        Endpoint.prototype.GetImage(_productImageDetails, function (response) {
            $("#ProductImage").html("")
            $("#ProductImage").html(response.html)
            $("#ProductImage").html("")
            $("#ProductImage").html(response.html)
        });
    }

    OnConfigurableProductQuantityChange(control): any {
        var enteredQuantity: number = parseInt($(control).val()) || 0;
        var maxQty: number = parseInt($(control).attr("data-max-quantity"));
        var minQty: number = parseInt($(control).attr("data-min-quantity"));
        var dataProductId: number = parseInt($(control).attr("data-productId"));
        var publishProductId = $("#dynamic-parentproductid").val()
        var validation = $("#val_" + dataProductId)
        validation.text("");

        if ($.trim($(control).val()) == '') {
            return false;
        }
        if (enteredQuantity == 0) {
            $(validation).text(ZnodeBase.prototype.getResourceByKeyName("ErrorProductQuantity"));
            $("#button-addtocart_" + publishProductId).attr("disabled", true)
            return false;
        }
        else if (maxQty < enteredQuantity || minQty > enteredQuantity) {
            $(validation).text(ZnodeBase.prototype.getResourceByKeyName("SelectedQuantityBetween") + minQty + ZnodeBase.prototype.getResourceByKeyName("To") + maxQty);
            $("#button-addtocart_" + publishProductId).attr("disabled", true)
            return false;
        }
        else {
            $("#button-addtocart_" + publishProductId).removeAttr("disabled");
        }
    }

    CheckConfigurableChildProductQuantity(parentProductId: number, childSKUs: string, childQuantities: string): boolean {
        var isSuccess: boolean = false;
        
        Endpoint.prototype.CheckConfigurableChildProductQuantity(parentProductId, childSKUs, childQuantities, function (response) {
            if (!response.ShowAddToCart) {
                Product.prototype.CheckQuickViewAndShowErrorMessage(response.InventoryMessage);
            }
            isSuccess = response.ShowAddToCart;
        });
        return isSuccess;
    }

    GetConfigurableProductSKUQuantity(control): Znode.Core.AddOnDetails {
        var _productSKU: Znode.Core.AddOnDetails
        $("input[type=number].configquantity").each(function () {
            var configurableProductQuantity = $(this).val();
            if (configurableProductQuantity != null && configurableProductQuantity != "") {
                _productSKU = {
                    Quantity: configurableProductQuantity,
                    SKU: $(this).attr("data-sku"),
                    ParentSKU: $(control).attr("data-sku"),
                    ParentProductId: parseInt($("#dynamic-parentproductid").val()),
                }
            }
        });
        return _productSKU;
    }
}

$(document).on("keypress", ".configquantity", function (e) {
    var key = e.keyCode || e.which;
    if ((47 < key) && (key < 58) || key === 8) {
        return true;
    }
    return false;
});