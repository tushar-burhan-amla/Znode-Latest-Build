(function(ThemeBuilder) {
    "use strict";

    var ViewModel = function(metadataRepository, historyChangesManager, previewIframeNotifier, cssBeautifier) {
        var exportedMetadata = ko.observableArray(),
            updatePreviewStyles = true,
            updateIsModifiedState = true,
            initialKeyValuePairs = [],
            defaultHue = null,
            defaultWidgetGroup = ThemeBuilder.defaultWidgetGroup,
            metadataVersion = ThemeBuilder.metadataVersion,
            $previewIframe = $("#preview-panel-iframe"),
            that = this;

        ThemeBuilder.Utils.proxyEventsToFrame($previewIframe);

        this.supportFileReader = !!window.FileReader;

        var loadPreviewIframe = function(metadata) {
            $previewIframe.attr("src", "preview.html");
            var loaded = false;
            $(document).on("iframeready", function() {
                if(!loaded) {
                    previewIframeNotifier.notify($previewIframe, "load", {
                        metadata: metadata,
                        currentTheme: that.currentTheme(),
                        currentColorScheme: that.currentColorScheme(),
                        metadataRepositoryData: ThemeBuilder.metadataRepository.getData(),
                        group: ThemeBuilder.defaultWidgetGroup
                    });
                }
                loaded = true;
            });
        };

        var getExportedMetadataValueByKey = function(metadata, key) {
            var result = null;

            for(var i = 0, len = metadata.length; i < len; i++) {
                if(metadata[i].key === key) {
                    result = metadata[i].value;
                    break;
                }
            }

            return result;
        };

        var showFailureMessage = function(message) {
            DevExpress.ui.dialog.alert(message, "Theme Builder");
            this.metadataPopupButtonsVisible(false);
        };

        var makeExportedMetadataString = function(metadata) {
            this.exportedMetadataAsString(null);
            if(metadata) {
                var items = "",
                    themeId = "\"themeId\": \"" + metadata.themeId + "\"",
                    date = "\"date\": \"" + metadata.date + "\"",
                    hue = "\"hue\": " + (metadata.hue ? "\"" + metadata.hue + "\"" : null),
                    version = "\"version\": \"" + metadataVersion + "\"";

                if(metadata.items.length)
                    items = "\"items\": [" + metadata.items.join(",\n") + "]";

                this.exportedMetadataAsString("{" + [version, date, themeId, hue, items].join(",\n") + "}");
            }
        };

        var getLastTreeItemKey = function() {
            var lastItem = this.tree.slice(-1)[0];
            if(lastItem.data)
                return lastItem.data.slice(-1)[0].key;

            return null;
        };

        var _updatedData = [];
        var createDataItem = function(item) {
            _updatedData.push(item);
        };
        
        var updateTreeData = function(items, valueUpdateFunction, lastTreeItemKey, updateTree) {
            $.each(items, function(_, item) {
                if(item.data) {
                    for(var i = 0, len = item.data.length; i < len; i++) {
                        if(item.data[i].key === undefined) return;
                        var updatedValue = valueUpdateFunction.call(that, item.data[i]);
                        if(item.data[i].key === lastTreeItemKey) {
                            updatePreviewStyles = true;
                            createDataItem({ key: item.data[i].key, value: updatedValue });
                            metadataRepository.updateData(_updatedData, { name: that.currentTheme(), colorScheme: that.currentColorScheme() });

                            _updatedData = [];
                            //that.tree = items;
                        }

                        var previousValue = item.data[i].value();
                        item.data[i].value(updatedValue);

                        if(previousValue === updatedValue)
                            that.toolboxValueSubscribe(item, updatedValue);

                        createDataItem({ key: item.data[i].key, value: updatedValue});
                    }
                }
            });

            if(updateTree) this.tree(items);
        };

        var updateToolboxGroupsValues = function(valueUpdateFunction, updateTree) {
            var tree = $.extend(true, [], this.tree()),
                lastTreeItemKey = getLastTreeItemKey.call(this);

            updatePreviewStyles = false;
            updateTreeData.call(this, tree, valueUpdateFunction, lastTreeItemKey, !!updateTree);
        };

        var updateUndoButtonActivity = function(historyItemsCount, indexOfActivHistoryItem) {
            var isButtonDisabled = false;

            if(!historyItemsCount || !indexOfActivHistoryItem) {
                isButtonDisabled = true;
            }

            this.isUndoButtonDisabled(isButtonDisabled);
        };

        var updateRedoButtonActivity = function(historyItemsCount, indexOfActivHistoryItem) {
            var isButtonDisabled = false;

            if(!historyItemsCount || indexOfActivHistoryItem === historyItemsCount - 1) {
                isButtonDisabled = true;
            }

            this.isRedoButtonDisabled(isButtonDisabled);
        };

        var convertMetadataToKeyValuePairs = function(stringifyData) {
            var metadata = metadataRepository.getData({
                name: that.currentTheme(),
                colorScheme: that.currentColorScheme()
            });
            var result = [];

            $.each(metadata, function(i, group) {
                for(var j = 0, len = group.length; j < len; j++) {
                    if(stringifyData) {
                        if(group[j].isModified) {
                            result.push(JSON.stringify({ key: group[j].Key, value: group[j].Value}));
                        }
                    } else {
                        result.push({ key: group[j].Key, value: group[j].Value, isModified: group[j].isModified });
                    }
                }
            });

            return result;
        };

        var createHistoryChangesItem = function() {
            var result = {
                items: convertMetadataToKeyValuePairs.call(this),
                themeId: this.selectedThemeId(),
                hue: this.hue()
            };

            this.historyChangesManager.addItem(result);

            var historyItemsCount = this.historyChangesManager.getItems().length,
                indexOfActivHistoryItem = this.historyChangesManager.getCurrentIndex();

            updateUndoButtonActivity.call(this, historyItemsCount, indexOfActivHistoryItem);
            updateRedoButtonActivity.call(this, historyItemsCount, indexOfActivHistoryItem);
        };

        var doNotCreateHistoryItem = false;
        var useNewMetadata = function(metadata) {
            doNotCreateHistoryItem = true;
            this.updateTheme(getThemeById(Number(metadata.themeId)));

            if(metadata.hue)
                this.hue(metadata.hue);
            
            updateToolboxGroupsValues.call(this, function(item) {
                var value = getExportedMetadataValueByKey(metadata.items, item.key);
                value = value ? value : item.value();
                return value;
            }, true);
            doNotCreateHistoryItem = false;
            createHistoryChangesItem.call(this);
        };

        var backwardCompatibililyForVariables = function(metadata) {
            var mainPartUpdatedMetadataVersion = metadata.version.split('.')[0]++;
            var metadataItems = metadata.items;

            if(mainPartUpdatedMetadataVersion <= 15) {
                var variablesMetadata = ThemeBuilder.__variables_migration_metadata();
                var additionalVariablesMetadata = ThemeBuilder.__additional_migration_metadata();

                for(var i = 0, len = metadataItems.length; i < len; i++) {
                    var replaceableValue = variablesMetadata[metadataItems[i].key];

                    if(replaceableValue) {
                        metadataItems[i].key = replaceableValue;
                    }

                    var borderRadiusValue = additionalVariablesMetadata[metadataItems[i].key];

                    if(borderRadiusValue) {
                        metadataItems[i].value = borderRadiusValue;
                    }
                }
            }

            return metadataItems;
        };

        var applyImportedMetadata = function() {
            var updatedMetadata = "",
                d = $.Deferred();
            try {
                updatedMetadata = JSON.parse(this.exportedMetadataAsString() || "");
                updatedMetadata.items = backwardCompatibililyForVariables(updatedMetadata);
            } catch(e) {
                showFailureMessage.call(this, "Metadata has a wrong format");
                d.resolve();
            }

            if(updatedMetadata) {
                $.map(updatedMetadata.items, function(item) {
                    return $.extend(item, { isModified: true });
                });

                if(updatedMetadata.version !== metadataVersion) {
                    DevExpress.ui.dialog.confirm(
                        "Your version of metadata does not match the Theme Builder v" + metadataVersion + "<br />Do you want to continue?", "Theme Builder")
                        .done(function(dialogResult) {
                            if(dialogResult) {
                                useNewMetadata.call(that, updatedMetadata);
                                d.resolve();
                            } else {
                                that.metadataPopupOptions.visible(false);
                                that.exportedMetadataAsString(null);
                            }
                        });
                } else {
                    useNewMetadata.call(this, updatedMetadata);
                    d.resolve();
                }
            }

            return d.promise();
        };

        var historyChangesHasItems = function() {
            var historyItems = this.historyChangesManager.getItems(),
                historyItemsCount = historyItems.length;

            if(!historyItemsCount)
                return false;

            return true;
        };

        var getThemeById = function(themeId) {
            return $.grep(that.themes, function(theme) {
                return theme.themeId === Number(themeId);
            })[0];
        };

        var updateMetadata = function(item, metadata) {
            $.each(metadata, function(_, group) {
                $.each(group, function(_, currentItem) {
                    if(currentItem.Key === item.key) {
                        currentItem.Value = item.value || currentItem.Value;
                        currentItem.isModified = item.isModified;
                        return false;
                    }
                });
            });

            return metadata;
        };

        // todo: get rid of this
        var isHistoryNavigationAction = false;
        var historyChangesManagerNavigated = function(direction) {

            this.loadMetadata(true);

            var historyItemsCount = this.historyChangesManager.getItems().length,
                indexOfCurrentHistoryItem = this.historyChangesManager.getCurrentIndex(),
                data = this.historyChangesManager.getCurrentItem().data;

            if(direction === "back" && !data.items.length)
                data.items = initialKeyValuePairs;

            isHistoryNavigationAction = true;

            this.hue(data.hue);

            updateIsModifiedState = false;
            updateToolboxGroupsValues.call(this, function(item) {
                return getExportedMetadataValueByKey(data.items, item.key) || item.value();
            });
            updateIsModifiedState = true;
            previewIframeNotifier.notify($previewIframe, "update", {
                metadata:  metadataRepository.getData({
                    name: this.currentTheme(),
                    colorScheme: this.currentColorScheme()
                }),
                group: ThemeBuilder.GetOpenedGroup(this.currentWidgetGroup()),
                currentTheme: this.currentTheme(),
                currentColorScheme: this.currentColorScheme()
            });
            isHistoryNavigationAction = false;

            updateUndoButtonActivity.call(this, historyItemsCount, indexOfCurrentHistoryItem);
            updateRedoButtonActivity.call(this, historyItemsCount, indexOfCurrentHistoryItem);
        };

        var updateTreeScroll = function() {
            $("#tree").dxTreeList("instance").updateDimensions();
        };

        this._inlineStylesUpdatedCallback = $.noop;
        this.historyChangesManager = historyChangesManager;
        this.currentTheme = ko.observable("generic");
        this.currentColorScheme = ko.observable("light");
        this.CSSContent = ko.observable();
        this.hue = ko.observable(defaultHue);
        this.exportedMetadataAsString = ko.observable(null);
        this.metadataPopupButtonsVisible = ko.observable(false);
        this.isUndoButtonDisabled = ko.observable(true);
        this.isRedoButtonDisabled = ko.observable(true);
        this.toolboxItems = ko.observableArray();
        this.currentWidgetGroup = ko.observable(defaultWidgetGroup);
        this.isMobileTheme = ko.observable(false);
        this.tree = ko.observableArray([]);
        this.searchValue = ko.observable();
        this.importMenuOpened = ko.observable(false);
        this.exportMenuOpened = ko.observable(false);

        this.toolboxValueSubscribe = function(item, value) {
            var metadata = metadataRepository.getData({
                name: this.currentTheme(),
                colorScheme: this.currentColorScheme()
            });

            var currentWidgetGroup = item.Group || this.currentWidgetGroup();

            $.each(metadata[currentWidgetGroup], function(index, metadataItem) {
                if(metadataItem.Key === item.Key) {
                    metadata[currentWidgetGroup][index].Value = value;
                    if(updateIsModifiedState)
                        metadata[currentWidgetGroup][index].isModified = true;
                    return false;
                }
            });

            if(updatePreviewStyles && updateIsModifiedState) {
                previewIframeNotifier.notify($previewIframe, "update", {
                    metadata: metadata,
                    group: ThemeBuilder.GetOpenedGroup(this.currentWidgetGroup()),
                    currentTheme: this.currentTheme(),
                    currentColorScheme: this.currentColorScheme()
                });

                if(!isHistoryNavigationAction && !doNotCreateHistoryItem) {
                    createHistoryChangesItem.call(this);
                }
            }
        };

        this.toolboxValueUpdated = function(compiledMetada, modifyVars) {
            updateIsModifiedState = false;
            if(modifyVars) {
                var metadata = metadataRepository.getData({
                    name: this.currentTheme(),
                    colorScheme: this.currentColorScheme()
                });

                $.each(metadata, function(i, group) {
                    for(var j = 0, len = group.length; j < len; j++) {
                        group[j].isModified = modifyVars[group[j].Key] ? true : false;
                    }
                });
            }
            updateToolboxGroupsValues.call(this, function(item) {
                return compiledMetada[item.key];
            });
            updateIsModifiedState = true;
            this._inlineStylesUpdatedCallback();
        };

        this.selectedThemeId = ko.observable(1);

        this.updateThemeFromMenu = function(e) {
            if(!e.jQueryEvent) return;

            return DevExpress
                .ui
                .dialog
                .confirm("Are you sure you want to change the base theme?<br />All changes will be lost.", "Theme Builder")
                .done(function(dialogResult) {
                    if(dialogResult) {
                        var theme = getThemeById(e.value);
                        that.updateTheme(theme);
                    }
                });
        };

        this._isMobileTheme = function(themeName) {
            return $.inArray(themeName, ["ios7", "android5"]) > -1;
        };

        this.updateTheme = function(value) {

            this.currentTheme(value.name);
            that.selectedThemeId(value.themeId);
            this.currentColorScheme(value.colorScheme);
            this.currentWidgetGroup(defaultWidgetGroup);
            _updatedData = [];

            this.init(this.themes, true);
            this.historyChangesManager.clearHistory();
            
            this.isMobileTheme(this._isMobileTheme(value.name));

            this.hue(defaultHue);

            if(!doNotCreateHistoryItem)
                createHistoryChangesItem.call(this);
        };

        this.loadMetadata = function(checkHistoryChanges) {
            var metadata = metadataRepository.getData({ name: this.currentTheme(), colorScheme: this.currentColorScheme() });
            if(checkHistoryChanges && historyChangesHasItems.call(this)) {
                var currentTheme = getThemeById.call(this, this.historyChangesManager.getCurrentItem().data.themeId);
                metadata = metadataRepository.getData({ name: currentTheme.name, colorScheme: currentTheme.colorScheme });
                $.each(this.historyChangesManager.getCurrentItem().data.items, function(index, item) {
                    updateMetadata(item, metadata);
                });

                this.selectedThemeId(currentTheme.themeId);

                this.isMobileTheme(this._isMobileTheme(currentTheme.name));

                this.currentTheme(currentTheme.name);
                this.currentColorScheme(currentTheme.colorScheme);
            }

            if(!historyChangesHasItems.call(this)) {
                initialKeyValuePairs = convertMetadataToKeyValuePairs.call(this);
                createHistoryChangesItem.call(this);
            }

            if(!this.selectedThemeId())
                this.selectedThemeId(1);

            return metadata;
        };

        this.saveCSS = function() {
            if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "export", "save css (" + that.currentTheme() + "." + that.currentColorScheme() + ")");
            that.getCSS(true);
        };

        this.copyCSS = function() {
            if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "export", "copy css (" + that.currentTheme() + "." + that.currentColorScheme() + ")");
            that.getCSS();
        };

        this.getCSS = function(saveFile) {
            var actionName = saveFile ? "saveCSS" : "getCSS";
            previewIframeNotifier.notify($previewIframe, actionName, metadataVersion);
        };

        this.showCSSPopup = function(content) {
            this.cssPopupOptions.visible(true);
            this.CSSContent(cssBeautifier.beautify(content));
        };

        this.cssPopupOptions = {
            title: "Copy CSS",
            visible: ko.observable(false),
            height: 450,
            width: 600,
            dragEnabled: false
        },

        this.saveCSSAs = function(css) {
            var fileName = [this.currentTheme(), this.currentColorScheme(), "custom"].join(".");
            DevExpress.clientExporter.fileSaver.saveAs(fileName, "CSS", new Blob([cssBeautifier.beautify(css)]));
        };

        this._applyCustomHue = function(e) {
            var hue = e.value;
            if(!hue)
                return;

            var customThemeCreator = new ThemeBuilder.CustomThemeCreator(hue);
            updateToolboxGroupsValues.call(this, function(item) {
                var result = item.value();
                if(item.type === "color") {
                    result = customThemeCreator.getColor(item.value());
                }

                return result;
            });
        };

        this._getBuildDate = function() {
            return DevExpress.localization.date.format(new Date(), "M/d/yyyy");
        },

        this.importMetadata = function() {
            if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "import", "metadata");
            applyImportedMetadata.call(this).done(function() {
                that.metadataPopupOptions.visible(false);
                that.exportedMetadataAsString(null);
            });
        };

        this.exportMetadata = function() {
            if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "export", "metadata (" + that.currentTheme() + "." + that.currentColorScheme() + ")");
            var result = {
                items: convertMetadataToKeyValuePairs.call(that, true),
                themeId: that.selectedThemeId(),
                hue: that.hue(),
                date: that._getBuildDate()
            };
            that.metadataPopupOptions.visible(true);
            that.metadataPopupButtonsVisible(false);
            exportedMetadata(result);
            makeExportedMetadataString.call(that, result);
        };

        this.showMetadataPopup = function() {
            that.metadataPopupButtonsVisible(true);
            that.metadataPopupOptions.visible(true);
            exportedMetadata(null);
        };

        this.closeMetadataPopup = function() {
            this.metadataPopupOptions.visible(false);
            this.exportedMetadataAsString(null);
        };

        this.metadataPopupOptions = {
            visible: ko.observable(false),
            title: ko.computed(function() {
                return (this.metadataPopupButtonsVisible() ? "Import" : "Export") + " Theme Builder Metadata";
            }, this),
            height: ko.computed(function() {
                return this.metadataPopupButtonsVisible() ? 500 : "auto";
            }, this),
            width: 600,
            dragEnabled: false,
            toolbarItems: ko.computed(function() {
                return this.metadataPopupButtonsVisible() ? [{
                    toolbar: "bottom",
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Apply",
                        onClick: $.proxy(this.importMetadata, this),
                        visible: this.metadataPopupButtonsVisible
                    }
                },
                {
                    toolbar: "bottom",
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Cancel",
                        onClick: $.proxy(function() {
                            this.metadataPopupOptions.visible(false);
                        }, this),
                        visible: this.metadataPopupButtonsVisible
                    }
                }] : [];
            }, this)
        },

        this.bootstrapAnalyzeFileUploaderOptions = {
            selectButtonText: "Upload LESS Variables",
            showFileList: false,
            uploadMode: "useForm",
            onValueChanged: $.proxy(function(e) {
                var file = e.value[0];
                if(file) {
                    var fileReader;
                    fileReader = new FileReader();
                    fileReader.onload = function(e) {
                        if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "import", "bootstrap variables (file)");
                        that.analyzeBootstrap(fileReader.result);
                        that.bootstrapAnalyzePopupOptions.visible(false);
                    };
                    fileReader.readAsText(file);
                }
            }, this)
        };

        this.bootstrapAnalyzePopupOptions = {
            visible: ko.observable(false),
            title: "Analyze Bootstrap LESS Variables",
            height: this.supportFileReader ? 250 : 500,
            width: 600,
            dragEnabled: false,
            toolbarItems: this.supportFileReader ? [] : [{
                toolbar: "bottom",
                location: "after",
                widget: "button",
                options: {
                    text: "Apply",
                    onClick: $.proxy(function() {
                        if(window.trackGAEvent) trackGAEvent("ThemeBuilder", "import", "bootstrap variables (string)");
                        this.analyzeBootstrap(this.AnalyzedBootstrapCustomThemeString());
                        this.bootstrapAnalyzePopupOptions.visible(false);
                    }, this)
                }
            },
            {
                toolbar: "bottom",
                location: "after",
                widget: "button",
                options: {
                    text: "Cancel",
                    onClick: $.proxy(function() {
                        this.bootstrapAnalyzePopupOptions.visible(false);
                    }, this)
                }
            }]
        };

        this.showAnalyzeBootstrapPopup = function() {
            that.bootstrapAnalyzePopupOptions.visible(true);
        };

        this.analyzedBootstrapCustomThemeString = ko.observable();

        this.analyzeBootstrap = function(customLess) {
            // TODO: get clean metadata
            var metadata = metadataRepository.getData({
                name: this.currentTheme(),
                colorScheme: this.currentColorScheme()
            });

            previewIframeNotifier.notify($previewIframe, "analyze", {
                metadata: metadata,
                currentTheme: this.currentTheme(),
                currentColorScheme: this.currentColorScheme(),
                bootstrapMetadata: ThemeBuilder.__bootstrap_metadata(),
                customLessContent: customLess
            });
            
        };

        this.init = function(themes, doNotCheckLocalChanges) {
            doNotCheckLocalChanges = doNotCheckLocalChanges || false;
            that.themes = themes;
            that.groupedThemes = new DevExpress.data.DataSource({
                store: themes,
                key: "themeId",
                group: "group"
            });

            return metadataRepository.init(themes).done(function() {

                var historyItems = that.historyChangesManager.getItems(),
                    lastHistoryItem = null,
                    theme;

                if(historyItems.length && !doNotCheckLocalChanges) {

                    var currentHistoryData = that.historyChangesManager.getCurrentItem().data;

                    lastHistoryItem = currentHistoryData.items;

                    if(currentHistoryData.hue && currentHistoryData.hue !== that.hue())
                        that.hue(currentHistoryData.hue);

                    try {
                        var currentTheme = getThemeById.call(that, that.historyChangesManager.getCurrentItem().data.themeId);
                        theme = {};
                        theme.name = currentTheme.name;
                        theme.colorScheme = currentTheme.colorScheme;
                    } catch(e) { }
                }

                ThemeBuilder.navigationTreeManager.initTree(lastHistoryItem, that.currentWidgetGroup(), theme);
                var metadata = that.loadMetadata(!doNotCheckLocalChanges);

                loadPreviewIframe.call(that, metadata);
            });
        };

        this.undo = function() {
            this.historyChangesManager.navigateToPreviousItem();
            historyChangesManagerNavigated.call(this, "back");
        };

        this.redo = function() {
            this.historyChangesManager.navigateToNextItem();
            historyChangesManagerNavigated.call(this, "forward");
        };

        this.setDefault = function() {
            return DevExpress.ui.dialog.confirm("Are you sure you want to reset all changes?", "Theme Builder").done(function(dialogResult) {
                if(dialogResult) {

                    that.updateTheme(that.themes[0]);
                    that.hue(defaultHue);

                    that.historyChangesManager.clearHistory();

                    historyChangesManagerNavigated.call(that, "back");
                    that.isUndoButtonDisabled(true);
                    that.isRedoButtonDisabled(true);
                }
            });
        };

        this.sortToolboxEditors = function(a, b) {
            var aOrd = Number(a.name.split(".")[0]),
                bOrd = Number(b.name.split(".")[0]);

            if(!aOrd || b.Ord)
                return 0;

            return aOrd < bOrd ? -1 : 1;
        };

        exportedMetadata.subscribe($.proxy(makeExportedMetadataString, this));

        this.hidePreviewBody = function() {
            previewIframeNotifier.notify($previewIframe, "hideBody");
        };

        var itemClick = function(item) {
            var itemData = item.data;
            var currentThemeName = this.currentTheme();

            ThemeBuilder.navigationTreeManager.setActivePath(item);
            if(!itemData.formOpened()) {
                itemData.formOpened(true);
                updateTreeScroll();
            }

            if(this.currentWidgetGroup() === ThemeBuilder.GetOpenedGroup(itemData.key)) {
                return;
            }

            var metadata = metadataRepository.getData({
                name: currentThemeName,
                colorScheme: this.currentColorScheme()
            });

            this.currentWidgetGroup(itemData.key);
            this.hidePreviewBody();

            if(ThemeBuilder.groupsAliasesRepository.getIsMobile(itemData.key))
                this.isMobileTheme(true);
            else {
                this.isMobileTheme(this._isMobileTheme(currentThemeName));
            }

            previewIframeNotifier.notify($previewIframe, "update", {
                metadata: metadata,
                currentTheme: currentThemeName,
                currentColorScheme: this.currentColorScheme(),
                group: ThemeBuilder.GetOpenedGroup(itemData.key)
            });
        };

        this.treeItemClick = function(e) {
            var node = e.node;

            if(e.isExpanded) {
                e.component.collapseRow(node.data.key);
                e.jQueryEvent.stopPropagation();
                return;
            }

            while(node.hasChildren) {
                e.component.expandRow(node.data.key);
                node = node.children[0];
            }

            itemClick.call(this, node);
        };

        this.headerClick = function(item, e) {
            if(item.data.formOpened()) {
                item.data.formOpened(false);
                updateTreeScroll();
                e.stopPropagation();
            }
        };

        var foreachNodes = function(nodes, func) {
            for(var i = 0; i < nodes.length; i++) {
                func(nodes[i]);
                foreachNodes(nodes[i].children, func);
            }
        };

        this.onNodesInitialized = function(e) {
            foreachNodes(e.root.children, function(node) {
                if(node.visible && !node.hasChildren && node.children.length) {
                    node.hasChildren = true;
                    node.children.forEach(function(node) {
                        node.visible = true;
                    });
                }
            });
        };

        this.importMenuSource = ko.computed(function() {
            return that.isMobileTheme()
                ? []
                : [
                {
                    text: "Import metadata",
                    click: that.showMetadataPopup
                },
                {
                    text: "Bootstrap variables",
                    click: that.showAnalyzeBootstrapPopup
                }
            ];
        });

        this.importClick = function() {
            if(that.isMobileTheme())
                that.showMetadataPopup();
        };
    };

    ThemeBuilder.ViewModel = ViewModel;

})(ThemeBuilder);