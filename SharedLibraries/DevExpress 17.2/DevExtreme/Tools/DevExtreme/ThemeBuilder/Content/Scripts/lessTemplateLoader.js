(function(ThemeBuilder) {
    "use strict";

    var DYNAMIC_STYLES_ID = "dynamic-styles";
    var LESS_DIR_PATH = "Content/Data/LESS/";
    var BOOTSTRAP_VERIABLE_LESS_FILENAME = "theme-builder-bootstrap-variables.less.js";
    var parentIframeNotifier = new ThemeBuilder.ParentIframeNotifier();
    var LessFontPlugin = function() {};
    LessFontPlugin.prototype = {
        process: function(css, extra) {
            css = css.replace(/(\f)(\d+)/g, "\\f$2");
            css = css.replace(/url\(icons\/dxicons/gi, "url(Content/DevExtreme/css/icons/dxicons");
            return css;
        }
    };

    var LessMetadataPreCompilerPlugin = function(less, metadata) {
        this._metadata = metadata;
    };
    LessMetadataPreCompilerPlugin.prototype = {
        process: function(less, config) {
            less += "#devexpress-metadata-compiler{";
            $.each(this._metadata, function(key, value) {
                less += key + ": " + value + ";";
            });
            less += "}";
            return less;
        }
    };

    var LessMetadataPostCompilerPlugin = function(less, compiledMetadata) {
        this._metadata = compiledMetadata;
    };
    LessMetadataPostCompilerPlugin.prototype = {
        process: function(css, config) {
            var that = this;
            var metadataRegex = /#devexpress-metadata-compiler\s*\{((.|\n|\r)*)\}/;
            $.each(metadataRegex.exec(css)[1].split(";"), function(_, item) {
                $.extend(that._metadata, getCompiledRule(item));
            });
            return css.replace(metadataRegex, "");
        }
    };

    var getCompiledRule = function(cssString) {
        var result = {};
        var ruleRegex = /([-\w\d]*)\s*:\s*(.*)\s*/;
        var matches = ruleRegex.exec(cssString);
        if (matches) {
            result["@" + matches[1]] = matches[2];
        } else {
            result = null;
        }
        return result;
    };

    ThemeBuilder.LessTemplateLoader = function() {
        this.load = function(theme, colorScheme, metadata) {
            var that = this;
            var d = $.Deferred();
            this._loadLess(theme, colorScheme).done(function(less) {
                var modifyVars = {};
                var metadataVariables = {};
                $.each(metadata, function(i, group) {
                    for (var j = 0, len = group.length; j < len; j++) {
                        if (group[j].isModified) {
                            modifyVars[group[j].Key.replace("@", "")] = group[j].Value;
                        }
                        metadataVariables[group[j].Key.replace("@", "")] = group[j].Key;
                    }
                });

                that.compileLess(less, modifyVars, metadataVariables).done(function(compiledMetadata, css) {
                    that._createDynamicStyles(css);
                    d.resolve(compiledMetadata, css);
                });
            });

            return d.promise();
        };

        this.compileLess = function(less, modifyVars, metadata) {
            var d = new $.Deferred();
            var compiledMetadata = {};
                window.less.render(less, {
                modifyVars: modifyVars, plugins: [{
                        install: function(less, pluginManager) {
                            pluginManager.addPostProcessor(new LessFontPlugin(this.options));
                        }
                    }, {
                        install: function(less, pluginManager) {
                            pluginManager.addPreProcessor(new LessMetadataPreCompilerPlugin(less, metadata));
                        }
                    }, {
                        install: function(less, pluginManager) {
                            pluginManager.addPostProcessor(new LessMetadataPostCompilerPlugin(less, compiledMetadata));
                        }
                    }]
                }).then(function(output){
                    d.resolve(compiledMetadata, output.css);
                }, function(error) {
                    parentIframeNotifier.notify("handleError", {message: error.message});
                    d.reject();
                });

            return d.promise();
        }

        this.analyzeBootstrapTheme = function(theme, colorScheme, metadata, bootstrapMetadata, customLessContent) {
            var that = this;
            var d = $.Deferred();

            var preLessString = "";
            $.each(bootstrapMetadata, function(key, value) {
                preLessString += value + ": dx-empty;";
            })

            that.compileLess(preLessString + customLessContent, {}, bootstrapMetadata).done(function(compiledMetadata) {
                var modifyVars = {};
                $.each(compiledMetadata, function(key, value) {
                    if (value !== "dx-empty") {
                        modifyVars[key] = value;
                    }
                });

                that._loadLess(theme, colorScheme).done(function(less) {
                    var metadataVariables = {};
                    $.each(metadata, function(i, group) {
                        for (var j = 0, len = group.length; j < len; j++) {
                            metadataVariables[group[j].Key.replace("@", "")] = group[j].Key;
                        }
                    });
                    that.compileLess(less, modifyVars, metadataVariables).done(function(compiledMetadata, css) {
                        that._createDynamicStyles(css);
                        d.resolve(compiledMetadata, modifyVars);
                    });
                });
            });

            return d.promise();
        };

        this._loadLess = function(theme, colorScheme) {
            var themeName = (theme ? theme + "-" : "");
            return this._loadLessByFileName(LESS_DIR_PATH + "theme-builder-" + themeName + colorScheme + ".less.js");
        };

        this._loadLessByFileName = function(fileName) {
            var d = $.Deferred();
            window.lessTemplateLoadedCallback = function(less) {
                d.resolve(less);
            };
            $.ajax({
                url: fileName,
                dataType: "jsonp",
                crossDomain: true,
                jsonpCallback: "lessTemplateLoadedCallback"
            });
            return d.promise();
        }

        this._createDynamicStyles = function(css) {
            $("#" + DYNAMIC_STYLES_ID).remove();

            $("<style>" + css + "</style>")
                .attr("type", "text/css")
                .attr("id", DYNAMIC_STYLES_ID)
                .appendTo("head");
        };

        this.getCSSFromPage = function(metadataVersion) {
            var css = this._makeInfoHeader(metadataVersion);
            css += $("#" + DYNAMIC_STYLES_ID).html();
            return css.replace(/url\(Content\/DevExtreme\/css\/icons\/dxicons/gi, "url(icons/dxicons");
        };

        this._makeInfoHeader = function(metadataVersion) {
            var generatedBy = "* Generated by the DevExpress Theme Builder",
                version = "* Version: " + metadataVersion,
                link = "* http://js.devexpress.com/themebuilder/";

            return ["/*", generatedBy, version, link, "*/"].join("\n");
        };
    };
})(ThemeBuilder);
