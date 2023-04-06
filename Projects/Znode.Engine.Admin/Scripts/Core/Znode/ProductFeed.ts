class ProductFeed extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    _multiFastItemdata: Array<string>;

    constructor() {
        super();
        ProductFeed.prototype._multiFastItemdata = new Array<string>();
    }

    Init(): any {
        ProductFeed.prototype.AddCreateFileName();
        ProductFeed.prototype.DisplayXMLSiteMapType();
    }

    DisplayCustomDate(control): any {
        if ($.trim($(control).val()) === "Use date / time of this update") {
            $('#CustomDate-content').show();
        }
        else {
            $('#CustomDate-content').hide();
        }
        if ($.trim($(control).val()) === "Use the database update date") {
            var dt = new Date(Date.now());
            var date = dt.toLocaleDateString() + " " + dt.toLocaleTimeString();
            $('#DBDate').val(date);
        }
    }

    DisplayXMLSiteMapType(): any {
        if (($('#ddlXMLSiteMap').val() == Enum.ProductFeedType[Enum.ProductFeedType.XmlSiteMap]) || ($('#txtSitemapType').data('typecode') == Enum.ProductFeedType[Enum.ProductFeedType.XmlSiteMap])) {
            $('#rdbXMLSiteMapType').show();
            $('#GoogleFeedFields').hide();
            $('.defaultLocalId').hide();
            ProductFeed.prototype.AddFileName();
        } else if (($('#ddlXMLSiteMap').val() == Enum.ProductFeedType[Enum.ProductFeedType.Google]) || $('#txtSitemapType').data('typecode') == Enum.ProductFeedType[Enum.ProductFeedType.Google]) {
            $('#rdbXMLSiteMapType').hide();
            $('#GoogleFeedFields').show();
            $('.defaultLocalId').show();
            $('#FileName').val(Constant.googleProductFeed);
            ProductFeed.prototype.AppendPortalId();
        } else if (($('#ddlXMLSiteMap').val() == Enum.ProductFeedType[Enum.ProductFeedType.Bing]) || $('#txtSitemapType').data('typecode') == Enum.ProductFeedType[Enum.ProductFeedType.Bing]) {
            $('#rdbXMLSiteMapType').hide();
            $('#GoogleFeedFields').show();
            $('.defaultLocalId').show();
            $('#FileName').val(Constant.bingProductFeed);
            ProductFeed.prototype.AppendPortalId();
        }
        else if (($('#ddlXMLSiteMap').val() == Enum.ProductFeedType[Enum.ProductFeedType.Xml]) || $('#txtSitemapType').data('typecode') == Enum.ProductFeedType[Enum.ProductFeedType.Xml]) {
            $('#rdbXMLSiteMapType').hide();
            $('#GoogleFeedFields').show();
            $('.defaultLocalId').show();
            $('#FileName').val(Constant.xmlProductFeed);
            ProductFeed.prototype.AppendPortalId();
        }
    }

    AppendPortalId(): any {
        if ($('#PortalId').val() > 0) {
            var portalId: string = $('#PortalId').val();
            var fileName: string = $("#FileName").val()
            $("#FileName").val("");
            $("#FileName").val(fileName + "_" + portalId);
        }
    }

    DisableXMLSiteMapType(): any {
        if ($('#ProductFeedId').val() > 0) {
            $('#XMLTypeInputs input').prop('disabled', true);
        }
    }

    public DeleteProductFeed(control): void {
        var productFeedId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (productFeedId.length > 0) {
            Endpoint.prototype.DeleteProductFeed(productFeedId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }


    OnSelectPortalResult(item: any): void {
        Store.prototype.OnSelectStoreAutocompleteDataBind(item);
        ProductFeed.prototype.DisplayXMLSiteMapType();
    }
    SetPortals() {
        if ($("#txtPortalName").val() == '' || $("#txtPortalName").val() == undefined) {

            $("#StoreName-error").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectPortal")).addClass("field-validation-error").show();
            $("#txtPortalName").parent("div").addClass('input-validation-error');
            return false;
        }
        return true;
    }

    SetSubmitOnSave() {
        var isPortalValidate = ProductFeed.prototype.SetPortals();
        if ($('#frmProductFeed').valid() && isPortalValidate) {
            var localeId: number = parseInt($("#LocaleId").val());
            var fileName: string = $("#FileName").val();
            if (($('#ProductFeedId').val() > 0)) {
                $('#frmProductFeed').submit();
            }
            else {
                Endpoint.prototype.CheckFileNameExist(localeId, fileName, function (response) {
                    if (response.data) {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ProductFeedAlreadyExist"), 'error', isFadeOut, fadeOutTime);
                        return false;
                    }
                    $('#frmProductFeed').submit();
                });
            }
        }
    }

    SetFeedIsSelectAllPortalOnInit() {
        if ($('input:checkbox[name="PortalId"]').prop('checked') == true) {
            $(".chkStoresList").hide();
        } else {
            if (($('#Stores').val() != undefined) && ($('#Stores').val() != "")) {
                var portalsArray = $('#Stores').val().split(',');
                Endpoint.prototype.GetPortalList(Constant.storelist, function (response) {
                    ZnodeBase.prototype.SetInitialMultifastselectInput(portalsArray, response, $("#txtPortalName"));
                });
            }
            else {
                ZnodeBase.prototype.SetInitialMultifastselectInput(null, null, $("#txtPortalName"));
            }
            $(".chkStoresList").show();
        }
    }

    public GenerateProductFeed(url): void {
        Endpoint.prototype.GenerateProductFeed(url, function (response) {
            if (response.success) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
    }

    AddCreateFileName(): any {
        if (!($('#ProductFeedId').val() > 0)) {
            ProductFeed.prototype.AddFileName();
        }
    }

    AddFileName(): any {
        $('#FileName').val('');
        if ($('input[name="ProductFeedSiteMapTypeCode"]:checked').val() === Enum.ProductFeedSiteMapType[Enum.ProductFeedSiteMapType.Category]) {
            $('#FileName').val(Constant.categoryXMLSitemap);
        } else if ($('input[name="ProductFeedSiteMapTypeCode"]:checked').val() === Enum.ProductFeedSiteMapType[Enum.ProductFeedSiteMapType.Content]) {
            $('#FileName').val(Constant.contentPagesXMLSitemap);
        } else if ($('input[name="ProductFeedSiteMapTypeCode"]:checked').val() === Enum.ProductFeedSiteMapType[Enum.ProductFeedSiteMapType.Product]) {
            $('#FileName').val(Constant.productXMLSitemap);
        } else if ($('input[name="ProductFeedSiteMapTypeCode"]:checked').val() === Enum.ProductFeedSiteMapType[Enum.ProductFeedSiteMapType.ALL]) {
            $('#FileName').val(Constant.allXMLSitemap);
        }
        ProductFeed.prototype.AppendPortalId();
    }
}

$(document).off("click", "#ZnodeProductFeed .z-download");
$(document).on("click", "#ZnodeProductFeed .z-download", function (e) {
    e.preventDefault();
    ProductFeed.prototype.GenerateProductFeed($(this).attr('href'));
});