class Theme extends ZnodeBase {
    _endPoint: Endpoint;
    unsaved: any;
    constructor() {
        super();
    }
    Init() {
        Theme.prototype.DownloadCSS();
        Theme.prototype.ValidateCSSFile();
        Theme.prototype.ValidateZipFile();
        Theme.prototype.GetFileName();
        Theme.prototype.RemoveParentThemeValidationRule();
    }

    //Validate theme to check theme already exists or not.
    ValidateThemeName(): boolean {
        var isValid = true;
        var name: string = $("#Name").val();
        if (name != '') {
            Endpoint.prototype.IsThemeNameExist(name, $("#CMSThemeId").val(), function (response) {
                if (!response) {
                    $("#Name").addClass("input-validation-error");
                    $("#errorSpanThemeName").addClass("error-msg");
                    $("#errorSpanThemeName").text(ZnodeBase.prototype.getResourceByKeyName("AlreadyExistThemeName"));
                    $("#errorSpanThemeName").show();
                    isValid = false;
                }
            });
        }
        return isValid;
    }

    //Handles the Parent Theme checkbox change event.
    IsParentThemeToggleCallback(checked, target): void {
        if (target)
            if (checked === true) {
                target.hide();
                Theme.prototype.RemoveParentThemeValidationRule();
            }
            else {
                target.show();
                Theme.prototype.AddParentThemeValidationRule();
            }
    }

    RemoveParentThemeValidationRule(): void {
        $("#ParentThemeId").rules("remove", 'required');
    }

    AddParentThemeValidationRule(): void {
        $("#ParentThemeId").rules("add", 'required');
    }

    //Get file name.
    GetFileName(): void {
        if ($("#CMSThemeId").val() > 0) {
            $("#txtUpload").attr("title", $("#Name").val());
            var filename = $("#fileName").attr("title");
            $('#fileName').text(filename);
        }
    }

    UnSavedChanges(): any {
        Theme.prototype.unsaved = false;
        $("#frmThemeAsset :input").on("change", function () {
            Theme.prototype.unsaved = true;
        });
    }

