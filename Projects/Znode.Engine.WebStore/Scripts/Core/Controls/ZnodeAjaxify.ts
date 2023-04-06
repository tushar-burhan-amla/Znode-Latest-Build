//Parses extended directives on the view and replaces them all with their own dynamic content.
//Extended Directives:
//<z-widget-ajax>...</z-widget-ajax>
var _znodeAjaxifyOnLoadAllSubscriptions: ((event: Event) => any)[] = [];
var _znodeAjaxifyDirectives: _ZnodeAjaxifiedDirective[] = [];
var _znodeAjaxifyDirectivesArray: _ZnodeAjaxifiedDirectives;
var _znodeAjaxifyOnLoadSubscriptions: ((event: Event) => any)[] = [];
class ZnodeAjaxify extends ZnodeBase {
	Init() {

	}

	//Applies this plug-in on per need basis and replace all the extended directives with their ajax place holders.
    Apply(onLoadAllCallback: (event: Event) => any = null, onLoadCallback: (event: Event) => any = null): any {
		_znodeAjaxifyDirectives = this._buildAjaxifiedDirectiveQueue();

        _znodeAjaxifyDirectivesArray = new _ZnodeAjaxifiedDirectives(_znodeAjaxifyDirectives);
		//Subscribe to the events to trigger supplied callbacks.
		if (onLoadCallback)
			this.OnLoad(onLoadCallback);
		if (onLoadAllCallback)
			this.OnLoadAll(onLoadAllCallback);

		_znodeAjaxifyDirectives.forEach(function (directive: _ZnodeAjaxifiedDirective, index: number, array: _ZnodeAjaxifiedDirective[]) {
            ZnodeAjaxify.prototype._renderDirective(directive, _znodeAjaxifyDirectivesArray);
		});
	}

	OnLoadAll(callback: (event: Event) => any) {
		_znodeAjaxifyOnLoadAllSubscriptions.push(callback);
	}

	OnLoad(callback: (event: Event) => any) {
		_znodeAjaxifyOnLoadSubscriptions.push(callback);
	}

    _renderDirective(directive: _ZnodeAjaxifiedDirective, directivesArray: _ZnodeAjaxifiedDirectives): any {
		let directiveType: string = directive.DirectiveType;
		switch (directiveType) {
			case 'widget':
                ZnodeAjaxify.prototype._renderWidget(directive, directivesArray);
				break;
			case 'partial':
                ZnodeAjaxify.prototype._renderPartial(directive, directivesArray);
				break;
			default:
				let identifier = directive.Identifier;
				console.error("Invalid 'type' provided for one of the '<z-**> tags having identifier '" + identifier + "'. Skipping this element.");
				break;
		}
	}

    _renderPartial(directive: _ZnodeAjaxifiedDirective, directivesArray: _ZnodeAjaxifiedDirectives) {
		let actionName = $(directive.Directive).attr('data-actionName');
		let controllerName = $(directive.Directive).attr('data-controllerName');
		let parameters = JSON.parse($(directive.Directive).attr('data-parameters'));
		let identifier = $(directive.Directive).attr('data-identifier');
		let replaceTargetSelector = $(directive.Directive).attr('data-replaceTargetSelector');
		let url = '/' + controllerName + '/' + actionName;
		if (url && url.length > 0) {
			try {
				jQuery.get(url, parameters, function (data, status, jqXHR: JQueryXHR) {
					if (jqXHR.status == 200) {
						//Success.
						$(directive.Directive).html(data);
						// target can be any Element or other EventTarget.
						ZnodeAjaxify.prototype._triggerLoadEvent(directive.Directive);
                        directive.MarkOnLoad();
                        directivesArray.MarkOnLoad();
                        if (directivesArray.IsLoaded == true)
                            ZnodeAjaxify.prototype._checkAndTriggerLoadAll();
					}
					if (replaceTargetSelector && replaceTargetSelector.length > 0) {
						$.each($(replaceTargetSelector), function (index, replaceTargetElement) {
							if (replaceTargetElement)
								$(replaceTargetElement).empty();
						});
					}
					$(directive.Directive).prev().hide();
				}, 'html');
			}
			catch { //Additional routine here to run in case of failure 
				$(directive.Directive).prev().hide();
			}
		}
		else
			console.error("'Url' can not be built for one of the '<z-**> tags having identifier '" + identifier + "'. Skipping this element.");
	}

