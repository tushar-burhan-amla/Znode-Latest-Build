(function(ThemeBuilder) {
    "use strict";

    ThemeBuilder.Utils = {
        makeArrayOfDeferreds: function(count) {
            var result = [];
            for(var i = 0; i < count; i++)
                result.push($.Deferred());

            return result;
        },
        proxyEventsToFrame: function($frame) {
            if(!$frame.length)
                return;

            var frameWindow = $frame.get(0).contentWindow,
                events = {
                    "pointerout": {
                        fireEvent: "pointerup",
                        $target: $frame,
                        needToFire: function() { return true; }
                    },
                    "touchend": {
                        fireEvent: "touchend",
                        $target: $(window.document),
                        needToFire: function() { return true; }
                    },
                    "mouseout": {
                        fireEvent: "mouseup",
                        $target: $frame,
                        needToFire: function(event) { return event.which === 1; }
                    }
                };

            var eventsNameSpace = ".dxThemeBuilder";
            $.each(events, function(handleEventType, handleEvent) {
                handleEvent.$target.off(handleEventType + eventsNameSpace).on(handleEventType + eventsNameSpace, function(event) {
                    try {
                        if(frameWindow.$ && handleEvent.needToFire(event)) {
                            frameWindow.$(frameWindow.document).trigger(
                                createFrameEvent(frameWindow, $frame, event, handleEvent.fireEvent)
                            );
                        }
                    } catch(e) { }
                });
            });

            function createFrameEvent(frameWindow, $frame, sourceEvent, newEventType) {
                var event = frameWindow.$.Event(sourceEvent),
                    originalEvent = event.originalEvent;

                var propNames = frameWindow.$.event.props.slice();
                $.merge(propNames, frameWindow.$.event.mouseHooks.props);
                $.each(propNames, function() {
                    event[this] = originalEvent[this];
                });

                var frameOffsets = $frame.offset(),
                    framedX = fitCoordinateToLimits(originalEvent.clientX, frameOffsets.left, $frame.width()),
                    framedY = fitCoordinateToLimits(originalEvent.clientY, frameOffsets.top, $frame.height());
                $.each(['client', 'page', 'offset', 'screen'], function() {
                    event[this + 'X'] = framedX;
                    event[this + 'Y'] = framedY;
                });

                return frameWindow.$.extend(event, {
                    srcElement: frameWindow.document,
                    currentTarget: frameWindow.document,
                    target: frameWindow.document,
                    view: frameWindow,
                    type: newEventType,
                    changedTouches: originalEvent.changedTouches,
                    touches: originalEvent.touches,
                });
            }

            function fitCoordinateToLimits(coordinate, startValue, length) {
                if(coordinate < startValue) {
                    return 0;
                }
                var fitedValue = coordinate - startValue;
                return fitedValue > length ? length : fitedValue;
            }
        }
    };
})(ThemeBuilder);