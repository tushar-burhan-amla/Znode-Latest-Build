declare var znodeDataLayer: any;

class GoogleAnalytics extends ZnodeBase {
    constructor() {
        super();
    }

    //Send the product list ecommerce data to Google Analytics through data layer
    SendProductListDataImpressions(data: any, currencyCode: string) {
        var ecommerce = {};
        var impressions = [];
        $.each(data, function (v, e) {
            var listName: string;
            if (e.SearchKeyword != null && e.SearchKeyword != "") {
                listName = "Search results for " + e.SearchKeyword;
            }
            else {
                listName = e.CategoryName;
            }
            var product: Znode.Core.EcommerceProductListDataModel = {
                id : e.SKU,
                name : e.Name,
                category: e.CategoryName,
                brand: e.BrandName,
                list: listName,
                price: e.ProductPrice
            };
            impressions.push(product);
        });
        ecommerce["currencyCode"] = currencyCode;
        ecommerce["impressions"] = impressions;
        znodeDataLayer.push({ "ecommerce": ecommerce });
    };

    //Send the product detail views ecommerce data to Google Analytics through data layer
    SendProductDetailViews(data: any, currencyCode: string) {
        var product: Znode.Core.EcommerceProductDataModel = {
            id: data.SKU,
            name: data.Name,
            category: data.CategoryName,
            brand: data.BrandName,
            price: data.ProductPrice
        };
        znodeDataLayer.push({
            "event": "view_item",
            "ecommerce": {
                "currencyCode": currencyCode,
                "detail": {
                    "products": [product]
                }
            }
        });
    }

    SendProductClicks(data: any, currencyCode: string, productUrl: string) {
        var product: Znode.Core.EcommerceProductDataModel = {
            id: data.SKU,
            name: data.Name,
            category: data.CategoryName,
            brand: data.BrandName,
            price: data.ProductPrice
        };
        znodeDataLayer.push({
            'event': 'select_item',
            'ecommerce': {
                'currencyCode': currencyCode,
                'click': {
                    'actionField': { 'list': data.CategoryName },
                    'products': [product]
                }
            },
            'eventCallback': function () { document.location.href = productUrl }
        });
    }

    SendProductAddToCarts(control, event, currencyCode: string): any {
        var brand: string = "";
        var quantity: string = "";
        var price: string = "";
        var name: string = "";
        if ($(control).closest('form').children("#dynamic-productname").val() != undefined) {
            name = $(control).closest('form').children("#dynamic-productname").val();
        }
        else { name = $(".product-name").html() };
        if ($("#lnkProductBrand").html() != undefined) { brand = $("#lnkProductBrand").html().trim() };
        if ($("#spnProductPrice").html() != undefined) { price = $("#spnProductPrice").html().trim() };
        if ($("#Quantity").val() != undefined) { quantity = $("#Quantity").val(); } else { quantity = $(control).closest('form').children("#dynamic-quantity").val() }
        var cartItem: Znode.Core.EcommerceCartItemDataModel = {
            id: $(control).closest('form').children("#dynamic-sku").val(),
            name: name,
            brand: brand,
            variant: $(control).closest('form').children("#dynamic-configurableproductskus").val(),
            quantity: quantity,
            price: price
        }
        znodeDataLayer.push({
            'event': 'add_to_cart',
            'ecommerce': {
                'currencyCode': currencyCode,
                'add': {
                    'products': [cartItem]
                }
            }
        });
    }

    SendAddToCartsFromQuickOrder(): any {
        var _content = $(".quickOrderAddToCart").closest(".quick-order-container");
        var productId: number = parseInt(_content.find("#hdnQuickOrderProductId").val());
        var cartItem: Znode.Core.EcommerceCartItemDataModel = {
            id: _content.find("#hdnQuickOrderSku").val(),
            name: _content.find("#hdnQuickOrderProductName").val(),
            brand: "",
            price: _content.find("#hdnRetailPrice").val(),
            variant: $(".quickOrderAddToCart").closest('form').children("#dynamic-configurableproductskus").val(),
            quantity: $("#txtQuickOrderQuantity").val()
        }
        znodeDataLayer.push({
            'event': 'add_to_cart',
            'ecommerce': {
                'add': {
                    'products': [cartItem]
                }
            }
        });
    }

