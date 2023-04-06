(function() {
    "use strict";

    var getWidgetNameByContentType = function(type) {
        var widgetName = null;
        switch(type) {
            case "color": widgetName = "dxColorBox"; break;
            case "text": widgetName = "dxTextBox"; break;
            case "select": widgetName = "dxSelectBox"; break;
        }

        return widgetName;
    };

    function getHexValue(color) {
        return ("0" + parseInt(color, 10).toString(16)).slice(-2);
    }

    function getHexColor(red, green, blue) {
        return "#" + getHexValue(red) + getHexValue(green) + getHexValue(blue);
    }

    function rgb2hex(rgb) {
        if(!rgb) return "initial";
        var match = rgb.match(/^rgba[\s+]?\([\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?1/i);
        return (match && match.length === 4)
            ? getHexColor(match[1], match[2], match[3])
            : rgb;
    }

    var colorBoxFieldTemplate = function(value) {
        var textBox = $("<div>").dxTextBox({
            value: rgb2hex(value)
        });
        $("input", textBox).addClass("dx-texteditor-input dx-colorbox-input");
        return textBox;
    };

    ko.bindingHandlers.toolboxEditor = {
        init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
            var values = valueAccessor(),
                widgetName = getWidgetNameByContentType(values.type);
            var newValueAccessor = function() {
                switch(values.type) {
                    case "color":
                        values.editAlphaChannel = true;
                        values.fieldTemplate = colorBoxFieldTemplate;
                        break;
                    case "select":
                        values.acceptCustomValue = true;
                        values.placeholder = '';
                        values.items = (viewModel.typeValues || "").split("|");
                        break;
                }
                return values;
            }
            ko.bindingHandlers[widgetName].init.call(this, element, newValueAccessor, allBindings, viewModel, bindingContext);
        },
        update: function(element, valueAccessor, allBindings, viewModel, bindingContext) { }
    };

})();