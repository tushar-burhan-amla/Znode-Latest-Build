(function(ThemeBuilder){
    "use strict";

    ThemeBuilder.MetadataLoader = function() {       
        this.groupMetadata = function(metadata) {
            var groups = {};
            $.each(metadata, function(i, item) {
                if(!groups[item.Group])
                    groups[item.Group] = [];

                groups[item.Group].push(item);
            });

            return groups;
        };

        this.load = function(theme, colorScheme) {
            var d = $.Deferred(),
                that = this,
                metadata = ThemeBuilder["__" + theme + "_" + colorScheme.replace("-", "_") + "_metadata"]();

            d.resolve(that.groupMetadata(metadata));

            return d.promise();
        };
    };

})(ThemeBuilder);