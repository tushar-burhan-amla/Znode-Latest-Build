class StoreExperience extends ZnodeBase {
    PublishStoreCMSContent(): any {
        let publishStateFormData: string = 'NONE';
        let publishContentFormData: string = 'StoreSettings,CmsContent';
        
        if ($('#radBtnPublishState').length > 0)
            publishStateFormData = ZnodeBase.prototype.mergeNameValuePairsToString($('#radBtnPublishState').serializeArray());        

        Endpoint.prototype.PublishStoreCMSContent($("#HdnStoreId").val(), publishStateFormData, publishContentFormData, function (res) {
            ZnodeNotification.prototype.DisplayNotificationMessagesHelper(res.message, res.status ? 'success' : 'error', isFadeOut, fadeOutTime);
            ZnodeProgressNotifier.prototype.InitiateProgressBar(function () {
                DynamicGrid.prototype.RefreshGridNoNotification($("#ZnodeStoreExperience").find("#refreshGrid"));
            });
        });
    }
}