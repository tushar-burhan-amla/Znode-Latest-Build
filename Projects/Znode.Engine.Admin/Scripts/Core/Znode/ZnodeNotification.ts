class ZnodeNotification extends ZnodeBase {
    constructor() {
        super();
    }

    queuedNotification: any;

    pendingAjaxRequestsFinalized(): boolean {
        return !ZnodeBase.prototype.pendingAjaxRequests;
    }

    DisplayNotificationMessages(): void {
        this.queuedNotification = function () {
            var element: any = $(".messageBoxContainer");
            if (element.length) {
                var msgObj: any = element.data('message');
                if (msgObj !== "") {
                    this.DisplayNotificationMessagesHelper(msgObj.Message, msgObj.Type, msgObj.IsFadeOut, msgObj.FadeOutMilliSeconds);
                }
            }
        };

        if (this.pendingAjaxRequestsFinalized() === true && this.queuedNotification) {
            this.queuedNotification();
            this.queuedNotification = null;
        }
    }

    DisplayNotificationMessagesHelper(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        this.queuedNotification = function () {
            var element: any = $(".messageBoxContainer");
            $(".messageBoxContainer").removeAttr("style");
            ZnodeNotification.prototype.BindDisplayNotificationMessage(element, message, type, fadeOutMilliSeconds);
        };

        if (this.pendingAjaxRequestsFinalized() === true && this.queuedNotification) {
            this.queuedNotification();
            this.queuedNotification = null;
        }
    }

    DisplayNotificationMessagesHelperForAsidePopupPanel(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        this.queuedNotification = function () {
            var element: any = $('.aside-popup-panel').find(".messageBoxContainer");
            $('.aside-popup-panel').find(".messageBoxContainer").removeAttr("style");
            ZnodeNotification.prototype.BindDisplayNotificationMessage(element, message, type, fadeOutMilliSeconds);
        };

        if (this.pendingAjaxRequestsFinalized() === true && this.queuedNotification) {
            this.queuedNotification();
            this.queuedNotification = null;
        }
    }

    DisplayNotificationMessagesHelperForAsidePanel(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        this.queuedNotification = function () {
            var element: any = $('.panel-container').find('.error-msg');
            if (!element.length)
                element = $('.panel-container').find('.success-msg');
            $(window).scrollTop(0);
            $(document).scrollTop(0);   
            if (element.length) {
                if (message !== "" && message != null) {
                    element.text(message);
                    switch (type) {
                        case "success":
                            {
                                element.removeClass('error-msg').addClass('success-msg');
                                element.parent('div').show();
                                element.show();
                                break;
                            }
                        case "error":
                            {
                                element.removeClass('success-msg').addClass('error-msg');
                                element.parent('div').show();
                                element.show();
                                break;
                            }
                        default:
                            {
                                element.addClass('alert-info');
                                element.parent('div').show();
                                element.show();
                            }
                    }

                    if (isFadeOut == null || typeof isFadeOut === "undefined") isFadeOut = true;
                    if (fadeOutMilliSeconds == null || typeof fadeOutMilliSeconds === "undefined") fadeOutMilliSeconds = 10000;

                    if (isFadeOut == true) {
                        setTimeout(function () {
                            element.fadeOut().empty();
                        }, fadeOutMilliSeconds);
                    }
                }
            }
        };

        if (this.pendingAjaxRequestsFinalized() === true && this.queuedNotification) {
            this.queuedNotification();
            this.queuedNotification = null;
        }
    }

    BindDisplayNotificationMessage(element: any, message: string, type: string, fadeOutMilliSeconds: number): void {
        var closeBtnHtml: string = "<span onclick='ZnodeNotification.prototype.CloseMessageNotificationContainer(this);' class='close pull-right right z-close-circle'></span>";
        if (element.length) {
            if (message !== "" && message != null) {
                element.html("<div class='message-box alert'><p class='text-center' data-test-selector='popMessageBoxContainer'>" + message + "</p>" + closeBtnHtml + "</div>");
                switch (type) {
                    case "success":
                        {
                            element.find('div').addClass('alert-success');
                            break;
                        }
                    case "error":
                        {
                            element.find('div').addClass('alert-danger');
                            break;
                        }
                    default:
                        {
                            element.find('div').addClass('alert-info');
                        }
                }

                if (isFadeOut == null || typeof isFadeOut === "undefined") isFadeOut = true;
                if (fadeOutMilliSeconds == null || typeof fadeOutMilliSeconds === "undefined") fadeOutMilliSeconds = 10000;

                if (isFadeOut == true) {
                    setTimeout(function () {
                        element.fadeOut().empty();
                    }, fadeOutMilliSeconds);
                }

                if (ZnodeBase.prototype.getBrowser() == "Chrome") {
                    $("html,body").animate({ scrollTop: 0 }, "slow");
                }
                else {
                    $(window).scrollTop(0);
                    $(document).scrollTop(0);
                }
            }
        }
    }

    CloseMessageNotificationContainer(messageContainer: any): void {
        $(messageContainer).parent("div").parent("div").hide();
    }
}