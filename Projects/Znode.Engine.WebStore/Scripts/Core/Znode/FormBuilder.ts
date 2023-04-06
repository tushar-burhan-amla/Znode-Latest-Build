var selectedTab: string;

class FormBuilder extends ZnodeBase {
    constructor() {
        super();
    }

    FormTemplateOnSuccess(data) {
        if ($("#IsTextMessage").val() == "True") {
            ZnodeBase.prototype.HideLoader();
            var message: string = $("#TextMessage").val();  
            if (message == undefined || message == "")
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RecordCreationSuccessMessage"), 'success', isFadeOut, fadeOutTime);
            else
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper($("#TextMessage").val(), 'success', isFadeOut, fadeOutTime);
            $('#layout-formtemplate').html("")
            setTimeout(function() {
                 window.location.reload();
            }, 2000);
        }
        else {
            var redirectURl: string = $("#RedirectURL").val();
            if (redirectURl != "" || redirectURl != undefined)
                window.location.href = $("#RedirectURL").val();
            else
                window.location.reload();
        };
    }

    //Method for Save Product
    SaveFormBuilder(backURL: string) {
        ZnodeBase.prototype.ShowLoader();
        if (!$("#frmFormBuilder").valid()) {
            ZnodeBase.prototype.HideLoader();
        } else if (!FormBuilder.prototype.IsAttributeValueUnique()) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        else
            return true;
    }

    ValidateDocument(control): boolean {
        ZnodeBase.prototype.ShowLoader();
        var totalFiles = control.files;
        var extensionsArray = $(control).attr("data-val").replace(/\./g, '').split(',');
        if (totalFiles.length > 0) {
            var fileExtension = $(control).val().split('.').pop().toLowerCase();
            if (fileExtension != "") {
                if ($.inArray(fileExtension, extensionsArray) == -1) {
                    $("#error_" + control.id).html(ZnodeBase.prototype.getResourceByKeyName("ErrorExtensionNotAllowed"));
                    $("#" + control.name).val('');
                    $("#FileName_" + control.id).text('');
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
                if (totalFiles[0].size > 5242880) {
                    $("#error_" + control.id).html(ZnodeBase.prototype.getResourceByKeyName("ErrorFileSizeMessage"));
                    ZnodeBase.prototype.HideLoader();
                    return false;
                }
                if (control.files.length > 1) {
                    FormBuilder.prototype.UploadMultipleDocument(control);
                }
                else {
                    $("#FileName_" + control.id).text(control.files[0].name);
                    FormBuilder.prototype.UploadDocument(totalFiles, function (response) {
                        $("#" + control.id.substring(1)).attr("value", response.FileName);
                        ZnodeBase.prototype.HideLoader();
                    });
                }
            }
            $("#btnCompleteCheckout").prop("disabled", false);
            $("#error_" + control.id).html("");
            ZnodeBase.prototype.HideLoader();
        }
    }

    UploadMultipleDocument(control): any {
        $.each(control.files, function (e, v) {
            $("#FileName_" + control.id).append("<li>" + v.name + "</li>");
        });
        FormBuilder.prototype.UploadDocument(control.files, function (response) {
            $("#" + control.id.substring(1)).attr("value", response.FileName);
            ZnodeBase.prototype.HideLoader();
        });
    }

    UploadDocument(file, callback): any {
        CommonHelper.prototype.GetAjaxHeaders(function (response) {
            var data = new FormData();
            $.each(file, function (e, v) {
                data.append("file", v);
            });
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", response.Authorization);
                    xhr.setRequestHeader("Znode-UserId", response.ZnodeAccountId);
                    response.DomainName = response.DomainName.replace(/^https?:\/\//, '');
                    response.DomainName = response.DomainName.replace(/^http?:\/\//, '');
                    xhr.setRequestHeader("Znode-DomainName", response.DomainName);
                    xhr.setRequestHeader("Token", response.Token);
        
                },
                url: response.ApiUrl + "/apiupload/uploadformdocument?filePath=~/Data/FormBuilderMedia",
                contentType: false,
                dataType: "json",
                processData: false,
                data: data,
                success: function (data) {
                    callback(data);
                },
                error: function (error) {
                    var jsonValue = JSON.parse(error.responseText);
                }
            });
        })
    }

