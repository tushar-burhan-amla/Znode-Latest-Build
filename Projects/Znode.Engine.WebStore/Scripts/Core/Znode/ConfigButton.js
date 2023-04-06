$(document).ready(function () {
  setTimeout(function () { SetConfigButton(); }, 500);
});

document.addEventListener('onZnodeDirectiveLoadAll', function () {    
    SetConfigButtonForAjaxifyWidget();
});

function enableOverlays(mappingId, widgetsKey, widgetsCode ='', isRemoveWidgetData = false) {
    var data = `{"MappingId":${mappingId},"WidgetsKey":${widgetsKey},"WidgetCode":"${widgetsCode}","IsRemoveWidgetData":"${isRemoveWidgetData}"}`;
    sendMessage(`Request:${data}`);
}
function sendMessage(msg) {
  console.log(`Web Store sending message: "${msg}"`);
  window.parent.postMessage(msg, '*');
};

function SetConfigButton() {
  var configButtons = $(".cms-configure-btn");
  for (var i = 0; i < configButtons.length; i++) {
    var widget = configButtons.eq(i).closest("z-widget");
    SetConfigButtonStyle(widget);
  }
}

function SetConfigButtonForAjaxifyWidget() {
    var ajaxifiyWidgets = $("z-widget-ajax");
    for (var m = 0; m < ajaxifiyWidgets.length; m++) {
        var widget = ajaxifiyWidgets.eq(m).find("z-widget");        
        if (widget.find(".widget-cms-overlay").length == 1) {
            var configButton = "<div class='widget-cms-overlay'></div><div class='widget-cms-button-container'><button class='btn-text btn-text-secondary btn-text-secondary-custom cms-configure-btn' type='button' onclick='enableOverlays(" + ajaxifiyWidgets.eq(m).attr('data-mappingId') + "," + ajaxifiyWidgets.eq(m).attr('data-widgetKey') + "," + ajaxifiyWidgets.eq(m).attr('data-widgetCode') + ");' style='visibility:hidden;'>CONFIGURE</button></div> ";
            widget.html(configButton + widget.html());
            SetConfigButtonStyle(widget);
        }
    }    
}

function SetConfigButtonStyle(widget) {
    var maxHeight = -1;
    var maxWidth = -1;
    widget.find('div,img,input').each(function () {
        if ($(this).height() > maxHeight)
            maxHeight = $(this).height();
        if ($(this).width() > maxWidth && $(this).width() < 2000) {
            maxWidth = $(this).width();
        }
    });
    widget.find('.widget-cms-overlay').addClass("widget-cms-overlay-custom");
    //widget.find('.widget-cms-button-container').addClass("widget-cms-button-container-center");
    widget.find('.cms-configure-btn').addClass("cms-configure-btn-custom").css("visibility", "visible");
    if (widget.find('.cms-img-icon').length > 0) {
        //widget.find('.cms-configure-btn').css("margin-top", "30px");
        var emptyIcone = widget.find('.cms-img-icon');
        if (widget.find('.widget-cms-button-container-center').find('.cms-img-icon').length == 0) {
            widget.find('.widget-cms-button-container-center').html(emptyIcone[0].outerHTML + widget.find('.widget-cms-button-container-center').html());
            emptyIcone = widget.find('.cms-img-icon');
        }
        //set max height if it is less than 140 and contain empty icon.
        if (maxHeight < 140)
            maxHeight = 140;
    }
    //widget.find('.widget-cms-overlay, .widget-cms-button-container').css("min-height", "140px");
    widget.find('.widget-cms-overlay').css({ "min-height": "40px", "width": "100%" })
    widget.find('.cms-img-icon').addClass("cms-img-icon-custom");
    widget.css({ "min-height": "140px", "margin-top": "20px" });
    widget.find('.cms-img-icon-custom').remove();
    widget.parent('.col-lg-6').addClass('align-content-top px-2').removeClass('align-content-center px-4');
    //$(".col-lg-6 z-widget .widget-cms-overlay").css("width", "97%");
    //$(".row .col-lg-6 z-widget .widget-cms-overlay").css("width", "97%");
    widget.find('.content-container .ticker-wrapper').css("padding-top", "50px");
    $('.ContentPage header').css("pointer-events", "none");
}



