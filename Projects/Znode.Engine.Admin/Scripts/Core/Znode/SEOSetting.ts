declare function fastselectwrapper(control: any, controlValue: any): any;
class SEO extends ZnodeBase {
    localId: number;
    constructor() {
        super();
    }

    Init() {
        SEO.prototype.LocaleDropDownChangeForSEODetails();
        SEO.prototype.GetContentPagList();
    }

    GetSelectedStoreSEOSetting() {
        Endpoint.prototype.GetPortalSeoSettings(parseInt($("#hdnPortalId").val()), function (response) {
            $("#PortalSeoSetting").html(response);
            $('*[data-url]').each(function () { fastselectwrapper($(this), $(this).data("onselect-function")); });
        });
    }

    DeleteUrlRedirect(control): any {
        var urlRedirectIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (urlRedirectIds.length > 0) {
            Endpoint.prototype.DeleteUrlRedirect(urlRedirectIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    LocaleDropDownChangeForSEODetails() {
        $("#ddl_locale_list_for_seoDetails").on("change", function () {
            Endpoint.prototype.SEODetails($("#ItemName").val(), $("#CMSSEOTypeId").val(), $("#SEOCode").val(), $("#ddl_locale_list_for_seoDetails").val(), $("#PortalId").val(), function (response) {
                $('#div_seo_details_locale_field').html(response);
                $('#seoName').text($('#name').val());
            });
        });
    }

    GetPublishedProductList() {
        SEO.prototype.SetLink();        
        Endpoint.prototype.GetPublishedProductList(parseInt($("#hdnPortalId").val()), function (response) {            
            $("#productList").html(response);
        });
    }

    GetPublishedCategoryList() {
        SEO.prototype.SetLink();
        Endpoint.prototype.GetPublishedCategoryList(parseInt($("#hdnPortalId").val()), function (response) {
            $("#categoryList").html(response);
        });
    }

    GetContentPagList() {
        SEO.prototype.SetLink();
        Endpoint.prototype.GetContentPagesList(parseInt($("#hdnPortalId").val()), function (response) {
            $("#contentPageList").html(response);
        });
    }

    GetURLRedirectPageList() {
        SEO.prototype.SetLink();
        Endpoint.prototype.UrlRedirectList(parseInt($("#hdnPortalId").val()), function (response) {
            $("#urlRedirectList").html(response);
        });
    }

    CreateUrlRedirect() {
        Endpoint.prototype.CreateUrlRedirect($("#hdnPortalId").val(), function (response) {
            $("#urlRedirect").html(response);
            $('*[data-url]').each(function () { fastselectwrapper($(this), $(this).data("onselect-function")); });
        });
    }

    SetLink() {
        var _newUrl = MediaManagerTools.prototype.UpdateQueryString("portalId", $("#hdnPortalId").val(), window.location.href);
        window.history.pushState({ path: _newUrl }, '', _newUrl);
    }

    //This method is used to get portal list on aside panel
    GetPortalList(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/SEO/GetPortalList', 'divStoreListAsidePanel');
    }

    GetPortalListForURL(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/SEO/GetPortalList', 'divStoreListAsidePanel');
    }

    //To Do: To bind portal information
    OnSelectPortalResult(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: string = item.Id;
            let dataView = $("body").data("view");          
            $('#StoreName').val(portalName);          
            Store.prototype.OnSelectStoreAutocompleteDataBind(item);
            if ($('#frmCreateEditUrlRedirect').length != 1) {
                if (dataView != undefined && dataView != "") {
                    switch (dataView) {
                        case "GetProductsForSEO": SEO.prototype.GetPublishedProductList(); break;
                        case "GetCategoriesForSEO": SEO.prototype.GetPublishedCategoryList(); break;
                        case "GetContentPages": SEO.prototype.GetContentPagList(); break;
                        case "UrlRedirectList": SEO.prototype.GetURLRedirectPageList(); break;
                        case "SEOSetting": SEO.prototype.GetSelectedStoreSEOSetting(); break;
                        case "SaveSEOSetting": SEO.prototype.GetSelectedStoreSEOSetting(); break;
                    }
                }
            }           
        }
    }

    GetPortalDetail(): void {
        $("#ZnodeUserPortalList").find("table tr").click(function () {
            let portalName: string = $(this).find("td[class='storecolumn']").text();
            let portalId: string = $(this).find("td")[0].innerHTML;
            let dataView = $("body").data("view");
            $('#txtPortalName').val(portalName);
            $('#StoreName').val(portalName);
            $('#hdnPortalId').val(portalId);
            $('#PortalId').val(portalId);
            if ($('#frmCreateEditUrlRedirect').length != 1) {
                if (dataView != undefined && dataView != "") {
                    switch (dataView) {
                        case "GetProductsForSEO": SEO.prototype.GetPublishedProductList(); break;
                        case "GetCategoriesForSEO": SEO.prototype.GetPublishedCategoryList(); break;
                        case "GetContentPages": SEO.prototype.GetContentPagList(); break;
                        case "UrlRedirectList": SEO.prototype.GetURLRedirectPageList(); break;
                        case "SEOSetting": SEO.prototype.GetSelectedStoreSEOSetting(); break;
                        case "SaveSEOSetting": SEO.prototype.GetSelectedStoreSEOSetting(); break;
                    }
                }
            }
            $('#divStoreListAsidePanel').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
            $('#ZnodeUserPortalList').html("");
        });
    }

    UpdatePublish(): any {
        $("#frmCreatEditSEODEtails").attr('action', 'UpdateAndPublishSeo');
        $("#frmCreatEditSEODEtails").submit();
    }

    DdlCultureChange() {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
            $.cookie("_productCulture", $("#ddlCultureSpan").attr("data-value"), { expires: expiresTime }); // expires after 2 hours
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];
        if (selectedTab != undefined)
            window.location.replace(orignalUrl + "?SEOCode=" + $("#portalId").val() + "&selectedtab=" + selectedTab);
        else {
            if (url.indexOf('SEOCode') > -1)
                window.location.replace(orignalUrl + "?SEOCode=" + $("#portalId").val());
            else
                window.location.reload();
        }
    }

    PublishSeoPopup(zPublishAnchor): any {
        zPublishAnchor.attr("href", "#");
        $("#SEOCode").val($(zPublishAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        this.localId = Number($("#ddlCultureSpan").attr("data-value"));
        $("#PublishSeo").modal('show');
    }

    GetContentPagePublishSeoPopup(zPublishAnchor): any {
        zPublishAnchor.attr("href", "#");
        var seoCode = $(zPublishAnchor).closest("tr").find("td").first().next().html();
        $("#SEOCode").val(encodeURIComponent(seoCode));
        this.localId = Number($("#ddlCultureSpan").attr("data-value"));
        $("#PublishSeo").modal('show');
    }

    PublishSeo(control): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        Endpoint.prototype.PublishSeoWithPreview($("#SEOCode").val(), $("#hdnPortalId").val(), 0, $("#hdnSEOTypeId").val(), publishStateFormData, true, function (res) {
            if (res.status == true)
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "success", isFadeOut, fadeOutTime);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, "error", isFadeOut, fadeOutTime);
            DynamicGrid.prototype.RefreshGridOndelete(control, res);
        });
    }

    UpdateAndPublishSeo(): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        $("#frmCreatEditSEODEtails [name=TargetPublishState]").val(publishStateFormData);
        $("#frmCreatEditSEODEtails").attr("action", "UpdateAndPublishSeo");
        $("#frmCreatEditSEODEtails").submit();
    }
    PublishSeoPopupEdit(zPublishAnchor): any {
        $("#UpdateAndPublishSeo").modal('show')
    }

}