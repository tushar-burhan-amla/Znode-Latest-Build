(function(ThemeBuilder) {
    "use strict";

    ThemeBuilder.PreviewDataLoader = function(){
        var utils = ThemeBuilder.Utils;

        var getGroups = function(groupedMetadata) {
            var result = [];
            $.each(groupedMetadata, function(theme, groups) {
                $.each(groups, function(name) {
                    if($.inArray(name, result) < 0)
                        result.push(name);
                });
            });

            return result;
        };

        this.load = function(groupedMetadata) {
            var groups = getGroups(groupedMetadata),
                countOfPreviewDeferreds = groups.length,
                previewDataDeferreds = utils.makeArrayOfDeferreds(countOfPreviewDeferreds),
                d = $.Deferred();

            for(var i = 0; i < countOfPreviewDeferreds; i++) {
                previewDataDeferreds[i].resolve(ThemeBuilder["__" + groups[i].replace(".", "_") + "_group"]());
            }

            $.when.apply(null, previewDataDeferreds).done(function() {
                d.resolve($.extend(true, [], arguments));
            });

            return d.promise();
        };
    };

})(ThemeBuilder);