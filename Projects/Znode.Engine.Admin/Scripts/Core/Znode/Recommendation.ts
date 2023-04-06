class Recommendation extends ZnodeBase {
    GenerateRecommendationData(portalId: number, isBuildPartial: string): any {
        $.ajax({
            url: "/Recommendation/GenerateRecommendationData?portalId=" + portalId + "&isBuildPartial=" + isBuildPartial,
            type: 'POST',
            success: function (response) {
                if (!response.hasError) {                    
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                }
                else {
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
                }
            }
        });
    }

    GetRecommendationSchedulerView(touchPointName: string, portalId: number): any {
        $("#createSchedulerError").hide();
        var header = "<button type='button' class='popup-panel-close' onclick='ZnodeBase.prototype.CancelUpload('divCreateSchedulerForRecommendation')'><i class='z-close'></i></button>";
        ZnodeBase.prototype.ShowLoader();
        var url = "/Recommendation/CreateScheduler?ConnectorTouchPoints=" + touchPointName + "&schedulerCallFor=RecommendationDataGenerationHelper&portalId=" + portalId;
        Endpoint.prototype.GetPartial(url, function (response) {
            if (response != "") {
                var htmlContent = header + response;
                $("#divCreateSchedulerForRecommendation").html(htmlContent);
                $($("#divCreateSchedulerForRecommendation").find("a.grey")).attr("href", "#");
                $($("#divCreateSchedulerForRecommendation").find("a.grey")).attr("onclick", "ZnodeBase.prototype.CancelUpload('divCreateSchedulerForRecommendation')");
                $("#divCreateSchedulerForRecommendation").show();
                $("body").append("<div class='modal-backdrop fade in'></div>");
            }
            else {
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("TouchPointNameRequired"), response.status ? 'success' : 'error', isFadeOut, fadeOutTime);                
            }
            ZnodeBase.prototype.HideLoader();
        });
    }

    CreateScheduler(): any {
        var isValid: boolean = SearchConfiguration.prototype.ValidateSchedulerData();
        var schedulerName: string = $("#SchedulerName").val();
        if (isValid) {
            var erpTaskSchedulerViewModel = {
                "ERPTaskSchedulerId": $("#ERPTaskSchedulerId").val(),
                "IndexName": $("#IndexName").val(),
                "IsEnabled": $("#divSchedulerSetting #IsActive").prop('checked'),
                "SchedulerCallFor": $("#SchedulerCallFor").val(),
                "PortalId": $("#PortalId").val(),
                "PortalIndexId": $("#PortalIndexId").val(),
                "SchedulerFrequency": $('[name=SchedulerFrequency]:checked').val(),
                "SchedulerName": $("#SchedulerName").val(),
                "StartDate": $("#StartDate").val(),
                "StartTime": $("#StartTime").val(),
                "TouchPointName": $("#TouchPointName").val(),
                "SchedulerType": $("#SchedulerType").val(),
                "CronExpression": $("#txtCronExpression").val(),
                "HangfireJobId": $("#HangfireJobId").val()
            };

            if (parseInt($("#ERPTaskSchedulerId").val(), 10) > 0) {
                Endpoint.prototype.EditSearchScheduler(erpTaskSchedulerViewModel, function (response) {
                    if (response.status) {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SchedulerUpdatedSuccessfully"), response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        ZnodeBase.prototype.CancelUpload('divCreateSchedulerForRecommendation');
                    }
                    else {
                        $("#createSchedulerError").text(response.message);
                        $("#createSchedulerError").show();
                    }
                });
            }
            else {
                Endpoint.prototype.CreateSearchScheduler(erpTaskSchedulerViewModel, function (response) {
                    if (response.status) {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SchedulerCreatedSuccessfully"), response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
                        ZnodeBase.prototype.CancelUpload('divCreateSchedulerForRecommendation');
                        $("#schedulerNameText").val(schedulerName);
                        $("#schedulerName").removeClass("hidden");
                        $(".createScheduler").html("");
                        $(".createScheduler").html("<i class='z-add-circle'></i>" + ZnodeBase.prototype.getResourceByKeyName("UpdateScheduler"));
                        $("#RecommendationScheduler").val(ZnodeBase.prototype.getResourceByKeyName("UpdateScheduler"));                        
                    }
                    else {
                        $("#createSchedulerError").text(response.message);
                        $("#createSchedulerError").show();
                    }
                });
            }
        }
    }
}