/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../typings/jquery.validation/jquery.validation.d.ts" />
/// <reference path="../../typings/jquery.cookie/jquery.cookie.d.ts" />
var isFadeOut = true;
var fadeOutTime = 10000;
var CheckBoxCollection = new Array();
var UpdateContainerId;
import BaseModel = Znode.Core.BaseModel;
abstract class ZnodeBase {
    errorAsAlert: boolean;
    public ajaxRequest(url: string, method: string, parameters: any, successCallback: any, responseType: string, async: boolean = true) {
        if (!method) { method = Constant.GET; }
        if (!responseType) {
            responseType = Constant.json;
        }
        if (typeof successCallback != Constant.Function) {
            this.errorOutfunction(ErrorMsg.CallbackFunction);
        } else {
            $.ajax({
                type: method,
                url: url,
                async: async,
                data: this.cachestampfunction(parameters),
                dataType: responseType,
                success: function (response) {
                    successCallback(response);
                },
                error: function (xhr, thrownError) {
                    ZnodeBase.prototype.errorOutfunction(xhr.status);
                    ZnodeBase.prototype.errorOutfunction(thrownError);
                    ZnodeBase.prototype.errorOutfunction(ErrorMsg.APIEndpoint + url);
                }
            });
        }
    }
    private cachestampfunction(data): any {
        var d = new Date();
        if (typeof data == Constant.string) {
            data += "&_=" + d.getTime();
        } else if (typeof data == Constant.object) {
            data["_"] = d.getTime();
        } else {
            data = { "_": d.getTime() };
        }
        return (data);
    }

    // Error output
    public errorOutfunction(message): void {
        console.log(message);
        if (this.errorAsAlert) {
            alert(message);
        }
    }
    public executeFunctionByName(functionName, context, args, target = undefined): void {
        try {
            var args = [].slice.call(arguments).splice(2);
            var namespaces = functionName.split(".");
            var func = namespaces.pop();
            for (var i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            if (target !== undefined)
                return context[func].apply(this, args, target);
            else
                return context[func].apply(this, args);
        }
        catch (ex) {
            console.log(ErrorMsg.InvalidFunction + functionName);
        }
    }

    public executeInit(functionName, context, args): void {
        try {
            if (functionName != "Product.Init") {
                localStorage.removeItem("isFromCategoryPage");
            }

            var args = [].slice.call(arguments).splice(2);
            var namespaces = functionName.split(".");
            var func = namespaces.pop();
            for (var i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            var _contextObject = new context();
            return _contextObject[func].apply(this, args);
        }
        catch (ex) {
            console.log("Invalid function name " + functionName);
        }
    }

    // Onready method runs when document is loaded and ready
    public onready(): void {
        // Set the controller and action names
        var modules = $("body").data("controller").split(".");
        var action = $("body").data("view");
        // Loads modules based on controller and view
        // If init() methods are present, they will be run on load
        modules.forEach(function (module) {
            if (module !== 'undefined') {
                var functionName = module + ".Init";
                ZnodeBase.prototype.executeInit(functionName, window, arguments);
            }
        });
    }

    //get cookies value by name.
    public getCookie(cname): string {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
    //set cookies value
    public setCookie(cname, cvalue, exdays): void {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    }

    //get locale resource by key name.
    public getResourceByKeyName(keyname): string {
        var defaultculter = this.getCookie("culture");
        var resourceClassObj = Object.create(window[defaultculter].prototype);
        resourceClassObj.constructor.apply(resourceClassObj);
        return resourceClassObj[keyname];
    }
    public onImageError() {
        jQuery('img').on('error', function (e) {
            this.src = window.location.protocol + "//" + window.location.host + "/Content/Images/no-image.png";
        });
    }

    public ShowLoader(): void {
        $("#Single-loader-content-backdrop").show();
    }
    public HideLoader(): void {
        $("#Single-loader-content-backdrop").hide();
    }

    public IsiPad(): void {
        if ((navigator.userAgent.match(/iPad/i)) && (navigator.userAgent.match(/iPad/i) != null)) {
            var clickTimer = null;
            $(".navbar-nav li").find("a").on("click", function (e) {
                if ($(this).parent().find("ul").length > 0) {
                    e.preventDefault();
                    var control = this;
                    if (clickTimer == null) {
                        clickTimer = setTimeout(function () {
                            clickTimer = null;
                            $(control).parent().find("ul").show();
                        }, 500);
                    }
                    else {
                        clearTimeout(clickTimer);
                        clickTimer = null;
                        window.location.href = $(control).attr("href");
                    }
                }
            });
        }
    }

    //Get google geo-locator api
    GetGeoLocatorAPI(): string {
        return Constant.gocoderGoogleAPI;
    }

    //Get google geo-locator api key
    GetGeoLocatorAPIKey(): string {
        return Constant.gocoderGoogleAPIKey;
    }

    //Method to Get Parameter Values
    GetParameterValues(param) {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == param) {
                return urlparam[1];
            }
        }
    }

    InitializeZnodeAjaxifier() {
        try { ZnodeAjaxify.prototype.Apply(); } catch (ex) { }
    }
}

$(window).on("load", function () {
    ZnodeBase.prototype.onready();
    ZnodeBase.prototype.HideLoader();
    CommonHelper.prototype.RemovePostFixAfterFacebookSocialLogin();
    ZnodeBase.prototype.IsiPad();
    ZnodeBase.prototype.InitializeZnodeAjaxifier();
});
ZnodeBase.prototype.onImageError();

$(document).off("contextmenu", "a[data-ajax=true]");
$(document).on("contextmenu", "a[data-ajax=true]", function () {
    return false;
})

