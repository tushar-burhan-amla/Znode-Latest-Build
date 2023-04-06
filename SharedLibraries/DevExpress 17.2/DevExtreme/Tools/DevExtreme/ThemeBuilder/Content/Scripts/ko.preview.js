(function(DX, undefined) {
    "use strict";

    function renderBasicTypographyContent() {
        return [
            $("<h1 />").text("h1 heading"),
            $("<h2 />").text("h2 heading"),
            $("<h3 />").text("h3 heading"),
            $("<h4 />").text("h4 heading"),
            $("<h5 />").text("h5 heading"),
            $("<h6 />").text("h6 heading"),
            $("<a />").text("Hyperlink").attr("href", "#").css("display", "block")
        ];
    }

    window.SHOW_ACTIONSHEET = function() {
        $(".dx-actionsheet").dxActionSheet("instance").show();
    };

    window.SHOW_DROPDOWN_MENU = function() {
        $(".dx-dropdownmenu").dxDropDownMenu("instance").option("visible", true);
    };

    window.SHOW_LOAD_PANEL = function() {
        $("#load-panel-sample").dxLoadPanel("instance").option("visible", true);
    };

    window.SHOW_DATE_PICKER = function() {
        $("#date-picker-sample").dxDatePicker("instance").option("visible", true);
    };

    window.SHOW_INFO_TOAST = function() {
        $("#info-toast-sample").dxToast("instance").show();
    };

    window.SHOW_WARNING_TOAST = function() {
        $("#warning-toast-sample").dxToast("instance").show();
    };

    window.SHOW_ERROR_TOAST = function() {
        $("#error-toast-sample").dxToast("instance").show();
    };

    window.SHOW_SUCCESS_TOAST = function() {
        $("#success-toast-sample").dxToast("instance").show();
    };

    window.SHOW_CUSTOM_TOAST = function() {
        $("#custom-toast-sample").dxToast("instance").show();
    };

    window.SHOW_POPUP = function() {
        $("#popup-sample .dx-popup-content").html("<p>The popup contents.</p>");
        $("#popup-sample").dxPopup("instance").option("visible", true);
    };

    window.SHOW_TOOLTIP = function() {
        $("#tooltip-sample .dx-popup-content").html("<p>The tooltip contents.</p>");
        $("#tooltip-sample").dxTooltip("instance").option("visible", true);
    };

    window.SHOW_CONFIRM_DIALOG = function() {
        DX.ui.dialog.confirm("Are you sure?", "Confirm changes");
    };

    window.LOAD_SCROLLVIEW_CONTENT = function($widgetNode, options) {
        $widgetNode
            .append("<p>Lorem ipsum curabitur fusce urna bibendum eu morbi integer eu a risus ligula tempus ligula quisque porttitor fusce sagittis ut lorem quisque. Ut lorem lectus magna proin a porttitor massa mattis lectus leo amet eu morbi magna. Quisque curabitur eu odio rutrum eros mattis tellus integer justo ut sapien in rutrum cursus mauris nibh. Sed metus risus donec in risus sem elementum at nibh cursus metus at commodo. Porta adipiscing vivamus malesuada gravida lectus vivamus ligula orci vivamus rutrum diam vitae integer ligula integer massa porta. Gravida sed quam molestie tellus eget sagittis ligula elementum nam diam magna. In eget proin orci in amet sagittis proin morbi leo tempus.</p>")
            .append("<p>Vulputate sem justo nec vulputate donec sed ipsum lectus massa porta gravida quam morbi vitae eros fusce. Odio lectus vitae quam ut sit quisque mauris tempus nulla cursus ligula eu leo magna enim. In diam urna non nam ligula justo vulputate enim metus amet gravida sit sapien maecenas sagittis. Malesuada auctor mattis arcu lorem urna orci nibh gravida donec integer ultricies justo leo nec sagittis bibendum. Eget non eu massa nam donec vulputate porta mauris ut enim fusce mattis.</p>")
            .append("<p>Sagittis eget mauris diam odio diam sodales mauris amet morbi lorem sed lorem arcu diam sed magna mauris congue sit proin maecenas malesuada nibh. Pharetra tellus duis nibh porta pharetra adipiscing nam maecenas pellentesque quisque congue donec nibh maecenas proin risus tellus. Massa odio lorem tempus gravida tempus vivamus ipsum mattis ipsum diam fusce et arcu maecenas lorem fusce non mattis sapien curabitur. Justo porta adipiscing amet fusce tellus rutrum morbi nec curabitur et gravida bibendum donec lorem sapien sodales porta congue sem tempus. Congue amet tellus enim at non sagittis adipiscing ut sit sed. Duis risus non ut odio bibendum ipsum eu nulla duis commodo sapien fusce molestie in cursus commodo urna bibendum adipiscing pharetra. Ut rutrum mattis eros nec maecenas rutrum porta arcu non tempus. Non duis integer tempus porta ornare morbi risus leo urna pharetra metus sodales.</p>")
            .append("<p>Sodales in et gravida justo ut eget et ut proin. Porta quisque ipsum sem lorem ultricies sit donec duis eu malesuada. Odio proin risus magna diam ultricies enim lorem cursus malesuada ultricies auctor non. Ipsum elementum sit et integer sit porta duis amet ligula justo. Cursus odio sagittis bibendum at mattis nibh pellentesque congue elementum vitae leo tempus gravida. Tempus ultricies bibendum congue vitae curabitur nulla et eu quisque arcu proin gravida at congue diam porta in et gravida. Nulla massa sodales leo sodales vulputate nulla massa diam vivamus ut.</p>")
            .append("<p>Arcu ultricies commodo leo ultricies auctor sapien vivamus vitae lorem enim malesuada in gravida bibendum. Orci ut et porta arcu lectus ipsum sed ultricies quisque magna gravida eu mauris urna amet mauris sed sagittis odio magna et non maecenas diam. Tempus orci sed ornare urna maecenas quisque elementum et congue proin fusce molestie eu adipiscing ultricies enim leo cursus sem molestie magna lorem. Duis ornare sagittis magna porta commodo ligula vivamus maecenas odio cursus pharetra magna sagittis congue. Vitae magna ipsum gravida congue elementum pharetra rutrum integer ornare tempus maecenas ut vulputate sem pharetra duis integer vitae. Sodales duis sodales eget auctor eros pellentesque sagittis orci gravida orci arcu eget non massa pharetra quisque malesuada ultricies congue ut nec. Tellus arcu at auctor proin et metus ipsum orci fusce massa sapien nulla urna enim pellentesque.</p>")
            .append("<p>Elementum ut fusce congue risus ornare cursus lectus vulputate justo rutrum fusce diam commodo sit pellentesque commodo non metus. Risus sed leo eget risus arcu non maecenas magna nec in auctor sem et eros arcu lectus justo tempus. Magna vulputate sed duis metus nibh bibendum magna ornare sem sodales congue sem nam odio cursus lectus vulputate elementum massa quisque nulla enim curabitur. Eget gravida nec proin orci in gravida risus congue diam congue nibh metus quam justo quisque massa auctor rutrum. Sapien sodales ut duis ornare sapien quisque vivamus quam quisque magna nec a.</p>")
            .append("<p>Justo mattis ligula ultricies et diam risus nec sem quam orci urna leo auctor mauris leo nec quisque tempus vitae et maecenas elementum. Adipiscing in eget nulla sit rutrum quam porttitor at ipsum orci sapien malesuada amet urna mattis sapien pellentesque lectus sem congue tellus enim lectus sapien. Bibendum maecenas pellentesque at enim at vitae proin cursus eu congue eget eu urna gravida a pharetra sed. Adipiscing molestie nam sagittis adipiscing sed arcu urna fusce duis mattis ornare. Tempus non tempus et metus proin sed amet sodales pellentesque maecenas bibendum sapien sem nam ipsum mattis auctor sit urna non duis sodales quisque.</p>")
            .append("<p>Duis odio porttitor leo sem cursus enim leo eget non integer arcu enim malesuada amet odio et mauris cursus malesuada leo metus in massa. In lorem justo non bibendum non ipsum sit risus sagittis eu bibendum mauris. Adipiscing quisque bibendum nam elementum eget quam massa eu at porta nibh commodo integer arcu tellus odio vivamus integer auctor non a. Pellentesque ornare molestie eros diam massa quam in lectus tellus ligula pellentesque adipiscing. Curabitur non porta sem non et tellus enim ipsum ligula orci risus urna quisque molestie porttitor integer amet. Ut urna proin cursus sapien mauris risus sapien tellus lorem elementum sodales tellus ornare risus et leo in tempus. Sagittis cursus morbi cursus bibendum justo eros mauris malesuada in bibendum eget mattis fusce pellentesque morbi.</p>")
            .append("<p>Quam non nec leo tellus ut nibh tellus justo pharetra nibh magna. Tellus lectus a sem proin maecenas donec rutrum proin at odio fusce auctor.</p>")
            .append("<p>Vitae proin diam bibendum duis sit sed adipiscing bibendum malesuada non ultricies sodales. Bibendum lectus nec enim eros eget nibh sagittis sapien massa orci donec nec gravida rutrum. Sodales adipiscing sodales sed amet risus nibh magna nulla bibendum quam odio sagittis quisque molestie auctor orci lectus nec porta amet morbi lectus congue gravida.</p>")
            .height(300);

        options.pullDownAction = $.noop;
    };

    window.SET_CALENDAR_VALUE = function($widgetNode, options) {
        options.value = new Date(2014, 5, 2);
    };

    window.LOAD_GENERIC_TYPOGRAPHY_CONTENT = function($widgetNode) {
        $widgetNode.append(
            renderBasicTypographyContent().concat([
            $("<div />").addClass("dx-fieldset").append([
                $("<div />").addClass("dx-field")
                    .append([
                         $("<div />").addClass("dx-field-label").text("Fieldset label:").css("width", "20%"),
                         $("<div />").addClass("dx-field-value-static").text("Fieldset value").css("width", "80%")
                    ]),
                $("<div />").addClass("dx-field icons-preview")
                    .append([
                            $("<div />").addClass("dx-field-label").text("Icons:").css("width", "20%"),
                            $("<div />").addClass("dx-field-value-static").css("width", "80%").append([
                                $("<div />").dxButton({ icon: "car", text: "Car", width: 100 }),
                                $("<div />").dxButton({ icon: "gift", text: "Gift", width: 100 }),
                                $("<div />").dxButton({ icon: "cart", text: "Cart", width: 100 })
                            ])
                    ])
            ])
            ]));
    };

    window.LOAD_ANDROID5_TYPOGRAPHY_CONTENT = function($widgetNode) {
        $widgetNode.append(
            renderBasicTypographyContent().concat([
            $("<div />").addClass("dx-fieldset").append([
                $("<div />").addClass("dx-field")
                    .append([
                         $("<div />").addClass("dx-field-label").text("Fieldset label:").css("width", "20%"),
                         $("<div />").addClass("dx-field-value-static").text("Fieldset value").css("width", "80%")
                    ]),
                $("<div />").addClass("dx-field icons-preview")
                    .append([
                            $("<div />").addClass("dx-field-label").text("Dark Icons:").css("width", "20%"),
                            $("<div />").addClass("dx-field-value-static").css("width", "80%").append([
                                $("<div />").dxButton({ icon: "car", text: "Car", width: 100 }),
                                $("<div />").dxButton({ icon: "gift", text: "Gift", width: 100 }),
                                $("<div />").dxButton({ icon: "cart", text: "Cart", width: 100 })
                            ])
                    ]),
                $("<div />").addClass("dx-field icons-preview")
                    .append([
                            $("<div />").addClass("dx-field-label").text("Light Icons:").css("width", "20%"),
                            $("<div />").addClass("dx-field-value-static").css("width", "80%").append([
                                $("<div />").dxButton({ icon: "car", text: "Car", width: 100, type: "success" }),
                                $("<div />").dxButton({ icon: "gift", text: "Gift", width: 100, type: "success" }),
                                $("<div />").dxButton({ icon: "cart", text: "Cart", width: 100, type: "success" })
                            ])
                    ])
            ])
            ]));
    };

    window.LOAD_IOS7_TYPOGRAPHY_CONTENT = function($widgetNode) {
        $widgetNode.append(
            renderBasicTypographyContent().concat([
            $("<div />").addClass("dx-fieldset").append([
                $("<div />").addClass("dx-field")
                    .append([
                         $("<div />").addClass("dx-field-label").text("Fieldset label:"),
                         $("<div />").addClass("dx-field-value-static").text("Fieldset value")
                    ])
            ])
            ]));
    };

    window.MAKE_TREEVIEW_BORDER = function($widgetNode) {
        $widgetNode.addClass("dx-treeview-border-visible");
    };

    window.VALIDATION_ACTION = function($widgetNode, options) {
        $widgetNode.dxValidator(options.validationOptions);
        $widgetNode.dxValidator("instance").validate();
    };

    var transformClickAction = function(clickActionAsString) {
        if(typeof clickActionAsString === "function")
            return clickActionAsString;

        if(clickActionAsString && !window[clickActionAsString])
            throw new Error(clickActionAsString + " is not defined");

        return window[clickActionAsString];
    };

    var invokePrerenderAction = function(prerenderAction, $widgetNode, options) {
        if(window[prerenderAction])
            window[prerenderAction]($widgetNode, options);
    };

    var invokeAfterRenderAction = function(afterRenderAction, $widgetNode, options) {
        if(window[afterRenderAction])
            window[afterRenderAction]($widgetNode, options);
    };

    ko.bindingHandlers.preview = {
        update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
            renderCore({
                $element: $(element),
                values: valueAccessor(),
                theme: bindingContext.$root.theme(),
                contentUrl: bindingContext.$root.contentUrl
            });
        }
    };

    function getDeviceInfo(theme) {
        var deviceType = "desktop";

        switch(theme) {
            case "ios7":
                deviceType = "iPad";
                break;
            case "android5":
                deviceType = "androidTablet";
                break;
            case "generic":
                deviceType = "desktop";
                break;

            default: break;
        }

        return deviceType;
    }

    function renderCore(options) {
        if("widgets" in options.values.data)
            return renderWidgets(options);

        if("application" in options.values.data)
            return renderApplication(options);

        throw Error("Unknown type of preview item");
    }

    function renderWidgets(options) {
        var $element = options.$element,
            values = options.values,
            theme = options.theme;

        var data = values.data,
            id = data.id,
            group = data.name,
            widgets = data.widgets,
            useFieldset = data.useFieldset;

        function renderWidget($box, widget) {
            if(!$.isFunction($.fn[widget.name])) {
                throw new Error("Unable to render widget: " + widget.name);
            }

            var $widgetNode = $("<div />");

            if(useFieldset)
                $widgetNode.addClass("dx-field-value");

            if(widget.id)
                $widgetNode.attr("id", widget.id);

            $box.append($widgetNode);

            if(widget.options.click)
                widget.options.onClick = transformClickAction(widget.options.click);

            if(widget.prerenderAction)
                invokePrerenderAction(widget.prerenderAction, $widgetNode, widget.options);

            $widgetNode[widget.name](widget.options);

            if(widget.afterRenderAction)
                invokeAfterRenderAction(widget.afterRenderAction, $widgetNode, widget.options);
        }

        DX.devices.current(getDeviceInfo(theme));

        $element.addClass(id.toLowerCase().replace(/\s|\./g, "-") + "-group");

        if(useFieldset)
            $element.addClass("dx-fieldset");

        $.each(widgets, function(index, widgetOptions) {
            var $box = $("<div/>")
                .addClass("widget-box-".concat(index))
                .addClass(widgetOptions.name.toLowerCase().concat("-box"))
                .appendTo($element);

            if(widgetOptions.title) {
                if(useFieldset) {
                    $box.addClass("dx-field")
                        .append($("<div />")
                            .addClass("dx-field-label")
                            .text(widgetOptions.title)
                        );
                }
                else {
                    $("<h4/>").addClass("widget-title")
                        .text(widgetOptions.title)
                        .appendTo($box);
                }
            }

            renderWidget($box, widgetOptions);
        });
    }

    var store = new DX.data.ArrayStore({
        data: [
            {
                "id": 0,
                "name": "Russia",
                "capital": "Moscow",
                "formedAt": 862,
                "area": "17,098,242 km<sup>2<sup>",
                "population": "143,975,923",
                "currency": "Russian ruble (RUB)",
                "group": "Group A"
            },
            {
                "id": 1,
                "name": "Czech",
                "capital": "Prague",
                "formedAt": 870,
                "area": "78,866 km<sup>2<sup>",
                "population": "10,538,275",
                "currency": "Czech koruna (CZK)",
                "group": "Group A"
            },
            {
                "id": 2,
                "name": "Poland",
                "capital": "Warsaw",
                "formedAt": 870,
                "area": "312,679 km<sup>2<sup>",
                "population": "38,483,957",
                "currency": "Złoty (PLN)",
                "group": "Group A"
            },
            {
                "id": 3,
                "name": "Greece",
                "capital": "Athens",
                "formedAt": 1821,
                "area": "131,957 km<sup>2<sup>",
                "population": "10,816,286",
                "currency": "Euro (EUR)",
                "group": "Group B"
            },
            {
                "id": 4,
                "name": "Germany",
                "capital": "Berlin",
                "formedAt": 962,
                "area": "357,168 km<sup>2<sup>",
                "population": "80,716,000",
                "currency": "Euro (EUR)",
                "group": "Group B"
            },
            {
                "id": 5,
                "name": "Portugal",
                "capital": "Lisbon",
                "formedAt": 868,
                "area": "92,212 km<sup>2<sup>",
                "population": "10,427,301",
                "currency": "Euro (EUR)",
                "group": "Group B"
            },
            {
                "id": 6,
                "name": "Denmark",
                "capital": "Copenhagen",
                "formedAt": "10th century",
                "area": "42,915.7 km<sup>2<sup>",
                "population": "5,659,715",
                "currency": "Euro (EUR)",
                "group": "Group B"
            },
            {
                "id": 7,
                "name": "Netherlands",
                "capital": "Amsterdam",
                "formedAt": "26 July 1581",
                "area": "41,543 km<sup>2<sup>",
                "population": "16,912,640",
                "currency": "Euro (EUR)",
                "group": "Group B"
            }
        ],
        key: "id"
    });

    window.list = function(params) {
        return {
            dataSource: new DX.data.DataSource({
                store: store,
                group: { selector: "group", desc: false }
            }),
            selectedId: ko.observable(),
            navigateToDetails: function(id) {
                this.selectedId(id);

                dxApp.app.navigate({
                    view: 'details',
                    id: id
                }, { root: true });
            }
        };
    };

    window.details = function(params) {
        var viewModel = {
            id: ko.observable(),
            name: ko.observable(),
            area: ko.observable(),
            capital: ko.observable(),
            formedAt: ko.observable(),
            currency: ko.observable(),
            population: ko.observable()
        };

        if(!!params.id) {
            store.byKey(params.id || 1)
                .done(function(entity) {
                    viewModel.id(entity.id);
                    viewModel.name(entity.name);
                    viewModel.area(entity.area);
                    viewModel.capital(entity.capital);
                    viewModel.currency(entity.currency);
                    viewModel.formedAt(entity.formedAt);
                    viewModel.population(entity.population);
                });
        }

        return viewModel;
    };

    function renderApplication(options) {
        var q = DevExpress.createQueue();
        var $element = options.$element,
            values = options.values;

        var device,
            appOptions = values.data.application[0];

        window.dxApp = window.dxApp || {};

        device = getDeviceInfo(options.theme);
        if($.isPlainObject(appOptions.device))
            device = $.extend(device, appOptions.device);

        DX.devices.current(device);
        ko.utils.domNodeDisposal.addDisposeCallback(
            $element.get(0),
            function handleDispose() {
                DX.viewPort(document.body);
            }
        );

        dxApp.app = new DX.framework.html.HtmlApplication({
            rootNode: $element,

            layoutSet: $.map(appOptions.layoutSet, function(layoutInfo) {
                var controllerImpl = DX.data.utils.compileGetter(
                        layoutInfo.controllerExpr
                    )(window, { functionsAsIs: true });

                return {
                    platform: layoutInfo.platform,
                    controller: !layoutInfo.controllerOptions
                        ? new controllerImpl
                        : new controllerImpl(layoutInfo.controllerOptions)
                };
            }),

            navigation: appOptions.navigation
        });

        dxApp.app.router.register(":view/:id", {
            view: appOptions.startupView,
            id: undefined
        });

        $.each(appOptions.views, function(_, viewName) {
            dxApp.app.loadTemplates($($("#" + viewName + "-view").html()));
        });

        $.each(appOptions.layouts, function(_, layoutName) {
            q.add(function() {
                return $.Deferred(function(d) {

                    function handleFail() {
                        DevExpress.ui.notify("An error occurred. Please, refresh the page", "error", 4000);
                        d.reject.apply(d, arguments);
                    }

                    ThemeBuilder.LayoutsLoader.load(layoutName)
                        .done(function(layoutMarkup) {
                            dxApp.app.loadTemplates($(layoutMarkup))
                                .done(d.resolve)
                                .fail(handleFail);
                        })
                        .fail(handleFail);

                }).promise();
            });
        });

        q.add(function() {
            DevExpress.data.query($.makeArray(appOptions.startupView))
                .select(function(navTarget) {
                    dxApp.app.navigate(navTarget);
                })
                .toArray();
        });

        function contentUrl() {
            return options.contentUrl;
        }
    }
})(DevExpress);