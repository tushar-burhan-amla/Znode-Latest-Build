(function(ThemeBuilder) {
    "use strict";

    var PreviewIframeNotifier = function() {
        this.notify = function($iframe, action, actionData) {
            $iframe.get(0).contentWindow.postMessage(JSON.stringify({
                action: action,
                actionData: actionData
            }), "*");
        };
    };

    ThemeBuilder.PreviewIframeNotifier = PreviewIframeNotifier;

})(ThemeBuilder);
