
$(window).on("load", function () {
    debugger;
    if (location.href.indexOf("EditArtifiProduct") > -1) {
        var _custmizeproductid = $("#artifi-design-call").data("custmizeproductid");

        if (_custmizeproductid == "0" || _custmizeproductid == "" || _custmizeproductid == 0 || _custmizeproductid == null)
            _custmizeproductid = getParameterByName("custmizeproductid");

        var sku = "";
        if (getParameterByName("groupsku") != "" && getParameterByName("groupsku") != null && getParameterByName("groupsku") != "undefined" && getParameterByName("quantity") != "" && getParameterByName("quantity") != null && getParameterByName("quantity") != "undefined") {
            $("#dynamic-groupproductskus").val(getParameterByName("sku"));
            $("#dynamic-groupproductsquantity").val(getParameterByName("quantity"));
            sku = getParameterByName("groupsku");
        }
        else if (getParameterByName("configurablesku") != "" && getParameterByName("configurablesku") != null && getParameterByName("configurablesku") != "undefined") {
            sku = getParameterByName("configurablesku");
        }
        else
            sku = getParameterByName("sku");

        var userId = getParameterByName("userId");
        if (userId == undefined || userId == null || userId == "") {
            userId = sessionStorage.getItem("UserId");
        }
        var artifiCredentials = "";
        GetArtfiCredentials($("#hdnPortalId").val(), userId, false, function (response) {
            artifiCredentials =
                        {
                            websiteId: response.websiteId,
                            webApiclientKey: response.webApiclientKey,
                            jsUrl: response.jsUrl,
                            userId: response.userId,
                        }
        });
        var artifijs = "" + artifiCredentials.jsUrl + "//script/sasintegration/artifiintegration.js";
        var len = $('script').filter(function () {
            return ($(this).attr('src') == artifijs);
        }).length;

        if (_custmizeproductid !== "0" && _custmizeproductid !== "" && _custmizeproductid !== 0 && _custmizeproductid !== null)
            $.getScript(artifijs, function () { LaunchArtifi(artifiCredentials, _custmizeproductid, sku) });
    }
})

$(document).off("click", "#artifi-design-edit");
$(document).on("click", "#artifi-design-edit", function (e) {
    var _custmizeproductid = $(this).data('custmizeproductid');
    var configurablesku = $(this).data('configurablesku');
    var groupsku = $(this).data('groupsku')

    var sku = configurablesku != "" && configurablesku != 'undefined' ? configurablesku : groupsku;
    var queryStringKey = "sku";
    if (configurablesku != "" && configurablesku != 'undefined')
        queryStringKey = "configurablesku";
    else if (groupsku != "" && groupsku != 'undefined')
        queryStringKey = "groupsku";
    else
        sku = $(this).data('sku');
    var quantity = "";
    if (queryStringKey != "")
        quantity = $(this).data('quantity');

    if (_custmizeproductid != "") {
        e.preventDefault();
        var user = $("#UserId");
        var userId = "";
        if (typeof user != "undefined" && user != null && user != "") {
            userId = $("#UserId").val();
        }
        if (typeof userId == "undefined" || userId == null || userId == "" || userId == "0") {
            userId = sessionStorage.getItem("UserId");
        }
        var _url = "../product/EditArtifiProduct?id=" + $(this).data('productid') + "&custmizeproductid=" + $(this).data('custmizeproductid') + "&" + queryStringKey + "=" + sku + "&quantity=" + quantity + "&productname=" + $(this).data('productname') + "&userId=" + userId + "";

        window.location.href = _url;
    }

});


