var FileUpload = {
    PreviewImport: function (type) {
        $("#preview").on("click", function () {
            var totalFiles = document.getElementById("importFile").files.length;
            if (totalFiles > 0) {
                var file = document.getElementById('importFile').files[0];
                var regularExpression = "";
                regularExpression = /.xlsx|.xls|.csv/;
                if (regularExpression.test(file.name)) {
                    $("#grid-container").html("");
                    var xhr = new XMLHttpRequest();
                    var fd = new FormData();
                    fd.append("file", file);
                    if (type == "Inventory") {
                        xhr.open("POST", "/Inventory/PreviewImportInventory/", true);
                    }
                    else if (type == "Price") {
                        xhr.open("POST", "/Price/PreviewImportPrice/", true);
                    }
                    xhr.send(fd);
                    xhr.addEventListener("load", function (event) {

                        if (event.target.response.length < 500) {
                            var data = JSON.parse(event.target.response);
                            if (data.HasError === true) {
                                CommonHelper.DisplayNotificationMessagesHelper(data.ErrorMessage, 'error', false, 5000);
                            }
                        }
                        else {
                            $("#preview-import-price").html(event.target.response);
                            $(".pagination").hide()
                            $(".dropdown").hide();
                        }

                    }, false);
                }
                else {
                    var fileTypeValidationError = (uploadFileExtensionError != undefined && uploadFileExtensionError != null && uploadFileExtensionError != "") ? uploadFileExtensionError : "Please select a valid .csv, .xls or .xlsx file.";
                    $("span[data-valmsg-for='ImportFile']").html("<span for='ImportFile'>" + fileTypeValidationError + "</span>");
                    $("span[data-valmsg-for='ImportFile']").attr("class", "field-validation-error");
                }
            }
            else {
                $("span[data-valmsg-for='ImportFile']").html("<span for='ImportFile'>" + $("#ImportFile").attr("data-val-required") + "</span>");
                $("span[data-valmsg-for='ImportFile']").attr("class", "field-validation-error");
            }
        });
    },

    ValidFileFormat: function () {
        var totalFiles = document.getElementById("importFile").files.length;
        if (totalFiles > 0) {
            $("#grid-container").html("");
            $("#preview-import-price").html("");
            var file = document.getElementById('importFile').files[0];
            var regularExpression = "";
            regularExpression = /.csv/;
            if (!regularExpression.test(file.name)) {
                $("#preview").hide()
                $("#file-upload-name").text("");
                $("#error-file-upload").html("Please select file having extension .csv");
                return false;
            }
            else {
                $("#preview").show();
                $("#file-upload-name").text(file.name);
                $("#error-file-upload").html("");
            }
        }
        return true;
    },

    OnSubmit: function () {
        var totalFiles = document.getElementById("importFile").files.length;
        if (totalFiles > 0) {
            var file = document.getElementById('importFile').files[0];
            var regularExpression = "";
            regularExpression = /.csv/;
            if (!regularExpression.test(file.name)) {
                return false;
            }
        }

        var mindate = new Date($("#ActivationDate").val());
        var maxdate = new Date($("#ExpirationDate").val());
        if ((mindate > maxdate) == 1) {
            $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorActivationDate"));
            return false;
        }
        //
        if ($("#ListCode").val() != '' && $("#ListName").val() != '') {
            $("#loading-div-background").show();
        }
        return true;
    },
    GetFileName: function () {
        var totalFiles = document.getElementById("txtUpload").files.length;
        var values = [];
        if (totalFiles > 0) {
            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById('txtUpload').files[i];
                values.push(file.name);
            }
            $("#fileName").text(values);
            $("#txtUpload").attr("title", $("#FileName").val())
            $("#errorTemplateFilePath").text('');
        }
        else {
            $("#fileName").text('');
        }
    },

    ValidateFileSizeAndType: function () {
        var ext = $('#txtUpload').val().split('.').pop().toLowerCase();
        if (ext != "") {
            if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg', 'pdf', 'doc', 'docx', 'ppt', 'xls', 'zip', 'ttf', 'xlsx', 'odt']) == -1) {
                $("#errorFileTypeAndSize").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFileTypeMessage"));
                return false;
            }
            if (document.getElementById('txtUpload').files[0].size > 1048576) {
                $("#errorFileTypeAndSize").html(ZnodeBase.prototype.getResourceByKeyName("ErrorFileSizeMessage"));
                return false;
            }
        }
        return true;
    },

    ValidateTemplateFile: function () {
        if (window.location.pathname.split("/")[2] == "Copy") { return true; }

        var ext = $('#txtUpload').val().split('.').pop().toLowerCase();
        if (ext != "") {
            if ($.inArray(ext, ['cshtml', ]) == -1) {
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
    },
}