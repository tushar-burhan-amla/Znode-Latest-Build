(function(ThemeBuilder){
    "use strict";

    ThemeBuilder.HistoryChangesManager = function(_sessionStorage) {
        var themeBuilderSessionStorage = {
            _storage: {},
            setItem: function(key, value) {
                this._storage[key] = value;
            },
            getItem: function(key) {
                return this._storage[key] || null;
            },
            removeItem: function(key) {
                delete this._storage[key];
            }
        };
        
        _sessionStorage = _sessionStorage || themeBuilderSessionStorage;

        var CHANGES_HISTORY_KEY = "changes-history";

        var historyItems = [];

        var updateCurrentIndex = function(index) {
            $.map(historyItems, function(item, i) {
                item.isCurrent = i === index;
            });
        };

        var updateHistory = function() {
            _sessionStorage.setItem(CHANGES_HISTORY_KEY, JSON.stringify(historyItems));
        };

        var init = function() {
            historyItems = this.getItems();
        };

        var cropHistory = function() {
            var from = this.getCurrentIndex() - 1,
                to;

            if(from < 0) {
                from = 0;
                to = 1;
            }

            historyItems = historyItems.slice(from, to);
        };

        this.getCurrentIndex = function() {
            var index = 0;
            for(var i = 0, len = historyItems.length; i < len; i++) {
                if(historyItems[i].isCurrent) {
                    index = i;
                    break;
                }
            }

            return index;
        };

        this.getItems = function() {
            return JSON.parse(_sessionStorage.getItem(CHANGES_HISTORY_KEY)) || [];
        };

        this.addItem = function(data) {
            var isNewItem = true,
                historyLength = historyItems.length;

            if(historyLength) {
                var previousItemData = JSON.stringify(historyItems[historyLength - 1].data);
                if(previousItemData === JSON.stringify(data))
                    isNewItem = false;
            }

            if(isNewItem) {

                if(historyLength && this.getCurrentIndex() !== historyItems.length - 1)
                    cropHistory.call(this);

                historyItems.push({
                    data: data,
                    isCurrent: true
                });

                updateCurrentIndex(historyItems.length - 1);
                updateHistory();
            }
        };

        this.navigateToPreviousItem = function() {
            if(!historyItems.length)
                throw new Error("History of changes is empty");

            var currentIndex = this.getCurrentIndex(),
                newIndex = currentIndex - 1;

            if(newIndex < 0)
                newIndex = 0;

            updateCurrentIndex(newIndex);
            updateHistory();
        };

        this.navigateToNextItem = function() {
            if(!historyItems.length)
                throw new Error("History of changes is empty");

            var currentIndex = this.getCurrentIndex(),
                newIndex = currentIndex + 1,
                lastIndex = historyItems.length - 1;

            if(newIndex > lastIndex)
                newIndex = lastIndex;

            updateCurrentIndex(newIndex);
            updateHistory();
        };

        this.getCurrentItem = function() {
            if(!historyItems.length)
                throw new Error("History of changes is empty");

            return historyItems[this.getCurrentIndex()];
        };

        this.clearHistory = function() {
            historyItems = [];
            _sessionStorage.removeItem(CHANGES_HISTORY_KEY);
        };
        
        init.call(this);

    };

})(ThemeBuilder);