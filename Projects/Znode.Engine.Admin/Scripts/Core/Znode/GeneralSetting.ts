class GeneralSetting extends ZnodeBase {
    _endPoint: Endpoint;
    _notification: ZnodeNotification;
    constructor() {
        super();
        this._endPoint = new Endpoint();
        this._notification = new ZnodeNotification();
    }

    Init() {
    }

    EnablePublishStateMapping(): any {
        var publishStateMappingId = $("#HdnPublishStateMappingId").val();
        if (publishStateMappingId.length > 0) {
            Endpoint.prototype.EnableDisablePublishStateMapping(publishStateMappingId, true, function (res) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status != true ? 'error' : 'success', isFadeOut, fadeOutTime);
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodePublishStateApplicationTypeMapping").find("#refreshGrid"), res);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    DisablePublishStateMapping(): any {
        var publishStateMappingId = $("#HdnPublishStateMappingId").val();
        if (publishStateMappingId.length > 0) {
            Endpoint.prototype.EnableDisablePublishStateMapping(publishStateMappingId, false, function (res) {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status != true ? 'error' : 'success', isFadeOut, fadeOutTime);
                DynamicGrid.prototype.RefreshGridOndelete($("#ZnodePublishStateApplicationTypeMapping").find("#refreshGrid"), res);
            });
        }
        else {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        }
    }

    ToggleEnableDisableAction(): void {
        $('#grid tbody tr').each(function () {
            $(this).find("td").each(function () {
                if ($(this).hasClass('grid-action')) {
                    if ($(this).next().children().hasClass("z-active")) {
                        $(this).children().children("ul").children().find(".z-disable").parent().show();
                        $(this).children().children("ul").children().find(".z-enable").parent().hide();
                    }
                    else if ($(this).next().children().hasClass("z-inactive")) {
                        $(this).children().children("ul").children().find(".z-disable").parent().hide();
                        $(this).children().children("ul").children().find(".z-enable").parent().show();
                    }
                }
            });
            $(this).find("td.IsEnabled").each(function () {
                if ($(this).children("i").hasClass("z-active")) {
                    $(this).next().children().children("ul").children().find(".z-disable").parent().show();
                    $(this).next().children().children("ul").children().find(".z-enable").parent().hide();
                }
                else if ($(this).children("i").hasClass("z-inactive")) {
                    $(this).next().children().children("ul").children().find(".z-disable").parent().hide();
                    $(this).next().children().children("ul").children().find(".z-enable").parent().show();
                }
            });
        });
    }

    EnablePublishStateMappingPopup(zEnableAnchor): any {
        zEnableAnchor.attr("href", "#");
        $("#HdnPublishStateMappingId").val($(zEnableAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        $("#publishStateMappingEnable").modal('show');
    }
    DisablePublishStateMappingPopup(zEnableAnchor): any {
        zEnableAnchor.attr("href", "#");
        $("#HdnPublishStateMappingId").val($(zEnableAnchor).attr("data-parameter").split('&')[0].split('=')[1]);
        $("#publishStateMappingDisable").modal('show');
    }

    GetPublishHistoryPopup(zViewAnchor): any {
        zViewAnchor.attr("href", "#");
        let publishState: string = $(zViewAnchor).attr("data-parameter").split('&')[0].split('=')[1];
        ZnodeBase.prototype.BrowseAsidePoupPanel('/PublishHistory/List?publishState=' + publishState, 'divPublishHistoryListPopup');
    }
}