    SendAddToCartsFromMultipleQuickOrder(data: any): any {
        var ecommerce = {};
        var add = {};
        var cartItems = [];

        $.each(data, function (v, e) {
            var cartItem: Znode.Core.EcommerceCartItemDataModel = {
                id: e.Sku,
                name: e.ProductName,
                brand: "",
                price: "",
                variant: "",
                quantity: e.Quantity
            }
            cartItems.push(cartItem);
        });
        add["products"] = cartItems;
        ecommerce["add"] = add;
        znodeDataLayer.push({ "ecommerce": ecommerce, "event": "add_to_cart" });
    }

    SendRemoveFromCartsForAllCartItems(data: any): any {
        var ecommerce = {};
        var remove = {};
        var cartItems = [];

        $.each(data, function (v, e) {
            var cartItem: Znode.Core.EcommerceCartItemDataModel = {
                id: e.SKU,
                name: e.Name,
                brand: "",
                price: e.ProductPrice,
                variant: e.Variant,
                quantity: e.Quantity
            }
            cartItems.push(cartItem);
        });
        remove["products"] = cartItems;
        ecommerce["remove"] = remove;
        znodeDataLayer.push({ "ecommerce": ecommerce, "event": "remove_from_cart" });
    }

    SendRemoveFromCartsForSingleCartItem(data: any): any {
        var cartItem: Znode.Core.EcommerceCartItemDataModel = {
            id: data.SKU,
            name: data.Name,
            brand: "",
            price: data.ProductPrice,
            variant: data.Variant,
            quantity: data.Quantity
        }
        znodeDataLayer.push({
            'event': 'remove_from_cart',
            'ecommerce': {
                'remove': {
                    'products': [cartItem]
                }
            }
        });
    }

    SendEcommerceCheckoutData(data): any {
        var ecommerce = {};
        var checkout = {};
        var products = [];
        var actionField = {};
        $.each(data, function (v, e) {
            var product = {};
            product["id"] = e.SKU;
            product["name"] = e.Name;
            product["price"] = e.ProductPrice;
            product["quantity"] = e.Quantity;
            products.push(product);
        });
        actionField["step"] = 1;
        actionField["option"] = "Shopping Cart Page";
        checkout["actionField"] = actionField;
        checkout["products"] = products;
        ecommerce["checkout"] = checkout;
        znodeDataLayer.push({ "ecommerce": ecommerce, "event": "begin_checkout" });
    }

    SendEcommerceCheckoutOptionData(step: number, option: string): any {
        znodeDataLayer.push({
            'event': 'checkout',
            'ecommerce': {
                'checkout': {
                    'actionField': { 'step': step, 'option': option }
                }
            }
        });
    }

    SendEcommercePurchaseData(data: any, storeName: string, currencyCode: string): any {
        var ecommerce = {};
        var purchase = {};
        var actionField: Znode.Core.EcommercePurchaseActionFieldModel = {
            id: data.OrderNumber,
            affiliation: storeName,
            revenue: data.Total,
            tax: data.TaxCost,
            shipping: data.ShippingCost,
            currency: currencyCode,
        }
        if (data.CouponCode != "" && data.CouponCode != null) {
            actionField["coupon"] = GoogleAnalytics.prototype.GetEcommerceOrderCouponCode(data.CouponCode);
        }
        purchase["actionField"] = actionField;

        var purchasedProducts = new Array();
        $.each(data.PurchasedProducts, function (v, e) {
            var purchasedProduct: Znode.Core.EcommercePurchaseItemsDataModel = {
                id: e.Id,
                sku: e.Sku,
                name: e.ProductName,
                quantity: e.Quantity,
                price: e.Price,
                total: e.Price * e.Quantity,
                description: e.Description
            }
            purchasedProducts.push(purchasedProduct);
        });

        purchase["products"] = purchasedProducts;
        ecommerce["purchase"] = purchase;
        znodeDataLayer.push({ "ecommerce": ecommerce });
    }

    GetEcommerceOrderCouponCode(item): any {
        var itemValue = item != undefined ? item : "";
        return itemValue;
    }
}