    _renderWidget(directive: _ZnodeAjaxifiedDirective, directivesArray: _ZnodeAjaxifiedDirectives) {
		let url: string = '/dynamicContent/widget';
		if (url && url.length > 0) {
			url += '?' + this._processDataParams(directive.Directive);
			try {
                jQuery.get(url, null, function (data, status, jqXHR: JQueryXHR) {
					if (jqXHR.status == 200) {
						//Success.
						$(directive.Directive).html(data);
						ZnodeAjaxify.prototype._triggerLoadEvent(directive.Directive);
                        directive.MarkOnLoad();
                        directivesArray.MarkOnLoad();
                        if (directivesArray.IsLoaded == true)
                            ZnodeAjaxify.prototype._checkAndTriggerLoadAll();
					}
					$(directive.Directive).prev().hide();
				}, 'html');
			}
			catch { //Additional routine here to run in case of failure 
				$(directive.Directive).prev().hide();
			}
		}
	}

	_processDataParams(element: Element): string {
		let uriParts: string[] = [];
		$.each($(element).data(), function (key, value) {
			uriParts.push(key + '=' + (ZnodeAjaxify.prototype._isObject(value) === true ? encodeURIComponent(JSON.stringify(value)) : value));
		});

		return uriParts.join('&');
	}

	_buildAjaxifiedDirectiveQueue(): _ZnodeAjaxifiedDirective[] {
		let directiveArray: _ZnodeAjaxifiedDirective[] = [];
		let extendedDirectives: string[] = ["z-widget-ajax", 'z-ajax'];
		extendedDirectives.forEach(function (value: string, i: number, array: string[]) {
			$(value).each(function (index: number, element: Element) {
				directiveArray.push(new _ZnodeAjaxifiedDirective(element));
			});
		});
		return directiveArray;
	}

	_triggerLoadEvent(element: Element): void {
		let subscriptions = _znodeAjaxifyOnLoadSubscriptions;
		if (subscriptions && subscriptions.length > 0) {
			// Create the event.
			let event = document.createEvent('CustomEvent');

			// Define that the event name is 'onZnodeDirectiveLoad'.
			event.initEvent('onZnodeDirectiveLoad', true, true);

			// Listen for the event.
			element.addEventListener('onZnodeDirectiveLoad', function (e) {
				// e.target matches elem
				subscriptions.forEach(function (callback: (event: Event) => any, index: number, arr: ((event: Event) => any)[]) {
					callback(e);
				});
			}, false);

			element.dispatchEvent(event);
		}
	}

    _checkAndTriggerLoadAll() {
		let subscriptions = _znodeAjaxifyOnLoadAllSubscriptions;
		let directives = _znodeAjaxifyDirectives;		
			directives.forEach(function (directive: _ZnodeAjaxifiedDirective, index: number, arr: _ZnodeAjaxifiedDirective[]) {
				if (directive.IsLoaded != true) {
					return;
				}
				//If it reaches here, all the directives have been loaded.

				// Create the event.
				let event = document.createEvent('CustomEvent');

				// Define that the event name is 'onZnodeDirectiveLoadAll'.
				event.initEvent('onZnodeDirectiveLoadAll', true, true);

				// Listen for the event.
				document.addEventListener('onZnodeDirectiveLoadAll', function (e) {
					// e.target matches elem
					subscriptions.forEach(function (callback: (event: Event) => any, index: number, arr: ((event: Event) => any)[]) {
						callback(e);
					});
				}, false);
                document.dispatchEvent(event);
			});
		
	}

	_isObject(obj): boolean {
		return obj !== undefined && obj !== null && obj.constructor == Object;
	}
}

class _ZnodeAjaxifyEventModel {
	Event: Event = null;
	EventType: string = null;

	constructor(_event: Event, _type: string) {
		this.Event = _event;
		this.EventType = _type;
	}
}

class _ZnodeAjaxifiedDirective {
	Directive: Element = null;
	DirectiveType: string = null;
	IsLoaded: boolean = false;
	Identifier: string = null;

	constructor(_directive: Element) {
		this.Directive = _directive;
		this.DirectiveType = $(_directive).attr('data-type').toLowerCase();
		this.Identifier = $(_directive).attr('data-identifier');
	}

	MarkOnLoad() {
		this.IsLoaded = true;
	}
}

class _ZnodeAjaxifiedDirectives {
    Directives: _ZnodeAjaxifiedDirective[] = null;
    IsLoaded: boolean = false;

    constructor(_directives: _ZnodeAjaxifiedDirective[]) {
        this.Directives = _directives;
    }

    MarkOnLoad() {
        this.IsLoaded = true;
        for (var i = 0; i < this.Directives.length; i++) {
            if (this.Directives[i].IsLoaded == false) {
                this.IsLoaded = false;
                break;
            }
        }        
    }
}