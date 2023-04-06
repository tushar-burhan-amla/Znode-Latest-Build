class CommonHelper extends ZnodeBase {
    toggleMessage: boolean;
    toggleTitle: boolean;
    RefreshLocationOndelete: boolean;

    constructor() {
        super();
    }

    Removetildslashfromstring(str, char): any {
        var notildslash = "";
        var newstr = str.split(char);
        for (var i = 0; i < newstr.length; i++) {
            notildslash += newstr[i];
        }
        return notildslash;
    }

    Validate(): any {
        var Locales = [];
        $(".LocaleLabel").each(function () {
            Locales.push($(this).attr('localename'));
        });

        var flag = true;
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

        return flag;
    }
    GetAjaxHeaders(callBackFUnction): any {
        return Endpoint.prototype.GetAjaxHeaders(callBackFUnction);
    }

    GetPaymentAppHeader(callBackFUnction): any {
        return Endpoint.prototype.GetPaymentAppHeader(callBackFUnction);
    }

    //This function use for show tooltip in partial view.
    DisplayTooltip(): void {
        $('[data-toggle="tooltip"]').tooltip();
    }


    //This method is used to validate DisplayOrder/numeric field in Grid.
    ValidateDisplayOrderField(object): any {
        var isValid = true;
        var regex = new RegExp('^\\d{0,}?$');
        if (isNaN($(object).val())) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("RequiredNumericValue"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if (!regex.test($(object).val()) || $(object).val() == 0) {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("DisplayOrderRange"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else if ($(object).val() == '') {
            $(object).addClass("input-validation-error");
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("ErrorRequiredDisplayOrder"), 'error', isFadeOut, fadeOutTime);
            isValid = false;
        }
        else {
            $(object).remove("input-validation-error");
            $(object).removeClass("input-validation-error");
            isValid = true;
        }
        return isValid;
    }
}

$(document).on("paste keypress change", ":input:not(.AllowHtml) :not(.mce-textbox.mce-multiline)", function (e) {
    if ($(this).val() != null) {
        if ($(this).val().indexOf("~") != -1) {
            var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "~");
            $(this).val(_inputValue);
        }
        if ($(this).val().indexOf("<") != -1) {
            _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "<");
            $(this).val(_inputValue);
        }
        if ($(this).val().indexOf(">") != -1) {
            _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), ">");
            $(this).val(_inputValue);
        }
        /*new validation*/
        var key = [e.keyCode || e.which];
        if (key[0] != undefined) {
            if ((key == null) || (key[0] == 0) || (key[0] == 126) || (key[0] == 60) || (key[0] == 62)) {
                return false;
            }
        }
    }
})

$(document).on("change", "input[type=text]", function (e) {
    if ($(this).val()) {
        var _inputValue = $(this).val().trim();
        $(this).val(_inputValue);
    }
})

$(document).ajaxError(function (e, jqxhr, settings, exception) {
    ZnodeBase.prototype.HideLoader();
    e.stopPropagation();
    if (jqxhr != null) {
        if (jqxhr.status === 403) {
            // when statusText is error then returnUrl value is error. so returning pathname instead of error if statusText is error
            if (jqxhr.statusText.toLowerCase() === "error")
            {
                var pathName = (location.pathname == null || location.pathname == undefined) ? "" : location.pathname;
                var searchValue = (location.search == null || location.search == undefined) ? "" : location.search;
                window.location.href = "/User/Login?returnUrl=" + encodeURIComponent(pathName + searchValue);
            }
            else if (jqxhr.statusText != undefined) {
                window.location.href = "/User/Login?returnUrl=" + jqxhr.statusText;
            }
            else
                window.location.reload();
        }
    }
});

$('.noSubmitOnEnterKeyPress').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
//Loader on ajax request
$(document).ajaxStart(function () {
    ZnodeBase.prototype.pendingAjaxRequests = false;
    if (showGlobalLoader) {
        ZnodeBase.prototype.ShowLoader();
    }
    else {
        ZnodeBase.prototype.HideLoader();
    }
   
});
$(document).ajaxStop(function ()
{
    ZnodeBase.prototype.pendingAjaxRequests = false;
    ZnodeBase.prototype.HideLoader();
    if (ZnodeBase.prototype.pendingAjaxRequests === false && ZnodeNotification.prototype.queuedNotification) {
        ZnodeNotification.prototype.queuedNotification();
        ZnodeNotification.prototype.queuedNotification = null;
    }
});

$.ajaxSetup({
    error: function (x, e) {
        if (x.status === 403) {
            if (x.statusText != undefined) {
                window.location.href = "/User/Login?returnUrl=" + x.statusText;
            }
            else
                window.location.reload();
        }
    }
});



