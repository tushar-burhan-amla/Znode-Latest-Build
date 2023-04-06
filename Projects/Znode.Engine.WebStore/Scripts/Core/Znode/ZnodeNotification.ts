class ZnodeNotification extends ZnodeBase {
    constructor() {
        super();
    }
   
    DisplayNotificationMessages(): void {
        var element: any = $(".messageBoxContainer");
        if (element.length) {
            var msgObj: any = element.data('message');
            if (msgObj !== "") {
                this.DisplayNotificationMessagesHelper(msgObj.Message, msgObj.Type, msgObj.IsFadeOut, msgObj.FadeOutMilliSeconds);
            }
        }
    }

    DisplayNotificationMessagesHelper(message: string, type: string, isFadeOut: boolean, fadeOutMilliSeconds: number): void {
        var element: any = $(".messageBoxContainer");
        $(".messageBoxContainer").removeAttr("style");
        var closeBtnHtml: string = "<span onclick='ZnodeNotification.prototype.CloseMessageNotificationContainer(this);' class='close pull-right right zf-close'></span>";
        $(window).scrollTop(0);
        $(document).scrollTop(0);
        if (element.length) {
            if (message !== "" && message != null) {
                element.html("<div class='message-box alert'><p class='text-center'>" + message + "</p>" + closeBtnHtml + "</div>");
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
            }
        }
    }

    CloseMessageNotificationContainer(messageContainer: any): void {
        $(messageContainer).parent("div").parent("div").hide();
    }
}