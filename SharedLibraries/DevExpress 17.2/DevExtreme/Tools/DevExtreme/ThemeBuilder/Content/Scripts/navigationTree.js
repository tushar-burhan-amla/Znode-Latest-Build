(function(ThemeBuilder) {
    "use strict";

    var NavigationTree = function() {

        var getTreeItemByKey = function(data, key) {
            for(var i = 0, len = data.length; i < len; i++) {
                if(data[i].key === key)
                    return data[i];
            }
            return false;
        };

        var getDataByKey = function(data, key) {
            var result = null;
            $.each(data, function(_, item) {
                if(item.Key === key) {
                    result = item;
                    return false;
                }
            });
            return result;
        };

        this.getTreeItemDataByKey = function(items, key) {
            var result = null;
            $.each(items, function(index, item) {
                if(item.data) {
                    result = getDataByKey(item.data, key);
                    if(result)
                        return false;
                }
                if(result)
                    return false;
            });

            return result;
        };

        var addHueForMobile = function(data) {
            if(data.length) return;
            data.push({ Name: "Main Theme Color", Type: "color", hue: true, IsLastSubGroupItem: false });
        };

        var createItem = function(key, data, parent, tree) {
            var child = {
                key: key,
                parentKey: parent ? parent.key : null,
                data: data,
                formOpened: ko.observable(key === "basetheming" ? true : false),
                active: ko.observable(key === "basetheming" ? true : false)
            };

            tree.push(child);
            return child;
        };

        var createAdvancedTheming = function(metadata, tree) {
            var that = this;
            var themingRoot = createItem.call(that, "advtheming", null, null, tree);

            $.each(metadata, function(name, item) {
                var parts = name.split("."),
                    groupName = parts[0],
                    childName = parts[1];

                var parent = themingRoot;

                if(!childName) {
                    parent = createItem.call(that, name, item, parent, tree);
                } else {
                    var groupParent = getTreeItemByKey(tree, groupName);
                    if(!groupParent)
                        parent = createItem.call(that, groupName, null, themingRoot, tree);
                    else
                        parent = groupParent;

                    createItem.call(that, name, item, parent, tree);
                }
            });
        };

        var createBaseTheming = function(metadata, tree) {
            var that = this;
            var data = [];
            var baseParameters = ["@base-accent", "@base-text-color", "@base-bg", "@base-border-color", "@base-border-radius"];

            $.each(baseParameters, function(index, parameterName) {
                var parameterData = that.getTreeItemDataByKey(tree, parameterName);
                if(parameterData) {
                    parameterData.Name = index + parameterData.Name;
                    data[index] = parameterData;
                }
            });

            addHueForMobile(data);

            createItem.call(that, "basetheming", data, null, tree);
        };

        this._tree = [];

        this._makeTree = function(metadata, mapFn) {
            var tree = [], that = this;

            createAdvancedTheming.call(that, metadata, tree);
            createBaseTheming.call(that, metadata, tree);

            if($.isFunction(mapFn))
                tree = mapFn(tree);

            this._tree = tree;
            return tree;
        };

        this.create = function(data, mapFn) {
            return this._makeTree(data, mapFn);
        };

    };

    ThemeBuilder.NavigationTree = NavigationTree;

})(ThemeBuilder);