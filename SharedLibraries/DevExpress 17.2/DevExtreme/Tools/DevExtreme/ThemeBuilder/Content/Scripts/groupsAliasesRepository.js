(function(ThemeBuilder) {
    "use strict";

    var GroupsAliasesRepository = function() {
        this._aliases = {

            "basetheming":          { name: "Base Theming", ord: 0 },

            "advtheming":           { name: "Advanced Theming", ord: 1 },

            "base":                 { name: "Common", ord: 100 },
            "base.common":          { name: "Basic Settings", ord: 110 },
            "base.widgetstates":    { name: "Widget States", ord: 120 },
            "base.typography":      { name: "Typography Settings", ord: 130 },

            "datagrid":             { name: "Data Grid", ord: 200 },
            "treelist":             { name: "Tree List", ord: 300 },
            "pivotgrid":            { name: "Pivot Grid", ord: 400 },
            "scheduler":            { name: "Scheduler", ord: 500 },

            "badges":               { name: "Badges", ord: 600 },

            "buttons":              { name: "Button", ord: 700 },
            "buttons.default":      { name: "Default Type", ord: 710 },
            "buttons.normal":       { name: "Normal Type", ord: 720 },
            "buttons.success":      { name: "Success Type", ord: 730 },
            "buttons.danger":       { name: "Danger Type", ord: 740 },
            "buttons.flat":         { name: "Flat Button", ord: 750 },

            "editors":              { name: "Editors", ord: 800 },
            "editors.autocomplete": { name: "Autocomplete", ord: 810 },
            "editors.calendar":     { name: "Calendar", ord: 815 },
            "editors.checkbox":     { name: "Check Box", ord: 820 },
            "editors.colorbox":     { name: "Color Box", ord: 825 },
            "editors.fileuploader": { name: "File Uploader", ord: 830 },
            "editors.lookup":       { name: "Lookup", ord: 835 },
            "editors.numberbox":    { name: "Number Box", ord: 840 },
            "editors.radiogroup":   { name: "Radio Group", ord: 845 },
            "editors.selectbox":    { name: "Select Box", ord: 850 },
            "editors.switch":       { name: "Switch", ord: 855 },
            "editors.tagbox":       { name: "Tag Box", ord: 860 },
            "editors.texteditors":  { name: "Text Editors", ord: 865 },
            "editors.validation":   { name: "Validation", ord: 870 },

            "form":                 { name: "Form", ord: 900 },
            "filterbuilder":        { name: "Filter Builder", ord: 950 },
            "gallery":              { name: "Gallery", ord: 1000 },

            "overlays":             { name: "Overlays", ord: 1100 },
            "overlays.common":      { name: "Overlays", ord: 1110 },
            "overlays.actionsheet": { name: "Action Sheet", ord: 1120 },
            "overlays.tooltip":     { name: "Tooltip", ord: 1130 },
            "overlays.toasts":      { name: "Toast", ord: 1140 },

            "list":                 { name: "List", ord: 1200 },

            "layouts":              { name: "Layouts", ord: 1300 },
            "layouts.desktop":      { name: "Desktop", ord: 1310 },
            "layouts.split":        { name: "Split", ord: 1320, isMobile: true },
            
            "navigations":          { name: "Navigation", ord: 1400 },
            "navigations.accordion": { name: "Accordion", ord: 1410 },
            "navigations.menu":     { name: "Menu", ord: 1420 },
            "navigations.navbar":   { name: "Navbar", ord: 1430 },
            "navigations.tabs":     { name: "Tabs", ord: 1440 },
            "navigations.toolbar":  { name: "Toolbar", ord: 1450 },
            "navigations.treeview": { name: "Tree View", ord: 1460 },

            "progressbars":         { name: "Progress Bar", ord: 1500 },
            
            "scrollview":           { name: "Scroll View", ord: 1600 },
            "sliders":              { name: "Sliders", ord: 1700 },
            "tileview":             { name: "Tile View", ord: 1800 }
        };

        this.getAlias = function(group, previevCaption) {
            if(previevCaption && group === "base.common")
                group = "basetheming";
            return this._aliases[group].name;
        };

        this.getOrd = function(group) {
            return this._aliases[group].ord;
        };

        this.getIsMobile = function(group) {
            return "isMobile" in this._aliases[group];
        };
    };

    ThemeBuilder.GroupsAliasesRepository = GroupsAliasesRepository;
    ThemeBuilder.defaultWidgetGroup = "base.common";
    ThemeBuilder.GetOpenedGroup = function(group) {
        if(group === "basetheming") return ThemeBuilder.defaultWidgetGroup;
        return group;
    };

})(ThemeBuilder);
