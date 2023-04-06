(function(ThemeBuilder) {
    "use strict";

    var PreviewViewModel = function(cssTemplateLoader, previewRepository, parentIframeNotifier) {
        var that = this;
        var _cssTemplateLoader = cssTemplateLoader,
             currentState = {
                 theme: {
                     name: null,
                     colorScheme: null
                 },
                 group: null
             };

        this.contentUrl = "Content/DevExtreme/";

        this.typography = ko.observable();

        this.isMobileTheme = ko.observable(false);
        this.themeColoring = ko.observable("coloring-light");
        this.isApplication = ko.observable(false);

        this.widgets = ko.observableArray([]);
        this.baseWidgets = ko.observableArray([]);
        this.application = ko.observable();

        this.theme = ko.observable();
        this.colorScheme = ko.observable();
        this.groupTitle = ko.observable();
        this.baseGroupTitle = ThemeBuilder.groupsAliasesRepository.getAlias(ThemeBuilder.defaultWidgetGroup, true);

        this.applyInlineStyles = function(metadata, theme, colorScheme) {
            var d = $.Deferred(),
                that = this;
            return _cssTemplateLoader.load(theme, colorScheme, metadata);
        };

        this.getCSS = function(metadataVersion) {
            var css = _cssTemplateLoader.getCSSFromPage(metadataVersion);
            parentIframeNotifier.notify("getCSS", css);
        };

        this.saveCSS = function(metadataVersion) {
            var css = _cssTemplateLoader.getCSSFromPage(metadataVersion);
            parentIframeNotifier.notify("saveCSS", css);
        };

        this.getPreviewContainer = function() {
            return $(".preview-container");
        };

        this.init = function(metadata, group) {
            var that = this,
                theme = this.theme(),
                colorScheme = this.colorScheme(),
                updateWidgetsPreview = false,
                previewData = previewRepository.getData({ name: theme, colorScheme: colorScheme }, group),
                previewBaseData = [];

            if(group !== ThemeBuilder.defaultWidgetGroup)
                previewBaseData = previewRepository.getData({ name: theme, colorScheme: colorScheme }, ThemeBuilder.defaultWidgetGroup);
            else
                previewBaseData = [];

            if(previewData.length)
                this.groupTitle(ThemeBuilder.groupsAliasesRepository.getAlias(previewData[0].id, true));

            this.typography("dx-theme-" + theme + "-typography");
            this.isMobileTheme($.inArray(theme, ["ios7", "android5"]) > -1);

            this.themeColoring($.inArray(colorScheme, ["light", "light-compact", "white", "default", "carmine", "greenmist", "softblue"]) > -1 ? "coloring-light" : "coloring-dark");

            this.isApplication(group.indexOf("layout") > -1);

            if(currentState.theme.name !== theme || currentState.theme.colorScheme !== colorScheme || currentState.group !== group)
                updateWidgetsPreview = true;

            return that.applyInlineStyles(metadata, theme, colorScheme).done(function(compiledMetadata) {
                if(updateWidgetsPreview || !that.widgets() || !that.widgets().length) {
                    that.getPreviewContainer().empty();
                    that.widgets(previewData);
                    that.baseWidgets(previewBaseData);
                }

                parentIframeNotifier.notify("stylesUpdated", { compiledMetadata: compiledMetadata });

                currentState.theme.name = theme;
                currentState.theme.colorScheme = colorScheme;
                currentState.group = group;
            });
        };

        this.analyzeBootstrapTheme = function(metadata, bootstrapMetadata, customLessContent) {
            var theme = this.theme();
            var colorScheme = this.colorScheme();

            return _cssTemplateLoader.analyzeBootstrapTheme(theme, colorScheme, metadata, bootstrapMetadata, customLessContent).done(function(compiledMetadata, modifyVars) {
                parentIframeNotifier.notify("stylesUpdated", {
                    compiledMetadata: compiledMetadata,
                    modifyVars: modifyVars
                });
            });
        };

        this.bodyClass = ko.computed(function() {
            return that.themeColoring() + " " + (that.isApplication() ? "application" : "");
        });
    };

    ThemeBuilder.PreviewViewModel = PreviewViewModel;

})(ThemeBuilder);