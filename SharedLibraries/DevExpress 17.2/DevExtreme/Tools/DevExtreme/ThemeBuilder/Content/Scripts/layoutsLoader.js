(function(ThemeBuilder) {
    "use strict";

    ThemeBuilder.LayoutsLoader = {
        load: function(layoutName) {
            return $.Deferred(function(d) {

                window.layoutLoaded = function(content) {
                    d.resolve(content);
                };

                $.ajax({
                    url: "Content/Data/Layouts/" + layoutName + ".html.js",
                    dataType: "jsonp",
                    crossDomain: true,
                    contentType: "application/json",
                    jsonpCallback: "layoutLoaded"
                }).always(function(obj, textStatus) {
                    // NOTE: Script execution won't reach this point in case of 200-th response status
                    // So here we can safely reject the deferred instance
                    d.reject.apply(d, arguments);
                });

            }).promise();
        }
    }


})(ThemeBuilder);