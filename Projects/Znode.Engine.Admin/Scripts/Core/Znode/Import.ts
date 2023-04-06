class Import extends ZnodeBase {
    _endPoint: Endpoint;

    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init() {
        $('#updateMappings').hide();
        $(document).on("click", "#SourceColumnList li", function (e) {
            Import.prototype.AppendSelectedClass($(this));
        });
        $(document).on("click", "#TargetColumnList li", function (e) {
            Import.prototype.AppendSelectedClass($(this));
        });
        $(document).on("click", "#btnSourceMoveUp", function () {
            Import.prototype.listbox_move("SourceColumnList", "up", "sourceColumnName");
        });
        $(document).on("click", "#btnSourceMoveDown", function () {
            Import.prototype.listbox_move("SourceColumnList", "down", "sourceColumnName");
        });
        $(document).on("click", "#btnSourceAllUp", function () {
            Import.prototype.listbox_move("SourceColumnList", "First", "sourceColumnName");
        });
        $(document).on("click", "#btnSourceAllDown", function () {
            Import.prototype.listbox_move("SourceColumnList", "Last", "sourceColumnName");
        });
        $(document).on("click", "#CsvHeaders li", function (e) {
            Import.prototype.AppendSelectedClass($(this));
        });
        $(document).on("click", "#btnMoveRight", function () {
            Import.prototype.MoveSelectedElement("#CsvHeaders", "#SourceColumnList");
            Import.prototype.ClearMappingErrorMessage();
        });
        $(document).on("click", "#btnMoveLeft", function () {
            Import.prototype.MoveSelectedElement("#SourceColumnList", "#CsvHeaders");
            Import.prototype.ClearMappingErrorMessage();
        });
        $(document).on("click", "#btnAllMoveRight", function () {
            Import.prototype.MoveAllRight();
        });
        $(document).on("click", "#btnAllMoveLeft", function () {
            Import.prototype.MoveAllLeft();
        });
        $(document).on("click", "#btnTargetMoveUp", function () {
            Import.prototype.listbox_move("TargetColumnList", "up", "targetColumnName");
        });
        $(document).on("click", "#btnTargetMoveDown", function () {
            Import.prototype.listbox_move("TargetColumnList", "down", "targetColumnName");
        });
        $(document).on("click", "#btnTargetAllUp", function () {
            Import.prototype.listbox_move("TargetColumnList", "First", "targetColumnName");
        });
        $(document).on("click", "#btnTargetAllDown", function () {
            Import.prototype.listbox_move("TargetColumnList", "Last", "targetColumnName");
        });
        $(document).on("blur", "#TemplateName", function () {
            Import.prototype.ClearErrorMessage();
        });

        $(document).on("click", "#btnCreateTemplateName", function () {
            if ($("#TemplateName").val() == 0 || $("#TemplateName").val() == undefined || $("#TemplateName").val() == "") {
                $("#error-template-name").html(ZnodeBase.prototype.getResourceByKeyName("EnterTemplateNameError"));
            }
            else {
                $("#error-template-name").html("");
                Import.prototype.AddTemplateName();
                $("#ShowHideTemplates").show();
            }
        });

        $(document).on("change", "#txtUpload", function () {
            Import.prototype.ValidateImportedFileType();
        });

        /*Add this event for refreshing the import grid on popup back button*/
        $(document).on("click", "#btnbackforimport", function () {
            $('.z-new-refresh').click();
        });
    }

    RemoveTargetColumns(): boolean {
        $("#TargetColumnList").empty();
        return true;
    }

    MoveAllLeft(): void {
        $("#SourceColumnList li").each(function () {
            var header = $(this).html().trim();
            var attr = $(this).attr("CsvHeaders");
            var isAvailable = false;
            if ($("#CsvHeaders").find("li").length > 0) {
                $("#CsvHeaders li").each(function () {
                    if ($(this).html().trim() != header) {
                        isAvailable = true;
                    } else {
                        isAvailable = false;
                        return false;
                    }
                });
            }
            else {
                $("#CsvHeaders").append("<li sourcecolumnname=" + $(this).attr("sourceColumnName") + ">" + $(this).html() + "</li>");
                $(this).remove();
            }

            if (isAvailable) {
                $("#CsvHeaders").append("<li sourcecolumnname=" + $(this).attr("sourceColumnName") + ">" + $(this).html() + "</li>");
                $(this).remove();
            }
        });
    }

    MoveAllRight(): void {
        $("#CsvHeaders li").each(function () {
            var header = $(this).html().trim();
            var attr = $(this).attr("sourceColumnName");
            var isAvailable = false;
            if ($("#SourceColumnList").find("li").length > 0) {
                $("#SourceColumnList li").each(function () {
                    if ($(this).html().trim() != header) {
                        isAvailable = true;
                    } else {
                        isAvailable = false;
                        return false;
                    }
                });
            }
            else {
                $("#SourceColumnList").append("<li sourceColumnName=" + $(this).attr("sourceColumnName") + ">" + $(this).html() + "</li>");
                $(this).remove();
            }

            if (isAvailable) {
                $("#SourceColumnList").append("<li sourceColumnName=" + $(this).attr("sourceColumnName") + ">" + $(this).html() + "</li>");
                $(this).remove();
            }
        });
        Import.prototype.ClearMappingErrorMessage();
    }

    MoveSelectedElement(fromControl, toControl): void {
        var fromliControl = fromControl + " li.selected ";
        var elements = $(fromliControl);
        if (elements != undefined) {
            if (Import.prototype.CheckHeaderIsNotAvailable($(fromliControl).html().trim(), toControl)) {
                $(toControl).append($(fromliControl));
                elements.removeClass("selected");
                $($(fromControl + " li ")[0]).addClass("selected");
            }
        }
    }

    AppendSelectedClass(controlName): void {
        if ($(controlName) != undefined) {
            $(controlName).parent().find("li").removeClass("selected");
            $(controlName).addClass("selected");
        }
    }

    GetCsvHeaders(dataParam): void {
        ZnodeBase.prototype.ShowLoader();
        var ajaxRequest = $.ajax({
            type: "POST",
            url: "/Import/GetCsvData",
            contentType: false,
            processData: false,
            data: dataParam
        });
        ajaxRequest.done(function (data, textStatus) {
            var dynamicHtml = "";
            if (data.Csvlist != "" && data.Csvlist != null) {
                var headers = data.Csvlist.split(',');
                for (var arrayCounter = 0; arrayCounter < headers.length; ++arrayCounter) {
                    var header = headers[arrayCounter];
                    dynamicHtml += "<li sourceColumnName='" + header + "' >" + header + " </li>";
                }
                $("#CsvHeaders").html(dynamicHtml);
                $("#ChangedFileName").val(data.UpdateFileName);
                ZnodeBase.prototype.HideLoader();
            }
        });
    }

    CheckHeaderIsNotAvailable(header, toControl): boolean {
        var isAvailable = true;
        $(toControl + " li").each(function () {
            if ($(this).html().trim() == header) {
                isAvailable = false;
                return isAvailable;
            } else {
                isAvailable = true;
            }
        });
        return isAvailable;
    }

    ValidateImportFileType(): boolean {
        if ($("#txtUpload").val() != "") {
            if ($("#fileName").html().split(".")[1] == "csv") {
                $("#error-file-upload").html("");
                return true;
            }
            else {
                $("#error-file-upload").html(ZnodeBase.prototype.getResourceByKeyName("SelectCSVFileError"));
                return false;
            }
        } else {
            $("#error-file-upload").html(ZnodeBase.prototype.getResourceByKeyName("FileNotPresentError"));
            return false;
        }
    }

    ValidateImportedFileType(): any {
        var totalFiles = (<HTMLInputElement>$("#txtUpload").get(0)).files.length;
        if (totalFiles > 0) {
            var file = (<HTMLInputElement>$("#txtUpload").get(0)).files[0];
            var regularExpression = /.csv/;
            if (!regularExpression.test(file.name)) {
                $("#error-file-upload").html(ZnodeBase.prototype.getResourceByKeyName("SelectCSVFileError"));
                $("#CsvHeaders").empty();
                return false;
            }
            else {
                var files = (<HTMLInputElement>$("#txtUpload").get(0)).files;
                var dataParm = new FormData();
                dataParm.append("FilePath", files[0]);
                Import.prototype.GetCsvHeaders(dataParm);
                $("#error-file-upload").html("");
                return true;
            }
        }
        return false;
    }

    CheckCSVFileName(fileName): boolean {
        var format = /[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
        return !format.test(fileName);
    }

    ValidateModel(): boolean {
        return (Import.prototype.ValidateImportType() &&
            Import.prototype.ValidateTemplateName() &&
            Import.prototype.ValidateImportFileType() &&
            Import.prototype.ValidateMappings());
    }

    ValidateImportType(): boolean {
        if ($("#importTypeList option:selected").val() == 0 || $("#importTypeList option:selected").val() == undefined || $("#importTypeList option:selected").val() == "") {
            $("#error-importname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportNameError"));
            return false;
        } else {
            $("#error-importname").html("");
            return true;
        }
    }

    ValidateTemplateName(): boolean {
        if ($("#templateList option:selected").val() == 0 || $("#templateList option:selected").val() == undefined || $("#templateList option:selected").val() == "") {
            if ($("#TemplateName").val() == undefined || $("#TemplateName").val() == "") {
                $("#error-templatename").html(ZnodeBase.prototype.getResourceByKeyName("SelectTemplateNameError"));
                if ($("#txtUpload") != undefined && $("#txtUpload").val() != undefined && $("#txtUpload").val() != "") {
                    $("#txtUpload").val("");
                    $("#fileName").html("");
                }
                return false;
            } else {
                $("#error-templatename").html("");
                return true;
            }
        } else {
            return true;
        }
    }

    ValidateMappings(): boolean {
        if ($("#SourceColumnList > li").length > 0 && $("#TargetColumnList> li").length > 0) {
            $("#error-sourcecolumnlist").html("");
            return true;
        } else {
            $("#error-sourcecolumnlist").html(ZnodeBase.prototype.getResourceByKeyName("SelectTemplateMappingError"));
            return false;
        }
    }

    ValidateAndPost(): boolean {
        if (Import.prototype.ValidateModel()) {
            Import.prototype.CreateAndPostModel();
            return false;
        } else {
            return false;
        }
    }

    CreateModel(): any {
        var mappings = new Array();
        var loopCount = $("#TargetColumnList > li").length;
        for (var iCount = 0; iCount < loopCount; iCount++) {
            var item = {
                MappingId: $($("#TargetColumnList > li")[iCount]).attr('targetvalue'),
                MapCsvColumn: $($("#SourceColumnList > li")[iCount]).text().trim(),
                MapTargetColumn: $($("#TargetColumnList > li")[iCount]).text().trim()
            };
            mappings.push(item);
        }
        var templateName = "";
        if ($("#templateList").val() > 0) {
            templateName = $("#templateList option:selected").text();
        } else {
            templateName = $("#TemplateName").val();
        }

        var ImportViewModels = {
            LocaleId: $("#LocaleId").val(),
            ImportHeadId: $("#importTypeList").val(),
            ImportType: $("#importTypeList option:selected").text(),
            TemplateId: $("#templateList").val(),
            TemplateName: templateName,
            TemplateVersion: $("#TemplateVersion").val(),
            FileName: $("#ChangedFileName").val(),
            FamilyId: $("#familyList").val(),
            PriceListId: $("#pricingList option:selected").val(),
            CatalogId: $("#catalogList option:selected").val(),
            Mappings: mappings,
            CountryCode: $("#countryList option:selected").val(),
            PortalId: $("#PortalId").val(),
            PromotionTypeId: $("#promotionTypeList").find('option:selected').val(),
        };

        return ImportViewModels;
    }

    CreateAndPostModel(): any {
        var ImportViewModels = Import.prototype.CreateModel();
        Import.prototype.PostData(ImportViewModels);
    }

    PostData(importModel): any {
        ZnodeBase.prototype.ShowLoader();
        Endpoint.prototype.ImportPost(importModel, function (res) {
            setTimeout(function () {
                ZnodeBase.prototype.HideLoader();
                window.location.href = window.location.protocol + "//" + window.location.host + "/Import/List";
            }, 900);
        });
    }

    GetTemplateAndFamilies(): void {
        var importHeadId = $("#importTypeList option:selected").val();
        var familyId = $("#familyList option:selected").val();
        var promotionTypeId = $("#promotionTypeList").find('option:selected').val()
        if (familyId == "" || familyId == undefined) {
            familyId = 0;
        }
        if (promotionTypeId == "" || promotionTypeId == undefined) {
            promotionTypeId = 0;
        }

        Import.prototype.ResetDropdowns();

        $("#downloadImportHeadId").val(importHeadId);
        $("#downloadImportName").val($("#importTypeList option:selected").text());
        $("#downloadImportFamilyId").val(familyId);
        $("#downloadImportPromotionTypeId").val(promotionTypeId);
        $('#divSelectPricing').hide();

        if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "product" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "category") {
            $('#divSelectFamily').show();
            $('#divSelectCountry').hide();
            $('#divSelectCatalog').hide();
            $('#divSelectPromotionType').hide();
            Import.prototype.GetAllFamilies();
        }
        else if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "pricing") {
            $('#divSelectPricing').show();
            $('#divSelectCatalog').hide();
            $('#divSelectCountry').hide();
            $('#divSelectPromotionType').hide();
            Endpoint.prototype.GetPricingList(function (res) {
                let dynamicHtml: string = "";
                if (res.pricingList != "" && res.pricingList != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#pricingList").html(Import.prototype.GetDropDownHtml(res.pricingList, dynamicHtml));
                }
            });
        }
        else if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "catalogcategoryassociation") {
            $('#divSelectCatalog').show();
            $('#divSelectCountry').hide();
            $('#divSelectPromotionType').hide();
            Endpoint.prototype.GetImportCatalogList(function (res) {
                let dynamicHtml: string = "";
                if (res.catalogList != "" && res.catalogList != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#catalogList").html(Import.prototype.GetDropDownHtml(res.catalogList, dynamicHtml));
                }
            });
        }

        else if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "promotions") {
            $('#divSelectPromotionType').show();
            $('#divSelectCatalog').hide();
            $('#divSelectCountry').hide();
            Endpoint.prototype.GetImportPromotionTypeList(function (res) {
                let dynamicHtml: string = "";
                if (res.promotionTypeList != "" && res.promotionTypeList != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#promotionTypeList").html(Import.prototype.GetDropDownHtml(res.promotionTypeList, dynamicHtml));
                }
            });
        }
        else if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "zipcode") {
            $('#divSelectCountry').show();
            $('#divSelectCatalog').hide();
            $('#divSelectPromotionType').hide();
            Endpoint.prototype.GetImportCountryList(function (res) {
                let dynamicHtml: string = "";
                if (res.countryList != "" && res.countryList != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#countryList").html(Import.prototype.GetDropDownHtml(res.countryList, dynamicHtml));
                }
            });
        }
        else if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "seodetails" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "customer" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "adminb2bcustomer" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "account") {
            $('#divSelectSEODetails').show();
            $('#divSelectCountry').hide();
            $('#divSelectCatalog').hide();
            $('#divSelectPromotionType').hide();
            Import.prototype.GetTemplateList();
            Endpoint.prototype.GetImportPortalList(function (res) {
                let dynamicHtml: string = "";
                if (res.portalList != "" && res.portalList != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#portalList").html(Import.prototype.GetDropDownHtml(res.portalList, dynamicHtml));
                }
            });
        }
        else {
            $('#divSelectFamily').hide();
            $('#divSelectCountry').hide();
            $('#divSelectPricing').hide();
            $('#divSelectCatalog').hide();
            $('#divSelectPromotionType').hide();
            Import.prototype.GetTemplateList();
        }
        $('#updateMappings').hide();
        if ($("#importTypeList").val() != "")
            $("#error-importname").html("");
    }

    GetTemplatesFromFamily(): void {
        $("#error-familyname").html("");
        $("#TargetColumnList").html("");
        $("#SourceColumnList").html("");
        Import.prototype.GetTemplateList();
    }

    GetAllFamilies(): void {
        var importType = $("#importTypeList option:selected").text().toLocaleLowerCase();
        if (importType == "product" || importType == "category" || importType == "promotions") {
            var isCategory = true;
            if (importType == "product" || importType == "promotions") {
                isCategory = false;
            }

            Endpoint.prototype.GetAllFamilies(isCategory, function (res) {
                var dynamicHtml = "";
                if (res.productFamilies != "" && res.productFamilies != null) {
                    dynamicHtml = "<option value='0'>Please Select</option>";
                    $("#familyList").html(Import.prototype.GetDropDownHtml(res.productFamilies, dynamicHtml));
                }
            });
        }
    }

    public GetDropDownHtml(responseList, dynamicHtml): string {
        for (var arrayCounter = 0; arrayCounter < responseList.length; ++arrayCounter) {
            var value = responseList[arrayCounter].Value;
            var templateList = responseList[arrayCounter].Text;
            dynamicHtml += "<option value='" + value + "'>" + templateList + " </option>";
        }
        return dynamicHtml;
    }

    ResetDropdowns(): void {
        $('#divSelectFamily').hide();
        $('#divSelectPricing').hide();
        $('#divSelectSEODetails').hide();
        $("#familyList").val('0');
        $("#templateList").val('0');
        $("#pricingList").val('0');
        $("#TargetColumnList").html("");
        $("#SourceColumnList").html("");
        $("#familyList").html("");
        $("#templateList").html("");
        $("#pricingList").html("");
        $("promotionTypeList").html("");
    }

    GetTemplateList(): void {
        $("#error-importname").html("");
        var importHeadId = $("#importTypeList option:selected").val();
        var familyId = 0;
        var promotionTypeId = 0;
        if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "product" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "category") {
            familyId = $("#familyList option:selected").val();
        }

        if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "promotions") {
            promotionTypeId = $("#promotionTypeList option:selected").val();
        }

        $("#downloadImportHeadId").val(importHeadId);
        $("#downloadImportName").val($("#importTypeList option:selected").text());
        $("#downloadImportFamilyId").val(familyId);
        $("#downloadImportPromotionTypeId").val(promotionTypeId);
        Endpoint.prototype.GetTemplateList(importHeadId, familyId, promotionTypeId,function (res) {
            var dynamicHtml = "";
            if (res.templatenamelist != "" && res.templatenamelist != null) {
                $("#templateList").html(Import.prototype.GetDropDownHtml(res.templatenamelist, dynamicHtml));
                $("#TargetColumnList").html("");
                $("#SourceColumnList").html("");
            } else {
                $("#templateList").html('');
            }
        });
    }

    GetTemplateMappings(): void {
        $("#error-templatename").html("");
        var templateId = $("#templateList option:selected").val();
        var importHeadId = $("#importTypeList option:selected").val();
        var familyId = $("#familyList option:selected").val();
        var promotionTypeId = $("#promotionTypeList option:selected").val();
        if (templateId == "" || templateId == undefined) {
            templateId = 0;
        }

        if (familyId == "" || familyId == undefined) {
            familyId = 0;
        }

        if (promotionTypeId == "" || promotionTypeId == undefined) {
            promotionTypeId = 0;
        }

        Endpoint.prototype.GetTemplateMappings(templateId, importHeadId, familyId, promotionTypeId,function (res) {
            if (res.templateMappingList != "" && res.templateMappingList != null) {
                var targetColumns = res.templateMappingList[0];
                var sourceColumns = res.templateMappingList[1];
                if ($("#divAddTemplatePopup").is(":visible") && templateId == 0) {
                    Import.prototype.ShowColumnLists(targetColumns, sourceColumns);
                } else if (!$("#divAddTemplatePopup").is(":visible") && templateId > 0) {
                    Import.prototype.ShowColumnLists(targetColumns, sourceColumns);
                } else {
                    $("#TargetColumnList").empty();
                    $("#SourceColumnList").empty();
                }
            }
        });
        Import.prototype.ShowUpdateMappingButton();
    }

    ShowColumnLists(targetColumns, sourceColumns): void {
        var dynamicHtml = "";
        if (targetColumns != "" && targetColumns != null) {
            for (var arrayCounter = 0; arrayCounter < targetColumns.length; ++arrayCounter) {
                var targetValue = targetColumns[arrayCounter].Value;
                var targetColumnName = targetColumns[arrayCounter].Text;
                dynamicHtml += "<li targetColumnName='" + targetColumnName + "'>" + targetColumnName + " </li>";
            }
            $("#TargetColumnList").html(dynamicHtml);
        }
        dynamicHtml = "";
        if (sourceColumns != "" && sourceColumns != null) {
            for (var arrayCounter = 0; arrayCounter < sourceColumns.length; ++arrayCounter) {
                var sourceColumnName = sourceColumns[arrayCounter].Text;
                if (sourceColumnName != null && $.trim(sourceColumnName) != "")
                    dynamicHtml += "<li sourceColumnName='" + sourceColumnName + "'>" + sourceColumnName + " </li>";
            }
            $("#SourceColumnList").html(dynamicHtml);
        }
    }

    listbox_move(listID, direction, attributeVal) {
        var selValue = $("#" + listID + " .selected").attr(attributeVal);
        var selText = $("#" + listID + " .selected").html();
        if (selText != undefined && selValue != undefined) {
            var dynamicHtml = "<li " + attributeVal + "='" + selValue + "'>" + selText + "</li>";
            if (direction == "First") {
                $("#" + listID).prepend(dynamicHtml);
            } else if (direction == "Last") {
                $("#" + listID).append(dynamicHtml);
            } else if (direction == "down") {
                var dynamicHtmlTest = $("#" + listID + " .selected").next("li");
                if (dynamicHtmlTest.length != 0) {
                    $("#" + listID + " .selected").next("li").after(dynamicHtml);
                } else { return false; }
            } else {
                var dynamicHtmlTest = $("#" + listID + " .selected").prev("li");
                if (dynamicHtmlTest.length != 0) {
                    $("#" + listID + " .selected").prev("li").before(dynamicHtml);
                } else { return false; }
            }
            $("#" + listID + " .selected").remove();
            $("#" + listID + " li[" + attributeVal + "='" + selValue + "']").addClass("selected");
        }
    }

    CreateNewTemplate(): boolean {
        if ($("#importTypeList option:selected").val() == "") {
            $("#error-importname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportNameError"));
            return false;
        } else {
            if ($("#importTypeList option:selected").text().toLocaleLowerCase() == "product" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "category") {
                if ($("#familyList option:selected").val() == "0" || $("#familyList option:selected").val() == undefined || $("#familyList option:selected").val() == "") {
                    $("#error-familyname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportFamilyError"));
                    return false;
                }
                if ($("#promotionTypeList option:selected").text().toLocaleLowerCase() == "promotions") {
                    $("#error-familyname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportFamilyError"));
                    return false;
                }
            }
            Import.prototype.CreateTemplate();
            $("#error-familyname").html("");
            $("#error-importname").html("");
            $("#templateList").val(0);
            $("#SourceColumnList").empty();
            $("#TargetColumnList").empty();
            setTimeout(function () { Import.prototype.GetTemplateMappings(); }, 300);
            return true;
        }
    }

    CheckImportType(): boolean {
        if ($("#downloadImportHeadId").val() == "" || $("#downloadImportHeadId").val() == undefined) {
            $("#error-importname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportNameError"));
            return false;
        }
        else {
            if (($("#importTypeList option:selected").text().toLocaleLowerCase() == "product" || $("#importTypeList option:selected").text().toLocaleLowerCase() == "category") && $("#familyList option:selected").val() == 0) {
                $("#error-familyname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportFamilyError"));
                return false;
            }
            if (($("#importTypeList option:selected").text().toLocaleLowerCase() == "promotions") && $("#promotionTypeList option:selected").val() == 0) {
                $("#error-familyname").html(ZnodeBase.prototype.getResourceByKeyName("SelectImportFamilyError"));
                return false;
            }
            setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
            return true;
        }
    }

    ClearErrorMessage(): void {
        if ($("#TemplateName").val() != undefined || $("#TemplateName").val() != "") {
            $("#error-templatename").html("");
        }
    }

    ClearMappingErrorMessage(): void {
        if ($("#SourceColumnList > li").length > 0) {
            $("#error-sourcecolumnlist").html("");
        }
    }

    ShowMappingErrorMessage(): void {
        if ($("#SourceColumnList > li").length == 0) {
            $("#error-sourcecolumnlist").html(ZnodeBase.prototype.getResourceByKeyName("SelectTemplateMappingError"));
        }
    }

    CreateTemplate(): any {
        $("#error-template-name").html("");
        $("#divAddTemplatePopup").modal("show");
    }

    AddTemplateName(): any {
        var templateName = $("#TemplateName").val();
        $("#templateList").empty();
        $("#templateList").append($("<option></option>").val("0").html(templateName).attr('selected', 'selected'));
        $("#divAddTemplatePopup").modal("hide");
        ZnodeBase.prototype.RemovePopupOverlay();
    }

    ShowTemplateList(): void {
        Import.prototype.GetTemplateList();
        $("#ShowHideTemplates").hide();
    }

    RemovePopupOverlay(): any {
        //Below code is used to close te overlay of popup, As it was not closed in server because Container is updated by Ajax call
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    }

    RemoveHrefAttributes(): void {
        $(".z-publish").removeAttr("href");
        $(".z-view").removeAttr("href");

        $(document).off("click", ".z-publish");
        $(document).on("click", ".z-publish", function (e) {
            ZnodeBase.prototype.ShowLoader();
            e.preventDefault();
            Import.prototype.ShowLogStatusInPopup($(this));
            return false;
        });
        $(document).off("click", ".z-view");
        $(document).on("click", ".z-view", function (e) {
            ZnodeBase.prototype.ShowLoader();
            e.preventDefault();
            Import.prototype.ShowLogDetailsInPopup($(this));
            return false;
        });
    }

    ShowLogStatusInPopup(control): void {
        var importProcessLogId = $(control).attr("data-parameter").split("=")[1];
        Endpoint.prototype.ShowLogStatus(parseInt(importProcessLogId), function (res) {
            if (res != "") {
                $("#divStatusPopup").html(res);
                $("#divStatusPopup").modal('show');
                ZnodeBase.prototype.HideLoader();
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), 'error', isFadeOut, fadeOutTime);
        });
    }

    ShowLogDetailsInPopup(control): void {
        var importProcessLogId = $(control).attr("data-parameter").split("=")[1];
        Endpoint.prototype.ShowLogDetails(parseInt(importProcessLogId), function (res) {
            if (res != "") {
                $("#divLogDetailsPopup").html(res);
                $("#divLogDetailsPopup").show(700);
                ZnodeBase.prototype.ShowLoader();
                $("body").append("<div class='modal-backdrop fade in'></div>");
            }
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), 'error', isFadeOut, fadeOutTime);
        });
    }

    UpdateMappings(): any {
        var importViewModels = Import.prototype.CreateModel();
        $.ajax({
            type: "POST",
            url: "/Import/UpdateMappings",
            data: importViewModels,
            success: function (res) {
                if (res.status) {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'success', isFadeOut, fadeOutTime);
                } else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, 'error', isFadeOut, fadeOutTime);
                }
            },
            error: function (error) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorMessage"), 'error', isFadeOut, fadeOutTime);
            }
        });
    }

    ShowUpdateMappingButton(): boolean {
        var templateId = $("#templateList option:selected").val();
        var importHeadId = $("#importTypeList option:selected").val();
        if (templateId == 0 && importHeadId == 0) {
            $('#updateMappings').hide();
            return false;
        }
        else {
            $('#updateMappings').show();
            return true;
        }
    }

    //Delete Import process Log details.
    DeleteImportLogs(control): any {
        let importProcessLogId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (importProcessLogId.length > 0) {
            Endpoint.prototype.DeleteImportLogs(importProcessLogId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //This method is used to select portal from list and show it on textbox
    OnSelectPortalResult(item: any): any {
        if (item != undefined) {
            $('#PortalId').val(item.Id);
        }
    }

    DownloadImportLogDetails(event): any {
        $("body").click();
        if (event == 'undefined' || event == "" || event == null) {
            return false;
        } else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelperForAsidePopupPanel("Export has been initiated. Click" + " <a href='/Export/List'> here</a> or redirect to the Exports screen to view more details.", 'success', isFadeOut, fadeOutTime);
            var newForm = $('<form ></form>', { action: '/Import/DownloadPDF', method: 'POST' }).append($('<input></input>', {
                'name': 'importProcessLogId',
                'id': 'importProcessLogId',
                'value': $("#importProcessLogId").val(),
                'type': 'hidden'
            })).append($('<input></input>', {
                'name': 'pageIndex',
                'id': 'pageIndex',
                'value': $('.aside-popup-panel').find("#pagerTxt").val(),
                'type': 'hidden'
            })).append($('<input></input>', {
                'name': 'pageSize',
                'id': 'pageSize',
                'value': $('.aside-popup-panel').find("#pageSizeList").find(':selected').val(),
                'type': 'hidden'
            }));

            $("body").append(newForm);
            newForm.submit();
            setTimeout(function () { ZnodeBase.prototype.HideLoader() }, 1000);
            return true;
        }
    }

    //Method to make Export ajax request
    Export(e: any) {
        $("body").click();
        var currentTarget = e.currentTarget;
        var controller: string = currentTarget.getAttribute("data-controller");
        var action: string = currentTarget.getAttribute("data-action");
        var exportTypeId: string = currentTarget.getAttribute("data-exportTypeId");
        var exportType: string = currentTarget.getAttribute("data-exporttype");
        var localId: string = $("#ddlCultureSpan").attr("data-value") ? $("#ddlCultureSpan").attr("data-value") : "0";
        var url = this.getExportUrl(controller, action);
        var param = this.getExportParam(exportTypeId, exportType, localId, $("#importProcessLogId").val(), $('.aside-popup-panel').find("#pagerTxt").val(), $('.aside-popup-panel').find("#pageSizeList").find(':selected').val());
        var catalogId = $('#hdnFilterCatalogId').val();
        param = catalogId != undefined ? (param += "&pimCatalogId=" + catalogId + "&catalogName=" + $('#hdnFilterCatalogName').val()) : param;
        var exportBase = this;
        ZnodeBase.prototype.ShowLoader();
        ZnodeNotification.prototype.DisplayNotificationMessagesHelperForAsidePopupPanel(ZnodeBase.prototype.getResourceByKeyName("ExportPleaseWaitMsg"), "success", true, 5000);
        ZnodeBase.prototype.ajaxRequest(url, "GET", param, function (response) {
            ZnodeBase.prototype.HideLoader();
            if (!response.HasError)
                ZnodeNotification.prototype.DisplayNotificationMessagesHelperForAsidePopupPanel("Export has been initiated. Click" + " <a href='/Export/List'> here</a> or redirect to the Exports screen to view more details.", 'success', isFadeOut, fadeOutTime);
                //exportBase.downloadFile(response);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelperForAsidePopupPanel(response.Message, "error", true, 5000);
        }, null);

    }

    //Return export parameter
    getExportParam(exportTypeId: string, exportType: string, localId: string, paramId: string, pageIndex: string, pageSize :string) {
        return ("exportFileTypeId=".concat(exportTypeId, "&Type=", exportType, "&localId=", localId, "&paramId=", paramId, "&pageIndex=", pageIndex, "&pageSize=", pageSize));
    }

    //Returns export url
    getExportUrl(controller, action) {
        return ("/".concat(controller, "/", action));
    }

    //Download Data into file to user
    downloadFile(response: any) {
        /*Byte Order Mark - used in encoding that allow reader to identify a file as being encoded in UTF-8.*/
        var BOM = "\uFEFF";
        var blob = new Blob([BOM.concat(response.content)], { type: "text/csv;charset=utf-8;" });

        if (ZnodeBase.prototype.getBrowser() == "IE")
            window.navigator.msSaveBlob(blob, response.fileName);
        else {
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement("a");
            document.body.appendChild(a);
            a.href = url;
            a.download = response.fileName;
            a.click();
            window.URL.revokeObjectURL(url); /* Not to keep the reference to the file any longer.*/
        }
    }

    //Delete Custom Import templates.
    DeleteImportTemplate(control): any {
        let importTemplateId: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (importTemplateId.length > 0) {
            Endpoint.prototype.DeleteImportTemplate(importTemplateId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Enable and disable checkbox and delete icon on the basis of custom template
    SetActiveDeActive(): void {
        $("[data-swhgcontainer='ZnodeImportTemplate'] tbody tr").each(function () {
            $(this).find("td").each(function () {
                if ($(this).next().children().hasClass("z-active")) {
                    $(this).parent('tr').find('.grid-row-checkbox').attr("disabled", true);
                    $(this).find('.z-delete').attr("disabled", true).css({ "pointer-events": "none", "opacity": "0.5" });                   
                }
            });
        });
    }

    //Hide Status column from the xml grid
    HideStatusColumn(): void {
            var tableElement = $("[data-swhgcontainer=ZnodeImportTemplate]");
            tableElement.find('.IsStartedorNot').hide();          
    }
}