$(document).off("click", "#artifi-previewproduct");
$(document).on("click", "#artifi-previewproduct", function (e) {
    e.preventDefault();
    var _this = this;
    var artifiCredentials = "";
    GetArtfiCredentials($("#hdnPortalId").val(), sessionStorage.getItem("UserId"), false, function (response) {
        artifiCredentials =
                    {
                        websiteId: response.websiteId,
                        webApiclientKey: response.webApiclientKey,
                        jsUrl: response.jsUrl,
                        userId: response.userId,
                    }
    });
    var configurablesku = $(_this).data('configurablesku');
    var groupsku = $(_this).data('groupsku')
    var configSKU = $("#dynamic-configurableproductskus").val();
    var _param = "";
    var sku = configurablesku != "" && configurablesku != 'undefined' ? configurablesku : groupsku;
    sku = sku == "" || typeof (sku) == 'undefined' ? $(_this).data("sku") : sku;
    if (configurablesku != "" && configurablesku != null && configurablesku != 'undefined') {
        _param = {
            websiteId: artifiCredentials.websiteId,
            webApiclientKey: artifiCredentials.webApiclientKey,
            sku: sku,
            designId: $(_this).data("custmizeproductid"),
            //isGuest: true,
            userId: artifiCredentials.userId,
            height: "100%",
            width: "100%",
        }
    }
    else {
        _param = {
            websiteId: artifiCredentials.websiteId,
            webApiclientKey: artifiCredentials.webApiclientKey,
            productCode: sku,
            designId: $(_this).data("custmizeproductid"),
            //isGuest: true,
            userId: artifiCredentials.userId,
            height: "100%",
            width: "100%",
        }
    }
    var _url = Artifi.getPreviewUrl(_param);
    $("#preview-popup-content").html("<iframe src='" + _url + "' height='500px' width='100%' frameborder='0' style='position: relative;'></iframe>")
});

$(document).off("click", "#artifi-design-call");
$(document).on("click", "#artifi-design-call", function (e) {
    debugger;
    e.preventDefault();
    var groupSku = $(this).attr("data-artifigroupsku");
    var sku = groupSku;
    var status = false;

    if (sku != "" && sku != null && sku != 'undefined') {
        var publishProductId = $(this).attr("data-productId");
        var quantity = $("#" + publishProductId + "").val();
        if (quantity == null || quantity == "") {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredProductQuantity"), "error", true, fadeOutTime);
            return false;
        }
        Product.prototype.OnAssociatedProductQuantityChange();
        if (isAddToCartGroupProduct && Product.prototype.CheckGroupProductQuantity($("#dynamic-parentproductid").val(), sku, quantity)) {
            CallArtifiDesigner(0, sku);
        }
    }
    else if (Product.prototype.OnQuantityChange()) {
        CallArtifiDesigner(0);
    }
})

function UpdateUserId(OldUserId, newUserId, portalId) {
    if (OldUserId != null && OldUserId != 'undefined' && OldUserId != '0') {
        var integrationValues = "";
        GetArtfiCredentials(portalId, newUserId, false, function (response) {
            integrationValues =
                        {
                            websiteId: response.websiteId,
                            webApiclientKey: response.webApiclientKey,
                            jsUrl: response.jsUrl,
                            newUserId: response.userId,
                            oldUserId: OldUserId
                        }
        });
        var artifijs = "" + integrationValues.jsUrl + "//script/sasintegration/artifiintegration.js";
        $.getScript(artifijs, function () { UpdateUser(integrationValues); });
    }
}

