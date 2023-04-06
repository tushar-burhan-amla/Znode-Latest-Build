(function(ThemeBuilder) {
    "use strict";

    var ParentIframeNotifier = function() {
        this.notify = function(action, actionData) {
            window.top.postMessage(JSON.stringify({
                action: action,
                actionData: actionData
            }), "*");
        };
    };

    ThemeBuilder.ParentIframeNotifier = ParentIframeNotifier;

})(ThemeBuilder);
