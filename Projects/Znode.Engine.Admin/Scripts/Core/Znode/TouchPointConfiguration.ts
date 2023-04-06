class TouchPointConfiguration extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        TouchPointConfiguration.prototype.ShowLogDetails();
    }

    ShowLogDetails(): any {
        $("#TouchPointSchedulerHistory tbody tr td").find(".z-history").each(function () {
            $(this).removeAttr("href");
        });

        $("#grid tbody tr td").find(".z-history").click(function (e) {
            var schedulerName = $(this).attr("data-parameter").split('&')[0].split('=')[1];
            var recordId = $(this).attr("data-parameter").split('&')[1].split('=')[1];
            ZnodeBase.prototype.ShowLoader();
            Endpoint.prototype.ShowTaskSchedularLogDetails(schedulerName,recordId,function (res) {
                $("#divShowLogDetails").show(700);
                $("#divShowLogDetails").html(res);
                ZnodeBase.prototype.HideLoader();
                $("body").css('overflow', 'hidden');
                $("body").append("<div class='modal-backdrop fade in'></div>");
            });
        });
    }
}
