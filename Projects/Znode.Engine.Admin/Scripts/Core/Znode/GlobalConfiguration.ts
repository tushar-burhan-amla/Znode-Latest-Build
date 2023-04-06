class GlobalConfiguration extends ZnodeBase {
    _endpoint: Endpoint;
    _Model: any;
    _notification: ZnodeNotification;

    constructor() {
        super();
        this._endpoint = new Endpoint();
        this._notification = new ZnodeNotification();
    }

    DefaultSubmit(SelectedIdArr: string[], Controller: string, Action: string, Callback: string) {
        var action = "SetDefault";
        var ids = [];
        ids = MediaManagerTools.prototype.unique();

        if (ids.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else if (ids.length > 1)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAnyOneToSetAsDefault"), 'error', isFadeOut, fadeOutTime);
        else {
            this.submit(ids, action, Controller, Action, Callback);
        }
    }

    ActiveSubmit(SelectedIdArr: string[], Controller: string, Action: string, Callback: string) {
        var action = "SetActive";
        var ids = [];
        ids = MediaManagerTools.prototype.unique();
        if (ids.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else {
            this.submit(ids, action, Controller, Action, Callback);
        }
    }

    DeActivateSubmit(SelectedIdArr: string[], Controller: string, Action: string, Callback: string) {
        var action = "SetDeActive";
        var ids = [];
        ids = MediaManagerTools.prototype.unique();
        if (ids.length == 0)
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(ZnodeBase.prototype.getResourceByKeyName("SelectAtleastOneRecord"), 'error', isFadeOut, fadeOutTime);
        else {
            //ajax call.
            this.submit(ids, action, Controller, Action, Callback);
        }
    }

    submit(SelectedIdArr: string[], action: string, Controller: string, Action: string, Callback: any) {
        this._Model = { "SelectedIds": SelectedIdArr.toString(), "Action": action };
        var url = "/" + Controller + "/" + Action;
        this.GetGlobalSetting(url, this._Model, Controller, Callback);
    }

    GetGlobalSetting(url: string, model: any, controller: string, callback: any): void {
        Endpoint.prototype.SetGlobalConfigSetting(url, model, function (data) {
            if (data != "") {
                window.location.href = "/" + controller + "/" + callback;
                //window.location.assign("");               
            }
        });
    }

    RefreshCacheData(control): any {
        var applicationType: string = $(control).attr("name").trim();
        var cacheId: string = $(control).attr("id").split("-")[1];
        GlobalConfiguration.prototype.RefreshCache(cacheId, "");
    }

    RefreshCache(cacheId: string, domainIds: string): any {
        ZnodeBase.prototype.ShowLoader();
        $.ajax({
            url: "/GeneralSetting/RefreshCache?id=" + cacheId + "&domainIds=" + domainIds,
            type: 'POST',
            success: function (response) {
                ZnodeBase.prototype.HideLoader();
                if ($("#hdnApplicationType").val() == Constant.Cloudflare) {
                    $('#grid tr').each(function () {
                        if ($(this).find("input[type=checkbox]").attr('id') != "check-all") {
                            $(this).closest('tr').append("<td id='status_" + $(this).find("input[type=checkbox]").attr('id').split('_')[1] + "'></td>");
                            $(this).find('td:last').remove();
                        }
                    });
                    $(response.Data.CloudflareErrorList).each(function () {
                        if (this.Status)
                            $("#status_" + this.DomainId).html("Refreshed");
                        else
                            $("#status_" + this.DomainId).html("Not Refreshed");
                    });
                }
                else {
                    if (!response.HasError) {
                        $("#startDate_" + cacheId).text(response.Data.StartDate);
                        $("#hdnstartDate_" + cacheId).val(response.Data.StartDate);
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "success", isFadeOut, fadeOutTime);
                    }
                    else {
                        ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.Message, "error", isFadeOut, fadeOutTime);
                    }
                }
            }
        });
    }

    UpdateFullPageCacheParameters(): any {
        var domainIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (domainIds == "") {
            $("#urlId").show();
        }
        else {
           //var cacheId: string = $("#div-FullPageCache").find('input:button').attr("id").split("-")[1];
            var cacheId: string = $("#hdnApplicationCacheId").val();
            GlobalConfiguration.prototype.RefreshCache(cacheId, domainIds);
            $("#urlId").hide();
            if ($("#hdnApplicationType").val() != Constant.Cloudflare)
                ZnodeBase.prototype.CancelUpload("domainList");
        }
    }

    GetDomains(control): any {
        $("#hdnApplicationCacheId").val($(control).attr("id").split("-")[1]);
        ZnodeBase.prototype.BrowseAsidePoupPanel('/GeneralSetting/GetWebstoreDomains', 'domainList');
    }

    GetCloudflareDomains(control): any {
        $("#hdnApplicationCacheId").val($(control).attr("id").split("-")[1]);
        $("#hdnApplicationType").val(Constant.Cloudflare);
        ZnodeBase.prototype.BrowseAsidePoupPanel('/GeneralSetting/GetWebstoreDomainsForCloudflare', 'domainList');
    }
}
