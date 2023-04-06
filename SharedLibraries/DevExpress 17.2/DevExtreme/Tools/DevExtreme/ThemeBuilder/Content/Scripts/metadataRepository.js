(function(ThemeBuilder) {
    "use strict";

    ThemeBuilder.MetadataRepository = function(metadataLoader) {
        var repositoryData = {},
            utils = ThemeBuilder.Utils;

        var findDataItemInGroupItems = function(key, items) {
            var result = null;
            $.each(items, function(_, item) {
                if(item.Key === key) {
                    result = item;
                    return false;
                }
            });
            return result;
        };

        var findDataItemInGroups = function(key, groups) {
            var result = null;
            $.each(groups, function(name, items) {
                result = findDataItemInGroupItems(key, items);
                if(result)
                    return false;
            });
            return result;
        };

        var getDataItemByKey = function(key, theme) {
            var result = null;
            
            $.each(repositoryData[theme.name + "-" + theme.colorScheme], function(theme, groups) {
                result = findDataItemInGroupItems(key, groups);
                if(result)
                    return false;
            });
            return result;
        };

        this.init = function(themes) {
            var themesCount = themes.length,
                deferreds = utils.makeArrayOfDeferreds(themesCount),
                d = $.Deferred();

            for(var i = 0, len = themesCount; i < len; i++) {
                (function(i) { 
                    metadataLoader
                        .load(themes[i].name, themes[i].colorScheme)
                        .done(function(metadata) {
                            repositoryData[themes[i].name + "-" + themes[i].colorScheme] = metadata;
                            deferreds[i].resolve();
                        });
                })(i);
            }

            $.when.apply(null, deferreds).done(function() {
                d.resolve();
            });

            return d.promise();
        };

        this.getData = function(theme) {
            if(!theme)
                return repositoryData;

            return repositoryData[theme.name + "-" + theme.colorScheme];
        };

        this.updateData = function(data, theme) {
            $.each(data, function(_, item) {
                var dataItem = getDataItemByKey(item.key, theme);
                if(item)
                    dataItem.Value = item.value;
            });  
        };

    };

})(ThemeBuilder);