function UpdateUser(integrationValues) {
    $.ajax({
        url: "" + integrationValues.jsUrl + "/Designer/Services/UpdateUserId?newUserId=" + integrationValues.newUserId + "&OldUserId=" + integrationValues.oldUserId + "&websiteId=" + integrationValues.websiteId + "&webApiClientKey=" + integrationValues.webApiclientKey,
        method: "GET",
        dataType: "json",
        async: true,
        success: function (data) {
            console.log(data);
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function CallArtifiDesigner(custmizeproductid, groupProductSKU) {
    var artifiCredentials = "";
    var UserId = sessionStorage.getItem("UserId");
    var isGuest = false;
    GetArtfiCredentials($("#hdnPortalId").val(), UserId, false, function (response) {
        artifiCredentials =
        {
            websiteId: response.websiteId,
            webApiclientKey: response.webApiclientKey,
            jsUrl: response.jsUrl,
            userId: response.userId,
            portalId: $("#hdnPortalId").val()
        }
        isGuest = response.isGuest;
    });
    if (isGuest)
        sessionStorage.setItem("UserId", artifiCredentials.userId)
    var configurableSku = $("#dynamic-configurableproductskus").val();
    var sku = (groupProductSKU == "" || groupProductSKU == null || groupProductSKU == 'undefined') ? (configurableSku == "" || configurableSku == null || configurableSku == 'undefined') ? $("#dynamic-sku").val() : configurableSku : groupProductSKU;
    var len = $('script').filter(function () {
        return ($(this).attr('src') == artifiCredentials.jsUrl);
    }).length;
    ShowLoader();
    var artifijs = "" + artifiCredentials.jsUrl + "//script/sasintegration/artifiintegration.js";
    $.getScript(artifijs, function () { LaunchArtifi(artifiCredentials, 0, sku) });
}

if (window.addEventListener) {
    addEventListener("message", receiveArtifiMessage, false)
} else {
    attachEvent("onmessage", receiveArtifiMessage)
}

function GetArtfiCredentials(portalId, userId, isGetScript, callbackMethod) {
    $.ajax({
        url: "/artificheckout/getartificredentials?portalId=" + portalId + "&userId=" + userId,
        method: "GET",
        dataType: "json",
        async: false,
        success: function (data) {
            var _param = {
                websiteId: data.WebsiteCode,
                webApiclientKey: data.WebApiClientKey,
                jsUrl: data.JsUrl,
                userId: data.userId,
                isGuest: data.isGuest
            }
            if (isGetScript) {
                $.getScript("" + _param.jsUrl + "//script/sasintegration/artifiintegration.js");
            }
            callbackMethod(_param);
        },
        error: function (data) {
            console.log(data);
        }
    });
}


function receiveArtifiMessage(event) {
    var origin = event.origin || event.originalEvent.origin;
    var eventObj = JSON.parse(event.data);
    var action = eventObj.action;
    var isFromDesignList = getParameterByName("isFromDesignList");
    switch (action) {
        case Artifi.Constant.addToCart:
            var custmizeproductid = getParameterByName("custmizeproductid");
            var data = eventObj.data;
            if ((custmizeproductid != "" && typeof (custmizeproductid) != 'undefined' && custmizeproductid > 0)) {
                if ((isFromDesignList == "" || isFromDesignList == null || typeof (isFromDesignList) == 'undefined' || !isFromDesignList)) {
                    $.ajax({
                        url: "/artifiproduct/updateartificartitem?orderLineItemsDesignId=" + data.custmizeProductId + "&aiImagePath=" + data.savedDesigns[0],
                        method: "GET",
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            location.href = "/cart";
                        },
                        error: function (data) {
                            localStorage.removeItem("isedit");
                            console.log(data);
                        }
                    });
                }
                else {
                    SubmitEditDesignForm(data.savedDesigns[0]);
                }
            }
            else {

                var _formId = "#Form_" + $("#dynamic-productid").val()
                Product.prototype.BindAddOnProductSKU($("#button-addtocart_" + $("#dynamic-parentproductid").val()));

                var personalisedCodes = [];
                var personalisedValues = [];
                $("input[IsPersonalizable = True]").each(function () {
                    personalisedValues.push($(this).val());
                    personalisedCodes.push($(this).attr('id'));
                });
                var customProductId = data.custmizeProductId;
                if (typeof customProductId != 'undefined' && customProductId != null && customProductId != "") {
                    personalisedCodes.push('ArtifiDesignId');
                    personalisedValues.push(customProductId);
                    personalisedCodes.push('ArtifiImagePath');
                    personalisedValues.push(data.savedDesigns[0]);
                    $(_formId).children("#dynamic-personalisedcodes").val(personalisedCodes);
                    $(_formId).children("#dynamic-personalisedvalues").val(personalisedValues);
                }
                $(_formId).submit();
            }
            break;
        case "close-pop-up":
            if (window.artifiAddToCartSuccess == false) {
                closeArtifiWindow();
            }
            break;
        case "error":
            $(".personalizeDivCover").addClass("na");
            break;
    }
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function LaunchArtifi(data, custmizeproductid, groupProductSKU) {
    var sku = groupProductSKU == "" || groupProductSKU == null || groupProductSKU == 'undefined' ? $("#dynamic-sku").val() : groupProductSKU;
    var _param = "";
    var configSKU = $("#dynamic-configurableproductskus").val();
    if (configSKU == "" || configSKU == null || configSKU == 'undefined') {
        configSKU = getParameterByName("configurablesku");
    }
    if (configSKU != "" && configSKU != null && configSKU != 'undefined') {
        _param = {
            divId: "Artifi-Designer",
            websiteId: data.websiteId,
            webApiclientKey: data.webApiclientKey,
            sku: sku,
            designId: custmizeproductid,
            //isGuest: true,
            portalId: data.portalId,
            userId: data.userId,
            height: "700px",
            width: "100%",
        }

    }
    else {
        _param = {
            divId: "Artifi-Designer",
            websiteId: data.websiteId,
            webApiclientKey: data.webApiclientKey,
            productCode: sku,
            designId: custmizeproductid,
            //isGuest: true,
            portalId: data.portalId,
            userId: data.userId,
            height: "700px",
            width: "100%",
        }
    }

    var productName = "";
    if (custmizeproductid > 0) {
        productName = getParameterByName("productname")
    }
    else {
        productName = $('.product-name').html();
        groupProductSKU = $(".product-number-details").text().trim().split(":")[1];
    }
    $("#Artifi-Designer-parent").prepend("<h1 class='product-name'>" + productName + "</h1>");
    $(".product-meta").find("#main-content").hide();
    $(document).scrollTop("-600");
    Artifi.Initialize(_param);
    HideLoader();
    return _param;
}

function editSavedDesign(control) {
    var _custmizeproductid = $(control).attr("data-customizedProductId");
    var sku = $(control).attr("data-sku");
    var productDetails = "";
    getProductDetailsBySKU(sku, function (res) {
        productDetails = res;
    })
    var queryStringKey = "groupsku";
    var _url = "../product/EditArtifiProduct?id=" + productDetails.productId + "&custmizeproductid=" + _custmizeproductid + "&" + queryStringKey + "=" + sku + "&quantity=1&productname=" + productDetails.productName + "&userId=" + $(control).attr("data-userId") + "&isFromDesignList=" + true + "";
    window.location.href = _url;
}

function SubmitEditDesignForm(savedDesign) {
    var _formId = "#addtocart-form";
    $(_formId + " input[name='SKU']").val(getParameterByName("groupsku"));
    $(_formId + " input[name='Quantity']").val(1);
    $(_formId).append("<input type='hidden' id='savedDesigns' name='AISavedDesigns' value='" + savedDesign + "'>");
    $(_formId).append("<input type='hidden' id='UserId' name='UserId' value='" + getParameterByName("userId") + "'>");
    $(_formId).append("<input type='hidden' id='custmizeProductId' name='OrderLineItemsDesignId' value='" + getParameterByName("custmizeproductid") + "'>");

    // submit form
    $(_formId).submit();

}

function getProductDetailsBySKU(sku, callBackMethod) {
    $.ajax({
        url: "/artifiproduct/getproductdetailsbysku",
        method: "GET",
        data: { sku: sku },
        dataType: "json",
        async: false,
        success: function (data) {
            callBackMethod(data);
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function ShowLoader() {
    $("#Single-loader-content-backdrop").show();
}

function HideLoader() {
    $("#Single-loader-content-backdrop").hide();
}