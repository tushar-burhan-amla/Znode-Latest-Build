$(function() {
    "use strict";

    var metadataLoader = new ThemeBuilder.MetadataLoader(),
        historyChangesManager = new ThemeBuilder.HistoryChangesManager(window.sessionStorage),
        previewIframeNotifier = new ThemeBuilder.PreviewIframeNotifier();

    function listener(event) {
        var data = JSON.parse(event.data);
        switch(data.action) {
            case "getCSS":
                ThemeBuilder.viewModel.showCSSPopup(data.actionData);
                break;
            case "saveCSS":
                ThemeBuilder.viewModel.saveCSSAs(data.actionData);
                break;
            case "stylesUpdated":
                ThemeBuilder.viewModel.toolboxValueUpdated(data.actionData.compiledMetadata, data.actionData.modifyVars);
                break;
            case "handleError":
                DevExpress.ui.dialog.alert(data.actionData.message, "Compilation error");
                break;
        }
    }

    if(window.addEventListener)
        window.addEventListener("message", listener, false);
    else
        window.attachEvent("onmessage", listener);

    ThemeBuilder.themes = [
        { themeId: 1, name: "generic", colorScheme: "light", text: "Light", group: 1 },
        { themeId: 2, name: "generic", colorScheme: "dark", text: "Dark", group: 1 },
        { themeId: 13, name: "generic", colorScheme: "carmine", text: "Carmine", group: 2 },
        { themeId: 14, name: "generic", colorScheme: "darkmoon", text: "Dark Moon", group: 2 },
        { themeId: 15, name: "generic", colorScheme: "softblue", text: "Soft Blue", group: 2 },
        { themeId: 16, name: "generic", colorScheme: "darkviolet", text: "Dark Violet", group: 2 },
        { themeId: 17, name: "generic", colorScheme: "greenmist", text: "Green Mist", group: 2 },
        { themeId: 9, name: "generic", colorScheme: "light-compact", text: "Light Compact", group: 3 },
        { themeId: 10, name: "generic", colorScheme: "dark-compact", text: "Dark Compact", group: 3 },
        { themeId: 18, name: "generic", colorScheme: "carmine-compact", text: "Carmine Compact", group: 4 },
        { themeId: 19, name: "generic", colorScheme: "darkmoon-compact", text: "Dark Moon Compact", group: 4 },
        { themeId: 20, name: "generic", colorScheme: "softblue-compact", text: "Soft Blue Compact", group: 4 },
        { themeId: 21, name: "generic", colorScheme: "darkviolet-compact", text: "Dark Violet Compact", group: 4 },
        { themeId: 22, name: "generic", colorScheme: "greenmist-compact", text: "Green Mist Compact", group: 4 },
        { themeId: 3, name: "ios7", colorScheme: "default", text: "iOS", group: 5 },
        { themeId: 6, name: "android5", colorScheme: "light", text: "Android", group: 5 }
    ];

    ThemeBuilder.groupsAliasesRepository = new ThemeBuilder.GroupsAliasesRepository();
    ThemeBuilder.metadataRepository = new ThemeBuilder.MetadataRepository(metadataLoader);
    ThemeBuilder.metadataVersion = ThemeBuilder.__get_metadata_version();
    ThemeBuilder.viewModel = new ThemeBuilder.ViewModel(ThemeBuilder.metadataRepository, historyChangesManager, previewIframeNotifier, { beautify: css_beautify });
    ThemeBuilder.navigationTree = new ThemeBuilder.NavigationTree({
        rootItemClick: $.proxy(ThemeBuilder.viewModel.treeRootItemClick, ThemeBuilder.viewModel),
        childItemClick: $.proxy(ThemeBuilder.viewModel.treeItemClick, ThemeBuilder.viewModel)
    });
    ThemeBuilder.navigationTreeManager = new ThemeBuilder.NavigationTreeManager(ThemeBuilder.navigationTree, ThemeBuilder.viewModel, ThemeBuilder.metadataRepository);

    ThemeBuilder.viewModel.init(ThemeBuilder.themes);

    ko.applyBindings(ThemeBuilder.viewModel);
});
