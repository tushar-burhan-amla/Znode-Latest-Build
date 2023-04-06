$(function () {
    $('*[data-autocomplete-url]').each(function () { autocompletewrapper($(this), $(this).data("onselect-function")); });
});

function autocompletewrapper(obj, responceFun) {
    var autos = new Bloodhound({
        datumTokenizer: function (datum) {
            return Bloodhound.tokenizers.whitespace(datum.value);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            wildcard: "%QUERY",
            url: $(obj).data("autocomplete-url") + "?query=%QUERY",
            filter: function (autos) {
                // Map the remote source JSON array to a JavaScript object array
                return $.map(autos, function (auto) {
                    return {
                        value: auto.Name,
                        id: auto.Id,
                        displaytext: auto.DisplayText,
                        properties: auto.Properties
                    };
                });
            }
        },
        limit: 1000
    });

    autos.initialize();
    
    $(obj).typeahead({ highlight: true, minLength: 3, hint: true }, {
        name: 'autos', displayKey: 'value', source: autos.ttAdapter()
    }).on('typeahead:selected', function (obj, datum) {
        onselected(obj, datum, responceFun);
    });

    if ($(obj).hasClass("focus")) {
        $(obj).focus();
    }

};

function onselected(obj, datum, responceFun) {
    if (!obj || !obj.target || !datum) return;
    $('#' + jQuery(obj.target).data("autocomplete-id-field")).val(datum.id.toString());
    $(jQuery(obj.target)).val(datum.displaytext);
    executeFunctionByName(responceFun, window, datum)

}

function executeFunctionByName(functionName, context, args) {
    try {
        var args = [].slice.call(arguments).splice(2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(this, args);
    }
    catch (ex) {
        console.log(ErrorMsg.InvalidFunction + functionName);
    }
}