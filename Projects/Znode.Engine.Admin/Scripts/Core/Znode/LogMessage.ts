class LogMessage extends ZnodeBase {
    _Model: any;
    _endPoint: Endpoint;
    _callBackCount = 0;
    isMoveFolder;
    localeId: number;
    constructor() {
        super();

    }
    Init() {
        ZnodeDateRangePicker.prototype.Init(LogMessage.prototype.DateTimePickerRange());
    }

    DateTimePickerRange(): any {
        var ranges = {
            'All Logs': [],
            'Last Hour': [],
            'Last Day': [],
            'Last 7 Days': [],
            'Last 30 Days': [],
        }
        return ranges;
    }

    ConfigureLogs(): any {
        $("#frmConfigureLogs").attr('action', 'ConfigureLogs');
        $("#frmConfigureLogs").submit();
    }

    PurgeLogs(): any {
        var logCategoryIds = $("#LogCategoryIdToBeDeleted").val();
        ZnodeBase.prototype.ShowLoader();
        $.ajax({
            url: "/LogMessage/PurgeLogs?logCategoryIds=" + logCategoryIds,
            type: 'POST',
            success: function (response) {
                ZnodeBase.prototype.HideLoader();
                if (response.status)
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "success", isFadeOut, fadeOutTime);
                else
                    ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, "error", isFadeOut, fadeOutTime);
            }
        });
    }

    DeleteLogsPopup(): any {
        $("#LogsDeletePopup").modal('show')
    }

    PurgeLogsPopup(zPublishAnchor) {
        if (zPublishAnchor != null) {
            zPublishAnchor.attr("href", "#");
            $("#LogCategoryIdToBeDeleted").val($(zPublishAnchor).attr('id'));
        }
        $("#LogsDeletePopup").modal('show');
    }

    SaveLoggingLevel(): any {
        $("#frmLoggingLevel").attr('action', 'LoggingLevelsList');
        $("#frmLoggingLevel").submit();
    }

    SelectAllLog(): any {
        if (!($("#IsLoggingLevelsEnabledAll").prop('checked'))) {
            $("#IsLoggingLevelsEnabledAll").prop('checked', false);
            $("#IsLoggingLevelsEnabledInfo").prop('checked', false);
            $("#IsLoggingLevelsEnabledWarning").prop('checked', false);
            $("#IsLoggingLevelsEnabledDebug").prop('checked', false);
            $("#IsLoggingLevelsEnabledError").prop('checked', false);
            $("#IsLoggingLevelsEnabledAll").prop('value', false);
            $("#IsLoggingLevelsEnabledInfo").prop('value', false);
            $("#IsLoggingLevelsEnabledWarning").prop('value', false);
            $("#IsLoggingLevelsEnabledDebug").prop('value', false);
            $("#IsLoggingLevelsEnabledError").prop('value', false);
        }
        else {
            $("#IsLoggingLevelsEnabledAll").prop('checked', true);
            $("#IsLoggingLevelsEnabledInfo").prop('checked', true);
            $("#IsLoggingLevelsEnabledWarning").prop('checked', true);
            $("#IsLoggingLevelsEnabledDebug").prop('checked', true);
            $("#IsLoggingLevelsEnabledError").prop('checked', true);
            $("#IsLoggingLevelsEnabledAll").prop('value', true);
            $("#IsLoggingLevelsEnabledInfo").prop('value', true);
            $("#IsLoggingLevelsEnabledWarning").prop('value', true);
            $("#IsLoggingLevelsEnabledDebug").prop('value', true);
            $("#IsLoggingLevelsEnabledError").prop('value', true);
        }
    }

    CheckUncheck(control: any): any {     
        var id = "";
        if ($(control).prop('checked')) {
            id = $(control).attr("id")
            $("#" + id).prop('checked', true);
            $("#" + id).prop('value', true);
        }
        else {

            id = $(control).attr("id")
            $("#" + id).prop('checked', false);
            $("#" + id).prop('value', false);
        }

        var searchIDs = $("#loggingLevelDiv input:checkbox:not(:checked)").map(function () {
            return $(this).val();
        }).get();
        if (searchIDs.indexOf('false') > -1) {
            $("#IsLoggingLevelsEnabledAll").prop('checked', false);
            $("#IsLoggingLevelsEnabledAll").prop('value', false);
        }
        else {
            $("#IsLoggingLevelsEnabledAll").prop('checked', true);
            $("#IsLoggingLevelsEnabledAll").prop('value', true);
        }
        
    }
}