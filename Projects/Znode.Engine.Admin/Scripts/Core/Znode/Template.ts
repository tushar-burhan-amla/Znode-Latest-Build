class Template extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    Init() {

        var filename = $("#txtUpload").attr("title");
        let uploadedFileName: string = $("#FileName").val() == "" ? $("#FilePath_FileName").val() : $("#FileName").val();
        filename = filename == "" ? uploadedFileName : filename;
        $('#fileName').text(filename);
    }

    DeleteTemplate(control): any {
        var templateIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        var fileName = DynamicGrid.prototype.GetMultipleValuesOfGridColumn('File Name');
        if (templateIds.length > 0) {
            Endpoint.prototype.DeleteTemplate(templateIds, fileName, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    public CheckTemplateName(): void {

        $("#Name").on("blur", function () {
            return Template.prototype.ValidateTemplateName(true);
        });
    }

    public ValidateTemplateName(isOnBlur: boolean = false, backURL: string = ""): any {
        let templateName: string = $("#Name").val();
        let status: boolean = false;
        if ($("#frmTemplate").valid()) {
            if ($("#CMSTemplateId").val() > 0) {
                if (Template.prototype.ValidateUploadedTemplateFile() && !isOnBlur) {
                    if (typeof (backURL) != "undefined")
                        $.cookie("_backURL", backURL, { path: '/' });
                    $("#frmTemplate").submit();
                    return true;
                } else {
                    return false;
                }
            }
        }
        Endpoint.prototype.CheckTemplateName(templateName, function (res) {
            if (res == false) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorTemplateName"), 'error', isFadeOut, fadeOutTime);
                Template.prototype.ValidateUploadedTemplateFile();
                return false;
            }
            else {
                if (!isOnBlur) {
                    status = Template.prototype.ValidateUploadedTemplateFile() ? true : false;
                    if (!isOnBlur && status) {
                        if (typeof (backURL) != "undefined")
                            $.cookie("_backURL", backURL, { path: '/' });
                        $("#frmTemplate").submit();
                    }
                }
            }
        });

    }


    public ValidateUploadedTemplateFile(): boolean {
        if (window.location.pathname.split("/")[2] == "Copy") { return true; }

        var ext = $('#txtUpload').val().split('.').pop().toLowerCase();
        if (ext != "") {
            if ($.inArray(ext, ['cshtml',]) == -1) {
                $("#errorTemplateFilePath").html(ZnodeBase.prototype.getResourceByKeyName("ErrorCSHTMLFile"));
                return false;
            }
            return true;
        }
        var fileName = $("#fileName").text();
        if (fileName == "" || fileName == 'undefined') {
            $("#errorTemplateFilePath").html(ZnodeBase.prototype.getResourceByKeyName("FileRequiredError"));
            return false;
        }
        else {
            $("#errorTemplateFilePath").text('');
            return true;
        }
    }
}