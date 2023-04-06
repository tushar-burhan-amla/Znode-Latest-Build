var selectedTab: string;

class BlogNews extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        if (parseInt($("#BlogNewsId").val(), 10) <= 0) {
            $("#ddlCulture").prop("disabled", true);
            $("#ddlCulture").addClass("disabled");
        }
        BlogNews.prototype.EnableSelectedTab();
    }

    GetPortalList(): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/BlogNews/GetPortalList', 'divBlogNewsStoreList');
    }

    //To Do: To bind portal information
    OnSelectPortalResult(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            $('#StoreName').val(portalName);
            Store.prototype.OnSelectStoreAutocompleteDataBind(item);
        }
    }

    //This method is used to select store from list and show it on textbox.
    GetPortalDetail(): void {
        $("#grid").find("tr").on("click", function () {
            let portalName: string = $(this).find("td[class='storecolumn']").text();
            let portalId: string = $(this).find("td")[0].innerHTML;
            $('#txtPortalName').val(portalName);
            $('#StoreName').val(portalName);
            $('#hdnPortalId').val(portalId);
            $('#PortalId').val(portalId);
            $("#errorRequiredStore").text("").removeClass("field-validation-error").hide();
            $("#txtPortalName").removeClass('input-validation-error');
            $('#divBlogNewsStoreList').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
        });
    }

    GetContentPageList(portalId: number, localeId: number): any {
        ZnodeBase.prototype.BrowseAsidePoupPanel('/BlogNews/GetContentPageList?portalId=' + portalId + "&localeId=" + localeId + '', 'divBlogNewsContentPageList');
    }

    //This method is used to select content page from list and show it on textbox.
    GetContentPageDetail(): void {
        $("#grid").find("tr").on("click", function () {
            var pageName: string = $(this).find("td[class='pagenamecolumn']").text();
            var cmsContentPagesId: string = $(this).find("td")[0].innerHTML;
            $('#txtPageName').val(pageName);
            $('#hdnCMSContentPagesId').val(cmsContentPagesId);
            $('#CMSContentPagesId').val(cmsContentPagesId);
            $('#divBlogNewsContentPageList').hide(700);
            ZnodeBase.prototype.RemovePopupOverlay();
            $('#divBlogNewsContentPageList').html("");
        });
    }

    //Validations for blog/news.
    Validate(): boolean {
        //Checks validation for store.
        if ($("#txtPortalName").is(':visible')) {
            if ($("#txtPortalName").val() == "") {
                $("#errorRequiredStore").text('').text(ZnodeBase.prototype.getResourceByKeyName("ErrorSelectPortal")).addClass("field-validation-error").show();
                $("#txtPortalName").parent("div").addClass('input-validation-error');
                return false;
            }
        }
        return true;
    }

    //Delete blog(s)/news by blog/news id.
    DeleteBlogNews(control): any {
        var blogNewsId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (blogNewsId.length > 0) {
            Endpoint.prototype.DeleteBlogNews(blogNewsId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Show selected tab contents.
    EnableTab(id: string): any {
        selectedTab = id;
        if (id == "div-ContentPage") {
            $('#' + id).show();
            $('#div-loginForm').hide();
            $("#BlogNewsTab").parent("li").removeClass("tab-selected");
            $("#ContentPageTab").parent("li").addClass("tab-selected");
        }
        else {
            $('#div-ContentPage').hide();
            $('#div-loginForm').show();
            $("#BlogNewsTab").parent("li").addClass("tab-selected");
            $("#ContentPageTab").parent("li").removeClass("tab-selected");
        }
        $("#SelectedTab").val(selectedTab);
    }

    //Set cookies on locale change.
    DdlCultureChange() {
        var expiresTime: Date = ZnodeBase.prototype.SetCookiesExpiry();
        $.cookie("_blogNewsCulture", $("#ddlCultureSpan").attr("data-value"), { expires: expiresTime });
        var url = decodeURIComponent(window.location.href);
        var orignalUrl = url.split(/[?#]/)[0];
        if (selectedTab != undefined)
            window.location.replace(orignalUrl + "?blogNewsId=" + $("#BlogNewsId").val() + "&selectedtab=" + selectedTab);
        else {
            if (url.indexOf('blogNewsId') > -1)
                window.location.replace(orignalUrl + "?blogNewsId=" + $("#BlogNewsId").val());
            else
                window.location.reload();
        }
    }

    //Clears content page details.
    ClearContentPageName(): boolean {
        $('#txtPageName').val(undefined);
        $('#hdnCMSContentPagesId').val(undefined);
        $('#CMSContentPagesId').val(undefined);
        return false;
    }

    //Delete blog/news comment by blog/news id.
    DeleteBlogNewsComment(control): any {
        var blogNewsCommentId = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (blogNewsCommentId.length > 0) {
            Endpoint.prototype.DeleteBlogNewsComment(blogNewsCommentId, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    //Activate/Deactivate blog(s)/news
    ActivateDeactivateBlogNews(isActive: boolean): void {
        let activity = "IsActive";
        var blogNewsIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (blogNewsIds.length > 0) {
            Endpoint.prototype.ActivateDeactivateBlogNews(blogNewsIds, isActive, activity, function (response) {
                $("#ZnodeBlogNewsList #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }

    //Allow/Deny blog(s)/news guest comments.
    ActiveDeactiveGuestCommentsBlogNews(isAllowGuestComment: boolean): void {
        let activity = "IsAllowGuestComment";
        var blogNewsIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (blogNewsIds.length > 0) {
            Endpoint.prototype.ActivateDeactivateBlogNews(blogNewsIds, isAllowGuestComment, activity, function (response) {
                $("#ZnodeBlogNewsList #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }

    //Approve/Disapprove blog/news comment(s).
    ApproveDisapproveBlogNewsComment(isApproved: boolean): void {
        let action = "IsAllowGuestComments";
        var blogNewsCommentIds: string = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (blogNewsCommentIds.length > 0) {
            Endpoint.prototype.ApproveDisapproveBlogNewsComment(blogNewsCommentIds, isApproved, function (response) {
                $("#ZnodeBlogNewsCommentList #refreshGrid").click();
                DynamicGrid.prototype.ClearCheckboxArray();
                ZnodeNotification.prototype.DisplayNotificationMessagesHelper(response.message, response.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            });
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }

    //Method to show selected tab data.
    EnableSelectedTab(): void {
        selectedTab = $("#SelectedTab").val();
        BlogNews.prototype.EnableTab(selectedTab);
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

    public PublishBlogNews(): void {  
        ZnodeBase.prototype.ShowLoader();
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());

        Endpoint.prototype.PublishBlogNewsPage($("#BlogNewsId").val(), publishStateFormData, 0, function (res) {
            DynamicGrid.prototype.RefreshGridOndelete($("#ZnodeBlogNewsList").find("#refreshGrid"), res);
        });
    }

    public PublishBlogNewsPopup(control): void {
        if (control != undefined || control != null) {
            control.attr("href", "#");
            $("#BlogNewsId").val($(control).attr("data-parameter").split('=')[1]);
        }
        else {
            $("#BlogNewsId").val($("#BlogNewsId").val());
        }
        $("#PublishBlogNewsPagePopdiv").modal('show');
    }

    public UpdateAndPublishBlogNewsPage(control: any, formId: string, backURL: string): any {
        let publishStateFormData: string = 'NONE';
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());
        $("#" + formId + " [name=TargetPublishState]").val(publishStateFormData);
        $("#" + formId).attr("action", "UpdateAndPublishBlogNews");
        $("#" + formId).addClass("dirtyignore");
        SaveCancel.prototype.SubmitForm(formId, null, undefined);
        ZnodeBase.prototype.ShowLoader();
        $("#" + formId).removeClass("dirtyignore");
    }
}