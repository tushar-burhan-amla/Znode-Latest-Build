declare function reInitializationMce(): any;
class WebSite extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    constructor() {
        super();
    }
    Init() {
        WebSite.prototype.LocaleDropDownChange();
        WebSite.prototype.ValidationAutoPlayTime();
        WebSite.prototype.AutplayRequiredOnClick();

        if (parseInt($("#CMSSliderBannerId").val(), 10) < 1) {
            $("#ddlCulture").prop("disabled", true);
            $("#ddlCulture").addClass("disabled");
        }
    }

    ValidateSlider(): any {
        $("#Name").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            WebSite.prototype.ValidateSliderName($("#Name").val());
            ZnodeBase.prototype.HideLoader();
        });
        $("#saveSlider").off("click");
        $("#saveSlider").on("click", function () {
            if (WebSite.prototype.ValidateSliderName($("#Name").val()) == false)
                return false;
        });
    }


    ValidateSliderName(name: string): boolean {
        var isValid = true;
        if (!name) { //if sliderName is empty then show required msg
            WebSite.prototype.showError(ZnodeBase.prototype.getResourceByKeyName("SliderNameRequired"))
            return false;
        }
        if (name && name.length > 100) { // if SliderName is more than 100 characters then show length exceed msg
            WebSite.prototype.showError(ZnodeBase.prototype.getResourceByKeyName("SliderNameLengthExceed"))
            return false;
        }
        if (name != '') {
            Endpoint.prototype.IsSliderNameExist(name, $("#cmsSliderId").val(), function (response) {
                if (!response) {
                    WebSite.prototype.showError(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistSliderName"))
                    isValid = false;
                }
            });
        }
        return isValid;
    }

    showError(message: string): void {
        $("#Name").addClass("input-validation-error");
        $("#errorSpanSliderName").addClass("error-msg");
        $("#errorSpanSliderName").text(message);
        $("#errorSpanSliderName").show();

    }

    ValidateBanner(): any {
        $("#Title").on("blur", function () {
            ZnodeBase.prototype.ShowLoader();
            WebSite.prototype.ValidateExistBannerTitle();
            ZnodeBase.prototype.HideLoader();
        });
    }

    ValidateExistBannerTitle(): boolean {
        var isValid = true;
        if ($("#Title").val() != '') {
            Endpoint.prototype.IsBannerNameExist($("#Title").val(), $("#CMSSliderBannerId").val(), $("#CMSSliderId").val(), function (response) {
                if (!response) {
                    $("#Title").addClass("input-validation-error");
                    $("#errorSpanBannerTitle").addClass("error-msg");
                    $("#errorSpanBannerTitle").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistBannerTitle"));
                    $("#errorSpanBannerTitle").show();
                    isValid = false;
                }
                ZnodeBase.prototype.HideLoader();
            });
        }
        return isValid;
    }

    AddSlider(): any {
        Endpoint.prototype.AddSlider(function (res) {
            $("#divAddSliderPopup").modal("show");
            $("#divAddSliderPopup").html(res);
        });
    }


    AddSliderResult(response: any) {
        if (response.isSuccess) {
            $("#divAddSliderPopup").modal("hide");
            ZnodeBase.prototype.RemovePopupOverlay();
            window.location.href = window.location.protocol + "//" + window.location.host + "/WebSite/GetBannerList?cmsSliderId=" + response.cmsSliderId;
        }
        else {
            WebSite.prototype.AddSlider();
            if (!WebSite.prototype.ValidateSliderName(response.name)) {
                return false;
            }
        }
    }


    DeleteSliders(control): any {
        var cmsSliderId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsSliderId.length > 0) {
            Endpoint.prototype.DeleteSliders(cmsSliderId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    PublishSliderPopup(zPublishAnchor) {
        if (zPublishAnchor != null) {
            zPublishAnchor.attr("href", "#");
            $("#CMSSliderId").val($(zPublishAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        }
        $("#SliderPublishPopup").modal('show');
    }

    PublishSlider(): any {
        ZnodeBase.prototype.ShowLoader();
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        var cmsSliderId = $("#CMSSliderId").val();
        cmsSliderId = isNaN(cmsSliderId) ? $("#hdnCMSSliderId").val() : $("#CMSSliderId").val();
        Endpoint.prototype.PublishSlider(cmsSliderId, publishStateFormData, function (res) {
            DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeCMSSlider").find("#refreshGrid"), res);
        });
    }

    UpdatePublishSlider(): any {
        ZnodeBase.prototype.ShowLoader();
        var cmsSliderId = $("#CMSSliderId").val();
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        cmsSliderId = isNaN(cmsSliderId) ? $("#hdnCMSSliderId").val() : $("#CMSSliderId").val();
        window.location.href = "/WebSite/UpdateAndPublishSliderWithPreview?cmsSliderId=" + cmsSliderId + "&targetPublishState=" + publishStateFormData + "&portalId=0&localeId=0&takefromdraftfirst=" + true;
    }

    PublishPopup(): any {
        $("#SliderPublishPopup").modal('show')
    }

    DeleteBanners(control): any {
        var cmsSliderBannerId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsSliderBannerId.length > 0) {
            Endpoint.prototype.DeleteBanners(cmsSliderBannerId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteLinkWidgetConfiguration(control): any {
        var cmsWidgetTitleConfigurationId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsWidgetTitleConfigurationId.length > 0) {
            Endpoint.prototype.DeleteLinkWidgetConfiguration(cmsWidgetTitleConfigurationId, null, function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }
    SaveWebSiteLogo(response: any): any {
        if (response.Url !== "")
            window.location.href = response.Url;
        else
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    SaveDisplaySetting(response: any): void {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    SaveWidgetConfiguration(response: any): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        if (response.widgetConfigurationId > 0) {
            $("#CMSWidgetConfigurationId").val(response.widgetConfigurationId);
        }
    }
    GetAddContentPageResult(response: any) {
        if (response.status) {
            window.location.href = "/WebSite/EditStaticPage?cmsContentPagesId=" + response.cmsContentPagesId;
        }
        ZnodeBase.prototype.RemovePopupOverlay();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
    }

    DeleteStaticPage(control): any {
        var cmsContentPagesId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsContentPagesId.length > 0) {
            Endpoint.prototype.DeleteStaticPage(cmsContentPagesId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ValidateAddContentSubmit(): any {
        var pageName = $("#PageName").val();
        var pageTitle = $("#PageTitle").val();
        if (pageName == "") {
            $("#valPageName").html(ZnodeBase.prototype.getResourceByKeyName("PageNameRequired"));
            return false;
        }
        else {
            $("#valPageName").html("");
        }

        if (pageTitle == "") {
            $("#valPageTitle").html(ZnodeBase.prototype.getResourceByKeyName("PageTitleRequired"));
            return false;
        }
        else {
            $("#valPageTitle").html("");
        }

    }

    DeleteManageMessages(control): any {
        var cmsMessageId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsMessageId.length > 0) {
            Endpoint.prototype.DeleteManageMessageForWebsite(cmsMessageId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    RemovePopupOverlay(): any {
        //Below code is used to close te overlay of popup, As it was not closed in server because Container is updated by Ajax call
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    }

    GetPDPTemplates(): any {
        var templates = [];
        $(".PDPTemplateNameId").each(function () {
            var pdpTemplate = $(this).attr("data-producttype") + $(this).val();
            templates.push(pdpTemplate);
        });
        $("#Templates").val(templates);
    }

    AssociateProduct(cmsWidgetsId: any, cmsMappingId: any, widgetsKey: any, typeOfMapping: any, displayName: string, widgetName: string, fileName: string, localeId: any): any {
        var SKUs = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (SKUs.length > 0) {
            Endpoint.prototype.AssociateProduct(cmsWidgetsId, cmsMappingId, widgetsKey, typeOfMapping, SKUs, function (res) {
                $("#associateproductlist").hide(700);
                if (typeOfMapping == 'ContentPageMapping') {
                    if (res.status) {
                        window.location.href = window.location.protocol + "//" + window.location.host + "/Content/EditContentPage?cmsContentPagesId=" + cmsMappingId + "&fileName=" + fileName + "&localeId=" + localeId + "&isFromWebSiteController=false";
                    }
                    else {
                        WebSite.prototype.RefreshGridContent("associatedPanel");
                        ZnodeBase.prototype.RemovePopupOverlay();
                        if ($("#associatedPanel").css('display') == "block") {
                            WebSite.prototype.ShowOverlay('associatedPanel');
                        }
                        WebSite.prototype.DisplayNotification(res.message, 'error');
                    }
                }
                else {
                    ZnodeBase.prototype.RemovePopupOverlay();
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                    window.location.href = window.location.protocol + "//" + window.location.host + "/WebSite/GetAssociatedProductList?cmsWidgetsId=" + cmsWidgetsId + "&cmsMappingId=" + cmsMappingId + "&widgetKey=" + widgetsKey + "&typeOfMapping=" + typeOfMapping + "&widgetName=" + widgetName + "&displayName=" + displayName + "&fileName=" + fileName;
                }
            });
        }
        else {
            $("#associateproductlist").show(700);
            $("#UnAssociatedProductList").show();
            $("#CMSProductAssociatedSuccessMessage").hide();
        }
    }

    RefreshGridContent(targetDiv): any {
        if ($("#hdnPageRefreshUrl") != undefined && $("#hdnPageRefreshUrl").val() != "") {
            var url = $("#hdnPageRefreshUrl").val();
            Endpoint.prototype.GetPartial(url, function (response) {
                $("#" + targetDiv).html('');
                $("#" + targetDiv).append(response);
                GridPager.prototype.Init();
                DynamicGrid.prototype.ClearCheckboxArray();
            });
        }
    }

    ShowNotification(): any {
        $("#CMSProductAssociatedSuccessMessage").show();
        $("#CMSProductAssociatedSuccessMessage").addClass('alert-info');
        $("#CMSProductAssociatedSuccessMessage").html(ZnodeBase.prototype.getResourceByKeyName("CMSNotificationForConfigure"));
        $("#CMSProductAssociatedSuccessMessage").fadeOut(10000);
    }

    DisplayNotification(message: string, type: string): any {
        $("#CMSProductAssociatedSuccessMessage").show();
        switch (type) {
            case "success":
                {
                    $("#CMSProductAssociatedSuccessMessage").addClass('alert-success');
                    break;
                }
            case "error":
                {
                    $("#CMSProductAssociatedSuccessMessage").addClass('alert-danger');
                    break;
                }
            default:
                {
                    $("#CMSProductAssociatedSuccessMessage").addClass('alert-info');
                }
        }
        $("#CMSProductAssociatedSuccessMessage").html(message);
        $("#CMSProductAssociatedSuccessMessage").fadeOut(10000);
    }

    UnAssociateProduct(control, cmsMappingId: any, fileName: string, localeId: any): any {
        var cmsWidgetProductId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsWidgetProductId.length > 0) {
            Endpoint.prototype.UnAssociateProduct(cmsWidgetProductId, function (res) {
                if (res.status) {
                    if (fileName.toLowerCase() === 'widgets') {
                        window.location.href = window.location.protocol + "//" + window.location.host + "/Content/EditContentPage?cmsContentPagesId=" + cmsMappingId + "&fileName=" + fileName + "&localeId=" + localeId + "&isFromWebSiteController=false";
                    }
                    else
                        window.location.reload();
                }
                else {
                    WebSite.prototype.DisplayNotification(res.message, res.status ? 'success' : 'error');
                    DynamicGrid.prototype.RefreshGridOndelete(control, res);
                }
            });
        }
    }

    AssociateCategories(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string) {
        var categoryCodes = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (categoryCodes.length > 0) {
            Endpoint.prototype.AssociateCategoriesForWebSite(cmsWidgetsId, cmsMappingId, widgetKey, typeOFMapping, categoryCodes, function (res) {
                $("#associateCategoryList").hide(700);
                if (typeOFMapping == 'ContentPageMapping') {
                    $("#CMSProductAssociatedSuccessMessage").show();
                    $("#CMSProductAssociatedSuccessMessage").html(ZnodeBase.prototype.getResourceByKeyName("CMSNotificationForConfigure"));
                    $("#CMSProductAssociatedSuccessMessage").fadeOut(10000);
                    Content.prototype.RefreshGridContent("associatedPanel");
                }
                else {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/WebSite/GetAssociatedCategoryList?cmsWidgetsId=" + cmsWidgetsId + "&cmsMappingId=" + cmsMappingId + "&widgetKey=" + widgetKey + "&typeOFMapping=" + typeOFMapping + "&widgetName=" + widgetName + "&displayName=" + displayName + "&fileName=" + fileName;
                }
            });
        }
        else {
            $('#AssociateError').show();
        }
    }

    //Delete associated categories for website.
    DeleteAssociatedCategories(control): any {
        var cmsWidgetCategoryId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsWidgetCategoryId.length > 0) {
            Endpoint.prototype.RemoveAssociatedCategories(cmsWidgetCategoryId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    GetUnassociatedCategory(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string): any {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetUnassociatedCategory(cmsWidgetsId, cmsMappingId, widgetKey, typeOFMapping, displayName, widgetName, fileName, function (res) {
            if (res != null && res != "") {
                $("#associateCategoryList").html(res);
                $("#associateCategoryList").show(700);
                $("body").css('overflow', 'hidden');
                ZnodeBase.prototype.HideLoader();
            }
        });
    }

    AssociateBrands(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string) {
        var brandId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (brandId.length > 0) {
            Endpoint.prototype.AssociateBrandsForWebSite(cmsWidgetsId, cmsMappingId, widgetKey, typeOFMapping, brandId, function (res) {
                $("#associateBrandList").hide(700);
                if (typeOFMapping == 'ContentPageMapping') {
                    $("#CMSProductAssociatedSuccessMessage").show();
                    $("#CMSProductAssociatedSuccessMessage").html(ZnodeBase.prototype.getResourceByKeyName("CMSNotificationForConfigure"));
                    $("#CMSProductAssociatedSuccessMessage").fadeOut(10000);
                    Content.prototype.RefreshGridContent("associatedPanel");
                }
                else {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/WebSite/GetAssociatedBrandList?cmsWidgetsId=" + cmsWidgetsId + "&cmsMappingId=" + cmsMappingId + "&widgetKey=" + widgetKey + "&typeOFMapping=" + typeOFMapping + "&widgetName=" + widgetName + "&displayName=" + displayName + "&fileName=" + fileName;
                }
            });
        }
        else {
            $('#AssociateError').show();
        }
    }

    //Delete associated brands for website.
    DeleteAssociatedBrands(control): any {
        var cmsWidgetBrandId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (cmsWidgetBrandId.length > 0) {
            Endpoint.prototype.RemoveAssociatedBrands(cmsWidgetBrandId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    GetUnassociatedBrand(cmsWidgetsId: number, cmsMappingId: number, widgetKey: string, typeOFMapping: string, displayName: string, widgetName: string, fileName: string): any {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.GetUnassociatedBrand(cmsWidgetsId, cmsMappingId, widgetKey, typeOFMapping, displayName, widgetName, fileName, function (res) {
            if (res != null && res != "") {
                $("#associateBrandList").html(res);
                $("#associateBrandList").show(700);
                $("body").css('overflow', 'hidden');
                ZnodeBase.prototype.HideLoader();
            }
        });
    }

    AddLinkWidget(data: any) {
        data = 0;
        WebSite.prototype.GetAddLinkWidgetConfiguration(data);
        $(".MessageBox").remove();
        $("#partial").show();
        $('.thead-div').show();
    }

    GetAddLinkWidgetConfiguration(data: any) {
        data = 0;
        var localeId = $("#ddlCultureSpan").attr("data-value");
        localeId = localeId == undefined ? $("#ddl_locale_list_content_pages").val() : localeId;
        Endpoint.prototype.GetAddLinkWidgetConfiguration($("#CMSWidgetsId").val(), $("#CMSMappingId").val(), $("#WidgetsKey").val(), $("#TypeOfMapping").val(), $("#DisplayName").val(), $("#WidgetName").val(), $("#FileName").val(), parseInt(localeId), function (response) {
            $("#partial").html(response.html);
            WebSite.prototype.CreateEditLinkWidgetShowHideFormAttributes(data);
        });
    }

    EditLinkWidget(data: any) {
        WebSite.prototype.CreateEditLinkWidgetShowHideFormAttributes(data);
    }

    CreateEditLinkWidgetShowHideFormAttributes(data) {
        $("#saveMedia_" + data + "").show();
        $("#CancelMedia_" + data + "").show();
        $("#EditMedia_" + data + "").attr("style", "display: none !important");
        $("#deleteMedia_" + data + "").attr("style", "display: none !important");
        $("#Media_" + data + "").removeClass("disable-image");
        $("#mediaTitle_" + data + "").attr('disabled', false);
        $("#mediaDisplayOrder_" + data + "").attr('disabled', false);
        var displayOrder = parseInt($("#mediaDisplayOrder_" + data + "").attr("Value"));
        var displayOrderValue = displayOrder > 0 ? displayOrder : ZnodeBase.prototype.getResourceByKeyName("DefaultDisplayOrderValue");
        $("#mediaDisplayOrder_" + data + "").val(displayOrderValue);
        $("#mediaUrl_" + data + "").attr('disabled', false);
        $("#mediaIsNewTab_" + data + "").attr("disabled", false)
        if (data > 0 && $("#Media_" + data + "").attr("src") != "/MediaFolder/no-image.png") {
            if ($("#uploadImageCloseId_" + data + "").children().length > 0) {
                $("#uploadImageCloseId_" + data + "").children().addClass('z-close-circle');
            }
            else {
                $("#uploadImageCloseId_" + data + "").addClass('z-close-circle');
            }
            $("#uploadImageCloseId_" + data + "").show();
        }
        $("#spanChangeImage_" + data + "").show();
    }

    DeleteLinkWidget(data: any, localeId: any, control: any) {
        Endpoint.prototype.DeleteLinkWidgetConfiguration(data, localeId, function (response) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            if (response.status) {
                $(control).closest("form").remove();
                //  window.location.href = window.location.protocol + "//" + window.location.host + "/WebSite/GetLinkWidgetConfigurationList?cmsMappingId=" + $('#CMSMappingId').val() + "&cmsWidgetsId=" + $('#CMSWidgetsId').val() + "&widgetsKey=" + $('#WidgetsKey').val() + "&typeOFMapping=" + $('#TypeOfMapping').val() + "&displayName=" + $('#DisplayName').val() + "&widgetName=" + $('#WidgetName').val() + "&fileName=" + $('#FileName').val() + "&localeId=" + $('#LocaleId').val();
            }
            WebSite.prototype.DisplayNoRecordFoundMessage();
        });
    }

    CancelNewAddLinkWidget(data: any, control: any) {
        var mediaId = data.split('_')[1];
        if (mediaId <= 0) {
            $(control).closest("form").remove();
            WebSite.prototype.DisplayNoRecordFoundMessage();
        }
        else {
            WebSite.prototype.LinkWidgetListCancel(mediaId);
        }
    }

    ShowOverlayOnCancel(divName) {
        ZnodeBase.prototype.CancelUpload(divName);
        $("body").css('overflow', 'hidden');
        $('body').append("<div class='modal-backdrop fade in'></div>");
    }

    ShowOverlay(divName) {
        $('body').append("<div class='modal-backdrop fade in'></div>");
    }

    LinkWidgetListCancel(cmsWidgetTitleConfigurationId: any) {
        $("#saveMedia_" + cmsWidgetTitleConfigurationId + "").attr("style", "display: none !important");
        $("#CancelMedia_" + cmsWidgetTitleConfigurationId + "").attr("style", "display: none !important");
        $("#EditMedia_" + cmsWidgetTitleConfigurationId + "").show();
        $("#deleteMedia_" + cmsWidgetTitleConfigurationId + "").show();
        $("#Media_" + cmsWidgetTitleConfigurationId + "").addClass("disable-image");
        $("#divMedia_" + cmsWidgetTitleConfigurationId + "").each(function () {
            $(this).find(".upload-images-close").hide();
        });
        $("#divMedia_" + cmsWidgetTitleConfigurationId + "").find(".upload-images-close").first().attr('id', "uploadImageCloseId_" + cmsWidgetTitleConfigurationId);
        $("#uploadImageCloseId_" + cmsWidgetTitleConfigurationId + "").hide();
        $("#mediaTitle_" + cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaDisplayOrder_" + cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaUrl_" + cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaIsNewTab_" + cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#spanChangeImage_" + cmsWidgetTitleConfigurationId + "").hide();
    }

    LinkWidgetAddResult(data: any, control: any) {
        var controlId = $(control).closest("form").attr("id");
        var id = controlId.split('_')[1];
        if ($("#TypeOfMapping").val() == 'ContentPageMapping') {
            $(".cms-pages-notification").show();
            $("#divmessage").html(data.message);
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(data.message, data.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        }
        if (data.cmsWidgetTitleConfigurationId > 0 && data.status == true) {
            if (parseInt(id) == 0) {
                $("#partial").after(data.html);
                $("#partial").html("");
            }
            WebSite.prototype.LinkWidgetListShowHideFormAttributes(data);
        }
        else {
            Endpoint.prototype.GetLinkWidgetConfigurationList($("#CMSWidgetsId").val(), $("#CMSMappingId").val(), $("#WidgetsKey").val(), $("#TypeOfMapping").val(), $("#DisplayName").val(), $("#WidgetName").val(), $("#FileName").val(), null, function (res) {
                $(".Show").html(res);
                $(".Show").show();
            });
        }
        $(control).closest("form").removeClass('dirty valid');
        $("#" + controlId + " input.dirty").removeClass('dirty valid');
        $(".cms-pages-notification").delay(5000).fadeOut(300);
    }

    LinkWidgetListShowHideFormAttributes(data) {
        $("#saveMedia_" + data.cmsWidgetTitleConfigurationId + "").attr("style", "display: none !important");
        $("#CancelMedia_" + data.cmsWidgetTitleConfigurationId + "").attr("style", "display: none !important");
        $("#EditMedia_" + data.cmsWidgetTitleConfigurationId + "").show();
        $("#deleteMedia_" + data.cmsWidgetTitleConfigurationId + "").show();
        $("#Media_" + data.cmsWidgetTitleConfigurationId + "").addClass("disable-image");
        $("#divMedia_" + data.cmsWidgetTitleConfigurationId + "").each(function () {
            $(this).find(".upload-images-close").hide();
        });
        $("#divMedia_" + data.cmsWidgetTitleConfigurationId + "").find(".upload-images-close").first().attr('id', "uploadImageCloseId_" + data.cmsWidgetTitleConfigurationId);
        $("#uploadImageCloseId_" + data.cmsWidgetTitleConfigurationId + "").hide();
        $("#mediaTitle_" + data.cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaDisplayOrder_" + data.cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaUrl_" + data.cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#mediaIsNewTab_" + data.cmsWidgetTitleConfigurationId + "").attr('disabled', 'disabled');
        $("#spanChangeImage_" + data.cmsWidgetTitleConfigurationId + "").hide();
    }

    LocaleDropDownChange() {
        $("#ddl_locale_list").on("change", function () {
            var mappingId = $("#CMSMappingId").val();
            var widgetId = $("#CMSWidgetsId").val();
            var widgetKey = $("#WidgetsKey").val();
            var mappingType = $("#TypeOFMapping").val();
            var displayName = $("#DisplayName").val();
            var widgetName = $("#WidgetName").val();
            var fileName = $("#FileName").val();
            var localeId = $("#ddl_locale_list").val();
            Endpoint.prototype.ManageTextWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId, function (response) {
                $('#div_text_widget').html(response);
                $("#div_text_widget textarea").attr("wysiwygenabledproperty", "true");
                reInitializationMce();
            });
        });

        $("#ddl_locale").on("change", function () {
            var mappingId = $("#CMSMappingId").val();
            var widgetId = $("#CMSWidgetsId").val();
            var widgetKey = $("#WidgetsKey").val();
            var mappingType = $("#TypeOFMapping").val();
            var displayName = $("#DisplayName").val();
            var widgetName = $("#WidgetName").val();
            var fileName = $("#FileName").val();
            var localeId = $("#ddl_locale").val();
            Endpoint.prototype.ManageFormWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId, function (response) {
                $('#div_form_widget').html(response);
                reInitializationMce();
            });
        });

        $("#ddl_SearchWidget_locale_list").on("change", function () {
            var mappingId = $("#CMSMappingId").val();
            var widgetId = $("#CMSWidgetsId").val();
            var widgetKey = $("#WidgetsKey").val();
            var mappingType = $("#TypeOFMapping").val();
            var displayName = $("#DisplayName").val();
            var widgetName = $("#WidgetName").val();
            var fileName = $("#FileName").val();
            var localeId = $("#ddl_SearchWidget_locale_list").val();
            Endpoint.prototype.ManageSearchWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, displayName, widgetName, fileName, localeId, function (response) {
                $('#div_Search_widget').html(response);
            });
        });
    }

    DdlCultureChange() {
        var localeId = $("#ddlCultureSpan").attr("data-value");
        var localeText = $("#ddlCultureSpan").text;
        if ($('#divAddLinkWidget').length === 1) {
            var controlId = $(this).attr('id');
            var mappingId = $("#CMSMappingId").val();
            var widgetId = $("#CMSWidgetsId").val();
            var widgetKey = $("#WidgetsKey").val();
            var mappingType = $("#TypeOfMapping").val();
            var displayName = $("#DisplayName").val();
            var widgetName = $("#WidgetName").val();
            var fileName = $("#FileName").val();
            Endpoint.prototype.GetLinkWidgetConfigurationList(widgetId, mappingId, widgetKey, mappingType, displayName, widgetName, fileName, parseInt(localeId), function (res) {
                $(".body-wrapper").html(res);
            });
        }
        else {
            Endpoint.prototype.EditBanner($("#CMSSliderBannerId").val(), parseInt(localeId), function (response) {
                $('#grid-container').html(response);
                $("#grid-container textarea").attr("wysiwygenabledproperty", "true");
                reInitializationMce();
            });
        }
    }

    DisplayNoRecordFoundMessage() {
        if (!$("#Show").has("form").length) {
            $('.thead-div').hide();
            $("#partial").after("<div class=\"MessageBox\"><p class=\"text-center\">" + ZnodeBase.prototype.getResourceByKeyName("NoResult") + "</p></div>");
        }
    }

    ValidationAutoPlayTime() {
        if ($("input[name = AutoPlay]:checked").val() == "true") {
            $("#LabelAutoPlay").addClass('required');
        } else {
            $("#LabelAutoPlay").removeClass('required');
        }

        if ($("input[name = AutoPlay]:checked").val() == "false") {
            return true;
        }
        else if ($("#AutoplayTimeOut").val() == "") {

            $("#AutoplayTimeOut").closest("div").parent().find("span").text(ZnodeBase.prototype.getResourceByKeyName("AutoplayTimeOutRequired"));
            $("#AutoplayTimeOut").closest("div").parent().find("span").show();
            $("#AutoplayTimeOut").closest("div").parent().find("span").prop("class", "error-msg");
            return false;
        }
        return true;
    }

    AutplayRequiredOnClick(): any {
        var _autoplayTimeOut = $('#AutoplayTimeOut').val();
        $(".switchAutoPlay").find("label").click(function () {
            if ($(this).attr('id') == "AutoPlay_true") {
                $("#divAutoPlayTimeout").show();
                $("#LabelAutoPlay").addClass('required');
                $('#AutoplayTimeOut').val(_autoplayTimeOut);
            }
            else if ($(this).attr('id') == "AutoPlay_false") {
                $("#LabelAutoPlay").removeClass('required');
                $("#divAutoPlayTimeout").hide();
                $('#AutoplayTimeOut').val('');
            }
        });
    }
    ValidateBannerSlider(object): any {
        var isValid = true;
        if ($(object).val() == '' || $(object).val() == 0) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("NameIsRequired"), 'error', isFadeOut, fadeOutTime);
            $(object).addClass("input-validation-error");
            isValid = false;
        }
        else if ($(object).val().length > 100) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSliderLengthName"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (!WebSite.prototype.IsSliderNameExist(object)) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistBannerName"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }
    IsSliderNameExist(object): any {
        var isValid = false;
        var cmsSliderId = $(object).parent().parent().find("td.grid-checkbox").find("input").attr("id");
        if (cmsSliderId != undefined && cmsSliderId.length > 0) {
            cmsSliderId = cmsSliderId.split("_")[1];
            Endpoint.prototype.IsSliderNameExist($(object).val().trim(), parseInt(cmsSliderId), function (response) {
                isValid = response;
            });
        }
        return isValid;
    }

    //check if the entered banner sequence is a number and within sequence
    ValidateBannerSequenceField(object): any {
        var regex = new RegExp('^\\d{0,}?$');
        var isValid = true;
        if (isNaN($(object).val()) || $(object).val() == '' || $(object).val() == 0) {
            $(object).addClass("input-validation-error");
            if (isNaN($(object).val()))
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("InvalidBannerSequence"), 'error', isFadeOut, fadeOutTime);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("BannerSequenceRange"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (!regex.test($(object).val())) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("BannerSequenceRange"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }

    RadioChangeEvent(control): any {
        $("#TextMessageLable").toggle();
        $("#RedirectURLLable").toggle();
        if ($(control).attr("id") == "RadioIsDefault")
            $("#IsTextMessage").val("true");
        else
            $("#IsTextMessage").val("false");
    }

    //This method is used to get email template list on aside panel
    GetEmailTemplateList(emailType: string): any {
        $("#CheckEmailType").val(emailType);
        $('#divEmailTemplatelistPopup').show();
        $('.modal-dialog.modal-xl.modal-xxl').append("<div class='modal-backdrop fade in'></div>");
        ZnodeBase.prototype.BrowseAsidePoupPanel('/Website/EmailTemplateList?checkEmailType=' + emailType, 'divEmailTemplatelistPopup');
    }

    GetEmailTemplateDetail(): void {
        $("#ZnodeFormWidgetEmailTemplate").find("tr").click(function () {
            let emailTemplateName: string = "";
            let emailTemplateId: string = "";
            if ($("#CheckEmailType").val() == "NotificationEmailTemplate") {
                emailTemplateName = $(this).find("td[class='emailtemplatenamecolumn']").text();
                emailTemplateId = $(this).find("td")[0].innerHTML;
                $('#txtNotificationEmailTemplate').val(emailTemplateName);
                $('#hdnNotificationEmailTemplateId').val(emailTemplateId);
                $("#errorRequiredNotificationEmailTemplate").text("").removeClass("field-validation-error").hide();
                $("#txtNotificationEmailTemplate").removeClass('input-validation-error');
                $('#divEmailTemplatelistPopup').hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
            }
            else if ($("#CheckEmailType").val() == "AcknowledgementEmailTemplate") {
                emailTemplateName = $(this).find("td[class='emailtemplatenamecolumn']").text();
                emailTemplateId = $(this).find("td")[0].innerHTML;
                $('#txtAcknowledgementEmailTemplate').val(emailTemplateName);
                $('#hdnAcknowledgementEmailTemplateId').val(emailTemplateId);
                $("#errorRequiredAcknowledgementEmailTemplate").text("").removeClass("field-validation-error").hide();
                $("#txtAcknowledgementEmailTemplate").removeClass('input-validation-error');
                $('#divEmailTemplatelistPopup').hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
            }
        });
    }

    ValidateFormWidget(): any {
        var formName = $("#FormTitle").val();
        if (formName == null || formName == undefined || formName == "") {
            $("#divFormWidgetError").show();
            $("#divFormWidgetError").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFormNameRequired"));
            $("#divFormWidgetError").fadeOut(5000);
        }
    }

    SaveFormWidget(data: any, control: any) {
        if ($("#FormTitle").val() == undefined || $("#FormTitle").val() == '') {
            $("#divFormWidgetError").show();
            $("#divFormWidgetError").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFormNameRequired"));
            $("#divFormWidgetError").fadeOut(5000);
            return false;
        }
        if (!control.baseURI.includes("fileName")) {
            window.location.href = control.baseURI + "&fileName=Widgets";
        }
        else {
            window.location.href = control.baseURI.split('#')[0];
        }
    }

    SaveEmailWidget(data: any, control: any) {
        if ($("#hdnFormTitle").val() == undefined || $("#hdnFormTitle").val() == '') {
            $("#divFormWidgetError").show();
            $("#divFormWidgetError").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFormNameRequired"));
            $("#divFormWidgetError").fadeOut(5000);
            return false;
        }
        if (!control.baseURI.includes("fileName")) {
            window.location.href = control.baseURI + "&fileName=Widgets";
        }
        else {
            window.location.href = control.baseURI.split('#')[0];
        }
    }

    //This method is used check validation when user insert notification email
    ValidateNotificationEmail(): boolean {
        if ($("#hdnFormTitle").val() == undefined || $("#hdnFormTitle").val() == '') {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorFormNameRequired"), "error", isFadeOut, fadeOutTime);
            $("#divFormWidgetError").show();
            $("#divFormWidgetError").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFormNameRequired"));
            $("#divFormWidgetError").fadeOut(5000);
            return false;
        }
        if ($("#txtNotificationEmail").val() != null && $("#txtNotificationEmail").val() != "") {

            if ($("#hdnNotificationEmailTemplateId").val() == null || $("#hdnNotificationEmailTemplateId").val() == "" || $("#hdnNotificationEmailTemplateId").val() == "0") {
                $("#errorRequiredNotificationEmailTemplate").text('').text(ZnodeBase.prototype.getResourceByKeyName("NotificationEmailTemplate")).addClass("field-validation-error").show();
                $("#txtNotificationEmailTemplate").addClass('input-validation-error');
                return false;
            }
            else {
                return true;
            }
        }
        if ($("#txtNotificationEmailTemplate").val() != null && $("#txtNotificationEmailTemplate").val() != "") {

            if ($("#txtNotificationEmail").val() == null || $("#txtNotificationEmail").val() == "" || $("#txtNotificationEmail").val() == "0") {
                $("#errorRequiredNotificationEmailId").text('').text(ZnodeBase.prototype.getResourceByKeyName("NotificationEmailId")).addClass("field-validation-error").show();
                $("#txtNotificationEmail").addClass('input-validation-error');
                return false;
            }
            else {
                return true;
            }
        }
        else {

            $("#errorRequiredNotificationEmailTemplate").text("").removeClass("field-validation-error").hide();
            $("#txtNotificationEmailTemplate").removeClass('input-validation-error');
            $("#errorRequiredNotificationEmailId").text("").removeClass("field-validation-error").hide();
            $("#txtNotificationEmail").removeClass('input-validation-error');
            return true;
        }
    }

    // set the container details 
    SetContainerWidgetConfiguration(): any {
        $("#grid").find("tr").on("click", function (e) {
            e.preventDefault();
            var containerKey = $(this).find("td")[0].innerHTML;
            $("input[id=ContainerKey]").val(containerKey);
            $("#formCmscontainerWidgetConfiguration").submit();
        });
    }
}