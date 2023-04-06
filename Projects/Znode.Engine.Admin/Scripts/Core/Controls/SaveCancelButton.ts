class SaveCancel extends ZnodeBase {
    SubmitForm(formid: string, callback: string, backURL: string) {
        ZnodeBase.prototype.ShowLoader();
        if (!$("#" + formid).valid() && $(".ui-tabs").length > 0) {
            if ($("#" + formid + " .input-validation-error").closest(".ui-tabs-panel").length > 0) {
                var tabId = $("#" + formid + " .input-validation-error").closest(".ui-tabs-panel").get(0).id;
                $(".ui-tabs ul").find("[aria-controls='" + tabId + "']").find('a').click();
            }
        }

        ZnodeBase.prototype.setCookie("CurrentUrl", window.location.href, 1);
        if (typeof (backURL) != "undefined" && $("#" + formid).valid())
            $.cookie("_backURL", backURL, { path: '/' });

        if (typeof callback !== 'undefined' && callback != null && callback !== "") {
            var checkStatus;
            var boolResult = ZnodeBase.prototype.executeFunctionByName(callback, window, null);

            if (!boolResult) {
                ZnodeBase.prototype.HideLoader();
                return;
            }
        }

        $('#' + formid).submit();
        ZnodeBase.prototype.HideLoader();
    }

    Cancel() {
        if (document.referrer.indexOf(window.location.hostname) != -1) {
            var referrer = document.referrer;
            if (referrer.indexOf("returnUrl") >= 0) {
                window.location.href = '/Dashboard/Dashboard';
            }
            else {
                window.location.replace(referrer);
            }
        }
        else {
            window.location.href = '/Dashboard/Dashboard';
        }
    }
}