    labelClick(className: string, AttributeId: string) {
        if (className === "yes") {
            $("input#" + AttributeId + ".yes").prop("checked", true);
            $("input#" + AttributeId + ".no").prop("checked", false);
        }
        else {
            $("input#" + AttributeId + ".no").prop("checked", true);
            $("input#" + AttributeId + ".yes").prop("checked", false);
        }
    }

    //Method for return true if value is unique else false
    IsAttributeValueUnique(): boolean {
        //check for other validations        
        let result: boolean = FormBuilder.prototype.Validate();
        let attributeCodeValues: string = "";
        $("input[type='text']").each(function () {
            if ($(this).attr("data-unique") != undefined && $(this).attr("data-unique") != "" && $(this).attr("data-unique") != "false") {
                attributeCodeValues = attributeCodeValues + $(this).attr("id").split('_')[0] + '#' + $(this).val() + '~';
            }
        });

        let id: number = parseInt($("#FormBuilderId").val());
        let entityType: string = $("#FormCode").val();
        attributeCodeValues = attributeCodeValues.substr(0, attributeCodeValues.length - 1);

        Endpoint.prototype.IsGlobalAttributeValueUnique(attributeCodeValues, id, entityType, function (res) {
            if (res.data != null && res.data != "") {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.data, 'error', isFadeOut, fadeOutTime);
                result = false;
            }
        });
        return result;
    }

    //Validate default date value.
    ValidateNumberDefaultValue(): boolean {
        var defaultValue = $("#AttributeDefaultValue").val();

        if (!(defaultValue == undefined || defaultValue.length == 0 || /\s/g.test(defaultValue))) {
            if (!/^[+-]?[0-9]{1,13}(?:\.[0-9]{1,6})?$/i.test(defaultValue)) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("OnlyNumericValueallow"));
                $("#errorAttributeDefaultValue").show();
                return true;
            }
            else if ($("input[name = AllowDecimals]:checked").val() == "false" && FormBuilder.prototype.IsDecimalExist(defaultValue)) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("DeciamlValueNotAllowed"));
                return true;
            }
            else if ($("input[name = AllowNegative]:checked").val() == "false" && parseInt(defaultValue) < 0) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("NegitaveValueNotAllowed"));
                return true;
            }
            else if (FormBuilder.prototype.BetweenNumber($("#MinNumber").val(), $("#MaxNumber").val())) {
                $("#errorAttributeDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("InvalidNumber"));
                return true;
            }
            else {
                $("#errorAttributeDefaultValue").text("");
                return false;
            }
        }

        else {
            $("#errorAttributeDefaultValue").text("");
            return false;
        }
    }

    IsValidateNumberLength(val) {
        var isNumeric = false;
        if (val != undefined) {
            if (val.length >= 13)
                isNumeric = true;
        }
        return isNumeric;
    }

    IsDecimalExist(p_decimalNumber) {
        var l_boolIsExist = true;

        if (p_decimalNumber % 1 == 0)
            l_boolIsExist = false;

        return l_boolIsExist;
    }

    //Validate default date value.
    ValidateDateDefaultValue(): boolean {
        var defaultValue = $("#AttributeDefaultDateValue").val();

        if (!(defaultValue == undefined || defaultValue.length == 0 || /\s/g.test(defaultValue))) {
            if (FormBuilder.prototype.BetweenDate($("#MinDate").val(), $("#MaxDate").val(), defaultValue)) {
                $("#errorAttributeDateDefaultValue").text(ZnodeBase.prototype.getResourceByKeyName("InvalidDate"));
                return true;
            }
            else {
                $("#errorAttributeDateDefaultValue").text("");
                return false;
            }
        }
        else {
            $("#errorAttributeDateDefaultValue").text("");
            return false;
        }
    }

    //Validate min and max number rang for number type default value.
    BetweenNumber(first, last): boolean {
        var defaultValue = $("#AttributeDefaultValue").val();

        if (isNaN(first) || first == undefined || first == "")
            return false;

        else if (isNaN(last) || last == undefined || last == "")
            return false;

        else if (defaultValue.length == 0 || /\s/g.test(defaultValue))
            return false;

        else
            return !(parseFloat(defaultValue) >= first && parseFloat(defaultValue) <= last);
    }

    //Validate min and max date rang for date type default value.
    BetweenDate(first: Date, last: Date, defaultValue: Date): boolean {
        if (first.toString() != "" && last.toString() != "") {
            var srcDate = first.toString().replace(/-/g, ' ');
            var startDate = new Date(srcDate);
            startDate.setDate(startDate.getDate());
            srcDate = last.toString().replace(/-/g, ' ');
            var endDate = new Date(srcDate);
            endDate.setDate(endDate.getDate());
            srcDate = defaultValue.toString().replace(/-/g, ' ');
            var defaultDate = new Date(srcDate);
            defaultDate.setDate(defaultDate.getDate());
            return !((defaultDate >= startDate) && (defaultDate <= endDate));
        }
        else
            return false;
    }

    ShowMessage(id: string, key: string): boolean {
        $("#" + id).text(ZnodeBase.prototype.getResourceByKeyName(key));
        $("#" + id).show();
        return false;
    }

    //Vaidate attribute input validation and default values.
    Validate(): boolean {
        var Locales = [];
        var mindate = new Date($("#MinDate").val());
        var maxdate = new Date($("#MaxDate").val());
        var minNumber = $("#MinNumber").val();
        var maxNumber = $("#MaxNumber").val();
        var isAllowedNegative = $("input[name = AllowNegative]:checked").val();
        var isAllowedDecimals = $("input[name = AllowDecimals]:checked").val();
        var attributeType = $("#attributeTypeList option:selected").text();
        var flag = true;

        $(".LocaleLabel").each(function () {
            Locales.push($(this).attr('localename'));
        });

        //Validate attribute locale
        for (var i = 0; i < Locales.length; i++) {
            var value = $("#Locale" + Locales[i]).val();
            if (value.length > 100) {
                $("#error" + Locales[i]).html(ZnodeBase.prototype.getResourceByKeyName("LocaleError"));
                flag = false;
            }
            else if (value.length > 0 && value.indexOf(',') > -1) {
                $("#error" + Locales[i]).html(ZnodeBase.prototype.getResourceByKeyName("ErrorCommaNotAllowed"));
                flag = false;
            }
        }

        if (isNaN(minNumber) && minNumber != undefined) return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "OnlyNumericallowforMaxNumber");

        if (FormBuilder.prototype.IsValidateNumberLength(minNumber)) return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "OnlyNumericallowforMaxNumber");

        if (isNaN(maxNumber) && maxNumber != undefined) return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "OnlyNumericallowforMaxNumber");

        if (FormBuilder.prototype.IsValidateNumberLength(maxNumber)) return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "OnlyNumericallowforMaxNumber");

        if (isAllowedNegative == "false") {
            if (isNaN(minNumber))
                return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "OnlyNumericallowforMinNumber");
            if (!isNaN(minNumber) && parseInt(minNumber) < 0)
                return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "NegitaveValueNotAllowed");

        }

        if (isAllowedNegative == "false") {
            if (isNaN(maxNumber))
                return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "OnlyNumericallowforMaxNumber");
            if (!isNaN(maxNumber) && parseInt(maxNumber) < 0) {
                return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "NegitaveValueNotAllowed");
            }

            if (isAllowedDecimals == "false") {
                if (isNaN(minNumber))
                    return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "OnlyNumericallowforMinNumber");
                if (!isNaN(minNumber) && FormBuilder.prototype.IsDecimalExist(minNumber))
                    return FormBuilder.prototype.ShowMessage("errorSpamMinNumber", "DeciamlValueNotAllowed");
            }

            if (isAllowedDecimals == "false") {
                if (isNaN(maxNumber))
                    return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "OnlyNumericallowforMaxNumber");
                if (!isNaN(maxNumber) && FormBuilder.prototype.IsDecimalExist(maxNumber))
                    return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "DeciamlValueNotAllowed");
            }

            if (parseFloat(minNumber) > parseFloat(maxNumber))
                return FormBuilder.prototype.ShowMessage("errorSpamMaxNumber", "MaxNumberAlwaysGreaterThanMinNumber");

            if (FormBuilder.prototype.IsValidateNumberLength($("#MaxCharacters").val()))
                return FormBuilder.prototype.ShowMessage("errorSpamMaxCharacters", "NumericNumberOutofRang");

            if (attributeType == "Number" && FormBuilder.prototype.ValidateNumberDefaultValue()) {
                $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueNumber").parent().parent().attr("id") + '"]').parent().index() });
                flag = false;
            }

            if (attributeType == "Date" && FormBuilder.prototype.ValidateDateDefaultValue()) {
                $("#tabs").tabs({ active: $('#tabs a[href="#' + $("#divDefaultValueDate").parent().parent().attr("id") + '"]').parent().index() });
                flag = false;
            }
            if (mindate > maxdate) {
                $("#spamDate").html(ZnodeBase.prototype.getResourceByKeyName("ErrorMindate"));
                flag = false;
            }
        }
        return flag;
    }

    //Method for Save Product
    SaveEntityAttribute(backURL: string) {
        ZnodeBase.prototype.ShowLoader();
        $("#globalAttributeAsidePannel li.active-tab-validation").each(function () {
            $(this).removeClass('active-tab-validation');
        });

        FormBuilder.prototype.ValidateFileTypeControl();

        if (!$("#frmGlobalAttribute").valid()) {
            $(".input-validation-error").closest("div.tab-pane").each(function () {
                var groupId = $(this).attr('id');
                FormBuilder.prototype.SetActiveGroup(groupId);
                ZnodeBase.prototype.HideLoader();
            });
        } else if (!FormBuilder.prototype.IsAttributeValueUnique()) {
            ZnodeBase.prototype.HideLoader();
            return false;
        }
        else {
            if (FormBuilder.prototype.ValidateFileTypeControl()) {
                return;
            }
            var url = decodeURIComponent(window.location.href);
            var orignalUrl = url.split(/[?#]/)[0];

            if (typeof (backURL) != "undefined")
                $.cookie("_backURL", backURL, { path: '/' });

            $("#frmGlobalAttribute").submit();
        }
    }

    //Method for Validate File Type Control
    ValidateFileTypeControl(): boolean {
        var flag = false;
        $(".fileuploader").each(function () {
            var value = $(this).parent().find("input[type=text]").val();
            var isRequired = $(this).parent().find("input[type=text]").attr("isrequired");
            if ((value === undefined || value == "") && (isRequired === "True")) {
                $(this).parent().find('span[id="fileerrormsg"]').html(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredfile"));
                $(this).parent().find('span[id="fileerrormsg"]').show();
                var groupId = $(this).closest("div.tab-pane").attr('id');
                FormBuilder.prototype.SetActiveGroup(groupId);
                return flag = true;
            }
            else {
                $(this).parent().find('span[id="fileerrormsg"]').html("");
                $(this).parent().find('span[id="fileerrormsg"]').hide();
            }
        });
        return flag
    }

    SetActiveGroup(group: string) {
        $("#globalAttributeAsidePannel li").each(function () {
            var grpName = $('a', this).attr('href').replace('#', '');
            if (grpName == group) {
                $(this).addClass('active-tab-validation');
            }
        });
    }
}