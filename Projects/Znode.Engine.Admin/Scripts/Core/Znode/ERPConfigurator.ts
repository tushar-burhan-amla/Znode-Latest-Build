class ERPConfigurator extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
    }

    Delete(control): any {
        var ERPConfiguratorIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (ERPConfiguratorIds.length > 0) {
            Endpoint.prototype.DeleteERPConfigurator(ERPConfiguratorIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    DeleteERPTaskScheduler(control): any {
        var ERPTaskSchedulerIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (ERPTaskSchedulerIds.length > 0) {
            Endpoint.prototype.DeleteERPTaskScheduler(ERPTaskSchedulerIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    AssignTouchPointToActiveERP(control): any {
        ZnodeBase.prototype.ShowLoader();
        var touchPointNames = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (touchPointNames.length > 0) {
            Endpoint.prototype.AssignTouchPointToActiveERP(touchPointNames, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
                Endpoint.prototype.AssignedTouchPointList(function (response) {
                    $("#View_ZnodeTouchPointConfiguration").html('');
                    $("#View_ZnodeTouchPointConfiguration").html(response);
                });
                $("#UnassignedTouchPointsAsidePannel").hide(700);
                ZnodeBase.prototype.RemovePopupOverlay();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? "success" : "error", isFadeOut, fadeOutTime);
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeBase.prototype.HideLoader();
                ZnodeBase.prototype.RemoveAsidePopupPanel();
                ZnodeBase.prototype.HideLoader();
            });
        }
        else {
            $("#asidePannelmessageBoxContainerId").show();
            ZnodeBase.prototype.HideLoader();
        }
    }

    //Method for Show/Hide Save Cancel Button
    ShowHideSaveCancelButton() {
        if ($("#UnassignedTouchPointsAsidePannel").find("tr").length > 0)
            $("#divSave").show();
        else
            $("#divSave").hide();
    }

    RemoveHrefAttribute(): void {
        $(".z-view").removeAttr("href");

        $(document).off("click", ".z-view");
        $(document).on("click", ".z-view", function (e) {
            ZnodeBase.prototype.ShowLoader();
            e.preventDefault();
            Import.prototype.ShowLogDetailsInPopup($(this));
            return false;
        });
    }
}

$(document).on('click', "#SchedulerFrequency", function (e) {
    var SchedulerFrequencyValue = $(this).val();
    var url = null;
    switch (SchedulerFrequencyValue) {
        case "OneTime":
            url = "/TouchPointConfiguration/OneTime";
            break;
        case "Recurring":
            url = "/TouchPointConfiguration/Recurring";
            break;
    }
    if (url) {
        Endpoint.prototype.GetSchedulerFrequency(url, function (response) {
            $("#divSchedulerFrequency").html('');
            $("#divSchedulerFrequency").html(response);
            $("#createSchedulerError").empty();

            switch (SchedulerFrequencyValue) {
                case "OneTime":
                    var undefinedDateTime = '1/1/1900 12:00:00 AM';
                    if ($("#hdnStartDate").val().trim() != '' && $("#hdnStartDate").val() != undefinedDateTime) {
                        var startDate = new Date($("#hdnStartDate").val());
                        //Get Date format from global setting 
                        Endpoint.prototype.GetDateFormatGlobalSetting(function (result) {
                            $("#StartDate").val(moment(startDate.toLocaleDateString()).format(result.dateFormat.toUpperCase()));
                        });
                           //Get time format from global setting
                        $("#StartTime").val(startDate.toLocaleTimeString(navigator.language, { hour: '2-digit', minute: '2-digit' }));
                    }                    
                    break;
                case "Recurring":
                    $("#txtCronExpression").val($("#hdnCronExpr").val());
                    break;
            }
        });
    }
});

$(document).on('click', "#SchedulerType", function (e) {
    var SchedulerType = $(this).val();
    if (SchedulerType == "Scheduled") {
        $("#divSchedulerSetting").show();
    } else {
        $("#StartDate").val(new Date().toDateString());
        $("#StartTime").val("00:00 AM");
        $("#divSchedulerSetting").hide();
    }
});



