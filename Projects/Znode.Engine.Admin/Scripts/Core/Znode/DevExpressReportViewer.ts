var _reportviewid = 0;
class DevExpressReport extends ZnodeBase {
    _endPoint: Endpoint;
    isCatalogValid: boolean;
    catalogId: number;

    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init() {
        $("#closebuttonSaveLayoutPopup").on("click", function () {
            $('#SaveReportLayoutPopup').hide();
        });

        $("#closebuttonLoadLayoutPopup").on("click", function () {
            $('#LoadReportLayoutPopup').hide();
        });

        $("#dvSavebutton").on("click", function () {
            DevExpressReport.prototype.fnSaveReportcomponents();
        });
        $("#Reset").on("click", function () {
            $("#txtReportLayoutName").val("");
        });
        DevExpressReport.prototype.InisilizeCss();
    }

    InisilizeCss(): any {
        $(document).element.find('.dx-overlay-content').css('height', 'auto');
        $(document).element.find('.dx-overlay-content').css('top', '10px');
    }

    ViewSavedHistories(): any {
        DevExpressReport.prototype.fnLoadSavedReportcomponents();
        $('#LoadReportLayoutPopup').show();
    }

    ShowSaveLayoutPopup(): any {
        $('#SaveReportLayoutPopup').show();
    }

    GetUrlParameter(name: any): any {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    };

    AddOrUpdateUrlParam(uri: any, paramKey: any, paramVal: any) {
        var re = new RegExp("([?&])" + paramKey + "=[^&#]*", "i");
        if (re.test(uri)) {
            uri = uri.replace(re, '$1' + paramKey + "=" + paramVal);
        } else {
            var separator = /\?/.test(uri) ? "&" : "?";
            uri = uri + separator + paramKey + "=" + paramVal;
        }
        return uri;
    }

    fnSaveReportcomponents(): any {
        var reportName = $("#txtReportLayoutName").val();
        if (reportName == "")
        {
            $('#statusmessage').html(ZnodeBase.prototype.getResourceByKeyName("SliderNameRequired"));
            $('#AlertPopup').show();
            return;
        }

        var reportCode = DevExpressReport.prototype.GetUrlParameter("reportCode");
        Endpoint.prototype.SaveReportLayout(reportName, reportCode, function (res) {
            $('#statusmessage').html(res.message);
            $('#AlertPopup').show();
            $("#txtReportLayoutName").val("");
            $('#SaveReportLayoutPopup').hide();
        });
    }

    fnDeleteSavedReportLayoutConfirmation(reportviewid: any): any {
        _reportviewid = reportviewid;
        $('#ConfirmPopup').show();
    }

    fnDeleteSavedReportLayout(): any {
        Endpoint.prototype.fnDeleteSavedReportLayout(_reportviewid, function (res) {
            DevExpressReport.prototype.fnLoadSavedReportcomponents();
        });
    }


    fnLoadSavedReportcomponents(): any {
        var reportName = $("#txtReportLayoutName").val();
        var reportCode = DevExpressReport.prototype.GetUrlParameter("reportCode");
        Endpoint.prototype.LoadSavedReportLayout(reportName, reportCode, function (res) {
            var html = "";
            if (res.data.length > 0) {
                for (var i = 0; i < res.data.length; i++) {
                    html += "<div class='report-popup-container-div'><label class='report-popup-container-label'><input type='radio' name ='reportsname' value='" + res.data[i].ReportName + "'/><span class='lbl padding-8'>" + res.data[i].ReportName + "</span></label><label class='report-popup-file-remove-label' onclick='DevExpressReport.prototype.fnDeleteSavedReportLayoutConfirmation(" + res.data[i].ReportViewId + ")'>x</label></div>";
                }
            }
            else
            {
                html += ZnodeBase.prototype.getResourceByKeyName("NoResult");
            }
            $("#dvFilesHistory").html(html);
        });
    }

    fnLoadReportcomponents(): any {
        if ($('input[name=reportsname]:checked').val() == undefined)
        {
            $('#statusmessage').html(ZnodeBase.prototype.getResourceByKeyName("FileNameSelectionValidation"));
            $('#AlertPopup').show();
            return;
        }
        window.location.href = DevExpressReport.prototype.AddOrUpdateUrlParam(window.location.href, "reportName", $('input[name=reportsname]:checked').val());

    }

    CustomizeMenuActions(s: any, e: any, field: any): any {
        var actions = e.Actions;

        var hightlightEditingFields = e.GetById(field);
        if (hightlightEditingFields)
            hightlightEditingFields.visible = false;
    }

    OnInit(s: any, e: any): any {
        var reportPreview = s.GetPreviewModel().reportPreview;
        //set the properties of web document viewer.
        reportPreview.showMultipagePreview(true);
        reportPreview.zoom(1);
        //Report parameter panel properties configuration.
        var previewModel = s.GetPreviewModel();
        previewModel.tabPanel.width(350);
        previewModel.tabPanel.collapsed(false);
        previewModel.tabPanel.tabs[0].active(true)
        var currentExportOptions = reportPreview.exportOptionsModel;
        var optionsUpdating = false;
        var fixExportOptions = function (options) {
            try {
                optionsUpdating = true;
                if (!options) {
                    currentExportOptions(null);
                } else {
                    delete options["docx"];
                    delete options["mht"];
                    delete options["html"];
                    delete options["textExportOptions"];
                    delete options["rtf"];
                    delete options["image"];
                    currentExportOptions(options);
                }
            } finally {
                optionsUpdating = false;
            }
        };
        currentExportOptions.subscribe(function (newValue) {
            !optionsUpdating && fixExportOptions(newValue);
        });
        fixExportOptions(currentExportOptions());
    }

    EnableDisableParameterByOther(affectedParamName: any, affectedByParamName: any, s: any): any {
        var parametersModel = s.GetPreviewModel().parametersModel;
        if (parametersModel[affectedByParamName]() == true) {
            $("input[name=" + affectedParamName + "]").next().children(".dx-texteditor-input").attr("disabled", "disabled");
        }
        else {
            $("input[name=" + affectedParamName + "]").next().children(".dx-texteditor-input").removeAttr("disabled");
        }
    }

    DisableFieldIfChecked(s: any, e: any): any {
        setTimeout(function () {
            s.parametersInfo.parameters.forEach(function (param, count) {
                if (param.Name == 'ShowAllProducts') {
                    DevExpressReport.prototype.EnableDisableParameterByOther('paramTopProducts', 'ShowAllProducts', s);
                }
                if (param.Name == 'ShowAllCustomers') {
                    DevExpressReport.prototype.EnableDisableParameterByOther('paramTopCustomers', 'ShowAllCustomers', s);
                }
            });
        }, 1000);
    }
    
}