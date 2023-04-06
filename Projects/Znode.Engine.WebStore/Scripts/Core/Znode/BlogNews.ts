class BlogNews extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
    }
    SavedCommentSuccessMessage(response): any {
        ZnodeNotification.prototype.DisplayNotificationMessagesHelper("Comment added successfully.", "success", false, 0);
        $("#BlogNewsComment").val("");
        Endpoint.prototype.GetUserCommentList($("#BlogNewsId").val(), function (response) {
            $("#comments-display-section").html('');
            $('#comments-display-section').show();
            $("#comments-display-section").html(response);
        });
        return true;
    }
}