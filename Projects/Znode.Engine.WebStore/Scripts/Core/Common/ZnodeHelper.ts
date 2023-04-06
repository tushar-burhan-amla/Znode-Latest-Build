/// <reference path="../../typings/jquery.cookie/jquery.cookie.d.ts" />

class CommonHelper extends ZnodeBase {
    toggleMessage: boolean;
    toggleTitle: boolean;
    RefreshLocationOndelete: boolean;

    constructor() {
        super();
    }

    BlockHtmlTagForTextBox(): any {
        /* validated all text box in project*/
        $(':input').not('.AllowHtml').on("paste keypress change", function (e) {
            if ($(this).val().indexOf("~") != -1) {
                var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "~");
                $(this).val(_inputValue);
            }
            if ($(this).val().indexOf("<") != -1) {
                var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "<");
                $(this).val(_inputValue);
            }
            if ($(this).val().indexOf(">") != -1) {
                var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), ">");
                $(this).val(_inputValue);
            }
            /*new validation*/
            var key = [e.keyCode || e.which];
            if (key[0] != undefined) {
                if ((key == null) || (key[0] == 0) || (key[0] == 126) || (key[0] == 60) || (key[0] == 62)) {
                    return false;
                }
            }
        });

        /* validated all text box in project*/
        $(':input').not('.AllowHtml').on("paste", function (e) {
            if ($(this).attr('data-datype') === "Int32" || $(this).attr('data-datype') === "Decimal") {
                return false;
            }
        });

        $(":input").not('.AllowHtml').on("keypress", function (e) {
            if (e.which === 32 && !this.value.length)
                e.preventDefault();
        });
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
                $("#error" + Locales[i]).html("Error");
                flag = false;
            }
        }

        return flag;
    }

    GetAjaxHeaders(callBackFUnction): any {
        return Endpoint.prototype.GetAjaxHeaders(callBackFUnction);
    }

    GetPaymentAppHeader(callBackFUnction): any {
        var paymentApiHeaderResponseValue = $("#hdnPaymentApiResponseHeader").val();
        if (paymentApiHeaderResponseValue) {
            var response = {};
            response["Authorization"] = paymentApiHeaderResponseValue ;
            return callBackFUnction(response);
        }
        return Endpoint.prototype.GetPaymentAppHeader(callBackFUnction);
    }

    RemovePostFixAfterFacebookSocialLogin(): void {
        if (window.location.hash && window.location.hash == '#_=_') {
            if (window.history && history.pushState) {
                window.history.pushState("", document.title, window.location.pathname + window.location.search);
            }
        }
    }
}

$(document).on("paste keypress change", ":input", function (e) {
    if ($(this).val().indexOf("~") != -1) {
        var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "~");
        $(this).val(_inputValue);
    }
    if ($(this).val().indexOf("<") != -1) {
        var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), "<");
        $(this).val(_inputValue);
    }
    if ($(this).val().indexOf(">") != -1) {
        var _inputValue = CommonHelper.prototype.Removetildslashfromstring($(this).val(), ">");
        $(this).val(_inputValue);
    }
    /*new validation*/
    var key = [e.keyCode || e.which];
    if (key[0] != undefined) {
        if ((key == null) || (key[0] == 0) || (key[0] == 126) || (key[0] == 60) || (key[0] == 62)) {
            return false;
        }
    }
})

$(document).ajaxError(function (e, jqxhr, settings, exception) {
    e.stopPropagation();
    if (jqxhr != null) {
        if (jqxhr.status === 403) {
            if (jqxhr.statusText != undefined) {
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

$(document).off("change", "#ddlCulture")
$(document).on("change", "#ddlCulture", function () {
    $(this).closest("form").submit();
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