    CheckChangeInForm(): any {
        if (Theme.prototype.unsaved) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorSaveChanges"), 'error', isFadeOut, fadeOutTime);
            return !Theme.prototype.unsaved;
        }
    }

    GetUnAssociatedStoreList(PriceListId: number): any {
        Endpoint.prototype.GetUnAssociatedStoreListForCMS(PriceListId, function (res) {
            if (res != null && res != "") {
                $("#associatestorelist").html(res);

                if ($("#modelAssociatedStore").find("tr").length == 0) {
                    $("#modelAssociatedStore").find(".modal-footer").hide();
                    $("#modelAssociatedStore").find(".filter-component").hide();
                }
                $("#associatestorelist").show();
            }
        });
    }

    AssociateCMSThemeStore(priceListId: number) {
        var storeIds = "";
        var storeIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (storeIds.length > 0) {
            Endpoint.prototype.AssociateStoreListForCMS(priceListId, storeIds, function (res) {
                $("#associatestorelist").hide();
                Endpoint.prototype.GetAssociatedStoreList(priceListId, function (response) {
                    $("#associatedassets").html('');
                    $("#associatedassets").html(response);
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                    ZnodeBase.prototype.RemovePopupOverlay();
                });
            });
        }
        else {
            $("#associatestorelist").hide();
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneStore"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DeleteAssociatedThemeStores(control): any {
        var priceListPortalId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (priceListPortalId.length > 0) {
            Endpoint.prototype.DeleteAssociatedStoresForCMS(priceListPortalId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    GetCMSAreaWidgets(cmsThemeId): any {
        var cmsAreaId = $("#areaId :selected").val();
        Endpoint.prototype.GetCMSAreaWidgets(cmsThemeId, cmsAreaId, function (response) {
            $("#widgetsDivId").html('');
            $("#widgetsDivId").html(response);
        });
    }

    GetMultipleSelectedIds() {
        var ids = [];
        $.each($("input[name='CMSAreaWidgetsData.WidgetIds']:checked"), function () {
            if ($(this).length > 0) {
                if ($(this).is(":checked")) {
                    var id = $(this).attr("value");
                    ids.push(id);
                }
            }
        });
        var result = [];
        $.each(ids, function (i, e) {
            if ($.inArray(e, result) == -1) result.push(e);
        });
        return result.join();
    }

    CheckCheckbox(): any {
        var widgetIds = Theme.prototype.GetMultipleSelectedIds();
        if (widgetIds.length > 0) {
            $("#frmCreateAreaWidget").submit();
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneCheckbox"), 'error', isFadeOut, fadeOutTime);
            $("#frmCreateAreaWidget").off("submit");
            return false;
        }
    }

    DeleteMultipleTheme(control, themePropertyName): any {
        var cmsThemeId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var cmsThemeName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn(themePropertyName);

        if (cmsThemeId.length > 0 || cmsThemeName != "") {
            Endpoint.prototype.DeleteTheme(cmsThemeId, cmsThemeName, function (response) {
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }

    DeleteRevisedTheme(control, themePropertyName): any {
        var cmsThemeId = $("#CMSThemeId").val();
        var cmsThemeName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn(themePropertyName);

        if (cmsThemeName != "") {
            Endpoint.prototype.DeleteRevisedTheme(cmsThemeId, cmsThemeName, function (response) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                DynamicGrid.prototype.RefreshGridOndelete(control, response);      
                ZnodeBase.prototype.RemovePopupOverlay();         
            });
        }
    }

    DeleteMultipleCss(control, cssPropertyName): any {
        var cmsThemeCssId = DynamicGrid.prototype.GetMultipleSelectedIds();
        var cssName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn(cssPropertyName);
        var themeName = $("#CMSThemeName").val();
        if (cmsThemeCssId.length > 0 || cssName != "") {
            Endpoint.prototype.DeleteCss(cmsThemeCssId, cssName, themeName, function (response) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                DynamicGrid.prototype.RefreshGridOndelete(control, response);
            });
        }
    }

    GetWidgetResult(data: any) {
        var cmsThemeId = $("input[name=CMSThemeId]", $(this).parent()).val();
        var message = JSON.parse(data.Message);
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(message.Message, message.Type, isFadeOut, fadeOutTime);
        Endpoint.prototype.GetCMSAreas(cmsThemeId, function (response) {
            $("#associatedassets").html('');
            $("#associatedassets").html(response);
        });
    }

    SaveThemeAsset(response: any): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
        Theme.prototype.unsaved = false;
    }

    GetPDPAssets(): any {
        var assets = [];
        $(".pdpassets").each(function () {
            var pdpAsset = $(this).children().children("#assets").attr("data-producttype") + "_" + $(this).children().children("#CMSAssetId").find(":selected").val();
            assets.push(pdpAsset);
        });
        $("#Assets").val(assets);
    }

    DownloadCSS(): any {
        $("#themecsslist .z-download").on("click", function (e) {
            e.preventDefault();
            var CMSThemeId = $(this).attr("data-parameter").split("?")[1].split('&')[0].split('=')[1];
            var CSSName = $(this).attr("data-parameter").split("&")[1].split("=")[1];
            var themeName = $("#Name").val();
            window.location.href = "/Theme/DownloadCSS?CMSThemeId=" + CMSThemeId + "&CSSName=" + CSSName + "&themeName=" + themeName + "";
        });
    }

    ValidateCSSFile(): any {
        $(document).on('change', '#txtUpload', function () {
            Theme.prototype.ValidateCSSFileType();
        });
    }

    ValidateCSSFileType(): boolean {
        var isCssFile: boolean = false;
        var regex: RegExp = new RegExp("^[^'\"{}]+$");
        if ($("#txtUpload").val() != "") {
            $.each($("#fileName").html().split(","), function (index, item) {
                if (item.split(".")[1] == "css" || item.split(".")[2] == "css") {
                    if (!regex.test(item)) {
                        $("#errInvalidType").html(ZnodeBase.prototype.getResourceByKeyName("ErrorCssInvalidFileName"));
                    } else {
                        $("#errInvalidType").html("");
                        isCssFile = true;
                    }
                    if (isCssFile) { return isCssFile; } else { $("#errInvalidType").html(ZnodeBase.prototype.getResourceByKeyName("ErrorCssInvalidFileName")); return isCssFile; }
                }
                else {
                    $("#errInvalidType").html(ZnodeBase.prototype.getResourceByKeyName("SelectCSSFileError"));
                    return isCssFile;
                }
            });
        }
        return true;
    }

    ValidateZipFile(): any {
        $(document).on('change', '#txtUpload', function () {
            Theme.prototype.ValidateZipFileType();
        });
    }

    ValidateZipFileType(): boolean {
        //Validate theme to check theme already exists or not.
        var isExists: boolean = Theme.prototype.ValidateThemeName();
       
        var fileName = $("#fileName").text();
        if ($("#txtUpload").val() != "") {
            if ($('#txtUpload').val().split('.').pop().toLowerCase() == "zip") {
                $("#errZipFileType").html("");
                if (!isExists)
                    return false;
                return true;
            }
            else {
                $("#errZipFileType").html(ZnodeBase.prototype.getResourceByKeyName("SelectZipFileError"));
                return false;   
            }
        }
        else {
            $("#errZipFileType").text('');
            if (!isExists)
                return false;
            return true;
        }
    }


    HideLoader(): any {
        $("#loading-div-background").hide();
    }

    Reload(): any {
        setTimeout(function (): any { ZnodeBase.prototype.HideLoader() }, 1000);
    }
}
