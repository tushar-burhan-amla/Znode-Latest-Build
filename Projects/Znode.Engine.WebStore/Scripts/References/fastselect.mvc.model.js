
//fast select wrapper binding

$(function () {
    $('*[data-url]').each(function () { fastselectwrapper($(this), $(this).data("onselect-function")); });
});

function fastselectwrapper(obj, responceFun) {    
    if ($(obj).hasClass("focus")) {
        $(obj).focus();
    }
    
    if (obj.attr("multiple") != "multiple") {        
        obj.fastselect({
            searchPlaceholder: obj.attr("placeholder"),
            onItemSelect: function ($item, itemModel) {                
                    onfastselected(obj, itemModel, responceFun);
            }
        });
        if (obj.val() !== "") {
            $(".fstToggleBtn").html('').html(obj.val());
        }
    }
}

function onfastselected(obj, datum, responceFun) {
    if (!obj || !datum) return;
    executeFunctionByName(responceFun, window, datum);
}